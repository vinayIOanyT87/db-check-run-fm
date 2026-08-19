namespace FMPointService.ThreadSupport
{
   using System;
	using System.Collections.Generic;
	using System.Linq;
   using FMBusinessObjects.DataObjects;
   using FMCore;
   using OpcClient;
   using Opc.Ua;

	internal class PointTagValueChanger 
	{
		public bool SetAlarmAndAlarmStatusIfChanged(PointTag originalTag, PointTag newTag)
		{
			bool ret = false;
			if (newTag.Alarms.Any())
			{
            foreach (var newAlarm in newTag.Alarms.Values)
				{
					Alarm originalAlarm;
					if (originalTag.Alarms.TryGetValue(newAlarm.IdentityGuid, out originalAlarm))
					{
						if (originalAlarm.ShelvedOneShot != newAlarm.ShelvedOneShot
						    || originalAlarm.ShelvedStartTimeStamp != newAlarm.ShelvedStartTimeStamp
						    || originalAlarm.ShelvedEndTimeStamp != newAlarm.ShelvedEndTimeStamp)
						{
							originalAlarm.ShelvedOneShot = newAlarm.ShelvedOneShot;
							originalAlarm.ShelvedStartTimeStamp = newAlarm.ShelvedStartTimeStamp;
							originalAlarm.ShelvedEndTimeStamp = newAlarm.ShelvedEndTimeStamp;
							originalAlarm.ShelvedBy = newAlarm.ShelvedBy;
							originalAlarm.UpdatedBy = newAlarm.UpdatedBy;
							originalAlarm.UpdatedDate = newAlarm.UpdatedDate;
							ret = true;
						}

						foreach (var newAlarmStatus in newAlarm.AlarmStatus.Values)
						{
							PointTagAlarmStatus originalAlarmStatus;
							if (originalAlarm.AlarmStatus.TryGetValue(newAlarmStatus.IdentityGuid, out originalAlarmStatus))
							{
								bool alarmTestFailedUpdated = false;

								if (originalAlarmStatus.AlarmTestFailedTimestamp.HasValue
								&& newAlarmStatus.AlarmTestFailedTimestamp.HasValue
								&& newAlarmStatus.AlarmTestFailedTimestamp.Value > originalAlarmStatus.AlarmTestFailedTimestamp.Value)
								{
									alarmTestFailedUpdated = true;
								}


								if (originalAlarmStatus.AlarmTestFailed != newAlarmStatus.AlarmTestFailed
								|| alarmTestFailedUpdated)
								{
									// Alarm Test transition to true or update AlarmTestFailedTimestamp updates Acknowledge and Silence
									if (!originalAlarmStatus.AlarmTestFailed && newAlarmStatus.AlarmTestFailed
									|| alarmTestFailedUpdated)
									{
                              originalAlarmStatus.Acknowledged = newAlarmStatus.Acknowledged;
										originalAlarmStatus.AcknowledgedTimestamp = newAlarmStatus.AcknowledgedTimestamp;
										originalAlarmStatus.AcknowledgedBy = newAlarmStatus.AcknowledgedBy;
										originalAlarmStatus.Silenced = newAlarmStatus.Silenced;
										originalAlarmStatus.SilencedTimestamp = newAlarmStatus.SilencedTimestamp;
										originalAlarmStatus.SilencedBy = newAlarmStatus.SilencedBy;
									}

									originalAlarmStatus.AlarmTestFailed = newAlarmStatus.AlarmTestFailed;
									originalAlarmStatus.AlarmTestFailedTimestamp = newAlarmStatus.AlarmTestFailedTimestamp;
									originalAlarmStatus.UpdatedBy = newAlarmStatus.UpdatedBy;
									originalAlarmStatus.UpdatedDate = newAlarmStatus.UpdatedDate;
									originalAlarmStatus.WrittenToEnterprise = false;
									ret = true;
								}

                        if (originalAlarmStatus.Acknowledged != newAlarmStatus.Acknowledged
								&& newAlarmStatus.AcknowledgedTimestamp.HasValue)
								{
									originalAlarmStatus.Acknowledged = newAlarmStatus.Acknowledged;
									originalAlarmStatus.AcknowledgedTimestamp = newAlarmStatus.AcknowledgedTimestamp;
									originalAlarmStatus.AcknowledgedBy = newAlarmStatus.AcknowledgedBy;
									originalAlarmStatus.UpdatedBy = newAlarmStatus.UpdatedBy;
									originalAlarmStatus.UpdatedDate = newAlarmStatus.UpdatedDate;
									originalAlarmStatus.WrittenToEnterprise = false;
									ret = true;
								}

								if (originalAlarmStatus.Silenced != newAlarmStatus.Silenced
								&& newAlarmStatus.SilencedTimestamp.HasValue)
								{
									originalAlarmStatus.Silenced = newAlarmStatus.Silenced;
									originalAlarmStatus.SilencedTimestamp = newAlarmStatus.SilencedTimestamp;
									originalAlarmStatus.SilencedBy = newAlarmStatus.SilencedBy;
									originalAlarmStatus.UpdatedBy = newAlarmStatus.UpdatedBy;
									originalAlarmStatus.UpdatedDate = newAlarmStatus.UpdatedDate;
									originalAlarmStatus.WrittenToEnterprise = false;
									ret = true;
								}

								if (originalAlarmStatus.AlarmTestInTimedHoldOff != newAlarmStatus.AlarmTestInTimedHoldOff)
								{
									originalAlarmStatus.AlarmTestInTimedHoldOff = newAlarmStatus.AlarmTestInTimedHoldOff;
									originalAlarmStatus.AlarmTestInTimedHoldOffTimestamp = newAlarmStatus.AlarmTestInTimedHoldOffTimestamp;
									originalAlarmStatus.UpdatedBy = newAlarmStatus.UpdatedBy;
									originalAlarmStatus.UpdatedDate = newAlarmStatus.UpdatedDate;
								}
							}
						}
					}
				}
			}


			if (ret)
			{
				originalTag.UpdatedDate = newTag.UpdatedDate;
				originalTag.UpdatedBy = newTag.UpdatedBy;
				originalTag.ServerTimeStamp = newTag.ServerTimeStamp;
				originalTag.WrittenToEnterprise = false;
			}


			return ret;
		}

		public bool SetShelvedIfChanged(PointTag originalTag, PointTag newTag)
		{
			bool ret = false;
			if (newTag.Alarms.Any())
			{
				foreach (var newAlarm in newTag.Alarms.Values)
				{
					Alarm originalAlarm;
					if (originalTag.Alarms.TryGetValue(newAlarm.IdentityGuid, out originalAlarm))
					{
						if (originalAlarm.ShelvedOneShot != newAlarm.ShelvedOneShot || originalAlarm.ShelvedStartTimeStamp != newAlarm.ShelvedStartTimeStamp || originalAlarm.ShelvedEndTimeStamp != newAlarm.ShelvedEndTimeStamp)
						{
							ret = true;
							originalAlarm.ShelvedOneShot = newAlarm.ShelvedOneShot;
							originalAlarm.ShelvedStartTimeStamp = newAlarm.ShelvedStartTimeStamp;
							originalAlarm.ShelvedEndTimeStamp = newAlarm.ShelvedEndTimeStamp;
							originalAlarm.ShelvedBy = newAlarm.ShelvedBy;
							originalAlarm.UpdatedBy = newAlarm.UpdatedBy;
							originalAlarm.UpdatedDate = newAlarm.UpdatedDate;
						}
					}

				}
			}
			if (ret)
			{
				originalTag.UpdatedDate = newTag.UpdatedDate;
				originalTag.UpdatedBy = newTag.UpdatedBy;
			}
			return ret;
		}

		public bool SetAcknowledgedAndSilencedIfChanged(Point point, PointTag originalTag, PointTag newTag, List<PointTagAlarmStatus> acknowledgedAlarms, List<AandEDataElement> aandEDataElements, string comment = "")
		{
			bool ret = false;
			if (newTag.Alarms.Any())
			{
				foreach (var newAlarm in newTag.Alarms.Values)
				{
					Alarm originalAlarm;
					if (originalTag.Alarms.TryGetValue(newAlarm.IdentityGuid, out originalAlarm))
					{
						foreach (var newAlarmStatus in newAlarm.AlarmStatus.Values)
						{
							AlarmTest originalalarmTest;
							if (originalAlarm.AlarmTests.TryGetValue(newAlarmStatus.AlarmTestGuid, out originalalarmTest))
							{
								PointTagAlarmStatus originalAlarmStatus;
								if (originalAlarm.AlarmStatus.TryGetValue(newAlarmStatus.IdentityGuid, out originalAlarmStatus))
								{
									// Apply changes only if Alarm Test Failed Matches
									if (originalAlarmStatus.AlarmTestFailed == newAlarmStatus.AlarmTestFailed
									&& originalAlarmStatus.AlarmTestFailedTimestamp == newAlarmStatus.AlarmTestFailedTimestamp)
									{

										if (originalAlarmStatus.Acknowledged != newAlarmStatus.Acknowledged)
										{
											originalAlarmStatus.Acknowledged = newAlarmStatus.Acknowledged;
											originalAlarmStatus.AcknowledgedTimestamp = newAlarmStatus.AcknowledgedTimestamp;
											originalAlarmStatus.AcknowledgedBy = newAlarmStatus.AcknowledgedBy;
											originalAlarmStatus.UpdatedBy = newAlarmStatus.UpdatedBy;
											originalAlarmStatus.UpdatedDate = newAlarmStatus.UpdatedDate;
											originalAlarmStatus.AcknowledgedComment = comment;
											originalAlarmStatus.WrittenToEnterprise = false;

											acknowledgedAlarms.Add(originalAlarmStatus);
											aandEDataElements.Add(new AandEDataElement(originalAlarmStatus, point, newTag, originalAlarm, originalalarmTest, true));

											ret = true;
										}

										if (originalAlarmStatus.Silenced != newAlarmStatus.Silenced)
										{
											originalAlarmStatus.Silenced = newAlarmStatus.Silenced;
											originalAlarmStatus.SilencedTimestamp = newAlarmStatus.SilencedTimestamp;
											originalAlarmStatus.SilencedBy = newAlarmStatus.SilencedBy;
											originalAlarmStatus.UpdatedBy = newAlarmStatus.UpdatedBy;
											originalAlarmStatus.UpdatedDate = newAlarmStatus.UpdatedDate;
											originalAlarmStatus.WrittenToEnterprise = false;
											ret = true;
										}
									}
								}
							}
						}
					}
				}
			}

			if (ret)
			{
				originalTag.UpdatedDate = newTag.UpdatedDate;
				originalTag.UpdatedBy = newTag.UpdatedBy;
				originalTag.ServerTimeStamp = newTag.ServerTimeStamp;
			}

			return ret;
		}

		public bool SetAlarmIfChanged(PointTag originalTag, PointTag newTag)
		{
			bool ret = false;

			originalTag.HighestPriorityAlarm = newTag.HighestPriorityAlarm;
			originalTag.HighestOrderAlarmTest = newTag.HighestOrderAlarmTest;
			originalTag.HighestOrderPointTagAlarmStatus = newTag.HighestOrderPointTagAlarmStatus;

         if (originalTag.Acknowledged != newTag.Acknowledged
			|| originalTag.AlarmPriorityGuid != newTag.AlarmPriorityGuid
			|| (originalTag.AlarmState == null && newTag.AlarmState != null)
			|| (originalTag.AlarmState != null && originalTag.AlarmState.Equals(newTag.AlarmState) == false))
			{
            originalTag.Acknowledged = newTag.Acknowledged;
				originalTag.AlarmPriorityGuid = newTag.AlarmPriorityGuid;
				originalTag.AlarmState = newTag.AlarmState;
				originalTag.ServerTimeStamp = newTag.ServerTimeStamp;
				originalTag.SourceTimeStamp = newTag.SourceTimeStamp;
				ret = true;
			}

			return ret;
		}

		public void SetValuesIfChanged(PointTag tagToUpdate, PointTag tagDataSource, ref bool valueChanged, ref bool statusChanged)
		{
			tagToUpdate.ThrowIfNull("originalTag");
			tagDataSource.ThrowIfNull("newTag");

			valueChanged = false;
			statusChanged = false;


			// Perform Range Check First as this causes Status Change, Particularly Info Type bit's 10:11 is set to 01 = DataValue
			ThreadSharedData.RangeCheck(tagDataSource);

			if ((tagToUpdate.Value == null && tagDataSource.Value != null)
			|| (tagToUpdate.Value != null && tagToUpdate.Value.Equals(tagDataSource.Value) == false))
			{
				valueChanged = true;
			}

			if(tagToUpdate.Status != tagDataSource.Status)
			{
				statusChanged = true;
			}

			if(valueChanged || statusChanged)
			{
				// On clearing an override for an opc input, refresh from the source.
				if (tagToUpdate.OpcStatusCodeBits == StatusCodes.GoodLocalOverride
				&& tagDataSource.OpcStatusCodeBits != StatusCodes.GoodLocalOverride
				&& tagToUpdate.InputOutputType == PointTemplateTag.PointTagInputOutputType.OpcUa)
				{
					if (ThreadSharedData.Instance().UseOpcUaClientPolling)
					{
						OpcUaClientProcessor2.Instance().RefreshPointTag(tagToUpdate, tagDataSource);
					}
					else
					{
						OpcUaClientProcessor.Instance().RefreshPointTag(tagToUpdate, tagDataSource);
					}

               //update the reference list for PCS tags is still necessary
               if (tagDataSource.Value is PointCommandStatusListReference)
               {
                   tagToUpdate.ValueXml = tagDataSource.ValueXml;
               }

					// Range Check again as the OriginalTag has been set to a refresh from the OpcUaClient
               ThreadSharedData.RangeCheck(tagToUpdate);
				}
				else
				{
					if (tagDataSource.Value is ValueType)
					{
						tagToUpdate.Value = tagDataSource.Value;
					}
					else
					{
						tagToUpdate.ValueXml = tagDataSource.ValueXml;
					}

					tagToUpdate.Status = tagDataSource.Status;
					tagToUpdate.ServerTimeStamp = tagDataSource.ServerTimeStamp;
					tagToUpdate.SourceTimeStamp = tagDataSource.SourceTimeStamp;
				}

				tagToUpdate.WrittenToEnterprise = false;
			}
		}
	}
}
