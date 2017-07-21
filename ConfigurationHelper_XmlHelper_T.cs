using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Seismic.Data.CommonLib;

namespace EmpowerDeliveryOptionToZenventoryHandler
{

    public class ConfigurationModel
    {
        public string StorageConnectionString { get; set; }
        public string AzureBolbStorageContainerName { get; set; }
        public string EncryptionKey { get; set; }
        public string EncryptionPassword { get; set; }
        public string Emails { get; set; }
        public string DownLoadFolder { get; set; }
        public string CollateralCodeToStateFile { get; set; }
    }


    public class ConfigurationModelParse<T> where T : new()
    {
        public static string UploaderElement { get; set; }

        private static XmlDocument _xdoc = new XmlDocument();

        public static List<T> CreateConfigurationModelInstance(string filePath, string rootElement)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("Configuration file is not exists!");
            }

            UploaderElement = rootElement;

            List<T> list = new List<T>();

            if (!string.IsNullOrEmpty(filePath))
            {
                _xdoc.Load(filePath);
                list = LoadConfigurationModelElementNode();
            }

            return list;
        }

        public static List<T> CreateConfigurationModelInstanceByXmlString(string xmlConfigPath)
        {
            List<T> list = new List<T>();

            if (!string.IsNullOrEmpty(xmlConfigPath))
            {
                _xdoc.LoadXml(xmlConfigPath);
                list = LoadConfigurationModelElementNode();
            }

            return list;
        }

        private static List<T> LoadConfigurationModelElementNode()
        {
            XmlNode node = XMLOperation.GetElementNode(_xdoc, "//" + UploaderElement);
            List<T> list = new List<T>();

            if (node != null)
            {
                T model = PopulateEntityFromCollection<T>(new T(), node.ChildNodes);
                list.Add(model);
            }

            return list;
        }

        private static T PopulateEntityFromCollection<T>(T entity, XmlNodeList collection) where T : new()
        {
            if (entity == null)
            {
                entity = new T();
            }

            Type type = typeof(T);
            PropertyInfo[] propertyInfos = type.GetProperties();

            foreach (XmlNode xnode in collection)
            {
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    if (xnode.Name == propertyInfo.Name)
                    {
                        propertyInfo.SetValue(entity, Convert.ChangeType(xnode.InnerText.Trim(), propertyInfo.PropertyType), null);
                        break;
                    }
                }
            }

            return entity;
        }
    }
}

