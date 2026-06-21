using System.Text.Json;
using Microsoft.Extensions.Options;

namespace ElementalAlchemist.Server;

/// <summary>The set of valid element tags, sourced from the exported tag library.</summary>
public sealed class ElementTagCatalog(IOptions<ContentOptions> options)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HashSet<string> _tags = new(Load(options.Value.TagsPath), StringComparer.Ordinal);

    public IReadOnlySet<string> Tags => _tags;

    private static string[] Load(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            return [];
        }

        using var stream = File.OpenRead(path);
        return JsonSerializer.Deserialize<string[]>(stream, JsonOptions) ?? [];
    }
}
