using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ESRI.WebApi.Controllers
{
    [RoutePrefix("sharing/rest")]
    public class PortalContentController : ApiController
    {
        [Route("content/items/{id}")]
        [HttpGet]
        public Portal.Content.Item getItem(string id)
        {
            return new Portal.Content.Item();
        }

        [Route("content/items/{id}/data")]
        [HttpGet]
        public Portal.Content.ItemData getItemData(string id)
        {
            string itemDataContent = ConfigLoader.loadItemData(id);

            var itemDataObj = JsonConvert.DeserializeObject<Portal.Content.ItemData>(itemDataContent);

            return itemDataObj;
        }
    }
}
