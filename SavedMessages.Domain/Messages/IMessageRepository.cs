using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
