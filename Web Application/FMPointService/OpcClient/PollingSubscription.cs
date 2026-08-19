


namespace FMPointService.OpcClient
{
	using System.Security.Cryptography.X509Certificates;
	using FMBusinessObjects.DataObjects;
	using InProcLogging;
	using Opc.Ua;
	using Softing.Opc.Ua.Client;
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using ThreadSupport;

	public class PollingSubscription : SrmThread
	{
		protected ClientSession session;

		protected Guid siteGuid;

		protected Dictionary<Guid, PointTag> monitoredTags = new Dictionary<Guid, PointTag>();

		protected int pollingPeriodInMilliseconds;

		protected long lastReadTime = 0;

		protected bool enableUseLastKnownGood = false;

		protected Object LockObject = new object();


		public PollingSubscription(UaApplication application, bool enableUseLastKnownGood, PointTag tag)
		{
			this.enableUseLastKnownGood = enableUseLastKnownGood;
			this.session = OpcUaTags.CreateSessionForPointTag(application, tag);
			this.siteGuid = tag.SiteGuid;
			lock (this.LockObject)
			{
				this.monitoredTags.Add(tag.PointTagGuid, tag);
			}

			this.pollingPeriodInMilliseconds = tag.OpcUaPublishingInterval == null ? 30000 : (int)tag.OpcUaPublishingInterval;
		}


