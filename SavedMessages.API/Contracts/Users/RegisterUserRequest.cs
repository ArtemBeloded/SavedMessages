﻿namespace SavedMessages.API.Contracts.Users
{
    public record RegisterUserRequest(
        string Email,
        string FirstName,
        string LastName,
        string Password);
}
