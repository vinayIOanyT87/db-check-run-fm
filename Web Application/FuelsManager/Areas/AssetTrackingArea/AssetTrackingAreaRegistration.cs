namespace FuelsManager.Areas.AssetTrackingArea
{
	using System.Web.Mvc;

	public class AssetTrackingAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "AssetTrackingArea";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.Routes.MapMvcAttributeRoutes();

			context.MapRoute(
				"AssetTrackingArea_default",
				"AssetTrackingArea/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional });
		}
	}
}