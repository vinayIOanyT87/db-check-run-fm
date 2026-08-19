namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.ServiceModel;
	using System.Security;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using FMBusinessObjects.Constants;
	using FMCore;
	using Opc.Ua;

	using DataAccessLayer;


	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class MovementService : FMServiceBase, IMovementService
	{

		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void InitiateMovement(SecurityClass security, Guid movementGuid)
		{
			security.ThrowIfNull(nameof(security));
			movementGuid.ThrowIfNull(nameof(movementGuid));

			var movementWellKnownTagGuidList = new Guid[] {
				Guids.MovementCommandGuid
			};

			var nodeWellKnownTagGuidList = new Guid[] {
				Guids.TransferModeGuid,
				Guids.TransferTargetGuid
			};

			// Get the actual movement point
			var points = new Points();
			var movementPoint = points.Get(security, movementGuid);

			// Check that we aren't blocked by other movements
			List<string> blockingMovements = CheckForActiveInterlockedMovements(security, movementGuid);
			if (blockingMovements.Count > 0)
			{
				string blockedMessage = $"Movement '{movementPoint.ID}' activation is blocked by active movements {string.Join(",", blockingMovements)}";
				throw new Exception(blockedMessage);
			}

			// Get the settings property for the movement
			var movementSettingsProperty = movementPoint.Properties.Values.Single(u => u.ValueTypeString == "FMBusinessObjects.DataObjects.MovementModuleSettings");
			var movementSettings = movementSettingsProperty.Value as MovementModuleSettings;

			var pointGuidList = new List<Guid>
			{
				movementPoint.PointGuid
			};

			// Get the point value identifier for the movement command (only specified tag in movementWellKnownTagGuidList) of the single movement to be initiated
			// We expect the returned list to have exactly one entry
			var pointTags = new PointTags();
			var movementPointValueIdentifierList = pointTags.EnumeratePointValueIdentifersByPointAndTagLists(security, pointGuidList, movementWellKnownTagGuidList.ToList());

			// Now get the point guids for the movement nodes
			// Skip nodes that are controlled individually instead of by the enclosing Movement
			pointGuidList.Clear();
			foreach (var movementNodeData in movementSettings.MovementNodeDataList)
			{
				// Nodes with IndividualNodeControl are Initiated by the User
				if(movementNodeData.IndividualNodeControl)
				{
					continue;
				}

				pointGuidList.Add(movementNodeData.MovementNodeGuid);
			}

			// Getting two tags per point, so expect a list of twice as many identifiers as nodes.
			var nodePointValueIdentifierList = pointTags.EnumeratePointValueIdentifersByPointAndTagLists(security, pointGuidList, nodeWellKnownTagGuidList.ToList());

			var pointServiceManager = new PointServiceManager();
			var movementPointValueList = pointServiceManager.GetPointValueData(security, movementPointValueIdentifierList, false); ;
			var nodePointValueList = pointServiceManager.GetPointValueData(security, nodePointValueIdentifierList, false);
			movementPointValueList[0].Value = MovementCommand.Initiate;
			movementPointValueList[0].Status = StatusCodes.Good;
			movementPointValueList[0].ServerTimeStamp = DateTimeOffset.UtcNow;
			movementPointValueList[0].SourceTimeStamp = DateTimeOffset.UtcNow;


			var nodeIndex = 0;
			foreach (var movementNodeData in movementSettings.MovementNodeDataList)
			{
				// Nodes with IndividualNodeControl are Initiated by the User
				if (movementNodeData.IndividualNodeControl)
				{
					continue;
				}

				if (movementNodeData.TransferMode == TransferModes.Batch)
				{
					if (nodePointValueList[nodeIndex].ValueTypeString == "FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode")
					{
						nodePointValueList[nodeIndex].Value = FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Batch;
					}
					else if (nodePointValueList[nodeIndex].ValueTypeString == "FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode")
					{
						nodePointValueList[nodeIndex].Value = FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode.Batch;
					}
					else if (nodePointValueList[nodeIndex].ValueTypeString == "FMBusinessObjects.DataObjects.CodedVariables.NodeTransferMode")
					{
						nodePointValueList[nodeIndex].Value = FMBusinessObjects.DataObjects.CodedVariables.NodeTransferMode.Batch;
					}

				}

				if (movementNodeData.TransferMode == TransferModes.Level)
				{
					if (nodePointValueList[nodeIndex].ValueTypeString == "FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode")
					{
						nodePointValueList[nodeIndex].Value = FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Level;
					}
				}



				nodePointValueList[nodeIndex].Status = StatusCodes.Good;
				nodePointValueList[nodeIndex].ServerTimeStamp = DateTimeOffset.UtcNow;
				nodePointValueList[nodeIndex].SourceTimeStamp = DateTimeOffset.UtcNow;


				nodePointValueList[nodeIndex + nodePointValueList.Count / 2].Value = movementNodeData.TransferTarget;
				nodePointValueList[nodeIndex + nodePointValueList.Count / 2].Status = StatusCodes.Good;
				nodePointValueList[nodeIndex + nodePointValueList.Count / 2].ServerTimeStamp = DateTimeOffset.UtcNow;
				nodePointValueList[nodeIndex + nodePointValueList.Count / 2].SourceTimeStamp = DateTimeOffset.UtcNow;

				nodeIndex++;
			}

			// Set status of interlocked movements to disabled in database and get list of those movements
			// to send a Disable command to.
			List<Guid> disabledMovementGuids = this.DisableInterlockedMovements(security, movementGuid);
			List<PointValueIdentifier> disablePointValueIdentifierList = pointTags.EnumeratePointValueIdentifersByPointAndTagLists(security, disabledMovementGuids, movementWellKnownTagGuidList.ToList());
			List<PointValue> disablePointValueList = pointServiceManager.GetPointValueData(security, disablePointValueIdentifierList, false);
			foreach (PointValue disablePointValue in disablePointValueList)
			{
				disablePointValue.Value = MovementCommand.Disable;
				disablePointValue.Status = StatusCodes.Good;
				disablePointValue.ServerTimeStamp = DateTimeOffset.UtcNow;
				disablePointValue.SourceTimeStamp = DateTimeOffset.UtcNow;
			}

			pointServiceManager.SetPointValueData(security, disablePointValueList, false);
			pointServiceManager.SetPointValueData(security, movementPointValueList, false);
			pointServiceManager.SetPointValueData(security, nodePointValueList.Where(x => x.PointValueIdentifier.IdentityGuid != Guid.Empty).ToList(), false);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void StopMovement(SecurityClass security, Guid movementGuid)
		{
			security.ThrowIfNull(nameof(security));
			movementGuid.ThrowIfNull(nameof(movementGuid));

			var movementWellKnownTagGuidList = new Guid[] {
				Guids.MovementCommandGuid
			};

			var nodeWellKnownTagGuidList = new Guid[] {
				Guids.TransferModeGuid
			};

			var points = new Points();
			var movementPoint = points.Get(security, movementGuid);

			// Get the settings property for the movement
			var movementSettingsProperty = movementPoint.Properties.Values.Single(u => u.ValueTypeString == "FMBusinessObjects.DataObjects.MovementModuleSettings");
			var movementSettings = movementSettingsProperty.Value as MovementModuleSettings;

			var pointGuidList = new List<Guid>
			{
				movementPoint.PointGuid
			};

			// Get the point value identifier for the movement command (only specified tag in movementWellKnownTagGuidList) of the single movement to be initiated
			// We expect the returned list to have exactly one entry
			var pointTags = new PointTags();
			var movementPointValueIdentifierList = pointTags.EnumeratePointValueIdentifersByPointAndTagLists(security, pointGuidList, movementWellKnownTagGuidList.ToList());

			// Now get the point guids for the movement nodes
			// Skip nodes that are controlled individually instead of by the enclosing Movement
			pointGuidList.Clear();
			foreach (var movementNodeData in movementSettings.MovementNodeDataList)
			{
				pointGuidList.Add(movementNodeData.MovementNodeGuid);
			}

			// Getting one tag per point, so expect a list of as many identifiers as nodes.
			var nodePointValueIdentifierList = pointTags.EnumeratePointValueIdentifersByPointAndTagLists(security, pointGuidList, nodeWellKnownTagGuidList.ToList());

			var pointServiceManager = new PointServiceManager();
			var movementPointValueList = pointServiceManager.GetPointValueData(security, movementPointValueIdentifierList, false);
			var nodePointValueList = pointServiceManager.GetPointValueData(security, nodePointValueIdentifierList, false);
			movementPointValueList[0].Value = MovementCommand.Stop;
			movementPointValueList[0].Status = StatusCodes.Good;
			movementPointValueList[0].ServerTimeStamp = DateTimeOffset.UtcNow;
			movementPointValueList[0].SourceTimeStamp = DateTimeOffset.UtcNow;


			var nodeIndex = 0;
			foreach (var movementNodeData in movementSettings.MovementNodeDataList)
			{
				if (nodePointValueList[nodeIndex].ValueTypeString == "FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode")
				{
					nodePointValueList[nodeIndex].Value = FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Inactive;
				}
				else if(nodePointValueList[nodeIndex].ValueTypeString == "FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode")
				{
					nodePointValueList[nodeIndex].Value = FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode.Inactive;
				}
				else if (nodePointValueList[nodeIndex].ValueTypeString == "FMBusinessObjects.DataObjects.CodedVariables.NodeTransferMode")
				{
					nodePointValueList[nodeIndex].Value = FMBusinessObjects.DataObjects.CodedVariables.NodeTransferMode.Inactive;
				}

				nodePointValueList[nodeIndex].Status = StatusCodes.Good;
				nodePointValueList[nodeIndex].ServerTimeStamp = DateTimeOffset.UtcNow;
				nodePointValueList[nodeIndex].SourceTimeStamp = DateTimeOffset.UtcNow;
				nodeIndex++;
			}

			pointServiceManager.SetPointValueData(security, movementPointValueList, false);
			pointServiceManager.SetPointValueData(security, nodePointValueList, false);

			// Set status of interlocked movements to inactive in database and get list of those movements
			// to send a Stop command to.
			List<Guid> reenabledMovementGuids = this.ReenableInterlockedMovements(security, movementGuid);
			List<PointValueIdentifier> reenablePointValueIdentifierList = pointTags.EnumeratePointValueIdentifersByPointAndTagLists(security, reenabledMovementGuids, movementWellKnownTagGuidList.ToList());
			List<PointValue> reenablePointValueList = pointServiceManager.GetPointValueData(security, reenablePointValueIdentifierList, false);
			foreach (PointValue reenablePointValue in reenablePointValueList)
			{
				reenablePointValue.Value = MovementCommand.Stop;
				reenablePointValue.Status = StatusCodes.Good;
				reenablePointValue.ServerTimeStamp = DateTimeOffset.UtcNow;
				reenablePointValue.SourceTimeStamp = DateTimeOffset.UtcNow;
			}
			pointServiceManager.SetPointValueData(security, reenablePointValueList, false);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void InitiateMovementNode(SecurityClass security, Guid movementGuid, Guid nodeGuid)
		{
			security.ThrowIfNull("security");
			movementGuid.ThrowIfNull("movementGuid");
			nodeGuid.ThrowIfNull("nodeGuid");

			var nodeWellKnownTagGuidList = new Guid[] {
				Guids.TransferModeGuid,
				Guids.TransferTargetGuid
			};

			var movementStatus = this.GetTagValue(security, movementGuid, Guids.MovementStatusGuid);
			if (movementStatus != null
			&& movementStatus.Value is MovementStatus
			&& (MovementStatus) movementStatus.Value == MovementStatus.Inactive)
			{
				InitiateMovement(security, movementGuid);
			}

			var points = new Points();
			var movementPoint = points.Get(security, movementGuid);

			var movementSettingsProperty = movementPoint.Properties.Values.Single(u => u.ValueTypeString == "FMBusinessObjects.DataObjects.MovementModuleSettings");
			var movementSettings = movementSettingsProperty.Value as MovementModuleSettings;

			var pointGuidList = new List<Guid>();
			pointGuidList.Add(movementPoint.PointGuid);

			var pointTags = new PointTags();
			var movementPointValueIdentifierList = pointTags.EnumeratePointValueIdentifersByPointAndTagLists(security, pointGuidList, nodeWellKnownTagGuidList.ToList());

			pointGuidList.Clear();
			foreach (var movementNodeData in movementSettings.MovementNodeDataList)
			{
				// Nodes with IndividualNodeControl are Initiated by the User
				if (movementNodeData.MovementNodeGuid != nodeGuid)
				{
					continue;
				}

				pointGuidList.Add(movementNodeData.MovementNodeGuid);
			}

			var nodePointValueIdentifierList = pointTags.EnumeratePointValueIdentifersByPointAndTagLists(security, pointGuidList, nodeWellKnownTagGuidList.ToList());

			var pointServiceManager = new PointServiceManager();
			var nodePointValueList = pointServiceManager.GetPointValueData(security, nodePointValueIdentifierList, false);

			var nodeIndex = 0;
			foreach (var movementNodeData in movementSettings.MovementNodeDataList)
			{
				if (movementNodeData.MovementNodeGuid != nodeGuid)
				{
					continue;
				}

				if(movementNodeData.TransferMode == TransferModes.Batch)
				{
					if (nodePointValueList[nodeIndex].ValueTypeString == "FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode")
					{
						nodePointValueList[nodeIndex].Value = FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Batch;
					}
					else if (nodePointValueList[nodeIndex].ValueTypeString == "FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode")
					{
						nodePointValueList[nodeIndex].Value = FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode.Batch;
					}
					else if (nodePointValueList[nodeIndex].ValueTypeString == "FMBusinessObjects.DataObjects.CodedVariables.NodeTransferMode")
					{
						nodePointValueList[nodeIndex].Value = FMBusinessObjects.DataObjects.CodedVariables.NodeTransferMode.Batch;
					}
				}

				if (movementNodeData.TransferMode == TransferModes.Level)
				{
					if (nodePointValueList[nodeIndex].ValueTypeString == "FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode")
					{
						nodePointValueList[nodeIndex].Value = FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Level;
					}
				}

				nodePointValueList[nodeIndex].Status = StatusCodes.Good;
				nodePointValueList[nodeIndex].ServerTimeStamp = DateTimeOffset.UtcNow;
				nodePointValueList[nodeIndex].SourceTimeStamp = DateTimeOffset.UtcNow;
				nodePointValueList[nodeIndex + 1].Value = movementNodeData.TransferTarget;
				nodePointValueList[nodeIndex + 1].Status = StatusCodes.Good;
				nodePointValueList[nodeIndex + 1].ServerTimeStamp = DateTimeOffset.UtcNow;
				nodePointValueList[nodeIndex + 1].SourceTimeStamp = DateTimeOffset.UtcNow;

				break;
			}

			pointServiceManager.SetPointValueData(security, nodePointValueList.Where(x => x.PointValueIdentifier.IdentityGuid != Guid.Empty).ToList(), false);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void StopMovementNode(SecurityClass security, Guid nodeGuid)
		{
			security.ThrowIfNull("security");
			nodeGuid.ThrowIfNull("nodeGuid");

			var nodeWellKnownTagGuidList = new Guid[] {
				Guids.TransferModeGuid
			};


			var points = new Points();

			var pointGuidList = new List<Guid>();

			var pointTags = new PointTags();
			var movementPointValueIdentifierList = pointTags.EnumeratePointValueIdentifersByPointAndTagLists(security, pointGuidList, nodeWellKnownTagGuidList.ToList());

			pointGuidList.Add(nodeGuid);

			var nodePointValueIdentifierList = pointTags.EnumeratePointValueIdentifersByPointAndTagLists(security, pointGuidList, nodeWellKnownTagGuidList.ToList());

			var pointServiceManager = new PointServiceManager();
			var nodePointValueList = pointServiceManager.GetPointValueData(security, nodePointValueIdentifierList, false);

			if (nodePointValueList[0].ValueTypeString == "FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode")
			{
				nodePointValueList[0].Value = FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Inactive;
			}
			else if (nodePointValueList[0].ValueTypeString == "FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode")
			{
				nodePointValueList[0].Value = FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode.Inactive;
			}
			else if (nodePointValueList[0].ValueTypeString == "FMBusinessObjects.DataObjects.CodedVariables.NodeTransferMode")
			{
				nodePointValueList[0].Value = FMBusinessObjects.DataObjects.CodedVariables.NodeTransferMode.Inactive;
			}
			nodePointValueList[0].Status = StatusCodes.Good;
			nodePointValueList[0].ServerTimeStamp = DateTimeOffset.UtcNow;
			nodePointValueList[0].SourceTimeStamp = DateTimeOffset.UtcNow;

			pointServiceManager.SetPointValueData(security, nodePointValueList, false);
		}

		private PointValue GetTagValue(SecurityClass security, Guid movementGuid, Guid wellKnownTagGuid)
		{
			var wellKnownTagGuidList = new Guid[] {
					wellKnownTagGuid
				};

			var pointGuidList = new List<Guid>();
			pointGuidList.Add(movementGuid);

			var pointTags = new PointTags();
			var pointValueIdentifierList = pointTags.EnumeratePointValueIdentifersByPointAndTagLists(security, pointGuidList, wellKnownTagGuidList.ToList());

			var pointServiceManager = new PointServiceManager();
			var movementPointValueList = pointServiceManager.GetPointValueData(security, pointValueIdentifierList, false);

			return movementPointValueList[0];
		}

		private void CheckIsPointInSystemUse(SecurityClass security, Guid pointGuid)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				// Execute the stored procedure, passing in the list (table) of alarms and event log records
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "EXEC @Ret = [dbo].[udf_CheckIsPointInUseBySystem] @PointGuid";

				cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
				cmd.Parameters.AddWithValue("@Ret", 0);

				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		private List<Guid> DisableInterlockedMovements(SecurityClass security, Guid movementGuid)
		{
			SqlCommand cmd = new SqlCommand
			{
				CommandText = "dbo.usp_MovementDisableInterlockedMovements",
				CommandType = CommandType.StoredProcedure
			};

			cmd.Parameters.AddWithValue("@ActivatedMovementGuid", movementGuid);
			DataTable interlockedMovementsTable = this.ConsolidatedDa.GetDataTable(cmd, security);

			List<Guid> interlockedGuids = new List<Guid>();
			foreach(DataRow row in interlockedMovementsTable.Rows)
			{
				interlockedGuids.Add(DataObject.getValue(row["MovementGuid"], Guid.Empty));
			}

			return interlockedGuids;
		}

		private List<Guid> ReenableInterlockedMovements(SecurityClass security, Guid movementGuid)
		{
			SqlCommand cmd = new SqlCommand
			{
				CommandText = "dbo.usp_MovementReenableInterlockedMovements",
				CommandType = CommandType.StoredProcedure
			};

			cmd.Parameters.AddWithValue("@DeactivatedMovementGuid", movementGuid);
			DataTable interlockedMovementsTable = this.ConsolidatedDa.GetDataTable(cmd, security);

			List<Guid> interlockedGuids = new List<Guid>();
			foreach (DataRow row in interlockedMovementsTable.Rows)
			{
				interlockedGuids.Add(DataObject.getValue(row["MovementGuid"], Guid.Empty));
			}

			return interlockedGuids;
		}

		[OperationBehavior(TransactionScopeRequired = false, TransactionAutoComplete = true)]
		public List<string> CheckForActiveInterlockedMovements(SecurityClass security, Guid movementGuid)
		{
			SqlCommand cmd = new SqlCommand
			{
				CommandText = "dbo.usp_MovementCheckForActiveInterlockedMovements",
				CommandType = CommandType.StoredProcedure
			};

			cmd.Parameters.AddWithValue("@ActivatingMovementGuid", movementGuid);
			DataTable interlockedMovementsTable = this.ConsolidatedDa.GetDataTable(cmd, security);

			List<string> interlockedIds = new List<string>();
			foreach (DataRow row in interlockedMovementsTable.Rows)
			{
				interlockedIds.Add(DataObject.getValue(row["MovementID"], string.Empty));
			}

			return interlockedIds;
		}
	}
}