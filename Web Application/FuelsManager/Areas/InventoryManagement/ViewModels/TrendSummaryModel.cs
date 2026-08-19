namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	[Serializable]
	public class TrendSummaryModel
	{
		public List<TrendName> Names { get; set; }

		public bool ModifyTrendsRight;

		public TrendSummaryModel()
		{
		}
	}
}