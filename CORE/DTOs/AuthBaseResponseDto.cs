using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CORE.DTOs
{
    public class AuthBaseResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public bool? IsVerified { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public AuthBaseResponseDto()
        {
        }
        public AuthBaseResponseDto(bool success, string message, string? id, string? firstName, string? lastName, string? email, bool? isVerified, DateTimeOffset? createdAt, string? accessToken, string? refreshToken)
        {
            Success = success;
            Message = message;
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            IsVerified = isVerified;
            CreatedAt = createdAt;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public AuthBaseResponseDto(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
