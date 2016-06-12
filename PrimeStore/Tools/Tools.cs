using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace Tools
{
    /// <summary>
    /// Summary description for Tools.
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Simple helper to validate arguments of methods.
        /// </summary>
        public class ArgumentValidatorHelper
        {
            /// <summary>
            /// Check if passed object is not null.
            /// </summary>
            /// <exception cref="ArgumentNullException">If checked object is null</exception>
            /// <param name="toCheck">object to check</param>
            /// <param name="argumentName">name of agrument in calling method</param>
            public static void IsNotNull(object toCheck, string argumentName)
            {
                if (toCheck == null)
                {
                    throw new ArgumentNullException(string.Format("The {0} should not be null.", argumentName));
                }
            }

            /// <summary>
            /// Check if passed string ins not empty (strings that contains only white
            /// space characters are considered empty)
            /// </summary>
            /// <param name="toCheck">object to check</param>
            /// <param name="argumentName">name of argument in calling method</param>
            public static void IsNotEmptyString(string toCheck, string argumentName)
            {
                if (toCheck.Trim().Length == 0)
                {
                    throw new ArgumentException(string.Format("The {0} should not be empty.", argumentName));
                }
            }

            /// <summary>
            /// Check if passed string is not null, empty or consists only of white-space characters
            /// </summary>
            /// <param name="toCheck">object to check</param>
            /// <param name="argumentName">name of argument in calling method</param>
            public static void IsNotNullOrEmptyString(string toCheck, string argumentName)
            {
                if (string.IsNullOrWhiteSpace(toCheck))
                {
                    throw new ArgumentException(string.Format("The {0} should not be null, empty or consists only of white-space characters.", argumentName));
                }
            }
        }

        public static readonly char SEPARATOR = '|';

        public static bool IsNullOrEmptyString(this object value)
        {
            return value == null || value.ConvertToString().IsEmpty();
        }

        public static bool EqualsEmptyOrNullString(this object value1, object value2)
        {
            return ((value1.IsNullOrEmptyString() && value2.IsNullOrEmptyString()) || String.Equals(value1, value2));
        }

        public static bool IsNotNullNorEmptyString(this object value)
        {
            return !value.IsNullOrEmptyString();
        }

        public static string GetDataSetErrorText(DataSet ds)
        {
            StringBuilder error = new StringBuilder();
            foreach (DataTable table in ds.Tables)
            {
                DataRow[] rows = table.GetErrors();
                if (rows != null && rows.Length > 0)
                {
                    error.Append("Wrong rows in table:");
                    error.Append(table.TableName);
                    error.Append("\n");
                    foreach (DataRow row in rows)
                    {
                        error.Append(row.RowError);
                        error.Append("\n");
                    }
                }
            }
            return error.ToString();
        }

        public static string DisplayString(this Enum value)
        {
            //Using reflection to get the field info
            FieldInfo info = value.GetType().GetField(value.ToString());

            //Get the Description Attributes
            DescriptionAttribute[] attributes = (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length == 1)
            {
                return attributes[0].Description;
            }
            else //Use the value for display if not concrete result
            {
                return value.ToString();
            }
        }

        public static object EnumValueOf<T>(this string descriptionOrValue)
        {
            //Get all possible values of this enum type
            Array tValues = Enum.GetValues(typeof(T));

            //Cycle through all values searching for a match (description or value)
            foreach (Enum val in tValues)
            {
                if (val.DisplayString().Equals(descriptionOrValue) || val.ToString().Equals(descriptionOrValue))
                {
                    return val;
                }
            }

            throw new ArgumentException(string.Format("The string value is not of type {0}.", typeof(T).ToString()));
        }

        public static string DataTable2String(DataTable dt, System.Data.XmlWriteMode m)
        {
            StringWriter strWriter = new StringWriter(new System.Globalization.CultureInfo("en-US"));
            dt.Locale = new System.Globalization.CultureInfo("en-US");
            dt.WriteXml(strWriter, m);
            string sRes = strWriter.GetStringBuilder().ToString();
            return sRes;
        }

        public static string DataSet2String(DataSet ds, System.Data.XmlWriteMode m)
        {
            StringWriter strWriter = new StringWriter(new System.Globalization.CultureInfo("en-US"));
            ds.Locale = new System.Globalization.CultureInfo("en-US");
            ds.WriteXml(strWriter, m);
            string sRes = strWriter.GetStringBuilder().ToString();
            return sRes;
        }

        public static string DataSet2String(DataSet ds, System.Data.XmlWriteMode m, int bufferSize)
        {
            XmlWriter strWriter;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;
            StringBuilder sb = new StringBuilder(bufferSize);
            strWriter = XmlWriter.Create(sb, settings);
            ds.WriteXml(strWriter, m);
            return sb.ToString();
        }

        public static bool String2DataTable(DataTable table, string xml)
        {
            bool ret = false;
            using (DataSet ds = new DataSet("NewDataSet"))
            {
                ds.Tables.Add(table);
                ret = Tools.String2DataSet(ds, xml, XmlReadMode.InferSchema, false);
                ds.Tables.Remove(table);
            }
            return ret;
        }

        public static bool String2DataSet(DataSet ds, string xml, System.Data.XmlReadMode m)
        {
            return String2DataSet(ds, xml, m, false);
        }

        public static bool String2DataSet(DataSet ds, string xml, System.Data.XmlReadMode m, bool clearBeforeRead)
        {
            try
            {
                if (xml.Length < 1)
                {
                    return false;
                }
                using (StringReader strReader = new StringReader(xml))
                {
                    if (clearBeforeRead)
                    {
                        ds.Clear();
                    }
                    if (m == XmlReadMode.ReadSchema)//special modification for java web services
                    {
                        m = System.Data.XmlReadMode.InferSchema;
                    }
                    ds.ReadXml(strReader, m);
                    strReader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\n" + GetDataSetDiagnosticInfo(ds));
            }
            return true;
        }

        public static bool String2DataSet(DataSet ds, Stream stream, System.Data.XmlReadMode m, bool clearBeforeRead)
        {
            if (stream == null)
            {
                return false;
            }
            try
            {
                using (XmlReader strReader = XmlReader.Create(stream))
                {
                    if (clearBeforeRead)
                    {
                        ds.Clear();
                    }
                    if (m == XmlReadMode.ReadSchema)//special modification for java web services
                    {
                        m = System.Data.XmlReadMode.InferSchema;
                    }
                    ds.ReadXml(strReader, m);
                    strReader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\n" + GetDataSetDiagnosticInfo(ds));
            }
            return true;
        }

        public static bool Stream2DataSet(DataSet ds, StreamReader streamReader, System.Data.XmlReadMode m, bool clearBeforeRead)
        {
            if (streamReader == null)
            {
                return false;
            }
            try
            {
                using (XmlReader xmlReader = XmlReader.Create(streamReader))
                {
                    if (clearBeforeRead)
                    {
                        ds.Clear();
                    }
                    if (m == XmlReadMode.ReadSchema)//special modification for java web services
                    {
                        m = System.Data.XmlReadMode.InferSchema;
                    }
                    ds.ReadXml(xmlReader, m);
                    xmlReader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\n" + GetDataSetDiagnosticInfo(ds));
            }
            return true;
        }

        private static string GetDataSetDiagnosticInfo(DataSet ds)
        {
            string err = string.Empty;
            foreach (DataTable table in ds.Tables)
            {
                if (table.HasErrors)
                {
                    err += "Table " + table.TableName + "\r\n";
                    foreach (DataRow row in table.Rows)
                    {
                        if (row.HasErrors)
                        {
                            err += row.RowError + "\r\n";
                        }
                    }
                }
            }
            return err;
        }

        static public void AddTable(DataSet dataSet, string table)
        {
            if (dataSet.Tables.Contains(table) == false)
            {
                dataSet.Tables.Add(table);
            }
        }

        static public void AddColumn(DataSet dataSet, string table, string column)
        {
            DataTable dataTable;

            dataTable = dataSet.Tables[table];
            if (dataTable.Columns.Contains(column) == false)
            {
                dataTable.Columns.Add(column);
            }
        }

        static public void AddColumn(DataSet dataSet, string table, string column, Type type)
        {
            DataTable dataTable;

            dataTable = dataSet.Tables[table];
            if (dataTable.Columns.Contains(column) == false)
            {
                dataTable.Columns.Add(column, type);
            }
        }

        static public void SetPK(DataSet ds, string table, string column, bool autoinc)
        {
            DataColumn col = ds.Tables[table].Columns[column];
            if (autoinc)
            {
                col.AutoIncrement = true;
                col.AutoIncrementSeed = -1;
                col.AutoIncrementStep = -1;
            }
            ds.Tables[table].PrimaryKey = new DataColumn[] { col };
        }

        static public void AddRelation(DataSet ds, string name, string pkTable, string pkColumn, string fkTable, string fkColumn)
        {
            ForeignKeyConstraint fk = new ForeignKeyConstraint(name,
                    ds.Tables[pkTable].Columns[pkColumn],
                    ds.Tables[fkTable].Columns[fkColumn]);
            fk.DeleteRule = Rule.Cascade;
            fk.UpdateRule = Rule.Cascade;
            ds.Tables[fkTable].Constraints.Add(fk);
            DataRelation rel = new DataRelation(name,
                    ds.Tables[pkTable].Columns[pkColumn],
                    ds.Tables[fkTable].Columns[fkColumn]);
            ds.Relations.Add(rel);
        }

        static public bool ReadBinaryFile(string szPath, ref byte[] buff)
        {
            System.IO.FileStream mStreamR;
            System.IO.BinaryReader mBinaryR;
            long Filelen = 0;
            try
            {
                mStreamR = System.IO.File.OpenRead(szPath);
                Filelen = mStreamR.Length;
                mBinaryR = new System.IO.BinaryReader(mStreamR);
                buff = new byte[Filelen];
                mBinaryR.Read(buff, 0, (int)Filelen);
                mBinaryR.Close();
                mStreamR.Close();
            }
            catch (System.IO.FileNotFoundException e)
            {
                return false;
            }
            catch (System.Exception e)
            {
                return false;
            }
            return true;
        }

        static public bool WriteBinaryFile(string szPath, byte[] buff)
        {
            System.IO.FileStream mStreamW;
            System.IO.BinaryWriter mBinaryW;
            try
            {
                int i = szPath.LastIndexOf('\\');
                string dirName = szPath.Substring(0, i);
                //string fileName = szPath.Substring (i+1);
                //create directory
                DirectoryInfo objDirInf;
                objDirInf = new System.IO.DirectoryInfo(dirName);
                objDirInf.Create();

                //write
                mStreamW = System.IO.File.Create(szPath);
                mBinaryW = new System.IO.BinaryWriter(mStreamW);
                mBinaryW.Write(buff);
                mBinaryW.Close();
                mStreamW.Close();
                return true;
            }
            catch (System.UnauthorizedAccessException e)
            {
                return false;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        static public short TryConvertToShort(object ob)
        {
            string s = GetStringDB(ob);
            if (s.Length > 0)
                return TryConvertToShort(s);
            return 0;
        }

        static public short TryConvertToShort(string sText)
        {
            if (String.IsNullOrEmpty(sText))
            {
                return 0;
            }
            Int16 nRes = 0;
            if (Int16.TryParse(sText, out nRes))
            {
                return nRes;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Convert object to long
        /// </summary>
        /// <param name="ob">object to convert</param>
        /// <param name="defValue">default value</param>
        /// <returns>-1 if conversion fail</returns>
        static public long ConvertToLong(this object ob, long defValue = -1)
        {
            long result = defValue;
            string s = GetStringDB(ob);
            if (s.Length > 0)
                result = ConvertToLong(s);
            return result;
        }

        static public long ConvertToLong(string sText)
        {
            if (String.IsNullOrEmpty(sText))
            {
                return -1;
            }
            long nRes = -1;
            if (Int64.TryParse(sText, out nRes))
            {
                return nRes;
            }
            else
            {
                return -1;
            }
        }

        public static long? ConvertToLongOrNull(this object ob)
        {
            long result;

            if (long.TryParse(ob.ConvertToString(), out result))
                return result;

            return null;
        }

        public static string PrepareXml(string xml)
        {
            StringBuilder res = new StringBuilder(xml);
            res = res.Replace("&lt;", "<");
            res = res.Replace("&gt;", ">");
            res = res.Replace("&apos;", "\"");
            return res.ToString();
        }

        public static string GetXml(string xml)
        {
            StringBuilder res = new StringBuilder(xml);
            res = res.Replace("\"", "&apos;");
            res = res.Replace(">", "&gt;");
            res = res.Replace("<", "&lt;");
            return res.ToString();
        }

        public static int ConvertToInt(this object value, int defValue = 0, bool throwError = false)
        {
            int result = defValue;
            if (value.IsNullOrEmptyString())
            {
                if (throwError)
                {
                    throw new ArgumentException("Can not convert empty value to int");
                }
            }
            else
            {
                try
                {
                    result = Convert.ToInt32(value);
                }
                catch (Exception exc)
                {
                    if (throwError)
                    {
                        throw new ArgumentException("Can not convert '" + value.ConvertToString() + "' to int: " + exc.Message);
                    }
                }
            }
            return result;
        }

        public static int? ConvertToIntOrNull(this object ob)
        {
            int result;

            if (int.TryParse(ob.ConvertToString(), out result))
                return result;

            return null;
        }

        static public double ConvertToDouble(this object ob)
        {
            double res = 0;
            string sNumber = "";

            string dec = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            try
            {
                if (ob != null && ob != DBNull.Value)
                {
                    sNumber = ob.ToString();
                    if (sNumber.IndexOf(dec) == -1)
                    {
                        sNumber = sNumber.Replace(".", dec);
                        sNumber = sNumber.Replace(",", dec);
                    }
                    res = Convert.ToDouble(sNumber);
                }
            }
            catch
            {
                res = 0;
            }
            return res;
        }

        static public decimal ConvertToDecimal(this object ob)
        {
            decimal res;
            return TryConvertToDecimal(ob, out res) ? res : 0;
        }

        static public bool TryConvertToDouble(object ob, out double d)
        {
            bool res = false;
            string sNumber = "";
            d = 0.0;

            string dec = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (ob != null && ob != DBNull.Value)
            {
                sNumber = ob.ToString();
                if (sNumber.IndexOf(dec) == -1)
                {
                    sNumber = sNumber.Replace(".", dec);
                    sNumber = sNumber.Replace(",", dec);
                }
                res = double.TryParse(sNumber, out d);
            }
            return res;
        }

        static public bool TryConvertToDecimal(object ob, out decimal d)
        {
            bool res = false;
            string sNumber = "";
            d = 0.0m;

            string dec = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (ob != null && ob != DBNull.Value)
            {
                sNumber = ob.ToString();
                if (sNumber.IndexOf(dec) == -1)
                {
                    sNumber = sNumber.Replace(".", dec);
                    sNumber = sNumber.Replace(",", dec);
                }
                res = decimal.TryParse(sNumber, out d);
            }
            return res;
        }

        static public bool TryConvertToDouble(object ob, out double d, out string sign)
        {
            string sNumber = string.Empty;
            sign = string.Empty;
            if (ob != null && ob != DBNull.Value)
            {
                sNumber = ob.ToString();
                if (sNumber.StartsWith("<") || sNumber.StartsWith(">"))
                {
                    sign = sNumber.Substring(0, 1);
                    sNumber = sNumber.Substring(1, sNumber.Length - 1);
                }
            }
            return Tools.TryConvertToDouble(sNumber, out d);
        }

        public static string FormatShortcutMenuPath(string entry, string entryName)
        {
            return string.Format("[{0}][{1}]", entry, entryName);
        }

        static public bool GetLogicValue(object obj)
        {
            bool ret = false;
            string value = GetStringDB(obj);
            if (StringUtils.IsNotBlank(value))
            {
                ret = "Y".Equals(value);
            }
            return ret;
        }

        static public bool TryConvertToDecimal(object ob, out decimal d, out string sign)
        {
            string sNumber = string.Empty;
            sign = string.Empty;
            if (ob != null && ob != DBNull.Value)
            {
                sNumber = ob.ToString();
                if (sNumber.StartsWith("<") || sNumber.StartsWith(">"))
                {
                    sign = sNumber.Substring(0, 1);
                    sNumber = sNumber.Substring(1, sNumber.Length - 1);
                }
            }
            return Tools.TryConvertToDecimal(sNumber, out d);
        }

        static public bool ConvertToDateTime(ref System.DateTime objData, string sText)
        {
            if (!String.IsNullOrEmpty(sText))
            {
                try
                {
                    objData = System.DateTime.Parse(sText,
                            System.Globalization.CultureInfo.CurrentCulture,
                        System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                    return true;
                }
                catch (System.ArgumentNullException)
                {
                }
                catch (System.FormatException)
                {
                }
            }
            return false;
        }

        static public string ConvertListStringToString(this List<string> listString, string seperator)
        {
            string ret = "";
            foreach (string text in listString)
            {
                if (string.IsNullOrEmpty(ret))
                    ret = text.ToString();
                else
                    ret = ret + seperator + text.ToString();
            }
            return ret;
        }

        static public string ConvertListStringToString(this List<string> listString, string seperator, Func<string, string> predicate)
        {
            string ret = "";
            foreach (string text in listString)
            {
                if (string.IsNullOrEmpty(ret))
                    ret = predicate(text.ToString());
                else
                    ret = ret + seperator + predicate(text.ToString());
            }
            return ret;
        }

        static public string ConvertListLongToString(List<long> listLong, string seperator)
        {
            string ret = "";
            foreach (long text in listLong)
            {
                if (string.IsNullOrEmpty(ret))
                    ret = text.ToString();
                else
                    ret = ret + seperator + text.ToString();
            }
            return ret;
        }

        static public string ConvertListObjectToString(List<object> listLong, string seperator, string itemSeparator)
        {
            string ret = "";
            foreach (object text in listLong)
            {
                string _text = text.ToString();

                if (StringUtils.IsNotEmpty(itemSeparator))
                    _text = itemSeparator + _text + itemSeparator;

                if (ret == "")
                    ret = _text.ToString();
                else
                    ret = ret + seperator + _text.ToString();
            }
            return ret;
        }

        static public bool ConvertToDateTime(ref System.DateTime objData, string sText, string sFormat)
        {
            try
            {
                objData = System.DateTime.ParseExact(sText, sFormat, new System.Globalization.DateTimeFormatInfo());
                return true;
            }
            catch (System.ArgumentNullException)
            {
            }
            catch (System.FormatException)
            {
            }
            return false;
        }

        static public object ConvertToDBValue(object value)
        {
            return value != null ? value : DBNull.Value;
        }

        static public string ConvertToString(this object data)
        {
            if (data != null)
            {
                return data.ToString();
            }
            return string.Empty;
        }

        static public string ConvertToStringOrNull(this object data)
        {
            if (data == null)
                return null;

            string result = data.ToString();

            if (result.Length > 0)
            {
                return result;
            }
            return null;
        }

        static public decimal? ConvertPercentValueToDecimalOrNull(this object data)
        {
            decimal? dValue = null;

            if (data != null && data != DBNull.Value && !string.IsNullOrEmpty(data.ToString()))
            {
                string sValue = data.ToString().Replace("%", "");
                dValue = Tools.ConvertToDecimalOrNull(sValue);
            }
            return dValue;
        }

        static public decimal? ConvertToDecimalOrNull(this object data)
        {
            if (data != null && data != DBNull.Value && !string.IsNullOrEmpty(data.ToString()))
            {
                decimal ret;
                if (decimal.TryParse(data.ToString(), out ret))
                    return ret;
            }
            return null;
        }

        static public double? ConvertToDoubleOrNull(this object data)
        {
            double d = 0;
            if (TryConvertToDouble(data, out d))
                return d;
            else
                return null;
        }

        static public bool ConvertToBool(this object data)
        {
            bool ret = false;
            if (data != null)
            {
                Boolean.TryParse(data.ToString(), out ret);
            }
            return ret;
        }

        /// <param name="bConv"></param>	- converted value
        /// <param name="sText"></param>	- text to convert
        /// <returns></returns>				- if false conversion wasn't posible
        static public bool ConvertToBoolean(ref bool bConv, string sText)
        {
            try
            {
                bConv = Convert.ToBoolean(sText);
                return true;
            }
            catch (System.ArgumentNullException)
            {
            }
            catch (System.FormatException)
            {
            }
            return false;
        }

        static public DataTable String2DataTable(string sList, params string[] sColumns)
        {
            DataTable dtTab = new DataTable();
            if (sColumns.Length == 0)
                sColumns = new string[] { "Code", "Name" };

            foreach (string colName in sColumns)
                dtTab.Columns.Add(colName);

            string[] sValues = sList.Split('|');

            for (int i = 0; i < sValues.Length; i += sColumns.Length)
            {
                DataRow row = dtTab.NewRow();
                for (int j = 0; j < sColumns.Length && i + j < sValues.Length; j++)
                {
                    row[j] = sValues[i + j];
                }
                dtTab.Rows.Add(row);
            }
            return dtTab;
        }

        static public void AddSubString(ref string sText, string sSubString)
        {
            AddSubString(ref sText, sSubString, "|");
        }

        static public void AddSubString(ref string sText, string sSubString, string separator)
        {
            string sRes = GetValidSubstring(sSubString, separator);
            sText += sRes;
            sText += separator;
        }

        static private string GetValidSubstring(string subString, string separator)
        {
            string res = subString.Replace("/", "//");
            if (!string.IsNullOrEmpty(separator))
            {
                res = res.Replace(separator, "/" + separator);
            }
            return res;
        }

        static public void AddSubString(StringBuilder textBuffer, string subString)
        {
            AddSubString(textBuffer, subString, "|");
        }

        static public void AddSubString(StringBuilder textBuffer, string subString, string separator)
        {
            if (string.IsNullOrEmpty(subString))
            {
                return;
            }
            string res = GetValidSubstring(subString, separator);
            if (textBuffer.Length > 0 && !string.IsNullOrEmpty(separator))
            {
                textBuffer.Append(separator);
            }
            textBuffer.Append(res);
        }

        static public string RemoveChars(this string str, IEnumerable<char> toExclude)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (!toExclude.Contains(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Used to create compound string
        /// </summary>
        /// <param name="subStrings">An array of strings</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        static public string CreateCompoundString(this string[] subStrings, string separator = "|")
        {
            string res = string.Empty;
            if (null != subStrings && subStrings.Length > 0)
            {
                foreach (string subString in subStrings)
                {
                    AddSubString(ref res, subString, separator);
                }
            }
            return res;
        }

        static public string CreateCompoundString(long[] subObjects)
        {
            string res = String.Empty;
            if (subObjects != null)
            {
                foreach (long obj in subObjects)
                    AddSubString(ref res, obj.ToString());
            }
            return res;
        }

        static public string FormatCustomString(string subStrings, char[] separator, int count, String ends)
        {
            StringBuilder res = new StringBuilder();
            int i = 0;
            String[] tabString = subStrings.Split(separator);

            foreach (String str in tabString)
            {
                if (i < count)
                {
                    if (res.Length != 0)
                        res.Append(", ");
                    res.Append(tabString[i]);
                    i++;
                }
                else
                {
                    res.Append(ends);
                    break;
                }
            }
            return res.ToString();
        }

        static public string ChangeDateOnVectorDate(string sDate)
        {
            //MM/DD/YYYY | YYYY/MM/DD
            int user_day, user_month, user_year, tmp_day;
            int curr_day = DateTime.Now.Day;
            int curr_month = DateTime.Now.Month;
            int curr_year = DateTime.Now.Year;

            string sDate_ = sDate;

            sDate = sDate.Replace("//", "/");
            DateTime dat;
            if (!DateTime.TryParse(sDate, out dat))
                return sDate;

            int nPos = sDate.IndexOf("/");
            if (nPos == -1)
            {
                nPos = sDate.IndexOf("-");
            }

            try
            {
                user_month = DBTools.ConvertToInt(sDate.Substring(0, nPos));
                if (user_month <= 0)
                    return sDate_;

                sDate = sDate.Substring(nPos + 1);

                nPos = sDate.IndexOf("/");
                if (nPos == -1)
                {
                    nPos = sDate.IndexOf("-");
                }

                user_day = DBTools.ConvertToInt(sDate.Substring(0, nPos));
                if (user_day <= 0)
                    return sDate_;

                sDate = sDate.Substring(nPos + 1);

                user_year = DBTools.ConvertToInt(sDate);
                if (user_year <= 0)
                    return sDate_;
            }
            catch (Exception)
            {
                return sDate_;
            }

            if (user_month > 12)
            {
                tmp_day = user_year;
                user_year = user_month;
                user_month = user_day;
                user_day = tmp_day;
            }

            user_day = user_day - curr_day;
            user_month = user_month - curr_month;
            user_year = user_year - curr_year;
            user_month = user_month + (user_year * 12);

            sDate = "Vec(" + user_day.ToString() + "," + user_month.ToString() + ")";

            return sDate;
        }

        static public string RetriveSubString(ref string sText)
        {
            return RetriveSubString(ref sText, "|");
        }

        static public string RetriveSubString(ref string sText, string separator)
        {
            return RetriveSubString(ref sText, separator, '/');
        }

        static public string RetriveSubString(ref string sText, string separator, char escapeCharacter)
        {
            string sRes;
            int nPos;
            string escChar = Convert.ToString(escapeCharacter);

            sRes = "";
            nPos = sText.IndexOf(separator);
            string dblEscChar = escChar + escChar;

            while (nPos > 0 && sText.Substring(0, nPos).Replace(dblEscChar, "").EndsWith(escChar))
            {
                nPos = sText.IndexOf(separator, nPos + separator.Length);
            }
            if (nPos != -1)
            {
                sRes = sText.Substring(0, nPos);
                sText = sText.Substring(nPos + separator.Length);
            }
            else
            {
                sRes = sText;
                sText = "";
            }
            sRes = sRes.Replace(dblEscChar, escChar);
            sRes = sRes.Replace(escChar + separator, separator);
            return sRes;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sText"></param>
        /// <param name="maxArrayLength">The maximum length of resulting array. If param value 0 or negative, then array will be contains no more one item.</param>
        /// <returns></returns>
        static public String[] RetriveSubStringTab(this string sText, int maxArrayLength = int.MaxValue)
        {
            return RetriveSubStringTab(sText, "|", maxArrayLength);
        }

        static public String[] RetriveSubStringTab(this string sText, string separator, int maxArrayLength = int.MaxValue)
        {
            List<String> list = new List<string>();
            if (sText != null)
            {
                int cnt = 0;
                while (sText.Length > 0)
                {
                    list.Add(RetriveSubString(ref sText, separator));
                    cnt++;

                    if (cnt >= maxArrayLength)
                        break;
                }
            }
            return list.ToArray();
        }

        static public string TryGetSubStringByIndex(this string val, int index, string defaultVal = "")
        {
            string[] retrStr = RetriveSubStringTab(val, index + 1);
            return retrStr.Length > index ? retrStr[index] : defaultVal;
        }

        static public int GetSubStringNum(string sText)
        {
            int num = 0;
            int nPos = sText.IndexOf("|");
            while (nPos > 0 && sText[nPos - 1] != '/')
            {
                num++;
                nPos = sText.IndexOf("|", nPos + 1);
            }
            return num;
        }

        public static string AddWord(string sSentence, string sWord, string sSeparator)
        {
            string sRet = string.Copy(sSentence);
            if (sRet.Length > 0)
                sRet += sSeparator;
            sRet += sWord;
            return sRet;
        }

        static public string FormatPipedString(string sPipedString, string sSeparator)
        {
            string sRet = string.Empty;
            string sTmp = string.Empty;

            while (sPipedString.Length > 0)
            {
                sTmp = Tools.RetriveSubString(ref sPipedString);
                if (sTmp.Length > 0)
                {
                    if (sRet.Length > 0)
                        sRet += sSeparator;
                    sRet += sTmp;
                }
            }
            return sRet;
        }

        /// <summary>
        /// Returns string from piped string.
        /// </summary>
        /// <param name="sPipedString">piped string</param>
        /// <param name="sSeparator">sign put between words</param>
        /// <param name="iNumCodes">number of words</param>
        /// <param name="sEndSign">sign put at the end when some words were cutted</param>
        /// <returns></returns>
        static public string FormatPipedString(string sPipedString, string sSeparator, int iNumCodes, string sEndSign)
        {
            StringBuilder sRet = new StringBuilder();
            string sTmp = string.Empty;
            int counter = 0;

            while (sPipedString.Length > 0)
            {
                sTmp = Tools.RetriveSubString(ref sPipedString);
                if (sTmp.Length > 0)
                {
                    if (counter < iNumCodes)
                    {
                        if (sRet.Length > 0)
                        {
                            sRet.Append(sSeparator);
                        }
                        sRet.Append(sTmp);
                    }
                    else if (sRet.Length > 0)
                    {
                        sRet.Append(sSeparator).Append(sEndSign);
                        break;
                    }
                    counter++;
                }
            }
            return sRet.ToString();
        }

        static public void PrepareMemoEdit(DataRow dr, string c)
        {
            dr.BeginEdit();
            string toConvert = dr[c].ToString();

            StringBuilder builder = new StringBuilder(Convert.ToInt32(toConvert.Length * 1.1));
            // marker if last read char was CR
            bool prevCR = false;
            char ch;
            // convert all occurences of single new line char '\n'
            // or single cariage return char '\r' to sequence \r\n
            for (int i = 0; i < toConvert.Length; ++i)
            {
                ch = toConvert[i];
                if (prevCR && ch != '\n')
                {
                    builder.Append('\n');
                }
                if (!prevCR && ch == '\n')
                {
                    builder.Append('\r');
                }
                builder.Append(ch);
                prevCR = ch == '\r';
            }
            if (prevCR)
            {
                builder.Append('\n');
            }
            dr[c] = builder.ToString();
            dr.AcceptChanges();
        }

        /// <summary>
        /// Returns string from datasets field, when is db_null empty string is returned
        /// </summary>
        /// <param name="dbString"></param>
        /// <returns></returns>
        static public string GetStringDB(object dbString)
        {
            string sRet = "";

            if ((dbString != null) && !Convert.IsDBNull(dbString))
                sRet = dbString.ToString();
            return sRet;
        }

        static public string GetFormatedDate(string sDate, string sFormat)
        {
            string sRet = string.Empty;
            DateTime dtTmp = DateTime.Now;

            if (sDate != string.Empty)
            {
                if (Tools.ConvertToDateTime(ref dtTmp, sDate))
                    sRet = dtTmp.ToString(sFormat);
            }
            return sRet;
        }

        static public string GetFormatedDateList(string sDate, string sFormat)
        {
            string sRet = string.Empty;
            DateTime dtTmp = DateTime.Now;
            char separator = ',';

            if (sDate != string.Empty)
            {
                if (sDate.Contains(separator.ToString()))
                {
                    string[] tabDate = sDate.Split(separator);

                    foreach (string date in tabDate)
                    {
                        if (Tools.ConvertToDateTime(ref dtTmp, date))
                        {
                            sRet = StringUtils.AppendComma(sRet);
                            sRet += dtTmp.ToString(sFormat);
                        }
                    }
                }
                else
                {
                    sRet = GetFormatedDate(sDate, sFormat);
                }
            }
            return sRet;
        }

        /// <summary>
        /// Returns string from object, when is empty or DBNull empty string is returned
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sFormat">A format string.
        /// d :08/17/2000
        /// D :Thursday, August 17, 2000
        /// f :Thursday, August 17, 2000 16:32
        /// F :Thursday, August 17, 2000 16:32:32
        /// g :08/17/2000 16:32
        /// G :08/17/2000 16:32:32
        /// m :August 17
        /// r :Thu, 17 Aug 2000 23:32:32 GMT
        /// s :2000-08-17T16:32:32
        /// t :16:32
        /// T :16:32:32
        /// u :2000-08-17 23:32:32Z
        /// U :Thursday, August 17, 2000 23:32:32
        /// y :August, 2000
        /// dddd, MMMM dd yyyy :Thursday, August 17 2000
        /// ddd, MMM d "'"yy :Thu, Aug 17 '00
        /// dddd, MMMM dd :Thursday, August 17
        /// M/yy :8/00
        /// dd-MM-yy :17-08-00
        /// </param>
        /// <returns></returns>
        static public string GetStringDate(object obj, string sFormat)
        {
            string sRet = string.Empty;
            DateTime dt;

            try
            {
                if (obj != null && obj != DBNull.Value && !obj.ToString().IsNullOrEmptyString())
                {
                    dt = Convert.ToDateTime(obj);
                    sRet = dt.ToString(sFormat);
                }
            }
            catch (InvalidCastException) { }
            return sRet;
        }

        /// <summary>
        /// Gets formated Gestation Age ( Weeks: ..., Days: ... ) from
        /// string that can be convert to double val
        /// </summary>
        /// <param name="gestAge"></param>
        /// <returns></returns>
        static public string GetFormatedGestAge(string gestAge)
        {
            string returnStr = "";
            if (gestAge != null && !"".Equals(gestAge) && !"0".Equals(gestAge))
            {
                int weeks = 0;
                int days = 0;

                double gestAge_D;
                if (TryConvertToDouble(gestAge, out gestAge_D))
                {
                    weeks = (int)gestAge_D;
                    double decPart_D = gestAge_D - ((double)weeks);
                    if (decPart_D > 0)
                    {
                        int days_I = (int)(decPart_D / 0.1f);
                        if (days_I > 0)
                        {
                            double percDays_D = ((double)days_I) / 10.0f;
                            days = (int)(percDays_D * 7.0f);
                        }
                    }
                }
                returnStr = "Weeks: " + weeks.ToString() + ", days: " + days.ToString();
            }
            return returnStr;
        }

        /// <summary>
        /// Gets formated name ( LName, FName MName ) from
        /// string ( LName|FName|MName )
        /// </summary>
        /// <param name="sName"></param>
        /// <param name="sRole"></param>
        /// <returns></returns>
        static public string GetFormatedName(string sName, string sRole)
        {
            string sRet = string.Empty;
            char[] ctSep = { '|' };
            string[] stNames = sName.Split(ctSep);
            string sLast = string.Empty;
            string sFirst = string.Empty;
            string sMidle = string.Empty;

            if ((stNames == null) || (stNames.Length < 2))
            {
                return sRet;
            }
            sLast = stNames[0].Trim();
            sFirst = stNames[1].Trim();
            if (stNames.Length > 2)
            {
                sMidle = stNames[2].Trim();
                if (sMidle.Length == 1)
                    sMidle += ".";
            }
            switch (sRole)
            {
                case "L_COMMA_FM":
                case "PATNAME":
                    if (sLast.Length > 0)
                        sRet += sLast + ", ";
                    if (sFirst.Length > 0)
                        sRet += sFirst + " ";
                    if (sMidle.Length > 0)
                        sRet += sMidle;
                    break;

                case "REQNAME":
                    if (sFirst.Length > 0)
                        sRet += sFirst + " ";
                    if (sMidle.Length > 0)
                        sRet += sMidle + " ";
                    if (sLast.Length > 0)
                        sRet += sLast;
                    break;

                case "REQDOC":
                    if (sLast.Length > 0)
                        sRet += sLast;
                    if (sFirst.Length > 0)
                        sRet += " " + sFirst;
                    break;

                default:
                    if (sLast.Length > 0)
                        sRet += sLast + ", ";
                    if (sFirst.Length > 0)
                        sRet += sFirst + " ";
                    break;
            }
            return sRet;
        }

        /// <summary>
        /// Function forms name of doctor.
        /// </summary>
        /// <param name="lname">last name</param>
        /// <param name="fname">first name</param>
        /// <param name="mname">middle initial</param>
        /// <returns>string containing whole name of doctor</returns>
        public static string GetFormatedDoctorName(object lname, object fname, object mname)
        {
            return string.Format("{0}, {1} {2}", lname, fname, mname).Trim();
        }

        /// <summary>
        /// Function forms name of representative doctor.
        /// </summary>
        /// <param name="lname">last name</param>
        /// <param name="fname">first name</param>
        /// <param name="mname">middle initial</param>
        /// <returns>string containing whole name of the representative doctor</returns>
        public static string GetFormatedRepresentativeDoctorName(object lname, object fname, object mname)
        {
            return string.Format("{0} {1} {2}",
                null != fname ? fname.ToString() : "",
                null != mname ? mname.ToString() : "",
                null != lname ? lname.ToString() : ""
                ).Trim();
        }

        static public string GetRhName(string RhCode)
        {
            string sRet = string.Empty;
            switch (RhCode)
            {
                case "P":
                    sRet = "POS";
                    break;

                case "N":
                    sRet = "NEG";
                    break;

                default:
                    sRet = RhCode;
                    break;
            }
            return sRet;
        }

        /// <summary>
        /// Replace ilegal char in file name
        /// </summary>
        /// <param name="inputstr">Input String</param>
        /// <param name="newValue">New value</param>
        /// <returns></returns>
        static public string ReplaceIlegalIOChar(string inputstr, string newValue)
        {
            string tempName = inputstr;
            string[] ilegalchars = new string[] {@"\", @"/", @":", @"*", @"?",
                                                 @"<", @">", @"|", "\""};
            if (!tempName.Trim().Equals(string.Empty))
            {
                foreach (string oldvalue in ilegalchars)
                {
                    tempName = tempName.Replace(oldvalue, newValue);
                }
            }
            return tempName;
        }

        static public string FormatExportFileName(string sCaseNumber, string sCode)
        {
            return Tools.ReplaceIlegalIOChar(@"FRE_" + sCaseNumber + "_I_" + sCode, "_");
        }

        //		-------------------------------------------------------------------------------------
        //								Parsing Karyotype string
        //		-------------------------------------------------------------------------------------

        public class FormatterKeyPad : IFormatProvider, ICustomFormatter
        {
            private Dictionary<string, string> keyPadCodes = new Dictionary<string, string>();

            public FormatterKeyPad()
            {
                keyPadCodes.Add("Divide", "NumPad/ ");
                keyPadCodes.Add("Multiply", "NumPad* ");
                keyPadCodes.Add("Subtract", "NumPad- ");
                keyPadCodes.Add("Add", "NumPad+ ");
            }

            public object GetFormat(System.Type type)
            {
                return this;
            }

            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                if (arg != null)
                {
                    string formatValue = arg.ToString();
                    return Format(formatValue);
                }
                return string.Empty;
            }

            public string Format(string arg)
            {
                string result = arg;
                if (keyPadCodes.ContainsKey(arg))
                {
                    result = keyPadCodes[arg];
                }
                return result;
            }
            public bool IsCorrectKeyPadValue(string value)
            {
                bool result = false;
                string tmp;
                if (value.Length == 1)
                {
                    if (char.IsLetterOrDigit(value, 0)
                       || value == "-" || value == "+"
                       || value == "*" || value == "/")
                    {
                        result = true;
                    }
                }
                else if (value.StartsWith("NumPad"))
                {
                    tmp = value.Replace("NumPad", string.Empty);
                    if (tmp.Length == 1 && char.IsDigit(tmp, 0))
                    {
                        result = true;
                    }
                }
                else if (value.StartsWith("F") && value.Length > 1)
                {
                    result = true;
                }
                else
                {
                    result = keyPadCodes.ContainsKey(value);
                }
                return result;
            }
        }

        static private string DuplicateNTimes(char cDuplicated, int N)
        {
            string sRet = string.Empty;

            for (int i = 0; i < N; i++)
                sRet += cDuplicated;
            return sRet;
        }

        /// <summary>
        /// It gets gender chromosome in alphanumeric order from karyotype string
        /// </summary>
        /// <param name="sKaryotype"></param>
        /// <returns></returns>
        static public string GetGenderChromosome(string sKaryotype)
        {
            string sGender = string.Empty;
            int nIdx = -1;
            int nXCnt = 0;
            int nYCnt = 0;

            if (sKaryotype != null)
            {
                nIdx = sKaryotype.IndexOf(',');
                if (nIdx > -1)
                {
                    nIdx++; //Get after comma
                    while (nIdx < sKaryotype.Length)
                    {
                        //Don't get white spaces
                        if (!Char.IsWhiteSpace(sKaryotype[nIdx]))
                        {
                            if (Char.ToUpper(sKaryotype[nIdx]).Equals('X'))
                                nXCnt++;
                            else if (Char.ToUpper(sKaryotype[nIdx]).Equals('Y'))
                                nYCnt++;
                            else  //If other sign than X or Y - end loop
                                break;
                        }
                        nIdx++;
                    }
                }
            }
            sGender = DuplicateNTimes('X', nXCnt);
            sGender += DuplicateNTimes('Y', nYCnt);
            return sGender;
        }

        public static string OrderShortcut(string shortcut)
        {
            string sRet = "";
            string[] shortcutTab = shortcut.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            if (!string.IsNullOrEmpty(shortcut))
            {
                for (int i = 0; i < shortcutTab.Length; i++)
                {
                    if (shortcut.Contains("ALT"))
                    {
                        sRet += "ALT+";
                        shortcut = shortcut.Replace("ALT", "");
                    }
                    else if (shortcut.Contains("CTRL"))
                    {
                        sRet += "CTRL+";
                        shortcut = shortcut.Replace("CTRL", "");
                    }
                    else if (shortcut.Contains("SHIFT"))
                    {
                        sRet += "SHIFT+";
                        shortcut = shortcut.Replace("SHIFT", "");
                    }
                    else
                    {
                        sRet += shortcut.Replace("+", "");
                    }
                }
            }
            return sRet;
        }

        static public bool IsUsedProperGenderWord(string sComment, string sChrGender)
        {
            bool bProperGenderFound = true;
            char[] ctSeparators = { ' ', '\n', '\r', '\t', ',', '.', ';', '?', '!', ':', '-', '(', ')', '{', '}', '[', ']' };
            string[] stCommentWords = null;

            if (sChrGender != null)
            {
                bProperGenderFound = false;
                if (sChrGender.ToUpper().Equals("XX"))      //female's chromosome
                {
                    stCommentWords = sComment.Split(ctSeparators);
                    foreach (string sWord in stCommentWords)
                    {
                        if ((sWord != null) && (sWord.ToUpper().Equals("FEMALE")))
                        {
                            bProperGenderFound = true;
                            break;
                        }
                    }
                }
                else if (sChrGender.ToUpper().Equals("XY") ||
                    sChrGender.ToUpper().Equals("YX")) //male's chromosome
                {
                    stCommentWords = sComment.Split(ctSeparators);
                    foreach (string sWord in stCommentWords)
                    {
                        if ((sWord != null) && (sWord.ToUpper().Equals("MALE")))
                        {
                            bProperGenderFound = true;
                            break;
                        }
                    }
                }
                else
                    bProperGenderFound = true;
            }
            return bProperGenderFound;
        }

        //		-------------------------------------------------------------------------------
        //											RTF Tools
        //		-------------------------------------------------------------------------------
        static private string GetFormattedText(string sRTF)
        {
            string sRet = string.Empty;
            int nStartIdx = 0;
            int nEndIdx = 0;

            if (sRTF == null)
                return sRet;
            nStartIdx = sRTF.IndexOf("\\par");
            if (nStartIdx >= 0)
            {
                nEndIdx = sRTF.LastIndexOf('}');
                if ((nEndIdx > 0) && (sRTF[nEndIdx - 1] != '\\'))
                    sRet = sRTF.Substring(nStartIdx, nEndIdx - nStartIdx);
                else
                    sRet = sRTF.Substring(nStartIdx);
            }
            return sRet;
        }

        static private void UpdateBraces(string sRTF)
        {
            int nCount = 0;
            if (sRTF == null)
                return;
            for (int i = 0; i < sRTF.Length; i++)
            {
                if ((sRTF[i] == '{') &&
                    ((i == 0) || ((i > 0) && (sRTF[i] != '\\'))))
                    nCount++;
                else if ((sRTF[i] == '}') &&
                    ((i == 0) || ((i > 0) && (sRTF[i] != '\\'))))
                    nCount--;
            }
            if (nCount > 0)
                while (nCount > 0)
                {
                    sRTF = sRTF.Insert(sRTF.Length - 1, "}");
                    nCount--;
                }
            else if (nCount < 0)
                while (nCount < 0)
                {
                    sRTF = sRTF.Insert(0, "{");
                    nCount++;
                }
        }

        static private string GetSpecialSigns(string sRTF, int index)
        {
            string sRet = string.Empty;

            if ((sRTF != null) && (index > 0) && (sRTF.Length > index + 2))
            {
                sRet = "<0x";
                sRet += sRTF.Substring(index + 1, 2);
                sRet += ">";
            }
            return sRet;
        }

        /// <summary>
        /// Merges two RTF documents to one
        /// </summary>
        /// <param name="sRTF1"></param>
        /// <param name="sRTF2"></param>
        /// <returns></returns>
        static public string MergeRTFs(string sRTF1, string sRTF2)
        {
            string sRet = string.Empty;
            string sBuff = string.Empty;
            int nPastePos = 0;

            if ((sRTF1 == null) || (sRTF2 == null))
                return sRet;
            nPastePos = sRTF1.LastIndexOf("}");
            if (nPastePos > 0 && sRTF1[nPastePos - 1] != '\\')
            {
                sRet = sRTF1.Substring(0, nPastePos);
                sBuff = GetFormattedText(sRTF2);
                UpdateBraces(sBuff);
                sRet += sBuff;
                sRet += sRTF1.Substring(nPastePos, sRTF1.Length - nPastePos);
            }
            return sRet;
        }

        /// <summary>
        /// Adds rtf formatted text to the beggining of the text in rtfBox.
        /// </summary>
        /// <param name="richTextBox"></param>
        /// <param name="appendText"></param>
        /// <param name="appendBefore">append before this text (plain text, not rtf)</param>
       
        /// <summary>
        /// Gets Simple text from RTF
        /// </summary>
        /// <param name="rtf"></param>
        /// <param name="Token_ObjStart"></param>
        /// <param name="Token_ObjEnd"></param>
        /// <returns></returns>
        static public string RemoveRtfObjects(string rtf, string Token_ObjStart, string Token_ObjEnd)
        {
            if (!rtf.Contains(Token_ObjStart))
                return rtf;

            int length = rtf.Length;
            StringBuilder builder = new StringBuilder();
            bool objTokenFound = false;
            for (int i = 0; i < length; i++)
            {
                if (objTokenFound)
                {
                    if (MatchToken(i, Token_ObjEnd, rtf))
                        objTokenFound = false;
                }
                else
                {
                    if (MatchToken(i, Token_ObjStart, rtf))
                        objTokenFound = true;
                    else
                        builder.Append(rtf[i]);
                }
            }
            return builder.ToString();
        }

        private static bool MatchToken(int pos, string token, string source)
        {
            for (int i = 0; i < source.Length - pos && i < token.Length; i++)
            {
                if (source[i + pos] != token[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Gets Simple text from RTF
        /// </summary>
        /// <param name="sRTF"></param>
        /// <returns></returns>
        static public string GetSimpleTextRTF(string sRTF)
        {
            if (string.IsNullOrEmpty(sRTF))
                return string.Empty;

            const string Token_ImgStart = "{\\pict\\";
            const string Token_ImgEnd = "}";
            sRTF = RemoveRtfObjects(sRTF, Token_ImgStart, Token_ImgEnd);

            const string Token_BkmStartBegin = @"{\*\bkmkstart";
            const string Token_BkmStartEnd = "}";
            sRTF = Tools.RemoveRtfObjects(sRTF, Token_BkmStartBegin, Token_BkmStartEnd);

            const string Token_BkmEndBegin = @"{\*\bkmkend";
            const string Token_BkmEndLast = "}";
            sRTF = Tools.RemoveRtfObjects(sRTF, Token_BkmEndBegin, Token_BkmEndLast);

            const string Token_HyperlinkStart = @"{\*\fldinst";
            const string Token_HyperlinkEnd = "}";
            sRTF = Tools.RemoveRtfObjects(sRTF, Token_HyperlinkStart, Token_HyperlinkEnd);

            StringBuilder sRet = new StringBuilder();
            string sTmp = string.Empty;
            char cPrev = '\0';
            bool bIsText = false;

            if (sRTF == null)
                return sRet.ToString();
            sTmp = GetFormattedText(sRTF);
            //For TextControl markers
            if (sTmp != null && !sTmp.Equals(""))
            {
                sTmp = sTmp.Replace('\n', ' ');
                sTmp = sTmp.Replace('\r', ' ');
                sTmp = sTmp.Replace("\\txfielddata ", "\\txfielddata");
                sTmp = sTmp.Replace("\\par ", "\\par\n");
                sTmp = sTmp.Replace(@"\u8204 ?", " ");
            }
            for (int i = 0; i < sTmp.Length; i++)
            {
                switch (sTmp[i])
                {
                    case '\\':  //backslash
                        if ((cPrev == '\\') && (bIsText == false))
                        {
                            sRet.Append(sTmp[i]);
                            bIsText = true;
                        }
                        else
                            bIsText = false;
                        break;

                    case '{':   //opening brace
                    case '}':   //closing brace
                        if ((cPrev == '\\') && (bIsText == false))
                            sRet.Append(sTmp[i]);
                        bIsText = true;
                        break;

                    case ' ':   //white spaces
                        if (bIsText == true)
                            sRet.Append(sTmp[i]);
                        else
                            bIsText = true;
                        break;

                    case '\t':
                    case '\n':
                    case '\r':
                        sRet.Append(sTmp[i]);
                        bIsText = true;
                        break;

                    case '-':
                    case '_':
                        if ((cPrev == '\\') && (bIsText == false))
                        {
                            sRet.Append(sTmp[i]);
                            bIsText = true;
                        }
                        else if (bIsText == true)
                            sRet.Append(sTmp[i]);
                        break;

                    case '\'':  //special signs
                        if ((cPrev == '\\') && (bIsText == false))
                        {
                            sRet.Append(GetSpecialSigns(sTmp, i));
                            bIsText = true;
                            i += 2;
                        }
                        else if (bIsText == true)
                            sRet.Append(sTmp[i]);
                        break;

                    case '~':   //space
                        if ((cPrev == '\\') && (bIsText == false))
                        {
                            sRet.Append(" ");
                            bIsText = true;
                        }
                        else if (bIsText == true)
                            sRet.Append(sTmp[i]);
                        break;

                    default:
                        if (bIsText == true)    //simple text
                            sRet.Append(sTmp[i]);
                        break;
                }
                cPrev = sTmp[i];
            }
            return sRet.ToString();
        }

        /// <summary>
        /// Checks if specified string contain rtf text
        /// </summary>
        /// <param name="sRTF">string to check</param>
        /// <returns>true if string contains rtf</returns>
        static public bool IsRtfString(string sRTF)
        {
            bool isRtf = false;
            if (!string.IsNullOrEmpty(sRTF))
                isRtf = sRTF.StartsWith("{\\rtf1");
            return isRtf;
        }

        //		-------------------------------------------------------------------------------
        //									Numeric value handlers
        //		-------------------------------------------------------------------------------
        /*
         * Example:
         * Add EditValueChanging handler as below and call IsNewValueValidVolume
         * to know if new edit value is valid or not
         *
         *  ...
         *  textEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(OnEditValueChanging);
         *  ...
         *
         * 	private void OnEditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
         * 	{
         *		if (IsNewValueValid(e.NewValue.ToString(), 4, 2) == false)
         * 			e.Cancel = true;
         * 	}
         */

        // Checks if passed string is valid numeric value. This method takes number of
        // digits before and after decimal point into consideration.
        // Returns: true if string value is valid or false if it is not
        // Parameters:
        // newValue - string to be checked
        // beforeDecimalPoint - number of digits before decimal point
        // afterDecimalPoint - number of digits after decimal point
        //
        public static bool IsNewValueValid(string newValue, int beforeDecimalPoint, int afterDecimalPoint)
        {
            if (newValue == null || newValue.Length == 0)
                return true;

            try
            {
                Double.Parse(newValue);
            }
            catch
            {
                return false;
            }

            bool ret = true;
            string decimalPoint = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            // decimal point can't be frist character
            if (decimalPoint.IndexOf(newValue[0]) != -1)
                return false;

            int decimalPointIdx = newValue.IndexOf(decimalPoint);

            if (afterDecimalPoint == 0 && decimalPointIdx != -1)
            {
                // there is decimal point, but we want no digits after decimal point
                // so this value is invalid
                ret = false;
            }
            else if (decimalPointIdx != -1)
            {
                // there is decimal point
                if (newValue.LastIndexOf(decimalPoint) != decimalPointIdx)
                {
                    // there is more than one decimal point, so this value is invalid
                    ret = false;
                }
                else
                {
                    if (newValue.Length > 2 && newValue[0] == '0' && (decimalPoint.IndexOf(newValue[1]) == -1))
                        ret = false;
                    else if (newValue.Substring(0, decimalPointIdx).Length > beforeDecimalPoint)
                        ret = false;
                    else if (newValue.Substring(decimalPointIdx + 1).Length > afterDecimalPoint)
                        ret = false;
                }
            }
            else
            {
                //string does not contain decimal point, max length is beforeDecimalPoint
                if (newValue.Length > beforeDecimalPoint)
                {
                    ret = false;
                }
                else
                {
                    if (newValue.Length > 1 && newValue[0] == '0')
                        ret = false;
                }
            }
            return ret;
        }

        //		-------------------------------------------------------------------------------

        /// <summary>
        /// Checks if enum option is marked it is usefull when given option is keept on one bit
        /// </summary>
        /// <param name="enumOption">Option to check</param>
        /// <param name="mask">Checking mask</param>
        /// <returns></returns>
        public static bool IsMarkedEnumOption(int enumOption, int mask)
        {
            bool bRet = false;

            if ((enumOption & mask) == mask)
                bRet = true;
            return bRet;
        }

        public static List<string> CreateStringList(DataRow[] rows, string column)
        {
            List<string> list = new List<string>();
            if (rows != null && rows.Length > 0 && rows[0].Table.Columns.Contains(column))
            {
                foreach (DataRow row in rows)
                {
                    if (row.RowState != DataRowState.Deleted
                        && row.RowState != DataRowState.Detached
                        && StringUtils.IsNotEmpty(Tools.GetStringDB(row[column])))
                    {
                        list.Add(Tools.GetStringDB(row[column]));
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Creates sequence of numOfElements values from table separated by given separator.
        /// If there is more rows then numOfElements then postfix is appended at the end of result string.
        /// Ex. "A1, A2, A3, A4...", "T1-T2-T3-T4-T5"
        /// </summary>
        /// <param name="table">DataTable with data to be used</param>
        /// <param name="column">string with column name, which values will be used</param>
        /// <param name="numOfElements">number of elemens that should be in result string</param>
        /// <param name="separator">values separator</param>
        /// <param name="postfix">postfix to be appended at the end of the string</param>
        /// <returns>sequence of separated values from given field of the table with appended postfix</returns>
        public static string CreateSequenceString(DataTable table, string column, int numOfElements, string separator, string postfix)
        {
            DataRow[] rows = new DataRow[table.Rows.Count];
            table.Rows.CopyTo(rows, 0);

            return CreateSequenceString(rows, column, numOfElements, separator, postfix);
        }

        /// <summary>
        /// Creates sequence of numOfElements values from table separated by given separator.
        /// If there is more rows then numOfElements then postfix is appended at the end of result string.
        /// Ex. "A1, A2, A3, A4...", "T1-T2-T3-T4-T5"
        /// </summary>
        /// <param name="rows">DataRows[] with data to be used</param>
        /// <param name="column">string with column name, which values will be used</param>
        /// <param name="numOfElements">number of elemens that should be in result string</param>
        /// <param name="separator">values separator</param>
        /// <param name="postfix">postfix to be appended at the end of the string</param>
        /// <returns>sequence of separated values from given field of the table with appended postfix</returns>
        public static string CreateSequenceString(DataRow[] rows, string column, int numOfElements, string separator, string postfix)
        {
            string result = string.Empty;
            if (rows.Length > 0)
            {
                if (rows[0].Table.Columns.Contains(column))
                {
                    int count = Math.Min(rows.Length, numOfElements);
                    for (int i = 0; i < count; i++)
                    {
                        if (rows[i].RowState != DataRowState.Deleted && rows[i].RowState != DataRowState.Detached)
                        {
                            result += rows[i][column].ToString();
                            if (i != (count - 1))
                            {
                                result += separator;
                            }
                        }
                    }
                    if (count != rows.Length)
                    {
                        result += postfix;
                    }
                }
            }
            return result;
        }

        public static string WebServiceDatePattern
        {
            get { return "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffzzzzz"; }
        }

        public static string ConvertDateTimeToString(DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Unspecified) dt = new DateTime(dt.Ticks, DateTimeKind.Local);
            else if (dt.Kind == DateTimeKind.Utc)
                dt = dt.ToLocalTime();
            return XmlConvert.ToString(dt, WebServiceDatePattern);
        }

        public static string DataBaseDatePatternWithoutMiliseconds
        {
            get { return "yyyy'-'MM'-'dd'T'HH':'mm':'ss"; }
        }

        public static string ConvertDateTimeToDBString(DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Unspecified) dt = new DateTime(dt.Ticks, DateTimeKind.Local);
            else if (dt.Kind == DateTimeKind.Utc)
                dt = dt.ToLocalTime();
            return XmlConvert.ToString(dt, DataBaseDatePatternWithoutMiliseconds);
        }

        public static DateTime RemoveMilisecond(DateTime dt)
        {
            return dt.AddMilliseconds(-dt.Millisecond);
        }

        public static DateTime ConvertStringToDateTime(string dt)
        {
            return XmlConvert.ToDateTime(dt, WebServiceDatePattern).ToLocalTime();
        }

        /// <summary>
        /// Method returns default time zone for given country and state.
        /// (currently supported: USA and Poland)
        /// </summary>
        /// <param name="country">country, empty string means USA</param>
        /// <param name="state">state code, can be empty for some countries</param>
        /// <returns>time zone ID; empty string if given country/state is not supported</returns>
        public static string GetDefaultTimeZoneForCountry(string country, string state)
        {
            switch (country.ToUpper().Trim())
            {
                case "USA":
                case "US":
                case "":
                    switch (state.ToUpper().Trim())
                    {
                        case "HI":
                            return "US/Hawaii";

                        case "AK":
                            return "US/Alaska";

                        case "CA":
                        case "NV":
                        case "OR":
                        case "WA":
                            return "US/Pacific";

                        case "AZ":
                            return "US/Arizona";

                        case "CO":
                        case "ID":
                        case "MT":
                        case "NE":
                        case "NM":
                        case "ND":
                        case "SD":
                        case "UT":
                        case "WY":
                            return "US/Mountain";

                        case "AL":
                        case "AR":
                        case "IL":
                        case "IA":
                        case "KS":
                        case "KY":
                        case "LA":
                        case "MN":
                        case "MS":
                        case "MO":
                        case "OK":
                        case "TN":
                        case "TX":
                        case "WI":
                            return "US/Central";

                        case "CT":
                        case "DC":
                        case "DE":
                        case "FL":
                        case "GA":
                        case "IN":
                        case "ME":
                        case "MD":
                        case "MA":
                        case "MI":
                        case "NH":
                        case "NJ":
                        case "NY":
                        case "NC":
                        case "OH":
                        case "PA":
                        case "RI":
                        case "SC":
                        case "VT":
                        case "VA":
                        case "WV":
                            return "US/Eastern";
                    }
                    break;

                case "POLAND":
                    return "Europe/Warsaw";
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets joint parameters info. This function shall be used to join results from GetParamsInfo method calls.
        /// </summary>
        /// <param name="parametersInfos">Parameters info in the form of string[] or string...</param>
        /// <returns>Returns joint parameters info.</returns>
        public static string GetParamsInfoJoint(params string[] parametersInfos)
        {
            string result = "";

            for (int i = 0; i < parametersInfos.Length; i++)
            {
                if (!string.IsNullOrEmpty(parametersInfos[i]))
                {
                    result = StringUtils.AppendComma(result);
                    result += parametersInfos[i];
                }
            }
            return result;
        }

        /// <summary>
        /// Gets parameters info in the format: [key1]->[value1], [key2]->[value2], [key3]->[value3].
        /// Odd parameters should be the keys. Even parameters should be the values.
        /// </summary>
        /// <param name="parameters">Parameters in the form of string[] or string...</param>
        /// <returns>Returns formated parameters info.</returns>
        public static string GetParamsInfo(params string[] parameters)
        {
            string result = "";
            string key = "";
            string value = "";
            for (int i = 0; i < parameters.Length; i++)
            {
                if (i % 2 == 0)
                {
                    key = parameters[i];
                }
                else
                {
                    value = parameters[i];

                    result = StringUtils.AppendComma(result);
                    result += "[" + key + "]->[" + value + "]";
                }
            }
            return result;
        }

        /// <summary>
        /// Funcion remove all Variable Indicators used in calculated field in expressions
        /// ( &lt;RUNS&gt; , &lt;ARRAYRESULTS&gt; , [&lt;Mutation&gt;] )
        /// </summary>
        /// <param name="var">Parameters in the form of string[] or string...</param>
        /// <param name="removeProcColPrefix">Indicates if USERDEFCOL_ indicator shall be remove...</param>
        /// <returns>Returns string without indicators.</returns>
        public static string RemoveVariableIndicators(string var, bool removeProcColPrefix = false)
        {
            string variable = var;
            //runs and array indicators
            variable = variable.Replace("<RUNS>", "");
            variable = variable.Replace("<ARRAYRESULTS>", "");
            if (removeProcColPrefix == true)
            {
                variable = variable.Replace("USERDEFCOL_", "");
            }
            //[<Mutation>] indicator
            if (variable.Contains("<MUTATION:"))
            {
                int startIndex = variable.IndexOf("<MUTATION:");
                variable = variable.Remove(startIndex);

                if (variable.Contains("["))
                    variable = variable + "]";
            }
            //[<WellResult>] indicator
            if (variable.Contains("<WellResult:"))
            {
                int startIndex = variable.IndexOf("<WellResult:");
                variable = variable.Remove(startIndex);

                if (variable.Contains("["))
                    variable = variable + "]";
            }
            //[<NUM>] indicator
            if (variable.Contains("<NUM:"))
            {
                int startIndex = variable.IndexOf("<NUM:");
                variable = variable.Remove(startIndex);

                if (variable.Contains("["))
                    variable = variable + "]";
            }
            // Test indicator
            if (variable.Contains("."))
            {
                string[] temp = variable.Split(new char[] { '.' });
                foreach (string s in temp)
                {
                    if (!s.Contains("<TST>") &&
                        !s.Contains("<SPE>") &&
                        !s.Contains("<POP>") &&
                        !s.Contains("<CTRL>"))
                    {
                        variable = s;
                        break;
                    }
                }
                if (variable.Contains("["))
                    variable = variable + "]";
            }
            return variable;
        }

        public static int GetWellResultValue(string var)
        {
            string variable = var;
            string value = string.Empty;
            variable = variable.Replace("<MUTATION:", "");
            variable = variable.Replace("<NUM:", "");
            if (variable.Contains("<WellResult:"))
            {
                string[] temp = variable.Split(new char[] { ':' }, 2);
                if (temp.Length > 1)
                {
                    int startIndex = variable.IndexOf(":") + 1;
                    value = variable.Substring(startIndex);
                    int remIndex = value.IndexOf(">");
                    value = value.Remove(remIndex);
                }
            }
            return Convert.ToInt32(value);
        }

        public static string GetControlCodeFromIndicator(string var)
        {
            string variable = var;
            if (variable.Contains("<CTRL>"))
            {
                int startIndex = variable.IndexOf("<CTRL>");
                variable = variable.Remove(startIndex);
            }
            return variable;
        }

        public static string MergeSpecimenVariableIndicator(string var, string property)
        {
            return property.StartsWith("SPECUSERDEFCOL_") ? "SPECUSERDEFCOL_" + var : var;
        }

        public static string GetTestIndicatorCode(string var)
        {
            string variable = var;
            string test = string.Empty;
            //runs and array indicators
            variable = variable.Replace("<RUNS>", "");
            variable = variable.Replace("<ARRAYRESULTS>", "");

            //[<Mutation>] indicator
            if (variable.Contains("<MUTATION:"))
            {
                int startIndex = variable.IndexOf("<MUTATION:");
                variable = variable.Remove(startIndex);

                if (variable.Contains("["))
                    variable = variable + "]";
            }
            // Test indicator
            if (variable.Contains("."))
            {
                string[] temp = variable.Split(new char[] { '.' }, 2);
                if (temp.Length > 1)
                {
                    int startIndex = variable.IndexOf(".") + 1;
                    test = variable.Substring(startIndex);
                }
                if (test.Contains("]"))
                    test = test.Trim(new char[] { ']' });
            }
            return test;
        }

        /// <summary>
        /// Checks if calcultead field contians Indicator
        /// </summary>
        /// <param name="var">Searched string</param>
        /// <returns>true if contains indcator otherwise false</returns>
        public static bool ContainsVariableIndicator(string var)
        {
            string[] indicators = { "<RUNS>", "<ARRAYRESULTS>", "<MUTATION:", "<NUM:", "<WellResult:" };

            bool result = false;
            Array.ForEach<String>(indicators, delegate (string item) { result |= var.Contains(item) ? true : false; });

            return result;
        }

        public static string LogException(Type type, Exception exception, string additionalInfo)
        {
            return LogException(type, exception, additionalInfo, false);
        }

        public static string LogException(Type type, Exception exception, string additionalInfo, bool onlyMessage)
        {
            string sep = "--------------------------------------------------";
            string exceptionInfo = "Exception Info:" + Environment.NewLine;

            if (string.IsNullOrEmpty(additionalInfo))
            {
                additionalInfo = "No info";
            }
            exceptionInfo += "\t" + additionalInfo + Environment.NewLine;

            if (onlyMessage)
            {
                exceptionInfo += "Exception Message:" + Environment.NewLine;
                exceptionInfo += "\t" + exception.GetType().FullName + ": " + exception.Message;
            }
            else
            {
                exceptionInfo += "Exception Stack:" + Environment.NewLine;
                exceptionInfo += "\t" + exception.GetType().FullName + ": " + exception.Message + Environment.NewLine + exception.StackTrace;
            }

            Trace.WriteLine(DateTime.Now.ToString("G") + " ERROR " + type.FullName + Environment.NewLine
                + sep + Environment.NewLine + exceptionInfo + Environment.NewLine + sep + Environment.NewLine);

            return exceptionInfo;
        }

        /// <summary>
        /// Used to retrieve minimum long column's value from data table
        /// </summary>
        /// <param name="dataTable">The given data table</param>
        /// <param name="columnName">The column name which contains the long values</param>
        /// <returns></returns>
        public static long GetMinLongValueFromColumn(DataTable dataTable, String columnName)
        {
            long minValue = long.Parse(dataTable.Rows[0][columnName].ToString());

            foreach (DataRow dr in dataTable.Rows)
            {
                long columnValue = long.Parse(dr[columnName].ToString());
                minValue = Math.Min(minValue, columnValue);
            }
            return minValue;
        }

        /// <summary>
        /// Used to compare string with boolean string and return appropriate generic flag
        /// </summary>
        /// <param name="str">The given string</param>
        /// <returns><b>GcmGlobals.GenericFlag.NO</b> if string is blank or equal to "false",
        /// <b>GcmGlobals.GenericFlag.YES</b> if string is not blank and equal to "true" or given string</returns>
        public static string GetGenericFlagFromBoolString(string str)
        {
            str = (string.IsNullOrEmpty(str) || StringUtils.EqualsIgnoreCase(false.ToString(), str)) ? GenericFlag.NO : str;
            str = (!string.IsNullOrEmpty(str) && StringUtils.EqualsIgnoreCase(true.ToString(), str)) ? GenericFlag.YES : str;
            return str;
        }

        /// <summary>
        /// Public struct holding generic flags like: 'Y', 'N', which
        /// </summary>
        public struct GenericFlag
        {
            public const string COMPLETE = "Y";
            public const string UNCOMPLETE = "N";
            public const string INCOMPLETE = "I";
            public const string ACTIVE = "Y";
            public const string INACTIVE = "N";
            public const string YES = "Y";
            public const string NO = "N";
            public const string NOT = "N";
            public const string TRUE_UP = "TRUE";
            public const string FALSE_UP = "FALSE";
            public const string TRUE_LOW = "true";
            public const string FALSE_LOW = "false";
            public const string EMPTY = "";
        }

        public static bool IsEqualValues(object a, object b)
        {
            return ((a == null && b == null) || (a != null && a.Equals(b)));
        }

        public static bool IsEqual(object o1, object o2)
        {
            return (o1 == null && o2 == null) || (o1 != null && o1.Equals(o2));
        }

        public static string GetShortFormatTimeString(object datetime)
        {
            string retVal = "";
            if (datetime != null)
            {
                DateTime dateTime = DateTime.MinValue;
                string datePattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                string dateTimeValue = datetime.ToString();
                try
                {
                    if (DateTime.TryParse(dateTimeValue, out dateTime))
                    {
                        retVal = dateTime.ToString(datePattern);
                    }
                }
                catch (ArgumentException) { }
            }
            return retVal;
        }

        #region DataRow Tools

        /// <summary>
        /// Checks if column is present in row's table
        /// </summary>
        /// <param name="row">DataRow to check</param>
        /// <param name="columnName">column name to check</param>
        /// <returns></returns>
        /// <exception>ArgumentNullException if DataRow is null or Column Name is blank</exception>
        public static Boolean ContainsColumn(this DataRow row, String columnName)
        {
            if (row == null)
            {
                throw new ArgumentNullException("Data row in null");
            }
            if (columnName.IsBlank())
            {
                throw new ArgumentNullException("Column Name is blank");
            }
            return row.Table.Columns.Contains(columnName);
        }

        /// <summary>
        /// Get typed value from column in row in specified format with default value
        /// </summary>
        /// <typeparam name="T">type to get</typeparam>
        /// <param name="row"></param>
        /// <param name="columnName">Column name</param>
        /// <param name="formatProvider">Format Provider</param>
        /// <param name="defaultValue">Default Value</param>
        /// <returns>value of T type</returns>
        /// <exception>ArgumentNullException if DataRow is null or DataRow has DataRowState.Deleted state or Column Name is blank</exception>
        public static T Get<T>(this DataRow row, string columnName, IFormatProvider formatProvider, T defaultValue)
        {
            if (row.RowState == DataRowState.Deleted)
            {
                throw new ArgumentNullException("DataRow has DataRowState.Deleted state");
            }
            if (row == null)
            {
                throw new ArgumentNullException("Data row in null");
            }
            if (columnName.IsBlank())
            {
                throw new ArgumentNullException("Column Name is blank");
            }
            T result;
            try
            {
                result = (T)Convert.ChangeType(row[columnName], typeof(T), formatProvider);
            }
            catch (InvalidCastException) { result = defaultValue; }
            catch (FormatException) { result = defaultValue; }
            catch (OverflowException) { result = defaultValue; }
            catch (ArgumentNullException) { result = defaultValue; }
            return result;
        }

        /// <summary>
        /// Get typed value from column in row in specified format
        /// </summary>
        /// <typeparam name="T">Type to get</typeparam>
        /// <param name="row"></param>
        /// <param name="columnName">Column Name</param>
        /// <param name="formatProvider">Format Provider</param>
        /// <returns>value of T type</returns>
        /// <exception>ArgumentNullException if DataRow is null or DataRow has DataRowState.Deleted state or Column Name is blank</exception>
        public static T Get<T>(this DataRow row, string columnName, IFormatProvider formatProvider)
        {
            return row.Get<T>(columnName, formatProvider, default(T));
        }

        /// <summary>
        /// Get typed value from column in row in InvariantCulture format
        /// </summary>
        /// <param name="row"></param>
        /// <typeparam name="T">Type to get</typeparam>
        /// <param name="columnName">Column Name</param>
        /// <returns>value of T type</returns>
        /// <exception>ArgumentNullException if DataRow is null or DataRow has DataRowState.Deleted state or Column Name is blank</exception>
        public static T Get<T>(this DataRow row, string columnName)
        {
            return row.Get<T>(columnName, CultureInfo.InvariantCulture, default(T));
        }

        /// <summary>
        /// Get typed value from column in row in InvariantCulture format with default value
        /// </summary>
        /// <typeparam name="T">Type to get</typeparam>
        /// <param name="row"></param>
        /// <param name="columnName">Column Name</param>
        /// <param name="defaultValue">Defualt Value</param>
        /// <returns>value of T type</returns>
        /// <exception>ArgumentNullException if DataRow is null or DataRow has DataRowState.Deleted state or Column Name is blank</exception>
        public static T Get<T>(this DataRow row, string columnName, T defaultValue)
        {
            return row.Get<T>(columnName, CultureInfo.InvariantCulture, defaultValue);
        }

        private static void FillRelationsData(DataRow ChildRow, ref Dictionary<DataRow, bool> data)
        {
            DataRow parent = null;
            if (ChildRow != null && ChildRow.Table != null && ChildRow.Table.DataSet != null)
            {
                foreach (DataRelation rel in ChildRow.Table.DataSet.Relations)
                {
                    if (rel.ChildTable == ChildRow.Table)
                    {
                        parent = ChildRow.GetParentRow(rel);
                        if (parent != null && !data.ContainsKey(parent))
                        {
                            data.Add(parent, parent.RowState == DataRowState.Unchanged);
                            FillRelationsData(parent, ref data);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fore each KeyValuePair in dictionary, sets row[KeyValuePair.Key] = KeyValuePair.Value if it is different than current value.
        /// If acceptCahnges is set - accepts changes for row and not modified parent rows
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="columnsValues"></param>
        /// <param name="acceptChanges"></param>
        /// <param name="convertDecimal">Convert decimal values for different localizations</param>
        /// <returns>Returns true if any column was changed</returns>
        public static bool SetColVal(this DataRow row, Dictionary<string, object> columnsValues, bool acceptChanges, bool convertDecimal)
        {
            bool result = false;
            object value = null;
            string column = null;
            Dictionary<DataRow, bool> ParentRows = new Dictionary<DataRow, bool>();
            bool parentAdded = false;
            foreach (KeyValuePair<string, object> kvp in columnsValues)
            {
                column = kvp.Key;
                value = kvp.Value;
                parentAdded = false;
                if (row != null && !string.IsNullOrEmpty(column)
                    && row.Table.Columns.Contains(column))
                {
                    if (kvp.Value == null)
                        value = string.Empty;
                    if (row.RowState == DataRowState.Added ||
                        row.RowState == DataRowState.Detached ||
                        row[column].ToString() != ConvertToString(value))
                    {
                        if (!parentAdded)
                        {
                            FillRelationsData(row, ref ParentRows);
                            parentAdded = true;
                        }
                        if (convertDecimal && typeof(decimal).Equals(row[column].GetType()))
                        {
                            decimal fakeValue;
                            if (TryConvertToDecimal(value, out fakeValue))
                            {
                                value = (object)fakeValue;
                            }
                            else
                            {
                                value = DBNull.Value;
                            }
                        }
                        row[column] = value;
                        result = true;
                    }
                }
            }
            if (acceptChanges)
                row.AcceptChanges();
            foreach (KeyValuePair<DataRow, bool> kvp in ParentRows)
            {
                if (kvp.Value)
                    kvp.Key.AcceptChanges();
            }
            return result;
        }

        /// <summary>
        /// Fore each KeyValuePair in dictionary, sets row[KeyValuePair.Key] = KeyValuePair.Value if it is different than current value.
        /// If acceptCahnges is set - accepts changes for row and not modified parent rows
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="columnsValues"></param>
        /// <param name="acceptChanges"></param>
        /// <returns>Returns true if any column was changed</returns>
        public static bool SetColVal(this DataRow row, Dictionary<string, object> columnsValues, bool acceptChanges)
        {
            return SetColVal(row, columnsValues, acceptChanges, false);
        }

        /// <summary>
        /// Fore each KeyValuePair in dictionary, sets row[KeyValuePair.Key] = KeyValuePair.Value if it is different than current value.
        /// Row.AcceptChanges() is called only for not modified parent rows
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnsValues"></param>
        /// <returns></returns>
        public static bool SetColVal(this DataRow row, Dictionary<string, object> columnsValues)
        {
            return SetColVal(row, columnsValues, false);
        }

        /// <summary>
        /// Fore each KeyValuePair in dictionary, sets row[KeyValuePair.Key] = KeyValuePair.Value if it is different than current value.
        /// Row.AcceptChanges() is called for row and each parent row if it's previous state was Unchanged.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnsValues"></param>
        /// <returns></returns>
        public static bool SetFakeColVal(this DataRow row, Dictionary<string, object> columnsValues)
        {
            return SetColVal(row, columnsValues, row.RowState == DataRowState.Unchanged);
        }

        /// <summary>
        /// Sets columns value if row's table contains column and current value is different than new value
        /// Returns true if rows column value was modified
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="acceptChanges">if true Row.AcceptChanges() is called after modification</param>
        public static bool SetColVal(this DataRow row, string column, object value, bool acceptChanges)
        {
            Dictionary<string, object> colval = new Dictionary<string, object>();
            colval.Add(column, value);
            return SetColVal(row, colval, acceptChanges);
        }

        /// <summary>
        /// Sets columns value if row's table contains column and current value is different than new value.
        /// Row.AcceptChanges() is not called
        /// Returns true if rows column value was modified
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public static bool SetColVal(this DataRow row, string column, object value)
        {
            return SetColVal(row, column, value, false);
        }

        /// <summary>
        /// Sets columns value if row's table contains column and current value is different than new value.
        /// Row.AcceptChanges() is called if row.RowState == DataRowState.Unchanged
        /// Returns true if rows column value was modified
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public static bool SetFakeColVal(this DataRow row, string column, object value)
        {
            return SetColVal(row, column, value, row.RowState == DataRowState.Unchanged);
        }

        public static void RowsBeginEdit(ICollection rows)
        {
            foreach (DataRow row in rows)
            {
                row.BeginEdit();
            }
        }

        public static void RowsEndEdit(ICollection rows)
        {
            foreach (DataRow row in rows)
            {
                row.EndEdit();
            }
        }

        #endregion DataRow Tools

        #region CompareTools

        public static bool Compare(object o1, object o2)
        {
            return (o1 == null && o2 == null) ||
                   (o1 != null && o1.ToString().Equals(o2));
        }

        #endregion CompareTools

        public static string RemoveInvalidCharactersFromFileName(string fileName)
        {
            string result;
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            result = r.Replace(fileName, "");
            return result;
        }

        #region Monada

        public static TResult With<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator)
            where TResult : class
            where TInput : class
        {
            if (o == null) return null;
            return evaluator(o);
        }

        public static TResult Return<TInput, TResult>(this TInput o,
            Func<TInput, TResult> evaluator, TResult failureValue) where TInput : class
        {
            if (o == null) return failureValue;
            return evaluator(o);
        }

        public static TInput If<TInput>(this TInput o, Func<TInput, bool> evaluator)
            where TInput : class
        {
            if (o == null) return null;
            return evaluator(o) ? o : null;
        }

        public static TInput Unless<TInput>(this TInput o, Func<TInput, bool> evaluator)
          where TInput : class
        {
            if (o == null) return null;
            return evaluator(o) ? null : o;
        }

        public static TInput Do<TInput>(this TInput o, Action<TInput> action)
            where TInput : class
        {
            if (o == null) return null;
            action(o);
            return o;
        }

        public static IEnumerable<TInput> ForEach<TInput>(this IEnumerable<TInput> enumeration, Action<TInput> action)
           where TInput : class
        {
            if (enumeration == null) return new TInput[0];
            else foreach (TInput item in enumeration) action(item);
            return enumeration;
        }

        #endregion Monada

        #region Enumerables

        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            yield return item;
        }

        #endregion Enumerables
    }
}