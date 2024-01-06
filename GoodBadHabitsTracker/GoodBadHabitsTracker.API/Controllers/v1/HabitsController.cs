using Asp.Versioning;
using AutoMapper;
using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Core.DTOs;
using GoodBadHabitsTracker.Core.Services.HabitsService;
using GoodBadHabitsTracker.Core.Services.UserAccessor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoodBadHabitsTracker.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class HabitsController(IHabitsService habitsService, IMapper mapper, IUserAccessor userAccessor) : CustomControllerBase
    {
        /// <summary>
        /// To get list of habits from "Habits" table.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        
        public async Task<IActionResult> GetHabits(string? term, DateOnly date, int page = 1, int limit = 10)
        {
            var userId = userAccessor.GetLoggedUserId();
            PagedHabitsResult habits = await habitsService.GetHabits(term, page, limit, date, userId);
            if (habits.TotalCount == 0) return NotFound();
            return Ok(habits);
        }
        /// <summary>
        /// To get habit by Id.
        /// </summary>
        /// <param name="habitId"></param>
        /// <returns></returns>
        [HttpGet("{habitId}")]
        public async Task<ActionResult<Habit>> GetHabitById(Guid habitId)
        {
            var habit = await habitsService.GetHabitById(habitId);
            if (habit == null) return NotFound();
            return Ok(habit);
        }
        /// <summary>
        /// To create a new habit.
        /// </summary>
        /// <param name="habitDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HabitDto habitDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (habitDto == null) return NotFound();
            var userId = userAccessor.GetLoggedUserId();
            var habit = await habitsService.Create(habitDto, userId);
            return CreatedAtAction(nameof(GetHabits), new { habitId = habit.HabitId }, habit); //na później
        }
        /// <summary>
        /// To edit a specific habit and save changes.
        /// </summary>
        /// <param name="habitDto"></param>
        /// <param name="habitId"></param>
        /// <returns></returns>
        [HttpPut("{habitId}")]
        public async Task<IActionResult> Edit([FromBody] HabitDto habitDto, Guid habitId)
        {
            var habitResponse = await habitsService.GetHabitById(habitId);
            if (habitResponse == null) return NotFound();
            await habitsService.Edit(habitResponse, habitDto);
            return NoContent();
        }
        /// <summary>
        /// To delete a habit from database.
        /// </summary>
        /// <param name="habitId"></param>
        /// <returns></returns>
        [HttpDelete("{habitId}")]
        public async Task<IActionResult> Delete(Guid habitId)
        {
            var habitResponse = await habitsService.GetHabitById(habitId);

            if (habitResponse == null) return NotFound();

            await habitsService.Delete(habitResponse);
            return NoContent();
        }
        /// <summary>
        /// To delete all habits from database.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            var userId = userAccessor.GetLoggedUserId();
            await habitsService.DeleteAll(userId);
            return NoContent();
        }
    }
}
