using SavedMessages.Domain.Shared;

namespace SavedMessages.Domain.Messages
{
    public class Message
    {
        private Message(
            Guid id,
            Guid userId,
            DateTime createdInUtc,
            string text)
        {
            Id = id;
            UserId = userId;
            CreatedInUtc = createdInUtc;
            Text = text;
        }

        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public DateTime CreatedInUtc { get; private set; }

        public bool IsEdited { get; private set; } = false;

        public string? Text { get; private set; }

        public MessageFile? File { get; private set; }

        public static Result<Message> Create(
            Guid userId,
            string text)
        {
            //Credential verification

            var message = new Message(
                Guid.NewGuid(),
                userId,
                DateTime.Now,
                text);

            return message;
        }

        public Result AttachFile(MessageFile file) 
        {
            File = file;

            return Result.Success();
        }

        public Result Update(string text) 
        {
            //Credential verification

            IsEdited = true;
            Text = text;

            return Result.Success();
        }

        public Result RemoveMessageFile() 
        {
            if (File is not null) 
            {
                File = null;
            }

            return Result.Success();
        }
    }
}
