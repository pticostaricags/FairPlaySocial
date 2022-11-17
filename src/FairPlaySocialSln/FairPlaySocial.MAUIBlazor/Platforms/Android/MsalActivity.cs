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
        DataScheme = "msal74369fca-6e6f-4e55-9426-91fa6ded6f4a")]
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
