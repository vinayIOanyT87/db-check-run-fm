using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using FMBusinessObjects.DataObjects;
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;

	public class EditValueModel
	{
		public PointValue SelectedPointValue { get; set; }

		public SiteClass Site { get; set; }

		public Boolean UpdatePointService { get; set; }

		public Boolean AllowOverUnderRange { get; set; }

		public IEnumerable<SelectListItem> EnumerationList { get; set; }

		  public double TimeZoneOffset { get; set; }

        public string TimeZone { get; set; }

        public string DatepickerTimezoneString { get; set; }

    }
}