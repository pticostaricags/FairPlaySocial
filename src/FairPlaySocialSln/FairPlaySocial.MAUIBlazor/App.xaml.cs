using FairPlaySocial.MAUIBlazor.Features.LogOn;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter;
using Microsoft.Identity.Client;

namespace FairPlaySocial.MAUIBlazor;

public partial class App : Application
{
    public App(B2CConstants b2CConstants)
    {
        InitializeComponent();
#if ANDROID
        b2CConstants.PublicClientApp = PublicClientApplicationBuilder.Create(
                        b2CConstants.ClientId)
                        .WithB2CAuthority(b2CConstants.Authority)
                        .WithParentActivityOrWindow(() => ParentWindow)
                        .Build();
#else
        b2CConstants.PublicClientApp = PublicClientApplicationBuilder.Create(
                        b2CConstants.ClientId)
                        .WithB2CAuthority(b2CConstants.Authority)
                        .WithRedirectUri(b2CConstants.RedirectUri)
                        .Build();
#endif

        AppCenter.Start("9fd5a524-b775-4dba-8f37-9328f0c2f130", typeof(Analytics), typeof(Crashes));
        AppCenter.LogLevel= Microsoft.AppCenter.LogLevel.Verbose;
        MainPage = new MainPage();
    }

#if ANDROID
    public static MainActivity? ParentWindow { get; internal set; }

#endif
}
