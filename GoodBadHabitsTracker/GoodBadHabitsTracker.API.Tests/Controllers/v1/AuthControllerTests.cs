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
            //Fill this constructor with mocks
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
    }
}
