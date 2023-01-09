using FairPlaySocial.ClientsConfiguration;
using FairPlaySocial.Common.Global;
using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.DataAccess.Data;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Notifications.Hubs;
using FairPlaySocial.Server;
using FairPlaySocial.Server.CustomUserProvider;
using FairPlaySocial.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using System.Security.Claims;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Application Entryy class
/// </summary>
public class Program
{
    /// <summary>
    /// Application entry method
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        ModelsLocalizationSetup.ConfigureModelsLocalizers(host.Services);
        host.Run();
    }

    /// <summary>
    /// Initializes the Host Builder
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var azAppConfigSettings = config.Build();
                    var azAppConfigConnection = azAppConfigSettings["AppConfig"];
                    if (!string.IsNullOrEmpty(azAppConfigConnection))
                    {
                        // Use the connection string if it is available.
                        config.AddAzureAppConfiguration(options =>
                        {
                            options.Connect(azAppConfigConnection)
                            .ConfigureRefresh(refresh =>
                            {
                                // All configuration values will be refreshed if the sentinel key changes.
                                refresh.Register("TestApp:Settings:Sentinel", refreshAll: true);
                            });
                        });
                    }
                    else if (Uri.TryCreate(azAppConfigSettings["Endpoints:AppConfig"], UriKind.Absolute, out var endpoint))
                    {
                        // Use Azure Active Directory authentication.
                        // The identity of this app should be assigned 'App Configuration Data Reader' or 'App Configuration Data Owner' role in App Configuration.
                        // For more information, please visit https://aka.ms/vs/azure-app-configuration/concept-enable-rbac
                        config.AddAzureAppConfiguration(options =>
                        {
                            options.Connect(endpoint, new DefaultAzureCredential())
                            .ConfigureRefresh(refresh =>
                            {
                                // All configuration values will be refreshed if the sentinel key changes.
                                refresh.Register("TestApp:Settings:Sentinel", refreshAll: true);
                            });
                        });
                    }
                })
        .ConfigureLogging(configureLogging: (builderContext) =>
        {
            builderContext.AddSimpleConsole(simpleConsoleOptions =>
            {
                simpleConsoleOptions.TimestampFormat = "[HH:mm:ss] ";
            });
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
}