using Android.App;
using Android.Content.PM;
using Microsoft.Identity.Client;

namespace FairPlaySocial.MAUIBlazor.Platforms.Android
{

    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
#if DEBUG
    [IntentFilter(new[] { global::Android.Content.Intent.ActionView },
        Categories = new[] { global::Android.Content.Intent.CategoryBrowsable,
            global::Android.Content.Intent.CategoryDefault },
        DataHost = "auth",
        DataScheme = "msal86fd3439-fca2-4b8f-89c0-bdb4b13c520e")]
#else
    [IntentFilter(new[] { global::Android.Content.Intent.ActionView },
            Categories = new[] { global::Android.Content.Intent.CategoryBrowsable,
                global::Android.Content.Intent.CategoryDefault },
            DataHost = "auth",
            DataScheme = "msal?")]
#endif
    public class MsalActivity : BrowserTabActivity
    {
    }
}
