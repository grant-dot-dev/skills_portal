using FastEndpoints;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillsPortal.API.Contracts;
using SkillsPortal.API.Features.Projects.Validators;
using SkillsPortal.API.Shared;
using SkillsPortal.Core.Domain;

namespace SkillsPortal.API.Features.Projects.Update;

public class UpdateProjectEndpoint(
    IProjectService projectService,
    ILogger<UpdateProjectEndpoint> logger)
    : Endpoint<UpdateProjectRequest, UpdateProjectResponse>
{
    public override void Configure()
    {
        Put("/projects/{Id:int}");
        AllowAnonymous();
        Validator<CreateProjectValidator>();
        Summary(s => s.Summary = "Update a project by Id from the URL, do not include Id in the body");
    }

    public override async Task HandleAsync(UpdateProjectRequest req, CancellationToken ct)
    {
        var projectUpdateResult = await projectService.UpdateAsync(req);

        switch (projectUpdateResult)
        {
            case ServiceResult<Project>.Failure failure:
            {
                foreach (var error in failure.Errors)
                {
                    AddError(error);
                }

                logger.LogError("Project update failed: {Errors}", string.Join(", ", failure.Errors));
                await Send.ErrorsAsync(failure.ErrorType.MapErrorToStatusCode(), ct);
                return;
            }
            case ServiceResult<Project>.Success success:
                await Send.OkAsync(success.Entity.MapToUpdateResponse(FakeUsers.Admin), ct);
                return;
        }
    }
}