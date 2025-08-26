using FastEndpoints;
using SkillsPortal.API.Contracts;
using SkillsPortal.API.Features.Projects.Create;
using SkillsPortal.Core.Domain;

namespace SkillsPortal.API.Features.Projects.Get;

public class GetProjectsEndpoint(IProjectService projectService, ILogger<CreateProjectEndpoint> logger)
    : EndpointWithoutRequest<ProjectsResponse>
{
    public override void Configure()
    {
        Get("/projects");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        logger.LogInformation("Requesting all projects");
        var result = await projectService.GetAllAsync();

        switch (result)
        {
            case ServiceResult<ICollection<Project>>.Failure failedResult:
                foreach (var error in failedResult.Errors)
                {
                    AddError(error);
                }

                await Send.ErrorsAsync(400, ct);
                return;

            case ServiceResult<ICollection<Project>>.Success successResult:
                var response = new ProjectsResponse(successResult.Entity.ToList());
                await Send.OkAsync(response, ct);
                return;
        }
    }
}