

using Android.App;
using Android.Content.PM;
using Android.OS;

namespace QSearcher_.Droid
{
    [Activity(Label = "QSearcher", Icon = "@drawable/icon2", NoHistory = true, Theme = "@style/SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class PrevActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            StartActivity(typeof(MainActivity));
        }
    }
}