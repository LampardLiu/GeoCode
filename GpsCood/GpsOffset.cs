using System;

namespace GeoCode.GpsCood
{
    public class GpsOffset
    {
        private static GpsOffset _instance;
        public static GpsOffset Create()
        {
            return _instance ?? (_instance = new GpsOffset());
        }

        //private EncryptClass _gpsfix;
        public GpsOffset()
        {
            //if (_gpsfix == null)
            //    _gpsfix = new EncryptClass();
        }

        /// <summary>
        /// 加密GPS
        /// </summary>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <returns></returns>
        public EncryptPoint EncryptPoint(decimal latitude, decimal longitude)
        {
            var point = new EncryptPoint() { Latitude = 0, Longitude = 0 };
            if (latitude < 1 || longitude < 1)
            {
                point.Longitude = (double)longitude;
                point.Latitude = (double)latitude;
                return point;
            }
            //_gpsfix.EncryptPoint(Convert.ToDouble(longitude), Convert.ToDouble(latitude), out point.Longitude,
            //                     out point.Latitude);
            var entity = GpsCoodCorrect.Convert(Convert.ToDouble(latitude), Convert.ToDouble(longitude));
            point.Latitude = entity.Latitude;
            point.Longitude = entity.Longitude;
            return point;
        }

        /// <summary>
        /// 加密GPS
        /// </summary>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <returns></returns>
        public EncryptPoint EncryptPoint(double latitude, double longitude)
        {
            var point = new EncryptPoint() { Latitude = 0, Longitude = 0 };
            if (latitude < 1 || longitude < 1)
            {
                point.Longitude = longitude;
                point.Latitude = latitude;
                return point;
            }
            //_gpsfix.EncryptPoint(longitude, latitude, out point.Longitude, out point.Latitude);
            return point;
        }

        /// <summary>
        /// 加密GSP偏移
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public OffSetPoint OffsetPoint(double latitude, double longitude)
        {
            var point = new OffSetPoint() { Latitude = 0, Longitude = 0 };
            if (latitude < 1 || longitude < 1)
            {
                return point;
            }
            double lat = 0;
            double lng = 0;
            point.Latitude = lat - latitude;
            point.Longitude = lng - longitude;
            //_gpsfix.EncryptPoint(longitude, latitude, out lng, out lat);

            return point;
        }

        /// <summary>
        /// 加密GSP偏移
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public OffSetPoint OffsetPoint(decimal latitude, decimal longitude)
        {
            var point = new OffSetPoint() { Latitude = 0, Longitude = 0 };
            if (latitude < 1 || longitude < 1)
            {
                return point;
            }
            double lat = 0;
            double lng = 0;
            point.Latitude = lat - (double)latitude;
            point.Longitude = lng - (double)longitude;
            //_gpsfix.EncryptPoint((double)longitude, (double)latitude, out lng, out lat);

            return point;
        }
    }

    public struct EncryptPoint
    {
        public double Latitude;

        public double Longitude;
    }

    public struct OffSetPoint
    {
        public double Latitude;

        public double Longitude;
    }
}
