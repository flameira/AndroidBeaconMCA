namespace AndroidBeacon.Droid
{
    using Android;
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.OS;
    using Android.Runtime;
    using Plugin.CurrentActivity;
    using Plugin.Permissions;
    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    [Activity(Label = "AndroidBeacon", Name = "AndroidBeacon.Droid.MainActivity", Icon = "@mipmap/icon",
        Theme = "@style/MainTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        private static readonly string LogFilename = "MainActivity";
        private AutoStart _receiver;
        private LogService _logService;
        private MCANotificationService _mcaNotificationService;

        public MCANotificationService GetMcaNotificationService()
        {
            if (_mcaNotificationService == null)
                _mcaNotificationService = DependencyService.Get<MCANotificationService>();

            return _mcaNotificationService;
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

            _logService = new LogService();
            _receiver = new AutoStart();

            base.OnCreate(savedInstanceState);

            CrossCurrentActivity.Current.Activity = this;

            Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            RequestPermissions(new[] {Manifest.Permission.AccessFineLocation, Manifest.Permission.BluetoothPrivileged},
                0);


            GetMcaNotificationService();
            _mcaNotificationService.SendNotification("AndroidBeacon", "APP STARTED");

            _logService.WriteToLog(LogFilename, "APP STARTED");

            StartService(new Intent(this, typeof(AppService)));
        }

        protected override void OnPause()
        {
            base.OnPause();
            UnregisterReceiver(_receiver);
        }

        protected override void OnResume()
        {
            base.OnResume();
            _receiver = new AutoStart();
            RegisterReceiver(_receiver, new IntentFilter("android.intent.action.BOOT_COMPLETED"));

        }
    }
}