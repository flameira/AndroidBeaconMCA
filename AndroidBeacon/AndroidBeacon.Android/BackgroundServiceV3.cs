namespace AndroidBeacon.Droid
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Util;
    using Android.Widget;
    using Java.Lang;

    /// <summary>
    ///     https://stackoverflow.com/questions/24077901/how-to-create-an-always-running-background-service
    /// </summary>
    /// <seealso cref="Android.App.Service" />
    [Service]
    //[BroadcastReceiver(Enabled = true, Exported = false)]
    //[IntentFilter(new[] {Intent.ActionBootCompleted})]
    public class BackgroundService : Service
    {
        public static string DefaultIdentifier = "FCS_Beacon_Region";
        public static string FCSUuid = "FC51BA92-74D5-4972-B2D9-9ECBC62ACFC9";

        public static Runnable runnable;
        private static readonly string TAG = typeof(BackgroundService).FullName;
        public Context context;
        public Handler handler;

        public BackgroundService()
        {
            context = this;
        }


        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(TAG, "OnBind");
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Toast.MakeText(this, "Service created!", ToastLength.Long).Show();
            handler = new Handler();
            runnable = new Runnable(RunAction);
            handler.PostDelayed(runnable, 15000);
        }


        public override void OnDestroy()
        {
            // base.OnDestroy();
            /* IF YOU WANT THIS SERVICE KILLED WITH THE APP THEN UNCOMMENT THE FOLLOWING LINE */
            //handler.removeCallbacks(runnable);

            Toast.MakeText(this, "Service stopped", ToastLength.Long).Show();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            //TRICK to keep app runing
            return StartCommandResult.Sticky;
        }

        public override void OnTaskRemoved(Intent rootIntent)
        {
            var restartService = new Intent(this, typeof(MainActivity));
            restartService.SetPackage(PackageName);

            var restartServicePI = PendingIntent.GetService(this, 1, restartService, PendingIntentFlags.OneShot);

            var alarmService = (AlarmManager) GetSystemService(AlarmService);

            alarmService.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + 100, restartServicePI);
        }


        private async void RunAction()
        {
            Toast.MakeText(context, $"Service is still running", ToastLength.Long).Show();
            handler.PostDelayed(runnable, 10000);
        }
    }
}