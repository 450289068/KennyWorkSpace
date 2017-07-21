using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Seismic.Data.CommonLib
{
    public class ExcelCommonHelper
    {
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

        public static string GetExcelAddress(string startRow, int curretnIndex)
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

        public static string GetExcelCellAddress(string startRow, int curretnIndex, bool IsVertical)
        {
            string index = Regex.Replace(startRow, "[a-zA-Z]", "");
            string column = Regex.Replace(startRow, "[0-9]", "");
            string currentRow = string.Empty;
            if (!string.IsNullOrEmpty(index))
            {
                if (IsVertical)  //B3->B4 true
                {
                    int currentRowIndex = Convert.ToInt32(index) + curretnIndex;
                    currentRow = column + currentRowIndex;
                }
                else
                {
                    var currentColumnIndex = IntToMoreChar(stringToMoreChar(column) + curretnIndex);
                    currentRow = currentColumnIndex + index;
                }
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

        public static int GetExcelCurrentColumnIndex(string startRow)
        {
            string column = Regex.Replace(startRow, "[0-9]", "");
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
    }
}
