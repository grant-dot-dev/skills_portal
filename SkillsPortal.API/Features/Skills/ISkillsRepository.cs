using SkillsPortal.Core.Domain;

namespace SkillsPortal.API.Features.Skills;

public interface ISkillsRepository
{
    IQueryable<Skill> Query();
    Task<ICollection<Skill>> GetAllAsync();
    Task<Skill?> GetByIdAsync(int id);
    Task AddAsync(Skill project);
    Task UpdateAsync(Skill project);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}