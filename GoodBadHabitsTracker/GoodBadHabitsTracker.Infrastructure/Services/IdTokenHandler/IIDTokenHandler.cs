using System.Security.Claims;

namespace GoodBadHabitsTracker.Infrastructure.Services.IdTokenHandler
{
    public interface IIDTokenHandler
    {
        public ClaimsPrincipal GetClaimsPrincipalFromIdToken(string idToken);
    }
}
