using MediatR;
using SavedMessages.Domain.Shared;
using SavedMessages.Domain.Users;
using static SavedMessages.Domain.Errors.DomainErrors;

namespace SavedMessages.Application.Users.LoginUser
{
    public sealed class LoginUserQueryHandler
        : IRequestHandler<LoginUserQuery, Result<LoginResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenProvider _tokenProvider;

        public LoginUserQueryHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenProvider tokenProvider)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenProvider = tokenProvider;
        }

        public async Task<Result<LoginResponse>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetByEmail(request.Email);

            if (user is null)
            {
                return Result.Failure<LoginResponse>(UserErrors.NotFoundByEmail(request.Email));
            }

            bool verified = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!verified)
            {
                return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);
            }

            var token = _tokenProvider.Create(user);
            var responce = new LoginResponse(token);

            return responce;
        }
    }
}
