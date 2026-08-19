///***************************************************************************
/// Module Name:  AutoDistributionReasonCodesForm
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

using System;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IAutoDistributionReasonCodes
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AutoDistributionReasonCodeClass reasonCode);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AutoDistributionReasonCodeClass reasonCode);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid reasonCodeGuid);

		[OperationContract]
		AutoDistributionReasonCodeClass Get(SecurityClass security, Guid reasonCodeGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string resultCodeID);

		[OperationContract]
		AutoDistributionReasonCodeCollectionClass Enumerate(SecurityClass security);
	}
}
