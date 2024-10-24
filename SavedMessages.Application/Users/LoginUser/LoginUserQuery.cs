using MediatR;
using SavedMessages.Domain.Shared;

namespace SavedMessages.Application.Users.LoginUser
{
    public record LoginUserQuery(string Email, string Password) : IRequest<Result<LoginResponse>>;
}
