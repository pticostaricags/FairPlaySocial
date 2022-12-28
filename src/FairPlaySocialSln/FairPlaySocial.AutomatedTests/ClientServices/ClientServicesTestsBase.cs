using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Models.CustomExceptions;
using FairPlaySocial.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Net.Http.Json;
using System.Text;
using static FairPlaySocial.Common.Global.Constants;

namespace FairPlaySocial.AutomatedTests.ClientServices
{
    public abstract class ClientServicesTestsBase
    {
        internal static TestServer? Server { get; private set; }
        private HttpClient? UserRoleAuthorizedHttpClient { get; set; }
        private HttpClient? AdminRoleAuthorizedHttpClient { get; set; }
        internal static string? UserBearerToken { get; set; }
        internal static TestAzureAdB2CAuthConfiguration? TestAzureAdB2CAuthConfiguration { get; set; }
        public TestsHttpClientFactory TestsHttpClientFactory { get; }
        public ClientServicesTestsBase()
        {
            ConfigurationBuilder configurationBuilder = new();
#if DEBUG
            configurationBuilder.AddUserSecrets<ClientServicesTestsBase>();
#else
            var appSettingsBase64Content = Environment.GetEnvironmentVariable("AppSettingsContent")!;
            var appSettingsBytes = Convert.FromBase64String(appSettingsBase64Content);
            MemoryStream memoryStream = new MemoryStream(appSettingsBytes);
            configurationBuilder.AddJsonStream(memoryStream);
#endif
            IConfiguration configuration = configurationBuilder.Build();
            TestAzureAdB2CAuthConfiguration = configuration.GetSection("TestAzureAdB2CAuthConfiguration").Get<TestAzureAdB2CAuthConfiguration>();
            var builder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .UseStartup<Startup>();
            ClientServicesTestsBase.Server = new TestServer(builder);
            this.TestsHttpClientFactory = new TestsHttpClientFactory();
        }

        public enum Role
        {
            Admin,
            User
        }

        protected HttpClient CreateAnonymousClient()
        {
            return Server!.CreateClient();
        }

        protected async Task<HttpClient> SignIn(Role role)
        {
            var authorizedHttpClient = await CreateAuthorizedClientAsync(role);
            _ = await authorizedHttpClient.GetStringAsync(ApiRoutes.AuthController.GetMyRoles);
            return authorizedHttpClient;
        }

        private async Task<HttpClient> CreateAuthorizedClientAsync(Role role)
        {

            switch (role)
            {
                case Role.Admin:
                    if (this.AdminRoleAuthorizedHttpClient != null)
                        return this.AdminRoleAuthorizedHttpClient;
                    break;
                case Role.User:
                    if (this.UserRoleAuthorizedHttpClient != null)
                        return this.UserRoleAuthorizedHttpClient;
                    break;
            }
            HttpClient httpClient = new();
            List<KeyValuePair<string?, string?>> formData = new();
            formData.Add(new KeyValuePair<string?, string?>("username",
                role == Role.User ? TestAzureAdB2CAuthConfiguration!.UserRoleUsername : TestAzureAdB2CAuthConfiguration!.AdminRoleUsername));
            formData.Add(new KeyValuePair<string?, string?>("password",
                role == Role.User ? TestAzureAdB2CAuthConfiguration.UserRolePassword : TestAzureAdB2CAuthConfiguration.AdminRolePassword));
            formData.Add(new KeyValuePair<string?, string?>("grant_type", "password"));
            string? applicationId = TestAzureAdB2CAuthConfiguration.ApplicationId;
            formData.Add(new KeyValuePair<string?, string?>("scope", $"openid {applicationId} offline_access"));
            formData.Add(new KeyValuePair<string?, string?>("client_id", applicationId));
            formData.Add(new KeyValuePair<string?, string?>("response_type", "token id_token"));
            System.Net.Http.FormUrlEncodedContent form = new(formData);
            var response = await httpClient.PostAsync(TestAzureAdB2CAuthConfiguration.TokenUrl, form);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                var client = Server!.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result!.Access_token);
                switch (role)
                {
                    case Role.Admin:
                        this.AdminRoleAuthorizedHttpClient = client;
                        break;
                    case Role.User:
                        this.UserRoleAuthorizedHttpClient = client;
                        break;
                }
                UserBearerToken = result!.Access_token;
                return client;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new CustomValidationException(error);
            }
        }

        private HttpClientService CreateHttpClientService()
        {
            HttpClientService httpClientService = new(this.TestsHttpClientFactory);
            return httpClientService;
        }

        protected ApplicationUserClientService CreateApplicationUserClientService()
        {
            ApplicationUserClientService applicationUserClientService = new(CreateHttpClientService());
            return applicationUserClientService;
        }

        protected MyPostClientService CreateMyPostClientService()
        {
            MyPostClientService myPostClientService = new(CreateHttpClientService());
            return myPostClientService;
        }

        protected MyFeedClientService CreateMyFeedClientService()
        {
            MyFeedClientService myFeedClientService= new(CreateHttpClientService());
            return myFeedClientService;
        }
    }

    public class TestsHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            string assemblyName = Common.Global.Constants.Assemblies.MainAppAssemblyName;
            var serverApiClientName = $"{assemblyName}.ServerAPI";
            var client = ClientServicesTestsBase.Server!.CreateClient();
            if (name == serverApiClientName)
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                    ClientServicesTestsBase.UserBearerToken);
                return client;
            }
            else
                return client;
        }
    }

    public class AuthResponse
    {
        public string? Access_token { get; set; }
        public string? Token_type { get; set; }
        public string? Expires_in { get; set; }
        public string? Refresh_token { get; set; }
        public string? Id_token { get; set; }
    }

    public class TestAzureAdB2CAuthConfiguration
    {
        public string? TokenUrl { get; set; }
        public string? UserRoleUsername { get; set; }
        public string? UserRolePassword { get; set; }
        public string? AdminRoleUsername { get; set; }
        public string? AdminRolePassword { get; set; }
        public string? ApplicationId { get; set; }
        public string? AzureAdUserObjectId { get; set; }
    }
}
