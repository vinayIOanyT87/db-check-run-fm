// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMonthYearProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IMonthYearProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	[ServiceContract]
	public interface IMonthYearProcessor
	{
		[OperationContract]
		MonthYearDO Process ( MonthYearSR sr );
	}
}
