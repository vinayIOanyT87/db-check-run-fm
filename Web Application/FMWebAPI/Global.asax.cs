using FMCore.Interfaces;
using FMDepedencyManager;
using FMWebAPI.Filter;
using FMWebAPIBusinessLogic;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Description;
using Unity;
using Unity.AspNet.WebApi;
using Unity.Lifetime;
using FMCore;
using System.Web.Http.Cors;
using FMWebAPI.Services;

namespace FMWebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var thisAssembly = typeof(WebApiApplication).Assembly;

            var swaggerConfig = GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "FuelManagerWebAPIV3");
                    c.PrettyPrint();
                    c.ApiKey("userToken")
                        .Description("User Token Authentication")
                        .Name("userToken")
                        .In("header");
                });
#if DEBUG
            //http://localhost/FMWebAPI/swagger/ui/index
            swaggerConfig.EnableSwaggerUi(c =>
            {
                c.EnableApiKeySupport("userToken", "header");
            });
#endif

            FMServiceLocator.Container = new UnityContainer();
            FMServiceLocator.Container.RegisterFuelManagerWebAPIBusinessServices();
            FMServiceLocator.Container.RegisterFMCoreServices();
            FMServiceLocator.Container.RegisterType<IFMCustomLogger, FMWebAPISerilogEventlogLogger>(new ContainerControlledLifetimeManager()); //singleton
            //FMServiceLocator.Container.RegisterType<IHttpRequestFactory, HttpRequestFactory>(new HierarchicalLifetimeManager()); //singleton to the request
            //FMServiceLocator.Container.RegisterType<ISecurityCheckService, HttpSecurityCheckService>(); //singleton to the request
            //https://docs.microsoft.com/en-us/previous-versions/msp-n-p/ff660872(v=pandp.20)
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.AspNet.WebApi.UnityHierarchicalDependencyResolver(FMServiceLocator.Container);
            //GlobalConfiguration.Configuration.DependencyResolver = new Unity.AspNet.WebApi.UnityDependencyResolver(FMServiceLocator.Container);
            
            var config = GlobalConfiguration.Configuration;
#if DEBUG
            //var cors = new EnableCorsAttribute(@"http://localhost:4200", "*", "*");
            var cors = new EnableCorsAttribute(@"*", "*", "*");
            config.EnableCors(cors);
#endif

            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("text/html"));

            config.EnsureInitialized();
            config.Filters.Add(new FMSecurityTokenFilter());
        }
    }

    public class AddRequiredHeaderParameter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
                operation.parameters = new List<Parameter>();

            operation.parameters.Add(new Parameter
            {
                name = "userToken",
                @in = "header",
                type = "string",
                required = true
            });
        }
    }
}
