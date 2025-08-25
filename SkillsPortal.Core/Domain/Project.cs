namespace SkillsPortal.Core.Domain;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; } = "";
    public string Client { get; set; }

    // A project requires many skills
    public ICollection<Skill> RequiredSkills { get; set; } = new List<Skill>();

    // A project can have many users
    public ICollection<User> Users { get; set; } = new List<User>();
}