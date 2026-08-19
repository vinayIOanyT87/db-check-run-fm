
namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;
	using FMBusinessServices.InternalInterfaces;
	using FMCore;
	using FMPointCommon;
	using Opc.Ua;
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Drawing;
	using System.Linq;
	using System.ServiceModel;
	using IsolationLevel = System.Transactions.IsolationLevel;

	[ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class PointServiceManager : IPointServiceManager
	{
		private static readonly IPointServiceInfoGetter PointServiceInfoGetter = new PointServiceInfoGetter();

		private static readonly IPointDataGetter PointDataGetter = new PointDataGetter();

		private PointChecksumCollection LoadPointChecksums(DataTable table)
		{
			var ret = new PointChecksumCollection();
			for (int index = 0; index < table.Rows.Count; index++)
			{
				var row = table.Rows[index];
				var pointGuid = DataObject.getValue(row["PointGuid"], Guid.Empty);
				var pointRowVersion = BaseDataObject.RowVersionToInt64(row["PointRowVersion"] as byte[]);
				var currentPoint = new PointChecksum
				{
					PointGuid = pointGuid,
					MaxRowVersion = pointRowVersion
				};
				ret.Add(currentPoint);
			}
			return ret;
		}

		private PointChecksumCollection GetPointChecksumsForHostname(SecurityClass security, string hostname)
		{
				security.ThrowIfNull("security");
				hostname.ThrowIfNullOrEmpty("hostname");

				var consolidatedDA = new ConsolidatedDAClass();
				DataSet set;

				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandType = CommandType.Text;
						cmd.CommandText = "select p.PointGuid, p._RowVersion AS PointRowVersion FROM tblPoint p"
								+ " INNER JOIN map.tblPointToPointService pps ON pps.PointGuid = p.PointGuid"
								+ " INNER JOIN tblPointService ps ON ps.PointServiceGuid = pps.PointServiceGuid"
								+ " WHERE p.Enabled = CAST(1 as BIT) AND ps.Hostname = @Hostname";
					cmd.Parameters.Add("@Hostname", SqlDbType.NVarChar, 256);
					cmd.Parameters["@Hostname"].Value = hostname;
					set = consolidatedDA.GetDataSet(cmd, security);
				}

				if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
				{
					return new PointChecksumCollection();
				}

				DataTable table = set.Tables[0];

				// Do from here down
				return this.LoadPointChecksums(table);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public PointCollection GetPointsForHostnameEx(SecurityClass security, string hostname, List<Guid> pointGuidList)
		{
				security.ThrowIfNull("security");
				hostname.ThrowIfNullOrEmpty("hostname");
				pointGuidList.ThrowIfNull("pointGuidList");

				var pointServices = new PointServices();
				var pointService = pointServices.Get(security, hostname);

				if (pointService != null)
				{
					pointService.LastPingTime = DateTimeOffset.Now;
					pointServices.Modify(security, pointService);
				}

				var p = new Points();

				return p.Get(security, pointGuidList);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void UpdateTestFailedAndOneShot(
			SecurityClass security,
			List<PointTagAlarmStatus> alarmStatusList,
			List<Alarm> alarmList)
		{
			
			if (alarmStatusList != null && alarmStatusList.Any())
			{
				var ptas = new PointTagAlarmStatuses();
				ptas.UpdateTestFailed(security, alarmStatusList);
			}
			if (alarmList != null && alarmList.Any())
			{
				var a = new Alarms();
				a.UpdateShelvedOneShot(security, alarmList);
			}
			
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Ping(
			SecurityClass security,
			string hostname,
			PointServiceHealthStatus healthStatus,
			int pingIntervalInSeconds,
			int percentCpuUtilization,
			int percentCpuUtilizationThrottleLevel,
			int percentMemoryUtilization,
			int percentMemoryUtilizationThrottleLevel,
			int maxPointsToProcess)
		{
			var pointServices = new PointServices();
			var pointService = pointServices.Get(security, hostname);
			if (pointService == null)
			{
				pointService = new PointService()
				{
					Hostname = hostname,
					PingIntervalInSeconds = pingIntervalInSeconds,
					LastPingTime = DateTimeOffset.Now,
					HealthStatusIndex = (int)healthStatus,
					PercentCpuUtilization = percentCpuUtilization,
					PercentCpuUtilizationThrottleLevel =
											percentCpuUtilizationThrottleLevel,
					PercentMemoryUtilization = percentMemoryUtilization,
					PercentMemoryUtilizationThrottleLevel =
											percentMemoryUtilizationThrottleLevel,
					MaxNumberOfPoints = maxPointsToProcess
				};

				pointServices.Add(security, pointService);
			}
			else
			{
				pointService.LastPingTime = DateTimeOffset.Now;
				pointService.PingIntervalInSeconds = pingIntervalInSeconds;
				pointService.PercentCpuUtilization = percentCpuUtilization;
				pointService.PercentCpuUtilizationThrottleLevel = percentCpuUtilizationThrottleLevel;
				pointService.PercentMemoryUtilization = percentMemoryUtilization;
				pointService.PercentMemoryUtilizationThrottleLevel = percentMemoryUtilizationThrottleLevel;
				pointService.HealthStatusIndex = (int)healthStatus;
				pointService.MaxNumberOfPoints = maxPointsToProcess;
				pointServices.Modify(security, pointService);
			}
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public SchedulePointsResponse SchedulePoints(
				SecurityClass security,
				string hostname)
		{
				SchedulePointsResponse ret = new SchedulePointsResponse
				{
											Status = SchedulePointsStatus.Good,
											PointCheckSums = new PointChecksumCollection()
										};

				try
				{
					if (security == null)
					{
						ret.Status = SchedulePointsStatus.InvalidSecurity;
					}
					else
					{
						this.DoPointScheduling(security, hostname);

						ret.PointCheckSums = this.GetPointChecksumsForHostname(security, hostname);

						if (ret.PointCheckSums == null || ret.PointCheckSums.Count <= 0)
						{
								ret.Status = SchedulePointsStatus.NoPointsAssigned;
						}
					}
				}
				catch (Exception)
				{
					ret.Status = SchedulePointsStatus.Bad;
				}
				return ret;
		}

		protected void DoPointScheduling(SecurityClass security, string hostname)
		{
					var consolidatedDA = new ConsolidatedDAClass();
					using (SqlCommand cmd = new SqlCommand())
					{
						cmd.CommandTimeout = 200;
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.CommandText = "dbo.usp_SchedulePointsToPointServices";
						cmd.Parameters.Add("@Hostname", SqlDbType.NVarChar, 256);
						cmd.Parameters["@Hostname"].Value = hostname;
						consolidatedDA.ExecuteQuery(security, cmd);
					}
		}

		protected long MaxPointAndPointTagTrackRowVersion(SecurityClass security, string hostname)
		{
				var consolidatedDA = new ConsolidatedDAClass();
				DataSet set;

				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "dbo.usp_MaxPointAndPointTagTrackRowVersionForPointService";
					cmd.Parameters.Add("@Hostname", SqlDbType.NVarChar, 256);
					cmd.Parameters["@Hostname"].Value = hostname;
					set = consolidatedDA.GetDataSet(cmd, security);
				}

				if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
				{
					return 0;
				}

				DataTable table = set.Tables[0];
				DataRow row = table.Rows[0];

				if (row.IsNull("RowVersion"))
				{
					return 0;
				}

				return BaseDataObject.RowVersionToInt64(row["RowVersion"] as byte[]);

		}

		protected int GetNumberOfPoints(SecurityClass security, string hostname)
		{
				var consolidatedDA = new ConsolidatedDAClass();
				DataSet set;

				using (SqlCommand cmd = new SqlCommand())
				{

					cmd.CommandType = CommandType.Text;
					cmd.CommandText =
						"Select COUNT(m.PointToPointServiceGuid) AS PointCount from map.tblPointToPointService m INNER JOIN tblPointService s ON m.PointServiceGuid = s.PointServiceGuid WHERE s.Hostname = @Hostname";
					cmd.Parameters.Add("@Hostname", SqlDbType.NVarChar, 256);
					cmd.Parameters["@Hostname"].Value = hostname;
					set = consolidatedDA.GetDataSet(cmd, security);
				}

				if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
				{
					return -1;
				}

				DataTable table = set.Tables[0];
				DataRow row = table.Rows[0];

				if (row.IsNull("PointCount"))
				{
					return -1;
				}

				return (int)row["PointCount"];

		}

		protected int GetNumberOfTags(SecurityClass security, string hostname)
		{
				var consolidatedDA = new ConsolidatedDAClass();
				DataSet set;

				using (SqlCommand cmd = new SqlCommand())
				{

					cmd.CommandType = CommandType.Text;
					cmd.CommandText =
						"Select COUNT(t.PointTagGuid) AS TagCount from tblPointTag t INNER JOIN map.tblPointToPointService m ON t.PointGuid = m.PointGuid INNER JOIN tblPointService s ON m.PointServiceGuid = s.PointServiceGuid WHERE s.Hostname = @Hostname";
					cmd.Parameters.Add("@Hostname", SqlDbType.NVarChar, 256);
					cmd.Parameters["@Hostname"].Value = hostname;
					set = consolidatedDA.GetDataSet(cmd, security);
				}

				if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
				{
					return -1;
				}

				DataTable table = set.Tables[0];
				DataRow row = table.Rows[0];

				if (row.IsNull("TagCount"))
				{
					return -1;
				}

				return (int)row["TagCount"];

		}

		public List<PointTag> GetPointTagData(SecurityClass security, List<Guid> pointTagGuids)
		{
			security.ThrowIfNull("security");
			pointTagGuids.ThrowIfNull("pointTagGuids");

			return PointDataGetter.Get(security, pointTagGuids);
		}

		/// <summary>
		/// This method will get the point tag data without filtering on Point Access.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="pointTagGuids"></param>
		/// <returns></returns>
		public List<PointTag> GetPointTagDataWithoutPointAccess(SecurityClass security, List<Guid> pointTagGuids)
		{
				security.ThrowIfNull("security");
				pointTagGuids.ThrowIfNull("pointTagGuids");

				return PointDataGetter.GetWithoutPointAccess(security, pointTagGuids);
		}

		public List<PointValue> GetPointValueData(SecurityClass security, List<PointValueIdentifier> pointValueIdentifiers, bool applyPointAccess = true)
		{
			security.ThrowIfNull("security");
			pointValueIdentifiers.ThrowIfNull("pointValueIdentifiers");

			return PointDataGetter.Get(security, pointValueIdentifiers, applyPointAccess);
		}

		public List<PointValue> GetPointValueDataChanges(SecurityClass security, List<PointValueIdentifier> pointValueIdentifiers, bool applyPointAccess = true)
		{
			security.ThrowIfNull("security");
			pointValueIdentifiers.ThrowIfNull("pointValueIdentifiers");

			return PointDataGetter.GetChanges(security, pointValueIdentifiers, applyPointAccess);
		}




		public List<Statistic> GetStatistics(SecurityClass security, PointService pointService)
		{
				security.ThrowIfNull("security");
				pointService.ThrowIfNull("pointService");

				var info = PointServiceInfoGetter.Info;

				string protocol = info.PointServiceBindingEndPointAddress.Substring(
					0,
					info.PointServiceBindingEndPointAddress.IndexOf("/", StringComparison.Ordinal));

				string endPoint = protocol + "//" + pointService.Hostname + "/FMPointService";


				return FMChannelHelper.MakeCall<IPointService, List<Statistic>>(
					info.PointServiceBindingType,
					info.PointServiceBindingConfiguration,
					endPoint,
					x => x.GetStatistics(security));
		}

		public void ResetStatistics(SecurityClass security, PointService pointService)
		{
				security.ThrowIfNull("security");

				var info = PointServiceInfoGetter.Info;

				string protocol = info.PointServiceBindingEndPointAddress.Substring(
					0,
					info.PointServiceBindingEndPointAddress.IndexOf("/", StringComparison.Ordinal));

				string endPoint = protocol + "//" + pointService.Hostname + "/FMPointService";

				FMChannelHelper.MakeCall<IPointService>(
					info.PointServiceBindingType,
					info.PointServiceBindingConfiguration,
					endPoint,
					x => x.ResetStatistics(security));
		}



		public void SetPointTagData(SecurityClass security, List<PointTag> pointTagsList, bool enterpriseVisibility)
		{
			security.ThrowIfNull("security");
			pointTagsList.ThrowIfNull("pointTagsList");


			var info = PointServiceInfoGetter.Info;

			var pointTagDictionary = new Dictionary<Guid, PointTag>(pointTagsList.Count);
			var aandEDataElements = new List<AandEDataElement>(pointTagsList.Count);

			foreach (var pointTag in pointTagsList)
			{
				pointTagDictionary.Add(pointTag.PointTagGuid, pointTag);

				// Do not archive Alarm and Event for enterprise visibility or (not manual && not override)
				if (enterpriseVisibility
				|| (pointTag.InputOutputType != PointTemplateTag.PointTagInputOutputType.Manual
				&& pointTag.OpcStatusCodeBits != StatusCodes.GoodLocalOverride))
				{
					continue;
				}

				AandEDataElement AandEElem = new AandEDataElement(security, new PointValue(pointTag));
				if (AandEElem.Action != "Unknown")
				{
					aandEDataElements.Add(AandEElem);
				}
			}

			string protocol = info.PointServiceBindingEndPointAddress.Substring(
				0,
				info.PointServiceBindingEndPointAddress.IndexOf("/", StringComparison.Ordinal));

			var pointsToPointServices = new PointsToPointServices();
			var hostNameToPointTagGuidListDictionary = pointsToPointServices.EnumerateHostNameByPointTagGuid(
				security,
				pointTagDictionary.Keys.ToList());

			foreach (var hostName in hostNameToPointTagGuidListDictionary.Keys)
			{
				List<Guid> pointTagGuids;
				if (hostNameToPointTagGuidListDictionary.TryGetValue(hostName, out pointTagGuids)
					&& pointTagGuids.Count > 0)
				{
					var pointTagPerServerList = new List<PointTag>(pointTagGuids.Count);
					foreach (var guid in pointTagGuids)
					{
						pointTagPerServerList.Add(pointTagDictionary[guid]);
					}

					if (!string.IsNullOrEmpty(hostName))
					{
						string endPoint = protocol + "//" + hostName + "/FMPointService";
						try
						{
							FMChannelHelper.MakeCall<IPointService>(
								info.PointServiceBindingType,
								info.PointServiceBindingConfiguration,
								endPoint,
								x => x.SetPointTagData(security, pointTagPerServerList));
						}
						catch (Exception)
						{
							foreach (var pointTag in pointTagPerServerList)
							{
								if (!pointTag.Input)
								{
									throw new Exception("SetPointTagData Error Setting Output " + pointTag.PointID + "." + pointTag.ID);
								}
							}
						}
					}
				}
			}

			if (pointTagsList.Any())
			{
				var pointTags = new PointTags();
				pointTags.ModifyTagValues(security, pointTagsList, enterpriseVisibility);
			}

			if (aandEDataElements.Any())
			{
				var aAndETagArchive = new AandETagArchive();
				aAndETagArchive.AddArchiveData(security, aandEDataElements);
			}
		}



		public void SetAcknowledge(SecurityClass security, DateTimeOffset timestamp, List<PointTag> pointTagList, string comment = "")
		{
			security.ThrowIfNull("security");
			pointTagList.ThrowIfNull("pointTags");

			var info = PointServiceInfoGetter.Info;

			var pointTagDictionary = new Dictionary<Guid, PointTag>(pointTagList.Count);
			foreach (var pointTag in pointTagList)
			{
				pointTagDictionary.Add(pointTag.PointTagGuid, pointTag);
			}

			string protocol = info.PointServiceBindingEndPointAddress.Substring(
				0,
				info.PointServiceBindingEndPointAddress.IndexOf("/", StringComparison.Ordinal));

			var pointsToPointServices = new PointsToPointServices();
			var hostNameToPointTagGuidListDictionary = pointsToPointServices.EnumerateHostNameByPointTagGuid(
				security,
				pointTagDictionary.Keys.ToList());

			foreach (var hostName in hostNameToPointTagGuidListDictionary.Keys)
			{
				List<Guid> pointTagGuids;
				List<PointTagAlarmStatus> acknowledgePointTagAlarmStatusListForPointService = new List<PointTagAlarmStatus>();
				List<PointTagAlarmStatus> silencePointTagAlarmStatusListForPointService = new List<PointTagAlarmStatus>();

				if (hostNameToPointTagGuidListDictionary.TryGetValue(hostName, out pointTagGuids)
					&& pointTagGuids.Count > 0)
				{
					var pointTagPerServerList = new List<PointTag>(pointTagGuids.Count);
					foreach (var guid in pointTagGuids)
					{
						pointTagPerServerList.Add(pointTagDictionary[guid]);
					}

					if (!string.IsNullOrEmpty(hostName))
					{
						string endPoint = protocol + "//" + hostName + "/FMPointService";
						try
						{
							FMChannelHelper.MakeCall<IPointService>(
								info.PointServiceBindingType,
								info.PointServiceBindingConfiguration,
								endPoint,
								x => x.SetAcknowledgeAndSilence(security, pointTagPerServerList, timestamp, comment));
						}
						catch (Exception)
						{
						}
					}
				}
			}
		}


		public void Shelve(SecurityClass security, int days, int hours, int minutes, bool oneShot, List<Guid> alarmGuidList)
		{
			var startTime = DateTimeOffset.UtcNow;
			var endTime = startTime.AddDays(days);
			endTime = endTime.AddHours(hours);
			endTime = endTime.AddMinutes(minutes);

			if (alarmGuidList != null && alarmGuidList.Any())
			{
				var alarmBL = new Alarms();
				var alarms = alarmBL.EnumerateByAlarmGuids(security, alarmGuidList);
				var tagGuidList = new List<Guid>();
				foreach (var alarm in alarms.Values)
				{
					if (!tagGuidList.Contains(alarm.InputTagGuid))
					{
						tagGuidList.Add(alarm.InputTagGuid);
					}
				}
				if (tagGuidList.Any())
				{
					var pointTagBL = new PointTags();
					var tags = pointTagBL.EnumerateByTagList(security, tagGuidList);
					foreach (var tag in tags.Values)
					{
						tag.Alarms = new Dictionary<Guid, Alarm>();
					}
					foreach (var alarm in alarms.Values)
					{
						PointTag tag;
						if (tags.TryGetValue(alarm.InputTagGuid, out tag))
						{
							alarm.ShelvedStartTimeStamp = startTime;
							if (!oneShot)
							{
								alarm.ShelvedEndTimeStamp = endTime;
							}
							else
							{
								alarm.ShelvedEndTimeStamp = null;
							}
							alarm.ShelvedBy = security.UserID;
							alarm.ShelvedOneShot = oneShot;
							tag.Alarms.Add(alarm.AlarmGuid, alarm);
						}
					}
					this.SetShelve(security, tags.Values.ToList());
				}
			}
		}

		public void SetShelve(SecurityClass security, List<PointTag> pointTagList)
		{
			security.ThrowIfNull("security");
			pointTagList.ThrowIfNull("pointTags");
			this.LocalShelve(security, pointTagList);

			var info = PointServiceInfoGetter.Info;

			var pointTagDictionary = new Dictionary<Guid, PointTag>(pointTagList.Count);
			foreach (var pointTag in pointTagList)
			{
				pointTagDictionary.Add(pointTag.PointTagGuid, pointTag);
			}

			string protocol = info.PointServiceBindingEndPointAddress.Substring(
				0,
				info.PointServiceBindingEndPointAddress.IndexOf("/", StringComparison.Ordinal));

			var pointsToPointServices = new PointsToPointServices();
			var hostNameToPointTagGuidListDictionary = pointsToPointServices.EnumerateHostNameByPointTagGuid(
				security,
				pointTagDictionary.Keys.ToList());

			foreach (var hostName in hostNameToPointTagGuidListDictionary.Keys)
			{
				List<Guid> pointTagGuids;
				if (hostNameToPointTagGuidListDictionary.TryGetValue(hostName, out pointTagGuids)
					&& pointTagGuids.Count > 0)
				{
					var pointTagPerServerList = new List<PointTag>(pointTagGuids.Count);
					foreach (var guid in pointTagGuids)
					{
						pointTagPerServerList.Add(pointTagDictionary[guid]);
					}

					if (!string.IsNullOrEmpty(hostName))
					{
						string endPoint = protocol + "//" + hostName + "/FMPointService";
						try
						{
							FMChannelHelper.MakeCall<IPointService>(
								info.PointServiceBindingType,
								info.PointServiceBindingConfiguration,
								endPoint,
								x => x.SetShelve(security, pointTagPerServerList));
						}
						catch (Exception)
						{
							var pointTags = new PointTags();
							pointTags.ModifyTagValues(security, pointTagPerServerList, false);
						}
					}
				}
			}
		}

		protected void LocalShelve(SecurityClass security, List<PointTag> pointTagList)
		{
			List<Alarm> shelveList = new List<Alarm>();
			foreach (var pointTag in pointTagList)
			{
				if (pointTag.Alarms != null && pointTag.Alarms.Any())
				{
					shelveList.AddRange(pointTag.Alarms.Values.ToList());
				}
			}
			if (shelveList.Any())
			{
				var alarms = new Alarms();
				alarms.UpdateShelved(security, shelveList);
			}
		}


		public void SetPointValueData(SecurityClass security, List<PointValue> pointValues, bool enterpriseVisibility)
		{
			security.ThrowIfNull("security");
			pointValues.ThrowIfNull("pointValueList");

			var pointTagValueList = new List<PointValue>(pointValues.Count);
			var pointSettingValueList = new List<PointValue>(pointValues.Count);
			var pointValueList = new List<PointValue>(pointValues.Count);
			var aandEDataElements = new List<AandEDataElement>(pointValues.Count);

			var pointValueDictionary = new Dictionary<PointValueIdentifier, PointValue>(pointValues.Count);
			foreach (var pointValue in pointValues)
			{
				if (pointValueDictionary.ContainsKey(pointValue.PointValueIdentifier))
				{
					pointValueDictionary[pointValue.PointValueIdentifier] = pointValue;
				}
				else
				{
					pointValueDictionary.Add(pointValue.PointValueIdentifier, pointValue);
				}

				// Do not archive Alarm and Event for enterprise visibility or (not manual && not override)
				if (enterpriseVisibility
				|| (pointValue.InputOutputType != PointTemplateTag.PointTagInputOutputType.Manual
				&& pointValue.OpcStatusCodeBits != StatusCodes.GoodLocalOverride))
				{
					continue;
				}

				AandEDataElement AandEElem = new AandEDataElement(security, pointValue);
				if (AandEElem.Action != "Unknown")
				{
					aandEDataElements.Add(AandEElem);
				}
			}

			var info = PointServiceInfoGetter.Info;

			string protocol = info.PointServiceBindingEndPointAddress.Substring(0,info.PointServiceBindingEndPointAddress.IndexOf("/", StringComparison.Ordinal));

			var pointsToPointServices = new PointsToPointServices();

			var hostNameToPointValueIdentifierListDictionary = pointsToPointServices.EnumerateHostNameByPointValueIdentifier(
				security,
				pointValueDictionary.Keys.ToList());

			foreach (var hostName in hostNameToPointValueIdentifierListDictionary.Keys)
			{
				List<PointValueIdentifier> pointValueIdentifiers;
				if (hostNameToPointValueIdentifierListDictionary.TryGetValue(hostName, out pointValueIdentifiers)
					&& pointValueIdentifiers.Count > 0)
				{
					var pointValuePerServerList = new List<PointValue>(pointValueIdentifiers.Count);
					foreach (var pointValueIdentifier in pointValueIdentifiers)
					{
						var pointValue = pointValueDictionary[pointValueIdentifier];
						if (pointValue.PointValueIdentifier.PointValueType == PointValueType.Tag)
						{
							// Only Persist Values that are not hosted or are Inputs
							if (string.IsNullOrEmpty(hostName)
							|| pointValue.Input)
							{
								pointTagValueList.Add(pointValue);
							}

							pointValuePerServerList.Add(pointValue);
						}
						else if (pointValue.PointValueIdentifier.PointValueType == PointValueType.Setting)
						{
							// Do not Persist Setting Values that are System, presently these will be limited to the Movement Data Point Property
							if (pointValue.InputOutputType != PointTemplateTag.PointTagInputOutputType.System)
							{
								pointSettingValueList.Add(pointValue);
							}

							pointValuePerServerList.Add(pointValue);
						}

						// PointValueType.Point are not added to the pointValuePerServerList as they are written to the point and reloaded.
						else if (pointValue.PointValueIdentifier.PointValueType == PointValueType.Point)
						{
							pointValueList.Add(pointValue);
						}
					}

					if (!string.IsNullOrEmpty(hostName))
					{
						string endPoint = protocol + "//" + hostName + "/FMPointService";
						try
						{
							FMChannelHelper.MakeCall<IPointService>(
								info.PointServiceBindingType,
								info.PointServiceBindingConfiguration,
								endPoint,
								x => x.SetPointValueData(security, pointValuePerServerList));
						}
						catch (Exception)
						{
							foreach(var pointValue in pointValuePerServerList)
							{
								if(!pointValue.Input)
								{
									throw new Exception("SetPointValueData Error Setting Output " + pointValue.PointID + "." + pointValue.ID);
								}
							}
						}
					}
				}
			}

			if (pointTagValueList.Count > 0)
			{
				var pointTags = new PointTags();
				pointTags.ModifyPointValues(security, pointTagValueList, enterpriseVisibility);
			}

			if (pointSettingValueList.Count > 0)
			{
				var pointProperties = new PointProperties();
				pointProperties.ModifyPointValues(security, pointSettingValueList);
			}

			if (pointValueList.Count > 0)
			{
				var points = new Points();
				points.ModifyPointValues(security, pointValueList);
			}


			if (aandEDataElements.Any())
			{
				var aAndETagArchive = new AandETagArchive();
				aAndETagArchive.AddArchiveData(security, aandEDataElements);
			}
		}

		public PointCalculatorData RunPointCalculator(SecurityClass security, Guid pointGuid, PointCalculatorData pointCalculatorData)
		{
			security.ThrowIfNull("security");
			pointGuid.ThrowIfNull("pointGuid");
            pointCalculatorData.ThrowIfNull("pointCalculatorData");
			// bds
			var info = PointServiceInfoGetter.Info;

			// the following code will get the protocol and location of the machine and fmpointservice
			// where the point is running. This may or may not be the current machine based on how it is being
			// load balanced.
			string protocol = info.PointServiceBindingEndPointAddress.Substring(
				0,
				info.PointServiceBindingEndPointAddress.IndexOf("/", StringComparison.Ordinal));

			var pointsToPointServices = new PointsToPointServices();

			List<Guid> pointGuidList = new List<Guid>();
			pointGuidList.Add(pointGuid);
			// this call will only return one value but there is no need to create a new call when this one will work
			// find the passed in point host name
			var hostNameToPointValueIdentifierListDictionary = pointsToPointServices.EnumerateHostNameByPointGuid(security, pointGuidList);

			foreach (var hostName in hostNameToPointValueIdentifierListDictionary.Keys)
			{
				if (!string.IsNullOrEmpty(hostName))
				{
					string endPoint = protocol + "//" + hostName + "/FMPointService";
					try
					{
						// make wcf call to the host where the point is located to run the calculator
						FMChannelHelper.MakeCall<IPointService>(
							info.PointServiceBindingType,
							info.PointServiceBindingConfiguration,
							endPoint,
							x => pointCalculatorData = x.RunPointCalculator(security, pointGuid, pointCalculatorData));
					}
					catch (Exception e)
					{
						throw new Exception("RunPointCalculator " + e.Message);
					}
				}
			}
			return pointCalculatorData;
		}

        public List<PointTag> RunPointCalculatorX(SecurityClass security, Guid pointGuid, List<PointTag> pointTags)
        {
            security.ThrowIfNull("security");
            pointGuid.ThrowIfNull("pointGuid");
            pointTags.ThrowIfNull("pointTags");
            // bds
            var info = PointServiceInfoGetter.Info;

            // the following code will get the protocol and location of the machine and fmpointservice
            // where the point is running. This may or may not be the current machine based on how it is being
            // load balanced.
            string protocol = info.PointServiceBindingEndPointAddress.Substring(
                0,
                info.PointServiceBindingEndPointAddress.IndexOf("/", StringComparison.Ordinal));

            var pointsToPointServices = new PointsToPointServices();

            List<Guid> pointGuidList = new List<Guid>();
            pointGuidList.Add(pointGuid);
            // this call will only return one value but there is no need to create a new call when this one will work
            // find the passed in point host name
            var hostNameToPointValueIdentifierListDictionary = pointsToPointServices.EnumerateHostNameByPointGuid(security, pointGuidList);

            foreach (var hostName in hostNameToPointValueIdentifierListDictionary.Keys)
            {
                if (!string.IsNullOrEmpty(hostName))
                {
                    string endPoint = protocol + "//" + hostName + "/FMPointService";
                    try
                    {
                        // make wcf call to the host where the point is located to run the calculator
                        FMChannelHelper.MakeCall<IPointService>(
                            info.PointServiceBindingType,
                            info.PointServiceBindingConfiguration,
                            endPoint,
                            x => pointTags = x.RunPointCalculatorX(security, pointGuid, pointTags));
                    }
                    catch (Exception e)
                    {
                        throw new Exception("RunPointCalculator " + e.Message);
                    }
                }
            }
            return pointTags;
        }

        public void CallAsyncMethods(SecurityClass security, List<AsyncMethodCallClass> methodInvocationList)
		{
			security.ThrowIfNull("security");
			methodInvocationList.ThrowIfNull("methodInvocationList");

			var info = PointServiceInfoGetter.Info;

			var pointMethodInvocationDictionary = new Dictionary<Guid, List<AsyncMethodCallClass>>();
			foreach (var methodInvocation in methodInvocationList)
			{
				List<AsyncMethodCallClass> asyncMethodCallList = null;
				if(!pointMethodInvocationDictionary.TryGetValue(methodInvocation.PointGuid, out asyncMethodCallList))
				{
					asyncMethodCallList = new List<AsyncMethodCallClass>();
					pointMethodInvocationDictionary.Add(methodInvocation.PointGuid, asyncMethodCallList);
				}
				asyncMethodCallList.Add(methodInvocation);
			}

			string protocol = info.PointServiceBindingEndPointAddress.Substring(
				0,
				info.PointServiceBindingEndPointAddress.IndexOf("/", StringComparison.Ordinal));

			var pointsToPointServices = new PointsToPointServices();
			var hostNameToPointGuidListDictionary = pointsToPointServices.EnumerateHostNameByPointGuid(
				security,
				pointMethodInvocationDictionary.Keys.ToList());

			foreach (var hostName in hostNameToPointGuidListDictionary.Keys)
			{
				List<Guid> pointGuids;
				if (hostNameToPointGuidListDictionary.TryGetValue(hostName, out pointGuids) && pointGuids.Count > 0)
				{
					var methodInvocationPerServerList = new List<AsyncMethodCallClass>();
					foreach (var pointGuid in pointGuids)
					{
						List<AsyncMethodCallClass> asyncMethodCallList = null;
						if (pointMethodInvocationDictionary.TryGetValue(pointGuid, out asyncMethodCallList))
						{
							foreach (var asyncMethodCall in asyncMethodCallList)
							{
								methodInvocationPerServerList.Add(asyncMethodCall);
							}
						}
					}

					string endPoint = protocol + "//" + hostName + "/FMPointService";

					FMChannelHelper.MakeCall<IPointService>(
							info.PointServiceBindingType,
							info.PointServiceBindingConfiguration,
							endPoint,
							x => x.ExecuteAsyncMethods(security, methodInvocationPerServerList));
				}
			}
		}

		public Guid? SavePointCalculatorTagValues(SecurityClass security, PointCalculatorResult result)
		{
			try
			{
				PointCalculatorRunDBI calcRunDBI = new PointCalculatorRunDBI();
				return calcRunDBI.Save(security, result);
			}
			catch (ConsolidatedDAException ex)
			{
				string sRet = "Saving Point Calculator Run results failed! Please see event log for more details!";
				var eventLogging = new EventLogging();
				eventLogging.LogEvent(sRet + ex.Message, EventLogEntryType.Error);
				throw new Exception(sRet);
			}
		}
		public void CleanupPointCalculatorRunsFromDB(SecurityClass security, int intervalMinutesToKeep)
		{
			try
			{
				PointCalculatorRunDBI calcRunDBI = new PointCalculatorRunDBI();
				calcRunDBI.CleanupPointCalculatorRunsFromDB(security, intervalMinutesToKeep);
			}
			catch (ConsolidatedDAException ex)
			{
				string sRet = "Cleanup Point Calculator Runs From DB failed! Please see event log for more details!";
				var eventLogging = new EventLogging();
				eventLogging.LogEvent(sRet + ex.Message, EventLogEntryType.Error);
				throw new Exception(sRet);
			}
		}
	}
}
