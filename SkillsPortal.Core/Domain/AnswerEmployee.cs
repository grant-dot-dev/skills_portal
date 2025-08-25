namespace SkillsPortal.Core.Domain;

public class User
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public int Level { get; set; }

    // A user can have many skills
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();

    public int? ProjectId { get; set; }

    public Project? Project { get; set; } // Navigation property to the one Project
}