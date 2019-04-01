using Dapper;
using map_stc.STC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace map_stc.STC.HouseNumber
{
    /// <summary>
    /// 第九条 城镇门牌号编排， 原则上应依据道路、里巷的走向确定：
    /// （一）东西走向的，由东向西编排；
    /// （二）南北走向的，由北向南编排；
    /// （三）东北西南走向的，由东北向西南编排；
    /// （四）西北东南走向的，由西北向东南编排。
    /// 第十一条 道路、 里巷仅一侧有院落或房屋的，门牌号自起编处按顺序号编排；
    /// 两侧均有院落或房屋的，自起编处按单、双号编排，左侧为单号，右侧为双号。
    /// </summary>
    public class Locate
    {
        public static double TOLERANCE = 100;//容差范围

        public static LocateResult loc(double x, double y)
        {
            var segs = getSegmentFromTable(x, y, TOLERANCE);
            if (segs.Count == 0)
                return new LocateResult() { flag = false, message = "距离道路太远，无法计算！", data = null };

            var result = new LocateData();
            result.distance = 10000000;

            foreach (var seg in segs)
            {
                var p1 = new Point(seg.START_X, seg.START_Y);
                var p2 = new Point(seg.END_X, seg.END_Y);
                var q = new Point(x, y);

                Point closest;
                var distance = Algorithm.distanceToSegment(p1, p2, q, out closest);
                if (distance < result.distance)//距离取最小的
                {
                    result.distance = distance;
                    result.closest = closest;
                    result.road_name = seg.ROAD_NAME;

                    if (seg.ROAD_DIRECTION == 1)
                    {
                        result.road_len = seg.START_LEN + Algorithm.distance(p1, closest);
                        result.orientation = Algorithm.orientationIndex(p1, p2, q);
                    }
                    else
                    {
                        result.road_len = seg.END_LEN + Algorithm.distance(closest, p2);
                        result.orientation = Algorithm.orientationIndex(p2, p1, q);
                    }

                    result.suggest = Algorithm.suggestNumber(result.road_len, result.orientation == 1);
                }                
            }
            return new LocateResult() { flag = true, message = "计算成功！", data = result };
        }
        public class LocateResult
        {
            public bool flag { get; set; }
            public string message { get; set; }
            public LocateData data { get; set; }
        }
        public class LocateData
        {
            public double distance { get; set; }
            public Point closest { get; set; }
            [JsonIgnore]
            public int orientation { get; set; }
            public string direction { get { return orientation == 1 ? "道路左侧" : "道路右侧"; } }
            public double road_len { get; set; }
            public int suggest { get; set; }
            public string road_name { get; set; }
        }
        public class DbRoadSeg
        {
            public int ID { get; set; }
            public int ROAD_ID { get; set; }
            public string ROAD_NAME { get; set; }
            public int ROAD_DIRECTION { get; set; }
            public double START_X { get; set; }
            public double START_Y { get; set; }
            public double END_X { get; set; }
            public double END_Y { get; set; }

            public double START_LEN { get; set; }
            public double END_LEN { get; set; }
            public double LEN { get; set; }
            public double ANGLE { get; set; }
        }

        private static List<DbRoadSeg> getSegmentFromTable(double x, double y, double tolerance)
        {
            DynamicParameters ps = new DynamicParameters();
            string sql = "select a.*,b.name as road_name,b.direction as road_direction from (select * from tb_road_seg where min_x<@PI_MINX and max_x>@PI_MAXX and min_y<@PI_MINY and max_y>@PI_MAXY) a left join tb_road b on a.road_id=b.id";

            ps.Add("PI_MINX", x + tolerance, System.Data.DbType.Double);
            ps.Add("PI_MAXX", x - tolerance, System.Data.DbType.Double);
            ps.Add("PI_MINY", y + tolerance, System.Data.DbType.Double);
            ps.Add("PI_MAXY", y - tolerance, System.Data.DbType.Double);

            using (var conn = Database.DbService())
            {
                return conn.Query<DbRoadSeg>(sql, ps).ToList();
            }
        }
    }
}