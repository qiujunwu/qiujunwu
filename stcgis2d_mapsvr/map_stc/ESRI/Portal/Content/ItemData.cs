using ESRI.Portal.Layer;
using System.Collections.Generic;

namespace ESRI.Portal.Content
{
    public class SpatialReference
    {
        public int wkid { get; set; }
    }

    public class ItemData
    {
        public List<OperationalLayerDef> operationalLayers { get; set; }
        public BasemapDef baseMap { get; set; }
        //public SpatialReference spatialReference { get; set; }
    }

}