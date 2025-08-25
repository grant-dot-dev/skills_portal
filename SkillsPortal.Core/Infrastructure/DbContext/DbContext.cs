using Microsoft.EntityFrameworkCore;
using SkillsPortal.Core.Domain;

namespace SkillsPortal.Core.Infrastructure.DbContext;

public class SkillsContext(DbContextOptions<SkillsContext> options) : 
    Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the many-to-many relationship between User and Skill
        modelBuilder.Entity<User>()
            .HasMany(u => u.Skills)
            .WithMany(s => s.Users);

        // Configure the one-to-many relationship between Project and User
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Users)
            .WithOne(u => u.Project);

        // Configure the many-to-many relationship between Project and Skill
        modelBuilder.Entity<Project>()
            .HasMany(p => p.RequiredSkills)
            .WithMany(s => s.Projects);
    }
}