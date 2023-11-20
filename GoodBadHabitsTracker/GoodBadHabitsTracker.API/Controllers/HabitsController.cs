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
        public async Task<ActionResult<Habit>> Create([FromBody]HabitDto habitDto)
        {
            if(!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            if(habitDto == null)
            {
                return Problem("Entity is null.");
            }
            await _habitsService.Create(habitDto);
            return Ok(); //na później
        }
    }
}
