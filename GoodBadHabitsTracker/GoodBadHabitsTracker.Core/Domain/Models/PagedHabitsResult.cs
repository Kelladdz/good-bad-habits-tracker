using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Domain.Models
{
    public class PagedHabitsResult
    {
        public IEnumerable<Habit> Habits { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
