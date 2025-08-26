using SkillsPortal.API.Features.Employees;
using SkillsPortal.Core.Domain;

namespace SkillsPortal.API.Features.Projects.Create;

public record CreateProjectResponse
{
    public int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required string Client { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public required DateTimeOffset CreatedOn { get; set; }
    public required BasicUserDto CreatedBy { get; set; }

    public List<User> Users { get; set; } = [];
    public List<Skill> RequiredSkillIds { get; set; } = [];
}