using Microsoft.EntityFrameworkCore;
using SkillsPortal.API.Features.Employees;
using SkillsPortal.API.Features.Projects.Create;
using SkillsPortal.API.Features.Projects.Update;
using SkillsPortal.API.Features.Skills;
using SkillsPortal.Core;
using SkillsPortal.Core.Domain;

namespace SkillsPortal.API.Features.Projects;

public class ProjectService(
    IProjectRepository repository,
    IRelationalValidationService validationService,
    IUserRepository userRepository,
    ISkillsRepository skillsRepository,
    ILogger logger) : IProjectService
{
    // Queries
    public async Task<ServiceResult<ICollection<Project>>> GetAllAsync()
    {
        try
        {
            var results = await repository.GetAllAsync();
            return new ServiceResult<ICollection<Project>>()
            {
                Success = true,
                Entity = results
            };
        }
        catch (Exception ex)
        {
            return new ServiceResult<ICollection<Project>>()
            {
                Success = false,
                Errors = [ex.Message],
                Entity = null,
            };
        }
    }

    public async Task<ServiceResult<Project?>> GetByIdAsync(int id)
    {
        try
        {
            var result = await repository.GetByIdAsync(id);
            return new ServiceResult<Project?>()
            {
                Success = true,
                Entity = result
            };
        }
        catch (Exception ex)
        {
            return new ServiceResult<Project?>()
            {
                Success = false,
                Errors = [ex.Message],
                Entity = null,
            };
        }
    }

    // Commands
    public async Task<ServiceResult<Project>> CreateAsync(CreateProjectRequest project)
    {
        try
        {
            var entity = project.MapToEntity();
            await repository.AddAsync(entity);
            await repository.SaveChangesAsync();

            return new ServiceResult<Project>
            {
                Entity = entity,
                Success = true,
            };
        }
        catch (DbUpdateException ex)
        {
            return new ServiceResult<Project>
            {
                Success = false,
                Errors =
                [
                    $"Failed to create project: {ex.Message}"
                ],
                Entity = null,
            };
        }
        catch (Exception ex)
        {
            return new ServiceResult<Project>
            {
                Success = false,
                Errors = [$"An unexpected error occurred: {ex.Message}"],
                Entity = null,
            };
        }
    }


    public async Task<ServiceResult<Project>> UpdateAsync(UpdateProjectRequest project)
    {
        var loadedProject = await repository.GetByIdAsync(project.Id);
        if (loadedProject == null)
            return new ServiceResult<Project> { Success = false, Errors = [$"Project with {project.Id}: Not found"] };

        // Validate users exist
        var users = userRepository.Query();
        var skills = skillsRepository.Query();

        var (validUsers, userErrors) = await validationService.ResolveEntitiesAsync(users, project.UserIds);
        var (validSkills, skillErrors) = await validationService.ResolveEntitiesAsync(skills, project.SkillIds);

        var allErrors = userErrors.Concat(skillErrors).ToList();
        if (allErrors.Count != 0)
            return new ServiceResult<Project> { Success = false, Errors = allErrors.ToArray() };

        // Update project properties
        loadedProject.Name = project.Name;
        loadedProject.Client = project.Client;
        loadedProject.Description = project.Description;

        // Update relationships
        loadedProject.Users.Clear();
        foreach (var user in validUsers)
        {
            loadedProject.Users.Add(user);
        }

        loadedProject.RequiredSkills.Clear();
        foreach (var skill in validSkills)
        {
            loadedProject.RequiredSkills.Add(skill);
        }

        try
        {
            await repository.SaveChangesAsync();
            return new ServiceResult<Project> { Success = true, Entity = loadedProject };
        }
        catch (DbUpdateException ex)
        {
            return new ServiceResult<Project> { Success = false, Errors = [ex.Message] };
        }
    }


    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var projectToDelete = await repository.GetByIdAsync(id);
        if (projectToDelete == null)
        {
            return new ServiceResult<bool> { Success = false, Errors = [$"Project with {id}: Not found"] };
        }

        try
        {
            await repository.DeleteAsync(id);
            await repository.SaveChangesAsync();
            return new ServiceResult<bool> { Success = true, Entity = true };
        }
        catch (DbUpdateException ex)
        {
            return new ServiceResult<bool> { Success = false, Errors = [ex.Message] };
        }
    }
}