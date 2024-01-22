using GoodBadHabitsTracker.API.Controllers.v1;
using GoodBadHabitsTracker.API.Services.EmailSender;
using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using GoodBadHabitsTracker.TestMisc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using GoodBadHabitsTracker.Core.DTOs;
using GoodBadHabitsTracker.API.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;

namespace GoodBadHabitsTracker.API.Tests.Controllers.v1
{
    public class AuthControllerTests
    {
        private readonly AuthController _authController;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Mock<IWebHostEnvironment> _environmentMock;
        private readonly IWebHostEnvironment _environment;
        private readonly Mock<ICustomEmailSender<ApplicationUser>> _emailSenderMock;
        private readonly ICustomEmailSender<ApplicationUser> _emailSender;
     
        private readonly Mock<IUserStore<ApplicationUser>> _userStoreMock;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly Mock<IOptions<IdentityOptions>> _identityOptionsMock;
        private readonly IOptions<IdentityOptions> _identityOptions;

        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Mock<IUserClaimsPrincipalFactory<ApplicationUser>> _userClaimsPrincipalFactoryMock;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly Mock<ILogger<SignInManager<ApplicationUser>>> _signInManagerLoggerMock;
        private readonly ILogger<SignInManager<ApplicationUser>> _signInManagerLogger;
        private readonly Mock<IAuthenticationSchemeProvider> _authenticationSchemeProviderMock;
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        private readonly Mock<IUserConfirmation<ApplicationUser>> _userConfirmationMock;
        private readonly IUserConfirmation<ApplicationUser> _userConfirmation;

        private readonly DataGenerator _dataGenerator;
        private readonly ITestOutputHelper _testOutputHelper;

        public AuthControllerTests(ITestOutputHelper testOutputHelper)
        {
            _environmentMock = new Mock<IWebHostEnvironment>();
            _environment = _environmentMock.Object;
            _emailSenderMock = new Mock<ICustomEmailSender<ApplicationUser>>();
            _emailSender = _emailSenderMock.Object;
            _dataGenerator = new DataGenerator();
            _authController = new AuthController(_userManager, _signInManager, _environment, _emailSender);
            
            _userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userStore = _userStoreMock.Object;
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(_userStore, null, null, null, null, null, null, null, null);
            _userManager = _userManagerMock.Object;

            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextAccessor = _httpContextAccessorMock.Object;
            _userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _userClaimsPrincipalFactory = _userClaimsPrincipalFactoryMock.Object;
            _identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
            _identityOptions = _identityOptionsMock.Object;
            _signInManagerLoggerMock = new Mock<ILogger<SignInManager<ApplicationUser>>>();
            _signInManagerLogger = _signInManagerLoggerMock.Object;
            _authenticationSchemeProviderMock = new Mock<IAuthenticationSchemeProvider>();
            _authenticationSchemeProvider = _authenticationSchemeProviderMock.Object;
            _userConfirmationMock = new Mock<IUserConfirmation<ApplicationUser>>();
            _userConfirmation = _userConfirmationMock.Object;
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManager, _httpContextAccessor, _userClaimsPrincipalFactory,
                _identityOptions, _signInManagerLogger, _authenticationSchemeProvider,
                _userConfirmation);
            _signInManager = _signInManagerMock.Object;
            
            _testOutputHelper = testOutputHelper;
                       
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"MailSettings:Email", "goodbadhabitstracker@gmail.com"},
                    {"MailSettings:DisplayName", "GHBT"},
                    {"MailSettings:Password", "gpig isdo ytzx shjy"},
                    {"MailSettings:Host", "smtp.gmail.com"},
                    {"MailSettings:Port", "587"}
                }!).Build();
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));

            services.AddSingleton(_userManager);
            services.AddSingleton(_signInManager);
            services.AddSingleton(_environment);
            services.TryAddTransient<ICustomEmailSender<ApplicationUser>, CustomEmailSender>();
            services.AddTransient<AuthController>();

            var serviceProvider = services.BuildServiceProvider();
            _authController = serviceProvider.GetRequiredService<AuthController>();
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
                .Returns(Task.CompletedTask);
            var routeName = result.RouteName;
            var routeValues = result.RouteValues;
            var value = result.Value;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status201Created);
            routeName.Should().Be("GetUserById");
            routeValues["userId"].Should().BeAssignableTo<string>();
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
            var result = await controller.Register(request) as ConflictObjectResult;
            

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


            //Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<Exception>();
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            result.Value.Should().BeAssignableTo<string>();
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            //Arrange
            var request = _dataGenerator.SeedLoginDto();
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                 .ReturnsAsync(new ApplicationUser {Email = request.Email });
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<ApplicationUser>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            //Act
            var result = await _authController.Login(request) as OkObjectResult;
                     
            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var value = result.Value.Should().BeAssignableTo<object>().Subject;
            var properties = value.GetType().GetProperties();
            properties.Should().HaveCount(2);
            properties.All(p => p.PropertyType == typeof(string)).Should().BeTrue();
        }

        [Fact]
        public async Task Login_NullRequest_ReturnsBadRequest()
        {
            //Arrange
            LoginDto request = null;

            //Act
            Func<Task> action = async () => await _authController.Login(request);
            var result = await _authController.Login(request) as BadRequestObjectResult;

            //Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<ArgumentNullException>().WithParameterName("message");
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeAssignableTo<string>();
        }

        [Fact]
        public async Task Login_CantFindUser_ReturnsUnauthorized()
        {
            //Arrange
            LoginDto request = _dataGenerator.SeedLoginDto();
            ApplicationUser response = null;
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                 .ReturnsAsync(response);
            //Act
            Func<Task> action = async () => await _authController.Login(request);
            var result = await _authController.Login(request) as UnauthorizedObjectResult;

            //Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<InvalidCredentialException>().WithMessage("Invalid email or password");
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            result.Value.Should().BeAssignableTo<string>();
        }

        [Fact]
        public async Task Login_PasswordSignInAsyncFailed_ReturnsUnauthorized()
        {
            //Arrange
            LoginDto request = _dataGenerator.SeedLoginDto();
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                 .ReturnsAsync(new ApplicationUser { Email = request.Email });
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<ApplicationUser>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
            //Act
            Func<Task> action = async () => await _authController.Login(request);
            var result = await _authController.Login(request) as UnauthorizedObjectResult;

            //Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<InvalidCredentialException>().WithMessage("Invalid email or password");
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            result.Value.Should().BeAssignableTo<string>();
        }

        [Fact]
        public async Task Login_CatchesAnotherException_ReturnsInternalServerError()
        {
            //Arrange
            var request = _dataGenerator.SeedLoginDto();
            var controller = new AuthController(_userManager, _signInManager, _environment, _emailSender);
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                 .ThrowsAsync(new Exception());
           

            //Act
            Func<Task> action = async () => await controller.Login(request);
            var result = await controller.Login(request)! as ObjectResult;


            //Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<Exception>();
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            result.Value.Should().BeAssignableTo<string>();
        }

        
    }
}
