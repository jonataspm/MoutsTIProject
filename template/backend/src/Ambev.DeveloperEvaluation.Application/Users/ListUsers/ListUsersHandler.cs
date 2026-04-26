using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

public class ListUsersHandler : IRequestHandler<ListUsersCommand, ListUsersResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ListUsersHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ListUsersResult> Handle(ListUsersCommand command, CancellationToken cancellationToken)
    {
        var validator = new ListUsersValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var (users, totalUsers) = await _userRepository.GetPagedAsync(command.Page.Value, command.Size.Value, command.Order, cancellationToken);

        var mapped = _mapper.Map<IEnumerable<ListUserItemResult>>(users);

        var totalItems = users.Count();
        var totalPages = command.Size == 0 ? 0 : (int)Math.Ceiling((double)totalUsers / command.Size.Value);

        return new ListUsersResult
        {
            Data = mapped,
            TotalItems = totalItems,
            CurrentPage = command.Page.Value,
            TotalPages = totalPages
        };
    }
}
