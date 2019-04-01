using System.Collections.Generic;

namespace ESRI.Portal.Layer
{
    public class OperationalLayerDef
    {
        public string id { get; set; }
        public string layerType { get; set; }
        public string url { get; set; }
        public bool visibility { get; set; }
        public double opacity { get; set; }
        public string title { get; set; }

        public PopupInfoDef popupInfo { get; set; }

        public List<SubLayerPopupInfoDef> layers { get; set; }
    }

    public class SubLayerPopupInfoDef
    {
        public int id { get; set; }
        public PopupInfoDef popupInfo { get; set; }
    }

    public class PopupInfoDef
    {
        public string title { get; set; }
        public List<PopupField> fieldInfos { get; set; }
        public string description { get; set; }
    }

    public class PopupField
    {
        public string fieldName { get; set; }
    }
}