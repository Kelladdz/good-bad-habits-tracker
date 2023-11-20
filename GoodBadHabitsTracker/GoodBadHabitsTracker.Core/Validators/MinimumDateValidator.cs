using System.ComponentModel.DataAnnotations;

namespace good_bad_habits_tracker_api.Core.Validators
{
    public class MinimumDateValidator : ValidationAttribute
    {
        public DateOnly MinimumDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string DefaultErrorMessage { get; set; } = "Start date should be in future or today.";


        public MinimumDateValidator() { }

        public MinimumDateValidator(DateOnly minimumDate)
        {
            MinimumDate = minimumDate;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                DateOnly date = (DateOnly)value;
                if (date < MinimumDate)
                {
                    return new ValidationResult(string.Format(ErrorMessage ?? DefaultErrorMessage));
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            return null;
        }
    }
}
