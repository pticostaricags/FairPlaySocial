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

namespace FairPlaySocial.Server;

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