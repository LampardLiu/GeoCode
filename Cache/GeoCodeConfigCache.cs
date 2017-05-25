using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoCode
{
    internal class GeoCodeConfigCache
    {
        /// <summary>
        /// 存储对象的字典 key：地图类型，Value：配置对象
        /// </summary>
        private Dictionary<string, IGeoCodeConfig> GeoCodeCache = new Dictionary<string, IGeoCodeConfig>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maptype"></param>
        /// <param name="config"></param>
        internal void AddConfig(string maptype, IGeoCodeConfig config)
        {
            if (GeoCodeCache.Keys.Contains(maptype))
            {
                GeoCodeCache[maptype] = config;
                return;
            }
            GeoCodeCache.Add(maptype, config);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maptype"></param>
        internal void RemoveConfig(string maptype)
        {
            if (GeoCodeCache.Keys.Contains(maptype))
            {
                GeoCodeCache.Remove(maptype);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maptype"></param>
        /// <returns></returns>
        internal IGeoCodeConfig GetConfig(string maptype)
        {
            if (GeoCodeCache.Keys.Contains(maptype))
            {
                return GeoCodeCache[maptype];
            }
            return null;
        }
    }
}
