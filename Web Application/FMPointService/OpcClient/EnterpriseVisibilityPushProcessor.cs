
namespace FMPointService.OpcClient
{
	using ThreadSupport;
	using InProcLogging;
	using Opc.Ua;
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.IO;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Threading;
	using System.Xml;
	using System.Xml.Serialization;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.UtilityObjects;
	using FMBusinessObjects.Constants;
	using FMPointCommon;
	using Softing.Opc.Ua.Client;
	using Softing.Opc.Ua.Configuration;
   using System.Runtime.Remoting.Messaging;

   public class EnterpriseVisibilityPushProcessor : SrmThread
	{
		protected UaApplication application;

		protected static EnterpriseVisibilityPushProcessor inst = null;

		protected readonly string host;

		protected readonly string port;

		protected SecurityClass security;

		private enum ProcessorEvents
		{
			Push = 0,
			Timeout = WaitHandle.WaitTimeout
		}

		protected bool successfulWrite = true;

		protected bool sendAll = true;


		protected static readonly AutoResetEvent PushProcessingEvent = new AutoResetEvent(false);

		protected Mutex PointTagDataMutex = new Mutex(initiallyOwned: false, name: SynchronizationConstants.PointTagDataMutexName);

		protected EnterpriseVisibilityPushProcessor(string host, string port)
		{
			this.host = host;
			this.port = port;
			security = ThreadSharedData.Instance().Login("SiteAdmin");
		}

		public static EnterpriseVisibilityPushProcessor Instance(string host, string port)
		{
			if (inst == null)
			{
				inst = new EnterpriseVisibilityPushProcessor(host, port);
			}
			return inst;
		}

