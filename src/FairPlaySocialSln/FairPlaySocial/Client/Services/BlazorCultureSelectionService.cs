using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.JSInterop;
using System.Globalization;

namespace FairPlaySocial.Client.Services
{
    public class BlazorCultureSelectionService : ICultureSelectionService
    {
        private IJSRuntime? JSRuntime { get; set; }

        public BlazorCultureSelectionService(IJSRuntime jsRuntime) 
        {
            this.JSRuntime = jsRuntime;
        }
        
        public CultureInfo GetCurrentCulture()
        {
            var js = (IJSInProcessRuntime)JSRuntime!;
            var cultureName = js.Invoke<string>("blazorCulture.set");
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
            return cultureInfo;
        }

        public void SetCurrentCulture(CultureInfo cultureInfo)
        {
            var js = (IJSInProcessRuntime)JSRuntime!;
            js.InvokeVoid("blazorCulture.set", cultureInfo.Name);
        }
    }
}
