using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AndroidBeacon.Droid
{
    using Android.Util;

    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    [IntentFilter(new[] { Intent.ActionLocaleChanged })]
    [IntentFilter(new[] { Intent.CategoryCarDock })]
    public class AppBroadcasts : BroadcastReceiver
    {
        private static readonly string TAG = "FLBEACON!!!";
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                Log.Info(TAG, intent.Action);
                Console.WriteLine(intent.Action);

                if (intent.Action == Intent.ActionBootCompleted)
                {
                    Intent serviceStart = new Intent(context, typeof (MainActivity));
                    serviceStart.AddFlags(ActivityFlags.NewTask);
                    context.StartActivity(serviceStart);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log.Error(TAG, e.Message);
                throw;
            }
            
        }
    }
}