using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Seismic.Data.CommonLib
{
    public class ExcelHelper
    {

        public static string FloatLength = "f18";

        public static string IntToMoreChar(int value)
        {
            string rtn = string.Empty;
            List<int> iList = new List<int>();

            //To single Int
            while (value / 26 != 0 || value % 26 != 0)
            {
                iList.Add(value % 26);
                value /= 26;
            }

            //Change 0 To 26
            for (int j = 0; j < iList.Count - 1; j++)
            {
                if (iList[j] == 0)
                {
                    iList[j + 1] -= 1;
                    iList[j] = 26;
                }
            }

            //Remove 0 at last
            if (iList[iList.Count - 1] == 0)
            {
                iList.Remove(iList[iList.Count - 1]);
            }

            //To String
            for (int j = iList.Count - 1; j >= 0; j--)
            {
                char c = (char)(iList[j] + 64);
                rtn += c.ToString();
            }

            return rtn;
        }

        public static int stringToMoreChar(string column)
        {
            if (!Regex.IsMatch(column.ToUpper(), @"[A-Z]+"))
            {
                throw new Exception("Invalid parameter");
            }
            int index = 0;
            char[] chars = column.ToUpper().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                index += ((int)chars[i] - (int)'A' + 1) * (int)Math.Pow(26, chars.Length - i - 1);
            }

            return index;
        }

        public static string GetExcelCellAddress(uint rowIndex, uint columnIndex)
        {
            uint remainder;
            string columnName = string.Empty;
            while (columnIndex > 0)
            {
                remainder = (columnIndex - 1) % 26;
                columnName = System.Convert.ToChar(65 + remainder).ToString() + columnName;
                columnIndex = (uint)((columnIndex - remainder) / 26);
            }
            return columnName + rowIndex;
        }

        public static string GetExcelCellAddress(string startRow, int curretnIndex)
        {
            string index = Regex.Replace(startRow, "[a-zA-Z]", "");
            string column = Regex.Replace(startRow, "[0-9]", "");
            string currentRow = string.Empty;
            if (!string.IsNullOrEmpty(index))
            {
                int currentRowIndex = Convert.ToInt32(index) + curretnIndex;
                currentRow = column + currentRowIndex;
            }

            return currentRow;
        }

        public static int GetExcelCurrentRowIndex(string startRow, int curretnIndex)
        {
            string index = Regex.Replace(startRow, "[a-zA-Z]", "");
            int currentRowIndex = 0;
            if (!string.IsNullOrEmpty(index))
            {
                currentRowIndex = Convert.ToInt32(index) + curretnIndex;
            }

            return currentRowIndex;
        }

        public static int GetExcelCurrentColumnIndex(string columnAddress)
        {
            string column = Regex.Replace(columnAddress, "[0-9]", "");
            return stringToMoreChar(column);
        }

        public static string GetExcelCurrentColumn(string currentColumnAddress)
        {
            return Regex.Replace(currentColumnAddress, "[0-9]", "").ToUpper();
        }

        public static int GetExcelRowIndex(string currentColumnAddress)
        {
            string rowIndex = Regex.Replace(currentColumnAddress, "[a-zA-Z]", "");
            int index;
            Int32.TryParse(rowIndex, out index);
            if (index == 0)
            {
                index = 1;
            }

            return index;
        }

        public static IWorksheet GetSheetBySheetName(IWorkbook workbook, string sheetName)
        {
            string sheetNames = string.Empty;
            for (int i = 0; i < workbook.Worksheets.Count; i++)
            {
                var tempSheet = workbook.Worksheets[i];
                if (tempSheet.Visible != SheetVisibility.Hidden)
                {
                    if (tempSheet.Name.ToLower().Trim() == sheetName.ToLower().Trim())
                    {
                        return tempSheet;
                    }

                    sheetNames += tempSheet.Name + ",";
                }
            }

            throw new Exception(string.Format("Sheet '{0}' doesn't exist in current file '{1}', all sheets can be found are '{2}'", sheetName, workbook.Name, sheetNames.Substring(0, sheetNames.Length - 1)));
        }

        public static IWorksheet GetSheetBySheetName(IWorkbook workbook, string sheetName,out string strString)
        {
            string sheetNames=string.Empty;
            strString = string.Empty;
            for (int i = 0; i < workbook.Worksheets.Count; i++)
            {
                var tempSheet = workbook.Worksheets[i];
                if(tempSheet.Visible!=SheetVisibility.Hidden)
                {
                    if (tempSheet.Name.ToLower().Trim() == sheetName.ToLower().Trim())
                    {
                        return tempSheet;
                    }

                    sheetNames += tempSheet.Name + ",";
                }
            }
            strString = string.Format("Sheet '{0}' doesn't exist in current file '{1}', all sheets can be found are '{2}'", sheetName, workbook.Name, sheetNames.Substring(0, sheetNames.Length - 1));
            Console.WriteLine("Sheet '{0}' doesn't exist in current file '{1}', all sheets can be found are '{2}'", sheetName, workbook.Name, sheetNames.Substring(0, sheetNames.Length - 1));
            return null;
        }

        public IWorksheet GetSheetLikeSheetName(IWorkbook workbook, string sheetName)
        {
            for (int i = 0; i < workbook.Worksheets.Count; i++)
            {
                var tempSheet = workbook.Worksheets[i];

                if (tempSheet.Name.ToLower().Trim().Contains(sheetName.ToLower().Trim()) && tempSheet.Visible != SheetVisibility.Hidden)
                {
                    return tempSheet;
                }
            }

            return null;
        }

        public IWorksheet GetWorksheetFirstOrDefault(IWorkbook workbook)
        {
            for (int i = 0; i < workbook.Worksheets.Count; i++)
            {
                var tempSheet = workbook.Worksheets[i];

                if (tempSheet.Visible != SheetVisibility.Hidden)
                {
                    return tempSheet;
                }
            }

            return null;
        }

        public static string GetRangeValue(IRange dataCaption, int rowNum, int columnNum)
        {
            string strRangeValue = string.Empty;

            switch (dataCaption.NumberFormatType)
            {
                case NumberFormatType.General:
                case NumberFormatType.Text:
                    if (dataCaption.Value != null && !string.IsNullOrEmpty(dataCaption.Value.ToString()))
                    {
                        strRangeValue = dataCaption.Value.ToString();
                    }

                    break;

                case NumberFormatType.Date:
                case NumberFormatType.DateTime:
                    strRangeValue = dataCaption.Text.ToString();
                    break;

                case NumberFormatType.Scientific:
                case NumberFormatType.Number:
                case NumberFormatType.Percent:
                    if (dataCaption.Value == null || dataCaption.Value.ToString().Trim() == "N/A" || string.IsNullOrEmpty(dataCaption.Value.ToString().Trim()))
                    {
                        strRangeValue = "0";
                    }
                    else
                    {
                        try
                        {
                            decimal tempDecimal;
                            double tempDouble;
                            if (Double.TryParse(dataCaption.Value.ToString(), out tempDouble))
                            {
                                if (decimal.TryParse(((double)dataCaption.Value).ToString("f18"), out tempDecimal))
                                {
                                    string decimalData = tempDecimal.ToString("f18").TrimEnd('0');
                                    if (decimalData.IndexOf('.') > 0 && decimalData.IndexOf('.') == decimalData.Length - 1)
                                    {
                                        decimalData = decimalData.Replace(".", string.Empty);
                                    }

                                    strRangeValue = decimalData;
                                }
                                else
                                {
                                    strRangeValue = "0";
                                }
                            }
                            else
                            {
                                strRangeValue = dataCaption.Value.ToString();
                            }
                        }
                        catch(Exception ex)
                        {
                            LogHelper.Debug("\r\n error value at " + rowNum + "_row :" + columnNum + "_column. this value is TEXT:" + dataCaption.Text + "   Value:" + dataCaption.Value + ". It is type Decimal. *****");
                            LogHelper.Error(ex.ToString());
                        }
                    }

                    break;
                case NumberFormatType.Time:
                case NumberFormatType.Currency:
                    strRangeValue = dataCaption.Value.ToString();
                    break;
                case NumberFormatType.None:
                    break;
            }

            return strRangeValue;
        }
    }
}
