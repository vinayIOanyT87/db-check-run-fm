namespace NodeTransfer
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


	public class FMNodeTransfer : FuelsManagerModule, IFuelsManagerModule
	{
		private bool firstTimeFlag = true;

		private NodeTransferMode? currentTransferMode;


		public void TransferCalculation(
			PointTag TransferMode, PointTag TransferStatus,
			PointTag TransferStartTime, PointTag TransferStopTime)
		{
			CalculateTransferInactive(TransferMode, TransferStatus, TransferStartTime, TransferStopTime);

			CalculateTransferInProgress(TransferMode, TransferStatus, TransferStartTime, TransferStopTime);

			this.currentTransferMode = (NodeTransferMode?)TransferMode.Value;

			firstTimeFlag = false;
		}

		private void CalculateTransferInactive(PointTag TransferMode, PointTag TransferStatus, PointTag TransferStartTime, PointTag TransferStopTime)
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


			if ((NodeTransferMode)TransferMode.Value != FMBusinessObjects.DataObjects.CodedVariables.NodeTransferMode.Inactive)
			{
				return;
			}

			List<PointTag> tagList = new List<PointTag>();
			var security = new SecurityClass() { UserID = "FMPointService" };

			var newValue = NodeTransferStatus.Inactive;
			var newStatus = StatusCodes.Good;

			if (!this.firstTimeFlag
			&& this.currentTransferMode != (NodeTransferMode)TransferMode.Value)
			{
				tagList.Add(TransferMode);
			}

			if (TransferStatus.Value == null
			|| (NodeTransferStatus)TransferStatus.Value != newValue
			|| IsStatusChange(TransferStatus.Status, newStatus))
			{
				TransferStatus.Value = newValue;
				TransferStatus.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferStatus);
				tagList.Add(TransferStatus);
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

			if (tagList.Count > 0)
			{
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
			}
		}

		private void CalculateTransferInProgress(PointTag TransferMode, PointTag TransferStatus, PointTag TransferStartTime, PointTag TransferStopTime)
		{
			if (!IsValueGood(TransferMode)
			|| !IsValueGood(TransferStatus))
			{
				return;
			}

			if ((NodeTransferMode)TransferMode.Value != FMBusinessObjects.DataObjects.CodedVariables.NodeTransferMode.Batch)
			{
				return;
			}

			var newValue = NodeTransferStatus.InProgress;
			var newStatus = StatusCodes.Good;

			List<PointTag> tagList = new List<PointTag>();
			var security = new SecurityClass() { UserID = "FMPointService" };

			// Initial Execution
			if (!this.firstTimeFlag
			&& this.currentTransferMode != (NodeTransferMode)TransferMode.Value)
			{
				tagList.Add(TransferMode);
			}


			if (this.currentTransferMode != (NodeTransferMode)TransferMode.Value)
			{
				this.currentTransferMode = (NodeTransferMode)TransferMode.Value;
				tagList.Add(TransferMode);
			}

			if (TransferStatus.Value == null
			|| (NodeTransferStatus)TransferStatus.Value != newValue
			|| IsStatusChange(TransferStatus.Status, newStatus))
			{
				TransferStatus.Value = newValue;
				TransferStatus.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferStatus);
				tagList.Add(TransferStatus);
			}

			if (TransferStartTime.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& (TransferStartTime.Value == null
			|| IsStatusChange(TransferStartTime.Status, newStatus)))
			{
				TransferStartTime.Value = DateTimeOffset.UtcNow;
				TransferStartTime.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferStartTime);
				tagList.Add(TransferStartTime);
			}


			if (TransferStopTime.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
			&& (TransferStopTime.Value != null
			|| IsStatusChange(TransferStopTime.Status, newStatus)))
			{
				TransferStopTime.Value = null;
				TransferStopTime.Status = newStatus;
				base.SetTimeStamps(new PointTag[] { TransferMode }, TransferStopTime);
				tagList.Add(TransferStopTime);
			}

			if (tagList.Count > 0)
			{
				FMChannelHelper.MakeCall<IPointTags>(x => x.ModifyTagValues(security, tagList, false));
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
