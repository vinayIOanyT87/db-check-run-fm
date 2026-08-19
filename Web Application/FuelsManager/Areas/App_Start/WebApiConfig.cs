namespace FuelsManager.Areas.App_Start
{
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Filters;
    using System.Web.Http.Results;

    public static class WebApiConfig
	{
	    public static string UrlPrefix { get { return "api"; } }
	    public static string UrlPrefixRelative { get { return "~/api"; } }
        public static void Register( HttpConfiguration config )
        {
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: WebApiConfig.UrlPrefix + "/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
            config.Filters.Add(new WebAPISecurityInSessionAuthenticationFilter());
        }
	}

    public class WebAPISecurityInSessionAuthenticationFilter : IAuthenticationFilter
    {
        public bool AllowMultiple => true;

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            //bypass if it is not for webapi route
            var isWebAPI =  HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiConfig.UrlPrefixRelative);
            if (!isWebAPI)
            {
                return Task.FromResult(0);
            }

            //security is based on the security class being already set inside of the session
            var session = HttpContext.Current.Session;
            if (session == null || session["Security"] == null)
            {
                context.ErrorResult = new UnauthorizedResult(
                    new AuthenticationHeaderValue[0],
                    context.Request);
            }
            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
