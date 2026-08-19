///***************************************************************************
/// Module Name:  IAutoDistributionProcessor
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

using System;
using System.Data;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	/// <summary>
	/// Interface for the AutoDistributionProcessor service
	/// </summary>
	[ServiceContract]
	public interface IAutoDistributionProcessor
	{
		[OperationContract]
		DataTable CalculateThruput(SecurityClass mySecurity, AutoDistributionThruputSR requestData);

		[OperationContract]
		AutoDistributionOperationHelper PrepareHelper(SecurityClass mySecurity, Guid siteGuid, Guid aliasGuid, Guid productGuid);
	}
}
