using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESRI.AGS.geocoder
{
    /// <summary>
    /// 依据关系数据库中的POI表提供地址查找定位服务
    /// </summary>
    public class PoiGeocoder
    {
        public static SuggestResultObj suggest(string text, int maxSuggestions)
        {
            return null;
            //DynamicParameters ps = new DynamicParameters();
            //string sql = "select id,name from tb_poi where instr(name,:PI_KEYWORD)>0 and ROWNUM <=" + maxSuggestions;
            //ps.Add("PI_KEYWORD", text, System.Data.DbType.String);

            //using (var conn = Database.DbService())
            //{
            //    var result = conn.Query<POCO_SUGGEST>(sql, ps).ToList();
            //    return new RESULT_SUGGEST() { suggestions = result };
            //}
        }
    }
}