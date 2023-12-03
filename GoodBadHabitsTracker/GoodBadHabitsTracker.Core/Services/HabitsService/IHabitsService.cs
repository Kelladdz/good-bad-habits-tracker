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
        Task<PagedHabitsResult> GetHabits(string? term, int page, int limit, DateOnly date, Guid userId);
        Task<Habit> GetHabitById(Guid habitId);
        Task<Habit> Create(HabitDto habitDto, Guid userId);
        Task<Habit> Edit(Habit habit, HabitDto habitDto);
        Task Delete(Habit habit);
        Task DeleteAll(Guid userId);
    }
}
