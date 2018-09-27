namespace AndroidBeacon.Droid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AltBeaconOrg.BoundBeacon;
    using Android.App;
    using Android.Bluetooth;
    using Android.Content;
    using Android.OS;
    using Android.Util;
    using Android.Widget;
    using Java.Lang;


    /// <summary>
    ///     https://stackoverflow.com/questions/24077901/how-to-create-an-always-running-background-service
    /// </summary>
    /// <seealso cref="Android.App.Service" />
    [Service]
    public class BackgroundService : Service, IBeaconConsumer
    {
        public static string DefaultIdentifier = "FCS_Beacon_Region";
        public static string FCSUuid = "FC51BA92-74D5-4972-B2D9-9ECBC62ACFC9";

        public static Runnable runnable;
        private static readonly string TAG = "FLBEACON";
       
        
        //private  IBluetoothLowEnergyAdapter ble;

        public Handler handler;

        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(TAG, "OnBind");
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Toast.MakeText(this, "Service created!", ToastLength.Long).Show();

            handler = new Handler();
            runnable = new Runnable(RunAction);
            handler.PostDelayed(runnable, 15000);
        }


        public override void OnDestroy()
        {
            // base.OnDestroy();
            /* IF YOU WANT THIS SERVICE KILLED WITH THE APP THEN UNCOMMENT THE FOLLOWING LINE */
            //handler.removeCallbacks(runnable);

            Toast.MakeText(this, "Service stopped", ToastLength.Long).Show();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            //TRICK to keep app runing
            return StartCommandResult.Sticky;
        }

        public override void OnTaskRemoved(Intent rootIntent)
        {
            var restartService = new Intent(this, typeof(MainActivity));
            restartService.SetPackage(PackageName);

            var restartServicePI = PendingIntent.GetService(this, 1, restartService, PendingIntentFlags.OneShot);

            var alarmService = (AlarmManager) GetSystemService(AlarmService);

            alarmService.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + 100, restartServicePI);
        }

        //private void logBroadCastResult(IBleAdvertisement adv)
        //{
        //    Log.Info(TAG, $"BrodCastResult");
        //    try
        //    {
        //        Log.Info(TAG, adv.DeviceName);
        //        Log.Info(TAG, adv.Services.Select(x => x.ToString()).Join(","));
        //        Log.Info(TAG, adv.ManufacturerSpecificData.FirstOrDefault().CompanyName());
        //        Log.Info(TAG, adv.ServiceData.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Info(TAG, $"BrodCastResult Error:{ex.Message}");
        //    }
        //    finally
        //    {
        //        Log.Info(TAG, $"BrodCastResultEnd ");
        //    }
        //}

    
        private async void RunAction()
        {

            var ble = BeaconManager.GetInstanceForApplication(ApplicationContext);
            
            ble.BeaconParsers.Add(
                new BeaconParser().SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24"));

            ble.Bind(this);

            ble.StartRangingBeaconsInRegion(new Region(DefaultIdentifier,Identifier.Parse(FCSUuid),null,null));



            ////  ble=  BluetoothLowEnergyAdapter.ObtainDefaultAdapter(ApplicationContext); 

            //var filter = new ScanFilter
            //{
                
            //    //AdvertisedDeviceName = "foobar",
            //    //AdvertisedManufacturerCompanyId = 76,
            //    //AdvertisedServiceIsInList = new List<Guid>() { guid },
            //    IgnoreRepeatBroadcasts = true
            //};
            
            //await ble.ScanForBroadcasts(new ScanSettings
            //{
            //    Mode = ScanMode.LowPower,
            //    // or
            //    //Mode = ScanMode.LowPower,
            //    // if not provided, defaults to
            //    //Mode = ScanMode.Balanced
            //    Filter = filter // You can add your filter here as well
            //}, peripheral =>
            //{
            //    var adv = peripheral.Advertisement;

            //    logBroadCastResult(adv);
            //}, TimeSpan.FromSeconds(1));


            //var devices = ble.DiscoveredPeripherals.ToList();
            //Log.Info(TAG, $"Service is still running, near devices: {devices.Count}");

            //foreach (var device in devices)
            //{
               
            //    Log.Info(TAG,
            //        $"Service is still running, DEVICE: id({device.DeviceId}) rssi({device.Rssi}) adress({string.Join(",", device.Address)}) isRandom({device?.AddressIsRandom}) ");

            //    if (device.DeviceId == Guid.Parse("00000000-0000-0000-0000-474405dd75f1"))
            //    {
            //        Log.Info(TAG, $"FCS Device FOUND!!!: id({device.DeviceId}) adress ({string.Join(",", device.Address)})");

            //                       }
                
            //    else
            //        Log.Info(TAG, $"Device NOt MATCH");
            //}

            Toast.MakeText(this, $"Service is still running, near devices: NA", ToastLength.Long)
                .Show();
            handler.PostDelayed(runnable, 10000);
        }

        public void OnBeaconServiceConnect()
        {
            Log.Info(TAG, $"Service Coonect!!!");
        }
    }
}