using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants
{
    /// <summary>
    /// Enum representing native two stage drive validation types for the Gasboy devices
    /// </summary>
    public enum GasboyTwoStageDriverValidationType
    {
        /// <summary>
        /// Not Selected
        /// </summary>
        NotSelected = 0,
        /// <summary>
        /// Filtered List of Driver Means
        /// </summary>
        SelectedDrivers = 2,
        /// <summary>
        /// Any driver in specified department(s)
        /// </summary>
        SelectedDepartments = 3,
        /// <summary>
        /// Any driver in same Fleet
        /// </summary>
        AnyDriverSameFleet = 4,
        /// <summary>
        /// Any driver in any Fleet
        /// </summary>
        AnyDriverAnyFleet = 5
    }
}
