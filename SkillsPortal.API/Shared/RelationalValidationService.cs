using Microsoft.EntityFrameworkCore;
using SkillsPortal.API.Contracts;
using SkillsPortal.API.Features.Employees;
using SkillsPortal.API.Features.Skills;
using SkillsPortal.Core.Domain;

namespace SkillsPortal.API.Shared;

public class RelationalValidationService(
    IUserRepository userRepository,
    ISkillsRepository skillsRepository,
    ILogger<RelationalValidationService> logger)
    : IRelationalValidationService
{
    public async Task<ServiceResult<CombinedValidationResponse>> ValidateRelationalProperties(
        RelationalValidationRequest request)
    {
        var userIds = request.UserIds?.Where(id => id > 0).ToList() ?? [];
        var skillIds = request.SkillIds?.Where(id => id > 0).ToList() ?? [];

        var userTask = userIds.Count != 0
            ? userRepository.Query().Where(u => userIds.Contains(u.Id)).ToListAsync()
            : Task.FromResult(new List<User>());

        var skillTask = skillIds.Count != 0
            ? skillsRepository.Query().Where(s => skillIds.Contains(s.Id)).ToListAsync()
            : Task.FromResult(new List<Skill>());

        try
        {
            await Task.WhenAll(userTask, skillTask);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during combined validation.");
            return ServiceResult<CombinedValidationResponse>.FromFailure(
                ["An unexpected server error occurred."],
                ErrorType.Server);
        }

        var users = userTask.Result;
        var skills = skillTask.Result;

        var allErrors = FindMissingIds<User>(userIds, users).Concat(FindMissingIds<Skill>(skillIds, skills)).ToList();

        return allErrors.Count != 0
            ? ServiceResult<CombinedValidationResponse>.FromFailure(allErrors.ToArray(), ErrorType.NotFound)
            : ServiceResult<CombinedValidationResponse>.FromSuccess(new CombinedValidationResponse(users, skills));
    }

    /// <summary>
    /// Finds the IDs from a requested list that were not found in a list of entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being checked.</typeparam>
    /// <param name="requestedIds">The list of IDs that were requested.</param>
    /// <param name="foundEntities">The list of entities that were successfully retrieved from the database.</param>
    /// <returns>A list of error strings for each ID that was not found.</returns>
    private static List<string> FindMissingIds<TEntity>(
        IReadOnlyList<int> requestedIds,
        List<TEntity> foundEntities) where TEntity : class
    {
        var existingIds = foundEntities.Select(e => EF.Property<int>(e, "Id")).ToHashSet();

        return requestedIds
            .Where(id => !existingIds.Contains(id))
            .Select(id => $"{typeof(TEntity).Name} with ID {id} was not found.")
            .ToList();
    }
}