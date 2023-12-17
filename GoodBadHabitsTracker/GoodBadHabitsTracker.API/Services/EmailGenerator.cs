using GoodBadHabitsTracker.Core.Domain.IdentityModels;

namespace GoodBadHabitsTracker.API.Services
{
    public class EmailGenerator(string path)
    {
        public string GenerateWelcomeEmailBody(ApplicationUser user) => File.ReadAllText(path);
    }
}
