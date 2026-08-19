namespace Movement
{
	using System;
	using System.Linq;
	using System.Diagnostics;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using FMPointCommon;
	using System.Collections.Generic;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.BusinessInterfaces;
	using Opc.Ua;


	public class FMMovement : FuelsManagerModule, IFuelsManagerModule
	{
		private static readonly EventLog EventLog = new EventLog("Application", ".", "Varec Movement Module");

		public MovementModuleSettings MovementModuleSettings { get; set; }

		public MovementData MovementData { get; set; }

		private MovementCommand? currentCommand;

		private bool FirstTimeFlag = true;

		private DateTimeOffset? nextExecutionTime;

		public SetPointTagHandler SetPointTag = null;

		public SetPointPropertyHandler SetPointProperty = null;

		private const int executionInterval = 10;

		private SecurityClass security = new SecurityClass() { UserID = "FMPointService" };

		private List<MovementNodeData> movementNodeDataCopy = new List<MovementNodeData>();


		public void MovementCalculation(
			PointTag PercentDeviation, PointTag Command,PointTag Status,
			PointTag TransferStartTime, PointTag TransferStopTime,
			PointTag InitiationCount, PointTag MovementHistoryWrittenTime,
			PointTag TransferredGOV, PointTag TransferredNSV,
			PointTag TransferTimeRemaining, PointTag StartIdentity, PointTag StopIdentity,
			PointTag MovementDiscreteAlarm)
		{
			CalculateInitialization(Status, MovementDiscreteAlarm);

			CalculateInitiateIdentity(StartIdentity, MovementDiscreteAlarm);

			CalculateStopIdentity(StopIdentity, MovementDiscreteAlarm);

			CalculateMovementStop(PercentDeviation, Command, Status, TransferStartTime, TransferStopTime);

			CalculateMovementInitiate(Command, Status, TransferStartTime, TransferStopTime, InitiationCount);

			CalculateMovementNonZeroFlow(Command, Status, TransferStartTime);

			CalculateMovementHoldForHandGaugeData(Command, Status);

			CalculateMovementZeroFlow(Command, Status);

			CalculateMovementDisable(Command, Status);

			this.currentCommand = (MovementCommand?)Command.Value;

			this.movementNodeDataCopy = MovementModuleSettings.MovementNodeDataList;

			this.FirstTimeFlag = false;
		}

		private void CalculateMovementDiscreteAlarm(PointTag MovementDiscreteAlarm, short status)
		{
			if (MovementDiscreteAlarm.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
				MovementDiscreteAlarm.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			MovementDiscreteAlarm.Value = status;
			MovementDiscreteAlarm.Status = StatusCodes.Good;
			MovementDiscreteAlarm.ServerTimeStamp = DateTimeOffset.UtcNow;
			MovementDiscreteAlarm.SourceTimeStamp = DateTimeOffset.UtcNow;
		}

		private void CalculateInitialization(PointTag Status, PointTag MovementDiscreteAlarm)
		{
			// Process only if not the FirstTimeFlag, which will indicated a revision to the MovementModuleSettings.MovementNodeDataList
			if (this.FirstTimeFlag)
			{
				return;
			}

			if (Status.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			Status.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if(!IsValueGood(Status))
			{
				return;
			}

			if ((MovementStatus)Status.Value == MovementStatus.Inactive
			|| (MovementStatus)Status.Value == MovementStatus.Disabled)
			{
				return;
			}


			// Compare current MovementNodeDataList with Copy
			var firstNotSecond = this.movementNodeDataCopy.Except(MovementModuleSettings.MovementNodeDataList).ToList();
			var secondNotFirst = MovementModuleSettings.MovementNodeDataList.Except(this.movementNodeDataCopy).ToList();

			if (!firstNotSecond.Any() && !secondNotFirst.Any())
			{
				return;
			}

			// Reinitiate Movement to Apply Changes
			try
			{
				FMChannelHelper.MakeCall<IMovementService>(x => x.InitiateMovement(this.security, Status.PointGuid));
				CalculateMovementDiscreteAlarm(MovementDiscreteAlarm, MovementModuleSettings.MovementDiscreteAlarm_Normal);
				return;
			}
			catch (Exception e)
			{
				EventLog.WriteEntry(e.Message);
				CalculateMovementDiscreteAlarm(MovementDiscreteAlarm, MovementModuleSettings.MovementDiscreteAlarm_ControlAlarm);
			}
		}

		private void CalculateInitiateIdentity(PointTag InitiateIdentity, PointTag MovementDiscreteAlarm)
		{
			Guid initiateIdentityGuid;

			if (!(InitiateIdentity.Value is String)
			|| !Guid.TryParse(InitiateIdentity.Value as String, out initiateIdentityGuid))
			{
				return;
			}

			InitiateIdentity.Value = null;

			// Start Movement
			try
			{
				if (initiateIdentityGuid == InitiateIdentity.PointGuid)
				{
					FMChannelHelper.MakeCall<IMovementService>(x => x.InitiateMovement(this.security, initiateIdentityGuid));
					CalculateMovementDiscreteAlarm(MovementDiscreteAlarm, MovementModuleSettings.MovementDiscreteAlarm_Normal);
					return;
				}

				// Stop Node
				else
				{
					var movementNodeData = MovementModuleSettings.MovementNodeDataList.SingleOrDefault(x => x.MovementNodeGuid == initiateIdentityGuid);

					if (movementNodeData != null)
					{
						FMChannelHelper.MakeCall<IMovementService>(x => x.InitiateMovementNode(this.security, InitiateIdentity.PointGuid, movementNodeData.MovementNodeGuid));
						CalculateMovementDiscreteAlarm(MovementDiscreteAlarm, MovementModuleSettings.MovementDiscreteAlarm_Normal);
						return;
					}
				}
			}
			catch(Exception e)
			{
				EventLog.WriteEntry(e.Message);
				CalculateMovementDiscreteAlarm(MovementDiscreteAlarm, MovementModuleSettings.MovementDiscreteAlarm_ControlAlarm);
			}

			return;
		}

		private void CalculateStopIdentity(PointTag StopIdentity, PointTag MovementDiscreteAlarm)
		{
			Guid stopIdentityGuid;

			if (!(StopIdentity.Value is String)
			|| !Guid.TryParse(StopIdentity.Value as String, out stopIdentityGuid))
			{
				return;
			}

			StopIdentity.Value = null;

			// Stop Movement
			try
			{
				if (stopIdentityGuid == StopIdentity.PointGuid)
				{
					FMChannelHelper.MakeCall<IMovementService>(x => x.StopMovement(this.security, stopIdentityGuid));
					CalculateMovementDiscreteAlarm(MovementDiscreteAlarm, MovementModuleSettings.MovementDiscreteAlarm_Normal);
					return;
				}

				// Stop Node
				else
				{
					var movementNodeData = MovementModuleSettings.MovementNodeDataList.SingleOrDefault(x => x.MovementNodeGuid == stopIdentityGuid);

					if (movementNodeData != null)
					{
						FMChannelHelper.MakeCall<IMovementService>(x => x.StopMovementNode(this.security, movementNodeData.MovementNodeGuid));
						CalculateMovementDiscreteAlarm(MovementDiscreteAlarm, MovementModuleSettings.MovementDiscreteAlarm_Normal);
						return;
					}
				}
			}
			catch (Exception e)
			{
				EventLog.WriteEntry(e.Message);
				CalculateMovementDiscreteAlarm(MovementDiscreteAlarm, MovementModuleSettings.MovementDiscreteAlarm_ControlAlarm);
			}


			return;
		}


		private void CalculateMovementStop(PointTag PercentDeviation, PointTag Command, PointTag Status, PointTag TransferStartTime, PointTag TransferStopTime)
		{
			if (Status.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			Status.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(Command)
			|| !IsValueGood(Status))
			{
				return;
			}


			if ((MovementCommand)Command.Value != MovementCommand.Stop)
			{
				return;
			}

			if((MovementStatus) Status.Value == MovementStatus.Inactive)
			{
				return;
			}

			var newValue = MovementStatus.Inactive;

			List<PointTag> tagList = new List<PointTag>();

			var newStatus = StatusCodes.Good;

			if (!this.FirstTimeFlag
			&& this.currentCommand != (MovementCommand)Command.Value)
			{
				tagList.Add(Command);
			}

			if (Status.Value == null
			|| (MovementStatus)Status.Value != newValue
			|| IsStatusChange(Status.Status, newStatus))
			{
				Status.Value = newValue;
				Status.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { Command }, Status);
				tagList.Add(Status);
			}

			if (TransferStartTime.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& TransferStartTime.OpcStatusSubCode != StatusCodes.GoodLocalOverride
			&& (TransferStartTime.Value != null
			|| IsStatusChange(PercentDeviation.Status, newStatus)))
			{
				TransferStartTime.Value = null;
				TransferStartTime.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { Command }, TransferStartTime);
				tagList.Add(TransferStartTime);
			}


			if (TransferStopTime.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& TransferStopTime.OpcStatusSubCode != StatusCodes.GoodLocalOverride
			&& (TransferStopTime.Value == null
			|| IsStatusChange(TransferStopTime.Status, newStatus)))
			{
				TransferStopTime.Value = DateTimeOffset.UtcNow;
				TransferStopTime.Status = StatusCodes.Good;
				base.SetTimeStamps(new PointTag[] { Command }, TransferStopTime);
				tagList.Add(TransferStopTime);
			}


			if (tagList.Count > 0)
			{
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(this.security, tagList, false));
			}
		}

		private void CalculateMovementInitiate(PointTag Command, PointTag Status, PointTag TransferStartTime, PointTag TransferStopTime, PointTag InitiationCount)
		{
			if (Status.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			Status.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(Command)
			&& !IsValueGood(Status))
			{
				return;
			}


			if ((MovementCommand)Command.Value != MovementCommand.Initiate)
			{
				return;
			}

			if ((MovementStatus)Status.Value == MovementStatus.Active
			|| (MovementStatus)Status.Value == MovementStatus.Starting
			|| (MovementStatus)Status.Value == MovementStatus.Halted)
			{
				return;
			}

			MovementStatus newValue;

			if (!this.MovementModuleSettings.StartTimeBasedOnNonZeroFlow)
			{
				newValue = MovementStatus.Active;
			}

			else
			{
				if (Status.Value is MovementStatus
				&& (MovementStatus)Status.Value == MovementStatus.Starting)
				{
					newValue = MovementStatus.Active;
				}
				else
				{
					newValue = MovementStatus.Starting;
				}
			}

			var newStatus = StatusCodes.Good;

			List<PointTag> tagList = new List<PointTag>();


			if (!this.FirstTimeFlag
			&& this.currentCommand != (MovementCommand)Command.Value)
			{
				tagList.Add(Command);
			}

			var previousStatus = Status.Value;

			if (Status.Value == null
			|| (MovementStatus)Status.Value != newValue
			|| IsStatusChange(Status.Status, newStatus))
			{
				Status.Value = newValue;
				Status.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { Command }, Status);
				tagList.Add(Status);
			}

			if (previousStatus is MovementStatus
			&& (MovementStatus)previousStatus != MovementStatus.Stopping)
			{
				if (InitiationCount.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
				&& InitiationCount.OpcStatusSubCode != StatusCodes.GoodLocalOverride)
				{
					if (InitiationCount.Value == null)
					{
						InitiationCount.Value = (short)1;
					}
					else
					{
						InitiationCount.Value = (short)((short)InitiationCount.Value + (short)1);
					}
					InitiationCount.Status = StatusCodes.Good;
					base.SetTimeStamps(new PointTag[] { Command }, InitiationCount);
					tagList.Add(InitiationCount);
				}

				if ((MovementStatus)Status.Value == MovementStatus.Active)
				{
					if (TransferStartTime.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
					&& TransferStartTime.OpcStatusSubCode != StatusCodes.GoodLocalOverride
					&& (TransferStartTime.Value == null
					|| IsStatusChange(TransferStartTime.Status, newStatus)))
					{
						TransferStartTime.Value = DateTimeOffset.UtcNow;
						TransferStartTime.Status = StatusCodes.Good;
						base.SetTimeStamps(new PointTag[] { Command }, TransferStartTime);
						tagList.Add(TransferStartTime);
					}
				}

				if (TransferStopTime.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
				&& TransferStopTime.OpcStatusSubCode != StatusCodes.GoodLocalOverride
				&& (TransferStopTime.Value != null
				|| IsStatusChange(TransferStopTime.Status, newStatus)))
				{
					TransferStopTime.Value = null;
					TransferStopTime.Status = StatusCodes.Good;
					base.SetTimeStamps(new PointTag[] { Command }, TransferStopTime);
					tagList.Add(TransferStopTime);
				}
			}

			if (tagList.Count > 0)
			{
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(this.security, tagList, false));
			}
		}

		private void CalculateMovementNonZeroFlow(PointTag Command, PointTag Status, PointTag TransferStartTime)
		{
			if (Status.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			Status.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(Command)
			&& !IsValueGood(Status))
			{
				return;
			}


			if ((MovementCommand)Command.Value != MovementCommand.NonZeroFlow)
			{
				return;
			}

			if ((MovementStatus)Status.Value != MovementStatus.Starting)
			{
				return;
			}

			var newValue = MovementStatus.Active;
			var newStatus = StatusCodes.Good;

			List<PointTag> tagList = new List<PointTag>();


			if (!this.FirstTimeFlag
			&& this.currentCommand != (MovementCommand)Command.Value)
			{
				tagList.Add(Command);
			}

			if (Status.Value == null
			|| (MovementStatus)Status.Value != newValue
			|| IsStatusChange(Status.Status, newStatus))
			{
				Status.Value = newValue;
				Status.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { Command }, Status);
				tagList.Add(Status);
			}

			if (TransferStartTime.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& TransferStartTime.OpcStatusSubCode != StatusCodes.GoodLocalOverride
			&& (TransferStartTime.Value == null
			|| IsStatusChange(TransferStartTime.Status, newStatus)))
			{
				TransferStartTime.Value = DateTimeOffset.UtcNow;
				TransferStartTime.Status = StatusCodes.Good;
				base.SetTimeStamps(new PointTag[] { Command }, TransferStartTime);
				tagList.Add(TransferStartTime);
			}

			if (tagList.Count > 0)
			{
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(this.security, tagList, false));
			}
		}



		private void CalculateMovementHoldForHandGaugeData(PointTag Command, PointTag Status)
		{
			if (Status.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			Status.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(Command)
			|| !IsValueGood(Status))
			{
				return;
			}


			if ((MovementCommand)Command.Value != MovementCommand.HoldForHandgaugeData)
			{
				return;
			}

			if ((MovementStatus)Status.Value == MovementStatus.Inactive)
			{
				return;
			}

			var newValue = MovementStatus.Halted;

			List<PointTag> tagList = new List<PointTag>();

			var newStatus = StatusCodes.Good;

			if (!this.FirstTimeFlag
			&& this.currentCommand != (MovementCommand)Command.Value)
			{
				tagList.Add(Command);
			}

			if (Status.Value == null
			|| (MovementStatus)Status.Value != newValue
			|| IsStatusChange(Status.Status, newStatus))
			{
				Status.Value = newValue;
				Status.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { Command }, Status);
				tagList.Add(Status);
			}

			if (tagList.Count > 0)
			{
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(this.security, tagList, false));
			}
		}


		private void CalculateMovementZeroFlow(PointTag Command, PointTag Status)
		{
			if (Status.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			Status.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(Command)
			|| !IsValueGood(Status))
			{
				return;
			}


			if ((MovementCommand)Command.Value != MovementCommand.ZeroFlow)
			{
				return;
			}

			if ((MovementStatus)Status.Value == MovementStatus.Inactive)
			{
				return;
			}

			var newValue = MovementStatus.Stopping;

			List<PointTag> tagList = new List<PointTag>();

			var newStatus = StatusCodes.Good;

			if (!this.FirstTimeFlag
			&& this.currentCommand != (MovementCommand)Command.Value)
			{
				tagList.Add(Command);
			}

			if (Status.Value == null
			|| (MovementStatus)Status.Value != newValue
			|| IsStatusChange(Status.Status, newStatus))
			{
				Status.Value = newValue;
				Status.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { Command }, Status);
				tagList.Add(Status);
			}

			if (tagList.Count > 0)
			{
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(this.security, tagList, false));
			}
		}


		private void CalculateMovementDisable(PointTag Command, PointTag Status)
		{
			if (Status.InputOutputType != PointTemplateTag.PointTagInputOutputType.Calculated ||
			Status.OpcStatusSubCode == StatusCodes.GoodLocalOverride)
			{
				return;
			}

			if (!IsValueGood(Command)
			|| !IsValueGood(Status))
			{
				return;
			}


			if ((MovementCommand)Command.Value != MovementCommand.Disable)
			{
				return;
			}

			// We can only disable Inactive movements; any other status should return immediately
			if ((MovementStatus)Status.Value != MovementStatus.Inactive)
			{
				return;
			}

			var newValue = MovementStatus.Disabled;

			List<PointTag> tagList = new List<PointTag>();

			var newStatus = StatusCodes.Good;

			if (!this.FirstTimeFlag
			&& this.currentCommand != (MovementCommand)Command.Value)
			{
				tagList.Add(Command);
			}

			if (Status.Value == null
			|| (MovementStatus)Status.Value != newValue
			|| IsStatusChange(Status.Status, newStatus))
			{
				Status.Value = newValue;
				Status.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { Command }, Status);
				tagList.Add(Status);
			}

			if (tagList.Count > 0)
			{
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(this.security, tagList, false));
			}
		}



		public ModuleInputOutputCollection GetInputOutputCollection(string calculationName)
		{
			var properties = new ModuleInputOutputCollection { };
			return properties;
		}
	}
}
