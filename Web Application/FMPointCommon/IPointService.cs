
namespace FMPointCommon
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	[ServiceKnownType(typeof(SecurityClass))]
	[ServiceKnownType(typeof(PointTag))]
	public interface IPointService
	{
		[OperationContract]
		void SignalPointChanged(SecurityClass security);

		[OperationContract]
		List<PointTag> GetPointTagData(SecurityClass security, List<Guid> pointTagGuids);

		[OperationContract]
		List<Statistic> GetStatistics(SecurityClass security);

		[OperationContract]
		void ResetStatistics(SecurityClass security);

		[OperationContract]
		void SetPointTagData(SecurityClass security, List<PointTag> pointTagList);

		[OperationContract]
		void SetAcknowledgeAndSilence(SecurityClass security, List<PointTag> pointTags, DateTimeOffset? timestamp = null, string comment = "");

		[OperationContract]
		void SetShelve(SecurityClass security, List<PointTag> pointTags);

		[OperationContract]
		void ExecuteAsyncMethods(SecurityClass security, List<AsyncMethodCallClass> methods);

		[OperationContract]
		List<PointValue> GetPointValueData(SecurityClass security, List<PointValueIdentifier> pointValueIdentifierList);

		[OperationContract]
		List<PointValue> GetPointValueDataChanges(SecurityClass security, List<PointValueIdentifier> pointValueIdentifierList);

		[OperationContract]
		void SetPointValueData(SecurityClass security, List<PointValue> pointValueList);

		[OperationContract]
        PointCalculatorData RunPointCalculator(SecurityClass security, Guid pointGuid, PointCalculatorData pointCalculatorData);

        [OperationContract]
        List<PointTag> RunPointCalculatorX(SecurityClass security, Guid pointGuid, List<PointTag> pointTags);

    }
}
