// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBsmeAdminDashboard.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.BusinessInterfaces
{
	using System.Collections.Generic;
	using System.Data;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IAdminDashboard
	{

		[OperationContract]
		DataSet GetNodeHealthSummary(SecurityClass security, string nodeHealth, string orderBy, string siteID, string nodeName);

	}
}