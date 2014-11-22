using System.Web.Http;

using Blinnikov.LikeMeUp.Web.Configuration;

using Microsoft.Owin;

using Nancy;
using Nancy.Owin;

using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace Blinnikov.LikeMeUp.Web.Configuration {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            app
                .UseNancy(opt => opt.PassThroughWhenStatusCodesAre(HttpStatusCode.NotFound))
                .UseWebApi(config);
        }
    }
}