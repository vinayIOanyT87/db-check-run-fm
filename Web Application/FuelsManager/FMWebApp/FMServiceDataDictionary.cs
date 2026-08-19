// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMServiceDataDictionary.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMServiceDataDictionary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The fuels manager table data dictionary.
	/// </summary>
	public class FMServiceDataDictionary : IDataDictionary
	{
		#region Explicit Interface Methods

		/// <summary>
		/// The keys.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// Array of strings as data dictionary keys.
		/// </returns>
		public string[] Keys(SecurityClass security)
		{
			return FMChannelHelper.MakeCall<IServiceDataDictionaryKeys, string[]>(x => x.GetKeys(security));
		}

		#endregion
	}
}