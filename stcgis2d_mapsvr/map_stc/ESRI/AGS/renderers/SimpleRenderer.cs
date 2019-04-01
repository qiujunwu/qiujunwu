using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESRI.AGS.renderers
{
    public class SimpleRenderer : Renderer
    {
        public override string type
        {
            get { return esriRendererType.simple; }
        }
        public symbols.Symbol symbol { get; set; }
        public string label { get; set; }
        public string description { get; set; }
    }
}