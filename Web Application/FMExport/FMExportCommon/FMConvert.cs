// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMConvert.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Common type conversions used by the FMExport service and custom aviation interfaces.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportService
{
	using System;
    using System.IO;
    using System.Reflection;

	/// <summary>
	/// Common type conversions used by the FMExport service and custom aviation interfaces.
	/// </summary>
	public static class FMConvert
	{
        
        public static string GetAssemblyDirectory()
        {
            var uri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
            return Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path)).TrimEnd("\\".ToCharArray()) + "\\";
        }

		/// <summary>
		/// Convert a DateTime object to a string with the format of "yyyyMMdd" or
		/// "yyyyMMdd HH:mm:ss" depending on whether the dateOnly parameter is true.
		/// </summary>
		/// <param name="dt">The DateTime object </param>
		/// <param name="dateOnly">Indicates that only the date should be included in the output string</param>
		/// <returns>The string representation of the DateTime object</returns>
		public static string ConvertDateTimeToString(DateTime dt, bool dateOnly)
		{
			if (dateOnly)
			{
				return dt.ToString("yyyyMMdd");
			}

			return dt.ToString("yyyyMMdd HH:mm:ss");
		}

		/// <summary>
		/// Converts the specified object to a string.  If the object is a string then the object itself
		/// is returned.  If the object is a DateTime or DateTimeOffset type then the object is converted
		/// to a string using a custom conversion method.  Otherwise the empty string is returned.
		/// </summary>
		/// <param name="o">The object to convert to a string</param>
		/// <param name="dateOnly">>Indicates that only the date should be included in the output string</param>
		/// <returns>The string representation of the DateTime object</returns>
		public static string ConvertCellToString(object o, bool dateOnly)
		{
			string retVal = string.Empty;

			if (o is string)
			{
				retVal = (string)o;
				return retVal;
			}

			if (o is DateTime)
			{
				retVal = ConvertDateTimeToString((DateTime)o, dateOnly);
				return retVal;
			}

			if (o is DateTimeOffset)
			{
				retVal = ConvertDateTimeToString(((DateTimeOffset)o).DateTime, dateOnly);
				return retVal;
			}

			return retVal;
		}

        /// <summary>
        /// Converts the specified object to a string.  If the object is a string then the object itself
        /// is returned.  If the object is a DateTime or DateTimeOffset type then the object is converted
        /// to a string using a custom conversion method.  Otherwise the empty string is returned.
        /// </summary>
        /// <param name="o">The object to convert to a string</param>
        /// <param name="dateOnly">>Indicates that only the date should be included in the output string</param>
        /// <returns>The string representation of the DateTime object</returns>
        public static DateTime ConvertCellToDateTime(object o, bool dateOnly = false)
        {
            DateTime retVal = DateTime.MinValue;

            if( o == null)
            {
                return retVal;
            }

            if (o is DateTime)
            {
                retVal = (DateTime)o;
            }
            else
            {

                if (o is DateTimeOffset)
                {
                    retVal = ((DateTimeOffset)o).DateTime;
                }
            }
            return (dateOnly) ? retVal.Date : retVal;
        }
	}
}
