using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.Models.UserPreference;
using Microsoft.Extensions.Configuration;

namespace FairPlaySocial.MAUIBlazor.MultiPlatformServices
{
    public class TextToSpeechService : ITextToSpeechService
    {
        private IEnumerable<Locale>? _availableLocales;
        private Locale? _matchingLocale;

        private bool IsEnabled => this.UserPreferenceModel.EnableAudibleCuesInMobile;
        public string? Locale { get; set; } = Constants.Locales.DefaultLocale;
        public UserPreferenceModel UserPreferenceModel { get; }

        public TextToSpeechService(UserPreferenceModel userPreferenceModel)
        {
            this.UserPreferenceModel = userPreferenceModel;
        }
        public async Task SpeakToDefaultSpeakersAsync(string text, CancellationToken cancellationToken)
        {
            if (this.IsEnabled)
            {
                if (this._availableLocales is null)
                {
                    this._availableLocales = await TextToSpeech.Default.GetLocalesAsync();
                }
                if (this._matchingLocale is null)
                {
                    this._matchingLocale = this._availableLocales
                        .Where(p=>p.Language == this.Locale)
                        .FirstOrDefault();
                }
                SpeechOptions speechOptions = new SpeechOptions()
                {
                    Locale = this._matchingLocale
                };
                await TextToSpeech.SpeakAsync(text, speechOptions, cancellationToken);
            }
        }

        public Task CancelRunningAudioAsync()
        {
            return Task.CompletedTask;
        }
    }
}
