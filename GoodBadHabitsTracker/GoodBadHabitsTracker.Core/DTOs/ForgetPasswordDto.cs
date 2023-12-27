using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.DTOs
{
    public class ForgetPasswordDto
    {
        [EmailAddress]
        public string? Email { get; set; }
    }
}
