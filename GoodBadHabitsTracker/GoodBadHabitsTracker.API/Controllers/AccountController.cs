using GoodBadHabitsTracker.API.Controllers.v1;
using GoodBadHabitsTracker.API.Services.EmailSender;
using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using GoodBadHabitsTracker.Core.DTOs;
using GoodBadHabitsTracker.Core.Services.UserService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Text;
using System.Net;

namespace GoodBadHabitsTracker.API.Controllers
{
    [Route("API/[controller]/[action]")]
    public class AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment environment, ICustomEmailSender<ApplicationUser> emailSender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Register
            ([FromBody]ApplicationUserDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new ApplicationUser()
            {
                Email = request.Email,
                UserName = request.Name,
            };
            IdentityResult result = await userManager.CreateAsync(user, request.Password!);
            if (!result.Succeeded) return BadRequest(result.Errors);
            await signInManager.SignInAsync(user, isPersistent: true);

            emailSender.SendWelcomeMessageAsync(user, user.Email);
            return new CreatedAtRouteResult("GetUserById", new {userId = user.Id}, user);
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
