using FullScreen.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoCode
{
    /// <summary>
    /// 腾讯地图解析规则
    /// </summary>
    public class GeoCodeConfigQQMap : IGeoCodeConfig
    {
        private string url;
        /// <summary>
        /// URL模板（包含未替换参数的url）
        /// </summary>
        public string Url
        {
            get { return url; }
        }

        public string key;
        /// <summary>
        /// 地图Key的Value
        /// </summary>
        public string Key
        {
            get { return key; }
        }

        public int usedCount;
        /// <summary>
        /// 已用次数
        /// </summary>
        public int UsedCount
        {
            get
            {
                return usedCount;
            }
            set
            {
                usedCount = value;
            }
        }

        public int maxCount;
        /// <summary>
        /// 最大支持次数
        /// </summary>
        public int MaxCount
        {
            get { return maxCount; }
        }
        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="address">webAPI的结果（JSON格式）</param>
        /// <returns>解析结果对象</returns>
        public AddressResult ResultFormat(string address)
        {
            LocationResult result = JsonConvert.DeserializeObject<LocationResult>(address);
            AddressResult addressResult = new AddressResult();
            try
            {
                if (CheckResult(result))
                {
                    addressResult.Success = true;
                    addressResult.ResultCode = result.status.ToString();
                    addressResult.Message = result.message;
                    addressResult.Address = result.result.address;
                    addressResult.City = result.result.address_component.city;
                    addressResult.Country = result.result.address_component.nation;
                    addressResult.District = result.result.address_component.district;
                    addressResult.Province = result.result.address_component.province;
                    addressResult.Towncode = result.result.address_component.street_number;
                }
            }
            catch (Exception ex)
            {
                addressResult.Message += ex.Message;
            }
            return addressResult;
        }

        /// <summary>
        /// 获取请求服务的url
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lng">精度</param>
        /// <returns></returns>
        public Uri GetRequestUrl(string lat, string lng)
        {
            return new Uri(string.Format(Url, lat + "," + lng, Key) + "&t=" + DateTime.Now.Ticks.ToString());
        }

        private bool CheckResult(LocationResult result)
        {
            if (result.status == 121)
            {
                if (KeyUsedOver != null)
                {
                    KeyUsedOver();
                }
                return false;
            }
            else if (result.status == 122)
            {
                if (KeyOneSecondUsedOver != null)
                {
                    KeyOneSecondUsedOver();
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 加载配置信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <param name="usedCount"></param>
        /// <param name="maxCount"></param>
        public void LoadConfig(string url, string key, int usedCount, int maxCount)
        {
            this.url = url;
            this.key = key;
            this.usedCount = usedCount;
            this.maxCount = maxCount;
        }


        public string MapType
        {
            get { return "AMap"; }
        }
        /// <summary>
        /// Key已经被使用超过了限制
        /// </summary>
        public event Action KeyUsedOver;
        /// <summary>
        /// 每秒使用超出限制
        /// </summary>
        public event Action KeyOneSecondUsedOver;

        public class LocationResult
        {
            public int status { get; set; }
            public string message { get; set; }
            public LocationAddressResult result { get; set; }
        }
        public class LocationAddressResult
        {
            public string address { get; set; }
            public LocationFormattedAddresses formatted_addresses { get; set; }
            public AddressComponent address_component { get; set; }
        }
        public class LocationFormattedAddresses
        {
            public string recommend { get; set; }
            public string rough { get; set; }
        }

        public class AddressComponent
        {
            public string nation { get; set; }
            public string province { get; set; }
            public string city { get; set; }
            public string district { get; set; }
            public string street { get; set; }
            public string street_number { get; set; }
        }
    }
}