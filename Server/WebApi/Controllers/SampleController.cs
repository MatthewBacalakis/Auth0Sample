using System.Web.Http;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [RoutePrefix("api")]
    public class SampleController : ApiController
    {
        [HttpGet]
        [Route("items")]
        [ScopeAuthorize("read:items")]
        public string read()
        {
            return "You are authorized to read";
        }

        [HttpPut]
        [Route("items")]
        [ScopeAuthorize("write:items")]
        public string write()
        {
            return "You are authorized to write";
        }

        [HttpDelete]
        [Route("items")]
        [ScopeAuthorize("delete:items")]
        public string delete()
        {
            return "You are authorized to delete";
        }
    }
}
