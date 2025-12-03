using CORE.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Interfaces
{
    public interface IEmailVerificationService
    {
        Task<AuthBaseResponseDto> GenerateOTPAsync(string email);
        Task<bool> OTPEmailVerifiyAsync(string email,string opt);
        Task<bool> OtpVerifyAsync(string otp,string email);
    }
}
