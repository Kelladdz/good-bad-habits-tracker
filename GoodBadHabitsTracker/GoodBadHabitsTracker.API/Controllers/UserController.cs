using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using GoodBadHabitsTracker.Core.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Text;

namespace GoodBadHabitsTracker.API.Controllers
{
    [Route("API/[controller]/[action]")]
    public class UserController(
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        IWebHostEnvironment environment
        ) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<ApplicationUser>> Register
            ([FromBody] ApplicationUserDto request, HttpContext context)
        {
            var emailStore = (IUserEmailStore<ApplicationUser>)userStore;
            var email = request.Email;

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new ApplicationUser();
            await userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await emailStore.SetEmailAsync(user, email, CancellationToken.None);
            var image = 
            var result = await userManager.CreateAsync(user, request.Password);

            if(!result.Succeeded) return BadRequest(result);
            return Ok();
        }

        [HttpPost("image")]
        public async Task<ActionResult> UploadImage(IFormFile image, string imageName)
        {            
            try
            {
                var uploadedFiles = Request.Form.Files;
                foreach(IFormFile source in uploadedFiles)
                {
                    string fileName = source.FileName;
                    string filePath = GetFilePath(fileName);

                    if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                    string imagePath = filePath + "\\image.png";

                    if (!System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

                    using(FileStream stream = System.IO.File.Create(imagePath))
                    {
                        await source.CopyToAsync(stream);
                        results = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Ok(results);
        }

        [NonAction]
        private string GetFilePath(string userName)
        {
            return environment.WebRootPath + "\\Uploads\\User\\" + userName;
        }
    }

   
}
