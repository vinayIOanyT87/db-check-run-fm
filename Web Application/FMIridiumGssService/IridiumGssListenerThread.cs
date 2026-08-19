namespace FMIridiumGssService
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.Net.Sockets;
	using System.Threading;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Parsers;

	public class IridiumGssListenerThread : BaseThread
	{
		#region Private data members
		private int listenerInterval;
		private bool listernerStartFlag;
		private int listenerPortNumber;
		private string listenerIpAddressStr;
		private IPAddress listenerIpAddress;
		private string rawDataFilePath;
		private int debugDelayInterval;
		private SecurityClass security;
		private int listenerRestartCount;

		private const int MaxRestartCount			= 3;
		private const string ListeningIntervalKey	= "ListeningInterval";
		private const string ListeningIpAddressKey	= "ListeningIpAddress";
		private const string ListeningPortNumberKey = "ListeningPortNumber";
		private const string RawDataFilePathKey		= "RawDataFilePath";
		private const string DebugDelayKey			= "DebugDelay";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public IridiumGssListenerThread()
		{		
			this.Init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Get or Set the listener start flag.
		/// </summary>
		public bool ListenerStartFlag
		{
			get { return this.listernerStartFlag; }
			set { this.listernerStartFlag = value; }
		}

		/// <summary>
		/// Get or set the listener restart count.
		/// </summary>
		public int ListenerRestartCount
		{
			get { return this.listenerRestartCount; }
			set { this.listenerRestartCount = value; }
		}

		/// <summary>
		/// Gets the listener port number.
		/// </summary>
		public int ListenerPortNumber
		{
			get { return this.listenerPortNumber; }
		}

		/// <summary>
		/// Gets the listener IP address as a string.
		/// </summary>
		public string ListenerIpAddressStr
		{
			get { return this.listenerIpAddressStr; }
		}
		#endregion

		#region Protected methods
		/// <summary>
		/// This method implements the thread handler and starts
		/// the listening.
		/// </summary>
		protected override void ThreadHandler()
		{
            try
			{
				this.eventLog.WriteEntry("The Iridium GSS Listener is ready.", EventLogEntryType.Information);
				this.StartListening();
			}
			catch (Exception ex)
			{
				// Ignore the "A blocking operation was interrupted by a call to WSACancelBlockingCall" 
				// because it is due to the socket being closed.  Log all other exceptions. HResult = -2147467259
				if (ex.Message.Contains("A blocking operation was interrupted by a call to WSACancelBlockingCall"))
				{
					this.ListenerStartFlag = false;
					this.Cleanup();
					return;
				}

				if (this.ListenerStartFlag)
				{
					this.Cleanup();

					if (this.listenerRestartCount < MaxRestartCount)
					{
						this.listenerRestartCount++;
						string msg = "Restarted TCP Client Listener #: " + this.listenerRestartCount + "  Due to error: " + ex.HResult + " " + ex.Message;
						this.eventLog.WriteEntry(msg, EventLogEntryType.Error);

						// Try again
						Thread.Sleep(this.listenerInterval);
						this.Listen();
					}
					else
					{
						this.eventLog.WriteEntry("Retry reach max count, stopping the service.", EventLogEntryType.Error);
						this.StopService();
					}
				}
				else
				{
					string msg = "Error: " + ex.HResult + "; " + ex.Message;
					this.eventLog.WriteEntry(msg, EventLogEntryType.Error);
					this.Cleanup();
					this.StopService();
				}
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method starts the tracking process.
		/// </summary>
		private void StartListening()
		{
			// Get the Debug delay value.
			string debugDelayIntervalStr = ConfigurationManager.AppSettings[DebugDelayKey];

			if (string.IsNullOrEmpty(debugDelayIntervalStr) == false)
			{
				int debugDelay;

				if (int.TryParse(debugDelayIntervalStr, out debugDelay))
				{
					// Debug delay is set seconds in the app.config file.
					this.debugDelayInterval = debugDelay * 1000;
				}
			}

			// This sleep statement is so you can attach the debugger before
			// the listening process begins. This is set in the app.config file.
			// If the configuration is not set or if set to zero no delay.
			Thread.Sleep(this.debugDelayInterval);

			// Build the security object and ensure that it was successful.
			// Terminate since we cannot write to the database without a 
			// valid security object.
			if (this.BuildSecurity() == false)
			{
				this.Cleanup();
				this.StopService();
				return;
			}

			try
			{
                FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.CheckActivatedLicenceVersion());
			}
			catch
			{
                this.Cleanup();
                this.StopService();
                return;
            }
           
            this.ReadConfiguration();
			this.Listen();
		}

		/// <summary>
		/// This method will read the tracking list and kill the processes
		/// in the list.
		/// </summary>
		private void Listen()
		{
			// Create an instance of the TcpListener class.
			try
			{
				// Set the listener on the configured IP address 
				// and specify the port.
				this.tcpListener = new TcpListener(this.listenerIpAddress, this.listenerPortNumber);
				this.tcpListener.Start();
				this.eventLog.WriteEntry("Iridium TCP/IP listener waiting for connection...", EventLogEntryType.Information);
			}
			catch (Exception e)
			{
				string errMsg = "Could not start the Iridium TCP/IP listener. " + e.Message;
				this.eventLog.WriteEntry(errMsg, EventLogEntryType.Error);
				this.ListenerStartFlag = false;
				this.Cleanup();
				this.StopService();
				return;
			}

			while (this.listernerStartFlag)
			{
				// Create a TCP socket. 
				// If you ran this server on the desktop, you could use 
				// Socket socket = tcpListener.AcceptSocket() 
				// for greater flexibility.
				this.tcpClient = this.tcpListener.AcceptTcpClient();
				this.eventLog.WriteEntry("Received message from Gateway.", EventLogEntryType.Information);

				// Get the message header bytes which contains the length of the message.
				var messageHeaderBytes = new byte[3];
				this.stream = this.tcpClient.GetStream();
				this.stream.Read(messageHeaderBytes, 0, 3);

				// Total message length is in bytes 1 and 2.
				int totalMessageLength = (messageHeaderBytes[1] << 8) | messageHeaderBytes[2];

				// Read the data stream from the client. 
				var messageBytes = new byte[totalMessageLength];
				this.stream.Read(messageBytes, 0, messageBytes.Length);

				byte[] totalBytes = this.CombinedBytes(messageHeaderBytes, messageBytes);

				// Create response message.
				var confirmationMessage = new byte[4];
				confirmationMessage[0] = 0x05; // Confirmation header IEI.
				confirmationMessage[1] = 0x0;  // Upper byte value of the confirmation length
				confirmationMessage[2] = 0x1;  // Lower byte value of the confirmation length

				try
				{
					// Save the raw data to a file.
					this.SaveRawData(totalBytes);

					// Set confirmation to successful = 1.
					confirmationMessage[3] = 0x1;
				}
				catch (Exception ex)
				{
					this.eventLog.WriteEntry("Error saving raw data. " + ex.Message, EventLogEntryType.Error);
					// Set confirmation to failed = 0.
					confirmationMessage[3] = 0x0;
				}

				// Write the confirmation message back to the gateway.
				this.stream.Write(confirmationMessage, 0, confirmationMessage.Length);

				// Close the TCP client
				this.tcpClient.Close();
				this.eventLog.WriteEntry("Message processed connection closed.", EventLogEntryType.Information);

				try
				{
					this.ParseAndSaveToDb(totalBytes);
				}
				catch (Exception ex)
				{
					if (this.security.SiteGuid != Guids.SiteAdminGuid)
					{
						this.security.SiteGuid = Guids.SiteAdminGuid;
					}

					string errMsg = "Could not parse and save data to the database. " + ex.Message;
					this.eventLog.WriteEntry(errMsg, EventLogEntryType.Error);
				}

				// Reset listener retry count
				this.listenerRestartCount = 0;

				// Always use a Sleep call in a while(true) loop 
				// to avoid locking up your CPU.
				Thread.Sleep(this.listenerInterval);
			}
		}

		/// <summary>
		/// This method will combine the message header bytes and the message bytes into one
		/// byte array.
		/// </summary>
		/// <param name="headerBytes">This is the message header bytes (3 bytes)</param>
		/// <param name="messageBytes">This is the message bytes.</param>
		/// <returns></returns>
		private byte[] CombinedBytes(byte[] headerBytes, byte[] messageBytes)
		{
			var totalBytes = new byte[messageBytes.Length + 3];

			for (int nextByte = 0; nextByte < 3; nextByte++)
			{
				totalBytes[nextByte] = headerBytes[nextByte];
			}

			for (int nextByte = 0; nextByte < messageBytes.Length; nextByte++)
			{
				totalBytes[nextByte + 3] = messageBytes[nextByte];
			}

			return totalBytes;
		}

		/// <summary>
		/// This method will read the configuration setting where the tracking list is
		/// located and the scan interval setting.
		/// </summary>
		private void ReadConfiguration()
		{
			string listeningIntervalStr		= ConfigurationManager.AppSettings[ListeningIntervalKey];
			this.listenerIpAddressStr		= ConfigurationManager.AppSettings[ListeningIpAddressKey];
			string listeningPortNumberStr	= ConfigurationManager.AppSettings[ListeningPortNumberKey];
			this.rawDataFilePath			= ConfigurationManager.AppSettings[RawDataFilePathKey];

			if (string.IsNullOrEmpty(listeningIntervalStr) == false)
			{
				int interval;

				if (int.TryParse(listeningIntervalStr, out interval))
				{
					// The configured listening interval is set in milli-seconds in the app.config file.
					this.listenerInterval = interval;
				}
			}

			if (string.IsNullOrEmpty(this.rawDataFilePath))
			{
				const string ErrMsg = "Must have the raw data file path set.";
				throw new Exception(ErrMsg);
			}

			if (string.IsNullOrEmpty(listeningPortNumberStr))
			{
				const string ErrMsg = "The Iridium GSS Listener must have a valid Port Number.";
				throw new Exception(ErrMsg);
			}

			int portNumber;

			if (int.TryParse(listeningPortNumberStr, out portNumber))
			{
				this.listenerPortNumber = portNumber;

				if (portNumber < 1 || portNumber > 65535)
				{
					const string ErrMsg = "The Iridium GSS Listener port number must be between 1 - 65,535.";
					throw new Exception(ErrMsg);
				}
			}

			if (string.IsNullOrEmpty(this.listenerIpAddressStr))
			{
				this.listenerIpAddress = Dns.GetHostEntry("localhost").AddressList[0];
				this.listenerIpAddressStr = this.listenerIpAddress.ToString();

				string infoMsg = "Defaulting to local host IP address" + this.ListenerIpAddressStr;
				this.eventLog.WriteEntry(infoMsg, EventLogEntryType.Information);
				return;
			}

			string[] parts = this.listenerIpAddressStr.Split('.');

			if (parts.Length < 4)
			{
				const string ErrMsg = "The Iridium GSS Listener must have a valid IP address xxx.xxx.xxx.xxx";
				throw new Exception(ErrMsg);		
			}

			foreach (string part in parts)
			{
				int addressPart;

				if (int.TryParse(part, out addressPart) == false)
				{
					const string ErrMsg = "The Iridium GSS Listener must have a valid IP address xxx.xxx.xxx.xxx";
					throw new Exception(ErrMsg);							
				}
			}

			// Parse the string IP address into an real IP address.
			this.listenerIpAddress = IPAddress.Parse(this.listenerIpAddressStr);
		}

		/// <summary>
		/// This method will save the raw data to a file located in the
		/// directory specified in the configuration.
		/// </summary>
		/// <param name="rawData">Contains the raw data to be saved.</param>
		private void SaveRawData(byte[] rawData)
		{
			string path = this.GetDeviceName(rawData);
			List<string> rawDataByteStrlist = this.ConvertBytesToStringRepresentation(rawData);

			File.WriteAllLines(path, rawDataByteStrlist);
		}

		/// <summary>
		/// This method will parse out the device name from the raw data.
		/// </summary>
		/// <param name="rawData">The byte array containing the message
		/// </param>
		/// <returns>Returns the Device ID or Unknown.</returns>
		private string GetDeviceName(byte[] rawData)
		{
			string deviceName = this.rawDataFilePath + "\\";

			if (rawData == null || rawData.Length < 28)
			{
				deviceName = "Unknown";
			}
			else
			{
				// The device name starts at byte 10 and ends on byte 24.
				for (int nextByte = 10; nextByte < 25; nextByte++)
				{
					// Check for ASCII value of 0 - 9, A - Z, a - z, or a space.
					if ((rawData[nextByte] >= 0x30 && rawData[nextByte] <= 0x39)
					    || (rawData[nextByte] >= 0x41 && rawData[nextByte] <= 0x5A)
						|| (rawData[nextByte] >= 0x61 && rawData[nextByte] <= 0x7A)
						|| (rawData[nextByte] == 0x20))
					{
						char charValue = (char)rawData[nextByte];
						deviceName = deviceName + charValue;
					}
					else
					{
						int errChar = rawData[nextByte];
						this.eventLog.WriteEntry("Non ASCII character in the message IMEI name. Value: " + errChar);
						deviceName = "Unknown";
						break;
					}
				}
			}

			var currentDateTime = DateTime.Now;

			string monthStr		= currentDateTime.Month < 10 ? "0" + currentDateTime.Month : currentDateTime.Month.ToString();
			string dayStr		= currentDateTime.Day < 10 ? "0" + currentDateTime.Day : currentDateTime.Day.ToString();
			string hourStr		= currentDateTime.Hour < 10 ? "0" + currentDateTime.Hour : currentDateTime.Hour.ToString();
			string minuteStr	= currentDateTime.Minute < 10 ? "0" + currentDateTime.Minute : currentDateTime.Minute.ToString();
			string secondStr	= currentDateTime.Second < 10 ? "0" + currentDateTime.Second : currentDateTime.Second.ToString();

			deviceName = deviceName.Trim();
			deviceName = deviceName + "_" + currentDateTime.Year + monthStr + dayStr + "_" + hourStr + minuteStr + secondStr + ".txt";
			return deviceName;
		}

		/// <summary>
		/// This method will convert the byte to a byte string representation
		/// of the data. It will return the collection.
		/// </summary>
		/// <param name="rawData">The raw data to covert.</param>
		/// <returns>Return the raw data string collection representation of the byte.</returns>
		private List<string> ConvertBytesToStringRepresentation(byte[] rawData)
		{
			var convertedByteList = new List<string>();
			int byteCount = 0;

			foreach (byte rawDataByte in rawData)
			{
				string s1 = (rawDataByte & 0x01).ToString();
				string s2 = ((rawDataByte & 0x02) >> 1).ToString();
				string s3 = ((rawDataByte & 0x04) >> 2).ToString();
				string s4 = ((rawDataByte & 0x08) >> 3).ToString();
				string s5 = ((rawDataByte & 0x10) >> 4).ToString();
				string s6 = ((rawDataByte & 0x20) >> 5).ToString();
				string s7 = ((rawDataByte & 0x40) >> 6).ToString();
				string s8 = ((rawDataByte & 0x80) >> 7).ToString();
				string rawDataByteStr = s8 + s7 + s6 + s5 + s4 + s3 + s2 + s1 + " >> Byte number: " + byteCount++;

				convertedByteList.Add(rawDataByteStr);
			}

			return convertedByteList;
		}

		/// <summary>
		/// This method will parse the Iridium gateway message and save the
		/// data to the database "tblAssetTrackingDetail".
		/// </summary>
		/// <param name="messageArray">Iridium message to parse.</param>
		private void ParseAndSaveToDb(byte[] messageArray)
		{
			var iridiumMessageParser = new IridiumMessageParser();
			iridiumMessageParser.Parse(messageArray);

			double? latitude = iridiumMessageParser.Latitude;
			double? longitude = iridiumMessageParser.Longitude;

			double overrideLatitude;
			double overrideLongitude;
			bool overrideCoordinates = this.OverrideCoordinateData(iridiumMessageParser, out overrideLatitude, out overrideLongitude);

			// Use the payload GPS data instead of the Iridium GPS data.
            // This happens with the WRDCU payload since it contains a more accurate measurement.
			if (overrideCoordinates)
			{
				latitude = overrideLatitude;
				longitude = overrideLongitude;
			}

			var trackingTankList				= new List<AssetTrackingTankClass>();
			string equipmentId					= "UNKNOWN";
			string productId					= "UNKNOWN";
			double? productDensity				= null;
			double? productDielectricTolerance	= null;
			bool anyContamination				= false;
            EngineeringUnit sourceUnit			= EngineeringUnit.FmvMeter3;
			Guid equipmentSiteGuid				= this.security.SiteGuid;

            // Handle the WRDCU payload data.
			if (iridiumMessageParser.WrdcuPayloadParser.HasWrdcuData)
			{

                Guid deviceGuid = FMChannelHelper.MakeCall<IAssetTrackingDevices, Guid>(x => x.GetIdentityGuidWithoutSite(this.security, iridiumMessageParser.Imei));

                if (deviceGuid != Guid.Empty)
                {
                    var productAndEquipment = FMChannelHelper.MakeCall<IAssetTrackingDevices, AssetTrackingDeviceClass>
                                                                        (x => x.GetAssociatedEquipmentIdAndProduct(this.security, deviceGuid));

                    var assetTrackingDevice = FMChannelHelper.MakeCall<IAssetTrackingDevices, AssetTrackingDeviceClass>
                                                        (x => x.GetByIdentityGuid(this.security, deviceGuid));

                    equipmentSiteGuid           = productAndEquipment.EquipmentSiteGuid;
                    equipmentId                 = productAndEquipment.EquipmentId;
                    productId                   = productAndEquipment.ProductId;
                    productDensity              = productAndEquipment.ProductDensity;
                    productDielectricTolerance  = productAndEquipment.ProductDielectricTolerance;
                    sourceUnit                  = assetTrackingDevice.SourceUnit;
                }

                var previousTankList =  FMChannelHelper.MakeCall<IAssetTrackingDetails, List<AssetTrackingTankClass>>(
														x => x.GetPreviousDetailTanks(iridiumMessageParser.Imei, this.security));

				ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetByID(this.security, productId));

				foreach (WrdcuData tank in iridiumMessageParser.WrdcuPayloadParser.WrdcuTankList)
				{
					bool contaminated = this.IsContaminated(tank.TankConfigurationNumber.ToString(), tank.Dielectric, previousTankList, productDielectricTolerance, product);
					double convertedVolume = this.ConvertToKnownUnit(sourceUnit, tank.Volume);

					var trackingTank = new AssetTrackingTankClass
											{
												Dielectric		= tank.Dielectric,
												Density			= productDensity,
												ProductId		= productId,
												TankId			= tank.TankConfigurationNumber.ToString(),
												Volume			= convertedVolume,
												Contaminated	= contaminated
					};

					trackingTankList.Add(trackingTank);

					if (contaminated)
					{
						anyContamination = true;
					}
				}

				this.SaveWrdcuDataToDb(iridiumMessageParser, deviceGuid, sourceUnit, equipmentSiteGuid);
			}

            // Handle the TDU payload data
            if (iridiumMessageParser.TduPayloadParser.HasTduData)
            {
                this.SaveTduDataToDb(iridiumMessageParser);
            }

            var assetTrackingDetail = new AssetTrackingDetailClass
			                          {
				                          Latitude				= latitude,
				                          Longitude				= longitude,
										  CepRadius				= (int)iridiumMessageParser.CepRadius,
				                          Momsn					= iridiumMessageParser.Momsn,
				                          Mtmsn					= iridiumMessageParser.Mtmsn,
				                          AssetSessionDateTime	= iridiumMessageParser.SessionDateTime,
				                          AssetTrackingDeviceId = iridiumMessageParser.Imei,
										  AssetSessionStatus	= (int)iridiumMessageParser.SessionStatus,
										  PayloadValues			= iridiumMessageParser.AssetTrackingPayloadCollection,
										  TrackingTanks			= trackingTankList,
										  CdrReference			= (int)iridiumMessageParser.CdrReference,
										  PayloadType			= iridiumMessageParser.PayloadType,
										  ChecksumFlag			= iridiumMessageParser.ChecksumFlag,
										  Contaminated			= anyContamination,
										  EquipmentId			= equipmentId,
										  ProductId				= productId
			};

			assetTrackingDetail.AssetTrackingDetailGuid = 
								FMChannelHelper.MakeCall<IAssetTrackingDetails, Guid>(x => x.Add(this.security, assetTrackingDetail));
		}

		/// <summary>
		/// This method will determine whether to override the coordinate information based on if the 
		/// payload data has coordinate information.
		/// </summary>
		/// <param name="iridiumMessageParser">The parser object.</param>
		/// <param name="latitude">The overriden latitude.</param>
		/// <param name="longitude">The overriden longitude.</param>
		/// <returns>Returns true if overriding coordinates, otherwise false.</returns>
		private bool OverrideCoordinateData(IridiumMessageParser iridiumMessageParser, out double latitude, out double longitude)
		{
			latitude = 0;
			longitude = 0;
			bool overrideCoordinates = false;

			// Use the WRDCU GPS data instead of the Iridium GPS data.
			if (iridiumMessageParser.WrdcuPayloadParser.HasWrdcuData)
			{
				latitude = iridiumMessageParser.WrdcuPayloadParser.Latitude;
				longitude = iridiumMessageParser.WrdcuPayloadParser.Longitude;

				const string InfoMsg = "Using coordinate data from the WRDCU payload.";
				this.eventLog.WriteEntry(InfoMsg, EventLogEntryType.Information);

				overrideCoordinates = true;
			}

			// Use the Position GPS data instead of the Iridium GPS data.
			if (iridiumMessageParser.PositionPayloadParser.HasCoordinateData)
			{
				latitude = iridiumMessageParser.PositionPayloadParser.Latitude;
				longitude = iridiumMessageParser.PositionPayloadParser.Longitude;

				const string InfoMsg = "Using coordinate data from the Position Only payload.";
				this.eventLog.WriteEntry(InfoMsg, EventLogEntryType.Information);

				overrideCoordinates = true;
			}

			return overrideCoordinates;
		}

		/// <summary>
		/// This method will save the WRDCU tank volumes into the associated Equipment record.
		/// </summary>
		/// <param name="iridiumMessageParser"></param>
		/// <param name="deviceGuid"></param>
		/// <param name="sourceUnit"></param>
		/// <param name="equipmentSiteGuid"></param>
		private void SaveWrdcuDataToDb(IridiumMessageParser iridiumMessageParser, Guid deviceGuid, EngineeringUnit sourceUnit, Guid equipmentSiteGuid)
		{
			if (deviceGuid == Guid.Empty)
			{
				string warningMsg = "Could not find Asset Tracking Device for ID: " + iridiumMessageParser.Imei;
				this.eventLog.WriteEntry(warningMsg, EventLogEntryType.Warning);
				return;
			}

			Guid equipmentGuid = FMChannelHelper.MakeCall<IAssetTrackingDevices, Guid>(x => x.GetAssociatedEquipmentGuid(this.security, deviceGuid));

			if (equipmentGuid == Guid.Empty)
			{
				string warningMsg = "Could not find associated Equipment for Device ID: " + iridiumMessageParser.Imei;
				this.eventLog.WriteEntry(warningMsg, EventLogEntryType.Warning);
				return;
			}

			// Save the current security site GUID.
			Guid loginSiteGuid = this.security.LoginSiteGuid;
			Guid siteGuid = this.security.SiteGuid;

			EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.security, equipmentGuid));

			if (equipment == null || equipment.IdentityGuid == Guid.Empty)
			{
				// Update the security site GUID to the equipment site GUID in order to retrieve and update.
				this.security.LoginSiteGuid = equipmentSiteGuid;
				this.security.SiteGuid = equipmentSiteGuid;

				equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.security, equipmentGuid));

				if (equipment == null || equipment.IdentityGuid == Guid.Empty)
				{
					string warningMsg = "Could not find Equipment for Device ID: " + iridiumMessageParser.Imei;
					this.eventLog.WriteEntry(warningMsg, EventLogEntryType.Warning);

					this.security.LoginSiteGuid = loginSiteGuid;
					this.security.SiteGuid = siteGuid;

					return;
				}
			}

			double totalTankVolumes = 0.0;

			foreach(WrdcuData tank in iridiumMessageParser.WrdcuPayloadParser.WrdcuTankList)
			{
				totalTankVolumes = totalTankVolumes + tank.Volume;
			}

			string infoMsg = "Updating equipment volume for equipment ID: " + equipment.ID;
			this.eventLog.WriteEntry(infoMsg, EventLogEntryType.Information);

			// Convert to SI
			double convertedValue = 0;
			EngineeringUnits.Convert(totalTankVolumes, sourceUnit, ref convertedValue, EngineeringUnit.FmvMeter3, convertedValue);
			SIDouble siDouble = new SIDouble { Units = EngineeringUnit.FmvMeter3, Value = convertedValue };
			equipment._Volume.SIValue = siDouble.SIValue;

			FMChannelHelper.MakeCall<IEquipments>(x => x.Modify(this.security, equipment));

			this.security.LoginSiteGuid = loginSiteGuid;
			this.security.SiteGuid		= siteGuid;

			infoMsg = "Equipment volume successfully updated. Volume = " + totalTankVolumes;
			this.eventLog.WriteEntry(infoMsg, EventLogEntryType.Information);
		}

		/// <summary>
		/// This method will save the TDU data to the database.
		/// </summary>
		/// <param name="iridiumMessageParser">The Iridium Message Parser object.</param>
		private void SaveTduDataToDb(IridiumMessageParser iridiumMessageParser)
		{
			// Find the tanks that are associated to the device that matches the IMEI (a.k.a. DeviceID).
			var tankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
										x => x.EnumerateBasicInfoLinkedToAssetTrackingDevices(this.security, iridiumMessageParser.Imei));

			if (tankCollection != null && tankCollection.Count > 0)
			{
				foreach (TduData tduData in iridiumMessageParser.TduPayloadParser.TduTankList)
				{
					TankClass tank = tankCollection.Find(x => x.TankConfigurationNumber == tduData.TankConfigurationNumber);

					if (tank != null)
					{
						// Need to ensure the security has the tank site in order to update.
						this.security.SiteGuid = tank.SiteGuid;

						var fullTank = FMChannelHelper.MakeCall<ITanks, TankClass>(x => x.Get(this.security, tank.IdentityGuid));

						if (fullTank != null)
						{
							var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.security, tank.SiteGuid, false, false, false));

							foreach (ProcessVariableClass processVariable in fullTank.ProcessVariableCollection)
							{
								switch (processVariable.ProcessVariableType)
								{
									case PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV:
                                        double tduPressure = EngineeringUnits.Convert(tduData.Pressure, EngineeringUnit.FmpMBar, site.PressureUnits, 15);
                                        processVariable.SetValue(tduPressure, site.PressureUnits);
										break;
									case PROCESS_VARIABLE_TYPE.TEMPERATURE_PV:
                                        double tduTemp = EngineeringUnits.Convert(tduData.Temperature, EngineeringUnit.FmtDegF, site.TemperatureUnits, 15);
                                        processVariable.SetValue(tduTemp, site.TemperatureUnits);
										break;
									case PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV:
								        double tduVolume = EngineeringUnits.Convert(tduData.Volume, EngineeringUnit.FmvMeter3, site.VolumeUnits, 15);
										processVariable.SetValue(tduVolume, site.VolumeUnits);
										break;
								}
							}

							// Save the tank
							FMChannelHelper.MakeCall<ITanks>(x => x.Modify(this.security, fullTank));
						}
						else
						{
							string warningMsg = "Could not retrieve full tank information for tank: " + tank.ID;
							this.eventLog.WriteEntry(warningMsg, EventLogEntryType.Warning);
						}

							this.security.SiteGuid = Guids.SiteAdminGuid;
					}
					else
					{
						string warningMsg = "Could not find Tank ID: " + tduData.TankConfigurationNumber
											+ " (Asset tracking device: " + iridiumMessageParser.Imei + ")";
						this.eventLog.WriteEntry(warningMsg, EventLogEntryType.Warning);
					}
				}
			}
			else
			{
				string warningMsg = "Could not find Tanks for asset tracking device: " + iridiumMessageParser.Imei;
				this.eventLog.WriteEntry(warningMsg, EventLogEntryType.Warning);
			}
		}

		/// <summary>
		/// This method will convert the volume to the know unit (Meters cubed).
		/// </summary>
		/// <param name="sourceUnit">Asset tracking device source unit.</param>
		/// <param name="volume">The volume to convert</param>
		/// <returns>Returns the converted value.</returns>
		private double ConvertToKnownUnit(EngineeringUnit sourceUnit, double volume)
		{
			double convertedValue = 0;
			EngineeringUnits.Convert(volume, sourceUnit, ref convertedValue, EngineeringUnit.FmvMeter3, convertedValue);

			return convertedValue;
		}

		/// <summary>
		/// This method will calculate the absolute value of the dielectric differences between the current
		/// tank dielectric and the previous message tank dielectric. It will determine if the difference
		/// is greater than the dielectric tolerance.
		/// </summary>
		/// <param name="tankId">Current tank ID.</param>
		/// <param name="tankDielectric">Current tank dielectric value.</param>
		/// <param name="previousTanks">The previous message tank list.</param>
		/// <param name="dielectricTolerance">The product dielectric tolerance in percent.</param>
		/// <param name="product">The product that contains the dielectric range.</param>
		/// <returns>Returns if found to be contaminated. Otherwise, it returns false.</returns>
		private bool IsContaminated(string tankId, double tankDielectric, List<AssetTrackingTankClass> previousTanks, double? dielectricTolerance, ProductClass product)
		{
			const bool NotContaminated = false;
			const bool Contaminated = true;

			// If no tank ID, then we cannot determine contamination.
			if (string.IsNullOrEmpty(tankId))
			{
				const string InfoMsg = "Could not calculate contamination due to invalid data Tank ID.";
				this.eventLog.WriteEntry(InfoMsg, EventLogEntryType.Information);

				return NotContaminated;
			}

			// This means that there is water in the tank. Return true for contaminated.
			if (tankDielectric == 0.0)
			{
				return Contaminated;
			}

			// Cannot calculate contamination if the tolerance is zero or null.
			if (dielectricTolerance ==  null || dielectricTolerance.Value == 0.0)
			{
				const string InfoMsg = "Could not calculate contamination due to invalid Product Dielectric Tolerance (null or zero).";
				this.eventLog.WriteEntry(InfoMsg, EventLogEntryType.Information);

				return NotContaminated;
			}

			if (previousTanks == null || previousTanks.Count == 0)
			{
				const string InfoMsg = "Could not calculate contamination due no previous tank data.";
				this.eventLog.WriteEntry(InfoMsg, EventLogEntryType.Information);

				return NotContaminated;
			}

			AssetTrackingTankClass previousTank = previousTanks.Find(x => x.TankId == tankId);

			// If no data or the previous dielectric is zero, there is not need to calculate contamination.
			if (previousTank == null || previousTank.Dielectric == null || previousTank.Dielectric.Value == 0.0)
			{
				const string InfoMsg = "Could not calculate contamination due to previous tank invalid data (no tank found or dielectric of null or zero).";
				this.eventLog.WriteEntry(InfoMsg, EventLogEntryType.Information);

				return NotContaminated;
			}

			double dkLowRange;
			double dkHighRange;
			bool hasDkRange = this.GetProductDielectricRange(product, out dkLowRange, out dkHighRange);

			if (hasDkRange)
			{
				if (tankDielectric < dkLowRange || tankDielectric > dkHighRange)
				{
					return Contaminated;
				}
			}

			double dielectricTolerancePercent = dielectricTolerance.Value / 100.0;
			double difference = Math.Abs(tankDielectric - previousTank.Dielectric.Value);

			if (difference > dielectricTolerancePercent)
			{
				return Contaminated;
			}

			return NotContaminated;
		}

		/// <summary>
		/// This method will retrieve the product's dielectric (DK) range.
		/// </summary>
		/// <param name="product">The product that contains the DK range in the user data fields.</param>
		/// <param name="dkLowRange">The low range output.</param>
		/// <param name="dkHighRange">The high range output.</param>
		/// <returns>Return True if there was a range, otherwise false.</returns>
		private bool GetProductDielectricRange(ProductClass product, out double dkLowRange, out double dkHighRange)
		{
			dkLowRange = 0.0;
			dkHighRange = 0.0;

			if (product == null)
			{
				return false;
			}

			if (string.IsNullOrEmpty(product.UserData1) || string.IsNullOrEmpty(product.UserData2))
			{
				return false;
			}

			if (double.TryParse(product.UserData1, out dkLowRange) == false)
			{
				return false;
			}

			if (double.TryParse(product.UserData2, out dkHighRange) == false)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// This method will build the security object in order to use
		/// for FM Business Services.
		/// </summary>
		/// <returns>Returns true if successful. Otherwise, it will return false.</returns>
		private bool BuildSecurity()
		{
			try
			{
				this.security = new SecurityClass
				                {
					                UserGuid		= Guid.Empty,
					                LoginSiteGuid	= Guids.SiteAdminGuid,
					                SiteGuid		= Guids.SiteAdminGuid,
					                UserID			= "Administrator"
				                };

				this.security.AddRight(RIGHT.VIEW_MAPS);
				this.security.AddRight(RIGHT.VIEW_TANK_DATA);
				this.security.AddRight(RIGHT.MODIFY_TANK_DATA);
				this.security.AddRight(RIGHT.VIEW_ASSET_TRACKING_DEVICES);
				this.security.AddRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES);
				this.security.AddRight(RIGHT.VIEW_EQUIPMENT_DATA);
				this.security.AddRight(RIGHT.MODIFY_EQUIPMENT_DATA);

				return true;
			}
			catch (Exception ex)
			{
				const string ErrMsg = "Unable to login to FuelsManager. Check that the FMBusinessServices "
									  + "address specified in app.config is correct and that FMBusinessServices is running. ";
				this.eventLog.WriteEntry(ErrMsg + ex.Message, EventLogEntryType.Error);
				return false;
			}
		}

		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.listenerInterval		= 500;  // Default to 0.5 seconds.
			this.listernerStartFlag		= true;
			this.listenerPortNumber		= 65535;
			this.rawDataFilePath		= string.Empty;
			this.debugDelayInterval		= 0;
			this.listenerRestartCount	= 0;
		}
		#endregion
	}
}
