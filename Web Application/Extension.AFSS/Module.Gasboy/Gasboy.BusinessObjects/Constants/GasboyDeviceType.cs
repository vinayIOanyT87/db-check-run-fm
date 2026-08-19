using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants
{
    /// <summary>
    /// Enum representing native device types for the Gasboy devices
    /// </summary>
    public enum GasboyDeviceType
    {
        /// <summary>
        /// Employee Tag
        /// </summary>
        EmployeeTag = 1,
        /// <summary>
        /// Vehicle
        /// </summary>
        Vehicle = 2,
        /// <summary>
        /// Vehicle Mounted
        /// </summary>
        VehicleMounted = 3,
        /// <summary>
        /// Driver
        /// </summary>
        Driver = 4,
        /// <summary>
        /// Customer Tag
        /// </summary>
        CustomerTag = 5
    }
}
