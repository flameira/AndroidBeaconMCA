using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AndroidBeacon.Droid.helpers
{
    public class BeaconRangedEventArgs : EventArgs
    {
        public bool Inside { get; set; }
        public string Uuid { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public BeaconProximity Proximity { get; set; }
        public double Accuracy { get; set; }
        public double Distance { get; set; }
        public double Rssi { get; set; }

        public bool IsClose 
        { 
            get
            { 
                return Proximity.IsIn(BeaconProximity.Near, BeaconProximity.Immediate); 
            }
        }

       
    }


    public enum BeaconProximity
    {
        Unknown,
        Far,
        Near,
        Immediate
    }
    public class BeaconRegion
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BeaconRegion"/> class.
        /// </summary>
        public BeaconRegion()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Uuid.
        /// </summary>
        /// <value>The Uuid.</value>
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the major.
        /// </summary>
        /// <value>The major.</value>
        public double Major { get; set; }

        /// <summary>
        /// Gets or sets the minor.
        /// </summary>
        /// <value>The minor.</value>
        public double Minor { get; set; }

        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; set; }

        #endregion
    }
    /// <summary>
    /// Interface to be implemented by a beacon monitoring service.
    /// </summary>
    public interface IBeaconMonitoringService
    {
        /// <summary>
        /// Occurs when the region is changed.
        /// </summary>
        event EventHandler<BeaconRangedEventArgs> BeaconRanged;

        /// <summary>
        /// Gets a value indicating whether this service is monitoring.
        /// </summary>
        /// <value><c>true</c> if this servive is monitoring; otherwise, <c>false</c>.</value>
        bool IsMonitoring { get; }

        /// <summary>
        /// Start monitoring beacons
        /// </summary>
        /// <param name="region">The beacon region</param>
        void StartMonitoring(BeaconRegion region);

        /// <summary>
        /// Stop monitoring beacons
        /// </summary>
        void StopMonitoring();	
    }
}