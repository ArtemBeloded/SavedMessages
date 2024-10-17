using MediatR;
using Microsoft.AspNetCore.Mvc;
using SavedMessages.API.Contracts.Users;
using SavedMessages.API.Extensions;
using SavedMessages.Application.Users.GetUser;
using SavedMessages.Application.Users.LoginUser;
using SavedMessages.Application.Users.RegisterUser;
using SavedMessages.Domain.Shared;

namespace SavedMessages.API.Endpoints
{
    [Route("api/users")]
    public sealed class UserController : Controller
    {
        private ISender _sender;

        public UserController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("registration")]
        public async Task<IResult> Register([FromBody] RegisterUserRequest request) 
        {
            var command = new RegisterUserCommand(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password);

            Result response = await _sender.Send(command);

            return response.IsSuccess ? Results.Ok() : response.ToProblemDetails();
        }

        [HttpPost("login")]
        public async Task<IResult> LoginUser([FromBody] LoginRequest request) 
        {
            var loginUserQuery = new LoginUserQuery(request.Email, request.Password);

            Result<LoginResponse> response = await _sender.Send(loginUserQuery);

            return response.IsSuccess ? Results.Ok(response.Value) : response.ToProblemDetails();
        }

        [HttpGet]
        public async Task<IResult> GetUser(string email) 
        {
            var getUserQuery = new GetUserQuery(email);

            Result<UserResponse> response = await _sender.Send(getUserQuery);

            return response.IsSuccess ? Results.Ok(response.Value) : response.ToProblemDetails();
        }
    }
}
