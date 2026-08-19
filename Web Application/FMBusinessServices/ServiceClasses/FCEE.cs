namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Security;
	using System.Data.SqlClient;


	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMCore;

	using DataAccessLayer;
	using InternalClasses;
	using System.Collections.Generic;
	using System.ServiceModel;
	using FMBusinessObjects.Exceptions;
	using crypto;
	using System.Linq;
	using FMBusinessObjects.ChannelFactories;
	using Opc.Ua;
	using Newtonsoft.Json;
	using System.IO;
	using System.Net;
	using FMBusinessObjects.Constants;
	using System.Text.Json;
	using FMBusinessObjects.DataObjects.Message;
	using IsolationLevel = System.Transactions.IsolationLevel;
	using System.Runtime.Caching;
	using System.Web.Caching;
	using System.Web.Services.Description;
    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
    /// Summary description for FCEE
    /// </summary>
    [SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class FCEE : FMServiceBase, IFCEEServiceManager
	{
		private readonly string[] hex = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
		/// <summary>
		/// Delete FCEE messages that are older than the maximum number of days to retain logs
		/// specified for the site corresponding to the log
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeOldRecords(SecurityClass security)
		{
				if (security == null)
				{
					throw new ArgumentNullException(nameof(security));
				}

				if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				{
					throw new FMInsufficientRightsException();
				}

				using (var cmd = new SqlCommand())
				{
					try
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.CommandText = "usp_FCEEMessageDeleteOldRecords";
						this.ConsolidatedDA.ExecuteQuery(security, cmd);
					}
					catch (ConsolidatedDAException)
					{
						throw new Exception();
					}
				}
		}

		/// <summary>
		/// When a FCEE Value override/force is removed
		/// If the Heartbeat is good (or the value is from the past 24 hours), set the value to the last known good value
		/// Otherwise, set the value to null with data quality unknown
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="pointValue">The FCEE point value that needs to have its value and quality updated from force removal</param>
		public bool Refresh(SecurityClass security, PointValue pointValue)
		{
			bool useLastKnownGood = false;

			try
			{
				ConfigurationSettingsClass configurationSettings = new ConfigurationSettingsClass();
				string stenableUseLastKnownGood = configurationSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_UseLastKnownGoodStatus);

				if (stenableUseLastKnownGood == "1")
				{
					useLastKnownGood = true;
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("FCEE Service Message: " + e.Message, FMEventLogEntryType.Error);
			}

			Dictionary<Guid, FCEEMapping> mappingsOnPoint = this.EnumerateByPointGuid(security, pointValue.PointGuid);
			List<Guid> keysToDelete = new List<Guid>();

			foreach (var mapping in mappingsOnPoint.Values)
			{
				if (!GetTagsFromMapping(mapping).Contains(pointValue.ID))
				{
					keysToDelete.Add(mapping.FCEEMappingGuid);
				}
			}
			foreach (var key in keysToDelete)
			{
				mappingsOnPoint.Remove(key);
			}

			if (mappingsOnPoint == null || mappingsOnPoint.Count != 1) {
				if (mappingsOnPoint.Count > 1)
				{
					AlarmAndEventLogsClass alarmAndEventLogsClass = new AlarmAndEventLogsClass();
					FCEEEvents fceeEvents = new FCEEEvents();
					alarmAndEventLogsClass.Add(security, fceeEvents.mappingCollusionEvent(pointValue.PointID + "." + pointValue.ID));
				}


				return false;
			}

			List<FCEEMessage> newestMessages = this.GetNewestMessageForMapping(security, mappingsOnPoint.First().Value);
			if (newestMessages.Count == 0)
			{
				return false;
			}

			FCEEMessage newestMessage = newestMessages[0];
			MemoryStream memoryStream = new MemoryStream(newestMessage.BinaryData);

			var msgBody = memoryStream.ToArray();
			memoryStream.Seek(0, SeekOrigin.Begin);

			byte[] imeiNumber = new byte[15];
			if(msgBody.Length <= 21)
			{
				return false;
			}

			memoryStream.Read(imeiNumber, 0, 15);
			string imei = new string(imeiNumber.Select(c => (char)c).ToArray());
			memoryStream.Seek(0, SeekOrigin.Begin);

			FCEDevices fceDevices = new FCEDevices();
			var fceDevice = fceDevices.GetbyIMEI(security, imei);

			if(fceDevice == null)
			{
				return false;
			}

			fceDevice.SoftwareVersion = newestMessage.SoftwareVersion;

			ProcessMessage(security, fceDevice, memoryStream, (EdgeMessageType)newestMessage.MsgType, useLastKnownGood, false, newestMessage.Validity);

			return true;
		}

		/// <summary>
		/// Mappings
		/// </summary>
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid? Add(SecurityClass security, FCEEMapping fceeMapping)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if(fceeMapping.FCEEMappingGuid == Guid.Empty)
			{
				fceeMapping.FCEEMappingGuid = Guid.NewGuid();
			}

			using (var cmd = new SqlCommand())
			{
				fceeMapping.SetCreationStamp(security);
				fceeMapping.AutoGenerateInsertProcSQL(cmd, "[dbo].[gsp_FCEEMappingInsertByPK]");
				cmd.Parameters["@FCEEMappingGuid"].Direction = ParameterDirection.InputOutput;

				ConsolidatedDA.ExecuteQuery(security, cmd);

				fceeMapping.FCEEMappingGuid = new Guid(cmd.Parameters["@FCEEMappingGuid"].Value.ToString());
			}
			return fceeMapping.FCEEMappingGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, FCEEMapping fceeMapping)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var existingMapping = this.Get(security, fceeMapping.FCEEMappingGuid);
			if (existingMapping.IdentityGuid == Guid.Empty)
			{
				throw new Exception("FCEE Mapping not found for update.");
			}

			using (var cmd = new SqlCommand())
			{
				fceeMapping.SetModifyStamp(security);
				fceeMapping.AutoGenerateModifyProcSQL(cmd, "[dbo].[gsp_FCEEMappingUpdateByPK]");
				ConsolidatedDA.ExecuteQuery(security, cmd);

			}
		}

		public Guid? GetFCEEToPointGuid(SecurityClass security, string imeiNumber, int msgType, int index)
		{
			return null;
		}

		public Tuple<string, Guid, string, Guid, long, int?> GetMapping(SecurityClass security, string imeiNumber, int msgType, int index, int? device = null)
		{
			security.ThrowIfNull("security");

			using (var sqlCommand = new SqlCommand())
			{
				sqlCommand.CommandType = System.Data.CommandType.Text;
				sqlCommand.CommandText = "SELECT s.ID AS SiteID, s.SiteGuid, p.ID AS PointID, p.PointGuid, fm.TagSelection, fm._RowVersion AS RowVersion FROM [dbo].[tblFCEEMapping] fm"
													+ " LEFT JOIN [dbo].[tblPoint] p ON p.PointGuid = fm.PointGuid"
													+ " LEFT JOIN [dbo].[tblSites] s ON s.SiteGuid = p.SiteGuid"
													+ " LEFT JOIN [dbo].[tblFCEDevice] d ON d.FCEDeviceGuid = fm.FCEDeviceGuid"
													+ " WHERE d.[ImeiNumber] = @ImeiNumber AND [MsgType] = @MsgType AND [Index] = @Index"
													+ " AND [Device] " + (device != null ? "= @Device" : "IS NULL");
				sqlCommand.Parameters.AddWithValue("@ImeiNumber", imeiNumber);
				sqlCommand.Parameters.AddWithValue("@MsgType", msgType);
				sqlCommand.Parameters.AddWithValue("@Index", index);

				if (device != null)
				{
					sqlCommand.Parameters.AddWithValue("@Device", device);
				}

				var dataSet = ConsolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet != null
				&& dataSet.Tables.Count == 1
				&& dataSet.Tables[0].Rows.Count == 1)
				{
					var siteID = dataSet.Tables[0].Rows[0]["SiteID"] as string;
					var siteGuid = (Guid)dataSet.Tables[0].Rows[0]["SiteGuid"];
					var pointID = dataSet.Tables[0].Rows[0]["PointID"] as string;
					var pointGuid = (Guid)dataSet.Tables[0].Rows[0]["PointGuid"];
					var rowVersion = (Byte[])dataSet.Tables[0].Rows[0]["RowVersion"];
					var tagSelection = (int?)dataSet.Tables[0].Rows[0]["TagSelection"];

					return Tuple.Create(siteID, siteGuid, pointID, pointGuid, DataAccessLayerDBI.ConvertRowVersion(rowVersion), tagSelection);
				}
			}

			return null;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Dictionary<Guid, Tuple<string, Guid, string, Guid, long>> EnumerateBySiteGuid(SecurityClass security, Guid siteGuid)
		{
			security.ThrowIfNull("security");
			using (var sqlCommand = new SqlCommand())
			{
				sqlCommand.CommandType = System.Data.CommandType.Text;
				sqlCommand.CommandText = "SELECT s.ID AS SiteID, s.SiteGuid, p.ID AS PointID, p.PointGuid, p._RowVersion AS RowVersion, fm.FCEEMappingGuid, d.ImeiNumber FROM [dbo].[tblFCEEMapping] fm"
													+ " LEFT JOIN [dbo].[tblPoint] p ON p.PointGuid = fm.PointGuid"
													+ " LEFT JOIN [dbo].[tblSites] s ON s.SiteGuid = p.SiteGuid"
													+ " LEFT JOIN [dbo].[tblFCEDevice] d ON d.FCEDeviceGuid = fm.FCEDeviceGuid"
													+ " WHERE s.SiteGuid = @siteGuid";
				sqlCommand.Parameters.AddWithValue("@siteGuid", siteGuid);

				var dataSet = ConsolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet != null &&
					dataSet.Tables[0].Rows.Count > 0)
				{
					DataTable table = dataSet.Tables[0];
					Dictionary<Guid, Tuple<string, Guid, string, Guid, long>> fceeMappingDictionary = new Dictionary<Guid, Tuple<string, Guid, string, Guid, long>>();

					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						var ImeiNumber = row["ImeiNumber"] as string;
						var mappingSiteGuid = (Guid)row["SiteGuid"];
						var pointID = row["PointID"] as string;
						var pointGuid = (Guid)row["PointGuid"];
						var rowVersion = (Byte[])row["RowVersion"];
						var FCEEMappingGuid = (Guid)row["FCEEMappingGuid"];


						fceeMappingDictionary.Add(FCEEMappingGuid, Tuple.Create(ImeiNumber, mappingSiteGuid, pointID, pointGuid, DataAccessLayerDBI.ConvertRowVersion(rowVersion)));
					}
					return fceeMappingDictionary;
				}
			}
			return null;
		}

		public FCEEMapping Get(SecurityClass security, Guid mappingGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			DataSet set = null;
			var fceeMapping = new FCEEMapping();
			using (var cmd = new SqlCommand())
			{
				fceeMapping.EnumerateByFCEEMappingGuidSQL(cmd, mappingGuid);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}	

			DataTable table = set.Tables[0]; 
			if (table.Rows.Count > 0)
			{
				var row = set.Tables[0].Rows[0];
				fceeMapping.AutoLoad(row);
			}
			return fceeMapping;
		}

		public Dictionary<Guid, FCEEMapping> EnumerateBySiteGuid2(SecurityClass security, Guid siteGuid)
		{
			security.ThrowIfNull("security");
			using (var sqlCommand = new SqlCommand())
			{
				DataSet set = null;
				var fceeMapping = new FCEEMapping();

				using (var cmd = new SqlCommand())
				{
					fceeMapping.EnumerateBySiteGuidSQL(cmd, siteGuid);
					set = ConsolidatedDA.GetDataSet(cmd, security);
				}
				return PopulateDictionary(set);
			}
		}

		public Dictionary<Guid, FCEEMappingWithDevice> EnumerateBySiteGuidWithDevice(SecurityClass security, Guid siteGuid)
		{
			security.ThrowIfNull("security");
			using (var sqlCommand = new SqlCommand())
			{
				DataSet set = null;
				var fceeMapping = new FCEEMappingWithDevice();

				using (var cmd = new SqlCommand())
				{
					fceeMapping.EnumerateBySiteGuidSQL(cmd, siteGuid);
					set = ConsolidatedDA.GetDataSet(cmd, security);
				}
				return PopulateDictionaryWithDevice(set);
			}
		}

		public Dictionary<Guid, FCEEMapping> EnumerateByPointGuid(SecurityClass security, Guid pointGuid)
		{
			security.ThrowIfNull("security");
			using (var sqlCommand = new SqlCommand())
			{
				DataSet set = null;
				var fceeMapping = new FCEEMapping();

				using (var cmd = new SqlCommand())
				{
					fceeMapping.EnumerateByPointGuidSQL(cmd, pointGuid);
					set = ConsolidatedDA.GetDataSet(cmd, security);
				}
				return PopulateDictionary(set);
			}
		}

		public void Purge(SecurityClass security, Guid fceeMappingguid)
		{
			using (var sqlCommand = new SqlCommand())
			{
				sqlCommand.CommandType = System.Data.CommandType.Text;
				sqlCommand.CommandText = "DELETE FROM [dbo].[tblFCEEMapping]"
												+ " WHERE [dbo].[tblFCEEMapping].[FCEEMappingGuid]=@fceeMappingGuid";
				sqlCommand.Parameters.Add("@fceeMappingGuid", SqlDbType.UniqueIdentifier);
				sqlCommand.Parameters["@fceeMappingGuid"].Value = fceeMappingguid;

				ConsolidatedDA.ExecuteQuery(security, sqlCommand);
			}
		}

		public void UpdateFCEEMappings(SecurityClass security, List<FCEEMapping> fceeMappings)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}
			if (fceeMappings == null)
			{
				throw new ArgumentNullException("fcee mappings");
			}
			foreach (FCEEMapping fceeMapping in fceeMappings)
			{
				if ((int)fceeMapping.MsgType < 16 || (int)fceeMapping.MsgType > 19)
				{
					fceeMapping.Device = null;
				}
				var existingMapping = this.Get(security, fceeMapping.FCEEMappingGuid);
				if (existingMapping.IdentityGuid == Guid.Empty)
				{
					this.Add(security, fceeMapping);
				}
				else
				{
					this.Modify(security, fceeMapping);
				}
			}
		}

		public Tuple<bool, int, byte[]> ProcessRequestHandler(SecurityClass security, bool pointStatusProcessing, MemoryStream memoryStream, string contentType, string httpMethod) {
			var isSuccess = true;
			var httpStatusCode = 200;
			var responseBody = new byte[0];

			try
			{
				var msgBody = memoryStream.ToArray();
				memoryStream.Seek(0, SeekOrigin.Begin);

				if (httpMethod == "GET" && msgBody.Length > 1) //FCE Device is asking for new configuration
				{
					byte[] imeiNumber = new byte[15];
					memoryStream.Read(imeiNumber, 0, 15);
					string imeiString = new string(imeiNumber.Select(c => (char)c).ToArray());
					responseBody = buildConfigurationResponseBody(security, imeiString);
				}

				else if (contentType != null
						&& msgBody.Length > 21
						&& (contentType == System.Net.Mime.MediaTypeNames.Image.Jpeg
						|| contentType == System.Net.Mime.MediaTypeNames.Application.Octet))
				{
					byte[] imeiNumber = new byte[15];
					byte[] timestamp = new byte[4];
					byte[] msgExt = new byte[1];
					byte[] msgType = new byte[1];
					byte[] index = new byte[1];

					memoryStream.Read(imeiNumber, 0, 15);
					memoryStream.Read(timestamp, 0, 4);
					memoryStream.Read(msgExt, 0, 1);
					memoryStream.Read(msgType, 0, 1);
					memoryStream.Read(index, 0, 1);

					var edgeMsgType = (EdgeMessageType)Convert.ToInt16(msgType[0]);
					string imei = new string(imeiNumber.Select(c => (char)c).ToArray());
					var tstmp = new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(BitConverter.ToInt32(timestamp.Reverse().ToArray(), 0)));
					int msgIndex = (int)index[0];

					FCEDevices fceDevices = new FCEDevices();
					var fceDevice = fceDevices.GetbyIMEI(security, imei);


					memoryStream.Seek(0, SeekOrigin.Begin);

					// get the lastknowngood configuration
					bool useLastKnownGood = false;

					try
					{
						var cache = MemoryCache.Default;
						string stenableUseLastKnownGood = cache["fceeUseLastKnownGood"] as string;
						if (stenableUseLastKnownGood == null)
						{
						ConfigurationSettingsClass configurationSettings = new ConfigurationSettingsClass();
						stenableUseLastKnownGood = configurationSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_UseLastKnownGoodStatus);
							var cacheItemPolicy = new CacheItemPolicy()
							{
									AbsoluteExpiration = DateTime.Now.AddMinutes(10)
							};
							cache.Set("fceeUseLastKnownGood", stenableUseLastKnownGood, cacheItemPolicy);
						}
									
						if (stenableUseLastKnownGood == "1")
						{
							useLastKnownGood = true;
						}
					}
					catch (Exception e)
					{
						FMEventLog eventLog = new FMEventLog();
						eventLog.WriteEntry("FCEE Service Message: " + e.Message, FMEventLogEntryType.Error);
					}
					var validity = true;
					var edgeMsg = ProcessMessage(security,fceDevice, memoryStream, edgeMsgType, useLastKnownGood, pointStatusProcessing, validity);

					string jsonEdgeData = null;
					if (edgeMsg != null)
					{
						// for some Microsoft reason, instantiating JsonSerializerOptions is expensive
						var cache = MemoryCache.Default;
						var options = cache["FCEEJsonSerializerOptions"] as JsonSerializerOptions;
						if (options == null)
						{
							options = new JsonSerializerOptions(JsonSerializerDefaults.General);
							options.Converters.Add(new DoubleJsonConverter()); // rounds the decimal representation of the double, which can look strange to users
							var cacheItemPolicy = new CacheItemPolicy()
							{
									Priority = System.Runtime.Caching.CacheItemPriority.NotRemovable
							};
							cache.Add("FCEEJsonSerializerOptions", options, cacheItemPolicy);
						}
						jsonEdgeData = System.Text.Json.JsonSerializer.Serialize(edgeMsg, edgeMsg.GetType(), options);
					} 
					else 
					{
						FMEventLog eventLog = new FMEventLog();
						eventLog.WriteEntry("No Mapping for : " + imei + "." + edgeMsgType + "." + msgIndex, FMEventLogEntryType.Error);
					}
		
					EdgeData parsedData = edgeMsg as EdgeData;
					byte? device = parsedData.Device;
								
					FCEEMessage messageToQueue = new FCEEMessage(imei,tstmp, (EDGEMESSAGETYPE)((ushort)msgExt[0] << 8 | (ushort)edgeMsgType), msgIndex,device, msgBody, jsonEdgeData, fceDevice.SoftwareVersion, validity);
					QueueMessageForDatabase(security, messageToQueue);

					httpStatusCode = GetHttpStatusCode(security, fceDevice);
				}
			}
			catch(Exception e)
			{
				isSuccess = false;
				httpStatusCode =  (int)HttpStatusCode.InternalServerError;
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry($"Error processing message: {e.Message}", FMEventLogEntryType.Error);
			}

			return new Tuple<bool, int, byte[]>(isSuccess, httpStatusCode, responseBody);
		}

		private void QueueMessageForDatabase(SecurityClass security, FCEEMessage messageToQueue)
		{
			var cache = MemoryCache.Default;
			Queue<FCEEMessage> fceeMessageQueue = cache["FCEEMessageQueue"] as Queue<FCEEMessage>;
			if (fceeMessageQueue == null)
			{
				fceeMessageQueue = new Queue<FCEEMessage>();

				// the cache is valid for 30 seconds, after which the callback will write to the db
				var cacheItemPolicy = new CacheItemPolicy()
				{
					AbsoluteExpiration = DateTime.Now.AddSeconds(30),
					RemovedCallback = new CacheEntryRemovedCallback(FCEEMessageQueueCacheRemovedCallback)
				};
				cache.Add("FCEEMessageQueue", fceeMessageQueue, cacheItemPolicy);
			}
			lock (fceeMessageQueue) // don't allow concurrent requests to queue while we are dequeueing
			{
				fceeMessageQueue.Enqueue(messageToQueue);
			}

			// if the queue is >500 items, write them to the db
			if (fceeMessageQueue.Count > 500)
			{
				WriteMessagesToDatabase(security, fceeMessageQueue);
			}
		}

		private void FCEEMessageQueueCacheRemovedCallback(CacheEntryRemovedArguments arguments)
		{
			var security = new SecurityClass();
			security.UserID = "FCEEService";
			security.SiteID = "SiteAdmin";
			security.SiteGuid = Guids.SiteAdminGuid;
			Queue<FCEEMessage> fceeMessageQueue = (Queue <FCEEMessage>) arguments.CacheItem.Value;
			WriteMessagesToDatabase(security, fceeMessageQueue);
		}

		private void WriteMessagesToDatabase(SecurityClass security, Queue<FCEEMessage> fceeMessageQueue)
		{
			var table = new DataTable();
			table.Columns.Add("ImeiNumber", typeof(string));
			table.Columns.Add("Timestamp", typeof(DateTimeOffset));
			table.Columns.Add("MsgType", typeof(int));
			table.Columns.Add("Idx", typeof(int));
			table.Columns.Add("Device", typeof(int));
			table.Columns.Add("BinaryData", typeof(byte[]));
			table.Columns.Add("EdgeData", typeof(string));
			table.Columns.Add("Validity", typeof(bool));

			lock (fceeMessageQueue) // don't allow concurrent requests to queue while we are dequeueing
			{
				while (fceeMessageQueue.Count != 0)
				{
					var row = table.NewRow();
					FCEEMessage message = fceeMessageQueue.Dequeue();
					row["ImeiNumber"] = message.ImeiNumber;
					row["Timestamp"] = message.Timestamp;
					row["Idx"] = message.Index;
					row["MsgType"] = message.MsgType;
					row["Device"] = message.Device ?? Convert.DBNull;
					row["BinaryData"] = message.BinaryData;
					row["EdgeData"] = message.EdgeData;
					row["Validity"] = message.Validity;
					table.Rows.Add(row);
				}
			}
			SqlCommand cmd = new SqlCommand();
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.CommandText = "dbo.usp_FCEEMessagesInsert";
			SqlParameter tableValuedParameter = cmd.Parameters.Add("@FCEEMessagesTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.FCEEMessagesType";

			ConsolidatedDA.ExecuteQuery(security, cmd);
		}

		#region ProcessRequestHandler support
		public EdgeData ProcessMessage(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, EdgeMessageType edgeMsgType, bool useLastKnownGood, bool pointStatusProcessing, bool validity)
		{
			object edgeMsg = null;
			var pointServiceManager = new PointServiceManager();
			switch (edgeMsgType)
			{
				case EdgeMessageType.Heartbeat:
					edgeMsg = ProcessHeartbeatMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.SoftwareVersion:
					edgeMsg = ProcessSoftwareVersionMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.DeviceStatus:
					edgeMsg = ProcessDeviceStatusMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.Enraf854TankGauge:
					edgeMsg = ProcessEnraf854TankGaugeMsg(security, fceDevice, memoryStream, useLastKnownGood, pointStatusProcessing, validity);
					break;

				case EdgeMessageType.Enraf854TankGaugeDensity:
					edgeMsg = ProcessEnraf854TankGaugeDensityMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.ModbusIntegerRegisterBlock:
					edgeMsg = ProcessModbusIntegerRegisterBlockMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.GenericScalingPoint:
					edgeMsg = ProcessGenericScalingPointMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.ITTBarton3500ATG:
					edgeMsg = ProcessITTBarton3500ATGMsg(security, fceDevice, memoryStream, useLastKnownGood, pointStatusProcessing, validity);
					break;

				case EdgeMessageType.VeederRootTLS350:
					edgeMsg = ProcessVeederRootTLS50Msg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.VeederRootSystemStatus:
					edgeMsg = ProcessVeederRootSystemStatusMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.VeederRootLeakTest:
					edgeMsg = ProcessVeederRootLeakTestMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.VeederRootSystemAlarms:
					edgeMsg = ProcessVeederRootSystemAlarmsMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.VeederRootInventoryReport:
					edgeMsg = ProcessVeederRootInventoryReportMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.VeederRootInTankStatusReport:
					edgeMsg = ProcessVeederRootInTankStatusReportMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.VeederRootLiquidSensorStatusReport:
					edgeMsg = ProcessVeederRootLiquidSensorStatusReportMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.ModbusInventory:
					edgeMsg = ProcessModbusInventoryMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.ModbusDensityAndAlarm:
					edgeMsg = ProcessModbusDensityAndAlarmMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.ModbusFacilityStatus:
					edgeMsg = ProcessModbusFacilityStatusMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.ModbusStorage:
					edgeMsg = ProcessModbusStorageMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.CommandStatus:
					edgeMsg = ProcessCommandStatusMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				case EdgeMessageType.WAGOPLC:
					edgeMsg = ProcessWAGOPLCMsg(security, fceDevice, memoryStream, useLastKnownGood, validity);
					break;

				default:
					// leave edgeMsg null. The calling function needs to handle the null and log appropriately
					break;
			}

			return edgeMsg as EdgeData;
		}

		private void getSoftwareVersion(string softwwareVersion, out int major, out int minor)
		{
			major = 1;
			minor = 0;

			string[] split = softwwareVersion.Split('.');

			if(split.Length >= 1
			&& split[0].Length >= 10)
			{
				try
				{
					int day = Convert.ToInt32(split[0].Substring(split[0].Length - 2, 2));
					int month = Convert.ToInt32(split[0].Substring(split[0].Length - 4, 2));
					int year = Convert.ToInt32(split[0].Substring(split[0].Length - 8, 4));

					if(year <= 2025
					&& month <= 6
					&& day <= 19)
					{
						return;
					}
					else if(year <= 2025
					&& month <= 10
					&& day <= 16)
					{
						minor = 1;
					}
					else
					{
						minor = 2;
					}
				}
				catch(Exception e)
				{
					FMEventLog eventLog = new FMEventLog();
					eventLog.WriteEntry("FCEE Service Error parsing SWVersion: " + e.Message, FMEventLogEntryType.Error);
				}
			}
		}

		private byte[] buildConfigurationResponseBody(SecurityClass security, string imei)
		{
			byte[] response = new byte[0];
			try
			{
				FCEDevices fceDevices = new FCEDevices();
				var fceDevice =  fceDevices.GetbyIMEI(security, imei);

				if (fceDevice != null)
				{
					byte[] responseType = new byte[2] { 0x40, 0x02 };
					byte[] length = new byte[4];
					byte[] minTime = new byte[4];
					byte[] maxTime = new byte[4];
					byte[] levelDeadband = new byte[4];
					byte[] tempDeadband = new byte[4];
					byte[] heartbeat = new byte[4];
					byte[] tlsTanks = new byte[1];
					byte[] modbusMap = new byte[1];
					byte[] midnightOffset = new byte[2];
					byte[] shortDeadband = new byte[4];
					byte[] shortTime = new byte[2];
					byte[] longDeadband = new byte[4];
					byte[] longTime = new byte[2];
					byte[] scalerType = new byte[12];

					// the size of the byte array returned from GetBytes is dependent upon parameter data type
					// https://learn.microsoft.com/en-us/dotnet/api/system.bitconverter.getbytes
					length = BitConverter.GetBytes((uint)48);
					minTime = BitConverter.GetBytes((uint)fceDevice.MinTime);
					maxTime = BitConverter.GetBytes((uint)fceDevice.MaxTime);
					levelDeadband = BitConverter.GetBytes((float)fceDevice.LevelDeadband);
					tempDeadband = BitConverter.GetBytes((float)(fceDevice.TempDeadband));
					heartbeat = BitConverter.GetBytes((uint)(fceDevice.Heartbeat));
					Array.Copy(BitConverter.GetBytes((ushort)fceDevice.TLStanks), tlsTanks, tlsTanks.Length); // ushort returns length 2, copy it to length 1 array
					Array.Copy(BitConverter.GetBytes((ushort)fceDevice.ModbusMap), modbusMap, modbusMap.Length);
					midnightOffset = BitConverter.GetBytes((ushort)fceDevice.MidnightOffset);
					shortDeadband = BitConverter.GetBytes((float)fceDevice.ShortDeadband);
					shortTime = BitConverter.GetBytes((ushort)fceDevice.ShortTime);
					longDeadband = BitConverter.GetBytes((float)fceDevice.LongDeadband);
					longTime = BitConverter.GetBytes((ushort)fceDevice.LongTime);
					scalerType = fceDevice.ScalerType.ToArray();

					int major;
					int minor;

					getSoftwareVersion(fceDevice.SoftwareVersion, out major, out minor);

					if (BitConverter.IsLittleEndian)
					{
						Array.Reverse(length);
						Array.Reverse(minTime);
						Array.Reverse(maxTime);
						Array.Reverse(levelDeadband);
						Array.Reverse(tempDeadband);
						Array.Reverse(heartbeat);
						Array.Reverse(midnightOffset);
						Array.Reverse(shortDeadband);
						Array.Reverse(shortTime);
						Array.Reverse(longDeadband);
						Array.Reverse(longTime);
						Array.Reverse(scalerType);
					}

					// Version 1.0 doesn't support scaler type
					if (minor == 1
					&& minor == 0)
					{
						response = responseType
										.Concat(length)
										.Concat(minTime)
										.Concat(maxTime)
										.Concat(levelDeadband)
										.Concat(tempDeadband)
										.Concat(heartbeat)
										.Concat(tlsTanks)
										.Concat(modbusMap)
										.Concat(midnightOffset)
										.Concat(shortDeadband)
										.Concat(shortTime)
										.Concat(longDeadband)
										.Concat(longTime)
										.ToArray();
					}
					else
					{
						response = responseType
										.Concat(length)
										.Concat(minTime)
										.Concat(maxTime)
										.Concat(levelDeadband)
										.Concat(tempDeadband)
										.Concat(heartbeat)
										.Concat(tlsTanks)
										.Concat(modbusMap)
										.Concat(midnightOffset)
										.Concat(shortDeadband)
										.Concat(shortTime)
										.Concat(longDeadband)
										.Concat(longTime)
										.Concat(scalerType)
										.ToArray();
					}

					// set the flag to stop sending 205 http code
					fceDevice.ConfigReady = false;
					fceDevices.Modify(security, fceDevice);
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error loading FCE Device for Configuration request IMEI : " + e.Message, FMEventLogEntryType.Error);
			}
			return response;
		}

		private object ProcessHeartbeatMsg(SecurityClass security, FCEDevice fceDevic, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var heartbeatMsg = new HeartbeatMsg();

			try
			{
				heartbeatMsg.Load(memoryStream);

				var fceeMapping = GetMapping(security, heartbeatMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();
					var pointServiceManager = new PointServiceManager();

					var pointValueList =  pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{
							ProcessValue(outputPointValueList, pointValueList[0], heartbeatMsg.Counter, 0, heartbeatMsg.TimeStamp, heartbeatMsg.MsgExt, useLastKnownGood, validity);

							if (outputPointValueList.Count > 0)
							{
								pointServiceManager.SetPointValueData(security, outputPointValueList, true);
							}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Heartbeat Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return heartbeatMsg;
		}

		private object ProcessSoftwareVersionMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var softwareVersionMsg = new SoftwareVersionMsg();

			try
			{
				softwareVersionMsg.Load(memoryStream);

				if (fceDevice != null)
				{
					if (fceDevice.MinTime != softwareVersionMsg.MinTime ||
							fceDevice.MaxTime != softwareVersionMsg.MaxTime ||
							(Math.Abs(fceDevice.LevelDeadband - softwareVersionMsg.LevelDeadband) > 0.00001) || // for 4 byte floats, just want to make sure it is approximately the same
							(Math.Abs(fceDevice.TempDeadband - softwareVersionMsg.TempDeadband) > 0.00001) ||
							fceDevice.Heartbeat != softwareVersionMsg.Heartbeat ||
							fceDevice.TLStanks != softwareVersionMsg.TLSTanks ||
							fceDevice.ModbusMap != softwareVersionMsg.ModbusMap ||
							fceDevice.MidnightOffset != softwareVersionMsg.MidnightOffset ||
							(Math.Abs(fceDevice.ShortDeadband - softwareVersionMsg.ShortDeadband) > 0.00001) ||
							fceDevice.ShortTime != softwareVersionMsg.ShortTime ||
							(Math.Abs(fceDevice.LongDeadband - softwareVersionMsg.LongDeadband) > 0.00001) ||
							fceDevice.LongTime != softwareVersionMsg.LongTime ||
							fceDevice.SoftwareVersion != softwareVersionMsg.SWVersion)
					{
						if (!fceDevice.ConfigReady)
						{
							//warn the user, then overwrite our config
							AlarmAndEventLogsClass alarmAndEventLogsClass = new AlarmAndEventLogsClass();

							FCEEEvents fceeEvents = new FCEEEvents();
							alarmAndEventLogsClass.Add(security, fceeEvents.FCEEConfigurationOverwrittenEvent(softwareVersionMsg.ImeiNumber));


							fceDevice.MinTime = (int)softwareVersionMsg.MinTime;
							fceDevice.MaxTime = (int)softwareVersionMsg.MaxTime;
							fceDevice.LevelDeadband = softwareVersionMsg.LevelDeadband;
							fceDevice.TempDeadband = softwareVersionMsg.TempDeadband;
							fceDevice.Heartbeat = (int)softwareVersionMsg.Heartbeat;
							fceDevice.TLStanks = softwareVersionMsg.TLSTanks;
							fceDevice.ModbusMap = softwareVersionMsg.ModbusMap;
							fceDevice.MidnightOffset = softwareVersionMsg.MidnightOffset;
							fceDevice.ShortDeadband = softwareVersionMsg.ShortDeadband;
							fceDevice.ShortTime = softwareVersionMsg.ShortTime;
							fceDevice.LongDeadband = softwareVersionMsg.LongDeadband;
							fceDevice.LongTime = softwareVersionMsg.LongTime;
							fceDevice.SoftwareVersion = softwareVersionMsg.SWVersion;

							FCEDevices fCEDevices = new FCEDevices();
							fCEDevices.Modify(security, fceDevice);
						}

						// Apply any update to Software Versaion
						else if(fceDevice.SoftwareVersion != softwareVersionMsg.SWVersion)
						{
							fceDevice.SoftwareVersion = softwareVersionMsg.SWVersion;

							FCEDevices fCEDevices = new FCEDevices();
							fCEDevices.Modify(security, fceDevice);
						}
					}
				}

				var fceeMapping = GetMapping(security, softwareVersionMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();
					var pointServiceManager = new PointServiceManager();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{
						ProcessValue(outputPointValueList, pointValueList[0], softwareVersionMsg.SWVersion, 0, softwareVersionMsg.TimeStamp, softwareVersionMsg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[1], softwareVersionMsg.MinTime, 0, softwareVersionMsg.TimeStamp, softwareVersionMsg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[2], softwareVersionMsg.MaxTime, 0, softwareVersionMsg.TimeStamp, softwareVersionMsg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[3], softwareVersionMsg.LevelDeadband, 0, softwareVersionMsg.TimeStamp, softwareVersionMsg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[4], softwareVersionMsg.TempDeadband, 0, softwareVersionMsg.TimeStamp, softwareVersionMsg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[5], softwareVersionMsg.Heartbeat, 0, softwareVersionMsg.TimeStamp, softwareVersionMsg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[6], softwareVersionMsg.TLSTanks, 0, softwareVersionMsg.TimeStamp, softwareVersionMsg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[7], softwareVersionMsg.ModbusMap, 0, softwareVersionMsg.TimeStamp, softwareVersionMsg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[8], softwareVersionMsg.MidnightOffset, 0, softwareVersionMsg.TimeStamp, softwareVersionMsg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[9], softwareVersionMsg.ShortDeadband, 0, softwareVersionMsg.TimeStamp, softwareVersionMsg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[10], softwareVersionMsg.ShortTime, 0, softwareVersionMsg.TimeStamp, softwareVersionMsg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[11], softwareVersionMsg.LongDeadband, 0, softwareVersionMsg.TimeStamp, softwareVersionMsg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[12], softwareVersionMsg.LongTime, 0, softwareVersionMsg.TimeStamp, softwareVersionMsg.MsgExt, useLastKnownGood, validity);

						if (outputPointValueList.Count > 0)
						{
							pointServiceManager.SetPointValueData(security, outputPointValueList, true);
						}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Software Version Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return softwareVersionMsg;
		}

		private object ProcessDeviceStatusMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var deviceStatusMsg = new DeviceStatusMsg();

			try
			{
				deviceStatusMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, deviceStatusMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{
							ProcessValue(outputPointValueList, pointValueList[0], deviceStatusMsg.DeviceType, 0, deviceStatusMsg.TimeStamp, deviceStatusMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[1], deviceStatusMsg.DeviceStatus, 0, deviceStatusMsg.TimeStamp, deviceStatusMsg.MsgExt, useLastKnownGood, validity);

							if (outputPointValueList.Count > 0)
							{
								pointServiceManager.SetPointValueData(security, outputPointValueList, true);
							}
					}
				}

				if (deviceStatusMsg.DeviceStatus == 0 || deviceStatusMsg.DeviceStatus == 1) {
					FCEDevices fCEDevices = new FCEDevices();
					var FCEDevice = fCEDevices.GetbyIMEI(security, deviceStatusMsg.ImeiNumber);
					var deviceGuid = FCEDevice.FCEDeviceGuid;

					this.SetNoResponse(security, deviceGuid, deviceStatusMsg.Index, deviceStatusMsg.DeviceType, deviceStatusMsg.DeviceStatus);
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Device Status Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return deviceStatusMsg;
		}

		private object ProcessEnraf854TankGaugeMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool pointStatusProcessing, bool validity)
		{
			var enraf854TankGaugeMsg = new Enraf854TankGaugeMsg();

			try
			{
				enraf854TankGaugeMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				if (!pointStatusProcessing)
				{
					enraf854TankGaugeMsg.PntStatus = 0;
				}

				var fceeMapping = GetMapping(security, enraf854TankGaugeMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{

							var quality = (ushort)(enraf854TankGaugeMsg.PntStatus & 0x4072);
							ProcessValue(outputPointValueList, pointValueList[0], enraf854TankGaugeMsg.Level, quality, enraf854TankGaugeMsg.TimeStamp, enraf854TankGaugeMsg.MsgExt, useLastKnownGood, validity);

							quality = (ushort)(enraf854TankGaugeMsg.PntStatus & 0x40F0);
							ProcessValue(outputPointValueList, pointValueList[1], enraf854TankGaugeMsg.Temp, quality, enraf854TankGaugeMsg.TimeStamp, enraf854TankGaugeMsg.MsgExt, useLastKnownGood, validity);

							quality = (ushort)(enraf854TankGaugeMsg.PntStatus & 0x4074);
							ProcessValue(outputPointValueList, pointValueList[2], enraf854TankGaugeMsg.WaterLevel, quality, enraf854TankGaugeMsg.TimeStamp, enraf854TankGaugeMsg.MsgExt, useLastKnownGood, validity);

							quality = (ushort)(enraf854TankGaugeMsg.PntStatus & 0x4070);
							ProcessValue(outputPointValueList, pointValueList[3], enraf854TankGaugeMsg.Position, quality, enraf854TankGaugeMsg.TimeStamp, enraf854TankGaugeMsg.MsgExt, useLastKnownGood, validity);

							quality = (ushort)(enraf854TankGaugeMsg.PntStatus & 0x4070);
							ProcessValue(outputPointValueList, pointValueList[4], enraf854TankGaugeMsg.GaugeStatus, quality, enraf854TankGaugeMsg.TimeStamp, enraf854TankGaugeMsg.MsgExt, useLastKnownGood, validity);

							if (outputPointValueList.Count > 0)
							{
								pointServiceManager.SetPointValueData(security, outputPointValueList, true);
							}
					}
				}

			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Enraf 854 Tank Gauge Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return enraf854TankGaugeMsg;
		}

		private object ProcessEnraf854TankGaugeDensityMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var enraf854TankGaugeDensityMsg = new Enraf854TankGaugeDensityMsg();

			try
			{
				enraf854TankGaugeDensityMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, enraf854TankGaugeDensityMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{
							ProcessValue(outputPointValueList, pointValueList[0], enraf854TankGaugeDensityMsg.Density, 0, enraf854TankGaugeDensityMsg.DensityTime, enraf854TankGaugeDensityMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[1], enraf854TankGaugeDensityMsg.DensityTemp, 0, enraf854TankGaugeDensityMsg.DensityTime, enraf854TankGaugeDensityMsg.MsgExt, useLastKnownGood, validity);

							if (outputPointValueList.Count > 0)
							{
								pointServiceManager.SetPointValueData(security, outputPointValueList, true);
							}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Enraf 854 Tank Gauge Density Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return enraf854TankGaugeDensityMsg;
		}

		private object ProcessModbusIntegerRegisterBlockMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var modbusIntegerRegisterBlockMsg = new ModbusIntegerRegisterBlockMsg();

			try
			{
				modbusIntegerRegisterBlockMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, modbusIntegerRegisterBlockMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{
							uint? value = null;

							ushort quality = (ushort)(modbusIntegerRegisterBlockMsg.PntStatus & 0x0001);

							// No response from device
							if (quality == 1)
							{
								ProcessValue(outputPointValueList, pointValueList[0], pointValueList[0].Value, quality, modbusIntegerRegisterBlockMsg.TimeStamp, modbusIntegerRegisterBlockMsg.MsgExt, useLastKnownGood, validity);
								ProcessValue(outputPointValueList, pointValueList[1], pointValueList[1].Value, quality, modbusIntegerRegisterBlockMsg.TimeStamp, modbusIntegerRegisterBlockMsg.MsgExt, useLastKnownGood, validity);
								ProcessValue(outputPointValueList, pointValueList[2], pointValueList[2].Value, quality, modbusIntegerRegisterBlockMsg.TimeStamp, modbusIntegerRegisterBlockMsg.MsgExt, useLastKnownGood, validity);
							}

							if (pointValueList[3].ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference"
							&& pointValueList[3].Value is FMBusinessObjects.DataObjects.DeviceAlarmMapReference)
							{
								value = (uint)(modbusIntegerRegisterBlockMsg.PntStatus & 0x0001);
								ProcessValue(outputPointValueList, pointValueList[3], value, 0, modbusIntegerRegisterBlockMsg.TimeStamp, modbusIntegerRegisterBlockMsg.MsgExt, useLastKnownGood, validity);
							}

							if (outputPointValueList.Count > 0)
							{
								pointServiceManager.SetPointValueData(security, outputPointValueList, true);
							}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Modbus Integer Register Block Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return modbusIntegerRegisterBlockMsg;
		}

		private object ProcessGenericScalingPointMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)	
		{
			var genericScalingPointMsg = new GenericScalingPointMsg();

			try
			{
				genericScalingPointMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, genericScalingPointMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{
							ProcessValue(outputPointValueList, pointValueList[0], genericScalingPointMsg.Value, 0, genericScalingPointMsg.TimeStamp, genericScalingPointMsg.MsgExt, useLastKnownGood, validity);

							if (outputPointValueList.Count > 0)
							{
								pointServiceManager.SetPointValueData(security, outputPointValueList, true);
							}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Generic Scaling Point Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return genericScalingPointMsg;
		}

		private object ProcessITTBarton3500ATGMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool pointStatusProcessing, bool validity)
		{
			var ittBarton3500ATGMsg = new ITTBarton3500ATGMsg();

			try
			{
				ittBarton3500ATGMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				if (!pointStatusProcessing)
				{
					ittBarton3500ATGMsg.PntStatus = 0;
				}

				var fceeMapping = GetMapping(security, ittBarton3500ATGMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{

							var quality = (ushort)(ittBarton3500ATGMsg.PntStatus & 0x0005);
							ProcessValue(outputPointValueList, pointValueList[0], ittBarton3500ATGMsg.Level, quality, ittBarton3500ATGMsg.TimeStamp, ittBarton3500ATGMsg.MsgExt, useLastKnownGood, validity);

							quality = (ushort)(ittBarton3500ATGMsg.PntStatus & 0x0005);
							ProcessValue(outputPointValueList, pointValueList[1], ittBarton3500ATGMsg.Temp, quality, ittBarton3500ATGMsg.TimeStamp, ittBarton3500ATGMsg.MsgExt, useLastKnownGood, validity);

							quality = (ushort)(ittBarton3500ATGMsg.PntStatus & 0x0005);
							ProcessValue(outputPointValueList, pointValueList[2], ittBarton3500ATGMsg.WaterLevel, quality, ittBarton3500ATGMsg.TimeStamp, ittBarton3500ATGMsg.MsgExt, useLastKnownGood, validity);

							quality = (ushort)(ittBarton3500ATGMsg.PntStatus & 0x0005);
							ProcessValue(outputPointValueList, pointValueList[3], ittBarton3500ATGMsg.Density, quality, ittBarton3500ATGMsg.TimeStamp, ittBarton3500ATGMsg.MsgExt, useLastKnownGood, validity);

							quality = (ushort)(ittBarton3500ATGMsg.PntStatus & 0x0005);
							ProcessValue(outputPointValueList, pointValueList[4], ittBarton3500ATGMsg.Value1, quality, ittBarton3500ATGMsg.TimeStamp, ittBarton3500ATGMsg.MsgExt, useLastKnownGood, validity);

							quality = (ushort)(ittBarton3500ATGMsg.PntStatus & 0x0001);
							ProcessValue(outputPointValueList, pointValueList[5], ittBarton3500ATGMsg.AlarmFlag, quality, ittBarton3500ATGMsg.TimeStamp, ittBarton3500ATGMsg.MsgExt, useLastKnownGood, validity);

							if (outputPointValueList.Count > 0)
							{
								pointServiceManager.SetPointValueData(security, outputPointValueList, true);
							}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing ITT Barton 3500 ATG Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return ittBarton3500ATGMsg;
		}

		private object ProcessVeederRootTLS50Msg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var veederRootTLS350Msg = new VeederRootTLS350Msg();

			try
			{
				veederRootTLS350Msg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, veederRootTLS350Msg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{
						ProcessValue(outputPointValueList, pointValueList[0], veederRootTLS350Msg.Level, 0, veederRootTLS350Msg.TimeStamp, veederRootTLS350Msg.MsgExt, useLastKnownGood, validity, EngineeringUnit.FmlInch);
						ProcessValue(outputPointValueList, pointValueList[1], veederRootTLS350Msg.Temp, 0, veederRootTLS350Msg.TimeStamp, veederRootTLS350Msg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[2], veederRootTLS350Msg.WaterLevel, 0, veederRootTLS350Msg.TimeStamp, veederRootTLS350Msg.MsgExt, useLastKnownGood, validity, EngineeringUnit.FmlInch);
						ProcessValue(outputPointValueList, pointValueList[3], veederRootTLS350Msg.GrossVolume, 0, veederRootTLS350Msg.TimeStamp, veederRootTLS350Msg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[4], veederRootTLS350Msg.NetVolume, 0, veederRootTLS350Msg.TimeStamp, veederRootTLS350Msg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[5], veederRootTLS350Msg.WaterVolume, 0, veederRootTLS350Msg.TimeStamp, veederRootTLS350Msg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[6], veederRootTLS350Msg.Ullage, 0, veederRootTLS350Msg.TimeStamp, veederRootTLS350Msg.MsgExt, useLastKnownGood, validity);

						if (outputPointValueList.Count > 0)
						{
							pointServiceManager.SetPointValueData(security, outputPointValueList, true);
						}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Veeder Root TLS350 Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return veederRootTLS350Msg;

		}

		private object ProcessVeederRootSystemStatusMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var veederRootSystemStatusMsg = new VeederRootSystemStatusMsg();

			try
			{
				veederRootSystemStatusMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, veederRootSystemStatusMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{

							uint? value = null;

							if (pointValueList[0].ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference"
							&& pointValueList[0].Value is FMBusinessObjects.DataObjects.DeviceAlarmMapReference)
							{
								value = (pointValueList[0].Value as FMBusinessObjects.DataObjects.DeviceAlarmMapReference).CurrentValue;

								if ((value as uint?).HasValue)
								{
									value = (uint?)(value.Value & 0xfffF0000) | (uint)veederRootSystemStatusMsg.PntStatus;
								}
								else
								{
									value = (uint?)veederRootSystemStatusMsg.PntStatus;
								}

								ProcessValue(outputPointValueList, pointValueList[0], value, 0, veederRootSystemStatusMsg.TimeStamp, veederRootSystemStatusMsg.MsgExt, useLastKnownGood, validity);

								if (outputPointValueList.Count > 0)
								{
									pointServiceManager.SetPointValueData(security, outputPointValueList, true);
								}
							}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Veeder Root System Status Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return veederRootSystemStatusMsg;
		}

		private object ProcessVeederRootLeakTestMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var veederRootLeakTestMsg = new VeederRootSystemStatusMsg();

			try
			{
				veederRootLeakTestMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, veederRootLeakTestMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{
							uint? value = null;

							if (pointValueList[0].ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference"
							&& pointValueList[0].Value is FMBusinessObjects.DataObjects.DeviceAlarmMapReference)
							{
								value = (pointValueList[0].Value as FMBusinessObjects.DataObjects.DeviceAlarmMapReference).CurrentValue;

								if ((value as uint?).HasValue)
								{
									value = (uint?)(value.Value & 0xfffeffff | (uint)(veederRootLeakTestMsg.PntStatus & 0x0001) << 16);
								}
								else
								{
									value = (uint?)(veederRootLeakTestMsg.PntStatus & 0x0001) << 16;
								}

								ProcessValue(outputPointValueList, pointValueList[0], value, 0, veederRootLeakTestMsg.TimeStamp, veederRootLeakTestMsg.MsgExt, useLastKnownGood, validity);

								if (outputPointValueList.Count > 0)
								{
									pointServiceManager.SetPointValueData(security, outputPointValueList, true);
								}
							}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Veeder Root Leak Test Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return veederRootLeakTestMsg;
		}

		private object ProcessVeederRootSystemAlarmsMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var veederRootSystemAlarmsMsg = new VeederRootSystemAlarmsMsg();

			try
			{
				veederRootSystemAlarmsMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, veederRootSystemAlarmsMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{
							uint? value = null;

							if (pointValueList[0].ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference"
							&& pointValueList[0].Value is FMBusinessObjects.DataObjects.DeviceAlarmMapReference)
							{
								value = (pointValueList[0].Value as FMBusinessObjects.DataObjects.DeviceAlarmMapReference).CurrentValue;

								if ((value as uint?).HasValue)
								{
									value = (uint?)(value.Value & 0xfffdffff | (uint)(veederRootSystemAlarmsMsg.PntStatus & 0x0001) << 17);
								}
								else
								{
									value = (uint?)(veederRootSystemAlarmsMsg.PntStatus & 0x0001) << 17;
								}

								ProcessValue(outputPointValueList, pointValueList[0], value, 0, veederRootSystemAlarmsMsg.TimeStamp, veederRootSystemAlarmsMsg.MsgExt, useLastKnownGood, validity);

								if (outputPointValueList.Count > 0)
								{
									pointServiceManager.SetPointValueData(security, outputPointValueList, true);
								}
							}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Veeder Root System Alarms Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return veederRootSystemAlarmsMsg;
		}

		private object ProcessVeederRootInventoryReportMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var veederRootInventoryReportMsg = new VeederRootInventoryReportMsg();

			try
			{
				veederRootInventoryReportMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, veederRootInventoryReportMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{
						// Veeder root inventory height and water level are always in inches. 

						ProcessValue(outputPointValueList, pointValueList[0], veederRootInventoryReportMsg.Height, 0, veederRootInventoryReportMsg.TimeStamp, veederRootInventoryReportMsg.MsgExt, useLastKnownGood, validity, EngineeringUnit.FmlInch);
							ProcessValue(outputPointValueList, pointValueList[1], veederRootInventoryReportMsg.Temperature, 0, veederRootInventoryReportMsg.TimeStamp, veederRootInventoryReportMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[2], veederRootInventoryReportMsg.Volume, 0, veederRootInventoryReportMsg.TimeStamp, veederRootInventoryReportMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[3], veederRootInventoryReportMsg.TCVolume, 0, veederRootInventoryReportMsg.TimeStamp, veederRootInventoryReportMsg.MsgExt, useLastKnownGood, validity);
						ProcessValue(outputPointValueList, pointValueList[4], veederRootInventoryReportMsg.Water, 0, veederRootInventoryReportMsg.TimeStamp, veederRootInventoryReportMsg.MsgExt, useLastKnownGood, validity, EngineeringUnit.FmlInch);
							ProcessValue(outputPointValueList, pointValueList[5], veederRootInventoryReportMsg.Ullage, 0, veederRootInventoryReportMsg.TimeStamp, veederRootInventoryReportMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[6], veederRootInventoryReportMsg.WaterVolume, 0, veederRootInventoryReportMsg.TimeStamp, veederRootInventoryReportMsg.MsgExt, useLastKnownGood, validity);

							if (outputPointValueList.Count > 0)
							{
								pointServiceManager.SetPointValueData(security, outputPointValueList, true);
							}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Veeder Root Inventory Report Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return veederRootInventoryReportMsg;
		}

		private object ProcessVeederRootInTankStatusReportMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var veederRootInTankStatusReportMsg = new VeederRootInTankStatusReportMsg();

			try
			{
				veederRootInTankStatusReportMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, veederRootInTankStatusReportMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{
							ProcessValue(outputPointValueList, pointValueList[0], veederRootInTankStatusReportMsg.TankStatus, 0, veederRootInTankStatusReportMsg.TimeStamp, veederRootInTankStatusReportMsg.MsgExt, useLastKnownGood, validity);

							if (outputPointValueList.Count > 0)
							{
								pointServiceManager.SetPointValueData(security, outputPointValueList, true);
							}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Veeder Root In Tank Status Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return veederRootInTankStatusReportMsg;
		}

		private object ProcessVeederRootLiquidSensorStatusReportMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var veederRootLiquidSensorStatusReportMsg = new VeederRootLiquidSensorStatusReportMsg();

			try
			{
				veederRootLiquidSensorStatusReportMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, veederRootLiquidSensorStatusReportMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{

							uint? value = null;

							if (pointValueList[0].ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference"
							&& pointValueList[0].Value is FMBusinessObjects.DataObjects.DeviceAlarmMapReference)
							{
								value = (uint?)veederRootLiquidSensorStatusReportMsg.PntStatus;

								ProcessValue(outputPointValueList, pointValueList[0], value, 0, veederRootLiquidSensorStatusReportMsg.TimeStamp, veederRootLiquidSensorStatusReportMsg.MsgExt, useLastKnownGood, validity);

								if (outputPointValueList.Count > 0)
								{
									pointServiceManager.SetPointValueData(security, outputPointValueList, true);
								}
							}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Veeder Root Liquid Sensor Status Report Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return veederRootLiquidSensorStatusReportMsg;
		}

		private object ProcessModbusInventoryMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var modbusInventoryMsg = new ModbusInventoryMsg();

			try
			{
				modbusInventoryMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, modbusInventoryMsg, modbusInventoryMsg.Device);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{

							ProcessValue(outputPointValueList, pointValueList[0], modbusInventoryMsg.Level, 0, modbusInventoryMsg.TimeStamp, modbusInventoryMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[1], modbusInventoryMsg.Temp, 0, modbusInventoryMsg.TimeStamp, modbusInventoryMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[2], modbusInventoryMsg.WaterLevel, 0, modbusInventoryMsg.TimeStamp, modbusInventoryMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[3], modbusInventoryMsg.Position, 0, modbusInventoryMsg.TimeStamp, modbusInventoryMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[4], modbusInventoryMsg.GaugeStatus, 0, modbusInventoryMsg.TimeStamp, modbusInventoryMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[5], modbusInventoryMsg.WaterSump, 0, modbusInventoryMsg.TimeStamp, modbusInventoryMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[6], modbusInventoryMsg.FuelVolume, 0, modbusInventoryMsg.TimeStamp, modbusInventoryMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[7], modbusInventoryMsg.WaterVolume, 0, modbusInventoryMsg.TimeStamp, modbusInventoryMsg.MsgExt, useLastKnownGood, validity);

							if (outputPointValueList.Count > 0)
							{
								pointServiceManager.SetPointValueData(security, outputPointValueList, true);
							}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Modbus Inventory Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return modbusInventoryMsg;
		}

		private object ProcessModbusDensityAndAlarmMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var modbusDensityAndAlarmMsg = new ModbusDensityAndAlarmMsg();

			try
			{
				modbusDensityAndAlarmMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, modbusDensityAndAlarmMsg, modbusDensityAndAlarmMsg.Device);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{

							ProcessValue(outputPointValueList, pointValueList[0], modbusDensityAndAlarmMsg.Density, 0, modbusDensityAndAlarmMsg.DensityTime, modbusDensityAndAlarmMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[1], modbusDensityAndAlarmMsg.DensityTemp, 0, modbusDensityAndAlarmMsg.DensityTime, modbusDensityAndAlarmMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[2], modbusDensityAndAlarmMsg.TroubleInfo, 0, modbusDensityAndAlarmMsg.DensityTime, modbusDensityAndAlarmMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[3], modbusDensityAndAlarmMsg.LevelAlarm, 0, modbusDensityAndAlarmMsg.DensityTime, modbusDensityAndAlarmMsg.MsgExt, useLastKnownGood, validity);

							if (outputPointValueList.Count > 0)
							{
								pointServiceManager.SetPointValueData(security, outputPointValueList, true);
							}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Modbus Density and Alarm Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return modbusDensityAndAlarmMsg;
		}

		private object ProcessModbusFacilityStatusMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var modbusFacilityStatusMsg = new ModbusFacilityStatusMsg();

			try
			{
				modbusFacilityStatusMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, modbusFacilityStatusMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{

							ProcessValue(outputPointValueList, pointValueList[0], modbusFacilityStatusMsg.FacilityStatus, 0, modbusFacilityStatusMsg.TimeStamp, modbusFacilityStatusMsg.MsgExt, useLastKnownGood, validity);

							if (outputPointValueList.Count > 0)
							{
								pointServiceManager.SetPointValueData(security, outputPointValueList, true);
							}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Modbus Facility Status Message : " + e.Message, FMEventLogEntryType.Error);
			}

			return modbusFacilityStatusMsg;
		}

		private object ProcessModbusStorageMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var modbusStorageMsg = new ModbusStorageMsg();

			try
			{
				modbusStorageMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, modbusStorageMsg, modbusStorageMsg.Device);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{
							ProcessValue(outputPointValueList, pointValueList[0], modbusStorageMsg.Level, 0, modbusStorageMsg.TimeStamp, modbusStorageMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[1], modbusStorageMsg.Temp, 0, modbusStorageMsg.TimeStamp, modbusStorageMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[2], modbusStorageMsg.WaterLevel, 0, modbusStorageMsg.TimeStamp, modbusStorageMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[3], modbusStorageMsg.Position, 0, modbusStorageMsg.TimeStamp, modbusStorageMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[4], modbusStorageMsg.Density, 0, modbusStorageMsg.TimeStamp, modbusStorageMsg.MsgExt, useLastKnownGood, validity);

							if (outputPointValueList.Count > 0)
							{
								pointServiceManager.SetPointValueData(security, outputPointValueList, true);
							}
					}
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Modbus Storage Message : " + e.Message, FMEventLogEntryType.Error);
			}
			return modbusStorageMsg;
		}

		private object ProcessCommandStatusMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
			var commandStatusMsg = new CommandStatusMsg();

			try
			{
				commandStatusMsg.Load(memoryStream);
				var pointServiceManager = new PointServiceManager();

				var fceeMapping = GetMapping(security, commandStatusMsg);

				if (fceeMapping != null)
				{
					var outputPointValueList = new List<PointValue>();

					var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

					if (pointValueList != null
					&& pointValueList.Count > 0)
					{
							//?? Anything else need to be done here
							ProcessValue(outputPointValueList, pointValueList[0], commandStatusMsg.CmdStatus, 0, commandStatusMsg.TimeStamp, commandStatusMsg.MsgExt, useLastKnownGood, validity);
							ProcessValue(outputPointValueList, pointValueList[1], commandStatusMsg.CmdSchedule, 0, commandStatusMsg.TimeStamp, commandStatusMsg.MsgExt, useLastKnownGood, validity);

							if (outputPointValueList.Count > 0)
							{
								pointServiceManager.SetPointValueData(security, outputPointValueList, true);
							}
					}
				}

			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("Error processing Command Status Message : " + e.Message, FMEventLogEntryType.Error);
			}
			return commandStatusMsg;
		}
		private object ProcessWAGOPLCMsg(SecurityClass security, FCEDevice fceDevice, MemoryStream memoryStream, bool useLastKnownGood, bool validity)
		{
				var wAGOPLCMsg = new WAGOPLCMsg();

				try
				{
					 wAGOPLCMsg.Load(memoryStream);
					 var pointServiceManager = new PointServiceManager();

					 var fceeMapping = GetMapping(security, wAGOPLCMsg);

					 if (fceeMapping != null)
					 {
						  var outputPointValueList = new List<PointValue>();

						  var pointValueList = pointServiceManager.GetPointValueData(security, fceeMapping.PointValueIdentifierList, false);

						  if (pointValueList != null
						  && pointValueList.Count > 0)
						  {
								// WAGO PLC always reports the density in units of KGm^3

								// wAGOPLCMsg.DeviceTypes
								// 0 = unknown
								// 1 = Enraf 854
								// 2 = TLS-350
								// 3 = MTS
								// 4 = Other

								// Enraf 854
								if (wAGOPLCMsg.DeviceType == 1)
                        {
									 ProcessValue(outputPointValueList, pointValueList[0], wAGOPLCMsg.DeviceType, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[1], wAGOPLCMsg.Level, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 if (wAGOPLCMsg.Temp >= -300 && wAGOPLCMsg.Temp <= 300)
										  ProcessValue(outputPointValueList, pointValueList[2], wAGOPLCMsg.Temp, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 if (wAGOPLCMsg.Density > 0.5 && wAGOPLCMsg.Density < 10000) 
										  ProcessValue(outputPointValueList, pointValueList[3], wAGOPLCMsg.Density, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity, EngineeringUnit.FmdKgM3);
									 if (wAGOPLCMsg.DensityTemp >= -300  && wAGOPLCMsg.DensityTemp <= 300)
									 ProcessValue(outputPointValueList, pointValueList[4], wAGOPLCMsg.DensityTemp, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[5], wAGOPLCMsg.Position, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 //ProcessValue(outputPointValueList, pointValueList[6], wAGOPLCMsg.WaterLevel, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[6], wAGOPLCMsg.WaterSump, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[8], wAGOPLCMsg.GaugeStatus, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[9], wAGOPLCMsg.TroubleInfo, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[10], wAGOPLCMsg.LevelAlarm, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[11], wAGOPLCMsg.Volume, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 //ProcessValue(outputPointValueList, pointValueList[12], wAGOPLCMsg.WaterVolume, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[13], wAGOPLCMsg.NetVolume, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[14], wAGOPLCMsg.Ullage, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[12], wAGOPLCMsg.WaterSumpVol, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
								}
								// TLS-350, MTS, or other device types
								else
								{
									 ProcessValue(outputPointValueList, pointValueList[0], wAGOPLCMsg.DeviceType, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[1], wAGOPLCMsg.Level, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 if (wAGOPLCMsg.Temp >= -300 && wAGOPLCMsg.Temp <= 300)
										  ProcessValue(outputPointValueList, pointValueList[2], wAGOPLCMsg.Temp, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 // ProcessValue(outputPointValueList, pointValueList[3], EngineeringUnits.Convert(wAGOPLCMsg.Density, EngineeringUnit.FmdKgM3, pointValueList[3].Units, 0), 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 // ProcessValue(outputPointValueList, pointValueList[4], wAGOPLCMsg.DensityTemp, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 // ProcessValue(outputPointValueList, pointValueList[5], wAGOPLCMsg.Position, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[6], wAGOPLCMsg.WaterLevel, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 // ProcessValue(outputPointValueList, pointValueList[7], wAGOPLCMsg.WaterSump, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 // ProcessValue(outputPointValueList, pointValueList[8], wAGOPLCMsg.GaugeStatus, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[9], wAGOPLCMsg.TroubleInfo, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[10], wAGOPLCMsg.LevelAlarm, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[11], wAGOPLCMsg.Volume, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[12], wAGOPLCMsg.WaterVolume, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[13], wAGOPLCMsg.NetVolume, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 ProcessValue(outputPointValueList, pointValueList[14], wAGOPLCMsg.Ullage, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
									 //ProcessValue(outputPointValueList, pointValueList[15], wAGOPLCMsg.WaterSumpVol, 0, wAGOPLCMsg.TimeStamp, wAGOPLCMsg.MsgExt, useLastKnownGood, validity);
								}
						

								if (outputPointValueList.Count > 0)
								{
									 pointServiceManager.SetPointValueData(security, outputPointValueList, true);
								}
						  }
					 }
				}
				catch (Exception e)
				{
					 FMEventLog eventLog = new FMEventLog();
					 eventLog.WriteEntry("Error processing WAGO PLC Message : " + e.Message, FMEventLogEntryType.Error);
				}
				return wAGOPLCMsg;
		  }


		  static private void ProcessValue(List<PointValue> outputPointValueList, PointValue pointValue, object value, ushort quality, DateTimeOffset timeStamp, byte MsgExt, bool useLastKnownGood, bool validity, EngineeringUnit? fallbackServerUnits=null)
		{
			if (pointValue.InputOutputType == PointTemplateTag.PointTagInputOutputType.FCEE
			&& pointValue.OpcStatusCodeBits != StatusCodes.GoodLocalOverride)
			{
				if (quality == 0)
				{
					value = ConvertIncomingValueUnits(pointValue, value, fallbackServerUnits);

					switch (pointValue.ValueTypeString)
					{
							case "System.Boolean":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = Convert.ToBoolean(value);
								}
								break;

							case "System.Int16":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = Convert.ToInt16(value);
								}
								break;

							case "System.UInt16":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = Convert.ToUInt16(value);
								}
								break;

							case "System.Int32":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = Convert.ToInt32(value);
								}
								break;

							case "System.UInt32":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = Convert.ToUInt32(value);
								}
								break;

							case "System.Single":

								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = Convert.ToSingle(value);
								}
								break;

							case "System.Double":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = Convert.ToDouble(value);
								}
								break;

							case "System.String":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = Convert.ToString(value);
								}
								break;

							case "System.DateTimeOffset":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = new DateTimeOffset(Convert.ToDateTime(value));
								}
								break;

							case "System.DateTime":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = Convert.ToDateTime(value);
								}
								break;

							case "System.TimeSpan":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = new TimeSpan(Convert.ToInt64(value));
								}
								break;

							case "FMBusinessObjects.DataObjects.PointCommandStatusListReference":
								if (pointValue.Value is PointCommandStatusListReference)
								{
									if (value == null)
									{
											(pointValue.Value as PointCommandStatusListReference).CurrentValue = (int?)Convert.ToInt32(value);
									}
									else
									{
											(pointValue.Value as PointCommandStatusListReference).CurrentValue = (int?)Convert.ToInt32(value);
									}
								}
								break;

							case "FMBusinessObjects.DataObjects.DeviceAlarmMapReference":
								DeviceAlarmMapReference damr = pointValue.Value as DeviceAlarmMapReference;
								if (value == null)
								{
									(pointValue.Value as DeviceAlarmMapReference).CurrentValue = (uint?)Convert.ToInt32(value);
								}
								else
								{
									if (damr != null)
									{ 
											(pointValue.Value as DeviceAlarmMapReference).CurrentValue = (uint?)Convert.ToInt32(value);
									}
								}
								break;


							case "FMBusinessObjects.DataObjects.CodedVariables.TankCommands":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = (FMBusinessObjects.DataObjects.CodedVariables.TankCommands)Convert.ToInt32(value);
								}
								break;

							case "FMBusinessObjects.DataObjects.CodedVariables.TankStatuses":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = (FMBusinessObjects.DataObjects.CodedVariables.TankStatuses)Convert.ToInt32(value);
								}
								break;

							case "FMBusinessObjects.DataObjects.CodedVariables.TransferModes":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = (FMBusinessObjects.DataObjects.CodedVariables.TransferModes)Convert.ToInt32(value);
								}
								break;

							case "FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = (FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode)Convert.ToInt32(value);
								}
								break;

							case "FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = (FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode)Convert.ToInt32(value);
								}
								break;


							case "FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = (FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses)Convert.ToInt32(value);
								}
								break;

							case "FMBusinessObjects.DataObjects.CodedVariables.TankOperationalMode":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = (FMBusinessObjects.DataObjects.CodedVariables.TankOperationalMode)Convert.ToInt32(value);
								}
								break;

							case "FMBusinessObjects.DataObjects.CodedVariables.MovementCommand":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = (FMBusinessObjects.DataObjects.CodedVariables.MovementCommand)Convert.ToInt32(value);
								}
								break;

							case "FMBusinessObjects.DataObjects.CodedVariables.MovementStatus":
								if (value == null)
								{
									pointValue.Value = null;
								}
								else
								{
									pointValue.Value = (FMBusinessObjects.DataObjects.CodedVariables.MovementStatus)Convert.ToInt32(value);
								}
								break;

							case "FMBusinessObjects.DataObjects.CodedVariables.StrapTableSelect":
								if (value != null)
								{
									value = (FMBusinessObjects.DataObjects.CodedVariables.StrapTableSelect)Convert.ToInt32(value);
								}
								break;
							case "FMBusinessObjects.DataObjects.CodedVariables.Reset":
								if (value != null)
								{
									value = (FMBusinessObjects.DataObjects.CodedVariables.Reset)Convert.ToInt32(value);
								}
								break;

							default:
								break;
					}

					if (validity)
					{
							pointValue.Status = StatusCodes.Good;
					}
					else
					{
							if (useLastKnownGood)
							{
								pointValue.Status = StatusCodes.UncertainLastUsableValue;
							}
							else
							{
								pointValue.Value = null;
								pointValue.Status = StatusCodes.Bad;
							}
					}
				}
				else
				{
					if (useLastKnownGood)
					{
							if (pointValue.Value == null)
							{
								pointValue.Status = StatusCodes.Bad;
							}
							else
							{
								pointValue.Status = StatusCodes.UncertainLastUsableValue;
							}
					}
					else
					{
							pointValue.Value = null;
							pointValue.Status = StatusCodes.Bad;
					}
				}

				// MsgExt == 0x80 means the message was forwarded and should be archived at message timestamp
				pointValue.ServerTimeStamp = (MsgExt == 0x80 ? timeStamp : DateTimeOffset.UtcNow);
				pointValue.SourceTimeStamp = timeStamp;

				outputPointValueList.Add(pointValue);
			}
		}

		private static object ConvertIncomingValueUnits(PointValue pointValue, object value, EngineeringUnit? fallbackServerUnits)
		{
			if (pointValue == null || value == null)
			{
				return value;
			}

			if (pointValue.EngineeringUnitsType == EngineeringUnitType.FmuNodim 
				|| pointValue.EngineeringUnitsType == EngineeringUnitType.FmuNone)
			{
				return value;
			}

			if (pointValue.ValueTypeString != "System.Single"
			&& pointValue.ValueTypeString != "System.Double")
			{
				return value;
			}

			if (!(value is float)
            && !(value is double)
            && !(value is int)
            && !(value is long)
            && !(value is UInt16)
            && !(value is UInt32)
            && !(value is UInt64))
			{
            return value;
			}

         var sourceUnits = pointValue.ServerUnits;
			if (sourceUnits == EngineeringUnit.FmuNone && fallbackServerUnits.HasValue)
			{
				sourceUnits = fallbackServerUnits.Value;
			}

			if (sourceUnits == EngineeringUnit.FmuNone
			|| pointValue.Units == EngineeringUnit.FmuNone
			|| sourceUnits == pointValue.Units)
			{
				return value;
			}

			var numericValue = Convert.ToDouble(value);
			return EngineeringUnits.Convert(numericValue, sourceUnits, pointValue.Units, pointValue.DecimalPlaces);
		}

		private int GetHttpStatusCode(SecurityClass security, FCEDevice fceDevice)
		{
			if (fceDevice != null)
				if (fceDevice.ConfigReady)
					return (int)HttpStatusCode.ResetContent;
				else
					return (int)HttpStatusCode.OK;
			else
				return (int)HttpStatusCode.OK;
		}

		private FCEETupleMapping GetMapping(SecurityClass security, EdgeData edgeData, int? device = null)
		{
			var fceeMapper = FCEEMapper.Instance;
			var fceeMapping = fceeMapper.GetMapping(security, edgeData.ImeiNumber, edgeData.MsgType, edgeData.Index, device);

			if (fceeMapping == null)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("No Mapping for : " + edgeData.ImeiNumber + "." + edgeData.MsgType + "." + edgeData.Index + "." + device, FMEventLogEntryType.Error);
			}

			return fceeMapping;
		}
		#endregion

		// MESSAGES
		public void AddMessage(SecurityClass security, string imeiNumber, DateTimeOffset timeStamp, int msgType, int index, byte? device, byte[] binaryData, string edgeData, bool validity)
		{
			using (var sqlCommand = new SqlCommand())
			{
				sqlCommand.CommandType = System.Data.CommandType.Text;
				sqlCommand.CommandText = "INSERT INTO [dbo].[tblFCEEMessage] "
													+ " (ImeiNumber, Timestamp, MsgType, [Index], Device, BinaryData, EdgeData, Validity) VALUES "
													+ " (@ImeiNumber, @TimeStamp, @MsgType, @Index, @Device, @BinaryData, @EdgeData, @Validity)";
				sqlCommand.Parameters.AddWithValue("@ImeiNumber", imeiNumber);
				sqlCommand.Parameters.AddWithValue("@TimeStamp", timeStamp);
				sqlCommand.Parameters.AddWithValue("@MsgType", msgType);
				sqlCommand.Parameters.AddWithValue("@Index", index);
				sqlCommand.Parameters.AddWithValue("@Device", device ?? System.Data.SqlTypes.SqlByte.Null);
				sqlCommand.Parameters.AddWithValue("@BinaryData", binaryData);
				sqlCommand.Parameters.Add("@EdgeData", SqlDbType.NVarChar);
				sqlCommand.Parameters.AddWithValue("@Validity", validity);

				if (!string.IsNullOrEmpty(edgeData))
				{
					sqlCommand.Parameters["@EdgeData"].Value = edgeData;
				}
				else
				{
					sqlCommand.Parameters["@EdgeData"].Value = DBNull.Value;
				}

				ConsolidatedDA.ExecuteQuery(security, sqlCommand);
			}
		}

		public List<FCEEMessage> EnumerateMessages(SecurityClass security, string startDate, string endDate)
		{
			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.gsp_FCEEMessagesEnumerate";
				if (!string.IsNullOrEmpty(startDate))
				{
					cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = startDate;
				}
				if (!string.IsNullOrEmpty(endDate))
				{
					cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value = endDate;
				}
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}
			return PopulateList(set);
		}

		public List<FCEEMessage> GetNewestMessageForMapping(SecurityClass security, FCEEMapping fceeMapping)
		{
			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_FCEEMessagesEnumerateForMapping";

				cmd.Parameters.Add("@fceeMappingGuid", SqlDbType.UniqueIdentifier).Value = fceeMapping.FCEEMappingGuid;

				set = ConsolidatedDA.GetDataSet(cmd, security);
			}
			return PopulateList(set);
		}


		// Utility functions
		protected Dictionary<Guid, FCEEMapping> PopulateDictionary(DataSet set)
		{

			Dictionary<Guid, FCEEMapping> fceeDictionary = new Dictionary<Guid, FCEEMapping>();

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var fceeMapping = new FCEEMapping();

				fceeMapping.AutoLoad(row);

				fceeDictionary.Add(fceeMapping.FCEEMappingGuid, fceeMapping);

			}
			return fceeDictionary;
		}

		protected Dictionary<Guid, FCEEMappingWithDevice> PopulateDictionaryWithDevice(DataSet set)
		{

			Dictionary<Guid, FCEEMappingWithDevice> fceeDictionary = new Dictionary<Guid, FCEEMappingWithDevice>();

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var fceeMapping = new FCEEMappingWithDevice();

				fceeMapping.AutoLoad(row);

				fceeDictionary.Add(fceeMapping.FCEEMappingGuid, fceeMapping);

			}
			return fceeDictionary;
		}

		protected List<FCEEMessage> PopulateList(DataSet set)
		{
			List<FCEEMessage> fceeMessages = new List<FCEEMessage>();
			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var fceeMessage = new FCEEMessage();
				fceeMessage.AutoLoad(row);
				fceeMessage.MsgType =
					(int)fceeMessage.MsgType <= 0x0015 || ((int)fceeMessage.MsgType >= 0x8000 && (int)fceeMessage.MsgType <= 0x8015)
					? fceeMessage.MsgType : EDGEMESSAGETYPE.Invalid;
				fceeMessages.Add(fceeMessage);
			}
			return fceeMessages;
		}

		protected List<string> GetTagsFromMapping(FCEEMapping fceeMapping)
		{
			switch (fceeMapping.MsgType)
			{
				case EDGEMESSAGETYPE.Heartbeat:
					return new List<string> { "Counter" };

				case EDGEMESSAGETYPE.SoftwareVersion:
					return new List<string> { "Software Version"
									, "Minimum Time"
									, "Maximum Time"
									, "Level Deadband"
									, "Temperature Deadband"
									, "Heartbeat"
									, "TLS Tanks"
									, "Modbus Map"
									, "Midnight Offset"
									, "Short Deadband"
									, "Short Time"
									, "Long Deadband"
									, "Long Time"};

				case EDGEMESSAGETYPE.DeviceStatus:
					return new List<string> { "Device Type"
									, "Device Status"};

				case EDGEMESSAGETYPE.Enraf854TankGauge:
					return new List<string> { "Level Product"
									, "Temperature Product"
									, "Level Water"
									, "Gauge Position"
									, "Gauge Status" };

				case EDGEMESSAGETYPE.Enraf854TankGaugeDensity:
					return new List<string> { "Density Product Observed"
									, "Temperature Density"};

				case EDGEMESSAGETYPE.ModbusIntegerRegisterBlock:
					return new List<string> { "Gauge Alarm" };

				case EDGEMESSAGETYPE.GenericScalingPoint:
					switch (fceeMapping.TagSelection)
					{
							case TAGSELECTIONTYPE.LevelProduct:
								return new List<string> { "Level Product" };
							case TAGSELECTIONTYPE.TemperatureProduct:
								return new List<string> { "Temperature Product" };
							case TAGSELECTIONTYPE.LevelWater:
								return new List<string> { "Level Water" };
							case TAGSELECTIONTYPE.None:
							default:
								return new List<string> { "Value" };
					}

				case EDGEMESSAGETYPE.ITTBarton3500ATG:
					return new List<string> { "Level Product"
									, "Temperature Product"
									, "Level Water"
									, "Density Product Observed"
									, "Pressure Bottom"
									, "Gauge Alarm" };

				case EDGEMESSAGETYPE.VeederRootTLS350:
					return new List<string> { "Level Product"
								, "Temperature Product"
								, "Level Water"
								, "Volume Gross Observed"
								, "Volume Net Standard"
								, "Volume Water"
								, "Volume Gross Observed Remaining" };

				case EDGEMESSAGETYPE.VeederRootSystemStatus:
					return new List<string> { "Gauge Alarm" };

				case EDGEMESSAGETYPE.VeederRootLeakTest:
					return new List<string> { "Gauge Alarm" };

				case EDGEMESSAGETYPE.VeederRootSystemAlarms:
					return new List<string> { "Gauge Alarm" };

				case EDGEMESSAGETYPE.VeederRootInventoryReport:
					return new List<string> { "Level Product"
									, "Temperature Product"
									, "Volume Gross Observed"
									, "Volume Net Standard"
									, "Level Water"
									, "Volume Gross Observed Remaining"
									, "Volume Water" };

				case EDGEMESSAGETYPE.VeederRootInTankStatusReport:
					return new List<string> { "Gauge Alarm" };

				case EDGEMESSAGETYPE.ModbusInventory:
					return new List<string> {"Level Product"
												, "Temperature Product"
												, "Level Water"
												, "Gauge Position"
												, "Gauge Status"
												, "Water Sump"
												, "Volume Gross Observed"
												, "Volume Water" };

				case EDGEMESSAGETYPE.ModbusDensityAndAlarm:
					return new List<string> {"Density Product Observed"
												, "Temperature Density"
												, "Trouble Info"
												, "Level Alarm" };

				case EDGEMESSAGETYPE.ModbusFacilityStatus:
					return new List<string> { "Facility Status" };

				case EDGEMESSAGETYPE.ModbusStorage:
					return new List<string> {"Level Product"
												, "Temperature Product"
												, "Level Water"
												, "Gauge Position"
												, "Density Product Observed" };
				case EDGEMESSAGETYPE.CommandStatus:
					return new List<string> {"Command Status"
												, "Command Schedule" };

				case EDGEMESSAGETYPE.WAGOPLC:
					 return new List<string> {"Device Type"
													 ,"Level Product"
													 ,"Temperature Product"
													 ,"Density Product Observed"
													 ,"Temperature Density"
													 ,"Gauge Position"
													 ,"Level Water"
													 ,"Water Sump"
													 ,"Gauge Status"
													 ,"Trouble Info"
													 ,"Level Alarm"
													 ,"Volume Gross Observed"
													 ,"Volume Water"
													 ,"Volume Net Standard"
													 ,"Volume Gross Observed Remaining"
													 ,"Water Sump Volume" };


					 default:
					return new List<string> {"Value" };
			}
		}

		protected string GetEdgeDataTagFromFuelsManagerTag(EDGEMESSAGETYPE msgType, string fuelsManagerTag)
		{
			if (msgType == EDGEMESSAGETYPE.GenericScalingPoint)
			{
				return "Value";
			}
			switch (fuelsManagerTag)
			{
				case "Software Version":
					return "SWVersion";
				case "Minimum Time":
					return "MinTime";
				case "Maximum Time":
					return "MaxTime";
				case "Level Deadband":
					return "LevelDeadband";
				case "Temperature Deadband":
					return "TempDeadband";
				case "TLS Tanks":
					return "TLStanks";
				case "Modbus Map":
					return "ModbusMap";
				case "Midnight Offset":
					return "MidnightOffset";
				case "Short Deadband":
					return "ShortDeadband";
				case "Short Time":
					return "ShortTime";
				case "Long Deadband":
					return "LongDeadband";
				case "Long Time":
					return "LongTime";
				case "Device Type":
					return "DeviceType";
				case "Device Status":
					return "DeviceStatus";

					//Enraf 854 Tank Gauge
				case "Level Product":
					return "Level";
				case "Temperature Product":
					return "Temp";
				case "Level Water":
					return "WaterLevel";
				case "Gauge Position":
					return "Position";
				case "Gauge Status":
					return "GaugeStatus";

					//Enraf 854 Density
				case "Density Product Observed":
					return "Density";
				case "Temperature Density":
					return "DensityTemp";

					//Barton 3500
				case "Pressure Bottom":
					return "Value1";

				case "Volume Gross Observed":
					switch (msgType)
					{
							case EDGEMESSAGETYPE.VeederRootInventoryReport:
								return "Volume";
							case EDGEMESSAGETYPE.ModbusInventory:
								return "FuelVolume";
							default:
								return "GrossVolume";
					}

				case "Volume Net Standard":
					switch (msgType)
					{
							case EDGEMESSAGETYPE.VeederRootInventoryReport:
								return "TCVolume";
							default:
								return "NetVolume";
					}
				case "Volume Water":
					switch (msgType)
					{
							case EDGEMESSAGETYPE.VeederRootInventoryReport:
								return "Water";
							default:
								return "WaterVolume";
					}

				case "Volume Gross Observed Remaining":
					return "Ullage";

				case "Water Sump":
					return "WaterSump";

				case "Trouble Info":
					return "TroubleInfo";
				case "Level Alarm":
					return "LevelAlarm";

				case "Gauge Alarm":
					return "PntStatus";

				case "Facility Status":
					return "FacilityStatus";

				case "Command Status":
					return "CmdStatus";
				case "Command Schedule":
					return "CmdSchedule";
				case "Water Sump Volume":
					return "WaterSumpVol";

				default:
					return fuelsManagerTag;
			}
		}

		// Retrieves the last HeartbeatMsg for every FCE Device
		protected Dictionary<Guid, FCEDeviceWithLastHeartbeat> GetDevicesWithLastHeartbeat(SecurityClass security)
		{
			security.ThrowIfNull("security");
			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_FCEEnumerateDevicesWithLastHeartbeat";

				set = ConsolidatedDA.GetDataSet(cmd, security);
			}
			return PopulateDevicesWithLastHeartbeatsList(set);
		}

		protected List<FCEEMessage> TimeoutDeviceMessages(SecurityClass security, Guid fceDeviceGuid)
		{
			security.ThrowIfNull("security");
			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_FCEEDeviceTimeoutMessages";
				cmd.Parameters.AddWithValue("@FCEDeviceGuid", fceDeviceGuid);

				set = ConsolidatedDA.GetDataSet(cmd, security);
			}
			return PopulateList(set);
		}

		protected List<FCEEMessage> LatestDeviceMessages(SecurityClass security, Guid fceDeviceGuid, int index, bool validity)
		{
			security.ThrowIfNull("security");
			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_FCEEDeviceLatestMessagesByImeiIndex";
				cmd.Parameters.AddWithValue("@FCEDeviceGuid", fceDeviceGuid);
				cmd.Parameters.AddWithValue("@index", index);
				cmd.Parameters.AddWithValue("@validity", validity);

				set = ConsolidatedDA.GetDataSet(cmd, security);
			}
			return PopulateList(set);
		}

		protected Dictionary<Guid, FCEDeviceWithLastHeartbeat> PopulateDevicesWithLastHeartbeatsList(DataSet set)
		{
			Dictionary<Guid, FCEDeviceWithLastHeartbeat> fceDevicesWithLastHeartbeat = new Dictionary<Guid, FCEDeviceWithLastHeartbeat>();
			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var fceDeviceWithLastHeartbeat = new FCEDeviceWithLastHeartbeat();
				fceDeviceWithLastHeartbeat.AutoLoad(row);

				fceDevicesWithLastHeartbeat.Add(fceDeviceWithLastHeartbeat.FCEDeviceGuid, fceDeviceWithLastHeartbeat);
			}
			return fceDevicesWithLastHeartbeat;
		}

		// Called periodically from FuelsManager Service to see if a device has not sent a heartbeat in time
		public void ProcessFceHeartbeats(SecurityClass security)
		{
			FCEDevices fCEDevices = new FCEDevices();
			Dictionary<Guid, FCEDeviceWithLastHeartbeat> fceDevicesWithLastHeartbeat = this.GetDevicesWithLastHeartbeat(security);
			Dictionary<Guid, FCEDeviceWithLastHeartbeat> expiredFceDevices = new Dictionary<Guid, FCEDeviceWithLastHeartbeat>();
			foreach (FCEDeviceWithLastHeartbeat fceDevice in fceDevicesWithLastHeartbeat.Values)
			{
				DateTimeOffset dateNow = DateTime.UtcNow;
				TimeSpan timeDifference = dateNow.Subtract(fceDevice.Timestamp);
				if (timeDifference.TotalMinutes > (fceDevice.Heartbeat * 2) && !fceDevice.HeartbeatTimeoutProcessed) // heartbeat has expired and hasn't been processed
				{
					this.SetNoResponse(security, fceDevice.FCEDeviceGuid, 0, 0, 1);
					FCEDevice fceDeviceToUpdate = fCEDevices.Get(security, fceDevice.IdentityGuid);
					fceDeviceToUpdate.HeartbeatTimeoutProcessed = true;
					fCEDevices.Modify(security, fceDeviceToUpdate);
				}
				else if (timeDifference.TotalMinutes < (fceDevice.Heartbeat * 2) && fceDevice.HeartbeatTimeoutProcessed) // device has come back online, clear the heartbeat
				{
					FCEDevice fceDeviceToUpdate = fCEDevices.Get(security, fceDevice.IdentityGuid);
					fceDeviceToUpdate.HeartbeatTimeoutProcessed = false;
					fCEDevices.Modify(security, fceDeviceToUpdate);
				}
			}
		}

		public void SetNoResponse(SecurityClass security, Guid deviceGuid, int index, int deviceType, int deviceStatus)
		{
			bool useLastKnownGood = false;
			bool validity;

			if (deviceStatus == 1 || deviceType == 0)
			{
				validity = false;
			}
			else // deviceStatus = 0; device has recovered
			{
				validity = true;
			}
				

			try
			{
				ConfigurationSettingsClass configurationSettings = new ConfigurationSettingsClass();
				string stenableUseLastKnownGood = configurationSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_UseLastKnownGoodStatus);
				if (stenableUseLastKnownGood == "1")
				{
					useLastKnownGood = true;
				}
			}
			catch (Exception e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("FCEE Service Message: " + e.Message, FMEventLogEntryType.Error);
			}

			List<FCEEMessage> messagesForReprocess;
			List<EdgeMessageType> validMessageTypesForReprocess;
			switch (deviceType)
			{
				case 0: //Heartbeat
					messagesForReprocess = this.TimeoutDeviceMessages(security, deviceGuid);
					break;

				//Device status processing will go here
				case 1:
					validMessageTypesForReprocess = new List<EdgeMessageType>() {
							EdgeMessageType.Enraf854TankGauge,
							EdgeMessageType.Enraf854TankGaugeDensity,
							EdgeMessageType.ModbusIntegerRegisterBlock,
							EdgeMessageType.GenericScalingPoint,
							EdgeMessageType.ITTBarton3500ATG,
							EdgeMessageType.VeederRootTLS350,
							EdgeMessageType.VeederRootSystemStatus,
							EdgeMessageType.VeederRootLeakTest,
							EdgeMessageType.VeederRootSystemAlarms,
					};
					messagesForReprocess = this.LatestDeviceMessages(security, deviceGuid, index, validity).Where(x => validMessageTypesForReprocess.Contains((EdgeMessageType)(x.MsgType))).ToList();
					break;

				case 2:
					validMessageTypesForReprocess = new List<EdgeMessageType>() {
							EdgeMessageType.VeederRootInventoryReport,
							EdgeMessageType.VeederRootInTankStatusReport,
							EdgeMessageType.VeederRootLiquidSensorStatusReport,
					};


					messagesForReprocess = new List<FCEEMessage>();
					for (var i = 1; i<=16; i++) { 
							messagesForReprocess = messagesForReprocess.Union(this.LatestDeviceMessages(security, deviceGuid, i, validity).Where(x => validMessageTypesForReprocess.Contains((EdgeMessageType)(x.MsgType)))).ToList();
					}
					break;

				case 3:
					validMessageTypesForReprocess = new List<EdgeMessageType>() {
							EdgeMessageType.VeederRootInventoryReport,
							EdgeMessageType.VeederRootInTankStatusReport,
					};
					messagesForReprocess = this.LatestDeviceMessages(security, deviceGuid, index, validity).Where(x => validMessageTypesForReprocess.Contains((EdgeMessageType)(x.MsgType))).ToList();
					break;

				case 4:
					validMessageTypesForReprocess = new List<EdgeMessageType>() {
							EdgeMessageType.VeederRootLiquidSensorStatusReport,
					};
					messagesForReprocess = this.LatestDeviceMessages(security, deviceGuid, index, validity).Where(x => validMessageTypesForReprocess.Contains((EdgeMessageType)(x.MsgType))).ToList();
					break;

				case 5:
					validMessageTypesForReprocess = new List<EdgeMessageType>() {
							EdgeMessageType.ModbusInventory,
							EdgeMessageType.ModbusDensityAndAlarm,
							EdgeMessageType.ModbusFacilityStatus,
							EdgeMessageType.ModbusStorage,
					};

					messagesForReprocess = new List<FCEEMessage>();
					for (var i = 1; i <= 11; i++)
					{
							messagesForReprocess = messagesForReprocess.Union(this.LatestDeviceMessages(security, deviceGuid, i, validity).Where(x => validMessageTypesForReprocess.Contains((EdgeMessageType)(x.MsgType)))).ToList();
					}
					break;
				case 6:
					validMessageTypesForReprocess = new List<EdgeMessageType>() {
							EdgeMessageType.WAGOPLC,
					};
					messagesForReprocess = this.LatestDeviceMessages(security, deviceGuid, index, validity).Where(x => validMessageTypesForReprocess.Contains((EdgeMessageType)(x.MsgType))).ToList();
					break;

				default:
					messagesForReprocess = null;
					break;
			}


			if (messagesForReprocess != null && messagesForReprocess.Count > 0)
			{
				FCEDevices fceDevices = new FCEDevices();
				var fceDevice = fceDevices.Get(security, deviceGuid);
				if (fceDevice != null)
				{
					foreach (FCEEMessage fceeMessage in messagesForReprocess)
					{
						using (MemoryStream stream = new MemoryStream(fceeMessage.BinaryData))
						{
							ProcessMessage(security, fceDevice, stream, (EdgeMessageType)fceeMessage.MsgType, useLastKnownGood, false, validity);
						}
					}
				}
			}
		}
	}
}
