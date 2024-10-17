using MediatR;
using SavedMessages.Domain.Shared;

namespace SavedMessages.Application.Users.RegisterUser
{
    public record RegisterUserCommand(
        string Email,
        string FirstName,
        string LastName,
        string Password) : IRequest<Result>;
}
