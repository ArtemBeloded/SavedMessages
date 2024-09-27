using SavedMessages.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedMessages.Application.Users
{
    public interface ITokenProvider
    {
        string Create(User user);
    }
}
