using GoodBadHabitsTracker.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GoodBadHabitsTracker.Core.Tests.Services
{
    public class HabitsServiceTests
    {
        private readonly IHabitsService _habitsService;
        public HabitsServiceTests()
        {
            
        }

        [Fact]
        public void Create_RepeatDailyTrueDaysOfMonthNotNull_ThrowsException()
        {
            //Arrange
            var habit = new Habit();

            Assert.Throws<Exception>(habit => habit.RepeatDailyTrueDaysOfMonth != null)
        }
    }
}
