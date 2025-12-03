using AutoMapper;
using CORE.DTOs;
using CORE.Entities;
using CORE.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using REPOSITORY.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SERVICES
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IEmailVerificationService _emailVerification;

        public AuthService(IMapper mapper,IUnitOfWork unitOfWork,ITokenService tokenService,IEmailVerificationService emailVerification)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _emailVerification = emailVerification;
        }
        public async Task<AuthBaseResponseDto> Register(UserRegisterDto dto)
        {
            if (dto is null) return new AuthBaseResponseDto(false,"Invalid Inputs");
            if (await _unitOfWork.IUserRepository.GetUserByEmailAsync(dto.Email) is not null) return new AuthBaseResponseDto(false,"Email Already Exist");
            var MappedUser = _mapper.Map<UserRegisterDto, User>(dto);
            MappedUser.HashPassword = HashPassowrdHandler(dto.Password);
            await _unitOfWork.GenericRepo<User>().AddAsync(MappedUser);
            var Result = await _unitOfWork.SaveChangesAsync();
            if (Result <= 0) return new AuthBaseResponseDto(false, "Errors During User Registration");

            // Send Email Verification OTP
            var respone = await _emailVerification.GenerateOTPAsync(dto.Email);
            //if (!respone.Success) return new AuthBaseResponseDto(false, respone.Message);
            if (!respone.Success)
            {
                _unitOfWork.GenericRepo<User>().Delete(MappedUser);
                await _unitOfWork.SaveChangesAsync();
                return new AuthBaseResponseDto(false, respone.Message + "Registeration Failed");
            };
            var ReturnedResponse = _mapper.Map<User, AuthBaseResponseDto>(MappedUser);
            ReturnedResponse.Success = true;
            ReturnedResponse.Message = "User Registered Successfully, " + respone.Message;
            return ReturnedResponse;
        }
        public async Task<AuthBaseResponseDto> Login(UserLoginDto dto)
        {
            if (dto is null) return new AuthBaseResponseDto(false, "Invalid Inputs");
            var User = await _unitOfWork.IUserRepository.GetUserByEmailAsync(dto.Email);
            if (User is null) return new AuthBaseResponseDto(false, "Invalid Email or Password");
            if (!VerifyHashedPassword(User.HashPassword,dto.Password)) return new AuthBaseResponseDto(false, "Invalid Email or Password");
            if (User.IsVerified == false) return new AuthBaseResponseDto(false, "Please Verifiy Your Email First");
            var accessToken = await _tokenService.CreateAccessTokenAsync(User);
            var refreshToken = _tokenService.CreateRefreshToken();
            var refreshTokenEntity = new RefreshToken()
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = User.Id
            };
            await _unitOfWork.GenericRepo<RefreshToken>().AddAsync(refreshTokenEntity);
            var Result = await _unitOfWork.SaveChangesAsync();
            if(Result <= 0) return new AuthBaseResponseDto(false, "Errors During Login");
            var ReturnedResponse = _mapper.Map<User, AuthBaseResponseDto>(User);
            ReturnedResponse.Success = true;
            ReturnedResponse.Message = "Login Success";
            ReturnedResponse.AccessToken = accessToken;
            ReturnedResponse.RefreshToken = refreshToken;
            return ReturnedResponse;
        }
        public string HashPassowrdHandler(string Password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(Password,salt,10000,HashAlgorithmName.SHA512,32);
            return Convert.ToHexString(salt) + "-" + Convert.ToHexString(hash);
        }
        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var Parts = hashedPassword.Split('-');
            if (Parts.Length != 2) return false;
            byte[] salt = Convert.FromHexString(Parts[0]);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(providedPassword,salt,10000,HashAlgorithmName.SHA512,32);
            return Convert.ToHexString(hash) == Parts[1];
        }
        public async Task<AuthBaseResponseDto> RefreshToken(string token)
        {
            var User = await _unitOfWork.IUserRepository.FindByRefreshTokenAsync(token);
            if (User is null) return new AuthBaseResponseDto(false, "Invalid Token *LoginAgain*");

            var rt = await _unitOfWork.IUserRepository.GetRefreshTokenAsync(token);
            _unitOfWork.GenericRepo<RefreshToken>().Delete(rt);
            var dResult = await _unitOfWork.SaveChangesAsync();
            if (dResult <= 0) return new AuthBaseResponseDto(false, "Errors During Generating New Tokens");
            var newRefreshToken = _tokenService.CreateRefreshToken();
            var refreshTokenEntity = new RefreshToken()
            {
                Token = newRefreshToken,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
                UserId = User.Id
            };
            await _unitOfWork.GenericRepo<RefreshToken>().AddAsync(refreshTokenEntity);
            var sResult = await _unitOfWork.SaveChangesAsync();
            if (sResult <= 0) return new AuthBaseResponseDto(false, "Errors During RefreshToken");

            var accessToken = await _tokenService.CreateAccessTokenAsync(User);

            var ReturnedResponse = _mapper.Map<User, AuthBaseResponseDto>(User);
            ReturnedResponse.Success = true;
            ReturnedResponse.Message = "Access Token Generated Successfully";
            ReturnedResponse.AccessToken = accessToken;
            ReturnedResponse.RefreshToken = newRefreshToken;
            return ReturnedResponse;
        }
        public async Task<bool> LogOutAsync(string token)
        {
            var rt = (await _unitOfWork.GenericRepo<RefreshToken>().FindAsync(rt => rt.Token == token)).FirstOrDefault();
            if (rt is null) return false;
            //var alltokenforuser = _unitOfWork.GenericRepo<RefreshToken>().FindAsync(rt => rt.UserId == rrt.UserId);
            //_unitOfWork.GenericRepo<RefreshToken>().DeleteRange(await alltokenforuser);
            _unitOfWork.GenericRepo<RefreshToken>().Delete(rt);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        public async Task<AuthBaseResponseDto> ForgetPasswordAsync(string email)
        {
            var User = await _unitOfWork.IUserRepository.GetUserByEmailAsync(email);
            if (User is null) return new AuthBaseResponseDto(false, "EmailNotFound");
            return await _emailVerification.GenerateOTPAsync(email);
        }
        public async Task<bool> ResetPassword(string email,string newpassword)
        {
            var user = await _unitOfWork.IUserRepository.GetUserByEmailAsync(email);
            var UserOtp = (await _unitOfWork.GenericRepo<OTP>().FindAsync(otp => otp.Email == email)).FirstOrDefault();
            if (user is null ||UserOtp is null || !UserOtp.IsUsed) return false;
            user.HashPassword = HashPassowrdHandler(newpassword);
            var oldotps = (await _unitOfWork.GenericRepo<OTP>().FindAsync(e => e.Email == email)).ToList();
            if (oldotps.Any())
            {
                _unitOfWork.GenericRepo<OTP>().DeleteRange(oldotps);
            }
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<AuthBaseResponseDto> UpdateProfileAsync(UpdateUserDto dto)
        {
            var user = await _unitOfWork.IUserRepository.GetUserByEmailAsync(dto.Email);
            if (user is null) return new AuthBaseResponseDto(false,"Invalid Email Please Loign Again");
            _mapper.Map(dto,user);
            _unitOfWork.GenericRepo<User>().Update(user);
            var newaccesToken = await _tokenService.CreateAccessTokenAsync(user);
            var newrefreshToken = _tokenService.CreateRefreshToken();
            var Oldtoken = await _unitOfWork.GenericRepo<RefreshToken>().FindAsync(t=>t.UserId == user.Id);
            _unitOfWork.GenericRepo<RefreshToken>().DeleteRange(Oldtoken);
            var refreshTokenEntity = new RefreshToken()
            {
                Token = newrefreshToken,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
                UserId = user.Id
            };
            await _unitOfWork.GenericRepo<RefreshToken>().AddAsync(refreshTokenEntity);
            var rResult = await _unitOfWork.SaveChangesAsync();
            if (rResult <= 0) return new AuthBaseResponseDto(false ,"Error During Update User");
            var ReturnedResponse = _mapper.Map<User, AuthBaseResponseDto>(user);
            ReturnedResponse.Success = true;
            ReturnedResponse.Message = "User Profile Updated Successfully";
            ReturnedResponse.AccessToken = newaccesToken;
            ReturnedResponse.RefreshToken = newrefreshToken;
            return ReturnedResponse;
        }
        public async Task<bool> ChangePassword(ChangePasswordDto dto,string Email)
        {
            var usr = await _unitOfWork.IUserRepository.GetUserByEmailAsync(Email);
            if (usr is null) return false;
            if(!VerifyHashedPassword(usr.HashPassword ,dto.CurrentPassword)) return false;
            if (dto.CurrentPassword == dto.Password) return false;
            usr.HashPassword = HashPassowrdHandler(dto.Password);
            _unitOfWork.GenericRepo<User>().Update(usr);
            var Result = await _unitOfWork.SaveChangesAsync();
            return Result > 0;

        }
        //public async Task<AuthBaseResponseDto?> RefreshTokenWithPrinciple(string token)
        //{
        //    var principal = _tokenService.GetUserPrincipal(token);
        //    var s = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        //    if (s != null) return new AuthBaseResponseDto(false,"invalid token");
        //    var usr = await _unitOfWork.IUserRepository.GetUserByEmailAsync(s);
        //    if (usr is null) return new AuthBaseResponseDto(false, "Please LoginAgain");
        //    var accesstoken = await _tokenService.CreateAccessTokenAsync(usr);
        //    var refreshtoken = _tokenService.CreateRefreshToken();
        //    var rtEntitiy = new RefreshToken()
        //    {
        //        Token = refreshtoken,
        //        ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
        //        UserId = usr.Id,
        //    };
        //    await _unitOfWork.GenericRepo<RefreshToken>().AddAsync(rtEntitiy);
        //    var Result = await _unitOfWork.SaveChangesAsync();
        //    if (Result <= 0) return null;
        //    var ReturnedResponse = _mapper.Map<User, AuthBaseResponseDto>(usr);
        //    ReturnedResponse.Success = true;
        //    ReturnedResponse.Message = "Access Token Generated Successfully";
        //    ReturnedResponse.AccessToken = accesstoken;
        //    ReturnedResponse.RefreshToken = refreshtoken;
        //    return ReturnedResponse;
        //}
    }
}
