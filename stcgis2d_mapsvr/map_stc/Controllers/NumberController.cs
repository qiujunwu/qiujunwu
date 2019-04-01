using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace map_stc.Controllers
{
    [RoutePrefix("api")]
    public class NumberController : ApiController
    {
        [Route("number/locate")]
        [HttpGet]
        public STC.HouseNumber.Locate.LocateResult locate(double x,double y)
        {
            //double x = 146348.728, y = 299468.267;
            return STC.HouseNumber.Locate.loc(x, y);
        }
    }
}
