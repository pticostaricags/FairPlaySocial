namespace FairPlaySocial.ClientServices
{
    public class HttpClientService
    {
        private IHttpClientFactory HttpClientFactory { get; }
        public HttpClientService(IHttpClientFactory httpClientFactory)
        {
            this.HttpClientFactory = httpClientFactory;
        }

        public HttpClient CreateAnonymousClient()
        {
            return this.HttpClientFactory.CreateClient(
                $"{Common.Global.Constants.Assemblies.MainAppAssemblyName}.ServerAPI.Anonymous");
        }

        public HttpClient CreateAuthorizedClient()
        {
            return this.HttpClientFactory.CreateClient(
                $"{Common.Global.Constants.Assemblies.MainAppAssemblyName}.ServerAPI");
        }
    }
}