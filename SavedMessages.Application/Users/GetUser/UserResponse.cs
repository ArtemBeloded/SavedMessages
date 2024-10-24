namespace SavedMessages.Application.Users.GetUser
{
    public record UserResponse(
        Guid Id,
        string FirstName,
        string LastName);
}
