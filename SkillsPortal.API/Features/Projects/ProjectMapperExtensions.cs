using SkillsPortal.API.Contracts;
using SkillsPortal.API.Features.Employees;
using SkillsPortal.API.Features.Projects.Create;
using SkillsPortal.API.Features.Projects.Update;
using SkillsPortal.API.Features.Skills;
using SkillsPortal.Core.Domain;

namespace SkillsPortal.API.Features.Projects;

public static class ProjectMapperExtensions
{
    public static Project MapToEntity(this CreateProjectRequest project)
    {
        return new Project()
        {
            Name = project.Name,
            Client = project.Client,
            Description = project.Description,
        };
    }

    public static CreateProjectResponse MapToCreateResponse(this Project project, User user)
    {
        var response = new CreateProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Client = project.Client,
            Description = project.Description ?? string.Empty,
            CreatedBy = new BasicUserDto(user.Id, user.Name, user.StartDate.ToString("yyyy-MM-dd"),
                user.Level.ToString()),
            CreatedOn = DateTimeOffset.UtcNow
        };
        return response;
    }


    public static UpdateProjectResponse MapToUpdateResponse(this Project project, User user)
    {
        var response = new UpdateProjectResponse(
            Id: project.Id,
            Name: project.Name,
            Client: project.Client,
            Description: project.Description ?? string.Empty,
            UpdatedBy: user.Name,
            UpdatedOn: DateTimeOffset.UtcNow,
            Skills: project.RequiredSkills
                .Select(s => new SkillDto(s.Id, s.Name))
                .ToList(),
            Users: project.Users
                .Select(u => new BasicUserDto(u.Id, u.Name, u.StartDate.ToString("yyyy-MM-dd"), u.Level.ToString()))
                .ToList()
        );
        return response;
    }

    public static int MapErrorToStatusCode(this ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => 400,
            ErrorType.NotFound => 404,
            ErrorType.Conflict => 500,
            ErrorType.Database => 500,
            ErrorType.Server => 500,
            _ => 500
        };
    }
}