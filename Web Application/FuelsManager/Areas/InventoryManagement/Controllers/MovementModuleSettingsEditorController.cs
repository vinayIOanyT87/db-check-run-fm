namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using FMBusinessObjects.Exceptions;

	using FMBusinessObjects.Constants;
	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;
	using System.ServiceModel;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;
	using System.Net.Sockets;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using System.Globalization;
	using FMPointCommon;
	using Opc.Ua;
    using System.Text;
    using System.Threading;
    using System.Security.Policy;

    public class MovementModuleSettingsEditorController : FMBaseControllerEx
	{

		private const bool isTemplatePoint = false;
		private const string movementSettingId = "Movement Settings";

		private const string TagLevelProduct = "Level Product";
		private const string TagLevelProductMaxOpLimit = "Level Product Max Op Limit";
		private const string TagLevelProductMinOpLimit = "Level Product Min Op Limit";

		private const string TagVolumeGrossObserved = "Volume Gross Observed";
		private const string TagVolumeGrossObservedAvailable = "Volume Gross Observed Available";
		private const string TagVolumeGrossObservedRemaining = "Volume Gross Observed Remaining";

		private const string TagVolumeNetStandard = "Volume Net Standard";
		private const string TagVolumeNetStandardAvailable = "Volume Net Standard Available";
		private const string TagVolumeNetStandardRemaining = "Volume Net Standard Remaining";

		/// <summary>
		/// Identifies the data dictionary keys needed for this item.
		/// </summary>
		/// <param name="model">The model to serialize</param>
		/// <returns>Returns a string of the model.</returns>
		[NonAction]
		public static string SerializeModel(MovementModuleSettingsEditorModel model)
		{
				return JsonConvert.SerializeObject(model);
		}

		/// <summary>
		/// This method will create a new movement point in the system. It will return the movement module
		/// settings editor model with the newly created movement information.
		/// </summary>
		/// <param name="movementName">The movement name to create.</param>
		/// <returns>Returns the movement module settings editor model.</returns>
		[HttpGet]
		public ActionResult CreateNewMovement(string movementName)
		{
			try
			{
				if (string.IsNullOrEmpty(movementName))
				{
					base.OnError(new Exception(this.GetTranslatedText("Movement Name cannot be blank.")));
					return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}

				var movementTypeGuid = FMChannelHelper.MakeCall<IApplicationStrings, Guid>(x => x.GetIdentityGuid(this.Security, STRING_TYPE.POINT_TEMPLATE_TYPE, "Movement"));

				if (movementTypeGuid == Guid.Empty)
				{

					base.OnError(new Exception(this.GetTranslatedText("Could not find Point Type Movement.")));
					return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}


				var pointTemplateCollection = FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(x => x.EnumerateByType(this.Security, movementTypeGuid));

				if(pointTemplateCollection == null || pointTemplateCollection.Count == 0)
				{
					base.OnError(new Exception(this.GetTranslatedText("Error retrieving point templates.")));
					return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}

				PointTemplate movementTemplate = null;

				foreach(PointTemplate pointTemplate in pointTemplateCollection)
				{
					if(pointTemplate.ID.ToUpper().Equals("STANDARD MOVEMENT"))
					{
						movementTemplate = pointTemplate;
					}
				}

				if (movementTemplate == null)
				{

					base.OnError(new Exception(this.GetTranslatedText("Could not find Standard Movement Template.")));
					return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}

				Guid movementGuid;

				try
				{
					// Create a new movement.
					movementGuid = FMChannelHelper.MakeCall<IPoints, Guid>(x => x.CreatePoint(this.Security, movementName, movementTemplate));
				}
				catch(Exception ex)
				{
					string msg = this.GetTranslatedText("Movement Name '" + movementName + "' already exists.");
					FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + " " + ex.Message, FMEventLogEntryType.Error));

					base.OnError(new Exception(msg));
					return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}

				// Retrieve the point property Guid.
				try
				{
					var pointPropertyGuid = FMChannelHelper.MakeCall<IPointProperties, Guid>(x => x.GetPointPropertyGuid(this.Security, movementGuid, movementSettingId));

					// Get the new movement module settings editor data.
					MovementModuleSettingsEditorModel model = this.GetMovementModuleSettingsModel("", isTemplatePoint, movementGuid, pointPropertyGuid);
					model.IsLaunchedFromSummary = true;
					return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
				}
				catch(Exception ex)
				{
					string msg = this.GetTranslatedText("Error retrieving movement point or movement point property.");
					FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + " " + ex.Message, FMEventLogEntryType.Error));

					base.OnError(new Exception(msg));
					return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception except)
			{
				string msg = this.GetTranslatedText("Error creating a new movement.");
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + " " + except.Message, FMEventLogEntryType.Error));

				base.OnError(new Exception(msg));
				return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

        [HttpGet]
        public ActionResult MovementModuleDefaultName() {
            var movementID = FMChannelHelper.MakeCall<ISites, string>(x => x.GetMovementID(this.Security));
            return base.JsonWithErrorMessages(movementID, JsonRequestBehavior.AllowGet);
        }

		/// <summary>
		/// This method retrieves the movement node editor model based on the point and point property GUID.
		/// </summary>
		/// <param name="isTemplatePoint">Determines if a template point (true) or a point (false).</param>
		/// <param name="pointGuid">The movement point GUID</param>
		/// <param name="pointPropertyGuid">The movement point property GUID</param>
		/// <returns>Returns the Movement Node Editor model.</returns>
		[HttpGet]
		public ActionResult MovementModuleSettingsEditor(string newId, bool isTemplatePoint, Guid pointGuid, Guid pointPropertyGuid, string caller)
		{
			if(string.IsNullOrEmpty(caller) == false && caller.Equals("OperateCreateNew"))
			{
				MovementModuleSettingsEditorModel emptyModel = this.CreateEmptyModel();
				emptyModel.IsLaunchedFromSummary = true;
				return base.PartialViewWithErrorMessages("MovementModuleSettingsEditor", emptyModel, JsonRequestBehavior.AllowGet);
			}

			if(string.IsNullOrEmpty(caller) == false && caller.Equals("OperateSetMovementSettings"))
			{
				try
				{
					pointPropertyGuid = FMChannelHelper.MakeCall<IPointProperties, Guid>(x => x.GetPointPropertyGuid(this.Security, pointGuid, movementSettingId));
				}
				catch(Exception ex)
				{
					string msg = "Error Point Property.";
					base.OnError(new Exception(this.GetTranslatedText(msg)));
					FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + " " + ex.Message, FMEventLogEntryType.Error));

					return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}

				if(pointPropertyGuid == null || pointPropertyGuid == Guid.Empty)
				{
					string msg = "Could not find Point Property.";
					base.OnError(new Exception(this.GetTranslatedText(msg)));
					FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg, FMEventLogEntryType.Error));

					return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}
			}

			try
			{
				MovementModuleSettingsEditorModel model = this.GetMovementModuleSettingsModel(newId, isTemplatePoint, pointGuid, pointPropertyGuid);
				
				model.IsLaunchedFromSummary = (caller.Contains("Operate"));

				return base.PartialViewWithErrorMessages("MovementModuleSettingsEditor", model, JsonRequestBehavior.AllowGet);
			}
			catch
			{
				base.OnError(new Exception(this.GetTranslatedText("Error Getting Movement Module Settings")));
				return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// This method will return the movement module settings editor model based on the point Guid,
		/// and point property Guid.
		/// </summary>
		/// <param name="isTemplatePoint">Flag to check for template point.</param>
		/// <param name="pointGuid">The point Guid used to retrieve the data.</param>
		/// <param name="pointPropertyGuid">The point property Guid used to retrieve the data.</param>
		/// <returns>Returns the movement module settings editor model.</returns>
		private MovementModuleSettingsEditorModel GetMovementModuleSettingsModel(string newId, bool isTemplatePoint, Guid pointGuid, Guid pointPropertyGuid)
		{
			MovementModuleSettings movementModuleSettings = null;
			BasePoint basePoint = null;
			string pointPropertyID = string.Empty;
         var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

            if (isTemplatePoint)
			{
				basePoint = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.GetPointTemplateBaseData(this.Security, pointGuid));
				var pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, pointPropertyGuid));
				pointPropertyID = pointTemplateProperty.ID;
				movementModuleSettings = pointTemplateProperty.Value as MovementModuleSettings;
			}
			else
			{
				basePoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, pointGuid));
				var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, pointPropertyGuid));
				pointPropertyID = pointProperty.ID;
				movementModuleSettings = pointProperty.Value as MovementModuleSettings;
			}

            //get the movement's status tag value
            var pointTags = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>> (x => x.EnumerateByPointGuid(this.Security, pointGuid));
			var movementIsActive = false;
            foreach (var pointTag in pointTags.Values)
			{
				if(pointTag.ID == "Status")
				{ 
					movementIsActive = pointTag.Value.ToString() == "Active";
				}
			}

            if (movementModuleSettings == null)
			{
				throw new InvalidOperationException("Movement Node Table not found in the Movement Point.");
			}

			string localPlannedStartTime = string.Empty;
			if (movementModuleSettings.PlannedStartDateTime.HasValue)
			{
                TimeZoneInfo sitesTimezone = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
                DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(TimeZoneInfo.ConvertTimeToUtc(movementModuleSettings.PlannedStartDateTime.Value.LocalDateTime), sitesTimezone);
                localPlannedStartTime = localDateTime.ToString();
			}

			var model = new MovementModuleSettingsEditorModel
			{
				NewId = newId,
				PointId = basePoint.ID,
				PointGuid = pointGuid,
				PointPropertyGuid = pointPropertyGuid,
				PointPropertyId = pointPropertyID,
				IncludeHandgaugeValues = movementModuleSettings.HandGaugeData,
				InterlockSourceDestinationSetpoints = movementModuleSettings.InterlockSourceDestinationSetpoints,
				DeleteAfterCompletion = movementModuleSettings.DeleteAfterCompletion,
				DeleteAfterStop = movementModuleSettings.DeleteAfterStop,
				OrderNumber = movementModuleSettings.OrderNumber,
				Comment = movementModuleSettings.Comment,
				SendToAccounting = movementModuleSettings.SendToAccounting,
				UseControlTagStartStop = movementModuleSettings.UseControlTagStartStop,
				SelectedControlTagGuid = movementModuleSettings.ControlTagGuid,
				StopHaltBasedOnZeroFlow = movementModuleSettings.StopHaltBasedOnZeroFlow,
				StartTimeBasedOnNonZeroFlow = movementModuleSettings.StartTimeBasedOnNonZeroFlow,
				SetPendingStatus = movementModuleSettings.SetPendingStatus,
				PlannedStartDateTime = localPlannedStartTime,
				Type = movementModuleSettings.Type,
				IsActive = movementIsActive
            };

			if (movementModuleSettings.ZeroFlowHoldOffTime != null)
			{
				model.ZeroFlowHoldOffTime = movementModuleSettings.ZeroFlowHoldOffTime;
			}

			// Set the date and number formats based on the site;
			this.SetDateAndNumberFormats(model);

			var nodeModelList = new List<MovementNodeModel>();
			var pointValueIdentifierList = new List<PointValueIdentifier>();

			foreach (var movementNodeData in movementModuleSettings.MovementNodeDataList)
			{
				pointValueIdentifierList.Add(new PointValueIdentifier(movementNodeData.MovementNodeGuid, PointValueType.Point, "PointId"));
			}

			var pointValueDictionary = FMChannelHelper.MakeCall<IPoints, Dictionary<PointValueIdentifier, PointValue>>(x => x.EnumerateByPointValueIdentifierList(this.Security, pointValueIdentifierList));

			foreach (var movementNodeData in movementModuleSettings.MovementNodeDataList)
			{
				var pointIdValueIdentifer = new PointValueIdentifier(movementNodeData.MovementNodeGuid, PointValueType.Point, "PointId");

				var movementNodeId = string.Empty;

				if (pointValueDictionary.ContainsKey(pointIdValueIdentifer))
				{
					movementNodeId = pointValueDictionary[pointIdValueIdentifer].Value as string;
				}

				var movementNodeModel = new MovementNodeModel
				{
					MovementNodeId			= movementNodeId, 
					MovementNodeGuid		= movementNodeData.MovementNodeGuid, 
					TransferTarget			= ConvertDoubleValueToString(movementNodeData.TransferTarget, movementNodeData.TransferMode, movementNodeData.Units), 
					TransferDirection		= movementNodeData.TransferDirection, 
					TransferMode			= movementNodeData.TransferMode, 
					IndividualNodeControl	= movementNodeData.IndividualNodeControl,
					Units						= movementNodeData.Units,
					IntLevelUnits			= movementNodeData.IntLevelUnits,
					IntVolumeUnits			= movementNodeData.IntVolumeUnits,
					ModuleType				= movementNodeData.ModuleType,
					NodeTransferVolumeMode  = movementNodeData.NodeTransferVolumeMode
				};

				nodeModelList.Add(movementNodeModel);
			}

			model.MovementNodeModelList = nodeModelList;

			return model;
		}

		[HttpGet]
		public ActionResult GetMovementNodes(bool isTemplatePoint)
		{
			_ = isTemplatePoint;
			var movementTankTransferModuleNodeList = new List<MovementNodeModel>();
			var movementVolumeTransferModuleNodeList = new List<MovementNodeModel>();
			var movementNodeTransferModuleNodeList = new List<MovementNodeModel>();

			string[] tankModuleTypeNames = new string[] { "TankTransfer.FMTankTransfer", "StandardTankCalculator.FMStandardTankCalculator" };
			string[] volumenModuleTypeNames = new string[] { "VolumeTransfer.FMVolumeTransfer" };
			string[] nodeModuleTypeNames = new string[] { "NodeTransfer.FMNodeTransfer" };

			DateTime startTime = DateTime.Now;
			int tankPoints = 0, volumePoints = 0, nodePoints = 0; 

			try
			{
				Thread tankPointsThread = new Thread(() => GetMovementNodesByModuleType(movementTankTransferModuleNodeList, tankModuleTypeNames, ref tankPoints, NodeModuleType.StandardTank));
				tankPointsThread.Start();
				Thread volumePointsThread = new Thread(() => GetMovementNodesByModuleType(movementVolumeTransferModuleNodeList, volumenModuleTypeNames, ref volumePoints, NodeModuleType.StandardVolume));
				volumePointsThread.Start();
				Thread nodePointsThread = new Thread(() => GetMovementNodesByModuleType(movementNodeTransferModuleNodeList, nodeModuleTypeNames, ref nodePoints, NodeModuleType.StandardNode));
				nodePointsThread.Start();

				tankPointsThread.Join();
				volumePointsThread.Join();
				nodePointsThread.Join();

				DateTime endTime = DateTime.Now;
				TimeSpan timespan = endTime - startTime;

				StringBuilder sb = new StringBuilder();
				sb.Append("Time to fetch ");
				sb.Append(tankPoints);
				sb.Append(" Tank points and ");
				sb.Append(volumePoints);
				sb.Append(" Volume points is: ");
				sb.Append(nodePoints);
				sb.Append(" Node points is: ");
				sb.Append(timespan.TotalSeconds);
				sb.Append(" secs.");
				System.Diagnostics.Debug.Write(sb.ToString());

				List<MovementNodeModel> movementNodeModelList = new List<MovementNodeModel>(movementTankTransferModuleNodeList);
				movementNodeModelList.AddRange(movementVolumeTransferModuleNodeList);
				movementNodeModelList.AddRange(movementNodeTransferModuleNodeList);
				movementNodeModelList.Sort();
				return this.JsonWithErrorMessages(movementNodeModelList, JsonRequestBehavior.AllowGet);
			}
			catch(Exception except)
            {
				string msg = this.GetTranslatedText("Movement|Error Getting Movement Nodes");
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + " " + except.Message, FMEventLogEntryType.Error));

				base.OnError(new Exception(msg));
				return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// This method will retrieve the Movement Tickets (reports) and returns the list
		/// to the UI.
		/// </summary>
		/// <returns>Returns a list of movement ticket models.</returns>
		[HttpGet]
		public ActionResult GetMovementTickets()
		{
			var modelList = new List<MovementTicketModel>();

			var model = new MovementTicketModel
			{
				TicketName = "--Select--",
				TicketValue = "0"
			};

			modelList.Add(model);

			try
			{
				var reportServiceRequest = new ReportConfigurationDetailSR
				{
					CurrentSiteGuid = this.Security.SiteGuid,
					RequestType = ReportConfigurationDetailSR.RequestTypes.NONE
				};

				var reportDetailDoList = FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor, ReportConfigurationDetailListDO>(x => x.GetAll(reportServiceRequest));

				if(reportDetailDoList != null && reportDetailDoList.ReportDetailDOList != null && reportDetailDoList.ReportDetailDOList.Count > 0)
				{
					foreach(ReportConfigurationDetailDO reportDetailDo in reportDetailDoList.ReportDetailDOList)
					{
						model = new MovementTicketModel
						{
							TicketName = reportDetailDo.ReportName,
							TicketValue = reportDetailDo.ReportGuid.ToString()
						};

						modelList.Add(model);
					}
				}

				return this.JsonWithErrorMessages(modelList, JsonRequestBehavior.AllowGet);
			}
			catch(Exception ex)
			{
				string msg = "Error retrieving Ticket (report) names.";
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + " " + ex.Message, FMEventLogEntryType.Error));

				base.OnError(new Exception(this.GetTranslatedText(msg)));
				return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// This method will retrieve the Movement printers and returns the list
		/// to the UI.
		/// </summary>
		/// <returns>Returns a list of movement printer models.</returns>
		[HttpGet]
		public ActionResult GetMovementPrinters()
		{
			var modelList = new List<MovementPrinterModel>();

			var model = new MovementPrinterModel
			{
				PrinterName = "--Select--",
				PrinterValue = "0"
			};

			modelList.Add(model);
			int printerIndex = 1;

			try
			{
				string[] InstalledPrinters = ReportServicePrintService.EnumeratePrinters("Movement Control");

				foreach (string printer in InstalledPrinters)
				{
					model = new MovementPrinterModel
						{
								PrinterName = printer,
								PrinterValue = printerIndex.ToString()
						};

					modelList.Add(model);
					printerIndex++;
				}
			}
			catch (SocketException socketExcept)
			{
				if (socketExcept.ErrorCode != 10061)
				{
					string msg = "Error retrieving Printers.";
					FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + " " + socketExcept.Message, FMEventLogEntryType.Error));

					base.OnError(new Exception(this.GetTranslatedText(msg)));
					return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}
			}

			return this.JsonWithErrorMessages(modelList, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// This method will retrieve the Movement Control Points and returns the list
		/// to the UI.
		/// </summary>
		/// <returns>Returns a list of movement control point models.</returns>
		[HttpGet]
		public ActionResult GetMovementControlPoints(string movementIdentity)
		{
			var modelList = new List<MovementControlTagsModel>();

			var selectControlTagsModel = new MovementControlTagsModel
			{
				ControlTagName = "--Select--",
				ControlTagValue = Guid.Empty.ToString()
			};

			modelList.Add(selectControlTagsModel);

			string[] movementControlModuleTypeNames = new string[] { "MovementControl.FMMovementControl" };

			var pointTemplateGuidList = FMChannelHelper.MakeCall<IModules, List<Guid>>(x => x.EnumeratePointTemplatesByAllModuleTypeNames(this.Security, movementControlModuleTypeNames));

			if (!isTemplatePoint
			&& pointTemplateGuidList.Count > 0)
			{

				var points = FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.EnumerateByPointTemplateGuids(this.Security, pointTemplateGuidList.ToArray()));

				foreach (var point in points)
				{
					var movementIdentityTag = point.Tags.Values.FirstOrDefault(x => x.WellKnownIdentityGuid == Guids.MovementControlIdentifier);

					if (movementIdentityTag == null)
					{
						continue;
					}

					if (movementIdentityTag.Value == null
					|| (movementIdentityTag.Value is string
					&& (string.IsNullOrEmpty(movementIdentityTag.Value as string)
					|| movementIdentityTag.Value as string == movementIdentity)))
					{
						var model = new MovementControlTagsModel
						{
							ControlTagName = point.ID,
							ControlTagValue = point.PointGuid.ToString()
						};


						modelList.Add(model);
					}
				}
			}

			return this.JsonWithErrorMessages(modelList, JsonRequestBehavior.AllowGet);
		}


		/// <summary>
		/// This method will save the movement node property.
		/// </summary>
		/// <param name="model">The movement node editor model that contains the data.</param>
		/// <returns>Returns a JSON result.</returns>
		[HttpPost]
		public ActionResult SaveMovementModuleSettings(string movementModuleSettingsEditorModel)
		{
			string optionalExMsg = string.Empty;
			try
			{
				var jsonSerializerSettings = new JsonSerializerSettings
				{
					MissingMemberHandling = MissingMemberHandling.Ignore
				};

				var model = JsonConvert.DeserializeObject<MovementModuleSettingsEditorModel>(movementModuleSettingsEditorModel, jsonSerializerSettings);

				List<Guid> movementStatusWkgList = new List<Guid> { Guids.MovementStatusGuid };
				List<Guid> movementCommandWkgList = new List<Guid> { Guids.MovementCommandGuid };

				BasePoint basePoint = null;
				MovementStatus? movementStatus = null;
				MovementModuleSettings moduleSettings;
				PointTemplateProperty pointTemplateProperty = null;
				PointProperty pointProperty = null;

				if (model.IsTemplatePoint)
				{
					basePoint = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.GetPointTemplateBaseData(this.Security, model.PointGuid));
					pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, model.PointPropertyGuid));
					moduleSettings = pointTemplateProperty.Value as MovementModuleSettings;
				}
				else
				{
					basePoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, model.PointGuid));
					List<PointValueIdentifier> movementStatusIdentifiers = FMChannelHelper.MakeCall<IPointTags, List<PointValueIdentifier>>(x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.Security, new List<Guid> { model.PointGuid }, movementStatusWkgList));
					if (movementStatusIdentifiers != null)
					{
						List<PointValue> movementStatusPointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, movementStatusIdentifiers, false));
						if (movementStatusPointValues?[0] != null)
						{
							optionalExMsg = model.PointId + " has already been deleted.";

							if (movementStatusPointValues[0].Value == null) throw new Exception(optionalExMsg);

							// We expect there to be exactly one PointValue (1 point x 1 tag)
							movementStatus = (MovementStatus)movementStatusPointValues[0].Value;
						}
					}

					pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, model.PointPropertyGuid));
					moduleSettings = pointProperty.Value as MovementModuleSettings;
				}

				if (this.ModelState.IsValid)
				{
					var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

					moduleSettings.MovementNodeDataList.Clear();

					foreach (var nodeModel in model.MovementNodeModelList)
					{
						var nodeTable = new MovementNodeData
						{
							MovementNodeGuid = nodeModel.MovementNodeGuid
							, TransferTarget = ConvertStringValueToDouble(nodeModel.TransferTarget, nodeModel.TransferMode, nodeModel.Units)
							, TransferDirection = nodeModel.TransferDirection
							, TransferMode = nodeModel.TransferMode
							, IndividualNodeControl = nodeModel.IndividualNodeControl
							, Units = nodeModel.Units
							, IntLevelUnits = nodeModel.IntLevelUnits
							, IntVolumeUnits = nodeModel.IntVolumeUnits
							, ModuleType = nodeModel.ModuleType
							, NodeTransferVolumeMode = nodeModel.NodeTransferVolumeMode
						};

						moduleSettings.MovementNodeDataList.Add(nodeTable);
					}

					if (!model.IsTemplatePoint)
					{
						var wellKnownTagGuidList = new Guid[] {
							Guids.MovementControlIdentifier
						};

						// Clear the MovementIdentity of the former Movement Control Point
						if (moduleSettings.ControlTagGuid != model.SelectedControlTagGuid
						&& moduleSettings.ControlTagGuid != Guid.Empty)
						{

							var pointGuidList = new List<Guid>
							{
								moduleSettings.ControlTagGuid
							};

							var pointValueIdentifierList = FMChannelHelper.MakeCall<IPointTags, List<PointValueIdentifier>>(x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.Security, pointGuidList, wellKnownTagGuidList.ToList()));
							var pointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, pointValueIdentifierList, false));

							pointValueList[0].Value = null;
							pointValueList[0].Status = StatusCodes.Good;
							pointValueList[0].ServerTimeStamp = DateTimeOffset.UtcNow;
							pointValueList[0].SourceTimeStamp = DateTimeOffset.UtcNow;

							SetPointValues(this.Security, site, pointValueList);
						}

						// Set the MovementIdentity of the new Movement Control Point
						if (model.SelectedControlTagGuid != Guid.Empty)
						{
							var pointGuidList = new List<Guid>
							{
								model.SelectedControlTagGuid
							};

							pointGuidList[0] = model.SelectedControlTagGuid;

							var pointValueIdentifierList = FMChannelHelper.MakeCall<IPointTags, List<PointValueIdentifier>>(x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.Security, pointGuidList, wellKnownTagGuidList.ToList()));
							var pointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, pointValueIdentifierList, false));

							pointValueList[0].Value = model.PointGuid.ToString();
							pointValueList[0].Status = StatusCodes.Good;
							pointValueList[0].ServerTimeStamp = DateTimeOffset.UtcNow;
							pointValueList[0].SourceTimeStamp = DateTimeOffset.UtcNow;

							SetPointValues(this.Security, site, pointValueList);
						}
					}

					DateTimeOffset? plannedStartTime = null;
					if (!string.IsNullOrEmpty(model.PlannedStartDateTime))
					{

						DateTime localTime1 = DateTime.Parse(model.PlannedStartDateTime);
						TimeZoneInfo sitesTimezone = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
						double systemTimezoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now).TotalMinutes;
						double timezoneOffset = sitesTimezone.GetUtcOffset(DateTimeOffset.Now).TotalMinutes;

                  // Use the difference in offset between the site time and system time to get the server timezone
                  localTime1 = localTime1.AddMinutes(-(timezoneOffset - systemTimezoneOffset));
						plannedStartTime = DateTime.SpecifyKind(localTime1, DateTimeKind.Local);
					}

					moduleSettings.HandGaugeData								= model.IncludeHandgaugeValues;
					moduleSettings.InterlockSourceDestinationSetpoints	= model.InterlockSourceDestinationSetpoints;
					moduleSettings.Type											= model.Type;
					moduleSettings.DeleteAfterCompletion					= model.DeleteAfterCompletion;
					moduleSettings.DeleteAfterStop							= model.DeleteAfterStop;
					moduleSettings.OrderNumber									= model.OrderNumber;
					moduleSettings.Comment										= model.Comment;
					moduleSettings.SendToAccounting							= model.SendToAccounting;
					moduleSettings.UseControlTagStartStop					= model.UseControlTagStartStop;
					moduleSettings.ControlTagGuid								= model.SelectedControlTagGuid;
					moduleSettings.StopHaltBasedOnZeroFlow					= model.StopHaltBasedOnZeroFlow;
					moduleSettings.StartTimeBasedOnNonZeroFlow			= model.StartTimeBasedOnNonZeroFlow;
					moduleSettings.ZeroFlowHoldOffTime						= model.ZeroFlowHoldOffTime;
					moduleSettings.SetPendingStatus							= model.SetPendingStatus;
					moduleSettings.PlannedStartDateTime						= plannedStartTime;

					if (model.IsTemplatePoint)
					{
						FMChannelHelper.MakeCall<IPointTemplateProperties>(x => x.ModifyPointTemplatePropertyValue(this.Security, pointTemplateProperty));
					}
					else
					{
						// Update the point Value to the Point Service Manager and/or OPC UA.
						SetPointPropertyValue(this.Security, site, pointProperty);

						// Now that we updated the point, evaluate if we need to enable or disable it.
						// this only applies if the status currently is "Disabled" or "Inactive".  Don't touch it otherwise
						if (movementStatus.HasValue)
						{
							switch (movementStatus.Value)
							{
								case MovementStatus.Inactive:
								case MovementStatus.Disabled:
									// get the command tag for the movement, in case we need to send a Stop or a Disable command
									List<PointValue> movementCommandPointValues = null;
									List<PointValueIdentifier> movementCommandIdentifiers = FMChannelHelper.MakeCall<IPointTags, List<PointValueIdentifier>>(x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.Security, new List<Guid> { model.PointGuid }, movementCommandWkgList));
									if (movementCommandIdentifiers != null)
									{
										movementCommandPointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, movementCommandIdentifiers, false));
									}

									if (movementCommandPointValues?[0]  != null)
									{
										// We expect there to be only one point value (1 point x 1 tag)
										PointValue movementCommandPointValue = movementCommandPointValues[0];

										// Send a command if we need to change the state only
										List<string> interlockedMovements = FMChannelHelper.MakeCall<IMovementService, List<string>>(x => x.CheckForActiveInterlockedMovements(this.Security, model.PointGuid));
										List<PointValue> movementCommandValueList = new List<PointValue>();
										if (interlockedMovements.Count > 0 && movementStatus.Value == MovementStatus.Inactive)
										{
											movementCommandPointValue.Value = MovementCommand.Disable;
											movementCommandPointValue.Status = StatusCodes.Good;
											movementCommandPointValue.SourceTimeStamp = DateTimeOffset.UtcNow;
											movementCommandPointValue.ServerTimeStamp = DateTimeOffset.UtcNow;
											movementCommandValueList.Add(movementCommandPointValue);
											SetPointValues(this.Security, site, movementCommandValueList);

										}
										else if (interlockedMovements.Count == 0 && movementStatus.Value == MovementStatus.Disabled)
										{
											movementCommandPointValue.Value = MovementCommand.Stop;
											movementCommandPointValue.Status = StatusCodes.Good;
											movementCommandPointValue.SourceTimeStamp = DateTimeOffset.UtcNow;
											movementCommandPointValue.ServerTimeStamp = DateTimeOffset.UtcNow;
											movementCommandValueList.Add(movementCommandPointValue);
											SetPointValues(this.Security, site, movementCommandValueList);
										}
									}
									break;
								default:
									break;
							}
						}
					}

					this.AddSuccess("Saved Successful");
				}
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch(CommunicationException e)
			{
				this.OnError(new Exception(this.GetTranslatedText(e.Message)));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				this.OnError(new Exception(this.GetTranslatedText("Error Saving Movement Module Settings" + 
					(string.IsNullOrEmpty(optionalExMsg) ?  "" : " - " + optionalExMsg))));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// This method will call the legacy system to recalculate the handgauge data based on
		/// the field change.
		/// </summary>
		/// <param name="fieldId">The field ID of the field that has a change</param>
		/// <param name="modelStr">The model string to update on return.</param>
		/// <returns>Return the handgauge model</returns>
		[HttpPost]
		public ActionResult RunCalculator(string givenTag, string givenValue, int givenUnits, Guid pointGuid, string expectedTag, int expectedUnits, Guid? expectedPointGuid)
		{
			_ = expectedPointGuid;
			string result = string.Empty;
			bool error = false;

			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			var numberFormatInfo = new NumberFormatInfo()
			{
				NumberGroupSizes = site.GetNumberGroupSizes(),
				NumberGroupSeparator = site.NumberGroupSeparator,
				NumberDecimalSeparator = site.NumberDecimalSeparator,
				NumberDecimalDigits = 0
			};

			if (string.IsNullOrEmpty(givenTag))
			{
				error = true;
				result = this.GetTranslatedText("MovementSettingsEditor|Referenced tag is not valid!");
			}

			if (!error && string.IsNullOrEmpty(givenValue))
			{
				error = true;
				result = this.GetTranslatedText("MovementSettingsEditor|Referenced value is not valid!");
			}

			if (!error && !Enum.IsDefined(typeof(EngineeringUnit), givenUnits))
			{
				error = true;
				result = this.GetTranslatedText("MovementSettingsEditor|Referenced units are not valid!");
			}

			Guid.TryParse(pointGuid.ToString(), out Guid givenGuid);
			if (!error && givenGuid == Guid.Empty)
			{
				error = true;
				result = this.GetTranslatedText("MovementSettingsEditor|Referenced point guid is not valid!");
			}

			if (!error && string.IsNullOrEmpty(expectedTag))
			{
				error = true;
				result = this.GetTranslatedText("MovementSettingsEditor|Expected tag is not valid!");
			}

			if (!error && !Enum.IsDefined(typeof(EngineeringUnit), expectedUnits))
			{
				error = true;
				result = this.GetTranslatedText("MovementSettingsEditor|Expected units are not valid!");
			}

			Dictionary<Guid, PointTag> pointTagDictionary = null;

			try
			{
				pointTagDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>>(
															x => x.EnumerateByPointGuid(this.Security, pointGuid));
			}
			catch (Exception ex)
			{
				string msgBasic = this.GetTranslatedText("Movement Settings Editor|Error Getting point tags for a given point");
				string msgEventLog = "Movement Settings Editor: " + msgBasic + " " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

				error = true;
				result = this.GetTranslatedText("MovementSettingsEditor|Error getting tags for referenced point!");

				base.OnError(new Exception(msgBasic));
			}

			if (!error)
			{
				PointTag levelProductTag = null;

				levelProductTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.LevelProductGuid).Clone();

				PointTag volumeGrossObservedTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeGrossObservedGuid).Clone();
				PointTag volumeNetStandardTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeNetStandardGuid).Clone();

				List<PointValueIdentifier> pointValueIdentifiers = new List<PointValueIdentifier>(10);
				pointValueIdentifiers.Add(new PointValueIdentifier(volumeGrossObservedTag));
				pointValueIdentifiers.Add(new PointValueIdentifier(volumeNetStandardTag));

				if(levelProductTag != null)
				{
					pointValueIdentifiers.Add(new PointValueIdentifier(levelProductTag));
				}

				var pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, pointValueIdentifiers));

				volumeGrossObservedTag.Value = pointValues[0].Value;
				volumeGrossObservedTag.ValueTypeString = pointValues[0].ValueTypeString;

				volumeNetStandardTag.Value = pointValues[1].Value;
				volumeNetStandardTag.ValueTypeString = pointValues[1].ValueTypeString;

				if (levelProductTag != null)
				{
					levelProductTag.Value = pointValues[2].Value;
					levelProductTag.ValueTypeString = pointValues[2].ValueTypeString;
				}

				List<PointTag> inputTags = new List<PointTag>();

				if (givenTag == TagLevelProduct)
				{
					double level = 0;
					if (givenUnits == 19 || givenUnits == 27)
						level = (double)PointManager.ParseValue(Type.GetType("System.Double"), (EngineeringUnit)givenUnits, numberFormatInfo, givenValue);
					else
					{
						if (!Double.TryParse(givenValue, out level))
						{
							error = true;
							result = this.GetTranslatedText("MovementSettingsEditor|Error converting given level value into Double!");
						}
					}
					if (levelProductTag == null)
					{
						error = true;
						result = this.GetTranslatedText("MovementSettingsEditor|Could not find Level Product Tag on a given point!");
					}

					levelProductTag.Value = level;
					levelProductTag.SourceTimeStamp = DateTimeOffset.Now;
					inputTags.Add(levelProductTag);

					if(expectedTag == TagVolumeGrossObserved)
						inputTags.Add(volumeGrossObservedTag);
					else
						inputTags.Add(volumeNetStandardTag);
				}
				else if (givenTag == TagVolumeGrossObserved)
				{
					if(expectedTag == TagLevelProduct)
						inputTags.Add(levelProductTag);

					if (!Double.TryParse(givenValue, out double volume))
					{
						error = true;
						result = this.GetTranslatedText("MovementSettingsEditor|Error converting given gross volume value into Double!");
					}

					if (volumeGrossObservedTag == null)
					{
						error = true;
						result = this.GetTranslatedText("MovementSettingsEditor|Could not find Volume Gross Observed Tag on a given point!");
					}

					volumeGrossObservedTag.Value = Math.Abs(volume);
					volumeGrossObservedTag.SourceTimeStamp = DateTimeOffset.Now;
					inputTags.Add(volumeGrossObservedTag);
				}
				else if (givenTag == TagVolumeNetStandard)
				{
					if (expectedTag == TagLevelProduct)
						inputTags.Add(levelProductTag);

					if (!Double.TryParse(givenValue, out double volume))
					{
						error = true;
						result = this.GetTranslatedText("MovementSettingsEditor|Error converting given net volume value into Double!");
					}

					if (volumeNetStandardTag == null)
					{
						error = true;
						result = this.GetTranslatedText("MovementSettingsEditor|Could not find Volume Net Standard Tag on a given point!");
					}

					volumeNetStandardTag.Value = Math.Abs(volume);
					volumeNetStandardTag.SourceTimeStamp = DateTimeOffset.Now;
					inputTags.Add(volumeNetStandardTag);
				}

				List<PointTag> outputTags = FMChannelHelper.MakeCall<IPointServiceManager, List<PointTag>>(
																	x => x.RunPointCalculatorX(this.Security, pointGuid, inputTags));

				PointTag updatedTag = outputTags.FirstOrDefault(tag => tag.ID == expectedTag);

				if (updatedTag != null)
				{
					if (updatedTag.Value != null)
					{
						error = false;
						Double.TryParse(updatedTag.Value.ToString(), out double level);

						result = PointManager.FormatValue(Type.GetType(updatedTag.ValueTypeString), (EngineeringUnit)expectedUnits, numberFormatInfo, updatedTag.Value);
					}
					else
					{
						error = true;
						result = this.GetTranslatedText("MovementSettingsEditor|Point calculator has returned a null value!");
					}
				}
			}

			if (error)
			{
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(result, FMEventLogEntryType.Error));

				this.OnError(new Exception(result));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			else
			{
				return this.JsonWithErrorMessages(result, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// This method will call the legacy system to recalculate the handgauge data based on
		/// the field change.
		/// </summary>
		/// <param name="fieldId">The field ID of the field that has a change</param>
		/// <param name="modelStr">The model string to update on return.</param>
		/// <returns>Return the handgauge model</returns>
		[HttpPost]
		public ActionResult RunCalculatorForInterlockedNodes(Guid refPointGuid, bool refTankOrVolume, bool refSourceOrDest, bool refLevelOrBatch, bool refGrossOrNet, string refCurrentSP, int refUnits,
		Guid PointGuid, bool TankOrVolume, bool SourceOrDest, bool LevelOrBatch, bool GrossOrNet, string CurrentValue, int Units)
		{
			string result = string.Empty;
			bool error = false;

			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			var numberFormatInfo = new NumberFormatInfo()
			{
				NumberGroupSizes = site.GetNumberGroupSizes(),
				NumberGroupSeparator = site.NumberGroupSeparator,
				NumberDecimalSeparator = site.NumberDecimalSeparator,
				NumberDecimalDigits = 0
			};

			Guid.TryParse(refPointGuid.ToString(), out Guid givenRefGuid);
			if (!error && givenRefGuid == Guid.Empty)
			{
				error = true;
				result = this.GetTranslatedText("MovementSettingsEditor|Referenced point guid is not valid!");
			}
			
			if (!error &&  string.IsNullOrEmpty(refCurrentSP))
				{
					error = true;
					result = this.GetTranslatedText("MovementSettingsEditor|Referenced target setpoint is not valid!");
				}

				Guid.TryParse(PointGuid.ToString(), out Guid givenGuid);
				if (!error && givenGuid == Guid.Empty)
				{
					error = true;
					result = this.GetTranslatedText("MovementSettingsEditor|Expected point guid is not valid!");
				}

				Dictionary<Guid, PointTag> pointTagDictionary = null;

			try
			{
				pointTagDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>>(
															x => x.EnumerateByPointGuid(this.Security, refPointGuid));
			}
			catch (Exception ex)
			{
				string msgBasic = this.GetTranslatedText("MovementSettingsEditor|Error getting tags for referenced point!");
				string msgEventLog = "Movement Settings Editor: " + msgBasic + " " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

				error = true;
				result = this.GetTranslatedText("MovementSettingsEditor|Error getting tags for referenced point!");

				base.OnError(new Exception(msgBasic));
			}

			PointTag 
				levelProductTag = null, levelProductMinOpLimitTag = null, levelProductMaxOpLimitTag = null,
				volumeGrossObservedTag = null, volumeGrossAvailableTag = null, volumeGrossRemainingTag = null,
				volumeNetStandardTag = null, volumeNetStandardAvailableTag = null, volumeNetStandardRemainingTag = null;

			if (!error)
			{

				double givenTargetSetpoint =
				(double)PointManager.ParseValue(Type.GetType("System.Double"), (EngineeringUnit)refUnits, numberFormatInfo, refCurrentSP);

				List<PointTag> inputTags = new List<PointTag>();
				List<PointTag> outputTags = new List<PointTag>();
				// Get latest values from point service
				List<PointValueIdentifier> pointValueIdentifiers = new List<PointValueIdentifier>(10);

				if (refTankOrVolume) // A tank
				{
					levelProductTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.LevelProductGuid).Clone();
					levelProductMinOpLimitTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.ID == TagLevelProductMinOpLimit).Clone();
					levelProductMaxOpLimitTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.ID == TagLevelProductMaxOpLimit).Clone();
					volumeGrossObservedTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeGrossObservedGuid).Clone();
					volumeGrossAvailableTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeGrossObservedAvailableGuid).Clone();
					volumeGrossRemainingTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeGrossObservedRemainingGuid).Clone();
					volumeNetStandardTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeNetStandardGuid).Clone();
					volumeNetStandardAvailableTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeNetStandardAvailableGuid).Clone();
					volumeNetStandardRemainingTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeNetStandardRemainingGuid).Clone();

					pointValueIdentifiers.Clear();

					pointValueIdentifiers.Add(new PointValueIdentifier(volumeGrossObservedTag));
					if (refTankOrVolume)// A tank
					{
						pointValueIdentifiers.Add(new PointValueIdentifier(volumeGrossAvailableTag));
						pointValueIdentifiers.Add(new PointValueIdentifier(volumeGrossRemainingTag));
					}

					pointValueIdentifiers.Add(new PointValueIdentifier(volumeNetStandardTag));
					if (refTankOrVolume)// A tank
					{
						pointValueIdentifiers.Add(new PointValueIdentifier(volumeNetStandardAvailableTag));
						pointValueIdentifiers.Add(new PointValueIdentifier(volumeNetStandardRemainingTag));
					}

					pointValueIdentifiers.Add(new PointValueIdentifier(levelProductTag));
					pointValueIdentifiers.Add(new PointValueIdentifier(levelProductMinOpLimitTag));
					pointValueIdentifiers.Add(new PointValueIdentifier(levelProductMaxOpLimitTag));

					var pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, pointValueIdentifiers));

					int index = pointValues.FindIndex(x => x.ID == TagVolumeGrossObserved);

					if (index >= 0)
					{
						volumeGrossObservedTag.Value = pointValues[index].Value;
						volumeGrossObservedTag.ValueTypeString = pointValues[index].ValueTypeString;
					}

					index = pointValues.FindIndex(x => x.ID == TagVolumeGrossObservedAvailable);

					if (index >= 0)
					{
						volumeGrossAvailableTag.Value = pointValues[index].Value;
						volumeGrossAvailableTag.ValueTypeString = pointValues[index].ValueTypeString;
					}

					index = pointValues.FindIndex(x => x.ID == TagVolumeGrossObservedRemaining);

					if (index >= 0)
					{
						volumeGrossRemainingTag.Value = pointValues[index].Value;
						volumeGrossRemainingTag.ValueTypeString = pointValues[index].ValueTypeString;
					}

					index = pointValues.FindIndex(x => x.ID == TagVolumeNetStandard);

					if (index >= 0)
					{
						volumeNetStandardTag.Value = pointValues[index].Value;
						volumeNetStandardTag.ValueTypeString = pointValues[index].ValueTypeString;
					}

					index = pointValues.FindIndex(x => x.ID == TagVolumeNetStandardAvailable);

					if (index >= 0)
					{
						volumeNetStandardAvailableTag.Value = pointValues[index].Value;
						volumeNetStandardAvailableTag.ValueTypeString = pointValues[index].ValueTypeString;
					}

					index = pointValues.FindIndex(x => x.ID == TagVolumeNetStandardRemaining);

					if (index >= 0)
					{
						volumeNetStandardRemainingTag.Value = pointValues[index].Value;
						volumeNetStandardRemainingTag.ValueTypeString = pointValues[index].ValueTypeString;
					}

					index = pointValues.FindIndex(x => x.ID == TagLevelProduct);

					if (index >= 0)
					{
						levelProductTag.Value = pointValues[index].Value;
						levelProductTag.ValueTypeString = pointValues[index].ValueTypeString;
					}

					index = pointValues.FindIndex(x => x.ID == TagLevelProductMinOpLimit);

					if (index >= 0)
					{
						levelProductMinOpLimitTag.Value = pointValues[index].Value;
						levelProductMinOpLimitTag.ValueTypeString = pointValues[index].ValueTypeString;
					}

					index = pointValues.FindIndex(x => x.ID == TagLevelProductMaxOpLimit);

					if (index >= 0)
					{
						levelProductMaxOpLimitTag.Value = pointValues[index].Value;
						levelProductMaxOpLimitTag.ValueTypeString = pointValues[index].ValueTypeString;
					}

					double diffCurrentToTarget = 0;

					if (refLevelOrBatch) // Tank with Level mode
					{
						if (levelProductTag.Value != null)
						{
							double currentLevel = (double)PointManager.ParseValue(
															Type.GetType(levelProductTag.ValueTypeString),
															levelProductTag.Units,
															numberFormatInfo,
															levelProductTag.Value.ToString());

							diffCurrentToTarget = Math.Abs(givenTargetSetpoint); // x feet

							levelProductTag.Value = diffCurrentToTarget;
							levelProductTag.SourceTimeStamp = DateTimeOffset.Now;
                        }
						else
						{
							error = true;
							result = this.GetTranslatedText("MovementSettingsEditor|Referenced Level Product Tag has null value!");
						}
					}
					else // Tank with Batch mode
					{
						if (refGrossOrNet)
						{
							if (volumeGrossObservedTag.Value != null)
							{
								double currentGrossVol = (double)PointManager.ParseValue(
																Type.GetType(volumeGrossObservedTag.ValueTypeString),
																volumeGrossObservedTag.Units,
																numberFormatInfo,
																volumeGrossObservedTag.Value.ToString());

								//diffCurrentToTarget = Math.Abs(currentGrossVol - givenTargetSetpoint);
								diffCurrentToTarget = Math.Abs(givenTargetSetpoint);

								volumeGrossObservedTag.Value = diffCurrentToTarget;
								volumeGrossObservedTag.SourceTimeStamp = DateTimeOffset.Now;
							}
							else
							{
								error = true;
								result = this.GetTranslatedText("MovementSettingsEditor|Referenced Volume Gross Observed Tag has null value!");
							}
						}
						else
						{
							if (volumeNetStandardTag.Value != null)
							{
								double currentNetVol = (double)PointManager.ParseValue(
																Type.GetType(volumeNetStandardTag.ValueTypeString),
																volumeNetStandardTag.Units,
																numberFormatInfo,
																volumeNetStandardTag.Value.ToString());

								//diffCurrentToTarget = Math.Abs(currentNetVol - givenTargetSetpoint);
								diffCurrentToTarget = Math.Abs(givenTargetSetpoint);

								volumeNetStandardTag.Value = diffCurrentToTarget;
								volumeNetStandardTag.SourceTimeStamp = DateTimeOffset.Now;
							}
							else
							{
								error = true;
								result = this.GetTranslatedText("MovementSettingsEditor|Referenced Volume Net Standard Tag has null value!");
							}
						}
					}

					if (!error)
					{
                        inputTags.Add(levelProductTag);

                        // For the given differential target level, compute volume
                        inputTags.Add(volumeGrossObservedTag);
                        inputTags.Add(volumeNetStandardTag);

                        // Run the calculator on the target difference
                        outputTags = FMChannelHelper.MakeCall<IPointServiceManager, List<PointTag>>(
																			x => x.RunPointCalculatorX(this.Security, refPointGuid, inputTags));
					}
				}

				// Now try to add/remove this volume to/from the other node
				try
				{
					pointTagDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>>(
																x => x.EnumerateByPointGuid(this.Security, PointGuid));
				}
				catch (Exception ex)
				{
					string msgBasic = this.GetTranslatedText("MovementSettingsEditor|Error getting tags for expected point!");
					string msgEventLog = "Movement Settings Editor: " + msgBasic + " " + ex.Message;
					FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

					error = true;
					result = this.GetTranslatedText("MovementSettingsEditor|Error getting tags for expected point!");

					base.OnError(new Exception(msgBasic));
				}

				PointTag
					levelProductTag1 = null, levelProductMinOpLimitTag1 = null, levelProductMaxOpLimitTag1 = null,
					volumeGrossObservedTag1 = null, volumeGrossAvailableTag1 = null, volumeGrossRemainingTag1 = null,
					volumeNetStandardTag1 = null, volumeNetStandardAvailableTag1 = null, volumeNetStandardRemainingTag1 = null;

					if (!error && TankOrVolume)
					{
						levelProductTag1 = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.LevelProductGuid).Clone();
						levelProductMinOpLimitTag1 = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.ID == TagLevelProductMinOpLimit).Clone();
						levelProductMaxOpLimitTag1 = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.ID == TagLevelProductMaxOpLimit).Clone();
						volumeGrossObservedTag1 = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeGrossObservedGuid).Clone();
						volumeGrossAvailableTag1 = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeGrossObservedAvailableGuid).Clone();
						volumeGrossRemainingTag1 = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeGrossObservedRemainingGuid).Clone();
						volumeNetStandardTag1 = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeNetStandardGuid).Clone();
						volumeNetStandardAvailableTag1 = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeNetStandardAvailableGuid).Clone();
						volumeNetStandardRemainingTag1 = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeNetStandardRemainingGuid).Clone();

						pointValueIdentifiers.Clear();

						pointValueIdentifiers.Add(new PointValueIdentifier(volumeGrossObservedTag1));
						pointValueIdentifiers.Add(new PointValueIdentifier(volumeGrossAvailableTag1));
						pointValueIdentifiers.Add(new PointValueIdentifier(volumeGrossRemainingTag1));

						pointValueIdentifiers.Add(new PointValueIdentifier(volumeNetStandardTag1));
						pointValueIdentifiers.Add(new PointValueIdentifier(volumeNetStandardAvailableTag1));
						pointValueIdentifiers.Add(new PointValueIdentifier(volumeNetStandardRemainingTag1));

						pointValueIdentifiers.Add(new PointValueIdentifier(levelProductTag1));
						pointValueIdentifiers.Add(new PointValueIdentifier(levelProductMinOpLimitTag1));
						pointValueIdentifiers.Add(new PointValueIdentifier(levelProductMaxOpLimitTag1));

						List<PointValue> pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, pointValueIdentifiers));

						int index = pointValues.FindIndex(x => x.ID == TagVolumeGrossObserved);

						if (index >= 0)
						{
							volumeGrossObservedTag1.Value = pointValues[index].Value;
							volumeGrossObservedTag1.ValueTypeString = pointValues[index].ValueTypeString;
						}

						index = pointValues.FindIndex(x => x.ID == TagVolumeGrossObservedAvailable);

						if (index >= 0)
						{
							volumeGrossAvailableTag1.Value = pointValues[index].Value;
							volumeGrossAvailableTag1.ValueTypeString = pointValues[index].ValueTypeString;
						}

						index = pointValues.FindIndex(x => x.ID == TagVolumeGrossObservedRemaining);

						if (index >= 0)
						{
							volumeGrossRemainingTag1.Value = pointValues[index].Value;
							volumeGrossRemainingTag1.ValueTypeString = pointValues[index].ValueTypeString;
						}

						index = pointValues.FindIndex(x => x.ID == TagVolumeNetStandard);

						if (index >= 0)
						{
							volumeNetStandardTag1.Value = pointValues[index].Value;
							volumeNetStandardTag1.ValueTypeString = pointValues[index].ValueTypeString;
						}

						index = pointValues.FindIndex(x => x.ID == TagVolumeNetStandardAvailable);

						if (index >= 0)
						{
							volumeNetStandardAvailableTag1.Value = pointValues[index].Value;
							volumeNetStandardAvailableTag1.ValueTypeString = pointValues[index].ValueTypeString;
						}

						index = pointValues.FindIndex(x => x.ID == TagVolumeNetStandardRemaining);

						if (index >= 0)
						{
							volumeNetStandardRemainingTag1.Value = pointValues[index].Value;
							volumeNetStandardRemainingTag1.ValueTypeString = pointValues[index].ValueTypeString;
						}

						index = pointValues.FindIndex(x => x.ID == TagLevelProduct);

						if (index >= 0)
						{
							levelProductTag1.Value = pointValues[index].Value;
							levelProductTag1.ValueTypeString = pointValues[index].ValueTypeString;
						}

						index = pointValues.FindIndex(x => x.ID == TagLevelProductMinOpLimit);

						if (index >= 0)
						{
							levelProductMinOpLimitTag1.Value = pointValues[index].Value;
							levelProductMinOpLimitTag1.ValueTypeString = pointValues[index].ValueTypeString;
						}

						index = pointValues.FindIndex(x => x.ID == TagLevelProductMaxOpLimit);

						if (index >= 0)
						{
							levelProductMaxOpLimitTag1.Value = pointValues[index].Value;
							levelProductMaxOpLimitTag1.ValueTypeString = pointValues[index].ValueTypeString;
						}
					}

					if (!error)
					{
						if (SourceOrDest) // Source, we will be removing so needs to check against available
						{
							object diffVolRemoved = null, availableVol = null;

							if (GrossOrNet)
							{
								if (TankOrVolume) availableVol = volumeGrossAvailableTag1.Value;
								
								if (refTankOrVolume) // A Tank
								{
									PointTag QtyRemovedFromCalc = outputTags.FirstOrDefault(tag => tag.ID == TagVolumeGrossObserved);
                           if (refLevelOrBatch)
										  diffVolRemoved = Math.Abs((double)volumeGrossObservedTag.Value - (double)QtyRemovedFromCalc.Value);
									else
										  diffVolRemoved = QtyRemovedFromCalc.Value;
                            }
								else // Volume node
								{
									diffVolRemoved = givenTargetSetpoint;
								}
							}
							else
							{
								if (TankOrVolume) availableVol = volumeNetStandardTag1.Value;
								
								if (refTankOrVolume) // A Tank
								{
									PointTag QtyRemovedFromCalc = outputTags.FirstOrDefault(tag => tag.ID == TagVolumeNetStandard);
                           if (refLevelOrBatch)
										  diffVolRemoved = Math.Abs((double)volumeNetStandardTag.Value - (double)QtyRemovedFromCalc.Value);
									else
										  diffVolRemoved = QtyRemovedFromCalc.Value;
								}
								else // Volume node
								{
									diffVolRemoved = givenTargetSetpoint;
								}
							}

							if (TankOrVolume)
							{
								double diffVolRemovedDouble, availableVolDouble;

								if (diffVolRemoved != null && availableVol != null)
								{
									diffVolRemovedDouble = (double)diffVolRemoved;
									availableVolDouble = (double)availableVol;

									if (diffVolRemovedDouble <= availableVolDouble)// We are within the range, so convert and return the value
									{
										if (GrossOrNet)
										{
											if (volumeGrossObservedTag1.Value != null)
											{
												//volumeGrossObservedTag1.Value = (double)volumeGrossObservedTag1.Value - diffVolRemovedDouble;
												volumeGrossObservedTag1.Value = LevelOrBatch ? (double)volumeGrossObservedTag1.Value - diffVolRemovedDouble : (double)diffVolRemovedDouble;
												volumeGrossObservedTag1.SourceTimeStamp = DateTimeOffset.Now;
											}
											else
											{
												error = true;
												result = this.GetTranslatedText("MovementSettingsEditor|Expected Volume Gross Observed Tag has null value!");
											}
										}
										else
										{
											if (volumeNetStandardTag1.Value != null)
											{
												//volumeNetStandardTag1.Value = (double)volumeNetStandardTag1.Value - diffVolRemovedDouble;
												volumeNetStandardTag1.Value = LevelOrBatch ? (double)volumeNetStandardTag1.Value - diffVolRemovedDouble : (double)diffVolRemovedDouble;
												volumeNetStandardTag1.SourceTimeStamp = DateTimeOffset.Now;
											}
											else
											{
												error = true;
												result = this.GetTranslatedText("MovementSettingsEditor|Expected Volume Net Standard Tag has null value!");
											}
										}
									}
									else
									{
										error = true;
										result = this.GetTranslatedText("MovementSettingsEditor|Quantity removed is less than the minimum operating limit!");
									}
								}
								else 
								{
									error = true;
									result = this.GetTranslatedText("MovementSettingsEditor|Available " + (GrossOrNet ? "Volume Gross Observed" : "Volume Net Standard") + " Tag has null value!");
								}
							}
							else // Volume node
							{
								result = PointManager.FormatValue(Type.GetType("System.Double"), (EngineeringUnit)Units, numberFormatInfo, diffVolRemoved);
							}
						}
						else// Destination, we will be adding so needs to check against remaining
						{
							object diffVolAdded = null, remainingVol = null;

							if (GrossOrNet)
							{
								if (TankOrVolume) remainingVol = volumeGrossRemainingTag1.Value;

								if (refTankOrVolume)
								{
									PointTag QtyAddedFromCalc = outputTags.FirstOrDefault(tag => tag.ID == TagVolumeGrossObserved);
									if (refLevelOrBatch)
                                    diffVolAdded = Math.Abs((double)volumeGrossObservedTag.Value - (double)QtyAddedFromCalc.Value);
									else
												diffVolAdded = QtyAddedFromCalc.Value;
                            }
								else // Volumen node
								{
									diffVolAdded = givenTargetSetpoint;
								}
							}
							else
							{
								if (TankOrVolume) remainingVol = volumeNetStandardRemainingTag1.Value;
								
								if (refTankOrVolume)
								{
									PointTag QtyAddedFromCalc = outputTags.FirstOrDefault(tag => tag.ID == TagVolumeNetStandard);
									if (refLevelOrBatch)
                                    diffVolAdded = Math.Abs((double)volumeNetStandardTag.Value - (double)QtyAddedFromCalc.Value);
									else
												diffVolAdded = QtyAddedFromCalc.Value;

								}
								else // Volumen node
								{
									diffVolAdded = givenTargetSetpoint;
								}
							}

							if (TankOrVolume)
							{
								double diffVolAddedDouble, remainingVolDouble;

								if (diffVolAdded != null && remainingVol != null)
								{
									diffVolAddedDouble = (double)diffVolAdded;
									remainingVolDouble = (double)remainingVol;
									
									if (diffVolAddedDouble <= remainingVolDouble) // We are within the range, so convert and return the value
									{
										if (GrossOrNet)
										{
											if (volumeGrossObservedTag1.Value != null)
											{
												//volumeGrossObservedTag1.Value = (double)volumeGrossObservedTag1.Value + diffVolAddedDouble;
												volumeGrossObservedTag1.Value = LevelOrBatch ? (double)volumeGrossObservedTag1.Value + diffVolAddedDouble : (double)diffVolAddedDouble;
												volumeGrossObservedTag1.SourceTimeStamp = DateTimeOffset.Now;
											}
											else
											{
												error = true;
												result = this.GetTranslatedText("MovementSettingsEditor|Expected Volume Gross Observed Tag has null value!");
											}
										}
										else
										{
											if (volumeNetStandardTag1.Value != null)
											{
												//volumeNetStandardTag1.Value = (double)volumeNetStandardTag1.Value + diffVolAddedDouble;
												volumeNetStandardTag1.Value = LevelOrBatch ? (double)volumeNetStandardTag1.Value + diffVolAddedDouble : (double)diffVolAddedDouble;
												volumeNetStandardTag1.SourceTimeStamp = DateTimeOffset.Now;
											}
											else
											{
												error = true;
												result = this.GetTranslatedText("MovementSettingsEditor|Expected Volume Net Standard Tag has null value!");
											}
										}
									}
									else
									{
										error = true;
										result = this.GetTranslatedText("MovementSettingsEditor|Quantity added is greater than the maximum operating limit!");
									}
								}
								else 
								{
									error = true;
									result = this.GetTranslatedText("MovementSettingsEditor|Remaining " + (GrossOrNet ? "Volume Gross Observed" : "Volume Net Standard") + " Tag has null value!");
								}
							}
							else // Volume node, simply return the value
							{
								result = PointManager.FormatValue(Type.GetType("System.Double"), (EngineeringUnit)Units, numberFormatInfo, diffVolAdded);
						}
					}
					}

					if (!error && TankOrVolume) // return for a tank
					{
						List<PointTag> outputTags1 = new List<PointTag>();
						// Run it through the calculator
						inputTags.Clear();
						inputTags.Add(levelProductTag1);
						inputTags.Add(volumeGrossObservedTag1);
						inputTags.Add(volumeNetStandardTag1);


						// Run the calculator
						outputTags1 = FMChannelHelper.MakeCall<IPointServiceManager, List<PointTag>>(
																				x => x.RunPointCalculatorX(this.Security, PointGuid, inputTags));
						if (LevelOrBatch)
						{
							PointTag levelTag = outputTags1.FirstOrDefault(tag => tag.ID == TagLevelProduct);

							if (levelTag != null && levelTag.Value != null)
							{
								result = PointManager.FormatValue(Type.GetType(levelTag.ValueTypeString), (EngineeringUnit)Units, numberFormatInfo, levelTag.Value);
							}
							else
							{
								error = true;
								result = this.GetTranslatedText("MovementSettingsEditor|Calculated Level Product Tag has null value!");
							}
						}
						else
						{
							if (GrossOrNet)
							{
								PointTag volTag = outputTags1.FirstOrDefault(tag => tag.ID == TagVolumeGrossObserved);
								if (volTag.Value != null)
								{
									result = PointManager.FormatValue(Type.GetType(volTag.ValueTypeString), (EngineeringUnit)Units, numberFormatInfo, volTag.Value);
								}
								else
								{
									error = true;
									result = this.GetTranslatedText("MovementSettingsEditor|Calculated Volume Gross Observed Tag has null value!");
								}
							}
							else
							{
								PointTag volTag = outputTags1.FirstOrDefault(tag => tag.ID == TagVolumeNetStandard);
								if (volTag.Value != null)
								{
									result = PointManager.FormatValue(Type.GetType(volTag.ValueTypeString), (EngineeringUnit)Units, numberFormatInfo, volTag.Value);
								}
								else
								{
									error = true;
									result = this.GetTranslatedText("MovementSettingsEditor|Calculated Volume Net Standard Tag has null value!");
								}
							}
						}
					}
			}

			if (error)
			{
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(result, FMEventLogEntryType.Error));

				this.OnError(new Exception(result));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			else
			{
				return this.JsonWithErrorMessages(result, JsonRequestBehavior.AllowGet);
			}
		}

		#region Private Methods
		/// <summary>
		/// This method will get the site's number and date/time formats and set the 
		/// model.
		/// </summary>
		/// <param name="model">The model to be updated.</param>
		private void SetDateAndNumberFormats(MovementModuleSettingsEditorModel model)
		{
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			if (model != null && site != null)
			{
				model.NumberGroupSeparator = site.NumberGroupSeparator;
				model.NumberDecimalSeparator = site.NumberDecimalSeparator;
				model.NumberGroupSizes = site.GetNumberGroupSizes();
				model.ShortDatePattern = site.ShortDatePattern;
				model.TimePattern = site.TimePattern;
				model.TimeZone = site.TimeZone;
			}
		}

		private double? ConvertStringValueToDouble(string input, TransferModes transferMode, string units)
		{
			double? dValue = null;
			try
			{
				if (!string.IsNullOrEmpty(input))
				{
					if (transferMode == TransferModes.Level)
					{
						if (units != null && (units.ToUpper() == "FT-IN-8TH" || units.ToUpper() == "FT-IN-16TH"))
						{
							double feet = 0, inches = 0, fraction = 0;
							bool negative = false;
							input = input.Trim();

							if (input.StartsWith("-"))
							{
								negative = true;
								input = input.Substring(1);
							}

							string[] parts = input.Split('-');
							if (parts.Length >= 3) fraction = int.Parse(parts[2].Trim());
							if (parts.Length >= 2) inches = int.Parse(parts[1].Trim());
							if (parts.Length >= 1) feet = int.Parse(parts[0].Trim());

							// convert fraction to feet
							fraction = (units.ToUpper() == "FT-IN-8TH") ? (fraction / 8.0) : (fraction / 16.0);
							fraction /= 12.0;
							// convert inches to feet
							inches /= 12.0;
							dValue = feet + inches + fraction;
							if (negative) dValue = -dValue;
						}
					}
					else
						dValue = Double.Parse(input);
				}
			}
			catch (Exception except)
			{
				string msg = "Error Converting String Value To Double";
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + " " + except.Message, FMEventLogEntryType.Error));
			}
			return dValue;
		}

		private string ConvertDoubleValueToString(double? input, TransferModes transferMode, string units)
		{
			string sValue = "";
			try 
			{
				if (input.HasValue)
				{
					if (transferMode == TransferModes.Level)
					{
						if (units != null && (units.ToUpper() == "FT-IN-8TH" || units.ToUpper() == "FT-IN-16TH"))
						{
							int feet = 0, inches = 0, fraction = 0;
							double fInches = 0.0;
							feet = (int)input;
							fInches = Math.Abs((input.Value - feet) * 12.0);
							inches = (int)fInches;
							fraction = (int)Math.Round((fInches - inches) * (units.ToUpper() == "FT-IN-8TH" ? 8 : 16), 0, MidpointRounding.AwayFromZero);

							sValue = feet.ToString(feet < 100 ? "00" : "000") + "-" + inches.ToString("00") + "-" + fraction.ToString(units.ToUpper() == "FT-IN-8TH" ? "0" : "00");
						}
					}
					else
						sValue = input.Value.ToString();
				}
			}
			catch (Exception except)
			{
				string msg = "Error Converting Double Value To String";
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + " " + except.Message, FMEventLogEntryType.Error));
			}

			return sValue;
		}

		/// <summary>
		/// This method will create an empty movement module settings editor model with
		/// the date and number formats.
		/// </summary>
		/// <returns>Returns the movement module settings editor model.</returns>
		private MovementModuleSettingsEditorModel CreateEmptyModel()
		{
			var model = new MovementModuleSettingsEditorModel
			{
				PointId = "New Movement",
				PointPropertyId = "Movement Settings",
				EnableCreateNewSection = true,
				Type = MovementType.Transfer
			};

			// Set the date and number formats based on the site;
			this.SetDateAndNumberFormats(model);

			model.MovementNodeModelList = new List<MovementNodeModel>();
			return model;
		}

		private void GetMovementNodesByModuleType(List<MovementNodeModel> movementNodeModelList, string[] moduleTypeNames, ref int totalPoints, NodeModuleType nodeModuleType)
		{
			// Get a list of point template guids that contains all of the given modules
			var pointTemplateGuidList = FMChannelHelper.MakeCall<IModules, List<Guid>>(x => x.EnumeratePointTemplatesByAllModuleTypeNames(this.Security, moduleTypeNames));
			if (!isTemplatePoint
			&& pointTemplateGuidList.Count > 0)
			{
				// Get all the points that are instances of the above templates
				var points = FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.EnumerateBasicByPointTemplateGuids(this.Security, pointTemplateGuidList.ToArray()));

				List<Thread> workerThreads = new List<Thread>();
				int totalPointCount = 0;

				var strTotalPointsPerThread = FMBusinessObjects.UtilityObjects.AppSettingsHelper.GetKeyValue<string>("NumberOfMovementPointsToFetchInMovementSettingsDialog", "25");
				Int32.TryParse(strTotalPointsPerThread, out int totalPointsPerThread);
				int totPointsToProcess = (totalPointsPerThread > 0) ? totalPointsPerThread : 25;

				for (int i = 0; i < points.Count; i += totPointsToProcess)
				{
					List<Point> pointList = points.GetRange(i, ((points.Count - i) >= totPointsToProcess) ? totPointsToProcess : (points.Count - i));
					Thread thrd = new Thread(() => { 
						List<MovementNodeModel>  nodeData = FetchPointData(pointList, nodeModuleType); 
						movementNodeModelList.AddRange(nodeData);
						totalPointCount += nodeData.Count;
					});
					workerThreads.Add(thrd);
					thrd.Start();
				}

				foreach(Thread thrd in workerThreads)
				{
					thrd.Join();
				}

				totalPoints = totalPointCount;
			}
		}

		private List<MovementNodeModel> FetchPointData(List<Point> points, NodeModuleType nodeModuleType)
		{
			var nodeWellKnownTagGuidList = new Guid[] {
				Guids.LevelProductGuid,
				Guids.VolumeGrossObservedGuid,
				Guids.VolumeNetStandardGuid
			};

			List<MovementNodeModel> movementNodeModelList = new List<MovementNodeModel>();
			foreach (var point in points)
			{

				var pointGuidList = new List<Guid>
				{
					point.PointGuid
				};

				var nodePointValueIdentifierList = FMChannelHelper.MakeCall<IPointTags, List<PointValueIdentifier>>(x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.Security, pointGuidList, nodeWellKnownTagGuidList.ToList()));

				var tagGuidList = new List<Guid> {
					nodePointValueIdentifierList[0].IdentityGuid,
					nodePointValueIdentifierList[1].IdentityGuid,
					nodePointValueIdentifierList[2].IdentityGuid
				};

				Dictionary<Guid, PointTag> pointTagDictionary =	FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>>(x => x.EnumerateByTagList(this.Security, tagGuidList));

				string strLevelUnits = "ft-in-16th";
				EngineeringUnit engLevelUnits = EngineeringUnit.FmlFtIn16Th;


				PointTag levelTag = pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.LevelProductGuid);
				if (levelTag != null)
				{
					engLevelUnits = levelTag.Units;
					strLevelUnits = EngineeringUnits.GetUnitAbbreviation(levelTag.Units);
				}

				string strGrossVolumeUnits = "gal (US)";
				EngineeringUnit engGrossVolumeUnits = EngineeringUnit.FmvUsGal;

				PointTag grossVolumeTag = pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeGrossObservedGuid);
				if (grossVolumeTag != null)
				{
					engGrossVolumeUnits = grossVolumeTag.Units;
					strGrossVolumeUnits = EngineeringUnits.GetUnitAbbreviation(grossVolumeTag.Units);
				}

				string strNeVolumeUnits = "gal (US)"; ;
				EngineeringUnit engNetVolumeUnits = EngineeringUnit.FmvUsGal;

				PointTag netVolumeTag = pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeNetStandardGuid); ;

				if (netVolumeTag != null)
				{
					engNetVolumeUnits = netVolumeTag.Units;
					strNeVolumeUnits = EngineeringUnits.GetUnitAbbreviation(netVolumeTag.Units);
				}

				TransferVolumeMode currentTransferVolumeModeMode = TransferVolumeMode.GrossObservedVolume;

				if (nodeModuleType == NodeModuleType.StandardTank)
				{
					var  pointPropertyGuid = FMChannelHelper.MakeCall<IPointProperties, Guid>(
														x => x.GetPointPropertyGuid(this.Security, point.PointGuid, "Tank Transfer Settings"));

					var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(
														x => x.Get(this.Security, pointPropertyGuid));


					if (pointProperty != null)
					{
						TankTransferModuleSettings tankTransferModuleSettings = pointProperty.Value as TankTransferModuleSettings;

						if (tankTransferModuleSettings != null)
						{
							currentTransferVolumeModeMode = tankTransferModuleSettings.TransferVolumeMode;
						}
					}
				}
				else if(nodeModuleType == NodeModuleType.StandardVolume)
				{
					var pointPropertyGuid = FMChannelHelper.MakeCall<IPointProperties, Guid>(
														x => x.GetPointPropertyGuid(this.Security, point.PointGuid, "Volume Transfer Settings"));

					var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(
														x => x.Get(this.Security, pointPropertyGuid));

					if (pointProperty != null)
					{
						VolumeTransferModuleSettings volTransferModuleSettings = pointProperty.Value as VolumeTransferModuleSettings;

						if (volTransferModuleSettings != null)
						{
							currentTransferVolumeModeMode = volTransferModuleSettings.TransferVolumeMode;
						}
					}
				}

				var movementNodeModel = new MovementNodeModel
				{
					MovementNodeId = point.ID,
					MovementNodeGuid = point.PointGuid,
					TransferTarget = "0.0",
					TransferDirection = TransferDirection.Source,
					TransferMode = TransferModes.Batch,
					IndividualNodeControl = false,
					Units = string.Empty,
					LevelProductUnits = strLevelUnits,
					VolumeUnits = strNeVolumeUnits,
					ModuleType = nodeModuleType,
					IntLevelUnits = (int)engLevelUnits,
					IntVolumeUnits = (int)engNetVolumeUnits,
					NodeTransferVolumeMode = currentTransferVolumeModeMode,
				};

				movementNodeModelList.Add(movementNodeModel);
			}
			return movementNodeModelList;
		}
		#endregion
	}
}