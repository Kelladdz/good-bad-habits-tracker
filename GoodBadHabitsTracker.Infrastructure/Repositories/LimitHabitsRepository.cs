﻿using GoodBadHabitsTracker.Core.Interfaces;
using GoodBadHabitsTracker.Core.Models.Habit;
using GoodBadHabitsTracker.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Infrastructure.Repositories
{
    public class LimitHabitsRepository(HabitsDbContext dbContext) : ILimitHabitsRepository
    {
        public async Task<bool> CreateAsync(LimitHabit habit, CancellationToken cancellationToken)
        {
            await dbContext.LimitHabit.AddAsync(habit, cancellationToken);
            return await dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
