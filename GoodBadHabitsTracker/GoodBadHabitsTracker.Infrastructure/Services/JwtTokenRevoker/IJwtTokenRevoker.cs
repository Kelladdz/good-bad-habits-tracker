using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Infrastructure.Services.JwtTokenRevoker
{
    public interface IJwtTokenRevoker
    {
        public bool ValidateTokenLifetime(DateTime? notBefore, 
            DateTime? expirationDate, SecurityToken securityToken, TokenValidationParameters tokenValidationParameters);

        public Task SignOutAsync(SecurityToken securityToken);
    }
}
