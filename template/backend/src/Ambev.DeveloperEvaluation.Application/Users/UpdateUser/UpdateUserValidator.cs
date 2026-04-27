using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(user => user.Email).SetValidator(new EmailValidator());
        RuleFor(user => user.Username).NotEmpty().Length(3, 50);
        RuleFor(user => user.Password).SetValidator(new PasswordValidator());
        RuleFor(user => user.Phone).Matches(@"^\+?[1-9]\d{1,14}$");
        RuleFor(user => user.Status).NotEqual(UserStatus.Unknown);
        RuleFor(user => user.Role).NotEqual(UserRole.None);
        
        RuleFor(x => x.Name).NotNull();
        RuleFor(x => x.Name.Firstname).NotEmpty().MaximumLength(50).When(x => x.Name != null);
        RuleFor(x => x.Name.Lastname).NotEmpty().MaximumLength(50).When(x => x.Name != null);

        RuleFor(x => x.Address).NotNull();
        RuleFor(x => x.Address.City).NotEmpty().When(x => x.Address != null);
        RuleFor(x => x.Address.Street).NotEmpty().When(x => x.Address != null);
        RuleFor(x => x.Address.Number).GreaterThan(0).When(x => x.Address != null);
        RuleFor(x => x.Address.Zipcode)
            .Matches(@"^\d{5}-?\d{3}$")
            .When(x => x.Address != null && !string.IsNullOrEmpty(x.Address.Zipcode));

        RuleFor(x => x.Address.Geolocation).NotNull().When(x => x.Address != null);
        RuleFor(x => x.Address.Geolocation.Lat).NotEmpty().When(x => x.Address?.Geolocation != null);
        RuleFor(x => x.Address.Geolocation.Long).NotEmpty().When(x => x.Address?.Geolocation != null);
    }
}
