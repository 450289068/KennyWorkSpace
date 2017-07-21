using Seismic.Data.CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StellaDataHandler
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XmlHelper<T> where T : new()
    {
        #region
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string EntityToXml(T item)
        {
            IList<T> items = new List<T>();
            items.Add(item);
            return EntityToXml(items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string EntityToXml(IList<T> items)
        {

            XmlDocument doc = new XmlDocument();

            XmlElement root = doc.CreateElement(typeof(T).Name + "s");

            foreach (T item in items)
            {
                EntityToXml(doc, root, item);
            }

            doc.AppendChild(root);

            return doc.InnerXml;
        }

        private static void EntityToXml(XmlDocument doc, XmlElement root, T item)
        {

            XmlElement xmlItem = doc.CreateElement(typeof(T).Name);


            System.Reflection.PropertyInfo[] propertyInfo =
            typeof(T).GetProperties(System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);



            foreach (System.Reflection.PropertyInfo pinfo in propertyInfo)
            {
                if (pinfo != null)
                {

                    string name = pinfo.Name;

                    string value = String.Empty;

                    if (pinfo.GetValue(item, null) != null)
                        value = pinfo.GetValue(item, null).ToString();

                    xmlItem.SetAttribute(name, value);
                }
            }

            root.AppendChild(xmlItem);
        }


        #endregion

        #region

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml">xml</param>
        /// <returns></returns>
        public static T XmlToEntity(string xml)
        {
            IList<T> items = XmlToEntityList(xml);
            if (items != null && items.Count > 0)
                return items[0];
            else return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml">xml</param>
        /// <returns></returns>
        public static IList<T> XmlToEntityList(string xml)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(xml);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Read DataFiles XML has error massage:" + ex.ToString());
                return null;
            }
            if (doc.ChildNodes.Count != 1)
                return null;

            if (doc.ChildNodes[0].Name.ToLower() != typeof(T).Name.ToLower() + "s")
                return null;

            XmlNode node = doc.ChildNodes[0];

            IList<T> items = new List<T>();

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name.ToLower() == typeof(T).Name.ToLower())
                    items.Add(XmlNodeToEntity(child));
            }

            return items;
        }

        private static T XmlNodeToEntity(XmlNode node)
        {
            T item = new T();

            if (node.NodeType == XmlNodeType.Element)
            {
                XmlElement element = (XmlElement)node;

                System.Reflection.PropertyInfo[] propertyInfo =
            typeof(T).GetProperties(System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);


                foreach (XmlAttribute attr in element.Attributes)
                {
                    string attrName = attr.Name.ToLower();
                    string attrValue = attr.Value.ToString();
                    foreach (System.Reflection.PropertyInfo pinfo in propertyInfo)
                    {
                        if (pinfo != null)
                        {
                            string name = pinfo.Name.ToLower();
                            Type dbType = pinfo.PropertyType;
                            if (name == attrName)
                            {
                                if (String.IsNullOrEmpty(attrValue))
                                    continue;
                                switch (dbType.ToString())
                                {
                                    case "System.Int32":
                                        pinfo.SetValue(item, Convert.ToInt32(attrValue), null);
                                        break;
                                    case "System.Boolean":
                                        pinfo.SetValue(item, Convert.ToBoolean(attrValue), null);
                                        break;
                                    case "System.DateTime":
                                        pinfo.SetValue(item, Convert.ToDateTime(attrValue), null);
                                        break;
                                    case "System.Decimal":
                                        pinfo.SetValue(item, Convert.ToDecimal(attrValue), null);
                                        break;
                                    case "System.Double":
                                        pinfo.SetValue(item, Convert.ToDouble(attrValue), null);
                                        break;
                                    default:
                                        pinfo.SetValue(item, attrValue, null);
                                        break;
                                }
                                continue;
                            }
                        }
                    }
                }
            }
            return item;
        }
        #endregion
    }
}