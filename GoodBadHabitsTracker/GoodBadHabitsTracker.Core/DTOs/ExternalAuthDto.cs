using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.DTOs
{
    public class ExternalAuthDto
    {
        public string? Provider { get; set; }
        public string? IdToken { get; set; }
    }
}
