namespace AndroidBeacon.Droid
{
    using System.Collections.Generic;
    using Android.App;
    using Android.Bluetooth;
    using Android.Bluetooth.LE;
    using Android.Content;
    using Android.OS;
    using Android.Util;
    using Android.Widget;
    using ScanMode = Android.Bluetooth.LE.ScanMode;

    [Service]

    public class MyService : Service
    {
        private static readonly string TAG = typeof(MyService).FullName;
        private readonly Context _context;

        public MyService()
        {
            _context = this;
        }

        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(TAG, "OnBind");
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Start();
        }

        public void Start()
        {
            var bluetoothManager = (BluetoothManager) _context.ApplicationContext.GetSystemService(BluetoothService);
            var bluetoothAdapter = bluetoothManager.Adapter;

            var myBeaconService = new MyBeaconService(_context);
            var intent = new Intent(_context, typeof(MyBeaconService));
            intent.PutExtra("o-scan", true);

            var settings = new ScanSettings.Builder().SetScanMode(ScanMode.LowPower).Build();
            var filters = GetScanFilters(); // Make a scan filter matching the beacons I care about
            var pendingIntent = PendingIntent.GetBroadcast(_context, 0, intent, PendingIntentFlags.UpdateCurrent);
            bluetoothAdapter.BluetoothLeScanner.StartScan(filters, settings, pendingIntent);

            Toast.MakeText(_context, "MyService created!", ToastLength.Long).Show();
        }

        private List<ScanFilter> GetScanFilters()
        {
            var result = new List<ScanFilter>();

            var uuid = ParcelUuid.FromString("FC51BA92-74D5-4972-B2D9-9ECBC62ACFC9");
            var newScanFilter = new ScanFilter.Builder().SetServiceUuid(uuid).Build();
            result.Add(newScanFilter);

            return result;
        }
    }
}