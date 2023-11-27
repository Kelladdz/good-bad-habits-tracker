using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Domain.Models
{
    public class Stats
    {
        public int Streak { get; set; }
        public int Complete { get; set; }
        public int Failed { get; set; }
        public int Skipped { get; set; }
        public int Total { get; set; }
    }
}
