using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.DataObjects
{
	using System.Data;
	using System.Data.SqlClient;
	using System.Drawing;
	using System.Globalization;
	using System.Runtime.CompilerServices;

	using FMBusinessObjects.Attributes;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;

	[Serializable]
	public class AlarmStatusSummaryCollectionClass : List<AlarmStatusSummaryClass>
	{

		public AlarmStatusSummaryCollectionClass()
		{
		}

		public AlarmStatusSummaryCollectionClass(IEnumerable<AlarmStatusSummaryClass> collection)
			 : base(collection)
		{
		}
	}

	[DataContract]
	[Serializable]
	public class AlarmStatusSummaryClass
	{
		public AlarmStatusSummaryClass()
		{
		}

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Guid AlarmGuid { get; set; }

		[DataMember]
		public DateTimeOffset Timestamp;

		[DataMember]
		public string FormattedTimestamp { get; set; }

		[DataMember]
		public string TimeStampUTCTicks
		{
			get
			{
				return this.Timestamp.UtcTicks.ToString();
			}
			private set
			{
			}
		}

		[DataMember]
		public string PointID { get; set; }

		[DataMember]
		public string TagID { get; set; }

		[DataMember]
		public string AlarmID { get; set; }

		[DataMember]
		public string Status { get; set; }

		[DataMember]
		public bool Acknowledged { get; set; }

		[DataMember]
		public bool AlarmTestFailed { get; set; }

		[DataMember]
		public bool UnAcknowledgedButNormal { get; set; }

		[DataMember]
		public string AlarmBackgroundSteadyColor { get; set; }

		[DataMember]
		public string AlarmTextSteadyColor { get; set; }

		[DataMember]
		public string AlarmBackgroundAlternateColor { get; set; }

		[DataMember]
		public string AlarmTextAlternateColor { get; set; }

		[DataMember]
		public string NormalUnacknowledgedAlarmBackgroundSteadyColor { get; set; }

		[DataMember]
		public string NormalUnacknowledgedAlarmTextSteadyColor { get; set; }

		[DataMember]
		public string NormalUnacknowledgedAlarmBackgroundAlternateColor { get; set; }

		[DataMember]
		public string NormalUnacknowledgedAlarmTextAlternateColor { get; set; }



		public void Copy(AlarmStatusSummaryClass src)
		{
			this.Description = src.Description;
			this.AlarmGuid = src.AlarmGuid;
			this.Timestamp = src.Timestamp;
			this.PointID = src.PointID;
			this.TagID = src.TagID;
			this.AlarmID = src.AlarmID;
			this.Status = src.Status;
			this.Acknowledged = src.Acknowledged;
			this.AlarmTestFailed = src.AlarmTestFailed;
			this.UnAcknowledgedButNormal = src.UnAcknowledgedButNormal;
			this.AlarmBackgroundSteadyColor = src.AlarmBackgroundSteadyColor;
			this.AlarmTextSteadyColor = src.AlarmTextSteadyColor;
			this.AlarmBackgroundAlternateColor = src.AlarmBackgroundAlternateColor;
			this.AlarmTextAlternateColor = src.AlarmTextAlternateColor;
		}

	}

	[Serializable]
	public class AlarmStatusCollectionClass : List<AlarmStatusClass>
	{
		public SiteClass Site { get; set; }
		public DateTimeFormatInfo DateFormatInfo { get; set; }

		public AlarmStatusCollectionClass()
		{
		}

		public AlarmStatusCollectionClass(IEnumerable<AlarmStatusClass> collection)
			 : base(collection)
		{
		}

		public AlarmStatusSummaryCollectionClass GetAlarmSummaries()
		{
			AlarmStatusSummaryCollectionClass coll = new AlarmStatusSummaryCollectionClass();
			if (this.Count > 0)
			{
				foreach (var alarmStatus in this)
				{
					if (this.Site == null)
					{
						coll.Add(alarmStatus.GetAlarmStatusSummary());
					}
					else
					{
						coll.Add(alarmStatus.GetAlarmStatusSummary(this.Site.GetDateTimeFormatInfo(), this.Site.GetTimeZoneInfo()));
					}

				}

			}
			return coll;
		}

		public void SetAlarmAcknowledgments(AlarmStatusSummaryCollectionClass alarmSummaries)
		{
			foreach (var alarmSummary in alarmSummaries)
			{
				var index = this.FindIndex(x => x.IdentityGuid == alarmSummary.AlarmGuid);
				if (index != -1)
				{
					this[index].Acknowledged = alarmSummary.Acknowledged;
				}
			}

		}
	}

	[DataContract]
	[Serializable]
	public class AlarmStatusClass : BaseDataObject, IAlarmAndEventDiscovery
    {
        static string PointAlarmStatusKey = "Point Alarm Status Not Normal Notification";
        public static AlarmAndEventDescriptorClass PointAlarmStatusNotNormalNotificationDescriptor = new AlarmAndEventDescriptorClass(true, BaseObjectClass.PointManagerKey, PointAlarmStatusKey);
        AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
        {
            get
            {
                AlarmAndEventDescriptorClass[] Descriptors ={  PointAlarmStatusNotNormalNotificationDescriptor   };

                return Descriptors;
            }
        }

        public AlarmStatusClass()
		{
		}

		public AlarmStatusSummaryClass GetAlarmStatusSummary()
		{
			var status = new AlarmStatusSummaryClass
			{
				Description = this.Description,
				AlarmGuid = this.AlarmGuid,
				Timestamp = this.Timestamp,
				FormattedTimestamp = this.Timestamp.ToString(),
				PointID = this.PointID,
				TagID = this.TagID,
				AlarmID = this.AlarmID,
				Status = this.AlarmState,
				Acknowledged = this.Acknowledged,
				AlarmTestFailed = this.AlarmTestFailed,
				UnAcknowledgedButNormal = this.UnAcknowledgedButNormal,
				AlarmBackgroundSteadyColor = this.AlarmBackgroundSteadyColor,
				AlarmTextSteadyColor = this.AlarmTextSteadyColor,
				AlarmBackgroundAlternateColor = this.AlarmBackgroundAlternateColor,
				AlarmTextAlternateColor = this.AlarmTextAlternateColor,
				NormalUnacknowledgedAlarmBackgroundSteadyColor = this.NormalUnacknowledgedAlarmBackgroundSteadyColor,
				NormalUnacknowledgedAlarmTextSteadyColor = this.NormalUnacknowledgedAlarmTextSteadyColor,
				NormalUnacknowledgedAlarmBackgroundAlternateColor = this.NormalUnacknowledgedAlarmBackgroundAlternateColor,
				NormalUnacknowledgedAlarmTextAlternateColor = this.NormalUnacknowledgedAlarmTextAlternateColor
			};
			return status;
		}

		public AlarmStatusSummaryClass GetAlarmStatusSummary(DateTimeFormatInfo formatInfo, TimeZoneInfo timeZoneInfo)
		{
			var status = this.GetAlarmStatusSummary();

			status.FormattedTimestamp = TimeConverter.ToSiteTime(timeZoneInfo, status.Timestamp).DateTime.ToString(formatInfo);

			return status;
		}

		public void SetFromAlarmStatusSummary(AlarmStatusSummaryClass status)
		{
			if (status == null
				 || this.AlarmGuid != status.AlarmGuid) return;

			this.Acknowledged = status.Acknowledged;
		}

		[DataMember]
		public string AcknowledgeComment = string.Empty;

		[DataMember]
		public string Description;

		[DataMember]
		public PointTag InputTag;

		[DataMember]
		public Guid AlarmGuid
		{
			get
			{
				return this.IdentityGuid;
			}
			set
			{
				this.IdentityGuid = value;
			}

		}

		public string AlarmState
		{
			get
			{
				var alarm = this.InputTag.Alarms[this.AlarmGuid];
				return alarm.GetActiveAlarmState();
			}

			private set
			{

			}

		}


		private bool CheckIfInputTagDefinedObject()
		{
			return (this.InputTag != null);
		}

		private bool CheckIfInputTagAlarmIsDefinedObject()
		{
			if (!this.CheckIfInputTagDefinedObject()) return false;
			return (this.AlarmGuid != Guid.Empty && this.InputTag.Alarms != null && this.InputTag.Alarms.Count > 0
					  && this.InputTag.Alarms.ContainsKey(this.AlarmGuid));

		}

		public AlarmTest GetAlarmTest()
		{
			var alarm = this.InputTag.Alarms[this.AlarmGuid];
			return alarm.GetAlarmTestByAlarmState(this.AlarmState);
		}


		public PointTagAlarmStatus GeTagAlarmStatus()
		{
			var alarm = this.InputTag.Alarms[this.AlarmGuid];
			return alarm.GetAlarmStatusByAlarmState(this.AlarmState);
		}

		public DateTimeOffset Timestamp
		{
			get
			{
				var alarmFailedTimeStamp = DateTimeOffset.MinValue;
				var alarmStatus = this.GeTagAlarmStatus();
				if (alarmStatus != null && alarmStatus.AlarmTestFailedTimestamp != null)
				{
					alarmFailedTimeStamp = (DateTimeOffset)alarmStatus.AlarmTestFailedTimestamp;
				}
				return alarmFailedTimeStamp;
			}
			set
			{
				var alarmStatus = this.GeTagAlarmStatus();
				if (alarmStatus != null && alarmStatus.AlarmTestFailedTimestamp != null)
				{
					alarmStatus.AlarmTestFailedTimestamp = value;
				}
			}
		}

		[DataMember]
		public string TimeStampUTCTicks
		{
			get
			{
				return this.Timestamp.UtcTicks.ToString();
			}
			private set
			{
			}
		}

		public string PointID
		{
			get
			{
				return (this.CheckIfInputTagDefinedObject()) ? this.InputTag.PointID : string.Empty;
			}
			set
			{
				if (!this.CheckIfInputTagDefinedObject()) return;
				this.InputTag.PointID = value;
			}
		}

		public string TagID
		{
			get
			{
				return (!this.CheckIfInputTagDefinedObject()) ? string.Empty : this.InputTag.ID;
			}
			set
			{
				if (!this.CheckIfInputTagDefinedObject()) return;
				this.InputTag.ID = value;
			}
		}

		public string AlarmID
		{
			get
			{
				return (this.CheckIfInputTagAlarmIsDefinedObject())
					 ? this.InputTag.Alarms[this.AlarmGuid].ID
					 : string.Empty;
			}
			set
			{
				if (!this.CheckIfInputTagAlarmIsDefinedObject()) return;
				this.InputTag.Alarms[this.AlarmGuid].ID = value;
			}
		}


		//[DataMember]
		public bool Acknowledged
		{
			get
			{
				var alarmStatus = this.GeTagAlarmStatus();
				if (alarmStatus != null)
				{
					return alarmStatus.Acknowledged;
				}
				return false;
			}
			set
			{
				var alarmStatus = this.GeTagAlarmStatus();
				if (alarmStatus != null)
				{
					alarmStatus.Acknowledged = value;
				}
			}
		}


		//[DataMember]
		public bool AlarmTestFailed
		{
			get
			{
				var alarmStatus = this.GeTagAlarmStatus();
				if (alarmStatus != null)
				{
					return alarmStatus.AlarmTestFailed;
				}
				return false;
			}
			set
			{
				var alarmStatus = this.GeTagAlarmStatus();
				if (alarmStatus != null)
				{
					alarmStatus.AlarmTestFailed = value;
				}
			}

		}


		//[DataMember]
		public bool UnAcknowledgedButNormal
		{
			get
			{
				var alarm = this.InputTag.Alarms[this.AlarmGuid];
				var unacknowledged = false;
				var normal = true;

				foreach (var alarmStatus in alarm.AlarmStatus.Values)
				{
					if (alarmStatus.Acknowledged == false)
					{
						unacknowledged =  true;
					}
					if (alarmStatus.AlarmTestFailed)
					{
						normal = false;
					}
				}

				return (normal && unacknowledged);
			}
			private set
			{

			}

		}

		[DataMember]
		public string AlarmBackgroundSteadyColor;

		[DataMember]
		public string AlarmTextSteadyColor;

		[DataMember]
		public string AlarmBackgroundAlternateColor;

		[DataMember]
		public string AlarmTextAlternateColor;

		[DataMember]
		public string NormalUnacknowledgedAlarmBackgroundSteadyColor;

		[DataMember]
		public string NormalUnacknowledgedAlarmTextSteadyColor;

		[DataMember]
		public string NormalUnacknowledgedAlarmBackgroundAlternateColor;

		[DataMember]
		public string NormalUnacknowledgedAlarmTextAlternateColor;

		[DataMember]
		public int ShelveDays;

		[DataMember]
		public int ShelveHours;

		[DataMember]
		public int ShelveMinutes;

	}

	[DataContract]
	[Serializable]
	public class AlarmStatusClass2
	{
		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Guid AlarmGuid { get; set; }

		[DataMember]
		public Guid AlarmTestGuid { get; set; }

		[DataMember]
		public DateTimeOffset Timestamp;

		[DataMember]
		public Guid PointTagAlarmStatusGuid;

		[DataMember]
		public string FormattedTimestamp { get; set; }

		public string TimeStampUTCTicks
		{
			get
			{
				return this.Timestamp.UtcTicks.ToString();
			}
			private set
			{
			}
		}

		[DataMember]
		public string SiteID { get; set; }

		[DataMember]
		public Guid SiteGuid { get; set; }


		[DataMember]
		public string PointID { get; set; }


		[DataMember]
		public string PointType { get; set; }


		[DataMember]
		public Guid PointGuid { get; set; }

		[DataMember]
		public string TagID { get; set; }

		[DataMember]
		public Guid TagGuid { get; set; }


		[DataMember]
		public string AlarmID { get; set; }

		[DataMember]
		public string Status { get; set; }

		[DataMember]
		public bool Acknowledged { get; set; }

		[DataMember]
		public bool Silenced { get; set; }

		[DataMember]
		public int AlarmTestPriority { get; set; }

		[DataMember]
		public int AlarmPriority { get; set; }

		[DataMember]
		public bool IsNormal { get; set; }

		[DataMember]
		public string AlarmPriorityID { get; set; }

		[DataMember]
		public string AlarmBackgroundSteadyColor { get; set; }

		[DataMember]
		public string AlarmTextSteadyColor { get; set; }

		[DataMember]
		public string AlarmBackgroundAlternateColor { get; set; }

		[DataMember]
		public string AlarmTextAlternateColor { get; set; }

		[DataMember]
		public string NormalUnacknowledgedAlarmBackgroundSteadyColor { get; set; }

		[DataMember]
		public string NormalUnacknowledgedAlarmTextSteadyColor { get; set; }

		[DataMember]
		public string NormalUnacknowledgedAlarmBackgroundAlternateColor { get; set; }

		[DataMember]
		public string NormalUnacknowledgedAlarmTextAlternateColor { get; set; }

		[DataMember]
		public string SoundFile { get; set; }

		[DataMember]
		public bool Acknowledge;


		public AlarmStatusClass2()
		{
			this.Acknowledge = true;
		}

		public AlarmStatusClass2(DataRow row)
		{
			this.Load(row);
			this.Acknowledge = true;
		}

		protected static string ConvertArchiveDateTimeToLocalTime(DateTime utcTime, string timePattern, string shortDatePattern, string timeZone)
		{
			string localTimeStr = utcTime.ToString(timePattern);
			string localDateStr = utcTime.ToString(shortDatePattern);
			string localDateTimeStr = localDateStr + " " + localTimeStr;


			try
			{
				TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
				DateTimeOffset localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, localTimeZone);

				localTimeStr = localDateTime.ToString(timePattern);
				localDateStr = localDateTime.ToString(shortDatePattern);
				localDateTimeStr = localDateStr + " " + localTimeStr;
				return localDateTimeStr;
			}
			catch (TimeZoneNotFoundException timeZoneNotFoundExcept)
			{
				Console.Write(timeZoneNotFoundExcept.Message);
			}
			catch (InvalidTimeZoneException invalidTimeZoneExcept)
			{
				Console.Write(invalidTimeZoneExcept.Message);
			}

			return localDateTimeStr;
		}

		public void Load(DataRow row)
		{
			if (row != null)
			{
				this.Description = DataObject.getValue<string>(row["Description"], "");
				this.AlarmGuid = DataObject.getValue<Guid>(row["AlarmGuid"], Guid.Empty);
				this.AlarmTestGuid = DataObject.getValue<Guid>(row["AlarmTestGuid"], Guid.Empty);
				this.AlarmTestPriority = DataObject.getValue<int>(row["AlarmTestPriority"], 99999);
				this.AlarmPriority = DataObject.getValue<byte>(row["AlarmPriority"], 255);
				this.Timestamp = DataObject.getValue<DateTimeOffset>(row["Timestamp"], DateTimeOffset.Now);
				this.PointTagAlarmStatusGuid = DataObject.getValue<Guid>(row["PointTagAlarmStatusGuid"], Guid.Empty);
				this.SiteID = DataObject.getValue<string>(row["SiteID"], "");
				this.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
				this.PointID = DataObject.getValue<string>(row["PointID"], "");
				this.PointType = DataObject.getValue<string>(row["PointType"], "");
				this.PointGuid = DataObject.getValue<Guid>(row["PointGuid"], Guid.Empty);
				this.TagID = DataObject.getValue<string>(row["TagID"], "");
				this.TagGuid = DataObject.getValue<Guid>(row["TagGuid"], Guid.Empty);
				this.AlarmID = DataObject.getValue<string>(row["AlarmID"], "");
				this.Status = DataObject.getValue<string>(row["Status"], "");
				this.Acknowledged = DataObject.getValue<bool>(row["Acknowledged"], true);
				this.Silenced = DataObject.getValue<bool>(row["Silenced"], true);
				this.IsNormal = DataObject.getValue<bool>(row["IsNormal"], true);
				this.AlarmPriorityID = DataObject.getValue<string>(row["AlarmPriorityID"], "");
				this.AlarmBackgroundSteadyColor = DataObject.getValue<string>(row["AlarmBackgroundSteadyColor"], "");
				this.AlarmTextSteadyColor = DataObject.getValue<string>(row["AlarmTextSteadyColor"], "");
				this.AlarmBackgroundAlternateColor = DataObject.getValue<string>(row["AlarmBackgroundAlternateColor"], "");
				this.AlarmTextAlternateColor = DataObject.getValue<string>(row["AlarmTextAlternateColor"], "");
				this.NormalUnacknowledgedAlarmBackgroundSteadyColor = DataObject.getValue<string>(row["NormalUnacknowledgedAlarmBackgroundSteadyColor"], "");
				this.NormalUnacknowledgedAlarmTextSteadyColor = DataObject.getValue<string>(row["NormalUnacknowledgedAlarmTextSteadyColor"], "");
				this.NormalUnacknowledgedAlarmBackgroundAlternateColor = DataObject.getValue<string>(row["NormalUnacknowledgedAlarmBackgroundAlternateColor"], "");
				this.NormalUnacknowledgedAlarmTextAlternateColor = DataObject.getValue<string>(row["NormalUnacknowledgedAlarmTextAlternateColor"], "");
				this.SoundFile = DataObject.getValue<string>(row["SoundFile"], "");
				var shortDatePattern = DataObject.getValue<string>(row["ShortDatePattern"], "");
				var timePattern = DataObject.getValue<string>(row["TimePattern"], "");
				var timeZone = DataObject.getValue<string>(row["TimeZone"], "");
				//Need to format with site format information.
				this.FormattedTimestamp = ConvertArchiveDateTimeToLocalTime(
					this.Timestamp.DateTime,
					timePattern,
					shortDatePattern,
					timeZone);
			}
			else
			{
				this.Init();
			}
		}

		public void Init()
		{
			this.Description = "";
			this.AlarmGuid = Guid.Empty;
			this.AlarmTestGuid = Guid.Empty;
			this.AlarmTestPriority = 99999;
			this.AlarmPriority = 99999;
			this.Timestamp = DateTimeOffset.Now;
			this.PointTagAlarmStatusGuid = Guid.Empty;
			this.FormattedTimestamp = this.Timestamp.DateTime.ToString();
			this.SiteID = "";
			this.PointID = "";
			this.PointType = "";
			this.PointGuid = Guid.Empty;
			this.TagID = "";
			this.TagGuid = Guid.Empty;
			this.AlarmID = "";
			this.Status = "";
			this.Acknowledged = true;
			this.Silenced = true;
			this.IsNormal = true;
			this.AlarmPriorityID = "";
			this.AlarmBackgroundSteadyColor = "";
			this.AlarmTextSteadyColor = "";
			this.AlarmBackgroundAlternateColor = "";
			this.AlarmTextAlternateColor = "";
			this.NormalUnacknowledgedAlarmBackgroundSteadyColor = "";
			this.NormalUnacknowledgedAlarmTextSteadyColor = "";
			this.NormalUnacknowledgedAlarmBackgroundAlternateColor = "";
			this.NormalUnacknowledgedAlarmTextAlternateColor = "";
			this.SoundFile = "";
		}
	}
}
