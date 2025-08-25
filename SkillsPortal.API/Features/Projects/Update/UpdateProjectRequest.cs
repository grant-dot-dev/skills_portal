using SkillsPortal.API.Features.Employees;
using SkillsPortal.API.Features.Projects.Create;
using SkillsPortal.API.Features.Skills;

namespace SkillsPortal.API.Features.Projects.Update;

public record UpdateProjectRequest : CreateProjectRequest
{
    public List<int> UserIds { get; set; } = [];
    public List<int> SkillIds { get; set; } = [];
    public required int Id { get; set; }
}

public record UpdateProjectResponse(
    int Id,
    string Name,
    string Client,
    string Description,
    string UpdatedBy,
    DateTimeOffset UpdatedOn,
    List<SkillDto>? Skills,
    List<BasicUserDto>? Users);