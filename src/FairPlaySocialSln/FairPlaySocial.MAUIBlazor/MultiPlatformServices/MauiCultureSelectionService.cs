using FairPlaySocial.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.MAUIBlazor.MultiPlatformServices
{
    internal class MauiCultureSelectionService : ICultureSelectionService
    {
        public CultureInfo GetCurrentCulture()
        {
            return CultureInfo.DefaultThreadCurrentUICulture!;
        }

        public void SetCurrentCulture(CultureInfo cultureInfo)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture =
                cultureInfo;
        }
    }
}
