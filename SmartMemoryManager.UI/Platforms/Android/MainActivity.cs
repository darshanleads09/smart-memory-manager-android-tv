using Android.App;
using Android.Content.PM;
using Android.OS;

namespace SmartMemoryManager.UI;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    Exported = true,
    LaunchMode = LaunchMode.SingleTop,
    ScreenOrientation = ScreenOrientation.Landscape,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(
    [global::Android.Content.Intent.ActionMain],
    Categories = [global::Android.Content.Intent.CategoryLeanbackLauncher, global::Android.Content.Intent.CategoryLauncher])]
public class MainActivity : MauiAppCompatActivity
{
}
