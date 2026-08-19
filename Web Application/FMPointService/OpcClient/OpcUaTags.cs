namespace FMPointService.OpcClient
{
	using System;
	using System.Collections.Generic;
	using System.Security.Cryptography.X509Certificates;
	using Opc.Ua;
	using Softing.Opc.Ua.Client;
	using FMBusinessObjects.DataObjects;
	using ThreadSupport;


	public class OpcUaTags
	{
		protected Object LockObject = new object();

		protected Dictionary<Guid, PointTag> TagDictionary = new Dictionary<Guid, PointTag>();

		public void SetOpcUaTags(Dictionary<Guid, PointTag> tagDictionary)
		{
			lock (this.LockObject)
			{
				this.TagDictionary = tagDictionary;
			}
		}

		public void UpdateOutputTagWithoutOverride(PointTag pointTag)
		{
			lock(this.LockObject)
			{
				PointTag outputTag;
				if(this.TagDictionary.TryGetValue(pointTag.PointTagGuid, out outputTag))
				{
					// skip update of the value on clear of override
					if (outputTag.OpcStatusCodeBits != StatusCodes.GoodLocalOverride)
					{
						if (pointTag.Value is ValueType)
						{
							outputTag.Value = pointTag.Value;
						}
						else
						{
							outputTag.ValueXml = pointTag.ValueXml;
						}
						outputTag.Status = pointTag.Status;
						outputTag.SourceTimeStamp = pointTag.SourceTimeStamp;
					}
				}
			}
		}

		public void UpdateOutputTagWithOverride(PointTag pointTag)
		{
			lock (this.LockObject)
			{
				PointTag outputTag;
				if (this.TagDictionary.TryGetValue(pointTag.PointTagGuid, out outputTag))
				{
					outputTag.Value = pointTag.Value;
					outputTag.Status = pointTag.Status;
					outputTag.SourceTimeStamp = pointTag.SourceTimeStamp;
				}
			}
		}



		/// <summary>
		/// Retrieves the NodeId object for a given node
		/// </summary>
		/// <param name="nodeIdStr">The NodeId address</param>
		/// <returns></returns>
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


		/// <summary>
		/// Creates a new Session object to an OPC Ua server, based on the OPC Ua connection parameters of a given PointTag.
		/// </summary>
		/// <param name="sessionName">Name/label for the session.</param>
		/// <param name="pointTag">PointTag holding the connection parameters</param>
		/// <returns></returns>
		public static ClientSession CreateSessionForPointTag(UaApplication application, PointTag pointTag)
		{
			string serverUrl = null;
			ClientSession opcuaSession = null;
			MessageSecurityMode securityMode = MessageSecurityMode.None;
			SecurityPolicy securityPolicy = SecurityPolicy.None;
			MessageEncoding messageEncoding = MessageEncoding.Binary;
			UserIdentity userIdentity = new UserIdentity();

			if (pointTag == null || string.IsNullOrEmpty(pointTag.OpcUaServerEndPoint))
				return null;

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
						throw new Exception("OpcUaClientProcessor.CreateSessionForPointTag : Invalid MessageSecurityMode " + pointTag.OpcUaSecurityMode);
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
						throw new Exception("OpcUaClientProcessor.CreateSessionForPointTag : Invalid SecurityPolicy " + pointTag.OpcUaSecurityPolicy);
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
						throw new Exception("OpcUaClientProcessor.CreateSessionForPointTag : Invalid MessageEncoding " + pointTag.OpcUaMessageEncoding);
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
						throw new Exception("OpcUaClientProcessor.CreateSessionForPointTag : Invalid UserIdentityMethod " + pointTag.OpcUaUserIdentityMethod);
				}
			}

			opcuaSession = application.CreateSession(serverUrl, securityMode, securityPolicy, messageEncoding, userIdentity, null);
			return opcuaSession;
		}


		public static void ConvertDataValue(PointTag pointTag, DataValueEx dataValue)
		{
			if (pointTag.OpcUaServerDataType.HasValue
			&& pointTag.OpcUaServerDataType.Value != 0
			&& dataValue.Value != null)
			{
				switch (pointTag.OpcUaServerDataType.Value)
				{
					case 1:
						dataValue.Value = Convert.ToBoolean(dataValue.Value);
						break;
					case 2:
						dataValue.Value = Convert.ToSByte(dataValue.Value);
						break;
					case 3:
						dataValue.Value = Convert.ToByte(dataValue.Value);
						break;
					case 4:
						dataValue.Value = Convert.ToInt16(dataValue.Value);
						break;
					case 5:
						dataValue.Value = Convert.ToUInt16(dataValue.Value);
						break;
					case 6:
						dataValue.Value = Convert.ToInt32(dataValue.Value);
						break;
					case 7:
						dataValue.Value = Convert.ToUInt32(dataValue.Value);
						break;
					case 8:
						dataValue.Value = Convert.ToInt64(dataValue.Value);
						break;
					case 9:
						dataValue.Value = Convert.ToUInt64(dataValue.Value);
						break;
					case 10:
						dataValue.Value = Convert.ToSingle(dataValue.Value);
						break;
					case 11:
						dataValue.Value = Convert.ToDouble(dataValue.Value);
						break;
					case 12:
						dataValue.Value = Convert.ToString(dataValue.Value);
						break;
					case 13:
						dataValue.Value = Convert.ToString(dataValue.Value);
						break;
					case 14:
						dataValue.Value = Convert.ToDateTime(dataValue.Value);
						break;
				}
			}
		}

		public Dictionary<ClientSession, Dictionary<Guid,WriteValue>> GetSessionWriteListDictionaryForWrites(UaApplication application)
		{
			var sessionTagListDictionary = new Dictionary<Guid, List<PointTag>>();

			lock (this.LockObject)
			{
				foreach (var pointTag in this.TagDictionary.Values)
				{
					if (pointTag.OpcStatusCodeBits == StatusCodes.GoodLocalOverride
					|| pointTag.IsBad())
					{
						continue;
					}

					//  Time to refresh output
					if (pointTag.OpcUaWritePeriodicUpdateInterval.HasValue
					&& DateTimeOffset.UtcNow >= pointTag.ServerTimeStamp.AddMilliseconds(pointTag.OpcUaWritePeriodicUpdateInterval.Value))
					{
						List<PointTag> tagList;
						if (!sessionTagListDictionary.TryGetValue(pointTag.OpcUaServerGuid, out tagList))
						{
							tagList = new List<PointTag>();
							sessionTagListDictionary.Add(pointTag.OpcUaServerGuid, tagList);
						}

						tagList.Add(pointTag);
					}

					// Value has been updated and OpcUaWriteHoldoffTime has expired or is not configured
					else if (pointTag.SourceTimeStamp > pointTag.ServerTimeStamp
					&& (!pointTag.OpcUaWriteHoldoffTime.HasValue
					|| DateTimeOffset.UtcNow >= pointTag.ServerTimeStamp.AddMilliseconds(pointTag.OpcUaWriteHoldoffTime.Value)))
					{
						List<PointTag> tagList;
						if (!sessionTagListDictionary.TryGetValue(pointTag.OpcUaServerGuid, out tagList))
						{
							tagList = new List<PointTag>();
							sessionTagListDictionary.Add(pointTag.OpcUaServerGuid, tagList);
						}

						tagList.Add(pointTag);
					}
				}

				var sessionValueListDictionary = new Dictionary<ClientSession, Dictionary<Guid, WriteValue>>();

				foreach (var tagList in sessionTagListDictionary.Values)
				{

					var valueListDictonary = new Dictionary<Guid, WriteValue>();

					foreach (var pointTag in tagList)
					{

						var nodeIdStr = pointTag.OpcUaNodeId;
						var writeValue = new WriteValue { AttributeId = 13 };
						writeValue.NodeId = GetNodeId(pointTag.OpcUaNodeId);
						if (writeValue.NodeId == null)
						{
							throw new Exception("OpcUaClientProcessor.OutputOPCCommand Error getting NodeId for " + pointTag.PointID + "." + pointTag.PointTagGuid);
						}
						var dataValue = new DataValue();
						if(pointTag.Value == null)
						{
							dataValue.Value = null;
						}
						else if (pointTag.Value.GetType().IsValueType)
						{
							if (pointTag.ValueTypeString == "System.DateTimeOffset")
							{
								dataValue.Value = ((DateTimeOffset)pointTag.Value).UtcDateTime;
							}
							else if (pointTag.ValueTypeString == "System.TimeSpan")
							{
								dataValue.Value = ((TimeSpan)pointTag.Value).Ticks;
							}
							else
							{
								dataValue.Value = pointTag.Value;
							}
						}
						else if (pointTag.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
						{
							var pointCommandStatusListReference = pointTag.Value as PointCommandStatusListReference;
							if (pointCommandStatusListReference != null
							&& pointCommandStatusListReference.CurrentValue.HasValue)
							{
								dataValue.Value = pointCommandStatusListReference.CurrentValue.Value;
							}
							else
							{
								dataValue.Value = null;
							}
						}
						else if (pointTag.ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
						{
							var deviceAlarmMapReference = pointTag.Value as DeviceAlarmMapReference;

							if (deviceAlarmMapReference != null
							&& deviceAlarmMapReference.CurrentValue.HasValue)
							{
								dataValue.Value = deviceAlarmMapReference.CurrentValue.Value;
							}
							else
							{
								dataValue.Value = null;
							}
						}
						else if (pointTag.ValueTypeString == "System.String")
						{
							dataValue.Value = pointTag.Value;
						}
						else
						{
							throw new Exception("OutputOpcCommand Error Point = " + pointTag.PointID + " ID = " + pointTag.ID + " Type = " + pointTag.ValueTypeString);
						}

						try
						{
							ConvertDataValue(pointTag, new DataValueEx(dataValue));

							// WCG 03/06/2018 - Softing Gateway or Matrikon doesn't support writing TimeStamps, not sure which, so for now wwe will not support it at all.
							pointTag.ServerTimeStamp = DateTimeOffset.UtcNow;
							writeValue.Value = dataValue;
							writeValue.Value.StatusCode = new StatusCode((uint)pointTag.OpcStatusSubCode);
							valueListDictonary.Add(pointTag.PointTagGuid, writeValue);
						}

						// Do not write values that cannot be converted
						catch (Exception)
						{
							pointTag.Status = StatusCodes.BadTypeMismatch;
						}
					}
					var session = CreateSessionForPointTag(application, tagList[0]);
					sessionValueListDictionary.Add(session, valueListDictonary);
				}

				return sessionValueListDictionary;
			}
		}


		public bool TryGetValue(Guid pointTagGuid, out PointTag pointTag)
		{
			lock (this.LockObject)
			{
				return this.TagDictionary.TryGetValue(pointTagGuid, out pointTag);
			}
		}


		protected void DiffOpcUaTags(
			Dictionary<Guid, PointTag> currentTagDictionary,
			Dictionary<Guid, PointTag> previousTagDictionary,
			ref Dictionary<Guid, PointTag> addedTagDictionary,
			ref Dictionary<Guid, PointTag> deletedTagDictionary)
		{
			// bds
			foreach (var tag in currentTagDictionary.Values)
			{
				PointTag oldTag;
				if (previousTagDictionary.TryGetValue(tag.PointTagGuid, out oldTag))
				{
					if (deletedTagDictionary != null
					&& (tag.OpcUaServerGuid != oldTag.OpcUaServerGuid
					|| tag.OpcUaPublishingInterval != oldTag.OpcUaPublishingInterval
					|| tag.OpcUaWriteHoldoffTime != oldTag.OpcUaWriteHoldoffTime
					|| tag.OpcUaWritePeriodicUpdateInterval != oldTag.OpcUaWritePeriodicUpdateInterval
					|| tag.OpcUaIsReadable != oldTag.OpcUaIsReadable
					|| tag.OpcUaNodeId != oldTag.OpcUaNodeId
					|| tag.OpcUaServerDataType != oldTag.OpcUaServerDataType
					|| tag.Deadband != oldTag.Deadband
					|| tag.Holdoff != oldTag.Holdoff
					|| ThreadSharedData.Instance().PointCommandStatusListReferenceChanged(tag, oldTag)))
					{
						addedTagDictionary.Add(tag.PointTagGuid, tag);
						deletedTagDictionary.Add(oldTag.PointTagGuid, oldTag);
					}
				}
				else
				{
					addedTagDictionary.Add(tag.PointTagGuid, tag);
				}
			}
		}

		public void DiffTags(
			Dictionary<Guid, PointTag> currentTagDictionary,
			ref Dictionary<Guid, PointTag> addedTagDictioary,
			ref Dictionary<Guid, PointTag> deleteTagDictionary)
		{
			lock (this.LockObject)
			{
				this.DiffOpcUaTags(currentTagDictionary, this.TagDictionary, ref addedTagDictioary, ref deleteTagDictionary);
				Dictionary<Guid, PointTag> nullTagDictionary = null;
				this.DiffOpcUaTags(this.TagDictionary, currentTagDictionary, ref deleteTagDictionary, ref nullTagDictionary);
			}
		}

		public void Clear()
		{
			this.SetOpcUaTags(new Dictionary<Guid, PointTag>());
		}
	}
}
