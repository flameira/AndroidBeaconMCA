using AndroidBeacon.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(MCANotificationService))]

namespace AndroidBeacon.Droid
{
    using System;
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Support.V4.App;

    public class MCANotificationService
    {
        private static readonly string CHANNEL_ID = "location_notification";
        internal static readonly string COUNT_KEY = "count";
        private static readonly int NOTIFICATION_ID = 1000;
        private NotificationManager notificationManager;


        public MCANotificationService()
        {
            if (notificationManager == null)
                notificationManager = (NotificationManager) Application.Context.GetSystemService(Context.NotificationService);

            CreateNotificationChannel();

        }


        public void SendNotification(string title, string text)
        {
            //Send Notification

          var builder = new NotificationCompat.Builder(Application.Context,CHANNEL_ID).SetContentTitle(title).SetContentText(text)
                .SetSmallIcon(Resource.Drawable.Icon_small);

            try
            {
                notificationManager.Notify(NOTIFICATION_ID, builder.Build());
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
            finally
            {}
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) return;

            var name = "MyApplication";
            var description = "MyApplicationDescription";
            var channel =
                new NotificationChannel(CHANNEL_ID, name, NotificationImportance.Default) {Description = description};
            channel.EnableVibration(true);
            channel.LockscreenVisibility = NotificationVisibility.Public;

            notificationManager.CreateNotificationChannel(channel);
        }
    }
}