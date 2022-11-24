using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FairPlaySocial.CustomValidation.CustomValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ProhibitHasTagsAttribute : ValidationAttribute
    {
        private const string HashTagsPattern = @"\#\w+";
        public override bool IsValid(object? value)
        {
            if (value != null)
            {
                var hashTags = Regex.Match(value!.ToString()!, HashTagsPattern);
                if (hashTags.Captures?.Count > 0)
                    return false;
            }
            return true;
        }
    }
}
