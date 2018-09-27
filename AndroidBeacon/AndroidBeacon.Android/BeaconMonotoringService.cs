using AndroidBeacon.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(BeaconMonitoringService))]

namespace AndroidBeacon.Droid
{
    using System;
    using helpers;

    public class BeaconMonitoringService : IBeaconMonitoringService
    {
        private readonly BeaconService beaconService;
        private bool isLinkingBeacon;
        private double previousRssi;

        public BeaconMonitoringService()
        {
            // LocationManger contains a complete implementation to communicate with beacon in foreground
            // https://github.com/chrisriesgo/Android-AltBeacon-Library
            beaconService = (Forms.Context as MainActivity).GetBeaconService();
            beaconService.BeaconRanged += LocationManager_BeaconRanged;

            // if we are using this DependencyService to check the proximity, we only 
            // need to read the event handler to get all details from the beacon
            //MileageCountApp.Instance.BeaconRanged += LocationManager_BeaconRanged;
        }

        public event EventHandler<BeaconRangedEventArgs> BeaconRanged;

        public bool IsMonitoring => beaconService.IsMonitoringBeacons;

        public void StartMonitoring(BeaconRegion region)
        {
            // call only necessary for linking beacons as this will use a different mechanism more reliable for use in foregorund
            if (IsMonitoring) return;

            if (region.Major == 0 && region.Minor == 0)
            {
                // Different mechanism to link beacons. more details in this constructor
                isLinkingBeacon = true;
                beaconService.StartMonitoring(region.Uuid, 0, 0, region.Identifier);
            }
        }

        public void StopMonitoring()
        {
            // call only necessary for linking beacons as this will use a different mechanism more reliable for use in foregorund
            if (!IsMonitoring) return;

            if (isLinkingBeacon) beaconService.StopMonitoring();
        }

        private void LocationManager_BeaconRanged(object sender, BeaconRangedEventArgs beaconData)
        {
            var sendEventArgs = true;

            if (beaconData.Inside)
            {
                if (beaconData.Proximity != BeaconProximity.Unknown)
                    sendEventArgs = isLinkingBeacon || !beaconData.Rssi.Equals(previousRssi);

                previousRssi = beaconData.Rssi;
            }

            if (sendEventArgs) OnBeaconRanged(beaconData);
        }

        private void OnBeaconRanged(BeaconRangedEventArgs beaconData)
        {
            var handler = BeaconRanged;

            if (handler != null)
                BeaconRanged(this, beaconData);
        }
    }
}