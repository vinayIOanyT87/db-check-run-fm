// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Common.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.ServiceClasses;

	/// <summary>
	///     This class contains shared methods among classes.
	///     It typically helper methods not big enough to be in its own class.
	/// </summary>
	internal class Common
	{
		#region Public Methods and Operators

		/// <summary>
		/// Checks whether the given string is a reserved key word (for ID)
		/// </summary>
		/// <param name="mySecurity">My security.</param>
		/// <param name="src">The SRC.</param>
		/// <returns>
		///   <c>true</c> if [is reserved word for ID] [the specified my security]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsReservedWordForID(SecurityClass mySecurity, string src)
		{
			string[] plainReservedList = { "{None}", "{Unassigned}", "{All}" };
			var allReservedWordList = new HashSet<string>(plainReservedList);

			// Get the translated list and append to the end
			var dataDictionaries = new DataDictionariesClass();
			foreach (string reservedWord in plainReservedList)
			{
				allReservedWordList.Add(dataDictionaries.Get(mySecurity.SiteGuid, reservedWord));
			}

			return allReservedWordList.Any(word => string.Compare(word, src, StringComparison.OrdinalIgnoreCase) == 0);
		}

		#endregion
	}
}