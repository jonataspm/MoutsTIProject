using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProductsByCategory;

public class ListProductsByCategoryRequestValidator : AbstractValidator<ListProductsByCategoryRequest>
{
    public ListProductsByCategoryRequestValidator()
    {
        RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required");
        RuleFor(x => x.Page).GreaterThan(0).When(x => x.Page.HasValue);
        RuleFor(x => x.Size).GreaterThan(0).When(x => x.Size.HasValue);
    }
}