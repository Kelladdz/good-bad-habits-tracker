using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Domain.IdentityModels
{
    public record UserSession(Guid? Id, string? Name, string? Email, string? Role);
}
