using AndroidBeacon.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(BeaconService))]

namespace AndroidBeacon.Droid
{
    using System;
    using System.Linq;
    using AltBeaconOrg.BoundBeacon;
    using Android.Content;
    using helpers;
    using MonitorNotifier = helpers.MonitorNotifier;
    using Object = Java.Lang.Object;

    /// <summary>
    /// 
    /// </summary>
    public class BeaconService : Object
    {
        private readonly MonitorNotifier _monitorNotifier;
        private readonly RangeNotifier _rangeNotifier;
        private BeaconManager _beaconManager;
        private Region _tagRegion;
        private string _identifier = string.Empty;
        private bool _isBinded;
        private bool _isServiceBound;

        private bool _isStartMonitoringRequested;
        private double _major;
        private double _minor;

        private string _uuid = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public BeaconService()
        {
            _monitorNotifier = new MonitorNotifier();
            _rangeNotifier = new RangeNotifier();
        }

        /// <summary>
        /// 
        /// </summary>
        public BeaconManager BeaconManagerImpl
        {
            get
            {
                if (_beaconManager == null) _beaconManager = InitializeBeaconManager();
                return _beaconManager;
            }
        }

        public bool IsMonitoringBeacons { get; set; }
        public event EventHandler<BeaconRangedEventArgs> BeaconRanged;


        public void InitializeService()
        {
            if (_isBinded)
                return;

            _beaconManager = InitializeBeaconManager();
        }

        public void OnBeaconServiceConnect()
        {
            _isServiceBound = true;

            if (_isStartMonitoringRequested)
            {
                _isStartMonitoringRequested = false;
                StartMonitoring(_uuid, _major, _minor, _identifier);
            }
        }

        public void StartMonitoring(string uuid, double major, double minor, string identifier)
        {
            if (IsMonitoringBeacons)
                return;

            // save region details
            this._uuid = uuid;
            this._major = major;
            this._minor = minor;
            this._identifier = identifier;

            // initiate service
            if (!_isBinded)
                InitializeService();

            //if (!_isServiceBound)
            //{
            //    // service not bound. 
            //    // this happened because we just started the service. Wait for OnBeaconServiceConnect event on MainActivity
            //    _isStartMonitoringRequested = true;
            //    return;
            //}


            // start monitoring
            if (major == 0 && minor == 0)
                _tagRegion = new Region(identifier, Identifier.Parse(uuid), null, null);
            else
                _tagRegion = new Region(identifier, Identifier.Parse(uuid), Identifier.FromInt((int) major),
                    Identifier.FromInt((int) minor));

            BeaconManagerImpl.ForegroundBetweenScanPeriod = 1000;

            BeaconManagerImpl.AddMonitorNotifier(_monitorNotifier);
            _beaconManager.StartMonitoringBeaconsInRegion(_tagRegion);

            BeaconManagerImpl.AddRangeNotifier(_rangeNotifier);
            _beaconManager.StartRangingBeaconsInRegion(_tagRegion);

            IsMonitoringBeacons = true;
        }

        public void StopMonitoring()
        {
            if (!IsMonitoringBeacons)
                return;

            _beaconManager.StopMonitoringBeaconsInRegion(_tagRegion);
            _beaconManager.StopRangingBeaconsInRegion(_tagRegion);

            _beaconManager.Bind((IBeaconConsumer) Forms.Context);
            _isBinded = false;

            IsMonitoringBeacons = false;
        }

     
        private void DeterminedStateForRegionComplete(object sender, MonitorEventArgs e)
        {}

        private void EnteredRegion(object sender, MonitorEventArgs e)
        {
            Console.WriteLine("EnteredRegion");

            // even
            var message = new BeaconRangedEventArgs {Inside = true};
            OnBeaconRanged(message);
        }

        private void ExitedRegion(object sender, MonitorEventArgs e)
        {
            Console.WriteLine("ExitedRegion");

            // even
            var message = new BeaconRangedEventArgs {Inside = false};
            OnBeaconRanged(message);
        }


        private BeaconManager InitializeBeaconManager()
        {
            if (_isBinded)
                return _beaconManager;

            // Enable the BeaconManager 
            var bm = BeaconManager.GetInstanceForApplication(Forms.Context);

            //	Estimote > 2013
            //var iBeaconParser = new BeaconParser();
            //iBeaconParser.SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24");
            //bm.BeaconParsers.Add(iBeaconParser);

            // events
            _monitorNotifier.EnterRegionComplete += EnteredRegion;
            _monitorNotifier.ExitRegionComplete += ExitedRegion;
            _monitorNotifier.DetermineStateForRegionComplete += DeterminedStateForRegionComplete;
            _rangeNotifier.DidRangeBeaconsInRegionComplete += RangingBeaconsInRegion;

            bm.BackgroundMode = false;
            //bm.SetEnableScheduledScanJobs(true);

            bm.Bind((IBeaconConsumer) Forms.Context);

            _isBinded = true;

            return bm;
        }

        private void OnBeaconRanged(BeaconRangedEventArgs beaconData)
        {
            var handler = BeaconRanged;

            if (handler != null)
                BeaconRanged(this, beaconData);
        }

        private void RangingBeaconsInRegion(object sender, RangeEventArgs e)
        {
            if (e.Beacons.Count > 0)
            {
                var beacon = e.Beacons.FirstOrDefault();

                if (beacon.Id1.ToString().Equals(_uuid, StringComparison.OrdinalIgnoreCase))
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
    }
}