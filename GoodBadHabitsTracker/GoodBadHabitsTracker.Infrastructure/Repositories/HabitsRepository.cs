using GoodBadHabitsTracker.Core.Domain.Interfaces;
using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Core.DTOs;
using GoodBadHabitsTracker.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace GoodBadHabitsTracker.Infrastructure.Repositories
{
    public class HabitsRepository(HabitsDbContext dbContext) : IHabitsRepository
    {
        public async Task<PagedHabitsResult> GetHabits(string? term, int page, int limit, DateOnly date, Guid userId)
        {
            /*IQueryable<Habit> habits = await dbContext.Habits.Where(x => x.UserId == userId).ToListAsync();
            IQueryable<Habit> habitsQueryable = habits.AsQueryable();
            return habitsQueryable.Take(habitsQueryable.Count());*/
            IQueryable<Habit> habits;

            if (string.IsNullOrWhiteSpace(term)) habits = dbContext.Habits.Where(h => h.UserId == userId && h.RepeatDaysOfWeek.Contains(date.DayOfWeek.ToString().ToLower()));
            else
            {
                term = term.Trim().ToLower();
                habits = dbContext.Habits.Where(h => h.Name!.ToLower().Contains(term) && h.UserId == userId && h.RepeatDaysOfWeek.Contains(date.DayOfWeek.ToString().ToLower()));

            }
            var totalCount = await dbContext.Habits.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)limit);
            var pagedHabits = await habits.Skip((page - 1) * limit).Take(limit).ToListAsync();

            var pagedHabitsData = new PagedHabitsResult()
            {
                Habits = pagedHabits,
                TotalCount = totalCount,
                TotalPages = totalPages,
            };
            return pagedHabitsData;
        }

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
            var habits = await dbContext.Habits.Where(x => x.UserId == userId).ToListAsync();
            dbContext.Habits.RemoveRange(habits);
            await dbContext.SaveChangesAsync();
        }
    }
}
