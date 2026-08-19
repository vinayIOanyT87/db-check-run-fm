// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpcUaServer.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FMOpcUaServerService
{
	using System;
	using System.Collections.Generic;
	using System.DirectoryServices.AccountManagement;
	using System.IO;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;
	using System.Text;
	using System.Threading;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;
	using FMPointCommon;
	using Opc.Ua;
	using Opc.Ua.Server;
	using Softing.Opc.Ua.Server;

	public class OpcUaServer : UaServer
	{
		#region Private Fields
		private Thread updateThread;
		private ManualResetEvent shutdownEvent;
		private Dictionary<NodeId, string> evDictionary = new Dictionary<NodeId, string> ();
		private Dictionary<string, Dictionary<Guid, PointValue>> evPointValueDictionaries =  new Dictionary<string, Dictionary<Guid, PointValue>>();
		private Dictionary<string, Dictionary<Guid, PointProperty>> evPointPropertyDictionaries = new Dictionary<string, Dictionary<Guid, PointProperty>>();
		private SessionManager sessionManager;
		private OpcUaServerNodeManager nodeManager;
		private readonly SecurityClass security;

		#endregion

		#region Constructors
		/// <summary>
		/// Initializes the DataAccessServer.
		/// </summary>
		public OpcUaServer(SecurityClass security)
		{
			this.security = security;
			shutdownEvent = new ManualResetEvent(true);

		}
		#endregion

		private void OpcUaUpdateNodes(object data)
		{
			int sleepCycle = Convert.ToInt32(data, CultureInfo.InvariantCulture);
			int timeToWait = sleepCycle;

			do
			{
				try
				{
					this.nodeManager.Update();

					if (shutdownEvent.WaitOne(timeToWait, false))
					{
						Utils.Trace(Utils.TraceMasks.Information, "OpcUaServer.OpcUaUpdateNodes", "Update Nodes Thread Exited Normally.");
						break;
					}

					timeToWait = sleepCycle;
				}
				catch (Exception e)
				{
					Utils.Trace(Utils.TraceMasks.Error, "OpcUaSubscriptionManager.OpcUaUpdateNodes", "Update Node Thread Exited Unexpectedly", e);
				}
			}
			while (true);
		}




		#region Overridden Methods



		/// <summary>
		/// Creates the node managers for the server.
		/// </summary>
		/// <remarks>
		/// This method allows the sub-class create any additional node managers which it uses. The SDK
		/// always creates a CoreNodeManager which handles the built-in nodes defined by the specification.
		/// Any additional NodeManagers are expected to handle application specific nodes.
		/// </remarks>
		protected override MasterNodeManager CreateMasterNodeManager(
			 IServerInternal server,
			 ApplicationConfiguration configuration)
		{
			List<INodeManager> nodeManagers = new List<INodeManager>();

			// create the custom node managers.
			this.nodeManager = new OpcUaServerNodeManager(server, configuration, this.security);
			nodeManagers.Add(this.nodeManager);

			// create master node manager.
			return new OpcUaServerMasterNodeManager(server, configuration, null, nodeManagers.ToArray());
		}


		/// <summary>
		/// Loads the non-configurable properties for the application.
		/// </summary>
		/// <remarks>
		/// These properties are exposed by the server but cannot be changed by administrators.
		/// </remarks>
		protected override ServerProperties LoadServerProperties()
		{
			ServerProperties properties = new ServerProperties
			{
				ManufacturerName = "Varec",
				ProductName = "Varec FuelsManager Opc Ua Server",
				ProductUri = "",
				SoftwareVersion = Utils.GetAssemblySoftwareVersion(),
				BuildNumber = Utils.GetAssemblyBuildNumber(),
				BuildDate = Utils.GetAssemblyTimestamp()
			};

			// TBD - All applications have software certificates that need to be added to the properties.

			return properties;
		}

		public override ResponseHeader Browse(
				RequestHeader requestHeader,
				ViewDescription view,
				uint requestedMaxReferencesPerNode,
				BrowseDescriptionCollection nodesToBrowse,
				out BrowseResultCollection results,
				out DiagnosticInfoCollection diagnosticInfos)
		{
			var responseHeader = base.Browse(requestHeader, view, requestedMaxReferencesPerNode, nodesToBrowse, out results, out diagnosticInfos);

			if (results != null)
			{
				foreach (var result in results)
				{
					result.References = new ReferenceDescriptionCollection(result.References.OrderBy(x => (x.DisplayName == null || string.IsNullOrEmpty(x.DisplayName.Text)) ? x.BrowseName : x.DisplayName.Text));
				}
			}

			return responseHeader;
		}

		public override ResponseHeader Write(
				RequestHeader requestHeader,
				WriteValueCollection nodesToWrite,
				out StatusCodeCollection results,
				out DiagnosticInfoCollection diagnosticInfos)
		{

			results = new StatusCodeCollection();
			diagnosticInfos = new DiagnosticInfoCollection();
			var responseHeader = new ResponseHeader();
			responseHeader.RequestHandle = requestHeader.RequestHandle;
			var pointValues = new List<PointValue>();
			var pointProperties = new List<PointProperty>();
			var pointTagAlarmStatusList = new List<PointTagAlarmStatus>();
			var tagsToLoad = new List<Guid>();
			var pointsToLoad = new List<Guid>();


			Session session = this.sessionManager.GetSession(requestHeader.AuthenticationToken);

			if (session == null)
			{
				foreach (var writeValue in nodesToWrite)
				{
					results.Add(StatusCodes.GoodShutdownEvent);
					diagnosticInfos.Add(null);
				}

				return responseHeader;
			}

			bool enterpriseVisibility = false;

			Dictionary<Guid, PointValue> evPointValueDictionary = null;
			Dictionary<Guid, PointProperty> evPointPropertyDictionary = null;

			lock (base.Lock)
			{
				if (evDictionary.ContainsKey(session.Id))
				{
					enterpriseVisibility = true;
					var applicationUri = evDictionary[session.Id];

					if (!evPointValueDictionaries.TryGetValue(applicationUri, out evPointValueDictionary))
					{
						evPointValueDictionary = new Dictionary<Guid, PointValue>();
						evPointValueDictionaries.Add(applicationUri, evPointValueDictionary);
					}

					if (!evPointPropertyDictionaries.TryGetValue(applicationUri, out evPointPropertyDictionary))
					{
						evPointPropertyDictionary = new Dictionary<Guid, PointProperty>();
						evPointPropertyDictionaries.Add(applicationUri, evPointPropertyDictionary);
					}

				}
			}

			foreach (var writeValue in nodesToWrite)
			{
				if (writeValue.NodeId.NamespaceIndex < 2)
				{
					continue;
				}

				if (!(writeValue.NodeId.Identifier is string))
				{
					continue;
				}

				string identifier = (string)writeValue.NodeId.Identifier;
				PointValue pointValue = null;
				Point point = null;
				if (PointManager.IsPointNodeID(identifier)
				|| PointManager.IsTagNodeID(identifier)
				|| PointManager.IsSettingNodeID(identifier)
				|| PointManager.IsAlarmSourceTagNodeID(identifier))
				{
					Guid guid = Guid.Empty;
					Guid pointGuid = Guid.Empty;
					string propertyID = null;

					if (PointManager.IsPointNodeID(identifier))
					{
						PointManager.ParsePointNodeID(identifier, out pointGuid, out propertyID);
					}
					else if (PointManager.IsTagNodeID(identifier))
					{
						PointManager.ParseTagNodeID(identifier, out guid);
					}
					else if (PointManager.IsSettingNodeID(identifier))
					{
						PointManager.ParseSettingNodeID(identifier, out pointGuid, out guid, out propertyID);
					}
					else if (PointManager.IsAlarmSourceTagNodeID(identifier))
					{
						PointManager.ParseAlarmSourceTagNodeID(identifier, out pointGuid, out guid);
					}
					else
					{
						continue;
					}

					lock (this.nodeManager.Lock)
					{
						if (propertyID == null)
						{
							if (evPointValueDictionary != null)
							{
								if (!evPointValueDictionary.TryGetValue(guid, out pointValue))
								{
									tagsToLoad.Add(guid);
								}
							}
							else
							{
								if (!this.nodeManager.PointValueDictionary.TryGetValue(guid, out pointValue))
								{
									tagsToLoad.Add(guid);
								}
							}
						}
						else
						{
							if (!this.nodeManager.PointDictionary.TryGetValue(pointGuid, out point))
							{
								pointsToLoad.Add(pointGuid);
							}
						}
					}
				}
			}

			if (tagsToLoad.Count > 0)
			{
				var pointTags = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>>(x => x.EnumerateByTagList(this.security, tagsToLoad));

				if (evPointValueDictionary != null)
				{
					foreach (var pointTag in pointTags.Values)
					{
						if (pointTag.IdentityGuid != Guid.Empty
						&& !evPointValueDictionary.ContainsKey(pointTag.IdentityGuid))
						{
							evPointValueDictionary.Add(pointTag.IdentityGuid, new PointValue(pointTag));
						}
					}
				}

				else
				{
					lock (this.nodeManager.Lock)
					{
						foreach (var pointTag in pointTags.Values)
						{
							if (pointTag.IdentityGuid != Guid.Empty
							&& !this.nodeManager.PointValueDictionary.ContainsKey(pointTag.IdentityGuid))
							{
								this.nodeManager.PointValueDictionary.Add(pointTag.IdentityGuid, new PointValue(pointTag));
							}
						}
					}
				}
			}

			if (pointsToLoad.Count > 0)
			{
				if (evPointPropertyDictionary != null)
				{

					foreach (var pointGuid in pointsToLoad)
					{
						var point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.security, pointGuid));
						if (point != null
						&& point.IdentityGuid != Guid.Empty)
						{
							lock (this.nodeManager.Lock)
							{
								if (!this.nodeManager.PointDictionary.ContainsKey(pointGuid))
								{
									this.nodeManager.PointDictionary.Add(pointGuid, point);
								}
							}
						}
					}
				}
			}


			foreach (var writeValue in nodesToWrite)
			{

				if (writeValue.NodeId.NamespaceIndex < 2)
				{
					StatusCodeCollection intermediateResults = null;
					DiagnosticInfoCollection intermediateDiagnosticInfos = null;

					responseHeader = base.Write(requestHeader, new WriteValueCollection() { writeValue }, out intermediateResults, out intermediateDiagnosticInfos);


					if (intermediateResults != null)
					{
						foreach (var statusCode in intermediateResults)
						{
							results.Add(statusCode);
							if (intermediateDiagnosticInfos == null)
							{
								diagnosticInfos.Add(null);
							}
						}
					}

					if (intermediateDiagnosticInfos != null)
					{
						foreach (var diagnosticInfo in intermediateDiagnosticInfos)
						{
							diagnosticInfos.Add(diagnosticInfo);
						}
					}
				}
				else
				{
					if (!(writeValue.NodeId.Identifier is string))
					{
						results.Add(StatusCodes.BadAttributeIdInvalid);
						diagnosticInfos.Add(null);
					}
					else
					{
						string identifier = (string)writeValue.NodeId.Identifier;
						PointValue pointValue = null;
						PointProperty property = null;

						if(PointManager.IsPointTagAlarmStatusNodeID(identifier))
						{
							using (MemoryStream memoryStream = new MemoryStream(new UTF8Encoding().GetBytes(writeValue.Value.Value.ToString())))
							{
								DataContractSerializer serializer = new DataContractSerializer(typeof(PointTagAlarmStatus));
								PointTagAlarmStatus pointTagAlarmStatus = serializer.ReadObject(memoryStream) as PointTagAlarmStatus;
								pointTagAlarmStatusList.Add(pointTagAlarmStatus);
							}
							results.Add(StatusCodes.Good);
							diagnosticInfos.Add(null);
						}

						else if (PointManager.IsPointNodeID(identifier)
						|| PointManager.IsTagNodeID(identifier)
						|| PointManager.IsSettingNodeID(identifier)
						|| PointManager.IsAlarmSourceTagNodeID(identifier))
						{
							Guid guid = Guid.Empty;
							Guid pointGuid = Guid.Empty;
							string propertyID = null;
							if (PointManager.IsPointNodeID(identifier))
							{
								PointManager.ParsePointNodeID(identifier, out pointGuid, out propertyID);
							}
							else if (PointManager.IsTagNodeID(identifier))
							{
								PointManager.ParseTagNodeID(identifier, out guid);
							}
							else if (PointManager.IsSettingNodeID(identifier))
							{
								PointManager.ParseSettingNodeID(identifier, out pointGuid, out guid, out propertyID);
							}
							else if (PointManager.IsAlarmSourceTagNodeID(identifier))
							{
								PointManager.ParseAlarmSourceTagNodeID(identifier, out pointGuid, out guid);
							}

							lock (this.nodeManager.Lock)
							{
								if (propertyID == null)
								{
									if (evPointValueDictionary != null)
									{
										evPointValueDictionary.TryGetValue(guid, out pointValue);
									}
									else
									{
										this.nodeManager.PointValueDictionary.TryGetValue(guid, out pointValue);
									}
								}
								else
								{
									Point point;
									this.nodeManager.PointDictionary.TryGetValue(pointGuid, out point);
									if (point != null)
									{
										if (guid == Guid.Empty)
										{
											var pointValueIdentifier = new PointValueIdentifier(pointGuid, PointValueType.Point, propertyID);
											pointValue = new PointValue(pointValueIdentifier, point);
											point = null;
										}
										else
										{
											point.Properties.TryGetValue(guid, out property);
											if (property != null
											&& property.ID != "Movement Data"
											&& property.ID != "Movement Settings")
											{
												var pointValueIdentifier = new PointValueIdentifier(guid, PointValueType.Setting, propertyID);

												pointValue = new PointValue(pointValueIdentifier, property, point);
												point = null;
											}
										}
									}
								}
							}

							if (pointValue != null)
							{
								pointValue.ServerTimeStamp = writeValue.Value.ServerTimestamp;
								pointValue.SourceTimeStamp = writeValue.Value.SourceTimestamp;

								// if TimeStamp not provided, set to UtcNow
								if (pointValue.ServerTimeStamp.Ticks == 0)
								{
									pointValue.ServerTimeStamp = DateTimeOffset.UtcNow;
								}

								if (pointValue.SourceTimeStamp.Ticks == 0)
								{
									pointValue.SourceTimeStamp = DateTimeOffset.UtcNow;
								}

								pointValue.Status = (long)writeValue.Value.StatusCode;
								
								try
								{
									switch (pointValue.ValueTypeString)
									{
										case "System.Single":
											if (writeValue.Value.Value is string)
											{
												XmlSerializer serializer = CachingXmlSerializerFactory.Create(typeof(EnterpriseVisibilityData));
												var tempReader = new StringReader(writeValue.Value.Value as string);
												var data = serializer.Deserialize(tempReader) as EnterpriseVisibilityData;
												pointValue.EngineeringUnitsType = data.EngineeringUnitsType;
												pointValue.Units = data.Units;
												pointValue.Value = data.Value;
												pointValue.DecimalPlaces = data.DecimalPlaces;
												pointValue.Maximum = data.Maximum;
												pointValue.Minimum = data.Minimum;
											}
											else
											{
												if (writeValue.Value.Value == null)
												{
													pointValue.Value = null;
												}
												else
												{
													pointValue.Value = Convert.ToSingle(writeValue.Value.Value);
												}
											}

											pointValues.Add(pointValue);
											results.Add(StatusCodes.Good);
											diagnosticInfos.Add(null);
											break;

										case "System.Double":
											if (writeValue.Value.Value is string)
											{
												XmlSerializer serializer = CachingXmlSerializerFactory.Create(typeof(EnterpriseVisibilityData));
												var tempReader = new StringReader(writeValue.Value.Value as string);
												var data = serializer.Deserialize(tempReader) as EnterpriseVisibilityData;
												pointValue.EngineeringUnitsType = data.EngineeringUnitsType;
												pointValue.Units = data.Units;
												pointValue.Value = data.Value;
												pointValue.DecimalPlaces = data.DecimalPlaces;
												pointValue.Maximum = data.Maximum;
												pointValue.Minimum = data.Minimum;
											}
											else
											{
												if (writeValue.Value.Value == null)
												{
													pointValue.Value = null;
												}
												else
												{
													pointValue.Value = Convert.ToDouble(writeValue.Value.Value);
												}
											}

											pointValues.Add(pointValue);
											results.Add(StatusCodes.Good);
											diagnosticInfos.Add(null);
											break;

										case "FMBusinessObjects.DataObjects.PointCommandStatusListReference":
											if (pointValue.Value is PointCommandStatusListReference)
											{
												if (writeValue.Value.Value is string)
												{
													XmlSerializer serializer = CachingXmlSerializerFactory.Create(typeof(PointCommandStatusListReference));
													var tempReader = new StringReader(writeValue.Value.Value as string);
													pointValue.Value = (object)serializer.Deserialize(tempReader);
												}
												else
												{
													if (writeValue.Value.Value == null)
													{
														(pointValue.Value as PointCommandStatusListReference).CurrentValue = (int?)writeValue.Value.Value;
													}
													else
													{
														(pointValue.Value as PointCommandStatusListReference).CurrentValue = (int?)writeValue.Value.Value;
													}
												}
											}

											pointValues.Add(pointValue);
											results.Add(StatusCodes.Good);
											diagnosticInfos.Add(null);
											break;

										case "FMBusinessObjects.DataObjects.DeviceAlarmMapReference":
											if (pointValue.Value is DeviceAlarmMapReference)
											{
												if (writeValue.Value.Value is string)
												{
													XmlSerializer serializer = CachingXmlSerializerFactory.Create(typeof(DeviceAlarmMapReference));
													var tempReader = new StringReader(writeValue.Value.Value as string);
													pointValue.Value = (object)serializer.Deserialize(tempReader);
												}
												else
												{

													if (writeValue.Value.Value == null)
													{
														(pointValue.Value as DeviceAlarmMapReference).CurrentValue = (uint?)writeValue.Value.Value;
													}
													else
													{
														(pointValue.Value as DeviceAlarmMapReference).CurrentValue = (uint?)writeValue.Value.Value;
													}
												}
											}
											pointValues.Add(pointValue);
											results.Add(StatusCodes.Good);
											diagnosticInfos.Add(null);
											break;

										default:
											object value = writeValue.Value.Value;
											StatusCode statusCode = writeValue.Value.StatusCode;
											FMPointCommon.PointManager.ValidatePointTagValueByItsType(pointValue.ValueTypeString,
												ref value, ref statusCode);

											pointValue.Value = value;
											pointValues.Add(pointValue);
											results.Add(StatusCodes.Good);
											diagnosticInfos.Add(null);
											break;
									}
								}
								catch (Exception)
								{
									results.Add(StatusCodes.BadTypeMismatch);
									diagnosticInfos.Add(null);
								}
							}
							else if(property != null)
							{
								if (writeValue.Value.Value == null)
								{
									property.Value = null;
								}
								else
								{
									property.ValueXml = Convert.ToString(writeValue.Value.Value);
								}

								if (evPointPropertyDictionary != null)
								{
									if (evPointPropertyDictionary.ContainsKey(property.IdentityGuid))
									{
										evPointPropertyDictionary[property.IdentityGuid] = property;
									}
									else
									{
										evPointPropertyDictionary.Add(property.IdentityGuid, property);
									}
								}

								pointProperties.Add(property);
								results.Add(StatusCodes.Good);
								diagnosticInfos.Add(null);
							}
							else
							{
								results.Add(StatusCodes.BadNodeIdUnknown);
								diagnosticInfos.Add(null);
							}
						}
						else
						{
							results.Add(StatusCodes.BadNodeIdUnknown);
							diagnosticInfos.Add(null);
						}
					}
				}
			}

			if (pointValues.Count > 0)
			{
				FMChannelHelper.MakeCall<IPointServiceManager>(x => x.SetPointValueData(this.security, pointValues, enterpriseVisibility));
			}

			if(pointProperties.Count > 0)
			{
				var isEnterprise = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsEnterpriseKey());
				foreach (var property in pointProperties)
				{
					if (isEnterprise)
					{
						FMChannelHelper.MakeCall<IPointProperties>(x => x.ModifyPointPropertyValue(this.security, property, true, true));
					}
					else
					{
						// At the terminal, set bypassUpdatePointRecordVersion to false to flag that this Point needs to be reloaded by FMPointService
						FMChannelHelper.MakeCall<IPointProperties>(x => x.ModifyPointPropertyValue(this.security, property, false, true));
					}
				}
			}

			if(pointTagAlarmStatusList.Count > 0)
			{
				// EnableAdd is false and EnableUpdate is true, a PointTagAlarmStatus isn't present it will be added by sync with the most
				// resent state. If the state doesn't match PointService it will remain out of sync until the next sync.
				FMChannelHelper.MakeCall<IPointTagAlarmStatuses>(x => x.AddModifyAlarmStatuses(this.security, pointTagAlarmStatusList, false, true));
			}

			return responseHeader;
		}

		protected PointTagAlarmStatus GetAlarmStatus(Alarm alarm, out bool normalState)
		{
			normalState = false;
			var tagGuids = new List<Guid>
				{
					 alarm.AlarmStateTagGuid
				};
			var tags = FMChannelHelper.MakeCall<IPointServiceManager, List<PointTag>>(x => x.GetPointTagDataWithoutPointAccess(this.security, tagGuids));
			if (tags != null && tags.Count == 1)
			{
				var alarmStateTag = tags[0];
				if (alarm.NotAlarmState != (string)alarmStateTag.Value)
				{
					foreach (var alarmTest in alarm.AlarmTests.Values)
					{
						if (alarmTest.AlarmState == (string)alarmStateTag.Value)
						{
							foreach (var alarmStatus in alarm.AlarmStatus.Values)
							{
								if (alarmStatus.AlarmTestGuid == alarmTest.AlarmTestGuid)
								{
									return alarmStatus;
								}
							}
						}
					}
				}
				else
				{
					normalState = true;
				}
			}
			return null;
		}


		protected Alarm GetAlarm(Guid pointGuid, Guid tagGuid, Guid alarmGuid)
		{
			Point alarmPoint = null;
			lock (this.nodeManager)
			{
				this.nodeManager.PointDictionary.TryGetValue(pointGuid, out alarmPoint);
			}

			if (alarmPoint == null)
			{
				alarmPoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.security, pointGuid));
			}
			if (alarmPoint == null || alarmPoint.IdentityGuid == Guid.Empty || alarmPoint.Tags == null || alarmPoint.Tags.Any() == false)
			{
				return null;
			}
			PointTag tag = null;
			if (alarmPoint.Tags.TryGetValue(tagGuid, out tag) == false)
			{
				return null;
			}
			if (tag == null || tag.IdentityGuid == Guid.Empty || tag.Alarms == null || tag.Alarms.Any() == false)
			{
				return null;
			}
			Alarm alarm;
			if (tag.Alarms.TryGetValue(alarmGuid, out alarm))
			{
				return alarm;
			}
			return null;
		}

		public override ResponseHeader Read(
				RequestHeader requestHeader,
				double maxAge,
				TimestampsToReturn timestampsToReturn,
				ReadValueIdCollection nodesToRead,
				out DataValueCollection results,
				out DiagnosticInfoCollection diagnosticInfos)
		{
			results = new DataValueCollection();
			diagnosticInfos = new DiagnosticInfoCollection();
			var responseHeader = new ResponseHeader();
			responseHeader.RequestHandle = requestHeader.RequestHandle;

			foreach (var readValueID in nodesToRead)
			{

				if (readValueID.NodeId.NamespaceIndex < 2)
				{
					DataValueCollection intermediateResults = null;
					DiagnosticInfoCollection intermediateDiagnosticInfos = null;

					responseHeader = base.Read(
							requestHeader,
							maxAge,
							timestampsToReturn,
							new ReadValueIdCollection() { readValueID },
							out intermediateResults,
							out intermediateDiagnosticInfos);


					if (intermediateResults != null)
					{
						foreach (var dataValue in intermediateResults)
						{
							results.Add(dataValue);

							if (dataValue.Value != null
							|| intermediateDiagnosticInfos == null)
							{
								diagnosticInfos.Add(null);
							}
						}
					}

					if (intermediateDiagnosticInfos != null)
					{
						foreach (var diagnosticInfo in intermediateDiagnosticInfos)
						{
							diagnosticInfos.Add(diagnosticInfo);
						}
					}
				}
				else
				{
					if (!(readValueID.NodeId.Identifier is string))
					{
						results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
						diagnosticInfos.Add(null);
					}
					else
					{
						string identifier = (string)readValueID.NodeId.Identifier;
						SiteClass site = null;
						Point point = null;
						PointValue pointValue = null;
						Alarm alarmMonitor = null;
						Alarm acknowledge = null;
						Alarm ackInputs = null;
						Alarm ackOutputs = null;
						Alarm acknowledged = null;


						if (PointManager.IsSiteNodeID(identifier))
						{
							Guid guid;
							PointManager.ParseSiteNodeID(identifier, out guid);
							lock (this.nodeManager.Lock)
							{
								this.nodeManager.SiteDictionary.TryGetValue(guid, out site);
							}
						}

						else if (PointManager.IsPointNodeID(identifier))
						{
							Guid guid;
							string propertyID;
							PointManager.ParsePointNodeID(identifier, out guid, out propertyID);
							lock (this.nodeManager.Lock)
							{
								this.nodeManager.PointDictionary.TryGetValue(guid, out point);
							}

							if (point == null)
							{
								point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.security, guid));
								if (point.IdentityGuid == Guid.Empty)
								{
									pointValue = null;
								}
							}

							if (!string.IsNullOrEmpty(propertyID))
							{
								var pointValueIdentifier = new PointValueIdentifier(guid, PointValueType.Point, propertyID);
								pointValue = new PointValue(pointValueIdentifier, point);
								point = null;
							}
						}

						else if (PointManager.IsTagNodeID(identifier))
						{
							Guid guid;
							PointManager.ParseTagNodeID(identifier, out guid);
							lock (this.nodeManager.Lock)
							{
								this.nodeManager.PointValueDictionary.TryGetValue(guid, out pointValue);
							}

							if (pointValue == null)
							{
								pointValue = new PointValue(FMChannelHelper.MakeCall<IPointTags, PointTag>(x => x.Get(this.security, guid)));
								if (pointValue.PointValueIdentifier.IdentityGuid == Guid.Empty)
								{
									pointValue = null;
								}
							}
						}

						else if (PointManager.IsSettingNodeID(identifier))
						{
							Guid pointGuid, guid;
							PointProperty property = null;
							string propertyID;
							PointManager.ParseSettingNodeID(identifier, out pointGuid, out guid, out propertyID);
							lock (this.nodeManager.Lock)
							{
								this.nodeManager.PointDictionary.TryGetValue(pointGuid, out point);
							}
							if (point == null)
							{
								point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.security, pointGuid));
								if (point.IdentityGuid == Guid.Empty)
								{
									point = null;
									property = null;
									pointValue = null;
								}
							}

							if (point != null)
							{
								point.Properties.TryGetValue(guid, out property);
								var pointValueIdentifier = new PointValueIdentifier(guid, PointValueType.Setting, propertyID);
								pointValue = new PointValue(pointValueIdentifier, property, point);
								point = null;
							}
						}

						else if (PointManager.IsAlarmSourceTagNodeID(identifier))
						{
							Guid pointGuid, guid;
							PointManager.ParseAlarmSourceTagNodeID(identifier, out pointGuid, out guid);


							this.nodeManager.PointValueDictionary.TryGetValue(guid, out pointValue);
							if (pointValue == null)
							{
								pointValue = new PointValue(FMChannelHelper.MakeCall<IPointTags, PointTag>(x => x.Get(this.security, guid)));
								if (pointValue.PointValueIdentifier.IdentityGuid == Guid.Empty)
								{
									pointValue = null;
								}
							}
						}

						else if (PointManager.IsAlarmMonitorNodeID(identifier))
						{
							Guid pointGuid, tagGuid, alarmGuid;
							PointManager.ParseAlarmMonitorNodeID(identifier, out pointGuid, out tagGuid, out alarmGuid);
							alarmMonitor = this.GetAlarm(pointGuid, tagGuid, alarmGuid);
						}

						else if (PointManager.IsAcknowledgeNodeID(identifier))
						{
							Guid pointGuid, tagGuid, alarmGuid;
							PointManager.ParseAcknowledgeNodeID(identifier, out pointGuid, out tagGuid, out alarmGuid);
							acknowledge = this.GetAlarm(pointGuid, tagGuid, alarmGuid);
						}

						else if (PointManager.IsAcknowledgedNodeID(identifier))
						{
							Guid pointGuid, tagGuid, alarmGuid;
							PointManager.ParseAcknowledgedNodeID(identifier, out pointGuid, out tagGuid, out alarmGuid);
							acknowledged = this.GetAlarm(pointGuid, tagGuid, alarmGuid);
						}

						else if (PointManager.IsAckInputsNodeID(identifier))
						{
							Guid pointGuid, tagGuid, alarmGuid;
							PointManager.ParseAckInputsNodeID(identifier, out pointGuid, out tagGuid, out alarmGuid);
							ackInputs = this.GetAlarm(pointGuid, tagGuid, alarmGuid);
						}

						else if (PointManager.IsAckOutputsNodeID(identifier))
						{
							Guid pointGuid, tagGuid, alarmGuid;
							PointManager.ParseAckOutputsNodeID(identifier, out pointGuid, out tagGuid, out alarmGuid);
							ackOutputs = this.GetAlarm(pointGuid, tagGuid, alarmGuid);
						}
						if (readValueID.AttributeId <= Attributes.AccessRestrictions)
						{
							switch (readValueID.AttributeId)
							{
								case Attributes.ValueRank:
									{
										if (acknowledged != null || pointValue != null)
										{
											results.Add(new DataValue(new Variant(ValueRanks.Scalar)));
											diagnosticInfos.Add(null);
										}
										else if (ackInputs != null || ackOutputs != null)
										{
											results.Add(new DataValue(new Variant(ValueRanks.OneDimension)));
											diagnosticInfos.Add(null);
										}
										else
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}

								case Attributes.ArrayDimensions:
									{
										if (acknowledged != null || pointValue != null)
										{
											var arrayDimensions = new UInt32[0];
											results.Add(new DataValue(new Variant(arrayDimensions)));
											diagnosticInfos.Add(null);
										}
										else if (ackInputs != null || ackOutputs != null)
										{
											results.Add(new DataValue(0));
											diagnosticInfos.Add(null);
										}
										else
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}

								case Attributes.Historizing:
									{
										if (acknowledged != null || pointValue != null)
										{
											results.Add(new DataValue(new Variant(true)));
											diagnosticInfos.Add(null);
										}
										else if (ackInputs != null || ackOutputs != null)
										{
											results.Add(new DataValue(new Variant(false)));
											diagnosticInfos.Add(null);
										}
										else
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}

								case Attributes.NodeId:
									{
										results.Add(new DataValue(new Variant(readValueID.NodeId)));
										diagnosticInfos.Add(null);
										break;
									}

								case Attributes.NodeClass:
									{
										if (site != null)
										{
											results.Add(new DataValue(new Variant(NodeClass.Object)));
											diagnosticInfos.Add(null);
										}

										else if (point != null && pointValue == null)
										{
											results.Add(new DataValue(new Variant(NodeClass.Object)));
											diagnosticInfos.Add(null);
										}

										else if (alarmMonitor != null)
										{
											results.Add(new DataValue(new Variant(NodeClass.Object)));
											diagnosticInfos.Add(null);
										}

										else if (acknowledge != null)
										{
											results.Add(new DataValue(new Variant(NodeClass.Method)));
											diagnosticInfos.Add(null);
										}

										else if (ackInputs != null || ackOutputs != null)
										{
											results.Add(new DataValue(new Variant(NodeClass.Variable)));
											diagnosticInfos.Add(null);
										}
										else if (acknowledged != null || pointValue != null)
										{
											results.Add(new DataValue(new Variant(NodeClass.Variable)));
											diagnosticInfos.Add(null);
										}
										else
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}

								case Attributes.BrowseName:
								case Attributes.DisplayName:
								case Attributes.Description:
									{
										if (site != null)
										{
											if (readValueID.AttributeId == Attributes.BrowseName)
											{
												results.Add(new DataValue(new Variant(new QualifiedName(site.ID, readValueID.NodeId.NamespaceIndex))));
												diagnosticInfos.Add(null);
											}
											else
											{
												results.Add(new DataValue(new Variant(new LocalizedText(site.ID))));
												diagnosticInfos.Add(null);
											}
										}

										else if (point != null && pointValue == null)
										{
											if (readValueID.AttributeId == Attributes.BrowseName)
											{
												results.Add(new DataValue(new Variant(new QualifiedName(point.ID, readValueID.NodeId.NamespaceIndex))));
												diagnosticInfos.Add(null);
											}
											else if (readValueID.AttributeId == Attributes.DisplayName)
											{
												results.Add(new DataValue(new Variant(new LocalizedText(point.ID))));
												diagnosticInfos.Add(null);
											}
											else if (readValueID.AttributeId == Attributes.Description)
											{
												results.Add(new DataValue(new Variant(new LocalizedText(point.Description))));
												diagnosticInfos.Add(null);
											}
											else
											{
												results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
												diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
											}
										}

										else if (alarmMonitor != null)
										{
											if (readValueID.AttributeId == Attributes.BrowseName)
											{
												results.Add(new DataValue(new Variant(new QualifiedName(alarmMonitor.ID + " Alarm Monitor", readValueID.NodeId.NamespaceIndex))));
												diagnosticInfos.Add(null);
											}
											else if (readValueID.AttributeId == Attributes.DisplayName)
											{
												results.Add(new DataValue(new Variant(new LocalizedText(alarmMonitor.ID + " Alarm Monitor"))));
												diagnosticInfos.Add(null);
											}
											else if (readValueID.AttributeId == Attributes.Description)
											{
												results.Add(new DataValue(new Variant(new LocalizedText(""))));
												diagnosticInfos.Add(null);
											}
											else
											{
												results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
												diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
											}
										}

										else if (acknowledge != null)
										{
											if (readValueID.AttributeId == Attributes.BrowseName)
											{
												results.Add(new DataValue(new Variant(new QualifiedName("Acknowledge", readValueID.NodeId.NamespaceIndex))));
												diagnosticInfos.Add(null);
											}
											else if (readValueID.AttributeId == Attributes.DisplayName)
											{
												results.Add(new DataValue(new Variant(new LocalizedText("Acknowledge"))));
												diagnosticInfos.Add(null);
											}
											else if (readValueID.AttributeId == Attributes.Description)
											{
												results.Add(new DataValue(new Variant(new LocalizedText("Acknowledge an Alarm"))));
												diagnosticInfos.Add(null);
											}
											else
											{
												results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
												diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
											}
										}

										else if (ackInputs != null)
										{
											if (readValueID.AttributeId == Attributes.BrowseName)
											{
												results.Add(new DataValue(new Variant(new QualifiedName("InputArguments", readValueID.NodeId.NamespaceIndex))));
												diagnosticInfos.Add(null);
											}
											else if (readValueID.AttributeId == Attributes.DisplayName)
											{
												results.Add(new DataValue(new Variant(new LocalizedText("InputArguments"))));
												diagnosticInfos.Add(null);
											}
											else if (readValueID.AttributeId == Attributes.Description)
											{
												results.Add(new DataValue(new Variant(new LocalizedText(""))));
												diagnosticInfos.Add(null);
											}
											else
											{
												results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
												diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
											}
										}

										else if (ackOutputs != null)
										{
											if (readValueID.AttributeId == Attributes.BrowseName)
											{
												results.Add(new DataValue(new Variant(new QualifiedName("OutputArguments", readValueID.NodeId.NamespaceIndex))));
												diagnosticInfos.Add(null);
											}
											else if (readValueID.AttributeId == Attributes.DisplayName)
											{
												results.Add(new DataValue(new Variant(new LocalizedText("OutputArguments"))));
												diagnosticInfos.Add(null);
											}
											else if (readValueID.AttributeId == Attributes.Description)
											{
												results.Add(new DataValue(new Variant(new LocalizedText(""))));
												diagnosticInfos.Add(null);
											}
											else
											{
												results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
												diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
											}
										}

										else if (pointValue != null)
										{
											if (readValueID.AttributeId == Attributes.BrowseName)
											{
												results.Add(new DataValue(new Variant(new QualifiedName(pointValue.ID, readValueID.NodeId.NamespaceIndex))));
												diagnosticInfos.Add(null);
											}
											else if (readValueID.AttributeId == Attributes.DisplayName)
											{
												results.Add(new DataValue(new Variant(new LocalizedText(pointValue.ID))));
												diagnosticInfos.Add(null);
											}
											else if (readValueID.AttributeId == Attributes.Description)
											{
												results.Add(new DataValue(new Variant(new LocalizedText(""))));
												diagnosticInfos.Add(null);
											}
											else
											{
												results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
												diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
											}
										}

										else if (acknowledged != null)
										{
											if (readValueID.AttributeId == Attributes.BrowseName)
											{
												results.Add(new DataValue(new Variant(new QualifiedName("Acknowledged", readValueID.NodeId.NamespaceIndex))));
												diagnosticInfos.Add(null);
											}
											else if (readValueID.AttributeId == Attributes.DisplayName)
											{
												results.Add(new DataValue(new Variant(new LocalizedText("Acknowledged"))));
												diagnosticInfos.Add(null);
											}
											else if (readValueID.AttributeId == Attributes.Description)
											{
												results.Add(new DataValue(new Variant(new LocalizedText(""))));
												diagnosticInfos.Add(null);
											}
											else
											{
												results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
												diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
											}
										}

										else
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}

								case Attributes.WriteMask:
									{
										results.Add(new DataValue(new Variant((uint)0)));
										diagnosticInfos.Add(null);
										break;
									}

								case Attributes.UserWriteMask:
									{
										results.Add(new DataValue(new Variant((uint)0)));
										diagnosticInfos.Add(null);
										break;
									}

								case Attributes.IsAbstract:
									{
										if (acknowledge != null || ackInputs != null || ackOutputs != null)
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										else
										{
											results.Add(new DataValue(StatusCodes.BadNodeIdUnknown));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}

								case Attributes.Symmetric:
									{
										if (acknowledge != null || ackInputs != null || ackOutputs != null)
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										else
										{
											results.Add(new DataValue(StatusCodes.BadNodeIdUnknown));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}

								case Attributes.InverseName:
									{
										if (acknowledge != null || ackInputs != null || ackOutputs != null)
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										else
										{
											results.Add(new DataValue(StatusCodes.BadNodeIdUnknown));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}

								case Attributes.ContainsNoLoops:
									{
										if (acknowledge != null || ackInputs != null || ackOutputs != null)
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										else
										{
											results.Add(new DataValue(StatusCodes.BadNodeIdUnknown));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}


								case Attributes.Value:
									{
										if (pointValue != null)
										{
											var pointValueIdentifiers = new List<PointValueIdentifier>();
											pointValueIdentifiers.Add(pointValue.PointValueIdentifier);
											var pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.security, pointValueIdentifiers, false));
											if (pointValues != null
											&& pointValues.Count == 1)
											{
												pointValue = pointValues[0];
												if (pointValue.Value == null
												|| pointValue.Value is ValueType)
												{
													if (pointValue.ValueTypeString == "System.DateTimeOffset"
													&& pointValue.Value != null)
													{
														results.Add(new DataValue(new Variant(((DateTimeOffset)pointValue.Value).UtcDateTime)));
													}
													else if (pointValue.ValueTypeString == "System.TimeSpan"
													&& pointValue.Value != null)
													{
														results.Add(new DataValue(new Variant(((TimeSpan)pointValue.Value).Ticks)));
													}
													else
													{
														results.Add(new DataValue(new Variant(pointValue.Value)));
													}
												}
												else
												{
													if (pointValue.Value is string)
													{
														results.Add(new DataValue(new Variant(pointValue.Value)));
													}
													else if (pointValue.Value is PointCommandStatusListReference)
													{
														var pcslr = pointValue.Value as PointCommandStatusListReference;
														if (pcslr.CurrentValue.HasValue)
														{
															results.Add(new DataValue(new Variant(pcslr.CurrentValue)));
														}
														else
														{
															results.Add(new DataValue(StatusCodes.BadNotReadable));
														}
													}
													else if (pointValue.Value is DeviceAlarmMapReference)
													{
														var damr = pointValue.Value as DeviceAlarmMapReference;
														if (damr.CurrentValue.HasValue)
														{
															results.Add(new DataValue(new Variant((ushort)damr.CurrentValue)));
														}
														else
														{
															results.Add(new DataValue(StatusCodes.BadNotReadable));
														}
													}
												}
												results[results.Count - 1].StatusCode = new StatusCode((uint)pointValue.Status);
												results[results.Count - 1].ServerTimestamp = new DateTime(pointValue.ServerTimeStamp.Ticks, DateTimeKind.Utc);
												results[results.Count - 1].SourceTimestamp = new DateTime(pointValue.SourceTimeStamp.Ticks, DateTimeKind.Utc);
												diagnosticInfos.Add(null);
											}
											else
											{
												results.Add(new DataValue(StatusCodes.BadNotReadable));
												diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
											}
										}

										else if (acknowledged != null)
										{
											bool normalState = false;
											PointTagAlarmStatus alarmStatus = GetAlarmStatus(acknowledged, out normalState);
											if (normalState)
											{
												bool normalAcknowledge = false;
												var ackTimestamp = DateTime.UtcNow;
												results.Add(new DataValue(new Variant(normalAcknowledge)));
												results[results.Count - 1].StatusCode = new StatusCode(StatusCodes.Good);
												results[results.Count - 1].ServerTimestamp = ackTimestamp;
												results[results.Count - 1].SourceTimestamp = ackTimestamp;
												diagnosticInfos.Add(null);
											}
											else
											{
												if (alarmStatus != null)
												{
													var ackTimestamp = alarmStatus.AcknowledgedTimestamp?.DateTime ?? DateTime.UtcNow;
													results.Add(new DataValue(new Variant(alarmStatus.Acknowledged)));
													results[results.Count - 1].StatusCode = new StatusCode(StatusCodes.Good);
													results[results.Count - 1].ServerTimestamp = ackTimestamp;
													results[results.Count - 1].SourceTimestamp = ackTimestamp;
													diagnosticInfos.Add(null);
												}
												else
												{
													results.Add(new DataValue(StatusCodes.BadNotReadable));
													diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
												}
											}
										}
										else if (ackInputs != null)
										{
											var inputVal = new Argument[0];


											results.Add(new DataValue(inputVal));
											results[results.Count - 1].StatusCode = StatusCodes.Good;
											results[results.Count - 1].ServerTimestamp = DateTime.Now;
											results[results.Count - 1].SourceTimestamp = DateTime.Now;
											diagnosticInfos.Add(null);
										}
										else if (ackOutputs != null)
										{
											var outputVal = new Argument[0];
											results.Add(new DataValue(outputVal));
											results[results.Count - 1].StatusCode = StatusCodes.Good;
											results[results.Count - 1].ServerTimestamp = DateTime.Now;
											results[results.Count - 1].SourceTimestamp = DateTime.Now;
											diagnosticInfos.Add(null);
										}
										else if (PointManager.IsDefinitionNodeID(identifier))
										{
											results.Add(new DataValue(StatusCodes.BadNodeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										else
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}

								case Attributes.EventNotifier:
									{
										if (acknowledge == null && ackInputs == null && ackOutputs == null && acknowledged == null && pointValue == null)
										{
											results.Add(new DataValue(new Variant(EventNotifiers.None)));
											diagnosticInfos.Add(null);
										}
										else
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}

								case Attributes.DataType:
									{
										if (pointValue != null)
										{
											Type type = null;

											if (pointValue.ValueTypeString.IndexOf("FMBusinessObjects") != -1)
											{
												type = Type.GetType(pointValue.ValueTypeString + ",FMBusinessObjects");
											}
											else
											{
												type = Type.GetType(pointValue.ValueTypeString);

											}
											results.Add(new DataValue(new Variant(PointManager.ConvertTypeToDataTypeId(type))));
											diagnosticInfos.Add(null);
										}
										else if (acknowledged != null)
										{
											results.Add(new DataValue(new Variant(DataTypeIds.Boolean)));
											diagnosticInfos.Add(null);
										}
										else if (ackInputs != null)
										{
											results.Add(new DataValue(new Variant(DataTypeIds.Argument)));
											diagnosticInfos.Add(null);
										}
										else if (ackOutputs != null)
										{
											results.Add(new DataValue(new Variant(DataTypeIds.Argument)));
											diagnosticInfos.Add(null);
										}
										else
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}

								case Attributes.AccessLevel:
									{
										if (pointValue != null)
										{
											if (pointValue.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual
											|| !pointValue.Input)
											{
												results.Add(new DataValue(new Variant((byte)(AccessLevels.CurrentReadOrWrite | AccessLevels.HistoryRead))));
												diagnosticInfos.Add(null);
											}
											else
											{
												results.Add(new DataValue(new Variant((byte)(AccessLevels.CurrentRead | AccessLevels.HistoryRead))));
												diagnosticInfos.Add(null);
											}
										}

										else if (acknowledged != null)
										{
											results.Add(new DataValue(new Variant((byte)(AccessLevels.CurrentRead | AccessLevels.HistoryRead))));
											diagnosticInfos.Add(null);
										}
										else if (ackInputs != null || ackOutputs != null)
										{
											results.Add(new DataValue(new Variant((byte)(AccessLevels.CurrentRead))));
											diagnosticInfos.Add(null);
										}
										else
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}

								case Attributes.UserAccessLevel:
									{
										if (pointValue != null)
										{
											if (pointValue.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual
											|| !pointValue.Input)
											{
												results.Add(new DataValue(new Variant((byte)(AccessLevels.CurrentReadOrWrite | AccessLevels.HistoryRead))));
												diagnosticInfos.Add(null);
											}
											else
											{
												results.Add(new DataValue(new Variant((byte)(AccessLevels.CurrentRead | AccessLevels.HistoryRead))));
												diagnosticInfos.Add(null);
											}
										}
										else if (acknowledged != null)
										{
											results.Add(new DataValue(new Variant((byte)(AccessLevels.CurrentRead | AccessLevels.HistoryRead))));
											diagnosticInfos.Add(null);
										}
										else if (ackOutputs != null || ackInputs != null)
										{
											results.Add(new DataValue(new Variant((byte)(AccessLevels.CurrentRead))));
											diagnosticInfos.Add(null);
										}
										else
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}

								case Attributes.Executable:
									{
										if (acknowledge != null)
										{
											//Not the right values
											results.Add(new DataValue(new Variant((bool)(true))));
											diagnosticInfos.Add(null);
										}
										else if (ackInputs != null || ackOutputs != null)
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										else
										{
											results.Add(new DataValue(StatusCodes.BadNodeIdUnknown));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}

								case Attributes.UserExecutable:
									{
										if (acknowledge != null)
										{
											results.Add(new DataValue(new Variant((bool)(true))));
											diagnosticInfos.Add(null);
										}
										else if (ackInputs != null || ackOutputs != null)
										{
											results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										else
										{
											results.Add(new DataValue(StatusCodes.BadNodeIdUnknown));
											diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
										}
										break;
									}

								case Attributes.MinimumSamplingInterval:
									{
										if (ackInputs != null || ackOutputs != null)
										{
											results.Add(new DataValue(new Variant(0)));
											diagnosticInfos.Add(null);
										}
										else
										{
											results.Add(new DataValue(new Variant((uint)1000)));
											diagnosticInfos.Add(null);
										}
										break;
									}
								case Attributes.RolePermissions:
									{
										results.Add(null);
										diagnosticInfos.Add(null);
										break;
									}

								case Attributes.UserRolePermissions:
									{
										results.Add(null);
										diagnosticInfos.Add(null);
										break;
									}

								case Attributes.AccessRestrictions:
									{
										results.Add(null);
										diagnosticInfos.Add(null);
										break;
									}



								default:
									results.Add(new DataValue(StatusCodes.BadAttributeIdInvalid));
									diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
									break;
							}
						}
						else
						{
							results.Add(new DataValue(StatusCodes.BadNodeIdUnknown));
							diagnosticInfos.Add(new DiagnosticInfo(-1, -1, -1, 0, null));
						}
					}
				}
			}

			return responseHeader;
		}

		protected override void OnServerStarted(IServerInternal server)
		{
			base.OnServerStarted(server);

			shutdownEvent.Reset();
			updateThread = new Thread(new ParameterizedThreadStart(OpcUaUpdateNodes));
			updateThread.IsBackground = true;
			updateThread.Name = "Publishing";
			updateThread.Start(Configuration.ServerConfiguration.PublishingResolution);

			// request notifications when the user identity is changed. all valid users are accepted by default.
			server.SessionManager.ImpersonateUser += new ImpersonateEventHandler(SessionManager_ImpersonateUser);
		}

		protected override void OnServerStopping()
		{
			shutdownEvent.Set();
			updateThread = null;

			base.OnServerStopping();

			foreach (var evPointValueDictionary in evPointValueDictionaries.Values)
			{
				this.SetPointValuesUncertain(evPointValueDictionary);
			}

			foreach (var evPointPropertyDictionary in evPointPropertyDictionaries.Values)
			{
				this.SetPointPropertiesUncertain(evPointPropertyDictionary);
			}
		}

		private void SessionManager_ImpersonateUser(Session session, ImpersonateEventArgs args)
		{

			// check for a user name token.
			UserNameIdentityToken userNameToken = args.NewIdentity as UserNameIdentityToken;

			if (userNameToken != null)
			{
				bool valid = false;
				using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
				{
					valid = context.ValidateCredentials(userNameToken.UserName, userNameToken.DecryptedPassword);
					if (!valid)
					{
						throw new Exception();
					}

					args.Identity = new UserIdentity(userNameToken);
					Utils.Trace("UserAuthenticationServer.SessionManager_ImpersonateUser", String.Format("UserName Token Accepted: {0}", args.Identity.DisplayName));
				}
			}
		}

		protected override SessionManager CreateSessionManager(IServerInternal server, ApplicationConfiguration configuration)
		{
			this.sessionManager = base.CreateSessionManager(server, configuration);

			this.sessionManager.SessionCreated += SessionManager_SessionCreated;
			this.sessionManager.SessionClosing += SessionManager_SessionClosing;

			return this.sessionManager;
		}

		private void SessionManager_SessionCreated(Session session, SessionEventReason reason)
		{
			if (session.SessionDiagnostics.ClientDescription.ApplicationName == "FMPointService EnterpriseVisibility")
			{
				lock (base.Lock)
				{

					if (this.evDictionary.ContainsKey(session.Id))
					{
						if (evPointValueDictionaries.ContainsKey(this.evDictionary[session.Id]))
						{
							evPointValueDictionaries.Remove(this.evDictionary[session.Id]);
						}

						this.evDictionary[session.Id] = session.SessionDiagnostics.ClientDescription.ApplicationUri;
					}
					else
					{
						this.evDictionary.Add(session.Id, session.SessionDiagnostics.ClientDescription.ApplicationUri);
					}
				}
			}
		}

		private void SetPointPropertiesUncertain(Dictionary<Guid, PointProperty> evPointPropertyDictionary)
		{
			foreach (var pointProperty in evPointPropertyDictionary.Values)
			{
				if (pointProperty == null
				|| !(pointProperty.Value is MovementData))
				{
					continue;
				}

				if (pointProperty.Value is MovementData)
				{

					Type movementDataType = typeof(MovementData);
					IList<PropertyInfo> propertyInfoList = new List<PropertyInfo>(movementDataType.GetProperties());

					foreach (PropertyInfo propertyInfo in propertyInfoList)
					{
						object pointValueList = propertyInfo.GetValue(pointProperty.Value, null);

						if (pointValueList is List<PointValue>)
						{

							foreach (var pointValue in pointValueList as List<PointValue>)
							{

								if (pointValue == null)
								{
									continue;
								}


								pointValue.Status = (pointValue.Status & 0xFFFF) | StatusCodes.UncertainNoCommunicationLastUsableValue;
								pointValue.ServerTimeStamp = DateTimeOffset.UtcNow;
								pointValue.SourceTimeStamp = DateTimeOffset.UtcNow;
							}
						}
					}

					pointProperty.UpdatedDate = DateTimeOffset.Now;
					FMChannelHelper.MakeCall<IPointProperties>(x => x.ModifyPointPropertyValue(this.security, pointProperty, true, true));
				}
			}
		}

		private void SetPointValuesUncertain(Dictionary<Guid, PointValue> evPointValueDictionary)
		{
			int numberOfTagsPerSend = 4096;
			try
			{
				string numTagsPerSendStr = FMChannelHelper.MakeCall<IConfigurationSettings, string>(configSettingsChannel => configSettingsChannel.GetKeyValueByKey(security,
					ConfigurationSettingDOClass.Key_EnterpriseVisibilityTagsPerCall));
				numberOfTagsPerSend = Convert.ToInt32(numTagsPerSendStr);
			}
			catch (Exception ex)
			{
				Utils.Trace("SetPointValues", String.Format("Cannot read " + ConfigurationSettingDOClass.Key_EnterpriseVisibilityTagsPerCall));
			}

			var pointValueList = new List<PointValue>(numberOfTagsPerSend);

			foreach (var pointValue in evPointValueDictionary.Values)
			{
				if(pointValue == null)
				{
					continue;
				}

				pointValue.Status = (pointValue.Status & 0xFFFF) | StatusCodes.UncertainNoCommunicationLastUsableValue;
				pointValue.ServerTimeStamp = DateTimeOffset.UtcNow;
				pointValue.SourceTimeStamp = DateTimeOffset.UtcNow;

				pointValueList.Add(pointValue);

				if (pointValueList.Count >= numberOfTagsPerSend)
				{
					FMChannelHelper.MakeCall<IPointServiceManager>(x => x.SetPointValueData(this.security, pointValueList, true));
					pointValueList.Clear();
				}
			}

			if (pointValueList.Count > 0)
			{
				FMChannelHelper.MakeCall<IPointServiceManager>(x => x.SetPointValueData(this.security, pointValueList, true));
			}

		}

		private void SessionManager_SessionClosing(Session session, SessionEventReason reason)
		{
			try
			{
				Dictionary<Guid, PointValue> evPointValueDictionary = null;
				Dictionary<Guid, PointProperty> evPointPropertyDictionary = null;

				lock (base.Lock)
				{
					if (evDictionary.ContainsKey(session.Id))
					{
						string applicatoinUri = evDictionary[session.Id];
						evDictionary.Remove(session.Id);

						if (evPointValueDictionaries.ContainsKey(applicatoinUri))
						{
							evPointValueDictionary = evPointValueDictionaries[applicatoinUri];

							evPointValueDictionaries.Remove(applicatoinUri);
						}

						if (evPointPropertyDictionaries.ContainsKey(applicatoinUri))
						{
							evPointPropertyDictionary = evPointPropertyDictionaries[applicatoinUri];

							evPointPropertyDictionaries.Remove(applicatoinUri);
						}
					}
				}

				if (evPointValueDictionary != null)
				{
					this.SetPointValuesUncertain(evPointValueDictionary);
				}

				if (evPointPropertyDictionary != null)
				{
					this.SetPointPropertiesUncertain(evPointPropertyDictionary);
				}


			}
			catch (Exception ex)
			{
				Utils.Trace("SessionManager_SessionClosing : ", ex.Message);
			}

		}
		#endregion
	}
}