using System.Web.Mvc;

namespace FuelsManager.Areas.MainArea
{
	public class MainAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "MainArea";
			}
		}

		public override void RegisterArea( AreaRegistrationContext context )
		{
			context.MapRoute(
				"Main_default",
				"MainArea/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
