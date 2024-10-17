using Microsoft.EntityFrameworkCore;
using SavedMessages.Domain.Messages;
using SavedMessages.Domain.Users;

namespace SavedMessages.Application.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Message> Messages { get; set; }

        DbSet<User> Users { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
