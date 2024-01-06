using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Domain.Interfaces
{
    public interface IUsersRepository
    {
        string GetExternalLoginProvider(Guid userId);
    }
}
