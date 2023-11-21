using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Services.HabitsService
{
    public interface IHabitsService
    {
        Task<IEnumerable<Habit>> GetHabits();
        Task<Habit> Create(HabitDto habitDto);
    }
}
