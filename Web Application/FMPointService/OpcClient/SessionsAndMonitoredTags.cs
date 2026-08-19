
namespace FMPointService.OpcClient
{
	using FMBusinessObjects.DataObjects;
	using ThreadSupport;
	using Logging;
	using System;
	using System.Collections.Generic;
	using InProcLogging;
	using Opc.Ua;
	using Softing.Opc.Ua.Client;

	public class SessionsAndMonitoredTags
	{
		List<PollingSubscription> subscriptionList = new List<PollingSubscription>();

		protected void RemoveSubscription(PollingSubscription subscription)
		{
			subscriptionList.Remove(subscription);
		}

		protected PollingSubscription FindSubscriptionForAddition(UaApplication application, bool enableUseLastKnownGood, PointTag tag)
		{
			foreach(var subscription in this.subscriptionList)
			{
				if(subscription.IsSessionForPointTag(tag))
				{
					return subscription;
				}

			}
			var subscript = new PollingSubscription(application, enableUseLastKnownGood, tag);
			subscriptionList.Add(subscript);
			subscript.Start();
			return subscript;
		}

		protected PollingSubscription FindSubscriptionForTag(PointTag tag)
		{
			foreach (var subscription in subscriptionList)
			{
				if (subscription.DoesSessionHavePointTag(tag.PointTagGuid))
				{
					return subscription;
				}
			}
			return null;
		}
		public void AddTag(UaApplication application, bool enableUseLastKnownGood, PointTag tag)
		{
			var subscription = FindSubscriptionForAddition(application, enableUseLastKnownGood, tag);
			if(subscription.AddPointTag(tag) == false)
			{
				Logger.LogError("SessionsAndMonitoredTags.AddTag failed to add tag " + tag.PointTagGuid);
			}
		}

		public void AddTags(UaApplication application, bool enableUseLastKnownGood, Dictionary<Guid,PointTag> tagDictionary)
		{
			foreach(var tag in tagDictionary.Values)
			{
				try
				{
					AddTag(application, enableUseLastKnownGood, tag);
				}
				catch
				{
					// do not care. Happens if a session was attempted but failed because the server does not exist.
				}
			}
		}

		public void RemoveTag(PointTag tag)
		{
			var subscription = FindSubscriptionForTag(tag);
			if(subscription == null)
			{
				Logger.LogError("SessionsAndMonitoredTags.RemoveTag subscription not found.  This should never happen");
				return;
			}

			if (subscription.RemovePointTag(tag) == false)
			{
				Logger.LogError("SessionsAndMonitoredTags.RemoveTag failed to remove tag " + tag.PointTagGuid);
			}

			if(subscription.MonitoredTagCount <= 0)
			{
				subscription.Terminate();
				subscription.Disconnect();
				RemoveSubscription(subscription);
			}
		}

		public void RemoveTags(Dictionary<Guid, PointTag> tagDictionary)
		{
			foreach (var tag in tagDictionary.Values)
			{
				RemoveTag(tag);
			}
		}

		public void CleanUp()
		{
			foreach (var subscription in subscriptionList)
			{
				subscription.Terminate();
				subscription.Disconnect();
			}

			subscriptionList.Clear();
		}

		public void RefreshPointTag(PointTag pointTag)
		{
			var pollingSubscription =  FindSubscriptionForTag(pointTag);
			if(pollingSubscription != null)
			{
				pollingSubscription.RefreshPointTag(pointTag);
			}
			else
			{
				throw new Exception("No Subscription");
			}
		}

		public void OutputOpcCommand(UaApplication application, SecurityClass security, PointTag pointTag)
		{
			if (security == null || pointTag == null)
			{
				throw new ArgumentNullException("OpcUaClientProcessor.OutputOPCCommand");
			}

			if (null == ThreadSharedData.Instance().GetPointTag(pointTag.PointTagGuid))
			{
				throw new Exception("PollingSubscription.OutputOPCCommand Point not found for " + pointTag.PointID + "." + pointTag.PointTagGuid);
			}

			var nodeIdStr = pointTag.OpcUaNodeId;
			var writeValue = new WriteValue { AttributeId = 13 };

			writeValue.NodeId = PollingSubscription. GetNodeId(nodeIdStr);
			if (writeValue.NodeId == null)
			{
				throw new Exception("PollingSubscription.OutputOPCCommand Error getting NodeId for " + pointTag.PointID + "." + pointTag.PointTagGuid);
			}
			var valueToWrite = new DataValue();
			if (pointTag.Value != null)
			{
				if (pointTag.Value.GetType().IsValueType)
				{
					valueToWrite.Value = pointTag.Value;
				}
				else if (pointTag.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
				{
					valueToWrite.Value = (pointTag.Value as PointCommandStatusListReference).CurrentValue.Value;
				}
				else
				{
					throw new Exception("OutputOpcCommand Error Point = " + pointTag.PointID + " ID = " + pointTag.ID + " Type = " + pointTag.ValueTypeString);
				}
			}

			writeValue.Value = valueToWrite;
			writeValue.Value.StatusCode = new StatusCode((uint)pointTag.Status);

			using (var session = OpcUaTags.CreateSessionForPointTag(application, pointTag))
			{
				try
				{
					session.Connect(false, true);
					var statusCode = session.Write(writeValue);
					if (statusCode == null)
					{
						throw new Exception("OpcUaClientProcessor.OutputOPCCommand StatusCode is null for " + pointTag.PointID + "." + pointTag.PointTagGuid);
					}
				}
				catch (Exception writeEx)
				{
					throw new Exception("OpcUaClientProcessor.OutputOPCCommand Write Exception " + writeEx.Message + " for " + pointTag.PointID + "." + pointTag.PointTagGuid);
				}
			}
		}
	}
}
