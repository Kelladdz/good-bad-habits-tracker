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
    public class HabitsService : IHabitsService
    {
        private readonly IHabitsRepository _habitsRepository;
        private readonly IMapper _mapper;

        public HabitsService(IHabitsRepository habitsRepository, IMapper mapper)
        {
            _habitsRepository = habitsRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<Habit>> GetHabits() => await _habitsRepository.GetHabits();

        public async Task<Habit> GetHabitById(Guid habitId) => await _habitsRepository.GetHabitById(habitId);

        public async Task<Habit> Create(HabitDto habitDto)
        {
            var habit = _mapper.Map<Habit>(habitDto);
            if(habit.IsRepeatDaily == true && habit.RepeatDaysOfMonth != null)
                throw new Exception("If repeat is daily then days of month to repeat should be null.");
            if (habit.IsRepeatDaily == false && habit.RepeatDaysOfWeek != null)
                throw new Exception("If repeat is month then days of week to repeat should be null.");
            habit.GenerateId();
            await _habitsRepository.Create(habit);
            return habit;
        }
        public async Task<Habit> Edit(Habit habit)
        {
            if (habit == null) throw new ArgumentNullException(nameof(habit));
            await _habitsRepository.Edit(habit);
            return habit;
        }
    }
}
