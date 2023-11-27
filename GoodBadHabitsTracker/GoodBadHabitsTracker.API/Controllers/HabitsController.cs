using AutoMapper;
using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Core.DTOs;
using GoodBadHabitsTracker.Core.Services.HabitsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoodBadHabitsTracker.API.Controllers
{
    [Route("API")]
    [ApiController]
    public class HabitsController(IHabitsService habitsService, IMapper mapper) : ControllerBase
    {
        [HttpGet("habits")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Habit>>> GetHabits()
        {
            var user = User;
            IEnumerable<Habit> habits = await habitsService.GetHabits();
            if (!habits.Any()) return NotFound();
            return Ok(habits);
        }
        [HttpGet("habits/{habitId}")]
        public async Task<ActionResult<Habit>> GetHabitById(Guid habitId)
        {
            var habit = await habitsService.GetHabitById(habitId);
            if(habit == null) return NotFound();
            return Ok(habit);
        }
        [HttpPost("habits")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody]HabitDto habitDto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            if (habitDto == null) return NotFound();
            var habit = await habitsService.Create(habitDto);
            return CreatedAtAction(nameof(GetHabits), new {habitId = habit.HabitId}, habit); //na później
        }
        [HttpPut("habits/{habitId}")]
        public async Task<IActionResult> Edit([FromBody]HabitDto habitDto, Guid habitId)
        {
            var habitResponse = await habitsService.GetHabitById(habitId);

            if(habitResponse == null) return NotFound();
            await habitsService.Edit(habitResponse, habitDto);
            return NoContent();
        }
        [HttpDelete("habits/{habitId}")]
        public async Task<IActionResult> Delete(Guid habitId)
        {
            var habitResponse = await habitsService.GetHabitById(habitId);

            if (habitResponse == null) return NotFound();

            await habitsService.Delete(habitResponse);
            return NoContent();
        }
        [HttpDelete("habits")]
        public async Task<IActionResult> DeleteAll()
        {
            IEnumerable<Habit> habits = await habitsService.GetHabits();
            if (!habits.Any()) return NotFound();
            await habitsService.DeleteAll();
            return NoContent();
        }
    }
}
