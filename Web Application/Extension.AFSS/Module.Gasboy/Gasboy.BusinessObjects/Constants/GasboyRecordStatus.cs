using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants
{
    /// <summary>
    /// Enum representing native Gasboy status values.
    /// </summary>
    public enum GasboyRecordStatus
    {
        /// <summary>
        /// Deleted
        /// </summary>
        Deleted = 0,
        /// <summary>
        /// Blocked
        /// </summary>
        Blocked = 1,
        /// <summary>
        /// Active
        /// </summary>
        Active = 2
    }
}
