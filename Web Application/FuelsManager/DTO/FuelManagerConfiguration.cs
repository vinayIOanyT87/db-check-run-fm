using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuelsManager.DTO
{
    public class FuelManagerConfiguration
    {
        public FuelManagerConfiguration()
        {
            EnableAjaxTransactionScreen = false;
        }

        /// <summary>
        /// This option will disable the legacy transaction edit screen and replace it with the new AJAX transaction screen
        /// </summary>
        public bool EnableAjaxTransactionScreen { get; set; }
    }
}