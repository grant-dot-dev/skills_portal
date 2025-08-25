using FastEndpoints;

namespace SkillsPortal.API.Features.Projects.Delete;

public class DeleteProjectEndpoint(IProjectService projectService, ILogger<DeleteProjectEndpoint> logger)
    : Endpoint<DeleteProjectRequest, DeleteProjectResponse>
{
    public override void Configure()
    {
        Delete("/projects/{Id:int}");
        AllowAnonymous();
        DontCatchExceptions();
    }

    public override async Task HandleAsync(DeleteProjectRequest req, CancellationToken ct)
    {
        logger.LogInformation("Deleting project with ID: {ProjectId}", req.ProjectId);

        var result = await projectService.DeleteAsync(req.ProjectId);

        if (!result.Success)
        {
            foreach (var error in result.Errors)
                AddError(error);

            await Send.ErrorsAsync(500, ct);
            return;
        }

        await Send.OkAsync(new DeleteProjectResponse(true), ct);
    }
}