namespace SavedMessages.Domain.Messages
{
    public class MessageImage
    {
        public Guid Id { get; set; }

        public Guid MessageId { get; set; }

        public string ImageName { get; set; }

        public string ImagePath { get; set; }

        public long ImageSize { get; set; }

        public string ContentType { get; set; }
    }
}
