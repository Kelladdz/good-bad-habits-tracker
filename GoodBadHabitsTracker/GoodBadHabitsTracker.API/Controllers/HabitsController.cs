using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Core.Services.HabitsService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoodBadHabitsTracker.API.Controllers
{
    [Route("API")]
    [ApiController]
    public class HabitsController : ControllerBase
    {
        private readonly IHabitsService _habitsService;

        public HabitsController(IHabitsService habitsService)
        {
            _habitsService = habitsService;
        }
        [HttpPost("habits")]
        public async Task<ActionResult> Create(Habit habit)
        {
            await _habitsService.Create(habit);
            return RedirectToAction(nameof(Create));
        }
    }
}
