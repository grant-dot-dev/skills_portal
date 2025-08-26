using SkillsPortal.API.Contracts;
using SkillsPortal.API.Features.Projects.Create;
using SkillsPortal.API.Features.Projects.Update;
using SkillsPortal.Core.Domain;

namespace SkillsPortal.API.Features.Projects;

public interface IProjectService
{
    // Queries
    Task<ServiceResult<ICollection<Project>>> GetAllAsync();
    Task<ServiceResult<Project>> GetByIdAsync(int id);

    // Commands
    Task<ServiceResult<Project>> CreateAsync(CreateProjectRequest project);
    Task<ServiceResult<Project>> UpdateAsync(UpdateProjectRequest project);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}