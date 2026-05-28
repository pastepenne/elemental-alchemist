using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;
using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using ElementalAlchemist.Shared;
using Microsoft.Extensions.Options;

namespace ElementalAlchemist.Server.Generation;

public class GeneratorException(string message, Exception? inner = null) : Exception(message, inner);

public sealed class ElementGenerator(AnthropicClient client1, IOptions<GenerationOptions> options, ILogger<ElementGenerator> logger)
{
    private static readonly Regex IdPattern = new("^[a-z]+(-[a-z]+)*$", RegexOptions.Compiled);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    
    private readonly AnthropicClient _client = client1;
    private readonly GenerationOptions _options = options.Value;
    private readonly ILogger<ElementGenerator> _logger = logger;

    private const string SystemPrompt = """
        You are the fusion engine for an alchemy game. Given two element names, you invent a single new
        element that could plausibly result from combining them.

        RULES:
        - Output ONLY a JSON object. No prose, no markdown, no code fences.
        - If the two elements cannot sensibly combine, output exactly: {"possible": false}
        - Otherwise output: {"possible": true, "id": "...", "name": "...", "description": "...", "tags": ["...","..."]}
        - "id": kebab-case slug derived from the name, lowercase letters and hyphens only (e.g. "molten-glass").
          Must match the regex ^[a-z]+(-[a-z]+)*$. No digits, no underscores, no spaces, no leading/trailing hyphen.
        - "name": 1-3 words, a substance/material/phenomenon, Title Case.
        - "description": one short sentence of flavor text (max ~200 chars).
        - "tags": 1 to 2 strings, chosen ONLY from this exact list:
          fire, water, air, earth, wood, stone, metal, ice, lightning, shadow, light, poison
        - NEVER output a tag outside that list.
        - Do NOT include a tier field; the server stamps every generated non-existing element as Exotic.
        - NO living things: no animals, plants, organisms, creatures, or body parts.
        - NO proper nouns, real people, places, brands, or fictional characters.
        - Keep it physically/alchemically themed (materials, energies, substances).
        """;

    public async Task<ElementDefinition?> GenerateAsync(string a, string b, CancellationToken ct)
    {
        var rawResult = await RequestModelAsync(a, b, ct);
        var parsed = ParseResult(rawResult);

        if (!parsed.Possible)
        {
            return null;
        }

        if (!TryBuildDefinition(parsed, out var definition))
        {
            _logger.LogWarning("AI returned a malformed element for ({A}, {B}); raw output: {Raw}", a, b, rawResult);
            return null;
        }

        return definition;
    }

    private async Task<string> RequestModelAsync(string a, string b, CancellationToken ct)
    {
        var parameters = new MessageParameters
        {
            Model = _options.Model,
            MaxTokens = _options.MaxTokens,
            System = [new SystemMessage(SystemPrompt)],
            Messages =
            [
                new Message(RoleType.User, $"Combine these two elements: \"{a}\" and \"{b}\". Return the JSON object now."),
            ],
        };

        try
        {
            var response = await _client.Messages.GetClaudeMessageAsync(parameters, ct);
            return response.Message.ToString();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new GeneratorException("Anthropic API call failed.", ex);
        }
    }

    private static GenerationResult ParseResult(string text)
    {
        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');
        if (start < 0 || end <= start)
        {
            throw new GeneratorException("No JSON object found in model output.");
        }

        try
        {
            return JsonSerializer.Deserialize<GenerationResult>(text[start..(end + 1)], JsonOptions)
                ?? GenerationResult.Impossible();
        }
        catch (JsonException ex)
        {
            throw new GeneratorException("Model output was not valid JSON.", ex);
        }
    }

    private static bool TryBuildDefinition(GenerationResult result, [NotNullWhen(true)] out ElementDefinition? definition)
    {
        if (string.IsNullOrWhiteSpace(result.Id) || !IdPattern.IsMatch(result.Id))
        {
            definition = null;
            return false;
        }

        if (string.IsNullOrWhiteSpace(result.Name) || string.IsNullOrWhiteSpace(result.Description))
        {
            definition = null;
            return false;
        }

        var tags = (result.Tags ?? [])
            .Where(t => !string.IsNullOrWhiteSpace(t) && ElementTags.All.Contains(t))
            .Distinct(StringComparer.Ordinal)
            .Take(2)
            .ToArray();

        if (tags.Length == 0)
        {
            definition = null;
            return false;
        }

        definition = new ElementDefinition
        {
            Id = result.Id,
            DisplayName = result.Name,
            Description = result.Description,
            Tier = ElementTier.Exotic,
            Tags = tags,
        };
        
        return true;
    }

    private sealed record GenerationResult
    {
        public bool Possible { get; init; }
        public string? Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public string[]? Tags { get; init; }

        public static GenerationResult Impossible() => new() { Possible = false };
    }
}
