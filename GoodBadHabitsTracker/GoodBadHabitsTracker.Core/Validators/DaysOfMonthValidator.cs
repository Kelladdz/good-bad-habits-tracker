using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBadHabitsTracker.Core.Validators
{
    public class DaysOfMonthValidator : ValidationAttribute
    {

        public uint[] DaysOfMonth = Enumerable.Range(1, 31).ToArray().Select(element => (uint)element).ToArray();
        public string DefaultErrorMessage { get; set; } = "Days of month have to be correct.";

        public DaysOfMonthValidator() { }

        public DaysOfMonthValidator(uint[] daysOfMonth)
        {
            DaysOfMonth = daysOfMonth;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                uint[] days = (uint[])value;
                if (days.All(element => DaysOfMonth.Contains(element)) || days.Length == 0)
                    return ValidationResult.Success;
                else return new ValidationResult(string.Format(ErrorMessage ?? DefaultErrorMessage));
            }
            return null;
        }
    }
}
