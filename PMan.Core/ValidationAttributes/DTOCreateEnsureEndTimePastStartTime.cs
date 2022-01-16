using PMan.Core.DTOModels;
using PMan.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMan.Core.ValidationAttributes
{
    public class Issue_CreateEnsureEndTimePastStartTime : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var issue = validationContext.ObjectInstance as DTOIssue;
            if (!issue.ValidateEndTimePastStartTime())
                return new ValidationResult("End time has to be after start time.");

            return ValidationResult.Success;
        }
    }
}
