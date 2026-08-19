// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMobileDeviceProfilePrinters.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   This is the Interface to the mobile device profile printers
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The purpose of this interface class is to provide an interface between the client and the
	/// service level for Mobile Device Profile Printer object.
	/// </summary>
	[ServiceContract]
	public interface IMobileDeviceProfilePrinters
	{
		/// <summary>
		/// The add.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="printer">
		/// The printer.
		/// </param>
		/// <returns>
		/// The System.Guid.
		/// </returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, MobileDeviceProfilePrinter printer);

		/// <summary>
		/// The modify.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="printer">
		/// The printer.
		/// </param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, MobileDeviceProfilePrinter printer);

		/// <summary>
		/// The purge.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="deleteList">
		/// The delete List.
		/// </param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, List<MobileDeviceProfilePrinter> deleteList );

		/// <summary>
		/// The purge all.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileGuid">
		/// The profile guid.
		/// </param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeAll(SecurityClass security, Guid profileGuid);

		/// <summary>
		/// The enumerate by profile guid.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileGuid">
		/// The profile guid.
		/// </param>
		/// <returns>
		/// The System.Data.DataSet.
		/// </returns>
		[OperationContract]
		DataSet EnumerateByProfileGuid(SecurityClass security, Guid profileGuid);
	}
}
