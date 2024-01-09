using Azure;
using GoodBadHabitsTracker.API.Controllers.v1;
using GoodBadHabitsTracker.API.Services.EmailSender;
using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using GoodBadHabitsTracker.Core.Services.UserAccessor;
using GoodBadHabitsTracker.TestMisc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using GoodBadHabitsTracker.Core.DTOs;
using Google.Apis.Util;
using GoodBadHabitsTracker.API.Exceptions;
using System.Web.Http.Results;

namespace GoodBadHabitsTracker.API.Tests.Controllers.v1
{
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Mock<IWebHostEnvironment> _environmentMock;
        private readonly IWebHostEnvironment _environment;
        private readonly Mock<ICustomEmailSender<ApplicationUser>> _emailSenderMock;
        private readonly ICustomEmailSender<ApplicationUser> _emailSender;
        private readonly DataGenerator _dataGenerator;
        private readonly ITestOutputHelper _testOutputHelper;
        public AuthControllerTests(ITestOutputHelper testOutputHelper)
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), 
                Mock.Of<IOptions<IdentityOptions>>(), 
                Mock.Of<IPasswordHasher<ApplicationUser>>(),
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<ApplicationUser>>>());
            _userManager = _userManagerMock.Object;
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManager,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<ILogger<SignInManager<ApplicationUser>>>(),
                Mock.Of<IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<ApplicationUser>>());
            _signInManager = _signInManagerMock.Object;
            _environmentMock = new Mock<IWebHostEnvironment>();
            _environment = _environmentMock.Object;
            _emailSenderMock = new Mock<ICustomEmailSender<ApplicationUser>>();
            _emailSender = _emailSenderMock.Object;
            _dataGenerator = new DataGenerator();
            _testOutputHelper = testOutputHelper;
            
        }

        [Fact]
        public async Task Register_ValidRequest_ReturnsCreatedAtRoute()
        {
            //Arrange
            var request = _dataGenerator.SeedRegisterDto();
            var controller = new AuthController(_userManager, _signInManager, _environment, _emailSender);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);            

            //Act
            var result = await controller.Register(request) as CreatedAtRouteResult;
            _emailSenderMock.Setup(x => x.SendWelcomeMessageAsync(It.IsAny<ApplicationUser>(), "dobrestilomusic66@gmail.com"))
                .Returns(Task.FromResult(result.Value as ApplicationUser));
            var routeName = result.RouteName;
            var routeValues = result.RouteValues;
            var value = result.Value;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status201Created);
            routeName.Should().Be("GetUserById");
            routeValues["userId"].Should().BeAssignableTo<Guid>();
            value.Should().BeAssignableTo<ApplicationUser>();            
        }

        [Fact]
        public async Task Register_NullRequest_ReturnsBadRequest()
        {
            //Arrange
            RegisterDto request = null;
            var controller = new AuthController(_userManager, _signInManager, _environment, _emailSender);
            

            //Act
            Func<Task> action = async () => await controller.Register(request);
            var result = await controller.Register(request) as BadRequestObjectResult;

            // Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<ArgumentNullException>().WithParameterName("message");
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeAssignableTo<string>();
        }

        [Fact]
        public async Task Register_InvalidRequest_ReturnsBadRequest()
        {
            //Arrange
            var request = _dataGenerator.SeedRegisterDto();
            var controller = new AuthController(_userManager, _signInManager, _environment, _emailSender);
            controller.ModelState.AddModelError("Email", "Email address is not correct.");


            //Act
            Func<Task> action = async () => await controller.Register(request);
            var result = await controller.Register(request) as BadRequestObjectResult;
            


            // Assert
            controller.ModelState.ErrorCount.Should().BeGreaterThan(0);            
            action.Should().NotBeNull();
            action.Should().ThrowAsync<ArgumentNullException>().WithParameterName("message");
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Register_NameOrEmailExists_ReturnsConflict()
        {
            //Arrange
            var request = _dataGenerator.SeedRegisterDto();
            var controller = new AuthController(_userManager, _signInManager, _environment, _emailSender);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(It.IsAny<IdentityError>()));
            

            //Act
            Func<Task> action = async () => await controller.Register(request);
            var result = await controller.Register(request)! as ConflictObjectResult;
            

            // Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<ConflictException>();
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            result.Value.Should().BeAssignableTo<string>();
        }

        [Fact]
        public async Task Register_CatchesAnotherException_ReturnsInternalServerError()
        {
            //Arrange
            var request = _dataGenerator.SeedRegisterDto();
            var controller = new AuthController(_userManager, _signInManager, _environment, _emailSender);


            //Act
            Func<Task> action = async () => await controller.Register(request);
            var result = await controller.Register(request)! as ObjectResult;


            // Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<Exception>();
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            result.Value.Should().BeAssignableTo<string>();
        }
    }
}
