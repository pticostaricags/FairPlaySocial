using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.AutomatedPostDeploymentTests
{
    public class PostDeploymentTestsBase
    {
        protected const string BaseUrl = "https://fairplaysocialdev.azurewebsites.net/";
        public TestAzureAdB2CAuthConfiguration? TestAzureAdB2CAuthConfiguration { get; }
        public PostDeploymentTestsBase()
        {
            ConfigurationBuilder configurationBuilder = new();
#if DEBUG
            configurationBuilder.AddUserSecrets<PostDeploymentTestsBase>();
#else
            var appSettingsBase64Content = Environment.GetEnvironmentVariable("AppSettingsContent")!;
            var appSettingsBytes = Convert.FromBase64String(appSettingsBase64Content);
            MemoryStream memoryStream = new MemoryStream(appSettingsBytes);
            configurationBuilder.AddJsonStream(memoryStream);
#endif
            IConfiguration configuration = configurationBuilder.Build();
            this.TestAzureAdB2CAuthConfiguration = configuration
                .GetSection(nameof(TestAzureAdB2CAuthConfiguration))
                .Get<TestAzureAdB2CAuthConfiguration>();
        }
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
