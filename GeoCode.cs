using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace GeoCode
{
    /// <summary>
    /// 
    /// </summary>
    public class GeoCode
    {
        /// <summary>
        /// 经纬度转地址
        /// </summary>
        /// <param name="lat">维度</param>
        /// <param name="lng">精度</param>
        /// <param name="mapType">
        /// <para>地图类型</para>
        /// <para>AMap:高德地图</para>
        /// <para>QQMap:腾讯地图</para>
        /// </param>       
        /// <returns></returns>
        public static AddressResult Gecode(string lng, string lat, string mapType = "AMap")
        {
            IGeoCodeConfig config = GeoCodeConfigManager.GetConfig(mapType);
            AddressResult address = null;
            LoggerManager.LogTimeInfo(() =>
            {
                address = GeoCodeAction(lng, lat, config);
            }, "lng, lat, mapType", lng, lat, mapType);
            return address;
        }

        private static AddressResult GeoCodeAction(string lng, string lat, IGeoCodeConfig config)
        {
            AddressResult address;
            try
            {
                var client = new WebClient { Encoding = Encoding.UTF8 };
                var result = client.DownloadString(config.GetRequestUrl(lat, lng));
                address = config.ResultFormat(result);
                GeoCodeConfigManager.AddUsedCount(config);
            }
            catch (Exception ex)
            {
                address = AddressResult.DefatulValue;
                LoggerManager.Logger.Error(ex);
                throw ex;
                //TODO:记录异常
            }
            return address;
        }

        /// <summary>
        /// 经纬度转地址
        /// </summary>
        /// <param name="lngAndLat">经纬度使用“,”连接</param>
        /// <param name="mapType">
        /// <para>地图类型</para>
        /// <para>AMap:高德地图</para>
        /// <para>QQMap:腾讯地图</para>
        /// </param>       
        /// <returns></returns>
        public static AddressResult Gecode(string lngAndLat, string mapType = "AMap")
        {
            if (string.IsNullOrEmpty(lngAndLat))
                throw new ArgumentNullException();
            var lngLat = lngAndLat.Split(',');
            if (lngLat.Length != 2)
            {
                throw new ArgumentException("参数格式不正确");
            }
            return Gecode(lngLat[0], lngLat[1], mapType);
        }

        /// <summary>
        /// 批量解析地址（实质是循环解析，返回结果，比较耗时）
        /// </summary>
        /// <param name="keyAndLngLat">key和lanlat，lng和lat之间使用英文逗号分隔</param>
        /// <param name="mapType">
        /// <para>地图类型</para>
        /// <para>AMap:高德地图</para>
        /// <para>QQMap:腾讯地图</para>
        /// </param>       
        /// <returns>解析结果</returns>
        /// <exception cref="Exception，解析失败返回异常"></exception>
        public static Dictionary<string, AddressResult> Gecodes(Dictionary<string, string> keyAndLngLat, string mapType = "AMap")
        {
            Dictionary<string, AddressResult> result = new Dictionary<string, AddressResult>();
            LoggerManager.LogTimeInfo(() =>
            {
                foreach (var key in keyAndLngLat.Keys)
                {
                    result.Add(key, Gecode(keyAndLngLat[key], mapType));
                }
            }, "keyAndLngLat,mapType", keyAndLngLat, mapType);
            return result;
        }

        /// <summary>
        /// 批量解析地址（实质是循环解析，返回结果，比较耗时）
        /// </summary>
        /// <param name="keyAndLngLat">key和lanlat，lng lat数组</param>
        /// <param name="mapType">
        /// <para>地图类型</para>
        /// <para>AMap:高德地图</para>
        /// <para>QQMap:腾讯地图</para>
        /// </param>       
        /// <returns>解析结果</returns>
        /// <exception cref="Exception，解析失败返回异常"></exception>
        public static Dictionary<string, AddressResult> Gecodes(Dictionary<string, string[]> keyAndLngLat, string mapType = "AMap")
        {
            Dictionary<string, AddressResult> result = new Dictionary<string, AddressResult>();
            LoggerManager.LogTimeInfo(() =>
            {
                foreach (var key in keyAndLngLat.Keys)
                {
                    if (keyAndLngLat[key].Length != 2)
                        throw new ArgumentException("参数格式不正确");
                    result.Add(key, Gecode(keyAndLngLat[key][0], keyAndLngLat[key][1], mapType));
                }
            }, "keyAndLngLat,mapType", keyAndLngLat, mapType);
            return result;
        }
    }
}
