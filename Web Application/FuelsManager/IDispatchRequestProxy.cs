// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDispatchRequestProxy.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Web proxy interface for Dispatch service requests. Primary interface for Dispatch.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager
{
	using System.Collections.Generic;
	using System.ServiceModel;
	using System.ServiceModel.Web;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Web proxy interface for Dispatch service requests. Primary interface for Dispatch.
	/// </summary>
	[ServiceContract]
	public interface IDispatchRequestProxy
	{
		/// <summary>
		/// Enumerates equipment entities for use in Dispatch.
		/// </summary>
		/// <param name="securityToken">The security token</param>
		/// <param name="topVersion">The top Version</param>
		/// <param name="siteGuid">The current site's GUID</param>
		/// <returns>A dispatch equipment data object</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		DispatchEquipmentDO EnumerateEquipment(string securityToken, string topVersion, string siteGuid);

		/// <summary>
		/// Enumerates personnel entities for use in Dispatch.
		/// </summary>
		/// <param name="securityToken">The security token</param>
		/// <param name="topVersion">The top Version</param>
		/// <param name="siteGuid">The current site's GUID</param>
		/// <returns>A dispatch personnel data object</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		DispatchPersonnelDO EnumeratePersonnel(string securityToken, string topVersion, string siteGuid);

		/// <summary>
		/// Enumerates standby personnel for use in Dispatch.
		/// </summary>
		/// <param name="securityToken">The security token</param>
		/// <param name="siteGuid">The current site's GUID</param>
		/// <returns>A list of dispatch personnel display data objects</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		List<DispatchPersonnelDisplayDO> EnumerateStandbyPersonnel(string securityToken, string siteGuid);

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
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		DispatchTransactionDO EnumerateTransactions(
			string securityToken,
			string topVersion,
			string beginDate,
			string endDate,
			string status,
			string requestName,
			string siteGuid);

		/// <summary>
		/// Sets status to Arrived for a set of transactions given an array of transaction Ids.
		/// Only transactions with statuses of Dispatched will be processed.
		/// </summary>
		/// <param name="securityToken">The security token</param>
		/// <param name="transactionIds">The array of transaction Ids</param>
		/// <param name="siteGuid">The current site's GUID</param>
		/// <returns>The number of transactions statuses set to Arrived</returns>
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		int SetArrived(string securityToken, string[] transactionIds, string siteGuid);

		/// <summary>
		/// Sets status to Started for a set of transactions given an array of transaction Ids.
		/// Only transactions with statuses of Arrived will be processed.
		/// </summary>
		/// <param name="securityToken">The security token</param>
		/// <param name="transactionIds">The array of transaction Ids</param>
		/// <param name="siteGuid">The current site's GUID</param>
		/// <returns>The number of transactions statuses set to Started</returns>
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		int SetServiceStarted(string securityToken, string[] transactionIds, string siteGuid);

		/// <summary>
		/// Sets status to Stopped for a set of transactions given an array of transaction Ids.
		/// Only transactions with statuses of Started will be processed.
		/// </summary>
		/// <param name="securityToken">The security token</param>
		/// <param name="transactionIds">The array of transaction Ids</param>
		/// <param name="siteGuid">The current site's GUID</param>
		/// <returns>The number of transactions statuses set to Stopped</returns>
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		int SetServiceStopped(string securityToken, string[] transactionIds, string siteGuid);

		/// <summary>
		/// This method will retrieve the optional times configuration information
		/// that is saved by the web dispatch optional times page.
		/// </summary>
		/// <param name="securityToken">The security token.</param>
		/// <param name="siteGuid">The site GUID.</param>
		/// <returns>Returns optional times configuration.</returns>
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		string RetrieveOptionalTimes(string securityToken, string siteGuid);

		/// <summary>
		/// To support FMMobile prototype for iOS HTML5 application.
		/// </summary>
		/// <param name="employeeId"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		string FMMobileLogin(string employeeId);
	}
}
