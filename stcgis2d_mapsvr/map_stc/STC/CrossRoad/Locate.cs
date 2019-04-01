using Dapper;
using map_stc.STC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace map_stc.STC.CrossRoad
{
    public static class Locate
    {
        public static List<DbRoadNameSuggest> suggest(string text)
        {
            DynamicParameters ps = new DynamicParameters();
            string sql = "select distinct(mc1) as name from tb_crossroad where mc1 like @PI_NAME";
            ps.Add("PI_NAME", "%" + text.Trim() + "%", System.Data.DbType.String);

            using (var conn = Database.DbService())
            {
                return conn.Query<DbRoadNameSuggest>(sql, ps).ToList();
            }
        }

        public static List<DbCrossRoadResult> query(string text)
        {
            DynamicParameters ps = new DynamicParameters();
            string sql = "select mc2 as name,x,y from tb_crossroad where mc1=@PI_NAME";
            ps.Add("PI_NAME", text.Trim(), System.Data.DbType.String);

            using (var conn = Database.DbService())
            {
                return conn.Query<DbCrossRoadResult>(sql, ps).ToList();
            }
        }

        public class DbCrossRoadResult
        {
            public string name { get; set; }
            public double x { get; set; }
            public double y { get; set; }
        }

        public class DbRoadNameSuggest
        {
            public string name { get; set; }
        }
    }
}