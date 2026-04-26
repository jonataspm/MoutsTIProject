using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers;

/// <summary>
/// Validator for GetUserRequest
/// </summary>
public class ListUsersRequestValidator : AbstractValidator<ListUsersRequest>
{
    /// <summary>
    /// Initializes validation rules for GetUserRequest
    /// </summary>
    public ListUsersRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1");

        RuleFor(x => x.Size)
            .InclusiveBetween(1, 100)
            .WithMessage("Size must be between 1 and 100");

        RuleFor(x => x.Order)
            .Must(orderBy =>
            {
                if (string.IsNullOrWhiteSpace(orderBy))
                    return true;

                var orderList = orderBy.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var order in orderList)
                {
                    var parts = order.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var field = parts[0].ToLower();

                    if (!Names.Contains(field))
                        return false;

                    if (parts.Length > 1)
                    {
                        var direction = parts[1].ToLower();
                        if (direction != "asc" && direction != "desc")
                            return false;
                    }
                }

                return true;
            })
            .WithMessage("Uma ou mais colunas de ordenação são inválidas ou não existem no usuário.");
    }

    public static readonly HashSet<string> Names = typeof(User)
        .GetProperties()
        .Select(p => p.Name.ToLower())
        .ToHashSet();
}
