/* ========================================================================
 * Copyright © 2011-2014 Softing Industrial Automation GmbH. 
 * All rights reserved.
 * 
 * The Software is subject to the Softing Industrial Automation GmbH’s 
 * license agreement, which can be found here:
 * http://www.softing.com/LicenseSIA.pdf
 * 
 * The Software is based on the OPC Foundation, Inc.’s software. This 
 * original OPC Foundation’s software can be found here:
 * http://www.opcfoundation.org
 * 
 * The original OPC Foundation’s software is subject to the OPC Foundation
 * MIT License 1.00, which can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * 
 * ======================================================================*/


namespace FMUAAlarmServer
{

	using FMUAAlarmPluginInterface;
	using System;
	using System.Collections.Generic;
	using Softing.Opc.Ua.Sdk;
	using Softing.Opc.Ua.Sdk.Server;


	/// <summary>
	/// A node manager for a server that provides an implementation of the Alarms and Conditions OPC UA feature.
	/// </summary>
	public class AlarmsNodeManager : CustomNodeManager2
	{

		#region IDisposable Members
		/// <summary>
		/// An overrideable version of the Dispose.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				// TBD
			}
		}
		#endregion

		#region AddNodes Service

		/// <summary>
		/// Handle AddNodes service request
		/// </summary>
		/// <param name="requestHeader"></param>
		/// <param name="nodesToAdd"></param>
		/// <param name="results"></param>
		/// <param name="diagnosticInfos"></param>
		/// <returns></returns>
		public void AddNodes(
			 OperationContext context,
			 AddNodesItemCollection nodesToAdd,
			 out AddNodesResultCollection results,
			 out DiagnosticInfoCollection diagnosticInfos)
		{
			// validate nodesToAdd parameter.
			if (nodesToAdd == null)
			{
				throw new ServiceResultException(StatusCodes.BadInvalidArgument, "The nodesToAdd parameter is null.");
			}

			// create result lists.
			results = new AddNodesResultCollection(nodesToAdd.Count);
			diagnosticInfos = new DiagnosticInfoCollection(nodesToAdd.Count);

			Softing.Opc.Ua.Sdk.Trace.Instance.Log(TraceLevels.Information, TraceMasks.ServerSDK,
				  "Opc.Ua.Server.NodeManagementNodeManager.AddNodes", string.Format("NodeManagementNodeManager.AddNodes - Count={0}", nodesToAdd.Count));

			for (int ii = 0; ii < nodesToAdd.Count; ii++)
			{
				// call AddNode and update results
				AddNodesResult addResult;
				DiagnosticInfo diagnosticInfo;

				AddNode(
					 context,
					 nodesToAdd[ii],
					 out addResult,
					 out diagnosticInfo);

				results.Add(addResult);
				diagnosticInfos.Add(diagnosticInfo);
			}
		}

		// Validates the AddNode request and adds the node in the address space.
		private void AddNode(
			 OperationContext context,
			 AddNodesItem nodeToAdd,
			 out AddNodesResult result,
			 out DiagnosticInfo diagnosticInfo)
		{
			result = new AddNodesResult();
			diagnosticInfo = new DiagnosticInfo();

			try
			{
				// pre-validate the request.
				ServiceResult error = ValidateAddNodesItem(context, nodeToAdd);

				if (ServiceResult.IsBad(error))
				{
					result.StatusCode = error.Code;

					// add diagnostics if requested.
					if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
					{
						diagnosticInfo = new DiagnosticInfo(error, context.DiagnosticsMask, false, new StringTable());
					}

					return;
				}

				// perform the custom validation of the request
				error = ValidateAddNodeRequest(context, nodeToAdd);

				if (ServiceResult.IsBad(error))
				{
					result.StatusCode = error.Code;

					// add diagnostics if requested.
					if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
					{
						diagnosticInfo = new DiagnosticInfo(error, context.DiagnosticsMask, false, new StringTable());
					}
				}
				else
				{
					NodeState parentNode = null;

					if (!PredefinedNodes.TryGetValue(ExpandedNodeId.ToNodeId(nodeToAdd.ParentNodeId, null), out parentNode))
					{
						// ParentNodeId not found in address space;
						throw new ServiceResultException(StatusCodes.BadParentNodeIdInvalid, "The specified ParentNodeId not found in address space.");
					}

					switch (nodeToAdd.NodeClass)
					{
						case NodeClass.Object:
							// Create object node
							result.AddedNodeId = AddObject(nodeToAdd, parentNode);
							result.StatusCode = StatusCodes.Good;
							break;
						case NodeClass.Variable:
							// Create variable node
							result.AddedNodeId = AddVariable(nodeToAdd, parentNode);
							result.StatusCode = StatusCodes.Good;
							break;
						default:
							result.AddedNodeId = null;
							result.StatusCode = StatusCodes.BadNodeClassInvalid;
							break;
					}
				}
			}
			catch (Exception e)
			{
				// Handle exception                
				ServiceResult error = ServiceResult.Create(e, StatusCodes.BadUnexpectedError, e.Message);
				diagnosticInfo = new DiagnosticInfo(error, context.DiagnosticsMask, false, new StringTable());

				result.StatusCode = error.StatusCode;
				result.AddedNodeId = NodeId.Null;
			}
		}

		// Validates if an AddNodesItem request structure respects the specification of the AddNodes service.
		private ServiceResult ValidateAddNodesItem(OperationContext context, AddNodesItem nodeToAdd)
		{
			// check parentNodeId
			//if (nodeToAdd.ParentNodeId.IsNull)
			//{
			//    return new ServiceResult(StatusCodes.BadParentNodeIdInvalid, "The specified ParentNodeId is null.");
			//}

			//NodeId parentNodeId = ExpandedNodeId.ToNodeId(nodeToAdd.ParentNodeId, null);

			//if (parentNodeId.IsNullNodeId)
			//{
			//    return new ServiceResult(StatusCodes.BadParentNodeIdInvalid, "The specified ParentNodeId is null.");
			//}

			//NodeState parentNode = null;

			//if (!PredefinedNodes.TryGetValue(parentNodeId, out parentNode))
			//{
			//    // ParentNodeId not found in address space;
			//    return new ServiceResult(StatusCodes.BadParentNodeIdInvalid, "The specified ParentNodeId not found in address space.");
			//}

			//if (!nodeToAdd.RequestedNewNodeId.IsNull)
			//{
			//    NodeState existingNode = null;

			//    if (PredefinedNodes.TryGetValue(ExpandedNodeId.ToNodeId(nodeToAdd.RequestedNewNodeId, null), out existingNode))
			//    {
			//        // Requested NodeId already present in address space
			//        return new ServiceResult(StatusCodes.BadNodeIdExists, "The requested node id is already used by another node.");
			//    }
			//}

			//// check referenceTypeId                
			//IReferenceType referenceType = Server.CoreNodeManager.GetLocalNode(nodeToAdd.ReferenceTypeId) as IReferenceType;

			//if (referenceType == null)
			//{
			//    // referenceTypeId not found in address space
			//    return new ServiceResult(StatusCodes.BadReferenceTypeIdInvalid, "The specified referenceTypeId not found in address space.");
			//}

			//// check BrowseName
			//if (nodeToAdd.BrowseName == null)
			//{
			//    return new ServiceResult(StatusCodes.BadBrowseNameInvalid, "The specified BrowseName parameter is null");
			//}

			//// check NodeClass
			//if (nodeToAdd.NodeClass == NodeClass.Unspecified)
			//{
			//    return new ServiceResult(StatusCodes.BadNodeClassInvalid, "The NodeClass parameter is not specified");
			//}

			//// check NodeAttributes
			//if (ExtensionObject.IsNull(nodeToAdd.NodeAttributes) && (nodeToAdd.NodeClass == NodeClass.Object || nodeToAdd.NodeClass == NodeClass.Variable))
			//{
			//    return new ServiceResult(StatusCodes.BadNodeAttributesInvalid, "The specified NodeAttributes parameter is null");
			//}

			return ServiceResult.Good;
		}

		// Validates an AddNodesItem request.
		public virtual ServiceResult ValidateAddNodeRequest(OperationContext context, AddNodesItem nodeToAdd)
		{
			// return BadNotSupported
			// this method should be overriden in the derived class in order to allow clients to use use the AddNodes service
			//return new ServiceResult(StatusCodes.BadNotSupported, "Server does not allow nodes to be added by client.");
			return ServiceResult.Good;
		}

		// Creates an object node according to AddNodes operation request.
		private NodeId AddObject(AddNodesItem nodeToAdd, NodeState parentNode)
		{
			// check NodeAttributes
			ObjectAttributes attributes = nodeToAdd.NodeAttributes.Body as ObjectAttributes;

			if (attributes == null)
			{
				throw new ServiceResultException(StatusCodes.BadNodeAttributesInvalid, "The node Attributes are not valid for the node class.");
			}

			// check TypeDefinition
			if (nodeToAdd.TypeDefinition == null)
			{
				throw new ServiceResultException(StatusCodes.BadTypeDefinitionInvalid, "The TypeDefinition parameter is required for object nodes");
			}

			if (!Server.TypeTree.IsKnown(nodeToAdd.TypeDefinition))
			{
				throw new ServiceResultException(StatusCodes.BadTypeDefinitionInvalid, "The TypeDefinition parameter is not valid.");
			}

			// attempt to create the node according to specified TypeDefinition
			BaseObjectState objectToAdd = SystemContext.NodeStateFactory.CreateInstance(
				 SystemContext,
				 parentNode,
				 nodeToAdd.NodeClass,
				 nodeToAdd.BrowseName,
				 nodeToAdd.ReferenceTypeId,
				 ExpandedNodeId.ToNodeId(nodeToAdd.TypeDefinition, null)) as BaseObjectState;

			if (objectToAdd == null)
			{
				objectToAdd = new BaseObjectState(parentNode);
			}

			// create the object and assign the nodeId returned by NodeIdFactory.New() method
			objectToAdd.Create(SystemContext, ExpandedNodeId.ToNodeId(nodeToAdd.RequestedNewNodeId, null), nodeToAdd.BrowseName, null, true);

			// assign the requested NodeId if specified
			if (!nodeToAdd.RequestedNewNodeId.IsNull)
			{
				objectToAdd.NodeId = ExpandedNodeId.ToNodeId(nodeToAdd.RequestedNewNodeId, null);
			}

			// DisplayName
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.DisplayName) != 0)
			{
				objectToAdd.DisplayName = attributes.DisplayName;
			}

			// Description
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.Description) != 0)
			{
				objectToAdd.Description = attributes.Description;
			}

			// EventNotifier
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.EventNotifier) != 0)
			{
				objectToAdd.EventNotifier = attributes.EventNotifier;
			}

			// WriteMask
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.WriteMask) != 0)
			{
				objectToAdd.WriteMask = (AttributeWriteMask)attributes.WriteMask;
			}

			// UserWriteMask
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.UserWriteMask) != 0)
			{
				objectToAdd.UserWriteMask = (AttributeWriteMask)attributes.UserWriteMask;
			}

			objectToAdd.TypeDefinitionId = ExpandedNodeId.ToNodeId(nodeToAdd.TypeDefinition, null);
			objectToAdd.ReferenceTypeId = nodeToAdd.ReferenceTypeId;

			if (parentNode != null)
			{
				parentNode.AddChild(objectToAdd);

				parentNode.AddReference(nodeToAdd.ReferenceTypeId, false, objectToAdd.NodeId);
				objectToAdd.AddReference(nodeToAdd.ReferenceTypeId, true, parentNode.NodeId);
			}

			AddPredefinedNode(SystemContext, objectToAdd);

			return objectToAdd.NodeId;
		}

		public NodeId performObjectInstantiation(BaseObjectState objectToAdd, NodeState parentNode, AddNodesItem nodeToAdd, VariableAttributes attributes)
		{
			// assign the requested NodeId if specified
			if (!nodeToAdd.RequestedNewNodeId.IsNull)
			{
				objectToAdd.NodeId = ExpandedNodeId.ToNodeId(nodeToAdd.RequestedNewNodeId, null);
			}

			// DisplayName
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.DisplayName) != 0)
			{
				objectToAdd.DisplayName = attributes.DisplayName;
			}

			// Description
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.Description) != 0)
			{
				objectToAdd.Description = attributes.Description;
			}

			// WriteMask
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.WriteMask) != 0)
			{
				objectToAdd.WriteMask = (AttributeWriteMask)attributes.WriteMask;
			}

			// UserWriteMask
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.UserWriteMask) != 0)
			{
				objectToAdd.UserWriteMask = (AttributeWriteMask)attributes.UserWriteMask;
			}

			objectToAdd.TypeDefinitionId = ExpandedNodeId.ToNodeId(nodeToAdd.TypeDefinition, null);
			objectToAdd.ReferenceTypeId = nodeToAdd.ReferenceTypeId;

			if (parentNode != null)
			{
				parentNode.AddChild(objectToAdd);

				parentNode.AddReference(nodeToAdd.ReferenceTypeId, false, objectToAdd.NodeId);
				objectToAdd.AddReference(nodeToAdd.ReferenceTypeId, true, parentNode.NodeId);
			}

			AddPredefinedNode(SystemContext, objectToAdd);

			return objectToAdd.NodeId;
		}

		// Creates a variable node according to AddNodes operation request.
		private NodeId AddVariable(AddNodesItem nodeToAdd, NodeState parentNode)
		{
			// check NodeAttributes
			VariableAttributes attributes = nodeToAdd.NodeAttributes.Body as VariableAttributes;

			if (attributes == null)
			{
				throw new ServiceResultException(StatusCodes.BadNodeAttributesInvalid, "The node Attributes are not valid for the node class.");
			}

			// check TypeDefinition
			if (nodeToAdd.TypeDefinition == null)
			{
				throw new ServiceResultException(StatusCodes.BadTypeDefinitionInvalid, "The TypeDefinition parameter is required for variable nodes.");
			}

			if (!Server.TypeTree.IsKnown(nodeToAdd.TypeDefinition))
			{
				throw new ServiceResultException(StatusCodes.BadTypeDefinitionInvalid, "The TypeDefinition parameter is not valid.");
			}

			AddNodeRequestClass nodeRequest = AddNodeRequestClass.FromXML((string)attributes.Value.Value);

			object newOpcObject = mdef.Create(
				 nodeRequest.DynamicEntityType,
				 nodeRequest.InputParameters,
				 parentNode,
				 NamespaceIndex,
				 SystemContext,
				 PredefinedNodes);

			if (newOpcObject.GetType().IsSubclassOf(typeof(BaseDataVariableState)) == false)
			{
				return performObjectInstantiation((BaseObjectState)newOpcObject, parentNode, nodeToAdd, attributes);
			}

			BaseDataVariableState variableToAdd = (BaseDataVariableState)newOpcObject;

			//// attempt to create the node according to specified TypeDefinition
			//BaseDataVariableState variableToAdd = SystemContext.NodeStateFactory.CreateInstance(
			//    SystemContext,
			//    parentNode,
			//    nodeToAdd.NodeClass,
			//    nodeToAdd.BrowseName,
			//    nodeToAdd.ReferenceTypeId,
			//    ExpandedNodeId.ToNodeId(nodeToAdd.TypeDefinition, null)) as BaseDataVariableState;

			//if (variableToAdd == null)
			//{
			//    variableToAdd = new BaseDataVariableState(parentNode);
			//}

			//// create the variable and assign the nodeId returned by NodeIdFactory.New() method.
			//variableToAdd.Create(SystemContext, ExpandedNodeId.ToNodeId(nodeToAdd.RequestedNewNodeId, null), nodeToAdd.BrowseName, null, true);

			// assign the requested NodeId if specified
			if (!nodeToAdd.RequestedNewNodeId.IsNull)
			{
				variableToAdd.NodeId = ExpandedNodeId.ToNodeId(nodeToAdd.RequestedNewNodeId, null);
			}

			// DisplayName
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.DisplayName) != 0)
			{
				variableToAdd.DisplayName = attributes.DisplayName;
			}
			else
			{
				variableToAdd.DisplayName = nodeToAdd.BrowseName.Name;
			}

			// Description
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.Description) != 0)
			{
				variableToAdd.Description = attributes.Description;
			}

			//// Value
			//if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.Value) != 0)
			//{
			//    variableToAdd.Value = attributes.Value;
			//}

			// DataType
			//if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.DataType) != 0)
			//{
			//    variableToAdd.DataType = attributes.DataType;
			//}

			// ValueRank
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.ValueRank) != 0)
			{
				variableToAdd.ValueRank = attributes.ValueRank;
			}

			// ArrayDimensions
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.ArrayDimensions) != 0)
			{
				variableToAdd.ArrayDimensions = new ReadOnlyList<uint>(attributes.ArrayDimensions);
			}

			// AccessLevel
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.AccessLevel) != 0)
			{
				variableToAdd.AccessLevel = attributes.AccessLevel;
			}

			// UserAccessLevel
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.UserAccessLevel) != 0)
			{
				variableToAdd.UserAccessLevel = attributes.UserAccessLevel;
			}

			// MinimumSamplingInterval
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.MinimumSamplingInterval) != 0)
			{
				variableToAdd.MinimumSamplingInterval = attributes.MinimumSamplingInterval;
			}

			// Historizing
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.Historizing) != 0)
			{
				variableToAdd.Historizing = attributes.Historizing;
			}

			// WriteMask
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.WriteMask) != 0)
			{
				variableToAdd.WriteMask = (AttributeWriteMask)attributes.WriteMask;
			}

			// UserWriteMask
			if (attributes != null && (attributes.SpecifiedAttributes & (uint)NodeAttributesMask.UserWriteMask) != 0)
			{
				variableToAdd.UserWriteMask = (AttributeWriteMask)attributes.UserWriteMask;
			}

			variableToAdd.StatusCode = StatusCodes.Good;
			variableToAdd.Timestamp = DateTime.UtcNow;

			variableToAdd.TypeDefinitionId = ExpandedNodeId.ToNodeId(nodeToAdd.TypeDefinition, null);
			variableToAdd.ReferenceTypeId = nodeToAdd.ReferenceTypeId;

			if (parentNode != null)
			{
				parentNode.AddChild(variableToAdd);

				parentNode.AddReference(nodeToAdd.ReferenceTypeId, false, variableToAdd.NodeId);
				variableToAdd.AddReference(nodeToAdd.ReferenceTypeId, true, parentNode.NodeId);
			}

			AddPredefinedNode(SystemContext, variableToAdd);
			return variableToAdd.NodeId;
		}

		#endregion

		#region DeleteNodes Service

		/// <summary>
		/// Handle DeleteNodes service request
		/// </summary>
		/// <param name="context"></param>
		/// <param name="nodesToDelete"></param>
		/// <param name="results"></param>
		/// <param name="diagnosticInfos"></param>
		public void DeleteNodes(
			 OperationContext context,
			 DeleteNodesItemCollection nodesToDelete,
			 out StatusCodeCollection results,
			 out DiagnosticInfoCollection diagnosticInfos)
		{
			// validate nodesToDelete parameter.
			if (nodesToDelete == null)
			{
				throw new ServiceResultException(StatusCodes.BadInvalidArgument, "The nodesToDelete parameter is null.");
			}

			// create result lists.
			results = new StatusCodeCollection(nodesToDelete.Count);
			diagnosticInfos = new DiagnosticInfoCollection(nodesToDelete.Count);

			Softing.Opc.Ua.Sdk.Trace.Instance.Log(TraceLevels.Information, TraceMasks.ServerSDK,
				  "Opc.Ua.Server.NodeManagementNodeManager.DeleteNodes", string.Format("NodeManagementNodeManager.DeleteNodes - Count={0}", nodesToDelete.Count));

			for (int ii = 0; ii < nodesToDelete.Count; ii++)
			{
				// call DeleteNode and update results
				StatusCode deleteResult;
				DiagnosticInfo diagnosticInfo;

				DeleteNode(
					 context,
					 nodesToDelete[ii],
					 out deleteResult,
					 out diagnosticInfo);

				results.Add(deleteResult);
				diagnosticInfos.Add(diagnosticInfo);
			}
		}

		// Validates the DeleteNode request and deletes the node from address space.
		private void DeleteNode(
			 OperationContext context,
			 DeleteNodesItem nodeToDelete,
			 out StatusCode result,
			 out DiagnosticInfo diagnosticInfo)
		{
			result = new StatusCode();
			diagnosticInfo = new DiagnosticInfo();

			try
			{
				// pre-validate the request.
				ServiceResult error = ValidateDeleteNodesItem(context, nodeToDelete);

				if (ServiceResult.IsBad(error))
				{
					result = error.Code;

					// add diagnostics if requested.
					if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
					{
						diagnosticInfo = new DiagnosticInfo(error, context.DiagnosticsMask, false, new StringTable());
					}

					return;
				}

				// perform the custom validation of the request
				error = ValidateDeleteNodesRequest(context, nodeToDelete);

				if (ServiceResult.IsBad(error))
				{
					result = error.Code;

					// add diagnostics if requested.
					if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
					{
						diagnosticInfo = new DiagnosticInfo(error, context.DiagnosticsMask, false, new StringTable());
					}
				}
				else
				{
					NodeState node = null;

					if (!PredefinedNodes.TryGetValue(nodeToDelete.NodeId, out node))
					{
						// NodeId not found in address space
						throw new ServiceResultException(StatusCodes.BadNodeIdInvalid, "The specified NodeId was not found in address space.");
					}

					List<LocalReference> referencesToRemove = new List<LocalReference>();

					// remove the specified node from address space
					lock (Lock)
					{
						// remove from predefined nodes
						PredefinedNodes.Remove(node.NodeId);

						mdef.Delete(node, SystemContext);
						OnNodeRemoved(node);

					}

					// must release the lock before removing cross references to other node managers.
					if (referencesToRemove.Count > 0)
					{
						Server.NodeManager.RemoveReferences(referencesToRemove);
					}

					result = StatusCodes.Good;
				}
			}
			catch (Exception e)
			{
				// Handle exception                
				ServiceResult error = ServiceResult.Create(e, StatusCodes.BadUnexpectedError, e.Message);
				diagnosticInfo = new DiagnosticInfo(error, context.DiagnosticsMask, false, new StringTable());

				result = error.StatusCode;
			}
		}

		// Validates if a DeleteNodesItem request structure respects the specification of the DeleteNodesItem service.
		private ServiceResult ValidateDeleteNodesItem(OperationContext context, DeleteNodesItem nodeToDelete)
		{
			// check NodeId
			if (nodeToDelete.NodeId.IsNullNodeId)
			{
				return new ServiceResult(StatusCodes.BadNodeIdInvalid, "The specified NodeId is null.");
			}

			NodeState node = null;

			if (!PredefinedNodes.TryGetValue(nodeToDelete.NodeId, out node))
			{
				// NodeId not found in address space
				return new ServiceResult(StatusCodes.BadNodeIdInvalid, "The specified NodeId was not found in address space.");
			}

			return ServiceResult.Good;
		}

		// Validates an DeleteNodesItem request.
		public virtual ServiceResult ValidateDeleteNodesRequest(OperationContext context, DeleteNodesItem nodeToDelete)
		{
			return ServiceResult.Good;
		}

		/// <summary>
		/// Called after a node has been deleted.
		/// </summary>
		protected override void OnNodeRemoved(NodeState node)
		{
			base.OnNodeRemoved(node);

			// When a deleted node is being monitored, then a Notification containing the status code Bad_NodeIdUnknown
			// should be sent to the monitoring Client indicating that the Node has been deleted.

			if (node.NodeClass == NodeClass.Variable)
			{
				BaseDataVariableState variableNode = node as BaseDataVariableState;

				if (variableNode != null)
				{
					variableNode.Value = Variant.Null;
					variableNode.Timestamp = DateTime.Now;
					variableNode.StatusCode = StatusCodes.BadNodeIdUnknown;

					// the call back pushes the updated values into the monitored items.
					variableNode.ClearChangeMasks(SystemContext, true);
				}
			}
		}

		#endregion

		#region AddReferences Service

		/// <summary>
		/// Handle AddReferences service request
		/// </summary>
		/// <param name="context"></param>
		/// <param name="referencesToAdd"></param>
		/// <param name="results"></param>
		/// <param name="diagnosticInfos"></param>
		public void AddReferences(
			 OperationContext context,
			 AddReferencesItemCollection referencesToAdd,
			 out StatusCodeCollection results,
			 out DiagnosticInfoCollection diagnosticInfos)
		{
			// validate referencesToAdd parameter.
			if (referencesToAdd == null)
			{
				throw new ServiceResultException(StatusCodes.BadInvalidArgument, "The referencesToAdd parameter is null.");
			}

			// create result lists.
			results = new StatusCodeCollection(referencesToAdd.Count);
			diagnosticInfos = new DiagnosticInfoCollection(referencesToAdd.Count);

			Softing.Opc.Ua.Sdk.Trace.Instance.Log(TraceLevels.Information, TraceMasks.ServerSDK,
				  "Opc.Ua.Server.NodeManagementNodeManager.AddReferences", string.Format("NodeManagementNodeManager.AddReferences - Count={0}", referencesToAdd.Count));

			for (int ii = 0; ii < referencesToAdd.Count; ii++)
			{
				// call AddReference and update results
				StatusCode addResult;
				DiagnosticInfo diagnosticInfo;

				AddReference(
					 context,
					 referencesToAdd[ii],
					 out addResult,
					 out diagnosticInfo);

				results.Add(addResult);
				diagnosticInfos.Add(diagnosticInfo);
			}
		}

		// Validates the AddReference request and adds the requested reference .
		private void AddReference(
			 OperationContext context,
			 AddReferencesItem referenceToAdd,
			 out StatusCode result,
			 out DiagnosticInfo diagnosticInfo)
		{
			result = new StatusCode();
			diagnosticInfo = new DiagnosticInfo();

			try
			{
				// pre-validate the request.
				ServiceResult error = ValidateAddReferencesItem(context, referenceToAdd);

				if (ServiceResult.IsBad(error))
				{
					result = error.Code;

					// add diagnostics if requested.
					if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
					{
						diagnosticInfo = new DiagnosticInfo(error, context.DiagnosticsMask, false, new StringTable());
					}

					return;
				}

				// perform the custom validation of the request
				error = ValidateAddReferencesRequest(context, referenceToAdd);

				if (ServiceResult.IsBad(error))
				{
					result = error.Code;

					// add diagnostics if requested.
					if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
					{
						diagnosticInfo = new DiagnosticInfo(error, context.DiagnosticsMask, false, new StringTable());
					}
				}
				else
				{
					NodeState node = null;

					if (!PredefinedNodes.TryGetValue(referenceToAdd.SourceNodeId, out node))
					{
						// sourceNodeId not found in address space
						throw new ServiceResultException(StatusCodes.BadSourceNodeIdInvalid, "The specified SourceNodeId was not found in address space.");
					}

					// add the reference
					node.AddReference(referenceToAdd.ReferenceTypeId, !referenceToAdd.IsForward, referenceToAdd.TargetNodeId);
					node.ClearChangeMasks(SystemContext, false);

					result = StatusCodes.Good;
				}
			}
			catch (Exception e)
			{
				// Handle exception                
				ServiceResult error = ServiceResult.Create(e, StatusCodes.BadUnexpectedError, e.Message);
				diagnosticInfo = new DiagnosticInfo(error, context.DiagnosticsMask, false, new StringTable());

				result = error.StatusCode;
			}
		}

		// Validates if an AddReferencesItem request structure respects the specification of the AddReferences service.
		private ServiceResult ValidateAddReferencesItem(OperationContext context, AddReferencesItem referenceToAdd)
		{
			// check sourceNodeId
			if (referenceToAdd.SourceNodeId.IsNullNodeId)
			{
				return new ServiceResult(StatusCodes.BadSourceNodeIdInvalid, "The specified SourceNodeId is null.");
			}

			NodeState node = null;

			if (!PredefinedNodes.TryGetValue(referenceToAdd.SourceNodeId, out node))
			{
				// sourceNodeId not found in address space
				return new ServiceResult(StatusCodes.BadSourceNodeIdInvalid, "The specified SourceNodeId was not found in address space.");
			}

			// check referenceTypeId
			IReferenceType referenceType = Server.CoreNodeManager.GetLocalNode(referenceToAdd.ReferenceTypeId) as IReferenceType;

			if (referenceType == null)
			{
				// referenceTypeId not found in address space
				return new ServiceResult(StatusCodes.BadReferenceTypeIdInvalid, "The specified referenceTypeId not found in address space.");
			}

			// check targetNodeId
			NodeState targetNode = null;

			if (!PredefinedNodes.TryGetValue(ExpandedNodeId.ToNodeId(referenceToAdd.TargetNodeId, null), out targetNode))
			{
				// targetNodeId not found in address space;
				return new ServiceResult(StatusCodes.BadTargetNodeIdInvalid, "The specified TargetNodeId not found in address space.");
			}

			// check if TargetNodeId is from a remote server
			if (referenceToAdd.TargetNodeId.ServerIndex != 0)
			{
				// do not allow references to a remote Server
				return new ServiceResult(StatusCodes.BadReferenceLocalOnly, "References to remote servers not allowed.");
			}

			// check NodeClass
			// The TargetNodeClass is an input parameter that is used to validate that
			// the Reference to be added matches the NodeClass of the TargetNode.
			if (referenceToAdd.TargetNodeClass != targetNode.NodeClass)
			{
				// NodeClass of the targetNode does not match the specified TargetNodeClass
				return new ServiceResult(StatusCodes.BadNodeClassInvalid, "The specified TargetNodeClass does not match the NodeClass of TargetNodeId.");
			}

			// retreive node references.
			List<IReference> references = new List<IReference>();
			node.GetReferences(SystemContext, references);

			for (int ii = 0; ii < references.Count; ii++)
			{
				IReference reference = references[ii];

				// check if the reference already exists
				if (reference.ReferenceTypeId == referenceToAdd.ReferenceTypeId && reference.TargetId == referenceToAdd.TargetNodeId)
				{
					// the requested reference already exists
					return new ServiceResult(StatusCodes.BadDuplicateReferenceNotAllowed, "The specified reference already exists.");
				}
			}

			return ServiceResult.Good;
		}

		// Validates an AddReferencesItem request.
		public virtual ServiceResult ValidateAddReferencesRequest(OperationContext context, AddReferencesItem referenceToAdd)
		{
			// return BadNotSupported
			// this method should be overriden in the derived class in order to allow clients to use use the AddReferences service
			return new ServiceResult(StatusCodes.BadNotSupported, "Server does not allow references to be added by client.");
		}
		#endregion

		#region DeleteReferences Service

		/// <summary>
		/// Handle DeleteReferences service request
		/// </summary>
		/// <param name="context"></param>
		/// <param name="referencesToDelete"></param>
		/// <param name="results"></param>
		/// <param name="diagnosticInfos"></param>
		public void DeleteReferences(
			 OperationContext context,
			 DeleteReferencesItemCollection referencesToDelete,
			 out StatusCodeCollection results,
			 out DiagnosticInfoCollection diagnosticInfos)
		{
			// validate referencesToDelete parameter.
			if (referencesToDelete == null)
			{
				throw new ServiceResultException(StatusCodes.BadInvalidArgument, "The referencesToDelete parameter is null.");
			}

			// create result lists.
			results = new StatusCodeCollection(referencesToDelete.Count);
			diagnosticInfos = new DiagnosticInfoCollection(referencesToDelete.Count);

			Softing.Opc.Ua.Sdk.Trace.Instance.Log(TraceLevels.Information, TraceMasks.ServerSDK,
				  "Opc.Ua.Server.NodeManagementNodeManager.DeleteReferences", string.Format("NodeManagementNodeManager.DeleteReferences - Count={0}", referencesToDelete.Count));

			for (int ii = 0; ii < referencesToDelete.Count; ii++)
			{
				// call DeleteReference and update results
				StatusCode addResult;
				DiagnosticInfo diagnosticInfo;

				DeleteReference(
					 context,
					 referencesToDelete[ii],
					 out addResult,
					 out diagnosticInfo);

				results.Add(addResult);
				diagnosticInfos.Add(diagnosticInfo);
			}
		}

		// Validates the DeleteReference request and deletes the requested reference .
		private void DeleteReference(
			 OperationContext context,
			 DeleteReferencesItem referenceToDelete,
			 out StatusCode result,
			 out DiagnosticInfo diagnosticInfo)
		{
			result = new StatusCode();
			diagnosticInfo = new DiagnosticInfo();

			try
			{
				// pre-validate the request.
				ServiceResult error = ValidateDeleteReferencesItem(context, referenceToDelete);

				if (ServiceResult.IsBad(error))
				{
					result = error.Code;

					// add diagnostics if requested.
					if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
					{
						diagnosticInfo = new DiagnosticInfo(error, context.DiagnosticsMask, false, new StringTable());
					}

					return;
				}

				// perform the custom validation of the request
				error = ValidateDeleteReferencesRequest(context, referenceToDelete);

				if (ServiceResult.IsBad(error))
				{
					result = error.Code;

					// add diagnostics if requested.
					if ((context.DiagnosticsMask & DiagnosticsMasks.OperationAll) != 0)
					{
						diagnosticInfo = new DiagnosticInfo(error, context.DiagnosticsMask, false, new StringTable());
					}
				}
				else
				{
					NodeState node = null;

					if (!PredefinedNodes.TryGetValue(referenceToDelete.SourceNodeId, out node))
					{
						// sourceNodeId not found in address space
						throw new ServiceResultException(StatusCodes.BadSourceNodeIdInvalid, "The specified SourceNodeId was not found in address space.");
					}

					// remove the specified reference.
					node.RemoveReference(referenceToDelete.ReferenceTypeId, !referenceToDelete.IsForward, referenceToDelete.TargetNodeId);

					if (referenceToDelete.DeleteBidirectional && referenceToDelete.TargetNodeId.ServerIndex == 0)
					{
						// delete also the oposite reference if required
						node.RemoveReference(referenceToDelete.ReferenceTypeId, referenceToDelete.IsForward, referenceToDelete.TargetNodeId);
					}

					result = StatusCodes.Good;
				}
			}
			catch (Exception e)
			{
				// Handle exception                
				ServiceResult error = ServiceResult.Create(e, StatusCodes.BadUnexpectedError, e.Message);
				diagnosticInfo = new DiagnosticInfo(error, context.DiagnosticsMask, false, new StringTable());

				result = error.StatusCode;
			}
		}

		// Validates if a DeleteReferencesItem request structure respects the specification of the DeleteReferences service.
		private ServiceResult ValidateDeleteReferencesItem(OperationContext context, DeleteReferencesItem referenceToDelete)
		{
			// check sourceNodeId
			if (referenceToDelete.SourceNodeId.IsNullNodeId)
			{
				return new ServiceResult(StatusCodes.BadSourceNodeIdInvalid, "The specified SourceNodeId is null.");
			}

			NodeState node = null;

			if (!PredefinedNodes.TryGetValue(referenceToDelete.SourceNodeId, out node))
			{
				// sourceNodeId not found in address space
				return new ServiceResult(StatusCodes.BadSourceNodeIdInvalid, "The specified SourceNodeId was not found in address space.");
			}

			// check referenceTypeId
			IReferenceType referenceType = Server.CoreNodeManager.GetLocalNode(referenceToDelete.ReferenceTypeId) as IReferenceType;

			if (referenceType == null)
			{
				// referenceTypeId not found in address space
				return new ServiceResult(StatusCodes.BadReferenceTypeIdInvalid, "The specified referenceTypeId not found in address space.");
			}

			// check targetNodeId
			NodeState targetNode = null;

			if (!PredefinedNodes.TryGetValue(ExpandedNodeId.ToNodeId(referenceToDelete.TargetNodeId, null), out targetNode))
			{
				// targetNodeId not found in address space;
				return new ServiceResult(StatusCodes.BadTargetNodeIdInvalid, "The specified TargetNodeId not found in address space.");
			}

			// check if TargetNodeId is from a remote server
			if (referenceToDelete.TargetNodeId.ServerIndex != 0)
			{
				// do not allow to delete references to a remote Server
				return new ServiceResult(StatusCodes.BadTargetNodeIdInvalid, "References to remote servers not allowed.");
			}

			// check if the reference exists
			if (!node.ReferenceExists(referenceToDelete.ReferenceTypeId, !referenceToDelete.IsForward, referenceToDelete.TargetNodeId))
			{
				// the specified reference does not exist.
				return new ServiceResult(StatusCodes.BadNoEntryExists, "The specified reference does not exist.");
			}

			return ServiceResult.Good;
		}

		// Validates a DeleteReferencesItem request.
		public virtual ServiceResult ValidateDeleteReferencesRequest(OperationContext context, DeleteReferencesItem referenceToDelete)
		{
			// return BadNotSupported
			// this method should be overriden in the derived class in order to allow clients to use use the AddReferences service
			return new ServiceResult(StatusCodes.BadNotSupported, "Server does not allow references to be deleted by client.");
		}


		#endregion

		private BaseObjectState machine;

		private BaseObjectState root;

		private MasterDynaicEntityFactory mdef;

		#region Constructors
		/// <summary>
		/// Initializes the node manager.
		/// </summary>
		public AlarmsNodeManager(IServerInternal server, ApplicationConfiguration configuration, params string[] namespaceUris)
			: base(server, configuration, Namespaces.Alarms)
		{
			SystemContext.NodeIdFactory = this;

			// get the configuration for the node manager.
			m_configuration = configuration.ParseExtension<AlarmsServerConfiguration>();

			// use suitable defaults if no configuration exists.
			if (m_configuration == null)
			{
				m_configuration = new AlarmsServerConfiguration();
			}
			string dllDir = Environment.CurrentDirectory;
			mdef = new MasterDynaicEntityFactory(dllDir);

		}
		#endregion

		#region INodeIdFactory Members
		/// <summary>
		/// Creates the NodeId for the specified node.
		/// </summary>
		public override NodeId New(ISystemContext context, NodeState node)
		{
			return GenerateNodeId();
		}
		#endregion

		#region INodeManager Members
		/// <summary>
		/// Does any initialization required before the address space can be used.
		/// </summary>
		/// <remarks>
		/// The externalReferences is an out parameter that allows the node manager to link to nodes
		/// in other node managers. For example, the 'Objects' node is managed by the CoreNodeManager and
		/// should have a reference to the root folder node(s) exposed by this node manager.  
		/// </remarks>
		public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
		{
			lock (Lock)
			{
				// Create the root of the node manager in the AddressSpace
				root = new BaseObjectState(null);

				// Set root object data 
				root.NodeId = GenerateNodeId();
				root.BrowseName = new QualifiedName("Alarms Module", NamespaceIndex);
				root.DisplayName = root.BrowseName.Name;
				root.Description = "Alarms Module";
				root.EventNotifier = EventNotifiers.SubscribeToEvents;
				root.TypeDefinitionId = ObjectTypeIds.BaseObjectType;

				// ensure the process object can be found via the server object. 
				IList<IReference> references = null;

				if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
				{
					externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
				}

				root.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
				references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, root.NodeId));

				// Add a folder representing the monitored device.
				machine = AddObject(root, "Machine A");

				BaseObjectState points = AddObject(root, "Points");

				// Add Support for Event Notifiers

				// creating notifier ensures events propogate up the hierarchy when they are produced.
				AddRootNotifier(root);

				// add link to server object.
				if (!externalReferences.TryGetValue(ObjectIds.Server, out references))
				{
					externalReferences[ObjectIds.Server] = references = new List<IReference>();
				}
				references.Add(new NodeStateReference(ReferenceTypeIds.HasNotifier, false, root.NodeId));

				// add sub-notifiers.
				root.AddNotifier(SystemContext, ReferenceTypeIds.HasNotifier, false, machine);
				machine.AddNotifier(SystemContext, ReferenceTypeIds.HasNotifier, true, root);

				// save the node for later lookup (all tightly coupled children are added with this call).
				AddPredefinedNode(SystemContext, root);
			}
		}

		/// <summary>
		/// Creates a new object node and adds it to the specified parent
		/// </summary>
		private BaseObjectState AddObject(NodeState parent, string name)
		{
			BaseObjectState objectNode = new BaseObjectState(parent);

			objectNode.NodeId = GenerateNodeId();
			objectNode.BrowseName = new QualifiedName(name, NamespaceIndex);
			objectNode.DisplayName = objectNode.BrowseName.Name;
			objectNode.Description = String.Empty;
			objectNode.EventNotifier = EventNotifiers.SubscribeToEvents;

			objectNode.ReferenceTypeId = ReferenceTypes.Organizes;
			objectNode.TypeDefinitionId = ObjectTypeIds.BaseObjectType;

			if (parent != null)
			{
				parent.AddChild(objectNode);
			}

			return objectNode;
		}

		/// <summary>
		/// Frees any resources allocated for the address space.
		/// </summary>
		public override void DeleteAddressSpace()
		{
			lock (Lock)
			{
				// TBD
			}
		}

		/// <summary>
		/// Returns a unique handle for the node.
		/// </summary>
		protected override NodeHandle GetManagerHandle(ServerSystemContext context, NodeId nodeId, IDictionary<NodeId, NodeState> cache)
		{
			lock (Lock)
			{
				// quickly exclude nodes that are not in the namespace. 
				if (!IsNodeIdInNamespace(nodeId))
				{
					return null;
				}

				NodeState node = null;

				if (PredefinedNodes != null && !PredefinedNodes.TryGetValue(nodeId, out node))
				{
					return null;
				}

				NodeHandle handle = new NodeHandle();

				handle.NodeId = nodeId;
				handle.Node = node;
				handle.Validated = true;

				return handle;
			}
		}

		/// <summary>
		/// Verifies that the specified node exists.
		/// </summary>
		protected override NodeState ValidateNode(
			 ServerSystemContext context,
			 NodeHandle handle,
			 IDictionary<NodeId, NodeState> cache)
		{
			// not valid if no root.
			if (handle == null)
			{
				return null;
			}

			// check if previously validated.
			if (handle.Validated)
			{
				return handle.Node;
			}

			// TBD

			return null;
		}

		private NodeId GenerateNodeId()
		{
			return new NodeId(++m_nextNodeId, NamespaceIndex);
		}

		#endregion

		#region Private Fields
		private AlarmsServerConfiguration m_configuration;
		private uint m_nextNodeId = 0;
		#endregion
	}
}
