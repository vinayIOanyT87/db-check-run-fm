using System.Web.Mvc;

namespace FuelsManager.Areas.InventoryManagement
{
	public class InventoryManagementAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "InventoryManagement";
			}
		}

		public override void RegisterArea( AreaRegistrationContext context )
		{
			context.MapRoute(
				"InventoryManagement_default",
				"InventoryManagement/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
