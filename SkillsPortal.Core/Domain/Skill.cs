namespace SkillsPortal.Core.Domain;

public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    // A skill can be had by many users
    public ICollection<User> Users { get; set; } = new List<User>();

    // A skill can be required by many projects
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}