using AutoFixture;
using AutoFixture.DataAnnotations;
using AutoMapper;
using EntityFrameworkCoreMock;
using FluentAssertions;
using GoodBadHabitsTracker.Core.Domain.Interfaces;
using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Core.DTOs;
using GoodBadHabitsTracker.Core.Services.HabitsService;
using GoodBadHabitsTracker.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using GoodBadHabitsTracker.TestMisc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GoodBadHabitsTracker.Core.Tests.Services
{
    public class HabitsServiceTests
    {
        private readonly IHabitsService _habitsService;
        private readonly Mock<IHabitsRepository> _habitsRepositoryMock;
        private readonly IHabitsRepository _habitsRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly DataGenerator _generator;
        private readonly IMapper _mapper;

        public HabitsServiceTests(ITestOutputHelper testOutputHelper)
        {
            _generator = new DataGenerator();
            _habitsRepositoryMock = new Mock<IHabitsRepository>();
            _habitsRepository = _habitsRepositoryMock.Object;

            var habitsInitialData = new List<Habit>() { };

            DbContextMock<HabitsDbContext> dbContextMock = new DbContextMock<HabitsDbContext>(new DbContextOptionsBuilder<HabitsDbContext>().Options);

            HabitsDbContext dbContext = dbContextMock.Object;

            dbContextMock.CreateDbSetMock(temp => temp.Habits, habitsInitialData);

            _habitsService = new HabitsService(_habitsRepository, _mapper!);

            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Create_RepeatDailyTrueDaysOfMonthNotNull_ThrowsException()
        {
            //Arrange

            var habitDto = new HabitDto()
            {
                Name = "Drink Water",
                UserId = Guid.Parse("f47e0a9d-36c2-4b77-ba15-123456789abc"),
                IsGood = true,
                IsGoalInTime = true,
                Quantity = 8,
                Frequency = "daily",
                IsRepeatDaily = true,
                RepeatDaysOfWeek = ["monday", "friday"],
                RepeatDaysOfMonth = [1, 2, 3, 4, 5, 6],
                StartDate = new DateOnly(2023, 12, 21),
                ReminderTime = new TimeOnly(12, 0, 0)
            };

            //Act
            Func<Task> action = async () => await _habitsService.Create(habitDto);

            await action.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task Create_RepeatDailyFalseDaysOfMonthNull_ThrowsException()
        {
            //Arrange

            var habitDto = new HabitDto()
            {
                Name = "Drink Water",
                UserId = Guid.Parse("f47e0a9d-36c2-4b77-ba15-123456789abc"),
                IsGood = true,
                IsGoalInTime = true,
                Quantity = 8,
                Frequency = "daily",
                IsRepeatDaily = false,
                RepeatDaysOfWeek = ["monday", "friday"],
                RepeatDaysOfMonth = [],
                StartDate = new DateOnly(2023, 12, 21),
                ReminderTime = new TimeOnly(12, 0, 0)
            };

            //Act
            Func<Task> action = async () => await _habitsService.Create(habitDto);

            await action.Should().ThrowAsync<Exception>();
        }
        [Fact]
        public async Task Create_RepeatDailyTrueDaysOfWeekNull_ThrowsException()
        {
            //Arrange

            var habitDto = new HabitDto()
            {
                Name = "Drink Water",
                UserId = Guid.Parse("f47e0a9d-36c2-4b77-ba15-123456789abc"),
                IsGood = true,
                IsGoalInTime = true,
                Quantity = 8,
                Frequency = "daily",
                IsRepeatDaily = true,
                RepeatDaysOfWeek = [],
                RepeatDaysOfMonth = [],
                StartDate = new DateOnly(2023, 12, 21),
                ReminderTime = new TimeOnly(12, 0, 0)
            };

            //Act
            Func<Task> action = async () => await _habitsService.Create(habitDto);

            await action.Should().ThrowAsync<Exception>();
        }
        [Fact]
        public async Task Create_RepeatDailyFalseDaysOfWeekNotNull_ThrowsException()
        {
            //Arrange

            var habitDto = new HabitDto()
            {
                Name = "Drink Water",
                UserId = Guid.Parse("f47e0a9d-36c2-4b77-ba15-123456789abc"),
                IsGood = true,
                IsGoalInTime = true,
                Quantity = 8,
                Frequency = "daily",
                IsRepeatDaily = false,
                RepeatDaysOfWeek = ["monday"],
                RepeatDaysOfMonth = [],
                StartDate = new DateOnly(2023, 12, 21),
                ReminderTime = new TimeOnly(12, 0, 0)
            };

            //Act
            Func<Task> action = async () => await _habitsService.Create(habitDto);

            await action.Should().ThrowAsync<Exception>();
        }
    }
}
