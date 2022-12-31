using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.CustomAttributes.Localization;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Globalization;

namespace FairPlaySocial.Client.Shared
{
    public partial class CultureSelector
    {
        [Inject]
        public NavigationManager? NavManager { get; set; }

        [Inject]
        public IJSRuntime? JSRuntime { get; set; }
        [Inject]
        private IStringLocalizer<CultureSelector>? Localizer { get; set; }
        [Inject]
        private LocalizationClientService? LocalizationClientService { get; set; }
        [Inject]
        private IToastService? ToastService { get; set; }
        private CultureModel[]? CultureModels { get; set; }
        private CultureInfo[]? SupportedCultures { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.CultureModels = await this.LocalizationClientService!
                    .GetSupportedCulturesAsync();
                this.SupportedCultures = CultureModels!
                    .Select(p => CultureInfo.GetCultureInfo(p.Name!)).ToArray();
            }
            catch (Exception ex)
            {
                await this.ToastService!
                    .ShowErrorMessageAsync(ex.Message,CancellationToken.None);
            }
        }

        CultureInfo Culture
        {
            get => CultureInfo.CurrentCulture;
            set
            {
                if (CultureInfo.CurrentCulture != value)
                {
                    var js = (IJSInProcessRuntime)JSRuntime!;
                    js.InvokeVoid("blazorCulture.set", value.Name);

                    NavManager!.NavigateTo(NavManager.Uri, forceLoad: true);
                }
            }
        }

        public string GetDisplayName(CultureInfo culture)
        {
            return CultureModels!.Single(p => p.Name == culture.Name).DisplayName!;
        }

        #region Resource Keys
        [ResourceKey(defaultValue: "Language")]
        public const string LanguageTextKey = "LanguageText";
        [ResourceKey(defaultValue: "Disclaimer: Translations are automatically generated " +
            "using Microsoft Azure Translator. As with any automated " +
            "translation technology the text may not be perfect.")]
        public const string TranslationsDisclaimerTextKey = "TranslationsDisclaimerText";
        #endregion Resource Keys
    }
}
