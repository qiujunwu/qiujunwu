using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESRI.AGS.response
{
    public class TableQuery
    {
        public string displayFieldName { get; set; }
        public Dictionary<string, string> fieldAliases
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                foreach (Field f in fields)
                    result.Add(f.name, f.alias);

                return result;
            }
        }
        public List<Field> fields { get; set; }
        public List<Feature> features { get; set; }
    }
    public class LayerQuery : TableQuery
    {
        public string geometryType { get; set; }
        public SpatialReference spatialReference { get; set; }
    }
    public class LayerQueryIdsOnly
    {
        public string objectIdFieldName { get; set; }
        public List<int> objectIds { get; set; }
    }
}