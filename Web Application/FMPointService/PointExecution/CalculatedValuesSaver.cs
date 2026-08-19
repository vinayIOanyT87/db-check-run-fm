namespace FMPointService.PointExecution
{

	using System.Linq;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using ThreadSupport;
	using Archiving;

	using FMCore;

	using OpcClient;
	using Opc.Ua;
    using global::FMPointService.AlarmAndEventArchive;

    internal class CalculatedValuesSaver
	{
		public ArchiveRecordQueuer ArchiveRecordQueuer = new ArchiveRecordQueuer();

		public static void UpdateOpcOutputTagWithoutOverride(PointTag pointTag)
		{
			if (ThreadSharedData.Instance().UseOpcUaClientPolling)
			{
				OpcUaClientProcessor2.Instance().UpdateOpcOutputTagWithoutOverride(pointTag);
			}
			else
			{
				OpcUaClientProcessor.Instance().UpdateOpcOutputTagWithoutOverride(pointTag);
			}
		}

		public static void UpdateOpcOutputTagWithOverride(PointTag pointTag)
		{
			if (ThreadSharedData.Instance().UseOpcUaClientPolling)
			{
				OpcUaClientProcessor2.Instance().UpdateOpcOutputTagWithOverride(pointTag);
			}
			else
			{
				OpcUaClientProcessor.Instance().UpdateOpcOutputTagWithOverride(pointTag);
			}
		}




		/// <summary>
		/// Saves the tags that have changed and queues them for archiving
		/// </summary>
		/// <param name="point">A point with tags that may need saving.</param>
		public void SaveChangedPointTags(Point point)
		{
			point.ThrowIfNull("point");

			var threadSharedData = ThreadSharedData.Instance();

			var queuePointForProcessing = false;

			// It is necessary to allow Modules to set Manual Values.  An example of this is the Tank Command, when Reset is issued,
			// the Tank Command Module must set the Tank Command back to it's prior value.
			point.Tags.Values.ToList().ForEach(
				tag =>
				{
					bool alarmChanged = false;
					bool valueChanged = false;
					bool statusChanged = false;

					if (tag.Value is FMBusinessObjects.DataObjects.PointCommandStatusListReference)
					{
						var pointCommandStatusListReference = tag.Value as PointCommandStatusListReference;
						if (pointCommandStatusListReference.CurrentValue.HasValue)
						{
							pointCommandStatusListReference.CurrentKey = ThreadSharedData.Instance().GetPointCommandStatusKey(tag.PointGuid, pointCommandStatusListReference.PointCommandStatusListGuid, pointCommandStatusListReference.CurrentValue.Value);
						}
						else
						{
							pointCommandStatusListReference.CurrentKey = string.Empty;
						}
					}

					if (tag.Alarms.Any())
					{
						alarmChanged = threadSharedData.SetPointTagAlarmIfChanged(tag);
					}


					// Update all Calculated Tags that are not overriden unless value is null and Manual Output Tags that are not Output to Opc Ua Server.
					if (!ThreadSharedData.IsPointTagToBeOutput(tag, false)
					&& ((tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
					&& (tag.OpcStatusCodeBits != StatusCodes.GoodLocalOverride
					|| tag.Value == null))
					|| (tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual && !tag.Input)))
					{
						threadSharedData.SetPointTagValueIfChanged(tag, true, ref valueChanged, ref statusChanged, ref alarmChanged);
					}
					else
					{
						if (ThreadSharedData.IsPointTagToBeOutput(tag, true, PointTemplateTag.PointTagInputOutputType.Calculated))
						{
							UpdateOpcOutputTagWithoutOverride(tag);
						}

						if (tag.Alarms.Any())
						{
							threadSharedData.SetPointTagAlarmAndAlarmStatusIfChanged(tag);
						}
					}

					if (alarmChanged)
					{
						queuePointForProcessing = true;
					}


					if (tag.Archived
					&& (valueChanged || alarmChanged || statusChanged))
					{
						// Only queue the tag for archiving if there were changes saved.
						this.ArchiveRecordQueuer.CreateAndQueueArchiveRecord(new PointValue(tag), false, false, alarmChanged || statusChanged);
					}

					if (valueChanged || statusChanged)
					{
						AlarmAndEventArchiveThread.Instance().LogEventsToArchive(new PointValue(tag));
					}

				});

			if (queuePointForProcessing)
			{
				threadSharedData.PointExecutionQueuer.QueuePointForProcessing(point.PointGuid);
			}
		}
	}
}
