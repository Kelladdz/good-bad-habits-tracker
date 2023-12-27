using GoodBadHabitsTracker.Core.Domain.IdentityModels;

namespace GoodBadHabitsTracker.API.Services.EmailSender
{
    public interface ICustomEmailSender<TUser> where TUser : class
    {
        Task SendWelcomeMessageAsync(TUser user, string email);
        Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink, string token);
    }
}