		public override void Run()
		{
			try
			{
				while (this.mShutdown != true)
				{
					try
					{
						Thread.Sleep(1000);
						this.ReadValues();
					}
					catch (Exception ex)
					{
						Logger.LogError("PollingSubscription.Run Exception: " + ex);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError("PollingSubscription.Run exception: " + ex);
			}
		}

		protected bool IsTimeForNextRead()
		{
			bool timeForRead = false;
			long currentTime = HighPerformanceTimer.Now;
			if (lastReadTime != 0)
			{
				double pollingPeriodInSecondsDouble;
				if (this.session.CurrentState == State.Disconnected)
				{
					pollingPeriodInSecondsDouble = ((double)(pollingPeriodInMilliseconds * 10)) / 1000.00;
				}
				else
				{
					pollingPeriodInSecondsDouble = ((double)(pollingPeriodInMilliseconds)) / 1000.00;
				}

				long readIntervalTicks = HighPerformanceTimer.convertToTicks(pollingPeriodInSecondsDouble);
				long nextReadTime = lastReadTime + readIntervalTicks;
				var ticksToSleep = nextReadTime - currentTime;
				double sleepTimeDouble = HighPerformanceTimer.convertToSeconds(ticksToSleep) * 1000.00;
				if (sleepTimeDouble < 1000.00)
				{
					lastReadTime = currentTime;
					timeForRead = true;
				}
			}
			else
			{
				lastReadTime = currentTime;
				timeForRead = true;
			}
			return timeForRead;
		}

		public void Disconnect()
		{
			this.session.Disconnect(false);
			this.session.Dispose();
		}

		public int MonitoredTagCount
		{
			get
			{
				return this.monitoredTags.Count;
			}
		}

		public bool DoesSessionHavePointTag(Guid pointTagGuid)
		{
			return monitoredTags.ContainsKey(pointTagGuid);
		}

		public bool IsSessionForPointTag(PointTag pointTag)
		{
			string serverUrl = null;
			MessageSecurityMode securityMode = MessageSecurityMode.None;
			SecurityPolicy securityPolicy = SecurityPolicy.None;
			MessageEncoding messageEncoding = MessageEncoding.Binary;
			UserIdentity userIdentity = new UserIdentity();

			if (pointTag == null || string.IsNullOrEmpty(pointTag.OpcUaServerEndPoint))
				return false;

			serverUrl = pointTag.OpcUaServerEndPoint;
			if (pointTag.OpcUaSecurityMode != null)
			{
				switch (pointTag.OpcUaSecurityMode.ToLower())
				{
					case "none":
							securityMode = MessageSecurityMode.None;
							break;
					case "signandencrypt":
							securityMode = MessageSecurityMode.SignAndEncrypt;
							break;
					default:
							throw new Exception("OpcUaClientProcessor2.CreateSessionForPointTag : Invalid MessageSecurityMode " + pointTag.OpcUaSecurityMode);
				}
			}

			if (pointTag.OpcUaSecurityPolicy != null)
			{
				switch (pointTag.OpcUaSecurityPolicy.ToLower())
				{
					case "none":
						securityPolicy = SecurityPolicy.None;
						break;
					case "basic256":
						securityPolicy = SecurityPolicy.Basic256;
						break;
					case "basic128rsa15":
						securityPolicy = SecurityPolicy.Basic128Rsa15;
						break;
					case "basic256sha256":
						securityPolicy = SecurityPolicy.Basic256Sha256;
						break;
					case "aes128_sha256_rsaoaep":
						securityPolicy = SecurityPolicy.Aes128_Sha256_RsaOaep;
						break;
					case "aes256_sha256_rsapss":
						securityPolicy = SecurityPolicy.Aes256_Sha256_RsaPss;
						break;
					default:
						throw new Exception("OpcUaClientProcessor2.CreateSessionForPointTag : Invalid SecurityPolicy " + pointTag.OpcUaSecurityPolicy);
				}
			}

			if (pointTag.OpcUaMessageEncoding != null)
			{
				switch (pointTag.OpcUaMessageEncoding.ToLower())
				{
					case "binary":
							messageEncoding = MessageEncoding.Binary;
							break;
					case "xml":
							messageEncoding = MessageEncoding.Xml;
							break;
					default:
							throw new Exception("OpcUaClientProcessor2.CreateSessionForPointTag : Invalid MessageEncoding " + pointTag.OpcUaMessageEncoding);
				}
			}

			if (pointTag.OpcUaUserIdentityMethod != null)
			{
				switch (pointTag.OpcUaUserIdentityMethod.ToLower())
				{
					case "anonymous":
							userIdentity = new UserIdentity();
							break;
					case "username":
							//TBD: Need to store the password as encrypted in the database...
							userIdentity = new UserIdentity(pointTag.OpcUaUserId, pointTag.OpcUaUserPassword);
							break;
					case "certificate":
							userIdentity = new UserIdentity(new X509Certificate2(pointTag.OpcUaUserCertificatePath, pointTag.OpcUaUserPassword));
							break;
					default:
							throw new Exception("OpcUaClientProcessor2.CreateSessionForPointTag : Invalid UserIdentityMethod " + pointTag.OpcUaUserIdentityMethod);
				}
			}

			if(this.session.Url == serverUrl
				&& this.session.SecurityMode == securityMode 
				&& this.session.SecurityPolicy.ToString().EndsWith(securityPolicy.ToString())
				&& this.session.Encoding == messageEncoding
				&& this.session.UserIdentity.ToString() == userIdentity.ToString()
				&& this.siteGuid == pointTag.SiteGuid
				&& this.pollingPeriodInMilliseconds == pointTag.OpcUaPublishingInterval)
			{
				return true;
			}

			return false;
		}


		public void RefreshPointTag(PointTag pointTag)
		{
			lock (this.LockObject)
			{
				PointTag currentTag = null;
				if (monitoredTags.TryGetValue(pointTag.IdentityGuid, out currentTag))
				{
					pointTag.Status = currentTag.Status;
					pointTag.Value = currentTag.Value;
					currentTag.ServerTimeStamp = DateTimeOffset.UtcNow;
					pointTag.ServerTimeStamp = currentTag.ServerTimeStamp;
					pointTag.SourceTimeStamp = currentTag.SourceTimeStamp;
				}
			}
		}

		public bool AddPointTag(PointTag tag)
		{
			lock (this.LockObject)
			{
				if (IsSessionForPointTag(tag) == false)
				{
					return false;
				}
				if (monitoredTags.ContainsKey(tag.PointTagGuid))
				{
					return true;
				}
				monitoredTags.Add(tag.PointTagGuid, tag);
				return true;
			}
		}

		public bool RemovePointTag(PointTag tag)
		{
			lock (this.LockObject)
			{
				if (monitoredTags.ContainsKey(tag.PointTagGuid) == false)
				{
					return false;
				}
				monitoredTags.Remove(tag.PointTagGuid);
				if (monitoredTags.Count <= 0)
				{
					this.session.Disconnect(false);
				}
				return true;
			}
		}

		private StatusCode GetAllowedStatusCodesFromPassedInParameter(StatusCode statusCode)
		{
			// this routine removes the unwated status codes being supplied from the opc interface before we set the tag status.
			// example is goodlocaloverride. We do not ever want to set that from and external source
			StatusCode returnedCode = statusCode;
			UInt32 OpcStatusCodeBits = new StatusCode((uint)statusCode.Code).CodeBits;

			if (OpcStatusCodeBits == StatusCodes.GoodLocalOverride)
			{
				returnedCode = new StatusCode(StatusCodes.Good);
			}

			return returnedCode;
		}

		public void ReadValues()
		{
			try
			{
				if (IsTimeForNextRead())
				{
					
					if (this.monitoredTags.Count <= 0)
					{
							Logger.LogError("PollingSubscription.ReadValues trying to read values of a subscription that has none.  Cleanup was not performed properly!!!");
							return;
					}
					var pointTags = new Dictionary<Guid, PointTag>();
					if (this.session.TargetState == State.Disconnected)
					{
						this.session.Connect(false, true);
					}

					if (this.session.CurrentState == State.Active)
					{
						var nodeList = new List<ReadValueId>();

						lock (this.LockObject)
						{

							//There maybe more tags than we desire to do a read at once, so we probably need to develop
							//a mechanism to have a max number of reads per tag.
							foreach (var tag in monitoredTags.Values)
							{
								string nodeIdStr = tag.OpcUaNodeId;
								var nodeId = GetNodeId(nodeIdStr);
								ReadValueId nodeReadValueId = new ReadValueId();
								nodeReadValueId.AttributeId = 13;
								nodeReadValueId.NodeId = nodeId;
								nodeList.Add(nodeReadValueId);
							}
						}

						var dataVals = this.session.Read(nodeList, 86400.00, TimestampsToReturn.Both);
						if (dataVals.Count != nodeList.Count)
						{
							Logger.LogError("PollingSubscription.ReadValues dataVals.Count " + dataVals.Count + " is not equal to nodeList.Count " + nodeList.Count);
							return;
						}

						lock (this.LockObject)
						{
							int dataValIndex = 0;

							foreach (var pointTag in monitoredTags.Values)
							{
								var currentStatusCodeBits = pointTag.OpcStatusCodeBits;
								var recievedStatusCode = GetAllowedStatusCodesFromPassedInParameter(dataVals[dataValIndex].StatusCode);

								object value = dataVals[dataValIndex].Value;

								if (pointTag.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference" &&
									pointTag.Value is PointCommandStatusListReference)
								{
									var currentPointTag = ThreadSharedData.Instance().GetPointTag(pointTag.IdentityGuid);

									// If the pointTag is being processed on this FMPointService instance
									if (currentPointTag != null)
									{
										if ((pointTag.Value as PointCommandStatusListReference).PointCommandStatusListGuid != (currentPointTag.Value as PointCommandStatusListReference).PointCommandStatusListGuid)
										{
											(pointTag.Value as PointCommandStatusListReference).PointCommandStatusListGuid = (currentPointTag.Value as PointCommandStatusListReference).PointCommandStatusListGuid;
										}

										int? intValue;
										string keyValue;
										try
										{
											intValue = new int?(Convert.ToInt32(value));
											keyValue = ThreadSharedData.Instance().GetPointCommandStatusKey(pointTag.PointGuid, (pointTag.Value as PointCommandStatusListReference).PointCommandStatusListGuid, intValue.Value);
										}
										catch (Exception)
										{
											intValue = null;
											keyValue = null;
										}
										value = new PointCommandStatusListReference()
										{
											PointCommandStatusListGuid = (pointTag.Value as PointCommandStatusListReference).PointCommandStatusListGuid,
											CurrentValue = intValue,
											CurrentKey = keyValue
										};
									}
								}
								else
								{
									bool isDeviceAlarmMapReference = pointTag.Value is DeviceAlarmMapReference;

									Guid deviceAlarmMapGuid = isDeviceAlarmMapReference ?
										(pointTag.Value as DeviceAlarmMapReference).DeviceAlarmMapGuid :
										Guid.Empty;

									FMPointCommon.PointManager.ValidatePointTagValueByItsType(pointTag.ValueTypeString,
										ref value, ref recievedStatusCode, isDeviceAlarmMapReference, deviceAlarmMapGuid);
								}

								var recievedStatusCodeBits = recievedStatusCode.Code;

								if ((pointTag.Value == null && value != null)
								|| (pointTag.Value != null && pointTag.Value.Equals(value) == false)
								|| currentStatusCodeBits != recievedStatusCodeBits)

								{
									var type = Type.GetType(pointTag.ValueTypeString);

									// Search for type in FMBusinessObjects
									if (type == null)
									{
										type = Type.GetType(pointTag.ValueTypeString + ", FMBusinessObjects");
									}

									if (type != null)
									{

										if (StatusCode.IsBad(recievedStatusCodeBits))
										{
											if (enableUseLastKnownGood)
											{
												if (pointTag.OpcStatusCodeBits != StatusCodes.UncertainLastUsableValue)
												{
													pointTag.Status = StatusCodes.UncertainLastUsableValue;
													pointTag.ServerTimeStamp = DateTimeOffset.UtcNow;
													pointTag.SourceTimeStamp = dataVals[dataValIndex].SourceTimestamp;
													pointTags.Add(pointTag.PointTagGuid, pointTag);
												}
											}
											else
											{
												if (pointTag.Value is PointCommandStatusListReference)
												{
													(pointTag.Value as PointCommandStatusListReference).CurrentKey = string.Empty;
													(pointTag.Value as PointCommandStatusListReference).CurrentValue = null;
												}
												else if (pointTag.Value is DeviceAlarmMapReference)
												{
													(pointTag.Value as DeviceAlarmMapReference).CurrentValue = null;
												}
												else
												{
													pointTag.Value = null;
												}
												pointTag.Status = recievedStatusCodeBits;
												pointTag.ServerTimeStamp = DateTimeOffset.UtcNow;
												pointTag.SourceTimeStamp = dataVals[dataValIndex].SourceTimestamp;
												pointTags.Add(pointTag.PointTagGuid, pointTag);
											}
										}
										else
										{
											pointTag.Value = value;
											pointTag.Status = recievedStatusCodeBits;
											pointTag.ServerTimeStamp = DateTimeOffset.UtcNow;
											pointTag.SourceTimeStamp = dataVals[dataValIndex].SourceTimestamp;
											pointTags.Add(pointTag.PointTagGuid, pointTag);
										}
									}
								}
								dataValIndex++;
							}
						}
					}
					else
					{
						lock (this.LockObject)
						{
							int dataValIndex = 0;

							foreach (var pointTag in monitoredTags.Values)
							{
								bool statusChanged = false;

								if (enableUseLastKnownGood)
								{
									if (!pointTag.IsBad()
									&& pointTag.Value != null)
									{
										if (pointTag.OpcStatusCodeBits != StatusCodes.UncertainLastUsableValue)
										{
											statusChanged = true;
											pointTag.Status = StatusCodes.UncertainLastUsableValue;
										}
									}
									else
									{
										if (pointTag.OpcStatusCodeBits != StatusCodes.BadSessionNotActivated)
										{
											statusChanged = true;
											pointTag.Status = StatusCodes.BadSessionNotActivated;
										}
									}
								}
								else
								{
									if (!pointTag.IsBad())
									{
										if (pointTag.OpcStatusCodeBits != StatusCodes.BadSessionNotActivated)
										{
											statusChanged = true;
											pointTag.Status = StatusCodes.BadSessionNotActivated;
											if (pointTag.Value is PointCommandStatusListReference)
											{
												(pointTag.Value as PointCommandStatusListReference).CurrentKey = string.Empty;
												(pointTag.Value as PointCommandStatusListReference).CurrentValue = null;
											}
											else if (pointTag.Value is DeviceAlarmMapReference)
											{
												(pointTag.Value as DeviceAlarmMapReference).CurrentValue = null;
											}
											else
											{
												pointTag.Value = null;
											}
										}
									}
								}

								if (statusChanged)
								{
									pointTag.ServerTimeStamp = DateTimeOffset.UtcNow;
									pointTag.SourceTimeStamp = DateTimeOffset.UtcNow;
									pointTags.Add(pointTag.PointTagGuid, pointTag);
								}

								dataValIndex++;
							}
						}
					}

					if (pointTags.Count > 0)
					{
						ThreadSharedData.Instance().ExternalUpdateTags(pointTags);
					}
				}
			}
			catch (Exception e)
			{
				Logger.LogError("PollingSubscription.ReadValues Exception " + e.Message);
				if (monitoredTags.Count > 0)
				{
					var pointTags = new Dictionary<Guid, PointTag>();

					lock (this.LockObject)
					{

						int dataValIndex = 0;
						foreach (var pointTag in monitoredTags.Values)
						{
							bool statusChanged = false;

							if (enableUseLastKnownGood)
							{
								if (!pointTag.IsBad()
								&& pointTag.Value != null)
								{
									if (pointTag.OpcStatusCodeBits != StatusCodes.UncertainLastUsableValue)
									{
										statusChanged = true;
										pointTag.Status = StatusCodes.UncertainLastUsableValue;
									}
								}
								else
								{
									if (pointTag.OpcStatusCodeBits != StatusCodes.BadSessionNotActivated)
									{
										statusChanged = true;
										pointTag.Status = StatusCodes.BadSessionNotActivated;
									}
								}
							}
							else
							{

								if (!pointTag.IsBad())
								{
									if (pointTag.OpcStatusCodeBits != StatusCodes.BadSessionNotActivated)
									{
										statusChanged = true;
										pointTag.Status = StatusCodes.BadSessionNotActivated;
										if (pointTag.Value is PointCommandStatusListReference)
										{
											(pointTag.Value as PointCommandStatusListReference).CurrentKey = string.Empty;
											(pointTag.Value as PointCommandStatusListReference).CurrentValue = null;
										}
										else if (pointTag.Value is DeviceAlarmMapReference)
										{
											(pointTag.Value as DeviceAlarmMapReference).CurrentValue = null;
										}
										else
										{
											pointTag.Value = null;
										}
									}
								}
							}

							if (statusChanged)
							{
								pointTag.ServerTimeStamp = DateTimeOffset.UtcNow;
								pointTag.SourceTimeStamp = DateTimeOffset.UtcNow;
								pointTags.Add(pointTag.PointTagGuid, pointTag);
							}

							dataValIndex++;
						}
					}

					if (pointTags.Count > 0)
					{
						ThreadSharedData.Instance().ExternalUpdateTags(pointTags);
					}
				}
			}
		}

		public static NodeId GetNodeId(string nodeIdStr)
		{
			NodeId nodeId = null;
			if (string.IsNullOrEmpty(nodeIdStr))
			{
				return null;
			}

			try
			{
				nodeId = new NodeId(nodeIdStr);
			}
			catch (Exception)
			{
				return null;
			}

			return nodeId;
		}
	}
}
