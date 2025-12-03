using CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateAccessTokenAsync(User user);
        string CreateRefreshToken();

    }
}
