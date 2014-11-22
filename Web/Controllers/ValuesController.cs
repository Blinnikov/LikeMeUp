using System.Web.Http;

namespace Blinnikov.LikeMeUp.Web.Controllers {
    public class ValuesController : ApiController {
        public IHttpActionResult Get() {
            return this.Ok();
        }
    }
}
