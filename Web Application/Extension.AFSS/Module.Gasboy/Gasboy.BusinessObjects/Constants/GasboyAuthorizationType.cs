using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants
{
    /// <summary>
    /// Enum representing native authorization types for the Gasboy devices
    /// </summary>
    public enum GasboyAuthorizationType
    {
        /// <summary>
        /// Fuelopass
        /// </summary>
        Fuelopass = 1,
        /// <summary>
        /// Fuel Card
        /// </summary>
        FuelCard = 14,
        /// <summary>
        /// Manual Entry
        /// </summary>
        ManualEntry = 21
    }
}
