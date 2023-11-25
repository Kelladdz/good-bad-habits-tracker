using GoodBadHabitsTracker.Core.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Domain.IdentityModels
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ICollection<Habit> Habits { get; set; } = new List<Habit>();
        public string? Avatar { get; set; }
    }
}
