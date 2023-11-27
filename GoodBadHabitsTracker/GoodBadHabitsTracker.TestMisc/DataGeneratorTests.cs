using Bogus;
using FluentAssertions;
using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GoodBadHabitsTracker.TestMisc
{
    public class DataGeneratorTests
    {
        Random random = new Random();
        [Fact]
        public void SeedCollection_ReturnsHabits()
        {
            //Arrange
            var habitsGenerator = new Faker<Habit>();

            //Act
            var habits = habitsGenerator.Generate(random.Next(2, 20));

            //Assert
            habits.Should().NotBeEmpty();
        }
        [Fact]
        public void Seed_ReturnsHabit()
        {
            //Arrange
            var habitsGenerator = new Faker<Habit>();

            //Act
            var habit = habitsGenerator.Generate();

            //Assert
            habit.Should().NotBeNull();
        }
        [Fact]
        public void SeedDto_ReturnsHabitDto()
        {
            //Arrange
            var habitsGenerator = new Faker<HabitDto>();

            //Act
            var habitDto = habitsGenerator.Generate();

            //Assert
            habitDto.Should().NotBeNull();
        }
    }
}
