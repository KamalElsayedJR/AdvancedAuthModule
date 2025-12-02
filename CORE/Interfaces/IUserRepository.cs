using CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Interfaces
{
    public interface IUserRepository 
    {
        Task<User?> GetUserByEmailAsync(string Email);
        Task<User?> FindByRefreshTokenAsync(string RefreshToken);

        //
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        bool DeleteAllRefreshTokensForUser(string userId);
    }
}
