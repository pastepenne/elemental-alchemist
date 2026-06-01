using System.Text.Json;
using System.Text.Json.Serialization;
using ElementalAlchemist.Server.Data;
using ElementalAlchemist.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ElementalAlchemist.Server.Seeding;

public class DatabaseSeeder(AppDbContext db, IOptions<SeedingOptions> options)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };
    
    private readonly AppDbContext _db = db;
    private readonly SeedingOptions _options = options.Value;

    public async Task SeedAsync(CancellationToken ct = default)
    {
        var elements = await LoadAsync<ElementDefinition[]>(_options.ElementsPath, ct) ?? [];
        await SeedElementsAsync(elements, ct);

        var recipes = await LoadAsync<RecipeDefinition[]>(_options.RecipesPath, ct) ?? [];
        await SeedFusionsAsync(elements, recipes, ct);
    }

    private async Task SeedElementsAsync(ElementDefinition[] elements, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var existing = await _db.Elements.Select(e => e.Id).ToHashSetAsync(ct);
        
        var ordered = elements
            .OrderBy(e => e.Tier)
            .ThenBy(e => e.Id, StringComparer.Ordinal)
            .ToArray();
        
        foreach (var e in ordered)
        {
            if (!existing.Add(e.Id))
            {
                continue;
            }

            _db.Elements.Add(new Element
            {
                Id = e.Id,
                DisplayName = e.DisplayName,
                Description = e.Description,
                Tier = e.Tier,
                Tags = e.Tags,
                CreatedAt = now,
            });
        }

        await _db.SaveChangesAsync(ct);
    }

    private async Task SeedFusionsAsync(ElementDefinition[] elements, RecipeDefinition[] recipes, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var existingPairs = await _db.Fusions
            .Select(f => new FusionPair(f.ElementA, f.ElementB))
            .ToHashSetAsync(ct);

        foreach (var recipe in recipes)
        {
            var pair = new FusionPair(recipe.ElementA, recipe.ElementB);
            if (!existingPairs.Add(pair))
            {
                continue;
            }

            _db.Fusions.Add(new Fusion
            {
                ElementA = pair.ElementA,
                ElementB = pair.ElementB,
                ResultId = string.IsNullOrEmpty(recipe.Result) ? null : recipe.Result,
                CreatedAt = now,
            });
        }

        // Lock the all core fusions
        var coreIds = elements
            .Where(e => e.Tier == ElementTier.Core)
            .Select(e => e.Id)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        for (var i = 0; i < coreIds.Length; i++)
        {
            for (var j = i + 1; j < coreIds.Length; j++)
            {
                var pair = new FusionPair(coreIds[i], coreIds[j]);
                if (!existingPairs.Add(pair))
                {
                    continue;
                }

                _db.Fusions.Add(new Fusion
                {
                    ElementA = pair.ElementA,
                    ElementB = pair.ElementB,
                    ResultId = null,
                    CreatedAt = now,
                });
            }
        }

        await _db.SaveChangesAsync(ct);
    }

    private static async Task<T?> LoadAsync<T>(string path, CancellationToken ct)
    {
        if (!File.Exists(path))
        {
            return default;
        }

        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, ct);
    }
}
