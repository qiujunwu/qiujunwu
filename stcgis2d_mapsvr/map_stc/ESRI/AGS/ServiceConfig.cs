using ESRI.AGS.renderers;
using ESRI.AGS.response;
using ESRI.AGS.symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace ESRI.AGS
{
    public static class ServiceConfig
    {
        private static Dictionary<string, Dictionary<int, LayerInfo>> _layers = new Dictionary<string, Dictionary<int, LayerInfo>>();
        private static Dictionary<string, Dictionary<int, TableInfo>> _tables = new Dictionary<string, Dictionary<int, TableInfo>>();
        public static void LoadConfig(string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlNodeList svrNodeList = doc.SelectNodes("/services/service");
            foreach (XmlNode svrNode in svrNodeList)
            {
                Dictionary<int, LayerInfo> lyrSvr = new Dictionary<int, LayerInfo>();
                foreach (XmlNode lyrNode in svrNode.SelectNodes("layer"))
                {
                    LayerInfo lyrInfo = parseLayerInfo(lyrNode);
                    lyrSvr.Add(lyrInfo.id, lyrInfo);
                }
                _layers.Add(svrNode.Attributes["name"].Value, lyrSvr);

                Dictionary<int, TableInfo> tbSvr = new Dictionary<int, TableInfo>();
                foreach (XmlNode tbNode in svrNode.SelectNodes("table"))
                {
                    TableInfo tbInfo = parseTableInfo(tbNode);
                    tbSvr.Add(tbInfo.id, tbInfo);
                }
                _tables.Add(svrNode.Attributes["name"].Value, tbSvr);
            }
        }

        public static TableInfo getServiceInfo(string sname, int id)
        {
            TableInfo result = getLayerInfo(sname, id);
            return result == null ? getTableInfo(sname, id) : result;
        }

        private static LayerInfo getLayerInfo(string sname, int id)
        {
            Dictionary<int, LayerInfo> svr;
            if (_layers.TryGetValue(sname, out svr))
            {
                LayerInfo result;
                if (svr.TryGetValue(id, out result))
                    return result;
                else
                    return null;
            }
            else
                return null;
        }
        private static TableInfo getTableInfo(string sname, int id)
        {
            Dictionary<int, TableInfo> svr;
            if (_tables.TryGetValue(sname, out svr))
            {
                TableInfo result;
                if (svr.TryGetValue(id, out result))
                    return result;
                else
                    return null;
            }
            else
                return null;
        }

        private static TableInfo parseTableInfo(XmlNode tbNode)
        {
            if (tbNode == null || !tbNode.HasChildNodes)
                return null;
            try
            {
                TableInfo result = new TableInfo();

                result.id = int.Parse(tbNode.SelectSingleNode("id").InnerText);
                result.name = tbNode.SelectSingleNode("name").InnerText;
                result.type = tbNode.SelectSingleNode("type").InnerText;
                result.description = tbNode.SelectSingleNode("description").InnerText;
                result.definitionExpression = tbNode.SelectSingleNode("definitionExpression").InnerText;

                result.displayField = tbNode.SelectSingleNode("displayField").InnerText;
                result.s_idField = tbNode.SelectSingleNode("s_idField").InnerText;

                List<Field> fields = new List<Field>();
                foreach (XmlNode fNode in tbNode.SelectNodes("fields/field"))
                    fields.Add(parseField(fNode));

                result.fields = fields;

                result.hasAttachments = "true".Equals(tbNode.SelectSingleNode("hasAttachments").InnerText);
                result.htmlPopupType = tbNode.SelectSingleNode("htmlPopupType").InnerText;

                result.relationships = new List<Relationship>();
                result.typeIdField = null;


                result.s_tableName = tbNode.SelectSingleNode("s_tableName").InnerText;
                result.s_connectionString = tbNode.SelectSingleNode("s_connectionString").InnerText;

                return result;
            }
            catch (Exception e)
            {
                //TODO: 解析XML的操作中，错误信息没有进行记录和返回，而是直接略过
                SysLog.Error(e.Message);
                return null;
            }
        }

        private static LayerInfo parseLayerInfo(XmlNode lyrNode)
        {
            if (lyrNode == null || !lyrNode.HasChildNodes)
                return null;
            try
            {
                LayerInfo result = new LayerInfo();

                result.id = int.Parse(lyrNode.SelectSingleNode("id").InnerText);
                result.name = lyrNode.SelectSingleNode("name").InnerText;
                result.type = lyrNode.SelectSingleNode("type").InnerText;
                result.description = lyrNode.SelectSingleNode("description").InnerText;
                result.definitionExpression = lyrNode.SelectSingleNode("definitionExpression").InnerText;
                result.geometryType = lyrNode.SelectSingleNode("geometryType").InnerText;
                result.copyrightText = lyrNode.SelectSingleNode("copyrightText").InnerText;

                result.displayField = lyrNode.SelectSingleNode("displayField").InnerText;
                result.s_idField = lyrNode.SelectSingleNode("s_idField").InnerText;
                result.s_xField = lyrNode.SelectSingleNode("s_xField").InnerText;
                result.s_yField = lyrNode.SelectSingleNode("s_yField").InnerText;

                List<Field> fields = new List<Field>();
                foreach (XmlNode fNode in lyrNode.SelectNodes("fields/field"))
                    fields.Add(parseField(fNode));

                result.fields = fields;

                result.minScale = double.Parse(lyrNode.SelectSingleNode("minScale").InnerText);
                result.maxScale = double.Parse(lyrNode.SelectSingleNode("maxScale").InnerText);
                result.defaultVisibility = "true".Equals(lyrNode.SelectSingleNode("defaultVisibility").InnerText);
                result.hasAttachments = "true".Equals(lyrNode.SelectSingleNode("hasAttachments").InnerText);
                result.htmlPopupType = lyrNode.SelectSingleNode("htmlPopupType").InnerText;

                //TODO: drawInfo属性使用系统自动生成，而没有读取xml，需改正。parentLayer、subLayers、relationships、typeIdField这四个属性也一样
                DrawingInfo drawInfo = new DrawingInfo();
                drawInfo.labelingInfo = null;
                drawInfo.renderer = new SimpleRenderer() { description = "", label = "", symbol = Symbol.getRandomSFS() };
                drawInfo.transparency = 0;

                result.drawingInfo = drawInfo;

                result.parentLayer = null;
                result.subLayers = new List<LayerIdName>();
                result.relationships = new List<Relationship>();
                result.typeIdField = null;

                result.s_tableName = lyrNode.SelectSingleNode("s_tableName").InnerText;
                result.s_connectionString = lyrNode.SelectSingleNode("s_connectionString").InnerText;

                int wkid;
                if (int.TryParse(lyrNode.SelectSingleNode("s_spatialReference").InnerText, out wkid) && (wkid == 4326 || wkid == 32650))//目前只能支持这两种空间参考
                    result.s_spatialReference = new SpatialReference() { wkid = wkid };

                return result;
            }
            catch (Exception e)
            {
                //TODO: 解析XML的操作中，错误信息没有进行记录和返回，而是直接略过
                SysLog.Error(e.Message);
                return null;
            }
        }
        private static Field parseField(XmlNode fNode)
        {
            if (fNode == null)
                return null;

            try
            {
                XmlAttribute name = fNode.Attributes["name"];
                XmlAttribute type = fNode.Attributes["type"];
                XmlAttribute alias = fNode.Attributes["alias"];
                XmlAttribute length = fNode.Attributes["length"];
                if (length == null)
                {
                    if (alias == null)
                        return new Field() { name = name.Value, type = type.Value, alias = name.Value };
                    else
                        return new Field() { name = name.Value, type = type.Value, alias = alias.Value };
                }
                else
                {
                    int len = int.Parse(length.Value);
                    if (alias == null)
                        return new Field2() { name = name.Value, type = type.Value, alias = name.Value, length = len };
                    else
                        return new Field2() { name = name.Value, type = type.Value, alias = alias.Value, length = len };
                }
            }
            catch (Exception e)
            {
                //TODO: 解析XML的操作中，错误信息没有进行记录和返回，而是直接略过
                SysLog.Error(e.Message);
                return null;
            }
        }
    }
}