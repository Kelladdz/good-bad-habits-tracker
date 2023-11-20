using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Core.DTOs;
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
        public async Task<ActionResult> Create(HabitDto habit)
        {
            await _habitsService.Create(habit);
            return RedirectToAction(nameof(Create)); //na później
        }
    }
}
