// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportingRequestProxy.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Implementation of web proxy interface for Dispatch service requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager
{
	using System.Collections.Generic;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
    using System.Data;
    using System.ServiceModel;

	/// <summary>
	/// Implementation of web proxy interface for Dispatch service requests.
	/// </summary>
    //[MessageContract(IsWrapped=true)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class ReportingRequestProxy : IReportingRequestProxy
	{
        //[MessageBodyMember]
        //public string SecurityToken;

        //[MessageBodyMember]
        //public string TopVersion;

        /// <summary>
		/// Enumerates equipment entities for use in Dispatch.
		/// </summary>
		/// <param name="securityToken">The security token</param>
		/// <param name="topVersion">The top version to check</param>
		/// <returns> A dispatch equipment data object</returns>
        //DataSet IReportingRequestProxy.GetReportData(string securityToken, string topVersion)
        DataSet IReportingRequestProxy.GetReportData(string p1)
        //DataSet IReportingRequestProxy.GetReportData()
		{
            //var security = FMChannelHelper.MakeCall<ISites, SecurityClass>(sites => sites.GetSecurity(securityToken));
            //return FMChannelHelper.MakeCall<IReportingRequests, DataSet>(
            //    reportingRequests => reportingRequests.GetReportData(security, topVersion));
            //string p1 = "";

            DataSet ds = new DataSet("ReportDataSet");
            DataTable dt = new DataTable("ReportDataTable");
            dt.Columns.Add("Column1");
            dt.Columns.Add("Column2");
            dt.Columns.Add("Column3");
            DataRow row1 = dt.NewRow();
            row1["Column1"] = "Row 1 Column 1 " + p1;
            row1["Column2"] = "Row 1 Column 2 " + p1;
            row1["Column3"] = "Row 1 Column 3 " + p1;
            dt.Rows.Add(row1);

            DataRow row2 = dt.NewRow();
            row2["Column1"] = "Row 2 Column 1 " + p1;
            row2["Column2"] = "Row 2 Column 2 " + p1;
            row2["Column3"] = "Row 2 Column 3 " + p1;
            dt.Rows.Add(row2);

            ds.Tables.Add(dt);
            return ds;
        }







	}
}
