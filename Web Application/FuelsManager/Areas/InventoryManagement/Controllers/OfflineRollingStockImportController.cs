
namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;
    using Opc.Ua;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;

    public class OfflineRollingStockImportController : FMBaseControllerEx
	 {
        public ActionResult OfflineRollingStockImportView(string logText = "")
        {
            var model = new OfflineRollingStockImportModel(this.Security.SiteGuid, logText);
            return this.View(model);
        }

        [HttpPost]
		  public ActionResult Import(HttpPostedFileBase file)
		  {
            try
            {
                var model = new OfflineRollingStockImportModel(this.Security.SiteGuid);
                model.logText = "Beginning import...";
                List<OPCPointValue> OPCPointValues = new List<OPCPointValue>();
                // Verify that the user selected a file
                if (file != null && file.ContentLength > 0)
                {
                    var ext = Path.GetExtension(file.FileName);
                    // Verify the file is json
                    if (ext != ".json")
                    {
                        model.logText = model.logText + "\nExpected json. Stopping import.";
                        return View("OfflineRollingStockImportView", model);
                    }
                    // get contents to string
                    string str = (new StreamReader(file.InputStream)).ReadToEnd();

                    // deserializes string into object
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    var deserializedJson = jss.Deserialize<dynamic>(str);

                    // Build list of OPCPointValues with the json
                    foreach (var site in deserializedJson)
                    {
                        foreach (var point in site["Points"])
                        {
                            foreach (var tag in point["Tags"])
                            {
                                OPCPointValue opcPointValue = new OPCPointValue();
                                opcPointValue.OPCNodeID = "ns=3;s=" + tag["Id"];
                                opcPointValue.OPCTimestamp = point["ChangeDate"];
                                opcPointValue.OPCValue = tag["Value"];
                                OPCPointValues.Add(opcPointValue);
                            }
                        }
                    }
                }
                model.logText = model.logText + "\nIdentified " + OPCPointValues.Count + " OPC tag value(s).";

                // Fetch Point Tags that have matching OpcUaNodeIds
                List<string> opcNodeIds = OPCPointValues.Select(OPCPointValue => OPCPointValue.OPCNodeID).ToList();
                Dictionary<String, Guid> tagGuidDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<String, Guid>>(x => x.EnumerateTagListByOpcUaNodeId(this.Security, opcNodeIds));
                List<OPCPointValue> matchedOPCPointValues = new List<OPCPointValue>();

                // Match the retrieved tags to the OPCPointValues
                foreach (OPCPointValue opcValue in OPCPointValues)
                {
                    if (tagGuidDictionary.TryGetValue(opcValue.OPCNodeID, out Guid tagGuid))
                    {
                        opcValue.tagGuid = tagGuid;
                        opcValue.pointValue = new PointValue();
                        opcValue.pointValue.PointValueIdentifier = new PointValueIdentifier(tagGuid, PointValueType.Tag, "");
                        matchedOPCPointValues.Add(opcValue);
                    }
                }
                model.logText = model.logText + "\nIdentified " + matchedOPCPointValues.Count + " matching tag(s).";

                // Retrieve point values for the point value identifiers
                var pointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, matchedOPCPointValues.Select(OPCPointValue => OPCPointValue.pointValue.PointValueIdentifier).ToList(), false));
                foreach (PointValue pointValue in pointValueList)
                {
                    foreach (OPCPointValue opcPointValue in matchedOPCPointValues)
                    {
                        if (pointValue.PointValueIdentifier == opcPointValue.pointValue.PointValueIdentifier)
                        {
                            opcPointValue.pointValue = pointValue;
                        }
                    }
                }

                // Process the values to the correct datatype and then submit to the point service manager for processing
                var outputPointValueList = new List<PointValue>();
                if (matchedOPCPointValues != null
                    && matchedOPCPointValues.Count > 0)
                {
                    foreach (OPCPointValue opcPointValue in matchedOPCPointValues)
                        try
                        {
                            if (ProcessValue(outputPointValueList, opcPointValue.pointValue, opcPointValue.OPCValue, 0, DateTimeOffset.Parse(opcPointValue.OPCTimestamp)))
                                model.logText = model.logText + "\n" + opcPointValue.OPCNodeID + " value and timestamp updated.";
                            else
                            {
                                model.logText = model.logText + "\n" + opcPointValue.OPCNodeID + " could not be processed.";
                            }
                        }
                        catch (Exception ex)
                        {
                            model.logText = model.logText + "\n" + opcPointValue.OPCNodeID + " " + ex.Message;
                        }

                    if (outputPointValueList.Count > 0)
                    {
                        FMChannelHelper.MakeCall<IPointServiceManager>(x => x.SetPointValueData(this.Security, outputPointValueList, false));
                    }
                }


                model.logText = model.logText + "\n" + outputPointValueList.Count + " value(s) updated.\nImport complete.";
                return View("OfflineRollingStockImportView", model);
            }
            catch (Exception e){
                var model = new OfflineRollingStockImportModel(this.Security.SiteGuid);
                model.logText = "The import encountered an exception!";
                model.logText = model.logText + "\n" + e.Message;
                return View("OfflineRollingStockImportView", model);
                  }
        }

        /// <summary>
        /// Applies a <paramref name="value"/> to a <paramref name="pointValue"/> and adds it to <paramref name="outputPointValueList"/>. The tag type must be OpcUa, not in a forced state, and the timestamp must be newer than the one currently saved.
        /// </summary>
        /// <param name="outputPointValueList"></param>
        /// <param name="pointValue"></param>
        /// <param name="value"></param>
        /// <param name="quality"></param>
        /// <param name="timeStamp"></param>
        /// <returns>True if the point value meets the criteria and is updated</returns>
        static private bool ProcessValue(List<PointValue> outputPointValueList, PointValue pointValue, object value, ushort quality, DateTimeOffset timeStamp)
        {
            // validations
            if (pointValue.OpcStatusCodeBits == StatusCodes.GoodLocalOverride)
                throw new Exception("value is forced.");
            if (timeStamp < pointValue.ServerTimeStamp)
                throw new Exception("import timestamp is older than server timestamp for value.");
            if (timeStamp > DateTimeOffset.UtcNow)
                throw new Exception("import timestamp is in the future.");
            if (pointValue.InputOutputType == PointTemplateTag.PointTagInputOutputType.OpcUa)
            {
                if (quality == 0)
                {
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
                            if (value == null)
                            {
                                (pointValue.Value as DeviceAlarmMapReference).CurrentValue = (uint?)Convert.ToInt32(value);
                            }
                            else
                            {
                                (pointValue.Value as DeviceAlarmMapReference).CurrentValue = (uint?)Convert.ToInt32(value);
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

                    pointValue.Status = StatusCodes.Good;
                }
                else
                {
                    pointValue.Value = null;
                    pointValue.Status = StatusCodes.Bad;
                }

                // Because this is an import, use the timestamp included in the file
                pointValue.ServerTimeStamp = timeStamp;
                pointValue.SourceTimeStamp = timeStamp;

                outputPointValueList.Add(pointValue);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class OPCPointValue
    {
        public string OPCNodeID { get; set; }
        public string OPCTimestamp { get; set; }
        public decimal OPCValue { get; set; }
        public Guid tagGuid { get; set; }
        public PointValue pointValue {get; set;}
	 }
}