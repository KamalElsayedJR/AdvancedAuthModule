using AUTH.API.Request;
using AUTH.API.Response;
using CORE.DTOs;
using CORE.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize]
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<AuthBaseResponseDto>> RefreshToken([FromBody] RTokenDto dto)
        {
            var responseDto = await _authService.RefreshToken(dto.Token);
            if (responseDto.Success == true) return Ok(responseDto);
            return Unauthorized(responseDto);
        }
        [Authorize]
        [HttpPost("LogOut")]
        public async Task<ActionResult> LogOut([FromBody] RTokenDto dto)
        {
            var result = await _authService.LogOutAsync(dto.Token);
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
            var Result = await _emailVerification.OTPEmailVerifiyAsync(dto.Email, dto.Opt);
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
        public async Task<ActionResult<bool>> OtpVerify(OtpVeryfiyDto dto)
        => await _emailVerification.OtpVerifyAsync(dto.otp,dto.Email);
        [Authorize]
        [HttpGet("Me")]
        public async Task<ActionResult<UserDto>> Profile()
        {
            var Email = User.Claims.FirstOrDefault(C=>C.Type == ClaimTypes.Email)?.Value;
            if (Email == null) return Unauthorized();
            return new UserDto() 
            { 
                Email = Email,
                DisplayName = User.Claims.FirstOrDefault(C => C.Type.ToString() == "DisplayName")?.Value,
                Verified = User.Claims.FirstOrDefault(C => C.Type.ToString() == "EmailVerifiy")?.Value,
                Roles = (List<string>) User.Claims.Where(C => C.Type.ToString() == ClaimTypes.Role)
            };
        }
        [Authorize]
        [HttpPut("Me")]
        public async Task<ActionResult<AuthBaseResponseDto>> Profile(UpdateUserDto dto)
        {
            if (dto.FirstName == null)
            {
                dto.FirstName = User.Claims.FirstOrDefault(C=>C.Type.ToString() == "FirstName")?.Value;
            }
            if (dto.LastName == null)
            {
                dto.LastName = User.Claims.FirstOrDefault(C => C.Type.ToString() == "LastName")?.Value;
            }
            if (dto.Email == null)
            {
                dto.Email = User.Claims.FirstOrDefault(C => C.Type == ClaimTypes.Email)?.Value;
            }
            var Respone = await _authService.UpdateProfileAsync(dto);
            if (Respone.Success) return Ok(Respone);
            return BadRequest(Respone);
        }
        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<ActionResult<bool>> ChanagePassword(ChangePasswordDto dto)
        {
            var email = User.Claims.FirstOrDefault(C=>C.Type == ClaimTypes.Email)?.Value;
            if (email == null) return Unauthorized();
            var responseResult = await _authService.ChangePassword(dto, email);
            if (responseResult) return Ok(true);
            return BadRequest(false);
        }
        [Authorize]
        [HttpGet("gettoken")]
        public IActionResult GetMyToken()
        {
            // ناخد الهيدر
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader))
                return BadRequest("No Authorization header found");

            // التوكن عادة بييجي بالشكل "Bearer <token>"
            var token = authHeader.StartsWith("Bearer ") ? authHeader["Bearer ".Length..] : authHeader;

            return Ok(new { token });
        }

    }
}
