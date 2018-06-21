using Owin;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApi
{
    public class WebApiConfig
    {
        public static void Configure(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional});

            app.UseWebApi(config);

            var corsOrigin = ConfigurationManager.AppSettings["CorsOrigin"];

            if (!string.IsNullOrEmpty(corsOrigin))
            {
                var corsSettings = new EnableCorsAttribute(corsOrigin, "*", "*");
                config.EnableCors(corsSettings);
            }
        }
    }
}