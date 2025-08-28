using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SkillsPortal.API.Features.Projects.Create;

namespace SkillsPortal.API.Features.Projects.Validators;

public class CreateProjectValidator : Validator<CreateProjectRequest>
{
    private readonly IProjectRepository _repository;

    public CreateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required.")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters.")
            .Matches("^[a-zA-Z ]*$")
            .WithMessage("Name must contain only alphabetical characters.")
            .MustAsync(async (name, ct) =>
            {
                var repo = Resolve<IProjectRepository>();
                return !await repo.Query().AnyAsync(x => x.Name == name, cancellationToken: ct);
            }).WithMessage("Name must be unique.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.Client)
            .NotEmpty()
            .WithMessage("Client is required.")
            .MinimumLength(3)
            .WithMessage("Client must be at least 3 characters.");
    }
}