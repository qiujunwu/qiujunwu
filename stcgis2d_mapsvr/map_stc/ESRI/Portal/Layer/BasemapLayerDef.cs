using System.Collections.Generic;

namespace ESRI.Portal.Layer
{
    public class BasemapLayerDef : OperationalLayerDef
    {
        public string type { get; set; }
        public string templateUrl { get; set; }
        public List<string> subDomains { get; set; }
    }
}