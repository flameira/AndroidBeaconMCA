namespace AndroidBeacon.Droid
{
    using System;
    using System.Linq;
    using AltBeaconOrg.BoundBeacon;
    using Android.Bluetooth;
    using Android.Content;
    using Android.Runtime;
    using Android.Util;
    using Android.Widget;
    using helpers;
    using Xamarin.Forms;
    using MonitorNotifier = helpers.MonitorNotifier;
    using Object = Java.Lang.Object;
    using Region = AltBeaconOrg.BoundBeacon.Region;

    /// <summary>
    ///     Class responsible to listening for beacons.
    ///     More details: /// https://github.com/chrisriesgo/Android-AltBeacon-Library
    /// </summary>
    public class BeaconManagerService : Object, IBeaconConsumer
    {
        private bool isBinded;
        private readonly MonitorNotifier _monitorNotifier;
        private readonly RangeNotifier _rangeNotifier;
        private BeaconManager _beaconManager;
        private Region _tagRegion;

        private string uuid = string.Empty;
        private double major;
        private double minor;
        private string identifier = string.Empty;

        private bool isStartMonitoringRequested;
        private bool isServiceBound;


        private LocationManagerService LocationManagerService;
        private MCANotificationService notificationService;




#if DEBUG
        public static int BEACON_LISTENER_TIME_IN_SECONDS = 20;
#else
        public static int BEACON_LISTENER_TIME_IN_SECONDS = 60 * 5; // 5 minutes
#endif

        private static readonly string LOG_FILENAME = "BeaconManagerService";
        private LogService logService;

        public BeaconManagerService(Context ctx)
        {
            ApplicationContext = ctx;
            _monitorNotifier = new MonitorNotifier();
            _rangeNotifier = new RangeNotifier();
            logService = new LogService();
            notificationService = new MCANotificationService();

            InitializeService();

            logService.WriteToLog(LOG_FILENAME, "Contructor");
        }

        public BeaconManager BeaconManagerImpl
        {
            get
            {
                if (_beaconManager == null) _beaconManager = InitializeBeaconManager();
                return _beaconManager;
            }
        }

        public void InitializeService()
        {
            if (isBinded)
                return;

            _beaconManager = InitializeBeaconManager();
        }

        private BeaconManager InitializeBeaconManager()
        {
            if (isBinded)
                return _beaconManager;

            // Enable the BeaconManager 
            var bm = BeaconManager.GetInstanceForApplication(ApplicationContext);

            //  Estimote > 2013
            var iBeaconParser = new BeaconParser();
            iBeaconParser.SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24");
            bm.BeaconParsers.Add(iBeaconParser);

            // events
            _monitorNotifier.EnterRegionComplete += EnteredRegion;
            _monitorNotifier.ExitRegionComplete += ExitedRegion;
            _monitorNotifier.DetermineStateForRegionComplete += DeterminedStateForRegionComplete;
            _rangeNotifier.DidRangeBeaconsInRegionComplete += RangingBeaconsInRegion;

            bm.BackgroundMode = false;
            bm.Bind(this);

            isBinded = true;

            logService.WriteToLog(LOG_FILENAME, "InitializeBeaconManager");

            return bm;
        }

        #region Methods

        private void DeterminedStateForRegionComplete(object sender, MonitorEventArgs e)
        {}

        private void ExitedRegion(object sender, MonitorEventArgs e)
        {
            Log.Debug("FLBEACON", "ExitedRegion");

            // even
            var message = new BeaconRangedEventArgs {Inside = false};
            OnBeaconRanged(message);

            Log.Debug("FLBEACON", "Stop Request Updates from Beacon");
            LocationManagerService.StopRequestLocationUpdates();
            
            notificationService.SendNotification("Beacon", "Exited Region");

            logService.WriteToLog(LOG_FILENAME, "ExitedRegion");
        }

        private void EnteredRegion(object sender, MonitorEventArgs e)
        {
            Log.Debug("FLBEACON", "EnteredRegion");
            // even
            var message = new BeaconRangedEventArgs {Inside = true};
            OnBeaconRanged(message);

            notificationService.SendNotification("Beacon", "Entered Region");

            Log.Debug("FLBEACON", "Get Last Location");
            LocationManagerService.GetLastLocationFromDevice();
           
            Log.Debug("FLBEACON", "Start Request Updates from Beacon");
            LocationManagerService.RequestLocationUpdates();

            logService.WriteToLog(LOG_FILENAME, "EnteredRegion");
        }

        private void RangingBeaconsInRegion(object sender, RangeEventArgs e)
        {
            if (e.Beacons.Count > 0)
            {
                var beacon = e.Beacons.FirstOrDefault();

                if (beacon.Id1.ToString().Equals(uuid, StringComparison.OrdinalIgnoreCase))
                {
                    // convert proximity to internal type
                    var beaconProximity = BeaconProximity.Unknown;
                    if (beacon.Distance > 0)
                        if (beacon.Distance < 0.50)
                            beaconProximity = BeaconProximity.Immediate;
                        else if (beacon.Distance < 2)
                            beaconProximity = BeaconProximity.Near;
                        else
                            beaconProximity = BeaconProximity.Far;

                    // event
                    var message = new BeaconRangedEventArgs
                    {
                        Inside = true,
                        Uuid = beacon.Id1.ToString(),
                        Major = beacon.Id2.ToInt(),
                        Minor = beacon.Id3.ToInt(),
                        Proximity = beaconProximity,
                        Distance = beacon.Distance,
                        Rssi = beacon.Rssi
                    };
                    OnBeaconRanged(message);
                }
            }
        }

        private void OnBeaconRanged(BeaconRangedEventArgs beaconData)
        {
            Log.Debug("FLBEACON", "OnBeaconRanged");
            var handler = BeaconRanged;

            if (handler != null)
                BeaconRanged(this, beaconData);

            logService.WriteToLog(LOG_FILENAME, "OnBeaconRanged");
        }

        #endregion

        #region ILocationManager implementation

        public event EventHandler<BeaconRangedEventArgs> BeaconRanged;

        public bool IsMonitoringBeacons { get; set; }

        public object Location // implementation not necessary for Android
            =>
                null;

        public bool CanUseBluetooth { get; private set; }

        public bool CanUseLocation // implementation not necessary for Android
            =>
                false;

        public Context ApplicationContext { get; }

        public void OnBeaconServiceConnect()
        {
            isServiceBound = true;

            if (isStartMonitoringRequested)
            {
                isStartMonitoringRequested = false;
                StartMonitoring(uuid, major, minor, identifier);
            }
        }

        public BeaconManagerService AddLocationPovider(LocationManagerService location)
        {
            LocationManagerService = location;
            return this;
        }

        public void StartMonitoring(string uuid, double major, double minor, string identifier)
        {
            if (IsMonitoringBeacons)
                return;

            // save region details
            this.uuid = uuid;
            this.major = major;
            this.minor = minor;
            this.identifier = identifier;

            // initiate service
            if (!isBinded)
                InitializeService();

            if (!isServiceBound)
            {
                // service not bound. 
                // this happened because we just started the service. Wait for OnBeaconServiceConnect event on MainActivity
                isStartMonitoringRequested = true;
                return;
            }

            // check bluetooh
            CheckBluetoothEnabled();

            // start monitoring
            if (major == 0 && minor == 0)
                _tagRegion = new Region(identifier, Identifier.Parse(uuid), null, null);
            else
                _tagRegion = new Region(identifier, Identifier.Parse(uuid), Identifier.FromInt((int) major),
                    Identifier.FromInt((int) minor));

            BeaconManagerImpl.ForegroundBetweenScanPeriod = 1000;

            BeaconManagerImpl.AddMonitorNotifier(_monitorNotifier);
            _beaconManager.StartMonitoringBeaconsInRegion(_tagRegion);

            _beaconManager.StartMonitoringBeaconsInRegion(_tagRegion);

            BeaconManagerImpl.AddRangeNotifier(_rangeNotifier);
            _beaconManager.StartRangingBeaconsInRegion(_tagRegion);

            IsMonitoringBeacons = true;

            Log.Debug("FLBEACON", "StartMonitoring");

            logService.WriteToLog(LOG_FILENAME, "StartMonitoring");
        }

        public void StopMonitoring()
        {
            if (!IsMonitoringBeacons)
                return;

            _beaconManager.StopMonitoringBeaconsInRegion(_tagRegion);
            _beaconManager.StopRangingBeaconsInRegion(_tagRegion);

            _beaconManager.Bind(this);
            isBinded = false;

            IsMonitoringBeacons = false;

            logService.WriteToLog(LOG_FILENAME, "StopMonitoring");
        }

        /// <summary>
        ///     Check bluetooth is enabled
        /// </summary>
        public void CheckBluetoothEnabled()
        {
            var bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            // If the adapter is null, then Bluetooth is not supported
            if (bluetoothAdapter == null)
                Toast.MakeText(ApplicationContext, "Bluetooth is not available", ToastLength.Long).Show();
        }

        #endregion

        #region IBeaconConsumer implementation

        public bool BindService(Intent intent, IServiceConnection serviceConnection, [GeneratedEnum] Bind flags)
        {
            //throw new NotImplementedException();
            return true;
        }

        public void UnbindService(IServiceConnection serviceConnection)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}