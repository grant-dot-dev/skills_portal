using Microsoft.EntityFrameworkCore;
using SkillsPortal.Core.Domain;
using SkillsPortal.Core.Infrastructure.DbContext;

namespace SkillsPortal.API.Features.Skills;

public class SkillsRepository(SkillsContext context) : ISkillsRepository
{
    public IQueryable<Skill> Query()
    {
        return context.Skills.AsNoTracking();
    }

    public Task<ICollection<Skill>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Skill?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Skill project)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Skill project)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}