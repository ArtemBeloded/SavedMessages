using SavedMessages.Domain.Messages;

namespace SavedMessages.Persistence.Repositories
{
    internal sealed class MessageFileRepository : IMessageFileRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageFileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(MessageFile messageFile)
        {
            _context.MessageFiles.Add(messageFile);
        }

        public void Remove(MessageFile messageFile)
        {
            _context.MessageFiles.Remove(messageFile);
        }
    }
}
