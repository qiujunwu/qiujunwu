using ESRI.AGS.geometries;
using ESRI.AGS.response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using Newtonsoft.Json;
using ESRI.AGS.DatabaseSupport;

namespace ESRI.AGS
{
    public class ServiceErrorResult
    {
        public ServiceError error { get; set; }
    }
    public class ServiceError
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<string> details { get; set; }
    }

    public class QueryParams
    {
        public string text { get; set; }
        public Geometry geometry { get; set; }
        public string geometryType { get; set; }
        public SpatialReference inSR { get; set; }
        public string spatialRel { get; set; }
        public string relationParam { get; set; }
        public string where { get; set; }
        public string objectIds { get; set; }
        public string time { get; set; }

        public static QueryParams createQueryParams(string txt, string geo, string geoType, string iSR, string spRel, string relParam, string wh, string objIds, string tm)
        {
            QueryParams result = new QueryParams();
            result.text = txt;
            result.geometryType = geoType;
            result.geometry = Geometry.fromJson(geoType, geo);

            int wkid;
            if (int.TryParse(iSR, out wkid))
                result.inSR = new SpatialReference() { wkid = wkid };

            result.spatialRel = spRel;
            result.relationParam = relParam;
            result.where = wh;
            result.objectIds = objIds;
            result.time = tm;

            return result;
        }

    }
    public class ResultOptions
    {
        public bool returnIdsOnly { get; set; }
        public bool returnGeometry { get; set; }
        public double maxAllowableOffset { get; set; }
        public SpatialReference outSR { get; set; }
        public string outFields { get; set; }
        public string f { get; set; }

        public static ResultOptions createResultOptions(string retIdsOnly, string retGeometry, string maxAllowOffset, string oSR, string oFields, string format)
        {
            ResultOptions result = new ResultOptions();

            result.returnIdsOnly = bool.TrueString.Equals(retIdsOnly, StringComparison.CurrentCultureIgnoreCase);
            result.returnGeometry = !bool.FalseString.Equals(retGeometry, StringComparison.CurrentCultureIgnoreCase);

            double offset = 0;
            double.TryParse(maxAllowOffset, out offset);
            result.maxAllowableOffset = offset;

            int wkid;
            if (int.TryParse(oSR, out wkid))
                result.outSR = new SpatialReference() { wkid = wkid };

            result.outFields = oFields;
            result.f = format;

            return result;
        }
    }

    public class LayerServicePerformer
    {
        private static GeoAPI.CoordinateSystems.ICoordinateSystem _convertToCRS(SpatialReference sr)
        {
            if (sr == null)
                return null;
            else if (sr.wkid == 4326)
                return ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;
            else if (sr.wkid == 3857 || sr.wkid == 102100)
                return ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator;
            else
                return null;
        }
        private static GeoAPI.CoordinateSystems.Transformations.IMathTransform _getTransform(SpatialReference source, SpatialReference target)
        {
            GeoAPI.CoordinateSystems.ICoordinateSystem sourceCS = _convertToCRS(source);
            GeoAPI.CoordinateSystems.ICoordinateSystem targetCS = _convertToCRS(target);

            if (sourceCS == null || targetCS == null || sourceCS.WKT == targetCS.WKT)
                return null;

            return new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory().CreateFromCoordinateSystems(sourceCS, targetCS).MathTransform;
        }
        private static string _processIds(string ids)
        {
            if (String.IsNullOrEmpty(ids))
                return null;

            List<int> result = new List<int>();
            string[] id_s = ids.Split(',');
            foreach (string id in id_s)
            {
                int i;
                if (int.TryParse(id.Trim(), out i))
                    result.Add(i);
            }
            return String.Join(",", result);
        }

        private static bool _verifySpatialRel(string spatialRel, GeoAPI.Geometries.IGeometry geoSource, GeoAPI.Geometries.IGeometry geoTarget)
        {
            if (spatialRel == esriSpatialRel.esriSpatialRelContains)
                return geoSource.Contains(geoTarget);
            else if (spatialRel == esriSpatialRel.esriSpatialRelCrosses)
                return geoSource.Crosses(geoTarget);
            else if (spatialRel == esriSpatialRel.esriSpatialRelEnvelopeIntersects)
                return true;//TODO: 怎么实现
            else if (spatialRel == esriSpatialRel.esriSpatialRelIndexIntersects)
                return true;//TODO: 如何实现
            else if (spatialRel == esriSpatialRel.esriSpatialRelOverlaps)
                return geoSource.Overlaps(geoTarget);
            else if (spatialRel == esriSpatialRel.esriSpatialRelTouches)
                return geoSource.Touches(geoTarget);
            else if (spatialRel == esriSpatialRel.esriSpatialRelWithin)
                return geoSource.Within(geoTarget);
            else if (spatialRel == esriSpatialRel.esriSpatialRelRelation)
                return true;//TODO:
            else
                return geoSource.Intersects(geoTarget);
        }

        private static Geometry _createGeometry(IDictionary<string, object> row, string geometryType)
        {
            Geometry result = null;
            if (esriGeometryType.esriGeometryPoint == geometryType)
            {
                double pntX = double.Parse(row["X"].ToString());
                double pntY = double.Parse(row["Y"].ToString());
                result = new Point() { x = pntX, y = pntY };
                //result = new NetTopologySuite.Geometries.Point(pntX, pntY);
            }
            else if (esriGeometryType.esriGeometryPolyline == geometryType)
            {
                return null;
            }
            else if (esriGeometryType.esriGeometryPolygon == geometryType)
            {
                var geoJson = row["GEOMETRY"].ToString();
                result = Geometry.fromJson(esriGeometryType.esriGeometryPolygon, geoJson) as Polygon;
            }

            return result;
        }

        public static object query(string sname, int lyrid, QueryParams qps, ResultOptions opt)
        {
            TableInfo service = ServiceConfig.getServiceInfo(sname, lyrid);
            _queryLayer(service as LayerInfo, qps, opt);

            if (service == null)
            {
                //无法完成查询                
                //return JsonConvert.SerializeObject(new ServiceErrorResult()
                //{
                //    error = new ServiceError()
                //    {
                //        code = 400,
                //        message = "不能完成操作",
                //        details = new List<string>() { "没有找到图层" }
                //    }
                //});
                return new ServiceErrorResult()
                {
                    error = new ServiceError()
                    {
                        code = 400,
                        message = "不能完成操作",
                        details = new List<string>() { "没有找到图层" }
                    }
                };
            }
            else if (service is LayerInfo)
                return _queryLayer(service as LayerInfo, qps, opt);
            else
                return _queryTable(service, qps, opt);

        }

        //private static void  

        private static object _queryTable(TableInfo tb, QueryParams qps, ResultOptions opt)
        {
            //TODO: 在有些情况下，idField并不存在，需要考虑
            List<Field> outFields = tb.calculateOutFields(opt.outFields);

            List<dynamic> result = null;

            if (!String.IsNullOrEmpty(qps.objectIds))
            {
                string ids = _processIds(qps.objectIds);
                string sql = "select " + String.Join(",", outFields.Select(f => f.name)) + " from " + tb.s_tableName + "where ";
                if (ids == String.Empty)
                    sql += "1>1";
                else
                    sql += tb.s_idField + " in (" + ids + ")";

                using (var conn = Database.DbService())
                {
                    result = conn.Query(sql).ToList();
                }
            }
            else
            {
                if (String.IsNullOrEmpty(qps.where) && !String.IsNullOrEmpty(qps.text))
                    qps.where = tb.displayField + " like '%" + qps.text + "%'";

                if (!String.IsNullOrEmpty(qps.where))
                {
                    string wherecause = "";

                    if (qps.where.Trim() == String.Empty)
                        Utility.appendSQL(ref wherecause, "(1=1)");
                    else
                        Utility.appendSQL(ref wherecause, "(" + qps.where + ")");

                    string sql = "select " + String.Join(",", outFields.Select(f => f.name)) + " from " + tb.s_tableName + " " + wherecause;

                    using (var conn = Database.DbService())
                    {
                        result = conn.Query(sql).ToList();
                    }
                }
            }

            if (result == null)
            {
                //无法完成查询
                //return JsonConvert.SerializeObject(new ServiceErrorResult()
                //{
                //    error = new ServiceError()
                //    {
                //        code = 400,
                //        message = "不能完成操作",
                //        details = new List<string>() { "无法完成查询", "result.MESSAGE" }//TODO:错误信息回传
                //    }
                //});
                return new ServiceErrorResult()
                {
                    error = new ServiceError()
                    {
                        code = 400,
                        message = "不能完成操作",
                        details = new List<string>() { "无法完成查询", "result.MESSAGE" }//TODO:错误信息回传
                    }
                };
            }
            else
            {
                //由查询结果生成
                if (opt.returnIdsOnly)
                {
                    LayerQueryIdsOnly retVal = new LayerQueryIdsOnly();
                    retVal.objectIdFieldName = tb.s_idField;
                    List<int> retIds = new List<int>();

                    foreach (var row in result)
                    {
                        var rowDic = (IDictionary<string, object>)row;
                        retIds.Add(int.Parse(rowDic[retVal.objectIdFieldName].ToString()));
                    }
                    retVal.objectIds = retIds;

                    return retVal;//JsonConvert.SerializeObject(retVal);
                }
                else
                {
                    TableQuery retVal = new TableQuery();
                    retVal.displayFieldName = tb.displayField;
                    retVal.fields = outFields;

                    List<Feature> feas = new List<Feature>();
                    foreach (var row in result)
                    {
                        var rowDic = (IDictionary<string, object>)row;
                        Feature fea = new Feature();

                        //添加属性
                        fea.attributes = new Dictionary<string, object>();
                        for (int i = 0; i < retVal.fields.Count; i++)
                            fea.attributes.Add(retVal.fields[i].name, rowDic[retVal.fields[i].name]);

                        feas.Add(fea);
                    }

                    retVal.features = feas;

                    return retVal;//JsonConvert.SerializeObject(retVal);
                }
            }
        }

        private static object _queryLayer(LayerInfo lyr, QueryParams qps, ResultOptions opt)
        {
            List<Field> outFields = lyr.calculateOutFields(opt.outFields);

            if (opt.outSR == null && lyr.s_spatialReference != null)
                opt.outSR = new SpatialReference() { wkid = lyr.s_spatialReference.wkid };

            List<dynamic> result = null;
            GeoAPI.Geometries.IGeometry queryGeometry = null;

            if (!String.IsNullOrEmpty(qps.objectIds))
            {
                string ids = _processIds(qps.objectIds);
                string sql = "select " + lyr.generateQueryFields(opt.outFields) + " from " + lyr.s_tableName + " where ";
                if (String.IsNullOrWhiteSpace(ids))
                    sql += "1>1";
                else
                    sql += lyr.s_idField + " in (" + ids + ")";

                using (var conn = Database.DbService())
                {
                    result = conn.Query(sql).ToList();                    
                }
            }
            else
            {
                if (String.IsNullOrEmpty(qps.where) && !String.IsNullOrEmpty(qps.text))
                    qps.where = lyr.displayField + " like '%" + qps.text + "%'";

                if (!String.IsNullOrEmpty(qps.where) || qps.geometry != null)
                {
                    string wherecause = "";
                    DynamicParameters ps = new DynamicParameters();

                    if (!String.IsNullOrEmpty(qps.where))
                    {
                        if (qps.where.Trim() == String.Empty)
                            Utility.appendSQL(ref wherecause, "(1=1)");
                        else
                            Utility.appendSQL(ref wherecause, "(" + qps.where + ")");
                    }
                    
                    if (qps.geometry != null)
                    {
                        GeoAPI.CoordinateSystems.Transformations.IMathTransform transform = _getTransform(qps.inSR, lyr.s_spatialReference);
                        GeoAPI.Geometries.Envelope queryEnvelope = null;
                        if (qps.geometryType == esriGeometryType.esriGeometryEnvelope)
                        {
                            queryEnvelope = (qps.geometry as Envelope).toEnvelope();
                            if (transform != null)
                                queryEnvelope = NetTopologySuite.CoordinateSystems.Transformations.GeometryTransform.TransformBox(queryEnvelope, transform);
                        }
                        else
                        {
                            queryGeometry = qps.geometry.toGeo();

                            GeoAPI.Geometries.IGeometryFactory geoFactory = new NetTopologySuite.Geometries.GeometryFactory();
                            if (transform != null)
                                queryGeometry = NetTopologySuite.CoordinateSystems.Transformations.GeometryTransform.TransformGeometry(geoFactory, queryGeometry, transform);

                            queryEnvelope = queryGeometry.EnvelopeInternal;
                        }

                        if (geometries.esriGeometryType.esriGeometryPoint == lyr.geometryType)
                        {
                            Utility.appendSQL(ref wherecause, "X<:PI_MAXX and X>:PI_MINX and Y<:PI_MAXY and Y>:PI_MINY");
                        }
                        else
                        {
                            Utility.appendSQL(ref wherecause, "MINX<@PI_MAXX and MAXX>@PI_MINX and MINY<@PI_MAXY and MAXY>@PI_MINY");
                        }
                        //TODO: 如果空间关系为相离，就不能使用Envelope加快索引
                        ps.Add("PI_MAXX", queryEnvelope.MaxX, System.Data.DbType.Double);
                        ps.Add("PI_MINX", queryEnvelope.MinX, System.Data.DbType.Double);
                        ps.Add("PI_MAXY", queryEnvelope.MaxY, System.Data.DbType.Double);
                        ps.Add("PI_MINY", queryEnvelope.MinY, System.Data.DbType.Double);
                    }

                    string sql = "select " + lyr.generateQueryFields(opt.outFields) + " from " + lyr.s_tableName + wherecause;

                    using (var conn = Database.DbService())
                    {
                        result = conn.Query(sql, ps).ToList();
                    }
                }
            }

            if (result == null)
            {
                //无法完成查询
                return new ServiceErrorResult()
                {
                    error = new ServiceError()
                    {
                        code = 400,
                        message = "不能完成操作",
                        details = new List<string>() { "无法完成查询", "result.MESSAGE" }//TODO:错误信息回传
                    }
                };
            }
            else
            {
                //由查询结果生成
                if (opt.returnIdsOnly)
                {
                    LayerQueryIdsOnly retVal = new LayerQueryIdsOnly();
                    retVal.objectIdFieldName = lyr.s_idField;
                    List<int> retIds = new List<int>();

                    //由数据库几何字段创建的geometry
                    Geometry rowGeometry = null;

                    foreach (var row in result)
                    {
                        var rowDic = (IDictionary<string, object>)row;
                        if (queryGeometry != null)
                        {
                            rowGeometry = _createGeometry(rowDic, lyr.geometryType);                            

                            if (!_verifySpatialRel(qps.spatialRel, queryGeometry, rowGeometry.toGeo()))
                                continue;
                        }
                        retIds.Add(int.Parse(rowDic[retVal.objectIdFieldName].ToString()));
                    }
                    retVal.objectIds = retIds;

                    return retVal;// JsonConvert.SerializeObject(retVal);
                }
                else
                {
                    LayerQuery retVal = new LayerQuery();
                    retVal.displayFieldName = lyr.displayField;
                    retVal.geometryType = lyr.geometryType;
                    retVal.spatialReference = opt.outSR;
                    retVal.fields = outFields;

                    GeoAPI.CoordinateSystems.Transformations.IMathTransform transform = _getTransform(lyr.s_spatialReference, opt.outSR);

                    List<Feature> feas = new List<Feature>();

                    //由数据库几何字段创建的geometry
                    Geometry rowGeometry = null;

                    foreach (var row in result)
                    {
                        var rowDic = (IDictionary<string, object>)row;

                        if (queryGeometry != null || opt.returnGeometry)
                            rowGeometry = _createGeometry(rowDic, lyr.geometryType);

                        if (queryGeometry != null)
                        {
                            if (!_verifySpatialRel(qps.spatialRel, queryGeometry, rowGeometry.toGeo()))
                                continue;
                        }

                        Feature fea = new Feature();

                        //添加属性
                        fea.attributes = new Dictionary<string, object>();
                        for (int i = 0; i < retVal.fields.Count; i++)
                            fea.attributes.Add(retVal.fields[i].name, rowDic[retVal.fields[i].name]);

                        //添加几何
                        if (opt.returnGeometry)
                        {
                            //object xData = rowDic[lyr.s_xField];
                            //object yData = rowDic[lyr.s_yField];
                            //if (Convert.IsDBNull(xData) || Convert.IsDBNull(yData))
                            //    continue;

                            //double fea_X = double.Parse(xData.ToString());
                            //double fea_Y = double.Parse(yData.ToString());

                            //if (transform != null)
                            //{
                            //    double[] transXY = transform.Transform(new double[] { fea_X, fea_Y });
                            //    fea.geometry = new Point() { x = transXY[0], y = transXY[1] };
                            //}
                            //else
                            //    fea.geometry = new Point() { x = fea_X, y = fea_Y };
                            fea.geometry = rowGeometry;
                        }
                        feas.Add(fea);
                    }
                    retVal.features = feas;

                    return retVal;//JsonConvert.SerializeObject(retVal);
                }
            }
        }
    }
}