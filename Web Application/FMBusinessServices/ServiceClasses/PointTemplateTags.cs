namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;
	using System.Linq;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;

	using FMBusinessServices.DataAccessLayer;
	using InternalClasses;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	[SecuritySafeCritical]
	[ServiceBehavior( TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted )]
	public class PointTemplateTags : FMServiceBase, IPointTemplateTags
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();


		protected void AddAlarmTemplates(SecurityClass security, Dictionary<Guid, PointTemplateTag> tags, Dictionary<Guid, Guid> pointTemplateTagGuidDictionary)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights.

			List<AlarmTemplate> alarmTemplateList = new List<AlarmTemplate>();
			foreach (var tag in tags.Values)
			{
				foreach (var alarmTemplate in tag.AlarmTemplates)
				{
					foreach (var alarmTest in alarmTemplate.Value.AlarmTestTemplates)
					{
						alarmTest.Value.LimitTemplateTagGuid = pointTemplateTagGuidDictionary[alarmTest.Value.LimitTemplateTagGuid];
					}
					alarmTemplate.Value.AlarmStateTemplateTagGuid = pointTemplateTagGuidDictionary[alarmTemplate.Value.AlarmStateTemplateTagGuid];
					alarmTemplate.Value.InputTemplateTagGuid = tag.IdentityGuid;
					alarmTemplateList.Add(alarmTemplate.Value);
				}
			}
			var alarmTemplates = new AlarmTemplates();
			alarmTemplates.AddModifyAlarmTemplates(security, alarmTemplateList, true, false);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddTags(SecurityClass security, Dictionary<Guid, PointTemplateTag> tags)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights.
			var pointTemplateTagGuidDictionary = new Dictionary<Guid, Guid>();
			using (var cmd = new SqlCommand())
			{
				foreach (var tag in tags)
				{
					Guid prevIdentityGuid = tag.Key;
					tag.Value.SetCreationStamp(security);
					tag.Value.AutoGenerateInsertProcSQL(cmd, "gsp_PointTemplateTagInsertByPK");
					cmd.Parameters["@PointTemplateTagGuid"].Direction = ParameterDirection.InputOutput;
					ConsolidatedDa.ExecuteQuery(security, cmd);
					tag.Value.IdentityGuid = new Guid(cmd.Parameters["@PointTemplateTagGuid"].Value.ToString());
					pointTemplateTagGuidDictionary.Add(prevIdentityGuid, tag.Value.IdentityGuid);
				}
			}
			this.AddAlarmTemplates(security, tags, pointTemplateTagGuidDictionary);
		}


		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void AddModuleTags(SecurityClass security, List<PointTemplateTag> tags)
		{
			if ( security == null )
			{
				throw new ArgumentNullException( "security" );
			}

			// TODO: Check security rights.
			using (var cmd = new SqlCommand())
			{
				foreach (var tag in tags)
				{
					tag.SetCreationStamp(security);
					tag.AutoGenerateModifyProcSQL( cmd, "usp_PointTemplateTagUpdateByPK" );

					cmd.Parameters.AddWithValue("@DeviceAlarmMapTag", false);

					ConsolidatedDa.ExecuteQuery( security, cmd );
				}
			}

			foreach (var tagsByTemplate in tags.GroupBy(x => x.PointTemplateGuid))
			{
				this.CascadeServerUnitsToPointTags(security, tagsByTemplate.Key, tagsByTemplate);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid pointTemplateTagGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var pointTemplateTag = this.Get(security, pointTemplateTagGuid);

			if (pointTemplateTag.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Point Template Tag not found.");
			}


			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, pointTemplateTag);


			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.[gsp_PointTemplateTagDeleteByRowGuid]";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@PointTemplateTagGuid", pointTemplateTagGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public void PurgeByPointTemplateGuidAndNotInList(SecurityClass security, Guid pointTemplateGuid, List<Guid> tagList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!this.HasStalePointTemplateTags(security, pointTemplateGuid, tagList))
			{
				return;
			}

			var pointAccessGroupToTagMaps = new PointAccessGroupToTagMaps();
			pointAccessGroupToTagMaps.PurgeByPointTemplateGuidAndNotInList(security, pointTemplateGuid, tagList);

			var pointAccessGroupToPointTagMaps = new PointAccessGroupToPointTagMaps();
			pointAccessGroupToPointTagMaps.PurgeByPointTemplateGuidAndNotInList(security, pointTemplateGuid, tagList);

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SET NOCOUNT ON"
										+ " DELETE FROM map.tblTrendPenToPointTrend WHERE PointTagGuid IN (SELECT PointTagGuid FROM dbo.tblPointTag WHERE PointGuid IN (SELECT PointGuid FROM tblPoint WHERE PointTemplateGuid = @PointTemplateGuid) AND PointTemplateTagGuid NOT IN (SELECT * FROM @PointTemplateTagGuidList))"
										+ " DELETE FROM tblPointTag WHERE PointGuid IN (SELECT PointGuid FROM tblPoint WHERE PointTemplateGuid = @PointTemplateGuid) AND PointTemplateTagGuid IS NOT NULL AND PointTemplateTagGuid NOT IN (SELECT * FROM @PointTemplateTagGuidList)"
										+ " DELETE FROM map.tblTrendPenToDetailTrend WHERE PointTemplateTagGuid IN (SELECT PointTemplateTagGuid FROM tblPointTemplateTag WHERE PointTemplateGuid = @PointTemplateGuid AND PointTemplateTagGuid NOT IN (SELECT * FROM @PointTemplateTagGuidList))"
										+ " DELETE FROM tblPointTemplateTag WHERE PointTemplateGuid = @PointTemplateGuid AND PointTemplateTagGuid NOT IN (SELECT * FROM @PointTemplateTagGuidList)";

				cmd.CommandType = CommandType.Text;

				using (var parameterTempTable = new DataTable())
				{
					parameterTempTable.Columns.Add("PointTemplateTagGuid", typeof(Guid));

					foreach (var pointTemplateTagGuid in tagList)
					{
						parameterTempTable.Rows.Add(pointTemplateTagGuid);
					}

					var pList = new SqlParameter("@PointTemplateTagGuidList", SqlDbType.Structured);
					pList.TypeName = "dbo.GuidListType";
					pList.Value = parameterTempTable;

					cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
					cmd.Parameters.Add(pList);
					ConsolidatedDa.ExecuteQuery(security, cmd);
				}
			}
		}

		private bool HasStalePointTemplateTags(SecurityClass security, Guid pointTemplateGuid, List<Guid> tagList)
		{
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT TOP (1) 1 FROM tblPointTemplateTag WHERE PointTemplateGuid = @PointTemplateGuid AND PointTemplateTagGuid NOT IN (SELECT * FROM @PointTemplateTagGuidList)";
				cmd.CommandType = CommandType.Text;

				using (var parameterTempTable = new DataTable())
				{
					parameterTempTable.Columns.Add("PointTemplateTagGuid", typeof(Guid));

					foreach (var pointTemplateTagGuid in tagList)
					{
						parameterTempTable.Rows.Add(pointTemplateTagGuid);
					}

					var pList = new SqlParameter("@PointTemplateTagGuidList", SqlDbType.Structured);
					pList.TypeName = "dbo.GuidListType";
					pList.Value = parameterTempTable;

					cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
					cmd.Parameters.Add(pList);

					var resultSet = ConsolidatedDa.GetDataSet(cmd, security);
					return resultSet.Tables.Count > 0 && resultSet.Tables[0].Rows.Count > 0;
				}
			}
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyTag(SecurityClass security, PointTemplateTag tag, bool deviceAlarmMapTag)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}

			var pointTags = new PointTags();

			if (tag.Value != null
			    && tag.Value.GetType() == typeof(PointCommandStatusListReference))
			{
				var pointCommandStatusListReference = tag.Value as PointCommandStatusListReference;
				if (tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual
				    && pointCommandStatusListReference.CurrentValue.HasValue)
				{
					pointCommandStatusListReference.CurrentKey = pointTags.GetPointCommandStatusListKey(security,
						tag.PointTemplateTagGuid,
						pointCommandStatusListReference.PointCommandStatusListGuid,
						pointCommandStatusListReference.CurrentValue.Value);
				}
				else if (!string.IsNullOrEmpty(pointCommandStatusListReference.CurrentKey))
				{
					pointCommandStatusListReference.CurrentValue = pointTags.GetPointCommandStatusListValue(security,
						tag.PointTemplateTagGuid,
						pointCommandStatusListReference.PointCommandStatusListGuid,
						pointCommandStatusListReference.CurrentKey);
				}
				else
				{
					pointCommandStatusListReference.CurrentKey = string.Empty;
				}
			}



			var consolidatedDa = new ConsolidatedDAClass();

			using (var cmd = new SqlCommand())
			{
				tag.SetModifyStamp(security);
				tag.AutoGenerateModifyProcSQL(cmd, "usp_PointTemplateTagUpdateByPK");

				if (tag.Value == null && cmd.Parameters.Contains("@Value"))
				{
					cmd.Parameters["@Value"].Value = DBNull.Value;
					cmd.Parameters.AddWithValue("@NullOverrideValue", 1);
				}

				cmd.Parameters.AddWithValue("@DeviceAlarmMapTag", deviceAlarmMapTag);

				consolidatedDa.ExecuteQuery(security, cmd);
			}

			this.CascadeServerUnitsToPointTags(security, tag.PointTemplateGuid, new[] { tag });

			AlarmTemplates alarms = new AlarmTemplates();
			alarms.AddModifyAlarmTemplates(security, tag.AlarmTemplates.Values.ToList(), true, true);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void UpdatePointTemplateTags(SecurityClass security, Guid pointTemplateGuid, Dictionary<Guid, PointTemplateTag> tags)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pointTemplateGuid == null)
			{
				throw new ArgumentNullException("pointTemplateGuid");
			}

			if (tags == null)
			{
				throw new ArgumentNullException("tags");
			}

			var deviceAlarmMapTags = new HashSet<Guid>();

			foreach (var tag in tags.Values)
			{
				if(tag.ValueType != typeof(DeviceAlarmMapReference))
				{
					continue;
				}

				foreach (var alarmTemplate in tag.AlarmTemplates.Values)
				{
					deviceAlarmMapTags.Add(alarmTemplate.AlarmStateTemplateTagGuid);

					foreach (var alarmTestTemplate in alarmTemplate.AlarmTestTemplates.Values)
					{
						deviceAlarmMapTags.Add(alarmTestTemplate.LimitTemplateTagGuid);
					}
				}
			}

			var existingTagsByGuid = this.GetPointTemplateTagsByPointTemplateGuidNoAlarms(security, pointTemplateGuid);

			bool hasDeviceAlarmMapTagColumn;
			var existingDeviceAlarmMapTagByGuid = this.GetExistingDeviceAlarmMapTagFlagsByTag(security, pointTemplateGuid, tags.Keys.ToList(), out hasDeviceAlarmMapTagColumn);

			var processedTagList = new List<Guid>();
			var tagsToUpsert = new List<PointTemplateTag>();
			foreach (var tag in tags.Values)
			{
				processedTagList.Add(tag.PointTemplateTagGuid);

				PointTemplateTag existingTag;
				var isDeviceAlarmMapTag = deviceAlarmMapTags.Contains(tag.PointTemplateTagGuid);
				bool existingDeviceAlarmMapTag = false;
				var hasEquivalentDeviceAlarmMapState = !hasDeviceAlarmMapTagColumn
					|| !existingDeviceAlarmMapTagByGuid.TryGetValue(tag.PointTemplateTagGuid, out existingDeviceAlarmMapTag)
					|| isDeviceAlarmMapTag == existingDeviceAlarmMapTag;

				if (existingTagsByGuid.TryGetValue(tag.PointTemplateTagGuid, out existingTag)
					&& hasEquivalentDeviceAlarmMapState
					&& this.AreEquivalentForUpsert(tag, existingTag))
				{
					continue;
				}

				tagsToUpsert.Add(tag);
			}

			this.UpsertPointTemplateTagsInBatches(security, tagsToUpsert, deviceAlarmMapTags);
			this.CascadeServerUnitsToPointTags(security, pointTemplateGuid, tags.Values);

			// After we have created tags, process alarms that depend on those tags.
			var alarmTemplates = new AlarmTemplates();
			var tagsWithAlarms = tags.Values.Where(x => x.AlarmTemplates.Count > 0).ToList();
			var tagsWithAlarmsGuids = tagsWithAlarms.Select(x => x.PointTemplateTagGuid).ToList();
			var totalAlarmTemplateCount = tagsWithAlarms.Sum(x => x.AlarmTemplates.Count);

			var existingAlarmTemplatesByTag = alarmTemplates.EnumerateByPointTemplateTagGuids(security, tagsWithAlarmsGuids);
			var existingAlarmTemplateIdsByTag = new Dictionary<Guid, HashSet<Guid>>();
			foreach (var existingAlarmTemplatesByTagEntry in existingAlarmTemplatesByTag)
			{
				existingAlarmTemplateIdsByTag[existingAlarmTemplatesByTagEntry.Key] = new HashSet<Guid>(existingAlarmTemplatesByTagEntry.Value.Keys);
			}

			if (totalAlarmTemplateCount > 0)
			{
				var changedAlarmTemplates = new List<AlarmTemplate>(totalAlarmTemplateCount);
				foreach (var tag in tagsWithAlarms)
				{
					Dictionary<Guid, AlarmTemplate> existingAlarmTemplatesForTag;
					if (!existingAlarmTemplatesByTag.TryGetValue(tag.PointTemplateTagGuid, out existingAlarmTemplatesForTag))
					{
						existingAlarmTemplatesForTag = null;
					}

					foreach (var alarmTemplate in tag.AlarmTemplates.Values)
					{
						AlarmTemplate existingAlarmTemplate= new AlarmTemplate();
						var hasExistingAlarmTemplate = existingAlarmTemplatesForTag != null
							&& existingAlarmTemplatesForTag.TryGetValue(alarmTemplate.AlarmTemplateGuid, out existingAlarmTemplate);

						if (!hasExistingAlarmTemplate
							|| !this.AreEquivalentForUpsert(alarmTemplate, existingAlarmTemplate))
						{
							changedAlarmTemplates.Add(alarmTemplate);
						}
					}
				}

				if (changedAlarmTemplates.Count > 0)
				{
					alarmTemplates.AddModifyAlarmTemplates(security, changedAlarmTemplates, true, true);
				}
			}

			foreach (var tag in tagsWithAlarms)
			{
				HashSet<Guid> existingAlarmTemplateIds;
				if (!existingAlarmTemplateIdsByTag.TryGetValue(tag.PointTemplateTagGuid, out existingAlarmTemplateIds)
					|| existingAlarmTemplateIds.Count == 0)
				{
					continue;
				}

				var currentAlarmTemplateIds = new HashSet<Guid>(tag.AlarmTemplates.Values.Select(x => x.AlarmTemplateGuid));
				var hasStaleAlarmTemplates = existingAlarmTemplateIds.Any(existingAlarmTemplateId => !currentAlarmTemplateIds.Contains(existingAlarmTemplateId));

				if (!hasStaleAlarmTemplates)
				{
					continue;
				}

				alarmTemplates.DeleteAlarmTemplatesForTagNotInList(
					security,
					tag.PointTemplateTagGuid,
					currentAlarmTemplateIds.ToList());
			}

			// All alarms may have been deleted in the UI for a tag. Remove alarms for tags not updated with alarms.
			alarmTemplates.DeleteAlarmTemplatesFromTagsNotInList(security, pointTemplateGuid, tagsWithAlarmsGuids);

			this.PurgeByPointTemplateGuidAndNotInList(security, pointTemplateGuid, processedTagList);

		}

		private Dictionary<Guid, PointTemplateTag> GetPointTemplateTagsByPointTemplateGuidNoAlarms(SecurityClass security, Guid pointTemplateGuid)
		{
			var pointTemplateTags = new Dictionary<Guid, PointTemplateTag>();

			var templateTag = new PointTemplateTag();
			DataSet set;
			using (var cmd = new SqlCommand())
			{
				templateTag.EnumerateByPointTemplateSQL(cmd, pointTemplateGuid);
				set = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			if (set.Tables.Count == 0)
			{
				return pointTemplateTags;
			}

			foreach (DataRow row in set.Tables[0].Rows)
			{
				var pointTemplateTag = new PointTemplateTag();
				pointTemplateTag.AutoLoad(row);
				pointTemplateTags[pointTemplateTag.PointTemplateTagGuid] = pointTemplateTag;
			}

			return pointTemplateTags;
		}

		private Dictionary<Guid, bool> GetExistingDeviceAlarmMapTagFlagsByTag(SecurityClass security, Guid pointTemplateGuid, List<Guid> pointTemplateTagGuids, out bool hasDeviceAlarmMapTagColumn)
		{
			hasDeviceAlarmMapTagColumn = false;
			var existingDeviceAlarmMapTagByGuid = new Dictionary<Guid, bool>();

			if (pointTemplateTagGuids == null || pointTemplateTagGuids.Count == 0)
			{
				return existingDeviceAlarmMapTagByGuid;
			}

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT CAST(CASE WHEN COL_LENGTH('dbo.tblPointTemplateTag', 'DeviceAlarmMapTag') IS NULL THEN 0 ELSE 1 END AS bit) AS HasDeviceAlarmMapTagColumn";
				cmd.CommandType = CommandType.Text;

				var resultSet = this.ConsolidatedDa.GetDataSet(cmd, security);
				hasDeviceAlarmMapTagColumn = resultSet.Tables.Count > 0
					&& resultSet.Tables[0].Rows.Count > 0
					&& resultSet.Tables[0].Rows[0]["HasDeviceAlarmMapTagColumn"] != DBNull.Value
					&& (bool)resultSet.Tables[0].Rows[0]["HasDeviceAlarmMapTagColumn"];
			}

			if (!hasDeviceAlarmMapTagColumn)
			{
				return existingDeviceAlarmMapTagByGuid;
			}

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT PointTemplateTagGuid, CAST(ISNULL(DeviceAlarmMapTag, 0) AS bit) AS DeviceAlarmMapTag FROM tblPointTemplateTag WHERE PointTemplateGuid = @PointTemplateGuid AND PointTemplateTagGuid IN (SELECT * FROM @PointTemplateTagGuidList)";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);

				using (var parameterTempTable = new DataTable())
				{
					parameterTempTable.Columns.Add("Guid", typeof(Guid));

					foreach (var pointTemplateTagGuid in pointTemplateTagGuids.Distinct())
					{
						parameterTempTable.Rows.Add(pointTemplateTagGuid);
					}

					var pointTemplateTagGuidList = new SqlParameter("@PointTemplateTagGuidList", SqlDbType.Structured);
					pointTemplateTagGuidList.TypeName = "dbo.GuidListType";
					pointTemplateTagGuidList.Value = parameterTempTable;
					cmd.Parameters.Add(pointTemplateTagGuidList);

					var resultSet = this.ConsolidatedDa.GetDataSet(cmd, security);
					if (resultSet.Tables.Count == 0 || resultSet.Tables[0].Rows.Count == 0)
					{
						return existingDeviceAlarmMapTagByGuid;
					}

					foreach (DataRow row in resultSet.Tables[0].Rows)
					{
						var pointTemplateTagGuid = (Guid)row["PointTemplateTagGuid"];
						var deviceAlarmMapTag = row["DeviceAlarmMapTag"] != DBNull.Value && (bool)row["DeviceAlarmMapTag"];
						existingDeviceAlarmMapTagByGuid[pointTemplateTagGuid] = deviceAlarmMapTag;
					}
				}
			}

			return existingDeviceAlarmMapTagByGuid;
		}

		private void CascadeServerUnitsToPointTags(SecurityClass security, Guid pointTemplateGuid, IEnumerable<PointTemplateTag> templateTags)
		{
			if (templateTags == null)
			{
				return;
			}

			var pointTemplateTagGuids = templateTags
				.Where(tag => tag != null
					&& tag.PointTemplateTagGuid != Guid.Empty
					&& Modules.SupportsServerEngineeringUnits(tag.EngineeringUnitsType))
				.Select(tag => tag.PointTemplateTagGuid)
				.Distinct()
				.ToList();

			if (pointTemplateTagGuids.Count == 0)
			{
				return;
			}

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText =
					"UPDATE pointTag"
					+ " SET pointTag.ServerEngineeringUnitsIndex = pointTemplateTag.ServerEngineeringUnitsIndex,"
					+ " pointTag.UpdatedBy = @UserId,"
					+ " pointTag.UpdatedDate = SYSDATETIMEOFFSET()"
					+ " FROM dbo.tblPointTag pointTag"
					+ " INNER JOIN dbo.tblPoint pointData ON pointData.PointGuid = pointTag.PointGuid"
					+ " INNER JOIN dbo.tblPointTemplateTag pointTemplateTag ON pointTemplateTag.PointTemplateTagGuid = pointTag.PointTemplateTagGuid"
					+ " INNER JOIN @PointTemplateTagGuidList templateTagList ON templateTagList.Guid = pointTemplateTag.PointTemplateTagGuid"
					+ " WHERE pointData.PointTemplateGuid = @PointTemplateGuid"
					+ " AND pointTemplateTag.PointTemplateGuid = @PointTemplateGuid"
					+ " AND pointTag.EngineeringUnitsType NOT IN (@FmuNodimEngineeringUnitsType, @FmuNoneEngineeringUnitsType)"
					+ " AND pointTag.ServerEngineeringUnitsIndex IN (@FmuNoneEngineeringUnit, @FmSiteUnitsEngineeringUnit)"
					+ " AND pointTemplateTag.ServerEngineeringUnitsIndex IS NOT NULL"
					+ " AND pointTag.ServerEngineeringUnitsIndex <> pointTemplateTag.ServerEngineeringUnitsIndex";
				cmd.CommandType = CommandType.Text;

				using (var parameterTempTable = new DataTable())
				{
					parameterTempTable.Columns.Add("Guid", typeof(Guid));

					foreach (var pointTemplateTagGuid in pointTemplateTagGuids)
					{
						parameterTempTable.Rows.Add(pointTemplateTagGuid);
					}

					var pointTemplateTagGuidList = new SqlParameter("@PointTemplateTagGuidList", SqlDbType.Structured);
					pointTemplateTagGuidList.TypeName = "dbo.GuidListType";
					pointTemplateTagGuidList.Value = parameterTempTable;

					cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
					cmd.Parameters.AddWithValue("@UserId", security.UserID);
					cmd.Parameters.AddWithValue("@FmuNodimEngineeringUnitsType", (int)EngineeringUnitType.FmuNodim);
					cmd.Parameters.AddWithValue("@FmuNoneEngineeringUnitsType", (int)EngineeringUnitType.FmuNone);
					cmd.Parameters.AddWithValue("@FmuNoneEngineeringUnit", (int)EngineeringUnit.FmuNone);
					cmd.Parameters.AddWithValue("@FmSiteUnitsEngineeringUnit", (int)EngineeringUnit.FmSiteUnits);
					cmd.Parameters.Add(pointTemplateTagGuidList);

					this.ConsolidatedDa.ExecuteQuery(security, cmd);
				}
			}
		}

		private void UpsertPointTemplateTagsInBatches(SecurityClass security, List<PointTemplateTag> tagsToUpsert, HashSet<Guid> deviceAlarmMapTags)
		{
			if (tagsToUpsert == null || tagsToUpsert.Count == 0)
			{
				return;
			}

			const int maxSqlParametersPerBatch = 1800;
			SqlCommand batchCommand = this.CreatePointTemplateTagUpsertBatchCommand();
			int currentBatchParameterCount = 0;
			int currentBatchCommandIndex = 0;

			try
			{
				foreach (var tag in tagsToUpsert)
				{
					var isDeviceAlarmMapTag = deviceAlarmMapTags.Contains(tag.PointTemplateTagGuid);
					using (var tagUpsertCommand = this.BuildPointTemplateTagUpsertCommand(security, tag, isDeviceAlarmMapTag))
					{
						var commandParameterCount = tagUpsertCommand.Parameters.Count;

						if (currentBatchParameterCount > 0
							&& currentBatchParameterCount + commandParameterCount > maxSqlParametersPerBatch)
						{
							this.ConsolidatedDa.ExecuteQuery(security, batchCommand);
							batchCommand.Dispose();
							batchCommand = this.CreatePointTemplateTagUpsertBatchCommand();
							currentBatchParameterCount = 0;
							currentBatchCommandIndex = 0;
						}

						this.AppendStoredProcedureExecutionToBatch(
							batchCommand,
							tagUpsertCommand,
							"usp_PointTemplateTagUpdateByPK",
							currentBatchCommandIndex);

						currentBatchParameterCount += commandParameterCount;
						currentBatchCommandIndex++;
					}
				}

				if (currentBatchParameterCount > 0)
				{
					this.ConsolidatedDa.ExecuteQuery(security, batchCommand);
				}
			}
			finally
			{
				batchCommand.Dispose();
			}
		}

		private SqlCommand CreatePointTemplateTagUpsertBatchCommand()
		{
			var command = new SqlCommand();
			command.CommandType = CommandType.Text;
			command.CommandText = string.Empty;
			return command;
		}

		private SqlCommand BuildPointTemplateTagUpsertCommand(SecurityClass security, PointTemplateTag tag, bool isDeviceAlarmMapTag)
		{
			var command = new SqlCommand();

			tag.SetModifyStamp(security);
			tag.AutoGenerateModifyProcSQL(command, "usp_PointTemplateTagUpdateByPK");

			if (tag.Value == null && command.Parameters.Contains("@Value"))
			{
				command.Parameters["@Value"].Value = DBNull.Value;
				command.Parameters.AddWithValue("@NullOverrideValue", 1);
			}

			command.Parameters.AddWithValue("@DeviceAlarmMapTag", (isDeviceAlarmMapTag ? 1 : 0));
			return command;
		}

		private void AppendStoredProcedureExecutionToBatch(SqlCommand batchCommand, SqlCommand sourceCommand, string storedProcedureName, int commandIndex)
		{
			var parameterAssignments = new List<string>(sourceCommand.Parameters.Count);
			foreach (SqlParameter sourceParameter in sourceCommand.Parameters)
			{
				var targetParameterName = this.BuildBatchParameterName(sourceParameter.ParameterName, commandIndex);
				var targetParameter = (SqlParameter)((ICloneable)sourceParameter).Clone();
				targetParameter.ParameterName = targetParameterName;
				targetParameter.Direction = ParameterDirection.Input;

				batchCommand.Parameters.Add(targetParameter);
				parameterAssignments.Add(string.Format(
					CultureInfo.InvariantCulture,
					"{0} = {1}",
					sourceParameter.ParameterName,
					targetParameterName));
			}

			batchCommand.CommandText += string.Format(
				CultureInfo.InvariantCulture,
				"EXEC {0} {1};",
				storedProcedureName,
				string.Join(", ", parameterAssignments));
		}

		private string BuildBatchParameterName(string sourceParameterName, int commandIndex)
		{
			var trimmedParameterName = sourceParameterName.StartsWith("@", StringComparison.Ordinal)
				? sourceParameterName.Substring(1)
				: sourceParameterName;

			return string.Format(CultureInfo.InvariantCulture, "@p{0}_{1}", commandIndex, trimmedParameterName);
		}

		private bool AreEquivalentForUpsert(PointTemplateTag currentTag, PointTemplateTag existingTag)
		{
			return currentTag.PointTemplateTagGuid == existingTag.PointTemplateTagGuid
				&& currentTag.PointTemplateGuid == existingTag.PointTemplateGuid
				&& currentTag.SiteGuid == existingTag.SiteGuid
				&& string.Equals(currentTag.ID, existingTag.ID, StringComparison.Ordinal)
				&& string.Equals(this.GetValueTypeStringSafe(currentTag), this.GetValueTypeStringSafe(existingTag), StringComparison.Ordinal)
				&& string.Equals(currentTag.ValueXml, existingTag.ValueXml, StringComparison.Ordinal)
				&& currentTag.EngineeringUnitsType == existingTag.EngineeringUnitsType
				&& currentTag.Units == existingTag.Units
				&& currentTag.ServerUnits == existingTag.ServerUnits
				&& currentTag.DecimalPlaces == existingTag.DecimalPlaces
				&& currentTag.Maximum.Equals(existingTag.Maximum)
				&& currentTag.Minimum.Equals(existingTag.Minimum)
				&& currentTag.InputOutputType == existingTag.InputOutputType
				&& currentTag.Input == existingTag.Input
				&& currentTag.AlarmStatus == existingTag.AlarmStatus
				&& currentTag.ApplyPointTemplateEngineeringUnits == existingTag.ApplyPointTemplateEngineeringUnits
				&& currentTag.ApplyPointTemplateDecimalPlaces == existingTag.ApplyPointTemplateDecimalPlaces
				&& currentTag.ApplyPointTemplateMaximum == existingTag.ApplyPointTemplateMaximum
				&& currentTag.ApplyPointTemplateMinimum == existingTag.ApplyPointTemplateMinimum
				&& currentTag.WellKnownIdentityGuid == existingTag.WellKnownIdentityGuid
				&& currentTag.AlarmsEnabled == existingTag.AlarmsEnabled
				&& currentTag.InhibitInputOutputTypeConfiguration == existingTag.InhibitInputOutputTypeConfiguration
				&& currentTag.InhibitOverride == existingTag.InhibitOverride
				&& currentTag.Module == existingTag.Module
				&& currentTag.Archived == existingTag.Archived;
		}

		private string GetValueTypeStringSafe(PointTemplateTag tag)
		{
			return (tag.ValueType == null) ? string.Empty : tag.ValueType.ToString();
		}

		private bool AreEquivalentForUpsert(AlarmTemplate currentAlarmTemplate, AlarmTemplate existingAlarmTemplate)
		{
			return currentAlarmTemplate.AlarmTemplateGuid == existingAlarmTemplate.AlarmTemplateGuid
				&& currentAlarmTemplate.InputTemplateTagGuid == existingAlarmTemplate.InputTemplateTagGuid
				&& currentAlarmTemplate.SiteGuid == existingAlarmTemplate.SiteGuid
				&& string.Equals(currentAlarmTemplate.ID, existingAlarmTemplate.ID, StringComparison.Ordinal)
				&& currentAlarmTemplate.Enabled == existingAlarmTemplate.Enabled
				&& currentAlarmTemplate.AlarmCategoryApplicationStringGuid == existingAlarmTemplate.AlarmCategoryApplicationStringGuid
				&& currentAlarmTemplate.Order == existingAlarmTemplate.Order
				&& string.Equals(currentAlarmTemplate.NotAlarmState, existingAlarmTemplate.NotAlarmState, StringComparison.Ordinal)
				&& string.Equals(currentAlarmTemplate.Comment, existingAlarmTemplate.Comment, StringComparison.Ordinal)
				&& currentAlarmTemplate.ShelvedStartTimeStamp == existingAlarmTemplate.ShelvedStartTimeStamp
				&& currentAlarmTemplate.ShelvedEndTimeStamp == existingAlarmTemplate.ShelvedEndTimeStamp
				&& currentAlarmTemplate.ShelvedOneShot == existingAlarmTemplate.ShelvedOneShot
				&& string.Equals(currentAlarmTemplate.ShelvedBy, existingAlarmTemplate.ShelvedBy, StringComparison.Ordinal)
				&& currentAlarmTemplate.Suppressed == existingAlarmTemplate.Suppressed
				&& currentAlarmTemplate.AlarmStateTemplateTagGuid == existingAlarmTemplate.AlarmStateTemplateTagGuid
				&& currentAlarmTemplate.ExclusiveAlarm == existingAlarmTemplate.ExclusiveAlarm
				&& this.AreEquivalentForUpsert(currentAlarmTemplate.AlarmTestTemplates, existingAlarmTemplate.AlarmTestTemplates)
				&& this.AreEquivalentForUpsert(currentAlarmTemplate.AlarmStatusTemplates, existingAlarmTemplate.AlarmStatusTemplates);
		}

		private bool AreEquivalentForUpsert(Dictionary<Guid, AlarmTestTemplate> currentAlarmTestTemplates, Dictionary<Guid, AlarmTestTemplate> existingAlarmTestTemplates)
		{
			var current = currentAlarmTestTemplates ?? new Dictionary<Guid, AlarmTestTemplate>();
			var existing = existingAlarmTestTemplates ?? new Dictionary<Guid, AlarmTestTemplate>();

			if (current.Count != existing.Count)
			{
				return false;
			}

			foreach (var currentAlarmTestTemplate in current)
			{
				AlarmTestTemplate existingAlarmTestTemplate;
				if (!existing.TryGetValue(currentAlarmTestTemplate.Key, out existingAlarmTestTemplate))
				{
					return false;
				}

				if (!this.AreEquivalentForUpsert(currentAlarmTestTemplate.Value, existingAlarmTestTemplate))
				{
					return false;
				}
			}

			return true;
		}

		private bool AreEquivalentForUpsert(AlarmTestTemplate currentAlarmTestTemplate, AlarmTestTemplate existingAlarmTestTemplate)
		{
			return currentAlarmTestTemplate.AlarmTestTemplateGuid == existingAlarmTestTemplate.AlarmTestTemplateGuid
				&& currentAlarmTestTemplate.AlarmTemplateGuid == existingAlarmTestTemplate.AlarmTemplateGuid
				&& currentAlarmTestTemplate.PointTemplateGuid == existingAlarmTestTemplate.PointTemplateGuid
				&& currentAlarmTestTemplate.PointTemplateTagGuid == existingAlarmTestTemplate.PointTemplateTagGuid
				&& currentAlarmTestTemplate.SiteGuid == existingAlarmTestTemplate.SiteGuid
				&& string.Equals(currentAlarmTestTemplate.ID, existingAlarmTestTemplate.ID, StringComparison.Ordinal)
				&& currentAlarmTestTemplate.LimitTemplateTagGuid == existingAlarmTestTemplate.LimitTemplateTagGuid
				&& currentAlarmTestTemplate.TagField == existingAlarmTestTemplate.TagField
				&& currentAlarmTestTemplate.AlarmPriorityGuid == existingAlarmTestTemplate.AlarmPriorityGuid
				&& currentAlarmTestTemplate.NormalUnacknowledgedAlarmPriorityGuid == existingAlarmTestTemplate.NormalUnacknowledgedAlarmPriorityGuid
				&& currentAlarmTestTemplate.TestType == existingAlarmTestTemplate.TestType
				&& currentAlarmTestTemplate.BitwiseOperator == existingAlarmTestTemplate.BitwiseOperator
				&& currentAlarmTestTemplate.BitMask == existingAlarmTestTemplate.BitMask
				&& currentAlarmTestTemplate.Enabled == existingAlarmTestTemplate.Enabled
				&& currentAlarmTestTemplate.Order == existingAlarmTestTemplate.Order
				&& string.Equals(currentAlarmTestTemplate.AlarmState, existingAlarmTestTemplate.AlarmState, StringComparison.Ordinal)
				&& currentAlarmTestTemplate.Holdoff.Equals(existingAlarmTestTemplate.Holdoff)
				&& currentAlarmTestTemplate.TimedHoldOffInSeconds == existingAlarmTestTemplate.TimedHoldOffInSeconds
				&& string.Equals(currentAlarmTestTemplate.AlarmText, existingAlarmTestTemplate.AlarmText, StringComparison.Ordinal)
				&& string.Equals(currentAlarmTestTemplate.HelpFile, existingAlarmTestTemplate.HelpFile, StringComparison.Ordinal)
				&& currentAlarmTestTemplate.DrawingGuid == existingAlarmTestTemplate.DrawingGuid;
		}

		private bool AreEquivalentForUpsert(Dictionary<Guid, PointTemplateTagAlarmStatus> currentAlarmStatusTemplates, Dictionary<Guid, PointTemplateTagAlarmStatus> existingAlarmStatusTemplates)
		{
			var current = currentAlarmStatusTemplates ?? new Dictionary<Guid, PointTemplateTagAlarmStatus>();
			var existing = existingAlarmStatusTemplates ?? new Dictionary<Guid, PointTemplateTagAlarmStatus>();

			if (current.Count != existing.Count)
			{
				return false;
			}

			foreach (var currentAlarmStatusTemplate in current)
			{
				PointTemplateTagAlarmStatus existingAlarmStatusTemplate;
				if (!existing.TryGetValue(currentAlarmStatusTemplate.Key, out existingAlarmStatusTemplate))
				{
					return false;
				}

				if (!this.AreEquivalentForUpsert(currentAlarmStatusTemplate.Value, existingAlarmStatusTemplate))
				{
					return false;
				}
			}

			return true;
		}

		private bool AreEquivalentForUpsert(PointTemplateTagAlarmStatus currentAlarmStatusTemplate, PointTemplateTagAlarmStatus existingAlarmStatusTemplate)
		{
			return currentAlarmStatusTemplate.PointTemplateTagAlarmStatusGuid == existingAlarmStatusTemplate.PointTemplateTagAlarmStatusGuid
				&& currentAlarmStatusTemplate.AlarmTestTemplateGuid == existingAlarmStatusTemplate.AlarmTestTemplateGuid
				&& currentAlarmStatusTemplate.SiteGuid == existingAlarmStatusTemplate.SiteGuid
				&& currentAlarmStatusTemplate.Acknowledged == existingAlarmStatusTemplate.Acknowledged
				&& currentAlarmStatusTemplate.AcknowledgedTimestamp == existingAlarmStatusTemplate.AcknowledgedTimestamp
				&& string.Equals(currentAlarmStatusTemplate.AcknowledgedBy, existingAlarmStatusTemplate.AcknowledgedBy, StringComparison.Ordinal)
				&& string.Equals(currentAlarmStatusTemplate.AcknowledgedComment, existingAlarmStatusTemplate.AcknowledgedComment, StringComparison.Ordinal)
				&& currentAlarmStatusTemplate.Silenced == existingAlarmStatusTemplate.Silenced
				&& currentAlarmStatusTemplate.SilencedTimestamp == existingAlarmStatusTemplate.SilencedTimestamp
				&& string.Equals(currentAlarmStatusTemplate.SilencedBy, existingAlarmStatusTemplate.SilencedBy, StringComparison.Ordinal)
				&& currentAlarmStatusTemplate.AlarmTestFailed == existingAlarmStatusTemplate.AlarmTestFailed
				&& currentAlarmStatusTemplate.AlarmTestFailedTimestamp == existingAlarmStatusTemplate.AlarmTestFailedTimestamp;
		}



		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void PurgeAll( SecurityClass security, Guid pointTemplateGuid )
		{
			if ( security == null )
			{
				throw new ArgumentNullException( "security" );
			}

			// TODO: Check security rights.

			var alarmTemplates = new AlarmTemplates();
			alarmTemplates.DeleteAlarmTemplatesByPointTemplateGuid(security, pointTemplateGuid);

			using ( var cmd = new SqlCommand() )
			{
				cmd.CommandText = "dbo.[gsp_PointTemplateTagDeleteByPointTemplateGuid]";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
				ConsolidatedDa.ExecuteQuery( security, cmd );
			}
		}

		protected void PopulateFields(SecurityClass security, Dictionary<Guid, PointTemplateTag> tagDictionary)
		{
			if (tagDictionary.Count > 0)
			{
				var alarmTemplates = new AlarmTemplates();
				var tagAlarmTemplateDictionary = alarmTemplates.EnumerateByPointTemplateTagGuids(security, tagDictionary.Keys.ToList());
				foreach (var alarmTemplateDictionary in tagAlarmTemplateDictionary)
				{
					PointTemplateTag tag;
					if (tagDictionary.TryGetValue(alarmTemplateDictionary.Key, out tag))
					{
						tag.AlarmTemplates = alarmTemplateDictionary.Value;
					}
				}
			}
		}

		/// <summary>
		/// This method will retrieve all the movement summary columns.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Return a list of tag IDs.</returns>
		public List<KeyValuePair<string,string>> EnumerateMovementSummaryColumnNames(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var keyValuePairList = new List<KeyValuePair<string, string>>(100);

			var dataDictionaries = new DataDictionariesClass();

			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Direction"), "TransferDirection"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Deviation"), "Deviation"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Percent Deviation"), "PercentDeviation"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Comment"), "Comment"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Order Number"), "OrderNumber"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Created By"), "CreatedBy"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Planned Start Time"), "PlannedStartTime"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Status"), "Status"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Product"), "Product"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Transfer Start Time"), "TransferStartTime"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Transfer Stop Time"), "TransferStopTime"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Initiation Count"), "InitiationCount"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Level Product"), "LevelProduct"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Level Water"), "LevelWater"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Mass Liquid"), "MassLiquid"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Temperature Ambient"), "TemperatureAmbient"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Temperature Density"), "TemperatureDensity"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Temperature Product"), "TemperatureProduct"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Density Product Observed"), "DensityProductObserved"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Density Product in Air"), "DensityProductinAir"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Density Product Standard"), "DensityProductStandard"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Density Product Standard in Air"), "DensityProductStandardinAir"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Volume Correction Factor"), "VolumeCorrectionFactor"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Volume Gross Observed"), "VolumeGrossObserved"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Volume Gross Standard"), "VolumeGrossStandard"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Volume Net Standard"), "VolumeNetStandard"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Volume Total Observed"), "VolumeTotalObserved"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Volume Water"), "VolumeWater"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Volume Roof Correction"), "VolumeRoofCorrection"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Tank Shell Correction"), "TankShellCorrection"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Volume Gross Observed Rate"), "VolumeGrossObservedRate"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Volume Net Standard Rate"), "VolumeNetStandardRate"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Volume Total Observed Rate"), "VolumeTotalObservedRate"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|User Data 01"), "UserData01"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|User Data 02"), "UserData02"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|User Data 03"), "UserData03"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|User Data 04"), "UserData04"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|User Data 05"), "UserData05"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|User Data 06"), "UserData06"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|User Data 07"), "UserData07"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|User Data 08"), "UserData08"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|User Data 09"), "UserData09"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|User Data 10"), "UserData10"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Transferred GOV"), "TransferredGOV"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Transferred NSV"), "TransferredNSV"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Transferred Volume Water"), "TransferredVolumeWater"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Transferred Volume"), "TransferredVolume"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Transfer Mode"), "TransferMode"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Transfer Target"), "TransferTarget"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Transfer Level Target"), "TransferLevelTarget"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Transfer Volume Target"), "TransferVolumeTarget"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Transfer Time Remaining"), "TransferTimeRemaining"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Transfer Status"), "TransferStatus"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Temperature Ambient"), "StartTemperatureAmbient"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Density Product Observed"), "StartDensityProductObserved"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Density Product in Air"), "StartDensityProductinAir"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Density Product Standard"), "StartDensityProductStandard"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Density Product Standard in Air"), "StartDensityProductStandardinAir"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Level Product"), "TransferStartLevel"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Level Water"), "StartLevelWater"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Mass Liquid"), "StartMassLiquid"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Percent BSW"), "StartPercentBsw"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Tank Shell Correction"), "StartTankShellCorrection"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Temperature Density"), "StartTemperatureDensity"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Temperature Product"), "StartTemperatureProduct"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Volume BSW"), "StartVolumeBsw"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Volume Correction Factor"), "StartVolumeCorrectionFactor"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Volume Gross Observed"), "TransferStartGOV"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Volume Gross Standard"), "StartVolumeGrossStandard"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Volume Net Standard"), "TransferStartNSV"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Volume Roof Correction"), "StartVolumeRoofCorrection"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Volume Total Observed"), "StartVolumeTotalObserved"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Volume Water"), "TransferStartWaterVolume"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Start Volume"), "TransferStartVolume"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Temperature Ambient"), "OpeningTemperatureAmbient"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Density Product Observed"), "OpeningDensityProductObserved"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Density Product in Air"), "OpeningDensityProductinAir"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Density Product Standard"), "OpeningDensityProductStandard"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Density Product Standard in Air"), "OpeningDensityProductStandardinAir"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Level Product"), "OpeningLevelProduct"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Level Water"), "OpeningLevelWater"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Mass Liquid"), "OpeningMassLiquid"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Percent BSW"), "OpeningPercentBsw"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Tank Shell Correction"), "OpeningTankShellCorrection"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Temperature Density"), "OpeningTemperatureDensity"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Temperature Product"), "OpeningTemperatureProduct"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Volume BSW"), "OpeningVolumeBsw"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Volume Correction Factor"), "OpeningVolumeCorrectionFactor"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Volume Gross Observed"), "OpeningVolumeGrossObserved"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Volume Gross Standard"), "OpeningVolumeGrossStandard"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Volume Net Standard"), "OpeningVolumeNetStandard"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Volume Roof Correction"), "OpeningVolumeRoofCorrection"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Volume Total Observed"), "OpeningVolumeTotalObserved"));
			keyValuePairList.Add(new KeyValuePair<string, string>(dataDictionaries.Get(security.SiteGuid, "Movement|Opening Volume Water"), "OpeningVolumeWater"));

			return keyValuePairList.OrderBy(o => o.Key).ToList();
		}

		public Dictionary<Guid, PointTemplateTag> EnumerateByPointTemplateGuid( SecurityClass security, Guid pointTemplateGuid )
		{
			if ( security == null )
			{
				throw new ArgumentNullException( "security" );
			}

			// TODO: Check security rights.

			var tag = new PointTemplateTag();
			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				tag.EnumerateByPointTemplateSQL(cmd, pointTemplateGuid);
				set = ConsolidatedDa.GetDataSet( cmd, security );
			}

			DataTable table = set.Tables[0];

			var tagList = new Dictionary<Guid, PointTemplateTag>();

			foreach (DataRow row in table.Rows)
			{
				tag = new PointTemplateTag();
				tag.AutoLoad(row);
				tagList.Add(tag.IdentityGuid, tag);
			}

			this.PopulateFields(security,tagList);
			return tagList;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public List<KeyValuePair<string, string>> EnumerateAllUniqueTagNames(SecurityClass security)
		{
			List<KeyValuePair<string, string>> pointTagNames = new List<KeyValuePair<string, string>>();
			DataSet Set;
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.[usp_GetAllUniquePointTemplateTagNames]";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
				Set = ConsolidatedDa.GetDataSet(cmd, security);
				
				if (Set.Tables.Count == 0)
				{
					throw new Exception("PointTemplateTags.cs Load() Set.Tables[0]");
				}

				DataTable table = Set.Tables[0];
				if (table.Rows.Count == 0)
					return pointTagNames;
				foreach (DataRow row in table.Rows)
				{
					var ID = DataObject.getValue<string>(row["ID"], "");
					pointTagNames.Add(new KeyValuePair<string, string>(ID, ID));
				}
			}
			return pointTagNames;
		}

		public PointTemplateTag Get(SecurityClass security, Guid tagGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Add security rights check

			var consolidatedDa = new ConsolidatedDAClass();

			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				PointTemplateTag.SelectSql(cmd, tagGuid);
				dataSet = consolidatedDa.GetDataSet(cmd, security);
			}

			var pointTemplateTag = new PointTemplateTag();

			DataTable table = dataSet.Tables[0];

			if (table.Rows.Count > 0)
			{
				pointTemplateTag.AutoLoad(table.Rows[0]);
			}

			var alarmTemplates = new AlarmTemplates();
			List<Guid> tagList = new List<Guid>();
			tagList.Add(tagGuid);
			var tagAlarmDictionary = alarmTemplates.EnumerateByPointTemplateTagGuids(security, tagList);
			Dictionary<Guid, AlarmTemplate> alarmDictionary;
			if (tagAlarmDictionary.TryGetValue(tagGuid, out alarmDictionary))
			{
				pointTemplateTag.AlarmTemplates = alarmDictionary;
			}

			return pointTemplateTag;
		}
	}
}
