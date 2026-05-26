using System.Text.Json;
using ElementalAlchemist.Server.Database;
using ElementalAlchemist.Server.Models;
using ElementalAlchemist.Server.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=fusions.db"));

builder.Services.AddScoped<FusionService>();

builder.Services.Configure<AnthropicOptions>(builder.Configuration.GetSection("Anthropic"));
builder.Services.PostConfigure<AnthropicOptions>(o =>
{
    // Convenience fallback to the conventional env var when not set under Anthropic:ApiKey.
    if (string.IsNullOrEmpty(o.ApiKey))
    {
        o.ApiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ?? "";
    }
});

builder.Services.AddHttpClient<AnthropicService>((sp, client) =>
{
    var o = sp.GetRequiredService<IOptions<AnthropicOptions>>().Value;
    if (string.IsNullOrEmpty(o.ApiKey))
    {
        throw new InvalidOperationException(
            "Anthropic API key not configured. Set ANTHROPIC_API_KEY or Anthropic:ApiKey.");
    }

    client.BaseAddress = new Uri("https://api.anthropic.com");
    client.DefaultRequestHeaders.Add("x-api-key", o.ApiKey);
    client.DefaultRequestHeaders.Add("anthropic-version", o.Version);
});

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreatedAsync();
}

app.MapPost("/fuse", async (FuseRequest req, FusionService fusion, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(req.ElementA) || string.IsNullOrWhiteSpace(req.ElementB))
    {
        return Results.BadRequest(new { error = "Both elementA and elementB are required." });
    }

    try
    {
        return Results.Ok(await fusion.FuseAsync(req.ElementA, req.ElementB, ct));
    }
    catch (Exception ex) when (ex is AnthropicException or JsonException)
    {
        return Results.StatusCode(StatusCodes.Status502BadGateway);
    }
});

app.Run();
