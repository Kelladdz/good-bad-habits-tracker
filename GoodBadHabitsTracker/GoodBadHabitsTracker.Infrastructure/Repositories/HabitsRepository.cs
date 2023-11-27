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
    public class HabitsRepository(HabitsDbContext dbContext) : IHabitsRepository
    {
        public async Task<IEnumerable<Habit>> GetHabits(Guid userId) => await dbContext.Habits.Where(x => x.UserId == userId).ToListAsync();
        public async Task<Habit?> GetHabitById(Guid habitId) => await dbContext.Habits.FindAsync(habitId);
        public async Task Create(Habit habit)
        {
            var user = dbContext.Users.Find(habit.UserId);
            habit.User = user;
            dbContext.Habits.Add(habit);
            await dbContext.SaveChangesAsync();
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

            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(Habit habit)
        {
            dbContext.Habits.Remove(habit);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeleteAll(Guid userId)
        {
            var habits = await GetHabits(userId);
            dbContext.Habits.RemoveRange(habits);
            await dbContext.SaveChangesAsync();
        }
    }
}
