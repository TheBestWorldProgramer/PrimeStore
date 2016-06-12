using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tools
{       /// <summary>
        /// Operations on string that are <code>null</code> safe.
        /// </summary>
    public static class StringUtils
    {
        private static readonly string DOS_LINE_END = "\r\n";
        private static readonly string UNIX_LINE_END = "\n";

        #region EMPTY / NOTEMPTY

        /// <summary>
        /// Checks if a string is empty or null.
        /// </summary>
        /// <param name="str">The string to check, may be null</param>
        /// <returns><b>true</b> if the String is empty or null, otherwise <b>false</b></returns>
        public static bool IsEmpty(this string str)
        {
            return (str == null || str.Length == 0);
        }

        /// <summary>
        /// Checks if string is not empty and not null.
        /// </summary>
        /// <param name="str">The string to check, may be null</param>
        /// <returns><b>true</b> if the string is not empty and not null, otherwise <b>false</b></returns>
        public static bool IsNotEmpty(this string str)
        {
            return (str != null && str.Length > 0);
        }

        public static bool IsTextInTable(string text, string[] table)
        {
            bool retVal = false;

            if (text != null && table != null)
            {
                foreach (string tblText in table)
                {
                    if (text.Equals(tblText))
                    {
                        retVal = true;
                        break;
                    }
                }
            }

            return retVal;
        }

        #endregion EMPTY / NOTEMPTY

        #region BLANK / NOTBLANK

        /// <summary>
        /// Checks if a string is whitespace, empty or null.
        /// </summary>
        /// <param name="str">The string to check, may be null</param>
        /// <returns><b>true</b> if the string is null, empty or whitespace, otherwise <b>false</b></returns>
        public static bool IsBlank(this string str)
        {
            if (str == null || str.Length == 0)
                return true;

            foreach (char ch in str)
            {
                if (char.IsWhiteSpace(ch) == false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a string is not empty , not null and not whitespace only.
        /// </summary>
        /// <param name="str">The string to check, may be null</param>
        /// <returns><b>true</b> if the string is not empty and not null
        /// and not whitespace, otherwise <b>false</b></returns>
        public static bool IsNotBlank(this string str)
        {
            if (str == null || str.Length == 0)
                return false;

            foreach (char ch in str)
            {
                if (char.IsWhiteSpace(ch) == false)
                    return true;
            }

            return false;
        }

        #endregion BLANK / NOTBLANK

        #region TRIM

        /// <summary>
        /// Removes all occurrences of white space from the beginning and end of the string
        /// </summary>
        /// <param name="str">The string to trim, may be null</param>
        /// <returns></returns>
        public static string Trim(string str)
        {
            return (str == null ? string.Empty : str.Trim());
        }

        /// <summary>
        /// Removes all occurrences of a set of characters specified in
        /// a Unicode character array from the beginning and end of
        /// string.
        /// </summary>
        /// <param name="str">The string to trim, may be null</param>
        /// <param name="trimChars">An array of Unicode characters to be removed
        /// or a null reference </param>
        /// <returns>The string that remains after all occurrences of the
        /// characters in trimChars are removed. If trimChars is a null
        /// reference, white space characters are removed instead.</returns>
        public static string Trim(string str, char[] trimChars)
        {
            return (str == null ? string.Empty : str.Trim(trimChars));
        }

        /// <summary>
        /// Removes all occurrences of a set of characters specified in
        /// a Unicode character array from the beginning of the string.
        /// </summary>
        /// <param name="str">The string to trim, may be null</param>
        /// <param name="trimChars">An array of Unicode characters to be removed
        /// or a null reference </param>
        /// <returns>The string that remains after all occurrences of characters
        /// in trimChars are removed. If trimChars is a null reference,
        /// white space characters are removed instead.</returns>
        public static string TrimStart(string str, char[] trimChars)
        {
            return (str == null ? string.Empty : str.TrimStart(trimChars));
        }

        /// <summary>
        /// Removes all occurrences of a set of characters specified in
        /// a Unicode character array from the end of the string.
        /// </summary>
        /// <param name="str">The string to trim, may be null</param>
        /// <param name="trimChars">An array of Unicode characters to be removed
        /// or a null reference </param>
        /// <returns>The string that remains after all occurrences of characters
        /// in trimChars are removed. If trimChars is a null reference,
        /// white space characters are removed instead.</returns>
        public static string TrimEnd(string str, char[] trimChars)
        {
            return (str == null ? string.Empty : str.TrimEnd(trimChars));
        }

        #endregion TRIM

        #region EQUALS

        /// <summary>
        /// <para>
        /// Compares two strings, returning <b>true</b> if they are equal.
        /// </para>
        /// <para><b>null</b>s are handled without exceptions. Two
        /// <b>null</b> references are considered to be equal. </para>
        /// </summary>
        /// <param name="str1">The first string, may be null</param>
        /// <param name="str2">The second String, may be null</param>
        /// <returns><b>true</b> if the strings are equal, or both <b>null</b></returns>
        public static bool Equals(string str1, string str2)
        {
            return (str1 == null ? str2 == null : str1.Equals(str2));
        }

        /// <summary>
        /// Used to compare two string with ignoring case
        /// </summary>
        /// <param name="str1">The first string, may be null</param>
        /// <param name="str2">The second String, may be null</param>
        /// <returns><b>true</b> if the strings are equal or both <b>null</b>, otherwise - <b>false</b></returns>
        public static bool EqualsIgnoreCase(string str1, string str2)
        {
            return (String.Compare(str1, str2, StringComparison.OrdinalIgnoreCase) == 0);
        }

        #endregion EQUALS

        #region INDEXOF

        /// <summary>
        /// Reports the index of the first occurrence of the specified
        /// Unicode character in this instance.
        /// </summary>
        /// <param name="str">The string to check, may be null</param>
        /// <param name="searchChar">A Unicode character to seek.</param>
        /// <returns>The index, that is the character position in the string
        /// where value was found; otherwise, -1 if value was not found.</returns>
        public static int IndexOf(string str, char searchChar)
        {
            if (str == null || str.Length == 0)
            {
                return -1;
            }
            return str.IndexOf(searchChar);
        }

        /// <summary>
        /// Reports the index of the first occurrence of the specified string
        /// in the string.
        /// </summary>
        /// <param name="str">the string to check, may be null</param>
        /// <param name="searchStr">the string to search, may be null</param>
        /// <returns>A positive index position if searchStr was found,
        /// 0 searchStr is empty
        /// -1 searchStr was not found.</returns>
        public static int IndexOf(string str, string searchStr)
        {
            if (str == null || str.Length == 0 || searchStr == null)
            {
                return -1;
            }

            if (searchStr.Length == 0)
            {
                return 0;
            }

            return str.IndexOf(searchStr);
        }

        /// <summary>
        /// Reports the index of the first occurrence of the specified
        /// Unicode character in the string. The search starts
        /// at a specified character position.
        /// </summary>
        /// <param name="str">the string to check, may be null</param>
        /// <param name="searchChar">A Unicode character to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The index, indicating the character position in the string
        ///  where searchChar was found; otherwise, -1 </returns>
        public static int IndexOf(string str, char searchChar, int startIndex)
        {
            if (str == null || str.Length == 0
                || startIndex < 0 || str.Length <= startIndex)
            {
                return -1;
            }
            return str.IndexOf(searchChar, startIndex);
        }

        /// <summary>
        /// Reports the index of the first occurrence of the specified string
        /// in the string. The search starts at a specified character position.
        /// </summary>
        /// <param name="str">The string to check, may be null</param>
        /// <param name="searchStr">The string to search, may be null</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>A positive index position if searchStr was found,
        /// 0 searchStr is empty
        /// -1 searchStr was not found.</returns>
        public static int IndexOf(string str, string searchStr, int startIndex)
        {
            if (str == null || str.Length == 0
                || searchStr == null
                || startIndex < 0 || str.Length <= startIndex)
            {
                return -1;
            }

            if (searchStr.Length == 0)
            {
                return 0;
            }

            return str.IndexOf(searchStr, startIndex);
        }

        /// <summary>
        /// Reports the index of the first occurrence of the specified character
        /// in the string. The search starts at a specified character position
        /// and examines a specified number of character positions.
        /// </summary>
        /// <param name="str">The string to check, may be null</param>
        /// <param name="searchChar">A Unicode character to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>The index, indicating the character position in the string
        ///  where searchChar was found; otherwise, -1</returns>
        public static int IndexOf(string str, char searchChar, int startIndex, int count)
        {
            if (str == null || str.Length == 0
                || startIndex < 0 || count < 0
                || str.Length <= startIndex || str.Length <= startIndex + count)
            {
                return -1;
            }

            return str.IndexOf(searchChar, startIndex, count);
        }

        /// <summary>
        /// Reports the index of the first occurrence of the specified string
        /// in the string. The search starts at a specified character position.
        /// </summary>
        /// <param name="str">The string to check, may be null</param>
        /// <param name="searchStr">The string to search, may be null</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>A positive index position if searchStr was found,
        /// 0 searchStr is empty
        /// -1 searchStr was not found.</returns>
        public static int IndexOf(string str, string searchStr, int startIndex, int count)
        {
            if (str == null || str.Length == 0
                || searchStr == null
                || startIndex < 0 || count < 0
                || str.Length <= startIndex || str.Length <= startIndex + count)
            {
                return -1;
            }

            if (searchStr.Length == 0)
            {
                return 0;
            }

            return str.IndexOf(searchStr, startIndex, count);
        }

        #endregion INDEXOF

        #region APPEND STRING

        /// <summary>
        /// Append copy of second string to the end of first string, is null safe.
        /// </summary>
        /// <param name="str">String, second string will be appended to this,
        /// can be null.</param>
        /// <param name="strToAppend">String to append, can be null.</param>
        /// <returns>A string after the append operation has occurred.</returns>
        public static string AppendString(this string str, string strToAppend)
        {
            return AppendString(str, strToAppend, false);
        }

        /// <summary>
        /// Conditionally append copy of second string to the end of first string,
        /// is null safe.
        /// </summary>
        /// <param name="str">String, second string will be appended to this,
        /// can be null.</param>
        /// <param name="strToAppend">String to append, can be null.</param>
        /// <param name="onlyIfNotEmpty">Indicates if second string should be appended
        /// to first string always, or only if first string is not null or empty.</param>
        /// <returns>A string after the append operation has occurred.</returns>
        public static string AppendString(this string str, string strToAppend, bool onlyIfNotEmpty)
        {
            if (onlyIfNotEmpty && str != null && str.Length > 0)
            {
                if (strToAppend != null)
                    return str + strToAppend;
                else
                    return str;
            }
            else
            {
                if (str == null)
                    str = string.Empty;
                if (!onlyIfNotEmpty && strToAppend != null)
                    return str + strToAppend;
                else
                    return str;
            }
        }

        /// <summary>
        /// Append new line character to given string. New line character ('\n') is appended
        /// only if string is not null and not empty.
        /// </summary>
        /// <param name="str">String to append comma, can be null.</param>
        /// <returns>String with appended new line character (if str was not null or empty).</returns>
        public static string AppendNewLine(string str)
        {
            return AppendString(str, StringUtils.UNIX_LINE_END, true);
        }

        /// <summary>
        /// Append all specified words to the given string with comma separation.
        /// </summary>
        /// <param name="word">String to append.</param>
        /// <param name="words">Words to be appended.</param>
        /// <returns>Initial string with words appended and separated by comma</returns>
        public static string AppendWithComma(this string word, params string[] words)
        {
            return AppendWithComma(word, words.AsEnumerable());
        }

        public static string AppendWithComma(this string word, IEnumerable<string> words)
        {
            return JoinWords(", ", Enumerable.Concat(word.ToEnumerable(), words));
        }

        #endregion APPEND STRING

        #region UTILS FOR SQL

        /// <summary>
        /// Append AND condition to given string. This function can be used
        /// while building SQL query, AND condition is appended only if string
        /// is not null and not empty.
        /// </summary>
        /// <param name="str">String to append AND condition, can be null.</param>
        /// <returns>String with appended condition (if str was not null or empty).</returns>
        public static string AppendAndCondition(this string str)
        {
            return AppendString(str, " AND ", true);
        }

        /// <summary>
        /// Append OR condition to given string. This function can be used
        /// while building SQL query, OR condition is appended only if string
        /// is not null and not empty.
        /// </summary>
        /// <param name="str">String to append OR condition, can be null.</param>
        /// <returns>String with appended condition (if str was not null or empty).</returns>
        public static string AppendOrCondition(this string str)
        {
            return AppendString(str, " OR ", true);
        }

        /// <summary>
        /// Append comma (and sapace) to given string. This function can be used
        /// while building SQL query, comma is appended only if string
        /// is not null and not empty.
        /// </summary>
        /// <param name="str">String to append comma, can be null.</param>
        /// <returns>String with appended comma (if str was not null or empty).</returns>
        public static string AppendComma(this string str)
        {
            return AppendString(str, ", ", true);
        }

        /// <summary>
        /// Escapes the characters in a given string to be suitable to
        /// pass to an SQL query. This method only trims given string,
        /// turns single-quotes into doubled single-quotes, and after
        /// that adds sinlge-quotes in start and end of the string.
        /// It does not handle the cases of percent (%) or underscore
        /// (_) for use in LIKE clauses.
        /// </summary>
        /// <param name="str">The string to escape, may be null.</param>
        /// <returns>A new string, escaped for SQL, empty quoted string
        /// ('') if null string input</returns>
        public static string EscapeSQL(this string str)
        {
            return StringUtils.EscapeSQL(str, true);
        }

        /// <summary>
        /// Escapes the characters in a given string to be suitable to
        /// pass to an SQL query. This method only trims given string,
        /// turns single-quotes into doubled single-quotes. It does
        /// not handle the cases of percent (%) or underscore (_) for
        /// use in LIKE clauses. Second parameter indicates if after
        /// that sinlge-quotes are added in start and end of the string.
        /// </summary>
        /// <param name="str">The string to escape, may be null.</param>
        /// <param name="appendQuotes">If true, escaped string is enclosed
        /// in single quotes.</param>
        /// <returns>A new string, escaped for SQL, empty string (quoted if
        /// needed) if null string input</returns>
        public static string EscapeSQL(this string str, bool appendQuotes)
        {
            return EscapeSQL(str, appendQuotes, true);
        }

        /// <summary>
        /// Escapes the characters in a given string to be suitable to
        /// pass to an SQL query. This method turns single-quotes into
        /// doubled single-quotes. It does not handle the cases of
        /// percent (%) or underscore (_) for use in LIKE clauses.
        /// </summary>
        /// <param name="str">The string to escape, may be null.</param>
        /// <param name="appendQuotes">If true, escaped string is enclosed
        /// in single quotes.</param>
        /// <param name="trimFirst">If true, all blank characters are removed
        /// from escaped string before any processing.</param>
        /// <returns>A new string, escaped for SQL, empty string (quoted if
        /// needed) if null string input</returns>
        public static string EscapeSQL(this string str, bool appendQuotes, bool trimFirst)
        {
            if (str == null)
                str = string.Empty;

            if (trimFirst)
            {
                str = str.Trim();
            }
            str = str.Replace("'", "''");
            if (appendQuotes)
            {
                return "'" + str + "'";
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// Escapes the characters in a given string to be suitable to
        /// pass to an DB Functions as parameters.
        /// </summary>

        public static string EscapeSQLForDBFunctions(string str)
        {
            if (str == null)
                str = string.Empty;

            str = str.Replace("'", "''''");
            return str;
        }

        /// <summary>
        /// Escapes the characters in a given string to be suitable to
        /// pass to an SQL query. This method trims given string,
        /// turns single-quotes into doubled single-quotes, escapes percent
        /// (%) and underscore (_) for use in LIKE clauses. Next percent (%)
        /// char is appended, so all fields that starts with given string will
        /// match. After that sinlge-quotes are added in start and end of the string.
        /// </summary>
        /// <param name="str">The string to escape, may be null</param>
        /// <returns>A new string, escaped for SQL,
        /// ('%') if null string input</returns>
        public static string EscapeSQLStartMatch(string str)
        {
            string ret = "'%' ESCAPE '/'";

            if (str != null && str.Trim().Length > 0)
            {
                int initialCapacity = str.Length * 2;
                StringBuilder sqlBuild =
                    new StringBuilder(str.Trim(), initialCapacity);

                if (sqlBuild.Length > 0)
                {
                    sqlBuild.Replace("'", "''")
                        .Replace("/", "//")
                        .Replace("%", "/%")
                        .Replace("_", "/_")
                        .Append("%' ESCAPE '/'");
                }
                ret = "'" + sqlBuild.ToString();
            }

            return ret;
        }

        public static string DTWildcardsSqlMatch(string str)
        {
            if (str == null)
                return string.Empty;

            StringBuilder build = new StringBuilder(str.Length * 2);
            for (int i = 0; i < str.Length; ++i)
            {
                switch (str[i])
                {
                    case '[':
                        build.Append("[[]");
                        break;

                    case ']':
                        build.Append("[]]");
                        break;

                    case '*':
                        build.Append("[*]");
                        break;

                    case '%':
                        build.Append("[%]");
                        break;

                    case '\'':
                        build.Append("''");
                        break;

                    case '"':
                        build.Append("\"");
                        break;

                    default:
                        build.Append(str[i]);
                        break;
                }
            }

            return build.ToString();
        }

        /// <summary>
        /// Escapes the characters in a given string to be suitable to
        /// pass to an SQL query. This method trims given string,
        /// turns single-quotes into doubled single-quotes, escapes percent
        /// (%) and underscore (_) for use in LIKE clauses. Next percent (%)
        /// char is inserted in start of the string, so all fields that starts
        /// with given string will match. After that sinlge-quotes are added
        /// in start and end of the string.
        /// </summary>
        /// <param name="str">The string to escape, may be null</param>
        /// <returns>A new string, escaped for SQL,
        /// ('%') if null string input</returns>
        public static string EscapeSQLEndMatch(string str)
        {
            string ret = "'%' ESCAPE '/'";

            if (str != null && str.Trim().Length > 0)
            {
                int initialCapacity = str.Length * 2;
                StringBuilder sqlBuild =
                    new StringBuilder(str.Trim(), initialCapacity);

                if (sqlBuild.Length > 0)
                {
                    sqlBuild.Replace("'", "''")
                        .Replace("/", "//")
                        .Replace("%", "/%")
                        .Replace("_", "/_")
                        .Append("' ESCAPE '/'");
                }
                ret = "'%" + sqlBuild.ToString();
            }
            return ret;
        }

        /// <summary>
        /// Escapes the characters in a given string to be suitable to
        /// pass to an SQL query. This method trims given string,
        /// turns single-quotes into doubled single-quotes, escapes percent
        /// (%) and underscore (_) for use in LIKE clauses. Next percent (%)
        /// char is inserted booth on start and end of the string, so all fields
        /// which contains given string will match. After that sinlge-quotes are added
        /// in start and end of the string.
        /// </summary>
        /// <param name="str">The string to escape, may be null</param>
        /// <returns>A new string, escaped for SQL, empty quoted string
        /// ('%') if null string input</returns>
        public static string EscapeSQLMiddleMatch(string str)
        {
            string ret = "'%' ESCAPE '/'";

            if (str != null && str.Trim().Length > 0)
            {
                int initialCapacity = str.Length * 2;
                StringBuilder sqlBuild =
                    new StringBuilder(str.Trim(), initialCapacity);

                if (sqlBuild.Length > 0)
                {
                    sqlBuild.Replace("'", "''")
                        .Replace("/", "//")
                        .Replace("%", "/%")
                        .Replace("_", "/_")
                        .Append("%' ESCAPE '/'");
                }
                ret = "'%" + sqlBuild.ToString();
            }
            return ret;
        }

        public static List<string> SeparateStringToList(string stringToSeparate, char separator)
        {
            List<string> list = new List<string>();
            if (StringUtils.IsNotEmpty(stringToSeparate))
            {
                list.AddRange(stringToSeparate.Split(separator));
            }
            return list;
        }

        public static string SeparateStringChars(string stringToSeparate, string separator)
        {
            string outputString = "";
            if (StringUtils.IsNotEmpty(stringToSeparate))
            {
                for (int i = 0; i < stringToSeparate.Length; i++)
                {
                    outputString += stringToSeparate[i];
                    if (i < stringToSeparate.Length - 1)
                    {
                        outputString += separator;
                    }
                }
            }
            return outputString;
        }

        /// <summary>
        /// Creates string with SOUNDEX function call, based on passed parameter,
        /// that is suitable to pass to an SQL query.
        /// </summary>
        /// <param name="dbFieldName">DataBase field on which SOUNDEX function
        /// should operate</param>
        /// <param name="strToMatch">Value to comapre by SOUNDEX function with
        /// value of dbFieldName field</param>
        /// <returns>String with call to SOUNDEX function, empty string if
        /// parameters are null or empty</returns>
        public static string SQLSoundEx(string dbFieldName, string strToMatch)
        {
            string ret = string.Empty;
            if (dbFieldName != null && strToMatch != null
                && dbFieldName.Length > 0 && strToMatch.Length > 0)
            {
                ret = " SOUNDEX (" + dbFieldName +
                    ") = SOUNDEX (" + StringUtils.EscapeSQL(strToMatch) + ") ";
            }
            return ret;
        }

        /// <summary>
        /// Makes criterion "COLUMN = value", when value is null "COLUMN IS NULL" criterion is created
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <param name="value">Criterion value</param>
        /// <returns>Criterion</returns>
        public static string MakeCriterion(string columnName, string value)
        {
            string criterion;
            if (string.IsNullOrEmpty(value))
            {
                criterion = columnName + " IS NULL";
            }
            else
            {
                criterion = columnName + " = " + StringUtils.EscapeSQL(value);
            }
            return criterion;
        }

        /// <summary>
        /// Makes criterion "COLUMN IN (commaSeparatedfilterValues)", when keys argument is null or empty "COLUMN IS NULL" criterion is created
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <param name="keys">List of filter Values</param>
        /// <param name="keysEscapeSQL">Should method escapeSQL key values</param>
        /// <returns>Criterion</returns>
        public static string MakeCriterion(string columnName, List<string> keys, bool keysEscapeSQL)
        {
            string filter = String.Empty;

            if (keys != null && keys.Count > 0)
            {
                foreach (string k in keys)
                {
                    filter = StringUtils.AppendComma(filter) + StringUtils.EscapeSQL(k, keysEscapeSQL);
                }
            }

            if (!string.IsNullOrEmpty(filter))
                filter = String.Format("{0} IN ({1})", columnName, filter);
            else
                filter = String.Format("{0} IS NULL", columnName);

            return filter;
        }

        /// <summary>
        /// Creates string with DBMS_LOB.INSTR function call, that can be passed to
        /// SQL query. If this string is passed as criterium, only rows with CLOBs
        /// that conatins any word from words table should be returned.
        /// NOTE: Before search all words are converted to uppercase, and then passed
        /// to function - as consecuence, CLOB field by which search is performed
        /// should contains text all in uppercase.
        /// </summary>
        /// <param name="dbFieldName">Name of data base CLOB field to search in.</param>
        /// <param name="words">Words to search in CLOB.</param>
        /// <returns></returns>
        public static string ClobContainsAnyWord(string dbFieldName, string[] words)
        {
            return ClobContainsWord(dbFieldName, words, "OR");
        }

        /// <summary>
        /// Creates string with DBMS_LOB.INSTR function call, that can be passed to
        /// SQL query. If this string is passed as criterium, only rows with CLOBs
        /// that conatins all words from words table should be returned.
        /// NOTE: Before search all words are converted to uppercase, and then passed
        /// to function - as consecuence, CLOB field by which search is performed
        /// should contains text all in uppercase.
        /// </summary>
        /// <param name="dbFieldName">Name of data base CLOB field to search in.</param>
        /// <param name="words">Words to search in CLOB.</param>
        /// <returns></returns>
        public static string ClobContainsAllWords(string dbFieldName, string[] words)
        {
            return ClobContainsWord(dbFieldName, words, "AND");
        }

        private static string ClobContainsWord(string dbFieldName, string[] words, string wordsOperator)
        {
            StringBuilder sqlBuild = new StringBuilder();

            if (dbFieldName != null && dbFieldName.Length > 0
                && words != null && words.Length > 0)
            {
                foreach (string singleWord in words)
                {
                    if (singleWord != null && singleWord.Length > 0)
                    {
                        if (sqlBuild.Length > 0)
                        {
                            sqlBuild.Append(" ").Append(wordsOperator);
                        }
                        sqlBuild.Append(" DBMS_LOB.INSTR(")
                            .Append("UPPER(")
                            .Append(dbFieldName)
                            .Append(")")
                            .Append(", ")
                            .Append(EscapeSQL(singleWord).ToUpper())
                            .Append(", 1, 1) > 0");
                    }
                }
                if (sqlBuild.Length > 0)
                {
                    sqlBuild.Insert(0, "( ").Append(" )");
                }
            }
            return sqlBuild.ToString();
        }

        public static string VarcharContainsAllWords(string dbFieldName, string[] words)
        {
            return VarcharContainsWord(dbFieldName, words, "AND");
        }

        private static string VarcharContainsWord(string dbFieldName, string[] words, string wordsOperator)
        {
            StringBuilder sqlBuild = new StringBuilder();

            if (dbFieldName != null && dbFieldName.Length > 0
                && words != null && words.Length > 0)
            {
                foreach (string singleWord in words)
                {
                    if (singleWord != null && singleWord.Length > 0)
                    {
                        if (sqlBuild.Length > 0)
                        {
                            sqlBuild.Append(" ").Append(wordsOperator);
                        }
                        sqlBuild.Append(" INSTR(")
                            .Append("UPPER(")
                            .Append(dbFieldName)
                            .Append(")")
                            .Append(", ")
                            .Append(EscapeSQL(singleWord).ToUpper())
                            .Append(", 1, 1) > 0");
                    }
                }
                if (sqlBuild.Length > 0)
                {
                    sqlBuild.Insert(0, "( ").Append(" )");
                }
            }
            return sqlBuild.ToString();
        }

        public static string BuildCommaSeparatedSequence(bool escapeValues, params string[] values)
        {
            return BuildCommaSeparatedSequence(new List<String>(values), escapeValues);
        }

        public static string BuildCommaSeparatedSequence(List<String> values, bool escapeValues)
        {
            StringBuilder sequence = new StringBuilder();

            for (int i = 0; i < values.Count; i++)
            {
                String value = values[i];

                if (!string.IsNullOrEmpty(value))
                {
                    if (sequence.Length > 0 && !string.IsNullOrEmpty(value))
                    {
                        sequence.Append(", ");
                    }

                    if (escapeValues)
                    {
                        value = StringUtils.EscapeSQL(value);
                    }

                    sequence.Append(value);
                }
            }

            return sequence.ToString();
        }

        #endregion UTILS FOR SQL

        #region LINE END CONVERSION

        /// <summary>
        /// Converts line ends in given string from DOS format to UNIX format.
        /// </summary>
        /// <param name="str">String to convert.</param>
        /// <returns>String with UNIX line end characters.</returns>
        public static string ConvertLineEndDOSToUNIX(string str)
        {
            if (str != null && str.Length > 0)
            {
                return str.Replace(StringUtils.DOS_LINE_END, StringUtils.UNIX_LINE_END);
            }
            return string.Empty;
        }

        /// <summary>
        /// Converts line ends in given string from UNIX format to DOS format.
        /// </summary>
        /// <param name="str">String to convert.</param>
        /// <returns>String with DOS line end characters.</returns>
        public static string ConvertLineEndUNIXToDOS(string str)
        {
            if (str != null && str.Length > 0)
            {
                return str.Replace(StringUtils.UNIX_LINE_END, StringUtils.DOS_LINE_END);
            }
            return string.Empty;
        }

        #endregion LINE END CONVERSION

        #region OTHER

        /// <summary>
        /// Gets string containing all characters from string1 that exists in string2
        /// (all characters from string2 are removed)
        /// </summary>
        /// <param name="string1">source string</param>
        /// <param name="string2">filter string</param>
        /// <returns></returns>
        public static string GetSubSet(string string1, string string2)
        {
            string sRes = "";
            string oneChar;
            if (string1 != null && string2 != null)
            {
                for (int i = 0; i < string1.Length; i++)
                {
                    oneChar = string1.Substring(i, 1);
                    if (string2.Contains(oneChar))
                    {
                        sRes += oneChar;
                    }
                }
            }
            return sRes;
        }

        /// <summary>
        /// Returns first line of given string
        /// </summary>
        /// <param name="text">source string</param>
        /// <returns></returns>
        public static string GetFirstLineText(string text)
        {
            string ret = string.Empty;
            if (text != string.Empty)
            {
                text = text.Replace("\r", string.Empty);
                int i = text.IndexOf("\n");
                if (i > -1)
                {
                    ret = text.Substring(0, i);
                }
                else
                {
                    if (text.Trim().Length > 0)
                    {
                        ret = text;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Returns first line of given string
        /// </summary>
        /// <param name="text">source string</param>
        /// <param name="isRichTextBox">if provided text come from RichTextBox</param>
        /// <returns></returns>
        public static bool StringIn(this string string1, params string[] strings)
        {
            foreach (string s in strings)
            {
                if (s == string1)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Create string which can be used in SQL statement for example  "IN () "
        /// </summary>
        /// <param name="itemRows">rows</param>
        /// <param name="columnCode">column code</param>
        /// <returns></returns>
        public static StringBuilder PrepareCodesToSearchUsingIn(DataRow[] itemRows, string columnCode)
        {
            StringBuilder str = new StringBuilder();
            DataRow row;
            string code;
            int i = 0;
            while (i < itemRows.Length)
            {
                row = itemRows[i];
                if (row != null && row.RowState != DataRowState.Deleted
                    && row[columnCode] != null)
                {
                    code = row[columnCode].ToString();
                    if (code.Length > 0)
                    {
                        if (str.Length > 0)
                        {
                            str.Append(",");
                        }
                        str.Append(StringUtils.EscapeSQL(code));
                    }
                }
                i++;
            }
            return str;
        }

        /// <summary>
        /// Create string which can be used in SQL statement for example  "IN () "
        /// </summary>
        /// <param name="itemLists">List of strings to prepare</param>
        /// <returns></returns>
        public static StringBuilder PrepareCodesToSearchUsingIn(System.Collections.Generic.List<string> itemLists)
        {
            return PrepareCodesToSearchUsingIn(itemLists.ToArray());
        }

        /// <summary>
        /// Create string which can be used in SQL statement for example  "IN () "
        /// </summary>
        /// <param name="itemLists">array of strings to prepare</param>
        /// <returns></returns>
        public static StringBuilder PrepareCodesToSearchUsingIn(string[] itemLists)
        {
            StringBuilder str = new StringBuilder();
            if (itemLists != null)
            {
                foreach (string item in itemLists)
                {
                    if (item != null)
                    {
                        if (str.Length > 0)
                        {
                            str.Append(",");
                        }
                        str.Append(StringUtils.EscapeSQL(item));
                    }
                }
            }
            return str;
        }

        /// <summary>
        /// Create string which can be used in SQL statement for example  "IN () "
        /// </summary>
        /// <param name="itemLists">List of longs to prepare</param>
        /// <returns></returns>
        public static StringBuilder PrepareCodesToSearchUsingIn(System.Collections.Generic.List<long> itemLists)
        {
            List<string> convertedList = new List<string>();
            foreach (long longValue in itemLists)
            {
                convertedList.Add(Convert.ToString(longValue));
            }
            return PrepareCodesToSearchUsingIn(convertedList.ToArray());
        }

        public static StringBuilder PrepareCodesToSearchUsingIn(IEnumerable<string> enumerableString)
        {
            StringBuilder str = new StringBuilder();
            IEnumerator e = enumerableString.GetEnumerator();
            while (e.MoveNext())
            {
                string item = e.Current.ToString();
                if (!string.IsNullOrEmpty(item))
                {
                    if (str.Length > 0)
                    {
                        str.Append(",");
                    }
                    str.Append(StringUtils.EscapeSQL(item));
                }
            }
            e.Reset();
            return str;
        }

        public static string GetNewPipedItems(string oldItems, string newItems)
        {
            List<string> addedItemList = new List<string>();
            List<string> oldItemList = new List<string>((oldItems ?? string.Empty).Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            List<string> newItemList = new List<string>((newItems ?? string.Empty).Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            foreach (string newItem in newItemList)
            {
                if (!oldItemList.Contains(newItem))
                {
                    addedItemList.Add(newItem);
                }
            }
            addedItemList.Sort();
            return string.Join("|", addedItemList.ToArray());
        }

        public static bool IsPipedValueChanged(string prevValue, string newValue)
        {
            bool isChanged = false;
            if (prevValue != newValue)
            {
                List<string> oldItemList = new List<string>((prevValue ?? string.Empty).Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                List<string> newItemList = new List<string>((newValue ?? string.Empty).Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                if (oldItemList.Count == newItemList.Count)
                {
                    oldItemList.Sort();
                    newItemList.Sort();
                    for (int i = 0; i < newItemList.Count; i++)
                    {
                        if (string.Compare(oldItemList[i], newItemList[i]) != 0)
                        {
                            isChanged = true;
                            break;
                        }
                    }
                }
                else
                {
                    isChanged = true;
                }
            }
            return isChanged;
        }

        [Flags]
        public enum SplitOptions
        {
            None = 0,
            RemoveEmptyEntries = 1 << 0,
            RemoveEscapeCharacter = 1 << 1,
        }

        public static IEnumerable<string> Split(this string input,
                                        string separator,
                                        char escapeCharacter)
        {
            return Split(input, separator, escapeCharacter, SplitOptions.None);
        }

        public static IEnumerable<string> Split(this string input,
                                        string separator,
                                        char escapeCharacter, SplitOptions options)
        {
            int startOfSegment = 0;
            int index = 0;
            string result;
            while (index < input.Length)
            {
                index = input.IndexOf(separator, index);
                if (index > 0 && input[index - 1] == escapeCharacter)
                {
                    index += separator.Length;
                    continue;
                }

                if (index == -1)
                {
                    break;
                }

                result = input.Substring(startOfSegment, index - startOfSegment);
                if (options.HasFlag(SplitOptions.RemoveEscapeCharacter))
                {
                    result = Tools.RetriveSubString(ref result, separator, escapeCharacter);
                }

                if (!(string.IsNullOrEmpty(result) && options.HasFlag(SplitOptions.RemoveEmptyEntries)))
                {
                    yield return result;
                }

                index += separator.Length;
                startOfSegment = index;
            }

            result = input.Substring(startOfSegment);
            if (options.HasFlag(SplitOptions.RemoveEscapeCharacter))
            {
                result = Tools.RetriveSubString(ref result, separator, escapeCharacter);
            }

            if (!(string.IsNullOrEmpty(result) && options.HasFlag(SplitOptions.RemoveEmptyEntries)))
            {
                yield return result;
            }
        }

        public static string Truncate(this string source, int length)
        {
            if (source == null)
                return null;
            if (source.Length <= length)
                return source;
            return source.Substring(0, length);
        }

        #endregion OTHER

        #region COMPARE

        /// <summary>
        /// Compares 2 strings. If strinngs contain integer values it compares these vales separately
        /// Ex: X1-X20 > X1-X1
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static int Compare(string v1, string v2)
        {
            int minLength = 0;
            int compared = 0;
            double number1 = 0;
            double number2 = 0;
            string s1 = "";
            string s2 = "";
            if (IsNotEmpty(v1) && IsNotEmpty(v2))
            {
                if (v1.Length > v2.Length)
                {
                    minLength = v2.Length;
                }
                else
                {
                    minLength = v1.Length;
                }
                for (int i = 0; i < minLength; i++)
                {
                    if (char.IsDigit(v1[i]) && char.IsDigit(v2[i]))
                    {
                        int n1 = 1, n2 = 1;
                        for (int i1 = i + 1; i1 < v1.Length; i1++)
                        {
                            if (char.IsDigit(v1[i1]))
                                n1++;
                            else
                                break;
                        }
                        for (int i2 = i + 1; i2 < v2.Length; i2++)
                        {
                            if (char.IsDigit(v2[i2]))
                                n2++;
                            else
                                break;
                        }
                        s1 = v1.Substring(i, n1);
                        s2 = v2.Substring(i, n2);
                        number1 = double.Parse(s1);
                        number2 = double.Parse(s2);
                        if (number1 > number2)
                            return 1;
                        if (number1 < number2)
                            return -1;

                        i += n1 - 1;
                        compared = 0;
                    }
                    else
                    {
                        compared = System.Collections.Comparer.Default.Compare(v1[i], v2[i]);
                    }

                    if (compared != 0)
                    {
                        return compared;
                    }
                }
                if (v1.Length > v2.Length)
                    return 1;
                else if (v1.Length == v2.Length)
                    return 0;
                else
                    return -1;
            }
            else
            {
                return System.Collections.Comparer.Default.Compare(v1, v2);
            }
        }

        #endregion COMPARE

        #region SpecialChars

        /// <summary>
        /// Converts character to its hexadecimal representation
        /// in the format suitable for the C# XML parser (e.g. _x007C_).
        /// </summary>
        /// <param name="charToConvert">character that will be converted.</param>
        /// <returns>character in the proper hexadecimal representation.</returns>
        private static String ConvertCharToHex(char charToConvert)
        {
            return String.Format("_x{0:X4}_", (int)Char.ToUpper(charToConvert));
        }

        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled);
        }

        public static bool ContainsSpecialChars(String input)
        {
            bool res = false;
            foreach (char character in input)
            {
                if ((character >= 0x00 && character <= 0x2C) || (character == 0x2F) || (character >= 0x3A && character <= 0x40) || (character >= 0x5B && character <= 0x5E) || (character == 0x60) || (character >= 0x7B && character <= 0xB6) || (character >= 0xB8 && character <= 0xBF) || (character == 0xD7) || (character == 0xF7))
                {
                    res = true;
                }
            }
            return res;
        }

        /// <summary>
        /// Replaces special characters from the given string to their hexadecimal notation.
        /// This function is useful, in XML processing, to substitute characters that are not
        /// allowed in element names.
        /// </summary>
        /// <param name="input">Input string to change characters.</param>
        /// <returns>Corrected string</returns>
        ///
        public static String ReplaceSpecialChars(String input)
        {
            if (input != null && !input.Equals(""))
            {
                String hex = "";
                char[] invalidchars = new char[]{'|', '\\', '+', '=', '-',
                                             '(', ')', '*', '&', '^',
                                             '%', '$', '#', '@', '!',
                                             '`', '~', '/', '?', '\'',
                                             '"', ';', ':', '{', '}',
                                             '[', ']', '<', '>', '.',
                                             ',', ' '};

                for (int i = 0; i < invalidchars.Length; i++)
                {
                    input = input.Replace(invalidchars[i].ToString(), ConvertCharToHex(invalidchars[i]));
                }

                if (Char.IsDigit(input[0]))
                {
                    hex = ConvertCharToHex(input[0]);
                    input = hex + (input.Length > 1 ? input.Substring(1) : "");
                }
            }
            return input;
        }

        /// <summary>
        /// Removes given characters, or replace doubled on one i.e. '&amp;' -> '' or '&amp;&amp;' -> '&amp;'
        /// </summary>
        /// <param name="input">Input string to remove characters</param>
        /// <param name="signs"></param>
        /// <returns>string without given characters</returns>
        public static String RemoveCharsFromString(string input, params char[] signs)
        {
            if (null != signs)
            {
                foreach (char sign in signs)
                {
                    input = RemoveCharsWhenNotDouble(input, sign);
                }
            }
            return input;
        }

        private static String RemoveCharsWhenNotDouble(string input, char sign)
        {
            StringBuilder sb = null;
            if (!string.IsNullOrEmpty(input))
            {
                sb = new StringBuilder(string.Empty);
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i].Equals(sign) && (i + 1) < input.Length && input[i + 1].Equals(sign))
                    {
                        sb.Append(input[i]);
                        i++;
                    }
                    else
                    {
                        if (!input[i].Equals(sign))
                        {
                            sb.Append(input[i]);
                        }
                    }
                }
            }
            return string.IsNullOrEmpty(input) ? input : sb.ToString();
        }

        /// <summary>
        /// This function is unsafe. It can broke \pard tag.
        /// Please use RtfTools.RemoveLastParagraphMark
        /// </summary>
        /// <param name="rtfText"></param>
        /// <returns></returns>
        public static string RemoveParagraphMark(string rtfText)
        {
            string result = rtfText;
            if (!string.IsNullOrEmpty(rtfText))
            {
                string newLineChar = "\\par";
                int index = rtfText.LastIndexOf(newLineChar);
                if (index != -1)
                {
                    result = rtfText.Remove(index, newLineChar.Length);
                }
            }
            return result;
        }

        public static bool ContainsAny(this string s, params char[] args)
        {
            foreach (char arg in args)
            {
                if (s.Contains(arg.ToString()))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Replace non-ASCII character with '?'
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetASCIIString(this string text)
        {
            string result = null;
            if (!string.IsNullOrEmpty(text))
            {
                StringBuilder sb = new StringBuilder(text);
                for (int i = 0; i < text.Length; i++)
                {
                    char ch = sb[i];
                    if (!IsPrintableASCII(ch))
                    {
                        sb[i] = '?';
                    }
                }
                result = sb.ToString();
            }
            return result;
        }

        private static bool IsPrintableASCII(char ch)
        {
            return (int)ch >= 9 && (int)ch <= 13 || (int)ch >= 32 && (int)ch <= 127;
        }

        #endregion SpecialChars

        #region Length
        public static bool IsInRange(this IEnumerable list, int index)
        {
            int length = -1;

            if ((list is ICollection))
            {
                length = ((ICollection)list).Count;
            }
            else if (list is string)
            {
                length = ((string)list).Length;
            }

            return index >= 0 && index < length;
        }

        #endregion Length

        public static string Join<T>(ICollection<T> collection)
        {
            return Join(collection.GetEnumerator(), null);
        }

        public static string Join<T>(ICollection<T> collection, string separator)
        {
            return Join(collection.GetEnumerator(), separator);
        }

        private static string Join<T>(IEnumerator<T> enumerator, string separator)
        {
            if (enumerator == null)
            {
                return null;
            }

            if (!enumerator.MoveNext())
            {
                return string.Empty;
            }

            Object first = enumerator.Current;
            StringBuilder buf = new StringBuilder();
            // two or more elements
            if (first != null)
            {
                buf.Append(first);
            }

            while (enumerator.MoveNext())
            {
                if (separator != null)
                {
                    buf.Append(separator);
                }

                Object obj = enumerator.Current;
                if (obj != null)
                {
                    buf.Append(obj);
                }
            }

            return buf.ToString();
        }

        public static string DefaultString(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return str;
        }

        public static string JoinWords(string separator, IEnumerable<string> words)
        {
            return JoinWords(separator, words, 0);
        }

        public static string JoinWords(string separator, IEnumerable<string> words, int maxLen)
        {
            Tools.ArgumentValidatorHelper.IsNotNull(separator, "separator");
            Tools.ArgumentValidatorHelper.IsNotEmptyString(separator, "separator");

            const string POSTFIX = "...";
            string result = "";
            if (words == null || !words.Any())
                return result;

            result = String.Join(separator, words.Where(word => word.IsNotNullNorEmptyString()));
            if (maxLen > 0 && result.Length > maxLen)
            {
                int end = result.LastIndexOf(separator, maxLen - POSTFIX.Length + separator.Length);
                if (end > 0)
                {
                    result = result.Substring(0, end) + POSTFIX;
                }
                else if (words.First().ConvertToString().Length <= maxLen)
                {
                    result = words.First();
                }
                else if (POSTFIX.Length <= maxLen)
                {
                    result = POSTFIX;
                }
                else
                {
                    result = string.Empty;
                }
            }
            return result;
        }
    }
}