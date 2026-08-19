namespace FMPointService.Archiving
{
	using System;
	using System.Linq;
	using FMBusinessObjects.DataObjects;
	using ThreadSupport;
	using System.Collections.Generic;
	using Opc.Ua;
    using global::FMPointService.AlarmAndEventArchive;

   internal sealed class ArchiveRecordQueuer 
	{
		public ArchiveProcessorSignaler ArchiveProcessorSignaler = new ArchiveProcessorSignaler();
		public static readonly int ArchiveCountSignalThreshold = 256;

		private static PointValueArchiveQueue archiveQueue = new PointValueArchiveQueue();
		private static PointValueArchiveQueue failedArchiveQueue = new PointValueArchiveQueue();

		private static Dictionary<PointValueIdentifier, ArchiveDataElement> overflowArchiveDictionary = new Dictionary<PointValueIdentifier, ArchiveDataElement>();


		public void ClearQueue()
		{
			archiveQueue = new PointValueArchiveQueue();
			failedArchiveQueue = new PointValueArchiveQueue();
		}

		public int Count
		{
			get
			{
				return archiveQueue.Count;
			}
		}

		public int FailedCount
		{
			get
			{
				return failedArchiveQueue.Count;
			}
		}

		public void CreateAndQueuePointArchive(Point point, bool forceMidnightTimeUTC)
		{
			foreach (var pointTag in point.Tags.Values)
			{
				// skip calculated tags when not midnight
				if (!pointTag.Archived)
				{
					continue;
				}

				// Archive Alarm Information on Startup but not Midnight.  The AlarmArchive is not partitioned by Year Month and is expected to be queried for
				// an entire period.  However and Alarm Information record is needed on startup in case the most recent record failed to write. 
				var archiveAlarm = (!forceMidnightTimeUTC && pointTag.Alarms.Any()) ? true : false;
				this.CreateAndQueueArchiveRecord(new PointValue(pointTag), true, forceMidnightTimeUTC, archiveAlarm);
			}

			foreach (var pointProperty in point.Properties.Values)
			{
				var exposedSettings = pointProperty.GetExposedSettings(point);
				foreach (var exposedSetting in exposedSettings)
				{
					this.CreateAndQueueArchiveRecord(exposedSetting, true, forceMidnightTimeUTC, false);
				}
			}

			foreach(var exposedSetting in point.GetExposedSettings())
			{
				this.CreateAndQueueArchiveRecord(exposedSetting, true, forceMidnightTimeUTC, false);
			}
		}

		/// <summary>
		/// Queues point tag data in the archive queue for writing to the archive.
		/// </summary>
		/// <param name="pointTag">The point tag to record.</param>
		public void CreateAndQueueArchiveRecord( PointValue pointValue, bool forceTimeStampValue, bool ForceMidnightTimeUTC, bool alarmOrStatusChanged)
		{
			// forceTimeStampValue if set will use the current time for the archive record
			// this is because we need a set at midnight UTC regardless of what the point time stamp actually is.
			DateTimeOffset UTCTimeNow = DateTime.UtcNow;
			if (ThreadSharedData.Instance().EnableArchiveData)
			{
				ArchiveDataElement archiveDataElement = new ArchiveDataElement()
				{
					ArchiveRecordType = 3, // not used
					DataType = pointValue.ValueTypeString,
					EngineeringUnitsIndex = (int)pointValue.Units,
					PointValueGuid = pointValue.PointValueIdentifier.IdentityGuid,
					PropertyID = pointValue.PointValueIdentifier.PropertyID,
					Value = pointValue.Value == null ? null : pointValue.Value.ToString(),
					ValueOpcStatus = pointValue.Status,
					AlarmPriorityGuid = pointValue.AlarmPriorityGuid,
					Acknowledged = pointValue.Acknowledged,
					AlarmState = pointValue.AlarmState,
					AlarmOrStatusChanged = alarmOrStatusChanged,
					RecordTimeStamp = DateTimeOffset.UtcNow,
					SiteGuid = pointValue.PointValueIdentifier.SiteGuid
				};

				// set the quality string for reports based on the tag status
				archiveDataElement.QualityString = SetArchiveElementQualityString(pointValue.Status, pointValue.InputOutputType);

				if (forceTimeStampValue == true)
				{
					if (ForceMidnightTimeUTC == true)
					{
						archiveDataElement.ValueTimeStamp = new DateTimeOffset(UTCTimeNow.Year, UTCTimeNow.Month, UTCTimeNow.Day, 0, 0, 0, 0, new System.TimeSpan(0));
					}
					else
					{
						archiveDataElement.ValueTimeStamp = UTCTimeNow;
					}
				}
				else
				{
					// Archive Value Time Stamp is always current time.
					archiveDataElement.ValueTimeStamp = UTCTimeNow;
				}

				// If the archive has reached the maximum count or archive failed for the PointValue, replace the prior ArchiveElement with this latest
				lock (overflowArchiveDictionary)
				{
					if (archiveQueue.Count > ThreadSharedData.Instance().ArchiveQueueMaximum
					|| overflowArchiveDictionary.ContainsKey(pointValue.PointValueIdentifier))
					{
						this.AddOverflowArchiveDataElement(archiveDataElement);
						return;
					}
				}

				archiveQueue.QueueItemForArchiving( archiveDataElement );

				if (archiveQueue.Count > ArchiveCountSignalThreshold)
				{
					this.ArchiveProcessorSignaler.SignalExpedite();
				}
			}
		}


		public void QueueOverflowDictionary()
		{
			lock (overflowArchiveDictionary)
			{
				foreach (var archiveDataElement in overflowArchiveDictionary.Values)
				{
					archiveQueue.QueueItemForArchiving(archiveDataElement);
				}

				overflowArchiveDictionary.Clear();
			}
		}

		public void AddFailedArchiveDataElement(ArchiveDataElement archiveDataElement)
		{
			failedArchiveQueue.QueueItemForArchiving(archiveDataElement);
		}


		public bool IsEmpty
		{
			get
			{
				return archiveQueue.IsEmpty;
			}
		}

		public bool IsFailedEmpty
		{
			get
			{
				return failedArchiveQueue.IsEmpty;
			}
		}


		public void AddOverflowArchiveDataElement(ArchiveDataElement archiveDataElement)
		{
			var pointValueIdentifier = new PointValueIdentifier() { IdentityGuid = archiveDataElement.PointValueGuid,
				PointValueType = (string.IsNullOrEmpty(archiveDataElement.PropertyID)) ? PointValueType.Tag : PointValueType.Setting,
				PropertyID = archiveDataElement.PropertyID };

			lock (overflowArchiveDictionary)
			{
				if (!overflowArchiveDictionary.ContainsKey(pointValueIdentifier))
				{
					overflowArchiveDictionary.Add(pointValueIdentifier, archiveDataElement);
				}
				else if(archiveDataElement.ValueTimeStamp > overflowArchiveDictionary[pointValueIdentifier].ValueTimeStamp)
				{
					overflowArchiveDictionary[pointValueIdentifier] = archiveDataElement;
				}
			}
		}

		public bool TryDequeueItem(out ArchiveDataElement archiveDataElement)
		{
			return archiveQueue.TryDequeueItem(out archiveDataElement);
		}

		public bool TryDequeueFailedItem(out ArchiveDataElement archiveDataElement)
		{
			return failedArchiveQueue.TryDequeueItem(out archiveDataElement);
		}

		public string SetArchiveElementQualityString(long Status, PointTemplateTag.PointTagInputOutputType inputoutputType)
		{
			// this function will set the quality field for the reports based on the following 
			string stReturn = string.Empty;
			bool addDash = false;
			var tagstatusCode = new StatusCode((uint)Status);

			// bad
			if (StatusCode.IsBad(tagstatusCode))
			{
				return "BAD";
			}
			// frc forced
			if (tagstatusCode.SubCode == StatusCodes.GoodLocalOverride)
			{
				stReturn += "FRC";
				addDash = true;
			}
			// man
			if (inputoutputType == PointTemplateTag.PointTagInputOutputType.Manual)
			{
				if (addDash)
					stReturn += "-";
				stReturn += "MAN";
				addDash = true;
			}
			// ovr
			if (tagstatusCode.LimitBits == LimitBits.High)
			{
				if (addDash)
					stReturn += "-";
				stReturn += "OVR";
				addDash = true;
			}
			// unr
			if (tagstatusCode.LimitBits == LimitBits.Low)
			{
				if (addDash)
					stReturn += "-";
				stReturn += "UNR";
				addDash = true;
			}
			// wrn
			if (StatusCode.IsUncertain(tagstatusCode))
			{
				if (addDash)
					stReturn += "-";
				stReturn += "UNC";
				addDash = true;
			}
			return stReturn;
		}

	}
}
