using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMBusinessObjects.UtilityObjects
{
	using System.Data;

	/// <summary>
	/// Extension methods for Guid classes
	/// </summary>
	public static class GuidExtension
	{
		/// <summary>
		/// Test whether the given Guid is empty or not
		/// </summary>
		/// <param name="self"></param>
		/// <returns>True if empty and false otherwise</returns>
		public static bool IsEmpty(this Guid self)
		{
			return self == Guid.Empty;
		}

		/// <summary>
		/// Test whether the given Guid is not empty and not equal to the specified other Guid
		/// </summary>
		/// <param name="self">This Guid</param>
		/// <param name="otherGuid">The other Guid to check for equality</param>
		/// <returns>True if not empty and not equal to the specified other Guid. False otherwise.</returns>
		public static bool IsNotEmptyAndNotEqualTo(this Guid self, Guid otherGuid)
		{
			return (!self.IsEmpty()) && (self != otherGuid);
		}

		/// <summary>
		/// Test whether the given Guid is not empty and equal to the specified other Guid
		/// </summary>
		/// <param name="self">This Guid</param>
		/// <param name="otherGuid">The other Guid to check for equality</param>
		/// <returns>True if not empty and not equal to the specified other Guid. False otherwise.</returns>
		public static bool IsNotEmptyAndEqualTo(this Guid self, Guid otherGuid)
		{
			return (!self.IsEmpty()) && (self == otherGuid);
		}
	}

	public static class StringBuilderExtension
	{
		#region Check for Null or Empty
		/// <summary>
		/// Get if string is null or the empty string.
		/// </summary>
		public static bool IsNullOrEmpty(this StringBuilder pString)
		{
			return (pString == null || pString.Length == 0);
		}
		#endregion

		#region AppendLine Conditionally
		/// <summary>
		/// Performs an AppendLine using the passed in value ONLY if the passed in value is not null or empty.
		/// </summary>
		/// <remarks>
		/// If the passed in string already ends with a NewLine, a standard Append is used.
		/// </remarks>
		public static StringBuilder AppendIffLine(this StringBuilder pString, string pValue)
		{
			if (!string.IsNullOrEmpty(pValue))
			{
				// Only add it if it's not empty and not a blank line.
				if (!pValue.Equals(Environment.NewLine))
				{
					if (pValue.EndsWith(Environment.NewLine))
					{
						pString.Append(pValue);
					}
					else
					{
						pString.AppendLine(pValue);
					}
				}
			}

			return (pString);
		}
		#endregion AppendLine Conditionally

		#region AppendWithDelimiter Conditionally
		/// <summary>
		/// Performs an AppendLine using the passed in value ONLY if the passed in value is not null or empty.
		/// </summary>
		/// <remarks>
		/// If the passed in string already ends with a NewLine, a standard Append is used.
		/// </remarks>
		public static StringBuilder AppendIffDelimited(this StringBuilder pString, string pValue, string pDelimiter)
		{
			if (!string.IsNullOrEmpty(pValue))
			{
				// Only add it if it's not empty and not a blank line.
				if (!pValue.Equals(Environment.NewLine))
				{
					if (!string.IsNullOrEmpty(pDelimiter) && !pString.IsNullOrEmpty())
					{
						pString.Append(pDelimiter);
					}

					pString.Append(pValue);
				}
			}

			return (pString);
		}
		#endregion AppendWithDelimiter Conditionally

	}

	public static class DataRowExtension
	{
		public static object SafeValue(this DataRow self, string columnName)
		{
			return (self.Table.Columns.Contains(columnName)) ? (object)self[columnName] : DBNull.Value;
		}

	}
}
