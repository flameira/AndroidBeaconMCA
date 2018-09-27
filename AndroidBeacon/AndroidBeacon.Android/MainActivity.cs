namespace AndroidBeacon.Droid
{
    using Android;
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.OS;
    using Android.Runtime;
    using nexus.protocols.ble;
    using Plugin.CurrentActivity;
    using Plugin.Permissions;
    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    [Activity(Label = "AndroidBeacon", Name = "AndroidBeacon.Droid.MainActivity", Icon = "@mipmap/icon",
        Theme = "@style/MainTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        private MCANotificationService mcaNotificationService;

        public MCANotificationService GetMCANotificationService()
        {
            if (mcaNotificationService == null)
                mcaNotificationService = DependencyService.Get<MCANotificationService>();

            return mcaNotificationService;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected sealed override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            //BluetoothLowEnergyAdapter.OnActivityResult(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;


            base.OnCreate(savedInstanceState);

            CrossCurrentActivity.Current.Activity = this;

            Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
          
            this.RequestPermissions(new []
            {
                Manifest.Permission.AccessFineLocation,
                Manifest.Permission.BluetoothPrivileged,
                
            }, 0);


            GetMCANotificationService();
            mcaNotificationService.SendNotification("AndroidBeacon", "APP STARTED");

            StartService(new Intent(this, typeof(AppService)));
          
        }
    }
}