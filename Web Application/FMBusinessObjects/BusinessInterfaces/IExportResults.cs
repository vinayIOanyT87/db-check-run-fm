// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExportResults.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IExportResults type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The ExportResults interface.
	/// </summary>
	[ServiceContract]
	public interface IExportResults
	{
		/// <summary>
		/// The add from import.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResult">
		/// The export result.
		/// </param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AddFromImport(SecurityClass security, ExportResultClass exportResult);

		/// <summary>
		/// The add.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResult">
		/// The export result.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add(SecurityClass security, ExportResultClass exportResult);

		/// <summary>
		/// The add with user information.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResult">
		/// The export result.
		/// </param>
		/// <param name="useSecurityUserInfo">
		/// The use security user information.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid AddWithUserInfo(SecurityClass security, ExportResultClass exportResult, bool useSecurityUserInfo);

		/// <summary>
		/// The get most recent.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="type">
		/// The type.
		/// </param>
		/// <param name="interfaceName">
		/// The interface name.
		/// </param>
		/// <returns>
		/// The <see cref="ExportResultClass"/>.
		/// </returns>
		[OperationContract]
		ExportResultClass GetMostRecent(SecurityClass security, EXPORT_RESULT_TYPE type, string interfaceName);

		/// <summary>
		/// The get maximum transaction version.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="type">
		/// The type.
		/// </param>
		/// <param name="interfaceName">
		/// The interface name.
		/// </param>
		/// <returns>
		/// The <see cref="ExportResultClass"/>.
		/// </returns>
		[OperationContract]
		ExportResultClass GetMaxTransVersion(SecurityClass security, EXPORT_RESULT_TYPE type, string interfaceName);

		/// <summary>
		/// The get GUID by interface name.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="interfaceName">
		/// The interface name.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		[OperationContract]
		Guid GetGuidByInterfaceName(SecurityClass security, string interfaceName);
	}
}
