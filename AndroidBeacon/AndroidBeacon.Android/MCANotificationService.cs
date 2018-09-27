using AndroidBeacon.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(MCANotificationService))]

namespace AndroidBeacon.Droid
{
    using System;
    using Android.App;
    using Android.Content;
    using Android.OS;

    public class MCANotificationService
    {
        private static readonly string CHANNEL_ID = "location_notification";
        internal static readonly string COUNT_KEY = "count";
        private static readonly int NOTIFICATION_ID = 1000;
        private NotificationManager notificationManager;


        public MCANotificationService()
        {
            CreateNotificationChannel();
        }

        public NotificationManager GetNotificationManager()
        {
            if (notificationManager == null)
                notificationManager = (NotificationManager) Forms.Context.GetSystemService(Context.NotificationService);

            return notificationManager;
        }


        public void SendNotification(string title, string text)
        {
            //Send Notification

            var builder = new Notification.Builder(Forms.Context).SetContentTitle(title).SetContentText(text)
                .SetSmallIcon(Resource.Drawable.Icon_small).SetChannelId(CHANNEL_ID);

            GetNotificationManager();

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

            GetNotificationManager();
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}