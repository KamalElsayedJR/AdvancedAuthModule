using CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Interfaces
{
    public interface IRolesRepository
    {
        IQueryable<UserRoles> GetRoleForUser(string UserId);
        string GetRoleidByName(string RoleName);
    }
}
