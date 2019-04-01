using System.Web.Http;

namespace ESRI.WebApi.Controllers
{
    [RoutePrefix("sharing/rest")]
    public class PortalsController : ApiController
    {
        [Route("portals/self")]
        [HttpGet]
        public Portal.Portals.PortalSelf getPortalSelf()
        {
            return new Portal.Portals.PortalSelf();
        }
    }
}
