using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.Core;
using Xunit;
using SkillsPortal.API.Contracts;
using SkillsPortal.API.Features.Projects;
using SkillsPortal.API.Features.Projects.Create;
using SkillsPortal.Core.Domain;


namespace SkillsPortal.Tests.Projects.Endpoint;

public class ProjectAppFixture : AppFixture<API.Program>
{
    public IProjectService? MockProjectService { get; set; }
    public IProjectRepository MockProjectRepository { get; set; }

    protected override void ConfigureServices(IServiceCollection services)
    {
        // Create the mock
        MockProjectService = Substitute.For<IProjectService>();
        MockProjectRepository = Substitute.For<IProjectRepository>();

        // Override the IProjectService with our mock for all tests
        services.AddScoped(_ => MockProjectService);
    }
}

public class CreateEndpointTests(ProjectAppFixture app) : TestBase<ProjectAppFixture>
{
    [Fact]
    public async Task CreateProject_Should_Return_Ok_WithEntity_When_Successful()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            Name = "New Project",
            Client = "Acme Corp",
            Description = "Important initiative"
        };

        var project = new Project
        {
            Id = 123,
            Name = request.Name,
            Client = request.Client,
            Description = request.Description
        };

        app.MockProjectService?.CreateAsync(Arg.Any<CreateProjectRequest>())
            .Returns(ServiceResult<Project>.FromSuccess(project));


        // Act
        var (http, response) =
            await app.Client.POSTAsync<CreateProjectEndpoint, CreateProjectRequest, CreateProjectResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, http.StatusCode);
        Assert.NotNull(response);
        Assert.Equal(123, response.Id);
        Assert.Equal("New Project", response.Name);
        Assert.Equal("Acme Corp", response.Client);
        Assert.Equal("Important initiative", response.Description);
    }

    [Fact]
    public async Task CreateProject_Should_Return_BadRequest_When_Failed()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            Name = "Broken Project",
            Client = "Acme Corp",
            Description = "Fails validation"
        };

        var errors = new[] { "Database connection failure" };
        var projects = new List<Project>
        {
            new Project { Id = 2, Name = "Test2" }
        }.AsQueryable();

        // Mock service to return a failure result
        app.MockProjectService?
            .CreateAsync(Arg.Any<CreateProjectRequest>())
            .Returns(ServiceResult<Project>.FromFailure(errors, ErrorType.Database));

        app.MockProjectRepository.Query().Returns(projects);

        // Act
        var (http, response) =
            await app.Client.POSTAsync<CreateProjectEndpoint, CreateProjectRequest, ErrorResponse>(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, http.StatusCode);
        Assert.NotNull(response);

        var allErrors = response.Errors.SelectMany(kv => kv.Value).ToList();
        Assert.Contains("Database connection failure", allErrors);
    }
}