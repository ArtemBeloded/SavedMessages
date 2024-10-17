namespace SavedMessages.Domain.Messages
{
    public interface IMessageFileRepository
    {
        void Add(MessageFile messageFile);

        void Remove(MessageFile messageFile);
    }
}
