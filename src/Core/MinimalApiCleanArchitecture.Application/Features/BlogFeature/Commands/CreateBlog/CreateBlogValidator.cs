using FluentValidation;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.CreateBlog;

public class CreateBlogValidator: AbstractValidator<CreateBlogCommand>
{
    public CreateBlogValidator()
    {
        RuleFor(_ => _.Name).NotNull().NotEmpty();
        RuleFor(_ => _.Description).NotNull().NotEmpty();
        RuleFor(_ => _.AuthorId).NotNull().NotEmpty();
    }
}