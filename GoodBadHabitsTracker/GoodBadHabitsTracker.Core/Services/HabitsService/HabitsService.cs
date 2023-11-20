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
        public async Task Create(HabitDto habitDto)
        {
            var habit = _mapper.Map<Habit>(habitDto);
            habit.GenerateId();
            await _habitsRepository.Create(habit);
        }
    }
}
