namespace TankTransfer
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
	using FloatingRoofCorrection;
	using StrapTables;
	using Quantities;
	using Opc.Ua;


	public class FMTankTransfer : FuelsManagerModule, IFuelsManagerModule
	{
		public TankTransferModuleSettings TankTransferSettings { get; set; }

		public FMFloatingRoofCorrection RoofCorrection { get; set; }

		public FMStrapTable StrapTable { get; set; }

		public FMQuantities Quantities { get; set; }

		private TankTransferMode? currentTransferMode;

		private double? currentTransferTarget;

		private DateTimeOffset? currenmtTransferTargetTimeStamp;

		private bool firstTimeFlag = true;

		public SetPointTagHandler SetPointTag = null;

		public SetPointPropertyHandler SetPointProperty = null;

		public Guid TankTransferModuleSettingsGuid;

		private const double ImperialToDoubleAdjustmentFactor = 1E-14;

		public void TransferCalculation(
			PointTag LevelProduct, PointTag VolumeGrossObserved, PointTag LevelProductRate, PointTag VolumeGrossObservedRate, PointTag VolumeStandartNetRate,
			PointTag VolumeNetStandard, PointTag VolumeWater, PointTag TransferMode, PointTag TransferStatus, PointTag TransferTarget,
			PointTag ProdTemperature, PointTag ProdDensity, PointTag ProdDensityInAir, PointTag Mass,
			PointTag VCF, PointTag VolumeBottom, PointTag TankShellCorrection, PointTag PercentBSW,
			PointTag VolumeGrossObservedAvailable, PointTag VolumeGrossObservedRemaining, PointTag VolumeNetStandardAvailable, PointTag VolumeNetStandardRemaining,
			PointTag TransferStartLevel, PointTag TransferStartGOV, PointTag TransferStartNSV, PointTag TransferStartVolumeWater, PointTag TransferStartVolume, PointTag TransferTimeRemaining,
			PointTag TransferTimeCompletion, PointTag TransferredGOV, PointTag TransferredNSV, PointTag TransferredVolumeWater, PointTag TransferredVolume,
			PointTag TankTransferDiscreteAlarms, PointTag TankCommand, PointTag LevelProductMax, PointTag LevelProductMin,
			PointTag TransferStartTime, PointTag TransferStopTime, PointTag TransferLevelTarget, PointTag TransferVolumeTarget)
		{

			CalculateTransferInactive(TransferMode, TransferTimeRemaining, TransferTimeCompletion,
											TransferStartLevel, TransferStartGOV, TransferStartNSV, TransferStartVolumeWater, TransferStartVolume,
											TransferredGOV, TransferredNSV, TransferredVolumeWater,TransferredVolume,
											TransferStatus, TransferTarget,
											TankCommand, TankTransferDiscreteAlarms, TransferStartTime, TransferStopTime,
											TransferLevelTarget, TransferVolumeTarget);

			CalculateTransferTarget(TransferMode,
											ProdTemperature, ProdDensity, ProdDensityInAir, Mass,
											VCF, VolumeBottom, TankShellCorrection, PercentBSW,
											LevelProduct, LevelProductMax, LevelProductMin,
											VolumeGrossObserved, VolumeGrossObservedAvailable, VolumeGrossObservedRemaining,
											VolumeNetStandard, VolumeNetStandardAvailable, VolumeNetStandardRemaining, VolumeWater,
											TransferStartLevel, TransferStartGOV, TransferStartNSV, TransferStartVolumeWater,TransferStartVolume,
											TransferStatus, TransferTarget,
											TankCommand, TransferStartTime, TransferStopTime,
											TransferLevelTarget, TransferVolumeTarget);

			CalculateTransferInProgress(	TransferMode, TransferStatus, 
													ProdTemperature, ProdDensity, ProdDensityInAir, Mass,
													VCF, VolumeBottom, TankShellCorrection, PercentBSW,
													LevelProduct, LevelProductMax, LevelProductMin,
													VolumeGrossObserved, VolumeGrossObservedAvailable, VolumeGrossObservedRemaining,
													VolumeNetStandard, VolumeNetStandardAvailable, VolumeNetStandardRemaining, VolumeWater,
													TransferStartLevel, TransferStartGOV, TransferStartNSV, TransferStartVolumeWater,
													LevelProductRate, VolumeGrossObservedRate, VolumeStandartNetRate,
													TransferredGOV, TransferredNSV, TransferredVolumeWater, TransferredVolume, TransferTarget,
													TankCommand, TransferTimeRemaining, TransferTimeCompletion,
													TankTransferDiscreteAlarms, TransferStartVolume,TransferStopTime,
													TransferLevelTarget, TransferVolumeTarget);

			CalculateTransferAlarms(TransferStatus, TransferTimeRemaining, TankTransferDiscreteAlarms);


			this.currentTransferMode = (TankTransferMode?) TransferMode.Value;

			firstTimeFlag = false;

		}

		private void CalculateTransferInactive(PointTag TransferMode, PointTag TransferTimeRemaining, PointTag TransferTimeCompletion,
															PointTag TransferStartLevel, PointTag TransferStartGOV, PointTag TransferStartNSV, PointTag TransferStartVolumeWater, PointTag TransferStartVolume,
															PointTag TransferredGOV, PointTag TransferredNSV, PointTag TransferredVolumeWater, PointTag TransferredVolume,
															PointTag TransferStatus, PointTag TransferTarget,
															PointTag TankCommand, PointTag TankTransferDiscreteAlarms, PointTag TransferStartTime, PointTag TransferStopTime,
															PointTag TransferLevelTarget, PointTag TransferVolumeTarget)
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


			if ((TankTransferMode)TransferMode.Value != FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Inactive)
			{
				return;
			}

			List<PointTag> tagList = new List<PointTag>();

			var newValue = TransferStatuses.Inactive;
			var newStatus = StatusCodes.Good;

			if (!this.firstTimeFlag
			&& this.currentTransferMode != (TankTransferMode)TransferMode.Value)
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

				CalculateTankCommand(TransferMode, TransferStatus, TransferTarget, TransferStartLevel, TankCommand);
			}

			// When TransferMode is set to Inactive, clear the Target
			if (this.currentTransferMode != (TankTransferMode)TransferMode.Value
			&& (TankTransferMode)TransferMode.Value == FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Inactive)
			{
				TransferTarget.Value = null;
				TransferTarget.Status = newStatus;
				TransferTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
				TransferTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
				tagList.Add(TransferTarget);
				SetPointTag(TransferTarget);

				TransferLevelTarget.Value = null;
				TransferLevelTarget.Status = newStatus;
				TransferLevelTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
				TransferLevelTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
				tagList.Add(TransferLevelTarget);
				SetPointTag(TransferLevelTarget);

				TransferVolumeTarget.Value = null;
				TransferVolumeTarget.Status = newStatus;
				TransferVolumeTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
				TransferVolumeTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
				tagList.Add(TransferVolumeTarget);
				SetPointTag(TransferVolumeTarget);

				this.currentTransferTarget = null;
				this.currenmtTransferTargetTimeStamp = null;
			}

			if (TransferStartLevel.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& (TransferStartLevel.Value != null
			|| IsStatusChange(TransferStartLevel.Status, newStatus)))
			{
				TransferStartLevel.Value = null;
				TransferStartLevel.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferStartLevel);
				tagList.Add(TransferStartLevel);
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

			if (TransferStartVolumeWater.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& (TransferStartVolumeWater.Value != null
			|| IsStatusChange(TransferStartVolumeWater.Status, newStatus)))
			{
				TransferStartVolumeWater.Value = null;
				TransferStartVolumeWater.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferStartVolumeWater);
				tagList.Add(TransferStartVolumeWater);
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

			if (TransferredVolumeWater.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& (TransferredVolumeWater.Value != null
			|| IsStatusChange(TransferredVolumeWater.Status, newStatus)))
			{
				TransferredVolumeWater.Value = null;
				TransferredVolumeWater.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferredVolumeWater);
				tagList.Add(TransferredVolumeWater);
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
			PointTag ProdTemperature, PointTag ProdDensity, PointTag ProdDensityInAir, PointTag Mass,
			PointTag VCF, PointTag VolumeBottom, PointTag TankShellCorrection, PointTag PercentBSW,
			PointTag LevelProduct, PointTag LevelProductMax, PointTag LevelProductMin,
			PointTag VolumeGrossObserved, PointTag VolumeGrossObservedAvailable, PointTag VolumeGrossObservedRemaining,
			PointTag VolumeNetStandard, PointTag VolumeNetStandardAvailable, PointTag VolumeNetStandardRemaining, PointTag VolumeWater,
			PointTag TransferStartLevel, PointTag TransferStartGOV, PointTag TransferStartNSV,
			PointTag TransferStartVolumeWater, PointTag TransferStartVolume,
			PointTag TransferStatus, PointTag TransferTarget,
			PointTag TankCommand, PointTag TransferStartTime, PointTag TransferStopTime,
			PointTag TransferLevelTarget, PointTag TransferVolumeTarget)
		{
			if (TransferStatus.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferStatus.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(TransferMode)
			|| !IsValueGood(ProdTemperature)
			|| !IsValueGood(ProdDensity)
			|| !IsValueGood(ProdDensityInAir)
			|| !IsValueGood(Mass)
			|| !IsValueGood(VCF)
			|| !IsValueGood(VolumeBottom)
			|| !IsValueGood(TankShellCorrection)
			|| !IsValueGood(PercentBSW)
			|| !IsValueGood(LevelProduct)
			|| !IsValueGood(LevelProductMax)
			|| !IsValueGood(LevelProductMin)
			|| !IsValueGood(VolumeGrossObserved)
			|| !IsValueGood(VolumeGrossObservedAvailable)
			|| !IsValueGood(VolumeGrossObservedRemaining)
			|| !IsValueGood(VolumeNetStandard)
			|| !IsValueGood(VolumeNetStandardAvailable)
			|| !IsValueGood(VolumeNetStandardRemaining)
			|| !IsValueGood(VolumeWater)
			|| !IsValueGood(TransferStatus))
			{
				return;
			}


			if ((TankTransferMode)TransferMode.Value == FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Inactive)
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
				var security = new SecurityClass() { UserID = "FMPointService" };

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
			if (!this.firstTimeFlag
			&& IsValueGood(TransferTarget)
			&& (TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget
			&& TransferTarget.Value != null
			&& (double)TransferTarget.Value != this.currentTransferTarget)
			{
				tagList.Add(TransferMode);

                TransferStatus.Value = TransferStatuses.InProgress;
                
				// May have new Target from Point Calculator and need to reset limits
                if (CalculateTransferTargetLimits(TransferMode, TransferStatus, LevelProduct,
														LevelProductMax, LevelProductMin,
														VolumeGrossObservedAvailable, VolumeGrossObservedRemaining,
														VolumeNetStandardAvailable, VolumeNetStandardRemaining,
														TransferTarget, TransferLevelTarget, TransferVolumeTarget, true))
				{
					SetPointTag(TransferTarget);
					SetPointTag(TransferLevelTarget);
					SetPointTag(TransferVolumeTarget);

					if (!tagList.Contains(TransferTarget))
					{
						tagList.Add(TransferTarget);
					}

					if (!tagList.Contains(TransferLevelTarget))
					{
						tagList.Add(TransferLevelTarget);
					}

					if (!tagList.Contains(TransferVolumeTarget))
					{
						tagList.Add(TransferVolumeTarget);
					}
				}

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



				if (TransferStartLevel.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
				&& TransferStartLevel.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					TransferStartLevel.Value = LevelProduct.Value;
					TransferStartLevel.Status = StatusCodes.Good;
					TransferStartLevel.SourceTimeStamp = DateTimeOffset.UtcNow;
					TransferStartLevel.ServerTimeStamp = DateTimeOffset.UtcNow;
					tagList.Add(TransferStartLevel);
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

				if (TransferStartVolumeWater.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
				&& TransferStartVolumeWater.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					TransferStartVolumeWater.Value = VolumeWater.Value;
					TransferStartVolumeWater.Status = StatusCodes.Good;
					TransferStartVolumeWater.SourceTimeStamp = DateTimeOffset.UtcNow;
					TransferStartVolumeWater.ServerTimeStamp = DateTimeOffset.UtcNow;
					tagList.Add(TransferStartVolumeWater);
				}

				if (TransferStartVolume.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
				&& TransferStartVolume.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					if (this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
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

				if ((TankTransferMode)TransferMode.Value == TankTransferMode.Level)
				{
					TransferLevelTarget.Value = TransferTarget.Value;
					TransferLevelTarget.Status = TransferTarget.Status;
					TransferLevelTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
					TransferLevelTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
					tagList.Add(TransferLevelTarget);
					SetPointTag(TransferLevelTarget);

					CalculateTransferTargetFromTargetLevel(TransferMode, TransferStatus,
																			ProdTemperature, ProdDensity, ProdDensityInAir, Mass,
																			VCF, VolumeBottom, TankShellCorrection, PercentBSW,
																			VolumeGrossObserved, VolumeGrossObservedAvailable, VolumeGrossObservedRemaining,
																			VolumeNetStandard, VolumeNetStandardAvailable, VolumeNetStandardRemaining,
																			TransferStartGOV, TransferStartNSV,
																			TransferTarget, TransferStartVolume, TransferVolumeTarget);

					tagList.Add(TransferVolumeTarget);
					SetPointTag(TransferVolumeTarget);
				}
				else
				{
					TransferVolumeTarget.Value = TransferTarget.Value;
					TransferVolumeTarget.Status = TransferTarget.Status;
					TransferVolumeTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
					TransferVolumeTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
					tagList.Add(TransferVolumeTarget);
					SetPointTag(TransferVolumeTarget);

					var TransferModeLevel = new PointTag(TransferMode) { Value = TransferModes.Level };

					if (this.TankTransferSettings.CurrentTransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
					{
						CalculateTransferTargetFromTargetBatchGOV(TransferModeLevel, TransferStatus,
																				ProdTemperature, ProdDensity, ProdDensityInAir, Mass,
																				VCF, VolumeBottom, TankShellCorrection, PercentBSW,
																				LevelProductMax, LevelProductMin,
																				VolumeGrossObserved, VolumeNetStandard,
																				VolumeNetStandardAvailable, VolumeNetStandardRemaining,
																				TransferStartGOV, TransferStartNSV,
																				TransferTarget, TransferStartVolume, TransferLevelTarget);
					}
					else
					{
						CalculateTransferTargetFromTargetBatchNSV(TransferModeLevel, TransferStatus,
																					ProdTemperature, ProdDensity, ProdDensityInAir, Mass,
																					VCF, VolumeBottom, TankShellCorrection, PercentBSW,
																					LevelProductMax, LevelProductMin,
																					VolumeGrossObserved, VolumeNetStandard,
																					VolumeGrossObservedAvailable, VolumeGrossObservedRemaining,
																					TransferStartGOV, TransferStartNSV,
																					TransferTarget, TransferStartVolume, TransferLevelTarget);
					}

					tagList.Add(TransferLevelTarget);
					SetPointTag(TransferLevelTarget);
				}

				if (tagList.Count > 0)
				{
					var security = new SecurityClass() { UserID = "FMPointService" };
					FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
				}

				this.TankTransferSettings.CurrentTransferVolumeMode = this.TankTransferSettings.TransferVolumeMode;
				this.SetPointProperty("TankTransferSettings");

				CalculateTankCommand(TransferMode, TransferStatus, TransferTarget, TransferStartLevel, TankCommand);
			}
			if (!this.firstTimeFlag
			&& this.currentTransferMode != (TankTransferMode)TransferMode.Value)
			{
				this.currentTransferMode = (TankTransferMode)TransferMode.Value;
				tagList.Add(TransferMode);
				tagList.Add(TransferTarget);
			}

			// Keep Target up to date with changes to Level Product, Maximum, Minimum, and Transfer Mode
			if (CalculateTransferTargetLimits(TransferMode, TransferStatus, LevelProduct,
													LevelProductMax, LevelProductMin,
													VolumeGrossObservedAvailable, VolumeGrossObservedRemaining,
													VolumeNetStandardAvailable, VolumeNetStandardRemaining,
													TransferTarget, TransferLevelTarget, TransferVolumeTarget))
			{
				SetPointTag(TransferTarget);
				SetPointTag(TransferLevelTarget);
				SetPointTag(TransferVolumeTarget);

				if(!tagList.Contains(TransferTarget))
				{
					tagList.Add(TransferTarget);
				}

				if (!tagList.Contains(TransferLevelTarget))
				{
					tagList.Add(TransferLevelTarget);
				}

				if (!tagList.Contains(TransferVolumeTarget))
				{
					tagList.Add(TransferVolumeTarget);
				}
			}

			if (tagList.Count > 0)
			{
				var security = new SecurityClass() { UserID = "FMPointService" };
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
			}


			this.currentTransferTarget = (double)TransferTarget.Value;
			this.currenmtTransferTargetTimeStamp = TransferTarget.ServerTimeStamp;
		}

		private void CalculateTransferInProgress(	PointTag TransferMode, PointTag TransferStatus, 
																PointTag ProdTemperature, PointTag ProdDensity, PointTag ProdDensityInAir, PointTag Mass,
																PointTag VCF, PointTag VolumeBottom, PointTag TankShellCorrection, PointTag PercentBSW,
																PointTag LevelProduct, PointTag LevelProductMax, PointTag LevelProductMin,
																PointTag VolumeGrossObserved, PointTag VolumeGrossObservedAvailable, PointTag VolumeGrossObservedRemaining,
																PointTag VolumeNetStandard, PointTag VolumeNetStandardAvailable, PointTag VolumeNetStandardRemaining, PointTag VolumeWater,
																PointTag TransferStartLevel, PointTag TransferStartGOV, PointTag TransferStartNSV, PointTag TransferStartVolumeWater,
																PointTag LevelProductRate, PointTag VolumeGrossObservedRate, PointTag VolumeStandartNetRate,
																PointTag TransferredGOV, PointTag TransferredNSV, PointTag TransferredVolumeWater, PointTag TransferredVolume, PointTag TransferTarget,
																PointTag TankCommand, PointTag TransferTimeRemaining, PointTag TransferTimeCompletion,
																PointTag TankTransferDiscreteAlarms, PointTag TransferStartVolume, PointTag TransferStopTime,
																PointTag TransferLevelTarget, PointTag TransferVolumeTarget)
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


			if (!IsValueGood(VolumeGrossObserved)
			|| !IsValueGood(ProdTemperature)
			|| !IsValueGood(ProdDensity)
			|| !IsValueGood(ProdDensityInAir)
			|| !IsValueGood(Mass)
			|| !IsValueGood(VCF)
			|| !IsValueGood(VolumeBottom)
			|| !IsValueGood(TankShellCorrection)
			|| !IsValueGood(PercentBSW)
			|| !IsValueGood(LevelProductMax)
			|| !IsValueGood(LevelProductMin)
			|| !IsValueGood(VolumeGrossObservedAvailable)
			|| !IsValueGood(VolumeGrossObservedRemaining)
			|| !IsValueGood(VolumeNetStandardAvailable)
			|| !IsValueGood(VolumeNetStandardRemaining)
			|| !IsValueGood(LevelProductRate)
			|| !IsValueGood(VolumeGrossObservedRate)
			|| !IsValueGood(VolumeStandartNetRate)
			|| !IsValueGood(TransferTarget))
			{
				return;
			}

			// Initial Execution
			if (this.currentTransferMode == null)
			{
				this.currentTransferMode = (TankTransferMode)TransferMode.Value;
			}

			// No Change in Mode or TransferVolumeMode
			if (this.currentTransferMode == (TankTransferMode)TransferMode.Value
			&& this.TankTransferSettings.CurrentTransferVolumeMode == this.TankTransferSettings.TransferVolumeMode)
			{
				// Process change in Target
				if ((this.currentTransferTarget != (double)TransferTarget.Value
				&& this.currenmtTransferTargetTimeStamp != TransferTarget.ServerTimeStamp))
				{
					this.currentTransferTarget = (double)TransferTarget.Value;
					this.currenmtTransferTargetTimeStamp = TransferTarget.ServerTimeStamp;

					SetPointTag(TransferTarget);

					List<PointTag> tagList = new List<PointTag>();

					tagList.Add(TransferTarget);

					if ((TankTransferMode)TransferMode.Value == TankTransferMode.Level)
					{
						TransferLevelTarget.Value = TransferTarget.Value;
						TransferLevelTarget.Status = TransferTarget.Status;
						TransferLevelTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
						TransferLevelTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
						tagList.Add(TransferLevelTarget);
						SetPointTag(TransferLevelTarget);

						CalculateTransferTargetFromTargetLevel(TransferMode, TransferStatus,
																				ProdTemperature, ProdDensity, ProdDensityInAir, Mass,
																				VCF, VolumeBottom, TankShellCorrection, PercentBSW,
																				VolumeGrossObserved, VolumeGrossObservedAvailable, VolumeGrossObservedRemaining,
																				VolumeNetStandard, VolumeNetStandardAvailable, VolumeNetStandardRemaining,
																				TransferStartGOV, TransferStartNSV,
																				TransferTarget, TransferStartVolume, TransferVolumeTarget);

						tagList.Add(TransferVolumeTarget);
						SetPointTag(TransferVolumeTarget);
					}
					else
					{
						TransferVolumeTarget.Value = TransferTarget.Value;
						TransferVolumeTarget.Status = TransferTarget.Status;
						TransferVolumeTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
						TransferVolumeTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
						tagList.Add(TransferVolumeTarget);
						SetPointTag(TransferVolumeTarget);

						var TransferModeLevel = new PointTag(TransferMode) { Value = TransferModes.Level };

						if (this.TankTransferSettings.CurrentTransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
						{

							CalculateTransferTargetFromTargetBatchGOV(TransferModeLevel, TransferStatus,
																					ProdTemperature, ProdDensity, ProdDensityInAir, Mass,
																					VCF, VolumeBottom, TankShellCorrection, PercentBSW,
																					LevelProductMax, LevelProductMin,
																					VolumeGrossObserved, VolumeNetStandard,
																					VolumeNetStandardAvailable, VolumeNetStandardRemaining,
																					TransferStartGOV, TransferStartNSV,
																					TransferTarget, TransferStartVolume, TransferLevelTarget);
						}
						else
						{
							CalculateTransferTargetFromTargetBatchNSV(TransferModeLevel, TransferStatus,
																						ProdTemperature, ProdDensity, ProdDensityInAir, Mass,
																						VCF, VolumeBottom, TankShellCorrection, PercentBSW,
																						LevelProductMax, LevelProductMin,
																						VolumeGrossObserved, VolumeNetStandard,
																						VolumeGrossObservedAvailable, VolumeGrossObservedRemaining,
																						TransferStartGOV, TransferStartNSV,
																						TransferTarget, TransferStartVolume, TransferLevelTarget);
						}

						tagList.Add(TransferLevelTarget);
						SetPointTag(TransferLevelTarget);
					}

					var security = new SecurityClass() { UserID = "FMPointService" };
					FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
				}
			}

			// Process Change in TransferMode or Transfer Volume Mode
			else
			{
				List<PointTag> tagList = new List<PointTag>();

				// Process simultaneous change in Target as may occur from Point Calculator
				if ((this.currentTransferTarget != (double)TransferTarget.Value
				&& this.currenmtTransferTargetTimeStamp != TransferTarget.ServerTimeStamp))
				{
					this.currentTransferMode = (TankTransferMode)TransferMode.Value;

					tagList.Add(TransferMode);
					tagList.Add(TransferTarget);

					SetPointTag(TransferTarget);

					this.TankTransferSettings.CurrentTransferVolumeMode = this.TankTransferSettings.TransferVolumeMode;
					this.SetPointProperty("TankTransferSettings");


					if (CalculateTransferTargetLimits(TransferMode, TransferStatus, LevelProduct,
												LevelProductMax, LevelProductMin,
												VolumeGrossObservedAvailable, VolumeGrossObservedRemaining,
												VolumeNetStandardAvailable, VolumeNetStandardRemaining,
												TransferTarget, TransferLevelTarget, TransferVolumeTarget))
					{
						this.currentTransferTarget = (double)TransferTarget.Value;
						this.currenmtTransferTargetTimeStamp = TransferTarget.ServerTimeStamp;

						SetPointTag(TransferTarget);

						tagList.Add(TransferTarget);

						if ((TankTransferMode)TransferMode.Value == TankTransferMode.Level)
						{
							TransferLevelTarget.Value = TransferTarget.Value;
							TransferLevelTarget.Status = TransferTarget.Status;
							TransferLevelTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
							TransferLevelTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
							tagList.Add(TransferLevelTarget);
							SetPointTag(TransferLevelTarget);

							CalculateTransferTargetFromTargetLevel(TransferMode, TransferStatus,
																					ProdTemperature, ProdDensity, ProdDensityInAir, Mass,
																					VCF, VolumeBottom, TankShellCorrection, PercentBSW,
																					VolumeGrossObserved, VolumeGrossObservedAvailable, VolumeGrossObservedRemaining,
																					VolumeNetStandard, VolumeNetStandardAvailable, VolumeNetStandardRemaining,
																					TransferStartGOV, TransferStartNSV,
																					TransferTarget, TransferStartVolume, TransferVolumeTarget);

							tagList.Add(TransferVolumeTarget);
							SetPointTag(TransferVolumeTarget);
						}

						// Only change in Transfer Mode or Transfer Volume Mode
						else
						{
							TransferVolumeTarget.Value = TransferTarget.Value;
							TransferVolumeTarget.Status = TransferTarget.Status;
							TransferVolumeTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
							TransferVolumeTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
							tagList.Add(TransferVolumeTarget);
							SetPointTag(TransferVolumeTarget);

							var TransferModeLevel = new PointTag(TransferMode) { Value = TransferModes.Level };

							if (this.TankTransferSettings.CurrentTransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
							{

								CalculateTransferTargetFromTargetBatchGOV(TransferModeLevel, TransferStatus,
																						ProdTemperature, ProdDensity, ProdDensityInAir, Mass,
																						VCF, VolumeBottom, TankShellCorrection, PercentBSW,
																						LevelProductMax, LevelProductMin,
																						VolumeGrossObserved, VolumeNetStandard,
																						VolumeNetStandardAvailable, VolumeNetStandardRemaining,
																						TransferStartGOV, TransferStartNSV,
																						TransferTarget, TransferStartVolume, TransferLevelTarget);
							}
							else
							{
								CalculateTransferTargetFromTargetBatchNSV(TransferModeLevel, TransferStatus,
																							ProdTemperature, ProdDensity, ProdDensityInAir, Mass,
																							VCF, VolumeBottom, TankShellCorrection, PercentBSW,
																							LevelProductMax, LevelProductMin,
																							VolumeGrossObserved, VolumeNetStandard,
																							VolumeGrossObservedAvailable, VolumeGrossObservedRemaining,
																							TransferStartGOV, TransferStartNSV,
																							TransferTarget, TransferStartVolume, TransferLevelTarget);
							}

							tagList.Add(TransferLevelTarget);
							SetPointTag(TransferLevelTarget);
						}
					}

					var security = new SecurityClass() { UserID = "FMPointService" };
					FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
				}

				else
				{

					if ((TankTransferMode)TransferMode.Value == FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Batch
					&& this.currentTransferMode == FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Level)
					{
						CalculateTransferTargetFromTargetLevel(TransferMode, TransferStatus,
																				ProdTemperature, ProdDensity, ProdDensityInAir, Mass,
																				VCF, VolumeBottom, TankShellCorrection, PercentBSW,
																				VolumeGrossObserved, VolumeGrossObservedAvailable, VolumeGrossObservedRemaining,
																				VolumeNetStandard, VolumeNetStandardAvailable, VolumeNetStandardRemaining,
																				TransferStartGOV, TransferStartNSV,
																				TransferTarget, TransferStartVolume, TransferTarget);

						TransferVolumeTarget.Value = TransferTarget.Value;
						TransferVolumeTarget.Status = TransferTarget.Status;
						TransferVolumeTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
						TransferVolumeTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
                        tagList.Add(TransferVolumeTarget);
                        SetPointTag(TransferVolumeTarget);
					}

					else if (((TankTransferMode)TransferMode.Value == FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Level
					&& this.currentTransferMode == FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Batch)
					|| ((TankTransferMode)TransferMode.Value == FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Batch
					&& this.TankTransferSettings.CurrentTransferVolumeMode != this.TankTransferSettings.TransferVolumeMode))
					{
						if (this.TankTransferSettings.CurrentTransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
						{

							CalculateTransferTargetFromTargetBatchGOV(TransferMode, TransferStatus,
																					ProdTemperature, ProdDensity, ProdDensityInAir, Mass,
																					VCF, VolumeBottom, TankShellCorrection, PercentBSW,
																					LevelProductMax, LevelProductMin,
																					VolumeGrossObserved, VolumeNetStandard,
																					VolumeNetStandardAvailable, VolumeNetStandardRemaining,
																					TransferStartGOV, TransferStartNSV,
																					TransferTarget, TransferStartVolume, TransferTarget);
						}
						else
						{
							CalculateTransferTargetFromTargetBatchNSV(TransferMode, TransferStatus,
																						ProdTemperature, ProdDensity, ProdDensityInAir, Mass,
																						VCF, VolumeBottom, TankShellCorrection, PercentBSW,
																						LevelProductMax, LevelProductMin,
																						VolumeGrossObserved, VolumeNetStandard,
																						VolumeGrossObservedAvailable, VolumeGrossObservedRemaining,
																						TransferStartGOV, TransferStartNSV,
																						TransferTarget, TransferStartVolume, TransferTarget);
						}

						TransferLevelTarget.Value = TransferTarget.Value;
						TransferLevelTarget.Status = TransferTarget.Status;
						TransferLevelTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
						TransferLevelTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
						tagList.Add(TransferLevelTarget);
						SetPointTag(TransferLevelTarget);

					}

					this.currentTransferMode = (TankTransferMode)TransferMode.Value;

					tagList.Add(TransferMode);
					tagList.Add(TransferTarget);

					if (tagList.Count > 0)
					{
						var security = new SecurityClass() { UserID = "FMPointService" };
						FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
					}

					SetPointTag(TransferTarget);

					this.currentTransferTarget = (double)TransferTarget.Value;
					this.currenmtTransferTargetTimeStamp = TransferTarget.ServerTimeStamp;

					this.TankTransferSettings.CurrentTransferVolumeMode = this.TankTransferSettings.TransferVolumeMode;
					this.SetPointProperty("TankTransferSettings");
				}
			}

			CalculateTransferStartVolume(TransferStatus, TransferStartGOV, TransferStartNSV, TransferStartVolume);

			CalculateTransferredGrossObservedVolume(TransferStartGOV, VolumeGrossObserved, TransferredGOV, TransferredVolume);

			CalculateTransferredNetStandardVolume(TransferStartNSV, VolumeNetStandard, TransferredNSV, TransferredVolume);

			CalculateTransferredWaterVolume(TransferStartVolumeWater, VolumeWater, TransferredVolumeWater);

			CalculateTransferredVolume(TransferStatus, TransferredGOV, TransferredNSV, TransferredVolume);

			CalculateTransferTimeRemaining(TransferMode, TransferStatus, LevelProduct,
														TransferredGOV, TransferredNSV,
														LevelProductRate, VolumeGrossObservedRate, VolumeStandartNetRate,
														TransferTarget,
														TransferTimeRemaining);

			CalculateTransferTimeComplete(TransferTimeRemaining, TransferTimeCompletion, TransferStatus);


			CalculateTransferCompletion(	TransferMode, TransferTarget, LevelProduct, TransferStartLevel, TransferredGOV, TransferredNSV, TransferredVolume,
													LevelProductRate, VolumeGrossObservedRate, VolumeStandartNetRate,
													TransferStatus, TankCommand, TransferTimeRemaining, TransferTimeCompletion);
		}

		private void CalculateTransferAlarms(PointTag TransferStatus, PointTag TransferTimeRemaining, PointTag TankTransferDiscreteAlarms)
		{
			if (TankTransferDiscreteAlarms.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TankTransferDiscreteAlarms.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			short newValue = 0;
			var newStatus = StatusCodes.Good;

			if(TransferStatus.Value is TransferStatuses && (TransferStatuses) TransferStatus.Value == TransferStatuses.InProgress )
			{
				if(TransferTimeRemaining.Value is TimeSpan
				&& ((TimeSpan) TransferTimeRemaining.Value).TotalMinutes < this.TankTransferSettings.TransferAdvisoryTime)
				{
					newValue = 0x10;
				}
			}

			else if(TransferStatus.Value is TransferStatuses && (TransferStatuses)TransferStatus.Value == TransferStatuses.Complete)
			{
				newValue = 0x20;
			}

			if (TankTransferDiscreteAlarms.Value == null
			|| (short)TankTransferDiscreteAlarms.Value != newValue
			|| IsStatusChange(TankTransferDiscreteAlarms.Status, newStatus))
			{
				TankTransferDiscreteAlarms.Value = newValue;
				TankTransferDiscreteAlarms.Status = newStatus;

				this.SetTimeStamps(new PointTag[] { TransferStatus, TransferTimeRemaining }, TankTransferDiscreteAlarms);
			}
		}

		private void CalculateTransferCompletion(PointTag TransferMode, PointTag TransferTarget,
																PointTag LevelProduct, PointTag TransferStartLevel,
																PointTag TransferredGOV, PointTag TransferredNSV, PointTag TransferredVolume,
																PointTag LevelProductRate, PointTag VolumeGrossObservedRate, PointTag VolumeStandartNetRate,
																PointTag TransferStatus, PointTag TankCommand, PointTag TransferTimeRemaining, PointTag TransferTimeCompletion)
		{
			if (TransferStatus.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferStatus.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(TransferStartLevel)
			|| !IsValueGood(TransferredGOV)
			|| !IsValueGood(TransferredNSV))
			{
				return;
			}


			if ((TankTransferMode)TransferMode.Value == FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Level)
			{

				double adjFactor = 0;
				if ((LevelProduct.Units == EngineeringUnit.FmlFtIn16Th) || (LevelProduct.Units == EngineeringUnit.FmlFtIn8Th))
				{
					adjFactor = ImperialToDoubleAdjustmentFactor;
				}

				if ((double)TransferStartLevel.Value < (double)TransferTarget.Value
				&& ((adjFactor != 0 && (double)LevelProduct.Value + adjFactor >= (double)TransferTarget.Value)
				|| (adjFactor == 0 && Math.Round((double)LevelProduct.Value, LevelProduct.DecimalPlaces, MidpointRounding.AwayFromZero) >= Math.Round((double)TransferTarget.Value, TransferTarget.DecimalPlaces, MidpointRounding.AwayFromZero)))
				|| ((double)TransferStartLevel.Value > (double)TransferTarget.Value
				&& ((adjFactor != 0 && (double)LevelProduct.Value - adjFactor <= (double)TransferTarget.Value)
				|| (adjFactor == 0 && Math.Round((double)LevelProduct.Value, LevelProduct.DecimalPlaces, MidpointRounding.AwayFromZero) <= Math.Round((double)TransferTarget.Value, TransferTarget.DecimalPlaces, MidpointRounding.AwayFromZero)))))
				{
					var newStatus = StatusCodes.Good;
					if (IsStatusUncertain(LevelProduct)
					|| IsStatusUncertain(TransferStartLevel)
					|| IsStatusUncertain(TransferTarget))
					{
						newStatus = StatusCodes.Uncertain;
					}

					TransferStatus.Value = TransferStatuses.Complete;
					TransferStatus.Status = newStatus;
					base.SetTimeStamps(new PointTag[] { LevelProduct, TransferStartLevel, TransferTarget }, TransferStatus);
				}
			}
			else
			{
				if (this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
				{
					if (((double)TransferTarget.Value > 0
					&& Math.Round((double)TransferredGOV.Value, TransferredGOV.DecimalPlaces, MidpointRounding.AwayFromZero) >= Math.Round((double)TransferTarget.Value, TransferTarget.DecimalPlaces, MidpointRounding.AwayFromZero))
					|| ((double)TransferTarget.Value < 0.0
					&& Math.Round((double)TransferredGOV.Value, TransferredGOV.DecimalPlaces, MidpointRounding.AwayFromZero) <= Math.Round((double)TransferTarget.Value, TransferTarget.DecimalPlaces, MidpointRounding.AwayFromZero)))
					{
						var newStatus = StatusCodes.Good;
						if (IsStatusUncertain(LevelProduct)
						|| IsStatusUncertain(TransferStartLevel)
						|| IsStatusUncertain(TransferTarget))
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
						if (IsStatusUncertain(LevelProduct)
						|| IsStatusUncertain(TransferStartLevel)
						|| IsStatusUncertain(TransferTarget))
						{
							newStatus = StatusCodes.Uncertain;
						}

						TransferStatus.Value = TransferStatuses.Complete;
						TransferStatus.Status = newStatus;
						base.SetTimeStamps(new PointTag[] { TransferredNSV, TransferTarget }, TransferStatus);
					}
				}
			}

			if ((TransferStatuses)TransferStatus.Value == TransferStatuses.Complete)
			{
				List<PointTag> tagList = new List<PointTag>();


				CalculateTransferTimeRemaining(TransferMode, TransferStatus, LevelProduct,
															TransferredGOV, TransferredNSV,
															LevelProductRate, VolumeGrossObservedRate, VolumeStandartNetRate,
															TransferTarget,
															TransferTimeRemaining);

				CalculateTransferTimeComplete(TransferTimeRemaining, TransferTimeCompletion, TransferStatus);

				tagList.Add(TransferStatus);
				tagList.Add(TransferredGOV);
				tagList.Add(TransferredNSV);
				tagList.Add(TransferredVolume);
				tagList.Add(TransferTimeRemaining);
				tagList.Add(TransferTimeCompletion);


				if (tagList.Count > 0)
				{
					var security = new SecurityClass() { UserID = "FMPointService" };
					FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
				}

				CalculateTankCommand(TransferMode, TransferStatus, TransferTarget, TransferStartLevel, TankCommand);
			}
		}

		private void CalculateTransferTimeRemaining(PointTag TransferMode, PointTag TransferStatus, PointTag LevelProduct,
																	PointTag TransferredGOV, PointTag TransferredNSV,
																	PointTag LevelProductRate, PointTag VolumeGrossObservedRate, PointTag VolumeStandartNetRate,
																	PointTag TransferTarget,
																	PointTag TransferTimeRemaining)
		{
			if (TransferTimeRemaining.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferTimeRemaining.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(TransferMode)
			|| !IsValueGood(TransferStatus)
			|| !IsValueGood(LevelProduct)
			|| !IsValueGood(TransferredGOV)
			|| !IsValueGood(TransferredNSV)
			|| !IsValueGood(LevelProductRate)
			|| !IsValueGood(VolumeGrossObservedRate)
			|| !IsValueGood(VolumeStandartNetRate)
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
				if ((TankTransferMode)TransferMode.Value == FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Level)
				{
					var deltaLevel = (double)TransferTarget.Value - (double)LevelProduct.Value;
					double deltaLevelM = 0.0;
					EngineeringUnits.Convert(deltaLevel, TransferTarget.Units, ref deltaLevelM, EngineeringUnit.FmlMeter, 0.0);
					var levelRate = (double)LevelProductRate.Value;
					double levelRateMMin = 0.0;
					EngineeringUnits.Convert(levelRate, LevelProductRate.Units, ref levelRateMMin, EngineeringUnit.FmvrMMin, 0.0);
					if (levelRateMMin != 0.0)
					{
						newValue = new TimeSpan((Int64)(600000000 * deltaLevelM / levelRateMMin));
					}

					if (IsStatusUncertain(TransferTarget)
						|| IsStatusUncertain(LevelProduct)
						|| IsStatusUncertain(LevelProductRate))
					{
						newStatus = StatusCodes.Uncertain;
					}


				}
				else
				{
					double deltaVolume = 0.0;
					double volumeRate = 0.0;
					EngineeringUnit volumeRateUnits = EngineeringUnit.FmuNone;

					if (this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
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
						volumeRate = (double)VolumeStandartNetRate.Value;
						volumeRateUnits = VolumeStandartNetRate.Units;

						if (IsStatusUncertain(TransferTarget)
						|| IsStatusUncertain(TransferredNSV)
						|| IsStatusUncertain(VolumeStandartNetRate))
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

				if ((TankTransferMode)TransferMode.Value == FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Level)
				{
					this.SetTimeStamps(new PointTag[] { TransferTarget, LevelProduct, LevelProductRate }, TransferTimeRemaining);
				}
				else
				{
					if (this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
					{
						this.SetTimeStamps(new PointTag[] { TransferTarget, TransferredGOV, VolumeGrossObservedRate }, TransferTimeRemaining);
					}
					else
					{
						this.SetTimeStamps(new PointTag[] { TransferTarget, TransferredNSV, VolumeStandartNetRate }, TransferTimeRemaining);
					}
				}
			}
		}

		private void CalculateTransferTimeComplete(PointTag TransferTimeRemaining, PointTag TransferTimeComplete, PointTag TransferStatus)
		{
			if (TransferTimeComplete.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferTimeComplete.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if ((TransferStatuses)TransferStatus.Value == TransferStatuses.InProgress)
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
			else if ((TransferStatuses)TransferStatus.Value == TransferStatuses.Complete &&
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

		private bool CalculateTransferTargetLimits(	PointTag TransferMode, PointTag TransferStatus, PointTag LevelProduct,
																	PointTag LevelProductMax, PointTag LevelProductMin,
																	PointTag VolumeGrossObservedAvailable, PointTag VolumeGrossObservedRemaining,
																	PointTag VolumeNetStandardAvailable, PointTag VolumeNetStandardRemaining,
																	PointTag TransferTarget, PointTag TransferLevelTarget, PointTag TransferVolumeTarget, bool ForceUpdateMinMax = false)
		{

			if ((TankTransferMode)TransferMode.Value == FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Level)
			{
				if (((TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget
				&& TransferTarget.Value != LevelProduct.Value)
				||	TransferTarget.EngineeringUnitsType != LevelProductMax.EngineeringUnitsType
				|| TransferTarget.Units != LevelProductMax.Units
				|| TransferTarget.DecimalPlaces != LevelProductMax.DecimalPlaces
				|| TransferTarget.Maximum != (double)LevelProductMax.Value
				|| TransferTarget.Minimum != (double)LevelProductMin.Value)
				{
					if ((TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget)
					{
						TransferTarget.Value = LevelProduct.Value;
						TransferTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
						TransferTarget.SourceTimeStamp = DateTimeOffset.UtcNow;

						TransferLevelTarget.Value = LevelProduct.Value;
						TransferLevelTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
						TransferLevelTarget.SourceTimeStamp = DateTimeOffset.UtcNow;

						TransferVolumeTarget.Value = 0.0;
						TransferVolumeTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
						TransferVolumeTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
					}
					else
					{
						TransferTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
						TransferTarget.SourceTimeStamp = DateTimeOffset.UtcNow;

						TransferLevelTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
						TransferLevelTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
					}

					// if the transfer is inprogress or completed we do not want to reset the min and max values.
					// this will cause the value to be displayed as over/under range and if the setpoint is changed
					// will have the values at the time it is being set not the initial values which is what we want
					if (((TransferStatuses)TransferStatus.Value != TransferStatuses.InProgress &&
						(TransferStatuses)TransferStatus.Value != TransferStatuses.Complete)
						|| TransferTarget.EngineeringUnitsType != LevelProductMax.EngineeringUnitsType
						|| ForceUpdateMinMax)

                    {
						var decimalPrecision = TransferTarget.DecimalPlaces;
						if (LevelProduct.Units == EngineeringUnit.FmlFtIn16Th ||
							LevelProduct.Units == EngineeringUnit.FmlFtIn8Th)
							decimalPrecision = 6;

						TransferTarget.Maximum = Math.Round((double)LevelProductMax.Value, decimalPrecision);
						TransferTarget.Minimum = Math.Round((double)LevelProductMin.Value, decimalPrecision);
					}

                    TransferTarget.EngineeringUnitsType = LevelProductMax.EngineeringUnitsType;
                    TransferTarget.Units = LevelProductMax.Units;
                    TransferTarget.DecimalPlaces = LevelProductMax.DecimalPlaces;

                    return true;
				}
			}
			else if ((TankTransferMode)TransferMode.Value == FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Batch)
			{
				if (this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
				{
					if (((TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget
					&& (double?) TransferTarget.Value != 0.0)
					|| TransferTarget.EngineeringUnitsType != VolumeGrossObservedAvailable.EngineeringUnitsType
					|| TransferTarget.Units != VolumeGrossObservedAvailable.Units
					|| TransferTarget.DecimalPlaces != VolumeGrossObservedAvailable.DecimalPlaces
					|| (TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget
					&& (TransferTarget.Maximum != Math.Round((double)VolumeGrossObservedRemaining.Value, TransferTarget.DecimalPlaces) 
					|| TransferTarget.Minimum != Math.Round(-(double)VolumeGrossObservedAvailable.Value, TransferTarget.DecimalPlaces))
                    || ForceUpdateMinMax)
					{
						if ((TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget)
						{
							TransferTarget.Value = 0.0;
							TransferTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
							TransferTarget.SourceTimeStamp = DateTimeOffset.UtcNow;

							TransferLevelTarget.Value = LevelProduct.Value;
							TransferLevelTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
							TransferLevelTarget.SourceTimeStamp = DateTimeOffset.UtcNow;

							TransferVolumeTarget.Value = 0.0;
							TransferVolumeTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
							TransferVolumeTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
						}

                        TransferTarget.Units = VolumeGrossObservedAvailable.Units;
                        TransferTarget.DecimalPlaces = VolumeGrossObservedAvailable.DecimalPlaces;
                        
						// if the transfer is inprogress or completed we do not want to reset the min and max values.
                        // this will cause the value to be displayed as over/under range and if the setpoint is changed
                        // will have the values at the time it is being set not the initial values which is what we want
                        if (((TransferStatuses)TransferStatus.Value != TransferStatuses.InProgress &&
							(TransferStatuses)TransferStatus.Value != TransferStatuses.Complete)
							|| TransferTarget.EngineeringUnitsType != VolumeGrossObservedAvailable.EngineeringUnitsType
                            || ForceUpdateMinMax)

                        {
							// this is a manualy entered value which will be limited by the decimal precision. This is also being updated
							// by a process variable max and min that is not being limited. In order to prevent invalid over and under range
							// indications we need to format the min and max to the designated precision.
							TransferTarget.Maximum = Math.Round((double)VolumeGrossObservedRemaining.Value, TransferTarget.DecimalPlaces);
							TransferTarget.Minimum = Math.Round(-(double)VolumeGrossObservedAvailable.Value, TransferTarget.DecimalPlaces);
						}

                        TransferTarget.EngineeringUnitsType = VolumeGrossObservedAvailable.EngineeringUnitsType;

                        return true;
					}
				}
				else if (this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.NetStandardVolume)
				{
					if (((TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget
					&& (double?)TransferTarget.Value != 0.0)
					|| TransferTarget.EngineeringUnitsType != VolumeNetStandardAvailable.EngineeringUnitsType
					|| TransferTarget.Units != VolumeNetStandardAvailable.Units
					|| TransferTarget.DecimalPlaces != VolumeNetStandardAvailable.DecimalPlaces
					|| (TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget
					&& (TransferTarget.Maximum != Math.Round((double)VolumeNetStandardRemaining.Value, TransferTarget.DecimalPlaces)
					|| TransferTarget.Minimum != Math.Round(-(double)VolumeNetStandardAvailable.Value, TransferTarget.DecimalPlaces))
                    || ForceUpdateMinMax)
					{
						if ((TransferStatuses)TransferStatus.Value == TransferStatuses.TransferTarget)
						{
							TransferTarget.Value = 0.0;
							TransferTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
							TransferTarget.SourceTimeStamp = DateTimeOffset.UtcNow;

							TransferLevelTarget.Value = LevelProduct.Value;
							TransferLevelTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
							TransferLevelTarget.SourceTimeStamp = DateTimeOffset.UtcNow;

							TransferVolumeTarget.Value = 0.0;
							TransferVolumeTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
							TransferVolumeTarget.SourceTimeStamp = DateTimeOffset.UtcNow;

						}

						// if the transfer is inprogress or completed we do not want to reset the min and max values.
						// this will cause the value to be displayed as over/under range and if the setpoint is changed
						// will have the values at the time it is being set not the initial values which is what we want
						if (((TransferStatuses)TransferStatus.Value != TransferStatuses.InProgress &&
							(TransferStatuses)TransferStatus.Value != TransferStatuses.Complete)
							|| TransferTarget.Units != VolumeNetStandardAvailable.Units
                            || ForceUpdateMinMax)
						{
							// this is a manualy entered value which will be limited by the decimal precision. This is also being updated
							// by a process variable max and min that is not being limited. In order to prevent invalid over and under range
							// indications we need to format the min and max to the designated precision.
							TransferTarget.Maximum = Math.Round((double)VolumeNetStandardRemaining.Value, TransferTarget.DecimalPlaces);
							TransferTarget.Minimum = Math.Round(-(double)VolumeNetStandardAvailable.Value, TransferTarget.DecimalPlaces);
						}

                        TransferTarget.EngineeringUnitsType = VolumeNetStandardAvailable.EngineeringUnitsType;
                        TransferTarget.Units = VolumeNetStandardAvailable.Units;
                        TransferTarget.DecimalPlaces = VolumeNetStandardAvailable.DecimalPlaces;

                        return true;
					}
				}
			}

			return false;
		}

		private void CalculateTransferTargetFromTargetLevel(	PointTag TransferMode, PointTag TransferStatus,
																				PointTag ProdTemperature, PointTag ProdDensity, PointTag ProdDensityInAir, PointTag Mass,
																				PointTag VCF, PointTag VolumeBottom, PointTag TankShellCorrection, PointTag PercentBSW,
																				PointTag VolumeGrossObserved, PointTag VolumeGrossObservedAvailable, PointTag VolumeGrossObservedRemaining,
																				PointTag VolumeNetStandard, PointTag VolumeNetStandardAvailable, PointTag VolumeNetStandardRemaining,
																				PointTag TransferStartGOV, PointTag TransferStartNSV,
																				PointTag TransferTarget, PointTag TransferStartVolume, PointTag OutputTarget)
		{
			PointTag VolumeTotalObserved = new PointTag(VolumeGrossObserved) { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };
			PointTag CriticalZone = new PointTag() { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };
			PointTag VolumeRoofCorrection = new PointTag(VolumeGrossObserved) { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };
			PointTag TransferTargetGOV = new PointTag(VolumeGrossObserved) { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };
			PointTag TransferTargetNSV = new PointTag(VolumeNetStandard) { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };

			StrapTable.StrapVolumeCalculation(TransferTarget, VolumeTotalObserved);
			RoofCorrection.FloatingRoofCorrectionCalculation(ProdTemperature, ProdDensity, ProdDensityInAir, Mass, TransferTarget, VCF, CriticalZone, VolumeRoofCorrection);

			if (this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
			{
				Quantities.CalculateGrossObserverdVolume(VolumeRoofCorrection, VolumeTotalObserved, VolumeBottom, TankShellCorrection, TransferTargetGOV);
				if (TransferStartGOV.Value is double)
				{
					OutputTarget.Value = (double)TransferTargetGOV.Value - (double)TransferStartGOV.Value;
				}
				else
				{
					OutputTarget.Value = 0.0;
				}
				OutputTarget.EngineeringUnitsType = VolumeGrossObservedAvailable.EngineeringUnitsType;
				OutputTarget.Units = VolumeGrossObservedAvailable.Units;
				OutputTarget.DecimalPlaces = VolumeGrossObservedAvailable.DecimalPlaces;
				OutputTarget.Maximum = (double)VolumeGrossObservedRemaining.Value;
				OutputTarget.Minimum = -(double)VolumeGrossObservedAvailable.Value;
				OutputTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
				OutputTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
				OutputTarget.UpdatedDate = DateTimeOffset.UtcNow;
			}
			else
			{
				Quantities.CalculateGrossObserverdVolume(VolumeRoofCorrection, VolumeTotalObserved, VolumeBottom, TankShellCorrection, TransferTargetGOV);
				Quantities.CalculateNetStandardVolume(TransferTargetGOV, PercentBSW, VCF, TransferTargetNSV);
				if (TransferStartNSV.Value is double)
				{
					OutputTarget.Value = (double)TransferTargetNSV.Value - (double)TransferStartNSV.Value;
				}
				else
				{
					OutputTarget.Value = 0.0;
				}
				OutputTarget.EngineeringUnitsType = VolumeNetStandardAvailable.EngineeringUnitsType;
				OutputTarget.Units = VolumeNetStandardAvailable.Units;
				OutputTarget.DecimalPlaces = VolumeNetStandardAvailable.DecimalPlaces;
				OutputTarget.Maximum = (double)VolumeNetStandardRemaining.Value;
				OutputTarget.Minimum = -(double)VolumeNetStandardAvailable.Value;
				OutputTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
				OutputTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
				OutputTarget.UpdatedDate = DateTimeOffset.UtcNow;
			}
		}

		private void CalculateTransferTargetFromTargetBatchGOV(	PointTag TransferMode, PointTag TransferStatus,  
																					PointTag ProdTemperature, PointTag ProdDensity, PointTag ProdDensityInAir, PointTag Mass,
																					PointTag VCF, PointTag VolumeBottom, PointTag TankShellCorrection, PointTag PercentBSW,
																					PointTag LevelProductMax, PointTag LevelProductMin,
																					PointTag VolumeGrossObserved, PointTag VolumeNetStandard,
																					PointTag VolumeNetStandardAvailable, PointTag VolumeNetStandardRemaining,
																					PointTag TransferStartGOV, PointTag TransferStartNSV,
																					PointTag TransferTarget, PointTag TransferStartVolume, PointTag OutputTarget)
		{

			if(!IsValueGood(VolumeGrossObserved)
			|| !IsValueGood(VolumeNetStandard))
			{
				return;
			}


			PointTag CriticalZone = new PointTag() { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };
			PointTag VolumeRoofCorrection = new PointTag(VolumeNetStandard) { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };
			PointTag StrapVolume = new PointTag(VolumeNetStandard) { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };
			PointTag TransferTargetGOV = new PointTag(VolumeGrossObserved) { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };
			PointTag TransferTargetNSV = new PointTag(VolumeNetStandard) { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };
			PointTag StrapLevel = new PointTag(LevelProductMax) { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };

			TransferTargetGOV.Value = (double)TransferStartGOV.Value + (double)TransferTarget.Value;

			base.SetTimeStamps(new PointTag[] { TransferStartGOV, TransferTarget }, TransferTargetGOV);

			if ((TankTransferMode)TransferMode.Value == FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Batch)
			{
				Quantities.CalculateNetStandardVolume(TransferTargetGOV, PercentBSW, VCF, TransferTargetNSV);
				OutputTarget.Value = (double)TransferTargetNSV.Value - (double)TransferStartNSV.Value;
				OutputTarget.EngineeringUnitsType = VolumeNetStandardAvailable.EngineeringUnitsType;
				OutputTarget.Units = VolumeNetStandardAvailable.Units;
				OutputTarget.DecimalPlaces = VolumeNetStandardAvailable.DecimalPlaces;
				OutputTarget.Maximum = (double)VolumeNetStandardRemaining.Value;
				OutputTarget.Minimum = -(double)VolumeNetStandardAvailable.Value;
				OutputTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
				OutputTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
				OutputTarget.UpdatedDate = DateTimeOffset.UtcNow;
			}
			else
			{
				VolumeRoofCorrection.Value = 0.0;
				Quantities.CalculateStrapVolumeFromGrossObservedVolume(VolumeRoofCorrection, TransferTargetGOV, VolumeBottom, TankShellCorrection, StrapVolume);
				StrapTable.StrapLevelCalculation(StrapVolume, StrapLevel);

				RoofCorrection.FloatingRoofCorrectionCalculation(ProdTemperature, ProdDensity, ProdDensityInAir, Mass, StrapLevel, VCF, CriticalZone, VolumeRoofCorrection);
				if((double)VolumeRoofCorrection.Value != 0.0)
				{
					Quantities.CalculateStrapVolumeFromGrossObservedVolume(VolumeRoofCorrection, TransferTargetGOV, VolumeBottom, TankShellCorrection, StrapVolume);
					StrapTable.StrapLevelCalculation(StrapVolume, StrapLevel);
				}

				OutputTarget.Value = StrapLevel.Value;
				OutputTarget.EngineeringUnitsType = LevelProductMax.EngineeringUnitsType;
				OutputTarget.Units = LevelProductMax.Units;
				OutputTarget.DecimalPlaces = LevelProductMax.DecimalPlaces;
				OutputTarget.Maximum = (double)LevelProductMax.Value;
				OutputTarget.Minimum = (double)LevelProductMin.Value;
				OutputTarget.UpdatedDate = DateTimeOffset.UtcNow;
				OutputTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
				OutputTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
			}
		}

		private void CalculateTransferStartVolume(PointTag TransferStatus, PointTag TransferStartGOV, PointTag TransferStartNSV, PointTag TransferStartVolume)
		{
			if (TransferStartVolume.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferStartVolume.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			// check the status if the input variables
			if ((!IsValueGood(TransferStartGOV)
			&& this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
			|| (!IsValueGood(TransferStartNSV)
			&& this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.NetStandardVolume))
			{
				if (TransferStartVolume.Value != null ||
					TransferStartVolume.Status != StatusCodes.Bad)
				{
					TransferStartVolume.Value = null;
					TransferStartVolume.Status = StatusCodes.Bad;
					TransferStartVolume.ServerTimeStamp = DateTimeOffset.UtcNow;
					TransferStartVolume.SourceTimeStamp = DateTimeOffset.UtcNow;
				}
				return;
			}

			double? newValue=null; 
			var newStatus = StatusCodes.Good;

			if ((TransferStatuses)TransferStatus.Value == TransferStatuses.InProgress
			|| (TransferStatuses)TransferStatus.Value == TransferStatuses.Complete)
			{

				if (this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
				{
					newValue = (double)TransferStartGOV.Value;

					if (IsStatusUncertain(TransferStartGOV))
					{
						newStatus = StatusCodes.Uncertain;
					}
				}
				else
				{
					newValue = (double)TransferStartNSV.Value;

					if (IsStatusUncertain(TransferStartNSV))
					{
						newStatus = StatusCodes.Uncertain;
					}
				}
			}

			if (TransferStartVolume.Value == null
			|| (double)TransferStartVolume.Value != newValue
			|| IsStatusChange(TransferStartVolume.Status, newStatus))
			{
				TransferStartVolume.Value = newValue;
				TransferStartVolume.Status = newStatus;
				if (this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
				{
					base.SetTimeStamps(new PointTag[] { TransferStartGOV }, TransferStartVolume);
				}
				else
				{
					base.SetTimeStamps(new PointTag[] { TransferStartNSV }, TransferStartVolume);
				}
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
			&& this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
			|| (!IsValueGood(TransferredNSV)
			&& this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.NetStandardVolume))
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

				if (this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
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
				if (this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
				{
					base.SetTimeStamps(new PointTag[] { TransferredGOV }, TransferredVolume);
				}
				else
				{
					base.SetTimeStamps(new PointTag[] { TransferredNSV }, TransferredVolume);
				}
			}
		}


		private void CalculateTransferTargetFromTargetBatchNSV(	PointTag TransferMode, PointTag TransferStatus,
																					PointTag ProdTemperature, PointTag ProdDensity, PointTag ProdDensityInAir, PointTag Mass,
																					PointTag VCF, PointTag VolumeBottom, PointTag TankShellCorrection, PointTag PercentBSW,
																					PointTag LevelProductMax, PointTag LevelProductMin,
																					PointTag VolumeGrossObserved, PointTag VolumeNetStandard,
																					PointTag VolumeGrossObservedAvailable, PointTag VolumeGrossObservedRemaining,
																					PointTag TransferStartGOV, PointTag TransferStartNSV,
																					PointTag TransferTarget, PointTag TransferStartVolume, PointTag OutputTarget)
		{

			if (!IsValueGood(VolumeGrossObserved)
			|| !IsValueGood(VolumeNetStandard))
			{
				return;
			}

			PointTag CriticalZone = new PointTag() { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };
			PointTag VolumeRoofCorrection = new PointTag(VolumeGrossObserved) { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };
			PointTag StrapVolume = new PointTag(VolumeGrossObserved) { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };
			PointTag TransferTargetGOV = new PointTag(VolumeGrossObserved) { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };
			PointTag TransferTargetNSV = new PointTag(VolumeNetStandard) { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };
			PointTag StrapLevel = new PointTag(LevelProductMax) { InputOutputType = PointTemplateTag.PointTagInputOutputType.Calculated };


			if ((TankTransferMode)TransferMode.Value == FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Batch)
			{
				TransferTargetNSV.Value = (double)TransferStartNSV.Value + (double)TransferTarget.Value;
				base.SetTimeStamps(new PointTag[] { TransferStartNSV, TransferTarget }, TransferTargetNSV);

				Quantities.CalculateGrossObservedVolumeFromNetStandardVolume(TransferTargetNSV, PercentBSW, VCF, TransferTargetGOV);
				OutputTarget.Value = (double)TransferTargetGOV.Value - (double)TransferStartGOV.Value;
				OutputTarget.EngineeringUnitsType = VolumeGrossObservedAvailable.EngineeringUnitsType;
				OutputTarget.Units = VolumeGrossObservedAvailable.Units;
				OutputTarget.DecimalPlaces = VolumeGrossObservedAvailable.DecimalPlaces;
				OutputTarget.Maximum = (double)VolumeGrossObservedAvailable.Value;
				OutputTarget.Minimum = -(double)VolumeGrossObservedRemaining.Value;
				OutputTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
				OutputTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
				OutputTarget.UpdatedDate = DateTimeOffset.UtcNow;
			}
			else
			{
				TransferTargetNSV.Value = (double)TransferStartNSV.Value + (double)TransferTarget.Value;

				Quantities.CalculateGrossObservedVolumeFromNetStandardVolume(TransferTargetNSV, PercentBSW, VCF, TransferTargetGOV);

				VolumeRoofCorrection.Value = 0.0;
				Quantities.CalculateStrapVolumeFromGrossObservedVolume(VolumeRoofCorrection, TransferTargetGOV, VolumeBottom, TankShellCorrection, StrapVolume);
				StrapTable.StrapLevelCalculation(StrapVolume, StrapLevel);

				RoofCorrection.FloatingRoofCorrectionCalculation(ProdTemperature, ProdDensity, ProdDensityInAir, Mass, StrapLevel, VCF, CriticalZone, VolumeRoofCorrection);
				if ((double)VolumeRoofCorrection.Value != 0.0)
				{
					Quantities.CalculateStrapVolumeFromGrossObservedVolume(VolumeRoofCorrection, TransferTargetGOV, VolumeBottom, TankShellCorrection, StrapVolume);
					StrapTable.StrapLevelCalculation(StrapVolume, StrapLevel);
				}

				OutputTarget.Value = StrapLevel.Value;
				OutputTarget.EngineeringUnitsType = LevelProductMax.EngineeringUnitsType;
				OutputTarget.Units = LevelProductMax.Units;
				OutputTarget.DecimalPlaces = LevelProductMax.DecimalPlaces;
				OutputTarget.Maximum = (double)LevelProductMax.Value;
				OutputTarget.Minimum = (double)LevelProductMin.Value;
				OutputTarget.UpdatedDate = DateTimeOffset.UtcNow;
				OutputTarget.ServerTimeStamp = DateTimeOffset.UtcNow;
				OutputTarget.SourceTimeStamp = DateTimeOffset.UtcNow;
			}
		}



		private void CalculateTransferredGrossObservedVolume(PointTag TransferStartGOV, PointTag VolumeGrossObserved, PointTag TransferredGOV, PointTag TransferredVolume)
		{
			if (TransferredGOV.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferredGOV.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}


			// check the status if the input variables
			if (!IsValueGood(TransferStartGOV)
			|| !IsValueGood(VolumeGrossObserved))
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


			var newValue = (double)VolumeGrossObserved.Value - (double)TransferStartGOV.Value;
			var newStatus = StatusCodes.Good;
			if (IsStatusUncertain(TransferStartGOV)
			|| IsStatusUncertain(VolumeGrossObserved))
			{
				newStatus = StatusCodes.Uncertain;
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

		private void CalculateTransferredNetStandardVolume(PointTag TransferStartNSV, PointTag VolumeNetStandard, PointTag TransferredNSV, PointTag TransferredVolume)
		{
			if (TransferredNSV.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferredNSV.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}


			// check the status if the input variables
			if (!IsValueGood(TransferStartNSV)
			|| !IsValueGood(VolumeNetStandard))
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

			var newValue = (double)VolumeNetStandard.Value - (double)TransferStartNSV.Value;
			var newStatus = StatusCodes.Good;
			if (IsStatusUncertain(VolumeNetStandard)
			|| IsStatusUncertain(TransferStartNSV))
			{
				newStatus = StatusCodes.Uncertain;
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

		private void CalculateTransferredWaterVolume(PointTag TransferStartVolumeWater, PointTag VolumeWater, PointTag TransferredVolumeWater)
		{
			if (TransferredVolumeWater.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			TransferredVolumeWater.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}


			// check the status if the input variables
			if (!IsValueGood(TransferStartVolumeWater)
			|| !IsValueGood(VolumeWater))
			{
				if (TransferredVolumeWater.Value != null ||
					TransferredVolumeWater.Status != StatusCodes.Bad)
				{
					TransferredVolumeWater.Value = null;
					TransferredVolumeWater.Status = StatusCodes.Bad;
					TransferredVolumeWater.ServerTimeStamp = DateTimeOffset.UtcNow;
					TransferredVolumeWater.SourceTimeStamp = DateTimeOffset.UtcNow;
				}
				return;
			}


			var newValue = (double)VolumeWater.Value - (double)TransferStartVolumeWater.Value;
			var newStatus = StatusCodes.Good;
			if (IsStatusUncertain(TransferStartVolumeWater)
			|| IsStatusUncertain(VolumeWater))
			{
				newStatus = StatusCodes.Uncertain;
			}


			if (TransferredVolumeWater.Value == null
			|| (double)TransferredVolumeWater.Value != newValue
			|| IsStatusChange(TransferredVolumeWater.Status, newStatus))
			{
				TransferredVolumeWater.Value = newValue;
				TransferredVolumeWater.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { VolumeWater, TransferStartVolumeWater }, TransferredVolumeWater);
			}
		}



		private void CalculateTankCommand(	PointTag TransferMode, PointTag TransferStatus, PointTag TransferTarget,
														PointTag TransferStartLevel, PointTag TankCommand)
		{
			var tankCommand = TankCommands.Stop;

			if((TransferStatuses) TransferStatus.Value == TransferStatuses.InProgress)
			{
				if((TankTransferMode) TransferMode.Value == FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Level)
				{
					tankCommand = ((double)TransferTarget.Value > (double)TransferStartLevel.Value) ? TankCommands.Fill : TankCommands.Empty;
				}

				else
				{
					if(this.TankTransferSettings.TransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
					{
						tankCommand = ((double)TransferTarget.Value > 0.0) ? TankCommands.Fill : TankCommands.Empty;
					}
					else
					{
						tankCommand = ((double)TransferTarget.Value > 0.0) ? TankCommands.Fill : TankCommands.Empty;
					}
				}
			}

			if (TankCommand.Value == null
			|| ((TankCommands)TankCommand.Value != TankCommands.Run && (TankCommands)TankCommand.Value != tankCommand))
			{
				TankCommand.Value = tankCommand;
				TankCommand.Status = StatusCodes.Good;
				TankCommand.ServerTimeStamp = DateTimeOffset.UtcNow;
				TankCommand.SourceTimeStamp = DateTimeOffset.UtcNow;
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
