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
        private readonly IUnitOfWork _uow;

        public TokenService(IConfiguration configuration,IUnitOfWork uow)
        {
            _configuration = configuration;
            _uow = uow;
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
            var userRoles = _uow.IRolesRepository.GetRoleForUser(user.Id).ToList();
            foreach (var role in userRoles)
            {
                AuthClaims.Add(new Claim(ClaimTypes.Role, role.Role.Role));
            }
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

        public ClaimsPrincipal GetUserPrincipal(string token)
        {
            var VaildationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = _configuration["JWTConfig:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JWTConfig:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTConfig:SecretKey"])),
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, VaildationParameters, out SecurityToken securityToken);
            return principal;
        }
    }
}
