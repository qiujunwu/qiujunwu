using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESRI.AGS
{
    public static class esriFieldType
    {
        public static readonly string esriFieldTypeSmallInteger = "esriFieldTypeSmallInteger";
        public static readonly string esriFieldTypeInteger = "esriFieldTypeInteger";
        public static readonly string esriFieldTypeSingle = "esriFieldTypeSingle";
        public static readonly string esriFieldTypeDouble = "esriFieldTypeDouble";
        public static readonly string esriFieldTypeString = "esriFieldTypeString";
        public static readonly string esriFieldTypeDate = "esriFieldTypeDate";
        public static readonly string esriFieldTypeOID = "esriFieldTypeOID";
        public static readonly string esriFieldTypeGeometry = "esriFieldTypeGeometry";
        public static readonly string esriFieldTypeBlob = "esriFieldTypeBlob";
        public static readonly string esriFieldTypeRaster = "esriFieldTypeRaster";
        public static readonly string esriFieldTypeGUID = "esriFieldTypeGUID";
        public static readonly string esriFieldTypeGlobalID = "esriFieldTypeGlobalID";
        public static readonly string esriFieldTypeXML = "esriFieldTypeXML";
    }
    public static class esriHtmlPopupType
    {
        public static readonly string esriServerHTMLPopupTypeNone = "esriServerHTMLPopupTypeNone";
        public static readonly string esriServerHTMLPopupTypeAsURL = "esriServerHTMLPopupTypeAsURL";
        public static readonly string esriServerHTMLPopupTypeAsHTMLText = "esriServerHTMLPopupTypeAsHTMLText";
    }
    public class SpatialReference
    {
        public int wkid { get; set; }
    }
    public class LayerIdName
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class Field
    {
        public string name { get; set; }
        public string type { get; set; }
        public string alias { get; set; }
    }
    public class Field2 : Field
    {
        public int length { get; set; }
    }
    public class Relationship
    {
        public string id { get; set; }
        public string name { get; set; }
        public int relatedTableId { get; set; }
    }

    public class DrawingInfo
    {
        public renderers.Renderer renderer { get; set; }
        public int transparency { get; set; }
        public object labelingInfo { get; set; }//TODO: 实现labelingInfo，现在临时使用object代替
    }

    public class Feature
    {
        public geometries.Geometry geometry { get; set; }
        public Dictionary<string, object> attributes { get; set; }
    }
}