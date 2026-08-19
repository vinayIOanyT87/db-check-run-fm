// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSetValueRetreiver.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses
{
	using System.Data;

	public class DataSetValueRetreiver
	{
		#region Public Methods and Operators

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <param name="columnname">The columnname.</param>
		/// <returns>The value or null if exception occurs.</returns>
		public static object GetValue(DataRow row, string columnname)
		{
			if (row == null)
			{
				return null;
			}

			object o;
			try
			{
				o = row[columnname];
			}
			catch
			{
				o = null;
			}

			return o;
		}

		#endregion
	}
}