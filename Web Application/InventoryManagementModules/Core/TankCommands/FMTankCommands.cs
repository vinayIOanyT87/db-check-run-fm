
namespace TankCommands
{
	using System;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using FMPointCommon;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.BusinessInterfaces;
	using System.Collections.Generic;
	using Opc.Ua;

	public class FMTankCommands : FuelsManagerModule, IFuelsManagerModule
	{
		public TankCommandModuleSettings TankCommandSettings { get; set; }

		private TankCommands? CurrentTankCommand;

		private bool isResetAfterStopIssued = false;

      private const double FmlFtIn8Th  = 1.0 / 12.0 / 8.0;
      private const double FmlFtIn16Th = 1.0 / 12.0 / 16.0;

		private static double FmlFtIn8ThAdjustmentFactor = Math.Abs(FmlFtIn8Th - Math.Round(FmlFtIn8Th, 12));
		private static double FmlFtIn16ThAdjustmentFactor = Math.Abs(FmlFtIn16Th - Math.Round(FmlFtIn16Th, 12));

      private PointTagAlarmStatus MovementAlarmStatus = null;

		public FMTankCommands() : base() { }

		private PointTagAlarmStatus GetMovementAlarmStatus(PointTag TankModeAlarm)
		{
			Alarm tankModeAlarm = null;
			foreach (var alarm in TankModeAlarm.Alarms.Values)
			{
				if (alarm.ID == "Tank Mode Alarm")
				{
					tankModeAlarm = alarm;
					break;
				}
			}

			if (tankModeAlarm == null)
			{
				return null;
			}

			PointTagAlarmStatus movementAlarmStatus = null;
			foreach (var pointTagAlarmStatus in tankModeAlarm.AlarmStatus.Values)
			{
				if (pointTagAlarmStatus.AlarmTestID == "Movement Test")
				{
					movementAlarmStatus = pointTagAlarmStatus;
					break;
				}
			}

			return movementAlarmStatus;
		}



		public bool? TankCommandCalculation(PointTag TankCommand,
														PointTag FlowRate,
														PointTag LevelProduct,
														PointTag TankStatus,
														PointTag TankModeAlarm,
														PointTag LevelProductStop,
														PointTag LevelProductMovement)
		{
			this.MovementAlarmStatus = this.GetMovementAlarmStatus(TankModeAlarm);

			CalculateTankCommand(TankCommand);

			CalculateLevelProductStop(TankCommand, LevelProduct, LevelProductStop);

			CalculateLevelProductMovement(TankCommand, LevelProduct, LevelProductMovement, TankModeAlarm);

			CalculateTankStatus(TankCommand, FlowRate, TankStatus);

         CalculateTankModeAlarm(TankCommand, FlowRate, LevelProduct, LevelProductStop, LevelProductMovement, TankModeAlarm);

         return true;
		}


		public ModuleInputOutputCollection GetInputOutputCollection(string calculationName)
		{
			var properties = new ModuleInputOutputCollection
								{
									new ModuleInputOutput
									{
											ID = "Flow Rate",
											Type = typeof(double?),
											ParameterType = ModuleInputOutputType.Input
									}
								};
			return properties;
		}




		private void CalculateTankCommand(PointTag TankCommand)
		{
			isResetAfterStopIssued = false;
			if (!IsValueGood(TankCommand))
			{
				return;
			}

			TankCommands? newValue = (TankCommands)TankCommand.Value;

			if (((TankCommands)TankCommand.Value) == TankCommands.Reset)
			{
				if (CurrentTankCommand == null)
				{
					newValue = TankCommands.Stop;
				}
				else
				{
					newValue = CurrentTankCommand;
					if (CurrentTankCommand == TankCommands.Stop)
						isResetAfterStopIssued = true;
				}
			}			
			if ((newValue != CurrentTankCommand) || (newValue != (TankCommands)TankCommand.Value))
			{
				TankCommand.Value = newValue;
				TankCommand.ServerTimeStamp = DateTimeOffset.UtcNow;
				TankCommand.SourceTimeStamp = DateTimeOffset.UtcNow;
				var security = new SecurityClass() { UserID = "FMPointService" };
				List<PointTag> tagList = new List<PointTag>();
				tagList.Add(TankCommand);
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
			}
			CurrentTankCommand = newValue;
		}



