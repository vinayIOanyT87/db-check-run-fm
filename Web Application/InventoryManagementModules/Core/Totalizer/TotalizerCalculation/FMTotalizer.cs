using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Interfaces;
using FMPointCommon;
using System.Collections.Generic;
using Varec.CommonComponents.EngineeringUnitsLibrary;
using Opc.Ua;
using System;

namespace TotalizerCalculation
{
	public class FMTotalizer : FuelsManagerModule, IFuelsManagerModule
	{
		private readonly SecurityClass security = new SecurityClass() { UserID = "FMPointService" };

		public ModuleInputOutputCollection GetInputOutputCollection(string calculationName)
		{
			var properties = new ModuleInputOutputCollection
			{

			};
			return properties;
		}


		public void TotalizerCalculation(
								  PointTag VolumeGrossObserved,
								  PointTag VolumeNetStandard,
								  PointTag PulseMeterNumberOfRollOvers,
								  PointTag PulseMeterLastValue,
								  PointTag PulseMeterCurrentValue,
								  PointTag PulseMeterVolumePerPulse,
								  PointTag PulseMeterRollOverAmount,
								  PointTag PulseMeterLastReadWasRollOver,
								  PointTag VolumeCorrectionFactor)
		{
			_ = PulseMeterVolumePerPulse;

			if (!IsValueGood(PulseMeterCurrentValue))
			{
				return;
			}
			// For MVP we are only supporting a Pulse meter
			bool isPulseMeter = true;
			if (isPulseMeter)
			{
				try
				{
					if (CalcVolumeChange(PulseMeterLastValue, PulseMeterCurrentValue, PulseMeterRollOverAmount, PulseMeterNumberOfRollOvers, PulseMeterLastReadWasRollOver, out double volumeChange))
					{
						List<PointTag> tagList = new List<PointTag>
						  {
								PulseMeterLastValue,
								PulseMeterNumberOfRollOvers,
								PulseMeterLastReadWasRollOver
						  };

						// We use volumeChange since last read in case VCF changes
						if (VolumeGrossObserved.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated && !VolumeGrossObserved.IsForced())
						{
							var grossChange = volumeChange;
							if (PulseMeterCurrentValue.Units != EngineeringUnit.FmuNone && VolumeGrossObserved.Units != EngineeringUnit.FmuNone)
							{
								grossChange = EngineeringUnits.Convert(volumeChange, PulseMeterCurrentValue.Units, VolumeGrossObserved.Units, 15);
							}

							VolumeGrossObserved.Value = ((double)(VolumeGrossObserved.Value ?? 0.0)) + grossChange;
							VolumeGrossObserved.Status = StatusCodes.Good;
							base.SetTimeStamps(new PointTag[] { PulseMeterCurrentValue }, VolumeGrossObserved);
							tagList.Add(VolumeGrossObserved);
						}
						if (VolumeNetStandard.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated && !VolumeNetStandard.IsForced() && IsValueGood(VolumeCorrectionFactor))
						{
							var netChange = volumeChange;
							if (PulseMeterCurrentValue.Units != EngineeringUnit.FmuNone && VolumeNetStandard.Units != EngineeringUnit.FmuNone)
							{
								netChange = EngineeringUnits.Convert(volumeChange, PulseMeterCurrentValue.Units, VolumeNetStandard.Units, 15);
							}

							VolumeNetStandard.Value = ((double)(VolumeNetStandard.Value ?? 0.0)) + (netChange * (double)VolumeCorrectionFactor.Value);
							VolumeNetStandard.Status = StatusCodes.Good;
							base.SetTimeStamps(new PointTag[] { PulseMeterCurrentValue, VolumeCorrectionFactor }, VolumeNetStandard);
							tagList.Add(VolumeNetStandard);
						}
						FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(this.security, tagList, false));
					}

				}
				catch (Exception e)
				{
					if (VolumeGrossObserved != null && VolumeGrossObserved.Status != StatusCodes.Bad)
					{
						VolumeGrossObserved.Status = StatusCodes.Bad;
						FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(this.security, new List<PointTag> { VolumeGrossObserved }, false));
					}
					if (VolumeNetStandard != null && VolumeNetStandard.Status != StatusCodes.Bad)
					{
						VolumeNetStandard.Status = StatusCodes.Bad;
						FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(this.security, new List<PointTag> { VolumeNetStandard }, false));
					}

					LogError("Error in FMTotalizer CalculateVolume ", e, PulseMeterCurrentValue);
				}
			}
		}

		private bool CalcVolumeChange(PointTag LastMeterValue, PointTag CurrentMeterValue, PointTag RollOverAmount,
																PointTag NumberOfRollOvers, PointTag LastReadWasRollOver, out double volumeChange)
		{
			bool hasUpdated = false;
			// The current pulse meter read, as volume
			int currentMeterValue = (int)(CurrentMeterValue.Value ?? 0);

			// The previous pulse meter read, as volume
			int lastMeterValue = (int)(LastMeterValue.Value ?? 0);

			// The last value for meter display before it rolls over back to Zero
			int rollOverAmount = (int)(RollOverAmount.Value ?? 0);

			// Number of times meter has rolled over
			short numberOfRollOvers = (short)(NumberOfRollOvers.Value ?? 0);

			// Was the pervious read a roll over for the meter
			bool lastReadWasRollOver = (bool)(LastReadWasRollOver.Value ?? false);

			// If meter has rolled over between last read and current read.
			bool hasRolledOver = lastMeterValue > currentMeterValue;

			volumeChange = 0;

			if (hasRolledOver && rollOverAmount > 0)
			{
				++numberOfRollOvers;
				// if max meter display value was 9999 to get back to 0 would be 10000 (9999+1) i.e. (rollOverAmount + 1)
				volumeChange = currentMeterValue - lastMeterValue + (rollOverAmount + 1);
				lastReadWasRollOver = true;
				hasUpdated = true;
			}
			// meter value is less but no roll over configured
			else if (hasRolledOver && rollOverAmount == 0.0)
			{
				// just set the value to the current meter value
				volumeChange = currentMeterValue;
				numberOfRollOvers = 0;
				lastReadWasRollOver = false;
				hasUpdated = true;
			}
			else if (lastMeterValue != currentMeterValue || lastReadWasRollOver == true)
			{
				volumeChange = currentMeterValue - lastMeterValue;
				lastReadWasRollOver = false;
				hasUpdated = true;
			}

			if (hasUpdated)
			{
				// store the values
				LastMeterValue.Value = currentMeterValue;
				LastMeterValue.Status = StatusCodes.Good;
				LastReadWasRollOver.Value = lastReadWasRollOver;
				LastReadWasRollOver.Status = StatusCodes.Good;
				NumberOfRollOvers.Value = numberOfRollOvers;
				NumberOfRollOvers.Status = StatusCodes.Good;
				base.SetTimeStamps(new PointTag[] { CurrentMeterValue }, LastMeterValue);
				base.SetTimeStamps(new PointTag[] { CurrentMeterValue }, NumberOfRollOvers);
				base.SetTimeStamps(new PointTag[] { CurrentMeterValue }, LastReadWasRollOver);
			}
			return hasUpdated;
		}
	}
}
