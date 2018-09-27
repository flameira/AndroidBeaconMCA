using AndroidBeacon.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(MonotoringService))]

namespace AndroidBeacon.Droid
{
    using System;
    using AltBeaconOrg.BoundBeacon;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using helpers;
    using Xamarin.Forms;
    using Region = AltBeaconOrg.BoundBeacon.Region;

    public class MonotoringService : Object
    {
        public static string DefaultIdentifier = "FCS_Beacon_Region";
        public static string FCSUuid = "FC51BA92-74D5-4972-B2D9-9ECBC62ACFC9";

        private static readonly string TAG = typeof(MonotoringService).FullName;
        private readonly AppMonitorNotifier _monitorNotifier;
        private readonly BeaconManager beaconManager;

        public MonotoringService()
        {
            beaconManager = BeaconManager.GetInstanceForApplication(Forms.Context);
            beaconManager.Bind((IBeaconConsumer)Forms.Context);
            _monitorNotifier = new AppMonitorNotifier();
        }


        MonotoringService monotoringservice;
        public MonotoringService GetMonotoringService()
        {
            if (monotoringservice == null) monotoringservice = DependencyService.Get<MonotoringService>();
            return monotoringservice;
        }


        public void OnBeaconServiceConnect()
        {
            beaconManager.AddMonitorNotifier(_monitorNotifier);

            try
            {
                beaconManager.StartMonitoringBeaconsInRegion(new Region(DefaultIdentifier, Identifier.Parse(FCSUuid),null, null));
            }
            catch (RemoteException e)
            { }
        }


    }
}