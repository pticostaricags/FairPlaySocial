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
#elif IOS
        //Check https://learn.microsoft.com/en-us/azure/active-directory/develop/msal-net-xamarin-ios-considerations#enable-keychain-access
        b2CConstants.PublicClientApp = PublicClientApplicationBuilder.Create(
                        b2CConstants.ClientId)
                        .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                        .WithB2CAuthority(b2CConstants.Authority)
                        .WithRedirectUri(b2CConstants.RedirectUri)
                        .Build();
#elif MACCATALYST
        b2CConstants.PublicClientApp = PublicClientApplicationBuilder.Create(
                b2CConstants.ClientId)
                .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                .WithB2CAuthority(b2CConstants.Authority)
                .WithRedirectUri(b2CConstants.RedirectUri)
                .Build();
#endif

        MainPage = new MainPage();
    }

    protected override void OnStart()
    {
        AppCenter.Start("" +
            "ios=b1c8996d-c1e5-4ad7-ab93-b5dfb82e21c5;" +
            "android-=9fd5a524-b775-4dba-8f37-9328f0c2f130;",
            typeof(Analytics), typeof(Crashes));
        AppCenter.LogLevel = Microsoft.AppCenter.LogLevel.Verbose;
    }

#if ANDROID
    public static MainActivity? ParentWindow { get; internal set; }

#endif
}
