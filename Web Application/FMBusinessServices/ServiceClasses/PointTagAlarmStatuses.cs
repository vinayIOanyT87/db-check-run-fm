
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using DataAccessLayer;


	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class PointTagAlarmStatuses : IPointTagAlarmStatuses
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, PointTagAlarmStatus alarmStatus)
		{
			var alarmStatusList = new List<PointTagAlarmStatus>();
			alarmStatusList.Add(alarmStatus);
			this.AddModifyAlarmStatuses(security, alarmStatusList, true, false);
			return alarmStatus.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, PointTagAlarmStatus alarmStatus)
		{
			var alarmStatusList = new List<PointTagAlarmStatus>();
			alarmStatusList.Add(alarmStatus);
			this.AddModifyAlarmStatuses(security, alarmStatusList, false, true);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid alarmStatusGuid)
		{
			var alarmStatusGuidList = new List<Guid>();
			alarmStatusGuidList.Add(alarmStatusGuid);
			this.DeleteAlarmStatuses(security, alarmStatusGuidList);
		}

		public PointTagAlarmStatus Get(SecurityClass security, Guid alarmStatusGuid)
		{
			var alarmStatusGuidList = new List<Guid>();
			alarmStatusGuidList.Add(alarmStatusGuid);
			var alarmStatusDictionary = this.EnumerateByAlarmStatusGuids(security, alarmStatusGuidList);
			PointTagAlarmStatus alarmStatus;
			if (alarmStatusDictionary.TryGetValue(alarmStatusGuid, out alarmStatus) == false)
			{
				return null;
			}
			return alarmStatus;
		}

		public Dictionary<Guid, PointTagAlarmStatus> EnumerateByAlarmStatusGuids(SecurityClass security, List<Guid> alarmStatusGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (alarmStatusGuidList == null || alarmStatusGuidList.Count < 1)
			{
				return new Dictionary<Guid, PointTagAlarmStatus>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTagAlarmStatus.EnumerateByPointTagAlarmStatusGuidListSQL(cmd, alarmStatusGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var alarmStatusDictionary = new Dictionary<Guid, PointTagAlarmStatus>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmStatus = new PointTagAlarmStatus();

				alarmStatus.AutoLoad(row);
				alarmStatusDictionary.Add(alarmStatus.IdentityGuid, alarmStatus);
			}

			return alarmStatusDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, PointTagAlarmStatus>> EnumerateByAlarmTestGuids(SecurityClass security, List<Guid> alarmTestGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (alarmTestGuidList == null || alarmTestGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, PointTagAlarmStatus>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTagAlarmStatus.EnumerateByAlarmTestGuidListSQL(cmd, alarmTestGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment

			var retAlarmStatusDictionary = new Dictionary<Guid, Dictionary<Guid, PointTagAlarmStatus>>();
			Dictionary<Guid, PointTagAlarmStatus> alarmStatusDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmStatus = new PointTagAlarmStatus();
				alarmStatus.AutoLoad(row);
				Guid currentGuid = alarmStatus.AlarmTestGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmStatusDictionary != null && alarmStatusDictionary.Count > 0)
					{
						retAlarmStatusDictionary.Add(prevGuid, alarmStatusDictionary);
					}
					alarmStatusDictionary = new Dictionary<Guid, PointTagAlarmStatus>();
				}
				if (alarmStatusDictionary != null)
				{
					alarmStatusDictionary.Add(alarmStatus.IdentityGuid, alarmStatus);
					prevGuid = currentGuid;
				}

			}
			if (alarmStatusDictionary != null && alarmStatusDictionary.Count > 0)
			{
				retAlarmStatusDictionary.Add(prevGuid, alarmStatusDictionary);
			}

			return retAlarmStatusDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, PointTagAlarmStatus>> EnumerateByAlarmGuids(SecurityClass security, List<Guid> alarmGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (alarmGuidList == null || alarmGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, PointTagAlarmStatus>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTagAlarmStatus.EnumerateByAlarmGuidListSQL(cmd, alarmGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment

			var retAlarmStatusDictionary = new Dictionary<Guid, Dictionary<Guid, PointTagAlarmStatus>>();
			Dictionary<Guid, PointTagAlarmStatus> alarmStatusDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmStatus = new PointTagAlarmStatus();
				alarmStatus.AutoLoad(row);
				Guid currentGuid = alarmStatus.AlarmGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmStatusDictionary != null && alarmStatusDictionary.Count > 0)
					{
						retAlarmStatusDictionary.Add(prevGuid, alarmStatusDictionary);
					}
					alarmStatusDictionary = new Dictionary<Guid, PointTagAlarmStatus>();
				}
				if (alarmStatusDictionary != null)
				{
					alarmStatusDictionary.Add(alarmStatus.IdentityGuid, alarmStatus);
					prevGuid = currentGuid;
				}

			}
			if (alarmStatusDictionary != null && alarmStatusDictionary.Count > 0)
			{
				retAlarmStatusDictionary.Add(prevGuid, alarmStatusDictionary);
			}

			return retAlarmStatusDictionary;
		}

		public Dictionary<Guid,Dictionary<Guid, PointTagAlarmStatus>> EnumerateByPointGuids(SecurityClass security, List<Guid> pointGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (pointGuidList == null || pointGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, PointTagAlarmStatus>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTagAlarmStatus.EnumerateByPointListSQL(cmd, pointGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment

			var retAlarmStatusDictionary = new Dictionary<Guid, Dictionary<Guid, PointTagAlarmStatus>>();
			Dictionary<Guid, PointTagAlarmStatus> alarmStatusDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmStatus = new PointTagAlarmStatus();
				alarmStatus.AutoLoad(row);
				Guid currentGuid = alarmStatus.PointGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmStatusDictionary != null && alarmStatusDictionary.Count > 0)
					{
						retAlarmStatusDictionary.Add(prevGuid, alarmStatusDictionary);
					}
					alarmStatusDictionary = new Dictionary<Guid, PointTagAlarmStatus>();
				}
				if (alarmStatusDictionary != null)
				{
					alarmStatusDictionary.Add(alarmStatus.IdentityGuid, alarmStatus);
					prevGuid = currentGuid;
				}

			}
			if (alarmStatusDictionary != null && alarmStatusDictionary.Count > 0)
			{
				retAlarmStatusDictionary.Add(prevGuid, alarmStatusDictionary);
			}

			return retAlarmStatusDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, PointTagAlarmStatus>> EnumerateByPointTagGuids(SecurityClass security, List<Guid> pointTagGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (pointTagGuidList == null || pointTagGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, PointTagAlarmStatus>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTagAlarmStatus.EnumerateByTagGuidListSQL(cmd, pointTagGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment

			var retAlarmStatusDictionary = new Dictionary<Guid, Dictionary<Guid, PointTagAlarmStatus>>();
			Dictionary<Guid, PointTagAlarmStatus> alarmStatusDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmStatus = new PointTagAlarmStatus();
				alarmStatus.AutoLoad(row);
				Guid currentGuid = alarmStatus.TagGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmStatusDictionary != null && alarmStatusDictionary.Count > 0)
					{
						retAlarmStatusDictionary.Add(prevGuid, alarmStatusDictionary);
					}
					alarmStatusDictionary = new Dictionary<Guid, PointTagAlarmStatus>();
				}
				if (alarmStatusDictionary != null)
				{
					alarmStatusDictionary.Add(alarmStatus.IdentityGuid, alarmStatus);
					prevGuid = currentGuid;
				}

			}
			if (alarmStatusDictionary != null && alarmStatusDictionary.Count > 0)
			{
				retAlarmStatusDictionary.Add(prevGuid, alarmStatusDictionary);
			}

			return retAlarmStatusDictionary;

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddModifyAlarmStatuses(SecurityClass security, List<PointTagAlarmStatus> alarmStatusList, bool enableAdd, bool enableModify)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmStatusList == null || alarmStatusList.Count < 1)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())	// bds here needs to change
			{
				PointTagAlarmStatus.AddModifyStoredProcedure(cmd, alarmStatusList, security, enableAdd, enableModify);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void UpdateTestFailed(SecurityClass security,
									List<PointTagAlarmStatus> alarmStatusList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmStatusList == null || alarmStatusList.Count < 1)
			{
				return;
			}

			// temporary leave in for verification
			using (SqlCommand cmd = new SqlCommand())   // bds here needs to change
			{
				PointTagAlarmStatus.UpdateTestFailedStoredProcedure(cmd, alarmStatusList, security);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Acknowledge(SecurityClass security, List<PointTagAlarmStatus> alarmStatusList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			using (SqlCommand cmd = new SqlCommand())   // bds here needs to change
			{
				PointTagAlarmStatus.UpdateAcknowledgeAndSilenceStoredProcedure(cmd, alarmStatusList, security);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Silence(SecurityClass security, List<PointTagAlarmStatus> alarmStatusList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmStatusList == null || alarmStatusList.Count < 1)
			{
				return;
			}


			using (SqlCommand cmd = new SqlCommand())   // bds here needs to change
			{
				PointTagAlarmStatus.UpdateSilenceStoredProcedure(cmd, alarmStatusList, security);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}



		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmStatuses(SecurityClass security, List<Guid> alarmStatusGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmStatusGuidList == null || alarmStatusGuidList.Count < 1)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTagAlarmStatus.DeleteListSQL(cmd, alarmStatusGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmStatusesByAlarmGuidList(SecurityClass security, List<Guid> alarmGuidList)
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

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTagAlarmStatus.DeleteByAlarmGuidListSQL(cmd, alarmGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmStatusesByPointGuidList(SecurityClass security, List<Guid> pointGuidList)
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

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTagAlarmStatus.DeleteByPointGuidListSQL(cmd, pointGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}
	}
}
