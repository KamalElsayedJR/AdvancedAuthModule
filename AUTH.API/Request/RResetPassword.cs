using System.ComponentModel.DataAnnotations;

namespace AUTH.API.Request
{
    public class RResetPassword
    {
        public string Email { get; set; }
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&._-])[A-Za-z\\d@$!%*?&._-]{8,}$", ErrorMessage = " Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string NewPassword { get; set; }
    }
}
