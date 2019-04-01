using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ESRI.AGS.response
{
    //TODO: timeInfo段的描述被略过
    public class TableInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string definitionExpression { get; set; }

        public bool hasAttachments { get; set; }
        public string htmlPopupType { get; set; }
        public string displayField { get; set; }
        public string typeIdField { get; set; }

        public List<Field> fields { get; set; }

        public List<Relationship> relationships { get; set; }

        [JsonIgnore]
        public string s_idField { get; set; }
        [JsonIgnore]
        public string s_tableName { get; set; }
        [JsonIgnore]
        public string s_connectionString { get; set; }

        public virtual List<Field> calculateOutFields(string outFields)
        {
            if (String.IsNullOrEmpty(outFields))
            {
                return fields.FindAll(f => (f.name == displayField));
            }
            else if ("*".Equals(outFields.Trim()))
            {
                return fields;
            }
            else
            {
                List<Field> result = new List<Field>();
                foreach (string fname in outFields.Split(','))
                {
                    Field field = fields.Find(f => f.name == fname.Trim());
                    if (field != null)
                        result.Add(field);
                }
                return result;
            }
        }
    }

    public class LayerInfo : TableInfo
    {
        public string geometryType { get; set; }
        public string copyrightText { get; set; }

        public LayerIdName parentLayer { get; set; }
        public List<LayerIdName> subLayers { get; set; }

        public double minScale { get; set; }
        public double maxScale { get; set; }
        public bool defaultVisibility { get; set; }
        public string capabilities {
            get { return "Map,Query,Data"; }
        }

        public geometries.Extent extent { get; set; }

        public DrawingInfo drawingInfo { get; set; }

        [JsonIgnore]
        public string s_xField { get; set; }
        [JsonIgnore]
        public string s_yField { get; set; }
        [JsonIgnore]
        public SpatialReference s_spatialReference { get; set; }

        public override List<Field> calculateOutFields(string outFields)
        {
            if (String.IsNullOrEmpty(outFields))
            {
                return fields.FindAll(f => (f.name == s_idField || f.name == displayField));
            }
            else if ("*".Equals(outFields.Trim()))
            {
                return fields;
            }
            else
            {
                var outFieldsNames = outFields.Split(',').Select(fname=>fname.Trim()).ToList();
                return fields.FindAll(f => (outFieldsNames.Contains(f.name)));
            }
        }

        public string generateQueryFields(string outFields)
        {
            var outFieldsList = calculateOutFields(outFields);
            List<string> geoFields = null;
            if (geometries.esriGeometryType.esriGeometryPoint == geometryType)
                geoFields = new List<string>() { "X", "Y" };
            else
                geoFields = new List<string>() { "MINX", "MINY", "MAXX", "MAXY", "LBX", "LBY", "GEOMETRY" };

            var otherFields = outFieldsList.FindAll(f => (!geoFields.Contains(f.name))).Select(f => f.name);

            geoFields.AddRange(otherFields);

            return String.Join(",", geoFields);
        }
    }
}