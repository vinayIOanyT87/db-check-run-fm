// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceDataDictionaryKeys.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IServiceDataDictionaryKeys type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// IServiceDataDictionaryKeys is the WCF interface definition to retrieve database columns as data dictionary keys
	/// </summary>
	[ServiceContract]
	public interface IServiceDataDictionaryKeys
	{
		/// <summary>
		/// Gets the keys.
		/// </summary>
		/// <param name="security">The security token.</param>
		/// <returns>Data dictionary keys as a string array</returns>
		[OperationContract]
		string[] GetKeys(SecurityClass security);
	}
}
