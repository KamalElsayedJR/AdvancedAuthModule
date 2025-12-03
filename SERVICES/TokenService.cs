using CORE.Entities;
using CORE.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using REPOSITORY.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SERVICES
{
    public  class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> CreateAccessTokenAsync(User user)
        {
            var AuthClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim("EmailVerifiy",user.IsVerified.ToString()),
                new Claim("FirstName",user.FirstName),
                new Claim("LastName",user.LastName),
            };
            var Authkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTConfig:SecretKey"]));
            var Token = new JwtSecurityToken(
                    issuer: _configuration["JWTConfig:Issuer"],
                    audience: _configuration["JWTConfig:Audience"],
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWTConfig:DurationInMinutes"])),
                    claims: AuthClaims,
                    signingCredentials: new SigningCredentials(Authkey,SecurityAlgorithms.HmacSha256Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
        public string CreateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }


    }
}
