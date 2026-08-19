// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataDictionary.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Implement this interface to participate in the FuelsManager data dictionary translation system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// Implement this interface to participate in the FuelsManager data dictionary translation system.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", 
		Justification = "Existing interface that follows Varec Standard.")]
	public interface IDataDictionary
	{
		/// <summary>
		/// Identifies the data dictionary keys needed for this item.
		/// </summary>
		/// <param name="security">The current security object.</param>
		/// <returns>An array of data dictionary keys.</returns>
		string[] Keys( SecurityClass security );
	}
}
