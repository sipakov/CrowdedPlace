using System;
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace OnlineDemonstrator.MobileClient.Android
{
    [Activity(Label = "OnlineDemonstrator", Theme = "@style/MainTheme", MainLauncher = true, Icon = "@mipmap/ic_launcher",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            //var rawDeviceId = global::Android.Provider.Settings.Secure.GetString(global::Android.App.Application.Context.ContentResolver, global::Android.Provider.Settings.Secure.AndroidId);
         
            LoadApplication(new App(Guid.Empty));
        }
    }
}