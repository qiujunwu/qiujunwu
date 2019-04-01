using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ESRI.WebApi.Controllers
{
    [RoutePrefix("rest/services")]
    public class LayerController : ApiController
    {
        [Route("{serviceName}/MapServer/{layerOrTableId}")]
        [HttpGet]
        public ESRI.AGS.response.TableInfo getLayerInfo(string serviceName, int layerOrTableId)
        {
            return ESRI.AGS.ServiceConfig.getServiceInfo(serviceName, layerOrTableId);
        }

        [Route("{serviceName}/MapServer/{layerOrTableId}/query")]
        [HttpGet]
        public object getLayerQuery(string serviceName, int layerOrTableId, [FromUri]LayerQueryParams param)
        {
            ESRI.AGS.QueryParams queryParams = ESRI.AGS.QueryParams.createQueryParams(param.text, param.geometry, param.geometryType, param.inSR, param.spatialRel, param.relationParam, param.where, param.objectIds, param.time);
            ESRI.AGS.ResultOptions resultOptions = ESRI.AGS.ResultOptions.createResultOptions(param.returnIdsOnly, param.returnGeometry, param.maxAllowableOffset, param.outSR, param.outFields, param.f);

            return ESRI.AGS.LayerServicePerformer.query(serviceName, layerOrTableId, queryParams, resultOptions);
        }
    }

    public class LayerQueryParams
    {
        public string f { get; set; }
        public string text { get; set; }
        public string geometry { get; set; }
        public string geometryType { get; set; }
        public string inSR { get; set; }
        public string spatialRel { get; set; }
        public string relationParam { get; set; }
        public string where { get; set; }
        public string objectIds { get; set; }
        public string time { get; set; }
        public string outFields { get; set; }
        public string returnGeometry { get; set; }
        public string maxAllowableOffset { get; set; }
        public string outSR { get; set; }
        public string returnIdsOnly { get; set; }
        public string callback { get; set; }
    }
}
