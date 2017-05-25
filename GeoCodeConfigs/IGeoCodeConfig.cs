using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoCode
{
    public interface IGeoCodeConfig
    {
        /// <summary>
        /// Key已经被使用超过了限制
        /// </summary>
        event Action KeyUsedOver;
        /// <summary>
        /// 每秒使用超出限制
        /// </summary>
        event Action KeyOneSecondUsedOver;

        /// <summary>
        /// URL模板（包含未替换参数的url）
        /// </summary>
        string Url { get; }
        /// <summary>
        /// 地图Key的Value
        /// </summary>
        string Key { get; }
        /// <summary>
        /// 已用次数
        /// </summary>
        int UsedCount { get; set; }
        /// <summary>
        /// 最大支持次数
        /// </summary>
        int MaxCount { get; }
        /// <summary>
        /// 加载配置信息
        /// </summary>
        /// <param name="url">URL模板（包含未替换参数的url）</param>
        /// <param name="key">地图Key的Value</param>
        /// <param name="usedCount">已用次数</param>
        /// <param name="maxCount">最大支持次数</param>
        void LoadConfig(string url, string key, int usedCount, int maxCount);
        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="address">webAPI的结果（JSON格式）</param>
        /// <returns>解析结果对象</returns>
        AddressResult ResultFormat(string address);
        /// <summary>
        /// 获取请求服务的url
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lng">精度</param>
        /// <returns></returns>
        Uri GetRequestUrl(string lat, string lng);

        /// <summary>
        /// 地图类型
        /// </summary>
        string MapType { get; }
    }
}
