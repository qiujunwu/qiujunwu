using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace map_stc.STC.HouseNumber
{
    public class Point
    {
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public double x { get; set; }
        public double y { get; set; }
    }
    public class Algorithm
    {
        public static double distance(double x1, double y1, double x2, double y2)
        {
            double dx = x1 - x2;
            double dy = y1 - y2;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        public static double distance(Point p1, Point p2)
        {
            return distance(p1.x, p1.y, p2.x, p2.y);
        }
        public static double distanceToSegment(Point p1, Point p2, Point q, out Point closest)
        {
            double dx = p2.x - p1.x;
            double dy = p2.y - p1.y;

            if (dx == 0 && dy == 0)
            {
                closest = new Point(p1.x, p1.y);
                return distance(q, closest);
            }

            double len2 = dx * dx + dy * dy;
            double r = ((q.x - p1.x) * dx + (q.y - p1.y) * dy) / len2;

            if (r <= 0)
            {
                closest = new Point(p1.x, p1.y);
            }                
            else if (r >= 1)
            {
                closest = new Point(p2.x, p2.y);
            }                
            else
            {
                closest = new Point(p1.x + r * dx, p1.y + r * dy);
            }                

            return distance(q, closest);
        }
        /// <summary>
        /// 左侧为单号，右侧为双号《天津市门牌标志管理办法》
        /// </summary>
        /// <param name="len"></param>
        /// <param name="left">是否在左侧</param>
        /// <returns></returns>
        public static int suggestNumber(double len, bool left)
        {
            int number = (int)Math.Floor(len);
            bool flag = (number % 2) == 1;
            if (flag == left)
                return number;
            else
                return number + 1;
        }
        /// <summary>
        /// q在p1-p2左边返回1
        /// q在p1-p2右边返回-1
        /// q在p1-p2线上返回0
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static int orientationIndex(Point p1, Point p2, Point q)
        {
            var coor1 = new GeoAPI.Geometries.Coordinate(p1.x, p1.y);
            var coor2 = new GeoAPI.Geometries.Coordinate(p2.x, p2.y);
            var coor = new GeoAPI.Geometries.Coordinate(q.x, q.y);
            return NetTopologySuite.Algorithm.CGAlgorithmsDD.OrientationIndex(coor1, coor2, coor);
        }
    }
}