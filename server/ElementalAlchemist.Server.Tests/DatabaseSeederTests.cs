using System.Text.Json;
using System.Text.Json.Serialization;
using ElementalAlchemist.Server.Data;
using ElementalAlchemist.Server.Seeding;
using ElementalAlchemist.Shared;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace ElementalAlchemist.Server.Tests;

public class DatabaseSeederTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private SqliteConnection _connection = null!;
    private DbContextOptions<AppDbContext> _options = null!;
    private readonly List<string> _tempFiles = [];

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        await using var ctx = new AppDbContext(_options);
        await ctx.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        foreach (var file in _tempFiles)
        {
            File.Delete(file);
        }

        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task SeedAsync_RunTwice_DoesNotDuplicateRows()
    {
        // Arrange
        var content = WriteContent(
            elements: [Element("fire", ElementTier.Core), Element("water", ElementTier.Core), Element("steam", ElementTier.Natural)],
            recipes: [Recipe("fire", "water", "steam")]);

        // Act
        await new DatabaseSeeder(new AppDbContext(_options), content).SeedAsync();
        await new DatabaseSeeder(new AppDbContext(_options), content).SeedAsync();

        // Assert: the second run adds nothing.
        await using var verify = new AppDbContext(_options);
        Assert.Equal(3, await verify.Elements.CountAsync());
        Assert.Equal(1, await verify.Fusions.CountAsync());
    }

    [Fact]
    public async Task SeedAsync_LocksEveryCorePair_WithNullResult()
    {
        // Arrange
        var content = WriteContent(
            elements: [Element("air", ElementTier.Core), Element("earth", ElementTier.Core), Element("fire", ElementTier.Core)],
            recipes: []);

        // Act
        await new DatabaseSeeder(new AppDbContext(_options), content).SeedAsync();

        // Assert
        await using var verify = new AppDbContext(_options);
        var fusions = await verify.Fusions.ToListAsync();
        Assert.Equal(3, fusions.Count);
        Assert.Contains(fusions, f => f is { ElementA: "air", ElementB: "earth", ResultId: null });
        Assert.Contains(fusions, f => f is { ElementA: "air", ElementB: "fire", ResultId: null });
        Assert.Contains(fusions, f => f is { ElementA: "earth", ElementB: "fire", ResultId: null });
    }

    [Fact]
    public async Task SeedAsync_NormalizesRecipePairOrder()
    {
        // Arrange
        var content = WriteContent(
            elements: [Element("fire", ElementTier.Natural), Element("water", ElementTier.Natural), Element("steam", ElementTier.Natural)],
            recipes: [Recipe("water", "fire", "steam")]);

        // Act
        await new DatabaseSeeder(new AppDbContext(_options), content).SeedAsync();

        // Assert
        await using var verify = new AppDbContext(_options);
        var fusion = await verify.Fusions.SingleAsync();
        Assert.Equal("fire", fusion.ElementA);
        Assert.Equal("water", fusion.ElementB);
        Assert.Equal("steam", fusion.ResultId);
    }

    private IOptions<ContentOptions> WriteContent(ElementDefinition[] elements, RecipeDefinition[] recipes)
    {
        var elementsPath = NewTempFile();
        var recipesPath = NewTempFile();
        File.WriteAllText(elementsPath, JsonSerializer.Serialize(elements, JsonOptions));
        File.WriteAllText(recipesPath, JsonSerializer.Serialize(recipes, JsonOptions));

        return Options.Create(new ContentOptions { ElementsPath = elementsPath, RecipesPath = recipesPath });
    }

    private string NewTempFile()
    {
        var path = Path.GetTempFileName();
        _tempFiles.Add(path);
        return path;
    }

    private static ElementDefinition Element(string id, ElementTier tier) => new()
    {
        Id = id,
        DisplayName = id,
        Description = $"{id} description.",
        Tier = tier,
        Tags = [id],
    };

    private static RecipeDefinition Recipe(string a, string b, string result) => new()
    {
        ElementA = a,
        ElementB = b,
        Result = result,
    };
}
