﻿using AutoMapper;
using GoodBadHabitsTracker.Application.DTOs.Habit.Response;
using GoodBadHabitsTracker.Application.Exceptions;
using GoodBadHabitsTracker.Core.Interfaces;
using GoodBadHabitsTracker.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Application.Commands.Habit.LimitHabit.Create
{
    internal sealed class CreateLimitHabitCommandHandler(IMapper mapper, IHttpContextAccessor httpContextAccessor, ILimitHabitsRepository limitHabitsRepository) : IRequestHandler<CreateLimitHabitCommand, LimitHabitResponse>
    {
        public async Task<LimitHabitResponse> Handle(CreateLimitHabitCommand command, CancellationToken cancellationToken)
        {
            var habit = mapper.Map<Core.Models.Habit.LimitHabit>(command.Request);
            habit.UserId = Guid.Parse("83166a55-c2f1-44a5-2a39-08dc8bf473f7"); //TO CHANGE

            if (!await limitHabitsRepository.CreateAsync(habit, cancellationToken))
                throw new AppException(System.Net.HttpStatusCode.BadRequest, "Failed to create habit");

            return new LimitHabitResponse(habit);
        }
    }
}
