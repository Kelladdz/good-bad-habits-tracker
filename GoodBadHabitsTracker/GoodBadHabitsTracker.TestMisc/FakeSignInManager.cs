using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using Google.Apis.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.TestMisc
{
    public class FakeSignInManager : SignInManager<ApplicationUser> 
    {
        public FakeSignInManager()
            : base(new Mock<UserManager<ApplicationUser>>().Object,
                  new HttpContextAccessor(),
                  new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                  new Mock<IAuthenticationSchemeProvider>().Object,
                  new Mock<IUserConfirmation<ApplicationUser>>().Object
                  )
        {}
    }
}
