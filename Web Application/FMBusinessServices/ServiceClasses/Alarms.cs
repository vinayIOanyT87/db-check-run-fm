
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using DataAccessLayer;


	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class Alarms : IAlarms
	{

		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, Alarm alarm)
		{
			var alarmList = new List<Alarm>();
			alarmList.Add(alarm);
			this.AddModifyAlarms(security, alarmList,true,false);
			return alarm.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, Alarm alarm)
		{
			var alarmList = new List<Alarm>();
			alarmList.Add(alarm);
			this.AddModifyAlarms(security, alarmList, false, true);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid alarmGuid)
		{
			var alarmGuidList = new List<Guid>();
			alarmGuidList.Add(alarmGuid);
			this.DeleteAlarms(security,alarmGuidList);
		}

		public Alarm Get(SecurityClass security, Guid alarmGuid)
		{
			var alarmGuidList = new List<Guid>();
			alarmGuidList.Add(alarmGuid);
			var alarmDictionary = this.EnumerateByAlarmGuids(security, alarmGuidList);
			Alarm alarm;
			if (alarmDictionary.TryGetValue(alarmGuid, out alarm) == false)
			{
				return null;
			}
			return alarm;
		}

		protected void PopulateFields(SecurityClass security, Dictionary<Guid, Alarm> alarmDictionary)
		{
			if (alarmDictionary.Count > 0)
			{
				var alarmTests = new AlarmTests();
				var alarmGuidList = alarmDictionary.Keys.ToList();
				var alarmAlarmTestDictionary = alarmTests.EnumerateByAlarmGuids(security, alarmGuidList);
				var alarmStatus = new PointTagAlarmStatuses();
				var alarmAlarmStatusDictionary = alarmStatus.EnumerateByAlarmGuids(security, alarmGuidList);
				foreach (var alarm in alarmDictionary.Values)
				{
					Dictionary<Guid,AlarmTest> alarmTestDictionary;
					if (alarmAlarmTestDictionary.TryGetValue(alarm.IdentityGuid, out alarmTestDictionary))
					{
						alarm.AlarmTests = alarmTestDictionary;
					}
					Dictionary<Guid, PointTagAlarmStatus> alarmStatusDictionary;
					if (alarmAlarmStatusDictionary.TryGetValue(alarm.IdentityGuid, out alarmStatusDictionary))
					{
						alarm.AlarmStatus = alarmStatusDictionary;
					}
				}
			}
		}

		public Dictionary<Guid, Alarm> EnumerateByAlarmGuids(SecurityClass security, List<Guid> alarmGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (alarmGuidList == null || alarmGuidList.Count < 1)
			{
				return new Dictionary<Guid, Alarm>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				Alarm.EnumerateByAlarmGuidListSQL(cmd,alarmGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var alarmDictionary = new Dictionary<Guid, Alarm>();
			var populateFieldsAlarmDictionary = new Dictionary<Guid, Alarm>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarm = new Alarm();

				alarm.AutoLoad(row);
				alarmDictionary.Add(alarm.IdentityGuid,alarm);
				populateFieldsAlarmDictionary.Add(alarm.IdentityGuid, alarm);
			}
			this.PopulateFields(security, populateFieldsAlarmDictionary);
			return alarmDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, Alarm>> EnumerateByPointGuids(SecurityClass security, List<Guid> pointGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (pointGuidList == null || pointGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, Alarm>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				Alarm.EnumerateByPointListSQL(cmd, pointGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment
			var totalAlarmDictionary = new Dictionary<Guid, Alarm>();

         var retAlarmDictionary = new Dictionary<Guid, Dictionary<Guid, Alarm>>();
			Dictionary<Guid, Alarm> alarmDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarm = new Alarm();
				alarm.AutoLoad(row);
				Guid currentGuid = alarm.PointGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmDictionary != null && alarmDictionary.Count > 0)
					{
						retAlarmDictionary.Add(prevGuid, alarmDictionary);
					}
					alarmDictionary = new Dictionary<Guid, Alarm>();
				}
				if (alarmDictionary != null)
				{
					alarmDictionary.Add(alarm.IdentityGuid, alarm);
					totalAlarmDictionary.Add(alarm.IdentityGuid,alarm);
					prevGuid = currentGuid;
				}

			}
			if (alarmDictionary != null && alarmDictionary.Count > 0)
			{
				retAlarmDictionary.Add(prevGuid, alarmDictionary);
			}
			this.PopulateFields(security, totalAlarmDictionary);
			return retAlarmDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, Alarm>> EnumerateByPointTagGuids(SecurityClass security, List<Guid> pointTagGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (pointTagGuidList == null || pointTagGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, Alarm>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				Alarm.EnumerateByTagGuidListSQL(cmd, pointTagGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment
			var totalAlarmDictionary = new Dictionary<Guid, Alarm>();

			var retAlarmDictionary = new Dictionary<Guid, Dictionary<Guid, Alarm>>();
			Dictionary<Guid, Alarm> alarmDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarm = new Alarm();
				alarm.AutoLoad(row);
				Guid currentGuid = alarm.InputTagGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmDictionary != null && alarmDictionary.Count > 0)
					{
						retAlarmDictionary.Add(prevGuid, alarmDictionary);
					}
					alarmDictionary = new Dictionary<Guid, Alarm>();
				}
				if (alarmDictionary != null)
				{
					alarmDictionary.Add(alarm.IdentityGuid, alarm);
					totalAlarmDictionary.Add(alarm.IdentityGuid, alarm);
					prevGuid = currentGuid;
				}

			}
			if (alarmDictionary != null && alarmDictionary.Count > 0)
			{
				retAlarmDictionary.Add(prevGuid, alarmDictionary);
			}
			this.PopulateFields(security, totalAlarmDictionary);
			return retAlarmDictionary;
		}

		//
		public Dictionary<Guid, Alarm> EnumerateActiveAlarmsBySiteGuid(SecurityClass security, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				Alarm.EnumerateActiveAlarmsBySiteGuidSQL(cmd, siteGuid);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var alarmDictionary = new Dictionary<Guid, Alarm>();
			var populateFieldsAlarmDictionary = new Dictionary<Guid, Alarm>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarm = new Alarm();

				alarm.AutoLoad(row);
				alarmDictionary.Add(alarm.IdentityGuid, alarm);
				populateFieldsAlarmDictionary.Add(alarm.IdentityGuid, alarm);
			}
			this.PopulateFields(security, populateFieldsAlarmDictionary);
			return alarmDictionary;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void UpdateShelvedOneShot(SecurityClass security, List<Alarm> alarmList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmList == null || alarmList.Count < 1)
			{
				return;
			}
			// bds add a and e processing here

			using (SqlCommand cmd = new SqlCommand())
			{
				Alarm.UpdateShelvedOneShot(cmd, alarmList, security);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void UpdateShelved(SecurityClass security, List<Alarm> alarmList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmList == null || alarmList.Count < 1)
			{
				return;
			}

	
			using (SqlCommand cmd = new SqlCommand())
			{
				Alarm.UpdateShelved(cmd, alarmList, security);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddModifyAlarms(SecurityClass security, List<Alarm> alarmList, bool enableAdd, bool enableModify)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmList == null || alarmList.Count < 1)
			{
				return;
			}

			List<AlarmTest> alarmTestList = new List<AlarmTest>();
			List<PointTagAlarmStatus> alarmStatusList = new List<PointTagAlarmStatus>();

			foreach (var alarm in alarmList)
			{
				foreach (var alarmTest in alarm.AlarmTests.Values)
				{
					alarmTestList.Add(alarmTest);
				}
				foreach (var alarmStatus in alarm.AlarmStatus.Values)
				{
					alarmStatusList.Add(alarmStatus);
				}
			}
			using (SqlCommand cmd = new SqlCommand())
			{
				Alarm.AddModifyStoredProcedure(cmd, alarmList,security,enableAdd,enableModify);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
			if (alarmTestList.Count > 0)
			{
				AlarmTests alarmTests = new AlarmTests();
				alarmTests.AddModifyAlarmTests(security, alarmTestList, enableAdd, enableModify);
			}
			if (alarmStatusList.Count > 0)
			{
				PointTagAlarmStatuses alarmStatuss = new PointTagAlarmStatuses();
				alarmStatuss.AddModifyAlarmStatuses(security, alarmStatusList, enableAdd, enableModify);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarms(SecurityClass security, List<Guid> alarmGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmGuidList == null || alarmGuidList.Count < 1)
			{
				return;
			}

			PointTagAlarmStatuses alarmStatuses = new PointTagAlarmStatuses();
			alarmStatuses.DeleteAlarmStatusesByAlarmGuidList(security, alarmGuidList);
			AlarmTests alarmTests = new AlarmTests();
			alarmTests.DeleteAlarmTestsByAlarmGuidList(security, alarmGuidList);

			using (SqlCommand cmd = new SqlCommand())
			{
				Alarm.DeleteListSQL(cmd,alarmGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmsByPointGuidList(SecurityClass security, List<Guid> pointGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (pointGuidList == null || pointGuidList.Count < 1)
			{
				return;
			}

			PointTagAlarmStatuses alarmStatuses = new PointTagAlarmStatuses();
			alarmStatuses.DeleteAlarmStatusesByPointGuidList(security, pointGuidList);
			AlarmTests alarmTests = new AlarmTests();
			alarmTests.DeleteAlarmTestsByPointGuidList(security, pointGuidList);

			using (SqlCommand cmd = new SqlCommand())
			{
				Alarm.DeleteByPointGuidListSQL(cmd, pointGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public void DeleteAlarmForTagNotInList(SecurityClass security, Guid inputTagGuid, List<Guid> alarms)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (alarms == null)
			{
				alarms = new List<Guid>();
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				Alarm.DeleteAlarmForTagNotInList(cmd, inputTagGuid, alarms);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public void DeleteAlarmsFromTagsNotInList(SecurityClass security, Guid pointGuid, List<Guid> tagsWithAlarms)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (tagsWithAlarms == null)
			{
				tagsWithAlarms = new List<Guid>();
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				Alarm.DeleteAlarmsFromTagsNotInList(cmd, pointGuid, tagsWithAlarms);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}


	}
}
