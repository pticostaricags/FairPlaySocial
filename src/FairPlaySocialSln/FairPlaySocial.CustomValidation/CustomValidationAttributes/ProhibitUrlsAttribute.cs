﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FairPlaySocial.CustomValidation.CustomValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public partial class ProhibitUrlsAttribute: ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value != null)
            {
                var urls = UrlsPatternRegex().Match(value!.ToString()!);
                if (urls.Captures?.Count > 0)
                    return false;
            }
            return true;
        }

        [GeneratedRegex("(http|https|ftp|)\\:\\/\\/[0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*(:(0-9)*)*(\\/?)([a-zA-Z0-9\\-\\.\\?\\,\\'\\/\\\\\\+&%\\$#_]*)?([a-zA-Z0-9\\-\\?\\,\\'\\/\\+&%\\$#_]+)")]
        private static partial Regex UrlsPatternRegex();
    }
}
