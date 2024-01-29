using GoodBadHabitsTracker.Core.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Domain.IdentityModels
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; } //google: givenName
        public string? LastName { get; set; } //google: familyName
        public ICollection<Habit>? HabitsList { get; set; } = new List<Habit>();
        public string? ImageUrl { get; set; } //google: imageUrl
        public string? DisplayName { get; set; } //google: name
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpirationDate { get; set; }
        [EmailAddress(ErrorMessage = "This value should be a valid email")]
        public override string? Email { get; set; }
    }
}
