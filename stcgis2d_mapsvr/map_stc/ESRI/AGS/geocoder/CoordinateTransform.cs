using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESRI.AGS.geocoder.CoordinateTransform
{
    /// <summary>
    /// 提供了百度坐标（BD09）、国测局坐标（火星坐标，GCJ02）、和WGS84坐标系之间的转换
    /// 改写自开源项目https://github.com/wandergis/coordtransform
    /// </summary>
    public static class WebMapTransform
    {
        private static double a = 6378245.0;
        private static double ee = 0.00669342162296594323;

        public static double[] gcj02towgs84(double lng, double lat)
        {
            lat = +lat;
            lng = +lng;
            if (out_of_china(lng, lat))
            {
                return new double[] { lng, lat };
            }
            else
            {
                var dlat = transformlat(lng - 105.0, lat - 35.0);
                var dlng = transformlng(lng - 105.0, lat - 35.0);
                var radlat = lat / 180.0 * Math.PI;
                var magic = Math.Sin(radlat);
                magic = 1 - ee * magic * magic;
                var sqrtmagic = Math.Sqrt(magic);
                dlat = (dlat * 180.0) / ((a * (1 - ee)) / (magic * sqrtmagic) * Math.PI);
                dlng = (dlng * 180.0) / (a / sqrtmagic * Math.Cos(radlat) * Math.PI);
                var mglat = lat + dlat;
                var mglng = lng + dlng;
                return new double[] { lng * 2 - mglng, lat * 2 - mglat };
            }
        }

        private static bool out_of_china(double lng, double lat)
        {
            lat = +lat;
            lng = +lng;
            // 纬度3.86~53.55,经度73.66~135.05 
            return !(lng > 73.66 && lng < 135.05 && lat > 3.86 && lat < 53.55);
        }

        private static double transformlat(double lng, double lat)
        {
            lat = +lat;
            lng = +lng;
            var ret = -100.0 + 2.0 * lng + 3.0 * lat + 0.2 * lat * lat + 0.1 * lng * lat + 0.2 * Math.Sqrt(Math.Abs(lng));
            ret += (20.0 * Math.Sin(6.0 * lng * Math.PI) + 20.0 * Math.Sin(2.0 * lng * Math.PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(lat * Math.PI) + 40.0 * Math.Sin(lat / 3.0 * Math.PI)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(lat / 12.0 * Math.PI) + 320 * Math.Sin(lat * Math.PI / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        private static double transformlng(double lng, double lat)
        {
            lat = +lat;
            lng = +lng;
            var ret = 300.0 + lng + 2.0 * lat + 0.1 * lng * lng + 0.1 * lng * lat + 0.1 * Math.Sqrt(Math.Abs(lng));
            ret += (20.0 * Math.Sin(6.0 * lng * Math.PI) + 20.0 * Math.Sin(2.0 * lng * Math.PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(lng * Math.PI) + 40.0 * Math.Sin(lng / 3.0 * Math.PI)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(lng / 12.0 * Math.PI) + 300.0 * Math.Sin(lng / 30.0 * Math.PI)) * 2.0 / 3.0;
            return ret;
        }
    }

    /// <summary>
    /// 102100和4326两个坐标系的转换
    /// </summary>
    public static class WebMercatorTransform
    {
        public static readonly double ORIG_X = -20000000.0;
        public static readonly double ORIG_Y = -20000000.0;
        public static readonly double DEGREES_PER_RADIANS = 180.0 / Math.PI;
        public static readonly double RADIANS_PER_DEGREES = Math.PI / 180.0;
        public static readonly double PI_OVER_2 = Math.PI / 2.0;
        public static readonly double RADIUS = 6378137.0;
        public static readonly double RADIUS_2 = RADIUS * 0.5;
        public static readonly double RAD_RAD = RADIANS_PER_DEGREES * RADIUS;

        /**
         * Convert geo lat to vertical distance in meters.
         *
         * @param latitude the latitude in decimal degrees.
         * @return the vertical distance in meters.
         */
        public static double latitudeToY(double latitude)
        {
            double rad = latitude * RADIANS_PER_DEGREES;
            double sin = Math.Sin(rad);
            return RADIUS_2 * Math.Log((1.0 + sin) / (1.0 - sin));
        }

        /**
         * Convert geo lon to horizontal distance in meters.
         *
         * @param longitude the longitude in decimal degrees.
         * @return the horizontal distance in meters.
         */
        public static double longitudeToX(double longitude)
        {
            return longitude * RAD_RAD;
        }

        /**
         * Convert horizontal distance in meters to longitude in decimal degress.
         *
         * @param x the horizontal distance in meters.
         * @return the longitude in decimal degrees.
         */
        public static double xToLongitude(double x)
        {
            return xToLongitude(x, true);
        }

        /**
         * Convert horizontal distance in meters to longitude in decimal degress.
         *
         * @param x      the horizontal distance in meters.
         * @param linear if using continuous pan.
         * @return the longitude in decimal degrees.
         */
        public static double xToLongitude(
                double x,
                bool linear)
        {
            double rad = x / RADIUS;
            double deg = rad * DEGREES_PER_RADIANS;
            if (linear)
            {
                return deg;
            }
            double rotations = Math.Floor((deg + 180.0) / 360.0);
            return deg - (rotations * 360.0);
        }

        /**
         * Convert vertical distance in meters to latitude in decimal degress.
         *
         * @param y the vertical distance in meters.
         * @return the latitude in decimal degrees.
         */
        public static double yToLatitude(double y)
        {
            double rad = PI_OVER_2 - (2.0 * Math.Atan(Math.Exp(-1.0 * y / RADIUS)));
            return rad * DEGREES_PER_RADIANS;
        }
    }

    public static class TjismTransform
    {
        /// <summary>
        /// 平面到经纬度（地图上的X，Y与此处的参数正好相反）
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns>经度和纬度组成的数组</returns>
        public static double[] PlaneToGps(double X, double Y)
        {
            double num = 6378245.0;
            double num2 = 298.257223563;
            double num3 = 117.19666666666667;
            double num4 = 5.0;
            double num5 = -5.0;
            double num6 = 99932.0 + num5;
            double num7 = -4032489.5 + num4;
            double num8 = 1.0 / num2;
            double num9 = num * (1.0 - num8);
            double num10 = 1.0 - num9 / num * (num9 / num);
            double num11 = 206264.806247096;
            X -= num7;
            double num12 = 1.0 + 0.75 * num10 + 0.703125 * num10 * num10 + 0.68359375 * num10 * num10 * num10 + 0.67291259765625 * num10 * num10 * num10 * num10 + 0.6661834716796875 * num10 * num10 * num10 * num10 * num10;
            num12 = num12 * num * (1.0 - num10);
            double num13 = 0.75 * num10 + 0.9375 * num10 * num10 + 1.025390625 * num10 * num10 * num10 + 1.07666015625 * num10 * num10 * num10 * num10 + 1.1103057861328125 * num10 * num10 * num10 * num10 * num10;
            num13 = -num13 * num * (1.0 - num10) / 2.0;
            double num14 = 0.234375 * num10 * num10 + 0.41015625 * num10 * num10 * num10 + 0.538330078125 * num10 * num10 * num10 * num10 + 0.63446044921875 * num10 * num10 * num10 * num10 * num10;
            num14 = num14 * num * (1.0 - num10) / 4.0;
            double num15 = 0.068359375 * num10 * num10 * num10 + 0.15380859375 * num10 * num10 * num10 * num10 + 2.3856334149326806 * num10 * num10 * num10 * num10 * num10;
            num15 = -num15 * num * (1.0 - num10) / 6.0;
            double num16 = 0.01922607421875 * num10 * num10 * num10 * num10 + 0.0528717041015625 * num10 * num10 * num10 * num10 * num10;
            num16 = num16 * num * (1.0 - num10) / 8.0;
            double num17 = 0.00528717041015625 * num10 * num10 * num10 * num10 * num10;
            num17 = -num17 * num * (1.0 - num10) / 10.0;
            double num18 = num * num / num9;
            double num19 = 0.0;
            double num20 = X / num12 - num13 / num12 * Math.Sin(2.0 * num19) - num14 / num12 * Math.Sin(4.0 * num19) - num15 / num12 * Math.Sin(6.0 * num19) - num16 / num12 * Math.Sin(8.0 * num19) - num17 / num12 * Math.Sin(10.0 * num19);
            while (Math.Abs(num20 - num19) > 1E-14)
            {
                num19 = num20;
                num20 = X / num12 - num13 / num12 * Math.Sin(2.0 * num19) - num14 / num12 * Math.Sin(4.0 * num19) - num15 / num12 * Math.Sin(6.0 * num19) - num16 / num12 * Math.Sin(8.0 * num19) - num17 / num12 * Math.Sin(10.0 * num19);
            }
            double d = (num * num - num9 * num9) / (num9 * num9);
            double num21 = Y - num6;
            double num22 = Math.Tan(num20);
            double num23 = Math.Sqrt(d) * Math.Cos(num20);
            double num24 = 1.0 + Math.Pow(num23, 2.0);
            double num25 = num18 / Math.Sqrt(num24);
            double num26 = num20 - 0.5 * (num24 * num22) * Math.Pow(num21 / num25, 2.0);
            num26 += (5.0 + 3.0 * num22 * num22 + num23 * num23 - 9.0 * num23 * num23 * num22 * num22) * (num24 * num22) * Math.Pow(num21 / num25, 4.0) / 24.0;
            num26 -= (61.0 + 90.0 * num22 * num22 + 45.0 * Math.Pow(num22, 4.0)) * (num24 * num22) * Math.Pow(num21 / num25, 6.0) / 720.0;
            double num27 = 1.0 / Math.Cos(num20) * (num21 / num25);
            num27 -= (1.0 + 2.0 * num22 * num22 + num23 * num23) * (1.0 / Math.Cos(num20)) * Math.Pow(num21 / num25, 3.0) / 6.0;
            num27 += (5.0 + 28.0 * num22 * num22 + 24.0 * Math.Pow(num22, 4.0) + 6.0 * num23 * num23 + 8.0 * num23 * num23 * num22 * num22) * (1.0 / Math.Cos(num20)) * Math.Pow(num21 / num25, 5.0) / 120.0;
            num27 += 3600.0 * num3 / num11;
            double Bout = num26 * num11 / 3600.0;
            double Lout = num27 * num11 / 3600.0;

            return new double[] { Lout, Bout };
        }

        /// <summary>
        /// 经纬度到平面
        /// </summary>
        /// <param name="B84">纬度</param>
        /// <param name="L84">经度</param>
        /// <returns>X，Y坐标组成的数组</returns>
        public static double[] GpsToPlane(double B84, double L84)
        {
            double num = 6378245.0;
            double num2 = 298.257223563;
            double num3 = 117.19666666666667;
            double num4 = 5.0;
            double num5 = -5.0;
            double num6 = 99932.0 + num5;
            double num7 = -4032489.5 + num4;
            double num8 = 3.1415926535;
            double num9 = B84 * num8 / 180.0;
            double num10 = L84 * num8 / 180.0;
            double num11 = 1.0 / num2;
            double num12 = num * (1.0 - num11);
            double num13 = (num * num - num12 * num12) / (num * num);
            double num14 = 206264.806247096;
            double num15 = num7;
            double num16 = num6;
            double num17 = 1.0 + 0.75 * num13 + 0.703125 * num13 * num13 + 0.68359375 * num13 * num13 * num13 + 0.67291259765625 * num13 * num13 * num13 * num13 + 0.6661834716796875 * num13 * num13 * num13 * num13 * num13;
            double num18 = 0.75 * num13 + 0.9375 * num13 * num13 + 1.025390625 * num13 * num13 * num13 + 1.07666015625 * num13 * num13 * num13 * num13 + 1.1103057861328125 * num13 * num13 * num13 * num13 * num13;
            double num19 = 0.234375 * num13 * num13 + 0.41015625 * num13 * num13 * num13 + 0.538330078125 * num13 * num13 * num13 * num13 + 0.63446044921875 * num13 * num13 * num13 * num13 * num13;
            double num20 = 0.068359375 * num13 * num13 * num13 + 0.15380859375 * num13 * num13 * num13 * num13 + 2.3856334149326806 * num13 * num13 * num13 * num13 * num13;
            double num21 = 0.01922607421875 * num13 * num13 * num13 * num13 + 0.0528717041015625 * num13 * num13 * num13 * num13 * num13;
            double num22 = 0.00528717041015625 * num13 * num13 * num13 * num13 * num13;
            double num23 = num * num / num12;
            double num24 = (num * num - num12 * num12) / (num12 * num12);
            double num25 = num17 * num * (1.0 - num13);
            double num26 = num18 / 2.0 * num * (1.0 - num13);
            double num27 = num19 / 4.0 * num * (1.0 - num13);
            double num28 = num20 / 6.0 * num * (1.0 - num13);
            double num29 = num10 - num3 * 3600.0 / num14;
            double num30 = num29 * Math.Cos(num9);
            double num31 = num25 * num9 - num26 * Math.Sin(2.0 * num9) + num27 * Math.Sin(4.0 * num9) - num28 * Math.Sin(6.0 * num9);
            double num32 = Math.Tan(num9);
            double num33 = num24 * Math.Pow(Math.Cos(num9), 2.0);
            double d = 1.0 + num33;
            double num34 = num23 / Math.Sqrt(d);
            double num35 = num31 + 0.5 * num34 * num32 * num30 * num30;
            num35 += (5.0 - num32 * num32 + 9.0 * num33 + 4.0 * num33 * num33) * num34 * num32 * Math.Pow(num30, 4.0) / 24.0;
            num35 += (61.0 - 58.0 * num32 * num32 + Math.Pow(num32, 4.0)) * num34 * num32 * Math.Pow(num30, 6.0) / 720.0;
            double num36 = num34 * num30 + (1.0 - num32 * num32 + num33) * num34 * Math.Pow(num30, 3.0) / 6.0;
            num36 += (5.0 - 18.0 * num32 * num32 + Math.Pow(num32, 4.0) + 14.0 * num33 - 58.0 * num33 * num32 * num32) * num34 * Math.Pow(num30, 5.0) / 120.0;
            double Yout = num16 + num36;
            double Xout = num35 + num15;

            return new double[] { Yout, Xout };
        }
    }
}