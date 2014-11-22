using System;

using Nancy;

namespace Blinnikov.LikeMeUp.Web.Modules {
    public class HomeModule : NancyModule {
        public HomeModule() {
            this.Get["/"] = this.HomeView();
        }

        private Func<dynamic, dynamic> HomeView() {
            return _ => {
                var appPath = this.Request.Url.BasePath ?? "/";
                var model = new {
                    AppPath = appPath.TrimEnd('/') + "/",
                    AppRoot = appPath.TrimStart('/')
                };
                return this.View["index", model];
            };
        }
    }
}