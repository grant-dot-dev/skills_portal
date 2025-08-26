using FastEndpoints;
using Microsoft.AspNetCore.Mvc;
using SkillsPortal.API.Features.Employees;
using SkillsPortal.API.Features.Projects.Create;
using SkillsPortal.API.Features.Skills;

namespace SkillsPortal.API.Features.Projects.Update;

public record UpdateProjectRequest : CreateProjectRequest
{
    [FromRoute] public int Id { get; set; }
    public int[] UserIds { get; set; } = [];
    public int[] SkillIds { get; set; } = [];
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