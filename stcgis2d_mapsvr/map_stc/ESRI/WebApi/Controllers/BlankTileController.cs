using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace ESRI.WebApi.Controllers
{
    [RoutePrefix("rest/services")]
    public class BlankTileController : ApiController
    {

        [Route("BlankTile/MapServer")]
        [HttpGet]
        public ESRI.AGS.response.MapInfo getBlankTileInfo()
        {
            string jsonFileContent = ConfigLoader.loadJsonFile("BlankTileServer");

            var mapInfoObj = JsonConvert.DeserializeObject<ESRI.AGS.response.MapInfo>(jsonFileContent);

            return mapInfoObj;
        }

        private static string blank_tile_string = "iVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAIAAADTED8xAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAKbSURBVHhe7dNBDQAwEASh+ve776uQIcECbzvIEoA0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgDQBSBOANAFIE4A0AUgTgLDdByvDK6h+YZ8tAAAAAElFTkSuQmCC";

        private static byte[] blanktile = Convert.FromBase64String(blank_tile_string);

        [Route("BlankTile/MapServer/tile/{level}/{row}/{col}")]
        [CacheOutput(ClientTimeSpan = 86400, ServerTimeSpan = 86400)]
        [HttpGet]
        public HttpResponseMessage getBlankTile(string level, string row, string col)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            
            response.Content = new StreamContent(new System.IO.MemoryStream(blanktile));
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");

            return response;
        }
    }
}
