using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GoodBadHabitsTracker.Infrastructure.Services.IdTokenHandler
{
    public class IdTokenHandler : IIDTokenHandler
    {
        public ClaimsPrincipal GetClaimsPrincipalFromIdToken(string idToken)
        {
            if (string.IsNullOrEmpty(idToken)) return null;

            var handler = new JwtSecurityTokenHandler();
            var decodedIdToken = handler.ReadJwtToken(idToken);
            var claims = decodedIdToken.Claims;
            var claimsIdentity = new ClaimsIdentity(claims, "Google");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return claimsPrincipal;
        }
    }
}
