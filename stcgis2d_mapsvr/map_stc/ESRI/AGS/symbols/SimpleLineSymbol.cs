using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESRI.AGS.symbols
{
    public class SimpleLineSymbol : Symbol
    {
        public override string type { get { return esriSymbolType.esriSLS; } }
        public string style { get; set; }
        public int[] color { get; set; }
        public double width { get; set; }
    }
}