using System;
using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Avalonia;
using Avalonia.Android;
using VpMobilPlusPlus.Util;

namespace VpMobilPlusPlus.Android;

[Activity(
    Label = "VpMobilPlusPlus.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
    public override void OnConfigurationChanged(Configuration newConfig)
    {
        base.OnConfigurationChanged(newConfig);

        var metrics = Resources!.DisplayMetrics;

        var width = metrics.WidthPixels;
        var height = metrics.HeightPixels;

        DeviceResolution.Raise(width, height);
    }
}
