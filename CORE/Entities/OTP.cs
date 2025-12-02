using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Entities
{
    public class OTP
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string OTPCode { get; set; }
        public DateTimeOffset ExpiryDate { get; set; }
        public bool IsUsed { get; set; } = false;
    }
}
