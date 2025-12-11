using CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORY.Data
{
    public static class IdentityDataSeed
    {
        public static async Task SeedDataAsync(IdentityDbContext _dbContext)
        {
            Roles[] roles = new Roles[]
            {
                new Roles(){ Role="Admin"},
                new Roles(){ Role="Viewer"},
                new Roles(){ Role="Editor"},
            };
            if (!_dbContext.Roles.Any())
            {
                await _dbContext.Roles.AddRangeAsync(roles);
            }
            User[] users = new User[]
            {
                new User(){ FirstName="System", LastName="Admin", Email="Admin@admin.com", HashPassword="799650CFF1DB66E8AF04F9911132EA2F-3F622486D0DE519796F39B6DB38308A6151438053C2DE1CD7B36CEB40D22D407", IsVerified=true},
                new User(){ FirstName="System", LastName="Viewer", Email="Viewer@viewr.com", HashPassword="799650CFF1DB66E8AF04F9911132EA2F-3F622486D0DE519796F39B6DB38308A6151438053C2DE1CD7B36CEB40D22D407", IsVerified=true},
                new User(){ FirstName="System", LastName="Editor", Email="Editor@editor.com" ,HashPassword="799650CFF1DB66E8AF04F9911132EA2F-3F622486D0DE519796F39B6DB38308A6151438053C2DE1CD7B36CEB40D22D407", IsVerified=true}
            };
            if (!_dbContext.Users.Any())
            {
               await _dbContext.Users.AddRangeAsync(users);
            }
            UserRoles[] userRoles = new UserRoles[]
            {
                new UserRoles(){ User=users[0], Role=roles[0]},
                new UserRoles(){ User=users[1], Role=roles[1]},
                new UserRoles(){ User=users[2], Role=roles[2]},
            };
            if (!_dbContext.UserRoles.Any())
            {
                await _dbContext.UserRoles.AddRangeAsync(userRoles);
            }
            await _dbContext.SaveChangesAsync();

        }
    }
}
