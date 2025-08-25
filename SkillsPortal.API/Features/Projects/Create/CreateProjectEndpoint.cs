using FastEndpoints;

namespace SkillsPortal.API.Features.Projects.Create;

public class CreateProjectEndpoint(IProjectService projectService, ILogger<CreateProjectEndpoint> logger)
    : Endpoint<CreateProjectRequest, ProjectResponse>
{
    public override void Configure()
    {
        Post("/projects");
        AllowAnonymous();
        DontCatchExceptions();
        Validator<ProjectValidator>();
    }

    public override async Task HandleAsync(CreateProjectRequest req, CancellationToken ct)
    {
        logger.LogInformation("Received request to create a new project: {RequestName}", req.Name);

        try
        {
            var project = req.MapToEntity();
            var result = await projectService.CreateAsync(project);

            if (!result.Success)
            {
                foreach (var error in result.Errors)
                    AddError(error);

                await Send.ErrorsAsync(500, ct);
                return; // <-- early return
            }

            if (result.Entity is { } entity)
            {
                var response = entity.MapToResponse();
                logger.LogInformation("Successfully created project with ID: {ProjectId}", entity.Id);
                await Send.OkAsync(response, ct);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating project: {RequestName}", req.Name);
            AddError(ex.Message);
            await Send.ErrorsAsync(400, ct);
        }
    }
}