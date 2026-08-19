using System.Web.Mvc;

namespace FuelsManager.Areas.DataAnalyticsArea
{
    public class DataAnalyticsAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DataAnalyticsArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DataAnalyticsArea_default",
                "DataAnalyticsArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}