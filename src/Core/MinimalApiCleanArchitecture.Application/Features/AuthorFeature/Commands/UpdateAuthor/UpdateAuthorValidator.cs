using FluentValidation;

namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.UpdateAuthor
{
    public class UpdateAuthorValidator: AbstractValidator<UpdateAuthorCommand>
    {
        public UpdateAuthorValidator()
        {
            RuleFor(_=> _.FirstName)
                .NotEmpty()
                .NotNull();

            RuleFor(_ => _.LastName)
                .NotEmpty()
                .NotNull();

            RuleFor(_ => _.Bio)
                .NotEmpty()
                .NotNull();
        }
    }
}
