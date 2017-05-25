using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoCode
{
    /// <summary>
    /// 地址结果
    /// </summary>
    public class AddressResult
    {
        private static readonly AddressResult defaultValue = new AddressResult()
        {
            Success = false,
            Message = "默认值",
            Address = string.Empty
        };
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 异常消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 结果码
        /// </summary>
        public string ResultCode { get; set; }
        /// <summary>
        /// 地址描述
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 区县
        /// </summary>
        public string District { get; set; }
        /// <summary>
        /// 乡镇
        /// </summary>
        public string Towncode { get; set; }
        /// <summary>
        /// 转Json
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            StringBuilder sb = new StringBuilder();
            var properties = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.SetProperty);
            foreach (System.Reflection.PropertyInfo p in properties)
            {
                if (sb.Length != 0)
                    sb.Append(",");
                sb.Append("\"");
                sb.Append(p.Name.ToLower());
                sb.Append("\":\"");
                sb.Append(p.GetValue(this, null));
                sb.Append("\"");
            }
            return "{" + sb.ToString() + "}";
        }
        public static AddressResult DefatulValue
        {
            get
            {
                return defaultValue;
            }
        }
    }
}
