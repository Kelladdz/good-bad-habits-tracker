using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using GoodBadHabitsTracker.Core.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Infrastructure.Persistance
{
    public class HabitsDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public HabitsDbContext(DbContextOptions<HabitsDbContext> options) : base(options)
        {
            
        }

        public HabitsDbContext()
        {
            
        }

        public virtual DbSet<Habit> Habits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
