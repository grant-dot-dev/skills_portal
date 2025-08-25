using Microsoft.EntityFrameworkCore;
using SkillsPortal.Core.Infrastructure.DbContext;

namespace SkillsPortal.Core;

public class RelationalValidationService(SkillsContext context) : IRelationalValidationService
{
    public async Task<(List<T> Entities, List<string> Errors)> ResolveEntitiesAsync<T>(
        IQueryable<T> queryable, IReadOnlyList<int> entityIds) where T : class
    {
        var entities = await queryable
            .Where(e => entityIds.Contains(EF.Property<int>(e, "Id")))
            .ToListAsync();

        var errors = new List<string>();
        if (entities.Count == entityIds.Count) return (entities, errors);
        {
            var existingIds = entities.Select(e => EF.Property<int>(e, "Id"));
            var invalidIds = entityIds.Except(existingIds);
            errors.Add($"One or more IDs were not found: {string.Join(", ", invalidIds)}");
        }

        return (entities, errors);
    }
}