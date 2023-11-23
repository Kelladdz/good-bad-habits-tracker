using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Domain.Interfaces
{
    public interface IHabitsRepository
    {
        Task<IEnumerable<Habit>> GetHabits();
        Task<Habit?> GetHabitById(Guid habitId);
        Task Create(Habit habit);
        Task Edit(Habit habit, HabitDto habitDto);
        Task Delete(Habit habit);
        Task DeleteAll();


    }
}
