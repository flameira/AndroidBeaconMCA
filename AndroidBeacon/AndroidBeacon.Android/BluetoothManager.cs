namespace AndroidBeacon.Droid
{
    using System.Collections.Generic;
    using Android.Content;
    using nexus.protocols.ble;
    using nexus.protocols.ble.scan;
    using Xamarin.Forms;

    public class BluetoothManager 
    {
        private IBluetoothLowEnergyAdapter ble;

        public BluetoothManager()
        {
            ble = (Forms.Context as MainActivity).ble;
            EnableBle();
        }

        private async void EnableBle()
        {
            if (ble.AdapterCanBeEnabled && ble.CurrentState.IsDisabledOrDisabling())
                await ble.EnableAdapter();
        }

        public IEnumerator<IBlePeripheral> devices()
        {
           return ble.DiscoveredPeripherals.GetEnumerator();
        }

    }
}