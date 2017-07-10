using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GeoCode
{
    internal class GeoCodeConfigManager
    {
        private static GeoCodeXmlHelper xmlHelper = new GeoCodeXmlHelper();
        private static GeoCodeConfigCache cache = new GeoCodeConfigCache();
        private static IGeoCodeConfig config;
        static GeoCodeConfigManager()
        {
            CacheDepend<GeoCodeConfigCache> depend = new CacheDepend<GeoCodeConfigCache>(cache, xmlHelper.GetXmlFileName());
            //依赖文件发生变化
            depend.DependFiledChanged += (sender, e) =>
            {
                ReLoadConfig(xmlHelper.MapType);
            };
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>       
        internal static IGeoCodeConfig GetConfig()
        {
            var mapTypes = xmlHelper.GetGeocodeMaps();
            return GetConfig(mapTypes, mapTypes[0]);
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="mapType">地图类型 AMap；QQMap；BMap</param>
        /// <returns></returns>
        internal static IGeoCodeConfig GetConfig(string mapType)
        {
            var mapTypes = xmlHelper.GetGeocodeMaps();
            return GetConfig(mapTypes, mapType);
        }

        /// <summary>
        /// 设置某个地图的Key不可用
        /// </summary>
        /// <param name="config"></param>
        internal static void SetUnAble(IGeoCodeConfig config)
        {
            xmlHelper.SetKeyAttribute("able", "false");
            //文件依赖，不需要重新加载，会出发重新加载xml事件
            // ReLoadConfig(config.MapType);
        }

        /// <summary>
        /// 追加已经使用次数
        /// </summary>
        /// <param name="config"></param>
        /// <param name="appendCount"></param>
        internal static void AddUsedCount(IGeoCodeConfig config, int appendCount = 1)
        {
            if (config.UsedCount + appendCount > config.MaxCount)
            {
                SetUnAble(config);
            }
            else
            {
                //Tool.UsedDataManager.SetUseCount(config.Key, (config.UsedCount += appendCount), config.UsedCount == config.MaxCount);
            }
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        private static IGeoCodeConfig GetConfig(List<string> mapTypes, string mapType)
        {
            if (mapTypes.All(item => item != mapType))
            {
                throw new ArgumentException("参数mapType的地图类型不存在");
            }
            if (mapTypes.Count > 0)
            {
                //设置地图类型
                xmlHelper.SetMapType(mapType);
                config = cache.GetConfig(mapType);
                //没缓存
                if (config == null)
                {
                    config = GetGeoCodeCofnigInstance(mapType);
                    var key = xmlHelper.GetKey();
                    config.LoadConfig(xmlHelper.GetMapUrlFormat(),
                                        key,
                                        0,// Tool.UsedDataManager.GetUsedCount(key),
                                        xmlHelper.GetMaxCount());
                    cache.AddConfig(mapType, config);
                }
                config.KeyUsedOver += () =>
                {
                    SetUnAble(config);
                };
                return config;
            }
            return null;
        }

        /// <summary>
        /// 重载配置
        /// </summary>
        /// <param name="mapTypes"></param>
        /// <param name="mapType"></param>
        /// <returns></returns>
        private static IGeoCodeConfig ReLoadConfig(string mapType)
        {
            xmlHelper.ClearNodes();
            cache.RemoveConfig(mapType);
            return GetConfig(xmlHelper.GetGeocodeMaps(), mapType);
        }

        /// <summary>
        /// 重置Keys
        /// </summary>
        /// <param name="mapType"></param>
        private static void ReInitKeys()
        {
            var keynodes = xmlHelper.GetMapWillAbleKeyNodeList();
            xmlHelper.SetKeyAttribute(keynodes, new Dictionary<string, string>() { 
                {"able","true"}                 
            });
        }

        /// <summary>
        /// 获取地图配置类实例
        /// </summary>
        /// <param name="mapType"></param>
        /// <returns></returns>
        private static IGeoCodeConfig GetGeoCodeCofnigInstance(string mapType)
        {
            return (IGeoCodeConfig)Assembly.Load(typeof(IGeoCodeConfig).Assembly.FullName).CreateInstance("GeoCode.GeoCodeConfig" + mapType);
        }
    }
}
