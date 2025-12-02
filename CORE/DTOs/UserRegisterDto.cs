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
        public string Password { get; set; }

    }
}
