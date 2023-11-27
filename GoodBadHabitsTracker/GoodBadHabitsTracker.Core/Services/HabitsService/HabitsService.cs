using AutoMapper;
using GoodBadHabitsTracker.Core.Domain.Interfaces;
using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Services.HabitsService
{
    public class HabitsService(IHabitsRepository habitsRepository, IMapper mapper) : IHabitsService
    {
        public async Task<IEnumerable<Habit>> GetHabits() => await habitsRepository.GetHabits();

        public async Task<Habit> GetHabitById(Guid habitId) => await habitsRepository.GetHabitById(habitId);

        public async Task<Habit> Create(HabitDto habitDto)
        {
            var habit = mapper.Map<Habit>(habitDto);
            if(habit.IsRepeatDaily == true && habit.RepeatDaysOfMonth.Length > 0)
                throw new Exception("If repeat is daily then days of month to repeat should be null.");
            if (habit.IsRepeatDaily == false && habit.RepeatDaysOfWeek.Length > 0)
                throw new Exception("If repeat is month then days of week to repeat should be null.");
            habit.GenerateId();
            await habitsRepository.Create(habit);
            return habit;
        }
        public async Task<Habit> Edit(Habit habit, HabitDto habitDto)
        {
            if (habit == null) throw new ArgumentNullException(nameof(habit));
            await habitsRepository.Edit(habit, habitDto);
            return habit;
        }
        public async Task Delete(Habit habit)
        {
            if (habit == null) throw new ArgumentNullException(nameof(habit));
            await habitsRepository.Delete(habit);
        }
        public async Task DeleteAll()
        {
            await habitsRepository.DeleteAll();
        }
    }
}
