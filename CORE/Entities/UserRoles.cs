using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Entities
{
    public class UserRoles
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string RoleId { get; set; }
        public Roles Role { get; set; }
        public DateTimeOffset AssignedAt { get; set; }
        public UserRoles()
        {
            Id = Guid.NewGuid().ToString();
            AssignedAt = DateTimeOffset.UtcNow;
        }
    }
}
