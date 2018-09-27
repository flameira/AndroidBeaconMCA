namespace AndroidBeacon.Droid
{
    using System;
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Util;
    using Android.Widget;
    using Java.Lang;

    [Service]
    public class BackgroundService : Service
    {
        public static Runnable runnable = null;
        public Context context;
        public Handler handler = null;
        static readonly string TAG = typeof(BackgroundService).FullName;
        public IBinder Binder { get; private set; }

        public BackgroundService()
        {
            context = this;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Toast.MakeText(this, "Service created!",ToastLength.Long).Show();
            handler = new Handler();
            runnable = new Runnable(RunAction);
            handler.PostDelayed(runnable, 15000);
           
        }

        private void RunAction()
        {
            Toast.MakeText(context, "Service is still running", ToastLength.Long).Show();
            handler.PostDelayed(runnable, 10000);

        }

        public override void OnDestroy()
        {
           // base.OnDestroy();
            /* IF YOU WANT THIS SERVICE KILLED WITH THE APP THEN UNCOMMENT THE FOLLOWING LINE */
            //handler.removeCallbacks(runnable);

            Toast.MakeText(this, "Service stopped", ToastLength.Long).Show();
        }

        public override void OnStart(Intent intent, int startId)
        {
            base.OnStart(intent, startId);
            Toast.MakeText(context, "Service started by user.", ToastLength.Long).Show();
        }

        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(TAG, "OnBind");
            this.Binder = new BackgroundServiceBinder(this);
            return this.Binder;
        }
    }


    public class BackgroundServiceBinder : Binder
    {
        public BackgroundServiceBinder(BackgroundService service)
        {
            this.Service = service;
        }

        public BackgroundService Service { get; private set; }
    }
}