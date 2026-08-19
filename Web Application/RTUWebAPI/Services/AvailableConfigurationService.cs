using RTUWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Configuration;
using static RTUWebAPI.Models.AvailableChannel;

namespace RTUWebAPI.Services
{
	public class AvailableConfigurationService
	{
		public Dictionary<string, UInt32> OpcNodeIDLookupDictionary = new Dictionary<string, UInt32>(1000);
		// constants
		// hardcode the type of channel and graphical coordinates for the modules
		private Dictionary<string, Dictionary<string, RTUChannelDO>> ModuleDefinitions = new Dictionary<string, Dictionary<string, RTUChannelDO>>() {
				{  "CPU Module",  new Dictionary<string, RTUChannelDO>() {  { "channel1", new RTUChannelDO() { type = ChannelType.Physical, top = 245, left = 18, width = 44, height = 45 } },
																								{ "channel2", new RTUChannelDO() { type = ChannelType.Physical, top = 288, left = 20, width = 99, height = 60 }},
																								{ "channel3", new RTUChannelDO() { type = ChannelType.Physical, top = 375, left = 20, width = 48, height = 198 }},
																								{ "channel4", new RTUChannelDO() { type = ChannelType.Physical, top = 375, left = 80, width = 48, height = 198 }},
																								{ "channel5", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel6", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel7", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel8", new RTUChannelDO() { type = ChannelType.Virtual }}
																							}},
				{ "Serial Module",  new Dictionary<string, RTUChannelDO>() {  { "channel1", new RTUChannelDO() { type = ChannelType.Physical, top = 270, left = 16, width = 48, height = 175 } },
																								{ "channel2", new RTUChannelDO() { type = ChannelType.Physical, top = 270, left = 76, width = 48, height = 175 }},
																								{ "channel3", new RTUChannelDO() { type = ChannelType.Physical, top = 476, left = 16, width = 48, height = 225 }},
																								{ "channel4", new RTUChannelDO() { type = ChannelType.Physical, top = 476, left = 76, width = 48, height = 225 }},
																								{ "channel5", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel6", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel7", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel8", new RTUChannelDO() { type = ChannelType.Virtual }}
																							}},
				{ "Digital IO",  new Dictionary<string, RTUChannelDO>() {  { "channel1", new RTUChannelDO() { type = ChannelType.Physical, top = 284, left = 16, width = 140, height = 40 } },
																								{ "channel2", new RTUChannelDO() { type = ChannelType.Physical, top = 326, left = 16, width = 140, height = 40 }},
																								{ "channel3", new RTUChannelDO() { type = ChannelType.Physical, top = 368, left = 16, width = 140, height = 40 }},
																								{ "channel4", new RTUChannelDO() { type = ChannelType.Physical, top = 411, left = 16, width = 140, height = 40 }},
																								{ "channel5", new RTUChannelDO() { type = ChannelType.Physical, top = 532, left = 16, width = 140, height = 40 }},
																								{ "channel6", new RTUChannelDO() { type = ChannelType.Physical, top = 575, left = 16, width = 140, height = 40 }},
																								{ "channel7", new RTUChannelDO() { type = ChannelType.Physical, top = 617, left = 16, width = 140, height = 40 }},
																								{ "channel8", new RTUChannelDO() { type = ChannelType.Physical, top = 659, left = 16, width = 140, height = 40 }}
																							}},
				{ "Bi-Phase Mark",  new Dictionary<string, RTUChannelDO>() {  { "channel1", new RTUChannelDO() { type = ChannelType.Physical, top = 217, left = 16, width = 40, height = 110 } },
																								{ "channel2", new RTUChannelDO() { type = ChannelType.Physical, top = 340, left = 16, width = 40, height = 110 }},
																								{ "channel3", new RTUChannelDO() { type = ChannelType.Physical, top = 465, left = 16, width = 40, height = 110 }},
																								{ "channel4", new RTUChannelDO() { type = ChannelType.Physical, top = 590, left = 16, width = 40, height = 110 }},
																								{ "channel5", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel6", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel7", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel8", new RTUChannelDO() { type = ChannelType.Virtual }}
																							}},
				{ "Mark/Space",  new Dictionary<string, RTUChannelDO>() {  { "channel1", new RTUChannelDO() { type = ChannelType.Physical, top = 350, left = 16, width = 100, height = 75 } },
																								{ "channel2", new RTUChannelDO() { type = ChannelType.Physical, top = 590, left = 16, width = 100, height = 75 }},
																								{ "channel3", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel4", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel5", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel6", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel7", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel8", new RTUChannelDO() { type = ChannelType.Virtual }}
																							}},
				{ "Tankway",  new Dictionary<string, RTUChannelDO>() {  { "channel1", new RTUChannelDO() { type = ChannelType.Physical, top = 350, left = 16, width = 100, height = 75 } },
																								{ "channel2", new RTUChannelDO() { type = ChannelType.Physical, top = 590, left = 16, width = 100, height = 75 }},
																								{ "channel3", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel4", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel5", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel6", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel7", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel8", new RTUChannelDO() { type = ChannelType.Virtual }}
																							}},
				{ "Empty",  new Dictionary<string, RTUChannelDO>() {  { "channel1", new RTUChannelDO() { type = ChannelType.Virtual } },
																								{ "channel2", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel3", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel4", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel5", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel6", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel7", new RTUChannelDO() { type = ChannelType.Virtual }},
																								{ "channel8", new RTUChannelDO() { type = ChannelType.Virtual }}
																							}}
				};

		// We need to hardcode the list of protocols for now
		public static Dictionary<string, int> AllProtocols = new Dictionary<string, int>() { { "Virtual Chan", 1 },
                                                                              { "RTU Slave", 2 },
                                                                              { "Enraf Master", 3 },
                                                                              { "Modbus Master", 4 },
                                                                              { "Modbus Slave", 5 },
                                                                              { "Digital Input", 6 },
                                                                              { "Digital Output", 7 },
                                                                              { "Ethernet", 8 },
                                                                              { "Mark/Space", 9 },
                                                                              { "Tankway", 10 },
                                                                              { "TLS Master", 11 }};



      public Models.AvailableConfiguration GetAvailableConfiguration(bool readPoints, string filename)
		{
			var availableConfiguration = new Models.AvailableConfiguration();

			XmlDocument rtuxml = LoadandReadXMLFile(filename);
			if (rtuxml != null)
			{
				populateOpcNodeIDLookupDictionary(rtuxml);
				availableConfiguration.modules = this.GetAvailableModules(rtuxml);
				availableConfiguration.protocols = this.GetAvailableProtocols(rtuxml);
				if (readPoints)
				{
					availableConfiguration.points = this.GetAvailablePoints(rtuxml);
					availableConfiguration.PointAlarmNumberLookupDictionary = populatePointAlarmNumberDictionary(rtuxml);
					availableConfiguration.PointRefMapNumberLookupDictionary = populatePointRegMapNumberDictionary(rtuxml);
				}
         }
			return availableConfiguration;
		}


		public List<AvailablePoints> GetAvailablePoints(XmlDocument rtuxml)
		{
			string pointTankName = string.Empty;
			int maximumTanksAllowed = 0;

			string pointAlarmName = string.Empty;
			int maximumAlarmsAllowed = 0;

			string pointMFPRegName = string.Empty;
			int maximumMFPRegAllowed = 0;

			string pointMIRegName = string.Empty;
			int maximumMIRegAllowed = 0;

			string pointGWBlkName = string.Empty;
			int maximumGWBlkAllowed = 0;

			string pointRegMapName = string.Empty;
			int maximumRegMapAllowed = 0;

         string pointX509Name = string.Empty;
         int maximumX509Allowed = 0;

         var pointConfigurationTankParms = LoadandPopulatePointConfiguration(rtuxml, ref pointTankName, ref maximumTanksAllowed, "Tank");

			var pointConfigurationAlarmParms = LoadandPopulatePointConfiguration(rtuxml, ref pointAlarmName, ref maximumAlarmsAllowed, "ALARM");

			var pointConfigurationMFPRegParms = LoadandPopulatePointConfiguration(rtuxml, ref pointMFPRegName, ref maximumMFPRegAllowed, "MFPREG");

			var pointConfigurationMIRegParms = LoadandPopulatePointConfiguration(rtuxml, ref pointMIRegName, ref maximumMFPRegAllowed, "MIREG");

			var pointConfigurationGWBlkParms = LoadandPopulatePointConfiguration(rtuxml, ref pointGWBlkName, ref maximumGWBlkAllowed, "GWBLK");

			var pointConfigurationRegMapParms = LoadandPopulatePointConfiguration(rtuxml, ref pointRegMapName, ref maximumRegMapAllowed, "REGMAP");

         var pointConfigurationX509Parms = LoadandPopulatePointConfiguration(rtuxml, ref pointX509Name, ref maximumX509Allowed, "X509");


            return new List<AvailablePoints> {
					new AvailablePoints(){ Name = pointTankName, maximumAllowed = maximumTanksAllowed, pointConfiguration = pointConfigurationTankParms },
					new AvailablePoints(){ Name = pointAlarmName, maximumAllowed = maximumAlarmsAllowed, pointConfiguration = pointConfigurationAlarmParms },
					new AvailablePoints(){ Name = pointMFPRegName, maximumAllowed = maximumMFPRegAllowed, pointConfiguration = pointConfigurationMFPRegParms },
					new AvailablePoints(){ Name = pointMIRegName, maximumAllowed = maximumMIRegAllowed, pointConfiguration = pointConfigurationMIRegParms },
					new AvailablePoints(){ Name = pointGWBlkName, maximumAllowed = maximumGWBlkAllowed, pointConfiguration = pointConfigurationGWBlkParms },
					new AvailablePoints(){ Name = pointRegMapName, maximumAllowed = maximumRegMapAllowed, pointConfiguration = pointConfigurationRegMapParms },
               new AvailablePoints(){ Name = pointX509Name, maximumAllowed = maximumX509Allowed, pointConfiguration = pointConfigurationX509Parms },
            };

		}

		public List<AvailableModules> GetAvailableModules(XmlDocument rtuxml)
		{

			//sample initial configuration for Empty module
			var emptyModuleConfigurationParms = new Dictionary<UInt32, Parameter>();
			// get the start node ids from the xml file. bds
			var opcNodeId = this.OpcNodeIDLookupDictionary["INTFM_LABEL"];
			emptyModuleConfigurationParms.Add(opcNodeId, new Parameter(ConfigurationClass.CONFIG, "Label", "Point Description", "string", "Undefined", null, null, "pntname", 0, null, "pntname", 0, null, opcStartNodeId: opcNodeId));

			opcNodeId = this.OpcNodeIDLookupDictionary["INTFM_MODCONFIGURED"];
			emptyModuleConfigurationParms.Add(opcNodeId, new Parameter(ConfigurationClass.CONFIG, "ModConfigured", "Configured Module", "unsigned int", "dropdown", null, null, "0", 0, null, "0", 0, null, availableCommands: "Undefined,Bi-Phase Mark,Serial Module,Digital IO,Mark/Space,Tankway", opcStartNodeId: opcNodeId));

			opcNodeId = this.OpcNodeIDLookupDictionary["INTFM_MODINSTALLED"];
			emptyModuleConfigurationParms.Add(opcNodeId, new Parameter(ConfigurationClass.DYNAMIC, "ModInstalled", "Installed Module", "unsigned int", "dropdown", null, null, "0", 0, null, "0", 0, availableCommands: "Bi-Phase Mark,Serial Module,Digital IO,Mark/Space,Unknown Module,Tankway", opcStartNodeId: opcNodeId));

			if (this.OpcNodeIDLookupDictionary.ContainsKey("INTFM_WATCHDOG"))
			{
				opcNodeId = this.OpcNodeIDLookupDictionary["INTFM_WATCHDOG"];
				emptyModuleConfigurationParms.Add(opcNodeId, new Parameter(ConfigurationClass.CONFIG, "Watchdog", "Watchdog (DIO Channel 8)", "unsigned int", "dropdown", null, null, "0", 0, null, "0", 0, availableCommands: "Disable,Alarm Watchdog,CPU Watchdog", opcStartNodeId: opcNodeId));
			}

         if (this.OpcNodeIDLookupDictionary.ContainsKey("INTFM_WATCHDOGTIMER"))
         {
               opcNodeId = this.OpcNodeIDLookupDictionary["INTFM_WATCHDOGTIMER"];
               emptyModuleConfigurationParms.Add(opcNodeId, new Parameter(ConfigurationClass.DYNAMIC, "WatchdogTimer", "Watchdog Timer (msec)", "unsigned int", null, null, null, "0", 0, null, "0", 0, null, opcStartNodeId: opcNodeId));
         }

			var cpuModuleConfigurationParms = LoadandPopulateModuleConfiguration(rtuxml, "cpumoduleconfiguration");

			var serialModuleConfigurationParms = LoadandPopulateModuleConfiguration(rtuxml, "serialmoduleconfiguration");

			var digitalModuleConfigurationParms = LoadandPopulateModuleConfiguration(rtuxml, "digitaliomoduleconfiguration");

			var biPhaseModuleConfigurationParms = LoadandPopulateModuleConfiguration(rtuxml, "biphasemarkmoduleconfiguration");

			var MarkSpaceModuleConfigurationParms = LoadandPopulateModuleConfiguration(rtuxml, "markspacemoduleconfiguration");

			var TankwayModuleConfigurationParms = LoadandPopulateModuleConfiguration(rtuxml, "tankwaymoduleconfiguration");

			var CpuAvailableModule = LoadandPopulateChannelConfigurationandPopulateModuleChannelConfiguration(rtuxml, "CPU Module", cpuModuleConfigurationParms);

			var SerialAvailableModule = LoadandPopulateChannelConfigurationandPopulateModuleChannelConfiguration(rtuxml, "Serial Module", serialModuleConfigurationParms);

			var DigitalioAvailableModule = LoadandPopulateChannelConfigurationandPopulateModuleChannelConfiguration(rtuxml, "Digital IO", digitalModuleConfigurationParms);

			var ByPhaseMarkAvailableModule = LoadandPopulateChannelConfigurationandPopulateModuleChannelConfiguration(rtuxml, "Bi-Phase Mark", biPhaseModuleConfigurationParms);

			var MarkSpaceAvailableModule = LoadandPopulateChannelConfigurationandPopulateModuleChannelConfiguration(rtuxml, "Mark/Space", MarkSpaceModuleConfigurationParms);

			var TankwayAvailableModule = LoadandPopulateChannelConfigurationandPopulateModuleChannelConfiguration(rtuxml, "Tankway", MarkSpaceModuleConfigurationParms);

			return new List<AvailableModules> {
				CpuAvailableModule,
				new AvailableModules(){ Id = 0, Name = "Empty", Img = "emptymodule.png",
													moduleConfiguration = emptyModuleConfigurationParms,
													Channel1 = getChannelConfiguration ( ChannelType.Virtual, new List<string>{ "Virtual Chan" }),
													Channel2 = getChannelConfiguration ( ChannelType.Virtual, new List<string>{ "Virtual Chan" }),
													Channel3 = getChannelConfiguration ( ChannelType.Virtual, new List<string>{ "Virtual Chan" }),
													Channel4 = getChannelConfiguration ( ChannelType.Virtual, new List<string>{ "Virtual Chan" }),
													Channel5 = getChannelConfiguration ( ChannelType.Virtual, new List<string>{ "Virtual Chan" }),
													Channel6 = getChannelConfiguration ( ChannelType.Virtual, new List<string>{ "Virtual Chan" }),
													Channel7 = getChannelConfiguration ( ChannelType.Virtual, new List<string>{ "Virtual Chan" }),
													Channel8 = getChannelConfiguration ( ChannelType.Virtual, new List<string>{ "Virtual Chan" })},
				SerialAvailableModule,
				DigitalioAvailableModule,
				ByPhaseMarkAvailableModule,
				MarkSpaceAvailableModule,
				TankwayAvailableModule,
			};

		}

		public List<AvailableProtocols> GetAvailableProtocols(XmlDocument rtuxml)
		{
			// we need to read each posible port configuration and setup the protocol options based on the port type.
			// first is the virtual channel
			// public AvailableChannelConfigurationParms(ConfigurationClass configClass, string parameter, string description, string dataType, string displayFormat = "", float? minimumValue = null, float? maximumValue = null, string defaultValue = "", string availableCommands = "")
			// load the default path and file name from the app config file bds

			var virtualChannelConfigurationParms = LoadandPopulateChannelConfiguration(rtuxml, "VirtualChanchanneltype");

			var enrafMasterChannelConfigurationParms = LoadandPopulateChannelConfiguration(rtuxml, "EnrafMasterchanneltype");

			var ethernetInputMasterChannelConfigurationParms = LoadandPopulateChannelConfiguration(rtuxml, "Ethernetchanneltype");

			var rtuSlaveInputMasterChannelConfigurationParms = LoadandPopulateChannelConfiguration(rtuxml, "RTUSlavechanneltype");

			var modbusMasterInputMasterChannelConfigurationParms = LoadandPopulateChannelConfiguration(rtuxml, "ModbusMasterchanneltype");

			var modbusSlaveInputMasterChannelConfigurationParms = LoadandPopulateChannelConfiguration(rtuxml, "ModbusSlavechanneltype");

			var digitalInputChannelConfigurationParms = LoadandPopulateChannelConfiguration(rtuxml, "DigitalInputchanneltype");

			var digitalOutputChannelConfigurationParms = LoadandPopulateChannelConfiguration(rtuxml, "DigitalOutputchanneltype");

			var markSpaceChannelConfigurationParms = LoadandPopulateChannelConfiguration(rtuxml, "MarkSpacechanneltype");

			var tankwayChannelConfigurationParms = LoadandPopulateChannelConfiguration(rtuxml, "Tankwaychanneltype");



			var virtualChannelConfigurationDeviceTypes = LoadandPopulateChannelDeviceTypes(rtuxml, "VirtualChanchanneltype");

			var enrafMasterChannelConfigurationDeviceTypes = LoadandPopulateChannelDeviceTypes(rtuxml, "EnrafMasterchanneltype");

			var ethernetInputMasterChannelConfigurationDeviceTypes = LoadandPopulateChannelDeviceTypes(rtuxml, "Ethernetchanneltype");

			var rtuSlaveInputMasterChannelConfigurationDeviceTypes = LoadandPopulateChannelDeviceTypes(rtuxml, "RTUSlavechanneltype");

			var modbusMasterInputMasterChannelConfigurationDeviceTypes = LoadandPopulateChannelDeviceTypes(rtuxml, "ModbusMasterchanneltype");

			var modbusSlaveInputMasterChannelConfigurationDeviceTypes = LoadandPopulateChannelDeviceTypes(rtuxml, "ModbusSlavechanneltype");

			var digitalInputChannelConfigurationDeviceTypes = LoadandPopulateChannelDeviceTypes(rtuxml, "DigitalInputchanneltype");

			var digitalOutputChannelConfigurationDeviceTypes = LoadandPopulateChannelDeviceTypes(rtuxml, "DigitalOutputchanneltype");

			var markSpaceChannelConfigurationDeviceTypes = LoadandPopulateChannelDeviceTypes(rtuxml, "MarkSpacechanneltype");

			var tankwayChannelConfigurationDeviceTypes = LoadandPopulateChannelDeviceTypes(rtuxml, "Tankwaychanneltype");

			return new List<AvailableProtocols> {
					new AvailableProtocols(){ Name = "Virtual Chan", protocolConfiguration = virtualChannelConfigurationParms, AvailableDeviceTypes = virtualChannelConfigurationDeviceTypes},
					new AvailableProtocols() { Name = "RTU Slave", protocolConfiguration = rtuSlaveInputMasterChannelConfigurationParms, AvailableDeviceTypes = rtuSlaveInputMasterChannelConfigurationDeviceTypes },
					new AvailableProtocols() { Name = "Enraf Master", protocolConfiguration = enrafMasterChannelConfigurationParms, AvailableDeviceTypes = enrafMasterChannelConfigurationDeviceTypes },
					new AvailableProtocols() { Name = "Modbus Master", protocolConfiguration = modbusMasterInputMasterChannelConfigurationParms, AvailableDeviceTypes = modbusMasterInputMasterChannelConfigurationDeviceTypes },
					new AvailableProtocols() { Name = "Modbus Slave", protocolConfiguration = modbusSlaveInputMasterChannelConfigurationParms, AvailableDeviceTypes = modbusSlaveInputMasterChannelConfigurationDeviceTypes },
					new AvailableProtocols() { Name = "Digital Input", protocolConfiguration = digitalInputChannelConfigurationParms, AvailableDeviceTypes = digitalInputChannelConfigurationDeviceTypes },
					new AvailableProtocols() { Name = "Digital Output", protocolConfiguration = digitalOutputChannelConfigurationParms, AvailableDeviceTypes = digitalOutputChannelConfigurationDeviceTypes },
					new AvailableProtocols() { Name = "Ethernet", protocolConfiguration = ethernetInputMasterChannelConfigurationParms, AvailableDeviceTypes = ethernetInputMasterChannelConfigurationDeviceTypes },
					new AvailableProtocols() { Name = "Mark/Space", protocolConfiguration = markSpaceChannelConfigurationParms, AvailableDeviceTypes = markSpaceChannelConfigurationDeviceTypes },
					new AvailableProtocols() { Name = "Tankway", protocolConfiguration = tankwayChannelConfigurationParms, AvailableDeviceTypes = tankwayChannelConfigurationDeviceTypes }
				};

		}


		private ConfigurableChannelDO getChannelConfiguration(ChannelType type, List<string> channelProtocols, int top = 0, int left = 0, int width = 0, int height = 0)
		{

			var channelConfigurationParms = new ConfigurableChannelDO();
			channelConfigurationParms.channelProtocols = channelProtocols;
			channelConfigurationParms.type = type;
			channelConfigurationParms.top = top;
			channelConfigurationParms.left = left;
			channelConfigurationParms.width = width;
			channelConfigurationParms.height = height;

			return channelConfigurationParms;
		}

		private ConfigurableChannelDO getChannelConfigurationFromDefaults(RTUChannelDO defaultChannel)
		{

			var channelConfigurationParms = new ConfigurableChannelDO();
			channelConfigurationParms.type = defaultChannel.type;
			channelConfigurationParms.top = defaultChannel.top;
			channelConfigurationParms.left = defaultChannel.left;
			channelConfigurationParms.width = defaultChannel.width;
			channelConfigurationParms.height = defaultChannel.height;

			return channelConfigurationParms;
		}


		private XmlDocument LoadandReadXMLFile(string fileName)
		{
			string fileToLoad = string.Empty;
			XmlDocument rtuxml = new XmlDocument();
			try
			{
				string rtuXmlPath = ConfigurationManager.AppSettings["rtuxmlPath"].ToString();
				string rtuXmlDefaultFile = ConfigurationManager.AppSettings["DefaultRTUXmlFile"].ToString();
				if (string.IsNullOrEmpty(rtuXmlPath) == true ||
					string.IsNullOrEmpty(rtuXmlDefaultFile) == true)
				{
					return null;
				}
				if (!string.IsNullOrEmpty(fileName) && fileName.Length > 0)
					fileToLoad = fileName;
				else
					fileToLoad = rtuXmlDefaultFile;

				rtuxml.Load(rtuXmlPath + "\\" + fileToLoad);
			}
			catch
			{
				return null;
			}
			return rtuxml;
		}

		private Dictionary<UInt32, Parameter> LoadandPopulateChannelConfiguration(XmlDocument rtuxml, string channelType)
		{
			ConfigurationClass configClass = ConfigurationClass.COMMAND;
			string parameter = string.Empty;
			string description = string.Empty;
			string dataType = string.Empty;
			string dataTypeLength = string.Empty;
			string displayFormat = string.Empty;
			float? minimumValue = null;
			float? maximumValue = null;
			string defaultValue = string.Empty;
			string availableCommands = string.Empty;
			UInt32 opcstartnodeID = 0xffffffff;
			string DesignatedTab = string.Empty;
			string DesignatedSub = string.Empty;
			UInt32 status = 0x80000000;
			UInt32 availableCommandsOutputMatches = 0;
			string variableAlarmNumber = "0";



			var ChannelConfigurationParms = new Dictionary<UInt32, Parameter>();

			XmlNode ChannelListNode = rtuxml.SelectSingleNode("/Configuration/channels/channel/" + channelType);

			XmlNodeList configurationNodeList =
				ChannelListNode.SelectNodes("Configuration");
			foreach (XmlNode node in configurationNodeList)
			{
				if (node.ChildNodes.Count > 0)
				{
					configClass = ConfigurationClass.COMMAND;
					parameter = string.Empty;
					description = string.Empty;
					dataType = string.Empty;
					dataTypeLength = string.Empty;
					displayFormat = string.Empty;
					minimumValue = null;
					maximumValue = null;
					defaultValue = string.Empty;
					availableCommands = string.Empty;
					DesignatedTab = string.Empty;
					DesignatedSub = string.Empty;
					status = 0x8000000;
					availableCommandsOutputMatches = 0;
					variableAlarmNumber = "0";


					XmlNodeList configurationsDataNode = node.ChildNodes;

					// In this loop you have get all the child control 
					foreach (XmlNode configurationDataNode in configurationsDataNode)
					{
						if (configurationDataNode.Name == "ParameterType")
						{
							if (configurationDataNode.InnerXml == "DYNAMIC")
								configClass = ConfigurationClass.DYNAMIC;
							else if (configurationDataNode.InnerXml == "CONFIG")
								configClass = ConfigurationClass.CONFIG;
							else if (configurationDataNode.InnerXml == "COMMAND")
								configClass = ConfigurationClass.COMMAND;
							else if (configurationDataNode.InnerXml == "SCRATCH")
							{
								configClass = ConfigurationClass.SCRATCH;
								description = string.Empty;
								break;
							}
							else if (configurationDataNode.InnerXml == "SYSTEM")
								configClass = ConfigurationClass.SYSTEM;
						}
						else if (configurationDataNode.Name == "Variable")
						{
							parameter = configurationDataNode.InnerXml;

							try
							{
								if (parameter.IndexOf(" **") > 0)
								{
									string stTrimedvalue = parameter.Substring(0, parameter.IndexOf(" **"));
									opcstartnodeID = OpcNodeIDLookupDictionary["PORT_" + stTrimedvalue.ToUpper()];
								}
								else
								{
									opcstartnodeID = OpcNodeIDLookupDictionary["PORT_" + parameter.ToUpper()];
								}
							}
							catch
							{
								opcstartnodeID = 0xffffffff;
							}

						}
						else if (configurationDataNode.Name == "VariableAlarmNumber")
						{
							variableAlarmNumber = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "Datatype")
						{
							dataType = configurationDataNode.InnerXml;
							if (dataType == "unsigned int"
							|| dataType == "int"
							|| dataType == "unsigned long"
							|| dataType == "double")
							{
								defaultValue = "0";
								status = 0;
							}
						}
						else if (configurationDataNode.Name == "AvailableCommands")
						{
							availableCommands = configurationDataNode.InnerXml;
							displayFormat = "dropdown";
						}
						else if (configurationDataNode.Name == "AvailableCommandsOutputMatches")
						{
							availableCommandsOutputMatches = System.Convert.ToUInt32(configurationDataNode.InnerXml);
						}
						else if (configurationDataNode.Name == "DesignatedTab")
						{
							DesignatedTab = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "DesignatedSub")
						{
							DesignatedSub = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "MinimumValue")
						{
							minimumValue = System.Convert.ToSingle(configurationDataNode.InnerXml);
						}
						else if (configurationDataNode.Name == "MaximumValue")
						{
							maximumValue = System.Convert.ToSingle(configurationDataNode.InnerXml);
						}
						else if (configurationDataNode.Name == "VariableName")
						{
							description = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "SpecialFormatting")
						{
							displayFormat = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "DatatypeLength")
						{
							dataTypeLength = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "DefaultValue")
						{
							if (displayFormat == "dropdown" && availableCommandsOutputMatches == 0)
							{
								var foundIdx = Array.IndexOf(availableCommands.Split(','), configurationDataNode.InnerXml);
								defaultValue = (foundIdx + 1).ToString();  // we have to increment 1 since the index is zero based and a zero in the drop down is an unknown value
							}
							else
							{
								defaultValue = configurationDataNode.InnerXml;
							}
							status = 0;
						}
						else if (configurationDataNode.Name == "Identifier")
						{
						}
						else
						{
							int y = 0;
							++y;
						}
					}
					if (description.Length > 0)
					{
						ChannelConfigurationParms.Add(opcstartnodeID, new Parameter(configClass,
							parameter,
							description,
							dataType,
							displayFormat,
							minimumValue,
							maximumValue,
							defaultValue,
							status,
							null,
							defaultValue,
							status,
							null,
							availableCommands,
							opcstartnodeID,
							opcstartnodeID,
							DesignatedTab,
							DesignatedSub,
							1,
							availableCommandsOutputMatches,
							variableAlarmNumber,
							datatypeLength: dataTypeLength
							));

					}
				}
			}

			var output = ChannelConfigurationParms.OrderBy(x => x.Value.opcstartNodeID).ToDictionary(pair => pair.Key, pair => pair.Value);


			return output;
		}

		private List<DeviceType> LoadandPopulateChannelDeviceTypes(XmlDocument rtuxml, string channelType)
		{

			string Id = string.Empty;
			string Name = string.Empty;

			var ChannelDeviceTypes = new List<DeviceType>();

			XmlNode DeviceTypeNode = rtuxml.SelectSingleNode("/Configuration/channels/channel/" + channelType + "/AvailableDeviceTypes");
			if (DeviceTypeNode == null)
				return ChannelDeviceTypes;

			List<string> deviceTypeList = DeviceTypeNode.InnerXml.Split(',').ToList<string>();

			foreach (string deviceTypeId in deviceTypeList)
			{
				XmlNode DeviceDetailNode = rtuxml.SelectSingleNode("/Configuration/DeviceTypes/" + deviceTypeId);
				if (DeviceDetailNode == null)
					break;

				List<string> AvailableCommandsList = DeviceDetailNode.SelectSingleNode("AvailableDeviceCommands").InnerXml.Split(',').ToList<string>();

				DeviceType device = new DeviceType
				{
					//Id = deviceTypeId,
					//Name = DeviceDetailNode.SelectSingleNode("Name").InnerXml,
					Name = deviceTypeId,
					Id = DeviceDetailNode.SelectSingleNode("Name").InnerXml,
					DeviceTypeValue = DeviceDetailNode.SelectSingleNode("DeviceTypeValue").InnerXml,
					AvailableCommands = AvailableCommandsList
				};
				ChannelDeviceTypes.Add(device);
			}
			return ChannelDeviceTypes;
		}

		private Dictionary<UInt32, Parameter> LoadandPopulateModuleConfiguration(XmlDocument rtuxml, string moduleType)
		{
			ConfigurationClass configClass = ConfigurationClass.COMMAND;
			string parameter = string.Empty;
			string description = string.Empty;
			string dataType = string.Empty;
			string dataTypeLength = string.Empty;
			string displayFormat = string.Empty;
			float? minimumValue = null;
			float? maximumValue = null;
			string defaultValue = string.Empty;
			string availableCommands = string.Empty;
			UInt32 opcstartnodeID = 0xffffffff;
			string DesignatedTab = string.Empty;
			string DesignatedSub = string.Empty;
			UInt32 identifier = 0xFFFFFFFF;
			UInt32 status = 0x80000000;
			UInt32 parameterIsVisible = 1;
			UInt32 availableCommandsOutputMatches = 0;
			string variableAlarmNumber = "0";

			var ModuleConfigurationParms = new Dictionary<UInt32, Parameter>();

			XmlNode ModuleListNode = rtuxml.SelectSingleNode("/Configuration/" + moduleType);

			XmlNodeList configurationNodeList =
				ModuleListNode.SelectNodes("moduleconfiguration");
			foreach (XmlNode node in configurationNodeList)
			{
				if (node.ChildNodes.Count > 0)
				{
					configClass = ConfigurationClass.COMMAND;
					parameter = string.Empty;
					description = string.Empty;
					dataType = string.Empty;
					dataTypeLength = string.Empty;
					displayFormat = string.Empty;
					minimumValue = null;
					maximumValue = null;
					defaultValue = string.Empty;
					availableCommands = string.Empty;
					opcstartnodeID = 0xffffffff;
					DesignatedTab = string.Empty;
					DesignatedSub = string.Empty;
					parameterIsVisible = 1;
					availableCommandsOutputMatches = 0;
					variableAlarmNumber = "0";

					XmlNodeList configurationsDataNode = node.ChildNodes;

					// In this loop you have get all the child control 
					foreach (XmlNode configurationDataNode in configurationsDataNode)
					{
						if (configurationDataNode.Name == "ParameterType")
						{
							if (configurationDataNode.InnerXml == "DYNAMIC")
								configClass = ConfigurationClass.DYNAMIC;
							else if (configurationDataNode.InnerXml == "CONFIG")
								configClass = ConfigurationClass.CONFIG;
							else if (configurationDataNode.InnerXml == "COMMAND")
								configClass = ConfigurationClass.COMMAND;
							else if (configurationDataNode.InnerXml == "SCRATCH")
							{
								configClass = ConfigurationClass.SCRATCH;
								description = string.Empty;
								break;
							}
							else if (configurationDataNode.InnerXml == "SYSTEM")
								configClass = ConfigurationClass.SYSTEM;
						}
						else if (configurationDataNode.Name == "Name")
						{
							description = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "ParameterVisible")
						{
							if (configurationDataNode.InnerXml.ToUpper() == "0")
							{
								parameterIsVisible = 0;
							}
						}
						else if (configurationDataNode.Name == "VariableAlarmNumber")
						{
							variableAlarmNumber = configurationDataNode.InnerXml;
							//if (configurationDataNode.InnerXml.ToUpper() == "1")
							//{
							//availableCommandsOutputMatches = 1;
							//}
						}
						else if (configurationDataNode.Name == "Datatype")
						{
							dataType = configurationDataNode.InnerXml;
							if (dataType == "unsigned int"
							|| dataType == "int"
							|| dataType == "unsigned long"
							|| dataType == "double")
							{
								defaultValue = "0";
								status = 0;
							}
						}
						else if (configurationDataNode.Name == "AvailableCommands")
						{
							availableCommands = configurationDataNode.InnerXml;
							displayFormat = "dropdown";
						}
						else if (configurationDataNode.Name == "AvailableCommandsOutputMatches")
						{
							availableCommandsOutputMatches = System.Convert.ToUInt32(configurationDataNode.InnerXml);
						}
						else if (configurationDataNode.Name == "DesignatedTab")
						{
							DesignatedTab = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "DesignatedSub")
						{
							DesignatedSub = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "MinimumValue")
						{
							minimumValue = System.Convert.ToSingle(configurationDataNode.InnerXml);
						}
						else if (configurationDataNode.Name == "MaximumValue")
						{
							maximumValue = System.Convert.ToSingle(configurationDataNode.InnerXml);
						}
						else if (configurationDataNode.Name == "VariableName")
						{
							string prefaceString = "INTFM_";
							parameter = configurationDataNode.InnerXml;
							if (moduleType == "cpumoduleconfiguration")
							{
								prefaceString = "CPUM_";
							}
							try
							{
								if (parameter.ToUpper().IndexOf("BAUDRATE") >= 0)
								{
									int yy = 0;
									++yy;
								}
								else
								{
									opcstartnodeID = OpcNodeIDLookupDictionary[prefaceString + parameter.ToUpper()];
								}
							}
							catch
							{
								opcstartnodeID = 0xffffffff;
							}
						}
						else if (configurationDataNode.Name == "SpecialFormatting")
						{
							displayFormat = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "DatatypeLength")
						{
							dataTypeLength = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "DefaultValue")
						{
							if (displayFormat == "dropdown")
							{
								// see if we can find the name in the list
								var foundIdx = Array.IndexOf(availableCommands.Split(','), configurationDataNode.InnerXml);
								defaultValue = (foundIdx + 1).ToString();  // we have to increment 1 since the index is zero based and a zero in the drop down is an unknown value
							}
							else
							{
								defaultValue = configurationDataNode.InnerXml;
							}
							status = 0;
						}
						else
						{
						}
					}
					if (description.Length > 0)
					{
						ModuleConfigurationParms.Add(opcstartnodeID, new Parameter(configClass,
									parameter,
									description,
									dataType,
									displayFormat,
									minimumValue,
									maximumValue,
									defaultValue,
									status,
									null,
									defaultValue,
									status,
									null,
									availableCommands,
									opcstartnodeID,
									identifier,
									DesignatedTab,
									DesignatedSub,
									parameterIsVisible,
									availableCommandsOutputMatches,
									variableAlarmNumber,
									datatypeLength: dataTypeLength));
					}
				}
			}

			var output = ModuleConfigurationParms.OrderBy(x => x.Value.opcstartNodeID).ToDictionary(pair => pair.Key, pair => pair.Value);

			return output;
		}

		private AvailableModules LoadandPopulateChannelConfigurationandPopulateModuleChannelConfiguration(XmlDocument rtuxml, string moduleType, Dictionary<UInt32, Parameter> moduleConfParams)
		{
			string Name = string.Empty;
			string Image = string.Empty;
			string moduleNameType = string.Empty;
			string revision = string.Empty;
			List<string> stChannel1 = null;
			List<string> stChannel2 = null;
			List<string> stChannel3 = null;
			List<string> stChannel4 = null;
			List<string> stChannel5 = null;
			List<string> stChannel6 = null;
			List<string> stChannel7 = null;
			List<string> stChannel8 = null;

			AvailableModules moduleAvailableModule = new AvailableModules();
			moduleAvailableModule.moduleConfiguration = moduleConfParams;

			XmlNode ModuleListNode = rtuxml.SelectSingleNode("/Configuration/Modules");

			XmlNodeList configurationNodeList =
				ModuleListNode.SelectNodes("Module");
			foreach (XmlNode node in configurationNodeList)
			{
				if (node.ChildNodes.Count > 0)
				{

					XmlNodeList configurationsDataNode = node.ChildNodes;

					// In this loop you have get all the child control 
					foreach (XmlNode configurationDataNode in configurationsDataNode)
					{

						moduleAvailableModule.moduleConfiguration = moduleConfParams;

						if (configurationDataNode.Name == "Name")
						{
							if (moduleType != configurationDataNode.InnerXml)
								break;

							Name = configurationDataNode.InnerXml;

							if (moduleType == "CPU Module")
							{
								moduleAvailableModule.Name = "CPU";
								moduleAvailableModule.Img = "cpu.png";
								moduleAvailableModule.Id = -1;

								moduleAvailableModule.Channel1 = getChannelConfigurationFromDefaults(ModuleDefinitions["CPU Module"]["channel1"]);
								moduleAvailableModule.Channel2 = getChannelConfigurationFromDefaults(ModuleDefinitions["CPU Module"]["channel2"]);
								moduleAvailableModule.Channel3 = getChannelConfigurationFromDefaults(ModuleDefinitions["CPU Module"]["channel3"]);
								moduleAvailableModule.Channel4 = getChannelConfigurationFromDefaults(ModuleDefinitions["CPU Module"]["channel4"]);
								moduleAvailableModule.Channel5 = getChannelConfigurationFromDefaults(ModuleDefinitions["CPU Module"]["channel5"]);
								moduleAvailableModule.Channel6 = getChannelConfigurationFromDefaults(ModuleDefinitions["CPU Module"]["channel6"]);
								moduleAvailableModule.Channel7 = getChannelConfigurationFromDefaults(ModuleDefinitions["CPU Module"]["channel7"]);
								moduleAvailableModule.Channel8 = getChannelConfigurationFromDefaults(ModuleDefinitions["CPU Module"]["channel8"]);
							}
							else if (moduleType == "Serial Module")
							{
								moduleAvailableModule.Name = "Serial";
								moduleAvailableModule.Img = "serial.png";
								moduleAvailableModule.Id = 2;
								moduleAvailableModule.Channel1 = getChannelConfigurationFromDefaults(ModuleDefinitions["Serial Module"]["channel1"]);
								moduleAvailableModule.Channel2 = getChannelConfigurationFromDefaults(ModuleDefinitions["Serial Module"]["channel2"]);
								moduleAvailableModule.Channel3 = getChannelConfigurationFromDefaults(ModuleDefinitions["Serial Module"]["channel3"]);
								moduleAvailableModule.Channel4 = getChannelConfigurationFromDefaults(ModuleDefinitions["Serial Module"]["channel4"]);
								moduleAvailableModule.Channel5 = getChannelConfigurationFromDefaults(ModuleDefinitions["Serial Module"]["channel5"]);
								moduleAvailableModule.Channel6 = getChannelConfigurationFromDefaults(ModuleDefinitions["Serial Module"]["channel6"]);
								moduleAvailableModule.Channel7 = getChannelConfigurationFromDefaults(ModuleDefinitions["Serial Module"]["channel7"]);
								moduleAvailableModule.Channel8 = getChannelConfigurationFromDefaults(ModuleDefinitions["Serial Module"]["channel8"]);

							}
							else if (moduleType == "Digital IO")
							{
								moduleAvailableModule.Name = "Digital IO";
								moduleAvailableModule.Img = "digitalio.png";
								moduleAvailableModule.Id = 3;
								moduleAvailableModule.Channel1 = getChannelConfigurationFromDefaults(ModuleDefinitions["Digital IO"]["channel1"]);
								moduleAvailableModule.Channel2 = getChannelConfigurationFromDefaults(ModuleDefinitions["Digital IO"]["channel2"]);
								moduleAvailableModule.Channel3 = getChannelConfigurationFromDefaults(ModuleDefinitions["Digital IO"]["channel3"]);
								moduleAvailableModule.Channel4 = getChannelConfigurationFromDefaults(ModuleDefinitions["Digital IO"]["channel4"]);
								moduleAvailableModule.Channel5 = getChannelConfigurationFromDefaults(ModuleDefinitions["Digital IO"]["channel5"]);
								moduleAvailableModule.Channel6 = getChannelConfigurationFromDefaults(ModuleDefinitions["Digital IO"]["channel6"]);
								moduleAvailableModule.Channel7 = getChannelConfigurationFromDefaults(ModuleDefinitions["Digital IO"]["channel7"]);
								moduleAvailableModule.Channel8 = getChannelConfigurationFromDefaults(ModuleDefinitions["Digital IO"]["channel8"]);

							}
							else if (moduleType == "Bi-Phase Mark")
							{
								moduleAvailableModule.Name = "Bi-Phase";
								moduleAvailableModule.Img = "biphase.png";
								moduleAvailableModule.Id = 1;
								moduleAvailableModule.Channel1 = getChannelConfigurationFromDefaults(ModuleDefinitions["Bi-Phase Mark"]["channel1"]);
								moduleAvailableModule.Channel2 = getChannelConfigurationFromDefaults(ModuleDefinitions["Bi-Phase Mark"]["channel2"]);
								moduleAvailableModule.Channel3 = getChannelConfigurationFromDefaults(ModuleDefinitions["Bi-Phase Mark"]["channel3"]);
								moduleAvailableModule.Channel4 = getChannelConfigurationFromDefaults(ModuleDefinitions["Bi-Phase Mark"]["channel4"]);
								moduleAvailableModule.Channel5 = getChannelConfigurationFromDefaults(ModuleDefinitions["Bi-Phase Mark"]["channel5"]);
								moduleAvailableModule.Channel6 = getChannelConfigurationFromDefaults(ModuleDefinitions["Bi-Phase Mark"]["channel6"]);
								moduleAvailableModule.Channel7 = getChannelConfigurationFromDefaults(ModuleDefinitions["Bi-Phase Mark"]["channel7"]);
								moduleAvailableModule.Channel8 = getChannelConfigurationFromDefaults(ModuleDefinitions["Bi-Phase Mark"]["channel8"]);

							}
							else if (moduleType == "Mark/Space")
							{
								moduleAvailableModule.Name = "Mark/Space";
								moduleAvailableModule.Img = "markspace.png";
								moduleAvailableModule.Id = 4;
								moduleAvailableModule.Channel1 = getChannelConfigurationFromDefaults(ModuleDefinitions["Mark/Space"]["channel1"]);
								moduleAvailableModule.Channel2 = getChannelConfigurationFromDefaults(ModuleDefinitions["Mark/Space"]["channel2"]);
								moduleAvailableModule.Channel3 = getChannelConfigurationFromDefaults(ModuleDefinitions["Mark/Space"]["channel3"]);
								moduleAvailableModule.Channel4 = getChannelConfigurationFromDefaults(ModuleDefinitions["Mark/Space"]["channel4"]);
								moduleAvailableModule.Channel5 = getChannelConfigurationFromDefaults(ModuleDefinitions["Mark/Space"]["channel5"]);
								moduleAvailableModule.Channel6 = getChannelConfigurationFromDefaults(ModuleDefinitions["Mark/Space"]["channel6"]);
								moduleAvailableModule.Channel7 = getChannelConfigurationFromDefaults(ModuleDefinitions["Mark/Space"]["channel7"]);
								moduleAvailableModule.Channel8 = getChannelConfigurationFromDefaults(ModuleDefinitions["Mark/Space"]["channel8"]);
							}
							else if (moduleType == "Tankway")
							{
								moduleAvailableModule.Name = "Tankway";
								moduleAvailableModule.Img = "tankway.png";
								moduleAvailableModule.Id = 5;
								moduleAvailableModule.Channel1 = getChannelConfigurationFromDefaults(ModuleDefinitions["Tankway"]["channel1"]);
								moduleAvailableModule.Channel2 = getChannelConfigurationFromDefaults(ModuleDefinitions["Tankway"]["channel2"]);
								moduleAvailableModule.Channel3 = getChannelConfigurationFromDefaults(ModuleDefinitions["Tankway"]["channel3"]);
								moduleAvailableModule.Channel4 = getChannelConfigurationFromDefaults(ModuleDefinitions["Tankway"]["channel4"]);
								moduleAvailableModule.Channel5 = getChannelConfigurationFromDefaults(ModuleDefinitions["Tankway"]["channel5"]);
								moduleAvailableModule.Channel6 = getChannelConfigurationFromDefaults(ModuleDefinitions["Tankway"]["channel6"]);
								moduleAvailableModule.Channel7 = getChannelConfigurationFromDefaults(ModuleDefinitions["Tankway"]["channel7"]);
								moduleAvailableModule.Channel8 = getChannelConfigurationFromDefaults(ModuleDefinitions["Tankway"]["channel8"]);

							}
							else
							{
								moduleAvailableModule.Name = "Empty";
								moduleAvailableModule.Img = "emptymodule.png";
								moduleAvailableModule.Id = 0;
								moduleAvailableModule.Channel1 = getChannelConfigurationFromDefaults(ModuleDefinitions["Empty"]["channel1"]);
								moduleAvailableModule.Channel2 = getChannelConfigurationFromDefaults(ModuleDefinitions["Empty"]["channel2"]);
								moduleAvailableModule.Channel3 = getChannelConfigurationFromDefaults(ModuleDefinitions["Empty"]["channel3"]);
								moduleAvailableModule.Channel4 = getChannelConfigurationFromDefaults(ModuleDefinitions["Empty"]["channel4"]);
								moduleAvailableModule.Channel5 = getChannelConfigurationFromDefaults(ModuleDefinitions["Empty"]["channel5"]);
								moduleAvailableModule.Channel6 = getChannelConfigurationFromDefaults(ModuleDefinitions["Empty"]["channel6"]);
								moduleAvailableModule.Channel7 = getChannelConfigurationFromDefaults(ModuleDefinitions["Empty"]["channel7"]);
								moduleAvailableModule.Channel8 = getChannelConfigurationFromDefaults(ModuleDefinitions["Empty"]["channel8"]);
							}
						}
						else if (configurationDataNode.Name == "Id")
						{
							//maxAllowed = System.Convert.ToInt32(configurationDataNode.InnerXml);
						}
						else if (configurationDataNode.Name == "ModuleType")
						{
							//moduleNameType = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "Revision")
						{
							revision = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "Image")
						{
							//Image = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "Channel1")
						{
							stChannel1 = configurationDataNode.InnerText.Split(',').ToList();
							moduleAvailableModule.Channel1.channelProtocols = stChannel1;

						}
						else if (configurationDataNode.Name == "Channel2")
						{
							stChannel2 = configurationDataNode.InnerText.Split(',').ToList();
							moduleAvailableModule.Channel2.channelProtocols = stChannel2;
						}
						else if (configurationDataNode.Name == "Channel3")
						{
							stChannel3 = configurationDataNode.InnerText.Split(',').ToList();
							moduleAvailableModule.Channel3.channelProtocols = stChannel3;
						}
						else if (configurationDataNode.Name == "Channel4")
						{
							stChannel4 = configurationDataNode.InnerText.Split(',').ToList();
							moduleAvailableModule.Channel4.channelProtocols = stChannel4;
						}
						else if (configurationDataNode.Name == "Channel5")
						{
							stChannel5 = configurationDataNode.InnerText.Split(',').ToList();
							moduleAvailableModule.Channel5.channelProtocols = stChannel5;
						}
						else if (configurationDataNode.Name == "Channel6")
						{
							stChannel6 = configurationDataNode.InnerText.Split(',').ToList();
							moduleAvailableModule.Channel6.channelProtocols = stChannel6;
						}
						else if (configurationDataNode.Name == "Channel7")
						{
							stChannel7 = configurationDataNode.InnerText.Split(',').ToList();
							moduleAvailableModule.Channel7.channelProtocols = stChannel7;
						}
						else if (configurationDataNode.Name == "Channel8")
						{
							stChannel8 = configurationDataNode.InnerText.Split(',').ToList();
							moduleAvailableModule.Channel8.channelProtocols = stChannel8;
						}
						else
						{
							int y = 0;
							++y;
						}
					}

				}
			}

			return moduleAvailableModule;
		}


		private Dictionary<UInt32, Parameter> LoadandPopulatePointConfiguration(XmlDocument rtuxml, ref string pointName, ref int maximumallowed, string pointType)
		{
			ConfigurationClass configClass = ConfigurationClass.COMMAND;
			string parameter = string.Empty;
			string description = string.Empty;
			string dataType = string.Empty;
			string dataTypeLength = string.Empty;
			string displayFormat = string.Empty;
			float? minimumValue = null;
			float? maximumValue = null;
			string defaultValue = string.Empty;
			string availableCommands = string.Empty;
			string ioType = string.Empty;
			string changeofState = string.Empty;
			UInt32 opcstartnodeID = 0xffffffff;
			string tab = string.Empty;
			string section = string.Empty;
			UInt32 identifier = 0xFFFFFFFF;
			UInt32 status = 0x80000000;
			UInt32 availableCommandsOutputMatches = 0;
			string variableAlarmNumber = "0";

			pointName = string.Empty;
			maximumallowed = 0;

			var pointparameterDictionary = new Dictionary<UInt32, Parameter>();

			XmlNode PointListNode = rtuxml.SelectSingleNode("/Configuration/points/point/" + pointType);
         if (PointListNode == null) return pointparameterDictionary;

         if (PointListNode.ChildNodes.Count > 0)
			{
				foreach (XmlNode configurationDataNode in PointListNode)
				{
					if (configurationDataNode.Name == "Name")
					{
						pointName = configurationDataNode.InnerXml;
					}
					if (configurationDataNode.Name == "MaxAllowed")
					{
						maximumallowed = System.Convert.ToInt32(configurationDataNode.InnerXml);
					}
				}
			}
			XmlNodeList configurationNodeList =
			PointListNode.SelectNodes("Configuration");
			foreach (XmlNode node in configurationNodeList)
			{
				if (node.ChildNodes.Count > 0)
				{
					configClass = ConfigurationClass.COMMAND;
					parameter = string.Empty;
					description = string.Empty;
					dataType = string.Empty;
					dataTypeLength = string.Empty;
					displayFormat = string.Empty;
					minimumValue = null;
					maximumValue = null;
					defaultValue = string.Empty;
					availableCommands = string.Empty;
					ioType = string.Empty;
					changeofState = string.Empty;
					opcstartnodeID = 0xffffffff;
					tab = string.Empty;
					section = string.Empty;
					availableCommandsOutputMatches = 0;
					variableAlarmNumber = "0";

					XmlNodeList configurationsDataNode = node.ChildNodes;

					// In this loop you have get all the child control 
					foreach (XmlNode configurationDataNode in configurationsDataNode)
					{
						if (configurationDataNode.Name == "ParameterType")
						{
							if (configurationDataNode.InnerXml == "DYNAMIC")
								configClass = ConfigurationClass.DYNAMIC;
							else if (configurationDataNode.InnerXml == "CONFIG")
								configClass = ConfigurationClass.CONFIG;
							else if (configurationDataNode.InnerXml == "COMMAND")
								configClass = ConfigurationClass.COMMAND;
							else if (configurationDataNode.InnerXml == "SCRATCH")
							{
								configClass = ConfigurationClass.SCRATCH;
								description = string.Empty;
								break;
							}
							else if (configurationDataNode.InnerXml == "SYSTEM")
								configClass = ConfigurationClass.SYSTEM;
						}
						else if (configurationDataNode.Name == "Variable")
						{
							parameter = configurationDataNode.InnerXml;

							// get the configured opc node start id from the dictionary
							try
							{
								if (parameter.ToUpper().IndexOf("BAUDRATE") == -1
								&& parameter.ToUpper().IndexOf("UNUSED") == -1)							
								{
									opcstartnodeID = OpcNodeIDLookupDictionary[pointType.ToUpper() + "_" + parameter.ToUpper()];
								}
							}
							catch
							{
								opcstartnodeID = 0xffffffff;
							}
						}
						else if (configurationDataNode.Name == "VariableAlarmNumber")
						{
							variableAlarmNumber = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "Datatype")
						{
							dataType = configurationDataNode.InnerXml;
							if (dataType == "unsigned int"
							|| dataType == "int"
							|| dataType == "unsigned long"
							|| dataType == "double")
							{
								defaultValue = "0";
								status = 0;
							}
						}
						else if (configurationDataNode.Name == "AvailableCommands")
						{
							availableCommands = configurationDataNode.InnerXml;
							displayFormat = "dropdown";
						}
						else if (configurationDataNode.Name == "AvailableCommandsOutputMatches")
						{
							availableCommandsOutputMatches = System.Convert.ToUInt32(configurationDataNode.InnerXml);
						}
						else if (configurationDataNode.Name == "DesignatedTab")
						{
							tab = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "DesignatedSub")
						{
							section = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "MinimumValue")
						{
							minimumValue = System.Convert.ToSingle(configurationDataNode.InnerXml);
						}
						else if (configurationDataNode.Name == "MaximumValue")
						{
							maximumValue = System.Convert.ToSingle(configurationDataNode.InnerXml);
						}
						else if (configurationDataNode.Name == "VariableName")
						{
							description = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "SpecialFormatting")
						{
							displayFormat = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "DatatypeLength")
						{
							dataTypeLength = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "DefaultValue")
						{
							if (displayFormat == "dropdown" && availableCommandsOutputMatches == 0)
							{
								var foundIdx = Array.IndexOf(availableCommands.Split(','), configurationDataNode.InnerXml);
								defaultValue = (foundIdx + 1).ToString();  // we have to increment 1 since the index is zero based and a zero in the drop down is an unknown value
							}
							else
							{
								defaultValue = configurationDataNode.InnerXml;
							}
							status = 0;
						}
						else if (configurationDataNode.Name == "IOType")
						{
							ioType = configurationDataNode.InnerXml;
						}
						else if (configurationDataNode.Name == "ChangeOfState")
						{
							changeofState = configurationDataNode.InnerXml;
						}
						else
						{
							int y = 0;
							++y;
						}
					}
					if (description.Length > 0)
					{
						if (dataType == "PNTREF")
						{
							// pntref is a special type and requires 3 io points so we create here
							// the trhee types are type, number and param
							// first create the type
							string tempparameter = parameter + "_TYPE";
							string tempdescription = description + "_TYPE";
							string tempparameterName = string.Empty;
							for (int iloop = 0; iloop < 3; iloop++)
							{
								if (iloop == 0) // called PointType in RTU
								{
									tempparameter = parameter + "_TYPE";
									tempdescription = description + " Type";
									tempparameterName = parameter + "PointType";
									dataType = "string";
								}
								else if (iloop == 1) // called TypeIndex in RTU
								{
									tempparameter = parameter + "_NUMBER";
									tempdescription = description + " Number";
									tempparameterName = parameter + "TypeIndex";
									dataType = "unsigned int";
								}
								else  // called Parameter in RTU
								{
									tempparameter = parameter + "_PARAM";
									tempdescription = description + " Param";
									tempparameterName = parameter + "Parameter";
									dataType = "string";
								}
								try
								{
									opcstartnodeID = OpcNodeIDLookupDictionary["TANK_" + tempparameter.ToUpper()];
								}
								catch
								{
									opcstartnodeID = 0xffffffff;
								}

								if (opcstartnodeID == 0xffffffff)
								{
									continue;
								}

								pointparameterDictionary.Add(opcstartnodeID, new Parameter(configClass,
																tempparameterName,
																tempdescription,
																dataType,
																displayFormat,
																minimumValue,
																maximumValue,
																defaultValue,
																status,
																null,
																defaultValue,
																status,
																null,
																availableCommands,
																opcstartnodeID,
																identifier,
																tab,
																section,
																1,
																availableCommandsOutputMatches,
																variableAlarmNumber,
																datatypeLength: dataTypeLength));
							}
						}
						else
						{
							if (opcstartnodeID == 0xffffffff)
							{
								continue;
							}

							pointparameterDictionary.Add(opcstartnodeID, new Parameter(configClass,
								parameter,
								description,
								dataType,
								displayFormat,
								minimumValue,
								maximumValue,
								defaultValue,
								status,
								null,
								defaultValue,
								status,
								null,
								availableCommands,
								opcstartnodeID,
								identifier,
								tab,
								section,
								1,
								availableCommandsOutputMatches,
								variableAlarmNumber,
								datatypeLength: dataTypeLength));
						}
					}
				}
			}

			var output = pointparameterDictionary.OrderBy(x => x.Value.opcstartNodeID).ToDictionary(pair => pair.Key, pair => pair.Value);


			return output;
		}
		private void populateOpcNodeIDLookupDictionary(XmlDocument rtuxml)
		{

			// read the values from the xml file and create a dictionary based on these values.
			try
			{
				string dictName = string.Empty;
				string dictValue = string.Empty;

				XmlNode DictListNode = rtuxml.SelectSingleNode("/Configuration/OpcNodeIdDictionary");

				if (DictListNode == null)  // opc definitions do not exist
					return;

				if (DictListNode.ChildNodes.Count > 0)
				{
					foreach (XmlNode dictionarylistDataNode in DictListNode)
					{
						dictName = string.Empty;
						dictValue = string.Empty;

						dictName = dictionarylistDataNode.Name;
						dictValue = dictionarylistDataNode.InnerXml;
						if (dictName.Length > 0 &&
							dictValue.Length > 0)
						{
							// add it to the dictionary
							try
							{
								OpcNodeIDLookupDictionary.Add(dictName, System.Convert.ToUInt32(dictValue));
							}
							catch
							{
								// we should never hit this because the verification is being done in the xml generator application
								// this is just a good defensive programming practice
								int tt = 0;
								++tt;
							}
						}
					}
				}

			}
			catch
			{
				return;
			}

			return;

		}

		private List<AlarmNumberingClass> populatePointAlarmNumberDictionary(XmlDocument rtuxml)
		{
			List<AlarmNumberingClass> PointAlarmNumberLookupDictionary = new List<AlarmNumberingClass>();
			try
			{
				string dictName = string.Empty;
				string dictValue = string.Empty;
				string AlarmNumber = string.Empty;

				XmlNode DictListNode = rtuxml.SelectSingleNode("/Configuration/PointAlarmNumberDictionary");

				if (DictListNode == null)  // opc definitions do not exist
					return PointAlarmNumberLookupDictionary;
				if (DictListNode.ChildNodes.Count > 0)
				{

					XmlNodeList configurationsDataNode = DictListNode.ChildNodes;

					// In this loop you have get all the child control 
					foreach (XmlNode configurationDataNode in configurationsDataNode)
					{
						dictName = string.Empty;
						dictValue = string.Empty;
						AlarmNumber = string.Empty;

						if (configurationDataNode.Name == "ElementConfiguration" && configurationDataNode.ChildNodes.Count > 0)
						{
							foreach (XmlNode conf in configurationDataNode.ChildNodes)
							{
								if (conf.Name == "PointName")
								{
									dictName = conf.InnerXml;
								}
								if (conf.Name == "VariableName")
								{
									dictValue = conf.InnerXml;
								}
								if (conf.Name == "AlarmNumber")
								{
									AlarmNumber = conf.InnerXml;
								}
							}
						}
						if (dictName.Length > 0 && dictValue.Length > 0 && AlarmNumber.Length > 0)
						{
							AlarmNumberingClass AlarmClass = new AlarmNumberingClass();
							AlarmClass.pointName = dictName;
							AlarmClass.VariableName = dictValue;
							AlarmClass.AlarmNumber = AlarmNumber;
							PointAlarmNumberLookupDictionary.Add(AlarmClass);
						}


						int yy = 0;
						++yy;

						//PointAlarmNumberLookupDictionary
					}
				}
			}
			catch
			{
				return PointAlarmNumberLookupDictionary;
			}

			return PointAlarmNumberLookupDictionary;

		}



        private List<AlarmNumberingClass> populatePointRegMapNumberDictionary(XmlDocument rtuxml)
        {
            List<AlarmNumberingClass> PointRegMapNumberLookupDictionary = new List<AlarmNumberingClass>();
            try
            {
                string dictName = string.Empty;
                string dictValue = string.Empty;
                string AlarmNumber = string.Empty;

                XmlNode DictListNode = rtuxml.SelectSingleNode("/Configuration/PointRegisterMapDictionary");

                if (DictListNode == null)  // opc definitions do not exist
                    return PointRegMapNumberLookupDictionary;
                if (DictListNode.ChildNodes.Count > 0)
                {

                    XmlNodeList configurationsDataNode = DictListNode.ChildNodes;

                    // In this loop you have get all the child control 
                    foreach (XmlNode configurationDataNode in configurationsDataNode)
                    {
                        dictName = string.Empty;
                        dictValue = string.Empty;
                        AlarmNumber = string.Empty;

                        if (configurationDataNode.Name == "ElementConfiguration" && configurationDataNode.ChildNodes.Count > 0)
                        {
                            foreach (XmlNode conf in configurationDataNode.ChildNodes)
                            {
                                if (conf.Name == "PointName")
                                {
                                    dictName = conf.InnerXml;
                                }
                                if (conf.Name == "VariableName")
                                {
                                    dictValue = conf.InnerXml;
                                }
                                if (conf.Name == "AlarmNumber")
                                {
                                    AlarmNumber = conf.InnerXml;
                                }
                            }
                        }
                        if (dictName.Length > 0 && dictValue.Length > 0 && AlarmNumber.Length > 0)
                        {
                            AlarmNumberingClass AlarmClass = new AlarmNumberingClass();
                            AlarmClass.pointName = dictName;
                            AlarmClass.VariableName = dictValue;
                            AlarmClass.AlarmNumber = AlarmNumber;
                            PointRegMapNumberLookupDictionary.Add(AlarmClass);
                        }


                        int yy = 0;
                        ++yy;

                    }
                }
            }
            catch
            {
                return PointRegMapNumberLookupDictionary;
            }

            return PointRegMapNumberLookupDictionary;

        }
    }
}
