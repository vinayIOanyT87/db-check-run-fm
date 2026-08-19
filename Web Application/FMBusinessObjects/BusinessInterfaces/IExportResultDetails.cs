// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExportResultDetails.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IExportResultDetails type.
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
	/// The ExportResultDetails interface.
	/// </summary>
	[ServiceContract]
	public interface IExportResultDetails
	{
		/// <summary>
		/// The save from import.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResultDetail">
		/// The export result detail.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid SaveFromImport(SecurityClass security, ExportResultDetailClass exportResultDetail);

		/// <summary>
		/// The add.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResultDetail">
		/// The export result detail.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add(SecurityClass security, ExportResultDetailClass exportResultDetail);

		/// <summary>
		/// The add with user info.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResultDetail">
		/// The export result detail.
		/// </param>
		/// <param name="useSecurityUserInfo">
		/// The use security user info.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid AddWithUserInfo(SecurityClass security, ExportResultDetailClass exportResultDetail, bool useSecurityUserInfo);

		/// <summary>
		/// The modify.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResultDetail">
		/// The export result detail.
		/// </param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, ExportResultDetailClass exportResultDetail);

		/// <summary>
		/// The purge.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResultDetailGuid">
		/// The export result detail GUID.
		/// </param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid exportResultDetailGuid);

		/// <summary>
		/// The get.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="exportResultDetailGuid">
		/// The export result detail GUID.
		/// </param>
		/// <returns>
		/// The <see cref="ExportResultDetailClass"/>.
		/// </returns>
		[OperationContract]
		ExportResultDetailClass Get(SecurityClass security, Guid exportResultDetailGuid);

		/// <summary>
		/// The get trans history by record ID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="recordId">
		/// The record ID.
		/// </param>
		/// <param name="startDate">
		/// The start date.
		/// </param>
		/// <param name="endDate">
		/// The end date.
		/// </param>
		/// <param name="orderBy">
		/// The order by.
		/// </param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		[OperationContract]
		DataSet GetTransHistoryByRecordId(
			SecurityClass security, string recordId, DateTime? startDate, DateTime? endDate, string orderBy);

		/// <summary>
		/// The get by record ID and trans version.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="recordId">
		/// The record ID.
		/// </param>
		/// <param name="transVersion">
		/// The trans version.
		/// </param>
		/// <returns>
		/// The <see cref="ExportResultDetailClass"/>.
		/// </returns>
		[OperationContract]
		ExportResultDetailClass GetByRecordIdAndTransVersion(SecurityClass security, string recordId, long transVersion);

		/// <summary>
		/// The get by record ID and current.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="recordId">
		/// The record ID.
		/// </param>
		/// <returns>
		/// The <see cref="ExportResultDetailClass"/>.
		/// </returns>
		[OperationContract]
		ExportResultDetailClass GetByRecordIdAndCurrent(SecurityClass security, string recordId);

		/// <summary>
		/// The get error transactions and texts.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="startDate">
		/// The start date.
		/// </param>
		/// <param name="endDate">
		/// The end date.
		/// </param>
		/// <param name="siteList">
		/// The site list.
		/// </param>
		/// <param name="orderBy">
		/// The order by.
		/// </param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		[OperationContract]
		DataSet GetErrorTransactionsAndTexts(
			SecurityClass security, DateTime? startDate, DateTime? endDate, List<Guid> siteList, string orderBy);

	    /// <summary>
	    /// The get error transactions and texts.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <param name="interfaceName">
	    /// The name of the Interface
	    /// </param>
	    /// <param name="startDate">
	    /// The start date.
	    /// </param>
	    /// <param name="endDate">
	    /// The end date.
	    /// </param>
	    /// <param name="siteList">
	    /// The site list.
	    /// </param>
	    /// <param name="orderBy">
	    /// The order by.
	    /// </param>
	    /// <returns>
	    /// The <see cref="DataSet"/>.
	    /// </returns>
	    [OperationContract]
        DataSet GetErrorTransactionsAndTextsByInterface(
            SecurityClass security, string interfaceName, DateTime? startDate, DateTime? endDate, List<Guid> siteList, string orderBy);


		/// <summary>
		/// The get GUID by record ID and trans version.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="resultDetail">
		/// The result detail.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		[OperationContract]
		Guid GetGuidByRecordIdAndTransVersion(SecurityClass security, ExportResultDetailClass resultDetail);

		/// <summary>
		/// This method retrieves the unacknowledged transactions.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="startDate">The transaction start date.</param>
		/// <param name="endDate">The transaction end date.</param>
		/// <param name="siteList">The list of sites.</param>
		/// <param name="orderBy">Order by column.</param>
		/// <returns>Returns a data set containing the unacknowledged transactions</returns>
		[OperationContract]
		DataSet GetUnacknowledgedTransactions(SecurityClass security, DateTime? startDate, DateTime? endDate, List<Guid> siteList, string orderBy);

		/// <summary>
		/// The enumerate.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="ExportResultDetailCollectionClass"/>.
		/// </returns>
		[OperationContract]
		ExportResultDetailCollectionClass Enumerate(SecurityClass security);
	}
}
