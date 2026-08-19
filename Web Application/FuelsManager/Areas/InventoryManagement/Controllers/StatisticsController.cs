namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;

	using Areas.Controllers;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using ViewModels;

	public class StatisticsController : FMBaseController
	{

		[HttpGet]
		public ActionResult StatisticsSummary()
		{
			StatisticsSummaryModel model = new StatisticsSummaryModel(null);

			try
			{
				var context = this.Session[StatisticsSummaryContext.SessionKey] as StatisticsSummaryContext;
				if (context == null)
				{

					model = new StatisticsSummaryModel(context);
					model.PointServices = FMChannelHelper.MakeCall<IPointServices, List<PointService>>(x => x.Enumerate(this.Security));
					if (model.PointServiceList == null || model.PointServices.Count == 0)
					{
						throw new Exception("No Point Services Available");
					}

					model.SelectedPointService = model.PointServices[0];
				}
				else
				{
					model = context.Model;
				}

				model.GetStatistics(this.Security, model.SelectedPointService);

				this.Session[StatisticsSummaryContext.SessionKey] = new StatisticsSummaryContext(model);

				return this.View(model);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return this.View("StatisticsSummary", model);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ButtonSubmit(string command)
		{
			if (this.ModelState.IsValid)
			{
				var context = this.Session[StatisticsSummaryContext.SessionKey] as StatisticsSummaryContext;
				StatisticsSummaryModel model = context.Model;

				try
				{
					if (string.IsNullOrEmpty(command) == false)
					{
						if (command.Equals("resetButton"))
						{
							model.ResetStatistics(this.Security, model.SelectedPointService);
						}
					}
				}
				catch (Exception except)
				{
					this.ErrorHandler(except);
					return this.View("StatisticsSummary", model);
				}
			}

			return this.RedirectToAction("StatisticsSummary");
		}

		[HttpGet]
		public ActionResult PointServiceSelectionChanged(string id)
		{
			if (this.ModelState.IsValid)
			{
				var context = this.Session[StatisticsSummaryContext.SessionKey] as StatisticsSummaryContext;

				StatisticsSummaryModel model = context.Model;

				model.SelectedPointService = model.PointServices.Find(x => x.PointServiceGuid == new Guid(id));

				model.GetStatistics(this.Security, model.SelectedPointService);

				return this.View("StatisticsSummary", model);
			}

			return this.RedirectToAction("StatisticsSummary");
		}
	}
}
