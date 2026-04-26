using FluentValidation;
using Ambev.DeveloperEvaluation.Application.Products.Dtos;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero");
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
    }
}