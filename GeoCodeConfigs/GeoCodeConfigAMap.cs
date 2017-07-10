using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullScreen.Newtonsoft.Json;

namespace GeoCode
{
    public class GeoCodeConfigAMap : IGeoCodeConfig
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
                    addressResult.ResultCode = result.infocode;
                    addressResult.Message = result.info;
                    addressResult.Address = result.regeocode.formatted_address;
                    addressResult.City = result.regeocode.addressComponent.city.ToString();
                    addressResult.Country = result.regeocode.addressComponent.country.ToString();
                    addressResult.District = result.regeocode.addressComponent.district.ToString();
                    addressResult.Province = result.regeocode.addressComponent.province.ToString();
                    addressResult.Towncode = result.regeocode.addressComponent.towncode.ToString();
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
            var cood = GpsCood.GpsCoodCorrect.Convert(double.Parse(lat), double.Parse(lng));
            return new Uri(string.Format(Url, cood.Longitude + "," + cood.Latitude, Key) + "&t=" + DateTime.Now.Ticks.ToString());
            //  return new Uri(string.Format(Url, lng + "," + lat, Key) + "&t=" + DateTime.Now.Ticks.ToString());

        }

        private bool CheckResult(LocationResult result)
        {
            if (result.infocode == "10003")
            {
                if (KeyUsedOver != null)
                {
                    KeyUsedOver();
                }
                return false;
            }
            else if (result.infocode == "10004")
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
        /// <param name="useDate"></param>
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
            public string info { get; set; }
            public string infocode { get; set; }
            public RegeocodeEntity regeocode { get; set; }
        }

        public class RegeocodeEntity
        {
            public string formatted_address { get; set; }
            public AddressComponent addressComponent { get; set; }
        }

        public class AddressComponent
        {
            public string country { get; set; }
            public string province { get; set; }
            public object city { get; set; }
            public object citycode { get; set; }
            public object district { get; set; }
            public object adcode { get; set; }
            public object township { get; set; }
            public object towncode { get; set; }
        }



    }




}
