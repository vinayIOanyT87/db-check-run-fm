// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFMDataExchange.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IFMDataExchangeProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.Interfaces
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The FMDataExchangeProcessor interface.
	/// </summary>
	public interface IFMDataExchangeProcessor
	{
		/// <summary>
		/// Gets the interface ID.
		/// </summary>
		string InterfaceID { get; }

		/// <summary>
		/// Gets or sets the interface path.
		/// </summary>
		string InterfacePath { get; set; }

		/// <summary>
		/// Gets a value indicating whether authenticate.
		/// </summary>
		bool Authenticate { get; }

		/// <summary>
		/// The process data.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="xmlData">
		/// The XML data.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		string ProcessData(SecurityClass security, string xmlData);
	}
}
