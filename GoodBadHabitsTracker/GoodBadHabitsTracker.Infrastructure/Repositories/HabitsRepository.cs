using GoodBadHabitsTracker.Core.Domain.Interfaces;
using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Infrastructure.Persistance;
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

        public async Task Create(Habit habit)
        {
            _dbContext.Add(habit);
            await _dbContext.SaveChangesAsync();
        }
    }
}
