
namespace AvailableAndRemainingVolume
{
	using System;
	using System.Collections.Generic;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using FMPointCommon;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.BusinessInterfaces;
	using FloatingRoofCorrection;
	using StrapTables;
	using Quantities;
	using Opc.Ua;

	public class FMAvailableAndRemainingVolume : FuelsManagerModule, IFuelsManagerModule
	{
		public FMFloatingRoofCorrection RoofCorrection { get; set; }
		public FMStrapTable StrapTable { get; set; }
		public FMQuantities Quantities { get; set; }

		public FMAvailableAndRemainingVolume() : base() { }


		public ModuleInputOutputCollection GetInputOutputCollection(string calculationName)
		{
			var properties = new ModuleInputOutputCollection
								{
									new ModuleInputOutput
									{
											ID = "Available Volume",
											Type = typeof(double?),
											ParameterType = ModuleInputOutputType.Input
									}
								};
			return properties;
		}



		public void AvailableAndRemainingVolumeCalculation(
				PointTag levelMinOpLimit, PointTag levelMaxOpLimit,
				PointTag volumeStrapProduct, PointTag volumeRoofCorrection, PointTag temperatureProduct, PointTag densityObserved, PointTag densityStandardInAir, PointTag mass, PointTag volumeCorrectionFactor, PointTag volumeBottom, PointTag criticalZone,
				PointTag volumeStrapWater, PointTag volumeStrapSolids, PointTag percentBSW, PointTag tankShellCorrection, PointTag volumeGrossObserved, PointTag volumeNetStandard,
				PointTag volumeGOVAvailable, PointTag volumeNSVAvailable, PointTag volumeGOVRemaining, PointTag volumeNSVRemaining)
		{
			PointTag volumeTotalObservedMax = new PointTag();
			PointTag volumeTotalObservedMin = new PointTag();
			PointTag volumeRoofCorrectionMin = new PointTag();
			PointTag volumeRoofCorrectionMax = new PointTag();
			PointTag volumeGrossObservedMin = new PointTag();
			PointTag volumeGrossObservedMax = new PointTag();
			PointTag volumeNetStandardMin = new PointTag();
			PointTag volumeNetStandardMax = new PointTag();
			PointTag criticalZoneTemp = new PointTag();

			volumeTotalObservedMin.Units = volumeStrapProduct.Units;
			volumeTotalObservedMin.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			volumeTotalObservedMax.Units = volumeStrapProduct.Units;
			volumeTotalObservedMax.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			volumeRoofCorrectionMin.Units = volumeRoofCorrection.Units;
			volumeRoofCorrectionMin.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			volumeRoofCorrectionMax.Units = volumeRoofCorrection.Units;
			volumeRoofCorrectionMax.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			volumeGrossObservedMin.Units = volumeGrossObserved.Units;
			volumeGrossObservedMin.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			volumeGrossObservedMax.Units = volumeGrossObserved.Units;
			volumeGrossObservedMax.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			volumeNetStandardMin.Units = volumeNetStandard.Units;
			volumeNetStandardMin.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			volumeNetStandardMax.Units = volumeNetStandard.Units;
			volumeNetStandardMax.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
			criticalZoneTemp.Units = criticalZone.Units;
			criticalZoneTemp.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;

			if ((levelMinOpLimit != null) && (levelMinOpLimit.Value != null))
			{
				this.StrapTable.StrapVolumeCalculation(levelMinOpLimit, volumeTotalObservedMin);
				this.RoofCorrection.FloatingRoofCorrectionCalculation(temperatureProduct, densityObserved, densityStandardInAir, mass, levelMinOpLimit, volumeCorrectionFactor, criticalZoneTemp, volumeRoofCorrectionMin);
			}
			if ((levelMaxOpLimit != null) && (levelMaxOpLimit.Value != null))
			{
				this.StrapTable.StrapVolumeCalculation(levelMaxOpLimit, volumeTotalObservedMax);
				this.RoofCorrection.FloatingRoofCorrectionCalculation(temperatureProduct, densityObserved, densityStandardInAir, mass, levelMaxOpLimit, volumeCorrectionFactor, criticalZoneTemp, volumeRoofCorrectionMax);
			}
			if (volumeTotalObservedMin.Value != null)
			{
				PointTag volumeBottomTemp = new PointTag();
				volumeBottomTemp.Units = volumeBottom.Units;
				volumeBottomTemp.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
				this.Quantities.QuantityCalculationVolumeOnly(volumeRoofCorrectionMin, volumeTotalObservedMin, volumeStrapWater, volumeStrapSolids, percentBSW, volumeCorrectionFactor, tankShellCorrection,
															volumeBottomTemp,
															volumeGrossObservedMin, volumeNetStandardMin);
			}
			if (volumeTotalObservedMax.Value != null)
			{
				PointTag volumeBottomTemp = new PointTag();
				volumeBottomTemp.Units = volumeBottom.Units;
				volumeBottomTemp.InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated;
				this.Quantities.QuantityCalculationVolumeOnly(volumeRoofCorrectionMax, volumeTotalObservedMax, volumeStrapWater, volumeStrapSolids, percentBSW, volumeCorrectionFactor, tankShellCorrection,
															volumeBottomTemp,
															volumeGrossObservedMax, volumeNetStandardMax);
			}

			SetVolumeGOVAvailable(volumeGrossObserved, volumeGrossObservedMin, ref volumeGOVAvailable);
			SetVolumeNSVAvailable(volumeNetStandard, volumeNetStandardMin, ref volumeNSVAvailable);
			SetVolumeGOVRemaining(volumeGrossObserved, volumeGrossObservedMax, ref volumeGOVRemaining);
			SetVolumeNSVRemaining(volumeNetStandard, volumeNetStandardMax, ref volumeNSVRemaining);
		}


