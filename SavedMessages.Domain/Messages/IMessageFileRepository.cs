using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedMessages.Domain.Messages
{
    public interface IMessageFileRepository
    {
        void Add(MessageFile messageFile);

        void Remove(MessageFile messageFile);
    }
}
