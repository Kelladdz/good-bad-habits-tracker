using Asp.Versioning;
using Auth0.AuthenticationApi.Models;
using Azure.Core;
using GoodBadHabitsTracker.API.Exceptions;
using GoodBadHabitsTracker.API.Services;
using GoodBadHabitsTracker.API.Services.EmailSender;
using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using GoodBadHabitsTracker.Core.DTOs;
using GoodBadHabitsTracker.Core.Services.UserAccessor;
using GoodBadHabitsTracker.Core.Services.UserService;
using GoodBadHabitsTracker.Infrastructure.Services.IdTokenHandler;
using GoodBadHabitsTracker.Infrastructure.Services.JwtTokenHandler;
using GoodBadHabitsTracker.Infrastructure.Services.JwtTokenRevoker;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core.Tokenizer;
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
        private readonly IIDTokenHandler _idTokenHandler;
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly IJwtTokenRevoker _jwtTokenRevoker;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment environment, ICustomEmailSender<ApplicationUser> emailSender, IIDTokenHandler idTokenHandler, IJwtTokenHandler jwtTokenHandler, IJwtTokenRevoker jwtTokenRevoker)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
            _emailSender = emailSender;
            _idTokenHandler = idTokenHandler;
            _jwtTokenHandler = jwtTokenHandler;
            _jwtTokenRevoker = jwtTokenRevoker;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register
            ([FromBody] RegisterDto request)
        {
            try
            {
                if (request is null) throw new HttpRequestException("Request cannot be null.");
                if (!ModelState.IsValid) throw new ArgumentException("User data is invalid.");

                var user = new ApplicationUser()
                {
                    Email = request.Email,
                    UserName = request.Name,
                    PasswordHash = request.Password,
                };

                IdentityResult result = await _userManager.CreateAsync(user, request.Password!);
                if (!result.Succeeded) throw new ConflictException("This name or email exists.");

                await _userManager.AddToRoleAsync(user, "User");

                await _emailSender.SendWelcomeMessageAsync(user, user.Email);

                return CreatedAtRoute("GetUserById", new { userId = user.Id }, user);
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
                if (request is null) throw new HttpRequestException("Request cannot be null.");

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user is null) throw new InvalidCredentialException("Invalid email or password");

                var checkPasswordResult = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!checkPasswordResult) throw new InvalidCredentialException("Invalid email or password");

                var getUserRole = await _userManager.GetRolesAsync(user);
                if (getUserRole.Count == 0)
                {
                    var result = await _userManager.AddToRoleAsync(user, "User");
                    if (!result.Succeeded) throw new InvalidOperationException("User role cannot be added.");
                }
                var userSession = new UserSession(user.Id, user.UserName, user.Email, getUserRole[0]);

                var accessToken = _jwtTokenHandler.GenerateAccessToken(userSession, out string userFingerprint);
                if (accessToken is null) throw new InvalidOperationException("Access token cannot be null.");

                var refreshToken = _jwtTokenHandler.GenerateRefreshToken();
                if (refreshToken is null) throw new InvalidOperationException("Refresh token cannot be null.");

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpirationDate = DateTime.UtcNow.AddDays(7);
                var userUpdateResult = await _userManager.UpdateAsync(user);
                if (!userUpdateResult.Succeeded) throw new InvalidOperationException("User update failed.");

                Response.Cookies.Append("__Secure-Fgp", userFingerprint, new CookieOptions
                {
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = true,
                    Secure = true,
                    MaxAge = TimeSpan.FromMinutes(15),
                });
                return Ok(new { accessToken = accessToken.ToString(), refreshToken = refreshToken });
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException) return BadRequest(ex.Message);
                if (ex is InvalidCredentialException) return Unauthorized(ex.Message);
                if (ex is InvalidOperationException) return BadRequest(ex.Message);
                else return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("token/refresh")]
        public async Task<IActionResult> NewRefreshToken([FromBody]NewRefreshTokenDto request)
        {
            if (request is null) throw new HttpRequestException("Request cannot be null.");
            
            var principal = _jwtTokenHandler.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal is null) throw new InvalidOperationException("Principal cannot be null.");

            var userId = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) throw new InvalidOperationException("User id cannot be null.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) throw new InvalidOperationException("User cannot be null.");
                
            if (user.RefreshToken != request.RefreshToken ||
                user.RefreshTokenExpirationDate <= DateTime.UtcNow) throw new UnauthorizedAccessException("Refresh Token is invalid.");

            var getUserRole = await _userManager.GetRolesAsync(user);
            var userSession = new UserSession(user.Id, user.UserName, user.Email, getUserRole[0]);
            var newAccessToken = _jwtTokenHandler.GenerateAccessToken(userSession, out string userFingerprint);
            if (newAccessToken is null) throw new InvalidOperationException("New access token cannot be null.");

            var newRefreshToken = _jwtTokenHandler.GenerateRefreshToken();
            if (newRefreshToken is null) throw new InvalidOperationException("New refresh token cannot be null.");

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpirationDate = DateTime.UtcNow.AddDays(7);
            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded) throw new InvalidOperationException("User update failed.");

            Response.Cookies.Append("__Secure-Fgp", userFingerprint, new CookieOptions
            {
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                Secure = true,
                MaxAge = TimeSpan.FromMinutes(15),
            });
            return Ok(new { accessToken = newAccessToken.ToString(), refreshToken = newRefreshToken });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromHeader(Name = "Authorization")] string authorization)
        {
            if (string.IsNullOrWhiteSpace(authorization)) return NoContent();
            string bearerToken = authorization.Replace("Bearer ", "", StringComparison.InvariantCultureIgnoreCase);
            await _jwtTokenRevoker.SignOutAsync(new JwtSecurityToken(bearerToken));
            return Ok();
        }

        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromQuery] string provider)
        {
            try
            {
                if (provider == null || (provider != "Google" && provider != "Facebook")) throw new HttpRequestException("Provider is not correct.");

                var idToken = Request.Headers["Authentication"].ToString();
                if (idToken == "") throw new HttpRequestException("Id token cannot be empty.");

                var accessToken = Request.Headers["Authorization"].ToString();
                if (accessToken == "") throw new HttpRequestException("Access token cannot be empty.");

                var claimsPrincipal = _idTokenHandler.GetClaimsPrincipalFromIdToken(idToken);
                if (claimsPrincipal == null) throw new InvalidOperationException("Claims principal cannot be null.");

                var providerKey = claimsPrincipal.FindFirst(claim => claim.Type == "sub")?.Value;
                if (providerKey == null) throw new InvalidOperationException("Provider key cannot be null.");

                var userInfo = new ExternalLoginInfo(claimsPrincipal, provider, providerKey, provider);
                if (userInfo == null) throw new ArgumentNullException("User info cannot be null.");
                userInfo.AuthenticationTokens = new List<AuthenticationToken>()
                {
                    new AuthenticationToken(){ Name = "access_token", Value = accessToken},
                    new AuthenticationToken(){ Name = "id_token", Value = idToken}
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
                        SameSite = SameSiteMode.Lax,
                        IsEssential = true,
                        Expires = DateTimeOffset.UtcNow.AddHours(2)
                    });
                    return Ok(new { user.UserName, user.Email });
                }
                else
                {
                    var email = claimsPrincipal.FindFirst(claim => string.Equals(claim.Type, "email"))?.Value;
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
                        Response.Cookies.Append("ONSESS", "true", new CookieOptions
                        {
                            HttpOnly = false,
                            Secure = true,
                            SameSite = SameSiteMode.Lax,
                            IsEssential = true,
                            Expires = DateTimeOffset.UtcNow.AddHours(2)
                        });
                        return Ok(new { user.UserName, user.Email});
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

        [HttpPost("external-logout")]
        public async Task<IActionResult> ExternalLogout()
        {
            var provider = User.Claims.FirstOrDefault(claim => claim.Type == "loginProvider").Value;
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            if (userId == null) return NotFound();
            var user = await _userManager.FindByIdAsync(userId);    
            if (user == null) return NotFound();
            await _signInManager.SignOutAsync();
            await _userManager.RemoveAuthenticationTokenAsync(user, provider, "access_token");
            await _userManager.RemoveAuthenticationTokenAsync(user, provider, "id_token");
            return Ok();
        }

        [HttpGet("external-logout-callback")]
        public async Task<IActionResult> ExternalLogoutCallback()
        {
            return Ok();
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
