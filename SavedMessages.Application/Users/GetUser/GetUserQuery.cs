using MediatR;
using SavedMessages.Domain.Shared;

namespace SavedMessages.Application.Users.GetUser
{
    public record GetUserQuery(string Email) : IRequest<Result<UserResponse>>;
}
