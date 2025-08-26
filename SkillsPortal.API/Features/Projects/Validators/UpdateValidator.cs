using FastEndpoints;
using FluentValidation;
using SkillsPortal.API.Features.Projects.Update;

namespace SkillsPortal.API.Features.Projects.Validators;

public class UpdateValidator : Validator<UpdateProjectRequest>
{
    public UpdateValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0, and included in request URL");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required.")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters.")
            .Matches("[a-zA-Z]")
            .WithMessage("Name must contain only alphabetical characters.");

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