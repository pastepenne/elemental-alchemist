using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;
using ElementalAlchemist.Shared;

namespace ElementalAlchemist.Server.Generation;

/// <summary>Pure parsing and validation of model output into a safe <see cref="ElementDefinition"/>.</summary>
public static class GenerationParsing
{
    private static readonly Regex IdPattern = new("^[a-z]+(-[a-z]+)*$", RegexOptions.Compiled);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>Extracts the JSON object from model output and deserializes it.</summary>
    public static GenerationResult Parse(string text)
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

    /// <summary>Validates a parsed result and builds a sanitized definition, or fails.</summary>
    public static bool TryBuildDefinition(GenerationResult result, IReadOnlySet<string> validTags, [NotNullWhen(true)] out ElementDefinition? definition)
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
            .Where(t => !string.IsNullOrWhiteSpace(t) && validTags.Contains(t))
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
}
