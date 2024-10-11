using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedMessages.Domain.Users
{
    public interface IUserRepository
    {
        void Add(User user);

        Task<bool> IsExistsByEmail(string email);

        Task<bool> IsExistsById(Guid id);

        Task<User?> GetByEmail(string email);
    }
}
