using Asp.Versioning;
using Auth0.AuthenticationApi.Models;
using GoodBadHabitsTracker.API.Exceptions;
using GoodBadHabitsTracker.API.Services;
using GoodBadHabitsTracker.API.Services.EmailSender;
using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using GoodBadHabitsTracker.Core.DTOs;
using GoodBadHabitsTracker.Core.Services.UserAccessor;
using GoodBadHabitsTracker.Core.Services.UserService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Authentication;
using System.Security.Claims;


namespace GoodBadHabitsTracker.API.Controllers.v1
{
    [Route("api/[controller]")]  
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ICustomEmailSender<ApplicationUser> _emailSender;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment environment, ICustomEmailSender<ApplicationUser> emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
            _emailSender = emailSender;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register
            ([FromBody] RegisterDto request)
        {
            try
            {
                if (request == null) throw new HttpRequestException("Request cannot be null.");
                if (!ModelState.IsValid) throw new ArgumentException("User data is invalid.");

                var user = new ApplicationUser()
                {
                    Email = request.Email,
                    UserName = request.Name,
                };

                IdentityResult result = await _userManager.CreateAsync(user, request.Password!);
                if (!result.Succeeded) throw new ConflictException("This name or email exists.");

                await _emailSender.SendWelcomeMessageAsync(user, user.Email);

                return CreatedAtRoute("GetUserById", new { userId = user.UserName }, user);
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException) return BadRequest(ex.Message);
                if (ex is ArgumentException) return BadRequest(ex.Message);
                if (ex is ConflictException) return Conflict(ex.Message);
                else return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login
            ([FromBody] LoginDto request)
        {
            try
            {
                if (request == null) throw new HttpRequestException("Request cannot be null.");

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null) throw new InvalidCredentialException("Invalid email or password");

                _signInManager.AuthenticationScheme = "CookiesAuth";
                var result = await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent: true, lockoutOnFailure: false);
                if (!result.Succeeded) throw new InvalidCredentialException("Invalid email or password");

                return Ok(new {user.UserName, user.Email});
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException) return BadRequest(ex.Message);
                if (ex is InvalidCredentialException) return Unauthorized(ex.Message);
                else return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromQuery] string provider)
        {
            try
            {
                if (provider == null || (provider != "Google" && provider != "Facebook")) throw new HttpRequestException("Provider is not correct.");

                var idToken = Request.Headers["Authentication"].ToString();
                if (idToken == null) throw new HttpRequestException("Id token cannot be null.");

                var accessToken = Request.Headers["Authorization"].ToString();
                if (accessToken == null) throw new HttpRequestException("Access token cannot be null.");

                var tokenHandler = new IdTokenHandler();
                var claimsPrincipal = tokenHandler.GetClaimsPrincipalFromIdToken(idToken);

                var providerKey = claimsPrincipal.FindFirst(claim => claim.Type == "sub").Value;
                if (providerKey == null) throw new InvalidOperationException("Provider key cannot be null.");

                var userInfo = new ExternalLoginInfo(claimsPrincipal, provider, providerKey, provider);
                if (userInfo == null) throw new ArgumentNullException("User info cannot be null.");
                userInfo.AuthenticationTokens = new List<AuthenticationToken>()
                {
                    new AuthenticationToken(){ Name = "access_token", Value = accessToken},
                };

                
                var result = await _signInManager.ExternalLoginSignInAsync(provider, providerKey, isPersistent: false, bypassTwoFactor: true);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByLoginAsync(provider, providerKey);
                    if (user == null) throw new ArgumentNullException("User cannot be null.");
                    await _signInManager.UpdateExternalAuthenticationTokensAsync(userInfo);
                    Response.Cookies.Append("ONSESS", "true", new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        IsEssential = true,
                        Expires = DateTimeOffset.UtcNow.AddHours(2)
                    });
                    return Ok();
                }
                else
                {
                    var email = claimsPrincipal.FindFirst(claim => string.Equals(claim.Type, "email"))!.Value;
                    if (email != null)
                    {
                        var user = await _userManager.FindByEmailAsync(email);
                        if (user == null)
                        {
                            user = new ApplicationUser
                            {
                                UserName = claimsPrincipal.FindFirst(claim => string.Equals(claim.Type, "name"))!.Value,
                                Email = claimsPrincipal.FindFirst(claim => string.Equals(claim.Type, "email"))!.Value,
                                ImageUrl = claimsPrincipal.FindFirst(claim => string.Equals(claim.Type, "picture"))!.Value
                            };
                            await _userManager.CreateAsync(user);
                            await _userManager.AddClaimAsync(user, new Claim("loginProvider", provider));
                        }
                        await _userManager.AddLoginAsync(user, userInfo);
                        await _userManager.AddToRoleAsync(user, "User");
                        await _signInManager.ExternalLoginSignInAsync(provider, providerKey, isPersistent: false, bypassTwoFactor: true);
                        await _signInManager.UpdateExternalAuthenticationTokensAsync(userInfo);
                        return Ok();
                    }
                    else throw new InvalidOperationException($"Email claim not received from {userInfo.LoginProvider}");
                }
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException) return BadRequest(ex.Message);
                if (ex is HttpRequestException) return BadRequest(ex.Message);
                if (ex is InvalidOperationException) return BadRequest(ex.Message);
                else return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
         }   


        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback()
        
        {
            
            var userInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (userInfo == null) return BadRequest(ModelState);

            var result = await _signInManager.ExternalLoginSignInAsync(userInfo.LoginProvider, userInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByLoginAsync(userInfo.LoginProvider, userInfo.ProviderKey);
                if (user == null) return BadRequest();
                await _signInManager.UpdateExternalAuthenticationTokensAsync(userInfo);
                return Ok();
            }

            else
            {
                var email = userInfo.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        if (userInfo.LoginProvider == "Google")
                        {
                            user = new ApplicationUser
                            {
                                UserName = userInfo.Principal.FindFirstValue(ClaimTypes.Name),
                                Email = userInfo.Principal.FindFirstValue(ClaimTypes.Email),
                                ImageUrl = userInfo.Principal.FindFirst(claim => claim.Type == "image").Value
                            };
                        }
                        if (userInfo.LoginProvider == "Facebook")
                        {
                            var identifier = userInfo.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                            user = new ApplicationUser
                            {
                                UserName = userInfo.Principal.FindFirstValue(ClaimTypes.Name),
                                Email = userInfo.Principal.FindFirstValue(ClaimTypes.Email),
                                ImageUrl = $"https://graph.facebook.com/{identifier}/picturetype=album"
                            };
                        }
                        await _userManager.CreateAsync(user);
                        await _userManager.AddClaimAsync(user, new Claim("loginProvider", userInfo.LoginProvider));
                    }
                    await _userManager.AddLoginAsync(user, userInfo);
                    result = await _signInManager.ExternalLoginSignInAsync(userInfo.LoginProvider, userInfo.ProviderKey, isPersistent: true, bypassTwoFactor: true);
                    if (result.Succeeded)
                    {
                        await _signInManager.UpdateExternalAuthenticationTokensAsync(userInfo);
                        Redirect("https://localhost:8080");
                        return Ok();
                    }
                    /*await signInManager.SignInAsync(user, isPersistent: true, GoogleDefaults.AuthenticationScheme);*/
                    
                }
                return BadRequest($"Email claim not received from {userInfo.LoginProvider}");
            }
        }

        [HttpGet("external-logout")]
        public async Task<IActionResult> ExternalLogout()
        {

            var provider = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.AuthenticationMethod).Value;
            if (provider == null) return NoContent();
            var redirectUrl = Url.Action(nameof(ExternalLogoutCallback), "Auth");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(properties);
        }

        [HttpGet("external-logout-callback")]
        public async Task<IActionResult> ExternalLogoutCallback()
        {
            var userInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (userInfo.LoginProvider == "Google") _signInManager.AuthenticationScheme = GoogleDefaults.AuthenticationScheme;
            if (userInfo.LoginProvider == "Facebook") _signInManager.AuthenticationScheme = FacebookDefaults.AuthenticationScheme;

            var user = await _userManager.FindByLoginAsync(userInfo.LoginProvider, userInfo.ProviderKey);
            if (user == null) return BadRequest(ModelState);
            var result = await _userManager.RemoveLoginAsync(user, userInfo.LoginProvider, userInfo.ProviderKey);
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            Response.Cookies.Delete("Logged");
            if (!result.Succeeded) return new BadRequestResult();
            return new RedirectResult("https://localhost:8080/signin");
        }

        /*[HttpGet]
        public IActionResult GoogleLogin([FromQuery] string provider)
        {
            var redirectUrl = Url.Action("GoogleLoginCallback", "Account");
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            signInManager.AuthenticationScheme = GoogleDefaults.AuthenticationScheme;
            var userInfo = await signInManager.GetExternalLoginInfoAsync();
            if (userInfo == null) return BadRequest(ModelState);
            var result = await signInManager.ExternalLoginSignInAsync(userInfo.LoginProvider, userInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded) return new RedirectResult("https://localhost:8080");
            else
            {
                var email = userInfo.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = await userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = userInfo.Principal.FindFirstValue(ClaimTypes.Name),
                            Email = userInfo.Principal.FindFirstValue(ClaimTypes.Email),
                            ImageUrl = userInfo.Principal.FindFirst(claim => claim.Type == "image").Value
                        };
                        await userManager.CreateAsync(user);
                    }
                    await userManager.AddLoginAsync(user, userInfo);
                    Response.Cookies.Append("Logged", "true");
                    Redirect("https://localhost:8080");
                    return new RedirectResult("https://localhost:8080");
                }
                return BadRequest($"Email claim not received from {userInfo.LoginProvider}");
            }
        }

        [HttpGet]
        public IActionResult GoogleLogout([FromQuery] string provider)
        {
            var redirectUrl = Url.Action("GoogleLogoutCallback", "Account");
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleLogoutCallback()
        {
            signInManager.AuthenticationScheme = GoogleDefaults.AuthenticationScheme;
            var userInfo = await signInManager.GetExternalLoginInfoAsync();

            if (userInfo == null) return BadRequest(ModelState);
            var user = await userManager.FindByLoginAsync(userInfo.LoginProvider, userInfo.ProviderKey);
            if (user == null) return BadRequest(ModelState);
            var result = await userManager.RemoveLoginAsync(user, userInfo.LoginProvider, userInfo.ProviderKey);
            if (!result.Succeeded) return new BadRequestResult();
            return new RedirectResult("https://localhost:8080");
        }

        [HttpGet]
        public IActionResult FacebookLogin([FromQuery] string provider)
        {
            var redirectUrl = Url.Action("FacebookLoginCallback", "Account");
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> FacebookLoginCallback()
        {
            signInManager.AuthenticationScheme = FacebookDefaults.AuthenticationScheme;
            var userInfo = await signInManager.GetExternalLoginInfoAsync();
            if (userInfo == null) return BadRequest(ModelState);

            var result = await signInManager.ExternalLoginSignInAsync(userInfo.LoginProvider, userInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded) return new RedirectResult("https://localhost:8080");
            else
            {
                var email = userInfo.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = await userManager.FindByEmailAsync(email);
                    var identifier = userInfo.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = userInfo.Principal.FindFirstValue(ClaimTypes.Name),
                            Email = userInfo.Principal.FindFirstValue(ClaimTypes.Email),
                            ImageUrl = $"https://graph.facebook.com/{identifier}/picturetype=album"
                        };
                        await userManager.CreateAsync(user);


                    }
                    await userManager.AddLoginAsync(user, userInfo);
                    Response.Cookies.Append("Logged", "true");
                    Redirect("https://localhost:8080");
                    return new RedirectResult("https://localhost:8080");
                }
                return BadRequest($"Email claim not received from {userInfo.LoginProvider}");
            }
        }

        [HttpGet]
        public IActionResult FacebookLogout([FromQuery] string provider)
        {
            var redirectUrl = Url.Action("FacebookLogoutCallback", "Account");
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> FacebookLogoutCallback()
        {
            signInManager.AuthenticationScheme = FacebookDefaults.AuthenticationScheme;
            var userInfo = await signInManager.GetExternalLoginInfoAsync();

            if (userInfo == null) return BadRequest(ModelState);
            var user = await userManager.FindByLoginAsync(userInfo.LoginProvider, userInfo.ProviderKey);
            if (user == null) return BadRequest(ModelState);
            var result = await userManager.RemoveLoginAsync(user, userInfo.LoginProvider, userInfo.ProviderKey);
            if (!result.Succeeded) return new BadRequestResult();
            return new RedirectResult("https://localhost:8080");
        }*/

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookiesAuth");
            Response.Cookies.Delete("Logged");
            return new OkResult();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return new NotFoundResult();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var link = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme)!;
            await _emailSender.SendPasswordResetLinkAsync(user, user.Email, link, token);
            return new OkResult();
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return new BadRequestResult();
            IdentityResult result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (!result.Succeeded) return new UnauthorizedResult();
            return new OkResult();
        }
        [HttpPut("image")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image, string userName)
        {
            var uploadedFiles = Request.Form.Files;
            try
            {
                string filePath = _environment.WebRootPath + "\\Uploads\\User\\" + userName;

                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                string imagePath = filePath + ".png";

                if (!System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

                using (FileStream stream = System.IO.File.Create(imagePath))
                {
                    await image.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {

            }
            return Ok();
        }


    }


}
