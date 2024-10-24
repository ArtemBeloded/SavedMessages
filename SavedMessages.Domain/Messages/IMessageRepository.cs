namespace SavedMessages.Domain.Messages
{
    public interface IMessageRepository
    {
        void Add(Message message);

        Task<Message?> GetByIdAsync(Guid messageId);

        IQueryable<Message> GetMessages();

        void Remove(Message message);

        void Update(Message updateMessage);
    }
}
