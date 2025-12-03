using CORE.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CORE.Interfaces
{
    public interface IAuthService
    {
        Task<AuthBaseResponseDto> Register(UserRegisterDto user);
        Task<AuthBaseResponseDto> Login(UserLoginDto user);
        Task<AuthBaseResponseDto> RefreshToken(string token);
        Task<bool> LogOutAsync(string token);
        string HashPassowrdHandler(string Password);
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
        Task<AuthBaseResponseDto> ForgetPasswordAsync(string Email);
        Task<bool> ResetPassword(string email, string newpassword);
        Task<AuthBaseResponseDto> UpdateProfileAsync(UpdateUserDto dto);
        Task<bool> ChangePassword(ChangePasswordDto dto,string email);
    }
}
