using Microsoft.EntityFrameworkCore;
using SavedMessages.Domain.Messages;
using SavedMessages.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedMessages.Application.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Message> Messages { get; set; }

        DbSet<User> Users { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    }
}
