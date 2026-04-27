using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty().WithMessage("A venda deve conter pelo menos um item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantidade deve ser maior que zero.")
                .LessThanOrEqualTo(20).WithMessage("Não é possível vender acima de 20 itens idênticos.");
            item.RuleFor(i => i.UnitPrice).GreaterThan(0);
        });
    }
}