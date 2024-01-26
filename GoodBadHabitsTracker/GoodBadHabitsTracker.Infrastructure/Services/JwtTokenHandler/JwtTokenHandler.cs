using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GoodBadHabitsTracker.Infrastructure.Services.JwtTokenHandler
{
    public class JwtTokenHandler(IConfiguration configuration) : IJwtTokenHandler
    {
        public string GenerateAccessToken(UserSession userSession, out string userFingerprint)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt:Key").Value!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            userFingerprint = GenerateUserFingerprint();
            var userFingerprintHash = GenerateUserFingerprintHash(userFingerprint);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, userSession.Id.ToString()!),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Name, userSession.Name!),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, userSession.Email!),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("role", userSession.Role!),
                    new Claim("userFingerprint", userFingerprintHash )
                }),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = configuration.GetSection("Jwt:Issuer").Value!,
                Audience = configuration.GetSection("Jwt:Audience").Value!,
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            return jwtToken;
        }

        public string GenerateRefreshToken()
        {
            string refreshToken;
            var randomString = new byte[32];
            using (var secureRandom = RandomNumberGenerator.Create())
            {  
                secureRandom.GetBytes(randomString);
                refreshToken = Convert.ToBase64String(randomString);
            }

            return refreshToken;
        }

        public string GenerateUserFingerprint()
        {
            string userFingerprint;
            var randomString = new byte[32];
            using (var secureRandom = RandomNumberGenerator.Create())
            {
                secureRandom.GetBytes(randomString);
                userFingerprint = Convert.ToBase64String(randomString);
            }

            return userFingerprint;
        }

        public string GenerateUserFingerprintHash(string userFingerprint)
        {
            string userFingerprintHash;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] userFingerprintDigest = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(userFingerprint));
                userFingerprintHash = Convert.ToBase64String(userFingerprintDigest);
            }

            return userFingerprintHash;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration.GetSection("Jwt:Issuer").Value!,
                ValidateAudience = true,
                ValidAudience = configuration.GetSection("Jwt:Audience").Value!,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt:Key").Value!)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            return principal;
        }
    }
}
