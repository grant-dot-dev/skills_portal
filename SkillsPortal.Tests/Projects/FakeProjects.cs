using Bogus;
using Bogus.DataSets;
using SkillsPortal.API.Features.Projects.Create;
using SkillsPortal.Core.Domain;

namespace SkillsPortal.Tests.Projects;

public static class FakeProjects
{
    public static CreateProjectRequest FakeCreateProjectRequest => new Faker<CreateProjectRequest>()
        .RuleFor(u => u.Client, f => f.Company.CompanyName())
        .RuleFor(u => u.Name, f => f.Hacker.Noun())
        .RuleFor(u => u.Description, f => f.Lorem.Sentence());


    public static Project ToProject(this CreateProjectRequest request) => new()
    {
        Name = request.Name,
        Description = request.Description,
        Client = request.Client,
        Users = [],
        RequiredSkills = []
    };
}