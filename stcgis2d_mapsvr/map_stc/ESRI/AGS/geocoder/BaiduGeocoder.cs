using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace ESRI.AGS.geocoder
{
    public class BaiduGeocoder
    {
        private static string suggestion_url = "http://api.map.baidu.com/place/v2/suggestion";
        private static string search_url = "http://api.map.baidu.com/place/v2/search";
        private static string detail_url = "http://api.map.baidu.com/place/v2/detail";

        private static string access_key = "knwlDs3uvQPmKFEZQGG66IYOkLQt6DvH";//刘良杰的百度应用key

        private static double deltaByMeter = 1000;//获取点位范围时，容差值
        private static double deltaByDegree = 0.005;

        private static BaiduMap.SuggestResultObj _suggest(string text)
        {
            string url = suggestion_url;
            url += "?query=" + text;
            url += "&region=332";//区域为天津332
            url += "&city_limit=true";//仅返回region中指定城市检索结果
            url += "&ret_coordtype=gcj02ll";//返回国测局经纬度坐标
            url += "&output=json";
            url += "&ak=" + access_key;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 20000;

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<BaiduMap.SuggestResultObj>(responseContent);
        }

        private static BaiduMap.SearchResultObj _search(string text, int maxLocations)
        {
            string url = search_url;
            url += "?query=" + text;
            url += "&region=332";//区域为天津332
            url += "&city_limit=true";//仅返回region中指定城市检索结果
            url += "&ret_coordtype=gcj02ll";//返回国测局经纬度坐标
            url += "&page_size=" + maxLocations;//单次召回POI数量，默认为10条记录，最大返回20条
            url += "&output=json";
            url += "&ak=" + access_key;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 20000;

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<BaiduMap.SearchResultObj>(responseContent);
        }

        private static BaiduMap.DetailResultObj _detail(string magicKey)
        {
            string url = detail_url;
            url += "?uid=" + magicKey;
            url += "&ret_coordtype=gcj02ll";//返回国测局经纬度坐标
            url += "&output=json";
            url += "&ak=" + access_key;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 20000;

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<BaiduMap.DetailResultObj>(responseContent);
        }

        public static SuggestResultObj suggest(string text, int maxSuggestions)
        {
            var ret = Convert.toEsri(_suggest(text));

            if (ret != null && ret.suggestions.Count > maxSuggestions && maxSuggestions > 0)
                ret.suggestions.RemoveRange(maxSuggestions, ret.suggestions.Count - maxSuggestions);

            return ret;
        }

        public static CandidateResultObj findAddressCandidates(string SingleLine, string magicKey, int maxLocations, int outSR)
        {
            var ret = new CandidateResultObj();
            //此处将百度得到的结果转换后，看作是WGS84成果，因而latestWkid=4326
            ret.spatialReference = new CandidateSpatialReference() { latestWkid = 4326, wkid = outSR };
            ret.candidates = new List<CandidateItem>();

            if (String.IsNullOrWhiteSpace(magicKey))
            {
                var result = _search(SingleLine, maxLocations);
                if (result.status == 0 && result.results != null && result.results.Count > 0)
                {
                    foreach (var r in result.results)
                    {
                        var item = new CandidateItem();
                        item.address = r.name;
                        item.location = new CandidateLocation() { x = r.location.lng, y = r.location.lat };

                        ret.candidates.Add(item);
                    }
                }
            }
            else
            {
                var result = _detail(magicKey);
                if (result.status == 0 && result.result != null)
                {
                    var item = new CandidateItem();
                    item.address = result.result.name;
                    item.location = new CandidateLocation()
                    {
                        x = result.result.location.lng,
                        y = result.result.location.lat
                    };

                    ret.candidates.Add(item);
                }
            }

            foreach (var r in ret.candidates)
            {
                r.attributes = new CandidateAttributes();
                //将国测局坐标转换为wgs84坐标
                var wgs84Pnt = CoordinateTransform.WebMapTransform.gcj02towgs84(r.location.x, r.location.y);
                if (outSR == 102100)
                {
                    r.location.x = CoordinateTransform.WebMercatorTransform.longitudeToX(wgs84Pnt[0]);
                    r.location.y = CoordinateTransform.WebMercatorTransform.latitudeToY(wgs84Pnt[1]);

                    r.extent = new CandidateExtent
                    {
                        xmax = r.location.x + deltaByMeter,
                        xmin = r.location.x - deltaByMeter,
                        ymax = r.location.y + deltaByMeter,
                        ymin = r.location.y - deltaByMeter
                    };
                }
                else if (outSR == 4326 || outSR == 4490)//将WGS84和CGCS200约等于考虑，二者坐标几乎一致
                {
                    r.location.x = wgs84Pnt[0];
                    r.location.y = wgs84Pnt[1];

                    r.extent = new CandidateExtent
                    {
                        xmax = r.location.x + deltaByDegree,
                        xmin = r.location.x - deltaByDegree,
                        ymax = r.location.y + deltaByDegree,
                        ymin = r.location.y - deltaByDegree
                    };
                }
                else if (outSR == 32650)//在系统开发时习惯用此代替（天津90独立坐标）
                {

                }
                else
                {
                    r.location.x = r.location.y = 0;
                    r.extent = new CandidateExtent();
                }

                r.score = 100;
            }

            return ret;
        }
    }
}