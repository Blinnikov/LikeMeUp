using Nancy;
using Nancy.Bootstrapper;
using Nancy.Session;
using Nancy.TinyIoc;

namespace Blinnikov.LikeMeUp.Web.Modules {
    public class CustomBootstrapper : DefaultNancyBootstrapper {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines) {
            CookieBasedSessions.Enable(pipelines);
        }
    }
}