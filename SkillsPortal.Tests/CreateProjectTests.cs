using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Testing;
using NSubstitute;
using SkillsPortal.Core;
using SkillsPortal.Core.Domain;
using SkillsPortal.Core.Infrastructure.DbContext;
using SkillsPortal.API.Features.Projects;

public class CreateProjectTests
{
    private SkillsContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<SkillsContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SkillsContext(options);
    }

    [Fact]
    public async Task ExecuteAsync_CreatesProject_WhenAllValid()
    {
        var context = GetInMemoryContext();
        var logger = new FakeLogger<CreateProject>();
        var validation = Substitute.For<IRelationalValidationService>();

        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Name = "bob",
                Email = "test-mail@mail.com"
            }
        };
        var skills = new List<Skill> { new() { Id = 10 } };

        context.Users.Attach(users[0]);
        context.Skills.Attach(skills[0]);

        validation.ResolveEntitiesAsync<User>(Arg.Any<DbSet<User>>(), Arg.Any<IReadOnlyList<int>>())
            .Returns(Task.FromResult((users, new List<string>())));

        validation.ResolveEntitiesAsync<Skill>(Arg.Any<DbSet<Skill>>(), Arg.Any<IReadOnlyList<int>>())
            .Returns(Task.FromResult((skills, new List<string>())));

        var sut = new CreateProject(context, validation, logger);

        var dto = new ProjectRequestDto
        {
            Name = "TestProject",
            Client = "ClientA",
            Description = "Desc",
            UserIds = new List<int>() { 1 },
            RequiredSkillIds = new List<int>() { 10 }
        };

        var result = await sut.ExecuteAsync(dto);

        Assert.NotNull(result);
        var project = await context.Projects.Include(p => p.Users).Include(p => p.RequiredSkills).FirstAsync();
        Assert.Equal("TestProject", project.Name);
        Assert.Single(project.Users);
        Assert.Single(project.RequiredSkills);
        Assert.Contains(logger.Collector.GetSnapshot(), m => m.Message.Contains("Creating a new project"));
        Assert.Contains(logger.Collector.GetSnapshot(), m => m.Message.Contains("Project created successfully"));
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNull_WhenUserValidationFails()
    {
        var context = GetInMemoryContext();
        var logger = new FakeLogger<CreateProject>();
        var validation = Substitute.For<IRelationalValidationService>(context);

        validation.ResolveEntitiesAsync(Arg.Any<DbSet<User>>(), Arg.Any<IReadOnlyList<int>>())
            .Returns(Task.FromResult((new List<User>(), new List<string> { "User ID invalid" })));
        validation.ResolveEntitiesAsync(Arg.Any<DbSet<Skill>>(), Arg.Any<IReadOnlyList<int>>())
            .Returns(Task.FromResult((new List<Skill>(), new List<string>())));

        var sut = new CreateProject(context, validation, logger);

        var dto = new ProjectRequestDto
        {
            Name = "FailProject",
            Client = "ClientB",
            Description = "Desc",
            UserIds = [1],
            RequiredSkillIds = [0]
        };

        var result = await sut.ExecuteAsync(dto);

        Assert.Null(result);
        Assert.Contains(logger.Collector.GetSnapshot(),
            m => m.Message.Contains("Failed to create project due to validation errors"));
        Assert.Empty(context.Projects);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNull_WhenSkillValidationFails()
    {
        var context = GetInMemoryContext();
        var logger = new FakeLogger<CreateProject>();
        var validation = Substitute.For<IRelationalValidationService>();

        validation.ResolveEntitiesAsync(Arg.Any<DbSet<User>>(), Arg.Any<IReadOnlyList<int>>())
            .Returns(Task.FromResult((new List<User> { new() { Id = 1, Name = "Test", Email = "emaill" } },
                new List<string>())));
        validation.ResolveEntitiesAsync(Arg.Any<DbSet<Skill>>(), Arg.Any<IReadOnlyList<int>>())
            .Returns(Task.FromResult((new List<Skill>(), new List<string> { "Skill ID invalid" })));

        var sut = new CreateProject(context, validation, logger);

        var dto = new ProjectRequestDto
        {
            Name = "FailProject",
            Client = "ClientB",
            Description = "Desc",
            UserIds = [1],
            RequiredSkillIds = [0]
        };

        var result = await sut.ExecuteAsync(dto);

        Assert.Null(result);
        Assert.Contains(logger.Collector.GetSnapshot(),
            m => m.Message.Contains("Failed to create project due to validation errors"));
        Assert.Empty(context.Projects);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNull_WhenDbUpdateExceptionThrown()
    {
        var context = GetInMemoryContext();
        var logger = new FakeLogger<CreateProject>();
        var validation = Substitute.For<IRelationalValidationService>();

        var users = new List<User> { new() { Id = 1, Name = "Test", Email = "emaill" } };
        var skills = new List<Skill> { new() { Id = 10, Name = "C#" } };

        validation.ResolveEntitiesAsync<User>(Arg.Any<DbSet<User>>(), Arg.Any<IReadOnlyList<int>>())
            .Returns(Task.FromResult((users, new List<string>())));

        validation.ResolveEntitiesAsync<Skill>(Arg.Any<DbSet<Skill>>(), Arg.Any<IReadOnlyList<int>>())
            .Returns(Task.FromResult((skills, new List<string>())));

        var sut = new CreateProject(context, validation, logger);

        var dto = new ProjectRequestDto
        {
            Name = "DBFailProject",
            Client = "ClientC",
            Description = "Desc",
            UserIds = [1],
            RequiredSkillIds = [10]
        };

        // Simulate DbUpdateException by disposing the context before save
        context.Dispose();

        var result = await sut.ExecuteAsync(dto);

        Assert.Null(result);
        Assert.Contains(logger.Collector.GetSnapshot(),
            m => m.Message.Contains("Failed to create project due to a database update exception"));
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNull_WhenNoUsersOrSkillsProvided()
    {
        var context = GetInMemoryContext();
        var logger = new FakeLogger<CreateProject>();
        var validation = Substitute.For<IRelationalValidationService>();

        // Simulate empty lists with no errors
        validation.ResolveEntitiesAsync(Arg.Any<DbSet<User>>(), Arg.Any<IReadOnlyList<int>>())
            .Returns(Task.FromResult((new List<User>(), new List<string>())));
        validation.ResolveEntitiesAsync(Arg.Any<DbSet<Skill>>(), Arg.Any<IReadOnlyList<int>>())
            .Returns(Task.FromResult((new List<Skill>(), new List<string>())));

        var sut = new CreateProject(context, validation, logger);

        var dto = new ProjectRequestDto
        {
            Name = "EmptyProject",
            Client = "ClientD",
            Description = "Desc",
            UserIds = [],
            RequiredSkillIds = []
        };

        var result = await sut.ExecuteAsync(dto);

        // Even if empty lists, no errors, project is still created
        Assert.NotNull(result);
        var project = await context.Projects.FirstAsync();
        Assert.Empty(project.Users);
        Assert.Empty(project.RequiredSkills);
    }
}