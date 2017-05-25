/*
 * Target:配置文件读取帮助类
 * Date：2017年5月11日
 * Auto：刘会东
 * TODO：重构GetAttribute方法，代替之前的获取各个属性值的方法
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Linq.Expressions;

namespace GeoCode
{
    internal class GeoCodeXmlHelper
    {
        //xml文件路径
        readonly string xmlFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "GeoCode.config";
        const string geocodemapsXPath = "/geocode/geocodemaps/geocodemap";
        const string mapXPath = "/geocode/maps/map[@name='{0}']";
        const string mapKeyXpath = "keys/key[@able='true']";
        //xml文档对象
        private XmlDocument xmldocment = new XmlDocument();
        //当前操作的地图类型
        private string mapType;
        /// <summary>
        /// 当前操作的地图类型
        /// </summary>
        public string MapType
        {
            get { return mapType; }
        }
        //配置的地图节点对象
        private XmlNode mapNode;
        /// <summary>
        ///配置的地图节点对象
        /// </summary>
        internal XmlNode MapNode
        {
            get
            {
                if (string.IsNullOrEmpty(mapType))
                {
                    throw new Exception("未设置地图类型，请先调用SetMapType方法");
                }
                if (mapNode == null)
                    mapNode = GetMapNode(mapType);
                return mapNode;
            }
        }
        //地图节点下可用key的节点
        private XmlElement mapKeyNode;
        /// <summary>
        /// 地图节点下可用key的节点
        /// </summary>
        internal XmlElement MapKeyNode
        {
            get
            {
                if (mapKeyNode == null)
                    mapKeyNode = GetMapKeyNode(MapNode);
                return mapKeyNode;
            }
        }

        /// <summary>
        /// 构造函数，默认加载程序集目录下的GeoCode.XML
        /// </summary>
        internal GeoCodeXmlHelper()
        {
            xmldocment.Load(xmlFilePath);
        }

        /// <summary>
        /// 构造函数，
        /// </summary>
        /// <param name="xmlFilePath">加载xml文件的路径</param>
        internal GeoCodeXmlHelper(string xmlFilePath)
            : this()
        {
            this.xmlFilePath = xmlFilePath;
        }
        /// <summary>
        /// 设置地图类型
        /// </summary>
        /// <param name="mapType"></param>
        internal void SetMapType(string mapType)
        {
            this.mapType = mapType;
        }

        /// <summary>
        /// 获取已配置的解析地图列表
        /// </summary>
        /// <returns>已配地图类型</returns>
        internal List<string> GetGeocodeMaps()
        {
            List<string> geocodemaps = new List<string>();
            var nodes = xmldocment.SelectNodes(geocodemapsXPath);
            foreach (XmlNode node in nodes)
            {
                geocodemaps.Add(node.Attributes["use"].Value.Trim());
            }
            return geocodemaps;
        }

        /// <summary>
        /// 获取该地图的解析URL格式
        /// </summary>
        /// <param name="maptype">地图类型（找到对应的地图配置节点）</param>
        /// <returns>urlFormat</returns>
        internal string GetMapUrlFormat()
        {
            return MapNode.SelectSingleNode("url").InnerText.Trim();
        }

        /// <summary>
        /// 获取该地图的Key
        /// </summary>
        /// <param name="maptype">地图类型（找到对应的地图配置节点)</param>
        /// <returns></returns>
        internal string GetKey()
        {
            if (MapKeyNode == null) return string.Empty;
            return MapKeyNode.GetAttribute("value").Trim();
        }

        /// <summary>
        /// 获取该地图的MaxCount
        /// </summary>
        /// <param name="maptype">地图类型（找到对应的地图配置节点)</param>
        /// <returns></returns>
        internal int GetMaxCount()
        {
            if (MapKeyNode == null) return 0;
            int usedcount;
            int.TryParse(MapKeyNode.GetAttribute("maxcount"), out usedcount);
            return usedcount;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="maptype">地图类型（找到对应的地图配置节点)</param>
        /// <param name="attribute">属性名称</param>
        /// <param name="value">属性值</param>
        internal void SetKeyAttribute(string attribute, string value)
        {
            if (MapKeyNode == null) return;
            MapKeyNode.SetAttribute(attribute, value);
            xmldocment.Save(this.xmlFilePath);
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="attributeValues">属性和值的字典</param>
        internal void SetKeyAttribute(Dictionary<string, string> attributeValues)
        {
            if (MapKeyNode == null) return;
            foreach (var attribute in attributeValues.Keys)
            {
                MapKeyNode.SetAttribute(attribute, attributeValues[attribute]);
            }
            xmldocment.Save(this.xmlFilePath);
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="nodelist"></param>
        /// <param name="attributeValues"></param>
        internal void SetKeyAttribute(XmlNodeList nodelist, Dictionary<string, string> attributeValues)
        {
            foreach (XmlNode node in nodelist)
            {
                foreach (var attribute in attributeValues.Keys)
                {
                    node.Attributes[attribute].Value = attributeValues[attribute];
                }
            }
            xmldocment.Save(this.xmlFilePath);
        }


        /// <summary>
        /// 清除节点
        /// </summary>
        internal void ClearNodes()
        {
            mapNode = null;
            mapKeyNode = null;
        }

        /// <summary>
        /// 获取已经设置不可用但是应该可用的节点
        /// </summary>
        /// <returns></returns>
        internal XmlNodeList GetMapWillAbleKeyNodeList()
        {
            if (MapNode == null) return null;
            return this.MapNode.SelectNodes("keys/key");
        }

        internal string GetXmlFileName() {
            return this.xmlFilePath;
        }

        /// <summary>
        /// 获取Map地图配置节点
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        private XmlNode GetMapNode(string nodeName)
        {
            return xmldocment.SelectSingleNode(string.Format(mapXPath, nodeName));
        }

        /// <summary>
        /// 获取地图节点下可用的Key节点
        /// </summary>
        /// <param name="mapNode"></param>
        /// <returns></returns>
        private XmlElement GetMapKeyNode(XmlNode mapNode)
        {
            if (MapNode == null) return null;
            return MapNode.SelectSingleNode(mapKeyXpath) as XmlElement;
        }
    }
}
