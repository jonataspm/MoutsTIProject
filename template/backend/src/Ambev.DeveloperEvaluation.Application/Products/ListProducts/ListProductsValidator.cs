using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public class ListProductsValidator : AbstractValidator<ListProductsCommand>
{
    public ListProductsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.Size)
            .GreaterThan(0)
            .WithMessage("Size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Size cannot exceed 100 items per page."); // Boa prática para evitar consultas excessivamente grandes no banco
    }
}