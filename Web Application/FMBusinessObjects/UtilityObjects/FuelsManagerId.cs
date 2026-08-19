// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelsManagerId.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//  Defines the FuelsManagerId type.  Used to generate transaction IDs in the formate
//  required by FuelsManager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.UtilityObjects
{
	using System;

	/// <summary>
	///   Class for centralizing generation of FuelsManager IDs
	/// </summary>
	public class FuelsManagerId
	{
		#region Public Methods and Operators

		/// <summary>
		///   Function to generate a new id.  Currently a simple GUID with no punctuation
		/// </summary>
		/// <returns> Unpunctuated GUID as a string </returns>
		public static string NewId()
		{
			return Guid.NewGuid().ToString("N");
		}

		#endregion
	}
}