		private bool IsAlarmTestEnabled(PointTag targetTag, PointTag referenceTag)
		{
			bool alarmMatch = false;
			if (!targetTag.AlarmsEnabled)
				return false;

			if (!referenceTag.AlarmsEnabled)
				return false;

			foreach (KeyValuePair<Guid, Alarm> kvpAlarm in referenceTag.Alarms)
			{
				if (alarmMatch)
					break;
				Alarm alarm = kvpAlarm.Value;
				foreach (KeyValuePair<Guid, AlarmTest> kvpAlarmTest in alarm.AlarmTests)
				{
					AlarmTest alarmTest = kvpAlarmTest.Value;
					if (alarmTest.LimitTagGuid == targetTag.PointTagGuid)
					{
						alarmMatch = true;
						if (!alarmTest.Enabled)
							return false;
						if (!alarm.Enabled)
							return false;
						break;
					}
				}
			}

			return true;
		}


		private void SetVolumeGOVAvailable(PointTag volumeGrossObserved, PointTag volumeGrossObservedMin, ref PointTag volumeGOVAvailable)
		{
			if (volumeGOVAvailable.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				volumeGOVAvailable.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}
			if ((volumeGrossObserved == null) || (volumeGrossObserved.Value == null) || (!IsValueGood(volumeGrossObserved))
					|| (volumeGrossObservedMin == null) || (volumeGrossObservedMin.Value == null) || (!IsValueGood(volumeGrossObservedMin)))
			{
				if (volumeGOVAvailable.Value != null ||
					volumeGOVAvailable.Status != StatusCodes.Bad)
				{
					volumeGOVAvailable.Value = null;
					volumeGOVAvailable.Status = StatusCodes.Bad;
					 base.SetTimeStamps(new PointTag[] { volumeGrossObserved, volumeGrossObservedMin }, volumeGOVAvailable);
				}
				return;
			}

			double newValue = 0;
			if ((double)volumeGrossObserved.Value > (double)volumeGrossObservedMin.Value)
				newValue = (double)volumeGrossObserved.Value - (double)volumeGrossObservedMin.Value;

			long newStatus = StatusCodes.Good;
			if (IsStatusUncertain(volumeGrossObserved))
			{
				newStatus = StatusCodes.Uncertain;
			}
			if (volumeGOVAvailable.Value == null ||
				(double)volumeGOVAvailable.Value != newValue
				|| IsStatusChange(volumeGOVAvailable.Status, newStatus))
			{
				volumeGOVAvailable.Value = newValue;
				volumeGOVAvailable.Status = newStatus;
				CheckForAndSetOverUnderRange(volumeGOVAvailable);
				volumeGOVAvailable.SourceTimeStamp = volumeGrossObserved.SourceTimeStamp;
				volumeGOVAvailable.ServerTimeStamp = volumeGrossObserved.ServerTimeStamp;
			}
		}


