namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface ITrends
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, Trend trend);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, Trend trend);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid trendGuid);

		[OperationContract]
		List<TrendName> EnumerateAvailableTrendNames(SecurityClass security);

		[OperationContract]
		Trend Get(SecurityClass security, Guid trendGuid);

		[OperationContract]
		Trend GetPointTrend(SecurityClass security, Guid pointGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string ID);

		[OperationContract]
		Guid GetIdentityGuidByPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid);

	}
}

