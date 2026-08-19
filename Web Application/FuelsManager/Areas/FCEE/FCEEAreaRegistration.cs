using System.Web.Mvc;

namespace FuelsManager.Areas.FCEE
{
	public class FCEEAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "FCEE";
			}
		}

		public override void RegisterArea( AreaRegistrationContext context )
		{
			context.MapRoute(
				"FCEE_default",
				"FCEE/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
