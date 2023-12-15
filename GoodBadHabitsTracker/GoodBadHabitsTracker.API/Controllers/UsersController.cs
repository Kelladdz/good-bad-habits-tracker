using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using GoodBadHabitsTracker.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace GoodBadHabitsTracker.API.Controllers
{
    [Route("API/[controller]")]
    public class UsersController(UserManager<ApplicationUser> userManager) : ControllerBase
    {

    }
}
