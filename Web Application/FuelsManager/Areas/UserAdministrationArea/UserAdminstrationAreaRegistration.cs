namespace FuelsManager.Areas.UserAdministrationArea
{
    using System.Web.Mvc;

    public class UserAdminstrationAreaRegistration : AreaRegistration
    {
        public override string AreaName => "UserAdministrationArea";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapMvcAttributeRoutes();

            context.MapRoute(
                "UserAdministrationArea_default",
                "UserAdministrationArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional });
        }
    }
}