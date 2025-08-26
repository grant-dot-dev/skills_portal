using Microsoft.EntityFrameworkCore;
using SkillsPortal.API.Contracts;
using SkillsPortal.API.Features.Projects.Create;
using SkillsPortal.API.Features.Projects.Update;
using SkillsPortal.API.Shared;
using SkillsPortal.Core.Domain;

namespace SkillsPortal.API.Features.Projects;

public class ProjectService(
    IProjectRepository repository,
    IRelationalValidationService validationService,
    ILogger<ProjectService> logger) : IProjectService
{
    public async Task<ServiceResult<ICollection<Project>>> GetAllAsync()
    {
        try
        {
            var projects = await repository.GetAllAsync();
            return new ServiceResult<ICollection<Project>>.Success(projects);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve all projects");
            return new ServiceResult<ICollection<Project>>.Failure(
                ["Failed to retrieve projects."],
                ErrorType.Database
            );
        }
    }

    public async Task<ServiceResult<Project>> GetByIdAsync(int id)
    {
        try
        {
            var project = await repository.GetByIdAsync(id);
            if (project == null)
                return new ServiceResult<Project>.Failure(
                    [$"Project with ID {id} not found."],
                    ErrorType.NotFound
                );

            return new ServiceResult<Project>.Success(project);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve project {ProjectId}", id);
            return new ServiceResult<Project>.Failure(
                ["Failed to retrieve project."],
                ErrorType.Database
            );
        }
    }

    public async Task<ServiceResult<Project>> CreateAsync(CreateProjectRequest request)
    {
        try
        {
            var project = request.MapToEntity();
            await repository.AddAsync(project);
            await repository.SaveChangesAsync();

            return new ServiceResult<Project>.Success(project);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error while creating project {ProjectName}", request.Name);
            return new ServiceResult<Project>.Failure(
                [$"Failed to create project: {ex.Message}"],
                ErrorType.Database
            );
        }
    }

    public async Task<ServiceResult<Project>> UpdateAsync(UpdateProjectRequest request)
    {
        // Step 1: Validate the request and fetch the existing project.
        var projectResult = await GetByIdAsync(request.Id);

        if (projectResult is ServiceResult<Project>.Failure failedProjectLookup)
        {
            return failedProjectLookup;
        }

        logger.LogInformation("Updating project ID {ProjectId}", request.Id);

        // Step 2: Perform relational validation for users and skills.
        var validationResult = await validationService.ValidateRelationalProperties(
            new RelationalValidationRequest(request.UserIds, request.SkillIds));

        if (validationResult is ServiceResult<CombinedValidationResponse>.Failure failure)
        {
            return new ServiceResult<Project>.Failure(failure.Errors, failure.ErrorType);
        }

        // Step 3: All validation passed. Proceed with updating the project entity.
        var loadedProject = ((ServiceResult<Project>.Success)projectResult).Entity;
        var entities = ((ServiceResult<CombinedValidationResponse>.Success)validationResult).Entity;

        loadedProject.Name = request.Name;
        loadedProject.Client = request.Client;
        loadedProject.Description = request.Description;

        // Update the relational collections (users and skills).
        loadedProject.Users.Clear();
        foreach (var user in entities.Users)
            loadedProject.Users.Add(user);

        loadedProject.RequiredSkills.Clear();
        foreach (var skill in entities.Skills)
            loadedProject.RequiredSkills.Add(skill);

        try
        {
            await repository.SaveChangesAsync();
            return new ServiceResult<Project>.Success(loadedProject);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogWarning(ex, "Concurrency conflict while updating project {ProjectId}", request.Id);
            return new ServiceResult<Project>.Failure(
                ["Project was updated by another user. Please reload and try again."],
                ErrorType.Conflict
            );
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error while updating project {ProjectId}", request.Id);
            return new ServiceResult<Project>.Failure(
                [$"Failed to update project: {ex.Message}"],
                ErrorType.Database
            );
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var projectResult = await GetByIdAsync(id);

        if (projectResult is ServiceResult<Project>.Failure failure)
        {
            return new ServiceResult<bool>.Failure(failure.Errors, failure.ErrorType);
        }

        try
        {
            await repository.DeleteAsync(id);
            await repository.SaveChangesAsync();
            return new ServiceResult<bool>.Success(true);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error while deleting project {ProjectId}", id);
            return new ServiceResult<bool>.Failure(
                [$"Failed to delete project: {ex.Message}"],
                ErrorType.Database
            );
        }
    }
}