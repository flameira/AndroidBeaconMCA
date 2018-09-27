namespace AndroidBeacon.Droid
{
    using System;
    using Android.App;
    using Android.Content;
    using Android.Locations;
    using Android.OS;
    using Android.Util;
    using Object = Java.Lang.Object;
    public class LocationManagerService : Object, ILocationListener
    {
        private const long FIVE_MINUTES = 5 * ONE_MINUTE;
        private const long FIVE_Seconds = 5 * 1000;
        private const long ONE_MINUTE = 60 * 1000;
        private static readonly string Tag = "FLBEACON";

        private LocationManager LocationManager;

        private bool isRequestingLocationUpdates;
        private Context applicationContext;

        public LocationManagerService()
        {
          //  LocationManager = Application.Context.GetSystemService(Context.LocationService) as LocationManager;
        }
        public LocationManagerService(Context applicationContext)
        {
            this.applicationContext = applicationContext;
            LocationManager = applicationContext.GetSystemService(Context.LocationService) as LocationManager;
        }

        public void Dispose()
        {
            LocationManager.RemoveUpdates(this);
            Log.Info(Tag, $"Dispose");
        }

        public IntPtr Handle { get; }

        public void OnLocationChanged(Location location)
        {
            Log.Info(Tag,
                $"Location Changed lat:{location.Latitude} long:{location.Longitude} prov:{location.Provider}");
        }

        public void OnProviderDisabled(string provider)
        {
            isRequestingLocationUpdates = false;
            Log.Info(Tag, $"Location Provider:{provider} is Disabled");
        }

        public void OnProviderEnabled(string provider)
        {
            Log.Info(Tag, $"Location Provider:{provider} is enabled");
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            Log.Info(Tag, $"Location status changed:{provider} is {status.ToString()}");

            if (status == Availability.OutOfService)
            {
                StopRequestingLocationUpdates();
                isRequestingLocationUpdates = false;
            }
        }

        public void GetLastLocationFromDevice()
        {
            var criteria = new Criteria {PowerRequirement = Power.Medium};

            var bestProvider = LocationManager.GetBestProvider(criteria, true);
            var location = LocationManager.GetLastKnownLocation(bestProvider);

            if (location != null)
                Log.Info(Tag,
                    $"Last know location Changed lat:{location.Latitude} long:{location.Longitude} prov:{location.Provider}");
            else
                Log.Info(Tag, $"Last know location unavailable");
        }

        public void RequestLocationUpdates()
        {
            if (!isRequestingLocationUpdates)
            {
                StartRequestingLocationUpdates();
                isRequestingLocationUpdates = true;
            }
        }

        public void StopRequestLocationUpdates()
        { isRequestingLocationUpdates = false;
            StopRequestingLocationUpdates();
        }


        private void StartRequestingLocationUpdates()
        {
            Log.Info(Tag, $"Request location updates start");
            LocationManager.RequestLocationUpdates(LocationManager.GpsProvider, FIVE_Seconds, 1, this);
        }

        private void StopRequestingLocationUpdates()
        {
            Log.Info(Tag, $"Request location updates stop");

            LocationManager.RemoveUpdates(this);
        }
    }
}