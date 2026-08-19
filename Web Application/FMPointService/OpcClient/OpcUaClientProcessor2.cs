namespace FMPointService.OpcClient
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using FMBusinessObjects.DataObjects;
	using ThreadSupport;
	using InProcLogging;
	using Opc.Ua;
	using Softing.Opc.Ua.Client;
	using Softing.Opc.Ua.Configuration;

	public class OpcUaClientProcessor2 : SrmThread
	{
		protected UaApplication application;

		protected static OpcUaClientProcessor2 inst = null;

		protected readonly string host;

		protected readonly string port;

		protected SessionsAndMonitoredTags sessions = new SessionsAndMonitoredTags();

		protected OpcUaTags MonitoredTags = new OpcUaTags();

		protected OpcUaTags OutputTags = new OpcUaTags();

		protected readonly AutoResetEvent TagDictionaryChangedEvent = new AutoResetEvent(false);

		protected Object LockObject = new object();

		protected bool enableUseLastKnownGood = false;

		protected OpcUaClientProcessor2(string host, string port, bool enableUseLastKnownGood)
		{
			this.host = host;
			this.port = port;
			this.enableUseLastKnownGood = enableUseLastKnownGood;
		}

		public static OpcUaClientProcessor2 Instance(string host, string port,bool enableUseLastKnownGood)
		{
			if (inst == null)
			{
				inst = new OpcUaClientProcessor2(host, port, enableUseLastKnownGood);
			}
			return inst;
		}

		public static OpcUaClientProcessor2 Instance()
		{
			if (inst == null)
			{
				throw new Exception("OpcUaClientProcessor2 not initialized");
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
				Logger.LogError("OpcUaClientProcessor2.Application_CertificateValidation: " + ex);
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
			ApplicationConfigurationBuilderEx configuration = FMPointService.LoadApplicationConfiguration("FMPointService Client").Result;

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

						this.CheckDeadbandValuesforUpdate();
					}
					catch (Exception ex)
					{
						Logger.LogError("OpcUaClientProcessor2.ProcessScan Inner Loop Exception: " + ex);
					}
				}

				this.DropAllOpcUaConnections();
			}
			catch (Exception ex)
			{
				Logger.LogError("OpcUaClientProcessor2.ProcessScan exception: " + ex);
			}
		}

		/// <summary>
		/// Drops all open OPC UA MonitoredItems, Subscriptions, and Sessions created by this service.
		/// </summary>
		protected void DropAllOpcUaConnections()
		{
			sessions.CleanUp();
		}


		protected void WriteValues()
		{
			lock (this.LockObject)
			{
				var sessionValueListDictionary = OutputTags.GetSessionWriteListDictionaryForWrites(this.application);

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

		protected void CheckDeadbandValuesforUpdate()
		{
			lock (this.LockObject)
			{
				ThreadSharedData.Instance().ProcessHoldoffDeadbandDictionary();
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
				Logger.LogDebug("OpcUaClientProcessor2.CheckTagChanges <AddMonitoredItems,DeletedMonitoredItems> = " + addedMonitoredTags.Count + ", " + deletedMonitoredTags.Count);
			}
			sessions.RemoveTags(deletedMonitoredTags);
			sessions.AddTags(this.application, this.enableUseLastKnownGood, addedMonitoredTags);

			// update the holdoff dictionary
			lock (this.LockObject)
			{
				ThreadSharedData.Instance().UpdateHoldoffDeadbandDictionarywithDeletedTags(deletedMonitoredTags);
			}


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

		protected void GetMonitoredTagsDifferences(out Dictionary<Guid, PointTag> addedMonitoredTags, out Dictionary<Guid, PointTag> deletedMonitoredTags)
		{
			var currentMonitoredTags= ThreadSharedData.Instance().GetMonitoredTagDictionary();
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
						dataValue.Value = ((DateTimeOffset) pointTag.Value).UtcDateTime;
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
			catch(Exception)
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


		public void RefreshPointTag(PointTag pointTag,PointTag newTag)
		{
			try
			{
				sessions.RefreshPointTag(pointTag);
			}
			catch
			{
				pointTag.Status = newTag.Status;
				pointTag.Value = newTag.Value;
				pointTag.ServerTimeStamp = newTag.ServerTimeStamp;
				pointTag.SourceTimeStamp = newTag.SourceTimeStamp;
			}
		}
	}
}
