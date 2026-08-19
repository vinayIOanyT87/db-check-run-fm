// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAccessNodeManager.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FMOpcUaServerService
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMOpcUaServerService.InternalClasses;
	using FMOpcUaServerService.InternalInterfaces;
	using FMPointCommon;
	using Opc.Ua;
	using Opc.Ua.Server;
	using Namespaces = FMOpcUaServerService.InternalClasses.Namespaces;

	public class OpcUaServerNodeManager : CustomNodeManager2
	{
		private SecurityClass security;
		private FolderState rootFolder;
		public Dictionary<Guid, SiteClass> SiteDictionary = new Dictionary<Guid, SiteClass>();
		public Dictionary<Guid, object> SiteHierarchy = new Dictionary<Guid, object>();
		public Dictionary<Guid, Dictionary<Guid, Point>> SitePointDictionary = new Dictionary<Guid, Dictionary<Guid, Point>>();
		public Dictionary<Guid, Point> PointDictionary = new Dictionary<Guid, Point>();
		public Dictionary<Guid, PointValue> PointValueDictionary = new Dictionary<Guid, PointValue>();
		public Dictionary<Guid, long> SitePointRowVersion = new Dictionary<Guid, long>();
		public long? MaxSiteRowVersion = null;
		public long? MaxSiteToSiteMapRowVersion = null;
		private readonly IOpcUaHistoricalData historicalData = new OpcUaHistoricalData();

		/// <summary>
		/// Initializes the node manager.
		/// </summary>
		public OpcUaServerNodeManager(IServerInternal server, ApplicationConfiguration configuration, SecurityClass security)
			: base(server, configuration, Namespaces.DataAccess)
		{
			base.SystemContext.NodeIdFactory = this;
			this.security = security;
		}


		public void Update()
		{
			int maxValuesPerRead = 100;
			NodeId[] nodeIds;

			lock (this.Lock)
			{
				nodeIds = MonitoredNodes.Keys.ToArray();
			}

			var pointValueIdentifierNodeIdDictionary = new Dictionary<PointValueIdentifier, NodeId>(maxValuesPerRead);
			int nodeIdsProcessed = 0;
			int nodeIdIndexStart = 0;

			while (nodeIdsProcessed < nodeIds.Length)
			{
				var nodeId = nodeIds[nodeIdsProcessed];
				string identifier = (string)nodeId.Identifier;

				MonitoredNode2 monitoredNode;

				lock (this.Lock)
				{
					if (MonitoredNodes.TryGetValue(nodeId, out monitoredNode)
					&& monitoredNode.Node is DataItemState)
					{

						var node = monitoredNode.Node as DataItemState;


						if (PointManager.IsTagNodeID(identifier))
						{
							Guid guid;
							PointManager.ParseTagNodeID(identifier, out guid);
							var pointValueIdentifier = new PointValueIdentifier(guid, PointValueType.Tag, string.Empty, new DateTimeOffset(node.Timestamp));
							pointValueIdentifierNodeIdDictionary.Add(pointValueIdentifier, nodeId);
						}

						else if (PointManager.IsPointNodeID(identifier))
						{
							Guid pointGuid;
							string propertyID = null;

							PointManager.ParsePointNodeID(identifier, out pointGuid, out propertyID);

							// Point Node may have null propertyID if it is the Point Folder so set to PointID
							if (string.IsNullOrEmpty(propertyID))
							{
								propertyID = "PointId";
							}

							var pointValueIdentifier = new PointValueIdentifier(pointGuid, PointValueType.Point, propertyID, new DateTimeOffset(node.Timestamp));
							pointValueIdentifierNodeIdDictionary.Add(pointValueIdentifier, nodeId);
						}

						else if (PointManager.IsSettingNodeID(identifier))
						{
							Guid pointGuid, guid;
							string propertyID;
							PointManager.ParseSettingNodeID(identifier, out pointGuid, out guid, out propertyID);
							var pointValueIdentifier = new PointValueIdentifier(guid, PointValueType.Setting, propertyID, new DateTimeOffset(node.Timestamp));
							pointValueIdentifierNodeIdDictionary.Add(pointValueIdentifier, nodeId);
						}

						else if (PointManager.IsAlarmSourceTagNodeID(identifier))
						{
							Guid pointGuid, guid;
							PointManager.ParseAlarmSourceTagNodeID(identifier, out pointGuid, out guid);
							var pointValueIdentifier = new PointValueIdentifier(guid, PointValueType.Tag, string.Empty, new DateTimeOffset(node.Timestamp));

							// Presently Acknowledged NodeId's are associated with the AlarmSouurceTag
							if (pointValueIdentifierNodeIdDictionary.ContainsKey(pointValueIdentifier))
							{
								pointValueIdentifierNodeIdDictionary[pointValueIdentifier] = nodeId;
							}
							else
							{
								pointValueIdentifierNodeIdDictionary.Add(pointValueIdentifier, nodeId);
							}
						}

						// Presently not supporting subscription to Acknowledged Nodes as these must be reworked to reference tblPointTagAlarmStatus
//						else if (PointManager.IsAcknowledgedNodeID(identifier))
//						{
//							Guid pointGuid, tagGuid, alarmGuid;
//							PointManager.ParseAcknowledgedNodeID(identifier, out pointGuid, out tagGuid, out alarmGuid);
//							var pointValueIdentifier = new PointValueIdentifier(tagGuid, PointValueType.Tag, string.Empty, new DateTimeOffset(node.Timestamp));
//							pointValueIdentifierNodeIdDictionary.Add(pointValueIdentifier, nodeId);
//						}

						else if (PointManager.IsSiteNodeID(identifier))
						{
							if (MonitoredNodes.TryGetValue(nodeId, out monitoredNode))
							{

								Guid siteGuid;
								PointManager.ParseSiteNodeID(identifier, out siteGuid);

								var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetBasic(security, siteGuid));

								node = monitoredNode.Node as DataItemState;
								if (node != null
								&& ((node.Value != null && site == null)
								|| (node.Value == null && site != null)
								|| (node.Value != null && !node.Value.Equals(site.ID))
								|| node.StatusCode != ((site == null) ? StatusCodes.Bad : StatusCodes.Good)))
								{
									node.Value = (site == null) ? null : site.ID;
									node.StatusCode = ((site == null) ? StatusCodes.Bad : StatusCodes.Good);
									node.Timestamp = new DateTime(DateTimeOffset.UtcNow.Ticks, DateTimeKind.Utc);
									node.ClearChangeMasks(SystemContext, false);
								}
							}
						}
					}
				}


				nodeIdsProcessed++;

				if (nodeIdsProcessed >= nodeIds.Length
				|| pointValueIdentifierNodeIdDictionary.Count >= maxValuesPerRead)
				{

					var pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueDataChanges(this.security, pointValueIdentifierNodeIdDictionary.Keys.ToList(), false));

					lock (this.Lock)
					{
						foreach(var pointValue in pointValues)
						{
							if (pointValueIdentifierNodeIdDictionary.TryGetValue(pointValue.PointValueIdentifier, out nodeId))
							{
								identifier = (string)nodeId.Identifier;

								if (PointManager.IsTagNodeID(identifier)
								|| PointManager.IsPointNodeID(identifier)
								|| PointManager.IsSettingNodeID(identifier)
								|| PointManager.IsAlarmSourceTagNodeID(identifier))
								{
									if (MonitoredNodes.TryGetValue(nodeId, out monitoredNode))
									{
										object value;

										if (pointValue.Value is DateTimeOffset)
										{
											value = ((DateTimeOffset)pointValue.Value).UtcDateTime;
										}
										else if (pointValue.Value is TimeSpan)
										{
											value = ((TimeSpan)pointValue.Value).Ticks;
										}
										else if (pointValue.Value is PointCommandStatusListReference)
										{
											var pcslr = pointValue.Value as PointCommandStatusListReference;
											if (pcslr.CurrentValue.HasValue)
											{
												value = pcslr.CurrentValue.Value;
											}
											else
											{
												value = null;
											}
										}
										else if (pointValue.Value is DeviceAlarmMapReference)
										{
											var damr = pointValue.Value as DeviceAlarmMapReference;
											if (damr.CurrentValue.HasValue)
											{
												value = (ushort)damr.CurrentValue.Value;
											}
											else
											{
												value = null;
											}
										}
										else
										{
											value = pointValue.Value;
										}


										var node = monitoredNode.Node as DataItemState;
										if (node != null
										&& ((node.Value == null && value != null)
										|| (node.Value != null && value == null)
										|| (node.Value != null && !node.Value.Equals(value))
										|| node.StatusCode != new StatusCode((uint)pointValue.Status)
										|| node.Timestamp != new DateTime(pointValue.ServerTimeStamp.Ticks, DateTimeKind.Utc)))
										{
											node.Value = value;
											node.StatusCode = new StatusCode((uint)pointValue.Status);
											node.Timestamp = new DateTime(pointValue.ServerTimeStamp.Ticks, DateTimeKind.Utc);
											node.ClearChangeMasks(SystemContext, false);
										}
									}
								}

								// Presently not supporting subscription to Acknowledged Nodes as these must be reworked to reference tblPointTagAlarmStatus
								// Do not need to comment this out as Acknowledged NodeId will never be in pointValueIdentifierNodeIdDictionary 
								else if (PointManager.IsAcknowledgedNodeID(identifier))
								{
									if (MonitoredNodes.TryGetValue(nodeId, out monitoredNode))
									{
										var node = monitoredNode.Node as DataItemState;
										if (node != null
										&& (node.Value != null
										|| node.Value == null
										|| (node.Value != null && !node.Value.Equals(pointValue.Acknowledged))
										|| node.StatusCode != new StatusCode((uint)pointValue.Status)
										|| node.Timestamp != new DateTime(pointValue.ServerTimeStamp.Ticks, DateTimeKind.Utc)))
										{
											node.Value = pointValue.Acknowledged;
											node.StatusCode = new StatusCode((uint)pointValue.Status);
											node.Timestamp = new DateTime(pointValue.ServerTimeStamp.Ticks, DateTimeKind.Utc);
											node.ClearChangeMasks(SystemContext, false);
										}
									}
								}

								else if (PointManager.IsDefinitionNodeID(identifier))
								{
									if (MonitoredNodes.TryGetValue(nodeId, out monitoredNode))
									{

										object value = null;

										var node = monitoredNode.Node as DataItemState;
										if (node != null
										&& ((node.Value == null && value != null)
										|| (node.Value != null && value == null)
										|| (node.Value != null && !node.Value.Equals(value))
										|| node.StatusCode != StatusCodes.BadNodeIdInvalid))
										{
											node.Value = value;
											node.StatusCode = StatusCodes.BadNodeIdInvalid;
											node.Timestamp = new DateTime(DateTimeOffset.UtcNow.Ticks, DateTimeKind.Utc);
											node.ClearChangeMasks(SystemContext, false);
										}
									}
								}
							}
						}
					}

					nodeIdIndexStart = nodeIdsProcessed;
					pointValueIdentifierNodeIdDictionary.Clear();
				}
			}
		}


		public override NodeMetadata GetNodeMetadata(OperationContext context, object targetHandle, BrowseResultMask resultMask)
        {
			ServerSystemContext systemContext = base.SystemContext.Copy(context);

			lock (Lock)
			{
				// check for valid handle.
				NodeHandle handle = IsHandleInNamespace(targetHandle);

				if (handle == null)
				{
					return null;
				}

				// validate node.
				NodeState target = ValidateNode(systemContext, handle, null);

				if (target == null)
				{
					return null;
				}

				// read the attributes.
				List<object> values = target.ReadAttributes(
					systemContext,
					Attributes.WriteMask,
					Attributes.UserWriteMask,
					Attributes.DataType,
					Attributes.ValueRank,
					Attributes.ArrayDimensions,
					Attributes.AccessLevel,
					Attributes.UserAccessLevel,
					Attributes.EventNotifier,
					Attributes.Executable,
					Attributes.UserExecutable);

				// construct the metadata object.
				NodeMetadata metadata = new NodeMetadata(target, target.NodeId);

				metadata.NodeClass = target.NodeClass;
				metadata.BrowseName = target.BrowseName;
				metadata.DisplayName = target.DisplayName;

				if (values[0] != null && values[1] != null)
				{
					metadata.WriteMask = (AttributeWriteMask)(((uint)values[0]) & ((uint)values[1]));
				}

				metadata.DataType = (NodeId)values[2];

				if (values[3] != null)
				{
					metadata.ValueRank = (int)values[3];
				}

				metadata.ArrayDimensions = (IList<uint>)values[4];

				if (values[5] != null && values[6] != null)
				{
					metadata.AccessLevel = (byte)(((byte)values[5]) & ((byte)values[6]));
				}

				if (values[7] != null)
				{
					metadata.EventNotifier = (byte)values[7];
				}

				if (values[8] != null && values[9] != null)
				{
					metadata.Executable = (((bool)values[8]) && ((bool)values[9]));
				}

				// get instance references.
				BaseInstanceState instance = target as BaseInstanceState;

				if (instance != null)
				{
					metadata.TypeDefinition = instance.TypeDefinitionId;
					metadata.ModellingRule = instance.ModellingRuleId;
				}

				// fill in the common attributes.
				return metadata;
			}
		}

		public override NodeMetadata GetPermissionMetadata(OperationContext context, object targetHandle, BrowseResultMask resultMask, Dictionary<NodeId, List<object>> uniqueNodesServiceAttributes, bool permissionsOnly)
        {
			ServerSystemContext systemContext = base.SystemContext.Copy(context);

			lock (Lock)
			{
				// check for valid handle.
				NodeHandle handle = IsHandleInNamespace(targetHandle);

				if (handle == null)
				{
					return null;
				}

				// validate node.
				NodeState target = ValidateNode(systemContext, handle, null);

				if (target == null)
				{
					return null;
				}

				// read the attributes.
				List<object> values = target.ReadAttributes(
					systemContext,
					Attributes.RolePermissions,
					Attributes.UserRolePermissions);

				// construct the metadata object.
				NodeMetadata metadata = new NodeMetadata(target, target.NodeId);

				metadata.NodeClass = target.NodeClass;
				metadata.BrowseName = target.BrowseName;
				metadata.DisplayName = target.DisplayName;

				// get instance references.
				BaseInstanceState instance = target as BaseInstanceState;

				if (instance != null)
				{
					metadata.TypeDefinition = instance.TypeDefinitionId;
					metadata.ModellingRule = instance.ModellingRuleId;
				}

				// fill in the common attributes.
				return metadata;
			}
		}


		protected override StatusCode ValidateMonitoringFilter(ServerSystemContext context, NodeHandle handle, uint attributeId, double samplingInterval, uint queueSize, ExtensionObject filter, out MonitoringFilter filterToUse, out Range range, out MonitoringFilterResult result)
        {
            return base.ValidateMonitoringFilter(context, handle, attributeId, samplingInterval, queueSize, filter, out filterToUse, out range, out result);
        }

        //protected override void ReadInitialValue(ServerSystemContext context, NodeHandle handle, MonitoredItem monitoredItem)
        //{

        //    base.ReadInitialValue(context, handle, monitoredItem);
        //}

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
			lock (this.Lock)
			{
				IList<IReference> references;

				if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
				{
					externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
				}

				this.rootFolder = new FolderState(null)
				{
					NodeId = new NodeId(Guid.NewGuid(), this.NamespaceIndex),
					BrowseName = new QualifiedName("root", this.NamespaceIndex),
					EventNotifier = EventNotifiers.SubscribeToEvents
				};

				this.rootFolder.DisplayName = this.rootFolder.BrowseName.Name;
				this.rootFolder.TypeDefinitionId = ObjectTypeIds.FolderType;


				this.AddPredefinedNode(this.SystemContext, this.rootFolder);
			}
		}

		/// <summary>
		/// Frees any resources allocated for the address space.
		/// </summary>
		public override void DeleteAddressSpace()
		{

			lock (this.Lock)
			{
				this.SiteDictionary.Clear();
				this.SitePointRowVersion.Clear();
				this.SitePointDictionary.Clear();
				this.SitePointRowVersion.Clear();
				this.PointDictionary.Clear();
				this.PointValueDictionary.Clear();
			}
		}

		/// <summary>
		/// Returns a unique handle for the node.
		/// </summary>
		/// 
		protected override NodeHandle GetManagerHandle(
			ServerSystemContext context,
			NodeId nodeId,
			IDictionary<NodeId, NodeState> cache)
		{
			lock (this.Lock)
			{
				// quickly exclude nodes that are not in the namespace. 
				if (!this.IsNodeIdInNamespace(nodeId))
				{
					return null;
				}

				NodeState node = null;

				if (this.PredefinedNodes != null && !this.PredefinedNodes.TryGetValue(nodeId, out node))
				{
					node = new DataItemState(null);
					node.Create(context, nodeId, "", "", false);
					this.AddPredefinedNode(context, node);
				}

				NodeHandle handle = new NodeHandle();

				handle.NodeId = nodeId;
				handle.Node = node;
				handle.Validated = true;

				return handle;
			}
		}


		public override void Browse(
			OperationContext context,
			ref ContinuationPoint continuationPoint,
			IList<ReferenceDescription> references)
		{
			lock (this.Lock)
			{

				if (continuationPoint == null)
				{
					return;
				}

				var userIdentityToken = context.UserIdentity.GetIdentityToken() as X509IdentityToken;


				if (context.UserIdentity.TokenType == UserTokenType.UserName
				|| (context.UserIdentity.TokenType == UserTokenType.Certificate
				&& userIdentityToken != null
				&& userIdentityToken.Certificate.SubjectName.Name != null))
				{

					string[] subStrings = new String[] { };
					string certificateIssuedTo = null;

					if (context.UserIdentity.TokenType == UserTokenType.Certificate)
					{
						subStrings = userIdentityToken.Certificate.SubjectName.Name.Split(new char[] { ',' });
						if (subStrings.Length != 0)
						{
							certificateIssuedTo = subStrings[0].Replace("CN=", "");
						}
					}


					if (context.UserIdentity.TokenType == UserTokenType.UserName
					|| !string.IsNullOrEmpty(certificateIssuedTo))
					{
						this.LoadSites();



						if (continuationPoint.NodeToBrowse is BrowseDescription)
						{
							var node = continuationPoint.NodeToBrowse as BrowseDescription;
							if (node.NodeId == ObjectIds.ObjectsFolder && node.BrowseDirection == BrowseDirection.Forward)
							{
								this.AddSiteReferences(context.UserIdentity.TokenType, certificateIssuedTo, this.SiteHierarchy, continuationPoint, references);
							}
							else
							{

								if (node.NodeId.Identifier is string)
								{
									string identifier = (string)node.NodeId.Identifier;
									if (PointManager.IsSiteNodeID(identifier))
									{
										Guid guid;
										PointManager.ParseSiteNodeID(identifier, out guid);

										if (this.SiteDictionary.ContainsKey(guid))
										{
											this.AddSitePointReferences(guid, continuationPoint, references);

											var continuationHierarchy = this.SearchForSiteContinuationPoint(guid, this.SiteHierarchy);

											if (continuationHierarchy != null)
											{
												this.AddSiteReferences(context.UserIdentity.TokenType, certificateIssuedTo, continuationHierarchy, continuationPoint, references);
											}
										}
									}

									else if (PointManager.IsPointNodeID(identifier))
									{
										Guid guid;
										string propertyID;
										PointManager.ParsePointNodeID(identifier, out guid, out propertyID);
										if (string.IsNullOrEmpty(propertyID))
										{
											this.AddPointValueReferences(guid, continuationPoint, references);
										}
									}

									else if (PointManager.IsTagNodeID(identifier))
									{
										Guid guid;
										PointManager.ParseTagNodeID(identifier, out guid);

										this.AddTagSubReferences(guid, continuationPoint, references);
									}
									else if (PointManager.IsAlarmSourceTagNodeID(identifier))
									{
										Guid pointGuid, tagGuid;
										PointManager.ParseAlarmSourceTagNodeID(identifier, out pointGuid, out tagGuid);

										this.AddAlarmSourceTagReferences(pointGuid, tagGuid, continuationPoint, references);
									}
									else if (PointManager.IsAlarmMonitorNodeID(identifier))
									{
										Guid pointGuid, tagGuid, alarmGuid;
										PointManager.ParseAlarmMonitorNodeID(identifier, out pointGuid, out tagGuid, out alarmGuid);
										this.AddAlarmMonitorTagReferences(pointGuid, tagGuid, alarmGuid, continuationPoint, references);
									}
									else if (PointManager.IsAcknowledgeNodeID(identifier))
									{
										Guid pointGuid, tagGuid, alarmGuid;
										PointManager.ParseAcknowledgeNodeID(identifier, out pointGuid, out tagGuid, out alarmGuid);

										this.AddAlarmAcknowledgeArguments(pointGuid, tagGuid, alarmGuid.ToString(), continuationPoint, references);
									}
								}
							}
						}
					}
				}
			}
		}

		protected override void HistoryReadRawModified(
			ServerSystemContext context,
			ReadRawModifiedDetails details,
			TimestampsToReturn timestampsToReturn,
			IList<HistoryReadValueId> nodesToRead,
			IList<HistoryReadResult> results,
			IList<ServiceResult> errors,
			List<NodeHandle> nodesToProcess,
			IDictionary<NodeId, NodeState> cache)
		{
			for (int ii = 0; ii < nodesToRead.Count; ii++)
			{
				NodeHandle handle = nodesToProcess[ii];
				HistoryReadValueId nodeToRead = nodesToRead[handle.Index];
				HistoryReadResult result = results[handle.Index];
				try
				{
					// validate node.
					NodeState source = this.ValidateNode(context, handle, cache);

					//  handle.NodeId contains the guid for the requested tag

					if (source == null)
					{
						continue;
					}

					// add user name check here
					if (context.UserIdentity.DisplayName == "usr")
					{
						//						errors[handle.Index] = StatusCodes.BadUserAccessDenied;
						//						continue;
					}

					// load an exising request.
					// need to add later
					//						if (nodeToRead.ContinuationPoint != null)
					//						{
					//								request = LoadContinuationPoint(context, nodeToRead.ContinuationPoint);

					//								if (request == null)
					//								{
					//									errors[handle.Index] = StatusCodes.BadContinuationPointInvalid;
					//									continue;
					//								}
					//						}
					//						else	// create a new request.

					{
						var request = this.CreateHistoryReadRequest(context, details, handle, nodeToRead);
						if (request == null)
						{
							errors[handle.Index] = ServiceResult.Create(StatusCodes.BadNodeIdInvalid, "Invalid Node Selected.");
						}
						else if (request.Tables.Count != 1)
						{
							HistoryData data = (details.IsReadModified) ? new HistoryModifiedData() : new HistoryData();
							result.HistoryData = new ExtensionObject(data);
							errors[handle.Index] = ServiceResult.Create(
								StatusCodes.GoodNoData,
								"No Data Available for Selected Date Ranges.");
						}
						else if (request.Tables[0].Rows.Count < 1)
						{
							HistoryData data = (details.IsReadModified) ? new HistoryModifiedData() : new HistoryData();
							result.HistoryData = new ExtensionObject(data);
							errors[handle.Index] = ServiceResult.Create(
								StatusCodes.GoodNoData,
								"No Data Available for Selected Date Ranges.");
						}
						else
						{
							// process the data set into the return data
							HistoryData data = (details.IsReadModified) ? new HistoryModifiedData() : new HistoryData();
							HistoryModifiedData modifiedData = data as HistoryModifiedData;
							foreach (DataRow row in request.Tables[0].Rows)
							{
								// datavalues with softing sdk are added value,status,value timestamp,server timestamp
								uint StatusCodeValue = Convert.ToUInt32(row[5].ToString());
								StatusCode StatusCode = new StatusCode(StatusCodeValue);
								DataValue value = new DataValue(
									row[2].ToString(),
									StatusCode,
									Convert.ToDateTime(row[6].ToString()),
									Convert.ToDateTime(row[7].ToString()));
								data.DataValues.Add(value);
							}
							errors[handle.Index] = ServiceResult.Good;
							// return the data.
							result.HistoryData = new ExtensionObject(data);
						}
					}
				}
				catch (Exception e)
				{
					errors[handle.Index] = ServiceResult.Create(
						e,
						StatusCodes.BadUnexpectedError,
						"Unexpected error processing request.");
				}
			}
		}

		private DataSet CreateHistoryReadRequest(
			ServerSystemContext context,
			ReadRawModifiedDetails details,
			NodeHandle handle,
			HistoryReadValueId nodeToRead)
		{
			bool sizeLimited = (details.StartTime == DateTime.MinValue || details.EndTime == DateTime.MinValue);
			bool applyIndexRangeOrEncoding = (nodeToRead.ParsedIndexRange != NumericRange.Empty
																	|| !QualifiedName.IsNull(nodeToRead.DataEncoding));
			bool returnBounds = !details.IsReadModified && details.ReturnBounds;
			bool timeFlowsBackward = (details.StartTime == DateTime.MinValue)
													|| (details.EndTime != DateTime.MinValue && details.EndTime < details.StartTime);

			LinkedList<DataValue> Archivevalues = new LinkedList<DataValue>();

			// read history. 
			DataSet dataSet = this.historicalData.ReadArchiveHistory(
				this.security,
				details.StartTime,
				details.EndTime,
				details.IsReadModified,
				handle.Node.NodeId);
			if (dataSet == null)
			{
				return null;
			}
			if (dataSet.Tables.Count == 1 && dataSet.Tables[0].Rows.Count > 0)
			{
				// process the data retruned for the querry

			}

			//				HistoryReadRequest request = new HistoryReadRequest();
			//				request.Values = Archivevalues;
			//				request.NumValuesPerNode = 0;
			//				request.Filter = null;
			return dataSet;
		}

		protected void AddTagSubReferences(
			Guid tagGuid,
			ContinuationPoint continuationPoint,
			IList<ReferenceDescription> references)
		{
			PointValue pointValue;
			if (!this.PointValueDictionary.TryGetValue(tagGuid, out pointValue))
			{
				var pointTag = FMChannelHelper.MakeCall<IPointTags, PointTag>(x => x.Get(this.security, tagGuid));
				if (pointTag == null
				|| pointTag.IdentityGuid == Guid.Empty)
				{
					return;
				}

				this.PointValueDictionary.Add(tagGuid, new PointValue(pointTag));

			}

			if (continuationPoint.Index > 0
			&& references.Count == continuationPoint.Index)
			{
				references.Clear();
			}

			if (continuationPoint.MaxResultsToReturn != 0
			&& references.Count > continuationPoint.MaxResultsToReturn)
			{ 
				continuationPoint.Index += references.Count;
				return;
			}
		}

		protected bool GetAlarmSourceSubTags(Point point, out Dictionary<Guid, PointTag> alarmSubTagDictionary, out Dictionary<Guid, PointTag> alarmSourceTagDictionary)
		{
			alarmSubTagDictionary = new Dictionary<Guid, PointTag>();
			alarmSourceTagDictionary = new Dictionary<Guid, PointTag>();
			foreach (var pointTag in point.Tags.Values)
			{
				if (pointTag.Alarms.Any())
				{
					if (!alarmSourceTagDictionary.ContainsKey(pointTag.PointTagGuid))
					{
						alarmSourceTagDictionary.Add(pointTag.PointTagGuid, pointTag);
					}
					foreach (var alarm in pointTag.Alarms.Values)
					{
                  if (point.Tags.TryGetValue(alarm.AlarmStateTagGuid, out PointTag alarmStateTag))
                  {
                        if (!alarmSubTagDictionary.ContainsKey(alarmStateTag.PointTagGuid))
                        {
                           alarmSubTagDictionary.Add(alarmStateTag.PointTagGuid, alarmStateTag);
                        }
                        foreach (var alarmTest in alarm.AlarmTests.Values)
                        {
                           PointTag limitTag;
                           if (point.Tags.TryGetValue(alarmTest.LimitTagGuid, out limitTag))
                           {
                              if (!alarmSubTagDictionary.ContainsKey(limitTag.PointTagGuid))
                              {
                                    alarmSubTagDictionary.Add(limitTag.PointTagGuid, limitTag);
                              }
                           }
                        }
                  }
               }
				}
			}
			return true;
		}

		protected bool GetPointTagById(Dictionary<Guid, PointTag> tags, string tagId, out PointTag returnTag)
		{
			foreach (var tag in tags.Values)
			{
				if (tag.ID == tagId)
				{
					returnTag = tag;
					return true;
				}
			}

			returnTag = null;

			return false;
		}

		protected List<PointTag> GetAlarmSubTags(Point point, Guid tagGuid, Guid alarmGuid)
		{
			var alarmSubTags = new List<PointTag>();
			if (point != null && point.Tags != null && point.Tags.Any())
			{
				PointTag tag;
				if (point.Tags.TryGetValue(tagGuid, out tag))
				{
					Alarm alarm;
					if (tag.Alarms != null && tag.Alarms.Any() && tag.Alarms.TryGetValue(alarmGuid, out alarm))
					{
						PointTag alarmStateTag;
						if (point.Tags.TryGetValue(alarm.AlarmStateTagGuid, out alarmStateTag))
						{
							alarmSubTags.Add(alarmStateTag);
						}
						if (alarm.AlarmTests != null && alarm.AlarmTests.Any())
						{
							foreach (var alarmTest in alarm.AlarmTests.Values)
							{
								PointTag limitTag;
								if (point.Tags.TryGetValue(alarmTest.LimitTagGuid, out limitTag))
								{
									alarmSubTags.Add(limitTag);
								}
							}
						}
					}
				}
			}

			return alarmSubTags;
		}

		protected void AddAlarmMonitorTagReferences(
			Guid pointGuid,
			Guid tagGuid,
			Guid alarmGuid,
			ContinuationPoint continuationPoint,
			IList<ReferenceDescription> references)
		{
			Point point;
			if (!this.PointDictionary.TryGetValue(pointGuid, out point))
			{
				point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.security, pointGuid));
				if (point == null
				|| point.IdentityGuid == Guid.Empty)
				{
					return;
				}

				this.PointDictionary.Add(pointGuid, point);
			}
			var alarmSubTags = this.GetAlarmSubTags(point, tagGuid, alarmGuid);
			foreach (var tag in alarmSubTags)
			{
				var tagNodeId = new NodeId(PointManager.CreateTagNodeID(tag.IdentityGuid), this.NamespaceIndex);
				var referenceDescription = new ReferenceDescription
				{
					BrowseName = new QualifiedName(tag.ID, this.NamespaceIndex),
					DisplayName = tag.ID,
					IsForward = true,
					NodeClass = NodeClass.Variable,
					ReferenceTypeId = ReferenceTypeIds.Organizes,
					TypeDefinition = new ExpandedNodeId(ObjectTypeIds.BaseObjectType),
					NodeId = tagNodeId
				};

				references.Add(referenceDescription);

				if (continuationPoint.Index > 0 && references.Count == continuationPoint.Index)
				{
					references.Clear();
				}

				if (continuationPoint.MaxResultsToReturn != 0
				&& references.Count > continuationPoint.MaxResultsToReturn)
				{
					continuationPoint.Index += references.Count;
					return;
				}
			}
			this.AddAlarmAcknowledgedDataItem(pointGuid, tagGuid, alarmGuid, continuationPoint, references);
			this.AddAlarmAcknowledgeMethod(pointGuid, tagGuid, alarmGuid, continuationPoint, references);

		}

		//Need to get the constant for the acknowledge method type id from the softing code when we get it.
		protected static ExpandedNodeId ackMethodExpandedNodeId = new ExpandedNodeId(new NodeId("ns=0;i=9111"));

		protected void AddAlarmAcknowledgeArguments(Guid pointGuid, Guid tagGuid, string alarmGuid, ContinuationPoint continuationPoint, IList<ReferenceDescription> references)
		{
			var referenceDescription = new ReferenceDescription
			{
				BrowseName = BrowseNames.InputArguments,
				DisplayName = "InputArguments",
				IsForward = true,
				NodeClass = NodeClass.Variable,
				ReferenceTypeId = ReferenceTypeIds.HasProperty,
				TypeDefinition = VariableTypeIds.PropertyType, //DataTypeIds.Argument,
				NodeId = new NodeId(PointManager.CreateAckInputsNodeID(pointGuid ,tagGuid, new Guid(alarmGuid)), this.NamespaceIndex)
			};

			references.Add(referenceDescription);

			referenceDescription = new ReferenceDescription
			{
				BrowseName = BrowseNames.OutputArguments,
				DisplayName = "OutputArguments",
				IsForward = true,
				NodeClass = NodeClass.Variable,
				ReferenceTypeId = ReferenceTypeIds.HasProperty,
				TypeDefinition = VariableTypeIds.PropertyType,
				NodeId = new NodeId(PointManager.CreateAckOutputsNodeID(pointGuid, tagGuid, new Guid(alarmGuid)), this.NamespaceIndex)
			};

			references.Add(referenceDescription);

			if (continuationPoint.Index > 0 && references.Count == continuationPoint.Index)
			{
				references.Clear();
			}

			if (continuationPoint.MaxResultsToReturn != 0
			&& references.Count > continuationPoint.MaxResultsToReturn)
			{
				continuationPoint.Index += references.Count;
				return;
			}
		}

		protected void AddAlarmAcknowledgeMethod(Guid pointGuid, Guid tagGuid, Guid alarmGuid, ContinuationPoint continuationPoint, IList<ReferenceDescription> references)
		{
			var referenceDescription = new ReferenceDescription
			{
				BrowseName = new QualifiedName("Acknowledge", this.NamespaceIndex),
				DisplayName = "Acknowledge",
				IsForward = true,
				NodeClass = NodeClass.Method,
				ReferenceTypeId = ReferenceTypeIds.HasProperty,
				TypeDefinition = new ExpandedNodeId(ackMethodExpandedNodeId),
				NodeId = new NodeId(PointManager.CreateAcknowledgeNodeID(pointGuid, tagGuid, alarmGuid), this.NamespaceIndex)
			};

			references.Add(referenceDescription);

			if (continuationPoint.Index > 0 && references.Count == continuationPoint.Index)
			{
				references.Clear();
			}

			if (continuationPoint.MaxResultsToReturn != 0
			&& references.Count > continuationPoint.MaxResultsToReturn)
			{
				continuationPoint.Index += references.Count;
				return;
			}
		}

		protected void AddAlarmAcknowledgedDataItem(Guid pointGuid, Guid tagGuid, Guid alarmGuid, ContinuationPoint continuationPoint, IList<ReferenceDescription> references)
		{
			var acknowledgedNodeId = new NodeId(PointManager.CreateAcknowledgedNodeID(pointGuid,tagGuid,alarmGuid), this.NamespaceIndex);
			var referenceDescription = new ReferenceDescription
			{
				BrowseName = new QualifiedName("Acknowledged", this.NamespaceIndex),
				DisplayName = "Acknowledged",
				IsForward = true,
				NodeClass = NodeClass.Variable,
				ReferenceTypeId = ReferenceTypeIds.Organizes,
				TypeDefinition =
					new ExpandedNodeId(ObjectTypeIds.BaseObjectType),
				NodeId = acknowledgedNodeId
			};

			references.Add(referenceDescription);

			if (continuationPoint.Index > 0 && references.Count == continuationPoint.Index)
			{
				references.Clear();
			}

			if (continuationPoint.MaxResultsToReturn != 0
			&& references.Count > continuationPoint.MaxResultsToReturn)
			{
				continuationPoint.Index += references.Count;
			}
		}

		protected void AddAlarmSourceTagReferences(
			Guid pointGuid,
			Guid tagGuid,
			ContinuationPoint continuationPoint,
			IList<ReferenceDescription> references)
		{
			Point point;
			if (!this.PointDictionary.TryGetValue(pointGuid, out point))
			{
				point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.security, pointGuid));
				if (point == null
				|| point.IdentityGuid == Guid.Empty)
				{
					return;
				}

				this.PointDictionary.Add(pointGuid, point);

			}

			PointTag pointTag;
			if (!point.Tags.TryGetValue(tagGuid, out pointTag))
			{
				return;
			}


			foreach (var alarm in pointTag.Alarms.Values)
			{
				var tagNodeId = new NodeId(PointManager.CreateAlarmMonitorNodeID(pointGuid,tagGuid,alarm.IdentityGuid), this.NamespaceIndex);
				//NodeClass and TypeDefinition need to be modified.
				var referenceDescription = new ReferenceDescription
				{
					BrowseName = new QualifiedName(alarm.ID + " Alarm Monitor", this.NamespaceIndex),
					DisplayName = alarm.ID + " Alarm Monitor",
					IsForward = true,
					NodeClass = NodeClass.Object,
					ReferenceTypeId = ReferenceTypeIds.Organizes,
					TypeDefinition = new ExpandedNodeId(ObjectTypeIds.BaseObjectType),
					NodeId = tagNodeId
				};

				references.Add(referenceDescription);
			}

			if (continuationPoint.Index > 0 && references.Count == continuationPoint.Index)
			{
				references.Clear();
			}

			if (continuationPoint.MaxResultsToReturn != 0
			&& references.Count > continuationPoint.MaxResultsToReturn)
			{
				continuationPoint.Index += references.Count;
				return;
			}

		}

		/// <summary>
		/// Adds the point value references.
		/// </summary>
		/// <param name="pointGuid">The point unique identifier.</param>
		/// <param name="continuationPoint">The continuation point.</param>
		/// <param name="references">The references.</param>
		protected void AddPointValueReferences(
			Guid pointGuid,
			ContinuationPoint continuationPoint,
			IList<ReferenceDescription> references)
		{
			Point point;
			if (!this.PointDictionary.TryGetValue(pointGuid, out point))
			{
				point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.security, pointGuid));
				if (point == null
				|| point.IdentityGuid == Guid.Empty)
				{
					return;
				}

				this.PointDictionary.Add(pointGuid, point);
			}

			//SRM: I need to remove alarm specific tags from this level
			Dictionary<Guid, PointTag> alarmSubTagDictionary;
			Dictionary<Guid, PointTag> alarmSourceTagDictionary;
			this.GetAlarmSourceSubTags(point, out alarmSubTagDictionary, out alarmSourceTagDictionary);

			foreach (var tag in point.Tags.Values)
			{
				if (!this.PointValueDictionary.ContainsKey(tag.IdentityGuid))
				{
					this.PointValueDictionary.Add(tag.IdentityGuid, new PointValue(tag));
				}

				if (alarmSubTagDictionary != null && alarmSubTagDictionary.ContainsKey(tag.PointTagGuid))
				{
					continue;
				}

				NodeId tagNodeId;
				NodeId referenceTypeId;
				NodeClass nodeClass;
				ExpandedNodeId typeDefinition;
				if (alarmSourceTagDictionary != null && alarmSourceTagDictionary.ContainsKey(tag.PointTagGuid))
				{
					tagNodeId = new NodeId(PointManager.CreateAlarmSourceTagNodeID(tag.PointGuid,tag.IdentityGuid), this.NamespaceIndex);
					nodeClass = NodeClass.Variable;
					//var nodeIdForExpandedNodeId = new NodeId("ns=0;i=2138");
					//typeDefinition = new ExpandedNodeId(nodeIdForExpandedNodeId);
					typeDefinition = new ExpandedNodeId(ObjectTypeIds.BaseObjectType);
					referenceTypeId = ReferenceTypeIds.Organizes;
				}
				else
				{
					tagNodeId = new NodeId(PointManager.CreateTagNodeID(tag.IdentityGuid), this.NamespaceIndex);
					nodeClass = NodeClass.Variable;
					typeDefinition = new ExpandedNodeId(ObjectTypeIds.BaseObjectType);
					referenceTypeId = ReferenceTypeIds.Organizes;
				}
				var referenceDescription = new ReferenceDescription
				{
					BrowseName = new QualifiedName(tag.ID, this.NamespaceIndex),
					DisplayName = tag.ID,
					IsForward = true,
					NodeClass = nodeClass,
					ReferenceTypeId = referenceTypeId,
					TypeDefinition = typeDefinition,
					NodeId = tagNodeId
				};

				references.Add(referenceDescription);

				if (continuationPoint.Index > 0
				&& references.Count == continuationPoint.Index)
				{
					references.Clear();
				}

				if (continuationPoint.MaxResultsToReturn != 0
				&& references.Count > continuationPoint.MaxResultsToReturn)
				{
					continuationPoint.Index += references.Count;
					return;
				}

			}

			foreach (var property in point.Properties.Values)
			{
				var pointValues = property.GetExposedSettings(point);
				foreach (var pointValue in pointValues)
				{
					NodeId settingNodeId;
					NodeId referenceTypeId;
					NodeClass nodeClass;
					ExpandedNodeId typeDefinition;

					settingNodeId = new NodeId(PointManager.CreateSettingNodeID(pointValue.PointGuid, pointValue.PointValueIdentifier.IdentityGuid, pointValue.PointValueIdentifier.PropertyID), this.NamespaceIndex);
						nodeClass = NodeClass.Variable;
						typeDefinition = new ExpandedNodeId(ObjectTypeIds.BaseObjectType);
						referenceTypeId = ReferenceTypeIds.Organizes;

					var referenceDescription = new ReferenceDescription
					{
						BrowseName = new QualifiedName(pointValue.ID, this.NamespaceIndex),
						DisplayName = pointValue.ID,
						IsForward = true,
						NodeClass = nodeClass,
						ReferenceTypeId = referenceTypeId,
						TypeDefinition = typeDefinition,
						NodeId = settingNodeId
					};

					references.Add(referenceDescription);

					if (continuationPoint.Index > 0
					&& references.Count == continuationPoint.Index)
					{
						references.Clear();
					}

					if (continuationPoint.MaxResultsToReturn != 0
					&& references.Count > continuationPoint.MaxResultsToReturn)
					{
						continuationPoint.Index += references.Count;
						return;
					}
				}
			}

			foreach (var pointValue in point.GetExposedSettings())
			{
				NodeId settingNodeId;
				NodeId referenceTypeId;
				NodeClass nodeClass;
				ExpandedNodeId typeDefinition;

				settingNodeId = new NodeId(PointManager.CreatePointNodeID(pointValue.PointGuid, pointValue.PointValueIdentifier.PropertyID), this.NamespaceIndex);
				nodeClass = NodeClass.Variable;
				typeDefinition = new ExpandedNodeId(ObjectTypeIds.BaseObjectType);
				referenceTypeId = ReferenceTypeIds.Organizes;

				var referenceDescription = new ReferenceDescription
				{
					BrowseName = new QualifiedName(pointValue.ID, this.NamespaceIndex),
					DisplayName = pointValue.ID,
					IsForward = true,
					NodeClass = nodeClass,
					ReferenceTypeId = referenceTypeId,
					TypeDefinition = typeDefinition,
					NodeId = settingNodeId
				};

				references.Add(referenceDescription);

				if (continuationPoint.Index > 0
				&& references.Count == continuationPoint.Index)
				{
					references.Clear();
				}

				if (continuationPoint.MaxResultsToReturn != 0
				&& references.Count > continuationPoint.MaxResultsToReturn)
				{
					continuationPoint.Index += references.Count;
					return;
				}
			}
		}

		/// <summary>
		/// Adds the site points.
		/// </summary>
		/// <param name="siteGuid">The site unique identifier.</param>
		/// <param name="continuationPoint">The continuation point.</param>
		/// <param name="references">The references.</param>
		protected void AddSitePointReferences(
			Guid siteGuid,
			ContinuationPoint continuationPoint,
			IList<ReferenceDescription> references)
		{
			var currentSitePointMaxRowVersion = FMChannelHelper.MakeCall<IPoints, long?>(x => x.GetMaxPointRowVersionForSite(this.security, siteGuid));

			if (!currentSitePointMaxRowVersion.HasValue)
			{
				return;
			}

			long sitePointMaxRowVersion;

			if (!this.SitePointRowVersion.TryGetValue(siteGuid, out sitePointMaxRowVersion))
			{
				this.SitePointRowVersion.Add(siteGuid, currentSitePointMaxRowVersion.Value);
			}

			Dictionary<Guid, Point> pointDictionary;

			// Update the pointDictionary for this site if there have been changes.
			if (currentSitePointMaxRowVersion.Value != sitePointMaxRowVersion)
			{
				this.SitePointRowVersion[siteGuid] = currentSitePointMaxRowVersion.Value;

				var pointCollection =
					FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.EnumerateBySite(this.security, siteGuid));

				if (this.SitePointDictionary.TryGetValue(siteGuid, out pointDictionary))
				{
					foreach (var pointGuid in pointDictionary.Keys)
					{
						Point point;
						if (this.PointDictionary.TryGetValue(pointGuid, out point))
						{
							this.PointDictionary.Remove(pointGuid);
							foreach (var tagGuid in point.Tags.Keys)
							{
								this.PointValueDictionary.Remove(tagGuid);
							}
						}
					}
				}

				this.SitePointDictionary.Remove(siteGuid);

				pointDictionary = new Dictionary<Guid, Point>();

				this.SitePointDictionary.Add(siteGuid, pointDictionary);

				foreach (var point in pointCollection)
				{
					pointDictionary.Add(point.IdentityGuid, point);
					if (!this.PointDictionary.ContainsKey(point.IdentityGuid))
					{
						this.PointDictionary.Add(point.IdentityGuid, point);
					}
				}
			}

			if (this.SitePointDictionary.TryGetValue(siteGuid, out pointDictionary))
			{
				foreach (var point in pointDictionary.Values)
				{
					var referenceDescription = new ReferenceDescription
					{
						BrowseName = new QualifiedName(point.ID, this.NamespaceIndex),
						DisplayName = point.ID,
						IsForward = true,
						NodeClass = NodeClass.Object,
						ReferenceTypeId = ReferenceTypeIds.Organizes,
						TypeDefinition = new ExpandedNodeId(ObjectTypeIds.BaseObjectType),
						NodeId = new NodeId(PointManager.CreatePointNodeID(point.IdentityGuid, null), this.NamespaceIndex)
					};

					references.Add(referenceDescription);

					if (continuationPoint.Index > 0
					&& continuationPoint.MaxResultsToReturn != 0
					&& references.Count > continuationPoint.Index + continuationPoint.MaxResultsToReturn)
					{
						int index = 0;
						while (index < continuationPoint.Index)
						{
							references.RemoveAt(0);
							index++;
						}
					}

					if (continuationPoint.MaxResultsToReturn != 0
					&& references.Count > continuationPoint.MaxResultsToReturn)
					{
						continuationPoint.Index += references.Count;
						return;
					}
				}
			}
		}

		/// <summary>
		/// Adds the site references.
		/// </summary>
		/// <param name="certificateIssueTo">The certificate issue to.</param>
		/// <param name="siteHierarchy">The site hierarchy.</param>
		/// <param name="continuationPoint">The continuation point.</param>
		/// <param name="references">The references.</param>
		protected void AddSiteReferences(
		UserTokenType tokenType,
		string certificateIssueTo,
		Dictionary<Guid, object> siteHierarchy,
		ContinuationPoint continuationPoint,
		IList<ReferenceDescription> references)
		{
			var node = continuationPoint.NodeToBrowse as BrowseDescription;
			foreach (var siteGuid in siteHierarchy.Keys)
			{
				var site = this.SiteDictionary[siteGuid];
				if (site == null)
				{
					continue;
				}

				if (tokenType == UserTokenType.UserName
				|| site.SiteCertificateCollection.Find(x => x.ID == certificateIssueTo) != null)
				{

					var referenceDescription = new ReferenceDescription
					{
						BrowseName = new QualifiedName(site.ID, this.NamespaceIndex),
						DisplayName = site.ID,
						IsForward = true,
						NodeClass = NodeClass.Object,
						ReferenceTypeId = ReferenceTypeIds.Organizes,
						TypeDefinition = new ExpandedNodeId(ObjectTypeIds.FolderType),
						NodeId = new NodeId(PointManager.CreateSiteNodeID(site.SiteGuid), this.NamespaceIndex)
					};

					var found = false;
					foreach (var existingreference in references)
					{
						if (existingreference.NodeId == referenceDescription.NodeId)
						{
							found = true;
							break;
						}
					}

					if (found)
					{
						continue;
					}

					references.Add(referenceDescription);

					// The following technique is used to ensure that no duplicate sites are included in the references
					if (continuationPoint.Index > 0
					&& continuationPoint.MaxResultsToReturn != 0
					&& references.Count > continuationPoint.Index + continuationPoint.MaxResultsToReturn)
					{
						int index = 0;
						while (index < continuationPoint.Index)
						{
							references.RemoveAt(0);
							index++;
						}
					}

					if (continuationPoint.MaxResultsToReturn != 0
					&& references.Count > continuationPoint.MaxResultsToReturn)
					{
						continuationPoint.Index += references.Count;
						return;
					}
				}

				// When browsing the ObjectIds.ObjectsFolder forward, include member sites that could be excluded due to site group certificates
				else if (siteHierarchy[siteGuid] != null
				&& node.NodeId == ObjectIds.ObjectsFolder
				&& node.BrowseDirection == BrowseDirection.Forward)
				{
					this.AddSiteReferences(tokenType, certificateIssueTo, siteHierarchy[siteGuid] as Dictionary<Guid, Object>, continuationPoint, references);
				}
			}
		}



		/// <summary>
		/// Searches for site continuation point.  A site may exist multiple places in the hierachy. The first one found will do.
		/// </summary>
		/// <param name="siteGuid">The site unique identifier.</param>
		/// <param name="hierarchy">The hierarchy.</param>
		/// <returns></returns>
		private Dictionary<Guid, object> SearchForSiteContinuationPoint(Guid siteGuid, Dictionary<Guid, object> hierarchy)
		{
			foreach (var guid in hierarchy.Keys)
			{
				if (siteGuid == guid)
				{
					if (hierarchy[guid] is Dictionary<Guid, object>)
					{
						return hierarchy[guid] as Dictionary<Guid, object>;
					}

					return null;
				}

				if (hierarchy[guid] is Dictionary<Guid, object>)
				{
					var continuationHierarchy = this.SearchForSiteContinuationPoint(siteGuid, hierarchy[guid] as Dictionary<Guid, object>);

					if (continuationHierarchy != null)
					{
						return continuationHierarchy;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Loads the sites.
		/// </summary>
		public void LoadSites()
		{
			bool configurationChanged = false;

			var currentSiteRowVersion = FMChannelHelper.MakeCall<ISites, long?>(x => x.GetMaxSiteRowVersion(this.security));

			if (currentSiteRowVersion.HasValue
				&& (!this.MaxSiteRowVersion.HasValue || currentSiteRowVersion.Value != this.MaxSiteRowVersion.Value))
			{
				this.MaxSiteRowVersion = currentSiteRowVersion;

				var siteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(x => x.Enumerate(this.security));

				var siteCertificateCollection =
					FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
						x => x.EnumerateByTypeAndSite(this.security, STRING_TYPE.SITE_CERTIFICATE, null));

				this.SiteDictionary.Clear();

				foreach (var site in siteCollection)
				{
					this.SiteDictionary.Add(site.SiteGuid, site);
				}

				foreach (var siteCertificate in siteCertificateCollection)
				{
					SiteClass site;
					if (this.SiteDictionary.TryGetValue(siteCertificate.SiteGuid, out site))
					{
						site.SiteCertificateCollection.Add(siteCertificate);
					}
				}

				configurationChanged = true;
			}

			var currentSiteToSiteMapRowVersion =
				FMChannelHelper.MakeCall<ISiteToSiteMaps, long?>(x => x.GetMaxSiteToSiteMapRowVersion(this.security));

			// Site.Enabled and Site.Enterprise influence the SiteHierarchcy
			if (configurationChanged
				|| (currentSiteToSiteMapRowVersion.HasValue
					&& (!this.MaxSiteToSiteMapRowVersion.HasValue
								|| currentSiteToSiteMapRowVersion.Value != this.MaxSiteToSiteMapRowVersion.Value)))
			{
				this.MaxSiteToSiteMapRowVersion = currentSiteToSiteMapRowVersion;

				this.SiteHierarchy =
					FMChannelHelper.MakeCall<ISiteToSiteMaps, Dictionary<Guid, object>>(x => x.GetSiteHierarchy(this.security, true));
			}
		}
	}
}