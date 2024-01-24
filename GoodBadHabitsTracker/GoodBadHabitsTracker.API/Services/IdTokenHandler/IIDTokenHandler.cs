using System.Security.Claims;

namespace GoodBadHabitsTracker.API.Services.IdTokenHandler
{
    public interface IIDTokenHandler
    {
        public ClaimsPrincipal GetClaimsPrincipalFromIdToken(string idToken);
    }
}
