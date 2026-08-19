namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;

	[Serializable]
	public class StatisticsSummaryContext
	{
		public const string SessionKey = "StatisticsSummaryContext";

		public StatisticsSummaryModel Model;

		public StatisticsSummaryContext()
		{
		}

		public StatisticsSummaryContext( StatisticsSummaryModel model )
		{
			this.Model = model;
		}
	}
}
