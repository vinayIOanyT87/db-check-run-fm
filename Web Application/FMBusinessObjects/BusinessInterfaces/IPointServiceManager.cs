

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ServiceModel;

	using DataObjects;

	public enum PointServiceHealthStatus
	{
		Good = 0,
		Bad = 1
	};

	[InheritedExport]
	[ServiceContract]
	public interface IPointServiceManager
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void UpdateTestFailedAndOneShot(SecurityClass security, List<PointTagAlarmStatus> alarmStatusList,	List<Alarm> alarmList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Ping(SecurityClass security, string hostname, PointServiceHealthStatus healthStatus, int pingIntervalInSeconds, int percentCpuUtilization, int percentCpuUtilizationThrottleLevel, int percentMemoryUtilization, int percentMemoryUtilizationThrottleLevel, int maxPointsToProcess);


		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		SchedulePointsResponse SchedulePoints(SecurityClass security, string hostname);


		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		PointCollection GetPointsForHostnameEx(SecurityClass security, string hostname, List<Guid> pointGuidList);

		[OperationContract]
		List<PointTag> GetPointTagData(SecurityClass security, List<Guid> pointTagGuids);

		[OperationContract]
		List<PointTag> GetPointTagDataWithoutPointAccess(SecurityClass security, List<Guid> pointTagGuids);

		[OperationContract]
		List<Statistic> GetStatistics(SecurityClass security, PointService pointService);

		[OperationContract]
		void ResetStatistics(SecurityClass security, PointService pointService);

		[OperationContract]
		void SetPointTagData(SecurityClass security, List<PointTag> pointTags, bool enterpriseVisibility);

		[OperationContract]
		void SetAcknowledge(SecurityClass security, DateTimeOffset timestamp, List<PointTag> pointTagList, string comment = "");


		[OperationContract]
		void SetShelve(SecurityClass security, List<PointTag> pointTagList);

		[OperationContract]
		void Shelve(SecurityClass security, int days, int hours, int minutes, bool oneShot, List<Guid> alarmGuidList);

		[OperationContract]
		void CallAsyncMethods(SecurityClass security, List<AsyncMethodCallClass> methodInvocationList);

		[OperationContract]
		List<PointValue> GetPointValueData(SecurityClass security, List<PointValueIdentifier> pointValueIdentifiers, bool applyPointAccess = true);

		[OperationContract]
		void SetPointValueData(SecurityClass security, List<PointValue> pointValues, bool enterprisedVisibility);

		[OperationContract]
        PointCalculatorData RunPointCalculator(SecurityClass security, Guid pointGuid, PointCalculatorData pointCalculatorData);
        [OperationContract]
        List<PointTag> RunPointCalculatorX(SecurityClass security, Guid pointGuid, List<PointTag> pointTags);

        [OperationContract]
		List<PointValue> GetPointValueDataChanges(SecurityClass security, List<PointValueIdentifier> pointValueIdentifiers, bool applyPointAccess = true);

		[OperationContract]
		Guid? SavePointCalculatorTagValues(SecurityClass security, PointCalculatorResult result);

		[OperationContract]
		void CleanupPointCalculatorRunsFromDB(SecurityClass security, int deleteRowsPriorToMinutes);
	}
}
