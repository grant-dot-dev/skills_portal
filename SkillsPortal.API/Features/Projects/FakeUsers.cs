using SkillsPortal.Core.Domain;

namespace SkillsPortal.API.Features.Projects;

public static class FakeUsers
{
    public static User Admin => new()
    {
        Id = 1,
        Name = "Grant Test",
        Email = "grant.test@test.com",
        StartDate = new DateTimeOffset(new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), TimeSpan.Zero),
        Level = 4,
        Skills = [],
        ProjectId = null,
        Project = null
    };
}