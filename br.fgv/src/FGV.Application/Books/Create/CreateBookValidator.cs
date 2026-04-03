using FluentValidation;

namespace FGV.Application.Books.Create;

internal sealed class CreateBookValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(500)
            .WithMessage("Title must not exceed 500 characters");

        RuleFor(x => x.Author)
            .NotEmpty()
            .WithMessage("Author is required")
            .MaximumLength(300)
            .WithMessage("Author must not exceed 300 characters");

        RuleFor(x => x.Edition)
            .GreaterThan(0)
            .WithMessage("Edition must be greater than 0")
            .LessThanOrEqualTo(DateTime.UtcNow.Year)
            .WithMessage("Edition cannot be in the future");
    }
}
