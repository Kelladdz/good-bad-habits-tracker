using good_bad_habits_tracker_api.Core.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.DTOs
{
    public  class HabitDto
    {
        public string? Name { get; set; }
        public Guid UserId { get; set; }
        public bool IsGood { get; set; }
        public bool? IsGoalInTime { get; set; }
        public byte? Quantity { get; set; }
        public string? Frequency { get; set; }
        public bool? IsRepeatDaily { get; set; }
        public string[]? RepeatDaysOfWeek { get; set; }
        public byte[]? RepeatDaysOfMonth { get; set; }
        public DateOnly StartDate { get; set; }
        public TimeOnly ReminderTime { get; set; }
    }
}
