using FluentValidation;

namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;

public class CreateAuthorValidator: AbstractValidator<CreateAuthorCommand>
{
    public CreateAuthorValidator()
    {
        RuleFor(_ => _.FirstName)
            .NotNull()
            .NotEmpty();
        RuleFor(_ => _.LastName)
            .NotNull()
            .NotEmpty();
        RuleFor(_ => _.Bio)
            .NotNull()
            .NotEmpty();
    }
}