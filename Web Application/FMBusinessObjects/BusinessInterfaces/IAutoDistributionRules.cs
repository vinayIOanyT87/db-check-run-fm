///***************************************************************************
/// Module Name:  IAutoDistributionRules
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

using System;
using System.Collections.Generic;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	/// <summary>
	/// Interface for the AutoDistributionRules service
	/// </summary>
	[ServiceContract]
	public interface IAutoDistributionRules
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AutoDistributionRuleDO rule);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AutoDistributionRuleDO rule);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid ruleGuid);

		[OperationContract]
		AutoDistributionRuleDO Get(SecurityClass security, Guid ruleGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string resultCodeID);

		[OperationContract]
		AutoDistributionRuleDOCollection Enumerate(SecurityClass security, Guid managerGuid, Guid productGuid, string searchText);

		[OperationContract]
		List<BaseMapAssignedInfoDO> EnumerateAssigned(SecurityClass security, Guid ruleGuid, AutoDistributionRuleChildMapTypes childType);

	}
}
