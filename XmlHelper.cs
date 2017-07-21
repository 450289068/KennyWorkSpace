using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Seismic.Data.CommonLib
{
    public class XmlHelper
    {
        public static XmlNode GetXmlNodeByXpath(string xmlFileName, string xpath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileName);
            XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath);
            return xmlNode;
        }

        public static List<XmlNode> GetXmlNodeList(XmlNode xmlNode, string singleNodeName)
        {
            XmlNodeList xmlNodeList = xmlNode.SelectNodes(singleNodeName);
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

        public static List<XmlNode> GetXmlChildNodes(XmlNode xmlDoc, string singleNodeName)
        {
            XmlNodeList xmlNodeList = xmlDoc.SelectNodes(singleNodeName);
            List<XmlNode> list = new List<XmlNode>();
            if (xmlNodeList != null && xmlNodeList.Count > 0)
            {
                foreach (XmlNode node in xmlNodeList)
                {
                    if (node.NodeType != XmlNodeType.Comment)
                    {
                        list.Add(node);
                    }
                }
            }
            return list;
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

        public static string GetValueFromXml(XmlNode xmlNode, string attributeName)
        {
            string attributeValue = string.Empty;
            if (xmlNode != null && xmlNode.Attributes.GetNamedItem(attributeName) != null)
            {
                attributeValue = xmlNode.Attributes.GetNamedItem(attributeName).Value;
            }

            return attributeValue;
        }

        public static int GetIntValueFromXml(XmlNode xmlNode, string attributeName)
        {
            int result = 0;
            if (xmlNode != null && xmlNode.Attributes.GetNamedItem(attributeName) != null)
            {
                string attributeValue = xmlNode.Attributes.GetNamedItem(attributeName).Value;
                Int32.TryParse(attributeValue,out result);
            }

            return result;
        }

        public static bool GeBooleanValueFromXml(XmlNode xmlNode, string attributeName)
        {
            bool result = false;
            if (xmlNode != null && xmlNode.Attributes.GetNamedItem(attributeName) != null)
            {
                string attributeValue = xmlNode.Attributes.GetNamedItem(attributeName).Value;
                Boolean.TryParse(attributeValue, out result);
            }

            return result;
        }

        public static int GetIntValueFromXmlInnerText(XmlNode xmlNode, string nodeName)
        {
            int result = 0;

            XmlNode node = xmlNode.SelectSingleNode(nodeName);
            if (node != null && !string.IsNullOrEmpty(node.InnerText))
            {
                int.TryParse(node.InnerText, out result);
            }

            return result;
        }

        public static bool GetBooleanValueFromXmlInnerText(XmlNode xmlNode, string nodeName)
        {
            bool result = false;

            XmlNode node = xmlNode.SelectSingleNode(nodeName);
            if (node != null && !string.IsNullOrEmpty(node.InnerText))
            {
                Boolean.TryParse(node.InnerText, out result);
            }

            return result;
        }

        public static string GetValueFromXmlInnerText(XmlNode xmlNode, string nodeName)
        {
            string result = string.Empty;
            XmlNode node = xmlNode.SelectSingleNode(nodeName);
            if (node != null)
            {
                result = node.InnerText;
            }

            return result;
        }

        public static bool GetBooleanValueFromXml(XmlNode xmlNode, string attributeName)
        {
            bool attributeValue = false;
            if (xmlNode != null && xmlNode.Attributes.GetNamedItem(attributeName) != null)
            {
                attributeValue = ConvertToBoolean(xmlNode, attributeName);
            }

            return attributeValue;
        }

        public static bool ConvertToBoolean(XmlNode xmlNode, string attributeName, bool attributeDefaultValue = false)
        {
            if (xmlNode != null && xmlNode.Attributes.GetNamedItem(attributeName) != null)
            {
                string attributeValueString = xmlNode.Attributes.GetNamedItem(attributeName).Value;
                Boolean.TryParse(attributeValueString, out attributeDefaultValue);
            }

            return attributeDefaultValue;
        }
    }
}
