using Blazored.Toast;
using FairPlaySocial.Client;
using FairPlaySocial.Client.CustomClaims;
using FairPlaySocial.Client.Services;
using FairPlaySocial.ClientsConfiguration;
using FairPlaySocial.Common.Interfaces.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var faifairplaysocialApiAddress = builder.HostEnvironment.BaseAddress;
builder.Services.AddHttpClient(
            $"{FairPlaySocial.Common.Global.Constants.Assemblies.MainAppAssemblyName}.ServerAPI",
            client =>
            {
                client.BaseAddress = new Uri(faifairplaysocialApiAddress);
                client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
                client.DefaultRequestVersion = HttpVersion.Version30;
            })
    //.AddHttpMessageHandler<LocalizationMessageHandler>()
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
    //.AddHttpMessageHandler<LocalizationMessageHandler>()
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
builder.Services.AddMultiPlatformServices();

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
await builder.Build().RunAsync();
