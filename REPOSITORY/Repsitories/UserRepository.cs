using CORE.Entities;
using CORE.Interfaces;
using Microsoft.EntityFrameworkCore;
using REPOSITORY.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORY.Repsitories
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityDbContext _dbContext;

        public UserRepository(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<User?> FindByRefreshTokenAsync(string RefreshToken)
        {
            var Token = await _dbContext.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == RefreshToken);
            if (Token == null || Token.ExpiresAt <= DateTimeOffset.Now) return null;
            return Token?.User;
        }
        public async Task<User?> GetUserByEmailAsync(string Email)
        => await _dbContext.Users.FirstOrDefaultAsync(u=>u.Email == Email);


        //
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        => await _dbContext.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == token);

        public bool DeleteAllRefreshTokensForUser(string userId)
        {
            var tokens = _dbContext.RefreshTokens.Where(rt => rt.UserId == userId);
            if (!tokens.Any()) return false;
            _dbContext.RefreshTokens.RemoveRange(tokens);
            return true;
        }
    }
}
