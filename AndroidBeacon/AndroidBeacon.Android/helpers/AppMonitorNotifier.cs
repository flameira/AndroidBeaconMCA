namespace AndroidBeacon.Droid.helpers
{
    using System;
    using AltBeaconOrg.BoundBeacon;
    using Xamarin.Forms.Internals;
    using Object = Java.Lang.Object;

    public class MonitorEventArgs : EventArgs
    {
        public Region Region { get; set; }
        public int State { get; set; }
    }

    public class AppMonitorNotifier : Object, IMonitorNotifier
    {
        private static readonly string TAG = typeof(MonotoringService).FullName;

        public void DidDetermineStateForRegion(int p0, Region p1)
        {
            Android.Util.Log.Info(TAG, "I have just switched from seeing/not seeing beacons: "+p0);        

            OnDetermineStateForRegionComplete();
        }

        public void DidEnterRegion(Region p0)
        {
            Android.Util.Log.Info(TAG, "I just saw an beacon for the first time!");

            OnEnterRegionComplete();
        }

        public void DidExitRegion(Region p0)
        {
            Android.Util.Log.Info(TAG, "I no longer see an beacon!");
            OnExitRegionComplete();
        }

        public event EventHandler<MonitorEventArgs> DetermineStateForRegionComplete;
        public event EventHandler<MonitorEventArgs> EnterRegionComplete;
        public event EventHandler<MonitorEventArgs> ExitRegionComplete;

        private void OnDetermineStateForRegionComplete()
        {
            if (DetermineStateForRegionComplete != null) DetermineStateForRegionComplete(this, new MonitorEventArgs());
        }

        private void OnEnterRegionComplete()
        {
            if (EnterRegionComplete != null) EnterRegionComplete(this, new MonitorEventArgs());
        }

        private void OnExitRegionComplete()
        {
            if (ExitRegionComplete != null) ExitRegionComplete(this, new MonitorEventArgs());
        }
    }
}