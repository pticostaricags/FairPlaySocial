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
    public partial class ProhibitHashTagsAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value != null)
            {
                var hashTags = HashTagsRegex().Match(value!.ToString()!);
                if (hashTags.Captures?.Count > 0)
                    return false;
            }
            return true;
        }

        [GeneratedRegex("\\#\\w+")]
        private static partial Regex HashTagsRegex();
    }
}
