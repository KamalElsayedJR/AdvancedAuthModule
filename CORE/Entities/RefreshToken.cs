using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Entities
{
    public class RefreshToken
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        //public string Status { get; set; } = "Active";
        public string UserId { get; set; }
        public User User { get; set; }
        public RefreshToken()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
