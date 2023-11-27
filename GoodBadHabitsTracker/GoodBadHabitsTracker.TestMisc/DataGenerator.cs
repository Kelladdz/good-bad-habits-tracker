using Bogus;
using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Core.DTOs;
using GoodBadHabitsTracker.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.TestMisc
{
    public class DataGenerator
    {
        Random random = new Random();
        string[] allowedFreqValues = ["daily", "weekly", "monthly"];
        string[] allowedDaysOfWeek = ["monday",
            "tuesday",
            "wednesday",
            "thursday",
            "friday",
            "saturday",
            "sunday"];
        public IEnumerable<Habit> SeedCollection(int number)
        {
            var habitsGenerator = new Faker<Habit>()
                .RuleFor(h => h.HabitId, f => Guid.NewGuid())
                .RuleFor(h => h.Name, f => f.Name.JobTitle())
                .RuleFor(h => h.UserId, f => Guid.NewGuid())
                .RuleFor(h => h.IsGood, f => f.Random.Bool())
                .RuleFor(h => h.IsGoalInTime, f => f.Random.Bool())
                .RuleFor(h => h.Quantity, f => f.Random.Byte())
                .RuleFor(h => h.Frequency, f => f.PickRandom(allowedFreqValues))
                .RuleFor(h => h.IsRepeatDaily, f => true)
                .RuleFor(h => h.RepeatDaysOfWeek, f => Enumerable.Range(1, random.Next(2,7)).Select(x => f.PickRandom(allowedDaysOfWeek)).ToArray())
                .RuleFor(h => h.RepeatDaysOfMonth, f => [])
                .RuleFor(h => h.StartDate, f => f.Date.FutureDateOnly())
                .RuleFor(h => h.ReminderTime, f => f.Date.SoonTimeOnly());

            IEnumerable<Habit> habits = habitsGenerator.Generate(number);
            return habits;
        }
        public Habit Seed()
        {
            var habitsGenerator = new Faker<Habit>()
                .RuleFor(h => h.HabitId, f => Guid.NewGuid())
                .RuleFor(h => h.Name, f => f.Name.JobTitle())
                .RuleFor(h => h.UserId, f => Guid.NewGuid())
                .RuleFor(h => h.IsGood, f => f.Random.Bool())
                .RuleFor(h => h.IsGoalInTime, f => f.Random.Bool())
                .RuleFor(h => h.Quantity, f => f.Random.Byte())
                .RuleFor(h => h.Frequency, f => f.PickRandom(allowedFreqValues))
                .RuleFor(h => h.IsRepeatDaily, f => true)
                .RuleFor(h => h.RepeatDaysOfWeek, f => Enumerable.Range(1, random.Next(2, 7)).Select(x => f.PickRandom(allowedDaysOfWeek)).ToArray())
                .RuleFor(h => h.RepeatDaysOfMonth, f => [])
                .RuleFor(h => h.StartDate, f => f.Date.FutureDateOnly())
                .RuleFor(h => h.ReminderTime, f => f.Date.SoonTimeOnly());

            Habit habit = habitsGenerator.Generate();
            return habit;
        }
        public HabitDto SeedDto()
        {
            var habitsGenerator = new Faker<HabitDto>()
                .RuleFor(h => h.Name, f => f.Name.JobTitle())
                .RuleFor(h => h.IsGood, f => f.Random.Bool())
                .RuleFor(h => h.IsGoalInTime, f => f.Random.Bool())
                .RuleFor(h => h.Quantity, f => f.Random.Byte())
                .RuleFor(h => h.Frequency, f => f.PickRandom(allowedFreqValues))
                .RuleFor(h => h.IsRepeatDaily, f => true)
                .RuleFor(h => h.RepeatDaysOfWeek, f => Enumerable.Range(1, random.Next(2, 7)).Select(x => f.PickRandom(allowedDaysOfWeek)).ToArray())
                .RuleFor(h => h.RepeatDaysOfMonth, f => [])
                .RuleFor(h => h.StartDate, f => f.Date.FutureDateOnly())
                .RuleFor(h => h.ReminderTime, f => f.Date.SoonTimeOnly());

            HabitDto habitDto = habitsGenerator.Generate();
            return habitDto;
        }

    }
}
