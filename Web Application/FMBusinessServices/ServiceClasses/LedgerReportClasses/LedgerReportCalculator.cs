// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedgerReportCalculator.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The purpose of this class is to implement store procedure interfaces into the CLR functionality.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses.LedgerReportClasses
{
	using System;
	using System.Data;
	using System.Globalization;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// The ledger report calculator.
	/// </summary>
	public class LedgerReportCalculator : ILedgerReportCalculator
	{
		/// <summary>
		/// The calculate.
		/// </summary>
		/// <param name="inSecurityToken">
		/// The in security token.
		/// </param>
		/// <param name="inBeginDate">
		/// The in begin date.
		/// </param>
		/// <param name="inEndDate">
		/// The in end date.
		/// </param>
		/// <param name="inProductGuid">
		/// The in product guid.
		/// </param>
		/// <param name="inManagerGuid">
		/// The in manager guid.
		/// </param>
		/// <param name="inOwnerGuid">
		/// The in owner guid.
		/// </param>
		/// <param name="inSelectedSiteGuid">
		/// The in selected site guid.
		/// </param>
		/// <param name="inUserGuid">
		/// The in user guid.
		/// </param>
		/// <param name="inLedgerRequest">
		/// The in ledger request.
		/// </param>
		/// <param name="inReportLedger">
		/// The in report ledger.
		/// </param>
		/// <param name="inTankGuid">
		/// The in tank guid.
		/// </param>
		/// <param name="inSystemEdition">
		/// The in system edition.
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
		public DataSet Calculate(	string inSecurityToken,
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
									bool inIsBaseDb)
		{
			// Throws an exception if the security token is invalid.
			this.CheckSecurity(inSecurityToken);

			var ledgerProcessor = new LedgerCore.LRLedgerProcessor 
										{
											BeginDate				= inBeginDate,
											EndDate					= inEndDate,
											ProductGuid				= inProductGuid,
											ManagerGuid				= inManagerGuid,
											OwnerGuid				= inOwnerGuid,
											SiteGuid				= inSelectedSiteGuid,
											UserGuid				= inUserGuid,
											TankGuid				= inTankGuid,
											SystemEdition			= (LedgerCore.LRLedgerProcessor.SystemEditions)inSystemEdition,
											LedgerConnectionType	= LedgerCore.LRLedgerProcessor.LedgerConnectionTypes.NonClrConnection,
											DateProcessType			= (LedgerCore.LRLedgerProcessor.DateProcessTypes)inDateProcessType,
											IsBaseDb				= inIsBaseDb
										};

			if ( (inLedgerRequest <= 0) || (inLedgerRequest > 1) )
			{
				ledgerProcessor.LedgerRequestInt = 0;
			}
			else
			{
				ledgerProcessor.LedgerRequestInt = inLedgerRequest;
			}

			ledgerProcessor.ReportLedger = inReportLedger == 1;

			return ledgerProcessor.GetLedgerProcessingResultDataSet( );
		}

		/// <summary>
		/// The calculate report server.
		/// </summary>
		/// <param name="inSecurityToken">
		/// The security token.
		/// </param>
		/// <param name="inMonthYear">
		/// The Month Year.
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
		public DataSet CalculateReportServer(string inSecurityToken, 
											string inMonthYear, 
											string inProductGuid, 
											string inManagerGuid, 
											string inOwnerGuid, 
											string inSiteGuid, 
											string inUserGuid, 
											string inTankGuid)
		{
			Guid productGuid	= this.ConvertToGuid(inProductGuid);
			Guid managerGuid	= this.ConvertToGuid(inManagerGuid);
			Guid ownerGuid		= this.ConvertToGuid(inOwnerGuid);
			Guid siteGuid		= this.ConvertToGuid(inSiteGuid);
			Guid userGuid		= this.ConvertToGuid(inUserGuid);
			Guid tankGuid		= this.ConvertToGuid(inTankGuid);

			// Throws an exception if the security token is invalid.
			this.CheckSecurity(inSecurityToken);

			// Set the system edition based on the hardware key.
			LedgerCore.LRLedgerProcessor.SystemEditions systemEdition = this.SetSystemEdition();

			string beginDateStr = DateEfficacy.getFirstDayOfMonth(inMonthYear);
			string endDateStr = DateEfficacy.getLastDayOfMonth(inMonthYear);

			DateTime startTime = DateTimeOffset.Parse(beginDateStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).Date;
			DateTime endTime = DateTimeOffset.Parse(endDateStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).Date;

			var ledgerProcessor = new LedgerCore.LRLedgerProcessor 
									{
					                      BeginDate				= startTime,
					                      EndDate				= endTime,
					                      ManagerGuid			= managerGuid,
					                      OwnerGuid				= ownerGuid,
					                      ProductGuid			= productGuid,
					                      SystemEdition			= systemEdition,
					                      SiteGuid				= siteGuid,
					                      TankGuid				= tankGuid,
					                      UserGuid				= userGuid,
					                      ReportLedger			= true,
										  LedgerRequest			= LedgerCore.LRLedgerProcessor.LedgerRequests.ManagerLedger,
										  LedgerConnectionType	= LedgerCore.LRLedgerProcessor.LedgerConnectionTypes.NonClrConnection
				                      };

			return ledgerProcessor.GetLedgerProcessingResultDataSet( );
		}

		/// <summary>
		/// This method sets the system edition.
		/// </summary>
		/// <returns>Return system edition based on the hardware key.</returns>
		private LedgerCore.LRLedgerProcessor.SystemEditions SetSystemEdition()
		{
			var systemEdition = LedgerCore.LRLedgerProcessor.SystemEditions.Standard;

			var hardwareKey = new HardwareKeyClass();

			try
			{
				if (hardwareKey.IsADFKey())
				{
					systemEdition = LedgerCore.LRLedgerProcessor.SystemEditions.Adf;
				}
				else if (hardwareKey.IsMODKey())
				{
					systemEdition = LedgerCore.LRLedgerProcessor.SystemEditions.Mod;
				}
				else if (hardwareKey.IsDescEnterpriseKey() ||
						 hardwareKey.IsDescKey() ||
						 hardwareKey.IsDescProfessionalKey())
				{
					systemEdition = LedgerCore.LRLedgerProcessor.SystemEditions.Bsme;
				}
			}
			catch (NullReferenceException)
			{
				systemEdition = LedgerCore.LRLedgerProcessor.SystemEditions.Standard;
			}

			return systemEdition;
		}

		/// <summary>
		/// The convert to GUID.
		/// </summary>
		/// <param name="inGuid">
		/// The GUID.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		private Guid ConvertToGuid(string inGuid)
		{
			Guid convertedGuid = Guid.Empty;

			if ( string.IsNullOrEmpty(inGuid) )
			{
				return convertedGuid;
			}

			convertedGuid = Guid.Parse(inGuid);
			return convertedGuid;
		}

		/// <summary>
		/// This method will check the security and return the site name.
		/// </summary>
		/// <param name="securityToken">Security token.</param>
		private void CheckSecurity(string securityToken)
		{
			var sites = new SitesClass();
			SecurityClass security = sites.GetSecurity(securityToken);

			if (security == null)
			{
				throw new Exception("Invalid security");
			}
		}
	}
}