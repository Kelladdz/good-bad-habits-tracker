using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Validators
{
    public class DaysOfWeekValidator : ValidationAttribute
    {
        public string[] DaysOfWeek { get; set; } = ["monday",
            "tuesday",
            "wednesday",
            "thursday",
            "friday",
            "saturday",
            "sunday"];
        public string DefaultErrorMessage { get; set; } = "Days of week have to be correct.";
        public DaysOfWeekValidator() { }

        public DaysOfWeekValidator(string[] daysOfWeek)
        {
            DaysOfWeek = daysOfWeek;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string[] days = (string[])value;
                if (days.All(element => DaysOfWeek.Contains(element)) || days.Length == 0)
                    return ValidationResult.Success;
                else return new ValidationResult(string.Format(ErrorMessage ?? DefaultErrorMessage)); 
            }
            return null;
        }
    }
}
