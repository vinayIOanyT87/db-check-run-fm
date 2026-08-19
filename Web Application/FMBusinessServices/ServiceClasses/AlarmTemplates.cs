
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
	public class AlarmTemplates : IAlarmTemplates
	{

		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AlarmTemplate alarmTemplate)
		{
			var alarmTemplateList = new List<AlarmTemplate>();
			alarmTemplateList.Add(alarmTemplate);
			this.AddModifyAlarmTemplates(security, alarmTemplateList, true, false);
			return alarmTemplate.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, AlarmTemplate alarmTemplate)
		{
			var alarmTemplateList = new List<AlarmTemplate>();
			alarmTemplateList.Add(alarmTemplate);
			this.AddModifyAlarmTemplates(security, alarmTemplateList, false, true);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid alarmTemplateGuid)
		{
			var alarmGuidList = new List<Guid>();
			alarmGuidList.Add(alarmTemplateGuid);
			this.DeleteAlarmTemplates(security, alarmGuidList);
		}

		public AlarmTemplate Get(SecurityClass security, Guid alarmTemplateGuid)
		{
			var alarmTemplateGuidList = new List<Guid>();
			alarmTemplateGuidList.Add(alarmTemplateGuid);
			var alarmDictionary = this.EnumerateByAlarmTemplateGuids(security, alarmTemplateGuidList);
			AlarmTemplate alarmTemplate;
			if (alarmDictionary.TryGetValue(alarmTemplateGuid, out alarmTemplate) == false)
			{
				return null;
			}
			return alarmTemplate;
		}

		protected void PopulateFields(SecurityClass security, Dictionary<Guid, AlarmTemplate> alarmTemplateDictionary)
		{
			if (alarmTemplateDictionary.Count > 0)
			{
				var alarmTestTemplates = new AlarmTestTemplates();
				var alarmTemplateGuidList = alarmTemplateDictionary.Keys.ToList();
            var alarmTemplateAlarmTestTemplateDictionary = alarmTestTemplates.EnumerateByAlarmTemplateGuids(security, alarmTemplateGuidList);
				var alarmStatusTemplate = new PointTemplateTagAlarmStatuses();
				var alarmTemplateAlarmStatusTemplateDictionary = alarmStatusTemplate.EnumerateByAlarmTemplateGuids(security, alarmTemplateGuidList);
				foreach (var alarmTemplate in alarmTemplateDictionary.Values)
				{
					Dictionary<Guid, AlarmTestTemplate> alarmTestDictionary;
					if (alarmTemplateAlarmTestTemplateDictionary.TryGetValue(alarmTemplate.IdentityGuid, out alarmTestDictionary))
					{
						alarmTemplate.AlarmTestTemplates = alarmTestDictionary;
					}
					Dictionary<Guid, PointTemplateTagAlarmStatus> alarmStatusDictionary;
					if (alarmTemplateAlarmStatusTemplateDictionary.TryGetValue(alarmTemplate.IdentityGuid, out alarmStatusDictionary))
					{
						alarmTemplate.AlarmStatusTemplates = alarmStatusDictionary;
					}
				}
			}
		}

		public Dictionary<Guid, AlarmTemplate> EnumerateByAlarmTemplateGuids(SecurityClass security, List<Guid> alarmTemplateGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (alarmTemplateGuidList == null || alarmTemplateGuidList.Count < 1)
			{
				return new Dictionary<Guid, AlarmTemplate>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTemplate.EnumerateByAlarmTemplateGuidListSQL(cmd, alarmTemplateGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var alarmTemplateDictionary = new Dictionary<Guid, AlarmTemplate>();
			var alarmTemplatePopulateFieldsDictionary = new Dictionary<Guid, AlarmTemplate>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmTemplate = new AlarmTemplate();

				alarmTemplate.AutoLoad(row);
				alarmTemplateDictionary.Add(alarmTemplate.IdentityGuid, alarmTemplate);
				alarmTemplatePopulateFieldsDictionary.Add(alarmTemplate.IdentityGuid, alarmTemplate);
			}
			this.PopulateFields(security, alarmTemplatePopulateFieldsDictionary);
			return alarmTemplateDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, AlarmTemplate>> EnumerateByPointTemplateGuids(SecurityClass security, List<Guid> pointTemplateGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (pointTemplateGuidList == null || pointTemplateGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, AlarmTemplate>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTemplate.EnumerateByPointTemplateListSQL(cmd, pointTemplateGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment
			var totalAlarmTemplateDictionary = new Dictionary<Guid, AlarmTemplate>();

			var retAlarmTemplateDictionary = new Dictionary<Guid, Dictionary<Guid, AlarmTemplate>>();
			Dictionary<Guid, AlarmTemplate> alarmTemplateDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmTemplate = new AlarmTemplate();
				alarmTemplate.AutoLoad(row);
				Guid currentGuid = alarmTemplate.PointTemplateGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmTemplateDictionary != null && alarmTemplateDictionary.Count > 0)
					{
						retAlarmTemplateDictionary.Add(prevGuid, alarmTemplateDictionary);
					}
					alarmTemplateDictionary = new Dictionary<Guid, AlarmTemplate>();
				}
				if (alarmTemplateDictionary != null)
				{
					alarmTemplateDictionary.Add(alarmTemplate.IdentityGuid, alarmTemplate);
					totalAlarmTemplateDictionary.Add(alarmTemplate.IdentityGuid, alarmTemplate);
					prevGuid = currentGuid;
				}

			}
			if (alarmTemplateDictionary != null && alarmTemplateDictionary.Count > 0)
			{
				retAlarmTemplateDictionary.Add(prevGuid, alarmTemplateDictionary);
			}
			this.PopulateFields(security, totalAlarmTemplateDictionary);
			return retAlarmTemplateDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, AlarmTemplate>> EnumerateByPointTemplateTagGuids(SecurityClass security, List<Guid> pointTemplateTagGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security rights

			if (pointTemplateTagGuidList == null || pointTemplateTagGuidList.Count < 1)
			{
				return new Dictionary<Guid, Dictionary<Guid, AlarmTemplate>>();
			}

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTemplate.EnumerateByPointTemplateTagGuidListSQL(cmd, pointTemplateTagGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			//Dictionary of Dictionary Logic for First Guid replace currentGuid assignment
			var totalAlarTemplatemDictionary = new Dictionary<Guid, AlarmTemplate>();

			var retAlarmTemplateDictionary = new Dictionary<Guid, Dictionary<Guid, AlarmTemplate>>();
			Dictionary<Guid, AlarmTemplate> alarmTemplateDictionary = null;
			var prevGuid = Guid.Empty;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var alarmTemplate = new AlarmTemplate();
				alarmTemplate.AutoLoad(row);
				Guid currentGuid = alarmTemplate.InputTemplateTagGuid;

				if (prevGuid != currentGuid)
				{
					if (alarmTemplateDictionary != null && alarmTemplateDictionary.Count > 0)
					{
						retAlarmTemplateDictionary.Add(prevGuid, alarmTemplateDictionary);
					}
					alarmTemplateDictionary = new Dictionary<Guid, AlarmTemplate>();
				}
				if (alarmTemplateDictionary != null)
				{
					alarmTemplateDictionary.Add(alarmTemplate.IdentityGuid, alarmTemplate);
					totalAlarTemplatemDictionary.Add(alarmTemplate.IdentityGuid, alarmTemplate);
					prevGuid = currentGuid;
				}

			}
			if (alarmTemplateDictionary != null && alarmTemplateDictionary.Count > 0)
			{
				retAlarmTemplateDictionary.Add(prevGuid, alarmTemplateDictionary);
			}
			this.PopulateFields(security, totalAlarTemplatemDictionary);
			return retAlarmTemplateDictionary;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddModifyAlarmTemplates(SecurityClass security, List<AlarmTemplate> alarmTemplateList, bool enableAdd, bool enableModify)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			if (alarmTemplateList == null || alarmTemplateList.Count < 1)
			{
				return;
			}

			List<AlarmTestTemplate> alarmTestTemplateList = new List<AlarmTestTemplate>();
			List<PointTemplateTagAlarmStatus> alarmStatusTemplateList = new List<PointTemplateTagAlarmStatus>();

			foreach (var alarmTemplate in alarmTemplateList)
			{
				foreach (var alarmTestTemplate in alarmTemplate.AlarmTestTemplates.Values)
				{
					alarmTestTemplateList.Add(alarmTestTemplate);
				}
				foreach (var alarmStatusTemplate in alarmTemplate.AlarmStatusTemplates.Values)
				{
					alarmStatusTemplateList.Add(alarmStatusTemplate);
				}
			}

			AlarmTestTemplates alarmTestTemplates = new AlarmTestTemplates();

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTemplate.AddModifyStoredProcedure(cmd, alarmTemplateList, security, enableAdd, enableModify);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
			if (alarmTestTemplateList.Count > 0)
			{
				alarmTestTemplates.AddModifyAlarmTestTemplates(security, alarmTestTemplateList, enableAdd, enableModify);
			}
			if (alarmStatusTemplateList.Count > 0)
			{
				PointTemplateTagAlarmStatuses alarmStatusTemplates = new PointTemplateTagAlarmStatuses();
				alarmStatusTemplates.AddModifyAlarmStatusTemplates(security, alarmStatusTemplateList, enableAdd, enableModify);
			}

			// remove the alarm tests has been deleted from the UI 
			foreach (var alarmTemplate in alarmTemplateList)
			{
				alarmTestTemplates.DeleteAlarmTestTemplatesNotInList(
									security,
									alarmTemplate.AlarmTemplateGuid,
									alarmTemplate.AlarmTestTemplates.Values.Select(x => x.AlarmTestTemplateGuid).ToList());
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmTemplates(SecurityClass security, List<Guid> alarmTemplateGuidList)
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

			PointTemplateTagAlarmStatuses alarmStatusTemplates = new PointTemplateTagAlarmStatuses();
			alarmStatusTemplates.DeleteAlarmStatusTemplatesByAlarmTemplateGuidList(security, alarmTemplateGuidList);
			AlarmTestTemplates alarmTests = new AlarmTestTemplates();
			alarmTests.DeleteAlarmTestTemplatesByAlarmTemplateGuidList(security, alarmTemplateGuidList);

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTemplate.DeleteListSQL(cmd, alarmTemplateGuidList);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmTemplatesByPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights

			PointTemplateTagAlarmStatuses alarmStatuses = new PointTemplateTagAlarmStatuses();
			alarmStatuses.DeleteAlarmStatusTemplatesByPointTemplateGuid(security, pointTemplateGuid);
			AlarmTestTemplates alarmTests = new AlarmTestTemplates();
			alarmTests.DeleteAlarmTestTemplatesByPointTemplateGuid(security, pointTemplateGuid);

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTemplate.DeleteByPointTemplateGuidSQL(cmd, pointTemplateGuid);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmTemplatesFromTagsNotInList(SecurityClass security, Guid pointTemplateGuid, List<Guid> tagsWithAlarms)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (tagsWithAlarms == null )
			{
				tagsWithAlarms = new List<Guid>();
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTemplate.DeleteAlarmTemplatesFromTagsNotInList(cmd, pointTemplateGuid, tagsWithAlarms);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteAlarmTemplatesForTagNotInList(SecurityClass security, Guid inputTagTemplateGuid, List<Guid> AlarmTemplates)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (AlarmTemplates == null)
			{
				AlarmTemplates = new List<Guid>();
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				AlarmTemplate.DeleteAlarmTemplatesForTagNotInList(cmd, inputTagTemplateGuid, AlarmTemplates);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

	}
}
