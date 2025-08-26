namespace SkillsPortal.API.Contracts;

public enum ErrorType
{
    Validation,
    NotFound,
    Server,
    Database,
    Conflict,
}

public abstract record ServiceResult<TEntity>
{
    public sealed record Success(TEntity Entity) : ServiceResult<TEntity>;

    public sealed record Failure(string[] Errors, ErrorType ErrorType) : ServiceResult<TEntity>;

    public static ServiceResult<TEntity> FromSuccess(TEntity entity) => new Success(entity);

    public static ServiceResult<TEntity> FromFailure(string[] errors, ErrorType errorType) =>
        new Failure(errors, errorType);
}