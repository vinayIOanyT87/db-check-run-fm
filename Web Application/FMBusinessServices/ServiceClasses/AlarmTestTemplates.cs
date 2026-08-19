
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
	public class AlarmTestTemplates : IAlarmTestTemplates
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AlarmTestTemplate alarmTestTemplate)
		{
			var alarmTestTemplateList = new List<AlarmTestTemplate>();
			alarmTestTemplateList.Add(alarmTestTemplate);
			this.AddModifyAlarmTestTemplates(security, alarmTestTemplateList, true, false);
			return alarmTestTemplate.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, AlarmTestTemplate alarmTestTemplate)
		{
			var alarmTestTemplateList = new List<AlarmTestTemplate>();
			alarmTestTemplateList.Add(alarmTestTemplate);
			this.AddModifyAlarmTestTemplates(security, alarmTestTemplateList, false, true);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid alarmTestTemplateGuid)
		{
			var alarmTestTemplateGuidList = new List<Guid>();
			alarmTestTemplateGuidList.Add(alarmTestTemplateGuid);
			this.DeleteAlarmTestTemplates(security, alarmTestTemplateGuidList);
		}

		public AlarmTestTemplate Get(SecurityClass security, Guid alarmTestTemplateGuid)
		{
			var alarmTestTemplateGuidList = new List<Guid>();
			alarmTestTemplateGuidList.Add(alarmTestTemplateGuid);
			var alarmTestTemplateDictionary = this.EnumerateByAlarmTestTemplateGuids(security, alarmTestTemplateGuidList);
			AlarmTestTemplate alarmTestTemplate;
			if (alarmTestTemplateDictionary.TryGetValue(alarmTestTemplateGuid, out alarmTestTemplate) == false)
			{
				return null;
			}
			return alarmTestTemplate;
		}

		public Dictionary<Guid, AlarmTestTemplate> EnumerateByAlarmTestTemplateGuids(SecurityClass security, List<Guid> alarmTestTemplateGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (alarmTestTemplateGuidList == null || alarmTestTemplateGuidList.Count < 1)
			{
				return new Dictionary<Guid, AlarmTestTemplate>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTestTemplate.EnumerateByAlarmTestTemplateGuidListSQL(cmd, alarmTestTemplateGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var alarmTestTemplateDictionary = new Dictionary<Guid, AlarmTestTemplate>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmTestTemplate = new AlarmTestTemplate();

				alarmTestTemplate.AutoLoad(row);
				alarmTestTemplateDictionary.Add(alarmTestTemplate.IdentityGuid, alarmTestTemplate);
			}

			return alarmTestTemplateDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, AlarmTestTemplate>> EnumerateByAlarmTemplateGuids(SecurityClass security, List<Guid> alarmTemplateGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (alarmTemplateGuidList == null || alarmTemplateGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, AlarmTestTemplate>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTestTemplate.EnumerateByAlarmTemplateGuidListSQL(cmd, alarmTemplateGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment

			var retAlarmTestTemplateDictionary = new Dictionary<Guid, Dictionary<Guid, AlarmTestTemplate>>();
			Dictionary<Guid, AlarmTestTemplate> alarmTestTemplateDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmTestTemplate = new AlarmTestTemplate();
				alarmTestTemplate.AutoLoad(row);
				Guid currentGuid = alarmTestTemplate.AlarmTemplateGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmTestTemplateDictionary != null && alarmTestTemplateDictionary.Count > 0)
					{
						retAlarmTestTemplateDictionary.Add(prevGuid, alarmTestTemplateDictionary);
					}
					alarmTestTemplateDictionary = new Dictionary<Guid, AlarmTestTemplate>();
				}
				if (alarmTestTemplateDictionary != null)
				{
					alarmTestTemplateDictionary.Add(alarmTestTemplate.IdentityGuid, alarmTestTemplate);
					prevGuid = currentGuid;
				}

			}
			if (alarmTestTemplateDictionary != null && alarmTestTemplateDictionary.Count > 0)
			{
				retAlarmTestTemplateDictionary.Add(prevGuid, alarmTestTemplateDictionary);
			}

			return retAlarmTestTemplateDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, AlarmTestTemplate>> EnumerateByPointTemplateGuids(SecurityClass security, List<Guid> pointTemplateGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (pointTemplateGuidList == null || pointTemplateGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, AlarmTestTemplate>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTestTemplate.EnumerateByPointTemplateListSQL(cmd, pointTemplateGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment

			var retAlarmTestTemplateDictionary = new Dictionary<Guid, Dictionary<Guid, AlarmTestTemplate>>();
			Dictionary<Guid, AlarmTestTemplate> alarmTestTemplateDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmTestTemplate = new AlarmTestTemplate();
				alarmTestTemplate.AutoLoad(row);
				Guid currentGuid = alarmTestTemplate.PointTemplateGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmTestTemplateDictionary != null && alarmTestTemplateDictionary.Count > 0)
					{
						retAlarmTestTemplateDictionary.Add(prevGuid, alarmTestTemplateDictionary);
					}
					alarmTestTemplateDictionary = new Dictionary<Guid, AlarmTestTemplate>();
				}
				if (alarmTestTemplateDictionary != null)
				{
					alarmTestTemplateDictionary.Add(alarmTestTemplate.IdentityGuid, alarmTestTemplate);
					prevGuid = currentGuid;
				}

			}
			if (alarmTestTemplateDictionary != null && alarmTestTemplateDictionary.Count > 0)
			{
				retAlarmTestTemplateDictionary.Add(prevGuid, alarmTestTemplateDictionary);
			}

			return retAlarmTestTemplateDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, AlarmTestTemplate>> EnumerateByPointTemplateTagGuids(SecurityClass security, List<Guid> pointTemplateTagGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (pointTemplateTagGuidList == null || pointTemplateTagGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, AlarmTestTemplate>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTestTemplate.EnumerateByPointTemplateTagGuidListSQL(cmd, pointTemplateTagGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment

			var retAlarmTestTemplateDictionary = new Dictionary<Guid, Dictionary<Guid, AlarmTestTemplate>>();
			Dictionary<Guid, AlarmTestTemplate> alarmTestTemplateDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmTestTemplate = new AlarmTestTemplate();
				alarmTestTemplate.AutoLoad(row);
				Guid currentGuid = alarmTestTemplate.LimitTemplateTagGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmTestTemplateDictionary != null && alarmTestTemplateDictionary.Count > 0)
					{
						retAlarmTestTemplateDictionary.Add(prevGuid, alarmTestTemplateDictionary);
					}
					alarmTestTemplateDictionary = new Dictionary<Guid, AlarmTestTemplate>();
				}
				if (alarmTestTemplateDictionary != null)
				{
					alarmTestTemplateDictionary.Add(alarmTestTemplate.IdentityGuid, alarmTestTemplate);
					prevGuid = currentGuid;
				}

			}
			if (alarmTestTemplateDictionary != null && alarmTestTemplateDictionary.Count > 0)
			{
				retAlarmTestTemplateDictionary.Add(prevGuid, alarmTestTemplateDictionary);
			}

			return retAlarmTestTemplateDictionary;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddModifyAlarmTestTemplates(SecurityClass security, List<AlarmTestTemplate> alarmTestTemplateList, bool enableAdd, bool enableModify)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmTestTemplateList == null || alarmTestTemplateList.Count < 1)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTestTemplate.AddModifyStoredProcedure(cmd, alarmTestTemplateList, security, enableAdd, enableModify);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmTestTemplates(SecurityClass security, List<Guid> alarmTestTemplateGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmTestTemplateGuidList == null || alarmTestTemplateGuidList.Count < 1)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTestTemplate.DeleteListSQL(cmd, alarmTestTemplateGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmTestTemplatesByAlarmTemplateGuidList(SecurityClass security, List<Guid> alarmTemplateGuidList)
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

			var pointAccessGroupToAlarmTestMaps = new PointAccessGroupToAlarmTestMaps();
			pointAccessGroupToAlarmTestMaps.PurgeAlarmTestTemplatesByAlarmTemplateGuidList(security, alarmTemplateGuidList);

         using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTestTemplate.DeleteByAlarmTemplateGuidListSQL(cmd, alarmTemplateGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmTestTemplatesNotInList(SecurityClass security, Guid alarmTemplateGuid, List<Guid> alarmTestTemplateGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmTestTemplateGuidList == null || alarmTestTemplateGuidList.Count < 1)
			{
				return;
			}

			var pointAccessGroupToAlarmTestMaps = new PointAccessGroupToAlarmTestMaps();
			pointAccessGroupToAlarmTestMaps.PurgeAlarmTestTemplatesNotInList(security, alarmTemplateGuid, alarmTestTemplateGuidList);


			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTestTemplate.DeleteAlarmTestTemplatesNotInList(cmd, alarmTemplateGuid, alarmTestTemplateGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmTestTemplatesByPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTestTemplate.DeleteByPointTemplateGuid(cmd, pointTemplateGuid);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}


	}
}
