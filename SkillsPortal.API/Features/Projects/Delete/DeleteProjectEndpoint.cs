using FastEndpoints;
using SkillsPortal.API.Contracts;
using SkillsPortal.API.Features.Projects.Validators;

namespace SkillsPortal.API.Features.Projects.Delete;

public class DeleteProjectEndpoint(IProjectService projectService, ILogger<DeleteProjectEndpoint> logger)
    : Endpoint<DeleteProjectRequest, DeleteProjectResponse>
{
    public override void Configure()
    {
        Delete("/projects/{Id:int}");
        AllowAnonymous();
        Validator<DeleteValidator>();
    }

    public override async Task HandleAsync(DeleteProjectRequest req, CancellationToken ct)
    {
        logger.LogInformation("Deleting project with ID: {ProjectId}", req.Id);

        var result = await projectService.DeleteAsync(req.Id);

        switch (result)
        {
            case ServiceResult<bool>.Failure failedResult:
                foreach (var error in failedResult.Errors)
                {
                    AddError(error);
                }

                await Send.ErrorsAsync(400, ct);
                return;

            case ServiceResult<bool>.Success:
                await Send.OkAsync(new DeleteProjectResponse(true), ct);
                return;
        }
    }
}