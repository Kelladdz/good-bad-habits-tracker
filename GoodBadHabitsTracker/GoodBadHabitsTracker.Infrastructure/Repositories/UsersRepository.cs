using GoodBadHabitsTracker.Core.Domain.Interfaces;
using GoodBadHabitsTracker.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Infrastructure.Repositories
{
    public class UsersRepository(HabitsDbContext dbContext) : IUsersRepository
    {
        public string GetExternalLoginProvider(Guid userId)
        {
            var userLogin = dbContext.UserLogins.Find(userId);
            var loginProvider = userLogin?.LoginProvider;
            return loginProvider;
        }
    }
}
