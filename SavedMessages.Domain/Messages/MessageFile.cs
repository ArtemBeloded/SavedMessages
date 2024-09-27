namespace SavedMessages.Domain.Messages
{
    public class MessageFile
    {
        public Guid Id { get; set; }

        public Guid MessageId { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public long FileSize { get; set; }

        public string ContentType { get; set; }
    }
}
