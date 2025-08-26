namespace SkillsPortal.API.Features.Projects.Create;

public record CreateProjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string Client { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}