		private void CalculateLevelProductStop(PointTag TankCommand, PointTag LevelProduct, PointTag LevelProductStop)
		{
			if (LevelProductStop.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual
			|| LevelProductStop.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			double? newValue;
			long newStatus = StatusCodes.Good;
			// make sure all of the input data is valid
			if ((!IsValueGood(TankCommand)) || (!IsValueGood(LevelProduct)))
			{
				newStatus = StatusCodes.Bad;
				newValue = null;
			}
			else
			{
				if (((TankCommands)TankCommand.Value) == TankCommands.Stop)
				{
					if ((LevelProductStop.Value == null) || isResetAfterStopIssued)
					{
						newValue = (double)LevelProduct.Value;
					}
					else
					{
						newValue = (double)LevelProductStop.Value;
					}
				}
				else
				{
					newValue = null;
				}

				if ((IsStatusUncertain(TankCommand)) || (IsStatusUncertain(LevelProduct)))
				{
					newStatus = StatusCodes.Uncertain;
				}
			}

			if ((double?)LevelProductStop.Value != newValue
			|| (IsStatusChange(LevelProductStop.Status, newStatus)))
			{
				LevelProductStop.Value = newValue;
				LevelProductStop.Status = newStatus;

				LevelProductStop.ServerTimeStamp = TankCommand.ServerTimeStamp;
				LevelProductStop.SourceTimeStamp = TankCommand.SourceTimeStamp;
				if (LevelProduct.SourceTimeStamp > LevelProductStop.SourceTimeStamp)
				{
					LevelProductStop.ServerTimeStamp = LevelProduct.ServerTimeStamp;
					LevelProductStop.SourceTimeStamp = LevelProduct.SourceTimeStamp;
				}

				var security = new SecurityClass() { UserID = "FMPointService" };
				List<PointTag> tagList = new List<PointTag>();
				tagList.Add(LevelProductStop);
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
			}
		}

		private void CalculateLevelProductMovement(PointTag TankCommand, PointTag LevelProduct, PointTag LevelProductMovement, PointTag TankModeAlarm)
		{
			if (LevelProductMovement.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual
			|| LevelProductMovement.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if(this.MovementAlarmStatus == null)
			{
				return;
			}

			double? newValue;
			long newStatus = StatusCodes.Good;

			// make sure all of the input data is valid
			if (!IsValueGood(TankCommand)
			|| !IsValueGood(LevelProduct)
			|| !IsValueGood(TankModeAlarm))
			{
				newStatus = StatusCodes.Bad;
				newValue = null;
			}
			else
			{
				if ((TankCommands)TankCommand.Value == TankCommands.Stop
				&& (short) TankModeAlarm.Value == TankCommandModuleSettings.TankModeAlarm_MovementAlarm)
				{

					if (this.MovementAlarmStatus.ReAlarmDone
					|| (this.MovementAlarmStatus.AlarmTestFailed
					&& LevelProductMovement.Value == null))
					{
						newValue = (double)LevelProduct.Value;
						this.MovementAlarmStatus.ReAlarm = false;
						this.MovementAlarmStatus.ReAlarmDone = false;
					}
					else if (LevelProductMovement.Value is Double
					&& this.MovementAlarmStatus.AlarmTestFailed)
					{
						newValue = (double)LevelProductMovement.Value;
					}
					else
					{
						this.MovementAlarmStatus.ReAlarm = false;
						newValue = null;
					}

					if (IsStatusUncertain(TankCommand)
					|| IsStatusUncertain(LevelProduct)
					|| IsStatusUncertain(TankModeAlarm))
					{
						newStatus = StatusCodes.Uncertain;
					}
				}
				else
				{
					this.MovementAlarmStatus.ReAlarm = false;
					this.MovementAlarmStatus.ReAlarmDone = false;
					newValue = null;
				}
			}

			if ((double?)LevelProductMovement.Value != newValue
			|| (IsStatusChange(LevelProductMovement.Status, newStatus)))
			{
				LevelProductMovement.Value = newValue;
				LevelProductMovement.Status = newStatus;

				LevelProductMovement.ServerTimeStamp = TankCommand.ServerTimeStamp;
				LevelProductMovement.SourceTimeStamp = TankCommand.SourceTimeStamp;
				if (LevelProduct.SourceTimeStamp > LevelProductMovement.SourceTimeStamp)
				{
					LevelProductMovement.ServerTimeStamp = LevelProduct.ServerTimeStamp;
					LevelProductMovement.SourceTimeStamp = LevelProduct.SourceTimeStamp;
				}

				var security = new SecurityClass() { UserID = "FMPointService" };
				List<PointTag> tagList = new List<PointTag>();
				tagList.Add(LevelProductMovement);
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
			}
		}

		private void CalculateTankStatus(PointTag TankCommand, PointTag FlowRate, PointTag TankStatus)
		{
			long newStatus = StatusCodes.Good;
			FMBusinessObjects.DataObjects.CodedVariables.TankStatuses newValue = 0;
			DateTimeOffset CurrentDateTime = DateTimeOffset.UtcNow;
			if (TankStatus.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual)
			{
				// make sure all of the input data is valid
				if (!IsValueGood(TankCommand))
				{
					if ((TankStatus.Value != null) || (TankStatus.Status != StatusCodes.Bad))
					{
						TankStatus.Value = null;
						TankStatus.Status = StatusCodes.Bad;
						TankStatus.ServerTimeStamp = CurrentDateTime;
						TankStatus.SourceTimeStamp = CurrentDateTime;
					}
					return;
				}

				if (IsStatusUncertain(TankCommand))
				{
					newStatus = StatusCodes.Uncertain;
				}
				switch ((FMBusinessObjects.DataObjects.CodedVariables.TankCommands)TankCommand.Value)
				{
					case TankCommands.Stop:
						newValue = FMBusinessObjects.DataObjects.CodedVariables.TankStatuses.Stopped;
						break;
					case TankCommands.Fill:
						newValue = FMBusinessObjects.DataObjects.CodedVariables.TankStatuses.Filling;
						break;
					case TankCommands.Empty:
						newValue = FMBusinessObjects.DataObjects.CodedVariables.TankStatuses.Emptying;
						break;
					case TankCommands.Run:
						newValue = FMBusinessObjects.DataObjects.CodedVariables.TankStatuses.Running;
						break;
					case TankCommands.Test:
						newValue = FMBusinessObjects.DataObjects.CodedVariables.TankStatuses.Testing;
						break;
				}
			}
			else if (TankStatus.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated)
			{
				if (TankStatus.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
					return;

				// make sure all of the input data is valid
				if (!IsValueGood(FlowRate))
				{
					if ((TankStatus.Value != null) || (TankStatus.Status != StatusCodes.Bad))
					{
						TankStatus.Value = null;
						TankStatus.Status = StatusCodes.Bad;
						TankStatus.ServerTimeStamp = CurrentDateTime;
						TankStatus.SourceTimeStamp = CurrentDateTime;
					}
					return;
				}

				if (IsStatusUncertain(FlowRate))
				{
					newStatus = StatusCodes.Uncertain;
				}
				if ((double)(FlowRate.Value) == 0)
				{
					newValue = FMBusinessObjects.DataObjects.CodedVariables.TankStatuses.Stopped;
				}
				else if ((double)(FlowRate.Value) < 0)
				{
					newValue = FMBusinessObjects.DataObjects.CodedVariables.TankStatuses.Emptying;
				}
				else if ((double)(FlowRate.Value) > 0)
				{
					newValue = FMBusinessObjects.DataObjects.CodedVariables.TankStatuses.Filling;
				}
			}

			if ((TankStatus.Value == null)
				|| ((FMBusinessObjects.DataObjects.CodedVariables.TankStatuses)TankStatus.Value != newValue)
				|| (IsStatusChange(TankStatus.Status, newStatus)))
			{
				TankStatus.Value = newValue;
				TankStatus.Status = newStatus;

				TankStatus.ServerTimeStamp = TankCommand.ServerTimeStamp;
				TankStatus.SourceTimeStamp = TankCommand.SourceTimeStamp;
				if (TankStatus.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated)
				{
					TankStatus.ServerTimeStamp = FlowRate.ServerTimeStamp;
					TankStatus.SourceTimeStamp = FlowRate.SourceTimeStamp;
				}
			}
		}


		private void CalculateTankModeAlarm(PointTag TankCommand, PointTag FlowRate, PointTag LevelProduct, PointTag LevelProductStop, PointTag LevelProductMovement, PointTag TankModeAlarm)
		{
			if (TankModeAlarm.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated
			|| TankModeAlarm.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(TankCommand))
			{
				return;
			}


			short? newValue = 0;
			long newStatus = StatusCodes.Good;
			DateTimeOffset CurrentDateTime = DateTimeOffset.UtcNow;
			if (IsStatusUncertain(TankCommand))
			{
				newStatus = StatusCodes.Uncertain;
			}

			if ((FMBusinessObjects.DataObjects.CodedVariables.TankCommands)TankCommand.Value == TankCommands.Reset)
			{
				return;
			}

			if (this.MovementAlarmStatus == null)
			{
				return;
			}

			if ((FMBusinessObjects.DataObjects.CodedVariables.TankCommands)TankCommand.Value == TankCommands.Stop)
			{
				if (!IsValueGood(LevelProduct)
				|| !IsValueGood(LevelProductStop))
				{
					newStatus = StatusCodes.Bad;
					newValue = null;
				}
				else
				{
					double adjFactor = LevelProduct.Units == EngineeringUnit.FmlFtIn8Th ? FmlFtIn8ThAdjustmentFactor 
						: LevelProduct.Units == EngineeringUnit.FmlFtIn16Th ? FmlFtIn16ThAdjustmentFactor : 0.0;
					double movementAlarmDifferential = (adjFactor > 0.0 && TankCommandSettings.MovementAlarmDifferential.Value > adjFactor) 
						? TankCommandSettings.MovementAlarmDifferential.Value - adjFactor : TankCommandSettings.MovementAlarmDifferential.Value;

               if (Math.Abs((double)LevelProduct.Value - (double)LevelProductStop.Value) >= movementAlarmDifferential
					&& (double)LevelProduct.Value != (double)LevelProductStop.Value)
               {
                  newValue = TankCommandModuleSettings.TankModeAlarm_MovementAlarm;
               }
					else
					{
						newValue = 0;
					}

               if (newValue == TankCommandModuleSettings.TankModeAlarm_MovementAlarm
					&& LevelProductMovement.Value is double
					&& TankCommandSettings.MovementAlarmDifferential.Value != 0.0)
					{
						if (Math.Abs((double)LevelProduct.Value - (double)LevelProductMovement.Value) >= movementAlarmDifferential) 
						{
                     // Update Time Stamp to effect hold off in Alarm Engine IsAlarmTestFailed
                     this.MovementAlarmStatus.ReAlarm = true;
                     TankModeAlarm.ServerTimeStamp = LevelProduct.ServerTimeStamp;
							TankModeAlarm.SourceTimeStamp = LevelProduct.SourceTimeStamp;
						}
						else
						{
							this.MovementAlarmStatus.ReAlarm = false;
							this.MovementAlarmStatus.ReAlarmDone = false;
						}
					}
					else
					{
                  this.MovementAlarmStatus.ReAlarm = false;
						this.MovementAlarmStatus.ReAlarmDone = false;
					}
				}
         }
			else
			{
				this.MovementAlarmStatus.ReAlarm = false;
				this.MovementAlarmStatus.ReAlarmDone = false;

				if ((FMBusinessObjects.DataObjects.CodedVariables.TankCommands)TankCommand.Value == TankCommands.Test)
				{
					newValue = TankCommandModuleSettings.TankModeAlarm_Testing;
				}
				else
				{
					if (!IsValueGood(FlowRate))
					{
						newStatus = StatusCodes.Bad;
						newValue = null;
					}
					else
					{
						if (((FMBusinessObjects.DataObjects.CodedVariables.TankCommands)TankCommand.Value == TankCommands.Fill)
							&& ((double)FlowRate.Value < 0))
						{
							newValue = TankCommandModuleSettings.TankModeAlarm_ReverseFlow;
						}
						else if (((FMBusinessObjects.DataObjects.CodedVariables.TankCommands)TankCommand.Value == TankCommands.Empty)
							&& ((double)FlowRate.Value > 0))
						{
							newValue = TankCommandModuleSettings.TankModeAlarm_ReverseFlow;
						}
						else if (((FMBusinessObjects.DataObjects.CodedVariables.TankCommands)TankCommand.Value == TankCommands.Fill)
							&& ((double)FlowRate.Value == 0))
						{
							newValue = TankCommandModuleSettings.TankModeAlarm_NoFlow;
						}
						else if (((FMBusinessObjects.DataObjects.CodedVariables.TankCommands)TankCommand.Value == TankCommands.Empty)
							&& ((double)FlowRate.Value == 0))
						{
							newValue = TankCommandModuleSettings.TankModeAlarm_NoFlow;
						}
					}
				}
			}

			if ((TankModeAlarm.Value == null)
				|| ((short)TankModeAlarm.Value != newValue)
				|| (IsStatusChange(TankModeAlarm.Status, newStatus)))
			{
				TankModeAlarm.Value = newValue;
				TankModeAlarm.Status = newStatus;

				if ((FMBusinessObjects.DataObjects.CodedVariables.TankCommands)TankCommand.Value == TankCommands.Stop)
				{
					this.SetTimeStamps(new PointTag [] { TankCommand, LevelProduct, LevelProductStop}, TankModeAlarm);
				}
				else if ((FMBusinessObjects.DataObjects.CodedVariables.TankCommands)TankCommand.Value != TankCommands.Test
				&& (FMBusinessObjects.DataObjects.CodedVariables.TankCommands)TankCommand.Value != TankCommands.Run)
				{
					this.SetTimeStamps(new PointTag[] { TankCommand, FlowRate }, TankModeAlarm);
				}

				var security = new SecurityClass() { UserID = "FMPointService" };
				List<PointTag> tagList = new List<PointTag>();
				tagList.Add(TankModeAlarm);
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
			}
		}
   }
}
