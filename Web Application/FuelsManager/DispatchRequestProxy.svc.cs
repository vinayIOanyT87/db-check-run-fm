// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchRequestProxy.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Implementation of web proxy interface for Dispatch service requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager
{
	using System;
	using System.Collections.Generic;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;



	/// <summary>
	/// Implementation of web proxy interface for Dispatch service requests.
	/// </summary>
	public class DispatchRequestProxy : IDispatchRequestProxy
	{
		/// <summary>
		/// Enumerates equipment entities for use in Dispatch.
		/// </summary>
		/// <param name="securityToken">The security token</param>
		/// <param name="topVersion">The top version to check</param>
		/// <param name="siteGuid">The current site's GUID</param>
		/// <returns> A dispatch equipment data object</returns>
		DispatchEquipmentDO IDispatchRequestProxy.EnumerateEquipment(string securityToken, string topVersion, string siteGuid)
		{
			var security = FMChannelHelper.MakeCall<ISites, SecurityClass>(sites => sites.GetSecurity(securityToken));
			this.SetCurrentSiteInSecurity(security, siteGuid);

			return FMChannelHelper.MakeCall<IDispatchRequests, DispatchEquipmentDO>(
				dispatchRequests => dispatchRequests.EnumerateEquipment(security, topVersion));
		}

		/// <summary>
		/// Enumerates personnel entities for use in Dispatch.
		/// </summary>
		/// <param name="securityToken">The security token</param>
		/// <param name="topVersion">The top Version</param>
		/// <param name="siteGuid">The current site's GUID</param>
		/// <returns>A dispatch personnel data object</returns>
		DispatchPersonnelDO IDispatchRequestProxy.EnumeratePersonnel(string securityToken, string topVersion, string siteGuid)
		{
			var security = FMChannelHelper.MakeCall<ISites, SecurityClass>(sites => sites.GetSecurity(securityToken));
			this.SetCurrentSiteInSecurity(security, siteGuid);

			return FMChannelHelper.MakeCall<IDispatchRequests, DispatchPersonnelDO>(
				dispatchRequests => dispatchRequests.EnumeratePersonnel(security, topVersion));
		}

		/// <summary>
		/// Enumerates standby personnel for use in Dispatch.
		/// </summary>
		/// <param name="securityToken">The security token</param>
		/// <param name="siteGuid">The current site's GUID</param>
		/// <returns>A list of dispatch personnel display data objects</returns>
		List<DispatchPersonnelDisplayDO> IDispatchRequestProxy.EnumerateStandbyPersonnel(string securityToken, string siteGuid)
		{
			var security = FMChannelHelper.MakeCall<ISites, SecurityClass>(sites => sites.GetSecurity(securityToken));
			this.SetCurrentSiteInSecurity(security, siteGuid);

			return FMChannelHelper.MakeCall<IDispatchRequests, List<DispatchPersonnelDisplayDO>>(
				dispatchRequests => dispatchRequests.EnumerateStandbyPersonnel(security));
		}

		/// <summary>
		/// Enumerates transactions for use in Dispatch.
		/// </summary>
		/// <param name="securityToken">The security token</param>
		/// <param name="topVersion">The top Version</param>
		/// <param name="beginDate">The begin filter date</param>
		/// <param name="endDate">The end filter date</param>
		/// <param name="status">Optional status filter.</param>
		/// <param name="requestName">The optional request type filter.</param>
		/// <param name="siteGuid">The current site's GUID</param>
		/// <returns>
		/// A dispatch transaction data object
		/// </returns>
		DispatchTransactionDO IDispatchRequestProxy.EnumerateTransactions(	string securityToken, 
																			string topVersion, 
																			string beginDate, 
																			string endDate, 
																			string status, 
																			string requestName,
																			string siteGuid)
		{
			var security = FMChannelHelper.MakeCall<ISites, SecurityClass>(sites => sites.GetSecurity(securityToken));
			this.SetCurrentSiteInSecurity(security, siteGuid);

			return FMChannelHelper.MakeCall<IDispatchRequests, DispatchTransactionDO>(
				dispatchRequests => dispatchRequests.EnumerateTransactions(security, topVersion, beginDate, endDate, status, requestName));
		}

		/// <summary>
		/// Sets status to Arrived for a set of transactions given an array of transaction Ids.
		/// Only transactions with statuses of Dispatched will be processed.
		/// </summary>
		/// <param name="securityToken">The security token</param>
		/// <param name="transactionIds">The array of transaction Ids</param>
		/// <param name="siteGuid">The current site's GUID</param>
		/// <returns>The number of transactions statuses set to Arrived</returns>
		int IDispatchRequestProxy.SetArrived(string securityToken, string[] transactionIds, string siteGuid)
		{
			var security = FMChannelHelper.MakeCall<ISites, SecurityClass>(sites => sites.GetSecurity(securityToken));
			this.SetCurrentSiteInSecurity(security, siteGuid);

			return FMChannelHelper.MakeCall<IDispatchRequests, int>(
				dispatchRequests => dispatchRequests.SetArrived(security, transactionIds));
		}

		/// <summary>
		/// Sets status to Started for a set of transactions given an array of transaction Ids.
		/// Only transactions with statuses of Arrived will be processed.
		/// </summary>
		/// <param name="securityToken">The security token</param>
		/// <param name="transactionIds">The array of transaction Ids</param>
		/// <param name="siteGuid">The current site's GUID</param>
		/// <returns>The number of transactions statuses set to Started</returns>
		int IDispatchRequestProxy.SetServiceStarted(string securityToken, string[] transactionIds, string siteGuid)
		{
			var security = FMChannelHelper.MakeCall<ISites, SecurityClass>(sites => sites.GetSecurity(securityToken));
			this.SetCurrentSiteInSecurity(security, siteGuid);

			return FMChannelHelper.MakeCall<IDispatchRequests, int>(
				dispatchRequests => dispatchRequests.SetServiceStarted(security, transactionIds));
		}

		/// <summary>
		/// Sets status to Stopped for a set of transactions given an array of transaction Ids.
		/// Only transactions with statuses of Started will be processed.
		/// </summary>
		/// <param name="securityToken">The security token</param>
		/// <param name="transactionIds">The array of transaction Ids</param>
		/// <param name="siteGuid">The current site's GUID</param>
		/// <returns>The number of transactions statuses set to Stopped</returns>
		int IDispatchRequestProxy.SetServiceStopped(string securityToken, string[] transactionIds, string siteGuid)
		{
			var security = FMChannelHelper.MakeCall<ISites, SecurityClass>(sites => sites.GetSecurity(securityToken));
			this.SetCurrentSiteInSecurity(security, siteGuid);

			return FMChannelHelper.MakeCall<IDispatchRequests, int>(
				dispatchRequests => dispatchRequests.SetServiceStopped(security, transactionIds));
		}

		/// <summary>
		/// This method will retrieve the optional times configuration information
		/// that is saved by the web dispatch optional times page.
		/// </summary>
		/// <param name="securityToken">The security token.</param>
		/// <param name="siteGuid">The site GUID.</param>
		/// <returns>Returns optional times configuration.</returns>
		string IDispatchRequestProxy.RetrieveOptionalTimes(string securityToken, string siteGuid)
		{
			var security = FMChannelHelper.MakeCall<ISites, SecurityClass>(sites => sites.GetSecurity(securityToken));
			this.SetCurrentSiteInSecurity(security, siteGuid);

			string optionalTimeStr = 
				FMChannelHelper.MakeCall<IDispatchRequests, string>(x => x.RetrieveOptionalTimes(security));

			return optionalTimeStr;
		}

		string IDispatchRequestProxy.FMMobileLogin(string employeeId)
		{
			throw new NotImplementedException();

			/*  The following is prototype - proof-of-concept code for mobile fueling app for iPad using HTML5
			bool changePassword;
			int daysUntilExpiration;
			SecurityClass security = null;

			var sr = new SecurityLoginRequest
			         {
				         UserID = "Administrator",
				         Password = "marietta",
				         CACEnabled = false,
				         SiteID = "Varec",
						 TimeOut = 60
			         };

			FMChannelHelper.MakeCall<ISites, string>(
				x => x.Login(out changePassword, out daysUntilExpiration, out security, sr));

			if (security == null)
			{
				return "Security obtainment failed.";
			}

			return security.Token.ToString();
			*/
		}

		/// <summary>
		/// This method will set the current (selected) site's GUID and ID into
		/// the security object.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="stringGuid">The current site's GUID</param>
		private void SetCurrentSiteInSecurity(SecurityClass security, string stringGuid)
		{
			Guid currentSiteGuid;

			if (Guid.TryParse(stringGuid, out currentSiteGuid))
			{
				var currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.GetBasic(security, currentSiteGuid));

				if (currentSite != null)
				{
					security.SiteGuid = currentSiteGuid;
					security.SiteID = currentSite.ID;
				}
			}
		}
	}
}
