using GoodBadHabitsTracker.Core.Domain.Interfaces;
using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Core.DTOs;
using GoodBadHabitsTracker.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Infrastructure.Repositories
{
    public class HabitsRepository : IHabitsRepository
    {
        private readonly HabitsDbContext _dbContext;

        public HabitsRepository(HabitsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Habit>> GetHabits() => await _dbContext.Habits.ToListAsync();

        public async Task<Habit?> GetHabitById(Guid habitId) => await _dbContext.Habits.FindAsync(habitId);

        public async Task Create(Habit habit)
        {
            _dbContext.Habits.Add(habit);
            await _dbContext.SaveChangesAsync();
        }
        public async Task Edit(Habit habit, HabitDto habitDto)
        {
            /*var existingHabit = await _dbContext.Habits.FirstOrDefaultAsync(h => h.HabitId == habit.HabitId);*/
            if (habitDto == null) return;

            habit.Name = habitDto.Name;
            habit.IsGood = habitDto.IsGood;
            habit.IsGoalInTime = habitDto.IsGoalInTime;
            habit.Quantity = habitDto.Quantity;
            habit.Frequency = habitDto.Frequency;
            habit.IsRepeatDaily = habitDto.IsRepeatDaily;
            habit.RepeatDaysOfWeek = habitDto.RepeatDaysOfWeek;
            habit.RepeatDaysOfMonth = habitDto.RepeatDaysOfMonth;
            habit.StartDate = habitDto.StartDate;
            habit.ReminderTime = habitDto.ReminderTime;

            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Habit habit)
        {
            _dbContext.Habits.Remove(habit);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteAll()
        {
            var habits = await GetHabits();
            _dbContext.Habits.RemoveRange(habits);
            await _dbContext.SaveChangesAsync();
        }
    }
}
