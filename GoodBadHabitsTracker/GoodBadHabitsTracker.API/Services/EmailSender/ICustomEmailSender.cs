using GoodBadHabitsTracker.Core.Domain.IdentityModels;

namespace GoodBadHabitsTracker.API.Services.EmailSender
{
    public interface ICustomEmailSender<TUser> where TUser : class
    {
        Task SendWelcomeMessageAsync(TUser user, string email);
    }
}
