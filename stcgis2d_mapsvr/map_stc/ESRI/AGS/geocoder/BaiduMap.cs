using System.Collections.Generic;

namespace ESRI.AGS.geocoder.BaiduMap
{
    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class SuggestItem
    {
        public string name { get; set; }
        public Location location { get; set; }
        public string uid { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string business { get; set; }
        public string cityid { get; set; }
    }

    public class SuggestResultObj
    {
        public int status { get; set; }
        public string message { get; set; }
        public List<SuggestItem> result { get; set; }
    }

    public class SearchItem
    {
        public string name { get; set; }
        public Location location { get; set; }
        public string address { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string area { get; set; }
        public string street_id { get; set; }
        public string telephone { get; set; }
        public int detail { get; set; }
        public string uid { get; set; }
    }

    public class SearchResultObj
    {
        public int status { get; set; }
        public string message { get; set; }
        public int total { get; set; }
        public List<SearchItem> results { get; set; }
    }

    public class DetailResultObj
    {
        public int status { get; set; }
        public string message { get; set; }
        public SearchItem result { get; set; }
    }
}