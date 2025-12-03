using AUTH.API.Request;
using CORE.DTOs;
using CORE.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AUTH.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailVerificationService _emailVerification;

        public AuthController(IAuthService authService,IEmailVerificationService emailVerification)
        {
            _authService = authService;
            _emailVerification = emailVerification;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<AuthBaseResponseDto>> Register(UserRegisterDto dto)
        {
            var responeDto = await _authService.Register(dto);
            if (responeDto.Success == true) return Ok(responeDto);
            return BadRequest(responeDto);
        }
        [HttpPost("Login")]
        public async Task<ActionResult<AuthBaseResponseDto>> Login(UserLoginDto dto)
        {
            var responseDto = await _authService.Login(dto);
            if (responseDto.Success == true) return Ok(responseDto);
            return Unauthorized(responseDto);
        }
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<AuthBaseResponseDto>> RefreshToken([FromBody] RTokenDto dto)
        {
            var responseDto = await _authService.RefreshToken(dto.Token);
            if (responseDto.Success == true) return Ok(responseDto);
            return Unauthorized(responseDto);
        }
        [HttpPost("LogOut")]
        public async Task<ActionResult> LogOut([FromBody] RTokenDto dto)
        {
            var result = await _authService.LogOut(dto.Token);
            if (result) return Ok(new { Success = true, Message = "Logged Out Successfully" });
            return BadRequest(new { Success = false, Message = "Error During LogOut" });
        }

        [HttpPost("SendOTP")]
        public async Task<ActionResult<AuthBaseResponseDto>> SendOtp(REmailDto dto)
        {
            var responseDto = await _emailVerification.GenerateOTPAsync(dto.Email);
            if (responseDto.Success) return Ok(responseDto);
            return BadRequest(responseDto);
        }
        [HttpPost("VerifiyEmail")]
        public async Task<ActionResult<AuthBaseResponseDto>> VerifiyEmail(ROtpVerifiyDto dto)
        {
            var Result = await _emailVerification.OTPEmailVerifiyAsync(dto.Email, dto.Otp);
            if (Result) return Ok(new AuthBaseResponseDto(true,"Email Verified Successfully"));
            return BadRequest(new AuthBaseResponseDto(false,"Invalid OTP or OTP Expired"));
        }
        [HttpPost("ForgetPassword")]
        public async Task<ActionResult> ForgetPassword(REmailDto dto)
        {
            var response = await _authService.ForgetPasswordAsync(dto.Email);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }
        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword(RResetPassword dto)
        {
            var respnse = await _authService.ResetPassword(dto.Email, dto.NewPassword);
            if (!respnse) return BadRequest(new { Success = false, Message = "Error During Reseting Password" });
            return Ok(new { Success = true, Message = "Password Reseted Successfully" });
        }
        [HttpPost("OtpVerify")]
        public async Task<ActionResult<bool>> OtpVerify(ROtpVerifiyDto dto)
        => await _emailVerification.OtpVerifyAsync(dto.Otp,dto.Email);


        [Authorize]
        [HttpGet("Test")]
        public ActionResult Test()
        {
            return Ok("Auth Service is working fine");
        }
    }
}
