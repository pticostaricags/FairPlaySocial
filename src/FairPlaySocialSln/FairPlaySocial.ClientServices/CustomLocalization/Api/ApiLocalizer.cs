﻿using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.ClientServices.CustomLocalization.Api
{
    public class ApiLocalizer : IStringLocalizer
    {
        private readonly LocalizationClientService _localizationClientService;

        public ApiLocalizer(LocalizationClientService localizationClientService)
        {
            _localizationClientService = localizationClientService;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            return new ApiLocalizer(_localizationClientService);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures)
        {
            return _localizationClientService.GetAllStrings()!;
        }

        private string GetString(string name)
        {
            return _localizationClientService.GetString(null, name)!;
        }
    }
}
