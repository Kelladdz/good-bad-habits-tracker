using good_bad_habits_tracker_api.Core.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Domain.Models
{
    public class Habit
    {
        [Key]
        public Guid HabitId { get; private set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Maximum length of name is 100.")]
        public string? Name { get; set; }
        public Guid UserId { get; set; }
        public bool IsGood { get; set; }
        public bool? IsGoalInTime { get; set; }
        [Range(1, 100, ErrorMessage = "Quantity should be between ${1} and ${2}")]
        public byte? Quantity { get; set; }
        public string? Frequency { get; set; }
        public bool? IsRepeatDaily { get; set; }
        public string[]? RepeatDaysOfWeek { get; set; }
        public byte[]? RepeatDaysOfMonth { get; set; }
        [MinimumDateValidator]
        public DateOnly StartDate { get; set; }
        public TimeOnly ReminderTime { get; set; }

        public void GenerateId() => HabitId = Guid.NewGuid();
    }
}
