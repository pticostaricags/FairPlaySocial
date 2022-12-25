using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.CustomValidation.CustomValidationAttributes
{
    public class ProhibitDuplicateStrings : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var requiredType = typeof(string?[]).Name;
            if (value is not string?[])
                throw new ValidationException($"Value must implement {requiredType}");
            if (value is null) return true;
            var groupedItems = (value as string?[])!.GroupBy(p => p);
            if (groupedItems.Any(p => p.Count(x => !String.IsNullOrWhiteSpace(x)) > 1)) return false;
            return true;
        }
    }
}
