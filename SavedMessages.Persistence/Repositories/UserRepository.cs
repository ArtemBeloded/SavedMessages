using Microsoft.EntityFrameworkCore;
using SavedMessages.Domain.Users;

namespace SavedMessages.Persistence.Repositories
{
    internal sealed class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
        }

        public async Task<bool> IsExistsByEmail(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        public async Task<bool> IsExistsById(Guid id) 
        {
            return await _context.Users.AnyAsync(x => x.Id == id);
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
        }
    }
}
