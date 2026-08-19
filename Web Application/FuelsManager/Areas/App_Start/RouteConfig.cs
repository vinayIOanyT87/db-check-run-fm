namespace FuelsManager.Areas.App_Start
{
	using System.Web.Mvc;
	using System.Web.Routing;

	public class RouteConfig
	{
		public static void RegisterRoutes( RouteCollection routes )
		{
			routes.RouteExistingFiles = false;

			routes.IgnoreRoute( "Content/{*pathInfo}" );
			routes.IgnoreRoute( "Scripts/{*pathInfo}" );
			routes.IgnoreRoute( "Styles/{*pathInfo}" );
		    routes.IgnoreRoute("api/{*pathInfo}");
            routes.IgnoreRoute( "{*favicon}",
				new { favicon = @"(.*/)?favicon.([iI][cC][oO]|[gG][iI][fF])(/.*)?" } );
			
			// This one ensures that default IIS documents are found.
			routes.IgnoreRoute( "" );

			//Ignore handlers and resources
			routes.IgnoreRoute( "{resource}.ashx/{*pathInfo}" );
			routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" );


			routes.MapMvcAttributeRoutes();
			//If controller part of the UTL is not any of the listed controllers, then call fileNoteFoundController
			//which will redirect to login page.
			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
				constraints: new { controller="Home|FMBase|TransactionEditor|TransactionSummary|SessionInvalid|FileNotFound"} 
			);

			routes.MapRoute(
				name: "Redirect",
				url: "{webappdir}/{*pathInfo}",
				defaults: new { controller = "FileNotFound", action = "NotFound", id = UrlParameter.Optional }
			);

         routes.MapPageRoute("PopupReportLandingPage", "FMReportWebMain/PopupReportLandingPage.aspx", "~/FMReportWebMain/PopupReportLandingPage.aspx");
         routes.MapPageRoute("FuelsManagerForm", "FMWebApp/FuelsManagerForm.aspx", "~/FMWebApp/FuelsManagerForm.aspx");
         routes.MapPageRoute("LogoutForm", "FMWebApp/LogoutForm.aspx", "~/FMWebApp/LogoutForm.aspx");
         routes.MapPageRoute("ChangePasswordForm", "FMWebApp/ChangePasswordForm.aspx", "~/FMWebApp/ChangePasswordForm.aspx");
      }
   }
}
