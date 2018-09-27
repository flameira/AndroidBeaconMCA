namespace AndroidBeacon.Droid
{
    using Android.App;
    using Android.Bluetooth.LE;
    using Android.Content;
    using Android.Util;
    using Android.Widget;

    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    [IntentFilter(new[] { Intent.ActionLocaleChanged })]
    [IntentFilter(new[] { Intent.CategoryCarDock })]
    public class MyBeaconService : BroadcastReceiver
    {
        private readonly Context _context;

        public MyBeaconService()
        {
            
        }
        public MyBeaconService(Context ctx)
        {
            _context = ctx;

            Toast.MakeText(_context, "MyBeaconService", ToastLength.Long).Show();
        }

        public override void OnReceive(Context context, Intent intent)
        {
            var bleCallbackType = intent.GetIntExtra(BluetoothLeScanner.ExtraCallbackType, -1);
            if (bleCallbackType != -1)
            {
                var message = "Passive background scan callback type: " + bleCallbackType;
                Toast.MakeText(_context, message, ToastLength.Long).Show();

                var scanResults = intent.GetParcelableArrayListExtra(BluetoothLeScanner.ExtraListScanResult);
                message = "Result: " + scanResults;
                
                Log.Debug("BEACONTESTAPP",message);

                Toast.MakeText(_context, message, ToastLength.Long).Show();

                // Do something with your ScanResult list here.
                // These contain the data of your matching BLE advertising packets
            }
        }
    }
}