namespace FMPointService.PointExecution
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using InProcLogging;

	using Opc.Ua;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using FMBusinessObjects.Interfaces;
	using ThreadSupport;
	using Archiving;
	using Logging;

	public class MovementContainer
	{
		public Point MovementPoint { get; set; }

		public bool Complete { get; set; }

		public MovementContainer(Point movementPoint)
		{
			this.MovementPoint = movementPoint;
		}
	}

	public class MovementProcessor : SrmThread
	{

		private static MovementProcessor inst = null;

		private readonly AutoResetEvent pointChangeEvent = new AutoResetEvent(false);

		private Dictionary<Guid, MovementContainer> movementContainerDictionary = new Dictionary<Guid, MovementContainer>();

		private Dictionary<Guid, List<PointValueIdentifier>> movementDataPointValueListDictionary = new Dictionary<Guid, List<PointValueIdentifier>>();

		private SecurityClass security;

		private static readonly PointExecutionQueuer PointExecutionQueuer = new PointExecutionQueuer();

		private static readonly ArchiveRecordQueuer ArchiveRecordQueuer = new ArchiveRecordQueuer();

		private StatisticsLogger StatisticsLogger = new StatisticsLogger();

		private DateTimeOffset? LastCheckHistoryWriteTimeUtc = null;

		Dictionary<Guid, string> SiteTimeZoneDictionary = new Dictionary<Guid, String>();

		private static int processIntervalSecs = 1;

		private static int updateInactiveMovementsEvery = 10;

		private static int iterationCount = 0;

		List<Guid> SiteList = new List<Guid>();

		public static MovementProcessor Instance()
		{
			if (inst == null)
			{
				inst = new MovementProcessor();
			}
			return inst;
		}

		protected object LockObject = new object();


		/// <summary>
		/// Initial Movement Processor
		/// </summary>
		protected void Initialize()
		{
			var threadSharedData = ThreadSharedData.Instance();
			this.security = threadSharedData.Login("SiteAdmin");
			processIntervalSecs = threadSharedData.MovementResolutionInSeconds;
			updateInactiveMovementsEvery = threadSharedData.UpdateInactiveMovementsEveryXIterations;

		}

		protected bool IsStatusChange(long oldStatus, long newStatus)
		{
			return (new StatusCode((uint)oldStatus).CodeBits != new StatusCode((uint)newStatus).CodeBits) ? true : false;
		}



		public void SignalPointChanges()
		{
			pointChangeEvent.Set();
		}

		

		private bool IsFlowNonZero(object flowItem)
		{
			if (flowItem == null)
			{
				return false;
			}

			if (!(flowItem is PointValue))
			{
				return false;
			}

			if ((flowItem as PointValue).IsBad())
			{
				return false;
			}

			if (!((flowItem as PointValue).Value is double))
			{
				return false;
			}

			if ((double)(flowItem as PointValue).Value == 0.0)
			{
				return false;
			}

			return true;
		}

		// Clone the MovementData and update it to reflect proper status for shutdown of FMPointService
		public void SaveMovementData(PointProperty movementDataProperty)
		{
			PointProperty movementDataPropertyClone = (PointProperty)movementDataProperty.Clone();
			Type movementDataType = typeof(MovementData);
			movementDataPropertyClone.ValueXml = movementDataProperty.ValueXml;
			IList<PropertyInfo> propertyInfoList = new List<PropertyInfo>(movementDataType.GetProperties());

			foreach (PropertyInfo propertyInfo in propertyInfoList)
			{

				object pointValueListClone = propertyInfo.GetValue(movementDataPropertyClone.Value, null);

				// Skip properties that are persisted by modules
				if (propertyInfo.Name == "Status"
				|| propertyInfo.Name == "TransferStatus"
				|| propertyInfo.Name == "TransferStartTime"
				|| propertyInfo.Name == "TransferStopTime"
				|| propertyInfo.Name == "InitiationCount"
				|| propertyInfo.Name == "TransferStartLevel"
				|| propertyInfo.Name == "TransferStartGOV"
				|| propertyInfo.Name == "TransferStartNSV"
				|| propertyInfo.Name == "TransferStartWaterVolume"
				|| propertyInfo.Name == "TransferStartVolume"
				|| propertyInfo.Name == "TransferMode"
				|| propertyInfo.Name == "TransferTimeComopletion"
				|| propertyInfo.Name == "StartTemperatureAmbient"
				|| propertyInfo.Name.StartsWith("Start")
				|| propertyInfo.Name.StartsWith("Opening"))
				{
					continue;
				}

				if (pointValueListClone is List<PointValue>)
				{

					foreach (var pointValueClone in pointValueListClone as List<PointValue>)
					{

						if (pointValueClone == null)
						{
							continue;
						}

						if ((pointValueClone.InputOutputType == PointTemplateTag.PointTagInputOutputType.OpcUa
						|| pointValueClone.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated)
						&& pointValueClone.OpcStatusCodeBits != StatusCodes.GoodLocalOverride)
						{
							pointValueClone.Value = null;

							// Calculated Values that are default Status Good calculated by Transfer Module
							if (propertyInfo.Name == "TransferStartGOV"
							|| propertyInfo.Name == "TransferStartLevel"
							|| propertyInfo.Name == "TransferStartNSV"
							|| propertyInfo.Name == "TransferStartVolume"
							|| propertyInfo.Name == "TransferStartWaterVolume"
							|| propertyInfo.Name == "TransferStatus"
							|| propertyInfo.Name == "TransferredGOV"
							|| propertyInfo.Name == "TransferredNSV"
							|| propertyInfo.Name == "TransferStartTime"
							|| propertyInfo.Name == "TransferStopTime"
							|| propertyInfo.Name == "TransferTimeRemaining"
							|| propertyInfo.Name == "TransferTimeCompletion"
							|| propertyInfo.Name == "TransferTarget"
							|| propertyInfo.Name == "TransferLevelTarget"
							|| propertyInfo.Name == "TransferVolumeTarget")
							{
								pointValueClone.Status = StatusCodes.Good;
							}
							else
							{
								pointValueClone.Status = StatusCodes.Bad;
							}
						}
					}
				}
			}

			movementDataPropertyClone.UpdatedDate = DateTimeOffset.Now;
			FMChannelHelper.MakeCall<IPointProperties>(x => x.ModifyPointPropertyValue(this.security, movementDataPropertyClone, true, true));
		}

		public void SetMovementData(PointValue pointValue)
		{
			lock (this.LockObject)
			{
				MovementContainer movementContainer;

				this.movementContainerDictionary.TryGetValue(pointValue.PointGuid, out movementContainer);

				if (movementContainer == null)
				{
					throw new Exception("No such Movement : " + pointValue.PointGuid.ToString());
				}

				var movementDataPointProperty = movementContainer.MovementPoint.Properties.Values.Single(u => u.ValueTypeString == "FMBusinessObjects.DataObjects.MovementData");
				var movementData = movementDataPointProperty.Value as MovementData;

				var pointValueList = pointValue.Value as List<PointValue>;
				if (pointValueList == null)
				{
					throw new Exception("Improper PointValue PropertyID: " + pointValue.PointValueIdentifier.PropertyID);
				}



				Type movementDataType = typeof(MovementData);
				var propertyInfo = movementDataType.GetProperty(pointValue.PointValueIdentifier.PropertyID);

				if (propertyInfo == null)
				{
					throw new Exception("No such PointValue PropertyID: " + pointValue.PointValueIdentifier.PropertyID);
				}

				var propertyValue = propertyInfo.GetValue(movementData, null) as List<PointValue>;
				if (propertyValue == null)
				{
					throw new Exception("PointValue PropertyID is not Point Value List : " + pointValue.PointValueIdentifier.PropertyID);
				}

				foreach (var value in pointValueList)
				{
					var item = propertyValue.SingleOrDefault(x => x.PointGuid == value.PointGuid);
					if (item == null)
					{
						throw new Exception("Invalid Property Identitifier : " + value.PointValueIdentifier.IdentityGuid.ToString());
					}

					item.Value = value.Value;
					item.Status = value.Status;
					item.ServerTimeStamp = DateTimeOffset.UtcNow;
					item.SourceTimeStamp = value.SourceTimeStamp;
				}

				movementDataPointProperty.UpdatedDate = DateTimeOffset.Now;
				this.SaveMovementData(movementDataPointProperty);
				ThreadSharedData.Instance().ApplyPropertyChangesToMaster(movementContainer.MovementPoint);
			}
		}

		protected bool IsValueGood(PointValue valuetoCheck)
		{
			// simple routine to determine if the value is valid or not
			if (valuetoCheck == null
			|| valuetoCheck.Value == null
			|| StatusCode.IsBad(new StatusCode((uint)valuetoCheck.Status)))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		protected void WriteTagToMasterData(bool queuePoint, bool persist, PointTag pointTag)
		{
			bool valueChanged = false;
			bool statusChanged = false;
			bool alarmChanged = false;


			ThreadSharedData.Instance().SetPointTagValueIfChanged(pointTag, false, ref valueChanged, ref statusChanged, ref alarmChanged);
			if (valueChanged || statusChanged)
			{
				if (pointTag.Archived)
				{
					ArchiveRecordQueuer.CreateAndQueueArchiveRecord(new PointValue(pointTag), false, false, statusChanged);
				}

				if (queuePoint)
				{
					PointExecutionQueuer.QueuePointForProcessing(pointTag.PointGuid);
				}
			}

			if (persist)
			{
				var tagList = new List<PointTag>();
				tagList.Add(pointTag);
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(this.security, tagList, false));
			}
		}

		protected bool IsStatusUncertain(PointValue valueToCheck)
		{
			var tagstatusCode = new StatusCode((uint)valueToCheck.Status);

			if (tagstatusCode.LimitBits == LimitBits.High
			|| tagstatusCode.LimitBits == LimitBits.Low
			|| StatusCode.IsUncertain(tagstatusCode)
			|| tagstatusCode.SubCode == StatusCodes.GoodLocalOverride)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		protected void InitializeMovementData(Point movementPoint)
		{
			var movementSettingsPointProperty = movementPoint.Properties.Values.Single(u => u.ValueTypeString == "FMBusinessObjects.DataObjects.MovementModuleSettings");
			var movementSettings = movementSettingsPointProperty.Value as MovementModuleSettings;

			var movementData = new MovementData();
			var movementDataPointProperty = movementPoint.Properties.Values.Single(u => u.ValueTypeString == "FMBusinessObjects.DataObjects.MovementData");
			movementDataPointProperty.Value = movementData;


			movementData.TransferDirection.Add(null);
			movementData.IndividualNodeControl.Add(null);

			var pointIdValue = new PointValue()
			{
				ID = "PointId",
				ValueTypeString = typeof(System.String).FullName,
				Value = movementPoint.ID,
				Status = StatusCodes.Good,
				PointValueIdentifier = new PointValueIdentifier(movementPoint.PointGuid, PointValueType.Point, "PointId"),
				PointGuid = movementPoint.PointGuid
			};

			movementData.PointId.Add(pointIdValue);


			var commentValue = new PointValue()
			{
				ID = "Comment",
				ValueTypeString = typeof(System.String).FullName,
				Value = movementSettings.Comment,
				Status = StatusCodes.Good,
				PointValueIdentifier = new PointValueIdentifier(movementPoint.PointGuid, PointValueType.Setting, "Comment"),
				PointGuid = movementPoint.PointGuid
			};
			movementData.Comment.Add(commentValue);

			var orderNumberValue = new PointValue()
			{
				ID = "Order Number",
				ValueTypeString = typeof(System.String).FullName,
				Value = movementSettings.OrderNumber,
				Status = StatusCodes.Good,
				PointValueIdentifier = new PointValueIdentifier(movementPoint.PointGuid, PointValueType.Setting, "OrderNumber"),
				PointGuid = movementPoint.PointGuid
			};
			movementData.OrderNumber.Add(orderNumberValue);


			if (movementSettings.PlannedStartDateTime.HasValue)
			{
				var plannedStartTimeValue = new PointValue()
				{
					ID = "Planned Start Time",
					ValueTypeString = typeof(System.DateTimeOffset).FullName,
					Value = new DateTimeOffset(movementSettings.PlannedStartDateTime.Value.DateTime),
					Status = StatusCodes.Good,
					PointValueIdentifier = new PointValueIdentifier(movementPoint.PointGuid, PointValueType.Setting, "PlannedStartTime"),
					PointGuid = movementPoint.PointGuid,
				};
				movementData.PlannedStartTime.Add(plannedStartTimeValue);
			}

			var type = new PointValue()
			{
				ID = "Type",
				ValueTypeString = typeof(MovementType).FullName,
				Value = movementSettings.Type,
				Status = StatusCodes.Good,
				PointValueIdentifier = new PointValueIdentifier(movementPoint.PointGuid, PointValueType.Setting, "Type"),
				PointGuid = movementPoint.PointGuid,
			};
			movementData.Type.Add(type);

			foreach (var movementNodeData in movementSettings.MovementNodeDataList)
			{

				// The PointId drives the Summary
				pointIdValue = new PointValue()
				{
					ID = "PointId",
					ValueTypeString = typeof(System.String).FullName,
					Value = "",
					Status = StatusCodes.Good,
					PointValueIdentifier = new PointValueIdentifier(movementNodeData.MovementNodeGuid, PointValueType.Point, "PointId"),
					PointGuid = movementNodeData.MovementNodeGuid
				};

				movementData.PointId.Add(pointIdValue);


				var transferDirectionValue = new PointValue()
				{
					ID = "Direction",
					ValueTypeString = typeof(TransferDirection).FullName,
					Value = movementNodeData.TransferDirection,
					Status = StatusCodes.Good,
					PointValueIdentifier = new PointValueIdentifier(movementNodeData.MovementNodeGuid, PointValueType.Setting, "TransferDirection"),
					PointGuid = movementNodeData.MovementNodeGuid
				};
				movementData.TransferDirection.Add(transferDirectionValue);

				var individualNodeControl = new PointValue()
				{
					ID = "IndividualNodeControl",
					ValueTypeString = typeof(bool).FullName,
					Value = movementNodeData.IndividualNodeControl,
					Status = StatusCodes.Good,
					PointValueIdentifier = new PointValueIdentifier(movementNodeData.MovementNodeGuid, PointValueType.Setting, "IndividualNodeControl"),
					PointGuid = movementNodeData.MovementNodeGuid,
				};
				movementData.IndividualNodeControl.Add(individualNodeControl);
			}

			movementDataPointProperty.UpdatedDate = DateTimeOffset.Now;

			ThreadSharedData.Instance().ApplyPropertyChangesToMaster(movementPoint);

			this.SaveMovementData(movementDataPointProperty);
		}

		protected void UpdateTransferTimeRemaining(Point movementPoint, MovementData movementData, MovementModuleSettings movementSettings)
		{
			TimeSpan? transferTimeRemaining = null;
			var newStatus = StatusCodes.Good;


			var transferTimeRemainingList = movementData.TransferTimeRemaining;

			// First Entry in movementData is Movement
			var index = 1;

			foreach (var movementNode in movementSettings.MovementNodeDataList)
			{
				if (transferTimeRemainingList.Count > index
				&& transferTimeRemainingList[index] is PointValue
				&& (transferTimeRemainingList[index] as PointValue).Value is TimeSpan)
				{
					if (!transferTimeRemaining.HasValue
					|| transferTimeRemaining.Value > (TimeSpan)(transferTimeRemainingList[index] as PointValue).Value)
					{
						transferTimeRemaining = (TimeSpan)(transferTimeRemainingList[index] as PointValue).Value;
						newStatus = StatusCodes.Good;
					}
				}
			}

			var transferTimeRemainingTag = movementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.TransferTimeRemainingGuid);


			if (IsStatusChange(transferTimeRemainingTag.Status, newStatus)
			|| (newStatus != StatusCodes.Bad
			&& (!IsValueGood(new PointValue(transferTimeRemainingTag))
			|| (transferTimeRemaining == null
			|| transferTimeRemaining.Value != (TimeSpan)transferTimeRemainingTag.Value))))
			{
				transferTimeRemainingTag.Status = newStatus;
				if (newStatus != StatusCodes.Bad)
				{
					transferTimeRemainingTag.Value = transferTimeRemaining;
				}
				else
				{
					transferTimeRemainingTag.Value = null;
				}
				transferTimeRemainingTag.ServerTimeStamp = DateTimeOffset.UtcNow;
				transferTimeRemainingTag.SourceTimeStamp = transferTimeRemainingTag.ServerTimeStamp;

				this.WriteTagToMasterData(true, false, transferTimeRemainingTag);
			}
		}

		protected void UpdateDeviation(Point movementPoint, MovementData movementData, MovementModuleSettings movementSettings)
		{
			double? quantityIssuedGOVM3 = null;
			double? quantityReceivedGOVM3 = null;
			double? quantityIssuedNSVM3 = null;
			double? quantityReceivedNSVM3 = null;

			var newStatusDeviation = StatusCodes.Good;
			var newStatusGOV = StatusCodes.Good;
			var newStatusNSV = StatusCodes.Good;
			DateTimeOffset? newServerTimeStampGOV = null;
			DateTimeOffset? newServerTimeStampNSV = null;

			var transferredGOVList = movementData.TransferredGOV;
			var transferredNSVList = movementData.TransferredNSV;


			// First Entry in movementData is Movement
			var index = 1;

			foreach (var movementNode in movementSettings.MovementNodeDataList)
			{
				PointValue transferredGOV = null;
				if (transferredGOVList.Count > index)
				{
					transferredGOV = transferredGOVList[index];
				}

				PointValue transferredNSV = null;
				if (transferredNSVList.Count > index)
				{
					transferredNSV = transferredNSVList[index];
				}

				if (newStatusGOV == StatusCodes.Good
				&& transferredGOV != null
				&& IsStatusUncertain(transferredGOV))
				{
					newStatusGOV = StatusCodes.Uncertain;
				}


				if (newStatusNSV == StatusCodes.Good
				&& transferredNSV != null
				&& IsStatusUncertain(transferredNSV))
				{
					newStatusNSV = StatusCodes.Uncertain;
				}

				if (transferredGOV != null
				&& (!newServerTimeStampGOV.HasValue
				|| transferredGOV.ServerTimeStamp > newServerTimeStampGOV.Value))
				{
					newServerTimeStampGOV = transferredGOV.ServerTimeStamp;
				}
				else
				{
					newServerTimeStampGOV = DateTimeOffset.UtcNow;
				}


				if (transferredNSV != null
				&& (!newServerTimeStampNSV.HasValue
				|| transferredNSV.ServerTimeStamp > newServerTimeStampNSV.Value))
				{
					newServerTimeStampNSV = transferredNSV.ServerTimeStamp;
				}
				else
				{
					newServerTimeStampNSV = DateTimeOffset.UtcNow;
				}


				if (transferredGOV != null
				&& transferredGOV.Value is double
				&& transferredGOV.EngineeringUnitsType == EngineeringUnitType.FmuVolume)
				{
					double quantityMovedGOVM3 = 0.0;
					EngineeringUnits.Convert((double)transferredGOV.Value, transferredGOV.Units, ref quantityMovedGOVM3, EngineeringUnit.FmvMeter3, 0.0);

					if (movementNode.TransferDirection == TransferDirection.Destination)
					{
						if (quantityReceivedGOVM3.HasValue)
						{
							quantityReceivedGOVM3 += quantityMovedGOVM3;
						}
						else
						{
							quantityReceivedGOVM3 = quantityMovedGOVM3;
						}
					}
					else
					{
						if (quantityIssuedGOVM3.HasValue)
						{
							quantityIssuedGOVM3 += quantityMovedGOVM3;
						}
						else
						{
							quantityIssuedGOVM3 = quantityMovedGOVM3;
						}
					}
				}

				if (transferredNSV != null
				&& transferredNSV.Value is double
				&& transferredNSV.EngineeringUnitsType == EngineeringUnitType.FmuVolume)
				{
					double quantityMovedNSVM3 = 0.0;
					EngineeringUnits.Convert((double)transferredNSV.Value, transferredNSV.Units, ref quantityMovedNSVM3, EngineeringUnit.FmvMeter3, 0.0);

					if (movementNode.TransferDirection == TransferDirection.Destination)
					{
						if (quantityReceivedNSVM3.HasValue)
						{
							quantityReceivedNSVM3 += quantityMovedNSVM3;
						}
						else
						{
							quantityReceivedNSVM3 = quantityMovedNSVM3;
						}
					}
					else
					{
						if (quantityIssuedNSVM3.HasValue)
						{
							quantityIssuedNSVM3 += quantityMovedNSVM3;
						}
						else
						{
							quantityIssuedNSVM3 = quantityMovedNSVM3;
						}
					}
				}

				index++;
			}

			var deviationTag = movementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.MovementDeviationGuid);
			var percentDeviationTag = movementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.MovementPercentDeviationGuid);
			var transferredGOVTag = movementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.TransferredGOVGuid);
			var transferredNSVTag = movementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.TransferredNSVGuid);


			double newValueDeviation = 0.0;
			double newValuePercentDeviation = 0.0;
			double newValueDeviationM3 = 0.0;
			double? newValueTransferredGOV = null;
			double? newValueTransferredNSV = null;

			// When there are Issues & Receipts.  Do not Compute Deviation from GOV but we could especially if we decide to force 
			// the Nodes within  the Movement to have the same Transfer Volume Mode
			// Presently the expectatoin is that all Nodes will provide NSV and Deviation will be computed from  NSV
			if (newStatusGOV != StatusCodes.Bad
			&& quantityReceivedGOVM3.HasValue
			&& quantityIssuedGOVM3.HasValue
			&& newServerTimeStampGOV.HasValue)
			{
			}

			// Movement is either an Issue or Receipt
			else
			{
				newStatusDeviation = StatusCodes.Bad;

				if (quantityIssuedGOVM3.HasValue
				&& newServerTimeStampGOV.HasValue)
				{
					double convertedValue = 0.0;
					EngineeringUnits.Convert((double)quantityIssuedGOVM3.Value, EngineeringUnit.FmvMeter3, ref convertedValue, transferredGOVTag.Units, 0.0);
					newValueTransferredGOV = convertedValue;
				}

				if (quantityReceivedGOVM3.HasValue
				&& newServerTimeStampGOV.HasValue)
				{
					double convertedValue = 0.0;
					EngineeringUnits.Convert((double)quantityReceivedGOVM3.Value, EngineeringUnit.FmvMeter3, ref convertedValue, transferredGOVTag.Units, 0.0);
					newValueTransferredGOV = convertedValue;
				}
			}


			// When there are Issues & Receipts Compute Deviation
			if (newStatusNSV != StatusCodes.Bad
			&& quantityReceivedNSVM3.HasValue
			&& quantityIssuedNSVM3.HasValue
			&& newServerTimeStampNSV.HasValue)
			{
				if (quantityIssuedNSVM3.Value != 0)
				{
					newValueDeviationM3 = Math.Abs(quantityReceivedNSVM3.Value) - Math.Abs(quantityIssuedNSVM3.Value);
					newValuePercentDeviation = 100.0 * (newValueDeviationM3) / Math.Abs(quantityIssuedNSVM3.Value);
					EngineeringUnits.Convert((double)newValueDeviationM3, EngineeringUnit.FmvMeter3, ref newValueDeviation, deviationTag.Units, 0.0);
				}

				newStatusDeviation = newStatusNSV;
			}

			// Movement is either an Issue or Receipt
			else
			{
				newStatusDeviation = StatusCodes.Good;
				newValueDeviation = 0.0;

				if (quantityIssuedNSVM3.HasValue
				&& newServerTimeStampNSV.HasValue)
				{
					double convertedValue = 0.0;
					EngineeringUnits.Convert((double)quantityIssuedNSVM3.Value, EngineeringUnit.FmvMeter3, ref convertedValue, transferredNSVTag.Units, 0.0);
					newValueTransferredNSV = convertedValue;
				}

				if (quantityReceivedNSVM3.HasValue
				&& newServerTimeStampNSV.HasValue)
				{
					double convertedValue = 0.0;
					EngineeringUnits.Convert((double)quantityReceivedNSVM3.Value, EngineeringUnit.FmvMeter3, ref convertedValue, transferredNSVTag.Units, 0.0);
					newValueTransferredNSV = convertedValue;
				}
			}


			if (IsStatusChange(deviationTag.Status, newStatusDeviation)
			|| (newStatusDeviation != StatusCodes.Bad
			&& (!IsValueGood(new PointValue(deviationTag))
			|| newValueDeviation != (double)deviationTag.Value)))
			{
				deviationTag.Status = newStatusDeviation;
				if (newStatusDeviation != StatusCodes.Bad)
				{
					deviationTag.Value = newValueDeviation;
				}
				else
				{
					deviationTag.Value = null;
				}
				deviationTag.ServerTimeStamp = (newServerTimeStampNSV.HasValue) ? newServerTimeStampNSV.Value : DateTimeOffset.UtcNow;
				deviationTag.SourceTimeStamp = deviationTag.ServerTimeStamp;

				this.WriteTagToMasterData(true, false, deviationTag);
			}

			if (IsStatusChange(percentDeviationTag.Status, newStatusDeviation)
			|| (newStatusDeviation != StatusCodes.Bad
			&& (!IsValueGood(new PointValue(percentDeviationTag))
			|| newValuePercentDeviation != (double)percentDeviationTag.Value)))
			{
				percentDeviationTag.Status = newStatusDeviation;
				if (newStatusDeviation != StatusCodes.Bad)
				{
					percentDeviationTag.Value = newValuePercentDeviation;
				}
				else
				{
					percentDeviationTag.Value = null;
				}
				percentDeviationTag.ServerTimeStamp = (newServerTimeStampNSV.HasValue) ? newServerTimeStampNSV.Value : DateTimeOffset.UtcNow;
				percentDeviationTag.SourceTimeStamp = deviationTag.ServerTimeStamp;

				this.WriteTagToMasterData(true, false, percentDeviationTag);
			}



			if (IsStatusChange(transferredGOVTag.Status, newStatusGOV)
			|| (!newValueTransferredGOV.HasValue && transferredGOVTag.Value is double)
			|| (newValueTransferredGOV.HasValue && transferredGOVTag.Value == null)
			|| (newValueTransferredGOV.HasValue && newValueTransferredGOV.Value != (double)transferredGOVTag.Value))
			{
				transferredGOVTag.Status = newStatusGOV;
				if (newValueTransferredGOV.HasValue)
				{
					transferredGOVTag.Value = newValueTransferredGOV.Value;
				}
				else
				{
					transferredGOVTag.Value = null;
				}
				transferredGOVTag.ServerTimeStamp = (newServerTimeStampNSV.HasValue) ? newServerTimeStampNSV.Value : DateTimeOffset.UtcNow;
				transferredGOVTag.SourceTimeStamp = deviationTag.ServerTimeStamp;

				this.WriteTagToMasterData(true, false, transferredGOVTag);
			}

			if (IsStatusChange(transferredNSVTag.Status, newStatusNSV)
			|| (!newValueTransferredNSV.HasValue && transferredNSVTag.Value is double)
			|| (newValueTransferredNSV.HasValue && transferredNSVTag.Value == null)
			|| (newValueTransferredNSV.HasValue && newValueTransferredNSV.Value != (double)transferredNSVTag.Value))
			{
				transferredNSVTag.Status = newStatusNSV;
				if (newValueTransferredNSV.HasValue)
				{
					transferredNSVTag.Value = newValueTransferredNSV.Value;
				}
				else
				{
					transferredNSVTag.Value = null;
				}
				transferredNSVTag.ServerTimeStamp = (newServerTimeStampNSV.HasValue) ? newServerTimeStampNSV.Value : DateTimeOffset.UtcNow;
				transferredNSVTag.SourceTimeStamp = deviationTag.ServerTimeStamp;

				this.WriteTagToMasterData(true, false, transferredNSVTag);
			}
		}

		private void ProcessZeroFlow(Point movementPoint, MovementData movementData, MovementModuleSettings movementSettings)
		{

			if (!movementSettings.StopHaltBasedOnZeroFlow)
			{
				return;
			}

			var statusTag = movementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.MovementStatusGuid);
			var commandTag = movementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.MovementCommandGuid);

			if (!(statusTag.Value is MovementStatus)
			|| statusTag.IsBad()
			|| !(commandTag.Value is MovementCommand)
			|| commandTag.IsBad()
			|| (MovementCommand)commandTag.Value == MovementCommand.Stop
			|| !movementSettings.ZeroFlowHoldOffTime.HasValue)
			{
				return;
			}

			if ((MovementStatus)statusTag.Value != MovementStatus.Active
			&& (MovementStatus)statusTag.Value != MovementStatus.Stopping
			&& (MovementStatus)statusTag.Value != MovementStatus.Starting)
			{
				return;
			}

			bool zeroFlow = true;
			foreach (var pointValue in movementData.VolumeGrossObservedRate)
			{
				if (pointValue is PointValue)
				{
					PointValue flow = pointValue as PointValue;

					if (!flow.IsBad()
					&& (flow.Value is double)
					&& (double)flow.Value != 0.0)
					{
						zeroFlow = false;
						break;
					}
				}
			}

			if (zeroFlow == true)
			{
				foreach (var pointValue in movementData.VolumeNetStandardRate)
				{
					if (pointValue is PointValue)
					{
						PointValue flow = pointValue as PointValue;

						if (!flow.IsBad()
						&& (flow.Value is double)
						&& (double)flow.Value != 0.0)
						{
							zeroFlow = false;
							break;
						}
					}
				}
			}

			if (zeroFlow)
			{
				foreach (var pointValue in movementData.VolumeTotalObservedRate)
				{
					if (pointValue is PointValue)
					{
						PointValue flow = pointValue as PointValue;

						if (!flow.IsBad()
						&& (flow.Value is double)
						&& (double)flow.Value != 0.0)
						{
							zeroFlow = false;
							break;
						}
					}
				}
			}


			if ((MovementStatus)statusTag.Value == MovementStatus.Stopping
			&& !zeroFlow)
			{
				commandTag.Value = MovementCommand.Initiate;
				commandTag.Status = StatusCodes.Good;
				commandTag.ServerTimeStamp = DateTimeOffset.UtcNow;
				commandTag.SourceTimeStamp = DateTimeOffset.UtcNow;

				this.WriteTagToMasterData(true, false, commandTag);
			}

			if (((MovementStatus)statusTag.Value == MovementStatus.Active
			|| (MovementStatus)statusTag.Value == MovementStatus.Starting)
			&& zeroFlow)
			{
				commandTag.Value = MovementCommand.ZeroFlow;
				commandTag.Status = StatusCodes.Good;
				commandTag.ServerTimeStamp = DateTimeOffset.UtcNow;
				commandTag.SourceTimeStamp = DateTimeOffset.UtcNow;

				this.WriteTagToMasterData(true, false, commandTag);
			}

			if ((MovementStatus)statusTag.Value == MovementStatus.Stopping
			&& zeroFlow
			&& statusTag.ServerTimeStamp.AddMinutes(Convert.ToDouble(movementSettings.ZeroFlowHoldOffTime.Value)) <= DateTimeOffset.UtcNow)
			{
				FMChannelHelper.MakeCall<IMovementService>(x => x.StopMovement(this.security, movementPoint.PointGuid));
			}
		}


		private void ProcessStartOnNonZeroFlow(Point movementPoint, MovementData movementData, MovementModuleSettings movementSettings)
		{

			if (!movementSettings.StartTimeBasedOnNonZeroFlow)
			{
				return;
			}

			var statusTag = movementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.MovementStatusGuid);
			var commandTag = movementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.MovementCommandGuid);

			if (!(statusTag.Value is MovementStatus)
			|| statusTag.IsBad()
			|| !(commandTag.Value is MovementCommand)
			|| commandTag.IsBad())
			{
				return;
			}

			if ((MovementStatus)statusTag.Value != MovementStatus.Starting)
			{
				return;
			}

			bool nonZeroFlow = false;
			// Check VolumeGrossObservedRate
			foreach (var pointValue in movementData.VolumeGrossObservedRate)
			{
				if (IsFlowNonZero(pointValue))
				{
					nonZeroFlow = true;
					break;
				}
			}

			// Check VolumeNetStandardRate
			if (!nonZeroFlow)
			{
				foreach (var pointValue in movementData.VolumeNetStandardRate)
				{
					if (IsFlowNonZero(pointValue))
					{
						nonZeroFlow = true;
						break;
					}
				}
			}

			// Check VolumeTotalObserveddRate
			if (!nonZeroFlow)
			{
				foreach (var pointValue in movementData.VolumeTotalObservedRate)
				{
					if (IsFlowNonZero(pointValue))
					{
						nonZeroFlow = true;
						break;
					}
				}
			}


			if ((MovementStatus)statusTag.Value == MovementStatus.Starting
			&& nonZeroFlow)
			{
				commandTag.Value = MovementCommand.NonZeroFlow;
				commandTag.Status = StatusCodes.Good;
				commandTag.ServerTimeStamp = DateTimeOffset.UtcNow;
				commandTag.SourceTimeStamp = DateTimeOffset.UtcNow;

				this.WriteTagToMasterData(true, false, commandTag);
			}
		}


		private void ProcessStopAfterCompletion(MovementContainer movementContainer, MovementData movementData, MovementModuleSettings movementSettings)
		{
			if (!movementSettings.DeleteAfterCompletion)
			{
				movementContainer.Complete = false;
				return;
			}

			var statusTag = movementContainer.MovementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.MovementStatusGuid);
			var commandTag = movementContainer.MovementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.MovementCommandGuid);

			if (!(statusTag.Value is MovementStatus)
			|| statusTag.IsBad()
			|| !(commandTag.Value is MovementCommand)
			|| commandTag.IsBad()
			|| (MovementCommand)commandTag.Value == MovementCommand.Stop)
			{
				return;
			}

			movementContainer.Complete = true;
			foreach (var pointValue in movementData.TransferStatus)
			{
				if (pointValue is PointValue)
				{
					PointValue transferStatus = pointValue as PointValue;

					if (!transferStatus.IsBad()
					&& (transferStatus.Value is TransferStatuses)
					&& (TransferStatuses)transferStatus.Value != TransferStatuses.Complete)
					{
						movementContainer.Complete = false;
						break;
					}
				}
			}

			if ((MovementStatus)statusTag.Value == MovementStatus.Active
			&& movementContainer.Complete)
			{
				FMChannelHelper.MakeCall<IMovementService>(x => x.StopMovement(this.security, movementContainer.MovementPoint.PointGuid));
			}
		}

		private void ProcessDeleteAfterCompletion(MovementContainer movementContainer, MovementModuleSettings movementSettings)
		{
			if (!movementSettings.DeleteAfterCompletion
			|| !movementContainer.Complete)
			{
				return;
			}

			FMChannelHelper.MakeCall<IPoints>(x => x.Purge(this.security, movementContainer.MovementPoint.PointGuid));
		}

		private void ProcessDeleteAfterStop(MovementContainer movementContainer, MovementModuleSettings movementSettings)
		{
			if (!movementSettings.DeleteAfterStop
			|| (movementSettings.DeleteAfterCompletion
			&& movementContainer.Complete))
			{
				return;
			}

			FMChannelHelper.MakeCall<IPoints>(x => x.Purge(this.security, movementContainer.MovementPoint.PointGuid));
		}

        private void ProcessMovementNotifications(MovementContainer movementContainer, MovementData movementData, MovementModuleSettings movementSettings, NotificationType notificationType)
        {
            try
            {
				string strMovementNotifyIf = FMChannelHelper.MakeCall<IConfigurationSettings, string>(configSettings => configSettings.GetKeyValueByKey(this.security, ConfigurationSettingDOClass.Key_MovementNotifyInterface));

                if (string.IsNullOrEmpty(strMovementNotifyIf) == false)
                {
                    char[] separator = { ';' };
                    string[] enterpriseIfList = strMovementNotifyIf.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string assemblyName in enterpriseIfList)
                    {
                        try
                        {
                            Assembly dll = null;
                            if (!AssemblyDictionary.ContainsKey(assemblyName.ToLower()))
                            {
                                try
                                {
                                    dll = Assembly.LoadFrom(assemblyName.ToString());
                                }
                                catch
                                {
                                    try
                                    {
                                        dll = Assembly.Load(assemblyName);
                                    }
                                    catch (Exception ex)
                                    {
                                        string message = "Assembly Load Error in Send Movement Completion Notification. " + ex.Message;
                                        FMChannelHelper.MakeCall<IFMEventLog>(eventLog => eventLog.WriteEntry(message, FMEventLogEntryType.Warning));
                                    }
                                }

                                if (dll != null)
                                    AssemblyDictionary.Add(assemblyName.ToLower(), dll);
                            }
                            else
                            {
                                dll = AssemblyDictionary.Get(assemblyName.ToLower());
                            }

                            if (dll == null)
                                continue;

                            try
                            {
                                Type[] types = dll.GetTypes();

                                foreach (Type module in types)
                                {
                                    Type enterprise = module.GetInterface("IMovementNotify");

                                    if (enterprise != null)
                                    {
                                        object engine = Activator.CreateInstance(module);
                                        IMovementNotify movementNotifyEngine = (IMovementNotify)engine;

                                        movementNotifyEngine.Notify(notificationType, movementData);
                                    }
                                }
                            }
                            catch { }
                        }
                        catch (Exception e)
                        {
                            FMChannelHelper.MakeCall<IFMEventLog>(eventLog => eventLog.WriteEntry(e.ToString(), FMEventLogEntryType.Error));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                FMChannelHelper.MakeCall<IFMEventLog>(eventLog => eventLog.WriteEntry(e.ToString(), FMEventLogEntryType.Error));
            }
        }

        private void WriteMidnightMovementHistory(Point movementPoint, MovementData movementData, DateTimeOffset currentTimeUtc)
		{

			string siteTimeZone;
			if (this.SiteTimeZoneDictionary.TryGetValue(movementPoint.SiteGuid, out siteTimeZone))
			{
				var historyWrittenTimeTag = movementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.MovementHistoryWrittenTimeGuid);
				var transferStartTimeTag = movementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.TransferStartTimeGuid);

				var currentTimeLocal = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(currentTimeUtc, siteTimeZone);

				// This tag should be present, and the Value should be set when the Movement is Initiated
				DateTimeOffset? historyWrittenTimeLocal = null;
				if (historyWrittenTimeTag != null
				&& historyWrittenTimeTag.Value != null
				&& historyWrittenTimeTag.Value is DateTimeOffset)
				{
					historyWrittenTimeLocal = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((DateTimeOffset)historyWrittenTimeTag.Value, siteTimeZone);
				}

				DateTimeOffset? transferStartTimeLocal = null;
				if (transferStartTimeTag != null
				&& transferStartTimeTag.Value != null
				&& transferStartTimeTag.Value is DateTimeOffset)
				{
					transferStartTimeLocal = TimeZoneInfo.ConvertTimeBySystemTimeZoneId((DateTimeOffset)transferStartTimeTag.Value, siteTimeZone);
				}

				if (
					(historyWrittenTimeLocal.HasValue && currentTimeLocal.DayOfYear != historyWrittenTimeLocal.Value.DayOfYear) || // Once a record exists, this should always prevail
					(!historyWrittenTimeLocal.HasValue && transferStartTimeLocal.HasValue && currentTimeLocal.DayOfYear != transferStartTimeLocal.Value.DayOfYear) // Provides with first history record to be written
					) 
				{
					historyWrittenTimeTag.Value = currentTimeUtc;
					historyWrittenTimeTag.Status = StatusCodes.Good;
					historyWrittenTimeTag.ServerTimeStamp = DateTimeOffset.UtcNow;
					historyWrittenTimeTag.SourceTimeStamp = DateTimeOffset.UtcNow;

					this.WriteTagToMasterData(false, false, historyWrittenTimeTag);

					List<PointTag> tagList = new List<PointTag>();
					tagList.Add(historyWrittenTimeTag);
					FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(this.security, tagList, false));


					// Initialize Opening Data
					var movementSettingsPointProperty = movementPoint.Properties.Values.Single(u => u.ValueTypeString == "FMBusinessObjects.DataObjects.MovementModuleSettings");
					var movementSettings = movementSettingsPointProperty.Value as MovementModuleSettings;
					Type movementDataType = typeof(MovementData);
					IList<PropertyInfo> propertyInfoList = new List<PropertyInfo>(movementDataType.GetProperties());
					var pointCount = movementSettings.MovementNodeDataList.Count + 1;
					for (var pointIndex = 0; pointIndex < pointCount; pointIndex++)
					{
						this.InitializeOpeningData(pointIndex, propertyInfoList, movementData);
					}

					try
					{
						FMChannelHelper.MakeCall<IMovementHistories>(x => x.Add(this.security, movementData));
					}
					catch (Exception ex)
					{
						string msg = "MovementProcessor - Error writing Movement to Movement History. " + ex.Message;
						FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry("", FMEventLogEntryType.Error));
					}
				}
			}
		}

		private void InitializeOpeningData(int pointIndex, IList<PropertyInfo> propertyInfoList, MovementData movementData)
		{
			const int openingProperties = 20;

			string[,] openingDataPropertyMap = new string[,] {
				{ "OpeningDensityProductObserved", "DensityProductObserved" },
				{ "OpeningDensityProductinAir", "DensityProductinAir" },
				{ "OpeningDensityProductStandard", "DensityProductStandard" },
				{ "OpeningDensityProductStandardinAir", "DensityProductStandardinAir" },
				{ "OpeningLevelProduct", "LevelProduct" },
				{ "OpeningLevelWater", "LevelWater" },
				{ "OpeningMassLiquid", "MassLiquid" },
				{ "OpeningTankShellCorrection", "TankShellCorrection" },
				{ "OpeningTemperatureAmbient" , "TemperatureAmbient"},
				{ "OpeningTemperatureDensity", "TemperatureDensity" },
				{ "OpeningTemperatureProduct", "TemperatureProduct" },
				{ "OpeningVolumeCorrectionFactor", "VolumeCorrectionFactor" },
				{ "OpeningVolumeRoofCorrection", "VolumeRoofCorrection" },
				{ "OpeningVolumeTotalObserved", "VolumeTotalObserved" },
				{ "OpeningVolumeGrossStandard", "VolumeGrossStandard" },
				{ "OpeningVolumeGrossObserved", "VolumeGrossObserved" },
				{ "OpeningVolumeNetStandard", "VolumeNetStandard" },
                { "OpeningVolumeWater", "VolumeWater" },
                { "OpeningPercentBsw", "PercentBsw" },
                { "OpeningVolumeBsw", "VolumeBsw" },
};


			for (int propertyIndex = 0; propertyIndex < openingProperties; propertyIndex++)
			{
                var openingPropertyInfo = propertyInfoList.Single(x => x.Name == openingDataPropertyMap[propertyIndex, 0]);
				var openingPropertyValue = openingPropertyInfo.GetValue(movementData, null) as List<PointValue>;

				var sourcePropertyInfo = propertyInfoList.Single(x => x.Name == openingDataPropertyMap[propertyIndex, 1]);
				var sourcePropertyValue = sourcePropertyInfo.GetValue(movementData, null) as List<PointValue>;

                if (openingPropertyValue != null
				&& sourcePropertyValue != null
				&& sourcePropertyValue.Count > pointIndex)
				{
					// make sure the list has sufficient entries
					while (openingPropertyValue.Count < pointIndex + 1)
					{
						openingPropertyValue.Add(null);
					}

					if (sourcePropertyValue[pointIndex] is PointValue)
					{
						openingPropertyValue[pointIndex] = sourcePropertyValue[pointIndex].Clone() as PointValue;
					}
				}
            }
		}

		private void InitializeStartData(int pointIndex, IList<PropertyInfo> propertyInfoList, MovementData movementData)
		{
			const int startProperties = 16;

			string[,] startDataPropertyMap = new string[startProperties, 2] {
				{ "StartTemperatureAmbient","TemperatureAmbient" },
				{ "StartDensityProductObserved", "DensityProductObserved" },
				{ "StartDensityProductinAir", "DensityProductinAir" },
				{ "StartDensityProductStandard", "DensityProductStandard" },
				{ "StartDensityProductStandardinAir", "DensityProductStandardinAir" },
				{ "StartLevelWater", "LevelWater" },
				{ "StartMassLiquid", "MassLiquid" },
				{ "StartTankShellCorrection", "TankShellCorrection" },
				{ "StartTemperatureDensity", "TemperatureDensity" },
				{ "StartTemperatureProduct", "TemperatureProduct" },
				{ "StartVolumeCorrectionFactor", "VolumeCorrectionFactor" },
				{ "StartVolumeRoofCorrection", "VolumeRoofCorrection" },
				{ "StartVolumeTotalObserved", "VolumeTotalObserved" },
				{ "StartVolumeGrossStandard", "VolumeGrossStandard" },
            { "StartPercentBsw", "PercentBsw" },
            { "StartVolumeBsw", "VolumeBsw" },
            };


			for (int propertyIndex = 0; propertyIndex < startProperties; propertyIndex++)
			{
				var startPropertyInfo = propertyInfoList.Single(x => x.Name == startDataPropertyMap[propertyIndex, 0]);
				var startPropertyValue = startPropertyInfo.GetValue(movementData, null) as List<PointValue>;

				var sourcePropertyInfo = propertyInfoList.Single(x => x.Name == startDataPropertyMap[propertyIndex, 1]);
				var sourcePropertyValue = sourcePropertyInfo.GetValue(movementData, null) as List<PointValue>;

				if (startPropertyValue != null
				&& sourcePropertyValue != null
				&& sourcePropertyValue.Count > pointIndex)
				{
					// make sure the list has sufficient entries
					while (startPropertyValue.Count < pointIndex + 1)
					{
						startPropertyValue.Add(null);
					}

					if (sourcePropertyValue[pointIndex] is PointValue)
					{
						startPropertyValue[pointIndex] = sourcePropertyValue[pointIndex].Clone() as PointValue;
					}
				}
			}
		}


		private void WriteStopMovementHistory(MovementContainer movementContainer, MovementData movementData, MovementModuleSettings movementSettings)
		{

			if (movementData.InitiationCount == null || movementData.InitiationCount.Count == 0)
			{
				// There is no data to write
				return;
			}
				
			// Set MovementStatus to Complete for writing
			if (movementData.Status.Count >= 1
			&& movementData.Status[0] is PointValue)
			{
				movementData.Status[0].Value = MovementStatus.Complete;
			}

			// Initialize Opening Data
			Type movementDataType = typeof(MovementData);
			IList<PropertyInfo> propertyInfoList = new List<PropertyInfo>(movementDataType.GetProperties());
			var pointCount = movementSettings.MovementNodeDataList.Count + 1;
			for (var pointIndex = 0; pointIndex < pointCount; pointIndex++)
			{
				this.InitializeOpeningData(pointIndex, propertyInfoList, movementData);
			}

			try
			{
				this.security.SiteGuid = movementContainer.MovementPoint.SiteGuid;
				var movementHistoryGuid = FMChannelHelper.MakeCall<IMovementHistories, Guid>(x => x.Add(this.security, movementData));

				// Print Movement Ticket
				try
				{
					FMChannelHelper.MakeCall<IMovementHistories>(x => x.PrintMovementTicket(this.security, movementHistoryGuid, true));
				}
				catch (Exception ex)
				{
					string msg = "MovementProcessor - Error printing Movement Ticket. " + ex.Message;
					FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg, FMEventLogEntryType.Error));
				}

				// Archive Movement Ticket
				try
				{
					FMChannelHelper.MakeCall<IMovementHistories>(x => x.ArchiveMovementTicket(this.security, movementHistoryGuid, movementContainer.MovementPoint.ID));
				}
				catch (Exception ex)
				{
					string msg = "MovementProcessor - Error archiving Movement Ticket. " + ex.Message;
					FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg, FMEventLogEntryType.Error));
				}
			}
			catch (Exception ex)
			{
				string msg = "MovementProcessor - Error writing Movement to Movement History. " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry("", FMEventLogEntryType.Error));
			}

			// Set Movement Stauts back to Inactive
			if (movementData.Status.Count >= 1
			&& movementData.Status[0] is PointValue)
			{
				movementData.Status[0].Value = MovementStatus.Inactive;
			}


			var historyWrittenTimeTag = movementContainer.MovementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.MovementHistoryWrittenTimeGuid);
			historyWrittenTimeTag.Value = DateTimeOffset.UtcNow;
			historyWrittenTimeTag.Status = StatusCodes.Good;
			historyWrittenTimeTag.ServerTimeStamp = DateTimeOffset.UtcNow;
			historyWrittenTimeTag.SourceTimeStamp = DateTimeOffset.UtcNow;

			this.WriteTagToMasterData(false, false, historyWrittenTimeTag);


			List<PointTag> tagList = new List<PointTag>();
			tagList.Add(historyWrittenTimeTag);

			FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(this.security, tagList, false));
		}

		private bool UpdateMovementData(MovementContainer movementContainer, PointProperty movementDataPointProperty, PointProperty movementSettingsPointProperty, bool partial, Dictionary<PointValueIdentifier,PointValue> consolidatedPointValues)
		{
			bool changes = false;

			var movementPoint = movementContainer.MovementPoint;
			var movementData = movementDataPointProperty.Value as MovementData;
			var movementSettings = movementSettingsPointProperty.Value as MovementModuleSettings;

			var pointValueIdentifierList = movementDataPointValueListDictionary[movementPoint.PointGuid];
			List<PointValue> pointValueList = new List<PointValue>();

			var partialPointValueIdentiferList = new List<PointValueIdentifier>();

			if (partial)
			{
				foreach (var pointValueIdentifier in pointValueIdentifierList)
				{
					if (pointValueIdentifier.PropertyID != "CreatedBy"
					&& pointValueIdentifier.PropertyID != "PointId"
					&& pointValueIdentifier.WellKnownIdentityGuid != Guids.MovementStatusGuid)
					{
						break;
					}
					pointValueList.Add(consolidatedPointValues[pointValueIdentifier]);
				}
			}
			else
			{
				foreach (var pointValueIdentifier in pointValueIdentifierList)
				{
					 pointValueList.Add(consolidatedPointValues[pointValueIdentifier]);
				}
			}

			var pointCount = movementSettings.MovementNodeDataList.Count + 1;
			bool[] transferActivated = new bool[pointCount];

			List<PointValue> statusList = movementData.Status;
			List<PointValue> transferStatusList = movementData.TransferStatus;
			List<PointValue> transferStartTimeList = movementData.TransferStartTime;
			List<PointValue> volumeGrossObservedRateList = movementData.VolumeGrossObservedRate;
			List<PointValue> volumeNetStandardRateList = movementData.VolumeNetStandardRate;
			List<PointValue> volumeTotalObservedRateList = movementData.VolumeTotalObservedRate;

			PointValue priorMovementStatus = null;
			if (statusList.Count > 0)
			{
				priorMovementStatus = statusList[0];
			}

			var movementDataProperties = new List<List<PointValue>>(100);
			Type movementDataType = typeof(MovementData);
			IList<PropertyInfo> propertyInfoList = new List<PropertyInfo>(movementDataType.GetProperties());
			Dictionary<string, PropertyInfo> propertyInfoDictionary = propertyInfoList.ToDictionary(x => x.Name, x => x);

			List<PointValue> movementDataProperty = null;

			int pointValueIndex = 0;

			foreach (PropertyInfo propertyInfo in propertyInfoList)
			{
				if (partial)
				{
					if (propertyInfo.Name != "CreatedBy"
					&& propertyInfo.Name != "PointId"
					&& propertyInfo.Name != "Status"
					&& propertyInfo.Name != "Type")
					{
						break;
					}
				}

				object propertyValue = propertyInfo.GetValue(movementData, null);

				if (propertyValue is List<PointValue>)
				{
					movementDataProperties.Add(propertyValue as List<PointValue>);
				}
			}


			lock (this.LockObject)
			{

				foreach (var pointValue in pointValueList)
				{
					var itemIndex = pointValueIndex % pointCount;

					if (itemIndex == 0)
					{
						movementDataProperty = movementDataProperties[pointValueIndex / pointCount];
					}

					if (movementDataProperty.Count <= itemIndex)
					{
						// Do not set the Transfer Start Time for a Node until there is non zero flow 
						if (movementSettings.StartTimeBasedOnNonZeroFlow
						&& itemIndex != 0
						&& movementDataProperty == movementData.TransferStartTime)
						{
							pointValue.Value = null;
						}

						// Set Transfer Start Time when there is non zero flow
						if (movementSettings.StartTimeBasedOnNonZeroFlow
						&& itemIndex != 0
						&& (movementDataProperty == movementData.VolumeGrossObservedRate
						|| movementDataProperty == movementData.VolumeNetStandardRate
						|| movementDataProperty == movementData.VolumeTotalObservedRate)
						&& pointValue.Value is double
						&& (double)pointValue.Value != 0.0
						&& transferStartTimeList.Count >= itemIndex)
						{
							transferStartTimeList[itemIndex].Value = DateTimeOffset.UtcNow;
						}


						movementDataProperty.Add(pointValue);

						// Test for Transfer Activate
						if (pointValue.Value != null
						&& movementDataProperty == movementData.TransferStartTime)
						{
							transferActivated[(itemIndex)] = true;
						}

						changes = true;
					}
					else
					{
						// Test for Tag Changes
						if ((movementDataProperty[itemIndex].Value == null
						&& pointValue.Value != null)
						|| (movementDataProperty[itemIndex].Value != null
						&& !movementDataProperty[itemIndex].Value.Equals(pointValue.Value))
						|| movementDataProperty[itemIndex].Status != pointValue.Status
						|| movementDataProperty[itemIndex].Acknowledged != pointValue.Acknowledged
						|| movementDataProperty[itemIndex].AlarmState != pointValue.AlarmState
						|| movementDataProperty[itemIndex].AlarmPriorityGuid != pointValue.AlarmPriorityGuid)
						{

							// Movement Data
							if (itemIndex == 0
							&& movementDataProperty.Count > itemIndex)
							{
								// For Movement capture Transfer Start Time once to permit it being edited
								if (statusList.Count > itemIndex
								&& statusList[itemIndex].Value is MovementStatus
								&& (MovementStatus)statusList[itemIndex].Value != MovementStatus.Inactive
								&& movementDataProperty == transferStartTimeList)
								{
									if (movementDataProperty[itemIndex].Value == null)
									{
										movementDataProperty[itemIndex] = pointValue;
										transferActivated[(itemIndex)] = true;
										changes = true;
									}

									pointValueIndex++;
									continue;
								}


								// skip remaining updates when Inactive
								if (statusList.Count > itemIndex
								&& statusList[itemIndex].Value is MovementStatus
								&& ((MovementStatus)statusList[itemIndex].Value == MovementStatus.Inactive
								|| (MovementStatus)statusList[itemIndex].Value == MovementStatus.Disabled))
								{

									if (movementDataProperty == movementData.Status
									|| movementDataProperty == movementData.TransferStopTime
									|| movementDataProperty == movementData.Product
									|| movementDataProperty == movementData.PointId)
									{
										movementDataProperty[itemIndex] = pointValue;
										changes = true;
									}

									pointValueIndex++;
									continue;
								}

								movementDataProperty[itemIndex] = pointValue;
								changes = true;
								pointValueIndex++;
								continue;
							}



							// Node Data 
							if (itemIndex != 0
							&& movementDataProperty.Count > itemIndex)
							{
								if (transferStatusList.Count > itemIndex
								&& transferStatusList[itemIndex] != null
								&& transferStatusList[itemIndex].Value is TransferStatuses)
								{
									// Update TransferStartTime when not StartTimeBasedOnNonZeroFlow is configured
									// and Transfer Status is Inactive
									if (!movementSettings.StartTimeBasedOnNonZeroFlow
									&& movementDataProperty == movementData.TransferStartTime
									&& (TransferStatuses)transferStatusList[itemIndex].Value != TransferStatuses.Inactive)
									{
										if (movementDataProperty[itemIndex].Value == null)
										{
											movementDataProperty[itemIndex] = pointValue;
											transferActivated[(itemIndex)] = true;
											changes = true;
										}
										pointValueIndex++;
										continue;
									}
								}

								// skip remaining updates when not InProgress and not Complete
								if (transferStatusList.Count > itemIndex
								&& transferStatusList[itemIndex] != null
								&& transferStatusList[itemIndex].Value is TransferStatuses
								&& (TransferStatuses)transferStatusList[itemIndex].Value != TransferStatuses.InProgress
								&& (TransferStatuses)transferStatusList[itemIndex].Value != TransferStatuses.Complete)
								{

									if (movementDataProperty == movementData.TransferStatus
									|| movementDataProperty == movementData.TransferMode
									|| movementDataProperty == movementData.TransferStopTime
									|| movementDataProperty == movementData.Product
									|| movementDataProperty == movementData.PointId)
									{
										movementDataProperty[itemIndex] = pointValue;
										changes = true;
									}

									pointValueIndex++;
									continue;
								}

								// When flow is non Zero, set the Node Start Time
								if (movementSettings.StartTimeBasedOnNonZeroFlow
								&& (movementDataProperty == movementData.VolumeGrossObservedRate
								|| movementDataProperty == movementData.VolumeNetStandardRate
								|| movementDataProperty == movementData.VolumeTotalObservedRate)
								&& pointValue.Value is double
								&& (double)pointValue.Value != 0.0)
								{
									if (transferStartTimeList.Count >= itemIndex
									&& transferStartTimeList[itemIndex].Value == null)
									{
										transferStartTimeList[itemIndex].Value = DateTimeOffset.UtcNow;
										changes = true;
									}

									movementDataProperty[itemIndex] = pointValue;

									pointValueIndex++;
									continue;
								}

								if (movementSettings.StartTimeBasedOnNonZeroFlow
								&& movementDataProperty == movementData.TransferStartTime)
								{
									if ((IsFlowNonZero(volumeGrossObservedRateList[itemIndex])
									|| IsFlowNonZero(volumeNetStandardRateList[itemIndex])
									|| IsFlowNonZero(volumeNetStandardRateList[itemIndex]))
									&& transferStartTimeList.Count >= itemIndex
									&& transferStartTimeList[itemIndex].Value == null)
									{
										transferStartTimeList[itemIndex].Value = DateTimeOffset.UtcNow;
										changes = true;
									}

									pointValueIndex++;
									continue;
								}
							}

							movementDataProperty[itemIndex] = pointValue;
							changes = true;
							pointValueIndex++;
							continue;
						}
					}

					pointValueIndex++;
				}

				var saveMovementData = false;

				// Initialize Start Data
				for (var pointIndex = 0; pointIndex < pointCount; pointIndex++)
				{
					if (transferActivated[pointIndex])
					{
						this.InitializeStartData(pointIndex, propertyInfoList, movementData);
						saveMovementData = true;
					}

					if (saveMovementData
					|| (partial
					&& changes))
					{
						this.SaveMovementData(movementDataPointProperty);
					}
				}


				// Test for Movement Stopped
				if (priorMovementStatus != null
				&& priorMovementStatus.Value is MovementStatus
				&& (MovementStatus)priorMovementStatus.Value != MovementStatus.Inactive
				&& (MovementStatus)priorMovementStatus.Value != MovementStatus.Disabled
				&& statusList[0].Value is MovementStatus
				&& (MovementStatus)statusList[0].Value == MovementStatus.Inactive)
				{
					WriteStopMovementHistory(movementContainer, movementData, movementSettings);
					ProcessMovementNotifications(movementContainer, movementData, movementSettings, movementContainer.Complete ? NotificationType.Complete : NotificationType.Update);
					ProcessDeleteAfterCompletion(movementContainer, movementSettings);
					ProcessDeleteAfterStop(movementContainer, movementSettings);
				}
			}

			return changes;

		}


        /// <summary>
        /// This method updates the movement points.
        /// Function call resolution: 1 second
        /// </summary>
        private void UpdateMovements()
		{
			var currentTimeUtc = DateTimeOffset.UtcNow;
			bool writeHistoryTime = false;
			var consolidatedPointValueIdentiferList = new List<PointValueIdentifier>();

			iterationCount++;

			// Check for Movement History Write Time. As time progresses.  It occurs at Midnight for the various time zones
			if ((this.LastCheckHistoryWriteTimeUtc == null	// Hasn't been written yet OR
			|| this.LastCheckHistoryWriteTimeUtc.HasValue && LastCheckHistoryWriteTimeUtc.Value.Minute != currentTimeUtc.Minute) // Has been written and minute is diff from the current
			&& (currentTimeUtc.Minute == 0 || currentTimeUtc.Minute == 30 || currentTimeUtc.Minute == 45) // check every time the minute is 00 or 30 or 45
			) 
			{
				this.SiteTimeZoneDictionary = FMChannelHelper.MakeCall<ISites, Dictionary<Guid, string>>(x => x.EnumerateTimeZonesForSiteGuidList(this.security, this.SiteList));
				if (this.SiteTimeZoneDictionary != null)
				{
					writeHistoryTime = true;
				}
			}

			this.LastCheckHistoryWriteTimeUtc = currentTimeUtc;

			var timer = StatisticsLogger.Start("Update Movements");

			foreach (var movementContainer in this.movementContainerDictionary.Values)
			{
				try
				{

					var movementPoint = movementContainer.MovementPoint;
					var movementSettingsPointProperty = movementPoint.Properties.Values.Single(u => u.ValueTypeString == "FMBusinessObjects.DataObjects.MovementModuleSettings");
					var movementSettings = movementSettingsPointProperty.Value as MovementModuleSettings;
					var movementDataPointProperty = movementPoint.Properties.Values.Single(u => u.ValueTypeString == "FMBusinessObjects.DataObjects.MovementData");
					var movementData = movementDataPointProperty.Value as MovementData;

					ThreadSharedData.Instance().ApplyTagChangesToCopy(movementPoint);

					var statusTag = movementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.MovementStatusGuid);

					if (statusTag.Value is MovementStatus
					&& statusTag.IsGood())
					{
						// Movement is Inactive and has been inactive
						if (((MovementStatus)statusTag.Value == MovementStatus.Inactive
						|| (MovementStatus)statusTag.Value == MovementStatus.Disabled)
						// compare with the previous state
						&& (movementData.Status.Count == 0
						|| movementData.Status[0].Value == null
						|| (MovementStatus)movementData.Status[0].Value == MovementStatus.Inactive
						|| (MovementStatus)movementData.Status[0].Value == MovementStatus.Disabled))
						{
							if (iterationCount >= updateInactiveMovementsEvery)
							{
								var pointValueIdentifierList = movementDataPointValueListDictionary[movementPoint.PointGuid];

								var partialPointValueIdentiferList = new List<PointValueIdentifier>();


								foreach (var pointValueIdentifier in pointValueIdentifierList)
								{
									if (pointValueIdentifier.PropertyID != "CreatedBy"
									&& pointValueIdentifier.PropertyID != "PointId"
									&& pointValueIdentifier.WellKnownIdentityGuid != Guids.MovementStatusGuid)
									{
										break;
									}

									consolidatedPointValueIdentiferList.Add(pointValueIdentifier);
								}
							}
						}

						// Movement is Active Acquire Full Compliment of Data
						else
						{
							var pointValueIdentifierList = movementDataPointValueListDictionary[movementPoint.PointGuid];

							consolidatedPointValueIdentiferList.AddRange(pointValueIdentifierList);
						}
					}
				}
				catch (Exception e)
				{
					Logger.LogError("MovementProcessor Inner Loop Exception: " + e.Message);
				}
			}

			var consolidatedPointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.security, consolidatedPointValueIdentiferList, false));
			var consolidatedPointValueDict = new Dictionary<PointValueIdentifier, PointValue>();

            foreach (var pvi in consolidatedPointValueList)
			{
				consolidatedPointValueDict[pvi.PointValueIdentifier] = pvi;
			}


            foreach (var movementContainer in this.movementContainerDictionary.Values)
			{
				try
				{
					var movementPoint = movementContainer.MovementPoint;
					var movementSettingsPointProperty = movementPoint.Properties.Values.Single(u => u.ValueTypeString == "FMBusinessObjects.DataObjects.MovementModuleSettings");
					var movementSettings = movementSettingsPointProperty.Value as MovementModuleSettings;
					var movementDataPointProperty = movementPoint.Properties.Values.Single(u => u.ValueTypeString == "FMBusinessObjects.DataObjects.MovementData");
					var movementData = movementDataPointProperty.Value as MovementData;

					bool changes = false;

					var statusTag = movementPoint.Tags.Values.Single(u => u.WellKnownIdentityGuid == Guids.MovementStatusGuid);

					if (statusTag.Value is MovementStatus
					&& statusTag.IsGood())
					{
						// Movement is Inactive and has been inactive
						if (((MovementStatus)statusTag.Value == MovementStatus.Inactive
						|| (MovementStatus)statusTag.Value == MovementStatus.Disabled)
						// compare with the previous state
						&& (movementData.Status.Count == 0
						|| movementData.Status[0].Value == null
						|| (MovementStatus)movementData.Status[0].Value == MovementStatus.Inactive
						|| (MovementStatus)movementData.Status[0].Value == MovementStatus.Disabled))
						{
							if (iterationCount >= updateInactiveMovementsEvery)
							{
								lock (this.LockObject)
								{

									// Clear the Movement Data
									if (movementData.TransferStatus.Count > 0)
									{
										Type movementDataType = typeof(MovementData);
										IList<PropertyInfo> propertyInfoList = new List<PropertyInfo>(movementDataType.GetProperties());

										foreach (PropertyInfo propertyInfo in propertyInfoList)
										{
											if (propertyInfo.Name == "Status"
											|| propertyInfo.Name == "TransferDirection"
											|| propertyInfo.Name == "CreatedBy"
											|| propertyInfo.Name == "IndividualNodeControl"
											|| propertyInfo.Name == "PointId"
											|| propertyInfo.Name == "OrderNumber"
											|| propertyInfo.Name == "Comment"
											|| propertyInfo.Name == "PlannedStartTime"
											|| propertyInfo.Name == "Type")
											{
												continue;
											}

											object propertyValue = propertyInfo.GetValue(movementData, null);

											if (propertyValue is List<PointValue>)
											{
												(propertyValue as List<PointValue>).Clear();
											}

											this.SaveMovementData(movementDataPointProperty);

											changes = true;
										}
									}
								}

								changes = changes | this.UpdateMovementData(movementContainer, movementDataPointProperty, movementSettingsPointProperty, true, consolidatedPointValueDict);

								this.UpdateDeviation(movementPoint, movementData, movementSettings);
							}
						}
						// Movement is Active Acquire Full Compliment of Data
						else
						{

								changes = changes | this.UpdateMovementData(movementContainer, movementDataPointProperty, movementSettingsPointProperty, false, consolidatedPointValueDict);

								lock (this.LockObject)
								{

									this.UpdateDeviation(movementPoint, movementData, movementSettings);

									this.UpdateTransferTimeRemaining(movementPoint, movementData, movementSettings);

									this.ProcessZeroFlow(movementPoint, movementData, movementSettings);

									this.ProcessStopAfterCompletion(movementContainer, movementData, movementSettings);

									this.ProcessStartOnNonZeroFlow(movementPoint, movementData, movementSettings);

									// Check Time to write History
									if (writeHistoryTime)
									{
										this.WriteMidnightMovementHistory(movementPoint, movementData, currentTimeUtc);
									}
								}
						}

						if (changes)
						{
							lock (this.LockObject)
							{
								movementDataPointProperty.UpdatedDate = DateTimeOffset.Now;
								ThreadSharedData.Instance().ApplyPropertyChangesToMaster(movementPoint);
							}
						}
					}
				}
				catch (Exception e)
				{
					Logger.LogError("MovementProcessor Inner Loop Exception: " + e.Message);
				}
			}



			if (iterationCount >= updateInactiveMovementsEvery)
			{
				iterationCount = 0;
			}
			StatisticsLogger.Stop(timer);
		}

		private void GetMovements()
		{

			// Tag Guid List must be in the same order as the movementData Properties
			var wellKnownTagGuidList = new Guid[] {
				// Guids must match Movement Data
				Guids.PointIdGuid,
				Guids.CreatedByGuid,
				Guids.MovementStatusGuid,
				Guids.TransferStatusGuid,
				Guids.PointProductGuid,
				Guids.TransferStartTimeGuid,
				Guids.TransferStopTimeGuid,
				Guids.InitiationCountGuid,
				Guids.LevelProductGuid,
				Guids.LevelWaterGuid,
				Guids.MassLiquidGuid,
				Guids.TemperatureAmbientGuid,
				Guids.TemperatureDensityGuid,
				Guids.TemperatureProductGuid,
				Guids.DensityProductObservedGuid,
				Guids.DensityProductInAirGuid,
				Guids.DensityProductStandardGuid,
				Guids.DensityProductStandardInAirGuid,
				Guids.VolumeCorrectionFactorGuid,
				Guids.VolumeGrossObservedGuid,
				Guids.VolumeGrossStandardGuid,
				Guids.VolumeNetStandardGuid,
				Guids.VolumeTotalObservedGuid,
				Guids.VolumeWaterGuid,
				Guids.VolumeRoofCorrectionGuid,
				Guids.TankShellCorrectionGuid,
				Guids.VolumeGrossObservedRateGuid,
				Guids.VolumeNetStandardRateGuid,
				Guids.VolumeTotalObservedRateGuid,
				Guids.UserData01WellKnownGuid,
				Guids.UserData02WellKnownGuid,
				Guids.UserData03WellKnownGuid,
				Guids.UserData04WellKnownGuid,
				Guids.UserData05WellKnownGuid,
				Guids.UserData06WellKnownGuid,
				Guids.UserData07WellKnownGuid,
				Guids.UserData08WellKnownGuid,
				Guids.UserData09WellKnownGuid,
				Guids.UserData10WellKnownGuid,
				Guids.TransferredGOVGuid,
				Guids.TransferredNSVGuid,
				Guids.TransferredWaterVolumeGuid,
				Guids.TransferredVolumeGuid,
				Guids.TransferStartLevelGuid,
				Guids.TransferStartGOVGuid,
				Guids.TransferStartNSVGuid,
				Guids.TransferStartWaterVolumeGuid,
				Guids.TransferStartVolumeGuid,
				Guids.TransferModeGuid,
				Guids.TransferTargetGuid,
				Guids.TransferLevelTargetGuid,
				Guids.TransferVolumeTargetGuid,
				Guids.TransferTimeRemainingGuid,
				Guids.TransferTimeCompletionGuid,
				Guids.MovementDeviationGuid,
				Guids.MovementPercentDeviationGuid,
				Guids.PercentBSWGuid,
				Guids.VolumeBSWGuid
			};

			var newMovementContainerDictionary = ThreadSharedData.Instance().GetMovementContainers();

			this.SiteList.Clear();

			foreach (var movementContainer in newMovementContainerDictionary.Values)
			{
				if (!this.SiteList.Contains(movementContainer.MovementPoint.SiteGuid))
				{
					this.SiteList.Add(movementContainer.MovementPoint.SiteGuid);
				}

				MovementContainer existingMovementContainer;
				if (this.movementContainerDictionary.TryGetValue(movementContainer.MovementPoint.IdentityGuid, out existingMovementContainer))
				{
					if (existingMovementContainer.MovementPoint.RowVersion.SequenceEqual(movementContainer.MovementPoint.RowVersion))
					{
						continue;
					}
					this.movementContainerDictionary[movementContainer.MovementPoint.IdentityGuid] = movementContainer;
				}
				else
				{
					this.movementContainerDictionary.Add(movementContainer.MovementPoint.IdentityGuid, movementContainer);
				}

				// Get the Point Value Identifier List for this Movement
				var pointGuidList = new List<Guid>();
				pointGuidList.Add(movementContainer.MovementPoint.PointGuid);
				var movementSettingsProperty = movementContainer.MovementPoint.Properties.Values.Single(u => u.ValueTypeString == "FMBusinessObjects.DataObjects.MovementModuleSettings");
				var movementSettings = movementSettingsProperty.Value as MovementModuleSettings;
				foreach (var movementNodeData in movementSettings.MovementNodeDataList)
				{
					pointGuidList.Add(movementNodeData.MovementNodeGuid);
				}

				var pointValueIdentifierList = FMChannelHelper.MakeCall<IPointTags, List<PointValueIdentifier>>
					(x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.security, pointGuidList, wellKnownTagGuidList.ToList()));

				if (movementDataPointValueListDictionary.ContainsKey(movementContainer.MovementPoint.PointGuid))
				{
					movementDataPointValueListDictionary[movementContainer.MovementPoint.PointGuid] = pointValueIdentifierList;
				}
				else
				{
					movementDataPointValueListDictionary.Add(movementContainer.MovementPoint.PointGuid, pointValueIdentifierList);
				}

				InitializeMovementData(movementContainer.MovementPoint);
			}

			var deletedPointsList = new List<Guid>();
			foreach (var movementContainer in this.movementContainerDictionary.Values)
			{
				if (!newMovementContainerDictionary.ContainsKey(movementContainer.MovementPoint.PointGuid))
				{
					deletedPointsList.Add(movementContainer.MovementPoint.PointGuid);
				}
			}

			foreach (var deletedPointGuid in deletedPointsList)
			{
				this.movementDataPointValueListDictionary.Remove(deletedPointGuid);
				this.movementContainerDictionary.Remove(deletedPointGuid);
			}

			// Refresh the Site Time Zone Dictionary
			this.SiteTimeZoneDictionary?.Clear();
		}

		public override void Run()
		{
			try
			{
				this.Initialize();

				WaitHandle[] events = { this.pointChangeEvent };

				while (this.mShutdown != true)
				{
					try
					{
						var eventThatSignaled = WaitHandle.WaitAny(events, processIntervalSecs*1000);

						if (eventThatSignaled == 0)
						{
							lock (this.LockObject)
							{
								this.GetMovements();
							}
						}

						this.UpdateMovements();
					}
					catch (Exception ex)
					{
						Logger.LogError("MovementProcessor Inner Loop Exception: " + ex.Message);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError("MovementProcessor exception: " + ex.Message);
			}
		}
	}
}
