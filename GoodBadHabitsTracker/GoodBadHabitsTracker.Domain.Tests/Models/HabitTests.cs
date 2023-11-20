using FluentAssertions;
using GoodBadHabitsTracker.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace GoodBadHabitsTracker.Core.Domain.Tests.Models
{

    public class HabitTests
    {
        [Fact]
        public void GenerateId_ReturnsNewGuid()
        {
            //Arrange
            var habit = new Habit();

            //Act
            habit.GenerateId();

            //Assert
            habit.HabitId.Should().NotBeEmpty();
        }
    }
}
