using RTUWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using static RTUWebAPI.Models.AvailableChannel;

namespace RTUWebAPI.Services
{
	public class RTUConfigurationService
	{
		public RTUConfigurationDO GetBlankConfiguration(bool returnPoints, string filename = "")
		{
			var configuration = new RTUConfigurationDO();

			// get the available modules
			var availableConfiguration = new AvailableConfigurationService().GetAvailableConfiguration(returnPoints, filename);

			foreach (var alarnNumberObject in availableConfiguration.PointAlarmNumberLookupDictionary)
			{
				configuration.PointAlarmNumberLookupDictionary.Add(alarnNumberObject);
			}

            foreach (var alarnNumberObject in availableConfiguration.PointRefMapNumberLookupDictionary)
            {
                configuration.PointRefMapNumberLookupDictionary.Add(alarnNumberObject);
            }

            var cpuConfiguration = availableConfiguration.modules.Where(x => x.Name == "CPU").FirstOrDefault();
			if (cpuConfiguration != null)
			{
				foreach (KeyValuePair<UInt32, Parameter> configParameter in cpuConfiguration.moduleConfiguration)
				{
					//load the opcstartnodeID onto the parameter.identifier
					configuration.module0.moduleConfiguration.Add(configParameter.Value.opcstartNodeID,
					new Parameter(configParameter.Value.configClass,
					configParameter.Value.parameter,
					configParameter.Value.description,
					configParameter.Value.dataType,
					configParameter.Value.displayFormat,
					configParameter.Value.minimumValue,
					configParameter.Value.maximumValue,
					configParameter.Value.value,
					configParameter.Value.status,
					configParameter.Value.serverTimeStamp,
					configParameter.Value.pendingValue,
					configParameter.Value.pendingStatus,
					configParameter.Value.pendingServerTimeStamp,
					configParameter.Value.availableCommands,
					configParameter.Value.opcstartNodeID,
					configParameter.Value.opcstartNodeID,
					configParameter.Value.tab,
					configParameter.Value.section,
					configParameter.Value.parameterIsVisible,
					configParameter.Value.availableCommandsOutputMatches,
					configParameter.Value.variableAlarmNumber,
					configParameter.Value.datatypeLength));
				}

				copyCPUChannelConfiguration(configuration.module0, cpuConfiguration, availableConfiguration.protocols);
			}

			//available configuration needs to populate opcstartNodeID
			var emptyModule = availableConfiguration.modules.Where(x => x.Name == "Empty").FirstOrDefault();
			if (emptyModule != null)
			{
				foreach (KeyValuePair<UInt32, Parameter> configParameter in emptyModule.moduleConfiguration)
				{
					UInt32 identifierKey = configParameter.Value.opcstartNodeID;
					configuration.module1.moduleConfiguration.Add(configParameter.Value.opcstartNodeID + 0, new Parameter(configParameter.Value.configClass,
																				configParameter.Value.parameter,
																				configParameter.Value.description,
																				configParameter.Value.dataType, 
																				configParameter.Value.displayFormat, 
																				configParameter.Value.minimumValue, 
																				configParameter.Value.maximumValue, 
																				configParameter.Value.value, 
																				configParameter.Value.status, 
																				null, 
																				configParameter.Value.pendingValue, 
																				configParameter.Value.pendingStatus, 
																				null, 
																				availableCommands: configParameter.Value.availableCommands, 
																				identifier: configParameter.Value.opcstartNodeID + 0, 
																				opcStartNodeId: configParameter.Value.opcstartNodeID));

					configuration.module2.moduleConfiguration.Add(configParameter.Value.opcstartNodeID + 1, new Parameter(configParameter.Value.configClass, 
																				configParameter.Value.parameter, 
																				configParameter.Value.description, 
																				configParameter.Value.dataType, 
																				configParameter.Value.displayFormat, 
																				configParameter.Value.minimumValue, 
																				configParameter.Value.maximumValue, 
																				configParameter.Value.value, 
																				configParameter.Value.status,
																				null, 
																				configParameter.Value.pendingValue, 
																				configParameter.Value.pendingStatus,
																				null, 
																				availableCommands: configParameter.Value.availableCommands, 
																				identifier: configParameter.Value.opcstartNodeID + 1, 
																				opcStartNodeId: configParameter.Value.opcstartNodeID));

					configuration.module3.moduleConfiguration.Add(configParameter.Value.opcstartNodeID + 2, new Parameter(configParameter.Value.configClass, 
																				configParameter.Value.parameter, 
																				configParameter.Value.description, 
																				configParameter.Value.dataType, 
																				configParameter.Value.displayFormat, 
																				configParameter.Value.minimumValue, 
																				configParameter.Value.maximumValue, 
																				configParameter.Value.value, 
																				configParameter.Value.status, 
																				null, 
																				configParameter.Value.pendingValue, 
																				configParameter.Value.pendingStatus, 
																				null, 
																				availableCommands: configParameter.Value.availableCommands, 
																				identifier: configParameter.Value.opcstartNodeID + 2, 
																				opcStartNodeId: configParameter.Value.opcstartNodeID));

					configuration.module4.moduleConfiguration.Add(configParameter.Value.opcstartNodeID + 3, new Parameter(configParameter.Value.configClass, configParameter.Value.parameter, configParameter.Value.description, configParameter.Value.dataType, configParameter.Value.displayFormat, configParameter.Value.minimumValue, configParameter.Value.maximumValue, configParameter.Value.value, configParameter.Value.status, null, configParameter.Value.pendingValue, configParameter.Value.pendingStatus, null, availableCommands: configParameter.Value.availableCommands, identifier: configParameter.Value.opcstartNodeID + 3, opcStartNodeId: configParameter.Value.opcstartNodeID));
					configuration.module5.moduleConfiguration.Add(configParameter.Value.opcstartNodeID + 4, new Parameter(configParameter.Value.configClass, configParameter.Value.parameter, configParameter.Value.description, configParameter.Value.dataType, configParameter.Value.displayFormat, configParameter.Value.minimumValue, configParameter.Value.maximumValue, configParameter.Value.value, configParameter.Value.status, null, configParameter.Value.pendingValue, configParameter.Value.pendingStatus, null, availableCommands: configParameter.Value.availableCommands, identifier: configParameter.Value.opcstartNodeID + 4, opcStartNodeId: configParameter.Value.opcstartNodeID));
					configuration.module6.moduleConfiguration.Add(configParameter.Value.opcstartNodeID + 5, new Parameter(configParameter.Value.configClass, configParameter.Value.parameter, configParameter.Value.description, configParameter.Value.dataType, configParameter.Value.displayFormat, configParameter.Value.minimumValue, configParameter.Value.maximumValue, configParameter.Value.value, configParameter.Value.status, null, configParameter.Value.pendingValue, configParameter.Value.pendingStatus, null, availableCommands: configParameter.Value.availableCommands, identifier: configParameter.Value.opcstartNodeID + 5, opcStartNodeId: configParameter.Value.opcstartNodeID));
				}

				copyInterfaceChannelConfiguration(configuration.module1, emptyModule, availableConfiguration.protocols, 1);
				copyInterfaceChannelConfiguration(configuration.module2, emptyModule, availableConfiguration.protocols, 2);
				copyInterfaceChannelConfiguration(configuration.module3, emptyModule, availableConfiguration.protocols, 3);
				copyInterfaceChannelConfiguration(configuration.module4, emptyModule, availableConfiguration.protocols, 4);
				copyInterfaceChannelConfiguration(configuration.module5, emptyModule, availableConfiguration.protocols, 5);
				copyInterfaceChannelConfiguration(configuration.module6, emptyModule, availableConfiguration.protocols, 6);

			}

			foreach (var availablePoint in availableConfiguration.points)
			{
				if (availablePoint.Name == "Tank")
				{
					var numberOfTanksParameter = configuration.module0.moduleConfiguration.FirstOrDefault(x => x.Value.parameter == "NumberOfTanks");
					if (numberOfTanksParameter.Value.maximumValue.HasValue)
					{
						for (uint tankIndex = 0; tankIndex < (uint)numberOfTanksParameter.Value.maximumValue.Value; tankIndex++)
						{
							var tank = new Point(availablePoint.Name);
							configuration.points.Add(tank);

							foreach (var availablePointParameter in availablePoint.pointConfiguration.Values)
							{
								var tankParameter = new Parameter(
								availablePointParameter.configClass,
								availablePointParameter.parameter,
								availablePointParameter.description,
								availablePointParameter.dataType,
								availablePointParameter.displayFormat,
								availablePointParameter.minimumValue,
								availablePointParameter.maximumValue,
								((availablePointParameter.parameter == "Label") ? "Tank " + (tankIndex + 1).ToString("D3") : availablePointParameter.value),
								availablePointParameter.status,
								DateTime.UtcNow,
								((availablePointParameter.parameter == "Label") ? "Tank " + (tankIndex + 1).ToString("D3") : availablePointParameter.pendingValue),
								availablePointParameter.pendingStatus,
								DateTime.UtcNow,
								availablePointParameter.availableCommands,
								availablePointParameter.opcstartNodeID,
								availablePointParameter.opcstartNodeID + tankIndex,
								availablePointParameter.tab,
								availablePointParameter.section,
								availableCommandsOutputMatches: availablePointParameter.availableCommandsOutputMatches,
								variableAlarmNumber: availablePointParameter.variableAlarmNumber,
								datatypeLength: availablePointParameter.datatypeLength);
								tank.pointConfiguration.Add(tankParameter.identifier, tankParameter);
							}
						}
					}
				}

				else if (availablePoint.Name == " Alarms ")
				{
					var numberOfAlarmsParameter = configuration.module0.moduleConfiguration.FirstOrDefault(x => x.Value.parameter == "NumberOfAlarms");
					if (numberOfAlarmsParameter.Value.maximumValue.HasValue)
					{
						for (uint alarmIndex = 0; alarmIndex < (uint)numberOfAlarmsParameter.Value.maximumValue.Value; alarmIndex++)
						{
							var alarm = new Point(availablePoint.Name);
							configuration.points.Add(alarm);

							foreach (var availablePointParameter in availablePoint.pointConfiguration.Values)
							{
								var alarmParameter = new Parameter(
								availablePointParameter.configClass,
								availablePointParameter.parameter,
								availablePointParameter.description,
								availablePointParameter.dataType,
								availablePointParameter.displayFormat,
								availablePointParameter.minimumValue,
								availablePointParameter.maximumValue,
								((availablePointParameter.parameter == "Label") ? "Alarm " + (alarmIndex + 1).ToString("D3") : availablePointParameter.value),
								availablePointParameter.status,
								DateTime.UtcNow,
								((availablePointParameter.parameter == "Label") ? "Alarm " + (alarmIndex + 1).ToString("D3") : availablePointParameter.pendingValue),
								availablePointParameter.pendingStatus,
								DateTime.UtcNow,
								availablePointParameter.availableCommands,
								availablePointParameter.opcstartNodeID,
								availablePointParameter.opcstartNodeID + alarmIndex,
								availablePointParameter.tab,
								availablePointParameter.section,
								availableCommandsOutputMatches: availablePointParameter.availableCommandsOutputMatches,
								variableAlarmNumber: availablePointParameter.variableAlarmNumber,
								datatypeLength: availablePointParameter.datatypeLength);
								alarm.pointConfiguration.Add(alarmParameter.identifier, alarmParameter);
							}
						}
					}
				}

				else if (availablePoint.Name == " Modbus Floating Point Reg. ")
				{
					for (uint mfpregIndex = 0; mfpregIndex < 100; mfpregIndex++)
					{
						var mfpreg = new Point(availablePoint.Name);
						configuration.points.Add(mfpreg);

						foreach (var availablePointParameter in availablePoint.pointConfiguration.Values)
						{
							var mfpregParameter = new Parameter(
							availablePointParameter.configClass,
							availablePointParameter.parameter,
							availablePointParameter.description,
							availablePointParameter.dataType,
							availablePointParameter.displayFormat,
							availablePointParameter.minimumValue,
							availablePointParameter.maximumValue,
							((availablePointParameter.parameter == "Label") ? "Floating Point Reg " + (mfpregIndex + 1).ToString("D3") : availablePointParameter.value),
							availablePointParameter.status,
							DateTime.UtcNow,
							((availablePointParameter.parameter == "Label") ? "Floating Point Reg " + (mfpregIndex + 1).ToString("D3") : availablePointParameter.pendingValue),
							availablePointParameter.pendingStatus,
							DateTime.UtcNow,
							availablePointParameter.availableCommands,
							availablePointParameter.opcstartNodeID,
							availablePointParameter.opcstartNodeID + mfpregIndex,
							availablePointParameter.tab,
							availablePointParameter.section,
							availableCommandsOutputMatches: availablePointParameter.availableCommandsOutputMatches,
							variableAlarmNumber: availablePointParameter.variableAlarmNumber,
							datatypeLength: availablePointParameter.datatypeLength);
							mfpreg.pointConfiguration.Add(mfpregParameter.identifier, mfpregParameter);
						}
					}
				}

				else if (availablePoint.Name == " Modbus Integer Reg. ")
				{
					for (uint mfpregIndex = 0; mfpregIndex < 100; mfpregIndex++)
					{
						var mireg = new Point(availablePoint.Name);
						configuration.points.Add(mireg);

						foreach (var availablePointParameter in availablePoint.pointConfiguration.Values)
						{
							var miregParameter = new Parameter(
							availablePointParameter.configClass,
							availablePointParameter.parameter,
							availablePointParameter.description,
							availablePointParameter.dataType,
							availablePointParameter.displayFormat,
							availablePointParameter.minimumValue,
							availablePointParameter.maximumValue,
							((availablePointParameter.parameter == "Label") ? "Integer Reg " + (mfpregIndex + 1).ToString("D3") : availablePointParameter.value),
							availablePointParameter.status,
							DateTime.UtcNow,
							((availablePointParameter.parameter == "Label") ? "Integer Reg " + (mfpregIndex + 1).ToString("D3") : availablePointParameter.pendingValue),
							availablePointParameter.pendingStatus,
							DateTime.UtcNow,
							availablePointParameter.availableCommands,
							availablePointParameter.opcstartNodeID,
							availablePointParameter.opcstartNodeID + mfpregIndex,
							availablePointParameter.tab,
							availablePointParameter.section,
							availableCommandsOutputMatches: availablePointParameter.availableCommandsOutputMatches,
							variableAlarmNumber: availablePointParameter.variableAlarmNumber,
							datatypeLength: availablePointParameter.datatypeLength);
							mireg.pointConfiguration.Add(miregParameter.identifier, miregParameter);
						}
					}
				}

				else if (availablePoint.Name == " Gateway Block ")
				{
					for (uint gwblkIndex = 0; gwblkIndex < 108; gwblkIndex++)
					{
						var gwblk = new Point(availablePoint.Name);
						configuration.points.Add(gwblk);

						foreach (var availablePointParameter in availablePoint.pointConfiguration.Values)
						{
							var gwblkParameter = new Parameter(
							availablePointParameter.configClass,
							availablePointParameter.parameter,
							availablePointParameter.description,
							availablePointParameter.dataType,
							availablePointParameter.displayFormat,
							availablePointParameter.minimumValue,
							availablePointParameter.maximumValue,
							((availablePointParameter.parameter == "Label") ? "Gateway Block " + (gwblkIndex + 1).ToString("D3") : availablePointParameter.value),
							availablePointParameter.status,
							DateTime.UtcNow,
							((availablePointParameter.parameter == "Label") ? "Gateway Block " + (gwblkIndex + 1).ToString("D3") : availablePointParameter.pendingValue),
							availablePointParameter.pendingStatus,
							DateTime.UtcNow,
							availablePointParameter.availableCommands,
							availablePointParameter.opcstartNodeID,
							availablePointParameter.opcstartNodeID + gwblkIndex,
							availablePointParameter.tab,
							availablePointParameter.section,
							availableCommandsOutputMatches: availablePointParameter.availableCommandsOutputMatches,
							variableAlarmNumber: availablePointParameter.variableAlarmNumber,
							datatypeLength: availablePointParameter.datatypeLength);
							gwblk.pointConfiguration.Add(gwblkParameter.identifier, gwblkParameter);
						}
					}
				}

				else if (availablePoint.Name == " Register Map ")
				{
					for (uint regmapIndex = 0; regmapIndex < 800; regmapIndex++)
					{
						var regmap = new Point(availablePoint.Name);
						configuration.points.Add(regmap);

						foreach (var availablePointParameter in availablePoint.pointConfiguration.Values)
						{
							var regmapParameter = new Parameter(
							availablePointParameter.configClass,
							availablePointParameter.parameter,
							availablePointParameter.description,
							availablePointParameter.dataType,
							availablePointParameter.displayFormat,
							availablePointParameter.minimumValue,
							availablePointParameter.maximumValue,
							((availablePointParameter.parameter == "Label") ? "Register Map " + (regmapIndex + 1).ToString("D3") : availablePointParameter.value),
							availablePointParameter.status,
							DateTime.UtcNow,
							((availablePointParameter.parameter == "Label") ? "Register Map " + (regmapIndex + 1).ToString("D3") : availablePointParameter.pendingValue),
							availablePointParameter.pendingStatus,
							DateTime.UtcNow,
							availablePointParameter.availableCommands,
							availablePointParameter.opcstartNodeID,
							availablePointParameter.opcstartNodeID + regmapIndex,
							availablePointParameter.tab,
							availablePointParameter.section,
							availableCommandsOutputMatches: availablePointParameter.availableCommandsOutputMatches,
							variableAlarmNumber: availablePointParameter.variableAlarmNumber,
							datatypeLength: availablePointParameter.datatypeLength);
							regmap.pointConfiguration.Add(regmapParameter.identifier, regmapParameter);
						}
					}
				}
                else if (availablePoint.Name == " X.509 Certificate ")
                {
                    for (uint certIndex = 0; certIndex < 20; certIndex++)
                    {
                        var cert = new Point(availablePoint.Name);
                        configuration.points.Add(cert);

                        foreach (var availablePointParameter in availablePoint.pointConfiguration.Values)
                        {
                            var certParameter = new Parameter(
                            availablePointParameter.configClass,
                            availablePointParameter.parameter,
                            availablePointParameter.description,
                            availablePointParameter.dataType,
                            availablePointParameter.displayFormat,
                            availablePointParameter.minimumValue,
                            availablePointParameter.maximumValue,
                            ((availablePointParameter.parameter == "Label") ? "X.509 Certificate " + (certIndex + 1).ToString("D3") : availablePointParameter.value),
                            availablePointParameter.status,
                            DateTime.UtcNow,
                            ((availablePointParameter.parameter == "Label") ? "X.509 Certificate " + (certIndex + 1).ToString("D3") : availablePointParameter.pendingValue),
                            availablePointParameter.pendingStatus,
                            DateTime.UtcNow,
                            availablePointParameter.availableCommands,
                            availablePointParameter.opcstartNodeID,
                            availablePointParameter.opcstartNodeID + certIndex,
                            availablePointParameter.tab,
                            availablePointParameter.section,
                            availableCommandsOutputMatches: availablePointParameter.availableCommandsOutputMatches,
                            variableAlarmNumber: availablePointParameter.variableAlarmNumber,
                            datatypeLength: availablePointParameter.datatypeLength);
                            cert.pointConfiguration.Add(certParameter.identifier, certParameter);
                        }
                    }
                }
            }

			return configuration;
		}

