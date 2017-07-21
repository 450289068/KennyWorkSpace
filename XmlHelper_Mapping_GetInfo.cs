XmlDocument _xdoc = new XmlDocument();
            _xdoc.LoadXml(xmlText);
            XmlNodeList loaderNode = _xdoc.GetElementsByTagName("SourceMapping");
            this.sourceModels = new List<SourceModel>();
            foreach (XmlNode item in loaderNode)
            {
                SourceModel source = new SourceModel();
                source.Source = item["NameRegex"].InnerText;
                if (item["AsOfDate"] != null)
                {
                    source.ASOFDate = item["AsOfDate"].InnerText;
                }
                source.SheetName = item["SheetName"].InnerText;
                source.TableName = item["Table"].InnerText;
                if (item["Direction"] != null)
                {
                    source.Direction = item["Direction"].InnerText;
                }
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

                source.ColumnRange = GetColumnRange(item);

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