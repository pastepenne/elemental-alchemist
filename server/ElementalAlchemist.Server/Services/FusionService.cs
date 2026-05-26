using ElementalAlchemist.Server.Database;
using ElementalAlchemist.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ElementalAlchemist.Server.Services;

/// <summary>Orchestrates a fusion: normalize inputs, check the cache, otherwise ask the AI and persist.</summary>
public class FusionService(AppDbContext db, AnthropicService ai)
{
    private const string ExoticTier = "Exotic";

    public async Task<FuseResponse> FuseAsync(string rawA, string rawB, CancellationToken ct)
    {
        var (a, b) = Normalize(rawA, rawB);

        var existing = await db.Fusions
            .FirstOrDefaultAsync(f => f.ElementA == a && f.ElementB == b, ct);

        if (existing is not null)
        {
            return existing.ResultName is null ? Impossible() : ToResponse(existing);
        }

        var result = await ai.GenerateAsync(a, b, ct);
        var row = new Fusion
        {
            ElementA = a,
            ElementB = b,
            CreatedAt = DateTime.UtcNow.ToString("O"),
            ResultName = result.Possible ? result.Name : null,
            ResultDescription = result.Possible ? result.Description : null,
            ResultTier = result.Possible ? ExoticTier : null,
            ResultTags = result.Possible ? string.Join(",", result.Tags) : null,
        };

        db.Fusions.Add(row);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // Another request generated the same pair first; return the persisted winner.
            db.Entry(row).State = EntityState.Detached;
            existing = await db.Fusions.FirstAsync(f => f.ElementA == a && f.ElementB == b, ct);
            return existing.ResultName is null ? Impossible() : ToResponse(existing);
        }

        return result.Possible ? ToResponse(row) : Impossible();
    }

    /// <summary>Trims, lowercases, and alphabetically orders the pair so lookups are order-independent.</summary>
    private static (string, string) Normalize(string x, string y)
    {
        var a = x.Trim().ToLowerInvariant();
        var b = y.Trim().ToLowerInvariant();
        return string.CompareOrdinal(a, b) <= 0 ? (a, b) : (b, a);
    }

    private static FuseResponse Impossible() => new(false, null, null, null, []);

    private static FuseResponse ToResponse(Fusion f) => new(
        true,
        f.ResultName,
        f.ResultDescription,
        f.ResultTier,
        string.IsNullOrEmpty(f.ResultTags) ? [] : f.ResultTags.Split(','));
}