		public RTUConfigurationDO LoadRtuXmlConfiguration(string filename)
		{

			RTUConfigurationDO configuration = GetBlankConfiguration(true,filename);
			/*
			var configuration = new RTUConfigurationDO();
			XmlDocument rtuxml = new XmlDocument();
			string rtuXmlPath = ConfigurationManager.AppSettings["rtuxmlPath"].ToString();
			rtuxml.Load(rtuXmlPath + "\\" + filename);
			*/
			return configuration;
		}

		private void copyCPUChannelConfiguration(RTUCPUModuleDO newCpuModule, AvailableModules defaultCpuModule, List<AvailableProtocols> protocols)
		{
			var defaultProtocol = defaultCpuModule.Channel1.channelProtocols.FirstOrDefault();
			var protocol = protocols.Where(x => x.Name == defaultProtocol).FirstOrDefault();

			// channel 1
			foreach (var channel1 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newCpuModule.channel1, channel1.Value, defaultCpuModule.Channel1, 0, 1);
			}
			newCpuModule.channel1.top = defaultCpuModule.Channel1.top;
			newCpuModule.channel1.left = defaultCpuModule.Channel1.left;
			newCpuModule.channel1.width = defaultCpuModule.Channel1.width;
			newCpuModule.channel1.height = defaultCpuModule.Channel1.height;

