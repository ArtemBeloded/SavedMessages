using MediatR;
using SavedMessages.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedMessages.Application.Users.LoginUser
{
    public record LoginUserQuery(string Email, string Password) : IRequest<Result<LoginResponse>>;
}
