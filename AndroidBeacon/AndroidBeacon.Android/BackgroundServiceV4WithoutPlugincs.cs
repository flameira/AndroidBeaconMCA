namespace AndroidBeacon.Droid
{
    using System.Collections.Generic;
    using Android.App;
    using Android.Bluetooth;
    using Android.Bluetooth.LE;
    using Android.Content;
    using Android.OS;
    using Android.Text;
    using Android.Util;
    using Android.Widget;
    using Java.Lang;
    using Java.Nio.Charset;
    using ScanMode = Android.Bluetooth.LE.ScanMode;

    /// <summary>
    ///     https://stackoverflow.com/questions/24077901/how-to-create-an-always-running-background-service
    /// </summary>
    /// <seealso cref="Android.App.Service" />
    [Service]
    public class BackgroundService : Service
    {
        public static string DefaultIdentifier = "FCS_Beacon_Region";
        public static string FCSUuid = "FC51BA92-74D5-4972-B2D9-9ECBC62ACFC9";

        public static Runnable runnable;
        private static readonly string TAG = "FLBEACON";
        private BluetoothAdapter _bluetoothAdapter;
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

            var ctx = ApplicationContext;
            var bluetoothManager = (BluetoothManager) ctx.ApplicationContext.GetSystemService(BluetoothService);
            _bluetoothAdapter = bluetoothManager.Adapter;

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

        private List<ScanFilter> GetScanFilters()
        {
            var result = new List<ScanFilter>();

            var uuid = ParcelUuid.FromString("FC51BA92-74D5-4972-B2D9-9ECBC62ACFC9");
            var newScanFilter = new ScanFilter.Builder().SetServiceUuid(uuid).Build();
            result.Add(newScanFilter);

            return result;
        }

        private async void RunAction()
        {
            var settings = new ScanSettings.Builder().SetScanMode(ScanMode.LowPower).Build();
            var filters = GetScanFilters();

            //var intent = new Intent(ApplicationContext, typeof(BackgroundService));
            //intent.PutExtra("o-scan", true);

            //var pendingIntent =
            //    PendingIntent.GetBroadcast(ApplicationContext, 0, intent, PendingIntentFlags.UpdateCurrent);

            //_bluetoothAdapter.BluetoothLeScanner.StartScan(filters, settings,new AppScanCallBack());


            var advSettings = new AdvertiseSettings.Builder().SetAdvertiseMode(AdvertiseMode.LowPower)
                .SetConnectable(false).SetTimeout(1000).Build();

            var advData = new AdvertiseData.Builder()
                .AddServiceUuid(ParcelUuid.FromString("FC51BA92-74D5-4972-B2D9-9ECBC62ACFC9")).Build();

            _bluetoothAdapter.BluetoothLeAdvertiser.StartAdvertising(advSettings, advData, new AppAdvertisingCallBack());


            //Log.Info(TAG, $"Service is still running, near devices: {devices.Count}");

            //if (!devices.Any()) Log.Info(TAG, $"Device NOt MATCH");

            //foreach (var device in devices)
            //    Log.Info(TAG,
            //        $"Service is still running, DEVICE: id({device.BondState}) adress({string.Join(",", device.Address)})) ");

            Toast.MakeText(this, $"Service is still running, near devices: NA", ToastLength.Long).Show();
            handler.PostDelayed(runnable, 10000);
        }
    }

    
    public class AppScanCallBack : ScanCallback
    {
        private static readonly string TAG = "FLBEACON";

        public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
        {

            base.OnScanResult(callbackType, result);
           
            if( result == null
                || result.Device== null)
                return;
            Log.Info(TAG, $"AppScanCallBack " + 
                          $" Address ({result.Device.Address})" + 
                          $" DeviceBoundState({result.Device.BondState})"+
                          $" DeviceClass({result.Device.BluetoothClass.DeviceClass})"+
                          $" DeviceName({result.Device.Name})"+
                          $" DeviceType({result.Device.Type})"+
                          $" AdvertisingId({result.AdvertisingSid})"+
                          $" Rssi({result.Rssi})");
            
            
        }

        public override void OnScanFailed(ScanFailure errorCode)
        {    Log.Info(TAG, "AppScanCallBack onStartFailure: " + errorCode);
            base.OnScanFailed(errorCode);
        }
    }
    public class AppAdvertisingCallBack : AdvertiseCallback
    {
        private static readonly string TAG = "FLBEACON";

        public override void OnStartFailure(AdvertiseFailure errorCode)
        {
            Log.Info(TAG, "Advertising onStartFailure: " + errorCode);
            base.OnStartFailure(errorCode);
        }

        public override void OnStartSuccess(AdvertiseSettings settingsInEffect)
        {
            base.OnStartSuccess(settingsInEffect);
        }
    }
}