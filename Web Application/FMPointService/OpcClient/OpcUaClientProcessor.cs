namespace FMPointService.OpcClient
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Xml;
	using FMBusinessObjects.DataObjects;

	using global::FMPointService.ThreadSupport;
	using InProcLogging;
	using Opc.Ua;
	using Softing.Opc.Ua.Client;
	using Softing.Opc.Ua.Configuration;

	public class OpcUaClientProcessor : SrmThread
	{
		protected UaApplication application;

		protected static OpcUaClientProcessor inst = null;

		protected readonly string host;

		protected readonly string port;

		protected Object SessionSubscriptionMonitoredItemsLock = new object();

		protected SubscriptionDictionary SubscriptionAndSessionDictionary = new SubscriptionDictionary();

		protected OpcUaTags MonitoredTags = new OpcUaTags();

		protected OpcUaTags OutputTags = new OpcUaTags();

		protected Dictionary<Guid, ClientMonitoredItem> MonitoredItemDictionary = new Dictionary<Guid, ClientMonitoredItem>();

		protected readonly AutoResetEvent TagDictionaryChangedEvent = new AutoResetEvent(false);

		protected Object LockObject = new object();

		protected bool enableUseLastKnownGood = false;

		protected OpcUaClientProcessor(string host, string port, bool enableUseLastKnwonGood)
		{
			this.host = host;
			this.port = port;
			this.enableUseLastKnownGood = enableUseLastKnwonGood;
		}

		public static OpcUaClientProcessor Instance(string host, string port, bool enableUseLastKnownGood)
		{
			if (inst == null)
			{
				inst = new OpcUaClientProcessor(host, port, enableUseLastKnownGood);
			}
			return inst;
		}

		public static OpcUaClientProcessor Instance()
		{
			if (inst == null)
			{
				throw new Exception("OpcUaClientProcessor not initialized");
			}
			return inst;
		}


		/// <summary>
		/// Handles the certificate validation error event.
		/// This event is triggered when the certificate received from the server during connection is not trusted.
		/// </summary>
		private void Application_CertificateValidation(object sender, CertificateValidationEventArgs e)
		{
			try
			{
				CertificateValidator validator = (CertificateValidator)sender;
				this.HandleCertificateValidationError(validator, e);
			}
			catch (Exception ex)
			{
				Logger.LogError("OpcUaClientProcessor.Application_CertificateValidation: " + ex);
			}
		}

		/// <summary>
		/// Handles a certificate validation error.
		/// </summary>
		/// <param name="validator">The validator (not used).</param>
		/// <param name="e">The <see cref="CertificateValidationEventArgs"/> instance event arguments provided when a certificate validation error occurs.</param>
		public void HandleCertificateValidationError(CertificateValidator validator, CertificateValidationEventArgs e)
		{
			var buffer = new StringBuilder();

			buffer.AppendFormat("Certificate could not be validated\r\n\r\n");
			buffer.AppendFormat("Subject: {0}\r\n", e.Certificate.Subject);
			buffer.AppendFormat("Issuer: {0}\r\n", (e.Certificate.Subject == e.Certificate.Issuer) ? "Self-signed" : e.Certificate.Issuer);
			buffer.AppendFormat("Valid From: {0}\r\n", e.Certificate.NotBefore);
			buffer.AppendFormat("Valid To: {0}\r\n", e.Certificate.NotAfter);
			buffer.AppendFormat("Thumbprint: {0}\r\n\r\n", e.Certificate.Thumbprint);
		}


		/// <summary>
		/// Initial OPC UA node monitoring configuration
		/// </summary>
		protected void Initialize()
		{
			ApplicationConfigurationBuilderEx configuration = FMPointService.LoadApplicationConfiguration("FMMPointService Client").Result;
			lock (FMPointService.SoftingInitializationLock)
			{
				this.application = UaApplication.Create(configuration).Result;
			}
		}

		public void SignalTagChanges()
		{
			TagDictionaryChangedEvent.Set();
		}

		public override void Run()
		{
			try
			{
				this.Initialize();

				WaitHandle[] events = { TagDictionaryChangedEvent };

				while (this.mShutdown != true)
				{
					try
					{
						var eventThatSignaled = WaitHandle.WaitAny(events, 1000);
						if (eventThatSignaled == 0)
						{
							this.CheckTagChanges();
						}

						this.WriteValues();

						this.ManageConnections();
					}
					catch (Exception ex)
					{
						Logger.LogError("OpcUaClientProcessor.ProcessScan Inner Loop Exception: " + ex);
					}
				}

				this.DropAllOpcUaConnections();
			}
			catch (Exception ex)
			{
				Logger.LogError("OpcUaClientProcessor.ProcessScan exception: " + ex);
			}
		}

		protected void WriteValues()
		{

			var sessionValueListDictionary = OutputTags.GetSessionWriteListDictionaryForWrites(application);

			lock (this.LockObject)
			{

				foreach (var session in sessionValueListDictionary.Keys)
				{
					var valueListDictionary = sessionValueListDictionary[session];

					try
					{
						session.Connect(false, true);

						var valueList = valueListDictionary.Values.ToList();

						var statusCodes = session.Write(valueListDictionary.Values.ToList());

						var index = 0;
						foreach (var statusCode in statusCodes)
						{
							valueList[index++].Value.StatusCode = statusCode;
						}
					}
					catch (Exception)
					{
						foreach (var value in valueListDictionary.Values)
						{
							value.Value.StatusCode = StatusCodes.Bad;
						}
					}
					finally
					{
						session.Dispose();
					}

					ThreadSharedData.Instance().UpdateValueStatusAndTimeStampForValues(valueListDictionary);
				}
			}
		}
		/// <summary>
		/// Drops all open OPC UA MonitoredItems, Subscriptions, and Sessions created by this service.
		/// </summary>
		protected void DropAllOpcUaConnections()
		{
			this.DeleteSessionsAndSubscriptions();
			this.MonitoredItemDictionary.Clear();
			this.MonitoredTags.Clear();
			this.SubscriptionAndSessionDictionary.Clear();
			//OPCCommandOutputPointTags.Clear();
		}

		protected void DeleteSessionsAndSubscriptions()
		{
			var sessions = application.CurrentSessions;

			for (int i = sessions.Count - 1; i >= 0; i--)
			{
				var session = sessions[i];

				// Only concerned with sessions with subscriptions
				if (session.Subscriptions.Count == 0)
				{
					continue;
				}

				try
				{
					session.StateChanged -= this.SessionStateChanged;

					for (int j = session.Subscriptions.Count - 1; j >= 0; j--)
					{
						var subscription = session.Subscriptions[j];
						subscription.DataChangesReceived -= this.SubscriptionDataChangesReceived;
						subscription.StateChanged -= this.SubscriptionStateChanged;

						for (int k = subscription.MonitoredItems.Count - 1; k >= 0; k--)
						{
							var monitoredItem = subscription.MonitoredItems[k];
							monitoredItem.StateChanged -= this.MonitoredItemStateChanged;
						}
					}

					if (session.CurrentState != State.Disconnected)
					{
						session.Disconnect(true);
					}

					for (int j = session.Subscriptions.Count - 1; j >= 0; j--)
					{
						var subscription = session.Subscriptions[j];
						for (int k = subscription.MonitoredItems.Count - 1; k >= 0; k--)
						{
							try
							{
								var monitoredItem = subscription.MonitoredItems[k];
								monitoredItem.Delete();
							}
							catch (Exception ex)
							{
								Logger.LogError("OpcUaClientProcessor.DropAllOpcUaConnections: " + ex);
							}
						}

						session.DeleteSubscription(subscription);
					}

					session.Dispose();
				}
				catch (Exception ex)
				{
					Logger.LogError("OpcUaClientProcessor.DropAllOpcUaConnections: " + ex);
				}
			}
		}



		protected void ManageConnections()
		{
			var sessions = application.CurrentSessions;
			string sessionName = "Unknown";

			for (int i = sessions.Count - 1; i >= 0; i--)
			{
				if (this.mShutdown)
				{
					return;
				}

				try
				{
					var session = sessions[i];

					// only process sessions that have subscriptions.  Other sessions exist, i.e. EntepriseVisibilityPushProcessor
					if (session.Subscriptions.Count == 0)
					{
						continue;
					}

					sessionName = session.SessionName;

					if (session.CurrentState == State.Disconnected
					&& session.TargetState == State.Disconnected)
					{
						session.Connect(true, true);
					}

					for (int j = session.Subscriptions.Count - 1; j >= 0; j--)
					{
						if (this.mShutdown)
						{
							return;
						}

						var subscription = session.Subscriptions[j];
						if (subscription.CurrentState == State.Disconnected
						&& subscription.TargetState == State.Disconnected)
						{
							subscription.Connect(true, true);
						}

						for (int k = subscription.MonitoredItems.Count - 1; k >= 0; k--)
						{
							var monitoredItem = subscription.MonitoredItems[k];
							if (this.mShutdown)
							{
								return;
							}

							Guid pointTagGuid;
							this.GetIdentifierFromPointTagMarker(monitoredItem.DisplayName, out pointTagGuid);

							bool monitoredTag = false;
							lock (SessionSubscriptionMonitoredItemsLock)
							{
								if (MonitoredItemDictionary.ContainsKey(pointTagGuid))
								{
									PointTag pointTag = null;
									if (this.MonitoredTags.TryGetValue(pointTagGuid, out pointTag))
									{
										if (pointTag.OpcUaNodeId == monitoredItem.NodeId.ToString()
										&& pointTag.OpcUaPublishingInterval == subscription.PublishingInterval
										&& pointTag.OpcUaServerGuid == new Guid(session.SessionName))
										{
											monitoredTag = true;

											// If the session is active and the subscription is supposed to be active but isn't, set the pt status to BadSubscriptionIdInvalid
											if ((subscription.CurrentState == State.Disconnected
											&& subscription.TargetState == State.Active
											&& session.CurrentState == State.Active)
											|| (monitoredItem.CurrentState == State.Disconnected
											&& monitoredItem.TargetState == State.Active
											&& subscription.CurrentState == State.Active))
											{
												if (this.enableUseLastKnownGood
												&& pointTag.Value != null)
												{
													if (pointTag.Status != StatusCodes.UncertainLastUsableValue)
													{
														pointTag.Status = StatusCodes.UncertainLastUsableValue;
														Dictionary<Guid, PointTag> pointTags = new Dictionary<Guid, PointTag>();
														pointTags.Add(pointTag.PointTagGuid, pointTag);
														ThreadSharedData.Instance().ExternalUpdateTags(pointTags);
													}
												}
												else
												{
													if (pointTag.Status != StatusCodes.BadNodeIdUnknown)
													{
														pointTag.Status = StatusCodes.BadNodeIdUnknown;
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
														Dictionary<Guid, PointTag> pointTags = new Dictionary<Guid, PointTag>();
														pointTags.Add(pointTag.PointTagGuid, pointTag);
														ThreadSharedData.Instance().ExternalUpdateTags(pointTags);
													}
												}
											}
										}
									}
								}
							}

							if (monitoredTag)
							{
								// Note: StatusCodes.BadNodeIdUnknown is not auto reconnected.
								if (monitoredItem.CurrentState == State.Disconnected
								&& (monitoredItem.TargetState == State.Disconnected
								|| monitoredItem.Error.Code == StatusCodes.BadNodeIdUnknown
								|| monitoredItem.Error.Code == StatusCodes.BadSubscriptionIdInvalid))
								{
									// added try catch becuase the following function can throw which would take the app out of the scan loop thereby
									// not scanning any other items but leaving them as valid and good.
									try
									{
										monitoredItem.Connect(true, true);
									}
									catch
									{

									}
								}
							}
							else
							{
								// added try catch becuase the following function can throw which would take the app out of the scan loop thereby
								// not scanning any other items but leaving them as valid and good.
								try
								{
									if (monitoredItem.CurrentState == State.Connected
									|| monitoredItem.CurrentState == State.Active)
									{
										monitoredItem.Disconnect(true);
									}

									// Delete performs Disconnect(false)
									monitoredItem.Delete();
								}
								catch
								{

								}
							}
						}

						if (subscription.MonitoredItems.Count == 0)
						{
							// DeleteSubscription performs Disconnect(true)
							session.DeleteSubscription(subscription);
							lock (SessionSubscriptionMonitoredItemsLock)
							{
								this.SubscriptionAndSessionDictionary.CleanupSubscription(new Guid(subscription.DisplayName.Substring(5, 36)), new Guid(session.SessionName), (int?)subscription.PublishingInterval);
							}
						}
					}

					if (session.Subscriptions.Count == 0)
					{
						if (session.CurrentState == State.Connected
						|| session.CurrentState == State.Active)
						{
							session.Disconnect(true);
						}

						session.StateChanged -= this.SessionStateChanged;
						session.Dispose();
					}
				}
				catch (Exception ex)
				{
					Logger.LogError("OpcUaClientProcessor.ManageConnects " + sessionName + " " + ex.Message);
				}
			}
		}

		protected void CheckTagChanges()
		{
			Dictionary<Guid, PointTag> addedMonitoredTags, deletedMonitoredTags;
			this.GetMonitoredTagsDifferences(out addedMonitoredTags, out deletedMonitoredTags);
			if (addedMonitoredTags != null
			&& deletedMonitoredTags != null
			&& (addedMonitoredTags.Count != 0
			|| deletedMonitoredTags.Count != 0))
			{
				Logger.LogDebug("OpcUaClientProcessor.CheckTagChanges <AddMonitoredItems,DeletedMonitoredItems> = " + addedMonitoredTags.Count + ", " + deletedMonitoredTags.Count);
			}
			this.ProcessPointTagDeletions(deletedMonitoredTags);
			this.AddMonitoredItemsForPointTag(addedMonitoredTags);

			Dictionary<Guid, PointTag> addedOutputTags, deletedOutputTags;
			this.GetOutputTagsDifferences(out addedOutputTags, out deletedOutputTags);
			if (addedOutputTags != null
			&& deletedOutputTags != null
			&& (addedOutputTags.Count != 0
			|| deletedOutputTags.Count != 0))
			{
				Logger.LogDebug("OpcUaClientProcessor2.CheckTagChanges <AddOutputItems,DeletedOutputItems> = " + addedOutputTags.Count + ", " + deletedOutputTags.Count);
			}
		}

		protected void AddMonitoredItemsForPointTag(Dictionary<Guid, PointTag> addedMonitoredTags)
		{
			foreach (var addedTag in addedMonitoredTags.Values)
			{
				try
				{
					this.AddMonitoredItemForPointTag(addedTag);
				}
				catch (Exception ex)
				{
					Logger.LogError("OpcUaClientProcessor.AddMonitoredItemsForPointTag AddMonitoredItemForPointTag Exception: " + ex);
				}
			}
		}


		protected void GetMonitoredTagsDifferences(out Dictionary<Guid, PointTag> addedMonitoredTags, out Dictionary<Guid, PointTag> deletedMonitoredTags)
		{
			var currentMonitoredTags = ThreadSharedData.Instance().GetMonitoredTagDictionary();
			addedMonitoredTags = new Dictionary<Guid, PointTag>();
			deletedMonitoredTags = new Dictionary<Guid, PointTag>();
			this.MonitoredTags.DiffTags(currentMonitoredTags, ref addedMonitoredTags, ref deletedMonitoredTags);
			this.MonitoredTags.SetOpcUaTags(currentMonitoredTags);
		}

		protected void GetOutputTagsDifferences(out Dictionary<Guid, PointTag> addedOutputTags, out Dictionary<Guid, PointTag> deletedOutputTags)
		{
			var currentOutputTags = ThreadSharedData.Instance().GetOutputTagDictionary();
			addedOutputTags = new Dictionary<Guid, PointTag>();
			deletedOutputTags = new Dictionary<Guid, PointTag>();
			this.OutputTags.DiffTags(currentOutputTags, ref addedOutputTags, ref deletedOutputTags);
			this.OutputTags.SetOpcUaTags(currentOutputTags);
		}



		/// <summary>
		/// Process PointTags that have either been deleted or have been updated but do not reference an OpcUaServer
		/// </summary>
		/// <param name="deletedMonitoredTags"></param>
		private void ProcessPointTagDeletions(Dictionary<Guid, PointTag> deletedMonitoredTags)
		{
			lock (SessionSubscriptionMonitoredItemsLock)
			{
				foreach (var pointTag in deletedMonitoredTags.Values)
				{
					ClientMonitoredItem monitoredItem = null;
					if (MonitoredItemDictionary.TryGetValue(pointTag.PointTagGuid, out monitoredItem))
					{
						MonitoredItemDictionary.Remove(pointTag.IdentityGuid);
						monitoredItem.StateChanged -= this.MonitoredItemStateChanged;
					}
				}
			}
		}

		protected void CleanUpSession(ClientSession session)
		{
			if (session != null
			&& session.Subscriptions.Count == 0)
			{
				session.StateChanged -= this.SessionStateChanged;

				try
				{
					if (session.CurrentState == State.Connected
					|| session.CurrentState == State.Active)
					{
						session.Disconnect(true);
					}

					if (session.TargetState == State.Connected
					|| session.TargetState == State.Active)
					{
						session.Disconnect(false);
					}

					session.Dispose();
				}
				catch (Exception e)
				{
					Logger.LogError(
						"OpcUaClientProcessor.CleanUpSession exception: "
						+ e.Message);
					return;
				}
			}
		}


		/// <summary>
		/// Retrieves the identifier captured in a PointTagMarker
		/// </summary>
		/// <param name="pointTagMarker"></param>
		/// <param name="pointTagGuid"></param>
		protected void GetIdentifierFromPointTagMarker(string pointTagMarker, out Guid pointTagGuid)
		{
			try
			{
				pointTagGuid = new Guid(pointTagMarker);
			}
			catch
			{
				throw new Exception("OpcUaClientProcessor.GetIdentifierFromPointTagMarker : Invalid PointTag Marker");
			}

			if ((pointTagGuid == Guid.Empty))
				throw new Exception("OpcUaClientProcessor.GetIdentifierFromPointTagMarker : Invalid PointTag Marker");
		}

		public void SessionStateChanged(object sender, EventArgs eventArgs)
		{
			if (this.mShutdown)
			{
				return;
			}

			var session = sender as ClientSession;
			if (session == null)
			{
				return;
			}

			Logger.LogDebug("OpcUaClientProcessor.SessionStateChanged Session " + session.SessionName + " " + session.CurrentState.ToString());
		}

		public void SubscriptionStateChanged(object sender, EventArgs eventArgs)
		{
			if (this.mShutdown)
			{
				return;
			}

			//This method is executed in a Softing OPC UA framework thread and not the thread executing the Run() loop.
			var subscription = sender as ClientSubscription;
			if (subscription == null)
			{
				return;
			}

			Logger.LogDebug("OpcUaClientProcessor.SubscriptionStateChanged Subscription " + subscription.DisplayName + " " + subscription.CurrentState.ToString());
		}


		public void MonitoredItemStateChanged(object sender, EventArgs eventArgs)
		{
			if (this.mShutdown)
			{
				return;
			}

			var monitoredItem = sender as ClientMonitoredItem;
			if (monitoredItem == null)
			{
				return;
			}

			Dictionary<Guid, PointTag> pointTags = new Dictionary<Guid, PointTag>();

			PointTag pointTag = null;
			Guid pointTagGuid;
			bool statusChanged = false;

			this.GetIdentifierFromPointTagMarker(monitoredItem.DisplayName, out pointTagGuid);
			if (this.MonitoredTags.TryGetValue(pointTagGuid, out pointTag))
			{
				if (monitoredItem.CurrentState == State.Disconnected)
				{
					if (this.enableUseLastKnownGood)
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
				else if (monitoredItem.CurrentState == State.Active)
				{
					if (monitoredItem.LastValue != null)
					{
						statusChanged = true;
						monitoredItem.LastValue.ServerTimestamp = DateTimeOffset.UtcNow.UtcDateTime;
						this.ConvertDataValueToTag(monitoredItem.LastValue, pointTag);
					}
				}

				if (statusChanged)
				{
					pointTag.ServerTimeStamp = DateTimeOffset.UtcNow;
					pointTag.SourceTimeStamp = DateTimeOffset.UtcNow;
					pointTags.Add(pointTag.PointTagGuid, pointTag);
				}

			}
			ThreadSharedData.Instance().ExternalUpdateTags(pointTags);
		}


		/// <summary>
		/// Creates and returns the marker to be used to tag a MonitoredItem for a given PointTag
		/// </summary>
		/// <param name="pointTag"></param>
		/// <returns></returns>
		protected string GetPointTagMarker(PointTag pointTag)
		{
			string result = Convert.ToString(pointTag.PointTagGuid);
			return result;
		}


		public void ConvertDataValueToTag(DataValueEx dataValue, PointTag pointTag)
		{
			object value = dataValue.Value;

			// if the localoverride is set do not copy this to the destination tag
			var statusCode = GetAllowedStatusCodesFromPassedInParameter(dataValue.StatusCode);

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
					ref value, ref statusCode, isDeviceAlarmMapReference, deviceAlarmMapGuid);
			}

			if (StatusCode.IsBad(statusCode))
			{
				if (this.enableUseLastKnownGood)
				{
					if (pointTag.Value != null)
					{
						if (pointTag.OpcStatusCodeBits != StatusCodes.UncertainLastUsableValue)
						{
							pointTag.Status = StatusCodes.UncertainLastUsableValue;
						}
					}
					else
					{
						pointTag.Status = statusCode.Code;
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

					pointTag.Status = statusCode.Code;
				}
			}
			else
			{
				pointTag.Value = value;
				pointTag.Status = statusCode.Code;
			}

			pointTag.ServerTimeStamp = dataValue.ServerTimestamp;
			pointTag.SourceTimeStamp = dataValue.SourceTimestamp;

		}


		/// <summary>
		/// Handles the DataChangesReceived event of the Subscription control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="SubscriptionNotificationEventArgs"/> instance containing the event data.</param>
		public void SubscriptionDataChangesReceived(object sender, DataChangesNotificationEventArgs e)
		{
			if (this.mShutdown)
			{
				return;
			}

			var pointTags = new Dictionary<Guid, PointTag>();
			//This method is executed in a Softing OPC UA framework thread and not the thread executing the Run() loop.
			var subscription = sender as ClientSubscription;
			if (subscription == null)
			{
				throw new Exception("Subscription_DataChangesReceived : Invalid Subscription");
			}

			int numMissingClientHandles = 0;
			int numMissingDisplayNames = 0;

			lock (this.SessionSubscriptionMonitoredItemsLock)
			{
				Dictionary<uint, ClientMonitoredItem> subscriptionMonitoredItemsDictionary = new Dictionary<uint, ClientMonitoredItem>();
				foreach (var monItem in subscription.MonitoredItems)
				{
					try
					{
						//Why does softing put in multiple entries on failures on reconnects of Monitored Items?
						//This might eventually lead to a monitoredItem client handle leak.
						//Need to look at the softing code.
						if (monItem != null)
						{
							uint clientHandle = monItem.ClientHandle;
							if (!(subscriptionMonitoredItemsDictionary.ContainsKey(clientHandle)))
							{

								subscriptionMonitoredItemsDictionary.Add(clientHandle, monItem);

							}
						}
					}
					catch (Exception cliHandleEx)
					{
						Logger.LogError(
							"OpcUaClientProcessor.SubscriptionDataChangesReceived ClientHandle Dictionary Creation exception: "
							+ cliHandleEx.Message);
					}
				}

				foreach (var monitoredItemNotification in e.DataChangeNotifications)
				{
					try
					{
						if (monitoredItemNotification != null)
						{
							ClientMonitoredItem monitoredItem;
							if (subscriptionMonitoredItemsDictionary.TryGetValue(monitoredItemNotification.ClientHandle, out monitoredItem))
							{
								PointTag pointTag = null;
								Guid pointTagGuid;
								this.GetIdentifierFromPointTagMarker(monitoredItem.DisplayName, out pointTagGuid);
								if (this.MonitoredTags.TryGetValue(pointTagGuid, out pointTag))
								{

									// set the ServerTimestamp to the system UtcNow
									monitoredItemNotification.Value.ServerTimestamp = DateTimeOffset.UtcNow.UtcDateTime;

									this.ConvertDataValueToTag(new DataValueEx(monitoredItemNotification.Value), pointTag);

									// ProSys Simulator includes monitored items multiple times
									if (pointTags.ContainsKey(pointTag.PointTagGuid))
									{
										pointTags[pointTag.PointTagGuid] = pointTag;
									}
									else
									{
										pointTags.Add(pointTag.PointTagGuid, pointTag);
									}
								}
								else
								{
									numMissingDisplayNames++;
								}
							}
						}
						else
						{
							numMissingClientHandles++;
						}

					}
					catch (Exception processNotificationEx)
					{
						Logger.LogError("OpcUaClientProcessor.SubscriptionDataChangesReceived Process Notification exception: " + processNotificationEx.Message);
					}
				}
			}

			if (numMissingClientHandles > 0)
			{
				Logger.LogError("OpcUaClientThread.SubscriptionDataChangesReceived can't find corresponding MonitoredItem for " + numMissingClientHandles + " Handles");
			}
			if (numMissingDisplayNames > 0)
			{
				Logger.LogError("OpcUaClientThread.SubscriptionDataChangesReceived can't find corresponding MonitoredTag for " + numMissingDisplayNames + " MonitoredItem.DisplayName ");
			}
			ThreadSharedData.Instance().ExternalUpdateTags(pointTags);
		}

		/// <summary>
		/// Adds a MonitoredItem for a given node. This method tries to find a previously created subscription to which to attach the
		/// MonitoredItem, based on the ServerEndPoint and PublishingInterval. If one cannot be found, the MonitoredItem is created
		/// against a new Session and/or a new Subscription are created as necessary.
		/// </summary>
		/// <param name="pointTag">The point tag.</param>
		protected bool AddMonitoredItemForPointTag(PointTag pointTag)
		{
			ClientSession session = null;
			ClientSubscription subscription = null;
			NodeId nodeId = null;
			string nodeIdStr = pointTag.OpcUaNodeId;
			string publishingIntervalStr = "Default";

			lock (this.SessionSubscriptionMonitoredItemsLock)
			{

				if (string.IsNullOrEmpty(nodeIdStr))
				{
					Logger.LogWarning(
						"OpcUaClientProcessor.AddMonitoredItemsForPointTag : Node Id Parameters missing for PointTag: "
						+ Convert.ToString(pointTag.PointTagGuid));
					return false;
				}

				nodeId = OpcUaTags.GetNodeId(nodeIdStr);
				if (nodeId == null)
				{
					Logger.LogError("OpcUaClientProcessor.AddMonitoredItemForPointTag : Invalid NodeId: " + nodeIdStr);
					return false;
				}


				if (pointTag.OpcUaPublishingInterval != null)
				{
					publishingIntervalStr = Convert.ToString(pointTag.OpcUaPublishingInterval);
				}

				subscription = this.SubscriptionAndSessionDictionary.GetSubscription(
					pointTag.SiteGuid,
					pointTag.OpcUaServerGuid,
					pointTag.OpcUaPublishingInterval);

				if (subscription != null)
				{
					session = subscription.Session;
				}
				else
				{
					session = this.SubscriptionAndSessionDictionary.GetSession(pointTag.SiteGuid, pointTag.OpcUaServerGuid);
				}

				if (session == null)
				{
					session = OpcUaTags.CreateSessionForPointTag(application, pointTag);
					session.SessionName = pointTag.OpcUaServerGuid.ToString();
					session.StateChanged += this.SessionStateChanged;
				}

				//Create an identifier tag to attach to the MonitoredItem. This tag will be returned by the MonitoredItem Event Notification, thus eliminating lookups between NodeIds and application record guids.
				string pointTagMarker = this.GetPointTagMarker(pointTag);


				if (subscription == null)
				{
					subscription = new ClientSubscription(session, "Site_" + pointTag.SiteGuid + "_" + publishingIntervalStr + "_" + Guid.NewGuid());
					subscription.DataChangesReceived += this.SubscriptionDataChangesReceived;
					subscription.StateChanged += this.SubscriptionStateChanged;
					if ((pointTag.OpcUaPublishingInterval != null) && (pointTag.OpcUaPublishingInterval > 0))
					{
						subscription.PublishingInterval = Convert.ToInt32(pointTag.OpcUaPublishingInterval);
					}


					this.SubscriptionAndSessionDictionary.AddSubscription(
						pointTag.SiteGuid,
						pointTag.OpcUaServerGuid,
						pointTag.OpcUaPublishingInterval,
						subscription);
				}

				//Add a MonitoredItem for the node
				var monitoredItem = new ClientMonitoredItem(subscription, nodeId, pointTag.PointTagGuid.ToString(), 13, null);

				monitoredItem.StateChanged += this.MonitoredItemStateChanged;

				this.MonitoredItemDictionary.Add(pointTag.PointTagGuid, monitoredItem);

				if ((pointTag.OpcUaNodeId != nodeId.ToString()))
				{
					Logger.LogWarning(
						"OpcUaClientProcessor.AddMonitoredItemForPointTag : NodeId of PointTag ("
						+ Convert.ToString(pointTag.PointTagGuid)
						+ ") does not match the NodeId of the node. The OPC UA address fields of the PointTag records might need to be refreshed.");
				}
			}

			return true;
		}

		public void OutputOpcCommand(SecurityClass security, PointTag pointTag)
		{
			if (security == null || pointTag == null)
			{
				throw new ArgumentNullException("OpcUaClientProcessor.OutputOPCCommand");
			}

			if (null == ThreadSharedData.Instance().GetPointTag(pointTag.PointTagGuid))
			{
				throw new Exception("OpcUaClientProcessor.OutputOPCCommand Point not found for " + pointTag.PointID + "." + pointTag.PointTagGuid);
			}

			var nodeIdStr = pointTag.OpcUaNodeId;
			var writeValue = new WriteValue { AttributeId = 13 };
			writeValue.NodeId = OpcUaTags.GetNodeId(nodeIdStr);
			if (writeValue.NodeId == null)
			{
				throw new Exception("OpcUaClientProcessor.OutputOPCCommand Error getting NodeId for " + pointTag.PointID + "." + pointTag.PointTagGuid);
			}
			var dataValue = new DataValue();
			if (pointTag.Value != null)
			{
				if (pointTag.Value.GetType().IsValueType)
				{
					if (pointTag.ValueTypeString == "System.DateTimeOffset")
					{
						dataValue.Value = ((DateTimeOffset)pointTag.Value).UtcDateTime;
					}
					else
					{
						dataValue.Value = pointTag.Value;
					}
				}
				else if (pointTag.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
				{
					if ((pointTag.Value as PointCommandStatusListReference).CurrentValue.HasValue)
					{
						dataValue.Value = (pointTag.Value as PointCommandStatusListReference).CurrentValue.Value;
					}
				}
				else if (pointTag.ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
				{
					if ((pointTag.Value as DeviceAlarmMapReference).CurrentValue.HasValue)
					{
						dataValue.Value = (pointTag.Value as DeviceAlarmMapReference).CurrentValue.Value;
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
			}

			try
			{
				OpcUaTags.ConvertDataValue(pointTag, new DataValueEx(dataValue));
			}
			catch (Exception)
			{
				pointTag.Status = StatusCodes.BadTypeMismatch;
				this.UpdateOpcOutputTagWithOverride(pointTag);
				return;
			}


			writeValue.Value = dataValue;

			// If GoodLocalOverride cannot be written to sever then output should fail
			writeValue.Value.StatusCode = new StatusCode(pointTag.OpcStatusCodeBits);

			// This is done prior to output so that if it is an override, no more writes will be processed
			this.UpdateOpcOutputTagWithOverride(pointTag);

			lock (this.LockObject)
			{
				using (var session = OpcUaTags.CreateSessionForPointTag(application, pointTag))
				{
					try
					{
						session.Connect(false, true);
						var statusCode = session.Write(writeValue);

						if (statusCode != null)
						{
							pointTag.Status = statusCode.Code;
						}
						else
						{
							pointTag.Status = StatusCodes.Bad;
						}
					}
					catch (Exception)
					{
						pointTag.Status = StatusCodes.Bad;
					}
				}
			}

			// Update again if statusCode is bad
			if (pointTag.IsBad())
			{
				this.UpdateOpcOutputTagWithOverride(pointTag);
			}
		}

		public void UpdateOpcOutputTagWithoutOverride(PointTag pointTag)
		{
			this.OutputTags.UpdateOutputTagWithoutOverride(pointTag);
		}

		public void UpdateOpcOutputTagWithOverride(PointTag pointTag)
		{
			this.OutputTags.UpdateOutputTagWithOverride(pointTag);
		}

		public void RefreshPointTag(PointTag pointTag, PointTag newTag)
		{
			lock (this.SessionSubscriptionMonitoredItemsLock)
			{
				ClientMonitoredItem monitoredItem = null;
				if (this.MonitoredItemDictionary.TryGetValue(pointTag.IdentityGuid, out monitoredItem))
				{
					if (monitoredItem.LastValue != null)
					{
						// The ServerTimestamp should reflect current time.
						monitoredItem.LastValue.ServerTimestamp = DateTimeOffset.UtcNow.UtcDateTime;
						this.ConvertDataValueToTag(monitoredItem.LastValue, pointTag);
					}
					else
					{
						pointTag.Status = newTag.Status;
						pointTag.Value = newTag.Value;
						pointTag.ServerTimeStamp = newTag.ServerTimeStamp;
						pointTag.SourceTimeStamp = newTag.SourceTimeStamp;
					}
				}
				else
				{
					// the forced status may of been cleared so if so set the status
					if (pointTag.Status != newTag.Status)
					{
						pointTag.Status = newTag.Status;
						pointTag.Value = newTag.Value;
						pointTag.ServerTimeStamp = newTag.ServerTimeStamp;
						pointTag.SourceTimeStamp = newTag.SourceTimeStamp;
					}
				}
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
	}
}
