
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FMBusinessServices.DataAccessLayer;

	using FMPointCommon;



	public class AlarmStatus : IAlarmStatus
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion

		public AlarmStatus()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}


		public List<AlarmStatusClass2> GetActiveAlarms(SecurityClass security, bool unacknowledged, bool unsilenced, bool notify = false)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "usp_EnumerateActiveAlarmsBySite";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
				cmd.Parameters.AddWithValue("@Unacknowledged", unacknowledged);
				cmd.Parameters.AddWithValue("@Unsilenced", unsilenced);
				cmd.Parameters.AddWithValue("@Notify", notify);

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var activeAlarmAlarmGuidDictionary = new Dictionary<Guid, AlarmStatusClass2>();
			var activeAlarmAlarmTestGuidDictionary = new Dictionary<Guid, AlarmStatusClass2>();

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var activeAlarm = new AlarmStatusClass2(row);
				activeAlarmAlarmGuidDictionary.Add(activeAlarm.AlarmGuid, activeAlarm);
				activeAlarmAlarmTestGuidDictionary.Add(activeAlarm.AlarmTestGuid, activeAlarm);
			}

			if (notify == false)
			{

				using (var cmd = new SqlCommand())
				{
					AlarmTest.EnumerateRestrictedAccessByAlarmTestGuidList(cmd, security, activeAlarmAlarmTestGuidDictionary.Keys.ToList());
					set = this.consolidatedDA.GetDataSet(cmd, security);
				}


				if (set != null || set.Tables.Count == 1 || set.Tables[0].Rows.Count != 0)
				{
					table = set.Tables[0];

					foreach (DataRow row in table.Rows)
					{
						var alarmTestGuid = (Guid)row["AlarmTestGuid"];
						var view = (bool)row["View"];
						var acknowledge = (bool)row["Acknowledge"];

						var activeAlarm = activeAlarmAlarmTestGuidDictionary[alarmTestGuid];
						if (!view && !acknowledge)
						{
							activeAlarmAlarmGuidDictionary.Remove(activeAlarm.AlarmGuid);
						}
						else
						{
							activeAlarm.Acknowledge = acknowledge;
						}
					}
				}
			}

			return activeAlarmAlarmGuidDictionary.Values.ToList();
		}

		// Works for single alarm acknowledgement or acknowledge all alarms on the page
		public void AcknowledgeAlarms(SecurityClass security, string comment, List<Guid> alarmGuidList)
		{
			var unackAlarms = new Dictionary<Guid, AlarmStatusClass2>();

			List<AlarmStatusClass2> alarmList = GetActiveAlarms(security, true, true);
			HashSet<Guid> alarmGuidHashSet = new HashSet<Guid>();

			foreach(var alarmGuid in alarmGuidList)
			{
				alarmGuidHashSet.Add(alarmGuid);
			}

			foreach (var alarm in alarmList)
			{
				if (alarmGuidHashSet.Contains(alarm.AlarmGuid)
				&& !alarm.Acknowledged)
				{
					unackAlarms.Add(alarm.AlarmGuid, alarm);
				}
			}

			if (unackAlarms.Values.Count > 0)
			{

				HashSet<Guid> inputTagGuidHashSet = new HashSet<Guid>();

				foreach (var alarmToAck in unackAlarms.Values)
				{
					if (!inputTagGuidHashSet.Contains(alarmToAck.TagGuid))
					{
						inputTagGuidHashSet.Add(alarmToAck.TagGuid);
					}
				}

				DateTimeOffset now = DateTimeOffset.UtcNow;

				var pointTags = new PointTags();
				var pointTagDictionary = pointTags.EnumerateByTagList(security, inputTagGuidHashSet.ToList());
				var alarms = new Dictionary<Guid, Alarm>();

				foreach (var alarmToAck in unackAlarms.Values)
				{
					var tag = pointTagDictionary[alarmToAck.TagGuid];
					var alarm = tag.Alarms[alarmToAck.AlarmGuid];
					if(!alarms.ContainsKey(alarmToAck.AlarmGuid))
					{
						alarms.Add(alarmToAck.AlarmGuid, alarm.Clone() as Alarm);
						alarms[alarmToAck.AlarmGuid].AlarmStatus = new Dictionary<Guid, PointTagAlarmStatus> ();
						alarms[alarmToAck.AlarmGuid].AlarmTests = null;
					}
					var alarmStatus = alarm.AlarmStatus[alarmToAck.PointTagAlarmStatusGuid];

					alarmStatus.Acknowledged = true;
					alarmStatus.AcknowledgedTimestamp = now;
					alarmStatus.AcknowledgedBy = security.UserID;
					if (alarmStatus.Silenced == false)
					{
						alarmStatus.Silenced = true;
						alarmStatus.SilencedTimestamp = now;
						alarmStatus.SilencedBy = security.UserID;
					}

					if (!string.IsNullOrWhiteSpace(comment))
					{
						alarmStatus.AcknowledgedComment = comment;
					}

					alarms[alarmToAck.AlarmGuid].AlarmStatus.Add(alarmToAck.PointTagAlarmStatusGuid, alarmStatus);
				}

				// Send to Point Service
				if (pointTagDictionary.Values.Any())
				{
					var psm = new PointServiceManager();
					psm.SetAcknowledge(security, now, pointTagDictionary.Values.ToList(), comment);
				}
			}
		}

      public void SilenceAlarms(SecurityClass security)
      {
         var alarmsToSilence = new List<AlarmStatusClass2>();
         List<AlarmStatusClass2> alarmList = this.GetActiveAlarms(security, false, true);

         foreach (var alarm in alarmList)
         {
            if (!alarm.Silenced)
            {
               alarmsToSilence.Add(alarm);
            }
         }

         DateTimeOffset now = DateTimeOffset.UtcNow;
         Dictionary<Guid, PointTag> silenceList = new Dictionary<Guid, PointTag>();
         List<PointTagAlarmStatus> pointTagAlarmStatusList = new List<PointTagAlarmStatus>();

         HashSet<Guid> inputTagGuidHashSet = new HashSet<Guid>();

         foreach (var alarmToSilence in alarmsToSilence)
         {
            if (!inputTagGuidHashSet.Contains(alarmToSilence.TagGuid))
            {
               inputTagGuidHashSet.Add(alarmToSilence.TagGuid);
            }
         }

         var pointTags = new PointTags();
         var pointTagDictionary = pointTags.EnumerateByTagList(security, inputTagGuidHashSet.ToList());
         var aandEDataElements = new List<AandEDataElement>();
         var configSettings = new ConfigurationSettingsClass();
         var configDOA = configSettings.GetByKey(security, "AlarmSilenceAuditLoggingEnabled");
         bool alarmSilenceLoggingEnabled = !configDOA.SettingValue.Equals("false", StringComparison.OrdinalIgnoreCase) && !configDOA.SettingValue.Equals("0", StringComparison.OrdinalIgnoreCase);

         foreach (var alarmToSilence in alarmsToSilence)
         {
            var tag = pointTagDictionary[alarmToSilence.TagGuid];
            var alarm = tag.Alarms[alarmToSilence.AlarmGuid];
            var alarmStatus = alarm.AlarmStatus[alarmToSilence.PointTagAlarmStatusGuid];

            alarmStatus.Silenced = true;
            alarmStatus.SilencedTimestamp = now;
            alarmStatus.SilencedBy = security.UserID;

            pointTagAlarmStatusList.Add(alarmStatus);

            if (!silenceList.ContainsKey(alarmToSilence.TagGuid))
            {
               silenceList.Add(tag.PointTagGuid, tag);
            }

            if (alarmSilenceLoggingEnabled)
            {
               aandEDataElements.Add(new AandEDataElement(alarmStatus, alarmToSilence, true));
            }
         }

			// No longer sending Silence to Point Service

			if (pointTagAlarmStatusList.Any())
			{
				var pointTagAlarmStatus = new PointTagAlarmStatuses();
				pointTagAlarmStatus.Silence(security, pointTagAlarmStatusList);
			}

			if (aandEDataElements.Any())
         {
            var aAndETagArchive = new AandETagArchive();
            aAndETagArchive.AddArchiveData(security, aandEDataElements);
         }
      }

      public void ShelveAlarms(SecurityClass security, AlarmStatusCollectionClass alarmsToShelve)
		{
			DateTimeOffset lNow = DateTimeOffset.UtcNow;
			Dictionary< Guid, PointTag> shelveList = new Dictionary<Guid, PointTag>();
			{
				foreach (var alarmToShelve in alarmsToShelve)
				{
					var alarm = alarmToShelve.InputTag.Alarms[alarmToShelve.AlarmGuid];

					if (alarm != null)
					{
						if (alarmToShelve.ShelveDays > 0 || alarmToShelve.ShelveHours > 0 || alarmToShelve.ShelveMinutes > 0)
						{
							alarm.ShelvedEndTimeStamp =
								lNow.AddDays(alarmToShelve.ShelveDays)
									.AddHours(alarmToShelve.ShelveHours)
									.AddMinutes(alarmToShelve.ShelveMinutes);
							alarm.ShelvedStartTimeStamp = lNow;
						}
						else
						{
							alarm.ShelvedOneShot = true;
						}
						alarm.ShelvedBy = security.UserID;

						var alarmClone = (Alarm)alarm.Clone();
						alarmClone.AlarmTests = new Dictionary<Guid, AlarmTest>();
						alarmClone.AlarmStatus = new Dictionary<Guid, PointTagAlarmStatus>();

						PointTag inputTag;
						if (shelveList.TryGetValue(alarmToShelve.InputTag.PointTagGuid, out inputTag))
						{
							inputTag.Alarms.Add(alarmClone.AlarmGuid, alarmClone);
						}
						else
						{
							inputTag = (PointTag)alarmToShelve.InputTag.Clone();
							inputTag.Alarms = new Dictionary<Guid, Alarm>();
							inputTag.Alarms.Add(alarmClone.AlarmGuid, alarmClone);
							shelveList.Add(inputTag.PointTagGuid, inputTag);
						}
					}
				}
			}
			if (shelveList.Any())
			{
				var psm = new PointServiceManager();
				psm.SetShelve(security, shelveList.Values.ToList());
			}
		}

		public void OpcUaAcknowledgeAlarm(SecurityClass security, Guid alarmStatusIdentityGuid, string alarmStatus)
		{
			if (security == null || alarmStatusIdentityGuid == null || Guid.Empty == alarmStatusIdentityGuid || string.IsNullOrEmpty(alarmStatus))
			{
				return;
			}
		}
	}
}