namespace AndroidBeacon.Droid
{
    using AltBeaconOrg.BoundBeacon;
    using Android.App;
    using Android.OS;

    public class BeaconTestRanging : Activity, IBeaconConsumer
    {
        protected static string TAG = "BeaconTestRanging";
        private BeaconManager beaconManager;


        public void OnBeaconServiceConnect()
        {
            beaconManager.AddRangeNotifier(new RangeNotifier());

            try
            {
                beaconManager.StartMonitoringBeaconsInRegion(new Region("myMonitoringUniqueId", null, null, null));
            }
            catch (RemoteException e)
            {}
        }


        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);

            beaconManager = BeaconManager.GetInstanceForApplication(this);
            // To detect proprietary beacons, you must add a line like below corresponding to your beacon
            // type.  Do a web search for "setBeaconLayout" to get the proper expression.

            //MCA BEACON LAYOUT
            beaconManager.BeaconParsers.Add(
                new BeaconParser().SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24"));

            beaconManager.Bind(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            beaconManager.Unbind(this);
        }
    }
}