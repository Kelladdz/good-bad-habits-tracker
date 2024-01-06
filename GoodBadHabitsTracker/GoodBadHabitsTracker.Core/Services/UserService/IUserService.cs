
using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using GoodBadHabitsTracker.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Services.UserService
{
     public interface IUserService
    {
        string GetExternalLoginProvider(Guid userId);
    }
}
