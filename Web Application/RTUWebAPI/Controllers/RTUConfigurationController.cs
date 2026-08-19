using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RTUWebAPI.Models;
using RTUWebAPI.Services;
using Newtonsoft.Json.Linq;
using Softing.Opc.Ua.Toolkit;
using System.Globalization;
using Newtonsoft.Json;

namespace RTUWebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RTUConfigurationController : RTUControllerBase
	{

		// GET: api/RTUConfiguration
		[HttpGet]
		[Route("Get")]
		public RTUConfigurationDO Get()
		{
				RTUConfigurationService RTUConfiguration = new RTUConfigurationService();
				return RTUConfiguration.GetBlankConfiguration(true);
			}

        // GET: api/RTUConfiguration/GetVersion
        [HttpGet]
        [Route("GetVersion")]
        public ActionResult GetVersion()
        {
            var assemblyVersion = typeof(Startup).Assembly.GetName().Version.ToString();
            var json = JsonWithErrorMessages(assemblyVersion);
            return json;
        }

        // GET: api/RTUConfiguration/GetNewConfiguration
        [HttpGet]
		[Route("GetNewConfiguration")]
		public ActionResult GetNewConfiguration()
		{
			RTUConfigurationService RTUConfiguration = new RTUConfigurationService();
			var json = JsonWithErrorMessages(RTUConfiguration.GetBlankConfiguration(true));
			return json;
		}

		// GET: api/RTUConfiguration/SetRtuData
		[HttpPost]
		[Route("GetRtuData")]
		public ActionResult GetRtuData([FromBody] JObject data)  // (RTUConnection connectionParms, List<UInt32> identifierList)
		{
			List<RtuDataValue> rtuDataValueList = null;
			RTUConnection connectionParms = data["connectionParms"].ToObject<RTUConnection>();
			var identifierList = data["identifierList"].ToObject<List<UInt32>>();

			try
			{
				using (var opcUaInterface = new OpcUaInterface(connectionParms))
				{
					var dataValueList = opcUaInterface.ReadData(identifierList);
					if (dataValueList != null)
					{
						rtuDataValueList = dataValueList.Select((item, index) => new RtuDataValue(ConvertRtuValue(item.Value), item.ServerTimestamp, item.StatusCode.Code, "string", "string", identifierList[index])).ToList();
					}
				}

				return JsonWithErrorMessages(rtuDataValueList);
			}
			catch (Exception e)
			{
				this.OnError(e.Message);
				return this.JsonWithErrorMessages(rtuDataValueList);
			}
		}

		// POST: api/RTUConfiguration/SetRtuData
		[HttpPost]
		[Route("SetRtuData")]
		public ActionResult SetRtuData([FromBody] JObject data)  // (RTUConnection connectionParms, List<RtuDataValue> rtuDataValueList)
		{
			RTUConnection connectionParms = data["connectionParms"].ToObject<RTUConnection>();
			var rtuDataValueList = data["rtuDataValueList"].ToObject<List<RtuDataValue>>();

			var writeValueList = new List<WriteValue>();

			var errorDictionary = new Dictionary<int, uint>();

			int index = 0;

			foreach (var rtuDataValue in rtuDataValueList)
			{
				object value = rtuDataValue.value;

				try
				{
					if (value is string)
					{
						switch (rtuDataValue.dataType)
						{
							case "int":
								{

									if (rtuDataValue.displayFormat == "TIME")
									{
										value = DateTime.Parse(value as string);
									}
									else
									{
										value = System.Convert.ToInt32(value as string);
									}
									break;
								}

							case "unsigned int":
							case "unsigned long":
								{
									value = System.Convert.ToUInt32(value as string);
									break;
								}

							case "long":
								{
									value = System.Convert.ToInt32(value as string);
									break;
								}

							case "double":
							case "single":
								{
									value = System.Convert.ToSingle(value as string);
									break;
								}

							default:
								break;
						}
					}
					else
					{
						if (rtuDataValue.dataType == "string")
						{
							value = "";
						}
					}

					var writeValue = new WriteValue();
					writeValue.AttributeId = AttributeId.Value;
					writeValue.NodeId = new NodeId(rtuDataValue.identifier, 1);
					writeValue.Value.Value = value;
					writeValue.Value.StatusCode = new StatusCode(rtuDataValue.status);
					writeValue.Value.ServerTimestamp = (rtuDataValue.timeStamp.HasValue) ? rtuDataValue.timeStamp.Value : DateTime.UtcNow;
					writeValueList.Add(writeValue);
					index++;
				}
				catch
				{
					errorDictionary.Add(index, Softing.Opc.Ua.Sdk.StatusCodes.BadOutOfRange);
					index++;
				}
			}

			List<UInt32> statusList = null;

			try
			{
				using (var opcUaInterface = new OpcUaInterface(connectionParms))
				{
					var statusCodeList = opcUaInterface.WriteData(writeValueList);
					statusList = statusCodeList.Select(s => s.Code).ToList();

					foreach (var key in errorDictionary.Keys)
					{
						statusList.Insert(key, errorDictionary[key]);
					}
				}
				return JsonWithErrorMessages(statusList);
			}
			catch (Exception e)
			{
				this.OnError(e.Message);
				return this.JsonWithErrorMessages(statusList);
			}
		}

		// POST: api/RTUConfiguration/ConnectToRTU
		[HttpPost]
		[Route("ConnectToRTU")]
        public ActionResult ConnectToRTU([FromForm] IFormFile certFile, [FromForm] string connectionParms)
        {
            RTUConnection connectionInfo = JsonConvert.DeserializeObject<RTUConnection>(connectionParms);
            RTUConfigurationDO RTUConfiguration = new RTUConfigurationDO();
			try
			{
				// get the available modules
				var availableConfiguration = new AvailableConfigurationService().GetAvailableConfiguration(connectionInfo.returnPoints, connectionInfo.filename);
				RTUConfiguration = new RTUConfigurationService().GetBlankConfiguration(connectionInfo.returnPoints);


				using (var opcUaInterface = new OpcUaInterface(connectionInfo))
				{
					// Chassis Configuration
					var chassisConfigurationDataList = opcUaInterface.ReadChassisConfigurationData();
					if (chassisConfigurationDataList == null)
					{
						this.OnError("Cannot connect to RTU at " + "opc.tcp://" + connectionInfo.url + ":4840");
					}
					else
					{
						resolveModuleById(RTUConfiguration.module1, availableConfiguration, chassisConfigurationDataList[0], 1);
						resolveModuleById(RTUConfiguration.module2, availableConfiguration, chassisConfigurationDataList[1], 2);
						resolveModuleById(RTUConfiguration.module3, availableConfiguration, chassisConfigurationDataList[2], 3);
						resolveModuleById(RTUConfiguration.module4, availableConfiguration, chassisConfigurationDataList[3], 4);
						resolveModuleById(RTUConfiguration.module5, availableConfiguration, chassisConfigurationDataList[4], 5);
						resolveModuleById(RTUConfiguration.module6, availableConfiguration, chassisConfigurationDataList[5], 6);
					}
				}
			}
			catch (Exception e)
			{
				this.OnError(e.Message);
				return this.JsonWithErrorMessages(RTUConfiguration);
			}

			return this.JsonWithErrorMessages( RTUConfiguration );
		}

		private string ConvertRtuValue(object dataValue)
		{
			if(dataValue is float)
			{
				return (dataValue as float?)?.ToString("0.#######");
			}

			else if(dataValue is double)
			{
				return (dataValue as double?)?.ToString("0.#######");
			}
			else if (dataValue is Int16)
			{
				return (dataValue as Int16?)?.ToString("D");
			}
			else if (dataValue is UInt16)
			{
				return (dataValue as UInt16?)?.ToString("D");
			}
			else if (dataValue is Int32)
			{
				return (dataValue as Int32?)?.ToString("D");
			}
			else if (dataValue is UInt32)
			{
				return (dataValue as UInt32?)?.ToString("D");
			}

			return dataValue?.ToString();
		}

		private void resolveModuleById(RTUInterfaceModuleDO module, AvailableConfiguration availableConfiguration, Softing.Opc.Ua.Toolkit.DataValue configuredModule, int moduleSlot) {
			var moduleId = configuredModule.Value.ToString().Split(' ')[0];

			var availableModule = availableConfiguration.modules.Where(x => x.Id == Convert.ToInt64(moduleId)).FirstOrDefault();
			if (availableModule != null)
			{
				module.id = availableModule.Id.ToString();
				module.img = availableModule.Img;
				module.name = availableModule.Name;
				updateParameterIdentifiers(module, moduleSlot);
			}
			else
			{
				module.id = "0";
				module.img = "emptymodule.png ";
				module.name = "Unknown";
			}
		}

		private void updateParameterIdentifiers(RTUInterfaceModuleDO module, int moduleId)
		{
			uint moduleInt = Convert.ToUInt32(moduleId);
			Dictionary<UInt32, Parameter> updatedIdentifiersDictionary = new Dictionary<UInt32, Parameter>();
			//for each parameter in module configuration, add the moduleId offset
			foreach (KeyValuePair<UInt32,Parameter> parameter in module.moduleConfiguration)
			{
				parameter.Value.identifier = parameter.Value.identifier + moduleInt;
				updatedIdentifiersDictionary.Add(parameter.Value.identifier, parameter.Value);
			}
			module.moduleConfiguration = updatedIdentifiersDictionary;

			//for each channel in the module, add the offsets
			Dictionary<UInt32, Parameter> updatedIdentifiersDictionaryCh1 = new Dictionary<UInt32, Parameter>();
			foreach (KeyValuePair<UInt32, Parameter> parameter in module.channel1.channelConfiguration)
			{
				parameter.Value.identifier = parameter.Value.identifier + ((moduleInt) * 8);
				updatedIdentifiersDictionaryCh1.Add(parameter.Value.identifier, parameter.Value);
			}
			module.channel1.channelConfiguration = updatedIdentifiersDictionaryCh1;

			Dictionary<UInt32, Parameter> updatedIdentifiersDictionaryCh2 = new Dictionary<UInt32, Parameter>();
			foreach (KeyValuePair<UInt32, Parameter> parameter in module.channel2.channelConfiguration)
			{
				parameter.Value.identifier = parameter.Value.identifier + ((moduleInt) * 8) + 1;
				updatedIdentifiersDictionaryCh2.Add(parameter.Value.identifier, parameter.Value);
			}
			module.channel2.channelConfiguration = updatedIdentifiersDictionaryCh2;

			Dictionary<UInt32, Parameter> updatedIdentifiersDictionaryCh3 = new Dictionary<UInt32, Parameter>();
			foreach (KeyValuePair<UInt32, Parameter> parameter in module.channel3.channelConfiguration)
			{
				parameter.Value.identifier = parameter.Value.identifier + ((moduleInt) * 8) + 2;
				updatedIdentifiersDictionaryCh3.Add(parameter.Value.identifier, parameter.Value);
			}
			module.channel3.channelConfiguration = updatedIdentifiersDictionaryCh3;

			Dictionary<UInt32, Parameter> updatedIdentifiersDictionaryCh4 = new Dictionary<UInt32, Parameter>();
			foreach (KeyValuePair<UInt32, Parameter> parameter in module.channel4.channelConfiguration)
			{
				parameter.Value.identifier = parameter.Value.identifier + ((moduleInt) * 8) + 3;
				updatedIdentifiersDictionaryCh4.Add(parameter.Value.identifier, parameter.Value);
			}
			module.channel4.channelConfiguration = updatedIdentifiersDictionaryCh4;

			Dictionary<UInt32, Parameter> updatedIdentifiersDictionaryCh5 = new Dictionary<UInt32, Parameter>();
			foreach (KeyValuePair<UInt32, Parameter> parameter in module.channel5.channelConfiguration)
			{
				parameter.Value.identifier = parameter.Value.identifier + ((moduleInt) * 8) + 4;
				updatedIdentifiersDictionaryCh5.Add(parameter.Value.identifier, parameter.Value);
			}
			module.channel5.channelConfiguration = updatedIdentifiersDictionaryCh5;

			Dictionary<UInt32, Parameter> updatedIdentifiersDictionaryCh6 = new Dictionary<UInt32, Parameter>();
			foreach (KeyValuePair<UInt32, Parameter> parameter in module.channel6.channelConfiguration)
			{
				parameter.Value.identifier = parameter.Value.identifier + ((moduleInt) * 8) + 5;
				updatedIdentifiersDictionaryCh6.Add(parameter.Value.identifier, parameter.Value);
			}
			module.channel6.channelConfiguration = updatedIdentifiersDictionaryCh6;

			Dictionary<UInt32, Parameter> updatedIdentifiersDictionaryCh7 = new Dictionary<UInt32, Parameter>();
			foreach (KeyValuePair<UInt32, Parameter> parameter in module.channel7.channelConfiguration)
			{
				parameter.Value.identifier = parameter.Value.identifier + ((moduleInt) * 8) + 6;
				updatedIdentifiersDictionaryCh7.Add(parameter.Value.identifier, parameter.Value);
			}
			module.channel7.channelConfiguration = updatedIdentifiersDictionaryCh7;

			Dictionary<UInt32, Parameter> updatedIdentifiersDictionaryCh8 = new Dictionary<UInt32, Parameter>();
			foreach (KeyValuePair<UInt32, Parameter> parameter in module.channel8.channelConfiguration)
			{
				parameter.Value.identifier = parameter.Value.identifier + ((moduleInt) * 8) + 7;
				updatedIdentifiersDictionaryCh8.Add(parameter.Value.identifier, parameter.Value);
			}
			module.channel8.channelConfiguration = updatedIdentifiersDictionaryCh8;

		}

		// copy interface modules
		private string getInterfaceModuleConfiguration(OpcUaInterface opcUaInterface, List<AvailableModules> availableModules, List<AvailableProtocols> availableProtocols, RTUInterfaceModuleDO newInterfaceModule, int moduleId)
		{
			string errorMsg = ""; 
			// Interface Module Configuration and Dynamic Data
			var interfaceModuleConfigurationReferenceList = opcUaInterface.ReadReferences(opcUaInterface.GetChassisBrowsePathList(OpcUaInterface.PathId.InterfaceModuleConfiguration, moduleId, 0));
			if (interfaceModuleConfigurationReferenceList == null) {
				errorMsg = "Error connecting to RTU";
				return errorMsg;
			}
			var interfaceModuleConfigurationDataList = opcUaInterface.ReadData(interfaceModuleConfigurationReferenceList);
			var interfaceModuleDynamicReferenceList = opcUaInterface.ReadReferences(opcUaInterface.GetChassisBrowsePathList(OpcUaInterface.PathId.InterfaceModuleDynamic, moduleId, 0));
			var interfaceModuleDynamicDataList = opcUaInterface.ReadData(interfaceModuleDynamicReferenceList);

			newInterfaceModule.id = "0";
			newInterfaceModule.name = "Empty";
			newInterfaceModule.img = "emptymodule.png";

			// find the configured module
			var configuredModuleIdx = interfaceModuleConfigurationReferenceList.ToList().FindIndex(x => x.BrowseName.Name == "ModConfigured");
			var fullConfiguredModuleId = string.Empty;
			AvailableModules moduleConfiguration = new AvailableModules();
			if (configuredModuleIdx > -1)
			{
				fullConfiguredModuleId = interfaceModuleConfigurationDataList[configuredModuleIdx].Value.ToString();

				// from the rtu the protocol name is within parenthesis
				if (!string.IsNullOrEmpty(fullConfiguredModuleId))
				{
					var extractModuleId = Regex.Match(fullConfiguredModuleId, @"\d+").Value;
					int configuredModuleId = Convert.ToInt32(extractModuleId);

					moduleConfiguration = availableModules.Where(x => x.Id == configuredModuleId).FirstOrDefault();
				}

				if (moduleConfiguration != null)
				{
					newInterfaceModule.id = moduleConfiguration.Id.ToString();
					newInterfaceModule.name = moduleConfiguration.Name;
					newInterfaceModule.img = moduleConfiguration.Img;
					newInterfaceModule.channel1.type = moduleConfiguration.Channel1.type;
					newInterfaceModule.channel2.type = moduleConfiguration.Channel2.type;
					newInterfaceModule.channel3.type = moduleConfiguration.Channel3.type;
					newInterfaceModule.channel4.type = moduleConfiguration.Channel4.type;
					newInterfaceModule.channel5.type = moduleConfiguration.Channel5.type;
					newInterfaceModule.channel6.type = moduleConfiguration.Channel6.type;
					newInterfaceModule.channel7.type = moduleConfiguration.Channel7.type;
					newInterfaceModule.channel8.type = moduleConfiguration.Channel8.type;

					newInterfaceModule.channel1.top = moduleConfiguration.Channel1.top;
					newInterfaceModule.channel2.top = moduleConfiguration.Channel2.top;
					newInterfaceModule.channel3.top = moduleConfiguration.Channel3.top;
					newInterfaceModule.channel4.top = moduleConfiguration.Channel4.top;
					newInterfaceModule.channel5.top = moduleConfiguration.Channel5.top;
					newInterfaceModule.channel6.top = moduleConfiguration.Channel6.top;
					newInterfaceModule.channel7.top = moduleConfiguration.Channel7.top;
					newInterfaceModule.channel8.top = moduleConfiguration.Channel8.top;

					newInterfaceModule.channel1.left = moduleConfiguration.Channel1.left;
					newInterfaceModule.channel2.left = moduleConfiguration.Channel2.left;
					newInterfaceModule.channel3.left = moduleConfiguration.Channel3.left;
					newInterfaceModule.channel4.left = moduleConfiguration.Channel4.left;
					newInterfaceModule.channel5.left = moduleConfiguration.Channel5.left;
					newInterfaceModule.channel6.left = moduleConfiguration.Channel6.left;
					newInterfaceModule.channel7.left = moduleConfiguration.Channel7.left;
					newInterfaceModule.channel8.left = moduleConfiguration.Channel8.left;

					newInterfaceModule.channel1.width = moduleConfiguration.Channel1.width;
					newInterfaceModule.channel2.width = moduleConfiguration.Channel2.width;
					newInterfaceModule.channel3.width = moduleConfiguration.Channel3.width;
					newInterfaceModule.channel4.width = moduleConfiguration.Channel4.width;
					newInterfaceModule.channel5.width = moduleConfiguration.Channel5.width;
					newInterfaceModule.channel6.width = moduleConfiguration.Channel6.width;
					newInterfaceModule.channel7.width = moduleConfiguration.Channel7.width;
					newInterfaceModule.channel8.width = moduleConfiguration.Channel8.width;

					newInterfaceModule.channel1.height = moduleConfiguration.Channel1.height;
					newInterfaceModule.channel2.height = moduleConfiguration.Channel2.height;
					newInterfaceModule.channel3.height = moduleConfiguration.Channel3.height;
					newInterfaceModule.channel4.height = moduleConfiguration.Channel4.height;
					newInterfaceModule.channel5.height = moduleConfiguration.Channel5.height;
					newInterfaceModule.channel6.height = moduleConfiguration.Channel6.height;
					newInterfaceModule.channel7.height = moduleConfiguration.Channel7.height;
					newInterfaceModule.channel8.height = moduleConfiguration.Channel8.height;

				}
			}

			if (interfaceModuleConfigurationReferenceList != null)
			{
				for (int i = 0; i < interfaceModuleConfigurationReferenceList.Count; i++)
				{
					var interfaceModuleConfiguration = interfaceModuleConfigurationReferenceList[i];
					var interfaceModuleConfigurationData = interfaceModuleConfigurationDataList[i];

					// find the parameter in the module to get the metadata (data type, min, max, ...)
					Parameter parameterConfiguration = null;
					if (moduleConfiguration != null)
					{

                        parameterConfiguration = moduleConfiguration.moduleConfiguration.Where(x => x.Value.parameter == interfaceModuleConfiguration.BrowseName.Name).FirstOrDefault().Value;						}

                        if ( parameterConfiguration != null) { 
					    string newValue = interfaceModuleConfigurationData.Value != null ? this.ConvertRtuValue(interfaceModuleConfigurationData.Value) : "";
					    uint newStatus = interfaceModuleConfigurationData.StatusCode.Code;
					    DateTime newTimeStamp = interfaceModuleConfigurationData.ServerTimestamp;

					    var newAvailableCommands = parameterConfiguration != null ? parameterConfiguration.availableCommands : "";

					    UInt32 identifierKey = parameterConfiguration.opcstartNodeID + (uint) moduleId - 1;

					    newInterfaceModule.moduleConfiguration.Add(identifierKey,new Parameter(ConfigurationClass.CONFIG,
													    interfaceModuleConfiguration.BrowseName.Name,
													    parameterConfiguration != null ? parameterConfiguration.description : interfaceModuleConfiguration.DisplayName.Text,
													    parameterConfiguration != null ? parameterConfiguration.dataType : "string",
													    parameterConfiguration != null ? parameterConfiguration.displayFormat : "",
													    parameterConfiguration != null ? parameterConfiguration.minimumValue : null,
													    parameterConfiguration != null ? parameterConfiguration.maximumValue : null,
													    newValue,
													    newStatus,
													    newTimeStamp,
													    newValue,
													    newStatus,
													    newTimeStamp,
													    availableCommands: newAvailableCommands,
													    identifier: identifierKey,
													    opcStartNodeId: parameterConfiguration.opcstartNodeID,
													    availableCommandsOutputMatches: parameterConfiguration.availableCommandsOutputMatches,
													    variableAlarmNumber: parameterConfiguration.variableAlarmNumber,
													    datatypeLength:parameterConfiguration.datatypeLength
													    ));

                        }
				}
			}

            if (interfaceModuleDynamicReferenceList != null)
			{
				for (int i = 0; i < interfaceModuleDynamicReferenceList.Count; i++)
				{
					var interfaceModuleConfiguration = interfaceModuleDynamicReferenceList[i];
					Softing.Opc.Ua.Toolkit.DataValue interfaceModuleConfigurationData = new Softing.Opc.Ua.Toolkit.DataValue();

					if (interfaceModuleDynamicDataList != null)
					{
							interfaceModuleConfigurationData = interfaceModuleDynamicDataList[i];
					}

					// find the parameter in the module to get the metadata (data type, min, max, ...)
					Parameter parameterConfiguration = null;
					if (moduleConfiguration != null)
					{
                        //parameterConfiguration = moduleConfiguration.moduleConfiguration.Where(x => x.parameter == interfaceModuleConfiguration.BrowseName.Name).FirstOrDefault();
                        parameterConfiguration = moduleConfiguration.moduleConfiguration.Where(x => x.Value.parameter == interfaceModuleConfiguration.BrowseName.Name).FirstOrDefault().Value;
					}
                    if (parameterConfiguration != null)
                    {
                        string newValue = interfaceModuleConfigurationData.Value != null ? this.ConvertRtuValue(interfaceModuleConfigurationData.Value) : "";
					    uint newStatus = interfaceModuleConfigurationData.StatusCode.Code;
					    DateTime newTimeStamp = interfaceModuleConfigurationData.ServerTimestamp;

					    var newAvailableCommands = parameterConfiguration != null ? parameterConfiguration.availableCommands : "";

                        if (parameterConfiguration != null)
                        {
                            UInt32 identifierKey = parameterConfiguration.opcstartNodeID + (uint)moduleId - 1;

                            newInterfaceModule.moduleConfiguration.Add(identifierKey, new Parameter(parameterConfiguration != null ? parameterConfiguration.configClass : ConfigurationClass.DYNAMIC,
                                                        interfaceModuleConfiguration.BrowseName.Name,
                                                        parameterConfiguration != null ? parameterConfiguration.description : interfaceModuleConfiguration.DisplayName.Text,
                                                        parameterConfiguration != null ? parameterConfiguration.dataType : "string",
                                                        parameterConfiguration != null ? parameterConfiguration.displayFormat : "",
                                                        parameterConfiguration != null ? parameterConfiguration.minimumValue : null,
                                                        parameterConfiguration != null ? parameterConfiguration.maximumValue : null,
                                                        newValue,
                                                        newStatus,
                                                        newTimeStamp,
                                                        newValue,
                                                        newStatus,
                                                        newTimeStamp,
                                                        availableCommands: newAvailableCommands,
                                                        identifier: identifierKey,
                                                        opcStartNodeId: parameterConfiguration.opcstartNodeID,
                                                        availableCommandsOutputMatches: parameterConfiguration.availableCommandsOutputMatches,
                                                        variableAlarmNumber: parameterConfiguration.variableAlarmNumber,
                                                        datatypeLength: parameterConfiguration.datatypeLength
                                                        ));
                        }
					}
				}
			}

         // get the chennel info for the module
         getInterfaceChannelConfiguration(opcUaInterface, availableProtocols, newInterfaceModule.channel1, moduleId, 1, moduleConfiguration.Channel1.channelProtocols);
			getInterfaceChannelConfiguration(opcUaInterface, availableProtocols, newInterfaceModule.channel2, moduleId, 2, moduleConfiguration.Channel2.channelProtocols);
			getInterfaceChannelConfiguration(opcUaInterface, availableProtocols, newInterfaceModule.channel3, moduleId, 3, moduleConfiguration.Channel3.channelProtocols);
			getInterfaceChannelConfiguration(opcUaInterface, availableProtocols, newInterfaceModule.channel4, moduleId, 4, moduleConfiguration.Channel4.channelProtocols);
			getInterfaceChannelConfiguration(opcUaInterface, availableProtocols, newInterfaceModule.channel5, moduleId, 5, moduleConfiguration.Channel5.channelProtocols);
			getInterfaceChannelConfiguration(opcUaInterface, availableProtocols, newInterfaceModule.channel6, moduleId, 6, moduleConfiguration.Channel6.channelProtocols);
			getInterfaceChannelConfiguration(opcUaInterface, availableProtocols, newInterfaceModule.channel7, moduleId, 7, moduleConfiguration.Channel7.channelProtocols);
			getInterfaceChannelConfiguration(opcUaInterface, availableProtocols, newInterfaceModule.channel8, moduleId, 8, moduleConfiguration.Channel8.channelProtocols);
			return errorMsg;
		}

		// copy cpu modules
		private string getCPUModuleConfiguration(OpcUaInterface opcUaInterface, List<AvailableModules> availableModules, List<AvailableProtocols> availableProtocols, RTUCPUModuleDO newCPUModule)
		{
			string errorMsg = "";

			// CPU Module Configuration & Dynamic
			var cpuModuleConfigurationReferenceList = opcUaInterface.ReadReferences(opcUaInterface.GetChassisBrowsePathList(OpcUaInterface.PathId.CpuModuleConfiguration));
			if (cpuModuleConfigurationReferenceList == null)
			{
				errorMsg = "Error connecting to RTU";
				return errorMsg;
			}
			var cpuModuleConfigurationDataList = opcUaInterface.ReadData(cpuModuleConfigurationReferenceList);
			var cpuModuleDynamicReferenceList = opcUaInterface.ReadReferences(opcUaInterface.GetChassisBrowsePathList(OpcUaInterface.PathId.CpuModuleDynamic));
			var cpuModuleDynamicDataList = opcUaInterface.ReadData(cpuModuleDynamicReferenceList);

			newCPUModule.name = "CPU";
			newCPUModule.img = "cpu.png";

			// copy the CPU channel coordinates
			var moduleConfiguration = availableModules.Where(x => x.Id == -1).FirstOrDefault();
			if (moduleConfiguration != null)
			{
				newCPUModule.name = moduleConfiguration.Name;
				newCPUModule.img = moduleConfiguration.Img;
				newCPUModule.channel1.type = moduleConfiguration.Channel1.type;
				newCPUModule.channel2.type = moduleConfiguration.Channel2.type;
				newCPUModule.channel3.type = moduleConfiguration.Channel3.type;
				newCPUModule.channel4.type = moduleConfiguration.Channel4.type;
				newCPUModule.channel5.type = moduleConfiguration.Channel5.type;
				newCPUModule.channel6.type = moduleConfiguration.Channel6.type;
				newCPUModule.channel7.type = moduleConfiguration.Channel7.type;
				newCPUModule.channel8.type = moduleConfiguration.Channel8.type;

				newCPUModule.channel1.top = moduleConfiguration.Channel1.top;
				newCPUModule.channel2.top = moduleConfiguration.Channel2.top;
				newCPUModule.channel3.top = moduleConfiguration.Channel3.top;
				newCPUModule.channel4.top = moduleConfiguration.Channel4.top;
				newCPUModule.channel5.top = moduleConfiguration.Channel5.top;
				newCPUModule.channel6.top = moduleConfiguration.Channel6.top;
				newCPUModule.channel7.top = moduleConfiguration.Channel7.top;
				newCPUModule.channel8.top = moduleConfiguration.Channel8.top;

				newCPUModule.channel1.left = moduleConfiguration.Channel1.left;
				newCPUModule.channel2.left = moduleConfiguration.Channel2.left;
				newCPUModule.channel3.left = moduleConfiguration.Channel3.left;
				newCPUModule.channel4.left = moduleConfiguration.Channel4.left;
				newCPUModule.channel5.left = moduleConfiguration.Channel5.left;
				newCPUModule.channel6.left = moduleConfiguration.Channel6.left;
				newCPUModule.channel7.left = moduleConfiguration.Channel7.left;
				newCPUModule.channel8.left = moduleConfiguration.Channel8.left;

				newCPUModule.channel1.width = moduleConfiguration.Channel1.width;
				newCPUModule.channel2.width = moduleConfiguration.Channel2.width;
				newCPUModule.channel3.width = moduleConfiguration.Channel3.width;
				newCPUModule.channel4.width = moduleConfiguration.Channel4.width;
				newCPUModule.channel5.width = moduleConfiguration.Channel5.width;
				newCPUModule.channel6.width = moduleConfiguration.Channel6.width;
				newCPUModule.channel7.width = moduleConfiguration.Channel7.width;
				newCPUModule.channel8.width = moduleConfiguration.Channel8.width;

				newCPUModule.channel1.height = moduleConfiguration.Channel1.height;
				newCPUModule.channel2.height = moduleConfiguration.Channel2.height;
				newCPUModule.channel3.height = moduleConfiguration.Channel3.height;
				newCPUModule.channel4.height = moduleConfiguration.Channel4.height;
				newCPUModule.channel5.height = moduleConfiguration.Channel5.height;
				newCPUModule.channel6.height = moduleConfiguration.Channel6.height;
				newCPUModule.channel7.height = moduleConfiguration.Channel7.height;
				newCPUModule.channel8.height = moduleConfiguration.Channel8.height;
			}

			if (cpuModuleConfigurationReferenceList != null)
			{
				for (int i = 0; i < cpuModuleConfigurationReferenceList.Count; i++)
				{
					var cpuModuleConfiguration = cpuModuleConfigurationReferenceList[i];
					var cpuModuleConfigurationData = cpuModuleConfigurationDataList[i];

					// find the parameter in the module to get the metadata (data type, min, max, ...)
						Parameter parameterConfiguration = null;
					if (moduleConfiguration != null) {
						parameterConfiguration = moduleConfiguration.moduleConfiguration.Where(x => x.Value.parameter == cpuModuleConfiguration.BrowseName.Name).FirstOrDefault().Value;
						//parameterConfiguration = moduleConfiguration.moduleConfiguration.Where(x => x.parameter == cpuModuleConfiguration.BrowseName.Name).FirstOrDefault();
					}

					string newValue = cpuModuleConfigurationData.Value != null ? this.ConvertRtuValue(cpuModuleConfigurationData.Value) : "";
					var newAvailableCommands = parameterConfiguration != null ? parameterConfiguration.availableCommands : "";
					// for dropdowns we need to parse the value to extract the first digits
					if ( parameterConfiguration != null
					&& parameterConfiguration.displayFormat == "dropdown")
					{ 
						newValue = newValue.Split(' ')[0];
					}
	
					if (parameterConfiguration != null)
					{
						newCPUModule.moduleConfiguration.Add(parameterConfiguration.opcstartNodeID, new Parameter(ConfigurationClass.CONFIG,
														cpuModuleConfiguration.BrowseName.Name,
														parameterConfiguration != null ? parameterConfiguration.description : cpuModuleConfiguration.DisplayName.Text,
														parameterConfiguration != null ? parameterConfiguration.dataType : "string",
														parameterConfiguration != null ? parameterConfiguration.displayFormat : "",
														parameterConfiguration != null ? parameterConfiguration.minimumValue : null,
														parameterConfiguration != null ? parameterConfiguration.maximumValue : null,
														newValue,
														cpuModuleConfigurationData.StatusCode.Code,
														cpuModuleConfigurationData.ServerTimestamp,
														newValue,
														cpuModuleConfigurationData.StatusCode.Code,
														cpuModuleConfigurationData.ServerTimestamp,
														availableCommands: newAvailableCommands,
														opcStartNodeId: parameterConfiguration.opcstartNodeID,
														identifier: parameterConfiguration.opcstartNodeID,
														tab: parameterConfiguration.tab,
														section: parameterConfiguration.section,
														parameterIsVisible: parameterConfiguration.parameterIsVisible,
														availableCommandsOutputMatches: parameterConfiguration.availableCommandsOutputMatches,
														variableAlarmNumber: parameterConfiguration.variableAlarmNumber,
														datatypeLength: parameterConfiguration.datatypeLength
													));
					}
				}
			}

			if (cpuModuleDynamicReferenceList != null)
			{
				for (int i = 0; i < cpuModuleDynamicReferenceList.Count; i++)
				{
					var cpuModuleConfiguration = cpuModuleDynamicReferenceList[i];
					Softing.Opc.Ua.Toolkit.DataValue cpuModuleConfigurationData = new Softing.Opc.Ua.Toolkit.DataValue();

					if (cpuModuleDynamicDataList != null)
					{
							cpuModuleConfigurationData = cpuModuleDynamicDataList[i];
					}

					// find the parameter in the module to get the metadata (data type, min, max, ...)
					Parameter parameterConfiguration = null;
					if (moduleConfiguration != null)
					{
							//parameterConfiguration = moduleConfiguration.moduleConfiguration.Where(x => x.parameter == cpuModuleConfiguration.BrowseName.Name).FirstOrDefault();
							parameterConfiguration = moduleConfiguration.moduleConfiguration.Where(x => x.Value.parameter == cpuModuleConfiguration.BrowseName.Name).FirstOrDefault().Value;

					}

					string newValue = cpuModuleConfigurationData.Value != null ? cpuModuleConfigurationData.Value.ToString() : "";
					var newAvailableCommands = parameterConfiguration != null ? parameterConfiguration.availableCommands : "";
					// for dropdowns we need to parse the value to extract the first digits
					if (parameterConfiguration != null
					&& parameterConfiguration.displayFormat == "dropdown")
					{
						newValue = newValue.Split(' ')[0];
					}

					if (parameterConfiguration != null)
						newCPUModule.moduleConfiguration.Add(parameterConfiguration.opcstartNodeID,
												new Parameter(
												parameterConfiguration != null ? parameterConfiguration.configClass : ConfigurationClass.DYNAMIC,
												cpuModuleConfiguration.BrowseName.Name,
												parameterConfiguration != null ? parameterConfiguration.description : cpuModuleConfiguration.DisplayName.Text,
												parameterConfiguration != null ? parameterConfiguration.dataType : "string",
												parameterConfiguration != null ? parameterConfiguration.displayFormat : "",
												parameterConfiguration != null ? parameterConfiguration.minimumValue : null,
												parameterConfiguration != null ? parameterConfiguration.maximumValue : null,
												newValue,
												cpuModuleConfigurationData.StatusCode.Code,
												cpuModuleConfigurationData.ServerTimestamp,
												newValue,
												cpuModuleConfigurationData.StatusCode.Code,
												cpuModuleConfigurationData.ServerTimestamp,
												availableCommands: newAvailableCommands,
												opcStartNodeId: parameterConfiguration.opcstartNodeID,
												identifier: parameterConfiguration.opcstartNodeID,
												availableCommandsOutputMatches: parameterConfiguration.availableCommandsOutputMatches,
												variableAlarmNumber: parameterConfiguration.variableAlarmNumber,
												datatypeLength: parameterConfiguration.datatypeLength
												));

				}
			}

			// get the channel info for the module
			getCpuChannelConfiguration(opcUaInterface, availableProtocols, newCPUModule.channel1, 0, 1, moduleConfiguration.Channel1.channelProtocols);
			getCpuChannelConfiguration(opcUaInterface, availableProtocols, newCPUModule.channel2, 0, 2, moduleConfiguration.Channel2.channelProtocols);
			getCpuChannelConfiguration(opcUaInterface, availableProtocols, newCPUModule.channel3, 0, 3, moduleConfiguration.Channel3.channelProtocols);
			getCpuChannelConfiguration(opcUaInterface, availableProtocols, newCPUModule.channel4, 0, 4, moduleConfiguration.Channel4.channelProtocols);
			getCpuChannelConfiguration(opcUaInterface, availableProtocols, newCPUModule.channel5, 0, 5, moduleConfiguration.Channel5.channelProtocols);
			getCpuChannelConfiguration(opcUaInterface, availableProtocols, newCPUModule.channel6, 0, 6, moduleConfiguration.Channel6.channelProtocols);
			getCpuChannelConfiguration(opcUaInterface, availableProtocols, newCPUModule.channel7, 0, 7, moduleConfiguration.Channel7.channelProtocols);
			getCpuChannelConfiguration(opcUaInterface, availableProtocols, newCPUModule.channel8, 0, 8, moduleConfiguration.Channel8.channelProtocols);
			return errorMsg;
		}


		// copy channel information from the RTU
		private void getCpuChannelConfiguration(OpcUaInterface opcUaInterface, List<AvailableProtocols> availableProtocols, RTUChannelDO newCPUChannel, int moduleId, int channelId, List<string> protocols)
		{
			string protocolAvailableCommands = "";
			foreach (var protocol in protocols)
			{
				protocolAvailableCommands += protocol.Trim();
				if (protocol != protocols[protocols.Count - 1])
				{
					protocolAvailableCommands += ",";
				}
			}


			// Interface Module Channel Configuration and Dynamic Data
			var cpuModuleChannelConfigurationReferenceList = opcUaInterface.ReadReferences(opcUaInterface.GetChassisBrowsePathList(OpcUaInterface.PathId.CpuModuleChannelConfiguration, moduleId, channelId));
			if (cpuModuleChannelConfigurationReferenceList == null)
				return; // failed to read channel
			var cpuModuleChannelConfigurationDataList = opcUaInterface.ReadData(cpuModuleChannelConfigurationReferenceList);
			if (cpuModuleChannelConfigurationDataList == null)
				return; // failed to read channel 
			var cpuModuleChannelDynamicReferenceList = opcUaInterface.ReadReferences(opcUaInterface.GetChassisBrowsePathList(OpcUaInterface.PathId.CpuModuleChannelDynamic, moduleId, channelId));
			if (cpuModuleChannelDynamicReferenceList == null)
				return; // failed to read channel
			var cpuModuleChannelDynamicDataList = opcUaInterface.ReadData(cpuModuleChannelDynamicReferenceList);
			if (cpuModuleChannelDynamicDataList == null)
				return;  // failed to read channel

			// find the protocol
			var protocolIdx = cpuModuleChannelConfigurationReferenceList.ToList().FindIndex(x => x.BrowseName.Name == "Protocol");
			AvailableProtocols protocolParameters = new AvailableProtocols();

			if (protocolIdx > -1){
				Int32 protocolId = 0;
				var protocolParameter = cpuModuleChannelConfigurationDataList[protocolIdx];
				if (protocolParameter.Value is UInt32)
				{
					protocolId = Convert.ToInt32(protocolParameter.Value);
					if (protocolId > 0
					&& protocolId <= availableProtocols.Count)
					{
						protocolParameters = availableProtocols[protocolId - 1];
					}
				}	
			}

			if (cpuModuleChannelConfigurationReferenceList != null)
			{
				for (int i = 0; i < cpuModuleChannelConfigurationReferenceList.Count; i++)
				{
					var cpuModuleChannelConfiguration = cpuModuleChannelConfigurationReferenceList[i];
					var cpuModuleChannelConfigurationData = cpuModuleChannelConfigurationDataList[i];

					// find the parameter in the module to get the metadata (data type, min, max, ...)
					Parameter parameterConfiguration = null;
					if (protocolParameters != null && protocolParameters.protocolConfiguration != null)
					{
						parameterConfiguration = protocolParameters.protocolConfiguration.Where(x => x.Value.parameter == cpuModuleChannelConfiguration.BrowseName.Name).FirstOrDefault().Value;
					}

					string newValue = cpuModuleChannelConfigurationData.Value != null ? this.ConvertRtuValue(cpuModuleChannelConfigurationData.Value) : "";
					var newAvailableCommands = parameterConfiguration != null ? parameterConfiguration.availableCommands : "";

					if (parameterConfiguration != null && parameterConfiguration.parameter == "Protocol")
					{
						newAvailableCommands = protocolAvailableCommands;
					}


					// for dropdowns we need to parse the value to extract the first digits
					if (parameterConfiguration != null
					&& parameterConfiguration.displayFormat == "dropdown"
					&& parameterConfiguration.parameter != "ModbusMap")
					{
						newValue = newValue.Split(' ')[0];
					}
					if (parameterConfiguration != null)
							newCPUChannel.channelConfiguration.Add(parameterConfiguration.opcstartNodeID + (uint) channelId - 1, new Parameter(ConfigurationClass.CONFIG,
													cpuModuleChannelConfiguration.BrowseName.Name,
													parameterConfiguration != null ? parameterConfiguration.description : cpuModuleChannelConfiguration.BrowseName.Name,
													parameterConfiguration != null ? parameterConfiguration.dataType : "string",
													parameterConfiguration != null ? parameterConfiguration.displayFormat : "",
													parameterConfiguration != null ? parameterConfiguration.minimumValue : null,
													parameterConfiguration != null ? parameterConfiguration.maximumValue : null,
													newValue,
													cpuModuleChannelConfigurationData.StatusCode.Code,
													cpuModuleChannelConfigurationData.ServerTimestamp,
													newValue,
													cpuModuleChannelConfigurationData.StatusCode.Code,
													cpuModuleChannelConfigurationData.ServerTimestamp,
													availableCommands: newAvailableCommands,
													opcStartNodeId: parameterConfiguration.opcstartNodeID,
													identifier: parameterConfiguration.opcstartNodeID + (uint) channelId-1	,
													availableCommandsOutputMatches: parameterConfiguration.availableCommandsOutputMatches,
													variableAlarmNumber: parameterConfiguration.variableAlarmNumber,
													datatypeLength: parameterConfiguration.datatypeLength
													));
				}
			}

			if (cpuModuleChannelDynamicReferenceList != null)
			{
				for (int i = 0; i < cpuModuleChannelDynamicReferenceList.Count; i++)
				{
					var cpuModuleChannelConfiguration = cpuModuleChannelDynamicReferenceList[i];

					Softing.Opc.Ua.Toolkit.DataValue cpuModuleChannelConfigurationData = new Softing.Opc.Ua.Toolkit.DataValue();

					if (cpuModuleChannelDynamicDataList != null )
					{
							cpuModuleChannelConfigurationData = cpuModuleChannelDynamicDataList[i];
					}

					// find the parameter in the module to get the metadata (data type, min, max, ...)
					Parameter parameterConfiguration = null;
					if (protocolParameters != null)
					{
							//parameterConfiguration = protocolParameters.protocolConfiguration.Where(x => x.parameter == cpuModuleChannelConfiguration.BrowseName.Name).FirstOrDefault();
							parameterConfiguration = protocolParameters.protocolConfiguration.Where(x => x.Value.parameter == cpuModuleChannelConfiguration.BrowseName.Name).FirstOrDefault().Value;

					}

					string newValue = cpuModuleChannelConfigurationData.Value != null ? this.ConvertRtuValue(cpuModuleChannelConfigurationData.Value) : "";
					var newAvailableCommands = parameterConfiguration != null ? parameterConfiguration.availableCommands : "";
					// for dropdowns we need to parse the value to extract the first digits
					if (parameterConfiguration != null
					&& parameterConfiguration.displayFormat == "dropdown"
					&& parameterConfiguration.parameter != "ModbusMap")
					{

						newValue = newValue.Split(' ')[0];
					}
					if (parameterConfiguration != null)
							newCPUChannel.channelConfiguration.Add(parameterConfiguration.opcstartNodeID + (uint) channelId - 1, new Parameter(parameterConfiguration != null ? parameterConfiguration.configClass : ConfigurationClass.DYNAMIC,
													cpuModuleChannelConfiguration.BrowseName.Name,
													parameterConfiguration != null ? parameterConfiguration.description : cpuModuleChannelConfiguration.BrowseName.Name,
													parameterConfiguration != null ? parameterConfiguration.dataType : "string",
													parameterConfiguration != null ? parameterConfiguration.displayFormat : "",
													parameterConfiguration != null ? parameterConfiguration.minimumValue : null,
													parameterConfiguration != null ? parameterConfiguration.maximumValue : null,
													newValue,
													cpuModuleChannelConfigurationData.StatusCode.Code,
													cpuModuleChannelConfigurationData.ServerTimestamp,
													newValue,
													cpuModuleChannelConfigurationData.StatusCode.Code,
													cpuModuleChannelConfigurationData.ServerTimestamp,
													availableCommands: newAvailableCommands,
													opcStartNodeId: parameterConfiguration.opcstartNodeID,
													identifier: parameterConfiguration.opcstartNodeID + (uint) channelId - 1,
													availableCommandsOutputMatches: parameterConfiguration.availableCommandsOutputMatches,
													variableAlarmNumber: parameterConfiguration.variableAlarmNumber,
													datatypeLength: parameterConfiguration.datatypeLength
													));
				}
			}
		}

		private void getInterfaceChannelConfiguration(OpcUaInterface opcUaInterface, List<AvailableProtocols> availableProtocols, RTUChannelDO newInterfaceChannel, int moduleId, int channelId, List<string> protocols)
		{
			string protocolAvailableCommands = "";
			foreach(var protocol in protocols)
			{
				protocolAvailableCommands += protocol.Trim();
				if(protocol != protocols[protocols.Count-1])
				{
					protocolAvailableCommands += ",";
				}
			}

			// Interface Module Channel Configuration and Dynamic Data
			var interfaceModuleChannelConfigurationReferenceList = opcUaInterface.ReadReferences(opcUaInterface.GetChassisBrowsePathList(OpcUaInterface.PathId.InterfaceModuleChannelConfiguration, moduleId, channelId));
			if (interfaceModuleChannelConfigurationReferenceList == null)
				return; // failed to read channel
			var interfaceModuleChannelConfigurationDataList = opcUaInterface.ReadData(interfaceModuleChannelConfigurationReferenceList);
			if (interfaceModuleChannelConfigurationDataList == null)
				return; // failed to read channel 
			var interfaceModuleChannelDynamicReferenceList = opcUaInterface.ReadReferences(opcUaInterface.GetChassisBrowsePathList(OpcUaInterface.PathId.InterfaceModuleChannelDynamic, moduleId, channelId));
			if (interfaceModuleChannelDynamicReferenceList == null)
				return; // failed to read channel
			var interfaceModuleChannelDynamicDataList = opcUaInterface.ReadData(interfaceModuleChannelDynamicReferenceList);
			if (interfaceModuleChannelDynamicDataList == null)
				return;  // failed to read channel

			// find the protocol
			var protocolIdx = interfaceModuleChannelConfigurationReferenceList.ToList().FindIndex(x => x.BrowseName.Name == "Protocol");
			var protocolName = string.Empty;
			AvailableProtocols protocolParameters = new AvailableProtocols();
			if (protocolIdx > -1)
			{
				protocolName = interfaceModuleChannelConfigurationDataList[protocolIdx].Value.ToString();

				// from the rtu the protocol name is within parenthesis
				if (!string.IsNullOrEmpty(protocolName))
				{
					var protocolValueId = Convert.ToInt32( protocolName.Split(' ')[0]) ;
					string protocolValueName = AvailableConfigurationService.AllProtocols.FirstOrDefault(x => x.Value == protocolValueId).Key;
					protocolParameters = availableProtocols.Where(x => x.Name.ToLower() == protocolValueName.ToLower()).FirstOrDefault();
				}
			}

			if (interfaceModuleChannelConfigurationReferenceList != null)
			{
				for (int i = 0; i < interfaceModuleChannelConfigurationReferenceList.Count; i++)
				{
					var interfaceModuleChannelConfiguration = interfaceModuleChannelConfigurationReferenceList[i];
					var interfaceModuleChannelConfigurationData = interfaceModuleChannelConfigurationDataList[i];
					// find the parameter in the module to get the metadata (data type, min, max, ...)
					Parameter parameterConfiguration = null;
					if (protocolParameters != null && protocolParameters.protocolConfiguration != null)
					{
						parameterConfiguration = protocolParameters.protocolConfiguration.Where(x => x.Value.parameter == interfaceModuleChannelConfiguration.BrowseName.Name).FirstOrDefault().Value;
						if(parameterConfiguration == null)
							parameterConfiguration = protocolParameters.protocolConfiguration.Where(x => x.Value.parameter.IndexOf(interfaceModuleChannelConfiguration.BrowseName.Name) == 0).FirstOrDefault().Value;

					}
               if (parameterConfiguration != null) {

					string newValue = interfaceModuleChannelConfigurationData.Value != null ? this.ConvertRtuValue(interfaceModuleChannelConfigurationData.Value) : "";
					var newAvailableCommands = parameterConfiguration != null ? parameterConfiguration.availableCommands : "";

					if(parameterConfiguration.parameter == "Protocol")
					{
						newAvailableCommands = protocolAvailableCommands;
					}

					// for dropdowns we need to parse the value to extract the first digits
					if (parameterConfiguration != null
					&& parameterConfiguration.displayFormat == "dropdown"
					&& parameterConfiguration.parameter != "ModbusMap")
					{
						newValue = newValue.Split(' ')[0];
					}
					if (parameterConfiguration != null)
					{
						newInterfaceChannel.channelConfiguration.Add(parameterConfiguration.opcstartNodeID + (uint)moduleId * 8 + ((uint)channelId - 1), new Parameter(ConfigurationClass.CONFIG,
												interfaceModuleChannelConfiguration.BrowseName.Name,
												parameterConfiguration != null ? parameterConfiguration.description : interfaceModuleChannelConfiguration.BrowseName.Name,
												parameterConfiguration != null ? parameterConfiguration.dataType : "string",
												parameterConfiguration != null ? parameterConfiguration.displayFormat : "",
												parameterConfiguration != null ? parameterConfiguration.minimumValue : null,
												parameterConfiguration != null ? parameterConfiguration.maximumValue : null,
												newValue,
												interfaceModuleChannelConfigurationData.StatusCode.Code,
												interfaceModuleChannelConfigurationData.ServerTimestamp,
												newValue,
												interfaceModuleChannelConfigurationData.StatusCode.Code,
												interfaceModuleChannelConfigurationData.ServerTimestamp,
												availableCommands: newAvailableCommands,
												opcStartNodeId: parameterConfiguration.opcstartNodeID,
												identifier: parameterConfiguration.opcstartNodeID + (uint)moduleId * 8 + ((uint)channelId - 1),
												availableCommandsOutputMatches: parameterConfiguration.availableCommandsOutputMatches,
												variableAlarmNumber: parameterConfiguration.variableAlarmNumber,
												datatypeLength: parameterConfiguration.datatypeLength
												));
                  }
					}
				}
			}

			if (interfaceModuleChannelDynamicReferenceList != null)
			{
				for (int i = 0; i < interfaceModuleChannelDynamicReferenceList.Count; i++)
				{
					var interfaceModuleChannelConfiguration = interfaceModuleChannelDynamicReferenceList[i];

					Softing.Opc.Ua.Toolkit.DataValue interfaceModuleChannelConfigurationData = new Softing.Opc.Ua.Toolkit.DataValue();

					if (interfaceModuleChannelDynamicDataList != null)
					{
						interfaceModuleChannelConfigurationData = interfaceModuleChannelDynamicDataList[i];
					}

					// find the parameter in the module to get the metadata (data type, min, max, ...)
					Parameter parameterConfiguration = null;
					if (protocolParameters != null && protocolParameters.protocolConfiguration != null)
					{
						//parameterConfiguration = protocolParameters.protocolConfiguration.Where(x => x.parameter == interfaceModuleChannelConfiguration.BrowseName.Name).FirstOrDefault();
						parameterConfiguration = protocolParameters.protocolConfiguration.Where(x => x.Value.parameter == interfaceModuleChannelConfiguration.BrowseName.Name).FirstOrDefault().Value;

					}

					string newValue = interfaceModuleChannelConfigurationData.Value != null ? this.ConvertRtuValue(interfaceModuleChannelConfigurationData.Value) : "";
					var newAvailableCommands = parameterConfiguration != null ? parameterConfiguration.availableCommands : "";

					// for dropdowns we need to parse the value to extract the first digits
					if (parameterConfiguration != null
					&& parameterConfiguration.displayFormat == "dropdown"
					&& parameterConfiguration.parameter != "ModbusMap")
					{
						newValue = newValue.Split(' ')[0];
					}
					if (parameterConfiguration != null)
					{
						newInterfaceChannel.channelConfiguration.Add(parameterConfiguration.opcstartNodeID + (uint)moduleId * 8 + ((uint)channelId - 1), new Parameter(parameterConfiguration != null ? parameterConfiguration.configClass : ConfigurationClass.DYNAMIC,
												interfaceModuleChannelConfiguration.BrowseName.Name,
												parameterConfiguration != null ? parameterConfiguration.description : interfaceModuleChannelConfiguration.BrowseName.Name,
												parameterConfiguration != null ? parameterConfiguration.dataType : "string",
												parameterConfiguration != null ? parameterConfiguration.displayFormat : "",
												parameterConfiguration != null ? parameterConfiguration.minimumValue : null,
												parameterConfiguration != null ? parameterConfiguration.maximumValue : null,
												newValue,
												interfaceModuleChannelConfigurationData.StatusCode.Code,
												interfaceModuleChannelConfigurationData.ServerTimestamp,
												newValue,
												interfaceModuleChannelConfigurationData.StatusCode.Code,
												interfaceModuleChannelConfigurationData.ServerTimestamp,
												availableCommands: newAvailableCommands,
												opcStartNodeId: parameterConfiguration.opcstartNodeID,
												identifier: parameterConfiguration.opcstartNodeID + (uint)moduleId * 8 + ((uint)channelId - 1),
												availableCommandsOutputMatches: parameterConfiguration.availableCommandsOutputMatches,
												variableAlarmNumber: parameterConfiguration.variableAlarmNumber,
												datatypeLength: parameterConfiguration.datatypeLength
												));
					}
				}
			}
		}

		[HttpPost]
		[Route("GetRTUCPUModule")]
		public ActionResult GetRTUCPUModule([FromBody] RTUConnection connectionInfo)
		{
			RTUConfigurationDO RTUConfiguration = new RTUConfigurationDO();

			// get the available modules
			var availableModuleConfiguration = new AvailableConfigurationService().GetAvailableConfiguration(connectionInfo.returnPoints, connectionInfo.filename);

			RTUCPUModuleDO cpuModule = new RTUCPUModuleDO();

			try
			{

				using (var opcUaInterface = new OpcUaInterface(connectionInfo))
				{
					// we want the CPU module
					var errMsg = getCPUModuleConfiguration(opcUaInterface, availableModuleConfiguration.modules, availableModuleConfiguration.protocols, cpuModule);
					if (errMsg != "")
					{
						this.OnError(errMsg);
					}
				}

				return JsonWithErrorMessages(cpuModule);
			}
			catch (Exception e)
			{
				this.OnError(e.Message);
				return this.JsonWithErrorMessages(cpuModule);
			}
		}

		[HttpPost]
		[Route("GetRTUInterfaceModule")]
		public ActionResult GetRTUInterfaceModule([FromBody] JObject data)  // (RTUConnection connectionParms, int moduleId)
		{
			RTUConnection connectionParms = data["connectionParms"].ToObject<RTUConnection>();
			int moduleId = data["moduleId"].ToObject<int>();

			RTUInterfaceModuleDO module = new RTUInterfaceModuleDO();
			try
			{
				// get the available modules
				var availableModuleConfiguration = new AvailableConfigurationService().GetAvailableConfiguration(connectionParms.returnPoints, connectionParms.filename);

				using (var opcUaInterface = new OpcUaInterface(connectionParms))
				{
					var errMsg = this.getInterfaceModuleConfiguration(opcUaInterface, availableModuleConfiguration.modules, availableModuleConfiguration.protocols, module, moduleId);
					if (errMsg != "")
					{
						this.OnError(errMsg);
					}
				}

				return JsonWithErrorMessages(module);
			}
			catch (Exception e)
			{
				this.OnError(e.Message);
				return this.JsonWithErrorMessages(module);
			}
		}

		// POST: api/RTUConfiguration
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT: api/RTUConfiguration/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE: api/ApiWithActions/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}

		// GET: api/RTUConfiguration/GetAvailableRtuxmls
		[HttpGet]
		[Route("GetAvailableRtuxmls")]
		public ActionResult GetAvailableRtuxmls()
		{
				string rtuXmlPath = ConfigurationManager.AppSettings["rtuxmlPath"].ToString();
				string[] names;

				try
				{
					string[] files = Directory.GetFiles(rtuXmlPath, "*.rtuxml");
					names = new string[files.Count()];
					int i = 0;
					foreach (string file in files)
					{
						names[i] = Path.GetFileName(file);
						i++;
					}
				}
				catch (Exception e)
				{
					this.OnError(e.Message);
					return this.JsonWithErrorMessages( null );
				}
				return JsonWithErrorMessages(names);
		}

		// GET: api/RTUConfiguration/GetXmlConfiguration
		[HttpGet]
		[Route("GetXmlConfiguration")]
		public ActionResult GetXmlConfiguration(string filename)
		{
			RTUConfigurationService RTUConfiguration = new RTUConfigurationService();
			RTUConfigurationDO configuration = RTUConfiguration.LoadRtuXmlConfiguration(filename);
			return JsonWithErrorMessages(configuration);
		}
	}
}
