using Blazored.Toast;
using FairPlaySocial.Client;
using FairPlaySocial.Client.CustomClaims;
using FairPlaySocial.Client.Extensions;
using FairPlaySocial.Client.Services;
using FairPlaySocial.ClientsConfiguration;
using FairPlaySocial.ClientServices.CustomLocalization.Api;
using FairPlaySocial.Common.Handlers;
using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Localization;
using System.Net;
using System.Net.Http.Headers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSingleton<IStringLocalizerFactory, ApiLocalizerFactory>();
builder.Services.AddSingleton<IStringLocalizer, ApiLocalizer>();
builder.Services.AddLocalization();

builder.Services.AddScoped<LocalizationMessageHandler>();

var faifairplaysocialApiAddress = builder.HostEnvironment.BaseAddress;
builder.Services.AddHttpClient(
            $"{FairPlaySocial.Common.Global.Constants.Assemblies.MainAppAssemblyName}.ServerAPI",
            client =>
            {
                client.BaseAddress = new Uri(faifairplaysocialApiAddress);
                client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
                client.DefaultRequestVersion = HttpVersion.Version30;
            })
    .AddHttpMessageHandler<LocalizationMessageHandler>()
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
    .AddPolicyHandler(PollyHelper.GetRetryPolicy());

builder.Services.AddHttpClient(
    $"{FairPlaySocial.Common.Global.Constants.Assemblies.MainAppAssemblyName}.ServerAPI.Anonymous",
    client =>
    {
        client.BaseAddress = new Uri(faifairplaysocialApiAddress);
        client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
        client.DefaultRequestVersion = HttpVersion.Version30;
    })
    .AddHttpMessageHandler<LocalizationMessageHandler>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
    .AddPolicyHandler(PollyHelper.GetRetryPolicy());
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient(
    $"{FairPlaySocial.Common.Global.Constants.Assemblies.MainAppAssemblyName}.ServerAPI"));

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient($"{FairPlaySocial.Common.Global.Constants.Assemblies.MainAppAssemblyName}.ServerAPI.Anonymous"));

builder.Services.AddBlazoredToast();
builder.Services.AddTransient<IToastService, ToastService>();
builder.Services.AddTransient<ITextToSpeechService, TextToSpeechService>();
builder.Services.AddTransient<IGeoLocationService, BlazorGeoLocationService>();
builder.Services.AddTransient<ICultureSelectionService, BlazorCultureSelectionService>();
builder.Services.AddTransient<IAnalyticsService, BlazorAnalyticsService>();
builder.Services.AddMultiPlatformServices();

AppSettings appSettings = builder.Configuration.Get<AppSettings>()!;
builder.Services.AddSingleton(appSettings);

builder.Services.AddMsalAuthentication<RemoteAuthenticationState, CustomRemoteUserAccount>(options =>
{
    builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
    var defaultScope = builder.Configuration["AzureAdB2CScopes:DefaultScope"]!;
    options.ProviderOptions.DefaultAccessTokenScopes.Add(defaultScope);
    options.ProviderOptions.LoginMode = "redirect";
    options.UserOptions.NameClaim = "name";
    options.UserOptions.RoleClaim = "Role";
}).AddAccountClaimsPrincipalFactory<
                RemoteAuthenticationState, CustomRemoteUserAccount, CustomAccountClaimsPrincipalFactory>();
var host = builder.Build();
ModelsLocalizationSetup.ConfigureModelsLocalizers(host.Services);
await host.SetDefaultCulture();
await host.RunAsync();