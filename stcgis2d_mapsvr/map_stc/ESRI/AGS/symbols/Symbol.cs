using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESRI.AGS.symbols
{
    public static class esriSymbolType
    {
        public static readonly string esriSMS = "esriSMS";
        public static readonly string esriSLS = "esriSLS";
        public static readonly string esriSFS = "esriSFS";
        public static readonly string esriPMS = "esriPMS";
        public static readonly string esriPFS = "esriPFS";
        public static readonly string esriTS = "esriTS";
    }
    public static class esriSFSStyle
    {
        public static readonly string BackwardDiagonal = "esriSFSBackwardDiagonal";
        public static readonly string Cross = "esriSFSCross";
        public static readonly string DiagonalCross = "esriSFSDiagonalCross";
        public static readonly string ForwardDiagonal = "esriSFSForwardDiagonal";
        public static readonly string Horizontal = "esriSFSHorizontal";
        public static readonly string Null = "esriSFSNull";
        public static readonly string Solid = "esriSFSSolid";
        public static readonly string Vertical = "esriSFSVertical";
    }
    public static class esriSLSStyle
    {
        public static readonly string Dash = "esriSLSDash";
        public static readonly string DashDot = "esriSLSDashDot";
        public static readonly string DashDotDot = "esriSLSDashDotDot";
        public static readonly string Dot = "esriSLSDot";
        public static readonly string Null = "esriSLSNull";
        public static readonly string Solid = "esriSLSSolid";
    }
    public class Symbol
    {
        public virtual string type { get { return ""; } }

        public static SimpleFillSymbol getRandomSFS()
        {
            var sls = new SimpleLineSymbol()
            {
                style = esriSLSStyle.Solid,
                color = new int[4] { 160, 63, 60, 255 },
                width = 1.5
            };
            return new SimpleFillSymbol()
            {
                style = esriSFSStyle.Solid,
                color = new int[4] { 192, 80, 77, 128 },
                outline = sls
            };
        }

        public static SimpleMarkerSymbol getRandomSMS()
        {
            return new SimpleMarkerSymbol()
            {
                angle = 0,
                color = getRandomColor(),
                size = 4,
                style = esriSMSStyle.esriSMSCircle,
                xoffset = 0,
                yoffset = 0,
                outline = new SimpleMarkerSymbolOutline() { color = getOutlineColor(), width = 1 }
            };
        }
        public static int[] getRandomColor()
        {
            return new int[4] { 0, 102, 145, 255 };
        }
        public static int[] getOutlineColor()
        {
            return new int[4] { 0, 0, 0, 255 };
        }
    }
}