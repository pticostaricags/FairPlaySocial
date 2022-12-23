using CommunityToolkit.Maui;
using FairPlaySocial.ClientsConfiguration;
using FairPlaySocial.Common;
using FairPlaySocial.Common.Interfaces.Services;
using FairPlaySocial.MAUIBlazor.Authentication;
using FairPlaySocial.MAUIBlazor.Features.LogOn;
using FairPlaySocial.MAUIBlazor.MultiPlatformServices;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace FairPlaySocial.MAUIBlazor;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        builder.Services.AddOptions();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
        string strAppConfigStreamName = string.Empty;
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        strAppConfigStreamName = "FairPlaySocial.MAUIBlazor.appsettings.Development.json";
#else
		strAppConfigStreamName = "FairPlaySocial.MAUIBlazor.appsettings.Development.json";
#endif
        var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MauiProgram)).Assembly;
        var stream = assembly.GetManifestResourceStream(strAppConfigStreamName);
        builder.Configuration.AddJsonStream(stream);

        string fairPlayTubeapiAddress = builder.Configuration["ApiBaseUrl"];
        B2CConstants b2CConstants = builder.Configuration.GetSection("B2CConstants").Get<B2CConstants>();
        builder.Services.AddSingleton(b2CConstants);

        /* When running in an emulator localhost woult not work as expected.
             * You need to do forwarding, you can use ngrok, check an example before
             * Use your correct FairPlayTube API port
             * */
        //ngrok.exe http https://localhost:7115 -host-header="localhost:7115"
        //string fairplaysocialApiAddress = "REPLACE_WITH_NGROK_GENERATED_URL";
        builder.Services.AddScoped<BaseAddressAuthorizationMessageHandler>();
        builder.Services.AddHttpClient(
            $"{FairPlaySocial.Common.Global.Constants.Assemblies.MainAppAssemblyName}.ServerAPI", client =>
    client.BaseAddress = new Uri(fairPlayTubeapiAddress))
    //.AddHttpMessageHandler<LocalizationMessageHandler>()
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
    .AddPolicyHandler(PollyHelper.GetRetryPolicy());

        builder.Services.AddHttpClient(
            $"{FairPlaySocial.Common.Global.Constants.Assemblies.MainAppAssemblyName}.ServerAPI.Anonymous", client =>
            client.BaseAddress = new Uri(fairPlayTubeapiAddress))
            //.AddHttpMessageHandler<LocalizationMessageHandler>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
            .AddPolicyHandler(PollyHelper.GetRetryPolicy());
        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
            .CreateClient(
            $"{FairPlaySocial.Common.Global.Constants.Assemblies.MainAppAssemblyName}.ServerAPI"));

        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
            .CreateClient($"{FairPlaySocial.Common.Global.Constants.Assemblies.MainAppAssemblyName}.ServerAPI.Anonymous"));


        builder.Services.AddTransient<IToastService, ToastService>();
        builder.Services.AddSingleton<ITextToSpeechService, TextToSpeechService>();
        builder.Services.AddMultiPlatformServices();
        builder.Services.AddTransient<IGeoLocationService, MauiGeoLocationService>();
        builder.Services.AddScoped<IErrorBoundaryLogger, CustomBoundaryLogger>();

        return builder.Build();
    }

    public class CustomBoundaryLogger : IErrorBoundaryLogger
    {
        public ValueTask LogErrorAsync(Exception exception)
        {
            return ValueTask.CompletedTask;
        }
    }

    public class BaseAddressAuthorizationMessageHandler : DelegatingHandler
    {
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            BaseAddressAuthorizationMessageHandler.AddAuthToken(request);
            return base.Send(request, cancellationToken);
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddAuthToken(request);
            return await base.SendAsync(request, cancellationToken);
        }

        private static void AddAuthToken(HttpRequestMessage request)
        {
            request.Headers.Authorization = new System.Net.Http.Headers
                .AuthenticationHeaderValue("bearer", UserState.UserContext.AccessToken);
        }
    }
}
