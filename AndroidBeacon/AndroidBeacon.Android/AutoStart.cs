namespace AndroidBeacon.Droid
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Util;
    using Android.Widget;
    using Java.Lang;

    [BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true)]
    [IntentFilter(new[] {Intent.ActionBootCompleted, Intent.ActionLockedBootCompleted})]
    public class AutoStart : BroadcastReceiver
    {
        private readonly string LogFilename = "AutoStartBroadCast";
        private readonly string Tag = "FLBEACON";

        public override void OnReceive(Context context, Intent intent)
        {
            var logService = new LogService();
            Log.Info(Tag, "Start from BroadCast Intent:" + intent.Action);

            try
            {
                if (intent.Action.Equals(Intent.ActionBootCompleted))
                {
                    logService.WriteToLog(LogFilename, "Intent:" + intent.Action);

                    Toast.MakeText(context, "Received BroadCast intent!", ToastLength.Long).Show();

                    if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                        context.StartForegroundService(new Intent(context, typeof(AppService)));
                    else
                        context.StartService(new Intent(context, typeof(AppService)));


                    Log.Info(Tag, "Started from BroadCast");
                }
            }
            catch (Exception ex)
            {
                logService.WriteToLog(LogFilename, "ERROR:" + intent.Action);
                Log.Error(Tag, "ERROR: " + ex.Message);
            }
        }
    }
}