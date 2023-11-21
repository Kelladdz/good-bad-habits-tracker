using GoodBadHabitsTracker.Core.Domain.Interfaces;
using GoodBadHabitsTracker.Core.Domain.Models;
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
        public async Task Edit(Habit habit)
        {
            var existingHabit = await _dbContext.Habits.FirstOrDefaultAsync(h => h.HabitId == habit.HabitId);
            if (existingHabit == null) return;

            existingHabit.Name = habit.Name;
            existingHabit.IsGood = habit.IsGood;
            existingHabit.IsGoalInTime = habit.IsGoalInTime;
            existingHabit.Quantity = habit.Quantity;
            existingHabit.Frequency = habit.Frequency;
            existingHabit.IsRepeatDaily = habit.IsRepeatDaily;
            existingHabit.RepeatDaysOfWeek = habit.RepeatDaysOfWeek;
            existingHabit.RepeatDaysOfMonth = habit.RepeatDaysOfMonth;
            existingHabit.StartDate = habit.StartDate;
            existingHabit.ReminderTime = habit.ReminderTime;

            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Habit habit)
        {
            _dbContext.Habits.Remove(habit);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteAll(IEnumerable<Habit> habits)
        {
            foreach(var habit in  habits)
            {
                _dbContext.Habits.Remove(habit);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
