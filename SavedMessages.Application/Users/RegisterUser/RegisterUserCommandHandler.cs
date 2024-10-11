using MediatR;
using SavedMessages.Domain.Errors;
using SavedMessages.Domain.Shared;
using SavedMessages.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SavedMessages.Domain.Errors.DomainErrors;

namespace SavedMessages.Application.Users.RegisterUser
{
    public sealed class RegisterUserCommandHandler
        : IRequestHandler<RegisterUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken) 
        {
            if (await _userRepository.IsExistsByEmail(request.Email)) 
            {
                return Result.Failure(UserErrors.EmailAlreadyInUse);
            }

            Result<User> userResult = User.Create(
                request.Email,
                request.FirstName,
                request.LastName,
                _passwordHasher.Hash(request.Password));

            if (userResult.IsFailure) 
            {
                return Result.Failure(userResult.Error);
            }

            _userRepository.Add(userResult.Value);

            return Result.Success();
        }
    }
}
