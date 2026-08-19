namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	[Serializable]
	public class DrawingSummaryModel
	{
		public bool DeleteEnabled = true;

		public bool ReadOnly;
		public List<DrawingName> Names { get; set; }

		public DrawingSummaryModel()
		{
		}
	}
}
