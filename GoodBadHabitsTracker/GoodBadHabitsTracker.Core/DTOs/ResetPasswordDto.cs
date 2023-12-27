using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.DTOs
{
    public class ResetPasswordDto
    {
        public string? Password { get; set; }
        public string? Token { get; set; }
        public string? UserId { get; set; }
    }
}
