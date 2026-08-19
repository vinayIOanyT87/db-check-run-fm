namespace FMBusinessServices.ServiceClasses
{
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Constants;
	using InternalClasses;

	using DataAccessLayer;
	using InternalInterfaces;
	using System;
	using Cassandra;
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class AandETagArchive : FMServiceBase, IAandEArchive
	{
		private static readonly IAandEArchiveDatabase AandeArchiveDatabase = new AandEArchiveDatabase();

		public void InitializeArchive(SecurityClass security)
		{
			// TOOD: Check security rights

			AandeArchiveDatabase.Initialize(security);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddArchiveData(SecurityClass security, List<AandEDataElement> AandeDataElementList)
		{
			// TOOD: Check security rights

			AandeArchiveDatabase.AddArchiveData(security, AandeDataElementList);
		}

		public List<string> GetColumnFilterData(SecurityClass security,int selectedColumn, List<AlarmHistoryTabColumnFilterInfo> columnFilterInfoList)
		{
			// TOOD: Check security rights

			return AandeArchiveDatabase.GetColumnFilterData(security, selectedColumn, columnFilterInfoList);
		}

		public List<AandEDataElement> GetAandEArchiveData(SecurityClass security, List<AlarmHistoryTabColumnFilterInfo> columnFilterInfoList, int recordType)
		{
			// TOOD: Check security rights

			return AandeArchiveDatabase.GetAandEArchiveData(security,columnFilterInfoList, recordType);

		}

		public Tuple<string, DateTimeOffset> UpdateAandEComment(SecurityClass security, DateTimeOffset timeStamp, Guid alarmAndEventRecordGuid, string comment)
		{
			// TOOD: Check security rights

			return AandeArchiveDatabase.UpdateAandEComment(security, timeStamp, alarmAndEventRecordGuid, comment);

		}


		public List<Point> GetAandEPointList(List<PointTagAlarmStatus> alarmStatusList,
											SecurityClass security)
		{
			// build up a list of point tags from the point guid in the alarm status list
			List<Point> pointList = new List<Point>();
			Points points = new Points();

			foreach (var alarmStatus in alarmStatusList)
			{
				// we have to do this one at a time because there may be a point with multiple alarms being ack at the same time
				// and the routine that will get a point collection throws an error if the same guid is passed in
				// this is at user speed so it should not have to much of an impact
				var point = points.Get(security, alarmStatus.PointGuid);
				pointList.Add(point);
			}

			return pointList;
		}

		public List<Alarm> GetAandEAlarmsList(List<PointTagAlarmStatus> alarmStatusList,
											SecurityClass security)
		{
			// build up a list of point tags from the point guid in the alarm status list
			List<Alarm> alarmList = new List<Alarm>();
			Alarms alarms = new Alarms();

			// build up a list of guid
			foreach (var alarmStatus in alarmStatusList)
			{
				// we have to do this one at a time because there may be a point with multiple alarms being ack at the same time
				// and the routine that will get a point collection throws an error if the same guid is passed in
				// this is at user speed so it should not have to much of an impact
				var alarm = alarms.Get(security, alarmStatus.AlarmGuid);
				alarmList.Add(alarm);
			}

			return alarmList;
		}

	}
}