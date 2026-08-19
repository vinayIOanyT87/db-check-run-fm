// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelsManagerExtensions.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FuelsManagerExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMCore
{
    using System;
    using System.Collections;
	using System.Collections.Generic;
	using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Net.Mail;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Web;
	using System.Text.RegularExpressions;

	public static class FuelsManagerExtensions
	{
        /// <summary>
        /// (Varec) This method returns a default value for the string object if the object is null
        /// </summary>
        public static string DefaultIfNull(this String value, string defaultValue)
        {
            return (value == null) ? defaultValue : value;
        }

        /// <summary>
        /// (Varec) This method returns a default value for the string object if the object is null or empty.
        /// </summary>
        public static string DefaultIfNullOrEmpty(this String value, string defaultValue)
		{
			return (string.IsNullOrEmpty(value)) ? defaultValue : value;
		}

		/// <summary>
		/// (Varec) This method returns true if the string is NOT equal to the checkValue parameter
		/// </summary>
		/// <param name="checkValue">Value to compare</param>
		/// <returns>bool</returns>
		public static bool NotEquals(this String value, string checkValue)
		{
			return !value.Equals(checkValue);
		}

		/// <summary>
		/// (Varec) This method returns true if the string is NOT equal to the checkValue parameter
		/// </summary>
		/// <param name="checkValue">Value to compare</param>
		/// <returns>bool</returns>
		public static bool NotEquals(this string value, string checkValue, StringComparison comparisonValue)
		{
			return !value.Equals(checkValue, comparisonValue);
		}

		/// <summary>
		/// (Varec) Removes spaces from the string
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string RemoveSpaces(this String value)
		{
			return value.Replace(" ", String.Empty);
		}

		/// <summary>
		/// (Varec) This method returns the contents of the DataSet as XML in a string object.
		/// </summary>
		/// <returns>string</returns>
		public static string ToStringAsXml(this DataSet dataSet)
		{
			StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
			dataSet.WriteXml(sw, XmlWriteMode.IgnoreSchema);
			return sw.ToString();
		}

		/// <summary>
		/// (Varec) This method returns the contents of the table as XML in a string object.
		/// </summary>
		/// <returns>string</returns>
		public static string ToStringAsXml(this DataTable dt)
		{
			StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
			dt.WriteXml(sw, XmlWriteMode.IgnoreSchema);
			return sw.ToString();
		}

		/// <summary>
		/// (Varec) This method returns a four character string with the US military version of a julian date.  
		/// The first character is the last digit of the year.  The last three are the number of the 
		/// day in the year.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static string ToMilitaryJulianDateString(this DateTimeOffset date)
		{
			return date.Year.ToString("0000", CultureInfo.InvariantCulture).Substring(3, 1) + date.DayOfYear.ToString("000", CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// (Varec) This method splits a compound word into separate words at UpperCase breaks.  For example,
		/// ThisClassID becomes "This Class ID"
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string SplitIntoWords(this string value)
		{
			StringBuilder sb = new StringBuilder();

			foreach (char c in value)
			{
				if (Char.IsUpper(c)
				   && sb.Length != 0
				   && Char.IsUpper(sb[sb.Length - 1]) == false)
				{
					sb.Append(' ');
				}

				sb.Append(c);
			}

			return sb.ToString();
		}

		/// <summary>
		/// (Varec) This method removes a semicolon and any trailing text from a string.
		/// </summary>
		/// <param name="value">The input string to alter.</param>
		/// <returns>A string with any semicolon and trailing text removed.</returns>
		public static string RemoveSemicolonAndTextAfter(this string value)
		{
			int indexOfSemicolon = value.IndexOf(';');
			if (indexOfSemicolon != -1)
			{
				value = value.Substring(0, indexOfSemicolon);
			}

			return value;
		}

		/// <summary>
		/// (Varec) Checks string to see if it is a syntactically correct email address.  Consider using the
		/// FMEmailTextBox control instead of directly using this method.
		/// </summary>
		/// <param name="value">
		/// The string to test.
		/// </param>
		/// <returns>
		/// True if the string contains a valid email address syntactically
		/// </returns>
		public static bool IsValidEmailAddressSyntax(this string value)
		{
			// Consider an empty email address as valid
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}
			try
			{
				var mailAddress = new MailAddress(value);
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Trims the string to the specified maximum length.
		/// </summary>
		/// <param name="str">The string to trim.</param>
		/// <param name="length">The maximum length of the string to enforce.</param>
		/// <returns>The string trimmed (if necessary) to the maximum length.</returns>
		public static string TrimToMaxLength( this String str, int length )
		{
			if ( str != null && str.Length > length )
			{
				str = str.Substring( 0, length );
			}

			return str;
		}

		/// <summary>
		/// Retrieves value associated with the specified key from the QueryString or Form from
		/// the request object.
		/// </summary>
		/// <param name="request">The object to be extended.</param>
		/// <param name="key">The key to lookup.</param>
		/// <returns>The string value associated with the key.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static string GetQueryOrFormValue(this HttpRequest request, string key)
		{
			var value1 = request.QueryString[key];
			var value2 = request.Form[key];

			if ( value1 != null && value2 != null && value1 != value2 )
			{
				string message = string.Format("Duplicate key values found for page collections.\nKey={0}\nReferrer={1}", key, request.UrlReferrer);
				throw new Exception(message);
			}

			return value1 ?? value2;
		}

        /// <summary>
        /// Takes a collection of values and creates SqlParameters and a comma-delimited
        /// list for use in a SQL IN clause.
        /// </summary>
        /// <param name="paramCollection">Collection to which the new parameters will be
        /// appended</param>
        /// <param name="paramValues">The values for the parameters</param>
        /// <param name="baseParamName">The base name to use, e.g., "@Prm" for this
        /// would cause parameters to be created with names "@Prm1", "@Prm2", etc.</param>
        /// <param name="sqlDbType">Type of the parameters</param>
        /// <returns>Comma-delimited list of the parameters, e.g., "@Prm1, @Prm2, @Prm3"</returns>
        public static string ConstructSqlParametersFromCollection(SqlParameterCollection paramCollection, IEnumerable paramValues, string baseParamName, SqlDbType sqlDbType)
        {
            return ConstructSqlParametersFromCollection(paramCollection, paramValues, baseParamName, sqlDbType, -1);
        }

        /// <summary>
        /// Takes a collection of values and creates SqlParameters and a comma-delimited
        /// list for use in a SQL IN clause.
        /// </summary>
        /// <param name="paramCollection">Collection to which the new parameters will be
        /// appended</param>
        /// <param name="paramValues">The values for the parameters</param>
        /// <param name="baseParamName">The base name to use, e.g., "@Prm" for this
        /// would cause parameters to be created with names "@Prm1", "@Prm2", etc.</param>
        /// <param name="sqlDbType">Type of the parameters</param>
        /// <param name="size">Length of the parameters</param>
        /// <returns>Comma-delimited list of the parameters, e.g., "@Prm1, @Prm2, @Prm3"</returns>
        public static string ConstructSqlParametersFromCollection(SqlParameterCollection paramCollection, IEnumerable paramValues, string baseParamName, SqlDbType sqlDbType, int size)
        {
            System.Text.StringBuilder paramList = new System.Text.StringBuilder();
            int paramNumber = 0;

            if (!baseParamName.StartsWith("@"))
            {
                baseParamName = "@" + baseParamName;
            }

            foreach (object value in paramValues)
            {
                paramNumber++;
                string paramName = baseParamName + paramNumber.ToString(CultureInfo.InvariantCulture);

                paramList.Append(paramName + ",");

                SqlParameter param;
                if (size > 0)
                {
                    param = paramCollection.Add(paramName, sqlDbType, size);
                }
                else
                {
                    param = paramCollection.Add(paramName, sqlDbType);
                }

                if (value == null)
                {
                    param.Value = DBNull.Value;
                }
                else
                {
                    param.Value = value;
                }
            }

            if (paramList.Length > 0)
            {
                return paramList.ToString().TrimEnd(',');
            }

            return string.Empty;
        }

        /// <summary>
        /// This method will remove percent signs and escape any and all "'". It will return the 
        /// modified string or the if no change, then the original string.
        /// </summary>
        /// <param name="inStr">
        /// The base string object.
        /// </param>
        /// <returns>
        /// The escape like clause characters.
        /// </returns>
        public static string EscapeLikeClauseCharacters(string inStr)
        {
            string outStr = inStr;

            if (outStr.IndexOf('%') >= 0)
            {
                outStr = outStr.Replace("%", string.Empty);
            }

            if (outStr.IndexOf('\'') >= 0)
            {
                outStr = outStr.Replace("'", "''");
            }

            return outStr;
        }

        /// <summary>
        /// (Varec) Throws ArgumentNullException if the specified parameter object is null
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <param name="parameterName">The parameter name to report in the exception.</param>
        public static void ThrowIfNull(this object obj, string parameterName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// (Varec) Throws ArgumentNullException if the specified parameter object is null
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <param name="parameterName">The parameter name to report in the exception.</param>
        public static void ThrowIfNullOrEmpty(this string obj, string parameterName)
        {
            if (string.IsNullOrEmpty(obj))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

		public static string ToCsv(this DataTable table)
		{
			var result = new StringBuilder();
			for (var i = 0; i < table.Columns.Count; i++)
			{
				result.Append(table.Columns[i].ColumnName);
				result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
			}

			foreach (DataRow row in table.Rows)
			{
				for (var i = 0; i < table.Columns.Count; i++)
				{
					result.Append(row[i]);
					result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
				}
			}

			return result.ToString();
		}

		public static byte[] GetBytes(this string str)
		{
			var bytes = new byte[str.Length * sizeof(char)];
			Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		public static string GetString(this byte[] bytes)
		{
			var chars = new char[bytes.Length / sizeof(char)];
			Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}

		public static TimeSpan ToTimeSpan(this string timeStr)
		{
			DateTime dt;

			if (DateTime.TryParseExact(timeStr, "hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
				return dt.TimeOfDay;

			if (DateTime.TryParseExact(timeStr, "h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) 
				return dt.TimeOfDay;

			if (DateTime.TryParseExact(timeStr, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
				return dt.TimeOfDay;

			if (DateTime.TryParseExact(timeStr, "hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
				return dt.TimeOfDay;

			if (DateTime.TryParseExact(timeStr, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
				return dt.TimeOfDay;

			if (DateTime.TryParseExact(timeStr, "h:mmtt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
				return dt.TimeOfDay;

			if (DateTime.TryParseExact(timeStr, "hh:mmtt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
				return dt.TimeOfDay; 
			
			return TimeSpan.MinValue;
		}

		public static TimeSpan ToTimeSpan(this string timeStr, TimeSpan defaultScheduledTime)
		{
			var scheduledTime = timeStr.ToTimeSpan();
			return scheduledTime.Equals(TimeSpan.MinValue) ? defaultScheduledTime : scheduledTime;
		}


		/// <summary>
		/// Converts populated DateTimeOffset columns in a DataTable into DateTime columns so that the offset is not shown on the table
		/// </summary>
		/// <param name="table">The table to have the columns converted</param>
		public static void ConvertDateTimeOffsetColumns(this DataTable table)
		{
			//populate a list of DateTimeOffset columns to convert
			List<string> columnsToConvert = new List<string>();
			foreach (DataColumn dc in table.Columns)
			{
				if (dc.DataType == typeof(DateTimeOffset))
				{
					columnsToConvert.Add(dc.ColumnName);
				}
			}

			Type newType = typeof(DateTime);
			foreach (string columnName in columnsToConvert)
			{
				using (DataColumn dc = new DataColumn(columnName + "_new", newType))
				{
					// create a new column with the same ordinal and corrected type
					int ordinal = table.Columns[columnName].Ordinal;
					table.Columns.Add(dc);
					dc.SetOrdinal(ordinal);

					// convert the values of the old column
					foreach (DataRow dr in table.Rows)
					{
						//need to use Reflection as we don't know the properties until runtime
						Type ObjectType = dr[columnName].GetType();
						System.Reflection.PropertyInfo propertyInfo = ObjectType.GetProperty("DateTime");
						//make sure the property is there and populated before trying to convert it
						if (propertyInfo != null && !String.IsNullOrEmpty(propertyInfo.ToString()))
							dr[dc.ColumnName] = propertyInfo.GetValue(dr[columnName]);
					}
					// Remove the old column
					table.Columns.Remove(columnName);

					// Rename the new column
					dc.ColumnName = columnName;
				}
			}

		}

		public static bool IsStrongPassword(string pwd)
		{
			bool result = true;

			if (string.IsNullOrEmpty(pwd) || (pwd.Length <= 0))
			{
				result = false;
			}
			else
			{
				// Must contain at least one one lower case letter, one upper case letter, one digit and one special character.
				// Valid special characters (which are configurable) are -   @#$%^&+=
				// ^.*(?=.{10,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$
				// ^                      # anchor at the start
				// .*					  # wild card
				// (?=.*\d)               # must contain at least one numeric character
				// (?=.*[a-z])            # must contain one lowercase character
				// (?=.*[A-Z])            # must contain one uppercase character
				// (?=.*[@#$%^&+.,!=]).*  # must contain one special character
				// $                      # anchor at the end
				var regex = new Regex(@"^.*(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+.,!=]).*$");
				result = regex.IsMatch(pwd);
			}

			return result;
		}

		public static bool IsEnhancedStrongPassword(string pwd)
		{
			bool result = true;

			if (string.IsNullOrEmpty(pwd) || (pwd.Length <= 0))
			{
				result = false;
			}
			else
			{
				// Must contain at least one one lower case letter, one upper case letter, one digit and one special character.
				// Valid special characters (which are configurable) are -   @#$%^&+=
				// ^.*(?=.{10,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$
				// ^                         # anchor at the start
				// .*							     # wild card
				// (?=.*\d{2})               # must contain at least two numeric characters
				// (?=.*[a-z]{2})            # must contain at least two lowercase characters
				// (?=.*[A-Z]{2})            # must contain at least two uppercase characters
				// (?=.*[@#$%^&+.,!=]{2}).*  # must contain at least two special characters
				// $								  # anchor at the end

				// System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(
				// @"^.*(?=.*\d{2})(?=.*[a-z]{2})(?=.*[A-Z]{2})(?=.*[@#$%^&+.,!=]{2}).*$");

				// Right now, the single-pass regex is not working.  For now, in the interest of coding speed, use multiple simple
				// checks.

				// must contain at least two numeric characters
				Regex regex = new Regex(@"\d");
				MatchCollection matches = regex.Matches(pwd);
				if (matches.Count < 2)
				{
					result = false;
				}

				// must contain at least two lowercase characters
				regex = new Regex(@"[a-z]");
				matches = regex.Matches(pwd);
				if (matches.Count < 2)
				{
					result = false;
				}

				// must contain at least two uppercase characters
				regex = new Regex(@"[A-Z]");
				matches = regex.Matches(pwd);
				if (matches.Count < 2)
				{
					result = false;
				}

				// must contain at least two special characters
				regex = new Regex(@"[^a-zA-Z0-9]");
				matches = regex.Matches(pwd);
				if (matches.Count < 2)
				{
					result = false;
				}
			}


			return result;
		}

	}
}
