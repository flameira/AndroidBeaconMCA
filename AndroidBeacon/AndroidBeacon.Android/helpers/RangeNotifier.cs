namespace AndroidBeacon.Droid.helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AltBeaconOrg.BoundBeacon;
    using Android.Util;
    using Object = Java.Lang.Object;

    public class RangeEventArgs : EventArgs
    {
        public ICollection<Beacon> Beacons { get; set; }
        public Region Region { get; set; }
    }

    public class RangeNotifier : Object, IRangeNotifier
    {
         static  string TAG = "FLBEACON";


        public event EventHandler<RangeEventArgs> DidRangeBeaconsInRegionComplete;

        public void DidRangeBeaconsInRegion(ICollection<Beacon> beacons, Region region)
        {
            OnDidRangeBeaconsInRegion(beacons, region);
        }

        private void OnDidRangeBeaconsInRegion(ICollection<Beacon> beacons, Region region)
        {
            if (DidRangeBeaconsInRegionComplete != null)
            { 
                if (beacons.Any())
                {
                    var firstBeacon = beacons.First();

                    Log.Info(TAG, $"Found({beacons.Count}). The first  beacon ({firstBeacon.Id1}) I see is about "+Math.Round(firstBeacon.Distance,2)+" meters away.");
                    Console.Write($"Found({beacons.Count}). The first  beacon ({firstBeacon.Id1}) I see is about "+Math.Round(firstBeacon.Distance,2)+" meters away.");
                }
                DidRangeBeaconsInRegionComplete(this, new RangeEventArgs { Beacons = beacons, Region = region });
            }
        }

    }
}