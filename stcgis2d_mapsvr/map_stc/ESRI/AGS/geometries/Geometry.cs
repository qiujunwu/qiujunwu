using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ESRI.AGS.geometries
{
    public static class esriSpatialRel
    {
        public static readonly string esriSpatialRelIntersects = "esriSpatialRelIntersects";
        public static readonly string esriSpatialRelContains = "esriSpatialRelContains";
        public static readonly string esriSpatialRelCrosses = "esriSpatialRelCrosses";
        public static readonly string esriSpatialRelEnvelopeIntersects = "esriSpatialRelEnvelopeIntersects";
        public static readonly string esriSpatialRelIndexIntersects = "esriSpatialRelIndexIntersects";
        public static readonly string esriSpatialRelOverlaps = "esriSpatialRelOverlaps";
        public static readonly string esriSpatialRelTouches = "esriSpatialRelTouches";
        public static readonly string esriSpatialRelWithin = "esriSpatialRelWithin";
        public static readonly string esriSpatialRelRelation = "esriSpatialRelRelation";
    }
    public static class esriGeometryType
    {
        public static readonly string esriGeometryPoint = "esriGeometryPoint";
        public static readonly string esriGeometryMultipoint = "esriGeometryMultipoint";
        public static readonly string esriGeometryPolyline = "esriGeometryPolyline";
        public static readonly string esriGeometryPolygon = "esriGeometryPolygon";
        public static readonly string esriGeometryEnvelope = "esriGeometryEnvelope";
    }
    public abstract class Geometry
    {
        [JsonIgnore]
        public SpatialReference spatialReference { get; set; }

        public static Geometry fromJson(string geoType, string geoJson)
        {
            if (geoType == esriGeometryType.esriGeometryPoint)
                return JsonConvert.DeserializeObject<Point>(geoJson);
            else if (geoType == esriGeometryType.esriGeometryMultipoint)
                return JsonConvert.DeserializeObject<Multipoint>(geoJson);
            else if (geoType == esriGeometryType.esriGeometryPolyline)
                return JsonConvert.DeserializeObject<Polyline>(geoJson);
            else if (geoType == esriGeometryType.esriGeometryPolygon)
                return JsonConvert.DeserializeObject<Polygon>(geoJson);
            else if (geoType == esriGeometryType.esriGeometryEnvelope)
                return JsonConvert.DeserializeObject<Envelope>(geoJson);
            else
                return null;
        }
        public static T fromJson<T>(string json) where T : Geometry
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public abstract GeoAPI.Geometries.IGeometry toGeo();
    }
    
    public class Point : Geometry
    {
        public double x { get; set; }
        public double y { get; set; }
        public override GeoAPI.Geometries.IGeometry toGeo()
        {
            return new NetTopologySuite.Geometries.Point(x, y);
        }
    }
    public class Polyline : Geometry
    {
        public List<List<double[]>> paths { get; set; }
        public override GeoAPI.Geometries.IGeometry toGeo()
        {
            if (paths == null)
                return null;

            List<NetTopologySuite.Geometries.LineString> lines = new List<NetTopologySuite.Geometries.LineString>();
            for (int i = 0; i < paths.Count; i++)
            {
                List<GeoAPI.Geometries.Coordinate> pnts = new List<GeoAPI.Geometries.Coordinate>();
                for (int j = 0; j < paths[i].Count; j++)
                {
                    pnts.Add(new GeoAPI.Geometries.Coordinate(paths[i][j][0], paths[i][j][1]));
                }
                lines.Add(new NetTopologySuite.Geometries.LineString(pnts.ToArray()));
            }
            return new NetTopologySuite.Geometries.MultiLineString(lines.ToArray());
        }
    }
    public class Polygon : Geometry
    {
        public List<List<double[]>> rings { get; set; }
        public override GeoAPI.Geometries.IGeometry toGeo()
        {
            if (rings == null)
                return null;

            GeoAPI.Geometries.ILinearRing shell = null;
            List<GeoAPI.Geometries.ILinearRing> holes = new List<GeoAPI.Geometries.ILinearRing>();
            for (int i = 0; i < rings.Count; i++)
            {
                List<GeoAPI.Geometries.Coordinate> pnts = new List<GeoAPI.Geometries.Coordinate>();
                for (int j = 0; j < rings[i].Count; j++)
                {
                    pnts.Add(new GeoAPI.Geometries.Coordinate(rings[i][j][0], rings[i][j][1]));
                }
                if (i == 0)
                    shell = new NetTopologySuite.Geometries.LinearRing(pnts.ToArray());
                else
                    holes.Add(new NetTopologySuite.Geometries.LinearRing(pnts.ToArray()));
            }

            if (shell == null)
                return null;

            if (holes.Count == 0)
                return new NetTopologySuite.Geometries.Polygon(shell);
            else
                return new NetTopologySuite.Geometries.Polygon(shell, holes.ToArray());
        }
    }
    public class Multipoint : Geometry
    {
        public List<double[]> points { get; set; }
        public override GeoAPI.Geometries.IGeometry toGeo()
        {
            if (points == null)
                return null;
            List<GeoAPI.Geometries.IPoint> pnts = new List<GeoAPI.Geometries.IPoint>();
            for (int i = 0; i < points.Count; i++)
            {
                pnts.Add(new NetTopologySuite.Geometries.Point(points[i][0], points[i][1]));
            }
            return new NetTopologySuite.Geometries.MultiPoint(pnts.ToArray());
        }
    }
    public class Extent
    {
        public double xmin { get; set; }
        public double ymin { get; set; }
        public double xmax { get; set; }
        public double ymax { get; set; }
        public SpatialReference spatialReference { get; set; }
    }
    public class Envelope : Geometry
    {
        public double xmin { get; set; }
        public double ymin { get; set; }
        public double xmax { get; set; }
        public double ymax { get; set; }

        public Envelope transform(GeoAPI.CoordinateSystems.Transformations.IMathTransform trans)
        {
            if (trans == null)
                return null;

            double[] min = trans.Transform(new double[] { xmin, ymin });
            double[] max = trans.Transform(new double[] { xmax, ymax });

            return new Envelope() { xmin = min[0], ymin = min[1], xmax = max[0], ymax = max[0] };
        }
        public GeoAPI.Geometries.Envelope toEnvelope()
        {
            return new GeoAPI.Geometries.Envelope(xmin, xmax, ymin, ymax);
        }

        public override GeoAPI.Geometries.IGeometry toGeo()
        {
            return null;//应该返回由四个点组成的多边形
        }
    }
}