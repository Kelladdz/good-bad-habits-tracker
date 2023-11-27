using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Services.UserAccessor
{
    public class UserAccessor(IHttpContextAccessor httpContextAccessor) : IUserAccessor
    {
        public Guid GetLoggedUserId() => Guid.Parse(httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x =>
            x.Type == ClaimTypes.NameIdentifier)?.Value);
    }
}
