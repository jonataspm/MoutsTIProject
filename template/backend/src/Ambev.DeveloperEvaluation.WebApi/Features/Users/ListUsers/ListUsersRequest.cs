namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers;


public class ListUsersRequest
{
    public int? Page { get; set; }

    public int? Size { get; set; }

    public string? Order { get; set; }
}
