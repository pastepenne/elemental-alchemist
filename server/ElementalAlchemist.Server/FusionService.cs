using ElementalAlchemist.Server.Data;
using ElementalAlchemist.Server.Generation;
using ElementalAlchemist.Shared;
using Microsoft.EntityFrameworkCore;

namespace ElementalAlchemist.Server;

public class FusionService(AppDbContext db, IElementGenerator generator)
{
    private readonly AppDbContext _db = db;
    private readonly IElementGenerator _generator = generator;
    
    public async Task<ElementDefinition?> FuseAsync(FusionPair pair, CancellationToken ct)
    {
        if (string.Equals(pair.ElementA, pair.ElementB, StringComparison.Ordinal))
        {
            return null;
        }
        
        var existing = await _db.Fusions
            .AsNoTracking()
            .Include(f => f.Result)
            .FirstOrDefaultAsync(f => f.ElementA == pair.ElementA && f.ElementB == pair.ElementB, ct);

        if (existing is not null)
        {
            return existing.Result?.ToDefinition();
        }

        var generated = await _generator.GenerateAsync(pair.ElementA, pair.ElementB, ct);
        return await SaveGeneratedAsync(pair, generated, ct);
    }

    private async Task<ElementDefinition?> SaveGeneratedAsync(FusionPair pair, ElementDefinition? generated, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        Element? result = null;

        if (generated is not null)
        {
            result = await _db.Elements.FirstOrDefaultAsync(e => e.Id == generated.Id, ct);

            if (result is null)
            {
                result = new Element
                {
                    Id = generated.Id,
                    DisplayName = generated.DisplayName,
                    Description = generated.Description,
                    Tier = generated.Tier,
                    Tags = generated.Tags,
                    CreatedAt = now,
                };
                
                _db.Elements.Add(result);
            }
        }
        
        _db.Fusions.Add(new Fusion
        {
            ElementA = pair.ElementA,
            ElementB = pair.ElementB,
            ResultId = result?.Id,
            CreatedAt = now,
        });
        
        await _db.SaveChangesAsync(ct);
        
        return result?.ToDefinition();
    }
}
