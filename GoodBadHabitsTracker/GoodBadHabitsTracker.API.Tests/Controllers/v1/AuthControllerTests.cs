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
using System.Security.Claims;
using System;
using Bogus;
using Azure.Core;
using System.Net.Http;
using Microsoft.Extensions.Primitives;
using GoodBadHabitsTracker.Infrastructure.Services.IdTokenHandler;
using GoodBadHabitsTracker.Infrastructure.Services.JwtTokenHandler;
using GoodBadHabitsTracker.Infrastructure.Services.JwtTokenRevoker;
using Microsoft.AspNetCore.TestHost;
using HttpContextMoq;
using Microsoft.Identity.Client;
using GoodBadHabitsTracker.Infrastructure.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Moq.Microsoft.Configuration;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;

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

        private readonly Mock<IIDTokenHandler> _idTokenHandlerMock;
        private readonly IIDTokenHandler _idTokenHandler;
        private readonly Mock<IJwtTokenHandler> _jwtTokenHandlerMock;
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly Mock<IJwtTokenRevoker> _jwtTokenRevokerMock;
        private readonly IJwtTokenRevoker _jwtTokenRevoker;

        private readonly Mock<IUserStore<ApplicationUser>> _userStoreMock;
        private readonly IUserStore<ApplicationUser> _userStore;

        private readonly Mock<IOptions<IdentityOptions>> _identityOptionsMock;
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly Mock<IOptions<JwtSettings>> _jwtOptionsMock;
        private readonly IOptions<JwtSettings> _jwtOptions;

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

        /*private readonly Mock<IConfiguration> _configurationMock;
        private readonly IConfiguration _configuration;
        private readonly Mock<IServiceCollection> _serviceCollectionMock;
        private readonly IServiceCollection _serviceCollection;*/

        private readonly DataGenerator _dataGenerator;
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly IConfiguration _configuration;

        public AuthControllerTests(ITestOutputHelper testOutputHelper)
        {
            _environmentMock = new Mock<IWebHostEnvironment>();
            _environment = _environmentMock.Object;
            _emailSenderMock = new Mock<ICustomEmailSender<ApplicationUser>>();
            _emailSender = _emailSenderMock.Object;
            _jwtOptionsMock = new Mock<IOptions<JwtSettings>>();
            _jwtOptions = _jwtOptionsMock.Object;
            _idTokenHandlerMock = new Mock<IIDTokenHandler>();
            _idTokenHandler = _idTokenHandlerMock.Object;
            _jwtTokenHandlerMock = new Mock<IJwtTokenHandler>();
            _jwtTokenHandler = _jwtTokenHandlerMock.Object;
            _jwtTokenRevokerMock = new Mock<IJwtTokenRevoker>();
            _jwtTokenRevoker = _jwtTokenRevokerMock.Object;
            _dataGenerator = new DataGenerator();
            _authController = new AuthController(_userManager, _signInManager, _environment, _emailSender, _idTokenHandler, _jwtTokenHandler, _jwtTokenRevoker)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
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

            /*_configurationMock = new Mock<IConfiguration>();
            _configuration = _configurationMock.Object;
            _serviceCollectionMock = new Mock<IServiceCollection>();
            _serviceCollection = _serviceCollectionMock.Object;

            _configurationMock.Setup(x => x.GetSection("Jwt:Issuer").Value).Returns("https://localhost:7154");
            _configurationMock.Setup(x => x.GetSection("Jwt:Audience").Value).Returns("https://localhost:8080");
            _configurationMock.Setup(x => x.GetSection("Jwt:Key").Value).Returns("MvNiSDNDcszH+bMJviB4OHYCZVhoPXqM5JpzbmcwTXIN50U8RPMGqU5LmfvuyciP");*/

            var services = new ServiceCollection();

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"MailSettings:Email", "goodbadhabitstracker@gmail.com"},
                    {"MailSettings:DisplayName", "GHBT"},
                    {"MailSettings:Password", "gpig isdo ytzx shjy"},
                    {"MailSettings:Host", "smtp.gmail.com"},
                    {"MailSettings:Port", "587"},
                    {"Jwt:Issuer", "https://localhost:7154"},
                    {"Jwt:Audience", "https://localhost:8080" },
                    {"Jwt:Key", "MvNiSDNDcszH+bMJviB4OHYCZVhoPXqM5JpzbmcwTXIN50U8RPMGqU5LmfvuyciP" }
                }!)
                .Build();

            services.Configure<MailSettings>(_configuration.GetSection("MailSettings"));
            services.Configure<JwtSettings>(_configuration.GetSection("Jwt"));

            services.AddSingleton(_userManager);
            services.AddSingleton(_signInManager);
            services.AddSingleton(_environment);
            services.AddSingleton(_emailSender);
            services.AddSingleton(_idTokenHandler);
            services.AddSingleton(_jwtTokenHandler);
            services.AddSingleton(_jwtTokenRevoker);
            services.AddTransient<AuthController>();


            var serviceProvider = services.BuildServiceProvider();
            _authController = serviceProvider.GetRequiredService<AuthController>();
            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

        }

        [Fact]
        public async Task Register_ValidRequest_ReturnsCreatedAtRoute()
        {
            //Arrange
            var request = _dataGenerator.SeedRegisterDto();
            var controller = new AuthController(_userManager, _signInManager, _environment, _emailSender, _idTokenHandler, _jwtTokenHandler, _jwtTokenRevoker);
            
            _userManagerMock.Setup(x => x.CreateAsync(It.Is<ApplicationUser>(user => user.Id.GetType() == typeof(Guid)), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _emailSenderMock.Setup(x => x.SendWelcomeMessageAsync(It.IsAny<ApplicationUser>(), "dobrestilomusic66@gmail.com"))
                .Returns(Task.CompletedTask);

            //Act
            var result = await controller.Register(request) as CreatedAtRouteResult;
            var routeName = result.RouteName;
            var routeValues = result.RouteValues;
            var value = result.Value;

            // Assert
            result.StatusCode.Should().Be(StatusCodes.Status201Created);
            routeName.Should().Be("GetUserById");
            routeValues["userId"].Should().BeAssignableTo<Guid>();
            routeValues["userId"].ToString().Should().MatchRegex(@"^({){0,1}[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}(}){0,1}$");
            value.Should().BeAssignableTo<ApplicationUser>();            
        }

        [Fact]
        public async Task Register_NullRequest_ReturnsBadRequest()
        {
            //Arrange
            RegisterDto request = null;
            var controller = new AuthController(_userManager, _signInManager, _environment, _emailSender, _idTokenHandler, _jwtTokenHandler, _jwtTokenRevoker);


            //Act
            Func<Task> action = async () => await controller.Register(request);
            var result = await controller.Register(request) as BadRequestObjectResult;

            // Assert
            action.Should().ThrowAsync<HttpRequestException>().WithMessage("Request cannot be null.");
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeEquivalentTo("Request cannot be null.");
        }

        [Fact]
        public async Task Register_InvalidRequest_ReturnsBadRequest()
        {
            //Arrange
            var request = _dataGenerator.SeedRegisterDto();
            var controller = new AuthController(_userManager, _signInManager, _environment, _emailSender, _idTokenHandler, _jwtTokenHandler, _jwtTokenRevoker);

            controller.ModelState.AddModelError("Email", "Email address is not correct.");

            //Act
            Func<Task> action = async () => await controller.Register(request);
            var result = await controller.Register(request) as BadRequestObjectResult;
            
            // Assert
            controller.ModelState.ErrorCount.Should().BeGreaterThan(0);
            action.Should().ThrowAsync<ArgumentException>().WithMessage("User data is invalid.");
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeEquivalentTo("User data is invalid.");
        }

        [Fact]
        public async Task Register_NameOrEmailExists_ReturnsConflict()
        {
            //Arrange
            var request = _dataGenerator.SeedRegisterDto();
            var controller = new AuthController(_userManager, _signInManager, _environment, _emailSender, _idTokenHandler, _jwtTokenHandler, _jwtTokenRevoker);
            
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(It.IsAny<IdentityError>()));
            
            //Act
            Func<Task> action = async () => await controller.Register(request);
            var result = await controller.Register(request) as ConflictObjectResult;
            

            // Assert
            action.Should().ThrowAsync<ConflictException>().WithMessage("This name or email exists."); 
            result.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            result.Value.Should().BeAssignableTo<string>();
            result.Value.Should().BeEquivalentTo("This name or email exists.");
        }

        [Fact]
        public async Task Register_CatchesAnotherException_ReturnsInternalServerError()
        {
            //Arrange
            var request = _dataGenerator.SeedRegisterDto();
            var controller = new AuthController(_userManager, _signInManager, _environment, _emailSender, _idTokenHandler, _jwtTokenHandler, _jwtTokenRevoker);

            //Act
            Func<Task> action = async () => await controller.Register(request);
            var result = await controller.Register(request)! as ObjectResult;

            //Assert
            action.Should().ThrowAsync<Exception>();
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            result.Value.Should().BeAssignableTo<string>();
        }

        public delegate void GenerateAccessTokenCallback(UserSession userSession, out string userFingerprint);
        public delegate string GenerateAccessTokenReturns(UserSession userSession, out string userFingerprint);

        [Fact]
        public async Task Login_ValidCredentialsUserHasRole_ReturnsOk()
        {
            //Arrange
            var request = _dataGenerator.SeedLoginDto();
            var userId = Guid.NewGuid();
            var userSession = new UserSession(userId, request.Email, request.Email, "User");
            var accessToken = _dataGenerator.SeedToken();
            var refreshToken = _dataGenerator.SeedToken();
            var httpContext = _authController.ControllerContext.HttpContext;
            string userFingerprint;
            string expectedUserFingerprint = _dataGenerator.SeedFingerprint();

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                 .ReturnsAsync(new ApplicationUser {Id = userId, UserName = request.Email, Email = request.Email });
            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string> { "User" });
            _jwtTokenHandlerMock.Setup(x => x.GenerateUserFingerprint()).Returns(It.IsAny<string>());
            _jwtTokenHandlerMock.Setup(x => x.GenerateUserFingerprintHash(It.IsAny<string>())).Returns(It.IsAny<string>());
            _jwtTokenHandlerMock.Setup(x => x.GenerateAccessToken(It.IsAny<UserSession>(), out It.Ref<string>.IsAny))
                .Callback(new GenerateAccessTokenCallback((UserSession session, out string userFingerprint) =>
                {
                    userFingerprint = _jwtTokenHandler.GenerateUserFingerprint();
                }))
                .Returns(new GenerateAccessTokenReturns((UserSession session, out string userFingerprint) =>
                {
                    userFingerprint = expectedUserFingerprint;
                    return accessToken;
                }));
            _jwtTokenHandlerMock.Setup(x => x.GenerateRefreshToken()).Returns(refreshToken);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            //Act
            var result = await _authController.Login(request) as OkObjectResult;
            
            //Assert
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var expectedResult = new {accessToken, refreshToken};
            result.Value.Should().BeEquivalentTo(expectedResult);
            var properties = result.Value.GetType().GetProperties();
            properties.Should().HaveCount(2);
            properties.All(p => p.PropertyType == typeof(string)).Should().BeTrue();
            httpContext.Response.Headers["Set-Cookie"].Should().BeEquivalentTo($"__Secure-Fgp={expectedUserFingerprint}; max-age=900; path=/; secure; samesite=strict; httponly");
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
            action.Should().ThrowAsync<HttpRequestException>().WithMessage("Request cannot be null.");
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeEquivalentTo("Request cannot be null.");
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
            action.Should().ThrowAsync<InvalidCredentialException>().WithMessage("Invalid email or password");
            result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            result.Value.Should().BeEquivalentTo("Invalid email or password");
        }

        [Fact]
        public async Task Login_CheckPasswordAsyncFailed_ReturnsBadRequest()
        {
            //Arrange
            LoginDto request = _dataGenerator.SeedLoginDto();

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                 .ReturnsAsync(new ApplicationUser { Email = request.Email });
            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                 .ReturnsAsync(false);

            //Act
            Func<Task> action = async () => await _authController.Login(request);
            var result = await _authController.Login(request) as UnauthorizedObjectResult;

            //Assert

            action.Should().ThrowAsync<InvalidCredentialException>().WithMessage("Invalid email or password");
            result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            result.Value.Should().BeEquivalentTo("Invalid email or password");
        }

        [Fact]
        public async Task Login_UserRoleCannotbeAdded_ReturnsBadRequest()
        {
            //Arrange
            var request = _dataGenerator.SeedLoginDto();
            List<string> getUserRole = new List<string>();

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                 .ReturnsAsync(new ApplicationUser { Id = Guid.NewGuid(), UserName = request.Email, Email = request.Email });
            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(getUserRole);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(It.IsAny<IdentityError>()));

            //Act
            Func<Task> action = async () => await _authController.Login(request);
            var result = await _authController.Login(request) as BadRequestObjectResult;

            //Assert
            action.Should().ThrowAsync<InvalidOperationException>().WithMessage("User role cannot be added.");
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeEquivalentTo("User role cannot be added.");
        }

        [Fact]
        public async Task Login_UserHasRoleNullAccessToken_ReturnsUnauthorized()
        {
            //Arrange
            var request = _dataGenerator.SeedLoginDto();
            var userId = Guid.NewGuid();
            var userSession = new UserSession(userId, request.Email, request.Email, "User");
            string accessToken = null;

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                 .ReturnsAsync(new ApplicationUser { Id = userId, UserName = request.Email, Email = request.Email });
            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string> { "User" });
            _jwtTokenHandlerMock.Setup(x => x.GenerateAccessToken(It.IsAny<UserSession>(), out It.Ref<string>.IsAny))
                .Returns(accessToken);

            //Act
            Func<Task> action = async () => await _authController.Login(request);
            var result = await _authController.Login(request) as BadRequestObjectResult;

            //Assert
            action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Access token cannot be null.");
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeEquivalentTo("Access token cannot be null.");       
        }

        [Fact]
        public async Task Login_UserHasRoleNullRefreshToken_ReturnsUnauthorized()
        {
            //Arrange
            var request = _dataGenerator.SeedLoginDto();
            var userId = Guid.NewGuid();
            var userSession = new UserSession(userId, request.Email, request.Email, "User");
            var accessToken = _dataGenerator.SeedToken();
            string refreshToken = null;

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                 .ReturnsAsync(new ApplicationUser { Id = userId, UserName = request.Email, Email = request.Email });
            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string> { "User" });
            _jwtTokenHandlerMock.Setup(x => x.GenerateAccessToken(It.IsAny<UserSession>(), out It.Ref<string>.IsAny))
                .Returns(accessToken);
            _jwtTokenHandlerMock.Setup(x => x.GenerateRefreshToken()).Returns(refreshToken);

            //Act
            Func<Task> action = async () => await _authController.Login(request);
            var result = await _authController.Login(request) as BadRequestObjectResult;

            //Assert
            action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Refresh token cannot be null.");
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeEquivalentTo("Refresh token cannot be null.");
        }

        [Fact]
        public async Task Login_UserHasRoleCantUpdateUser_ReturnsUnauthorized()
        {
            //Arrange
            var request = _dataGenerator.SeedLoginDto();
            var userId = Guid.NewGuid();
            var userSession = new UserSession(userId, request.Email, request.Email, "User");
            var accessToken = _dataGenerator.SeedToken();
            string refreshToken = _dataGenerator.SeedToken();

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                 .ReturnsAsync(new ApplicationUser { Id = userId, UserName = request.Email, Email = request.Email });
            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string> { "User" });
            _jwtTokenHandlerMock.Setup(x => x.GenerateAccessToken(It.IsAny<UserSession>(), out It.Ref<string>.IsAny))
                .Returns(accessToken);
            _jwtTokenHandlerMock.Setup(x => x.GenerateRefreshToken()).Returns(refreshToken);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Failed(It.IsAny<IdentityError>()));

            //Act
            Func<Task> action = async () => await _authController.Login(request);
            var result = await _authController.Login(request) as BadRequestObjectResult;

            //Assert
            action.Should().ThrowAsync<InvalidOperationException>().WithMessage("User update failed.");
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeEquivalentTo("User update failed.");
        }

        [Fact]
        public async Task Login_UserDoesntHaveRoleNullAccessToken_ReturnsUnauthorized()
        {
            //Arrange
            var request = _dataGenerator.SeedLoginDto();
            var userId = Guid.NewGuid();
            List<string> getUserRole = new List<string>();
            var userSession = new UserSession(userId, request.Email, request.Email, "User");
            string accessToken = null;

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                 .ReturnsAsync(new ApplicationUser { Id = userId, UserName = request.Email, Email = request.Email });
            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(getUserRole);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback(() =>
                {
                    getUserRole.Add("User");
                })
                .ReturnsAsync(() =>
                {
                    return IdentityResult.Success;
                });    
            _jwtTokenHandlerMock.Setup(x => x.GenerateAccessToken(It.IsAny<UserSession>(), out It.Ref<string>.IsAny))
                .Returns(accessToken);


            //Act
            Func<Task> action = async () => await _authController.Login(request);
            var result = await _authController.Login(request) as BadRequestObjectResult;

            //Assert
            action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Access token cannot be null.");
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeEquivalentTo("Access token cannot be null.");
        }

        [Fact]
        public async Task Login_UserDoesntHaveRoleNullRefreshToken_ReturnsUnauthorized()
        {
            //Arrange
            var request = _dataGenerator.SeedLoginDto();
            var userId = Guid.NewGuid();
            List<string> getUserRole = new List<string>();
            var userSession = new UserSession(userId, request.Email, request.Email, "User");
            string accessToken = _dataGenerator.SeedToken();
            string refreshToken = null;

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                 .ReturnsAsync(new ApplicationUser { Id = userId, UserName = request.Email, Email = request.Email });
            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(getUserRole);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback(() =>
                {
                    getUserRole.Add("User");
                })
                .ReturnsAsync(() =>
                {
                    return IdentityResult.Success;
                });
            _jwtTokenHandlerMock.Setup(x => x.GenerateAccessToken(It.IsAny<UserSession>(), out It.Ref<string>.IsAny))
                .Returns(accessToken);
            _jwtTokenHandlerMock.Setup(x => x.GenerateRefreshToken()).Returns(refreshToken);

            //Act
            Func<Task> action = async () => await _authController.Login(request);
            var result = await _authController.Login(request) as BadRequestObjectResult;

            //Assert
            action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Refresh token cannot be null.");
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeEquivalentTo("Refresh token cannot be null.");
        }

        [Fact]
        public async Task Login_UserDoesntHaveRoleCantUpdateUser_ReturnsUnauthorized()
        {
            //Arrange
            var request = _dataGenerator.SeedLoginDto();
            var userId = Guid.NewGuid();
            List<string> getUserRole = new List<string>();
            var userSession = new UserSession(userId, request.Email, request.Email, "User");
            string accessToken = _dataGenerator.SeedToken();
            string refreshToken = _dataGenerator.SeedToken();

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                 .ReturnsAsync(new ApplicationUser { Id = userId, UserName = request.Email, Email = request.Email });
            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(getUserRole);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback(() =>
                {
                    getUserRole.Add("User");
                })
                .ReturnsAsync(() =>
                {
                    return IdentityResult.Success;
                });
            _jwtTokenHandlerMock.Setup(x => x.GenerateAccessToken(It.IsAny<UserSession>(), out It.Ref<string>.IsAny))
                .Returns(accessToken);
            _jwtTokenHandlerMock.Setup(x => x.GenerateRefreshToken()).Returns(refreshToken);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Failed(It.IsAny<IdentityError>()));

            //Act
            Func<Task> action = async () => await _authController.Login(request);
            var result = await _authController.Login(request) as BadRequestObjectResult;

            //Assert
            action.Should().ThrowAsync<InvalidOperationException>().WithMessage("User update failed.");
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeEquivalentTo("User update failed.");
        }

        [Fact]
        public async Task Login_CatchesAnotherException_ReturnsInternalServerError()
        {
            //Arrange
            var request = _dataGenerator.SeedLoginDto();

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                 .ThrowsAsync(new Exception());
           
            //Act
            Func<Task> action = async () => await _authController.Login(request);
            var result = await _authController.Login(request)! as ObjectResult;


            //Assert
            action.Should().ThrowAsync<Exception>();
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            result.Value.Should().BeAssignableTo<string>();
        }

        [Fact]
        public async Task ExternalLogin_ValidTokensAndUserHasLogin_ReturnsOk()
        {
            //Arrange
            var provider = "Google";
            var idToken = _dataGenerator.SeedGoogleIdToken();
            var accessToken = _dataGenerator.SeedToken();   
            var httpContext = _authController.ControllerContext.HttpContext;

            httpContext.Request.Headers.Authorization = $"Bearer {accessToken}";
            httpContext.Request.Headers["Authentication"] = idToken;


            _idTokenHandlerMock.Setup(x => x.GetClaimsPrincipalFromIdToken(It.IsAny<string>()))
                .Returns(It.IsAny<ClaimsPrincipal>);    
            _signInManagerMock.Setup(x => x.ExternalLoginSignInAsync(It.IsAny<string>(),
                               It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _userManagerMock.Setup(x => x.FindByLoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser());
            _signInManagerMock.Setup(x => x.UpdateExternalAuthenticationTokensAsync(It.IsAny<ExternalLoginInfo>()))
                .ReturnsAsync(IdentityResult.Success);
            //Act

            var result = await _authController.ExternalLogin(provider) as OkObjectResult;

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var value = result.Value.Should().BeAssignableTo<object>().Subject;
            var properties = value.GetType().GetProperties();
            properties.Should().HaveCount(2);
            properties.All(p => p.PropertyType == typeof(string)).Should().BeTrue();
        }

        [Fact]
        public async Task ExternalLogin_ValidTokensAndUserDoesntHaveLogin_ReturnsOk()
        {
            //Arrange
            var provider = "Google";
            var idToken = _dataGenerator.SeedGoogleIdToken();
            var accessToken = _dataGenerator.SeedToken();
            var httpContext = _authController.ControllerContext.HttpContext;

            httpContext.Request.Headers.Authorization = $"Bearer {accessToken}";
            httpContext.Request.Headers["Authentication"] = idToken;


            _idTokenHandlerMock.Setup(x => x.GetClaimsPrincipalFromIdToken(It.IsAny<string>()))
                .Returns(It.IsAny<ClaimsPrincipal>);
            _signInManagerMock.SetupSequence(x => x.ExternalLoginSignInAsync(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed)
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success); 
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser());
            _userManagerMock.Setup(x => x.AddLoginAsync(It.IsAny<ApplicationUser>(), It.IsAny<ExternalLoginInfo>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _signInManagerMock.Setup(x => x.UpdateExternalAuthenticationTokensAsync(It.IsAny<ExternalLoginInfo>()))
                .ReturnsAsync(IdentityResult.Success);

            //Act

            var result = await _authController.ExternalLogin(provider) as OkObjectResult;

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var value = result.Value.Should().BeAssignableTo<object>().Subject;
            var properties = value.GetType().GetProperties();
            properties.Should().HaveCount(2);
            properties.All(p => p.PropertyType == typeof(string)).Should().BeTrue();
        }

        [Fact]
        public async Task ExternalLogin_ValidTokensAndUserDoesntExists_ReturnsOk()
        {
            //Arrange
            Random random = new Random();
            var possibleProviders = new[] { "Google", "Facebook" };
            var provider = possibleProviders[random.Next(possibleProviders.Length)];
            var idToken = _dataGenerator.SeedGoogleIdToken();
            var accessToken = _dataGenerator.SeedToken();
            var httpContext = _authController.ControllerContext.HttpContext;

            httpContext.Request.Headers.Authorization = $"Bearer {accessToken}";
            httpContext.Request.Headers["Authentication"] = idToken;


            _idTokenHandlerMock.Setup(x => x.GetClaimsPrincipalFromIdToken(It.IsAny<string>()))
                .Returns(It.IsAny<ClaimsPrincipal>);
            _signInManagerMock.SetupSequence(x => x.ExternalLoginSignInAsync(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed)
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddLoginAsync(It.IsAny<ApplicationUser>(), It.IsAny<ExternalLoginInfo>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _signInManagerMock.Setup(x => x.UpdateExternalAuthenticationTokensAsync(It.IsAny<ExternalLoginInfo>()))
                .ReturnsAsync(IdentityResult.Success);

            //Act

            var result = await _authController.ExternalLogin(provider) as OkObjectResult;

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var value = result.Value.Should().BeAssignableTo<object>().Subject;
            var properties = value.GetType().GetProperties();
            properties.Should().HaveCount(2);
            properties.All(p => p.PropertyType == typeof(string)).Should().BeTrue();
        }

        [Fact]
        public async Task ExternalLogin_NullProvider_ReturnsBadRequest()
        {
            //Arrange
            string provider = null;

            //Act
            Func<Task> action = async () => await _authController.ExternalLogin(provider);
            var result = await _authController.ExternalLogin(provider) as BadRequestObjectResult;

            //Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<HttpRequestException>();
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeAssignableTo<string>();
        }

        [Fact]
        public async Task ExternalLogin_InvalidProvider_ReturnsBadRequest()
        {
            //Arrange
            string provider = _dataGenerator.SeedInvalidProvider();

            //Act
            Func<Task> action = async () => await _authController.ExternalLogin(provider);
            var result = await _authController.ExternalLogin(provider) as BadRequestObjectResult;

            //Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<HttpRequestException>();
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeAssignableTo<string>();
        }

        [Fact]
        public async Task ExternalLogin_NullIdToken_ReturnsBadRequest()
        {
            //Arrange
            Random random = new Random();
            var possibleProviders = new[] { "Google", "Facebook" };
            var provider = possibleProviders[random.Next(possibleProviders.Length)];
            var accessToken = _dataGenerator.SeedToken();
            var httpContext = _authController.ControllerContext.HttpContext;

            httpContext.Request.Headers["Authentication"] = StringValues.Empty;
            httpContext.Request.Headers.Authorization = $"Bearer {accessToken}";

            //Act
            Func<Task> action = async () => await _authController.ExternalLogin(provider);
            var result = await _authController.ExternalLogin(provider) as BadRequestObjectResult;

            //Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<HttpRequestException>();
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeAssignableTo<string>();
        }

        [Fact]
        public async Task ExternalLogin_NullAccessToken_ReturnsBadRequest()
        {
            //Arrange
            Random random = new Random();
            var possibleProviders = new[] { "Google", "Facebook" };
            var provider = possibleProviders[random.Next(possibleProviders.Length)];
            var idToken = _dataGenerator.SeedGoogleIdToken();
            var httpContext = _authController.ControllerContext.HttpContext;

            httpContext.Request.Headers.Authorization = StringValues.Empty;
            httpContext.Request.Headers["Authentication"] = idToken;

            //Act
            Func<Task> action = async () => await _authController.ExternalLogin(provider);
            var result = await _authController.ExternalLogin(provider) as BadRequestObjectResult;

            //Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<HttpRequestException>();
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeAssignableTo<string>();
        }

        [Fact]
        public async Task ExternalLogin_NullClaimsPrincipal_ReturnsBadRequest()
        {
            //Arrange
            Random random = new Random();
            var possibleProviders = new[] { "Google", "Facebook" };
            var provider = possibleProviders[random.Next(possibleProviders.Length)];
            var accessToken = _dataGenerator.SeedToken();
            var idToken = _dataGenerator.SeedGoogleIdToken();
            var httpContext = _authController.ControllerContext.HttpContext;

            httpContext.Request.Headers.Authorization = $"Bearer {accessToken}";
            httpContext.Request.Headers["Authentication"] = StringValues.Empty;

            _idTokenHandlerMock.Setup(x => x.GetClaimsPrincipalFromIdToken(It.IsAny<string>()))
                .Returns((ClaimsPrincipal)null);

            //Act
            Func<Task> action = async () => await _authController.ExternalLogin(provider);
            var result = await _authController.ExternalLogin(provider) as BadRequestObjectResult;
            var tokenHandlerResult = _idTokenHandler.GetClaimsPrincipalFromIdToken(idToken);

            //Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<HttpRequestException>();
            result.Should().NotBeNull();
            tokenHandlerResult.Should().BeNull();
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeAssignableTo<string>();
        }

        [Fact]
        public async Task ExternalLogin_NullProviderKey_ReturnsBadRequest()
        {
            //Arrange
            Random random = new Random();
            var possibleProviders = new[] { "Google", "Facebook" };
            var provider = possibleProviders[random.Next(possibleProviders.Length)];
            var idToken = _dataGenerator.SeedGoogleIdTokenWithoutProviderKey();
            var accessToken = _dataGenerator.SeedToken();
            var httpContext = _authController.ControllerContext.HttpContext;

            httpContext.Request.Headers.Authorization = $"Bearer {accessToken}";
            httpContext.Request.Headers["Authentication"] = idToken;

            _idTokenHandlerMock.Setup(x => x.GetClaimsPrincipalFromIdToken(idToken))
                .Returns((ClaimsPrincipal)null);

            //Act
            Func<Task> action = async () => await _authController.ExternalLogin(provider);
            var result = await _authController.ExternalLogin(provider) as BadRequestObjectResult;

            //Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Provider key cannot be null.");
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeAssignableTo<string>();
        }

        [Fact]
        public async Task ExternalLogin_NullUserInfo_ReturnsBadRequest()
        {
            //Arrange
            Random random = new Random();
            var possibleProviders = new[] { "Google", "Facebook" };
            var provider = possibleProviders[random.Next(possibleProviders.Length)];
            string idToken = _dataGenerator.SeedGoogleIdTokenWithoutProviderKey();
            string accessToken = _dataGenerator.SeedToken();
            var httpContext = _authController.ControllerContext.HttpContext;

            httpContext.Request.Headers.Authorization = $"Bearer {accessToken}";
            httpContext.Request.Headers["Authentication"] = idToken;

            _idTokenHandlerMock.Setup(x => x.GetClaimsPrincipalFromIdToken(idToken))
                .Returns(It.IsAny<ClaimsPrincipal>());

            //Act
            Func<Task> action = async () => await _authController.ExternalLogin(provider);
            var result = await _authController.ExternalLogin(provider) as BadRequestObjectResult;

            //Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<ArgumentNullException>().WithMessage("User info cannot be null.");
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeAssignableTo<string>();
        }

        [Fact]
        public async Task ExternalLogin_NullEmail_ReturnsBadRequest()
        {
            //Arrange
            Random random = new Random();
            var possibleProviders = new[] { "Google", "Facebook" };
            var provider = possibleProviders[random.Next(possibleProviders.Length)];
            var idToken = _dataGenerator.SeedGoogleIdTokenWithoutEmail();
            var accessToken = _dataGenerator.SeedToken();
            var httpContext = _authController.ControllerContext.HttpContext;

            httpContext.Request.Headers.Authorization = $"Bearer {accessToken}";
            httpContext.Request.Headers["Authentication"] = idToken;

            _idTokenHandlerMock.Setup(x => x.GetClaimsPrincipalFromIdToken(It.IsAny<string>()))
                .Returns(It.IsAny<ClaimsPrincipal>);
            _signInManagerMock.Setup(x => x.ExternalLoginSignInAsync(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            //Act
            Func<Task> action = async () => await _authController.ExternalLogin(provider);
            var result = await _authController.ExternalLogin(provider) as BadRequestObjectResult;

            //Assert
            action.Should().NotBeNull();
            action.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Email claim not received from {provider}");
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeAssignableTo<string>();
        }
    }
}
