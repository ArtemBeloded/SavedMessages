using SavedMessages.Domain.Shared;
using static SavedMessages.Domain.Errors.DomainErrors;

namespace SavedMessages.Domain.Users
{
    public class User
    {
        public const int MaxLength = 255;

        private User(
            Guid id,
            string email,
            string firstName,
            string lastName,
            string passwordHash)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            PasswordHash = passwordHash;
        }

        public Guid Id { get; private set; }

        public string Email { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string PasswordHash { get; private set; }

        public static Result<User> Create(
            string email,
            string firstName,
            string lastName,
            string passwordHash)
        {
            Result<string> emailResult = IsEmailCorrect(email);

            if (emailResult.IsFailure) 
            {
                return Result.Failure<User>(emailResult.Error);
            }

            Result<string> firstNameResult = IsFirstNameCorrect(firstName);

            if (firstNameResult.IsFailure) 
            {
                return Result.Failure<User>(firstNameResult.Error);
            }

            Result<string> lastNameResult = IsLastNameCorrect(lastName);

            if (lastNameResult.IsFailure) 
            {
                return Result.Failure<User>(lastNameResult.Error);
            }

            var user = new User(
                Guid.NewGuid(),
                emailResult.Value,
                firstNameResult.Value,
                lastNameResult.Value,
                passwordHash);

            return user;
        }

        private static Result<string> IsEmailCorrect(string email) 
        {
             return Result.Create(email)
                .Ensure(
                    e => !string.IsNullOrWhiteSpace(e),
                    EmailErrors.Empty)
                .Ensure(
                    e => e.Length <= MaxLength,
                    EmailErrors.TooLong)
                .Ensure(
                    e => e.Split('@').Length == 2,
                    EmailErrors.InvalidFormat)
                .Map(e => email);
        }

        private static Result<string> IsFirstNameCorrect(string firstName) 
        {
            return Result.Create(firstName)
                .Ensure(
                    f => !string.IsNullOrWhiteSpace(f),
                    FirstNameErrors.Empty)
                .Ensure(
                    f => f.Length <= MaxLength,
                    FirstNameErrors.TooLong)
                .Map(f => firstName);
        }

        private static Result<string> IsLastNameCorrect(string lastName) 
        {
            return Result.Create(lastName)
                .Ensure(
                    l => !string.IsNullOrWhiteSpace(l),
                    FirstNameErrors.Empty)
                .Ensure(
                    l => l.Length <= MaxLength,
                    FirstNameErrors.TooLong)
                .Map(l => lastName);
        }
    }
}
