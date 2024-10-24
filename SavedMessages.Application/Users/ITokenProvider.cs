using SavedMessages.Domain.Users;

namespace SavedMessages.Application.Users
{
    public interface ITokenProvider
    {
        string Create(User user);
    }
}
