using FastEndpoints;
using SkillsPortal.API.Contracts;
using SkillsPortal.API.Features.Projects.Validators;
using SkillsPortal.Core.Domain;

namespace SkillsPortal.API.Features.Projects.Create;

public class CreateProjectEndpoint(IProjectService projectService, ILogger<CreateProjectEndpoint> logger)
    : Endpoint<CreateProjectRequest, CreateProjectResponse>
{
    public override void Configure()
    {
        Post("/projects");
        AllowAnonymous();
        Validator<CreateProjectValidator>();
    }

    public override async Task HandleAsync(CreateProjectRequest req, CancellationToken ct)
    {
        logger.LogInformation("Received request to create a new project: {RequestName}", req.Name);

        var result = await projectService.CreateAsync(req);

        switch (result)
        {
            case ServiceResult<Project>.Failure failedResult:
                foreach (var error in failedResult.Errors)
                {
                    AddError(error);
                }

                await Send.ErrorsAsync(400, ct);
                return;

            case ServiceResult<Project>.Success successResult:
                var response = successResult.Entity.MapToCreateResponse(FakeUsers.Admin);
                logger.LogInformation("Successfully created project with ID: {ProjectId}", successResult.Entity.Id);
                await Send.OkAsync(response, ct);
                return;
        }
    }
}