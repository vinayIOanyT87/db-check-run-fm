// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnterprise.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The Enterprise interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.Interfaces
{
	using System;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The Enterprise interface.
	/// </summary>
	public interface IEnterprise
	{
		/// <summary>
		/// The send.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="dataObject">
		/// The data object.
		/// </param>
		void Send(SecurityClass security, object dataObject);

		/// <summary>
		/// The purge.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="dataObject">
		/// The data object.
		/// </param>
		void Purge(SecurityClass security, object dataObject);

		/// <summary>
		/// The end of day.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="inventoryDate">
		/// The inventory date.
		/// </param>
		void EndOfDay(SecurityClass security, DateTimeOffset inventoryDate);

		/// <summary>
		/// The end of month.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="inventoryDate">
		/// The inventory date.
		/// </param>
		void EndOfMonth(SecurityClass security, DateTimeOffset inventoryDate);
	}
}
