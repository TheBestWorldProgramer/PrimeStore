using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Tools
{
    /// <summary>
    /// Summary description for DBTools.
    /// </summary>
    public static class DBTools
    {
        public static bool CanRead(this DataRow row)
        {
            if (row == null || row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Detached)
                return false;
            return true;
        }

        public static bool IsRowValueEqual<T>(DataRow row, string columnName, T valueToCompare)
        {
            T rowValue = GetTypedRowValue<T>(row, columnName);
            return Equals(rowValue, valueToCompare);
        }

        public static T GetTypedRowValue<T>(DataRow row, string columnName)
        {
            T result = default(T);
            if (row == null || !row.Table.Columns.Contains(columnName))
                return result;
            bool isDeleted = row.RowState == DataRowState.Deleted;
            if (isDeleted)
            {
                if (row[columnName, DataRowVersion.Original] == null ||
                    row[columnName, DataRowVersion.Original] == DBNull.Value)
                    return result;
            }
            else if (row[columnName] == null || row[columnName] == DBNull.Value)
                return result;

            if (!isDeleted)
                result = (T)row[columnName];
            else
                result = (T)row[columnName, DataRowVersion.Original];
            return result;
        }

        public static object GetRowObjValue(DataRow row, string sColumn)
        {
            object res = null;
            if (row == null)
            {
                return res;
            }

            if (row.RowState != DataRowState.Deleted)
            {
                if (row.Table.Columns.Contains(sColumn))
                {
                    res = row[sColumn];
                }
            }
            else
            {
                res = row[sColumn, DataRowVersion.Original];
            }

            return res;
        }

        static public string GetRowValue(DataRow row, string sColumn)
        {
            string sRes = String.Empty;

            if (row == null)
            {
                return sRes;
            }

            if (row.RowState != DataRowState.Deleted)
            {
                if (row.Table.Columns.Contains(sColumn))
                {
                    sRes = row[sColumn].ToString();
                }
            }
            else
            {
                sRes = row[sColumn, DataRowVersion.Original].ConvertToString();
            }
            return sRes;
        }

        public static bool GetRowBool(DataRow row, string column)
        {
            return GetRowValue(row, column) == Tools.GenericFlag.YES;
        }

        public static bool IsEmptyValue(DataRow row, string column)
        {
            return GetRowValue(row, column).IsNullOrEmptyString();
        }

        static public long GetNumber(Object ob)
        {
            long nRes;

            nRes = -1;
            if (ob != null && ob != DBNull.Value)
            {
                if (ob is System.Decimal)
                    nRes = (long)(decimal)ob;
                else
                {
                    if (ob is System.Double)
                    {
                        nRes = (long)(Double)ob;
                    }
                    else
                    {
                        if (ob is System.Int64 || ob is System.Int32 || ob is System.Int16)
                        {
                            nRes = (long)ob;
                        }
                        else if (ob is System.String)
                        {
                            if (!Int64.TryParse(ob as string, out nRes))
                            {
                                throw new Exception(" Invalid Number Conversion Exception");
                            }
                        }
                        else
                        {
                            throw new Exception(" Invalid Number Conversion Exception");
                        }
                    }
                }
            }

            return nRes;
        }

        /// <summary>
        /// Converts object to double using Tools.ConvertToDouble.
        /// Replaces decimal place separator based on System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator
        /// </summary>
        /// <param name="ob">object parameter to be converted</param>
        /// <returns>double number, or 0.0 if conversion failed (does not throw exception)</returns>
        public static double ConvertToDouble(object ob)
        {
            string separator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            //especially for volum
            if (ob == null || ob == DBNull.Value || ob.ToString() == "")
                return 0.0;

            try
            {
                string val = ob.ToString();

                if ("." != separator)
                {
                    val = val.Replace(".", separator);
                }
                if ("," != separator)
                {
                    val = val.Replace(",", separator);
                }

                return Tools.ConvertToDouble(val);
            }
            catch (Exception)
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Converts object to double if object is parseable.
        /// Replaces decimal place separator based on System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator
        /// </summary>
        /// <param name="ob">object parameter to be converted</param>
        /// <param name="retValue">returned value - double number, or double.NaN if conversion failed</param>
        /// <returns>true if on successful conversion, or false if conversion failed (does not throw exception)</returns>
        public static bool TryConvertToDouble(object ob, out double retValue)
        {
            retValue = double.NaN;
            if (ob == null || ob == DBNull.Value || ob.ToString() == "")
            {
                return false;
            }
            try
            {
                string separator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                string val = ob.ToString();
                if (val.IndexOf(separator) == -1)
                {
                    val = val.Replace(".", separator);
                    val = val.Replace(",", separator);
                }
                retValue = Convert.ToDouble(val);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static decimal ConvertToDecimal(object ob)
        {
            string separator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            //especially for volum
            if (ob == null || ob == DBNull.Value || ob.ToString() == "")
                return decimal.Zero;

            try
            {
                string val = ob.ToString();

                if ("." != separator)
                {
                    val = val.Replace(".", separator);
                }
                if ("," != separator)
                {
                    val = val.Replace(",", separator);
                }

                return Convert.ToDecimal(val);
            }
            catch (Exception)
            {
                return decimal.Zero;
            }
        }

        public static int ConvertToInt(object ob)
        {
            //especially for volum
            if (ob == null || ob == DBNull.Value || ob.ToString() == "")
                return 0;

            try
            {
                return Convert.ToInt32(ob.ToString());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static long ConvertToLong(object ob)
        {
            //especially for volum
            if (ob == null || ob == DBNull.Value || ob.ToString() == "")
                return 0;

            try
            {
                return Convert.ToInt64(ob.ToString());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Converts "Y"/"N" to boolean.
        /// </summary>
        /// <param name="ob">Object containing Y/N string</param>
        /// <returns>true for "Y", false for other values</returns>
        public static bool ConvertToBool(object ob)
        {
            return Convert.ToString(ob) == Tools.GenericFlag.YES;
        }

        public static DateTime? ConvertToDateTime(object ob)
        {
            if (ob == null || ob == DBNull.Value || String.IsNullOrEmpty(ob.ToString()))
            {
                return null;
            }

            if (ob is DateTime)
            {
                return (DateTime)ob;
            }
            return null;
        }

        static public long GetRowNumber(DataRow row, string sColumn)
        {
            object oRes;

            if (row.RowState != DataRowState.Deleted)
            {
                oRes = row[sColumn];
            }
            else
            {
                oRes = row[sColumn, DataRowVersion.Original];
            }

            return GetNumber(oRes);
        }

        static public bool CompareRows(DataRow row1, DataRow row2)
        {
            bool bRes;

            bRes = false;
            if (row1.Table != null)
            {
                if (row1.Table.PrimaryKey.Length > 0)
                {
                    bRes = true;
                    foreach (DataColumn key in row1.Table.PrimaryKey)
                    {
                        if (GetRowValue(row1, key.ColumnName) != GetRowValue(row2, key.ColumnName))
                        {
                            bRes = false;
                        }
                    }
                }
            }
            return bRes;
        }

        public static string GetCommaSeparatedCodes<T>(System.Collections.Generic.IList<T> lIn)
        {
            string ret = string.Empty;

            if (lIn != null)
            {
                foreach (T code in lIn)
                {
                    string element = "" + code;
                    if (typeof(T) == (System.Type.GetType("System.String")))
                    {
                        element = StringUtils.EscapeSQL(element);
                    }

                    if (ret == string.Empty)
                    {
                        ret = element;
                    }
                    else
                    {
                        ret += "," + element;
                    }
                }
            }
            if (string.IsNullOrEmpty(ret))
            {
                if (typeof(T) == (System.Type.GetType("System.String")))
                {
                    ret = " null ";
                }
                else
                {
                    ret = " -1 ";
                }
            }

            return ret;
        }

        public static void MergeChangedRows(ref DataTable changedRows, DataTable currentRows)
        {
            DataRow row;
            DataRow newRow;
            bool bFound;
            int i;

            i = 0;
            // check all rows of the changed rows
            while (i < changedRows.Rows.Count)
            {
                // is row in table of the current rows
                bFound = false;
                row = changedRows.Rows[i];
                foreach (DataRow tempRow in currentRows.Rows)
                {
                    if (CompareRows(row, tempRow) == true)
                    {
                        bFound = true;
                    }
                }
                if (bFound != true)
                {
                    // if recort not exist in table of the current rows
                    if (row.RowState == DataRowState.Deleted)
                    {
                        // remove deleted rows from table of changed rows
                        row.AcceptChanges();
                    }
                    else
                    {
                        // add all rows which not exist in table of current rows
                        newRow = currentRows.NewRow();
                        foreach (DataColumn col in currentRows.Columns)
                        {
                            newRow[col.ColumnName] = row[col.ColumnName];
                        }
                        currentRows.Rows.Add(newRow);
                        // remove added rows from table of changed rows
                        if (row.RowState != DataRowState.Added)
                        {
                            row.Delete();
                            row.AcceptChanges();
                        }
                        else
                        {
                            row.Delete();
                        }
                    }
                }
                else
                {
                    i++;
                }
            }
            // check all rows of the current rows
            foreach (DataRow row1 in currentRows.Rows)
            {
                // is row in table of the changed rows
                bFound = false;
                foreach (DataRow row2 in changedRows.Rows)
                {
                    // special processing is required for unchanged rows, them
                    // have to be marked as deleted, after merging status for
                    // this rows is changed to modified
                    if (CompareRows(row1, row2) == true && row2.RowState != DataRowState.Unchanged)
                    {
                        bFound = true;
                    }
                }
                // mark as delete all rows which not exist in table of changed rows
                // and weren't added above
                if (bFound != true && row1.RowState != DataRowState.Added)
                {
                    row1.Delete();
                }
            }
            currentRows.DataSet.Merge(changedRows, false, MissingSchemaAction.Add);
            changedRows.Clear();
            changedRows.DataSet.Merge(currentRows, false, MissingSchemaAction.Add);
        }

        /// <summary>
        /// Gets a first DataRow object that match the filter.
        /// </summary>
        /// <param name="table">DataTable to find row.</param>
        /// <param name="filterExpression">The criteria to use to filter the rows.</param>
        /// <returns>Found DataRow object, if no matching row is find return null.</returns>
        public static DataRow SelectFirstRow(this DataTable table, string filterExpression)
        {
            return SelectFirstRow(table, filterExpression, null);
        }

        /// <summary>
        /// Gets a first DataRow object that match the filter in the order of the sort.
        /// </summary>
        /// <param name="table">DataTable to find row.</param>
        /// <param name="filterExpression">The criteria to use to filter the rows.</param>
        /// <param name="sort">A string specifying the column and sort direction.</param>
        /// <returns>Found DataRow object, if no matching row is find return null.</returns>
        public static DataRow SelectFirstRow(this DataTable table, string filterExpression, string sort)
        {
            return SelectFirstRow(table, filterExpression, sort, DataViewRowState.CurrentRows);
        }

        /// <summary>
        /// Gets a first DataRow object that match the filter in the order of the sort,
        /// that match the specified state.
        /// </summary>
        /// <param name="table">DataTable to find row.</param>
        /// <param name="filterExpression">The criteria to use to filter the rows.</param>
        /// <param name="sort">A string specifying the column and sort direction.</param>
        /// <param name="recordStates">One of the DataViewRowState values.</param>
        /// <returns>Found DataRow object, if no matching row is find return null.</returns>
        public static DataRow SelectFirstRow(this DataTable table, string filterExpression, string sort, DataViewRowState recordStates)
        {
            DataRow selected = null;
            if (table != null)
            {
                DataRow[] matchRows = table.Select(filterExpression, sort, recordStates);
                if (matchRows.Length > 0)
                {
                    selected = matchRows[0];
                }
            }
            return selected;
        }

        public static string PrepareString(string sText)
        {
            string sRes;

            sRes = sText.Replace("'", "''");
            return sRes;
        }

        public static string PrepareStringToCrit(string sText)
        {
            string sRes;
            sRes = sText.Replace("'", "''");
            sRes = sRes.Replace("/", "//");
            sRes = sRes.Replace("%", "/%");
            sRes = sRes.Replace("_", "/_");
            return sRes;
        }

        public static string PrepareStringToCritIN(string sText)
        {
            string sRes = "";
            string[] elements;
            if (sText.Contains(", "))
            {
                elements = sText.Split(new String[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                elements = sText.Split(new String[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
            }

            foreach (string tmpElement in elements)
            {
                sRes = sRes + ", '" + tmpElement + "'";
            }
            sRes = sRes.Trim(',', ' ');
            return sRes;
        }

        public static string ReplacePipesWithCommas(string sText)
        {
            string sRes = "";
            bool bLock = false;

            while (sText.Length > 0)
            {
                if (bLock)
                {
                    sRes += ",";
                }
                string sTxt = DBTools.PrepareString(Tools.RetriveSubString(ref sText));
                sRes += "'" + sTxt + "'";
                bLock = true;
            }
            return sRes;
        }

        /// <summary>
        /// Compares and merges two datasets with following mapping.
        /// </summary>
        /// <param name="originalDataSet">DataSet containing original data.</param>
        /// <param name="newDataSet">DataSet containing new data.</param>
        /// <param name="tablesMap">Mapping between table and primary key</param>
        /// <param name="updateStatus">Update row status after copying.</param>
        /// <param name="addAbsentColumns">Allow to add absent columns</param>
        /// <returns>Merged DataSet</returns>
        public static DataSet MergeDataSets(this DataSet originalDataSet, DataSet newDataSet, Dictionary<string, string> tablesMap,
            bool updateStatus = true, bool addAbsentColumns = true)
        {
            foreach (KeyValuePair<string, string> item in tablesMap)
            {
                if (newDataSet.Tables.Contains(item.Key) && originalDataSet.Tables.Contains(item.Key) &&
                    newDataSet.Tables[item.Key].Columns.Contains(item.Value) &&
                    originalDataSet.Tables[item.Key].Columns.Contains(item.Value))
                {
                    DataTable origTable = originalDataSet.Tables[item.Key];
                    DataTable newTable = newDataSet.Tables[item.Key];
                    string table = item.Key;
                    string column = item.Value;
                    foreach (DataRow row in newTable.Rows)
                    {
                        string filter = string.Format("[{0}] = '{1}' AND [{0}] IS NOT NULL", column, row[column]);
                        DataRow[] rows = origTable.Select(filter);
                        if (null == rows || rows.Length == 0)
                        {
                            DataRow newRow = origTable.NewRow();
                            foreach (DataColumn col in newTable.Columns)
                            {
                                if (!origTable.Columns.Contains(col.ColumnName))
                                {
                                    if (addAbsentColumns)
                                    {
                                        origTable.Columns.Add(col.ColumnName, col.DataType);
                                        newRow[col.ColumnName] = GetRowObjValue(row, col.ColumnName);
                                    }
                                }
                                else
                                {
                                    newRow[col.ColumnName] = GetRowObjValue(row, col.ColumnName);
                                }
                            }

                            origTable.Rows.Add(newRow);
                            if (updateStatus)
                            {
                                if (row.RowState == DataRowState.Deleted)
                                {
                                    newRow.AcceptChanges();
                                    newRow.Delete();
                                }
                                else if (row.RowState == DataRowState.Modified ||
                                    row.RowState == DataRowState.Unchanged)
                                {
                                    newRow.AcceptChanges();
                                    newRow.SetModified();
                                }
                            }
                        }
                    }
                }
            }

            return originalDataSet;
        }

        /// <summary>
        /// Copy data from input row to current table. New row in table will be created.
        /// </summary>
        /// <param name="origTable">Target table, where will be created new row with copied data.</param>
        /// <param name="row">Input row.</param>
        /// <param name="updateStatus">Update row status after copying.</param>
        /// <returns>Return new added row</returns>
        public static DataRow CopyRowToTable(this DataTable origTable, DataRow row, bool updateStatus = true)
        {
            DataRow newRow = origTable.NewRow();
            foreach (DataColumn col in row.Table.Columns)
            {
                if (origTable.Columns.Contains(col.ColumnName))
                {
                    newRow[col.ColumnName] = GetRowObjValue(row, col.ColumnName);
                }
            }

            origTable.Rows.Add(newRow);
            if (updateStatus)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    newRow.AcceptChanges();
                    newRow.Delete();
                }
                else if (row.RowState == DataRowState.Modified ||
                    row.RowState == DataRowState.Unchanged)
                {
                    newRow.AcceptChanges();
                    newRow.SetModified();
                }
            }

            return newRow;
        }

        /// <summary>
        /// Copy data from input row to current sorted table. New row in table will be created.
        /// </summary>
        /// <param name="origTable">Target table, where will be created new row with copied data.</param>
        /// <param name="row">Input row.</param>
        /// <param name="columnName">Name of column.</param>
        /// <returns>Return new added row</returns>
        public static DataRow CopyRowToSortedTable(this DataTable origTable, DataRow row, string columnName)
        {
            if (CanRead(row))
            {
                DataRow newRow = origTable.NewRow();
                foreach (DataColumn col in row.Table.Columns)
                {
                    if (origTable.Columns.Contains(col.ColumnName))
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];
                    }
                }

                int pos = origTable.Rows.Count;
                if (origTable.Columns.Contains(columnName) && row.Table.Columns.Contains(columnName))
                {
                    string columnValue = row[columnName].ToString();
                    pos = 0;
                    foreach (DataRow curRow in origTable.Rows)
                    {
                        string curRowValue = curRow[columnName].ToString();
                        if (System.String.Compare(curRowValue, columnValue) < 0)
                        {
                            pos++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                origTable.Rows.InsertAt(newRow, pos);
                return newRow;
            }

            return null;
        }

        /// <summary>
        /// Compares and merges two datasets with same schema into one dataset with properly
        /// set rowStates for all rows.
        /// All tables in datasets must have primary keys.
        /// </summary>
        /// <param name="originalDataSet">DataSet containing original data.</param>
        /// <param name="changedDataSet">DataSet containing changed data.</param>
        /// <returns>Merged DataSet with properly set rowStates.</returns>
        public static DataSet MergeDataSets(DataSet originalDataSet, DataSet changedDataSet)
        {
            DataSet resultDataSet = originalDataSet.Clone();
            resultDataSet.EnforceConstraints = false;

            if (EqualSchema(originalDataSet, changedDataSet))
            {
                foreach (DataTable table in originalDataSet.Tables)
                {
                    if (changedDataSet.Tables.Contains(table.TableName))
                    {
                        DataTable mergedTable = MergeTables(table, changedDataSet.Tables[table.TableName]);
                        if (mergedTable != null)
                            resultDataSet.Merge(mergedTable);
                    }
                }
            }

            return resultDataSet;
        }

        /// <summary>
        /// Compares and merges two tables with same schema into one table with
        /// properly set rowStates for all rows.
        /// Both tables must have primary keys.
        /// </summary>
        /// <param name="originalTable">DataTable containing original data.</param>
        /// <param name="changedTable">DataTable containing changed data.</param>
        /// <returns>Merged DataTable with properly set rowStates.</returns>
        public static DataTable MergeTables(DataTable originalTable, DataTable changedTable)
        {
            if (originalTable == null || changedTable == null)
                return null;

            DataTable resultTable = originalTable.Clone();

            Hashtable hashOrigRows = new Hashtable(originalTable.Rows.Count);
            Hashtable hashChangedRows = new Hashtable(changedTable.Rows.Count);

            // put changed rows references to hash table
            foreach (DataRow changedRow in changedTable.Rows)
            {
                if (changedRow.RowState == DataRowState.Deleted || changedRow.RowState == DataRowState.Detached)
                    continue;

                hashChangedRows[GetKeyValues(changedRow)] = changedRow;
            }

            // chcek for rows deleted in changedTable
            foreach (DataRow originalRow in originalTable.Rows)
            {
                if (originalRow.RowState == DataRowState.Deleted || originalRow.RowState == DataRowState.Detached)
                    continue;

                hashOrigRows[GetKeyValues(originalRow)] = originalRow;
                bool bFound = false;

                // check if originalRow exists in changedTable
                if (hashChangedRows[GetKeyValues(originalRow)] != null)
                    bFound = true;

                if (!bFound)
                {
                    // originalRow doesn't exist in changedTable, so it has been removed
                    DataRow row = resultTable.NewRow();

                    // copy all data from original row to new row
                    foreach (DataColumn col in originalTable.Columns)
                    {
                        row[col.ColumnName] = originalRow[col.ColumnName];
                    }
                    resultTable.Rows.Add(row);
                    row.AcceptChanges();
                    // change new row state to Deleted
                    row.Delete();
                }
            }

            // chcek for rows changed or inserted in changedTable
            foreach (DataRow changedRow in hashChangedRows.Values)
            {
                bool bFound = false;

                DataRow originalRow = hashOrigRows[GetKeyValues(changedRow)] as DataRow;

                // check if originalRow exists in changedTable
                if (originalRow != null)
                    bFound = true;

                if (!bFound)
                {
                    // changedRow doesn't exist in originalTable, so it has been added
                    DataRow row = resultTable.NewRow();

                    // copy all data from changed row to new row
                    foreach (DataColumn col in changedTable.Columns)
                    {
                        row[col.ColumnName] = changedRow[col.ColumnName];
                    }
                    resultTable.Rows.Add(row);
                    row.AcceptChanges();
                    // change new row state to Added
                    row.SetAdded();
                }
                else
                {
                    // changedRow exists in originalTable, it should be unchanged or modified
                    DataRow row = resultTable.NewRow();
                    bool bModified = false;

                    // copy all data from changed row to new row and check if row was modified or is unchanged
                    foreach (DataColumn col in changedTable.Columns)
                    {
                        if (originalRow != null)
                        {
                            string originalValue = DBTools.GetRowValue(originalRow, col.ColumnName);
                            string changedValue = DBTools.GetRowValue(changedRow, col.ColumnName);
                            if (!originalValue.Equals(changedValue))
                            {
                                // original row data differs from changed row
                                bModified = true;
                            }
                            row[col.ColumnName] = originalRow[col.ColumnName];
                        }
                        else
                        {
                            row[col.ColumnName] = changedRow[col.ColumnName];
                        }
                    }
                    resultTable.Rows.Add(row);
                    row.AcceptChanges();
                    // change new row state to Modified
                    if (bModified)
                    {
                        foreach (DataColumn col in changedTable.Columns)
                        {
                            row[col.ColumnName] = changedRow[col.ColumnName];
                        }
                    }
                }
            }

            return resultTable;
        }

        /// <summary>
        /// Compare two rows, column by column, to identify if they are contains the same value.
        /// </summary>
        /// <param name="row1">First row to compare.</param>
        /// <param name="row2">Second row to compare.</param>
        /// <param name="exceptColumn">List of columns that should be excluded from comparison.</param>
        /// <returns>Return true if rows equal.</returns>
        public static bool CompareTwoRow(DataRow row1, DataRow row2, params string[] exceptColumn)
        {
            bool bResult = true;
            foreach (DataColumn column in row1.Table.Columns)
            {
                string colName = column.ColumnName;
                if (null != exceptColumn && exceptColumn.Length > 0 && exceptColumn.Contains(colName))
                {
                    continue;
                }

                if (row2.Table.Columns.Contains(colName))
                {
                    bResult &= ColumnEqual(GetRowObjValue(row1, colName), GetRowObjValue(row2, colName));
                }
            }

            return bResult;
        }

        // returns prepared string containing value for row's all primary key columns
        private static string GetKeyValues(DataRow dr)
        {
            if (dr.RowState == DataRowState.Deleted)
            {
                return string.Empty;
            }
            string sRes = string.Empty;

            foreach (DataColumn col in dr.Table.PrimaryKey)
            {
                if (dr[col] != null)
                {
                    Tools.AddSubString(ref sRes, dr[col].ToString());
                }
            }
            return sRes;
        }

        // compares datasets schemas
        private static bool EqualSchema(DataSet primDataSet, DataSet secDataSet)
        {
            if (primDataSet.Tables.Count != secDataSet.Tables.Count)
                return false;

            foreach (DataTable primTable in primDataSet.Tables)
            {
                if (!secDataSet.Tables.Contains(primTable.TableName))
                    return false;

                DataTable secTable = secDataSet.Tables[primTable.TableName];
                if (secTable == null)
                    return false;

                if (primTable.Columns.Count != secTable.Columns.Count)
                    return false;

                if (primTable.PrimaryKey.Length != secTable.PrimaryKey.Length)
                    return false;

                foreach (DataColumn primCol in primTable.Columns)
                {
                    if (!secTable.Columns.Contains(primCol.ColumnName))
                        return false;

                    DataColumn secCol = secTable.Columns[primCol.ColumnName];
                    if (secCol == null)
                        return false;

                    if (primCol.DataType != secCol.DataType)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// If table contains column with specified name returns value;
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>

        public static object GetDataRowData(DataRow row, string columnName)
        {
            object value = null;

            if (row.Table.Columns.Contains(columnName))
            {
                value = row[columnName];
            }
            else
            {
                throw new ArgumentException("Column" + columnName + "is missed");
            }

            return value;
        }

        /// <summary>
        /// Returns string value from GetDataRowData;
        /// </summary>
        /// <param name="srow"></param>
        /// <param name="scolumnName"></param>
        /// <returns></returns>
        public static string GetStringDataRowData(DataRow srow, string scolumnName)
        {
            string value = string.Empty;

            value = (GetDataRowData(srow, scolumnName)).ToString();

            return value;
        }

        /// <summary>
        /// Returns table with distinct elements in specified field;
        /// </summary>
        /// <param name="SourceTable"></param>
        /// <param name="distinctFieldName"></param>
        /// <returns>DataTable</returns>
        public static DataTable SelectDistinct(DataTable SourceTable, string distinctFieldName)
        {
            DataTable resultTable = SourceTable.Clone();

            object LastValue = null;
            foreach (DataRow row in SourceTable.Select("", distinctFieldName))
            {
                if (LastValue == null || !(ColumnEqual(LastValue, row[distinctFieldName])))
                {
                    LastValue = row[distinctFieldName];
                    DataRow newRow = resultTable.NewRow();
                    newRow.ItemArray = row.ItemArray;
                    resultTable.Rows.Add(newRow);
                }
            }
            return resultTable;
        }

        /// <summary>
        /// Removes from given DataTable rows with recurrent specified field;
        /// </summary>
        /// <param name="SourceAndResultTable"></param>
        /// <param name="distinctFieldName"></param>
        /// <returns>DataTable</returns>
        public static void SelectDistinctInto(DataTable SourceAndResultTable, string distinctFieldName)
        {
            object LastValue = null;
            List<DataRow> rowsToRemove = new List<DataRow>();
            foreach (DataRow row in SourceAndResultTable.Select("", distinctFieldName))
            {
                if (LastValue != null && ColumnEqual(LastValue, row[distinctFieldName]))
                    rowsToRemove.Add(row);
                else
                    LastValue = row[distinctFieldName];
            }
            foreach (DataRow row in rowsToRemove)
                SourceAndResultTable.Rows.Remove(row);
        }

        public static bool ColumnEqual(object A, object B)
        {
            if (A == DBNull.Value && B == DBNull.Value) //  both are DBNull.Value
                return true;
            if (A == DBNull.Value || B == DBNull.Value) //  only one is DBNull.Value
                return false;
            return (A.Equals(B));
        }

        /// <summary>
        /// Method will update Column in DataRow only if New Value is different then Old Value.
        /// null/"" are treat as similar
        /// </summary>
        /// <param name="row">DataRow object to update.</param>
        /// <param name="column">string name of column to update.</param>
        /// <param name="value">object with new value.,</param>
        public static void UpdateColumnValue(DataRow row, string column, object value)
        {
            object a = Tools.GetStringDB(row[column]);
            object b = Tools.GetStringDB(value);

            if (!Tools.IsEqualValues(a, b))
                row[column] = value;
        }

        public static string MergeRowsIntoString(DataRow[] data, List<string> columns)
        {
            return MergeRowsIntoString(data, columns, "", ",");
        }

        public static string MergeRowsIntoString(DataRow[] data, List<string> columns, string columnSeparator, string rowSeparator)
        {
            string res = "";

            if (data == null || data.Length == 0)
                return res;

            foreach (DataRow dr in data)
            {
                string row = "";

                foreach (DataColumn col in dr.Table.Columns)
                {
                    if (columns == null || columns.Contains(col.ColumnName))
                        row += Tools.GetStringDB(dr[col.ColumnName]) + columnSeparator;
                }

                if (row.Length >= columnSeparator.Length)
                {
                    if (columnSeparator.Length > 0)
                        row = row.Remove(row.Length - columnSeparator.Length);

                    res += row + rowSeparator;
                }
            }

            if (res.Length >= rowSeparator.Length && rowSeparator.Length > 0)
                res = res.Remove(res.Length - rowSeparator.Length);

            return res;
        }

        public static void UpdateDataSet(DataSet ds, string xml, string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                DataSet temp = new DataSet();

                if (ds == null)
                {
                    ds = new DataSet();
                }

                if (Tools.String2DataSet(temp, xml, XmlReadMode.ReadSchema, true))
                {
                    ds.Merge(temp);
                    ds.AcceptChanges();
                }
            }
            else
            {
                throw new Exception(error);
            }
        }

        public static DataTable GetTable(string xml, string error, string tableName)
        {
            using (DataSet temp = new DataSet())
            {
                UpdateDataSet(temp, xml, error);
                return GetTable(temp, tableName);
            }
        }

        public static DataTable GetTable(DataSet ds, string tableName)
        {
            if (ds.Tables.Contains(tableName))
            {
                return ds.Tables[tableName];
            }
            else return new DataTable();
        }
    }
}