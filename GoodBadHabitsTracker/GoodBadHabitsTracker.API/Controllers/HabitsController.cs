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
        public async Task<IActionResult> Create([FromBody]HabitDto habitDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(habitDto == null)
            {
                return NotFound();
            }
            var habit = await _habitsService.Create(habitDto);
            return CreatedAtAction(nameof(Create), new {habitId = habit.HabitId}, habit); //na później
        }
    }
}
