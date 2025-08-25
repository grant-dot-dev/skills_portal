using SkillsPortal.Core.Domain;

namespace SkillsPortal.API.Features.Employees;

public interface IUserRepository
{
    IQueryable<User> Query();
    Task<ICollection<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task AddAsync(User project);
    Task UpdateAsync(User project);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}