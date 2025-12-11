using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.DTOs
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "FirstName Is Required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName Is Required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password Is Required")]
        [MinLength(8, ErrorMessage = "Password Must Be At Least 8 Characters Long")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&._-])[A-Za-z\\d@$!%*?&._-]{8,}$",ErrorMessage = " Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Role Is Required")]
        public string Role { get; set; }
    }
}
