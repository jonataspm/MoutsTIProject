using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

public class ListUsersCommand : IRequest<ListUsersResult>
{
    public int? Page { get; set; } = 1;

    public int? Size { get; set; } = 10;

    public string Order { get; set; }
}