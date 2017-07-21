using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Seismic.Data.CommonLib
{
    public class XMLOperation
    {
        private static XmlDocument xmlDoc = new XmlDocument();

        public static XmlElement GetRootElement
        {
            get
            {
                return xmlDoc.DocumentElement;
            }
        }

        public static string GetAttributeValue(XmlNode attribute)
        {
            string value = string.Empty;
            if (attribute != null)
            {
                value = attribute.Value;
            }

            return value;
        }

        public static XmlNode GetElementNode(XmlDocument xmlDoc, string xmlPath)
        {
            XmlNode node = xmlDoc.SelectSingleNode(xmlPath);
            return node;
        }

        public static List<XmlAttribute> GetXmlNodeAttribute(XmlNode node)
        {
            List<XmlAttribute> attribute = new List<XmlAttribute>();

            foreach (XmlAttribute attributeItem in node.Attributes)
            {
                attribute.Add(attributeItem);
            }

            return attribute;
        }

        public static List<XmlAttribute> GetXmlNodeAttribute(string filePath, string singleNodeName)
        {
            xmlDoc.Load(filePath);
            XmlNode node = xmlDoc.SelectSingleNode(singleNodeName);
            return GetXmlNodeAttribute(node);
        }

        public static string GetXmlNodeInnerText(XmlNode node)
        {
            string formatInnerText = node.InnerText;
            formatInnerText = formatInnerText.Replace("[", "<");
            formatInnerText = formatInnerText.Replace("]", ">");
            return formatInnerText;
        }

        public static List<XmlNode> GetXmlNodeList(string filePath, string singleNodeName)
        {
            xmlDoc.Load(filePath);
            return GetXmlNodeList(xmlDoc, singleNodeName);
        }

        public static List<XmlNode> GetXmlNodeList(string singleNodeName)
        {
            return GetXmlNodeList(xmlDoc, singleNodeName);
        }

        public static List<XmlNode> GetXmlNodeList(XmlDocument xmlDoc, string singleNodeName)
        {
            XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName(singleNodeName);
            List<XmlNode> list = new List<XmlNode>();
            if (xmlNodeList != null && xmlNodeList.Count > 0)
            {
                foreach (XmlNode node in xmlNodeList[0].ChildNodes)
                {
                    if (node.NodeType != XmlNodeType.Comment)
                    {
                        list.Add(node);
                    }
                }
            }
            return list;
        }

        public static void LoadXmlDocument(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                xmlDoc.Load(filePath);
            }
        }
    }
}
