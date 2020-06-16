using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Attributes
{
    public class AfterStartDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is DateTime))
                return new ValidationResult("Wrong");

            var modelType = validationContext.ObjectInstance.GetType();
            var StartDateProperty = modelType.GetProperty("StartDate");
            var startDate = StartDateProperty?.GetValue(validationContext.ObjectInstance, null);
           
            if(startDate == null)
                return new ValidationResult("Wrong");

            if ((DateTime)startDate <= (DateTime)value)
                return ValidationResult.Success;
            return new ValidationResult("End date must be after start date");
        }
    }
}
