using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESRI.AGS.symbols
{
    public class SimpleFillSymbol : Symbol
    {
        public override string type { get { return esriSymbolType.esriSFS; } }
        public string style { get; set; }
        public int[] color { get; set; }
        public SimpleLineSymbol outline { get; set; }
    }
}