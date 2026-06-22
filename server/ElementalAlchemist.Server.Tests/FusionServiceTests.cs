using ElementalAlchemist.Server.Data;
using ElementalAlchemist.Server.Generation;
using ElementalAlchemist.Shared;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ElementalAlchemist.Server.Tests;

public class FusionServiceTests : IAsyncLifetime
{
    private SqliteConnection _connection = null!;
    private DbContextOptions<AppDbContext> _options = null!;

    // Initialize SQLite in-memory database
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
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task FuseAsync_IdenticalElements_ReturnsNullWithoutCallingGenerator()
    {
        // Arrange
        var generated = new ElementDefinition
        {
            Id = "steam",
            DisplayName = "Steam",
            Description = "Hot vapor.",
            Tags = ["steam"],
        };

        await using var ctx = new AppDbContext(_options);
        var generator = new TestGenerator(generated);
        var service = new FusionService(ctx, generator);

        // Act
        var result = await service.FuseAsync(new FusionPair("fire", "fire"), CancellationToken.None);

        // Assert
        Assert.Null(result);
        Assert.False(generator.Generated);
    }

    [Fact]
    public async Task FuseAsync_CachedFusion_ReturnsCachedResultWithoutCallingGenerator()
    {
        // Arrange
        var cached = new Element
        {
            Id = "steam",
            DisplayName = "Steam",
            Description = "Hot vapor.",
            Tier = ElementTier.Natural,
            Tags = ["steam"],
            CreatedAt = DateTime.UtcNow,
        };

        var pair = new FusionPair("fire", "water");
        var fusion = new Fusion
        {
            ElementA = pair.ElementA,
            ElementB = pair.ElementB,
            ResultId = cached.Id,
            CreatedAt = DateTime.UtcNow,
        };

        // Seed database with data
        await using var ctx = new AppDbContext(_options);
        ctx.Elements.Add(cached);
        ctx.Fusions.Add(fusion);
        await ctx.SaveChangesAsync();

        var generator = new TestGenerator(null);
        var service = new FusionService(new AppDbContext(_options), generator);

        // Act
        var result = await service.FuseAsync(pair, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cached.Id, result.Id);
        
        // Generator is not called because the result is cached
        Assert.False(generator.Generated);
    }

    [Fact]
    public async Task FuseAsync_NewPair_GeneratesAndPersistsResult()
    {
        // Arrange
        var generated = new ElementDefinition
        {
            Id = "steam",
            DisplayName = "Steam",
            Description = "Hot vapor.",
            Tags = ["steam"],
        };

        var pair = new FusionPair("fire", "water");
        await using var ctx = new AppDbContext(_options);
        var generator = new TestGenerator(generated);
        var service = new FusionService(ctx, generator);

        // Act
        var result = await service.FuseAsync(pair, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(generated.Id, result.Id);
        Assert.True(generator.Generated);
        Assert.Single(ctx.Elements.Where(e => e.Id == generated.Id));
        Assert.Single(ctx.Fusions, f => f is { ElementA: "fire", ElementB: "water", ResultId: "steam" });
    }

    [Fact]
    public async Task FuseAsync_ImpossiblePair_PersistsNullResultAndCaches()
    {
        // Arrange
        var pair = new FusionPair("fire", "air");
        await using var ctx = new AppDbContext(_options);
        var generator = new TestGenerator(null);
        var service = new FusionService(new AppDbContext(_options), generator);

        // Act
        var result = await service.FuseAsync(pair, CancellationToken.None);

        // Assert
        Assert.Null(result);
        Assert.True(generator.Generated);
        Assert.Single(ctx.Fusions, f => f is { ElementA: "air", ElementB: "fire", ResultId: null });
    }
}

public sealed class TestGenerator(ElementDefinition? result) : IElementGenerator
{
    public bool Generated { get; private set; }

    public Task<ElementDefinition?> GenerateAsync(string a, string b, CancellationToken ct)
    {
        Generated = true;
        return Task.FromResult(result);
    }
}
