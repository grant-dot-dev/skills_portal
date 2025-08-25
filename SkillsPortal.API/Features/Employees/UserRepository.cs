using Microsoft.EntityFrameworkCore;
using SkillsPortal.Core.Domain;
using SkillsPortal.Core.Infrastructure.DbContext;

namespace SkillsPortal.API.Features.Employees;

public class UserRepository(SkillsContext context) : IUserRepository
{
    public IQueryable<User> Query()
    {
        return context.Users.AsNoTracking();
    }

    public Task<ICollection<User>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(User project)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(User project)
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