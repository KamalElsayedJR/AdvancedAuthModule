using CORE.DTOs;
using CORE.Entities;
using CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SERVICES
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public EmailVerificationService(IUnitOfWork unitOfWork,IEmailService emailService) 
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<AuthBaseResponseDto> GenerateOTPAsync(string email)
        {

            if (email == null || await _unitOfWork.IUserRepository.GetUserByEmailAsync(email) is null) 
                return new AuthBaseResponseDto(false,"Email Not Found");
            var otp = RandomNumberGenerator.GetInt32(1000,10000).ToString();
            var otpEntity = new OTP()
            {
                Email = email,
                OTPCode = otp,
                ExpiryDate = DateTimeOffset.Now.AddMinutes(5)
            };
            var oldotps =(await _unitOfWork.GenericRepo<OTP>().FindAsync(e=>e.Email == email)).ToList();
            if (oldotps.Any())
            {
                _unitOfWork.GenericRepo<OTP>().DeleteRange(oldotps);
            }
            await _unitOfWork.GenericRepo<OTP>().AddAsync(otpEntity);
            var Result = await _unitOfWork.SaveChangesAsync();
            if (Result <= 0) return new AuthBaseResponseDto(false,"Can't Send OTP to this Email");
            var emailDto = new EmailDto()
            {
                ToEmail = email,
                Subject = "Your OTP Verification Code",
                Body = $"Your OTP Code is {otp}. It will expire in 5 minutes."
            };
            await _emailService.SendEmailAsync(emailDto);
            return new AuthBaseResponseDto(true,"OTP Sent Successfully,Check Your Email");
        }
        public async Task<bool> OTPEmailVerifiyAsync(string email, string otp)
        {
            var user = await _unitOfWork.IUserRepository.GetUserByEmailAsync(email);
            if (user is null || !await OtpVerifyAsync(otp,email)) return false;
            user.IsVerified = true;
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }
        public async Task<bool> OtpVerifyAsync(string otp,string email)
        {
            var otpEntity = (await _unitOfWork.GenericRepo<OTP>().FindAsync(o=>o.OTPCode == otp&&o.Email == email)).FirstOrDefault();
            if (otpEntity is null || otpEntity.IsUsed  || otpEntity.ExpiryDate < DateTimeOffset.Now) return false;
            otpEntity.IsUsed = true;
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

    }
}
