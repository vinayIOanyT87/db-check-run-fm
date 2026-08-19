using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http;
using FMWebAPIBusinessLogic.Interfaces.Controllers;

namespace FMWebAPI.Filter
{
    public class FMSecurityTokenFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var skipAthorization = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0;
            if (skipAthorization)
            {
                return;
            }
            var token = "";
            IEnumerable<string> values;
            if (actionContext.Request.Headers.TryGetValues("userToken", out values))
            {
                token = values.FirstOrDefault();
            }
            //magic! 
            //https://stackoverflow.com/questions/33390047/new-instance-of-iauthorizationfilter-for-each-request
            var requestScope = actionContext.Request.GetDependencyScope();
            var _ISiteInteractions = requestScope.GetService(typeof(ISiteController)) as ISiteController;
            //var _ISiteInteractions = FMServiceLocator.GetInstance<ISiteController>();
            var passedSecurityCheck = _ISiteInteractions.CheckToken(token);

            if (!passedSecurityCheck)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            return;
        }
    }
}