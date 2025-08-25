namespace SkillsPortal.Core;

public interface IRelationalValidationService
{
    Task<(List<T> Entities, List<string> Errors)> ResolveEntitiesAsync<T>(
        IQueryable<T> queryable, IReadOnlyList<int> entityIds) where T : class;
}