using System.ComponentModel.DataAnnotations;

namespace AUTH.API.Request
{
    public class RTokenDto
    {
        [Required]
        public string Token { get; set; }
    }
}
