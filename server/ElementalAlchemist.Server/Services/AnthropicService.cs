using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace ElementalAlchemist.Server.Services;

/// <summary>Parsed outcome of an AI fusion attempt.</summary>
public record AiResult(bool Possible, string? Name, string? Description, string[] Tags);

/// <summary>Raised when the Anthropic API is unreachable or returns a non-success status.</summary>
public class AnthropicException(string message) : Exception(message);

/// <summary>Calls the Anthropic Claude Haiku API to invent an Exotic element from two inputs.</summary>
public class AnthropicService(HttpClient http, IOptions<AnthropicOptions> options, ILogger<AnthropicService> log)
{
    private const int MaxTokens = 400;

    private readonly string _model = options.Value.Model;

    /// <summary>Sprite tags the AI may use. Mirrors the prompt; also enforced server-side.</summary>
    private static readonly HashSet<string> AllowedTags = new(StringComparer.Ordinal)
    {
        "fire", "water", "air", "earth", "wood", "stone",
        "metal", "ice", "lightning", "shadow", "light", "poison",
    };

    private const string SystemPrompt = """
        You are the fusion engine for an alchemy game. Given two element names, you invent a single new
        "Exotic"-tier element that could plausibly result from combining them.

        RULES:
        - Output ONLY a JSON object. No prose, no markdown, no code fences.
        - If the two elements cannot sensibly combine, output exactly: {"possible": false}
        - Otherwise output: {"possible": true, "name": "...", "description": "...", "tags": ["...","..."]}
        - "name": 1-3 words, a substance/material/phenomenon, Title Case.
        - "description": one short sentence of flavor text (max ~140 chars).
        - "tags": 1 to 2 strings, chosen ONLY from this exact list:
          fire, water, air, earth, wood, stone, metal, ice, lightning, shadow, light, poison
        - NEVER output a tag outside that list.
        - The result is ALWAYS tier "Exotic" — do not include a tier field.
        - NO living things: no animals, plants, organisms, creatures, or body parts.
        - NO proper nouns, real people, places, brands, or fictional characters.
        - Keep it physically/alchemically themed (materials, energies, substances).
        """;

    /// <summary>Asks the model to fuse two (already-normalized) element names.</summary>
    public async Task<AiResult> GenerateAsync(string a, string b, CancellationToken ct)
    {
        var payload = new
        {
            model = _model,
            max_tokens = MaxTokens,
            system = SystemPrompt,
            messages = new[]
            {
                new { role = "user", content = $"Combine these two elements: \"{a}\" and \"{b}\". Return the JSON object now." },
            },
        };

        using var resp = await http.PostAsJsonAsync("/v1/messages", payload, ct);
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync(ct);
            log.LogError("Anthropic returned {Status}: {Body}", (int)resp.StatusCode, body);
            throw new AnthropicException($"Anthropic returned {(int)resp.StatusCode}");
        }

        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync(ct));
        var text = doc.RootElement.GetProperty("content")[0].GetProperty("text").GetString() ?? "";
        return ParseModelJson(text);
    }

    /// <summary>Extracts and validates the JSON object the model emitted.</summary>
    private static AiResult ParseModelJson(string text)
    {
        // Tolerate prose or code fences around the JSON by isolating the first {...} block.
        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');
        if (start < 0 || end <= start)
        {
            throw new JsonException("No JSON object found in model output.");
        }

        using var doc = JsonDocument.Parse(text[start..(end + 1)]);
        var root = doc.RootElement;

        if (!root.TryGetProperty("possible", out var possible) || possible.ValueKind == JsonValueKind.False)
        {
            return new AiResult(false, null, null, []);
        }

        var name = root.TryGetProperty("name", out var n) ? n.GetString() : null;
        var description = root.TryGetProperty("description", out var d) ? d.GetString() : null;
        if (string.IsNullOrWhiteSpace(name))
        {
            return new AiResult(false, null, null, []);
        }

        var tags = root.TryGetProperty("tags", out var t) && t.ValueKind == JsonValueKind.Array
            ? t.EnumerateArray()
                .Select(x => x.GetString()?.Trim().ToLowerInvariant())
                .Where(x => x is not null && AllowedTags.Contains(x))
                .Select(x => x!)
                .Distinct()
                .Take(2)
                .ToArray()
            : [];

        return new AiResult(true, name, description, tags);
    }
}
