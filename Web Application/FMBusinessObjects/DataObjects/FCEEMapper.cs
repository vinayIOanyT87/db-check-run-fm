
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Constants;
	using crypto;
    using FMBusinessObjects.LogClient;

    public sealed class FCEEMapper
	{
		static Dictionary<EdgeMessageType, string[]> messageTypeToTagIDs = new Dictionary<EdgeMessageType, string[]>();
		private static readonly FCEEMapper instance = new FCEEMapper();
		private Dictionary<string, Dictionary<EdgeMessageType, Dictionary<int, Dictionary<int, FCEETupleMapping>>>> meiDictionary = new Dictionary<string, Dictionary<EdgeMessageType, Dictionary<int, Dictionary<int, FCEETupleMapping>>>>();
		private Logger logger = new Logger("FCEEMapper");
		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static FCEEMapper()
		{
            messageTypeToTagIDs.Add(EdgeMessageType.Heartbeat, new string[] { "Counter" });
            messageTypeToTagIDs.Add(EdgeMessageType.SoftwareVersion, new string[] { "Software Version"
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
                                        , "Long Time" });

            messageTypeToTagIDs.Add(EdgeMessageType.DeviceStatus, new string[] { "Device Type"
                                        , "Device Status"});
            messageTypeToTagIDs.Add(EdgeMessageType.Enraf854TankGauge, new string[] {"Level Product"
                                        , "Temperature Product"
                                        , "Level Water"
                                        , "Gauge Position"
                                        , "Gauge Status" });
            messageTypeToTagIDs.Add(EdgeMessageType.Enraf854TankGaugeDensity, new string[] { "Density Product Observed"
                                        , "Temperature Density"});
            messageTypeToTagIDs.Add(EdgeMessageType.ModbusIntegerRegisterBlock, new string[] { "Level Product"
                                        ,   "Temperature Product"
                                        ,   "Level Water"
                                        ,   "Gauge Alarm"});
            messageTypeToTagIDs.Add(EdgeMessageType.GenericScalingPoint, new string[] { "Level Product", "Temperature Product", "Level Water"});
            messageTypeToTagIDs.Add(EdgeMessageType.ITTBarton3500ATG, new string[] { "Level Product"
                                        , "Temperature Product"
                                        , "Level Water"
                                        , "Density Product Observed"
                                        , "Pressure Bottom"
                                        , "Gauge Alarm" });
            messageTypeToTagIDs.Add(EdgeMessageType.VeederRootTLS350, new string[] { "Level Product"
                                    , "Temperature Product"
                                    , "Level Water"
                                    , "Volume Gross Observed"
                                    , "Volume Net Standard"
                                    , "Volume Water"
                                    , "Volume Gross Observed Remaining" });
            messageTypeToTagIDs.Add(EdgeMessageType.VeederRootSystemStatus, new string[] { "Gauge Alarm"});
            messageTypeToTagIDs.Add(EdgeMessageType.VeederRootLeakTest, new string[] { "Gauge Alarm"  });
            messageTypeToTagIDs.Add(EdgeMessageType.VeederRootSystemAlarms, new string[] { "Gauge Alarm"});
            messageTypeToTagIDs.Add(EdgeMessageType.VeederRootInventoryReport, new string[] { "Level Product"
                                        , "Temperature Product"
                                        , "Volume Gross Observed"
                                        , "Volume Net Standard"
                                        , "Level Water"
                                        , "Volume Gross Observed Remaining"
                                        , "Volume Water"});
            messageTypeToTagIDs.Add(EdgeMessageType.VeederRootInTankStatusReport, new string[] { "Gauge Alarm" });
            messageTypeToTagIDs.Add(EdgeMessageType.VeederRootLiquidSensorStatusReport, new string[] { "Sump Alarm"});
            messageTypeToTagIDs.Add(EdgeMessageType.ModbusInventory, new string[] {"Level Product"
                                        , "Temperature Product"
                                        , "Level Water"
                                        , "Gauge Position"
                                        , "Gauge Status"
                                        , "Water Sump"
                                        , "Volume Gross Observed"
                                        , "Volume Water" });
            messageTypeToTagIDs.Add(EdgeMessageType.ModbusDensityAndAlarm, new string[] { "Density Product Observed"
                                        , "Temperature Density"
                                        , "Trouble Info"
                                        , "Level Alarm"});
            messageTypeToTagIDs.Add(EdgeMessageType.ModbusFacilityStatus, new string[] { "Facility Status"});
            messageTypeToTagIDs.Add(EdgeMessageType.ModbusStorage, new string[] {  "Level Product"
                                        , "Temperature Product"
                                        , "Level Water"
                                        , "Gauge Position"
                                        , "Density Product Observed"});
            messageTypeToTagIDs.Add(EdgeMessageType.CommandStatus, new string[] { "Command Status"
                                        , "Command Schedule"});
            messageTypeToTagIDs.Add(EdgeMessageType.WAGOPLC, new string[] {  "Device Type"
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
                                        ,"Water Sump Volume"});
        }

		private FCEEMapper()
		{
		}

		static public string[] MessageTypeToTagIDs(EdgeMessageType msgType)
		{
			if (messageTypeToTagIDs.ContainsKey(msgType))
			{
                return messageTypeToTagIDs[msgType];

            }
            return null;

        }
        static public string[] MessageTypeToTagIDs(EDGEMESSAGETYPE mt)
        {
			EdgeMessageType msgType = (EdgeMessageType) mt;
            if (messageTypeToTagIDs.ContainsKey(msgType))
            {
                return messageTypeToTagIDs[msgType];

            }
            return null;

        }
        public static FCEEMapper Instance
		{
			get
			{
				return instance;
			}
		}

		public FCEETupleMapping GetMapping(SecurityClass security, string imei, EdgeMessageType edgeMsgType, int msgIndex, int? device = null)
		{
			FCEETupleMapping fceeMapping = null;
			try
			{
				Dictionary<EdgeMessageType, Dictionary<int, Dictionary<int, FCEETupleMapping>>> messageTypeDictionary;
				if (!meiDictionary.TryGetValue(imei, out messageTypeDictionary))
				{
					messageTypeDictionary = new Dictionary<EdgeMessageType, Dictionary<int, Dictionary<int, FCEETupleMapping>>>();
					meiDictionary.Add(imei, messageTypeDictionary);
				}

				Dictionary<int, Dictionary<int, FCEETupleMapping>> messageIndexDictionary;
				if (!messageTypeDictionary.TryGetValue(edgeMsgType, out messageIndexDictionary))
				{
					messageIndexDictionary = new Dictionary<int, Dictionary<int, FCEETupleMapping>>();
					messageTypeDictionary.Add(edgeMsgType, messageIndexDictionary);
				}

            Dictionary<int, FCEETupleMapping> messageDeviceDictionary = new Dictionary<int, FCEETupleMapping>();
            if (!messageIndexDictionary.TryGetValue(msgIndex, out messageDeviceDictionary))
				{
					messageDeviceDictionary = new Dictionary<int, FCEETupleMapping>();
					messageIndexDictionary.Add(msgIndex, messageDeviceDictionary);
				}


            var mapping = FMChannelHelper.MakeCall<IFCEEServiceManager, Tuple<string, Guid, string, Guid, long, int?>>(x => x.GetMapping(security, imei, (int)edgeMsgType, msgIndex, device));

				if (mapping != null)
				{

               if (!messageDeviceDictionary.TryGetValue(device ?? -1, out fceeMapping))
					{
						fceeMapping = new FCEETupleMapping(mapping);
						messageDeviceDictionary.Add(device ?? -1, fceeMapping);
					}
					else if (fceeMapping.PointGuid != mapping.Item4
					|| fceeMapping.RowVersion != mapping.Item5)
					{
						fceeMapping.SiteID = mapping.Item1;
						fceeMapping.SiteGuid = mapping.Item2;
						fceeMapping.PointID = mapping.Item3;
						fceeMapping.PointGuid = mapping.Item4;
						fceeMapping.RowVersion = mapping.Item5;
						fceeMapping.TagSelection = mapping.Item6;
						fceeMapping.PointValueIdentifierList.Clear();
					}

					if (fceeMapping.PointValueIdentifierList.Count == 0)
					{
						security.SiteID = fceeMapping.SiteID;
						security.SiteGuid = fceeMapping.SiteGuid;

						if (messageTypeToTagIDs.ContainsKey(edgeMsgType))
						{
							string []tagIDs = null;
							if (edgeMsgType == EdgeMessageType.GenericScalingPoint)
							{
								tagIDs=GetGenericScalingPointTags(security, fceeMapping);
							}
							else
							{
								tagIDs = messageTypeToTagIDs[edgeMsgType];
							}
							UpdatePointValueIdentifierList(security, fceeMapping, tagIDs);
						}
                    }
                }
				else
				{
					logger.Error("Invalid Mapping : " + imei + "." + edgeMsgType + "." + msgIndex + "." + device);
				}
			}
			catch (Exception e)
			{
				logger.Error("Get Mapping error : " + e.Message);
			}

			return fceeMapping;
		}

		void UpdatePointValueIdentifierList(SecurityClass security, FCEETupleMapping fceeMapping, string[] tagIDs)
		{
            foreach (var tagID in tagIDs)
            {
                var tagGuid = FMChannelHelper.MakeCall<IPointTags, Guid>(x => x.GetIdentityGuid(security, tagID, fceeMapping.PointGuid));
                if (tagGuid != null
                && tagGuid != Guid.Empty)
                {
                    fceeMapping.PointValueIdentifierList.Add(new PointValueIdentifier(tagGuid, PointValueType.Tag, ""));
                }
                else
                {
                    logger.Error("Invalid Point Tag : " + security.SiteID + "." + fceeMapping.PointID + "." + tagID);
                    fceeMapping.PointValueIdentifierList.Clear();
                    break;
                }
            }
        }

		string[] GetGenericScalingPointTags(SecurityClass security, FCEETupleMapping fceeMapping)
		{
            string[] tagIDs = new string[1] ;

			switch (fceeMapping.TagSelection)
			{
				case 1:
					tagIDs[0] = "Level Product";
					break;
				case 2:
					tagIDs[0] = "Temperature Product" ;
					break;
				case 3:
					tagIDs[0] = "Level Water" ;
					break;
				default:
					tagIDs[0] = "Level Product";
					break;
			}
			return tagIDs;

        }
    }
}
