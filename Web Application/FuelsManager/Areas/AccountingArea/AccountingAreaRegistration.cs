using System.Web.Mvc;

namespace FuelsManager.Areas.AccountingArea
{
	public class AccountingAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "AccountingArea";
			}
		}

		public override void RegisterArea( AreaRegistrationContext context )
		{
			context.Routes.MapMvcAttributeRoutes();

			context.MapRoute(
				"AccountingArea_default",
				"AccountingArea/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional });
		}
	}
}
