using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESRI.AGS.symbols
{
    public static class esriSMSStyle
    {
        public static readonly string esriSMSCircle = "esriSMSCircle";
        public static readonly string esriSMSCross = "esriSMSCross";
        public static readonly string esriSMSDiamond = "esriSMSDiamond";
        public static readonly string esriSMSSquare = "esriSMSSquare";
        public static readonly string esriSMSX = "esriSMSX";
    }
    public class SimpleMarkerSymbolOutline
    {
        public int[] color { get; set; }
        public double width { get; set; }
    }
    public class SimpleMarkerSymbol : Symbol
    {
        public override string type
        {
            get { return esriSymbolType.esriSMS; }
        }
        public string style { get; set; }
        public int[] color { get; set; }
        public double size { get; set; }
        public double angle { get; set; }
        public double xoffset { get; set; }
        public double yoffset { get; set; }
        public SimpleMarkerSymbolOutline outline { get; set; }
    }
}