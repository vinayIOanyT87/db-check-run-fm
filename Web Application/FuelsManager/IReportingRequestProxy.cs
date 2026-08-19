// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReportingRequestProxy.cs" company="Varec, Inc.">
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
    using System.Data;

	/// <summary>
	/// Web proxy interface for Dispatch service requests. Primary interface for Dispatch.
	/// </summary>
    [ServiceContract(Namespace = "http://tempuri.org")]
    //[ServiceContract]
    public interface IReportingRequestProxy
	{
		/// <summary>
		/// Enumerates equipment entities for use in Dispatch.
		/// </summary>
		/// <param name="securityToken">The security token</param>
		/// <param name="topVersion">The top Version</param>
		/// <returns>A dispatch equipment data object</returns>
		[OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat=WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        //DataSet GetReportData();
        DataSet GetReportData(string p1);
        //DataSet GetReportData(string securityToken, string topVersion);
    }
}
