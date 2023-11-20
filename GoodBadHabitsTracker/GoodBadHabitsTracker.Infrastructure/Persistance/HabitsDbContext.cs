using GoodBadHabitsTracker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Infrastructure.Persistance
{
    public class HabitsDbContext : DbContext
    {
        public HabitsDbContext(DbContextOptions<HabitsDbContext> options) : base(options)
        {
            
        }

        public HabitsDbContext()
        {
            
        }

        public DbSet<Habit> Habits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Habit>().ToTable("Habits");
            string habitJson = System.IO.File.ReadAllText("habits.json");
            Habit habit = System.Text.Json.JsonSerializer.Deserialize<Habit>(habitJson);
            modelBuilder.Entity<Habit>().HasData(habit);
        }
    }
}
