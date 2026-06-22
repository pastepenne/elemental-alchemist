using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using ElementalAlchemist.Shared;
using Microsoft.Extensions.Options;

namespace ElementalAlchemist.Server.Generation;

public class GeneratorException(string message, Exception? inner = null) : Exception(message, inner);

public sealed class ElementGenerator(
    AnthropicClient client1,
    IOptions<GenerationOptions> options,
    ILogger<ElementGenerator> logger,
    ElementTagCatalog tagCatalog) : IElementGenerator
{
    private readonly AnthropicClient _client = client1;
    private readonly GenerationOptions _options = options.Value;
    private readonly ILogger<ElementGenerator> _logger = logger;
    private readonly IReadOnlySet<string> _tags = tagCatalog.Tags;
    private readonly string _systemPrompt = BuildSystemPrompt(tagCatalog.Tags);

    public async Task<ElementDefinition?> GenerateAsync(string a, string b, CancellationToken ct)
    {
        var rawResult = await RequestModelAsync(a, b, ct);
        var parsed = GenerationParsing.Parse(rawResult);

        if (!parsed.Possible)
        {
            return null;
        }

        if (!GenerationParsing.TryBuildDefinition(parsed, _tags, out var definition))
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
            System = [new SystemMessage(_systemPrompt)],
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

    // The allowed tag list is injected from the catalog so it stays in sync with the game's tag library.
    private static string BuildSystemPrompt(IReadOnlySet<string> tags) => $$"""
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
          {{string.Join(", ", tags)}}
        - NEVER output a tag outside that list.
        - Do NOT include a tier field; the server stamps every generated non-existing element as Exotic.
        - NO living things: no animals, plants, organisms, creatures, or body parts.
        - NO proper nouns, real people, places, brands, or fictional characters.
        - Keep it physically/alchemically themed (materials, energies, substances).
        """;
}
