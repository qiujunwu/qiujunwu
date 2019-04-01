using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ESRI.WebApi.Controllers
{
    [RoutePrefix("rest")]
    public class RestInfoController : ApiController
    {
        [Route("info")]
        [HttpGet]
        public RestInfo getInfo()
        {
            return new RestInfo();
        }
    }
    public class RestInfo
    {
        public double currentVersion {
            get { return 10.2; }
        }
    }
}
