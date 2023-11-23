using AutoFixture;
using AutoFixture.Xunit2;
using AutoMapper;
using Azure;
using FluentAssertions;
using GoodBadHabitsTracker.API.Controllers;
using GoodBadHabitsTracker.Core.Domain.Models;
using GoodBadHabitsTracker.Core.DTOs;
using GoodBadHabitsTracker.Core.Services.HabitsService;
using GoodBadHabitsTracker.TestMisc;
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
        private readonly HabitsController _controller;
        private readonly IMapper _mapper;
        private readonly Mock<IMapper> _mapperMock;
        private readonly DataGenerator _generator;

        public HabitsControllerTests()
        {
            _habitsServiceMock = new Mock<IHabitsService>();
            _habitsService = _habitsServiceMock.Object;
            _mapperMock = new Mock<IMapper>();
            _mapper = _mapperMock.Object;
            _generator = new DataGenerator();
        }

        [Fact]
        public async Task GetHabits_IsThereAnyHabit_Returns200ResponseWithObjects()
        {
            //Arrange
            IEnumerable<Habit> response = _generator.SeedCollection(5);
            HabitsController habitsController = new HabitsController(_habitsService, _mapper);
            _habitsServiceMock.Setup(temp => temp.GetHabits()).Returns(Task.FromResult(response));

            //Act
            var result = await habitsController.GetHabits();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ActionResult<IEnumerable<Habit>>>();
            result.Result.Should().BeAssignableTo<OkObjectResult>();
        }
        [Fact]
        public async Task GetHabits_NoHabits_Returns404Response()
        {
            //Arrange
            IEnumerable<Habit> response = [];
            HabitsController habitsController = new HabitsController(_habitsService, _mapper);
            _habitsServiceMock.Setup(temp => temp.GetHabits()).Returns(Task.FromResult(response));

            //Act
            var result = await habitsController.GetHabits();

            //Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<NotFoundResult>();
        }
        [Fact]
        public async Task GetHabitsById_CorrectGuid_Returns200ResponseWithObject()
        {
            //Arrange
            Habit? response = _generator.Seed();
            HabitsController habitsController = new HabitsController(_habitsService, _mapper);
            _habitsServiceMock.Setup(temp => temp.GetHabitById(It.IsAny<Guid>())).Returns(Task.FromResult(response));

            //Act
            var result = await habitsController.GetHabitById(It.IsAny<Guid>());

            //Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<OkObjectResult>();
        }
        [Fact]
        public async Task GetHabitsById_HabitIsNull_Returns404Response()
        {
            //Arrange
            Habit? response = null;
            HabitsController habitsController = new HabitsController(_habitsService, _mapper);
            _habitsServiceMock.Setup(temp => temp.GetHabitById(It.IsAny<Guid>())).Returns(Task.FromResult(response));

            //Act
            var result = await habitsController.GetHabitById(It.IsAny<Guid>());

            //Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<NotFoundResult>();
        }
        [Fact]
        public async Task Create_DataIsValid_Returns201ResponseWithCreatedObject()
        {
            //Arrange
            var request = _generator.SeedDto();
            var response = _generator.Seed();
            _habitsServiceMock.Setup(temp => temp.Create(It.IsAny<HabitDto>())).Returns(Task.FromResult(response));
            HabitsController habitsController = new HabitsController(_habitsService, _mapper);

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
            var request = _generator.SeedDto();
            HabitsController habitsController = new HabitsController(_habitsService, _mapper);
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
            HabitsController habitsController = new HabitsController(_habitsService, _mapper);

            //Act
            var result = await habitsController.Create(request);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_HabitIsNull_Return404Response()
        {
            //Arrange
            HabitDto request = null;
            Habit response = null;

            _habitsServiceMock.Setup(temp => temp.Edit(It.IsAny<Habit>(), It.IsAny<HabitDto>())).Returns(Task.FromResult(response));
            HabitsController habitsController = new HabitsController(_habitsService, _mapper);

            //Act
            var result = await habitsController.Create(request);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_HabitExists_Return204Response()
        {
            //Arrange
            HabitDto request = _generator.SeedDto();
            Habit response = _generator.Seed();
            _habitsServiceMock.Setup(temp => temp.GetHabitById(It.IsAny<Guid>())).Returns(Task.FromResult(response));
            _habitsServiceMock.Setup(temp => temp.Edit(It.IsAny<Habit>(), It.IsAny<HabitDto>())).Returns(Task.FromResult(response));
            HabitsController habitsController = new HabitsController(_habitsService, _mapper);

            //Act
            var result = await habitsController.Edit(request, It.IsAny<Guid>());

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NoContentResult>();
        }

        [Fact]
        public async Task Delete_HabitIsNull_Return404Response()
        {
            //Arrange
            Habit response = null;
            _habitsServiceMock.Setup(temp => temp.GetHabitById(It.IsAny<Guid>())).Returns(Task.FromResult(response));
            _habitsServiceMock.Setup(temp => temp.Delete(It.IsAny<Habit>())).Returns(Task.FromResult(response));
            HabitsController habitsController = new HabitsController(_habitsService, _mapper);

            //Act
            var result = await habitsController.Delete(It.IsAny<Guid>());

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_HabitExists_Return204Response()
        {
            //Arrange
            Habit response = _generator.Seed();
            _habitsServiceMock.Setup(temp => temp.GetHabitById(It.IsAny<Guid>())).Returns(Task.FromResult(response));
            _habitsServiceMock.Setup(temp => temp.Delete(It.IsAny<Habit>())).Returns(Task.FromResult(response));
            HabitsController habitsController = new HabitsController(_habitsService, _mapper);

            //Act
            var result = await habitsController.Delete(It.IsAny<Guid>());

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NoContentResult>();
        }

        [Fact]
        public async Task DeleteAll_NoHabits_Return404Response()
        {
            //Arrange
            IEnumerable<Habit> response = [];
            _habitsServiceMock.Setup(temp => temp.GetHabits()).Returns(Task.FromResult(response));
            _habitsServiceMock.Setup(temp => temp.DeleteAll()).Returns(Task.FromResult(response));
            HabitsController habitsController = new HabitsController(_habitsService, _mapper);

            //Act
            var result = await habitsController.DeleteAll();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteAll_HabitExists_Return204Response()
        {
            //Arrange
            IEnumerable<Habit> response = _generator.SeedCollection(10);
            _habitsServiceMock.Setup(temp => temp.GetHabits()).Returns(Task.FromResult(response));
            _habitsServiceMock.Setup(temp => temp.DeleteAll()).Returns(Task.FromResult(response));
            HabitsController habitsController = new HabitsController(_habitsService, _mapper);

            //Act
            var result = await habitsController.DeleteAll();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NoContentResult>();
        }
    }
}
