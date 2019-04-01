using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESRI.AGS.response
{
    public class MapInfo
    {
        public double currentVersion { get; set; }
        public string serviceDescription { get; set; }
        public string mapName { get; set; }
        public string description { get; set; }
        public string copyrightText { get; set; }
        public bool supportsDynamicLayers { get; set; }
        public List<SubLayer> layers { get; set; }
        public List<SubTable> tables { get; set; }
        public SpatialReference spatialReference { get; set; }
        public bool singleFusedMapCache { get; set; }
        public TileInfo tileInfo { get; set; }
        public geometries.Envelope initialExtent { get; set; }
        public geometries.Envelope fullExtent { get; set; }
        public int minScale { get; set; }
        public int maxScale { get; set; }
        public string units { get; set; }
        public string supportedImageFormatTypes { get; set; }
        public DocumentInfo documentInfo { get; set; }
        public string capabilities { get; set; }
        public string supportedQueryFormats { get; set; }
        public bool exportTilesAllowed { get; set; }
        public int maxRecordCount { get; set; }
        public int maxImageHeight { get; set; }
        public int maxImageWidth { get; set; }
        public string supportedExtensions { get; set; }

        public class DocumentInfo
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public string Comments { get; set; }
            public string Subject { get; set; }
            public string Category { get; set; }
            public string AntialiasingMode { get; set; }
            public string TextAntialiasingMode { get; set; }
            public string Keywords { get; set; }
        }
        public class TileInfo
        {
            public int rows { get; set; }
            public int cols { get; set; }
            public int dpi { get; set; }
            public string format { get; set; }
            public int compressionQuality { get; set; }
            public Origin origin { get; set; }
            public SpatialReference spatialReference { get; set; }
            public List<Lod> lods { get; set; }


        }
        public class Origin
        {
            public double x { get; set; }
            public double y { get; set; }
        }
        public class Lod
        {
            public int level { get; set; }
            public double resolution { get; set; }
            public double scale { get; set; }
        }
        public class SubLayer
        {
            public int id { get; set; }
            public string name { get; set; }
            public int parentLayerId { get; set; }
            public bool defaultVisibility { get; set; }
            public List<int> subLayerIds { get; set; }
            public int minScale { get; set; }
            public int maxScale { get; set; }
        }
        public class SubTable
        { }
    }
}