using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProductsByCategory;

public class ListProductsByCategoryValidator : AbstractValidator<ListProductsByCategoryCommand>
{
    public ListProductsByCategoryValidator()
    {
        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Category cannot be empty.");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.Size)
            .GreaterThan(0)
            .WithMessage("Size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Size cannot exceed 100 items per page.");
    }
}