			// channel 2
			if (defaultProtocol != defaultCpuModule.Channel2.channelProtocols.FirstOrDefault())
			{
				defaultProtocol = defaultCpuModule.Channel2.channelProtocols.FirstOrDefault();
				protocol = protocols.Where(x => x.Name == defaultProtocol).FirstOrDefault();
			}

			foreach (var channel2 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newCpuModule.channel2, channel2.Value, defaultCpuModule.Channel2, 0, 2);
			}
			newCpuModule.channel2.top = defaultCpuModule.Channel2.top;
			newCpuModule.channel2.left = defaultCpuModule.Channel2.left;
			newCpuModule.channel2.width = defaultCpuModule.Channel2.width;
			newCpuModule.channel2.height = defaultCpuModule.Channel2.height;

			// channel 3
			if (defaultProtocol != defaultCpuModule.Channel3.channelProtocols.FirstOrDefault())
			{
				defaultProtocol = defaultCpuModule.Channel3.channelProtocols.FirstOrDefault();
				protocol = protocols.Where(x => x.Name == defaultProtocol).FirstOrDefault();
			}

			foreach (var channel3 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newCpuModule.channel3, channel3.Value, defaultCpuModule.Channel3, 0, 3);
			}
			newCpuModule.channel3.top = defaultCpuModule.Channel3.top;
			newCpuModule.channel3.left = defaultCpuModule.Channel3.left;
			newCpuModule.channel3.width = defaultCpuModule.Channel3.width;
			newCpuModule.channel3.height = defaultCpuModule.Channel3.height;

			// channel 4
			if (defaultProtocol != defaultCpuModule.Channel4.channelProtocols.FirstOrDefault())
			{
				defaultProtocol = defaultCpuModule.Channel4.channelProtocols.FirstOrDefault();
				protocol = protocols.Where(x => x.Name == defaultProtocol).FirstOrDefault();
			}

			foreach (var channel4 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newCpuModule.channel4, channel4.Value, defaultCpuModule.Channel4, 0, 4);
			}
			newCpuModule.channel4.top = defaultCpuModule.Channel4.top;
			newCpuModule.channel4.left = defaultCpuModule.Channel4.left;
			newCpuModule.channel4.width = defaultCpuModule.Channel4.width;
			newCpuModule.channel4.height = defaultCpuModule.Channel4.height;

			// channel 5
			if (defaultProtocol != defaultCpuModule.Channel5.channelProtocols.FirstOrDefault())
			{
				defaultProtocol = defaultCpuModule.Channel5.channelProtocols.FirstOrDefault();
				protocol = protocols.Where(x => x.Name == defaultProtocol).FirstOrDefault();
			}

			foreach (var channel5 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newCpuModule.channel5, channel5.Value, defaultCpuModule.Channel5, 0, 5);
			}
			newCpuModule.channel5.top = defaultCpuModule.Channel5.top;
			newCpuModule.channel5.left = defaultCpuModule.Channel5.left;
			newCpuModule.channel5.width = defaultCpuModule.Channel5.width;
			newCpuModule.channel5.height = defaultCpuModule.Channel5.height;


			// channel 6
			if (defaultProtocol != defaultCpuModule.Channel6.channelProtocols.FirstOrDefault())
			{
				defaultProtocol = defaultCpuModule.Channel6.channelProtocols.FirstOrDefault();
				protocol = protocols.Where(x => x.Name == defaultProtocol).FirstOrDefault();
			}

			foreach (var channel6 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newCpuModule.channel6, channel6.Value, defaultCpuModule.Channel6, 0, 6);
			}
			newCpuModule.channel6.top = defaultCpuModule.Channel6.top;
			newCpuModule.channel6.left = defaultCpuModule.Channel6.left;
			newCpuModule.channel6.width = defaultCpuModule.Channel6.width;
			newCpuModule.channel6.height = defaultCpuModule.Channel6.height;

			// channel 7
			if (defaultProtocol != defaultCpuModule.Channel7.channelProtocols.FirstOrDefault())
			{
				defaultProtocol = defaultCpuModule.Channel7.channelProtocols.FirstOrDefault();
				protocol = protocols.Where(x => x.Name == defaultProtocol).FirstOrDefault();
			}

			foreach (var channel7 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newCpuModule.channel7, channel7.Value, defaultCpuModule.Channel7, 0, 7);
			}
			newCpuModule.channel7.top = defaultCpuModule.Channel7.top;
			newCpuModule.channel7.left = defaultCpuModule.Channel7.left;
			newCpuModule.channel7.width = defaultCpuModule.Channel7.width;
			newCpuModule.channel7.height = defaultCpuModule.Channel7.height;

			// channel 8
			if (defaultProtocol != defaultCpuModule.Channel8.channelProtocols.FirstOrDefault())
			{
				defaultProtocol = defaultCpuModule.Channel8.channelProtocols.FirstOrDefault();
				protocol = protocols.Where(x => x.Name == defaultProtocol).FirstOrDefault();
			}

			foreach (var channel8 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newCpuModule.channel8, channel8.Value, defaultCpuModule.Channel8, 0, 8);
			}
			newCpuModule.channel8.top = defaultCpuModule.Channel8.top;
			newCpuModule.channel8.left = defaultCpuModule.Channel8.left;
			newCpuModule.channel8.width = defaultCpuModule.Channel8.width;
			newCpuModule.channel8.height = defaultCpuModule.Channel8.height;
		}

		private void copyInterfaceChannelConfiguration(RTUInterfaceModuleDO newInterfaceModule, AvailableModules defaultInterfaceModule, List<AvailableProtocols> protocols, uint ModuleId)
		{

			var defaultProtocol = defaultInterfaceModule.Channel1.channelProtocols.FirstOrDefault();
			var protocol = protocols.Where(x => x.Name == defaultProtocol).FirstOrDefault();

			// channel 1
			foreach (var channel1 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newInterfaceModule.channel1, channel1.Value, defaultInterfaceModule.Channel1, ModuleId, 1);
			}
			copyChannelCoordinates(newInterfaceModule.channel1, defaultInterfaceModule.Channel1);

			// channel 2
			foreach (var channel2 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newInterfaceModule.channel2, channel2.Value, defaultInterfaceModule.Channel2, ModuleId, 2);
			}
			copyChannelCoordinates(newInterfaceModule.channel2, defaultInterfaceModule.Channel2);

			// channel 3
			foreach (var channel3 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newInterfaceModule.channel3, channel3.Value, defaultInterfaceModule.Channel3, ModuleId, 3);
			}
			copyChannelCoordinates(newInterfaceModule.channel3, defaultInterfaceModule.Channel3);

			// channel 4
			foreach (var channel4 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newInterfaceModule.channel4, channel4.Value, defaultInterfaceModule.Channel4, ModuleId, 4);
			}
			copyChannelCoordinates(newInterfaceModule.channel4, defaultInterfaceModule.Channel4);

			// channel 5
			foreach (var channel5 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newInterfaceModule.channel5, channel5.Value, defaultInterfaceModule.Channel5, ModuleId, 5);
			}
			copyChannelCoordinates(newInterfaceModule.channel5, defaultInterfaceModule.Channel5);

			// channel 6
			foreach (var channel6 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newInterfaceModule.channel6, channel6.Value, defaultInterfaceModule.Channel6, ModuleId, 6);
			}
			copyChannelCoordinates(newInterfaceModule.channel6, defaultInterfaceModule.Channel6);

			// channel 7
			foreach (var channel7 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newInterfaceModule.channel7, channel7.Value, defaultInterfaceModule.Channel7, ModuleId, 7);
			}
			copyChannelCoordinates(newInterfaceModule.channel7, defaultInterfaceModule.Channel7);

			// channel 8
			foreach (var channel8 in protocol.protocolConfiguration)
			{
				copyRTUChannelParameter(newInterfaceModule.channel8, channel8.Value, defaultInterfaceModule.Channel8, ModuleId, 8);
			}
			copyChannelCoordinates(newInterfaceModule.channel8, defaultInterfaceModule.Channel8);

		}

		private void copyChannelCoordinates(RTUChannelDO newChannel, ConfigurableChannelDO defaultChannel)
		{
			newChannel.top = defaultChannel.top;
			newChannel.left = defaultChannel.left;
			newChannel.width = defaultChannel.width;
			newChannel.height = defaultChannel.height;
		}

		private void copyRTUChannelParameter(RTUChannelDO rtuChannel, Parameter channelParm, ConfigurableChannelDO availableChannelConfig, uint ModuleId, uint channelId)
		{
			var pendingValue = channelParm.pendingValue;
			var availableCommands = channelParm.availableCommands;
			if (channelParm.parameter == "Protocol")
			{
				var trimProtocolsForChannel = availableChannelConfig.channelProtocols.Select(x => x.Trim()).ToArray();
				availableCommands = string.Join(",", trimProtocolsForChannel);
				var trimAvailableProtocolArray = channelParm.availableCommands.Split(',').Select(x => x.Trim()).ToArray();
				pendingValue = (Array.IndexOf(trimAvailableProtocolArray, trimProtocolsForChannel[0]) + 1).ToString();
				rtuChannel.protocol = trimProtocolsForChannel[0];
			}
			rtuChannel.channelConfiguration.Add(channelParm.opcstartNodeID + ((ModuleId) * 8) + (channelId - 1),
						new Parameter(channelParm.configClass,
						channelParm.parameter,
						channelParm.description,
						channelParm.dataType,
						channelParm.displayFormat,
						channelParm.minimumValue,
						channelParm.maximumValue,
						pendingValue,
						0,
						null,
						pendingValue,
						0,
						null,
						availableCommands,
						identifier: channelParm.opcstartNodeID + ((ModuleId) * 8) + (channelId - 1),
						opcStartNodeId: channelParm.opcstartNodeID,
						availableCommandsOutputMatches: channelParm.availableCommandsOutputMatches,
						variableAlarmNumber: channelParm.variableAlarmNumber,
						datatypeLength: channelParm.datatypeLength));
		}
	}
}
