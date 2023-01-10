using PTI.Microservices.Library.AzureTextAnalytics.Models.DetectLanguage;
using PTI.Microservices.Library.Services;

namespace FairPlaySocial.Services
{
    public class TextAnalyticsService
    {
        private readonly AzureTextAnalyticsService azureTextAnalyticsService;

        public TextAnalyticsService(AzureTextAnalyticsService azureTextAnalyticsService)
        {
            this.azureTextAnalyticsService = azureTextAnalyticsService;
        }

        public async Task<Detectedlanguage> DetectLanguageAsync(string text, CancellationToken cancellationToken)
        {
            var result = await 
            this.azureTextAnalyticsService.DetectLanguageAsync(
                model: new PTI.Microservices.Library.AzureTextAnalytics.Models.DetectLanguage.DetectLanguageRequest()
                {
                    documents = new PTI.Microservices.Library.AzureTextAnalytics.Models.DetectLanguage.DetectLanguageRequestDocument[]
                    {
                        new PTI.Microservices.Library.AzureTextAnalytics.Models.DetectLanguage.DetectLanguageRequestDocument()
                        {
                            id="1",
                            text=text,
                        }
                    }
                });
            return result.documents.First().detectedLanguage;
        }

        public async Task<IEnumerable<string>?> GetTopicsAsync(string text, 
            string language,
            CancellationToken cancellationToken)
        {
            var response = await this.azureTextAnalyticsService.GetKeyPhrasesAsync(model: new PTI.Microservices.Library.Models.AzureTextAnalyticsService.GetKeyPhrases.GetKeyPhrasesRequest()
            {
                documents = new PTI.Microservices.Library.Models.AzureTextAnalyticsService.GetKeyPhrases.GetKeyPhrasesRequestDocument[] 
                {
                    new PTI.Microservices.Library.Models.AzureTextAnalyticsService.GetKeyPhrases.GetKeyPhrasesRequestDocument()
                    {
                        id="1",
                        language = language,
                        text = text
                    }
                }
            });
            var result = response.documents?.SelectMany(p => p.keyPhrases);
            return result;
        }
    }
}
