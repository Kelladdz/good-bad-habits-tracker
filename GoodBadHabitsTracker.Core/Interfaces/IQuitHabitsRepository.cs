﻿using GoodBadHabitsTracker.Core.Models.Habit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Interfaces
{
    public interface IQuitHabitsRepository
    {
        Task<bool> CreateAsync(QuitHabit quitHabit, CancellationToken cancellationToken);
    }
}
