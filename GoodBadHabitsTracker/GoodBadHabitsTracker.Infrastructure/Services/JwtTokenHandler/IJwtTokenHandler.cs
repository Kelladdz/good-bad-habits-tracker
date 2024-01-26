using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace GoodBadHabitsTracker.Infrastructure.Services.JwtTokenHandler
{
    public interface IJwtTokenHandler
    {
        public string GenerateAccessToken(UserSession userSession, out string userFingerprint);
        public string GenerateRefreshToken();
        public string GenerateUserFingerprint();
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
