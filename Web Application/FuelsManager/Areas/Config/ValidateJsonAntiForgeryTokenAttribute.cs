using System;
using System.Web.Helpers;
using System.Web.Mvc;

// check for the request verification token in the header instead of the body of the request.  To be used in JSON calls from the client
// see: http://johan.driessen.se/posts/Updated-Anti-XSRF-Validation-for-ASP.NET-MVC-4-RC


[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class,
                AllowMultiple = false, Inherited = true)]
public sealed class ValidateJsonAntiForgeryTokenAttribute
                            : FilterAttribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationContext filterContext)
    {
      if (Global.ComplianceFlag)
      {
         return;
      }
        if (filterContext == null)
        {
            throw new ArgumentNullException("filterContext");
        }

        var httpContext = filterContext.HttpContext;
        var cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
        AntiForgery.Validate(cookie != null ? cookie.Value : null,
                             httpContext.Request.Headers["__RequestVerificationToken"]);
    }
}