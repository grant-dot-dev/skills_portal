// Interface for the repository

using Microsoft.EntityFrameworkCore;
using SkillsPortal.Core.Domain;
using SkillsPortal.Core.Infrastructure.DbContext;

namespace SkillsPortal.API.Features.Projects;



public class ProjectRepository(SkillsContext context) : IProjectRepository
{
    public async Task<ICollection<Project>> GetAllAsync()
    {
        return await context.Projects.ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await context.Projects.FindAsync(id);
    }

    public async Task AddAsync(Project project)
    {
        await context.Projects.AddAsync(project);
    }

    public Task UpdateAsync(Project project)
    {
        context.Projects.Update(project);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var project = await context.Projects.FindAsync(id);
        if (project != null)
        {
            context.Projects.Remove(project);
        }
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}