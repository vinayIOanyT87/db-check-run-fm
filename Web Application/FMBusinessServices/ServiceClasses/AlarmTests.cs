
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
	using System.Linq;
	using FMBusinessObjects.UtilityObjects;

	using FMCore;

	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class AlarmTests : IAlarmTests
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AlarmTest alarmTest)
		{
			var alarmTestList = new List<AlarmTest>();
			alarmTestList.Add(alarmTest);
			this.AddModifyAlarmTests(security, alarmTestList, true, false);
			return alarmTest.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, AlarmTest alarmTest)
		{
			var alarmTestList = new List<AlarmTest>();
			alarmTestList.Add(alarmTest);
			this.AddModifyAlarmTests(security, alarmTestList, false, true);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid alarmTestGuid)
		{
			var alarmTestGuidList = new List<Guid>();
			alarmTestGuidList.Add(alarmTestGuid);
			this.DeleteAlarmTests(security, alarmTestGuidList);
		}

		public AlarmTest Get(SecurityClass security, Guid alarmTestGuid)
		{
			var alarmTestGuidList = new List<Guid>();
			alarmTestGuidList.Add(alarmTestGuid);
			var alarmTestDictionary = this.EnumerateByAlarmTestGuids(security, alarmTestGuidList);
			AlarmTest alarmTest;
			if (alarmTestDictionary.TryGetValue(alarmTestGuid, out alarmTest) == false)
			{
				return null;
			}
			return alarmTest;
		}

		public Dictionary<Guid, AlarmTest> EnumerateByAlarmTestGuids(SecurityClass security, List<Guid> alarmTestGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (alarmTestGuidList == null || alarmTestGuidList.Count < 1)
			{
				return new Dictionary<Guid, AlarmTest>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTest.EnumerateByAlarmTestGuidListSQL(cmd, alarmTestGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var alarmTestDictionary = new Dictionary<Guid, AlarmTest>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmTest = new AlarmTest();

				alarmTest.AutoLoad(row);
				alarmTestDictionary.Add(alarmTest.IdentityGuid, alarmTest);
			}

			return alarmTestDictionary;
		}

		public static void EnumerateRestrictedAccessByAlarmTestGuidList(SecurityClass security, Dictionary<Guid, Guid> alarmTestGuidDictionary)
		{
			security.ThrowIfNull("security");
			alarmTestGuidDictionary.ThrowIfNull("alarmTestGuidDictionary");

			using (var cmd = new SqlCommand())
			{
				AlarmTest.EnumerateRestrictedAccessByAlarmTestGuidList(cmd, security, alarmTestGuidDictionary.Keys.ToList());
				var consolidatedDA = new ConsolidatedDAClass();
				var set = consolidatedDA.GetDataSet(cmd, security);


				if (set != null || set.Tables.Count == 1 || set.Tables[0].Rows.Count != 0)
				{
					var table = set.Tables[0];

					foreach (DataRow row in table.Rows)
					{
						var alarmTestGuid = (Guid)row["AlarmTestGuid"];
						var view = (bool)row["View"];
						var acknowledge = (bool)row["Acknowledge"];

						if (!view && !acknowledge)
						{
							alarmTestGuidDictionary.Remove(alarmTestGuid);
						}
					}
				}
			}
		}

		public Dictionary<Guid, Dictionary<Guid, AlarmTest>> EnumerateByAlarmGuids(SecurityClass security, List<Guid> alarmGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (alarmGuidList == null || alarmGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, AlarmTest>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTest.EnumerateByAlarmGuidListSQL(cmd, alarmGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment

			var retAlarmTestDictionary = new Dictionary<Guid, Dictionary<Guid, AlarmTest>>();
			Dictionary<Guid, AlarmTest> alarmTestDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmTest = new AlarmTest();
				alarmTest.AutoLoad(row);
				Guid currentGuid = alarmTest.AlarmGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmTestDictionary != null && alarmTestDictionary.Count > 0)
					{
						retAlarmTestDictionary.Add(prevGuid, alarmTestDictionary);
					}
					alarmTestDictionary = new Dictionary<Guid, AlarmTest>();
				}
				if (alarmTestDictionary != null)
				{
					alarmTestDictionary.Add(alarmTest.IdentityGuid, alarmTest);
					prevGuid = currentGuid;
				}

			}
			if (alarmTestDictionary != null && alarmTestDictionary.Count > 0)
			{
				retAlarmTestDictionary.Add(prevGuid, alarmTestDictionary);
			}

			return retAlarmTestDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, AlarmTest>> EnumerateByPointGuids(SecurityClass security, List<Guid> pointGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (pointGuidList == null || pointGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, AlarmTest>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTest.EnumerateByPointListSQL(cmd, pointGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment

			var retAlarmTestDictionary = new Dictionary<Guid, Dictionary<Guid, AlarmTest>>();
			Dictionary<Guid, AlarmTest> alarmTestDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmTest = new AlarmTest();
				alarmTest.AutoLoad(row);
				Guid currentGuid = alarmTest.PointGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmTestDictionary != null && alarmTestDictionary.Count > 0)
					{
						retAlarmTestDictionary.Add(prevGuid, alarmTestDictionary);
					}
					alarmTestDictionary = new Dictionary<Guid, AlarmTest>();
				}
				if (alarmTestDictionary != null)
				{
					alarmTestDictionary.Add(alarmTest.IdentityGuid, alarmTest);
					prevGuid = currentGuid;
				}

			}
			if (alarmTestDictionary != null && alarmTestDictionary.Count > 0)
			{
				retAlarmTestDictionary.Add(prevGuid, alarmTestDictionary);
			}

			return retAlarmTestDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, AlarmTest>> EnumerateByPointTagGuids(SecurityClass security, List<Guid> pointTagGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (pointTagGuidList == null || pointTagGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, AlarmTest>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTest.EnumerateByTagGuidListSQL(cmd, pointTagGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment

			var retAlarmTestDictionary = new Dictionary<Guid, Dictionary<Guid, AlarmTest>>();
			Dictionary<Guid, AlarmTest> alarmTestDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmTest = new AlarmTest();
				alarmTest.AutoLoad(row);
				Guid currentGuid = alarmTest.LimitTagGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmTestDictionary != null && alarmTestDictionary.Count > 0)
					{
						retAlarmTestDictionary.Add(prevGuid, alarmTestDictionary);
					}
					alarmTestDictionary = new Dictionary<Guid, AlarmTest>();
				}
				if (alarmTestDictionary != null)
				{
					alarmTestDictionary.Add(alarmTest.IdentityGuid, alarmTest);
					prevGuid = currentGuid;
				}

			}
			if (alarmTestDictionary != null && alarmTestDictionary.Count > 0)
			{
				retAlarmTestDictionary.Add(prevGuid, alarmTestDictionary);
			}

			return retAlarmTestDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, AlarmTest>> EnumerateByPointLimitTagGuids(SecurityClass security, List<Guid> pointTagGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (pointTagGuidList == null || pointTagGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, AlarmTest>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTest.EnumerateByLimitTagGuidListSQL(cmd, pointTagGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment

			var retAlarmTestDictionary = new Dictionary<Guid, Dictionary<Guid, AlarmTest>>();
			Dictionary<Guid, AlarmTest> alarmTestDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmTest = new AlarmTest();
				alarmTest.AutoLoad(row);
				Guid currentGuid = alarmTest.LimitTagGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmTestDictionary != null && alarmTestDictionary.Count > 0)
					{
						retAlarmTestDictionary.Add(prevGuid, alarmTestDictionary);
					}
					alarmTestDictionary = new Dictionary<Guid, AlarmTest>();
				}
				if (alarmTestDictionary != null)
				{
					alarmTestDictionary.Add(alarmTest.IdentityGuid, alarmTest);
					prevGuid = currentGuid;
				}

			}
			if (alarmTestDictionary != null && alarmTestDictionary.Count > 0)
			{
				retAlarmTestDictionary.Add(prevGuid, alarmTestDictionary);
			}

			return retAlarmTestDictionary;
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddModifyAlarmTests(SecurityClass security, List<AlarmTest> alarmTestList, bool enableAdd, bool enableModify)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmTestList == null || alarmTestList.Count < 1)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTest.AddModifyStoredProcedure(cmd, alarmTestList, security, enableAdd, enableModify);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmTests(SecurityClass security, List<Guid> alarmTestGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmTestGuidList == null || alarmTestGuidList.Count < 1)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTest.DeleteListSQL(cmd, alarmTestGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmTestsByAlarmGuidList(SecurityClass security, List<Guid> alarmGuidList)
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
				AlarmTest.DeleteByAlarmGuidListSQL(cmd, alarmGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmTestsByPointGuidList(SecurityClass security, List<Guid> pointGuidList)
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
				AlarmTest.DeleteByPointGuidList(cmd, pointGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}
		

	}
}
