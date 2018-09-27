namespace AndroidBeacon.Droid
{
    using System.Linq;
    using AltBeaconOrg.BoundBeacon;
    using AltBeaconOrg.BoundBeacon.Startup;
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.Util;

   
    public class AppTestV2 : Application, IBootstrapNotifier
    {
        private const long backgroundIntervalScanInSeconds = 15; // frequency we listen for beacons

        private const long backgroundScanTimeInMilliseconds = 1100; // time listening for beacons
        private const int beaconListenerTimeInSecconds = 60 * 5;
        public const string TAG = "MileageCountTEST";
        private static BeaconManager beaconManager;


        private bool initialised;
        private RangeNotifier rangeNotifier;
        private RegionBootstrap regionBootstrap;

        public void DidDetermineStateForRegion(int state, Region region)
        {
            // Don't care
        }

        public void DidEnterRegion(Region region)
        {
            Log.Debug(TAG, "Got a didEnterRegion call");
            // This call to disable will make it so the activity below only gets launched the first time a beacon is seen (until the next time the app is launched)
            // if you want the Activity to launch every single time beacons come into view, remove this call.  
            regionBootstrap.Disable();
            var intent = new Intent(this, typeof(MainActivity));
            // IMPORTANT: in the AndroidManifest.xml definition of this activity, you must set android:launchMode="singleInstance" or you will get two instances
            // created when a user launches the activity manually and it gets launched from here.
            intent.SetFlags(ActivityFlags.NewTask);
            StartActivity(intent);
        }

        public void DidExitRegion(Region region)
        {
            // Don't care
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug(TAG, "App started up");
            InitialiseBeacons();


            // wake up the app when any beacon is seen (you can specify specific id filers in the parameters below)
            var region = new Region("com.wws.boostrapRegion", null, null, null);
            regionBootstrap = new RegionBootstrap(this, region);


            Log.Debug(TAG, "finished OnCreate");
        }


        private void InitialiseBeacons()
        {
            if (initialised) return;

            Log.Debug(TAG, "InitialiseBeacons");

            // initialise beacon manage
            beaconManager = BeaconManager.GetInstanceForApplication(this);

            // add pars
            beaconManager.BeaconParsers.Add(
                new BeaconParser().SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24"));

            // set event
            rangeNotifier = new RangeNotifier();
            beaconManager.AddRangeNotifier(rangeNotifier);
            rangeNotifier.DidRangeBeaconsInRegionComplete += RangingBeaconsInRegion;

            // set background interval
            beaconManager.BackgroundScanPeriod = backgroundScanTimeInMilliseconds;
            beaconManager.BackgroundBetweenScanPeriod = backgroundIntervalScanInSeconds * 1000;

            // Simply constructing this class and holding a reference to it in your custom Application class
            // enables auto battery saving of about 60%
            // backgroundPowerSaver = new BackgroundPowerSaver(this);

            Log.Debug(TAG, "End InitialisedBeacons");

            initialised = true;
        }

        private async void RangingBeaconsInRegion(object sender, RangeEventArgs e)
        {
            if (e.Beacons.Count > 0)
            {
                var beacon = e.Beacons.FirstOrDefault();

                Log.Debug(TAG, "Rssi " + beacon.Rssi);
            }
        }
    }
}