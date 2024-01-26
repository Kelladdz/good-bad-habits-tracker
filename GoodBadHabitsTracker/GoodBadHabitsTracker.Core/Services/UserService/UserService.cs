using GoodBadHabitsTracker.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Services.UserService
{
    public class UserService(IUsersRepository usersRepository) : IUserService
    {
        public string GetExternalLoginProvider(Guid userId)
        {
            var loginProvider = usersRepository.GetExternalLoginProvider(userId);
            return loginProvider;
        }

    }
}
