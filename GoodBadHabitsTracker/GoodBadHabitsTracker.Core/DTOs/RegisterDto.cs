using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.DTOs
{
    public class RegisterDto
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
