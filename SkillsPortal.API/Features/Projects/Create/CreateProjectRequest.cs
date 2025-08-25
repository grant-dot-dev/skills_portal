namespace SkillsPortal.API.Features.Projects.Create;

public record CreateProjectRequest
{
    public required string Name { get; set; } = string.Empty;
    public required string Client { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
}