using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GoodBadHabitsTracker.Infrastructure.Services.JwtTokenGenerator
{
    public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
    {
        public string GenerateJwtToken(UserSession userSession)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt:Key").Value!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var userFingerprintHash = GenerateUserFingerprint();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userSession.Id.ToString()!),
                    new Claim(JwtRegisteredClaimNames.Name, userSession.Name!),
                    new Claim(JwtRegisteredClaimNames.Email, userSession.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("role", userSession.Role!),
                    new Claim("userFingerprint", userFingerprintHash )
                }),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = configuration.GetSection("Jwt:Issuer").Value!,
                Audience = configuration.GetSection("Jwt:Audience").Value!,
                SigningCredentials = credentials
            };

            /*var claims = new List<Claim>
            {
                new Claim("sub", userSession.Id.ToString()!),
                new Claim("name", userSession.Name!),
                new Claim("email", userSession.Email!),
                new Claim("role", userSession.Role!),
                new Claim("userFingerprint", userFingerprintHash )
            };

            var token = new JwtSecurityToken(
                issuer: configuration.GetSection("Jwt:Issuer").Value!,
                audience: configuration.GetSection("Jwt:Audience").Value!,
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                notBefore: DateTime.Now,
                signingCredentials: credentials
                );*/
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            return jwtToken;
        }

        public string GenerateUserFingerprint()
        {
            string userFingerprint;
            using (var secureRandom = RandomNumberGenerator.Create())
            {
                var randomString = new byte[50];
                secureRandom.GetBytes(randomString);
                userFingerprint = BitConverter.ToString(randomString).Replace("-", "");

            }

            string userFingerprintHash;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] userFingerprintDigest = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(userFingerprint));
                userFingerprintHash = BitConverter.ToString(userFingerprintDigest).Replace("-", "");
            }
            return userFingerprintHash;
        }
    }
}
