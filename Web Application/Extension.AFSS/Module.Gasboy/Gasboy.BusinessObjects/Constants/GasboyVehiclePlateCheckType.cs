using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants
{
    /// <summary>
    /// Enum representing the validation options for Vehicle Plates
    /// </summary>
    public enum GasboyVehiclePlateCheckType
    {
        /// <summary>
        /// Verify as valid Vehicle No - By Proxy
        /// </summary>
        ValidVehicleNo = 1,
        /// <summary>
        /// Verify as valid Device name - By Proxy
        /// </summary>
        ValidDeviceName = 2,
        /// <summary>
        /// Save and Capture - No verification
        /// </summary>
        SaveAndCaptureOnly = 3,
        /// <summary>
        /// Verify as Vehicle No for the Current Device.  Only the Vehicle No associated with the Device record (Fuel Card) is acceptable.
        /// </summary>
        ValidVehicleNoForCurrentDevice = 4
    }
}
