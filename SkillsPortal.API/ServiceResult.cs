namespace SkillsPortal.API;

public class ServiceResult<TEntity>
{
    public bool Success { get; init; }
    public TEntity? Entity { get; init; }
    public string[] Errors { get; init; } = [];
}