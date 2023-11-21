using AutoFixture;
using FluentAssertions;
using GoodBadHabitsTracker.API.Controllers;
using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Core.DTOs;
using GoodBadHabitsTracker.Core.Services.HabitsService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GoodBadHabitsTracker.API.Tests.Controllers
{
    public class HabitsControllerTests
    {
        private readonly IHabitsService _habitsService;
        private readonly Mock<IHabitsService> _habitsServiceMock;
        private readonly IFixture _fixture;
        private readonly HabitsController _controller;

        public HabitsControllerTests()
        {
            _fixture = new Fixture();
            _habitsServiceMock = new Mock<IHabitsService>();
            _habitsService = _habitsServiceMock.Object;
        }
        [Fact]
        public async Task Create_DataIsValid_Returns201ResponseWithCreatedObject()
        {
            //Arrange
            var request = new HabitDto()
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
            var response = new Habit()
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
            response.GenerateId();
            _habitsServiceMock.Setup(temp => temp.Create(It.IsAny<HabitDto>())).Returns(Task.FromResult(response));
            HabitsController habitsController = new HabitsController(_habitsService);

            //Act
            var result = await habitsController.Create(request);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<CreatedAtActionResult>();
        }

        [Fact]
        public async Task Create_DataIsNotValid_Returns400Response()
        {
            //Arrange
            var request = new HabitDto()
            {
                Name = "Drink Water",
                UserId = Guid.Parse("f47e0a9d-36c2-4b77-ba15-123456789abc"),
                IsGood = true,
                IsGoalInTime = true,
                Quantity = 0,
                Frequency = "daily",
                IsRepeatDaily = true,
                RepeatDaysOfWeek = ["monday", "friday"],
                RepeatDaysOfMonth = [1, 2, 3, 4, 5, 6],
                StartDate = new DateOnly(2021, 12, 21),
                ReminderTime = new TimeOnly(12, 0, 0)
            };

            HabitsController habitsController = new HabitsController(_habitsService);
            habitsController.ModelState.AddModelError("StartDate", "Start date should be in future or today.");
            

            //Act
            
            var result = await habitsController.Create(request) as ObjectResult;

            //Assert
            result.Should().NotBeNull();
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Create_HabitIsNull_Return404Response()
        {
            //Arrange
            HabitDto request = null;
            HabitsController habitsController = new HabitsController(_habitsService);

            //Act
            var result = await habitsController.Create(request);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NotFoundResult>();
        }
    }
}
