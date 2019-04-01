using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESRI.AGS.renderers
{
    public static class esriRendererType
    {
        public static readonly string simple = "simple";
    }
    public class Renderer
    {
        public virtual string type { get { return ""; } }
    }
}