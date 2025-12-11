using Org.BouncyCastle.Crypto;

namespace AUTH.API.Response
{
    public class UserDto
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Verified { get; set; }
        public List<string> Roles { get; set; }
    }
}
