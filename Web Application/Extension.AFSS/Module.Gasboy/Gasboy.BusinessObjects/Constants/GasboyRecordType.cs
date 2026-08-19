using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants
{
    /// <summary>
    /// Enum representing native Gasboy record (Object) types.
    /// </summary>
    public enum GasboyRecordType
    {
        /// <summary>
        /// Limits Rule Record
        /// </summary>
        LimitsRule = 3,
        /// <summary>
        /// Visits Rule Record
        /// </summary>
        VisitsRule = 5,
        /// <summary>
        /// Fuel Rule Record
        /// </summary>
        FuelsRule= 7,
        /// <summary>
        /// Group Rule Record
        /// </summary>
        GroupRule = 9,
        /// <summary>
        /// Model Record
        /// </summary>
        Model = 11,
        /// <summary>
        /// Fleet Record
        /// </summary>
        Fleet = 14,
        /// <summary>
        /// Department Record
        /// </summary>
        Department = 16,
        /// <summary>
        /// Mean or Device Record
        /// </summary>
        Mean = 19
    }
}
