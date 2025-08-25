using FastEndpoints;
using SkillsPortal.Core;
using SkillsPortal.Core.Domain;

namespace SkillsPortal.API.Features.Projects.Update;

public class UpdateProjectEndpoint : Endpoint<UpdateProjectRequest, UpdateProjectResponse>
{
    private readonly IProjectService _projectService;
    private readonly IRelationalValidationService _validationService;
    private readonly ILogger<UpdateProjectEndpoint> _logger;

    public UpdateProjectEndpoint(
        IProjectService projectService,
        IRelationalValidationService validationService,
        ILogger<UpdateProjectEndpoint> logger)
    {
        _projectService = projectService;
        _validationService = validationService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/projects/{Id:int}");
        AllowAnonymous();
        Validator<ProjectValidator>();
    }

    public override async Task HandleAsync(UpdateProjectRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Updating project ID {ProjectId}", req.Id);

        var result = await _projectService.UpdateAsync(req);

        if (!result.Success || result.Entity is null)
        {
            foreach (var error in result.Errors)
            {
                AddError(error);
                _logger.LogError("Project creation failed: {Error}", error);
            }

            await Send.ErrorsAsync(500, ct);
            return;
        }

        var user = new User()
        {
            Id = 1,
            Name = "Grant Test", Email = "grant@test.com", Level = 4,
            StartDate = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero),
        };

        await Send.OkAsync(result.Entity.MapToResponse(user), ct);
    }
}