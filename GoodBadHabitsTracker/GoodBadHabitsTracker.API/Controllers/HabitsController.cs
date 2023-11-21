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
        [HttpGet("habits")]
        public async Task<ActionResult<IEnumerable<Habit>>> GetHabits()
        {
            IEnumerable<Habit> habits = await _habitsService.GetHabits();
            if (!habits.Any()) return NotFound();
            return Ok(habits);
        }
        [HttpGet("habits/{habitId}")]
        public async Task<ActionResult<Habit>> GetHabitById(Guid habitId)
        {
            var habit = await _habitsService.GetHabitById(habitId);
            if(habit == null) return NotFound();
            return Ok(habit);
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
            return CreatedAtAction(nameof(GetHabits), new {habitId = habit.HabitId}, habit); //na później
        }
        [HttpPut("habits/{habitId}")]
        public async Task<IActionResult> Edit(Guid habitId)
        {
            var habitResponse = await _habitsService.GetHabitById(habitId);
            if(habitResponse == null)
            {
                return NotFound();
            }
            await _habitsService.Edit(habitResponse);
            return NoContent();
        }
    }
}
