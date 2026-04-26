using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.Dtos;

public class RatingDtoValidator : AbstractValidator<RatingDto>
{
    public RatingDtoValidator()
    {
        RuleFor(x => x.Rate).InclusiveBetween(0, 5).WithMessage("Rate must be between 0 and 5");
        RuleFor(x => x.Count).GreaterThanOrEqualTo(0).WithMessage("Count cannot be negative");
    }
}