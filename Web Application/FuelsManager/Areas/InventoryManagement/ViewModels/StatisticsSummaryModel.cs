namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMBusinessObjects.UtilityObjects;
	using FMCore;

	public interface IStatisticsSummaryModel
	{
		void GetStatistics(SecurityClass security, PointService pointService);

		void ResetStatistics(SecurityClass security, PointService pointService);
	}

	[Serializable]
	public class StatisticsSummaryModel : IStatisticsSummaryModel
	{
		public List<Statistic> Statistics { get; set; }

		public List<PointService> PointServices { get; set; }

		public PointService SelectedPointService { get; set; }

		public StatisticsSummaryModel()
		{
		}

		public StatisticsSummaryModel( StatisticsSummaryContext context )
		{
			this.InitFromContext(context);
		}

		private void InitFromContext(StatisticsSummaryContext context)
		{
			if (context != null && context.Model != null)
			{
				this.Statistics = context.Model.Statistics;
			}
		}

		public IEnumerable<SelectListItem> PointServiceList
		{
			get
			{
				var allPointServices =
					this.PointServices.Select(pointService => new SelectListItem() { Value = pointService.IdentityGuid.ToString(), Text = pointService.Hostname });

				return allPointServices;
			}
		}


		public StatisticsSummaryModel(StatisticsSummaryModel model, StatisticsSummaryContext context)
		{
				this.InitFromContext(context);

				// TODO: Copy any display bound selections -- or should I leave that to the Controller?
		}

		public void ResetStatistics(SecurityClass security, PointService pointService)
		{
				security.ThrowIfNull("security");

				FMChannelHelper.MakeCall<IPointServiceManager>(x => x.ResetStatistics(security, pointService));
		}

		public void GetStatistics(SecurityClass security, PointService pointService)
		{
			security.ThrowIfNull("security");

			this.Statistics =	FMChannelHelper.MakeCall<IPointServiceManager, List<Statistic>>(x => x.GetStatistics(security, pointService));

			this.Statistics = this.Statistics.OrderBy(x => x.Name).ToList();

		}
	}
}
