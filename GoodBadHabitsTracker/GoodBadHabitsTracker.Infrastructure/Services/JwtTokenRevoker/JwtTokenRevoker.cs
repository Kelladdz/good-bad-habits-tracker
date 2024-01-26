using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using GoodBadHabitsTracker.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Infrastructure.Services.JwtTokenRevoker
{
    public class JwtTokenRevoker : IJwtTokenRevoker
    {
        public bool ValidateTokenLifetime(DateTime? notBefore, DateTime? expirationDate, SecurityToken securityToken, TokenValidationParameters tokenValidationParameters)
        {
            using (var dbContext = new HabitsDbContext(new DbContextOptions<HabitsDbContext>()))
            {
                var isValid = (securityToken is JsonWebToken token &&
                token.ValidFrom <= DateTime.UtcNow &&
                token.ValidTo >= DateTime.UtcNow &&
                dbContext.RevokedTokens.Any(rt => rt.JwtTokenDigest == token.EncodedToken) is false);

                return isValid;
            }
        }
        public async Task SignOutAsync(SecurityToken securityToken)
        {
            if (securityToken is JsonWebToken token)
            {
                using (var dbContext = new HabitsDbContext(new DbContextOptions<HabitsDbContext>()))
                {
                    dbContext.RevokedTokens.Add(new RevokedToken() { JwtTokenDigest = token.EncodedToken, RevokedAt = token.ValidTo });
                    await dbContext.SaveChangesAsync();
                }
                
            }

            using (var dbContext = new HabitsDbContext(new DbContextOptions<HabitsDbContext>()))
            {
                var expiredTokens = dbContext.RevokedTokens.Where(jwt => jwt.RevokedAt < DateTime.UtcNow).ToList();
                dbContext.RevokedTokens.RemoveRange(expiredTokens);
                await dbContext.SaveChangesAsync();
            }        
        }
    }
}
