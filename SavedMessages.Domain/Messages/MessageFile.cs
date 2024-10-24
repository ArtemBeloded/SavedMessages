using SavedMessages.Domain.Shared;

namespace SavedMessages.Domain.Messages
{
    public class MessageFile
    {
        private MessageFile(
            Guid id,
            Guid messageId,
            string fileName,
            byte[] data,
            string contentType)
        {
            Id = id;
            MessageId = messageId;
            FileName = fileName;
            Data = data;
            ContentType = contentType;
        }

        public Guid Id { get; private set; }

        public Guid MessageId { get; private set; }

        public string FileName { get; private set; }

        public byte[] Data { get; private set; }

        public string ContentType { get; private set; }

        public static Result<MessageFile> Create(Guid messageId, byte[] fileData, string contentType, string fileName)
        {
            var messageFile = new MessageFile(
                Guid.NewGuid(),
                messageId,
                fileName,
                fileData,
                contentType);

            return messageFile;
        }

        public Result Update(byte[] fileData, string contentType, string fileName)
        {
            Data = fileData;
            ContentType = contentType;
            FileName = fileName;

            return Result.Success();
        }
    }
}