		private void SetVolumeNSVAvailable(PointTag volumeNetStandard, PointTag volumeNetStandardMin, ref PointTag volumeNSVAvailable)
		{
			if (volumeNSVAvailable.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				volumeNSVAvailable.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}
			if ((volumeNetStandard == null) || (volumeNetStandard.Value == null) || (!IsValueGood(volumeNetStandard))
					|| (volumeNetStandardMin == null) || (volumeNetStandardMin.Value == null) || (!IsValueGood(volumeNetStandardMin)))
			{
				if (volumeNSVAvailable.Value != null ||
					volumeNSVAvailable.Status != StatusCodes.Bad)
				{
					volumeNSVAvailable.Value = null;
					volumeNSVAvailable.Status = StatusCodes.Bad;
               base.SetTimeStamps(new PointTag[] { volumeNetStandard, volumeNetStandardMin }, volumeNSVAvailable);
                }
				return;
			}

			double newValue = 0;
			if ((double)volumeNetStandard.Value > (double)volumeNetStandardMin.Value)
				newValue = (double)volumeNetStandard.Value - (double)volumeNetStandardMin.Value;

			long newStatus = StatusCodes.Good;
			if (IsStatusUncertain(volumeNetStandard))
			{
				newStatus = StatusCodes.Uncertain;
			}
			if (volumeNSVAvailable.Value == null ||
				(double)volumeNSVAvailable.Value != newValue
				|| IsStatusChange(volumeNSVAvailable.Status, newStatus))
			{
				volumeNSVAvailable.Value = newValue;
				volumeNSVAvailable.Status = newStatus;
				CheckForAndSetOverUnderRange(volumeNSVAvailable);
				volumeNSVAvailable.SourceTimeStamp = volumeNetStandard.SourceTimeStamp;
				volumeNSVAvailable.ServerTimeStamp = volumeNetStandard.ServerTimeStamp;
			}
		}


		private void SetVolumeGOVRemaining(PointTag volumeGrossObserved, PointTag volumeGrossObservedMax, ref PointTag volumeGOVRemaining)
		{
			if (volumeGOVRemaining.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				volumeGOVRemaining.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}
			if ((volumeGrossObserved == null) || (volumeGrossObserved.Value == null) || (!IsValueGood(volumeGrossObserved))
					|| (volumeGrossObservedMax == null) || (volumeGrossObservedMax.Value == null) || (!IsValueGood(volumeGrossObservedMax)))
			{
				if (volumeGOVRemaining.Value != null ||
					volumeGOVRemaining.Status != StatusCodes.Bad)
				{
					volumeGOVRemaining.Value = null;
					volumeGOVRemaining.Status = StatusCodes.Bad;
               base.SetTimeStamps(new PointTag[] { volumeGrossObserved, volumeGrossObservedMax }, volumeGOVRemaining);
                }
				return;
			}

			double newValue = 0;
			if ((double)volumeGrossObserved.Value < (double)volumeGrossObservedMax.Value)
				newValue = (double)volumeGrossObservedMax.Value - (double)volumeGrossObserved.Value;

			long newStatus = StatusCodes.Good;
			if (IsStatusUncertain(volumeGrossObserved))
			{
				newStatus = StatusCodes.Uncertain;
			}
			if (volumeGOVRemaining.Value == null ||
				(double)volumeGOVRemaining.Value != newValue
				|| IsStatusChange(volumeGOVRemaining.Status, newStatus))
			{
				volumeGOVRemaining.Value = newValue;
				volumeGOVRemaining.Status = newStatus;
				CheckForAndSetOverUnderRange(volumeGOVRemaining);
				volumeGOVRemaining.SourceTimeStamp = volumeGrossObserved.SourceTimeStamp;
				volumeGOVRemaining.ServerTimeStamp = volumeGrossObserved.ServerTimeStamp;
			}
		}


		private void SetVolumeNSVRemaining(PointTag volumeNetStandard, PointTag volumeNetStandardMax, ref PointTag volumeNSVRemaining)
		{
			if (volumeNSVRemaining.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				volumeNSVRemaining.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}
			if ((volumeNetStandard == null) || (volumeNetStandard.Value == null) || (!IsValueGood(volumeNetStandard))
					|| (volumeNetStandardMax == null) || (volumeNetStandardMax.Value == null) || (!IsValueGood(volumeNetStandardMax)))
			{
				if (volumeNSVRemaining.Value != null ||
					volumeNSVRemaining.Status != StatusCodes.Bad)
				{
					volumeNSVRemaining.Value = null;
					volumeNSVRemaining.Status = StatusCodes.Bad;
               base.SetTimeStamps(new PointTag[] { volumeNetStandard, volumeNetStandardMax }, volumeNSVRemaining);
                }
				return;
			}

			double newValue = 0;
			if ((double)volumeNetStandard.Value < (double)volumeNetStandardMax.Value)
				newValue = (double)volumeNetStandardMax.Value - (double)volumeNetStandard.Value;

			long newStatus = StatusCodes.Good;
			if (IsStatusUncertain(volumeNetStandard))
			{
				newStatus = StatusCodes.Uncertain;
			}
			if (volumeNSVRemaining.Value == null ||
				(double)volumeNSVRemaining.Value != newValue
				|| IsStatusChange(volumeNSVRemaining.Status, newStatus))
			{
				volumeNSVRemaining.Value = newValue;
				volumeNSVRemaining.Status = newStatus;
				CheckForAndSetOverUnderRange(volumeNSVRemaining);
				volumeNSVRemaining.SourceTimeStamp = volumeNetStandard.SourceTimeStamp;
				volumeNSVRemaining.ServerTimeStamp = volumeNetStandard.ServerTimeStamp;
			}
		}

	}
}
