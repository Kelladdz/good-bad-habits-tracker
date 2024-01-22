using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GoodBadHabitsTracker.API.Services
{
    public class IdTokenHandler
    {
        public ClaimsPrincipal GetClaimsPrincipalFromIdToken(string idToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var decodedIdToken = handler.ReadJwtToken(idToken);
            var claims = decodedIdToken.Claims;
            var claimsIdentity = new ClaimsIdentity(claims, "Google");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return claimsPrincipal;
        }
    }
}
