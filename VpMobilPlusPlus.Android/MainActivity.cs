using System;
using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
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

        RaiseDeviceResolution();
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        RaiseDeviceResolution();
        
        Window.SetBackgroundDrawable(new ColorDrawable(Color.Black));
    }

    private void RaiseDeviceResolution()
    {
        var metrics = Resources!.DisplayMetrics;
        
        float density = metrics.Density;

        int width = (int)(metrics.WidthPixels / density);
        int height = (int)(metrics.HeightPixels / density);

        DeviceResolution.Raise(width, height);
    }
}
