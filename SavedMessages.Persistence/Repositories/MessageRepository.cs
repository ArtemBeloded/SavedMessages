using Microsoft.EntityFrameworkCore;
using SavedMessages.Domain.Messages;

namespace SavedMessages.Persistence.Repositories
{
    internal sealed class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Message message)
        {
            _context.Messages.Add(message);
        }

        public Task<Message?> GetByIdAsync(Guid messageId)
        {
            return _context.Messages
                .Include(f => f.File)
                .SingleOrDefaultAsync(m => m.Id == messageId);
        }

        public IQueryable<Message> GetMessages()
        {
            IQueryable<Message> query = _context.Messages;

            return query;
        }

        public void Remove(Message message)
        {
            _context.Messages.Remove(message);
        }

        public void Update(Message updateMessage)
        {
            _context.Messages.Update(updateMessage);
        }
    }
}
