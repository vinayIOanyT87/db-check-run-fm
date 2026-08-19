
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.Constants;

	public sealed class AandEDataElement : ICloneable
	{
		public string PointDescription { get; set; }
		public Guid SiteGuid { get; set; }
		public Guid AlarmOrTagGuid { get; set; }
		public Guid AlarmTestGuid { get; set; }
		public string Point { get; set; }
		public string Site { get; set; }
		public string AlarmState { get; set; }
		public string PointType { get; set; }
		public string Variable { get; set; }
		public string Value { get; set; }
		public string Units { get; set; }
		public string Priority { get; set; }
		public string Action { get; set; }
		public string User { get; set; }
		public string Comments { get; set; }
		public int RecordType { get; set; }
		public Guid RecordGuid { get; set; }
		public DateTimeOffset DateAndTime { get; set; }
		public string CommentUser { get; set; }
		public DateTimeOffset CommentDateTime { get; set; }

		public long Partition
		{
			get
			{
				return AandEDataElement.GetPartition(this.DateAndTime);
			}
			set
			{
				var dummyYearMonth = value;
			}
		}

		public static int GetPartition(DateTimeOffset timeStamp)
		{
			return timeStamp.Year * 10000 + timeStamp.Month * 100 + timeStamp.Day;
		}


		public object Clone()
		{
			AandEDataElement aede = (AandEDataElement)this.MemberwiseClone();
			return aede;
		}

		public AandEDataElement()
		{
		}


		public AandEDataElement(SecurityClass security, PointValue value)
        {
			this.SiteGuid = value.PointValueIdentifier.SiteGuid;
			this.Site = value.SiteID;
			this.Point = value.PointID;
			this.PointType = value.PointType;
			this.PointDescription = value.PointDescription;
			this.AlarmOrTagGuid = value.PointValueIdentifier.IdentityGuid;
			this.Action = "Unknown";
			// A forced value or
			// Manual
			if (value.OpcStatusCodeBits == Opc.Ua.StatusCodes.GoodLocalOverride || 
				value.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual)
			{
				this.Action = "Command";
			}
			else if (value.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated ||
					value.InputOutputType == PointTemplateTag.PointTagInputOutputType.OpcUa ||
					value.InputOutputType == PointTemplateTag.PointTagInputOutputType.FCEE)
			{
				this.Action = "Status";
			}
			this.DateAndTime = DateTimeOffset.Now.ToUniversalTime();
			this.Variable = value.ID;
			this.Value = (value.Value != null) ? value.Value.ToString() : string.Empty;
			this.Units = value.Units.ToString();
			this.User = security.UserID;
			this.RecordType = 2;    // 1 = alarm record, 2 = event record
			this.RecordGuid = Guid.NewGuid();
		}

		public AandEDataElement(PointTagAlarmStatus alarmStatus, Point point, PointTag tag, Alarm alarm, AlarmTest alarmTest, bool ack = false)
		{

			this.PointDescription = point.Description;
			this.PointType = point.PointType;
			this.Site = point.SiteID;
			this.SiteGuid = point.SiteGuid;
			this.AlarmOrTagGuid = alarmStatus.AlarmGuid;
			this.AlarmTestGuid = alarmTest.AlarmTestGuid;
			this.Point = alarmStatus.PointID;
			this.AlarmState = alarmStatus.AlarmID;
			this.Action = "Normal";
			this.DateAndTime = tag.ServerTimeStamp.ToUniversalTime();
			this.User = "";
			this.Comments = "";

			this.CommentUser = "";
			//this.CommentDateTime = tag.ServerTimeStamp.ToUniversalTime();
			//Alarm Test Failed, Alarm Tested Passed, Alarm Test Acknowledged, Alarm Comment Edited, Shevled

			// set the tag data if present or just leave blank
			this.Value = (tag.Value != null) ? tag.Value.ToString() : string.Empty;
			this.Units = tag.Units.ToString();


			if (alarmStatus.AlarmTestFailed)
			{
				this.Action = "Alarm";
			}

			if (alarmStatus.Acknowledged && ack)
			{
				this.Action = "Acknowledged";
				this.User = alarmStatus.AcknowledgedBy;
				this.Comments = alarmStatus.AcknowledgedComment;

				if (!string.IsNullOrEmpty(this.Comments))
				{
					this.CommentUser = alarmStatus.AcknowledgedBy;

					if (alarmStatus.AcknowledgedTimestamp != null)
					{
						this.CommentDateTime = (DateTimeOffset)alarmStatus.AcknowledgedTimestamp;
					}
				}

				if (alarmStatus.AcknowledgedTimestamp != null)
				{
					this.DateAndTime = (DateTimeOffset)alarmStatus.AcknowledgedTimestamp;
				}
			}

			this.Variable = alarmStatus.TagID;
			this.Priority = alarmTest.AlarmPriority.ToString();

			string alarmState = alarm.GetActiveAlarmState(false, true);

			if (alarm.ExclusiveAlarm && !String.IsNullOrEmpty(alarmState) && alarmState != alarmTest.AlarmState)
				alarmState = alarmTest.AlarmState;

			this.AlarmState = alarmState;

			this.RecordType = 1;    // 1 = alarm record, 2 = event record
			this.RecordGuid = Guid.NewGuid();
		}


		public AandEDataElement(PointTagAlarmStatus alarmStatus, AlarmStatusClass2 alarmStatusClass2, bool silenced = false)
		{
			this.PointDescription = alarmStatusClass2.Description;
			this.PointType = alarmStatusClass2.PointType;
			this.Site = alarmStatusClass2.SiteID;
			this.SiteGuid = alarmStatusClass2.SiteGuid;
			this.AlarmOrTagGuid = alarmStatus.AlarmGuid;
			this.AlarmTestGuid = alarmStatus.AlarmTestGuid;
			this.Point = alarmStatus.PointID;
			this.AlarmState = alarmStatus.AlarmID;
			this.Action = "Normal";
			this.DateAndTime = DateTimeOffset.Now.ToUniversalTime();
			this.User = "";
			this.Comments = "";

			this.CommentUser = "";
			//this.CommentDateTime = DateTimeOffset.Now.ToUniversalTime();
			//Alarm Test Failed, Alarm Tested Passed, Alarm Test Acknowledged, Alarm Comment Edited, Shevled

			// set the tag data if present or just leave blank
			this.Value = string.Empty;
			this.Units = string.Empty;

			if (silenced)
			{
				this.Action = "Silenced";
				this.User = alarmStatus.SilencedBy;
				this.DateAndTime = (DateTimeOffset)alarmStatus.SilencedTimestamp;
         }
			else
			{
				this.Action = "Acknowledged";
				this.User = alarmStatus.AcknowledgedBy;
				this.Comments = alarmStatus.AcknowledgedComment;


				if (!string.IsNullOrEmpty(this.Comments))
				{
					this.CommentUser = alarmStatus.AcknowledgedBy;

					if (alarmStatus.AcknowledgedTimestamp != null)
					{
						this.CommentDateTime = (DateTimeOffset)alarmStatus.AcknowledgedTimestamp;
					}
				}

				this.DateAndTime = (DateTimeOffset)alarmStatus.AcknowledgedTimestamp;
			}
			this.Variable = alarmStatus.TagID;
			this.Priority = alarmStatusClass2.AlarmPriority.ToString();
			this.AlarmState = alarmStatusClass2.Status;

			this.RecordType = 1;    // 1 = alarm record, 2 = event record
			this.RecordGuid = Guid.NewGuid();
		}
	}
}