		public static EnterpriseVisibilityPushProcessor Instance()
		{
			if (inst == null)
			{
				throw new Exception("EnterpriseVisibilityPushProcessor not initialized");
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
				CertificateValidator validator = (CertificateValidator) sender;
				this.HandleCertificateValidationError(validator, e);
			}
			catch (Exception ex)
			{
				Logger.LogError("EnterpriseVisibilityPushProcessor.Application_CertificateValidation: " + ex);
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

			bool acceptNonValidateCertificates = Convert.ToBoolean(ConfigurationManager.AppSettings["OpcUaAcceptNonValidatedCertificates"]);
			if (acceptNonValidateCertificates)
			{
				e.Accept = true;
				Logger.LogError("EnterpriseVisibilityPushProcessor.HandleCertificateValidationError: Untrusted certificate accepted. " + buffer.ToString());
			}
		}

		/// <summary>
		/// Initial OPC UA node monitoring configuration
		/// </summary>
		protected void Initialize()
		{
         ApplicationConfigurationBuilderEx configuration = FMPointService.LoadApplicationConfiguration("FMPointService EnterpriseVisibility").Result;

			lock (FMPointService.SoftingInitializationLock)
			{
				this.application = UaApplication.Create(configuration).Result;
			}
		}

		public override void Run()
		{
			WaitHandle[] events = { PushProcessingEvent };

			try
			{
				this.Initialize();

            while (this.mShutdown != true)
				{
					try
					{
						if (this.mShutdown)
						{
							break;
						}
						this.SetupSession();
						if (this.mShutdown)
						{
							break;
						}
						this.WriteValues();
						var eventThatSignaled = (ProcessorEvents) WaitHandle.WaitAny(events, 1000);

						if (eventThatSignaled == ProcessorEvents.Push)
						{
							this.lastWriteTime = 0;
						}
					}
					catch (Exception ex)
					{
						Logger.LogError("EnterpriseVisibilityPushProcessor.ProcessScan Inner Loop Exception: " + ex);
					}
				}

				this.DropSession();
			}
			catch (Exception ex)
			{
				Logger.LogError("EnterpriseVisibilityPushProcessor.ProcessScan exception: " + ex);
			}
		}

		protected ClientSession CreateSession(EnterpriseVisibilityConnectionInformation connInfo)
		{
			ClientSession opcuaSession = null;

			if (string.IsNullOrEmpty(connInfo.EnterpriseVisibilityOpcUaServerUrl))
			{
				return null;
			}

			opcuaSession = application.CreateSession(connInfo.EnterpriseVisibilityOpcUaServerUrl,
				connInfo.SecurityMode,
				connInfo.SecurityPolicy,
				connInfo.MessageEncoding,
				connInfo.UserIdentity,
				null);


			return opcuaSession;
		}

		protected EnterpriseVisibilityConnectionInformation sessionConnectionInfo = null;

		protected ClientSession pushSession = null;

		protected void DropSession()
		{
			if (this.pushSession != null)
			{
				try
				{
					this.pushSession.Disconnect(false);
					this.pushSession.Dispose();
				}
				catch
				{
				}
				this.pushSession = null;
			}
			this.sessionConnectionInfo = null;
		}



		protected EnterpriseVisibilityConnectionInformation CheckForSessionInfo()
		{
			//Need to plug in information that we get from tblConfigurationSettings
			EnterpriseVisibilityConnectionInformation ret = new EnterpriseVisibilityConnectionInformation(security);
			return ret;
		}

		protected void SetupSession()
		{
			EnterpriseVisibilityConnectionInformation connectionInformation = CheckForSessionInfo();
			if (connectionInformation != null)
			{
				if (sessionConnectionInfo != null)
				{
					if (!sessionConnectionInfo.SessionInfoEqual(connectionInformation)
					|| this.pushSession == null)
					{
						DropSession();	

						if (!connectionInformation.IsEnterprise
						&& connectionInformation.EnterpriseVisibilityOpcUaEnabled)
						{
							this.pushSession = CreateSession(connectionInformation);
						}
					}

					sessionConnectionInfo = connectionInformation;
				}
				else
				{
					sessionConnectionInfo = connectionInformation;
					if (!connectionInformation.IsEnterprise
					&& connectionInformation.EnterpriseVisibilityOpcUaEnabled)
					{
						this.pushSession = CreateSession(sessionConnectionInfo);
					}
				}
			}

			if(this.pushSession != null)
			{
				this.pushSession.StateChanged += this.StateChanged;
			}
		}

		public void StateChanged(object sender, EventArgs e)
		{
			if(sender is ClientSession
			&& (sender as ClientSession).CurrentState != State.Active)
			{
				this.sendAll = true;				
			}
		}

		protected long prevWriteTime = 0;

		protected long lastWriteTime = 0;
		protected bool IsTimeForNextWrite()
		{
			bool timeForWrite = false;
			long currentTime = HighPerformanceTimer.Now;
			if (lastWriteTime != 0)
			{
				double pushPeriodInSeconds = sessionConnectionInfo.EnterpriseVisibilityOpcUaPushPeriodInMinutes * 60.00;
				long writeIntervalTicks = HighPerformanceTimer.convertToTicks(pushPeriodInSeconds);
				long nextWriteTime = lastWriteTime + writeIntervalTicks;
				var ticksToSleep = nextWriteTime - currentTime;
				double sleepTimeDouble = HighPerformanceTimer.convertToSeconds(ticksToSleep) * 1000.00;
				if (sleepTimeDouble < 1000.00)
				{
					prevWriteTime = lastWriteTime;
					lastWriteTime = currentTime;
					timeForWrite = true;
				}
			}
			else
			{
				prevWriteTime = lastWriteTime;
				lastWriteTime = currentTime;
				timeForWrite = true;
			}
			return timeForWrite;
		}

		protected bool Write(List<WriteValue> valuesToWrite)
		{
			bool ret = true;

			if (valuesToWrite != null && valuesToWrite.Count > 0)
			{
				if ( this.pushSession == null)
				{
					Logger.LogWarning("EnterpriseVisibilityPushProcessor.Write: pushSession is null.");
					return false;
				}				
				try
				{
					var hasHandle = false;
					try
					{
						bool waitResult = this.PointTagDataMutex.WaitOne(30000); // 30 second wait
						if (!waitResult)
						{
							Logger.LogWarning("EnterpriseVisibilityPushProcessor.Write: waited too long on sync to finish point data");
							return false;
						}

						hasHandle = true;
					}
					catch (AbandonedMutexException)
					{
						hasHandle = true;
					}

					IList<StatusCode> status;

					try
					{
						status = this.pushSession.Write(valuesToWrite);
					}
					finally
					{
						if (hasHandle)
						{
							this.PointTagDataMutex.ReleaseMutex();
						}
					}
               for (int i = 0; i < status.Count; i++)
					{
						if(valuesToWrite[i] == null)
						{
                     Logger.LogWarning($"EnterpriseVisibilityPushProcessor.Write: valuesToWrite[{i}] is null");
                     continue;
						}
						if (valuesToWrite[i].NodeId == null)
						{
                     Logger.LogWarning($"EnterpriseVisibilityPushProcessor.Write: valuesToWrite[{i}].NodeId is null");
                     continue;
						}
						bool tag = true;
						string identifier = valuesToWrite[i].NodeId.Identifier as string;
						if (identifier == null) 
						{ 

							if (valuesToWrite[i].NodeId.Identifier == null)
							{
								Logger.LogWarning($"EnterpriseVisibilityPushProcessor.Write: valuesToWrite[{i}].NodeId.Identifier is null.");
							}
							else
							{
                        Logger.LogWarning($"EnterpriseVisibilityPushProcessor.Write: valuesToWrite[{i}].NodeId.Identifier is not a string.");

                     }
							continue;
                  }
						Guid guid;
						if (PointManager.IsTagNodeID(identifier))
						{
							PointManager.ParseTagNodeID(identifier, out guid);
						}
						else if (PointManager.IsAlarmSourceTagNodeID(identifier))
						{
							Guid pointGuid = Guid.Empty;
							PointManager.ParseAlarmSourceTagNodeID(identifier, out pointGuid, out guid);
						}
						else if (PointManager.IsSettingNodeID(identifier))
						{
							Guid pointGuid = Guid.Empty;
							string propertyID = string.Empty;
							PointManager.ParseSettingNodeID(identifier, out pointGuid, out guid, out propertyID);
							tag = false;
						}
						else
						{
							guid = Guid.Empty;
						}

						if (status[i] == null) 
						{
                     Logger.LogWarning($"EnterpriseVisibilityPushProcessor.Write: status is null for NodeId {valuesToWrite[i].NodeId.ToString()}.");

                  }
                  else if (StatusCode.IsBad(status[i]))
						{
							Logger.LogWarning($"EnterpriseVisibilityPushProcessor.Write: Failed to write NodeId {valuesToWrite[i].NodeId.ToString()}.");
							ThreadSharedData.Instance().SetWrittenToEnterprise(tag, guid, false);
						}
					}

				}
				catch (Exception e)
				{
					string message = e.Message;
					var innerException = e.InnerException;
					while(innerException != null)
					{
						message = message + " " + innerException.Message;
						innerException = innerException.InnerException;
					}	
					
					Logger.LogError("EnterpriseVisibilityPushProcessor.Write: Exception Writing " + valuesToWrite.Count + " Tags : " + message);

					ret = false;
				}
			}
			return ret;
		}

		protected void PopulateValuesToWrite(ref int numberOfTagsToWrite, ref List<WriteValue> valuesToWrite, Point point)
		{
			if (valuesToWrite == null || point == null)
			{
				return;
			}
         if (point.ID == null)
         {
            Logger.LogWarning("EnterpriseVisibilityPushProcessor.PopulateValuesToWrite: Point ID is null");
	      }
         if (point.Tags == null)
			{
				Logger.LogWarning($"EnterpriseVisibilityPushProcessor.PopulateValuesToWrite: Point Tags is null. Point ID = {point.ID}");
			}
			else
			{
				foreach (var tag in point.Tags.Values)
				{
					if (tag == null)
					{
						Logger.LogWarning($"EnterpriseVisibilityPushProcessor.PopulateValuesToWrite: Point tag is null. Point ID = {point.ID}");
						continue;
					}

					if (prevWriteTime == 0
					|| !tag.WrittenToEnterprise
					|| HighPerformanceTimer.convertToTicks(tag.ServerTimeStamp.DateTime.ToLocalTime()) > prevWriteTime)
					{

						var writeValue = new WriteValue { AttributeId = 13 };

						writeValue.NodeId = new NodeId(PointManager.CreateTagNodeID(tag.PointTagGuid), 2);
						if (tag.Value is PointCommandStatusListReference)
						{
							writeValue.Value.Value = tag.ValueXml;
						}
						else if (tag.Value is DeviceAlarmMapReference)
						{
							writeValue.Value.Value = tag.ValueXml;
						}
						else if (tag.Value is DateTimeOffset)
						{
							writeValue.Value.Value = ((DateTimeOffset)tag.Value).DateTime;
						}
						else if (tag.Value is TimeSpan)
						{
							writeValue.Value.Value = ((TimeSpan)tag.Value).Ticks;
						}
						else if (tag.Value is double
						|| tag.Value is float)
						{
							var value = new EnterpriseVisibilityData(tag.EngineeringUnitsType, tag.Units, tag.Value, tag.DecimalPlaces, tag.Maximum, tag.Minimum);
							var xmlserializer = CachingXmlSerializerFactory.Create(value.GetType());
							var stringWriter = new StringWriter();
							var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
							using (var writer = XmlWriter.Create(stringWriter, settings))
							{
								var emptyNameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
								xmlserializer.Serialize(writer, value, emptyNameSpaces);
								writeValue.Value.Value = stringWriter.ToString();
							}
						}
						else
						{
							writeValue.Value.Value = tag.Value;
						}

						writeValue.Value.StatusCode = new StatusCode((uint)tag.Status);
						writeValue.Value.ServerTimestamp = new DateTime(tag.ServerTimeStamp.Ticks, DateTimeKind.Utc);
						writeValue.Value.SourceTimestamp = new DateTime(tag.SourceTimeStamp.Ticks, DateTimeKind.Utc);
						valuesToWrite.Add(writeValue);
						numberOfTagsToWrite++;
					}

					foreach (var alarm in tag.Alarms.Values)
					{
						foreach (var pointTagAlarmStatus in alarm.AlarmStatus.Values)
						{
							if (prevWriteTime == 0
							|| !pointTagAlarmStatus.WrittenToEnterprise
							|| HighPerformanceTimer.convertToTicks(tag.ServerTimeStamp.DateTime.ToLocalTime()) > prevWriteTime)
							{
								var writeValue = new WriteValue { AttributeId = 13 };

								writeValue.NodeId = new NodeId(PointManager.CreatePointTagAlarmStatusNodeID(pointTagAlarmStatus.PointTagAlarmStatusGuid), 2);

								writeValue.Value.StatusCode = StatusCodes.Good;
								using (MemoryStream stream = new MemoryStream())
								{
									DataContractSerializer serializer = new DataContractSerializer(pointTagAlarmStatus.GetType());
									serializer.WriteObject(stream, pointTagAlarmStatus);
									writeValue.Value.Value = new UTF8Encoding().GetString(stream.ToArray());
								}
								writeValue.Value.ServerTimestamp = new DateTime(pointTagAlarmStatus.UpdatedDate.Ticks, DateTimeKind.Utc);
								writeValue.Value.SourceTimestamp = new DateTime(pointTagAlarmStatus.UpdatedDate.Ticks, DateTimeKind.Utc);
								valuesToWrite.Add(writeValue);
							}
						}
					}
				}
			}

			if (point.Properties == null)
			{
				Logger.LogWarning($"EnterpriseVisibilityPushProcessor.PopulateValuesToWrite: Point Properties is null. Point ID = {point.ID}");
			}
			else
			{

				foreach (var property in point.Properties.Values)
				{
					if (property == null)
					{
						Logger.LogWarning($"EnterpriseVisibilityPushProcessor.PopulateValuesToWrite: Point property is null. Point ID = {point.ID}");
						continue;
					}
					if (prevWriteTime == 0
					|| !property.WrittenToEnterprise
					|| HighPerformanceTimer.convertToTicks(property.UpdatedDate.DateTime.ToLocalTime()) > prevWriteTime)
					{
						if (property.Value is MovementData || property.Value is MovementModuleSettings)
						{
							var writeValue = new WriteValue { AttributeId = Opc.Ua.Attributes.Value };
							writeValue.NodeId = new NodeId(PointManager.CreateSettingNodeID(property.PointGuid, property.PointPropertyGuid, property.ID), 2);
							writeValue.Value.StatusCode = StatusCodes.Good;
							writeValue.Value.Value = property.ValueXml;
							writeValue.Value.ServerTimestamp = new DateTime(property.UpdatedDate.Ticks, DateTimeKind.Utc);
							writeValue.Value.SourceTimestamp = new DateTime(property.UpdatedDate.Ticks, DateTimeKind.Utc);
							valuesToWrite.Add(writeValue);

							if(property.Value is MovementData)
							{
								Type movementDataType = typeof(MovementData);
								IList<PropertyInfo> propertyInfoList = new List<PropertyInfo>(movementDataType.GetProperties());

								foreach (PropertyInfo propertyInfo in propertyInfoList)
								{
									object pointValueList = propertyInfo.GetValue(property.Value, null);

									if (pointValueList is List<PointValue>)
									{

										foreach (var pointValue in pointValueList as List<PointValue>)
										{

											if (pointValue == null)
											{
												continue;
											}

											numberOfTagsToWrite++;
										}
									}
								}
							}
							else
							{
								numberOfTagsToWrite++;
							}
						}
					}
				}
			}
		}

		protected PointTag FindTagById(Dictionary<Guid,PointTag> tagDictionary, string tagId)
		{
			if (tagDictionary == null)
			{
				return null;
			}
			foreach(var tag in tagDictionary.Values)
			{
				if(tag != null && tag.ID == tagId)
				{
					return tag;
				}
			}
			return null;
		}


		protected void WriteValues()
		{
			// When EnterpriseVisibilityOpcUaEnabled transitions to true, send all values;
			if (this.sessionConnectionInfo != null
			&& !this.sessionConnectionInfo.IsEnterprise
			&& this.sessionConnectionInfo.EnterpriseVisibilityOpcUaEnabled)
			{
				if (IsTimeForNextWrite())
				{

					var initialSiteGuid = this.security.SiteGuid;

					try
					{
						int tagsWritten = 0;
						HashSet<Guid> siteSet = new HashSet<Guid>();
						var pointManagerAlarmAndEvents = new PointManagerAlarmsAndEvents();
						Logger.LogDebug("EnterpriseVisibilityPushProcessor.WriteValues Push Initiated");

						if (this.pushSession == null)
						{
							SetupSession();
						}
						if (this.pushSession == null)
						{
                     Logger.LogError("EnterpriseVisibilityPushProcessor.WriteValues: pushSession is null.");
                     this.sendAll = true;
							return;
                  }
                  if (this.pushSession.CurrentState == State.Disconnected)
						{
							if (this.pushSession.TargetState != State.Disconnected)
							{
								this.pushSession.Disconnect(false);
							}
							this.pushSession.Connect(false, true);
						}

						if (this.pushSession.CurrentState == State.Disconnected
						|| this.pushSession.CurrentState == State.Connecting)
						{
							Logger.LogError("EnterpriseVisibilityPushProcessor.WriteValues pushSession won't connect");
							this.sendAll = true;
						}
						else if ((this.pushSession.CurrentState == State.Connected
						|| this.pushSession.CurrentState == State.Active))
						{

							var pointDictionary = ThreadSharedData.Instance().GetPointDictionary(true);

							if (this.sendAll == true)
							{
								foreach (var point in pointDictionary.Values)
								{
									if (point != null)
									{
										foreach (var pointTag in point.Tags.Values)
										{
											pointTag.WrittenToEnterprise = false;
										}

										foreach (var pointProperty in point.Properties.Values)
										{
											pointProperty.WrittenToEnterprise = false;
										}
									}

								}

								this.sendAll = false;
							}

							var valuesToWrite = new List<WriteValue>();
							int numberOfTagsToWrite = 0;
							this.successfulWrite = true;

							foreach (var point in pointDictionary.Values)
							{

								if (this.mShutdown)
								{
									break;
								}

								if (point == null)
								{
                           Logger.LogWarning("EnterpriseVisibilityPushProcessor.WriteValues: point is null.");
                           continue;
								}

								if (!siteSet.Contains(point.SiteGuid))
								{
									siteSet.Add(point.SiteGuid);
									this.security.SiteGuid = point.SiteGuid;
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
										x => x.Add(this.security, pointManagerAlarmAndEvents.EnterpriseVisibilityPushInitiatedEvent()));
								}

								PopulateValuesToWrite(ref numberOfTagsToWrite, ref valuesToWrite, point);
								if (numberOfTagsToWrite >= sessionConnectionInfo.EnterpriseVisibilityOpcUaNumTagsPerSend)
								{
									if (!Write(valuesToWrite)
									&& successfulWrite)
									{
										successfulWrite = false;
										this.DropSession();
										this.sendAll = true;
										valuesToWrite.Clear();
										numberOfTagsToWrite = 0;
                              break;
									}
									else
									{
										tagsWritten += numberOfTagsToWrite;
									}

									valuesToWrite.Clear();
								}
							}

							if (!this.mShutdown
							&& successfulWrite
							&& !Write(valuesToWrite))
							{
								successfulWrite = false;
								this.DropSession();
								this.sendAll = true;
                        Logger.LogWarning("EnterpriseVisibilityPushProcessor.WriteValues : Dropping session due to Write issue.");
                     }
                     else
							{
								tagsWritten += numberOfTagsToWrite;
							}

							valuesToWrite.Clear();

							//Log to Alarm and Event Log successfulWrite Completion
							string successStr = successfulWrite ? "Success Tags Written " + tagsWritten.ToString() : "Failure";
							Logger.LogDebug("EnterpriseVisibilityPushProcessor.WriteValues Push Completed with " + successStr);
							foreach (var siteGuid in siteSet)
							{
								this.security.SiteGuid = siteGuid;
								FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
									x => x.Add(this.security, pointManagerAlarmAndEvents.EnterpriseVisibilityPushCompleteEvent(successStr)));
							}
						}
					}
					catch (Exception e)
					{
						string message = e.Message;
						var innerException = e.InnerException;
						while (innerException != null)
						{
							message = message + " " + innerException.Message;
							innerException = innerException.InnerException;
						}

						Logger.LogError("EnterpriseVisibilityPushProcessor.WriteValues Exception Writing Tags : " + message);
						this.DropSession();
						this.sendAll = true;
					}

					finally
					{
						this.security.SiteGuid = initialSiteGuid;
					}
				}
			}

			else
			{
				this.sendAll = true;
			}
		}  // end of writevalues

		/// <summary>
		/// Signals the EnterpriseVisibility processing task to push
		/// scheduled period timeout.
		/// </summary>
		public static void SignalPush()
		{
			if (PushProcessingEvent != null)
			{
				PushProcessingEvent.Set();
			}
		}

	}
}
