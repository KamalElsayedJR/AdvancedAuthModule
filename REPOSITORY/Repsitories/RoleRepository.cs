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
    public class RoleRepository : IRolesRepository
    {
        private readonly IdentityDbContext _dbContext;

        public RoleRepository(IdentityDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public IQueryable<UserRoles> GetRoleForUser(string UserId)
        => _dbContext.UserRoles.Include(r=>r.Role).Where(r=>r.UserId == UserId);

        public string GetRoleidByName(string RoleName)
        {
            var Role = _dbContext.Roles.Where(r=>r.Role.ToLower().Trim() == RoleName.ToLower().Trim());
            foreach (var value in Role)
            {
                return value.Id;
            }
            return string.Empty;
        }
    }
}
