using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ESRI.AGS.geocoder;

namespace ESRI.WebApi.Controllers
{
    [RoutePrefix("rest/services")]
    public class GeocodeController : ApiController
    {
        [Route("{serviceName}/GeocodeServer/suggest")]
        [HttpGet]
        public SuggestResultObj suggest(string serviceName, [FromUri]GeocodeSuggestParams param)
        {
            if (serviceName.Equals("baidu"))
                return BaiduGeocoder.suggest(param.text, param.maxSuggestions);
            else if (serviceName.Equals("poi"))
                return map_stc.STC.Geocoder.DBGeocoder.suggest(param.text, param.maxSuggestions);
            else
                return new SuggestResultObj() { suggestions = new List<SuggestItem>() };
        }

        [Route("{serviceName}/GeocodeServer/findAddressCandidates")]
        [HttpGet]
        public CandidateResultObj findAddressCandidates(string serviceName, [FromUri]GeocodeFindAddressParams param)
        {
            var outSR = Newtonsoft.Json.JsonConvert.DeserializeObject<CandidateSpatialReference>(param.outSR);

            if (serviceName.Equals("baidu"))
            {
                return BaiduGeocoder.findAddressCandidates(param.SingleLine, param.magicKey, param.maxLocations, (outSR == null ? 4326 : outSR.wkid));
            }
            else if (serviceName.Equals("poi"))
            {
                return map_stc.STC.Geocoder.DBGeocoder.findAddressCandidates(param.SingleLine, param.magicKey, param.maxLocations, (outSR == null ? 4326 : outSR.wkid));
            }                
            else
            {
                return new CandidateResultObj()
                {
                    candidates = new List<CandidateItem>(),
                    spatialReference = new CandidateSpatialReference() { latestWkid = 4326, wkid = 4326 }
                };
            } 
        }
    }

    public class GeocodeSuggestParams
    {
        public string f { get; set; }
        public string text { get; set; }
        public int maxSuggestions { get; set; }
    }
    public class GeocodeFindAddressParams
    {
        public string f { get; set; }
        public string outSR { get; set; }
        public string SingleLine { get; set; }
        public string magicKey { get; set; }
        public int maxLocations { get; set; }
    }
}
