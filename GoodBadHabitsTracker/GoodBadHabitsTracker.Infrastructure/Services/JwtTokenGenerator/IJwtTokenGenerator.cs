using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using Microsoft.IdentityModel.Tokens;

namespace GoodBadHabitsTracker.Infrastructure.Services.JwtTokenGenerator
{
    public interface IJwtTokenGenerator
    {
        public string GenerateJwtToken(UserSession userSession);
        public string GenerateUserFingerprint();
    }
}
