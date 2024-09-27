using SavedMessages.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedMessages.Domain.Errors
{
    public static class DomainErrors
    {
        public static class UserErrors
        {
            public static Error NotFoundByEmail(string email) => Error.NotFound(
                "User.NotFound",
                $"User with this email {email} was not found");

            public static Error InvalidCredentials => Error.Validation(
                "User.InvalidCredentials",
                "The provided credentials are invalid");

            public static Error EmailAlreadyInUse => Error.Conflict(
                "User.Conflict",
                $"The provided email is not unique");

            //public static Error NotFound(Guid userId) => Error.NotFound(
            //    "User.NotFound",
            //    $"User with the identifier {userId} was not found");
        }

        public static class MessageErrors 
        {
            public static Error NotFoundMessageById(Guid messageId) => Error.NotFound(
                "Message.NotFound",
                $"Message with this id {messageId.ToString()} was not found");
        }

        public static class EmailErrors 
        {
            public static Error Empty => Error.Validation(
                "Email.Empty",
                "Email is empty");

            public static Error TooLong => Error.Validation(
                "Email.TooLong",
                "Email is too long");

            public static Error InvalidFormat => Error.Validation(
                "Email.InvalidFormat",
                "Email format is invalid");
        }

        public static class FirstNameErrors 
        {
            public static Error Empty => Error.Validation(
                "FirstName.Empty",
                "FirstName is empty");

            public static Error TooLong => Error.Validation(
                "FirstName.TooLong",
                "FirstName is too long");
        }

        public static class LastNameErrors
        {
            public static Error Empty => Error.Validation(
                "LastName.Empty",
                "LastName is empty");

            public static Error TooLong => Error.Validation(
                "LastName.TooLong",
                "LastName is too long");
        }
    }
}
