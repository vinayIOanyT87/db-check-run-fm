// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILedgerReportCalculator.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The LedgerReportCalculator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses.LedgerReportClasses
{
	using System;
	using System.Data;
	using System.ServiceModel;

	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// The LedgerReportCalculator interface.
	/// </summary>
	[ServiceContract]
	public interface ILedgerReportCalculator
	{
		/// <summary>
		/// The calculate.
		/// </summary>
		/// <param name="inSecurityToken">
		/// The in security token.
		/// </param>
		/// <param name="inBeginDate">
		/// The begin date.
		/// </param>
		/// <param name="inEndDate">
		/// The end date.
		/// </param>
		/// <param name="inProductGuid">
		/// The product GUID.
		/// </param>
		/// <param name="inManagerGuid">
		/// The manager GUID.
		/// </param>
		/// <param name="inOwnerGuid">
		/// The owner GUID.
		/// </param>
		/// <param name="inSelectedSiteGuid">
		/// The selected site GUID.
		/// </param>
		/// <param name="inUserGuid">
		/// The user GUID.
		/// </param>
		/// <param name="inLedgerRequest">
		/// The ledger request.
		/// </param>
		/// <param name="inReportLedger">
		/// The report ledger.
		/// </param>
		/// <param name="inTankGuid">
		/// The tank GUID.
		/// </param>
		/// <param name="inSystemEdition">
		/// The system edition.
		/// </param>
		/// <param name="inDateProcessType">
		/// The date process type.
		/// </param>
		/// <param name="inIsBaseDb">
		/// The base/enterprise flag.
		/// </param>
		/// <returns>
		/// The System.Data.DataSet.
		/// </returns>
		[OperationContract]
		DataSet Calculate(
					string inSecurityToken,
					DateTime inBeginDate,
					DateTime inEndDate,
					Guid inProductGuid,
					Guid inManagerGuid,
					Guid inOwnerGuid,
					Guid inSelectedSiteGuid,
					Guid inUserGuid,
					int inLedgerRequest,
					int inReportLedger,
					Guid inTankGuid,
					int inSystemEdition,
					BsmeLedgerDateType.DateProcessTypes inDateProcessType,
					bool inIsBaseDb);

		/// <summary>
		/// The calculate report sever.
		/// </summary>
		/// <param name="inSecurityToken">
		/// The security token.
		/// </param>
		/// <param name="inMonthYear">
		/// Month Year string value "M YYYY"
		/// </param>
		/// <param name="inProductGuid">
		/// The product GUID.
		/// </param>
		/// <param name="inManagerGuid">
		/// The manager GUID.
		/// </param>
		/// <param name="inOwnerGuid">
		/// The owner GUID.
		/// </param>
		/// <param name="inSiteGuid">
		/// The selected site GUID.
		/// </param>
		/// <param name="inUserGuid">
		/// The user GUID.
		/// </param>
		/// <param name="inTankGuid">
		/// The tank GUID.
		/// </param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		[OperationContract]
		DataSet CalculateReportServer(
					string inSecurityToken,
					string inMonthYear,
					string inProductGuid,
					string inManagerGuid,
					string inOwnerGuid,
					string inSiteGuid,
					string inUserGuid,
					string inTankGuid);
	}
}
