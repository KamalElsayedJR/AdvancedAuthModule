using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Entities
{
    public class Roles
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public Roles()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
