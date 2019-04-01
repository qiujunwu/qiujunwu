using Dapper;
using ESRI.AGS.geocoder;
using map_stc.STC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace map_stc.STC.Geocoder
{
    public class DBGeocoder
    {
        public static SuggestResultObj suggest(string text, int maxSuggestions)
        {
            DynamicParameters ps = new DynamicParameters();
            string sql = "select top " + maxSuggestions + " id,name from tb_poi where name like @PI_NAME";
            ps.Add("PI_NAME", "%" + text.Trim() + "%", System.Data.DbType.String);

            using (var conn = Database.DbService())
            {
                var result = conn.Query<POCO_SUGGEST>(sql, ps).ToList();
                var ret = new SuggestResultObj();
                ret.suggestions = new List<SuggestItem>();

                for (var i = 0; i < result.Count; i++)
                {
                    var item = new SuggestItem();
                    item.magicKey = result[i].ID.ToString();
                    item.text = result[i].NAME;

                    ret.suggestions.Add(item);
                }
                return ret;
            }
        }

        public static CandidateResultObj findAddressCandidates(string SingleLine, string magicKey, int maxLocations, int outSR)
        {
            DynamicParameters ps = new DynamicParameters();
            string sql = "";

            int id = -1;
            if (Int32.TryParse(magicKey, out id) && id > 0)
            {
                sql = "select * from tb_poi where id=@PI_ID";
                ps.Add("PI_ID", id, System.Data.DbType.Int32);
            }
            else
            {
                sql = "select top " + maxLocations + " * from tb_poi where name like @PI_NAME";
                ps.Add("PI_NAME", "%" + SingleLine.Trim() + "%", System.Data.DbType.String);
            }

            using (var conn = Database.DbService())
            {
                var pois = conn.Query<POCO_POI>(sql, ps).ToList();
                var ret = new CandidateResultObj();
                ret.spatialReference = new CandidateSpatialReference() { latestWkid = outSR, wkid = outSR };
                ret.candidates = new List<CandidateItem>();

                for (var i = 0; i < pois.Count; i++)
                {
                    var item = new CandidateItem();
                    item.address = pois[i].NAME;
                    item.location = new CandidateLocation() { x = pois[i].X, y = pois[i].Y };
                    item.attributes = new CandidateAttributes();
                    item.extent = new CandidateExtent
                    {
                        xmax = item.location.x + 1000,
                        xmin = item.location.x - 1000,
                        ymax = item.location.y + 1000,
                        ymin = item.location.y - 1000
                    };
                    item.score = 100;

                    ret.candidates.Add(item);
                }

                return ret;
            }
        }

        class POCO_SUGGEST
        {
            public int ID { get; set; }
            public string NAME { get; set; }
        }

        class POCO_POI
        {
            public int ID { get; set; }
            public string NAME { get; set; }
            public string ADDRESS { get; set; }
            public string TELEPHONE { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
        }
    }
}