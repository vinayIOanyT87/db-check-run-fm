// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointService.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Service to provide a way to get and set point tag data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMPointService.WcfPointService
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;
	using FMPointCommon;
	using OpcClient;
	using PointExecution;
	using ThreadSupport;
	using Logging;
	using Archiving;
	using FMCore;
	using Opc.Ua;
	using global::FMPointService.AlarmAndEventArchive;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.BusinessInterfaces;

    internal class PointService : IPointService
	{
		public static readonly PointExecutionQueuer PointExecutionQueuer = new PointExecutionQueuer();
		public static readonly PointValueChangePersister PointValueChangePersister = new PointValueChangePersister();
		public static readonly StatisticsLogger StatisticsLogger = new StatisticsLogger();
		public static readonly ArchiveRecordQueuer ArchiveRecordQueuer = new ArchiveRecordQueuer();

		public static readonly CalculationEngine CalculationEngine = new CalculationEngine();

		/// <summary>
		/// Point Changed.
		/// </summary>
		/// <param name="security">A FuelsManager security object.</param>
		public void SignalPointChanged(SecurityClass security)
		{
			security.ThrowIfNull("security");

			PingProcessor.Instance().SignalPointChanged();
		}


		/// <summary>
		/// Gets the point tags that correspond to the pass list of point tag guids.
		/// </summary>
		/// <param name="security">A FuelsManager security object.</param>
		/// <param name="pointTagGuids">A list of point tag guids identifying the tag to get.</param>
		/// <returns>A list of <see cref="PointTag"></see>PointTag objects.</returns>
		public List<PointTag> GetPointTagData(SecurityClass security, List<Guid> pointTagGuids)
		{
			security.ThrowIfNull( "security" );
			pointTagGuids.ThrowIfNull( "pointTagGuids" );

			var threadSharedData = ThreadSharedData.Instance();

			return threadSharedData.GetPointTags(pointTagGuids);
		}


		/// <summary>
		/// Gets the point values that correspond to the list of point value identifiers.
		/// </summary>
		/// <param name="security">A FuelsManager security object.</param>
		/// <param name="pointValueIdenfierList">A list of point value identifiers to get.</param>
		/// <returns>A list of <see cref="PointTag"></see>PointTag objects.</returns>
		public List<PointValue> GetPointValueData(SecurityClass security, List<PointValueIdentifier> pointValueIdentifierList)
		{
			security.ThrowIfNull("security");
			pointValueIdentifierList.ThrowIfNull("pointValueIdentifierList");


			var threadSharedData = ThreadSharedData.Instance();

			return threadSharedData.GetPointValues(pointValueIdentifierList);
		}

		/// <summary>
		/// Gets the point value changes that correspond to the list of point value identifiers.
		/// </summary>
		/// <param name="security">A FuelsManager security object.</param>
		/// <param name="pointValueIdenfierList">A list of point value identifiers to get.</param>
		/// <returns>A list of <see cref="PointTag"></see>PointTag objects.</returns>
		public List<PointValue> GetPointValueDataChanges(SecurityClass security, List<PointValueIdentifier> pointValueIdentifierList)
		{
			security.ThrowIfNull("security");
			pointValueIdentifierList.ThrowIfNull("pointValueIdentifierList");

			var threadSharedData = ThreadSharedData.Instance();

			return threadSharedData.GetPointValueChanges(pointValueIdentifierList);
		}

		public void ResetStatistics(SecurityClass security)
		{
			security.ThrowIfNull("security");

			StatisticsLogger.ResetStatistics();
		}

		public void SetAcknowledgeAndSilence(SecurityClass security, List<PointTag> pointTags, DateTimeOffset? timestamp = null, string comment = "")
		{
			security.ThrowIfNull("security");
			pointTags.ThrowIfNull("pointTags");

			if (pointTags.Count == 0)
			{
				return;
			}

			if (timestamp == null)
			{
				timestamp = DateTimeOffset.UtcNow;
			}

			var threadSharedData = ThreadSharedData.Instance();

			var acknowledgedAlarms = new List<PointTagAlarmStatus>();
			var aandEDataElements = new List<AandEDataElement>();


			foreach (var pointTag in pointTags)
			{
				pointTag.UpdatedBy = security.UserID;
				pointTag.UpdatedDate = (DateTimeOffset)timestamp;
				pointTag.ServerTimeStamp = (DateTimeOffset)timestamp;

				if (threadSharedData.SetAcknowledgedAndSilencedIfChanged(pointTag, acknowledgedAlarms, aandEDataElements, comment))
				{
					PointExecutionQueuer.QueuePointForProcessing(pointTag.PointGuid);
				}
			}


			// set AcknowledgedTimestamp and SilencedTimestamp for each pt alarm status to use the identical timestamp as Tag ServerTimeStamp
			if (acknowledgedAlarms.Any())
			{
				FMChannelHelper.MakeCall<IPointTagAlarmStatuses>(x => x.Acknowledge(security, acknowledgedAlarms));
			}

			if (aandEDataElements.Any())
			{
				FMChannelHelper.MakeCall<IAandEArchive>(x => x.AddArchiveData(security, aandEDataElements));
			}
		}

		public void SetShelve(SecurityClass security, List<PointTag> pointTags)
		{
			security.ThrowIfNull("security");
			pointTags.ThrowIfNull("pointTags");

			if (pointTags.Count == 0)
			{
				return;
			}

			var threadSharedData = ThreadSharedData.Instance();


			foreach (var pointTag in pointTags)
			{
				pointTag.UpdatedBy = security.UserID;
				pointTag.UpdatedDate = DateTimeOffset.UtcNow;

				if (threadSharedData.SetShelvedIfChanged(pointTag))
				{
					PointExecutionQueuer.QueuePointForProcessing(pointTag.PointGuid);
				}
			}
		}

		/// <summary>
		/// Updates the point tags with the tags in the passed list of point tags.
		/// </summary>
		/// <param name="security">A FuelsManager security object.</param>
		/// <param name="pointTagList">A list of tags to replace in the service.</param>
		public void SetPointTagData(SecurityClass security, List<PointTag> pointTagList)
		{
			security.ThrowIfNull("security");
			pointTagList.ThrowIfNull("pointTagList");

			if (pointTagList.Count == 0)
			{
				return;
			}

			var threadSharedData = ThreadSharedData.Instance();

			foreach (var pointTag in pointTagList)
			{
				var updatePCSListReference = false;
				if (pointTag.Value is PointCommandStatusListReference)
				{
					var currentPointTag = threadSharedData.GetPointTag(pointTag.IdentityGuid);
					if (currentPointTag == null)
					{
						continue;
					}

					if (currentPointTag.Value == null
					|| !(currentPointTag.Value is PointCommandStatusListReference)
					|| (pointTag.Value as PointCommandStatusListReference).PointCommandStatusListGuid != (currentPointTag.Value as PointCommandStatusListReference).PointCommandStatusListGuid)
					{
						updatePCSListReference = true;
					}
				}

				if (!pointTag.Input
				&& ThreadSharedData.IsPointTagToBeOutput(pointTag, false))
				{
					this.OutputOpcCommand(security, pointTag);
					if (pointTag.OpcUaIsReadable
					&& !pointTag.IsBad()
					&& !updatePCSListReference)
					{
						continue;
					}
				}

				bool valueChanged = false;
				bool statusChanged = false;
				bool alarmChanged = false;
				threadSharedData.SetPointTagValueIfChanged(pointTag, false, ref valueChanged, ref statusChanged, ref alarmChanged);
				if(valueChanged || statusChanged)
				{
					if (pointTag.Archived)
					{
						ArchiveRecordQueuer.CreateAndQueueArchiveRecord(new PointValue(pointTag), false, false, statusChanged);
					}
					
					AlarmAndEventArchiveThread.Instance().LogEventsToArchive(new PointValue(pointTag));
					
					PointExecutionQueuer.QueuePointForProcessing(pointTag.PointGuid);
				}
			}
		}


		/// <summary>
		/// Updates the point tags with the tags in the passed list of point tags.
		/// </summary>
		/// <param name="security">A FuelsManager security object.</param>
		/// <param name="pointValues">A list of tags to replace in the service.</param>
		public void SetPointValueData(SecurityClass security, List<PointValue> pointValues)
		{
			security.ThrowIfNull("security");
			pointValues.ThrowIfNull("pointValues");

			if (pointValues.Count == 0)
			{
				return;
			}

			var threadSharedData = ThreadSharedData.Instance();

			foreach (var pointValue in pointValues)
			{
				if (pointValue.PointValueIdentifier.PointValueType == PointValueType.Tag)
				{
					var pointTag = threadSharedData.GetPointTag(pointValue.PointValueIdentifier.IdentityGuid);
					if(pointTag == null)
					{
						continue;
					}

					pointTag.Value = pointValue.Value;

					var clearOverride = (pointTag.OpcStatusCodeBits == StatusCodes.GoodLocalOverride && pointValue.OpcStatusCodeBits != StatusCodes.GoodLocalOverride) ? true : false;

					pointTag.Status = pointValue.Status;
					pointTag.ServerTimeStamp = pointValue.ServerTimeStamp;
					pointTag.SourceTimeStamp = pointValue.SourceTimeStamp;

					if (!pointTag.Input
					&& ThreadSharedData.IsPointTagToBeOutput(pointTag, false))
					{
						this.OutputOpcCommand(security, pointTag);
						if (pointTag.OpcUaIsReadable
						&& !clearOverride
						&& pointTag.OpcStatusCodeBits != StatusCodes.GoodLocalOverride
						&& !pointTag.IsBad())
						{
							continue;
						}
					}

					if (pointTag.Value != null
					&& pointTag.Value.GetType() == typeof(PointCommandStatusListReference))
					{
						if ((pointTag.Value as PointCommandStatusListReference).CurrentValue.HasValue)
						{
							int intValue = (pointTag.Value as PointCommandStatusListReference).CurrentValue.Value;
							string keyValue = ThreadSharedData.Instance().GetPointCommandStatusKey(pointTag.PointGuid, (pointTag.Value as PointCommandStatusListReference).PointCommandStatusListGuid, intValue);
							(pointTag.Value as PointCommandStatusListReference).CurrentKey = keyValue;
						}
						else
						{
							(pointTag.Value as PointCommandStatusListReference).CurrentKey = string.Empty;
						}
					}

					bool valueChanged = false;
					bool statusChanged = false;
					bool alarmChanged = false;



					// pointValues of type FCEE and with matching timestamps are forwarded
					// and should only be archived.
					//

					threadSharedData.SetPointTagValueIfChanged(pointTag, false, ref valueChanged, ref statusChanged, ref alarmChanged);


					if(valueChanged || statusChanged)
					{
						if (pointTag.Archived)
						{
							ArchiveRecordQueuer.CreateAndQueueArchiveRecord(new PointValue(pointTag), false, false, statusChanged);
						}

						AlarmAndEventArchiveThread.Instance().LogEventsToArchive(new PointValue(pointTag));

						PointExecutionQueuer.QueuePointForProcessing(pointTag.PointGuid);
					}
				}

				else if(pointValue.PointValueIdentifier.PointValueType == PointValueType.Setting)
				{
					var pointProperty = threadSharedData.GetPointProperty(pointValue.PointValueIdentifier.IdentityGuid);
					if(pointProperty == null)
					{
						continue;
					}

					if(pointProperty.ID == "Movement Data")
					{
						MovementProcessor.Instance().SetMovementData(pointValue);
						continue;
					}

					var point = threadSharedData.GetPoint(pointProperty.PointGuid);
					if(point == null)
					{
						continue;
					}

					var oldPointValues = pointProperty.GetExposedSettings(point);

					var propertyType = pointProperty.Value.GetType();
					var propertyInfo = propertyType.GetProperty(pointValue.PointValueIdentifier.PropertyID);
					if (propertyInfo == null)
					{
						throw new Exception("No such property : " + pointValue.PointValueIdentifier.PropertyID);
					}

					var valueTypeString = propertyInfo.PropertyType.ToString();
					if (valueTypeString == typeof(PointPropertyUnitTypedDouble).ToString())
					{
						var value = propertyInfo.GetValue(pointProperty.Value);
						(value as PointPropertyUnitTypedDouble).Value = (double)pointValue.Value;
						propertyInfo.SetValue(pointProperty.Value, value);
					}
					else
					{
						propertyInfo.SetValue(pointProperty.Value, pointValue.Value);
					}

					pointProperty.UpdatedBy = security.UserID;
					pointProperty.UpdatedDate = DateTimeOffset.Now;

					threadSharedData.SetPointProperty(pointProperty);

					var newPointValues = pointProperty.GetExposedSettings(point);


					PointExecutionQueuer.QueuePointForProcessing(pointProperty.PointGuid);

					var index = 0;
					foreach (var newPointValue in newPointValues)
					{
						var oldPointValue = oldPointValues[index++];
						if ((newPointValue.Value == null && oldPointValue.Value != null)
						|| (newPointValue.Value != null && oldPointValue.Value == null)
						|| (newPointValue.Value != null && !newPointValue.Value.Equals(oldPointValue.Value)))
						{
							ArchiveRecordQueuer.CreateAndQueueArchiveRecord(newPointValue, false, false, false);
						}
					}
				}
			}
		}


		public List<Statistic> GetStatistics(SecurityClass security)
		{
			security.ThrowIfNull("security");

			return StatisticsLogger.GetStatistics();
		}


		public void ExecuteAsyncMethods(SecurityClass security, List<AsyncMethodCallClass> methods)
		{
			var pointDictionary = ThreadSharedData.Instance().GetPointDictionary(false);
			var tagResults = new Dictionary<Guid, PointTag>();
			foreach (var methodRequest in methods)
			{
				var siteGuid = methodRequest.SiteGuid;
				var pointGuid = methodRequest.PointGuid;


				Point point = null;

				if(pointDictionary.TryGetValue(pointGuid, out point))
				{
					CalculationEngine.AsyncMethodInvoke(point, methodRequest.ModuleCalculationGuid, methodRequest.Parameters, security, ref tagResults);
				}
			}
			ThreadSharedData.Instance().ExternalUpdateTags(tagResults);
		}

		public void OutputOpcCommand(SecurityClass security, PointTag pointTag)
		{
			if (ThreadSharedData.Instance().UseOpcUaClientPolling)
			{
				OpcUaClientProcessor2.Instance().OutputOpcCommand(security, pointTag);
			}
			else
			{
				OpcUaClientProcessor.Instance().OutputOpcCommand(security, pointTag);
			}
		}
		public PointCalculatorData RunPointCalculator(SecurityClass security, Guid pointGuid, PointCalculatorData pointCalculatorData)
		{
			security.ThrowIfNull("security");
			pointGuid.ThrowIfNull("pointGuid");
            pointCalculatorData.ThrowIfNull("pointCalculatorData");

			// this is called from pointservicemanager
			// created the threadshared object and call the calculator routine

			var threadSharedData = ThreadSharedData.Instance();
            pointCalculatorData = threadSharedData.ExecutePointsCalculator(security, pointGuid, pointCalculatorData);

			return pointCalculatorData;
		}
        public List<PointTag> RunPointCalculatorX(SecurityClass security, Guid pointGuid, List<PointTag> pointTags)
        {
            security.ThrowIfNull("security");
            pointGuid.ThrowIfNull("pointGuid");
            pointTags.ThrowIfNull("pointTags");

            // this is called from pointservicemanager
            // created the threadshared object and call the calculator routine

            var threadSharedData = ThreadSharedData.Instance();
            pointTags = threadSharedData.ExecutePointsCalculator(security, pointGuid, pointTags);

            return pointTags;
        }
    }
}
