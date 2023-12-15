using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using GoodBadHabitsTracker.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace GoodBadHabitsTracker.API.Controllers
{
    [Route("API/[controller]")]
    [ApiController]
    public class UsersController(UserManager<ApplicationUser> userManager) : ControllerBase
    {
        [HttpGet("{userId}", Name = "GetUserById")]
        [ActionName(nameof(GetUserById))]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = userManager.FindByIdAsync(userId.ToString());
            return Ok(user);
        }
    }
}
