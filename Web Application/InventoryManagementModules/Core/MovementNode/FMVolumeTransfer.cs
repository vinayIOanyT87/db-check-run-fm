namespace VolumeTransfer
{
	using System;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using FMPointCommon;
	using System.Collections.Generic;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.BusinessInterfaces;
	using Opc.Ua;


	public class FMVolumeTransfer : FuelsManagerModule, IFuelsManagerModule
	{
		public VolumeTransferModuleSettings VolumeTransferSettings { get; set; }

		private VolumeTransferMode? currentTransferMode;

		private double? currentTransferTarget;

		private bool firstTimeFlag = true;

		public SetPointTagHandler SetPointTag = null;

		public SetPointPropertyHandler SetPointProperty = null;

		public void TransferCalculation(
			PointTag VolumeGrossObserved, PointTag VolumeGrossObservedRate,
			PointTag VolumeNetStandard, PointTag VolumeNetStandardRate,
			PointTag TransferMode, PointTag TransferStatus, PointTag TransferTarget,
			PointTag TransferStartGOV, PointTag TransferStartNSV, PointTag TransferStartVolume, PointTag TransferTimeRemaining,
			PointTag TransferTimeCompletion, PointTag TransferredGOV, PointTag TransferredNSV, PointTag TransferredVolume,
			PointTag VolumeTransferDiscreteAlarms, PointTag TransferStartTime, PointTag TransferStopTime,
			PointTag TransferVolumeTarget)
		{

			CalculateTransferInactive(	TransferMode, TransferTimeRemaining, TransferTimeCompletion,
												TransferStartGOV, TransferStartNSV, TransferStartVolume,
												TransferredGOV, TransferredNSV, TransferredVolume,
												TransferStatus, TransferTarget,
												TransferStartTime, TransferStopTime,
												TransferVolumeTarget);

			CalculateTransferTarget(TransferMode,
											VolumeGrossObserved,
											VolumeNetStandard,
											TransferStartGOV, TransferStartNSV,
											TransferStartVolume,
											TransferStatus, TransferTarget,
											TransferStartTime, TransferStopTime,
											TransferVolumeTarget);


			CalculateTransferInProgress(TransferMode, TransferStatus,
													VolumeGrossObserved, VolumeNetStandard,
													TransferStartGOV, TransferStartNSV,
													VolumeGrossObservedRate, VolumeNetStandardRate,
													TransferredGOV, TransferredNSV, TransferredVolume, TransferTarget,
													TransferTimeRemaining, TransferTimeCompletion,
													VolumeTransferDiscreteAlarms,
													TransferVolumeTarget);

			CalculateTransferAlarms(TransferMode, TransferStatus, TransferTarget, TransferStartGOV, VolumeGrossObservedRate, TransferTimeRemaining, VolumeTransferDiscreteAlarms);


			this.currentTransferMode = (VolumeTransferMode?) TransferMode.Value;


			firstTimeFlag = false;

		}

		private void CalculateTransferInactive(PointTag TransferMode, PointTag TransferTimeRemaining, PointTag TransferTimeCompletion,
															PointTag TransferStartGOV, PointTag TransferStartNSV, PointTag TransferStartVolume,
															PointTag TransferredGOV, PointTag TransferredNSV, PointTag TransferredVolume,
															PointTag TransferStatus, PointTag TransferTarget,
															PointTag TransferStartTime, PointTag TransferStopTime,
															PointTag TransferVolumeTarget)
		{
			if (TransferStatus.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferStatus.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(TransferMode))
			{
				return;
			}


			if ((VolumeTransferMode)TransferMode.Value != FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode.Inactive)
			{
				return;
			}

			List<PointTag> tagList = new List<PointTag>();

			var newValue = TransferStatuses.Inactive;
			var newStatus = StatusCodes.Good;

			if (!this.firstTimeFlag
			&& this.currentTransferMode != (VolumeTransferMode)TransferMode.Value)
			{
				tagList.Add(TransferMode);
			}

			if (TransferStatus.Value == null
			|| (TransferStatuses)TransferStatus.Value != newValue
			|| IsStatusChange(TransferStatus.Status, newStatus))
			{
				TransferStatus.Value = newValue;
				TransferStatus.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferStatus);
				tagList.Add(TransferStatus);
			}

			// When TransferMode is set to Inactive, clear the Target
			if (this.currentTransferMode != (VolumeTransferMode)TransferMode.Value
			&& (VolumeTransferMode)TransferMode.Value == FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode.Inactive)
			{
				TransferTarget.Value = null;
				TransferTarget.Status = newStatus;
				TransferTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
				TransferTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
				tagList.Add(TransferTarget);

				TransferVolumeTarget.Value = null;
				TransferVolumeTarget.Status = newStatus;
				TransferVolumeTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
				TransferVolumeTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
				tagList.Add(TransferVolumeTarget);

				SetPointTag(TransferTarget);
				SetPointTag(TransferVolumeTarget);
				this.currentTransferTarget = null;
			}

			if (TransferStartGOV.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& (TransferStartGOV.Value != null
			|| IsStatusChange(TransferStartGOV.Status, newStatus)))
			{
				TransferStartGOV.Value = null;
				TransferStartGOV.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferStartGOV);
				tagList.Add(TransferStartGOV);
			}

			if (TransferStartNSV.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& (TransferStartNSV.Value != null
			|| IsStatusChange(TransferStartNSV.Status, newStatus)))
			{
				TransferStartNSV.Value = null;
				TransferStartNSV.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferStartNSV);
				tagList.Add(TransferStartNSV);
			}

			if (TransferStartVolume.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& (TransferStartVolume.Value != null
			|| IsStatusChange(TransferStartVolume.Status, newStatus)))
			{
				TransferStartVolume.Value = null;
				TransferStartVolume.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferStartVolume);
				tagList.Add(TransferStartVolume);
			}


			if (TransferredGOV.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& (TransferredGOV.Value != null
			|| IsStatusChange(TransferredGOV.Status, newStatus)))
			{
				TransferredGOV.Value = null;
				TransferredGOV.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferredGOV);
				tagList.Add(TransferredGOV);
			}

			if (TransferredNSV.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& (TransferredNSV.Value != null
			|| IsStatusChange(TransferredNSV.Status, newStatus)))
			{
				TransferredNSV.Value = null;
				TransferredNSV.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferredNSV);
				tagList.Add(TransferredNSV);
			}


			if (TransferredVolume.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& (TransferredVolume.Value != null
			|| IsStatusChange(TransferredVolume.Status, newStatus)))
			{
				TransferredVolume.Value = null;
				TransferredVolume.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferredVolume);
				tagList.Add(TransferredVolume);
			}

			if (TransferStartTime.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& (TransferStartTime.Value != null
			|| IsStatusChange(TransferStartTime.Status, newStatus)))
			{
				TransferStartTime.Value = null;
				TransferStartTime.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferStartTime);
				tagList.Add(TransferStartTime);
			}

			if (TransferStopTime.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& (TransferStopTime.Value == null
			|| IsStatusChange(TransferStartTime.Status, newStatus)))
			{
				TransferStopTime.Value = DateTimeOffset.UtcNow;
				TransferStopTime.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferStopTime);
				tagList.Add(TransferStopTime);

			}


			if (TransferTimeRemaining.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& (TransferTimeRemaining.Value != null
			|| IsStatusChange(TransferTimeRemaining.Status, newStatus)))
			{
				TransferTimeRemaining.Value = null;
				TransferTimeRemaining.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferTimeRemaining);
				tagList.Add(TransferTimeRemaining);
			}

			if (TransferTimeCompletion.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& (TransferTimeCompletion.Value != null
			|| IsStatusChange(TransferTimeCompletion.Status, newStatus)))
			{
				TransferTimeCompletion.Value = null;
				TransferTimeCompletion.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferTimeCompletion);
				tagList.Add(TransferTimeCompletion);
			}

			if (tagList.Count > 0)
			{
				var security = new SecurityClass() { UserID = "FMPointService" };
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
			}
		}

		private void CalculateTransferTarget(
			PointTag TransferMode,
			PointTag VolumeGrossObserved,
			PointTag VolumeNetStandard,
			PointTag TransferStartGOV, PointTag TransferStartNSV,
			PointTag TransferStartVolume,
			PointTag TransferStatus, PointTag TransferTarget,
			PointTag TransferStartTime, PointTag TransferStopTime,
			PointTag TransferVolumeTarget)
		{
			if (TransferStatus.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferStatus.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(TransferMode)
			|| !IsValueGood(VolumeGrossObserved)
			|| !IsValueGood(VolumeNetStandard)
			|| !IsValueGood(TransferStatus))
			{
				return;
			}


			if ((VolumeTransferMode)TransferMode.Value == FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode.Inactive)
			{
				return;
			}

			if ((TransferStatuses)TransferStatus.Value == TransferStatuses.InProgress
			|| (TransferStatuses)TransferStatus.Value == TransferStatuses.Complete)
			{
				return;
			}

			List<PointTag> tagList = new List<PointTag>();

			// Test for initial Target to signal transition to TransferTarget
			if ((TransferStatuses)TransferStatus.Value == TransferStatuses.Inactive)
			{
				TransferStatus.Value = TransferStatuses.TransferTarget;
				TransferStatus.Status = StatusCodes.Good;
				TransferStatus.SourceTimeStamp = TransferMode.SourceTimeStamp;
				TransferStatus.ServerTimeStamp = TransferMode.ServerTimeStamp;

				tagList.Add(TransferStatus);

				TransferStopTime.Value = null;
				TransferStopTime.Status = StatusCodes.Good;
				TransferStopTime.SourceTimeStamp = TransferMode.SourceTimeStamp;
				TransferStopTime.ServerTimeStamp = TransferMode.ServerTimeStamp;

				tagList.Add(TransferStopTime);
			}

			// Test for a change in Transfer Target to signal transition to InProgress
			if (IsValueGood(TransferTarget)
			&& (TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget
			&& TransferTarget.Value != null
			&& (double)TransferTarget.Value != this.currentTransferTarget)
			{
				tagList.Add(TransferMode);
				tagList.Add(TransferTarget);
				SetPointTag(TransferTarget);

				TransferVolumeTarget.Value = TransferTarget.Value;
				TransferVolumeTarget.Status = TransferTarget.Status;
				TransferVolumeTarget.ServerTimeStamp = TransferTarget.ServerTimeStamp;
				TransferVolumeTarget.SourceTimeStamp = TransferTarget.SourceTimeStamp;

				tagList.Add(TransferVolumeTarget);

				TransferStatus.Value = TransferStatuses.InProgress;
				TransferStatus.Status = StatusCodes.Good;
				TransferStatus.SourceTimeStamp = TransferTarget.SourceTimeStamp;
				TransferStatus.ServerTimeStamp = TransferTarget.ServerTimeStamp;

				if (!tagList.Contains(TransferStatus))
				{
					tagList.Add(TransferStatus);
				}

				if (TransferStartTime.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
				&& TransferStartTime.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					TransferStartTime.Value = DateTimeOffset.UtcNow;
					TransferStartTime.Status = StatusCodes.Good;
					TransferStartTime.SourceTimeStamp = DateTimeOffset.UtcNow;
					TransferStartTime.ServerTimeStamp = DateTimeOffset.UtcNow;
					tagList.Add(TransferStartTime);
				}

				if (TransferStopTime.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
				&& TransferStopTime.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					TransferStopTime.Value = null;
					TransferStopTime.Status = StatusCodes.Good;
					TransferStopTime.SourceTimeStamp = DateTimeOffset.UtcNow;
					TransferStopTime.ServerTimeStamp = DateTimeOffset.UtcNow;
					tagList.Add(TransferStopTime);
				}


				if (TransferStartGOV.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
				&& TransferStartGOV.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					TransferStartGOV.Value = VolumeGrossObserved.Value;
					TransferStartGOV.Status = StatusCodes.Good;
					TransferStartGOV.SourceTimeStamp = DateTimeOffset.UtcNow;
					TransferStartGOV.ServerTimeStamp = DateTimeOffset.UtcNow;
					tagList.Add(TransferStartGOV);
				}

				if (TransferStartNSV.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
				&& TransferStartNSV.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					TransferStartNSV.Value = VolumeNetStandard.Value;
					TransferStartNSV.Status = StatusCodes.Good;
					TransferStartNSV.SourceTimeStamp = DateTimeOffset.UtcNow;
					TransferStartNSV.ServerTimeStamp = DateTimeOffset.UtcNow;
					tagList.Add(TransferStartNSV);
				}


				if (TransferStartVolume.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
				&& TransferStartVolume.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					if (this.VolumeTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
					{
						TransferStartVolume.Value = VolumeGrossObserved.Value;
						TransferStartVolume.Status = StatusCodes.Good;
						TransferStartVolume.SourceTimeStamp = DateTimeOffset.UtcNow;
						TransferStartVolume.ServerTimeStamp = DateTimeOffset.UtcNow;
						tagList.Add(TransferStartVolume);
					}
					else
					{
						TransferStartVolume.Value = VolumeNetStandard.Value;
						TransferStartVolume.Status = StatusCodes.Good;
						TransferStartVolume.SourceTimeStamp = DateTimeOffset.UtcNow;
						TransferStartVolume.ServerTimeStamp = DateTimeOffset.UtcNow;
						tagList.Add(TransferStartVolume);
					}
				}


				if (tagList.Count > 0)
				{
					var security = new SecurityClass() { UserID = "FMPointService" };
					FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
				}

				this.VolumeTransferSettings.CurrentTransferVolumeMode = this.VolumeTransferSettings.TransferVolumeMode;
				this.SetPointProperty("VolumeTransferSettings");
			}


			if (!this.firstTimeFlag
			&& this.currentTransferMode != (VolumeTransferMode)TransferMode.Value)
			{
				this.currentTransferMode = (VolumeTransferMode)TransferMode.Value;
				tagList.Add(TransferMode);
				tagList.Add(TransferTarget);
			}

			// Keep Target up to date with changes to Level Product, Maximum, Minimum, and Transfer Mode
			if(CalculateTransferTargetLimits(TransferMode, TransferStatus, TransferTarget, VolumeGrossObserved, VolumeNetStandard, TransferVolumeTarget))
			{
				SetPointTag(TransferTarget);
				SetPointTag(TransferVolumeTarget);
				if(!tagList.Contains(TransferTarget))
				{
					tagList.Add(TransferTarget);
					tagList.Add(TransferVolumeTarget);
				}
			}

			if (tagList.Count > 0)
			{
				var security = new SecurityClass() { UserID = "FMPointService" };
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
			}

			this.currentTransferTarget = (double)TransferTarget.Value;
		}


		private bool CalculateTransferTargetLimits(	PointTag TransferMode, PointTag TransferStatus, PointTag TransferTarget,
																	PointTag VolumeGrossObserved, PointTag VolumeNetStandard, PointTag TransferVolumeTarget)
		{

			if (this.VolumeTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
			{
				if (((TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget
				&& (double?)TransferTarget.Value != 0.0)
				|| TransferTarget.EngineeringUnitsType != VolumeGrossObserved.EngineeringUnitsType
				|| TransferTarget.Units != VolumeGrossObserved.Units
				|| TransferTarget.DecimalPlaces != VolumeGrossObserved.DecimalPlaces
				|| (TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget
				&& (TransferTarget.Maximum != VolumeGrossObserved.Maximum
				|| TransferTarget.Minimum != VolumeGrossObserved.Minimum))
				{
					if ((TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget)
					{
						TransferTarget.Value = 0.0;
						TransferTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
						TransferTarget.SourceTimeStamp = DateTimeOffset.UtcNow;

						TransferVolumeTarget.Value = 0.0;
						TransferVolumeTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
						TransferVolumeTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
					}

					TransferTarget.EngineeringUnitsType = VolumeGrossObserved.EngineeringUnitsType;
					TransferTarget.Units = VolumeGrossObserved.Units;
					TransferTarget.DecimalPlaces = VolumeGrossObserved.DecimalPlaces;

					// if the transfer is inprogress or completed we do not want to reset the min and max values.
					// this will cause the value to be displayed as over/under range and if the setpoint is changed
					// will have the values at the time it is being set not the initial values which is what we want
					if ((TransferStatuses)TransferStatus.Value != TransferStatuses.InProgress &&
						(TransferStatuses)TransferStatus.Value != TransferStatuses.Complete)
					{
						// this is a manualy entered value which will be limited by the decimal precision. This is also being updated
						// by a process variable max and min that is not being limited. In order to prevent invalid over and under range
						// indications we need to format the min and max to the designated precision.
						TransferTarget.Maximum = VolumeGrossObserved.Maximum;
						TransferTarget.Minimum = VolumeGrossObserved.Minimum;
					}

					return true;
				}
			}
			else if (this.VolumeTransferSettings.TransferVolumeMode == TransferVolumeMode.NetStandardVolume)
			{
				if (((TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget
				&& (double?)TransferTarget.Value != 0.0)
				|| TransferTarget.EngineeringUnitsType != VolumeNetStandard.EngineeringUnitsType
				|| TransferTarget.Units != VolumeNetStandard.Units
				|| TransferTarget.DecimalPlaces != VolumeNetStandard.DecimalPlaces
				|| (TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget
				&& (TransferTarget.Maximum != VolumeNetStandard.Maximum
				|| TransferTarget.Minimum != VolumeNetStandard.Minimum))
				{
					if ((TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget)
					{
						TransferTarget.Value = 0.0;
						TransferTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
						TransferTarget.SourceTimeStamp = DateTimeOffset.UtcNow;

						TransferVolumeTarget.Value = 0.0;
						TransferVolumeTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
						TransferVolumeTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
					}

					TransferTarget.EngineeringUnitsType = VolumeNetStandard.EngineeringUnitsType;
					TransferTarget.Units = VolumeNetStandard.Units;
					TransferTarget.DecimalPlaces = VolumeNetStandard.DecimalPlaces;

					// if the transfer is inprogress or completed we do not want to reset the min and max values.
					// this will cause the value to be displayed as over/under range and if the setpoint is changed
					// will have the values at the time it is being set not the initial values which is what we want
					if ((TransferStatuses)TransferStatus.Value != TransferStatuses.InProgress &&
						(TransferStatuses)TransferStatus.Value != TransferStatuses.Complete)
					{
						// this is a manualy entered value which will be limited by the decimal precision. This is also being updated
						// by a process variable max and min that is not being limited. In order to prevent invalid over and under range
						// indications we need to format the min and max to the designated precision.
						TransferTarget.Maximum = VolumeNetStandard.Maximum;
						TransferTarget.Minimum = VolumeNetStandard.Minimum;
					}

					return true;
				}
			}

			return false;
		}



		private void CalculateTransferInProgress(PointTag TransferMode, PointTag TransferStatus,
													PointTag VolumeGrossObserved, PointTag VolumeNetStandard, 
													PointTag TransferStartGOV, PointTag TransferStartNSV,
													PointTag VolumeGrossObservedRate, PointTag VolumeNetStandardRate,
													PointTag TransferredGOV, PointTag TransferredNSV, PointTag TransferredVolume, PointTag TransferTarget,
													PointTag TransferTimeRemaining, PointTag TransferTimeCompletion,
													PointTag AlarmState, PointTag TransferVolumeTarget)
		{
			if (!IsValueGood(TransferMode)
			|| !IsValueGood(TransferStatus))
			{
				return;
			}

			if ((TransferStatuses)TransferStatus.Value != TransferStatuses.InProgress &&
				(TransferStatuses)TransferStatus.Value != TransferStatuses.Complete) // this is required to continue to calculate transfered volume changes
			{
				return;
			}

			// Initial Execution
			if (this.currentTransferMode == null)
			{
				this.currentTransferMode = (VolumeTransferMode)TransferMode.Value;
			}

			List<PointTag> tagList = new List<PointTag>();

			if (this.currentTransferMode != (VolumeTransferMode)TransferMode.Value)
			{
				this.currentTransferMode = (VolumeTransferMode)TransferMode.Value;

				var security = new SecurityClass() { UserID = "FMPointService" };

				tagList.Add(TransferMode);
			}
			else if(this.currentTransferTarget != (double)TransferTarget.Value)
			{
				SetPointTag(TransferTarget);
				tagList.Add(TransferTarget);

				TransferVolumeTarget.Value = TransferTarget.Value;
				TransferVolumeTarget.Status = TransferTarget.Status;
				TransferVolumeTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
				TransferVolumeTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
				SetPointTag(TransferVolumeTarget);
				tagList.Add(TransferVolumeTarget);

			}

			this.currentTransferTarget = (double)TransferTarget.Value;

			if (tagList.Count > 0)
			{
				var security = new SecurityClass() { UserID = "FMPointService" };
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
			}

			if (!IsValueGood(VolumeGrossObserved)
			|| !IsValueGood(VolumeGrossObservedRate)
			|| !IsValueGood(VolumeNetStandard)
			|| !IsValueGood(VolumeNetStandardRate)
			|| !IsValueGood(TransferTarget))
			{
				return;
			}

			CalculateTransferredGrossObservedVolume(TransferStatus, VolumeGrossObserved, TransferStartGOV, TransferredGOV);

			CalculateTransferredNetStandardVolume(TransferStatus, VolumeNetStandard, TransferStartNSV, TransferredNSV);

			CalculateTransferredVolume(TransferStatus, TransferredGOV, TransferredNSV, TransferredVolume);

			CalculateTimeRemaining(TransferMode, TransferStatus, VolumeGrossObserved, VolumeNetStandard, VolumeGrossObservedRate, VolumeNetStandardRate, TransferTarget, TransferredGOV, TransferredNSV, TransferTimeRemaining);

			CalculateTransferTimeComplete(TransferTimeRemaining, TransferTimeCompletion, TransferStatus);


			CalculateTransferCompletion(	TransferMode, TransferTarget, VolumeGrossObserved, VolumeNetStandard, 
													TransferStartGOV, TransferStartNSV,
													TransferredGOV, TransferredNSV,
													VolumeGrossObservedRate, VolumeNetStandardRate, 
													TransferStatus, TransferTimeRemaining, TransferTimeCompletion);
		}


		private void CalculateTransferAlarms(	PointTag TransferMode, PointTag TransferStatus, PointTag TransferTarget,
															PointTag TransferStartGOV, PointTag VolumeGrossObservedRate, PointTag TransferTimeRemaining, PointTag VolumeTransferDiscreteAlarms)
		{
			if (VolumeTransferDiscreteAlarms.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			VolumeTransferDiscreteAlarms.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(TransferMode)
			|| !IsValueGood(TransferStatus)
			|| !IsValueGood(TransferTarget)
			|| !IsValueGood(TransferStartGOV)
			|| !IsValueGood(VolumeGrossObservedRate))
			{
				return;
			}



			short newValue = 0;
			var newStatus = StatusCodes.Good;

			if ((TransferStatuses)TransferStatus.Value == TransferStatuses.InProgress)
			{
				if (TransferTimeRemaining.Value is TimeSpan
				&& ((TimeSpan)TransferTimeRemaining.Value).TotalMinutes < this.VolumeTransferSettings.TransferAdvisoryTime)
				{
					newValue = 0x1;
				}

				if((double)TransferTarget.Value > 0
				&& (double)VolumeGrossObservedRate.Value < 0)
				{
					newValue |= 0x4;
				}

				if ((double)TransferTarget.Value < 0
				&& (double)VolumeGrossObservedRate.Value > 0)
				{
					newValue |= 0x4;
				}
			}

			else if ((TransferStatuses)TransferStatus.Value == TransferStatuses.Complete)
			{
				newValue = 0x2;
			}

			if (VolumeTransferDiscreteAlarms.Value == null
			|| (short)VolumeTransferDiscreteAlarms.Value != newValue
			|| IsStatusChange(VolumeTransferDiscreteAlarms.Status, newStatus))
			{
				VolumeTransferDiscreteAlarms.Value = newValue;
				VolumeTransferDiscreteAlarms.Status = newStatus;

				this.SetTimeStamps(new PointTag[] { TransferStatus, TransferTimeRemaining }, VolumeTransferDiscreteAlarms);
			}
		}

		private void CalculateTransferCompletion( PointTag TransferMode, PointTag TransferTarget, PointTag VolumeGrossObserved, PointTag VolumeNetStandard,
																PointTag TransferStartGOV, PointTag TransferStartNSV,
																PointTag TransferredGOV,PointTag TransferredNSV,
																PointTag VolumeGrossObservedRate, PointTag VolumeNetStandardRate,
																PointTag TransferStatus, PointTag TransferTimeRemaining, PointTag TransferTimeCompletion)
		{
			if (TransferStatus.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferStatus.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(TransferStartGOV)
			|| !IsValueGood(TransferredGOV))
			{
				return;
			}

			if((TransferStatuses)TransferStatus.Value == TransferStatuses.Complete)
			{
				return;
			}

			if (this.VolumeTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
			{
				if (((double)TransferTarget.Value > 0
				&& Math.Round((double)TransferredGOV.Value, TransferredGOV.DecimalPlaces, MidpointRounding.AwayFromZero) >= Math.Round((double)TransferTarget.Value, TransferTarget.DecimalPlaces, MidpointRounding.AwayFromZero))
				|| ((double)TransferTarget.Value < 0.0
				&& Math.Round((double)TransferredGOV.Value, TransferredGOV.DecimalPlaces, MidpointRounding.AwayFromZero) <= Math.Round((double)TransferTarget.Value, TransferTarget.DecimalPlaces, MidpointRounding.AwayFromZero)))
				{
					var newStatus = StatusCodes.Good;
					if (IsStatusUncertain(TransferTarget))
					{
						newStatus = StatusCodes.Uncertain;
					}

					TransferStatus.Value = TransferStatuses.Complete;
					TransferStatus.Status = newStatus;
					base.SetTimeStamps(new PointTag[] { TransferredGOV, TransferTarget }, TransferStatus);
				}
			}
			else
			{
				if (((double)TransferTarget.Value > 0
				&& Math.Round((double)TransferredNSV.Value, TransferredNSV.DecimalPlaces, MidpointRounding.AwayFromZero) >= Math.Round((double)TransferTarget.Value, TransferTarget.DecimalPlaces, MidpointRounding.AwayFromZero))
				|| ((double)TransferTarget.Value < 0.0
				&& Math.Round((double)TransferredNSV.Value, TransferredNSV.DecimalPlaces, MidpointRounding.AwayFromZero) <= Math.Round((double)TransferTarget.Value, TransferTarget.DecimalPlaces, MidpointRounding.AwayFromZero)))
				{
					var newStatus = StatusCodes.Good;
					if (IsStatusUncertain(TransferTarget))
					{
						newStatus = StatusCodes.Uncertain;
					}

					TransferStatus.Value = TransferStatuses.Complete;
					TransferStatus.Status = newStatus;
					base.SetTimeStamps(new PointTag[] { TransferredNSV, TransferTarget }, TransferStatus);
				}
			}

			if ((TransferStatuses)TransferStatus.Value == TransferStatuses.Complete)
			{
				List<PointTag> tagList = new List<PointTag>();

				CalculateTimeRemaining(TransferMode, TransferStatus, VolumeGrossObserved, VolumeNetStandard, VolumeGrossObservedRate, VolumeNetStandardRate, TransferTarget, TransferredGOV, TransferredNSV, TransferTimeRemaining);

				CalculateTransferTimeComplete(TransferTimeRemaining, TransferTimeCompletion, TransferStatus);


				tagList.Add(TransferStatus);
				tagList.Add(VolumeGrossObserved);
				tagList.Add(VolumeNetStandard);
				tagList.Add(TransferredGOV);
				tagList.Add(TransferredNSV);
				tagList.Add(TransferTimeRemaining);
				tagList.Add(TransferTimeCompletion);


				if (tagList.Count > 0)
				{
					var security = new SecurityClass() { UserID = "FMPointService" };
					FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
				}
			}
		}

		private void CalculateTimeRemaining(PointTag TransferMode, PointTag TransferStatus, PointTag VolumeGrossObserved, PointTag VolumeNetStandard,
														PointTag VolumeGrossObservedRate, PointTag VolumeNetStandardRate,
														PointTag TransferTarget, PointTag TransferredGOV, PointTag TransferredNSV, PointTag TransferTimeRemaining)
		{
			if (TransferTimeRemaining.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferTimeRemaining.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(TransferMode)
			|| !IsValueGood(TransferStatus)
			|| !IsValueGood(VolumeGrossObserved)
			|| !IsValueGood(VolumeNetStandard)
			|| !IsValueGood(TransferredGOV)
			|| !IsValueGood(TransferredNSV)
			|| !IsValueGood(VolumeGrossObservedRate)
			|| !IsValueGood(VolumeNetStandardRate)
			|| !IsValueGood(TransferTarget))
			{
				return;
			}



			object newValue = null;
			var newStatus = StatusCodes.Good;

			if ((TransferStatuses)TransferStatus.Value == TransferStatuses.Complete)
			{
				newValue = new TimeSpan(0);
			}
			else if ((TransferStatuses)TransferStatus.Value == TransferStatuses.InProgress)
			{
				double deltaVolume = 0.0;
				double volumeRate = 0.0;
				EngineeringUnit volumeRateUnits = EngineeringUnit.FmuNone;

				if (this.VolumeTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
				{
					deltaVolume = (double)TransferTarget.Value - (double)TransferredGOV.Value;
					volumeRate = (double)VolumeGrossObservedRate.Value;
					volumeRateUnits = VolumeGrossObservedRate.Units;

					if (IsStatusUncertain(TransferTarget)
					|| IsStatusUncertain(TransferredGOV)
					|| IsStatusUncertain(VolumeGrossObservedRate))
					{
						newStatus = StatusCodes.Uncertain;
					}
				}
				else
				{
					deltaVolume = (double)TransferTarget.Value - (double)TransferredNSV.Value;
					volumeRate = (double)VolumeNetStandardRate.Value;
					volumeRateUnits = VolumeNetStandardRate.Units;

					if (IsStatusUncertain(TransferTarget)
					|| IsStatusUncertain(TransferredNSV)
					|| IsStatusUncertain(VolumeNetStandardRate))
					{
						newStatus = StatusCodes.Uncertain;
					}
				}

				double deltaVolumeM3 = 0.0;
				EngineeringUnits.Convert(deltaVolume, TransferTarget.Units, ref deltaVolumeM3, EngineeringUnit.FmvMeter3, 0.0);
				double volmeRateM3Min = 0.0;
				EngineeringUnits.Convert(volumeRate, volumeRateUnits, ref volmeRateM3Min, EngineeringUnit.FmvfM3Min, 0.0);
				if (volmeRateM3Min != 0.0)
				{
					newValue = new TimeSpan((Int64)(600000000 * (deltaVolumeM3 / volmeRateM3Min)));
				}
			}

			if (TransferTimeRemaining.Value == null
			|| (newValue == null && TransferTimeRemaining.Value != null)
			|| (TimeSpan)TransferTimeRemaining.Value != (TimeSpan)newValue
			|| IsStatusChange(TransferTimeRemaining.Status, newStatus))
			{
				if (newValue != null && ((TimeSpan)newValue).TotalMilliseconds < 0)  // flow is going the wrong way
				{
					TransferTimeRemaining.Value = null;
					TransferTimeRemaining.Status = StatusCodes.BadInvalidState;
				}
				else
				{
					TransferTimeRemaining.Value = newValue;
					TransferTimeRemaining.Status = newStatus;
				}
			}
		}

		private void CalculateTransferTimeComplete(PointTag TransferTimeRemaining, PointTag TransferTimeComplete, PointTag Status)
		{
			if (TransferTimeComplete.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferTimeComplete.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if ((TransferStatuses)Status.Value == TransferStatuses.InProgress)
			{
				object newValue = null;
				var newStatus = StatusCodes.Good;
				if (TransferTimeRemaining.Value != null)
				{
					newValue = DateTimeOffset.UtcNow.AddTicks(((TimeSpan)TransferTimeRemaining.Value).Ticks);

					if (IsStatusUncertain(TransferTimeRemaining))
					{
						newStatus = StatusCodes.Uncertain;
					}
				}


				if (TransferTimeComplete.Value == null
				|| (newValue == null && TransferTimeComplete.Value != null)
				|| (DateTimeOffset)TransferTimeComplete.Value != (DateTimeOffset)newValue
				|| IsStatusChange(TransferTimeRemaining.Status, newStatus))
				{
					TransferTimeComplete.Value = newValue;
					TransferTimeComplete.Status = newStatus;
					this.SetTimeStamps(new PointTag[] { TransferTimeRemaining }, TransferTimeComplete);
				}
			}
			else if ((TransferStatuses)Status.Value == TransferStatuses.Complete &&
				TransferTimeComplete.Value == null)
			{
				object newValue = null;
				var newStatus = StatusCodes.Good;
				newValue = DateTimeOffset.UtcNow;

				TransferTimeComplete.Value = newValue;
				TransferTimeComplete.Status = newStatus;
				this.SetTimeStamps(new PointTag[] { TransferTimeRemaining }, TransferTimeComplete);
			}
		}

		private void CalculateTargetLimits(PointTag Mode, PointTag Status, PointTag Quantity, PointTag QuantityStart,
														PointTag QuantityMaximum, PointTag QuantityMinimum,
														PointTag Target)
		{
			if (((TransferStatuses)Status.Value == TransferStatuses.TransferTarget
			&& (double?)Target.Value != 0.0)
			|| Target.EngineeringUnitsType != QuantityMaximum.EngineeringUnitsType
			|| Target.Units != QuantityMaximum.Units
			|| Target.DecimalPlaces != QuantityMaximum.DecimalPlaces
			|| (TransferStatuses)Status.Value == TransferStatuses.TransferTarget
			&& (Target.Maximum != Math.Round((double)QuantityMaximum.Value - (double)Quantity.Value, Target.DecimalPlaces)
			|| Target.Minimum != Math.Round(-(double)Quantity.Value + (double)QuantityMinimum.Value, Target.DecimalPlaces))
			|| (TransferStatuses)Status.Value == TransferStatuses.InProgress
			&& (Target.Maximum != Math.Round((double)QuantityMaximum.Value - (double)QuantityStart.Value, Target.DecimalPlaces)
			|| Target.Minimum != Math.Round(-(double)QuantityStart.Value + (double)QuantityMinimum.Value, Target.DecimalPlaces)))
			{
				if ((TransferStatuses)Status.Value == TransferStatuses.TransferTarget)
				{
					Target.Value = 0.0;
					Target.ServerTimeStamp = DateTimeOffset.UtcNow;
					Target.SourceTimeStamp = DateTimeOffset.UtcNow;
				}

				Target.EngineeringUnitsType = QuantityMaximum.EngineeringUnitsType;
				Target.Units = QuantityMaximum.Units;
				Target.DecimalPlaces = QuantityMaximum.DecimalPlaces;
				if ((TransferStatuses)Status.Value == TransferStatuses.TransferTarget)
				{
					Target.Maximum = Math.Round((double)QuantityMaximum.Value - (double)Quantity.Value, Target.DecimalPlaces);
					Target.Minimum = Math.Round(-(double)Quantity.Value + (double)QuantityMinimum.Value, Target.DecimalPlaces);
				}
				else
				{
					Target.Maximum = Math.Round((double)QuantityMaximum.Value - (double)QuantityStart.Value, Target.DecimalPlaces);
					Target.Minimum = Math.Round(-(double)QuantityStart.Value + (double)QuantityMinimum.Value, Target.DecimalPlaces);
				}
				SetPointTag(Target);
			}
		}


		private void CalculateTransferredGrossObservedVolume(PointTag TransferStatus, PointTag VolumeGrossObserved, PointTag TransferStartGOV, PointTag TransferredGOV)
		{
			if (TransferredGOV.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferredGOV.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			// check the status if the input variables
			if (!IsValueGood(VolumeGrossObserved)
			|| !IsValueGood(TransferStartGOV))
			{
				if (TransferredGOV.Value != null ||
					TransferredGOV.Status != StatusCodes.Bad)
				{
					TransferredGOV.Value = null;
					TransferredGOV.Status = StatusCodes.Bad;
					TransferredGOV.ServerTimeStamp = DateTimeOffset.UtcNow;
					TransferredGOV.SourceTimeStamp = DateTimeOffset.UtcNow;
				}
				return;
			}

			double? newValue = null;
			var newStatus = StatusCodes.Good;

			if ((TransferStatuses)TransferStatus.Value == TransferStatuses.InProgress
			|| (TransferStatuses)TransferStatus.Value == TransferStatuses.Complete)
			{

				newValue = (double)VolumeGrossObserved.Value - (double)TransferStartGOV.Value;

				if (IsStatusUncertain(TransferredGOV))
				{
					newStatus = StatusCodes.Uncertain;
				}
			}

			if (TransferredGOV.Value == null
			|| (double)TransferredGOV.Value != newValue
			|| IsStatusChange(TransferredGOV.Status, newStatus))
			{
				TransferredGOV.Value = newValue;
				TransferredGOV.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { VolumeGrossObserved, TransferStartGOV }, TransferredGOV);
			}
		}

		private void CalculateTransferredNetStandardVolume(PointTag TransferStatus, PointTag VolumeNetStandard, PointTag TransferStartNSV, PointTag TransferredNSV)
		{
			if (TransferredNSV.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferredNSV.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			// check the status if the input variables
			if (!IsValueGood(VolumeNetStandard)
			|| !IsValueGood(TransferStartNSV))
			{
				if (TransferredNSV.Value != null ||
					TransferredNSV.Status != StatusCodes.Bad)
				{
					TransferredNSV.Value = null;
					TransferredNSV.Status = StatusCodes.Bad;
					TransferredNSV.ServerTimeStamp = DateTimeOffset.UtcNow;
					TransferredNSV.SourceTimeStamp = DateTimeOffset.UtcNow;
				}
				return;
			}

			double? newValue = null;
			var newStatus = StatusCodes.Good;

			if ((TransferStatuses)TransferStatus.Value == TransferStatuses.InProgress
			|| (TransferStatuses)TransferStatus.Value == TransferStatuses.Complete)
			{

				newValue = (double)VolumeNetStandard.Value - (double)TransferStartNSV.Value;

				if (IsStatusUncertain(TransferredNSV))
				{
					newStatus = StatusCodes.Uncertain;
				}
			}

			if (TransferredNSV.Value == null
			|| (double)TransferredNSV.Value != newValue
			|| IsStatusChange(TransferredNSV.Status, newStatus))
			{
				TransferredNSV.Value = newValue;
				TransferredNSV.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { VolumeNetStandard, TransferStartNSV }, TransferredNSV);
			}
		}

		private void CalculateTransferredVolume(PointTag TransferStatus, PointTag TransferredGOV, PointTag TransferredNSV, PointTag TransferredVolume)
		{
			if (TransferredVolume.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferredVolume.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			// check the status if the input variables
			if ((!IsValueGood(TransferredGOV)
			&& this.VolumeTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
			|| (!IsValueGood(TransferredNSV)
			&& this.VolumeTransferSettings.TransferVolumeMode == TransferVolumeMode.NetStandardVolume))
			{
				if (TransferredVolume.Value != null ||
					TransferredVolume.Status != StatusCodes.Bad)
				{
					TransferredVolume.Value = null;
					TransferredVolume.Status = StatusCodes.Bad;
					TransferredVolume.ServerTimeStamp = DateTimeOffset.UtcNow;
					TransferredVolume.SourceTimeStamp = DateTimeOffset.UtcNow;
				}
				return;
			}

			double? newValue = null;
			var newStatus = StatusCodes.Good;

			if ((TransferStatuses)TransferStatus.Value == TransferStatuses.InProgress
			|| (TransferStatuses)TransferStatus.Value == TransferStatuses.Complete)
			{

				if (this.VolumeTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
				{
					newValue = (double)TransferredGOV.Value;

					if (IsStatusUncertain(TransferredGOV))
					{
						newStatus = StatusCodes.Uncertain;
					}
				}
				else
				{
					newValue = (double)TransferredNSV.Value;

					if (IsStatusUncertain(TransferredNSV))
					{
						newStatus = StatusCodes.Uncertain;
					}
				}
			}

			if (TransferredVolume.Value == null
			|| (double)TransferredVolume.Value != newValue
			|| IsStatusChange(TransferredVolume.Status, newStatus))
			{
				TransferredVolume.Value = newValue;
				TransferredVolume.Status = newStatus;
				if (this.VolumeTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
				{
					base.SetTimeStamps(new PointTag[] { TransferredGOV }, TransferredVolume);
				}
				else
				{
					base.SetTimeStamps(new PointTag[] { TransferredNSV }, TransferredVolume);
				}
			}
		}



		public ModuleInputOutputCollection GetInputOutputCollection(string calculationName)
		{
			var properties = new ModuleInputOutputCollection {
					new ModuleInputOutput
					{
						ID = "Temperature Product",
						Type = typeof(double?),
						ParameterType = ModuleInputOutputType.Input
					} };
			return properties;
		}
	}
}
