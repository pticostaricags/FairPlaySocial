using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Common.Interfaces.Services
{
    public interface ICultureSelectionService
    {
        void SetCurrentCulture(CultureInfo cultureInfo);
        CultureInfo GetCurrentCulture();
    }
}
