
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
	public class PointTemplateTagAlarmStatuses : IPointTemplateTagAlarmStatuses
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, PointTemplateTagAlarmStatus alarmStatusTemplate)
		{
			var alarmStatusTemplateList = new List<PointTemplateTagAlarmStatus>();
			alarmStatusTemplateList.Add(alarmStatusTemplate);
			this.AddModifyAlarmStatusTemplates(security, alarmStatusTemplateList, true, false);
			return alarmStatusTemplate.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, PointTemplateTagAlarmStatus alarmStatusTemplate)
		{
			var alarmStatusTemplateList = new List<PointTemplateTagAlarmStatus>();
			alarmStatusTemplateList.Add(alarmStatusTemplate);
			this.AddModifyAlarmStatusTemplates(security, alarmStatusTemplateList, false, true);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid alarmStatusTemplateGuid)
		{
			var alarmStatusTemplateGuidList = new List<Guid>();
			alarmStatusTemplateGuidList.Add(alarmStatusTemplateGuid);
			this.DeleteAlarmStatusTemplates(security, alarmStatusTemplateGuidList);
		}

		public PointTemplateTagAlarmStatus Get(SecurityClass security, Guid alarmStatusTemplateGuid)
		{
			var alarmStatusTemplateGuidList = new List<Guid>();
			alarmStatusTemplateGuidList.Add(alarmStatusTemplateGuid);
			var alarmStatusTemplateDictionary = this.EnumerateByAlarmStatusTemplateGuids(security, alarmStatusTemplateGuidList);
			PointTemplateTagAlarmStatus alarmStatusTemplate;
			if (alarmStatusTemplateDictionary.TryGetValue(alarmStatusTemplateGuid, out alarmStatusTemplate) == false)
			{
				return null;
			}
			return alarmStatusTemplate;
		}

		public Dictionary<Guid, PointTemplateTagAlarmStatus> EnumerateByAlarmStatusTemplateGuids(SecurityClass security, List<Guid> alarmStatusTemplateGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (alarmStatusTemplateGuidList == null || alarmStatusTemplateGuidList.Count < 1)
			{
				return new Dictionary<Guid, PointTemplateTagAlarmStatus>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTemplateTagAlarmStatus.EnumerateByPointTemplateTagAlarmStatusGuidListSQL(cmd, alarmStatusTemplateGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var alarmStatusTemplateDictionary = new Dictionary<Guid, PointTemplateTagAlarmStatus>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmStatusTemplate = new PointTemplateTagAlarmStatus();

				alarmStatusTemplate.AutoLoad(row);
				alarmStatusTemplateDictionary.Add(alarmStatusTemplate.IdentityGuid, alarmStatusTemplate);
			}

			return alarmStatusTemplateDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>> EnumerateByAlarmTestTemplateGuids(SecurityClass security, List<Guid> alarmTestTemplateGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (alarmTestTemplateGuidList == null || alarmTestTemplateGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTemplateTagAlarmStatus.EnumerateByAlarmTestTemplateGuidListSQL(cmd, alarmTestTemplateGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment

			var retAlarmStatusTemplateDictionary = new Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>>();
			Dictionary<Guid, PointTemplateTagAlarmStatus> alarmStatusTemplateDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmStatusTemplate = new PointTemplateTagAlarmStatus();
				alarmStatusTemplate.AutoLoad(row);
				Guid currentGuid = alarmStatusTemplate.AlarmTestTemplateGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmStatusTemplateDictionary != null && alarmStatusTemplateDictionary.Count > 0)
					{
						retAlarmStatusTemplateDictionary.Add(prevGuid, alarmStatusTemplateDictionary);
					}
					alarmStatusTemplateDictionary = new Dictionary<Guid, PointTemplateTagAlarmStatus>();
				}
				if (alarmStatusTemplateDictionary != null)
				{
					alarmStatusTemplateDictionary.Add(alarmStatusTemplate.IdentityGuid, alarmStatusTemplate);
					prevGuid = currentGuid;
				}

			}
			if (alarmStatusTemplateDictionary != null && alarmStatusTemplateDictionary.Count > 0)
			{
				retAlarmStatusTemplateDictionary.Add(prevGuid, alarmStatusTemplateDictionary);
			}

			return retAlarmStatusTemplateDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>> EnumerateByAlarmTemplateGuids(SecurityClass security, List<Guid> alarmTemplateGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (alarmTemplateGuidList == null || alarmTemplateGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTemplateTagAlarmStatus.EnumerateByAlarmTemplateGuidListSQL(cmd, alarmTemplateGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment

			var retAlarmStatusTemplateDictionary = new Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>>();
			Dictionary<Guid, PointTemplateTagAlarmStatus> alarmStatusTemplateDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmStatusTemplate = new PointTemplateTagAlarmStatus();
				alarmStatusTemplate.AutoLoad(row);
				Guid currentGuid = alarmStatusTemplate.AlarmTemplateGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmStatusTemplateDictionary != null && alarmStatusTemplateDictionary.Count > 0)
					{
						retAlarmStatusTemplateDictionary.Add(prevGuid, alarmStatusTemplateDictionary);
					}
					alarmStatusTemplateDictionary = new Dictionary<Guid, PointTemplateTagAlarmStatus>();
				}
				if (alarmStatusTemplateDictionary != null)
				{
					alarmStatusTemplateDictionary.Add(alarmStatusTemplate.IdentityGuid, alarmStatusTemplate);
					prevGuid = currentGuid;
				}

			}
			if (alarmStatusTemplateDictionary != null && alarmStatusTemplateDictionary.Count > 0)
			{
				retAlarmStatusTemplateDictionary.Add(prevGuid, alarmStatusTemplateDictionary);
			}

			return retAlarmStatusTemplateDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>> EnumerateByPointTemplateGuids(SecurityClass security, List<Guid> pointTemplateGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (pointTemplateGuidList == null || pointTemplateGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTemplateTagAlarmStatus.EnumerateByPointTemplateListSQL(cmd, pointTemplateGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment

			var retAlarmStatusTemplateDictionary = new Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>>();
			Dictionary<Guid, PointTemplateTagAlarmStatus> alarmStatusTemplateDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmStatusTemplate = new PointTemplateTagAlarmStatus();
				alarmStatusTemplate.AutoLoad(row);
				Guid currentGuid = alarmStatusTemplate.PointTemplateGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmStatusTemplateDictionary != null && alarmStatusTemplateDictionary.Count > 0)
					{
						retAlarmStatusTemplateDictionary.Add(prevGuid, alarmStatusTemplateDictionary);
					}
					alarmStatusTemplateDictionary = new Dictionary<Guid, PointTemplateTagAlarmStatus>();
				}
				if (alarmStatusTemplateDictionary != null)
				{
					alarmStatusTemplateDictionary.Add(alarmStatusTemplate.IdentityGuid, alarmStatusTemplate);
					prevGuid = currentGuid;
				}

			}
			if (alarmStatusTemplateDictionary != null && alarmStatusTemplateDictionary.Count > 0)
			{
				retAlarmStatusTemplateDictionary.Add(prevGuid, alarmStatusTemplateDictionary);
			}

			return retAlarmStatusTemplateDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>> EnumerateByPointTemplateTagGuids(SecurityClass security, List<Guid> pointTemplateTagGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (pointTemplateTagGuidList == null || pointTemplateTagGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTemplateTagAlarmStatus.EnumerateByPointTemplateTagGuidListSQL(cmd, pointTemplateTagGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment

			var retAlarmStatusTemplateDictionary = new Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>>();
			Dictionary<Guid, PointTemplateTagAlarmStatus> alarmStatusTemplateDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmStatusTemplate = new PointTemplateTagAlarmStatus();
				alarmStatusTemplate.AutoLoad(row);
				Guid currentGuid = alarmStatusTemplate.PointTemplateTagGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmStatusTemplateDictionary != null && alarmStatusTemplateDictionary.Count > 0)
					{
						retAlarmStatusTemplateDictionary.Add(prevGuid, alarmStatusTemplateDictionary);
					}
					alarmStatusTemplateDictionary = new Dictionary<Guid, PointTemplateTagAlarmStatus>();
				}
				if (alarmStatusTemplateDictionary != null)
				{
					alarmStatusTemplateDictionary.Add(alarmStatusTemplate.IdentityGuid, alarmStatusTemplate);
					prevGuid = currentGuid;
				}

			}
			if (alarmStatusTemplateDictionary != null && alarmStatusTemplateDictionary.Count > 0)
			{
				retAlarmStatusTemplateDictionary.Add(prevGuid, alarmStatusTemplateDictionary);
			}

			return retAlarmStatusTemplateDictionary;

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddModifyAlarmStatusTemplates(SecurityClass security, List<PointTemplateTagAlarmStatus> alarmStatusTemplateList, bool enableAdd, bool enableModify)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmStatusTemplateList == null || alarmStatusTemplateList.Count < 1)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTemplateTagAlarmStatus.AddModifyStoredProcedure(cmd, alarmStatusTemplateList, security, enableAdd, enableModify);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmStatusTemplates(SecurityClass security, List<Guid> alarmStatusTemplateGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmStatusTemplateGuidList == null || alarmStatusTemplateGuidList.Count < 1)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTemplateTagAlarmStatus.DeleteListSQL(cmd, alarmStatusTemplateGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmStatusTemplatesByAlarmTemplateGuidList(SecurityClass security, List<Guid> alarmTemplateGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmTemplateGuidList == null || alarmTemplateGuidList.Count < 1)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTemplateTagAlarmStatus.DeleteByAlarmTemplateGuidListSQL(cmd, alarmTemplateGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmStatusTemplatesByPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			using (SqlCommand cmd = new SqlCommand())
			{
				PointTemplateTagAlarmStatus.DeleteByPointTemplateGuidSQL(cmd, pointTemplateGuid);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}



	}
}
