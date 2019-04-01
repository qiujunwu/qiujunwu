using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace ESRI.AGS.geocoder
{
    public class SuggestItem
    {
        public string magicKey { get; set; }
        public string text { get; set; }
        public bool isCollection { get { return false; } }
    }

    public class SuggestResultObj
    {
        public List<SuggestItem> suggestions { get; set; }
    }

    public class CandidateItem
    {
        public string address { get; set; }
        public CandidateAttributes attributes { get; set; }
        public CandidateExtent extent { get; set; }
        public CandidateLocation location { get; set; }
        public int score { get; set; }
    }

    public class CandidateResultObj
    {
        public List<CandidateItem> candidates { get; set; }
        public CandidateSpatialReference spatialReference { get; set; }
    }
    public class CandidateAttributes
    {

    }
    public class CandidateExtent
    {
        public double xmax { get; set; }
        public double xmin { get; set; }
        public double ymax { get; set; }
        public double ymin { get; set; }
    }
    public class CandidateLocation
    {
        public double x { get; set; }
        public double y { get; set; }
    }

    public class CandidateSpatialReference
    {
        public int latestWkid { get; set; }
        public int wkid { get; set; }
    }
    
    public static class Convert
    {
        public static SuggestResultObj toEsri(BaiduMap.SuggestResultObj obj)
        {
            if (obj == null)
                return null;

            var ret = new SuggestResultObj();
            ret.suggestions = new List<SuggestItem>();

            if (obj.status == 0 && obj.result != null && obj.result.Count > 0)
            {
                foreach (var r in obj.result)
                {
                    if (!String.IsNullOrWhiteSpace(r.uid) && !String.IsNullOrWhiteSpace(r.name))
                    {
                        var item = new SuggestItem();
                        item.magicKey = r.uid;
                        item.text = r.name;

                        ret.suggestions.Add(item);
                    }
                }
            }

            return ret;
        }
    }
}