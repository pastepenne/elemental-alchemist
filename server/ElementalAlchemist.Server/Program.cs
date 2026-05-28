using System.Text.Json;
using Anthropic.SDK;
using ElementalAlchemist.Server;
using ElementalAlchemist.Server.Data;
using ElementalAlchemist.Server.Generation;
using ElementalAlchemist.Server.Seeding;
using ElementalAlchemist.Shared;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// Seeding
builder.Services.Configure<SeedingOptions>(builder.Configuration.GetSection("Seeding"));
builder.Services.AddScoped<DatabaseSeeder>();

// Generation
builder.Services.Configure<GenerationOptions>(builder.Configuration.GetSection("Generation"));
builder.Services.AddSingleton(_ =>
{
    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")))
    {
        throw new InvalidOperationException("Anthropic API key not configured. Set the ANTHROPIC_API_KEY environment variable.");
    }
    
    return new AnthropicClient();
});

builder.Services.AddScoped<ElementGenerator>();

// Fusion
builder.Services.AddScoped<FusionService>();

var app = builder.Build();

// Ensure database is created and seeded on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
    
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

// Endpoint for fusing two elements
app.MapPost("/fuse", async (FusionPair request, FusionService fusion, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.ElementA) || string.IsNullOrWhiteSpace(request.ElementB))
    {
        return Results.BadRequest(new { error = "Both elementA and elementB are required." });
    }

    try
    {
        var result = await fusion.FuseAsync(request, ct);
        return result is null ? Results.NoContent() : Results.Ok(result);
    }
    catch (GeneratorException)
    {
        return Results.StatusCode(StatusCodes.Status502BadGateway);
    }
});

app.Run();
