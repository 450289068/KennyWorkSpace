        private void InitialParameters()
        {
            Console.WriteLine("Initializing parameters");
            XmlDocument _xdoc = new XmlDocument();
            _xdoc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mapping.xml"));
            XmlNodeList loaderNode = _xdoc.GetElementsByTagName("SourceMapping");
            this.sourceModels = new List<SourceModel>();
            foreach (XmlNode item in loaderNode)
            {
                SourceModel source = new SourceModel();
                source.Source = item["NameRegex"].InnerText;
                if (item["AsOfDateTab"] != null)
                {
                    source.ASOFDateTab = item["AsOfDateTab"].InnerText;
                }

                if (item["AsOfDate"] != null)
                {
                    source.ASOFDate = item["AsOfDate"].InnerText;
                }
                source.SheetName = item["SheetName"].InnerText;
                source.Location = item["Location"].InnerText;
                source.TableName = item["Table"].InnerText;
                source.Property = new List<string>();
                if (item["Properties"] != null)
                {
                    string[] parameterArray = item["Properties"].InnerText.Split(',');
                    for (int i = 0; i < parameterArray.Length; i++)
                    {
                        source.Property.Add(parameterArray[i].Trim());
                    }
                }

                source.ColumnList = item["ColumnList"].InnerText;

                bool isDelete;
                Boolean.TryParse(item["IsDelete"].InnerText, out isDelete);
                source.IsDelete = isDelete;

                bool tableHead;
                Boolean.TryParse(item["TableHead"].InnerText, out tableHead);
                source.TableHead = tableHead;

                bool multiTable;
                Boolean.TryParse(item["MultiTable"].InnerText, out multiTable);
                source.MultiTable = multiTable;

                if (item["KeyColumnIndex"] != null)
                {
                    source.KeyColumnIndex = item["KeyColumnIndex"].InnerText;
                }


                if (item["NeedDateColumn"] != null)
                {
                    var stringNeedDateColumn = item["NeedDateColumn"].InnerText;
                    bool needDateColumn;
                    Boolean.TryParse(stringNeedDateColumn, out needDateColumn);
                    source.NeedDateColumn = needDateColumn;
                }
                else
                {
                    //Default Value
                    source.NeedDateColumn = true;
                }

                if (item["WholeSheet"] != null)
                {
                    var stringWholeSheet = item["WholeSheet"].InnerText;
                    bool wholeSheet;
                    Boolean.TryParse(stringWholeSheet, out wholeSheet);
                    source.WholeSheet = wholeSheet;
                }

                //Get column and deleteColumns
                source.ColumnRange = GetColumnRange(item, source);

                if (item["TableColumn"] != null)
                {
                    source.Filter = item["TableColumn"].Attributes.GetNamedItem("Filter") == null ?
                        string.Empty : item["TableColumn"].Attributes.GetNamedItem("Filter").Value;

                    source.FormatString = item["TableColumn"].Attributes.GetNamedItem("Format") == null ?
                        string.Empty : item["TableColumn"].Attributes.GetNamedItem("Format").Value;

                    source.DefaultString = item["TableColumn"].Attributes.GetNamedItem("Default") == null ?
                        string.Empty : item["TableColumn"].Attributes.GetNamedItem("Default").Value;

                    var emptyValue = item["TableColumn"].Attributes.GetNamedItem("EmptyValue") == null ? string.Empty : item["TableColumn"].Attributes.GetNamedItem("EmptyValue").Value;

                    if (string.IsNullOrEmpty(emptyValue))
                    {
                        source.EmptyValue = false;
                    }
                    else
                    {
                        source.EmptyValue = bool.Parse(emptyValue);
                    }


                }

                this.sourceModels.Add(source);
            }

            #region SFTP
            //SFTPDownLoaderConfig.LoadConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SFTPConfiguration.xml"));
            #endregion

            xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Performance.xml");
            if (!File.Exists(xmlFilePath))
            {
                throw new Exception("The data xml configuration file (Performance.xml) is not exist.");
            }

            var localFolderString = ConfigurationManager.AppSettings["LocalFolder"];
            if (!string.IsNullOrEmpty(localFolderString) && !Directory.Exists(localFolderString))
            {
                Directory.CreateDirectory(localFolderString);
            }
            LocalFolder = localFolderString;

            Console.WriteLine("Initializing complete");
        }

private Dictionary<string, string> GetColumnRange(XmlNode item, SourceModel source)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (source.DeleteColumns == null)
            {
                source.DeleteColumns = new Dictionary<string, string>();
            }

            XmlElement tableColumnNode = item["TableColumn"];

            if (tableColumnNode == null)
                return null;

            foreach (XmlNode childNode in tableColumnNode.ChildNodes)
            {
                string key = childNode.Attributes.GetNamedItem("Name").Value;
                string value = childNode.Attributes.GetNamedItem("Range").Value;

                if (childNode.Attributes.GetNamedItem("DeleteColumn") != null && bool.Parse(childNode.Attributes.GetNamedItem("DeleteColumn").Value))
                {
                    source.DeleteColumns.Add(key, string.Empty);
                }

                dictionary.Add(key, value);
            }

            return dictionary;
        }