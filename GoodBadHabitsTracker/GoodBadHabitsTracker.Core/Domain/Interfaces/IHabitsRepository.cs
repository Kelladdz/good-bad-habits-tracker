using GoodBadHabitsTracker.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Domain.Interfaces
{
    public interface IHabitsRepository
    {
        Task Create(Habit habit);
    }
}
