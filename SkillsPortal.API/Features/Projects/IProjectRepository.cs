using SkillsPortal.Core.Domain;

namespace SkillsPortal.API.Features.Projects;

public interface IProjectRepository
{
    IQueryable<Project> Query();
    Task<ICollection<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}