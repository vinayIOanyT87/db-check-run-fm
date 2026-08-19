///***************************************************************************
/// Module Name:  IMeterReconciliationProcessor.cs
/// Author:       Ryan Hill
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using System.Collections.Generic;

	/// <summary>
	/// Defines methods which can be used to retrieve meter reconciliation data from the database
	/// </summary>
	[ServiceContract]
	public interface IMeterReconciliationProcessor
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		List<MeterReconciliationSummaryData> GetSummary(MeterReconciliationSR sr);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		List<MeterReconciliationDetailData> GetDetail(MeterReconciliationSR sr);
	}
}
