using FastEndpoints;
using FluentValidation;
using SkillsPortal.API.Features.Projects.Delete;

namespace SkillsPortal.API.Features.Projects.Validators;

public class DeleteValidator : Validator<DeleteProjectRequest>
{
    public DeleteValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than zero and included in the url");
    }
}