using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	 using FMBusinessObjects.DataObjects;
	 using System.Web.Mvc;

	 [Serializable]
	 public class OfflineRollingStockImportModel
	 {
        public Guid SiteGuid;

		  public string logText;
        public OfflineRollingStockImportModel(Guid siteGuid)
		  {
		  			this.SiteGuid = siteGuid;
		  }

        public OfflineRollingStockImportModel(Guid siteGuid, string logText)
        {
            this.SiteGuid = siteGuid;
				this.logText = logText;
        }
    }
}
