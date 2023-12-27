using GoodBadHabitsTracker.API.Controllers.v1;
using GoodBadHabitsTracker.API.Services.EmailSender;
using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using GoodBadHabitsTracker.Core.DTOs;
using GoodBadHabitsTracker.Core.Services.UserService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Text;
using System.Net;
using System.Security.Claims;

namespace GoodBadHabitsTracker.API.Controllers
{
    [Route("API/[controller]/[action]")]
    public class AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment environment, ICustomEmailSender<ApplicationUser> emailSender, IHttpContextAccessor httpContextAccessor) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Register
            ([FromBody]RegisterDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new ApplicationUser()
            {
                Email = request.Email,
                UserName = request.Name,
            };
            IdentityResult result = await userManager.CreateAsync(user, request.Password!);
            if (!result.Succeeded) return BadRequest(result.Errors);

            emailSender.SendWelcomeMessageAsync(user, user.Email);
            return new CreatedAtRouteResult("GetUserById", new {userId = user.Id}, user);
        }

        [HttpPost]
        public async Task<IActionResult> Login
            ([FromBody] LoginDto request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            var userName = user.UserName;
            signInManager.AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            var result = await signInManager.PasswordSignInAsync(userName, request.Password, isPersistent: true, lockoutOnFailure: false);

            if (!result.Succeeded) return new UnauthorizedResult();

            Response.Cookies.Append("Logged", "true");

            return new OkResult();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("Logged");
            return new OkResult();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword([FromBody]ForgetPasswordDto request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null) return new NotFoundResult();
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var link = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme)!;
            await emailSender.SendPasswordResetLinkAsync(user, user.Email, link, token);
            return new OkResult();
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordDto request)
        {
            var user = await userManager.FindByIdAsync(request.UserId);
            if (user == null) return new BadRequestResult();
            IdentityResult result = await userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (!result.Succeeded) return new UnauthorizedResult();
            return new OkResult();
        }
        [HttpPut("image")]
        public async Task<IActionResult> UploadImage([FromForm]IFormFile image, string userName)
        {
            var uploadedFiles = Request.Form.Files;
            try
            {                
                string filePath = environment.WebRootPath + "\\Uploads\\User\\" + userName;

                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                    string imagePath = filePath + ".png";

                    if (!System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

                    using(FileStream stream = System.IO.File.Create(imagePath))
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
