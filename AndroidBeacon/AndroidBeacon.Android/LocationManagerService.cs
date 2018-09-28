namespace AndroidBeacon.Droid
{
    using System;
    using System.Linq;
    using Android;
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.Locations;
    using Android.OS;
    using Android.Runtime;
    using Android.Support.V4.Content;
    using Android.Util;

    public class LocationManagerService : Java.Lang.Object, ILocationListener
    {
       
        private const long FiveSeconds = 5 * 1000;
        private const long OneMinute = 60 * 1000;

        private static readonly string LogFilename = "LocationManagerService";
        private static readonly string Tag = "FLBEACON";
 

        private readonly LocationManager _locationManager;
        private readonly LogService _logService;
        private HandlerThread _handlerThread;
        private HandlerThread _handlerThreadSingle;
        private bool _isRequestingLocationUpdates;

        public LocationManagerService (IntPtr a, JniHandleOwnership b) : base (a, b)
        {
            _locationManager = (LocationManager) Application.Context.GetSystemService(Context.LocationService);
         
            _logService = new LogService();
            _logService.WriteToLog(LogFilename, "Contructor");
        }
        public LocationManagerService()
        {
            _locationManager = (LocationManager) Application.Context.GetSystemService(Context.LocationService);
         
            _logService = new LogService();
            _logService.WriteToLog(LogFilename, "Contructor");
        }


        public IntPtr Handle { get; }

        public void OnLocationChanged(Location location)
        {
            var message =
                $"Location Changed lat:{location.Latitude} long:{location.Longitude} prov:{location.Provider} acc:{location.Accuracy} speed:{location.Speed}";
            Log.Info(Tag, message);

            _logService.WriteToLog(LogFilename, message);
        }

        public void OnProviderDisabled(string provider)
        {
            _isRequestingLocationUpdates = false;
            Log.Info(Tag, $"Location Provider:{provider} is Disabled");

            _logService.WriteToLog(LogFilename, $"Location Provider:{provider} is Disabled");
        }

        public void OnProviderEnabled(string provider)
        {
            Log.Info(Tag, $"Location Provider:{provider} is enabled");
            _logService.WriteToLog(LogFilename, $"Location Provider:{provider} is enabled");
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            Log.Info(Tag, $"Location status changed:{provider} is {status.ToString()}");
            _logService.WriteToLog(LogFilename, $"Location status changed:{provider} is {status.ToString()}");

            if (status == Availability.OutOfService)
            {
                StopRequestingLocationUpdates();
                _isRequestingLocationUpdates = false;
            }
        }

        public void GetLastLocationFromDevice()
        {
            var criteria = new Criteria {PowerRequirement = Power.Medium};

            var bestProvider = _locationManager.GetBestProvider(criteria, true);
            var location = _locationManager.GetLastKnownLocation(bestProvider);

            var message = "";
            if (location != null)
                message =
                    $"Last know location Changed lat:{location.Latitude} long:{location.Longitude} prov:{location.Provider}";
            else
                message = $"Last know location unavailable";

            Log.Info(Tag, message);
            _logService.WriteToLog(LogFilename, message);
        }

        public void RequestLocationUpdates()
        {
            try
            {
                if (ContextCompat.CheckSelfPermission(Application.Context, Manifest.Permission.AccessFineLocation) ==
                    Permission.Granted)
                {
                    if (!_isRequestingLocationUpdates)
                    {
                        StartRequestingLocationUpdates();
                        _isRequestingLocationUpdates = true;
                    }
                }
                else
                {
                    _logService.WriteToLog(LogFilename, $"is already requesting location updates");
                }
            }
            catch (Exception ex)
            {
                _logService.WriteToLog("Exceptions", $"({LogFilename}) {ex.Message}");
            }
        }

        public void StopRequestLocationUpdates()
        {
            try
            {
                _isRequestingLocationUpdates = false;
                StopRequestingLocationUpdates();
            }
            catch (Exception ex)
            {
                _logService.WriteToLog("Exceptions", $"({LogFilename}) {ex.Message}");
            }
        }

        private void StartRequestingLocationUpdates()
        {
            _locationManager.RemoveUpdates(this);
            if (_handlerThread != null) //ensure there is only one thread running
            {
                _handlerThread.Quit();
                _handlerThread = null;
            }

            _handlerThread = new HandlerThread("LocationLoggerThread", (int) ThreadPriority.Background);
            _handlerThread.Start();

            Log.Info(Tag, $"Request location updates start");
            _logService.WriteToLog(LogFilename, $"Request location updates start");

            try
            {
                _locationManager.RequestLocationUpdates(LocationManager.GpsProvider, FiveSeconds, 1, this);
            }
            catch (Exception ex)
            {
                _logService.WriteToLog("Exceptions", $"({LogFilename}) {ex.Message}");
            }
        }

        private void StopRequestingLocationUpdates()
        {
            Log.Info(Tag, $"Request location updates stop");
            _logService.WriteToLog(LogFilename, $"Request location updates stop");

            _locationManager.RemoveUpdates(this);
            if (_handlerThread != null)
            {
                _handlerThread.Quit();
                _handlerThread.Interrupt();
                _handlerThread = null;
            }

            if (_handlerThreadSingle != null)
            {
                _handlerThreadSingle.Quit();
                _handlerThreadSingle.Interrupt();
                _handlerThreadSingle = null;
            }
        }
    }

}