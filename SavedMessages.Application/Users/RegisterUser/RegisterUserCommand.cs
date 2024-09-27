using MediatR;
using SavedMessages.Domain.Shared;
using SavedMessages.Domain.Users;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedMessages.Application.Users.RegisterUser
{
    public record RegisterUserCommand(
        string Email,
        string FirstName,
        string LastName,
        string Password) : IRequest<Result>;
}
