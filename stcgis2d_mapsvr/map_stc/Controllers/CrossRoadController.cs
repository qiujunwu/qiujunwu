using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace map_stc.Controllers
{
    [RoutePrefix("api")]
    public class CrossRoadController : ApiController
    {
        [Route("crossroad/suggest")]
        [HttpGet]
        public List<STC.CrossRoad.Locate.DbRoadNameSuggest> suggest(string text)
        {
            return STC.CrossRoad.Locate.suggest(text);
        }

        [Route("crossroad/query")]
        [HttpGet]
        public List<STC.CrossRoad.Locate.DbCrossRoadResult> query(string text)
        {
            return STC.CrossRoad.Locate.query(text);
        }
    }
}
