namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Security;
	using System.ServiceModel;
	using Microsoft.SqlServer.Server;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using DataAccessLayer;
	using Opc.Ua;

	using FMCore;

	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class PointTags : FMServiceBase, IPointTags
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();


		protected void AddAlarms(SecurityClass security,Dictionary<Guid, PointTag> tags,Dictionary<Guid, Guid> pointTagGuidDictionary)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights.

			List<Alarm> alarmList = new List<Alarm>();
			foreach (var tag in tags.Values)
			{
				foreach (var alarmTemplate in tag.Alarms)
				{
					foreach (var alarmTest in alarmTemplate.Value.AlarmTests)
					{
						alarmTest.Value.LimitTagGuid = pointTagGuidDictionary[alarmTest.Value.LimitTagGuid];
					}
					alarmTemplate.Value.AlarmStateTagGuid = pointTagGuidDictionary[alarmTemplate.Value.AlarmStateTagGuid];
					alarmTemplate.Value.InputTagGuid = tag.IdentityGuid;
					alarmList.Add(alarmTemplate.Value);
				}
			}
			var alarms = new Alarms();
			alarms.AddModifyAlarms(security, alarmList, true,false);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AddTags(SecurityClass security, Dictionary<Guid, PointTag> tags)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (tags == null)
			{
				throw new ArgumentNullException("tags");
			}

			if (tags.Count == 0)
			{
				return;
			}

			// TODO: Add security rights check

			var pointTagGuidDictionary = new Dictionary<Guid, Guid>();
			foreach (var tag in tags)
			{

				Guid prevIdentityGuid = tag.Key;

				tag.Value.SetCreationStamp(security);
				tag.Value.IdentityGuid = Guid.NewGuid();

				pointTagGuidDictionary.Add(prevIdentityGuid, tag.Value.IdentityGuid);
			}
			using (var cmd = new SqlCommand())
			{

				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_PointTagsInsertByPK";

				SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointTags", SqlDbType.Structured);
				tableValuedParameter.Value = CreateSqlDataRecordsForInsert(tags);
				tableValuedParameter.TypeName = "dbo.PointTagType";

				ConsolidatedDAClass consolidatedDa = new ConsolidatedDAClass();
				consolidatedDa.ExecuteQuery(security, cmd);
			}
			this.AddAlarms(security, tags, pointTagGuidDictionary);
		}
		/// <summary>
		/// Create SqlDataRecords representing tag records to insert
		/// </summary>
		/// <param name="tags">The tags to create SqlDataRecords for</param>
		/// <returns>SqlDataRecords representing tag records to insert</returns>
		private static IEnumerable<SqlDataRecord> CreateSqlDataRecordsForInsert(Dictionary<Guid, PointTag> tags)
		{
			SqlMetaData[] metaData = new SqlMetaData[41];

			int i = 0;
			metaData[i++] = new SqlMetaData("PointTagGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("ID", SqlDbType.NVarChar, 50);
			metaData[i++] = new SqlMetaData("EngineeringUnitsType", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("EngineeringUnitsIndex", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("DecimalPlaces", SqlDbType.TinyInt);
			metaData[i++] = new SqlMetaData("ServerEngineeringUnitsIndex", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("ValueType", SqlDbType.NVarChar,SqlMetaData.Max);
			metaData[i++] = new SqlMetaData("Status", SqlDbType.BigInt);
			metaData[i++] = new SqlMetaData("Value", SqlDbType.Xml);
			metaData[i++] = new SqlMetaData("ServerTimeStamp", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("SourceTimeStamp", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("Maximum", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("Minimum", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("PointTagInputOutputTypeIndex", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("Input", SqlDbType.Bit);
			metaData[i++] = new SqlMetaData("AlarmStatus", SqlDbType.Bit);
			metaData[i++] = new SqlMetaData("ApplyPointEngineeringUnits", SqlDbType.Bit);
			metaData[i++] = new SqlMetaData("ApplyPointDecimalPlaces", SqlDbType.Bit);
			metaData[i++] = new SqlMetaData("ApplyPointMaximum", SqlDbType.Bit);
			metaData[i++] = new SqlMetaData("ApplyPointMinimum", SqlDbType.Bit);
			metaData[i++] = new SqlMetaData("OpcUaServerGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("OpcUaBrowsePath", SqlDbType.NVarChar,250);
			metaData[i++] = new SqlMetaData("OpcUaNamespaceUri", SqlDbType.NVarChar,250);
			metaData[i++] = new SqlMetaData("OpcUaPublishingInterval", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("OpcUaNodeId", SqlDbType.NVarChar,250);
			metaData[i++] = new SqlMetaData("OpcUaIsReadable", SqlDbType.Bit);
			metaData[i++] = new SqlMetaData("OpcUaServerDataType", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("OpcUaWriteHoldoffTime", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("OpcUaWritePeriodicUpdateInterval", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("AlarmsEnabled", SqlDbType.Bit);
			metaData[i++] = new SqlMetaData("InhibitInputOutputTypeConfiguration", SqlDbType.Bit);
			metaData[i++] = new SqlMetaData("InhibitOverride", SqlDbType.Bit);
			metaData[i++] = new SqlMetaData("Deadband", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("Holdoff", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("Archived", SqlDbType.Bit);
			metaData[i++] = new SqlMetaData("CreatedDate", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("CreatedBy", SqlDbType.NVarChar, 30);
			metaData[i++] = new SqlMetaData("UpdatedDate", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("UpdatedBy", SqlDbType.NVarChar, 30 );
			metaData[i++] = new SqlMetaData("PointGuid", SqlDbType.UniqueIdentifier);
			metaData[i] = new SqlMetaData("PointTemplateTagGuid", SqlDbType.UniqueIdentifier);

			SqlDataRecord record = new SqlDataRecord(metaData);

			foreach (var tag in tags)
			{
				int j = 0;

				record.SetValue(j++, tag.Value.PointTagGuid);
				record.SetValue(j++, tag.Value.ID);
				record.SetValue(j++, tag.Value.EngineeringUnitsType);
				record.SetValue(j++, tag.Value.Units);
				record.SetValue(j++, tag.Value.DecimalPlaces);
				record.SetValue(j++, tag.Value.ServerUnits);
				record.SetValue(j++, tag.Value.ValueType.ToString());
				record.SetValue(j++, tag.Value.Status);
				record.SetValue(j++, tag.Value.ValueXml);
				record.SetValue(j++, tag.Value.ServerTimeStamp);
				record.SetValue(j++, tag.Value.SourceTimeStamp);
				record.SetValue(j++, tag.Value.Maximum);
				record.SetValue(j++, tag.Value.Minimum);
				record.SetValue(j++, tag.Value.InputOutputType);
				record.SetValue(j++, tag.Value.Input);
				record.SetValue(j++, tag.Value.AlarmStatus);
				record.SetValue(j++, tag.Value.ApplyPointEngineeringUnits);
				record.SetValue(j++, tag.Value.ApplyPointDecimalPlaces);
				record.SetValue(j++, tag.Value.ApplyPointMaximum);
				record.SetValue(j++, tag.Value.ApplyPointMinimum);
				if ((tag.Value.OpcUaServerGuid == Guid.Empty))
				{
					record.SetValue(j++, DBNull.Value);
				}
				else
				{ 
					record.SetValue(j++, tag.Value.OpcUaServerGuid);
				}
				record.SetValue(j++, tag.Value.OpcUaBrowsePath);
				record.SetValue(j++, tag.Value.OpcUaNamespaceUri);
				record.SetValue(j++, tag.Value.OpcUaPublishingInterval);
				record.SetValue(j++, tag.Value.OpcUaNodeId);
				record.SetValue(j++, tag.Value.OpcUaIsReadable);
				record.SetValue(j++, tag.Value.OpcUaServerDataType);
				record.SetValue(j++, tag.Value.OpcUaWriteHoldoffTime);
				record.SetValue(j++, tag.Value.OpcUaWritePeriodicUpdateInterval);
				record.SetValue(j++, tag.Value.AlarmsEnabled);
				record.SetValue(j++, tag.Value.InhibitInputOutputTypeConfiguration);
				record.SetValue(j++, tag.Value.InhibitOverride);
				record.SetValue(j++, tag.Value.Deadband);
				record.SetValue(j++, tag.Value.Holdoff);
				record.SetValue(j++, tag.Value.Archived);
				record.SetValue(j++, tag.Value.CreatedDate);
				record.SetValue(j++, tag.Value.CreatedBy);
				record.SetValue(j++, tag.Value.UpdatedDate);
				record.SetValue(j++, tag.Value.UpdatedBy);
				record.SetValue(j++, tag.Value.PointGuid);
				if (tag.Value.PointTemplateTagGuid == Guid.Empty)
				{
					record.SetValue(j, DBNull.Value);
				}
				else
				{
					record.SetValue(j, tag.Value.PointTemplateTagGuid);
				}
			yield return record;
			}
		}
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteTags(SecurityClass security, Guid pointGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Add security rights check
			var alarms = new Alarms();
			var pointGuidList = new List<Guid>();
			pointGuidList.Add(pointGuid);
			alarms.DeleteAlarmsByPointGuidList(security, pointGuidList);
			var pointTag = new PointTag();
			pointTag.DeleteTags(security, pointGuid);
		}

		private bool OpcUaServerChanges(OpcUAServerClass opcUaServer, PointTag tag)
		{
			if (opcUaServer.SecurityMode != tag.OpcUaSecurityMode || opcUaServer.SecurityPolicy != tag.OpcUaSecurityPolicy
			    || opcUaServer.MessageEncoding != tag.OpcUaMessageEncoding
			    || opcUaServer.UserIdentityMethod != tag.OpcUaUserIdentityMethod || opcUaServer.UserId != tag.OpcUaUserId
			    || opcUaServer.UserPassword != tag.OpcUaUserPassword
			    || opcUaServer.UserCertificatePath != tag.OpcUaUserCertificatePath)
			{
				return true;
			}
			return false;
		}

		private void ApplyOpcUaServerChanges(SecurityClass security, OpcUAServerClass opcUaServer, PointTag tag)
		{
			opcUaServer.ServerEndPoint = tag.OpcUaServerEndPoint;
			opcUaServer.SecurityMode = tag.OpcUaSecurityMode;
			opcUaServer.SecurityPolicy = tag.OpcUaSecurityPolicy;
			opcUaServer.MessageEncoding = tag.OpcUaMessageEncoding;
			opcUaServer.UserIdentityMethod = tag.OpcUaUserIdentityMethod;
			opcUaServer.UserId = tag.OpcUaUserId;
			opcUaServer.UserPassword = tag.OpcUaUserPassword;
			opcUaServer.UserCertificatePath = tag.OpcUaUserCertificatePath;
			opcUaServer.SiteGuid = security.SiteGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, PointTag tag)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}

			// TODO: Add security rights check

			var opcUaServers = new OpcUAServersClass();
			Guid opcUaServerGuidToPurge = Guid.Empty;

			if (tag.OpcUaServerGuid != Guid.Empty)
			{
				// Endpoint cleared - purge exising Endpoint
				if (string.IsNullOrEmpty(tag.OpcUaServerEndPoint))
				{
					opcUaServerGuidToPurge = tag.OpcUaServerGuid;
					tag.OpcUaServerGuid = Guid.Empty;
				}

				else
				{
					var opcUaServer = opcUaServers.Get(security, tag.OpcUaServerGuid);

					// No Endpoint Change - apply any changes
					if (opcUaServer.ServerEndPoint == tag.OpcUaServerEndPoint)
					{
						if (OpcUaServerChanges(opcUaServer, tag))
						{
							ApplyOpcUaServerChanges(security, opcUaServer, tag);

							opcUaServers.Modify(security, opcUaServer);
						}
					}
					else
					{
						opcUaServer = opcUaServers.GetByEndpoint(security, tag.OpcUaServerEndPoint);

						// Endpoint Changed - apply any changes
						if (opcUaServer != null)
						{
							opcUaServerGuidToPurge = tag.OpcUaServerGuid;
							tag.OpcUaServerGuid = opcUaServer.IdentityGuid;

							if (OpcUaServerChanges(opcUaServer, tag))
							{
								ApplyOpcUaServerChanges(security, opcUaServer, tag);

								opcUaServers.Modify(security, opcUaServer);
							}
						}

						// New Endpoint
						else
						{
							opcUaServer = new OpcUAServerClass();

							ApplyOpcUaServerChanges(security, opcUaServer, tag);

							tag.OpcUaServerGuid = opcUaServers.Add(security, opcUaServer);
						}
					}
				}
			}

			else if (tag.OpcUaServerGuid == Guid.Empty && !string.IsNullOrEmpty(tag.OpcUaServerEndPoint))
			{
				var opcUaServer = opcUaServers.GetByEndpoint(security, tag.OpcUaServerEndPoint);

				// Endpoint Changed - apply any changes
				if (opcUaServer != null)
				{
					opcUaServerGuidToPurge = tag.OpcUaServerGuid;
					tag.OpcUaServerGuid = opcUaServer.IdentityGuid;

					if (OpcUaServerChanges(opcUaServer, tag))
					{
						ApplyOpcUaServerChanges(security, opcUaServer, tag);

						opcUaServers.Modify(security, opcUaServer);
					}
				}

				// New Endpoint
				else
				{
					opcUaServer = new OpcUAServerClass();

					ApplyOpcUaServerChanges(security, opcUaServer, tag);

					tag.OpcUaServerGuid = opcUaServers.Add(security, opcUaServer);
				}
			}

			if (tag.IsForced()
			&& (tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual
			|| tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.UnAssigned))
			{
				if (tag.Value == null)
				{
					tag.Status = StatusCodes.Bad;
				}
				else
				{
					tag.Status = StatusCodes.Good;
				}
			}

			if (tag.Value != null)
			{
				if(tag.Value.GetType() == typeof(PointCommandStatusListReference))
				{
					var pointCommandStatusListReference = tag.Value as PointCommandStatusListReference;

					if ((tag.OpcStatusCodeBits == StatusCodes.GoodLocalOverride
					|| tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual)
					&& pointCommandStatusListReference.CurrentValue.HasValue)
					{
						pointCommandStatusListReference.CurrentKey = GetPointCommandStatusListKey(security,
							tag.PointTemplateTagGuid,
							pointCommandStatusListReference.PointCommandStatusListGuid,
							pointCommandStatusListReference.CurrentValue.Value);
					}
					else if (!string.IsNullOrEmpty(pointCommandStatusListReference.CurrentKey))
					{
						pointCommandStatusListReference.CurrentValue = GetPointCommandStatusListValue(security,
							tag.PointTemplateTagGuid,
							pointCommandStatusListReference.PointCommandStatusListGuid,
							pointCommandStatusListReference.CurrentKey);
					}
					else
					{
						pointCommandStatusListReference.CurrentKey = string.Empty;
						tag.Status = StatusCodes.Bad;
					}
				}

				if (tag.Value.GetType() == typeof(DeviceAlarmMapReference))
				{
					var deviceAlarmMapReference = tag.Value as DeviceAlarmMapReference;

					if(!deviceAlarmMapReference.CurrentValue.HasValue)
					{
						tag.Status = StatusCodes.Bad;
					}
				}
			}



			tag.ModifyTag(security);




			if (opcUaServerGuidToPurge != Guid.Empty)
			{
				opcUaServers.Purge(security, opcUaServerGuidToPurge);
			}
		}

		public void PurgeByPointGuidAndNotInList(SecurityClass security, Guid pointGuid, List<Guid> tagList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SET NOCOUNT ON"
										+ " DELETE pagtpt FROM map.tblPointAccessGroupToPointTag pagtpt"
										+ " INNER JOIN tblPointTag pt ON pt.PointTagGuid = pagtpt.TagGuid"
										+ " WHERE PointGuid = @PointGuid AND PointTagGuid NOT IN (SELECT * FROM @PointTagGuidList)"
										+ ""
										+ " DELETE FROM map.tblTrendPenToPointTrend WHERE PointTagGuid IN (SELECT PointTagGuid FROM tblPointTag WHERE PointGuid = @PointGuid AND PointTagGuid NOT IN (SELECT * FROM @PointTagGuidList))"
										+ ""
										+ " DELETE FROM tblPointTag WHERE PointGuid = @PointGuid AND PointTagGuid NOT IN (SELECT * FROM @PointTagGuidList)";

				cmd.CommandType = CommandType.Text;

				using (var parameterTempTable = new DataTable())
				{
					parameterTempTable.Columns.Add("PointTagGuid", typeof(Guid));

					foreach (var pointTagGuid in tagList)
					{
						parameterTempTable.Rows.Add(pointTagGuid);
					}

					var pList = new SqlParameter("@PointTagGuidList", SqlDbType.Structured);
					pList.TypeName = "dbo.GuidListType";
					pList.Value = parameterTempTable;

					cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
					cmd.Parameters.Add(pList);
					ConsolidatedDa.ExecuteQuery(security, cmd);
				}
			}
		}

		public int? GetPointCommandStatusListValue(SecurityClass security, Guid pointTemplateTagGuid, Guid pointCommandStatusListGuid, string currentKey)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			DataSet set;

			using (SqlCommand cmd = new SqlCommand())
			{
				// Execute the stored procedure, passing in the list (table) of alarms and event log records
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.CommandText = "usp_GetPointCommandStatusValue";

				cmd.Parameters.AddWithValue("@PointTemplateTagGuid", pointTemplateTagGuid);
				cmd.Parameters.AddWithValue("@ListGuid", pointCommandStatusListGuid.ToString());
				cmd.Parameters.AddWithValue("@Key", currentKey.ToString());

				set = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			int? value = null;

			if (set.Tables.Count == 1 && set.Tables[0].Rows.Count == 1)
			{
				DataTable table = set.Tables[0];
				var valueString = table.Rows[0]["Value"] as string;
				value = new int?(Convert.ToInt32(valueString));
			}

			return value;
		}


		public string GetPointCommandStatusListKey(SecurityClass security, Guid pointTemplateTagGuid, Guid pointCommandStatusListGuid, int currentValue)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			DataSet set;

			using (SqlCommand cmd = new SqlCommand())
			{
				// Execute the stored procedure, passing in the list (table) of alarms and event log records
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.CommandText = "usp_GetPointCommandStatusKey";

				cmd.Parameters.AddWithValue("@PointTemplateTagGuid", pointTemplateTagGuid);
				cmd.Parameters.AddWithValue("@ListGuid", pointCommandStatusListGuid.ToString());
				cmd.Parameters.AddWithValue("@Value", currentValue.ToString());

				set = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			string key = string.Empty;

			if (set.Tables.Count == 1 && set.Tables[0].Rows.Count == 1)
			{
				DataTable table = set.Tables[0];
				key = table.Rows[0]["Key"] as string;
			}

			return key;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyTagValues(SecurityClass security, List<PointTag> pointTags, bool enterpriseVisibility)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			DataTable pointTagDataTable = new DataTable();
			pointTagDataTable.Columns.Add("PointTagGuid", typeof(Guid));
			pointTagDataTable.Columns.Add("EngineeringUnitsType", typeof(int));
			pointTagDataTable.Columns.Add("EngineeringUnitsIndex", typeof(int));
			pointTagDataTable.Columns.Add("DecimalPlaces", typeof(byte));
			pointTagDataTable.Columns.Add("Maximum", typeof(double));
			pointTagDataTable.Columns.Add("Minimum", typeof(double));
			pointTagDataTable.Columns.Add("Value", typeof(string));
			pointTagDataTable.Columns.Add("Status", typeof(Int64));
			pointTagDataTable.Columns.Add("ServerTimeStamp", typeof(DateTimeOffset));
			pointTagDataTable.Columns.Add("SourceTimeStamp", typeof(DateTimeOffset));

			foreach (var pointTag in pointTags)
			{
				var row = pointTagDataTable.NewRow();
				row["PointTagGuid"] = pointTag.IdentityGuid;
				row["EngineeringUnitsType"] = pointTag.EngineeringUnitsType;
				row["EngineeringUnitsIndex"] = pointTag.Units;
				row["DecimalPlaces"] = pointTag.DecimalPlaces;
				row["Maximum"] = pointTag.Maximum;
				row["Minimum"] = pointTag.Minimum;

				if(pointTag.Value != null
				&& pointTag.Value.GetType() == typeof(PointCommandStatusListReference))
				{
					var pointCommandStatusListReference = pointTag.Value as PointCommandStatusListReference;
					if ((pointTag.OpcStatusCodeBits == StatusCodes.GoodLocalOverride
					|| pointTag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual)
					&& pointCommandStatusListReference.CurrentValue.HasValue)
					{
						pointCommandStatusListReference.CurrentKey = GetPointCommandStatusListKey(	security,
																															pointTag.PointTemplateTagGuid,
																															pointCommandStatusListReference.PointCommandStatusListGuid,
																															pointCommandStatusListReference.CurrentValue.Value);
					}
					else if(!string.IsNullOrEmpty(pointCommandStatusListReference.CurrentKey))
					{
						pointCommandStatusListReference.CurrentValue = GetPointCommandStatusListValue(	security,
																																pointTag.PointTemplateTagGuid,
																																pointCommandStatusListReference.PointCommandStatusListGuid,
																																pointCommandStatusListReference.CurrentKey);
					}
					else
					{
						pointCommandStatusListReference.CurrentKey = string.Empty;
					}
				}

				row["Value"] = pointTag.ValueXml;
				row["Status"] = pointTag.Status;
				row["ServerTimeStamp"] = pointTag.ServerTimeStamp;
				row["SourceTimeStamp"] = pointTag.SourceTimeStamp;
				pointTagDataTable.Rows.Add(row);
			}

			// TODO: Check for appropriate security rights

			using (SqlCommand cmd = new SqlCommand())
			{
				// Execute the stored procedure, passing in the list (table) of alarms and event log records
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_PointTagDataUpdate";

				cmd.Parameters.AddWithValue("@User", security.UserID);
				cmd.Parameters.AddWithValue("@EnterpriseVisibility", enterpriseVisibility);
				SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointTagData", SqlDbType.Structured);
				tableValuedParameter.Value = pointTagDataTable;
				tableValuedParameter.TypeName = "dbo.PointTagDataType";

				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyPointValues(SecurityClass security, List<PointValue> pointValues, bool enterpriseVisibility)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}


			DataTable pointTagDataTable = new DataTable();
			pointTagDataTable.Columns.Add("PointTagGuid", typeof(Guid));
			pointTagDataTable.Columns.Add("EngineeringUnitsType", typeof(int));
			pointTagDataTable.Columns.Add("EngineeringUnitsIndex", typeof(int));
			pointTagDataTable.Columns.Add("DecimalPlaces", typeof(byte));
			pointTagDataTable.Columns.Add("Maximum", typeof(double));
			pointTagDataTable.Columns.Add("Minimum", typeof(double));
			pointTagDataTable.Columns.Add("Value", typeof(string));
			pointTagDataTable.Columns.Add("Status", typeof(Int64));
			pointTagDataTable.Columns.Add("ServerTimeStamp", typeof(DateTimeOffset));
			pointTagDataTable.Columns.Add("SourceTimeStamp", typeof(DateTimeOffset));

			foreach (var pointValue in pointValues)
			{
				if(pointValue.PointValueIdentifier.PointValueType != PointValueType.Tag)
				{
					continue;
				}

				var row = pointTagDataTable.NewRow();
				row["PointTagGuid"] = pointValue.PointValueIdentifier.IdentityGuid;
				row["EngineeringUnitsType"] = pointValue.EngineeringUnitsType;
				row["EngineeringUnitsIndex"] = pointValue.Units;
				row["DecimalPlaces"] = pointValue.DecimalPlaces;
				row["Maximum"] = pointValue.Maximum;
				row["Minimum"] = pointValue.Minimum;

				if (pointValue.Value != null
				&& pointValue.Value.GetType() == typeof(PointCommandStatusListReference))
				{
					if ((pointValue.Value as PointCommandStatusListReference).CurrentValue.HasValue)
					{
						(pointValue.Value as PointCommandStatusListReference).CurrentKey = GetPointCommandStatusListKey(security,
																																						pointValue.PointTemplateTagGuid,
																																						(pointValue.Value as PointCommandStatusListReference).PointCommandStatusListGuid,
																																						(pointValue.Value as PointCommandStatusListReference).CurrentValue.Value);
					}
					else
					{
						(pointValue.Value as PointCommandStatusListReference).CurrentKey = string.Empty;
					}
				}

				row["Value"] = pointValue.ValueXml;
				row["Status"] = pointValue.Status;
				row["ServerTimeStamp"] = pointValue.ServerTimeStamp;
				row["SourceTimeStamp"] = pointValue.SourceTimeStamp;
				pointTagDataTable.Rows.Add(row);
			}

			// TODO: Check for appropriate security rights

			using (SqlCommand cmd = new SqlCommand())
			{
				// Execute the stored procedure, passing in the list (table) of alarms and event log records
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_PointTagDataUpdate";

				cmd.Parameters.AddWithValue("@User", security.UserID);
				cmd.Parameters.AddWithValue("@EnterpriseVisibility", enterpriseVisibility);
				SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointTagData", SqlDbType.Structured);
				tableValuedParameter.Value = pointTagDataTable;
				tableValuedParameter.TypeName = "dbo.PointTagDataType";

				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}



		public PointTag Get(SecurityClass security, Guid tagGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Add security rights check

			var pointTag = new PointTag();
			var tag = pointTag.Get(security, tagGuid);
			var alarms = new Alarms();
			List<Guid> tagList = new List<Guid>();
			tagList.Add(tagGuid);
			var tagAlarmDictionary = alarms.EnumerateByPointTagGuids(security, tagList);
			Dictionary<Guid, Alarm> alarmDictionary;
			if (tagAlarmDictionary.TryGetValue(tagGuid, out alarmDictionary))
			{
				tag.Alarms = alarmDictionary;
			}
         
			return tag;
		}

		public Guid GetIdentityGuid(SecurityClass security, string pointTagID, Guid pointGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			DataSet set;
			var pointTagGuid = Guid.Empty;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT PointTagGuid FROM dbo.tblPointTag WHERE ID = @ID AND PointGuid = @PointGuid";
				cmd.Parameters.AddWithValue("@ID", pointTagID);
				cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
				set = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			if (set.Tables.Count == 1 && set.Tables[0].Rows.Count == 1)
			{
				DataTable table = set.Tables[0];
				pointTagGuid = (Guid)table.Rows[0]["PointTagGuid"];
			}

			return pointTagGuid;
		}

		protected void PopulateFields(SecurityClass security, Dictionary<Guid, PointTag> tagDictionary)
		{
			if (tagDictionary.Count > 0)
			{
				var alarms = new Alarms();
				var tagAlarmDictionary = alarms.EnumerateByPointTagGuids(security, tagDictionary.Keys.ToList());
				foreach (var alarmDictionary in tagAlarmDictionary)
				{
					PointTag tag;
					if (tagDictionary.TryGetValue(alarmDictionary.Key, out tag))
					{
						tag.Alarms = alarmDictionary.Value;
					}
				}
			}
		}

		public Dictionary<Guid, PointTag> EnumerateByPointGuid(SecurityClass security, Guid pointGuid, bool enforcePointAccess = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Add security rights check

			DataSet dataSet = null;

			using (var cmd = new SqlCommand())
			{
				var pointTag = new PointTag();
				if (enforcePointAccess == false)
				{
					pointTag.EnumerateByPointSql(cmd, pointGuid);
				}
				else
				{
					pointTag.EnumerateGuidAndIdByPointGuidSql(cmd, security.SiteGuid, security.UserGuid, pointGuid);
				}
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var tagDictionary = new Dictionary<Guid, PointTag>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var tag = new PointTag();

				tag.AutoLoad(row);
				tagDictionary.Add(tag.IdentityGuid, tag);
			}

			this.PopulateFields(security,tagDictionary);
			return tagDictionary;
		}


		public Dictionary<Guid, string> EnumerateIdByPointGuid(SecurityClass security, Guid pointGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Add security rights check

			DataSet dataSet = null;

			using (var cmd = new SqlCommand())
			{
				var pointTag = new PointTag();
				pointTag.EnumerateGuidAndIdByPointGuidSql(cmd, security.SiteGuid, security.UserGuid, pointGuid);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var tagIDDictionary = new Dictionary<Guid, string>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				tagIDDictionary.Add((Guid) row["PointTagGuid"], (string) row["ID"]);
			}

			return tagIDDictionary;
		}


		public Dictionary<Guid, Dictionary<Guid, PointTag>> EnumerateForSimulator(
			SecurityClass security,
			string opcUaEndPoint)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Add security rights check

			DataSet dataSet = null;

			using (var cmd = new SqlCommand())
			{
				var pointTag = new PointTag();
				pointTag.EnumerateForSimulatorSql(cmd, opcUaEndPoint);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}
			Dictionary<Guid, PointTag> totalTagDictionary = new Dictionary<Guid, PointTag>();
			var pointTagDictionary = new Dictionary<Guid, Dictionary<Guid, PointTag>>();
			Dictionary<Guid, PointTag> tagDictionary = null;
			Guid? pointGuid = null;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var tag = new PointTag();

				tag.AutoLoad(row);
				if (tagDictionary == null || pointGuid.Value != tag.PointGuid)
				{
					pointGuid = tag.PointGuid;
					tagDictionary = new Dictionary<Guid, PointTag>();
					pointTagDictionary.Add(pointGuid.Value, tagDictionary);
				}

				tagDictionary.Add(tag.IdentityGuid, tag);
				totalTagDictionary.Add(tag.IdentityGuid, tag);
			}

			this.PopulateFields(security,totalTagDictionary);
			return pointTagDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, PointTag>> EnumerateForHostName(
			SecurityClass security,
			string hostname,
			int startIndex,
			int count)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Add security rights check

			DataSet dataSet = null;

			using (var cmd = new SqlCommand())
			{
				var pointTag = new PointTag();
				pointTag.EnumerateForHostNameSql(cmd, hostname, startIndex, count);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			Dictionary<Guid, PointTag> totalTagDictionary = new Dictionary<Guid, PointTag>();
			var pointTagDictionary = new Dictionary<Guid, Dictionary<Guid, PointTag>>();
			Dictionary<Guid, PointTag> tagDictionary = null;
			Guid? pointGuid = null;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var tag = new PointTag();

				tag.AutoLoad(row);
				if (tagDictionary == null || pointGuid.Value != tag.PointGuid)
				{
					pointGuid = tag.PointGuid;
					tagDictionary = new Dictionary<Guid, PointTag>();
					pointTagDictionary.Add(pointGuid.Value, tagDictionary);
				}

				tagDictionary.Add(tag.IdentityGuid, tag);
				totalTagDictionary.Add(tag.IdentityGuid, tag);
			}

			this.PopulateFields(security, totalTagDictionary);
			return pointTagDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, PointTag>> Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Add security rights check

			DataSet dataSet = null;

			using (var cmd = new SqlCommand())
			{
				var pointTag = new PointTag();
				pointTag.EnumerateSql(cmd);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			Dictionary<Guid, PointTag> totalTagDictionary = new Dictionary<Guid, PointTag>();
			var pointTagDictionary = new Dictionary<Guid, Dictionary<Guid, PointTag>>();
			Dictionary<Guid, PointTag> tagDictionary = null;
			Guid? pointGuid = null;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var tag = new PointTag();

				tag.AutoLoad(row);
				if (tagDictionary == null || pointGuid.Value != tag.PointGuid)
				{
					pointGuid = tag.PointGuid;
					tagDictionary = new Dictionary<Guid, PointTag>();
					pointTagDictionary.Add(pointGuid.Value, tagDictionary);
				}

				tagDictionary.Add(tag.IdentityGuid, tag);
				totalTagDictionary.Add(tag.IdentityGuid, tag);
			}

			this.PopulateFields(security, totalTagDictionary);
			return pointTagDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, PointTag>> EnumerateBySiteGuid(SecurityClass security, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Add security rights check

			DataSet dataSet = null;

			using (var cmd = new SqlCommand())
			{
				var pointTag = new PointTag();
				pointTag.EnumerateBySiteSql(cmd, siteGuid);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var pointTagDictionary = new Dictionary<Guid, Dictionary<Guid, PointTag>>();
			Dictionary<Guid, PointTag> tagDictionary = null;
			Guid? pointGuid = null;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var tag = new PointTag();

				tag.AutoLoad(row);
				if (tagDictionary == null || pointGuid.Value != tag.PointGuid)
				{
					pointGuid = tag.PointGuid;
					tagDictionary = new Dictionary<Guid, PointTag>();
					pointTagDictionary.Add(pointGuid.Value, tagDictionary);
				}

				tagDictionary.Add(tag.IdentityGuid, tag);
			}


			return pointTagDictionary;
		}

		public Dictionary<Guid, Dictionary<Guid, PointTag>> EnumerateByPointList(
			SecurityClass security,
			List<Guid> pointGuidList,
			List<string> tagIDFilter = null
			)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Add security rights check

			DataSet dataSet = null;

			using (var cmd = new SqlCommand())
			{
				PointTagsDao.EnumerateByPointListSql(cmd, pointGuidList, tagIDFilter);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			Dictionary<Guid, PointTag> totalTagDictionary = new Dictionary<Guid, PointTag>();
			var pointTagDictionary = new Dictionary<Guid, Dictionary<Guid, PointTag>>();
			Dictionary<Guid, PointTag> tagDictionary = null;
			Guid? pointGuid = null;

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var tag = new PointTag();

				tag.AutoLoad(row);
				if (tagDictionary == null || pointGuid.Value != tag.PointGuid)
				{
					pointGuid = tag.PointGuid;
					tagDictionary = new Dictionary<Guid, PointTag>();
					pointTagDictionary.Add(pointGuid.Value, tagDictionary);
				}

				tagDictionary.Add(tag.IdentityGuid, tag);
				totalTagDictionary.Add(tag.IdentityGuid, tag);
			}

			this.PopulateFields(security, totalTagDictionary);
			return pointTagDictionary;
		}

		public List<Guid> EnumerateTagListByPointAccess(SecurityClass security, List<Guid> tagGuidList)
		{
			security.ThrowIfNull("security");

			DataSet dataSet = null;

			using (var cmd = new SqlCommand())
			{
				PointTagsDao.EnumerateTagListByPointAccessSql(cmd, tagGuidList, security.SiteGuid, security.UserGuid);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			List<Guid> tagList = new List<Guid>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				tagList.Add((Guid) row["PointTagGuid"]);
			}

			return tagList;
		}

        public Dictionary<String, Guid> EnumerateTagListByOpcUaNodeId(SecurityClass security, List<String> OpcUaNodeIds)
        {
            security.ThrowIfNull("security");

            DataSet dataSet = null;

            using (var cmd = new SqlCommand())
            {
                PointTagsDao.EnumerateTagListByOpcUaNodeIdSql(cmd, OpcUaNodeIds);
                dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
            }

            Dictionary<String, Guid> tagDictionary = new Dictionary<String, Guid>();

            DataTable table = dataSet.Tables[0];

            foreach (DataRow row in table.Rows)
            {
                tagDictionary.Add( (String)row["OpcUaNodeId"], (Guid)row["PointTagGuid"]);
            }

            return tagDictionary;
        }


        public Dictionary<Guid, PointTag> EnumerateByTagList(SecurityClass security, List<Guid> tagGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Add security rights check

			DataSet dataSet = null;

			using (var cmd = new SqlCommand())
			{
				PointTagsDao.EnumerateByTagListSql(cmd, tagGuidList);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			Dictionary<Guid, PointTag> tagDictionary = new Dictionary<Guid, PointTag>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var tag = new PointTag();

				tag.AutoLoad(row);

				tagDictionary.Add(tag.IdentityGuid, tag);
			}
			this.PopulateFields(security,tagDictionary);
			return tagDictionary;
		}


		/// <summary>
		/// Gets the maximum point tag row version.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns></returns>
		public Int64? GetMaxPointTagRowVersion(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT MAX(RowVersion) AS RowVersion FROM"
				                  + " (SELECT MAX(UpdatedRowVersion) AS RowVersion FROM track.tblPointTag WHERE UpdatedRowVersion <  MIN_ACTIVE_ROWVERSION()"
				                  + " UNION SELECT MAX(InsertedRowVersion) AS RowVersion FROM track.tblPointTag WHERE InsertedRowVersion <  MIN_ACTIVE_ROWVERSION()"
				                  + " UNION SELECT MAX(DeletedRowVersion) AS RowVersion FROM track.tblPointTag WHERE DeletedRowVersion <  MIN_ACTIVE_ROWVERSION()) RowVersions";

				set = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			DataTable table = set.Tables[0];
			DataRow row = table.Rows[0];

			if (row.IsNull("RowVersion"))
			{
				return null;
			}

			return BaseDataObject.RowVersionToInt64(row["RowVersion"] as byte[]);
		}

		/// <summary>
		/// Gets the maximum point tag row version.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns></returns>
		public List<Guid> EnumerateArchivedPointTagGuidsBySite(SecurityClass security, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT pt.PointTagGuid FROM dbo.tblPointTag pt"
										+ " INNER JOIN dbo.tblPoint p ON p.PointGuid = pt.PointGuid"
										+ " WHERE p.SiteGuid = @SiteGuid AND pt.Archived = 1";

				cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);


				set = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
			{
				return new List<Guid>();
			}

			DataTable table = set.Tables[0];

			var pointTagGuidList = new List<Guid>(set.Tables[0].Rows.Count);

			foreach (DataRow row in set.Tables[0].Rows)
			{
				pointTagGuidList.Add((Guid)row["PointTagGuid"]);
			}

			return pointTagGuidList;
		}

		/// <summary>
		/// Retrieve a list of Tags for the specified list of points knowing the tag ID
		/// For example, I want to retrieve the tags called 'Water Volume' for a list of points
		/// </summary>
		/// <param name="security"></param>
		/// <param name="points"></param>
		/// <param name="tagID"></param>
		/// <returns></returns>
		public List<PointTag> EnumerateTagsByPointList(SecurityClass security, List<Guid> points, string tagID)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "exec usp_GetTagsByPointListAndTagID @pointGuidList, @tagID";

				using (var parameterTempTable = new DataTable())
				{
					parameterTempTable.Columns.Add("pointGuid", typeof(Guid));

					foreach ( var pointGuid in points)
					{
						parameterTempTable.Rows.Add(pointGuid);
					}

					var pList = new SqlParameter("@pointGuidList", SqlDbType.Structured);
					pList.TypeName = "dbo.GuidListType";
					pList.Value = parameterTempTable;

					cmd.Parameters.Add(pList);

					var pList2 = new SqlParameter("@tagID", SqlDbType.NVarChar);
					pList2.Value = tagID;

					cmd.Parameters.Add(pList2);



					set = this.ConsolidatedDa.GetDataSet(cmd, security);
				}
			}

			if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
			{
				return new List<PointTag>();
			}

			DataTable table = set.Tables[0];

			var pointTagList = new List<PointTag>(set.Tables[0].Rows.Count);
			var tagDictionary = new Dictionary<Guid, PointTag>();
			foreach (DataRow row in set.Tables[0].Rows)
			{
				var tag = new PointTag();
				tag.AutoLoad(row);
				pointTagList.Add(tag);
				tagDictionary.Add(tag.IdentityGuid, tag);
			}
			this.PopulateFields(security, tagDictionary);
			return pointTagList;
		}

		/// <summary>
		/// Retrieve a list of PointValueIdentiers for the specified list of points and WellKnownTagGuids
		/// </summary>
		/// <param name="security"></param>
		/// <param name="pointGuids"></param>
		/// <param name="wellKnownTagGuids"></param>
		/// <returns></returns>
		public List<PointValueIdentifier> EnumeratePointValueIdentifersByPointAndTagLists(SecurityClass security, List<Guid> pointGuids, List<Guid> tagGuids)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pointGuids == null)
			{
				throw new ArgumentNullException("pointGuids");
			}

			if (tagGuids == null)
			{
				throw new ArgumentNullException("tagGuids");
			}

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT"
										+ " CASE"
										+ " WHEN tl.Guid = 'F1248A03-4E5B-4670-AC64-FA31FCB764E7' THEN pl.Guid"
										+ " WHEN tl.Guid = '5E2B6E97-3E50-4F16-900B-1D86DE9537F3' THEN pl.Guid"
										+ " WHEN tl.Guid = '5327ED53-7967-44E4-8A43-75AED4144469' THEN pl.Guid"
										+ " WHEN tl.Guid = 'C0C85360-EDF9-4279-B9C9-F38F23A13C26' THEN pl.Guid"
										+ " ELSE (SELECT TOP 1 PointTagGuid FROM tblPointTag pt WHERE pt.PointGuid = pl.Guid AND pt.PointTemplateTagGuid IN(SELECT PointTemplateTagGuid FROM tblPointTemplateTag ptt WHERE ptt.WellKnownIdentityGuid = tl.Guid))"
										+ " END AS PointValueGuid"
										+ ", CASE"
										+ " WHEN tl.Guid = 'F1248A03-4E5B-4670-AC64-FA31FCB764E7' THEN 2"
										+ " WHEN tl.Guid = '5E2B6E97-3E50-4F16-900B-1D86DE9537F3' THEN 2"
										+ " WHEN tl.Guid = '5327ED53-7967-44E4-8A43-75AED4144469' THEN 2"
										+ " WHEN tl.Guid = 'C0C85360-EDF9-4279-B9C9-F38F23A13C26' THEN 2"
										+ " ELSE 0"
										+ " END AS PointValueType"
										+ ", CASE"
										+ " WHEN tl.Guid = 'F1248A03-4E5B-4670-AC64-FA31FCB764E7' THEN 'PointId'"
										+ " WHEN tl.Guid = '5E2B6E97-3E50-4F16-900B-1D86DE9537F3' THEN 'ProductID'"
										+ " WHEN tl.Guid = '5327ED53-7967-44E4-8A43-75AED4144469' THEN 'CreatedBy'"
										+ " WHEN tl.Guid = 'C0C85360-EDF9-4279-B9C9-F38F23A13C26' THEN 'UpdatedBy'"
										+ " ELSE NULL"
										+ " END AS PropertyID"
										+ ", tl.Guid AS WellKnownIdentityGuid"
										+ ", pl.Guid as PointGuid"
										+ " FROM @PointList pl"
										+ " CROSS JOIN @TagList tl"
										+ " ORDER BY tl.[Order], pl.[Order]";

				using (var parameterTable1 = new DataTable())
				{
					parameterTable1.Columns.Add("pointOrder", typeof(int));
					parameterTable1.Columns.Add("pointGuid", typeof(Guid));

					int order = 0;
					foreach (var pointGuid in pointGuids)
					{
						parameterTable1.Rows.Add(new object [] { order,pointGuid});
						order++;
					}

					var pList1 = new SqlParameter("@pointList", SqlDbType.Structured)
					{
						TypeName = "dbo.OrderedGuidListType",
						Value = parameterTable1
					};
					cmd.Parameters.Add(pList1);

					using (var parameterTable2 = new DataTable())
					{
						parameterTable2.Columns.Add("tagOrder", typeof(int));
						parameterTable2.Columns.Add("tagGuid", typeof(Guid));

						order = 0;
						foreach (var tagGuid in tagGuids)
						{
							parameterTable2.Rows.Add(new object[] { order, tagGuid });
							order++;
						}

						var pList2 = new SqlParameter("@tagList", SqlDbType.Structured)
						{
							TypeName = "dbo.OrderedGuidListType",
							Value = parameterTable2
						};

						cmd.Parameters.Add(pList2);

						set = this.ConsolidatedDa.GetDataSet(cmd, security);
					}
				}
			}

			if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
			{
				return new List<PointValueIdentifier>();
			}

			DataTable table = set.Tables[0];

			var pointValueIdentifierList = new List<PointValueIdentifier>(table.Rows.Count);
			foreach (DataRow row in table.Rows)
			{
				pointValueIdentifierList.Add(new PointValueIdentifier(row.IsNull("PointValueGuid") ? Guid.Empty : (Guid) row["PointValueGuid"]
														, (PointValueType) row["PointValueType"]
														, row.IsNull("PropertyID") ? null : row["PropertyID"] as string
														, (Guid) row["WellKnownIdentityGuid"]
														, (Guid) row["PointGuid"]));
			}

			return pointValueIdentifierList;
		}

		public Dictionary<Guid, PointTag> EnumerateTagsAssociatedWithDeviceAlarmMapBySiteGuid(SecurityClass security, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Add security rights check

			DataSet dataSet = null;

			using (var cmd = new SqlCommand())
			{
				var pointTag = new PointTag();
				pointTag.EnumerateTagsAssociatedWithDeviceAlarmMapBySiteGuidSql(cmd, siteGuid);
				dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);
			}

			var tagDictionary = new Dictionary<Guid, PointTag>();

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var tag = new PointTag();

				tag.AutoLoad(row);
				tagDictionary.Add(tag.PointTagGuid, tag);
			}

			this.PopulateFields(security, tagDictionary);

			return tagDictionary;
		}
	}
}
