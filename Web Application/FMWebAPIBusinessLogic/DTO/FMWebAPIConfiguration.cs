using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.DTO
{
    public class FMWebAPIConfiguration
    {
        /// <summary>
        /// Controls timeout when calling FMBusinessService
        /// </summary>
        public int FuelsManagerSessionTimeoutMinutes { get; set; }
        public bool EnableAJAXTransactionEntry { get; set; }
    }
}
