using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using FMBusinessObjects.DataObjects;
	using System;
	using System.Collections.Generic;
	using System.Globalization;

	[Serializable]
	public class PointTagSelectionModel
	{
		public Dictionary<Guid, PointTag> PointTags { get; set; }


		public PointTagSelectionModel()
		{
		}
	}
}
