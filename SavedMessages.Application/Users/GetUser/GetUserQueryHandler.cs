using MediatR;
using SavedMessages.Domain.Shared;
using SavedMessages.Domain.Users;
using static SavedMessages.Domain.Errors.DomainErrors;

namespace SavedMessages.Application.Users.GetUser
{
    public sealed class GetUserQueryHandler
        : IRequestHandler<GetUserQuery, Result<UserResponse>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmail(request.Email);

            if (user is null) 
            {
                return Result.Failure<UserResponse>(UserErrors.NotFoundByEmail(request.Email));
            }

            var userResponse = new UserResponse(
                user.Id,
                user.FirstName,
                user.LastName);

            return userResponse;
        }
    }
}
