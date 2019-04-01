using System.Collections.Generic;

namespace ESRI.Portal.Layer
{
    public class BasemapDef
    {
        public List<BasemapLayerDef> baseMapLayers { get; set; }
        public string title { get; set; }
    }
}