using PMan.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMan.Core.ValidationAttributes
{
    public class Issue_EnsureStartTimeFuture : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var issue = validationContext.ObjectInstance as Issue;
            if (!issue.ValidateStartTimeFuture())
                return new ValidationResult("Start time has to be in future.");

            return ValidationResult.Success;
        }
    }
}
