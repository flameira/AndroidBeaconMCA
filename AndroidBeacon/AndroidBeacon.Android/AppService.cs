namespace AndroidBeacon.Droid
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Support.V4.App;
    using Android.Util;
    using Android.Widget;
    using Java.Lang;

    /// <summary>
    ///     https://stackoverflow.com/questions/24077901/how-to-create-an-always-running-background-service
    /// </summary>
    /// <seealso cref="Android.App.Service" />
    [Service]
    public class AppService : Service
    {
        private static string _fcsUuid = "FC51BA92-74D5-4972-B2D9-9ECBC62ACFC9";
        private static Runnable _runnable;
        private static readonly string Tag = "FLBEACON";


       
        private Handler _handler;

        private BeaconManagerService _beaconManagerService;
        private BeaconManagerService BeaconManagerService 
        {
            get
            {
                if (_beaconManagerService == null)
                {
                    _beaconManagerService = new BeaconManagerService(ApplicationContext);
                }

                return _beaconManagerService;
            }
        } 

        private LocationManagerService _locationManagerService;
        private LocationManagerService LocationManagerService 
        {
            get
            {
                if (_locationManagerService == null)
                {
                    _locationManagerService = new LocationManagerService();
                }

                return _locationManagerService;
            }
        } 

        private static readonly string LOG_FILENAME = "AppService";
        private LogService logService = new LogService();

        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(Tag, "OnBind");
            logService.WriteToLog(LOG_FILENAME, "OnBind");
            return null;
        }
        private static readonly string CHANNEL_ID = "location_notification";
        internal static readonly string COUNT_KEY = "count";
        private static readonly int NOTIFICATION_ID = 1000;


        public override void OnCreate()
        {
            base.OnCreate();
            Log.Info(Tag, "Service created!");
            Toast.MakeText(this, "Service created!", ToastLength.Long).Show();

            _handler = new Handler();
            _runnable = new Runnable(RunAction);
            Log.Info(Tag, "Start Runing Actions");
            _handler.PostDelayed(_runnable, 15000);

            logService.WriteToLog(LOG_FILENAME, "OnCreate");

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O) {
             
                NotificationChannel channel = new NotificationChannel(CHANNEL_ID,
                    "ANDROID BEACON",NotificationImportance.Default);

                ((NotificationManager) Application.Context.GetSystemService(Context.NotificationService))
                    .CreateNotificationChannel(channel);

                Notification notification = new NotificationCompat.Builder(this, CHANNEL_ID)
                    .SetContentTitle("Android Beacon is running in the background").SetContentText("Android Beacon is using background service").Build();
                
                StartForeground(101, notification);
            }
        }


        public override void OnDestroy()
        {
            // base.OnDestroy();
            /* IF YOU WANT THIS SERVICE KILLED WITH THE APP THEN UNCOMMENT THE FOLLOWING LINE */
            //handler.removeCallbacks(runnable);

            Toast.MakeText(this, "Service stopped", ToastLength.Long).Show();

            logService.WriteToLog(LOG_FILENAME, "OnDestroy");
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            logService.WriteToLog(LOG_FILENAME, "OnStartCommand");

            //TRICK to keep app runing
            return StartCommandResult.Sticky;
        }

        public override void OnTaskRemoved(Intent rootIntent)
        {
            var restartService = new Intent(this, typeof(MainActivity));
            restartService.SetPackage(PackageName);

            var restartServicePi = PendingIntent.GetService(this, 1, restartService, PendingIntentFlags.OneShot);

            var alarmService = (AlarmManager) GetSystemService(AlarmService);

            alarmService.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + 100, restartServicePi);

            logService.WriteToLog(LOG_FILENAME, "OnTaskRemoved");
        }

        private async void RunAction()
        {
            var uuid = _fcsUuid;
            double major = 0;
            double minor = 0;
            var identifier = "FCS";
            BeaconManagerService
                .AddLocationPovider(LocationManagerService)
                .StartMonitoring(uuid, major, minor, identifier);
        }
    }
}