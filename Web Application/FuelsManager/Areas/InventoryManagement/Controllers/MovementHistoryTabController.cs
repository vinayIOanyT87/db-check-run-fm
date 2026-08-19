namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using FMBusinessObjects.Constants;
	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Web.Mvc;
	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public class MovementHistoryTabController : FMBaseControllerEx
	{
		protected const string MovementHistoryTabID = "MovementHistoryTab";
		private Guid previousSiteGuid;
		private string previousSiteId;

		#region Public static methods
		/// <summary>
		/// Identifies the data dictionary keys needed for this item.
		/// </summary>
		/// <param name="model">The model to serialize</param>
		/// <returns>An array of data dictionary keys.</returns>
		[NonAction]
		public static string SerializeModel(MovementHistoryTabModel model)
		{
			return JsonConvert.SerializeObject(model);
		}

		/// <summary>
		/// Identifies the data dictionary keys needed for this item.
		/// </summary>
		/// <param name="modelStr">The model to serialize</param>
		/// <returns>An array of data dictionary keys.</returns>
		[NonAction]
		public static MovementHistoryTabModel DeserializeModel(string modelStr)
		{
			var jsonSerializerSettings = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			var obj = JsonConvert.DeserializeObject<MovementHistoryTabModel>(modelStr, jsonSerializerSettings);
			return obj;
		}

		/// <summary>
		/// This method will return movement history items in a model.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="columnFilterInfoList"></param>
		/// <param name="initialLoadRequest"></param>
		/// <param name="initialLoadCount"></param>
		/// <returns>Returns the movement history model.</returns>
		[NonAction]
		public MovementHistoryTabModel GetModel(SecurityClass security
												, List<MovementHistoryTabColumnFilterInfo> columnFilterInfoList
												, bool initialLoadRequest
												, int initialLoadCount
												, string orderColumnName
												, string orderDirection)
		{
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(security, security.SiteGuid, false, false, false));

			this.previousSiteId = site.ID;
			this.previousSiteGuid = site.IdentityGuid;


			var model = new MovementHistoryTabModel
			{
				MovementHistories				= new List<MovementHistoryTabRow>(), 
				Site							= site, 
				HasModifyMovementHistoryRight	= security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_HISTORY),
				HasViewMovementHistoryRight		= security.HasRight(RIGHT.OPERATE_VIEW_MOVEMENT_HISTORY)
			};

			List<MovementHistoryDO> movementHistoryDoList;

			// On an initial load request, we want to get the top x number of records. This will only be invoked
			// once, when the page is first brought up.
			if (initialLoadRequest)
			{
				movementHistoryDoList = FMChannelHelper.MakeCall<IMovementHistories, List<MovementHistoryDO>>(
															x => x.GetMovementsByInitialLoadRequest(security, site.SiteGuid, initialLoadCount));
			}
			else
			{
				// Get the date filters.
				this.GetDateFilters(columnFilterInfoList, site, out DateTime startTime, out DateTime endTime);
				this.GetRecordFilters(columnFilterInfoList, out bool autogauge, out bool handgauge);
				bool midnightRecord = this.GetMidnightFilter(columnFilterInfoList);

				// Get all the movements from the history table
				movementHistoryDoList = FMChannelHelper.MakeCall<IMovementHistories, List<MovementHistoryDO>>(
									x => x.GetAllMovementsBySiteGuid(security, site.SiteGuid, startTime, endTime, autogauge, handgauge, midnightRecord, orderColumnName, orderDirection));
			}

			// This looks reverse, but is not.
			DateTimeOffset currentMinDate = DateTimeOffset.MaxValue;
			DateTimeOffset currentMaxDate = DateTimeOffset.MinValue;

			foreach (MovementHistoryDO movementHistoryDo in movementHistoryDoList)
			{
				string movementType = movementHistoryDo.Type;
				if(Enum.TryParse<MovementType>(movementType, out var movementTypeEnum))
				{
                    movementType = FMBusinessObjects.DataObjects.CodedVariables.SelectList.CreateUIString(movementTypeEnum);
                }

                var row = new MovementHistoryTabRow
				{
					DT_RowId								= MovementHistoryTabRow.RowPrefix + movementHistoryDo.MovementHistoryGuid.ToString(),
					MovementHistoryGuid						= movementHistoryDo.MovementHistoryGuid,
					Name									= movementHistoryDo.Name,
					Node									= movementHistoryDo.Node,
					SiteId									= this.GetSiteId(movementHistoryDo.SiteGuid),
					InitiationCount							= movementHistoryDo.InitiationCount,
					RecordType								= (int)movementHistoryDo.RecordType,
					TimeStampStr							= this.ConvertUtcToLocalTime(movementHistoryDo.TimeStamp, site),
					ParentGuid								= movementHistoryDo.ParentGuid,
					AutoStart								= movementHistoryDo.AutoStart,
					AutoStartTimeStr						= this.ConvertDateTimeToLocalTime(movementHistoryDo.AutoStartTime, site),
					AutoStop								= movementHistoryDo.AutoStop,
					AutoStopTimeStr							= this.ConvertDateTimeToLocalTime(movementHistoryDo.AutoStopTime, site),
					CloseoutDataModifiedBy					= movementHistoryDo.CloseoutDataModifiedBy,
					CloseoutDensityProductInAirStr			= this.GetFormattedValue(movementHistoryDo.CloseoutDensityProductInAir, site, SITE_VARIABLE_TYPE.DENSITY, movementHistoryDo.DecimalPlacesDensity),
					CloseoutDensityProductObservedStr		= this.GetFormattedValue(movementHistoryDo.CloseoutDensityProductObserved, site, SITE_VARIABLE_TYPE.DENSITY, movementHistoryDo.DecimalPlacesDensity),
					CloseoutDensityProductObservedTimeStr	= movementHistoryDo.CloseoutDensityProductObservedTime.ToString(),
					CloseoutDensityProductStandardStr		= this.GetFormattedValue(movementHistoryDo.CloseoutDensityProductStandard, site, SITE_VARIABLE_TYPE.DENSITY, movementHistoryDo.DecimalPlacesDensity),
					CloseoutDensityProductStandardTimeStr	= this.ConvertDateTimeToLocalTime(movementHistoryDo.CloseoutDensityProductStandardTime, site),
					CloseoutDensityProductStandardInAirStr	= this.GetFormattedValue(movementHistoryDo.CloseoutDensityProductStandardInAir, site, SITE_VARIABLE_TYPE.DENSITY, movementHistoryDo.DecimalPlacesDensity),
					CloseoutLevelProductStr					= this.ConvertToLevelValue(movementHistoryDo.UnitsLevelProductIndex, movementHistoryDo.CloseoutLevelProduct),
					CloseoutLevelProductTimeStr				= this.ConvertDateTimeToLocalTime(movementHistoryDo.CloseoutLevelProductTime, site),
					CloseoutLevelWaterStr					= this.ConvertToLevelValue(movementHistoryDo.UnitsLevelProductIndex, movementHistoryDo.CloseoutLevelWater),
					CloseoutMassLiquidStr					= this.GetFormattedValue(movementHistoryDo.CloseoutMassLiquid, site, SITE_VARIABLE_TYPE.MASS, null),
					CloseoutPercentBswStr					= this.GetPercentFormattedValue(movementHistoryDo.CloseoutPercentBsw),
					CloseoutRoofMassStr						= this.GetFormattedValue(movementHistoryDo.CloseoutRoofMass, site, SITE_VARIABLE_TYPE.MASS, null),
					CloseoutTankShellCorrectionStr			= this.GetFormattedValue(movementHistoryDo.CloseoutTankShellCorrection, site, SITE_VARIABLE_TYPE.VCF, null),
					CloseoutTemperatureAmbientStr			= this.GetFormattedValue(movementHistoryDo.CloseoutTemperatureAmbient, site, SITE_VARIABLE_TYPE.TEMPERATURE, movementHistoryDo.DecimalPlacesTemperature),
					CloseoutTemperatureAmbientTimeStr		= this.ConvertDateTimeToLocalTime(movementHistoryDo.CloseoutTemperatureAmbientTime, site),
					CloseoutTemperatureDensityStr			= this.GetFormattedValue(movementHistoryDo.CloseoutTemperatureDensity, site, SITE_VARIABLE_TYPE.TEMPERATURE, movementHistoryDo.DecimalPlacesTemperature),
					CloseoutTemperatureProductStr			= this.GetFormattedValue(movementHistoryDo.CloseoutTemperatureProduct, site, SITE_VARIABLE_TYPE.TEMPERATURE, movementHistoryDo.DecimalPlacesTemperature),
					CloseoutTimeStr							= this.ConvertDateTimeToLocalTime(movementHistoryDo.CloseoutTime, site),
					CloseoutTransferGovStr					= this.GetFormattedValue(movementHistoryDo.CloseoutTransferGov, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					CloseoutTransferNsvStr					= this.GetFormattedValue(movementHistoryDo.CloseoutTransferNsv, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					CloseoutTransferMassLiquidStr			= this.GetFormattedValue(movementHistoryDo.CloseoutTransferMassLiquid, site, SITE_VARIABLE_TYPE.MASS, null),
					CloseoutTransferVolumeWaterStr			= this.GetFormattedValue(movementHistoryDo.CloseoutTransferVolumeWater, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					CloseoutVolumeBswStr					= this.GetFormattedValue(movementHistoryDo.CloseoutVolumeBsw, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					CloseoutVolumeCorrectionFactorStr		= this.GetFormattedValue(movementHistoryDo.CloseoutVolumeCorrectionFactor, site, SITE_VARIABLE_TYPE.VCF, null),
					CloseoutVolumeGrossObservedStr			= this.GetFormattedValue(movementHistoryDo.CloseoutVolumeGrossObserved, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					CloseoutVolumeGrossStandardStr			= this.GetFormattedValue(movementHistoryDo.CloseoutVolumeGrossStandard, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					CloseoutVolumeNetStandardStr			= this.GetFormattedValue(movementHistoryDo.CloseoutVolumeNetStandard, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					CloseoutVolumeRoofCorrectionStr			= this.GetFormattedValue(movementHistoryDo.CloseoutVolumeRoofCorrection, site, SITE_VARIABLE_TYPE.VCF, null),
					CloseoutVolumeTotalObservedStr			= this.GetFormattedValue(movementHistoryDo.CloseoutVolumeTotalObserved, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					CloseoutVolumeWaterStr					= this.GetFormattedValue(movementHistoryDo.CloseoutVolumeWater, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					Comment										= movementHistoryDo.Comment,
					Type									= movementType,
					OrderNumber								= movementHistoryDo.OrderNumber,
					PlannedStartTimeStr					= this.ConvertDateTimeToLocalTime(movementHistoryDo.PlannedStartTime, site),
					Product									= movementHistoryDo.Product,
					ProductDescription						= movementHistoryDo.ProductDescription,
					StartTimeStr							= this.ConvertDateTimeToLocalTime(movementHistoryDo.StartTime, site),
					StopTimeStr								= this.ConvertDateTimeToLocalTime(movementHistoryDo.StopTime, site),
					StartDensityProductObservedStr			= this.GetFormattedValue(movementHistoryDo.StartDensityProductObserved, site, SITE_VARIABLE_TYPE.DENSITY, movementHistoryDo.DecimalPlacesDensity),
					StartDensityProductObservedTimeStr		= this.ConvertDateTimeToLocalTime(movementHistoryDo.StartDensityProductObservedTime, site),
					StartDensityProductObservedInAirStr		= this.GetFormattedValue(movementHistoryDo.StartDensityProductObservedInAir, site, SITE_VARIABLE_TYPE.DENSITY, movementHistoryDo.DecimalPlacesDensity),
					StartDensityProductStandardStr			= this.GetFormattedValue(movementHistoryDo.StartDensityProductStandard, site, SITE_VARIABLE_TYPE.DENSITY, movementHistoryDo.DecimalPlacesDensity),
					StartDensityProductStandardTimeStr		= this.ConvertDateTimeToLocalTime(movementHistoryDo.StartDensityProductStandardTime, site),
					StartUserID								= movementHistoryDo.StartUserID,
					StartLevelProductStr					= this.ConvertToLevelValue(movementHistoryDo.UnitsLevelProductIndex, movementHistoryDo.StartLevelProduct),
					StartLevelProductTimeStr				= this.ConvertDateTimeToLocalTime(movementHistoryDo.StartLevelProductTime, site),
					StartLevelWaterStr						= this.ConvertToLevelValue(movementHistoryDo.UnitsLevelProductIndex, movementHistoryDo.StartLevelWater),
					StartLevelWaterTimeStr					= this.ConvertDateTimeToLocalTime(movementHistoryDo.StartLevelWaterTime, site),
					StartPercentBswStr						= this.GetPercentFormattedValue(movementHistoryDo.StartPercentBsw),
					StartMassLiquidStr						= this.GetFormattedValue(movementHistoryDo.StartMassLiquid, site, SITE_VARIABLE_TYPE.MASS, null),
					StartTankShellCorrectionStr				= this.GetFormattedValue(movementHistoryDo.StartTankShellCorrection, site, SITE_VARIABLE_TYPE.VCF, null),
					StartTemperatureAmbientStr				= this.GetFormattedValue(movementHistoryDo.StartTemperatureAmbient, site, SITE_VARIABLE_TYPE.TEMPERATURE, movementHistoryDo.DecimalPlacesTemperature),
					StartTemperatureAmbientTimeStr			= this.ConvertDateTimeToLocalTime(movementHistoryDo.StartTemperatureAmbientTime, site),
					StartTemperatureProductStr				= this.GetFormattedValue(movementHistoryDo.StartTemperatureProduct, site, SITE_VARIABLE_TYPE.TEMPERATURE, movementHistoryDo.DecimalPlacesTemperature),
					StartTemperatureProductTimeStr			= this.ConvertDateTimeToLocalTime(movementHistoryDo.StartTemperatureProductTime, site),
					StartTemperatureDensityStr				= this.GetFormattedValue(movementHistoryDo.StartTemperatureDensity, site, SITE_VARIABLE_TYPE.TEMPERATURE, movementHistoryDo.DecimalPlacesTemperature),
					StartTemperatureDensityTimeStr			= this.ConvertDateTimeToLocalTime(movementHistoryDo.StartTemperatureDensityTime, site),
					StartVolumeStr							= this.GetFormattedValue(movementHistoryDo.StartVolume, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
                    StartVolumeBswStr						= this.GetFormattedValue(movementHistoryDo.StartVolumeBsw, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
                    StartVolumeCorrectionFactorStr = this.GetFormattedValue(movementHistoryDo.StartVolumeCorrectionFactor, site, SITE_VARIABLE_TYPE.VCF, null),
					StartVolumeGrossObservedStr				= this.GetFormattedValue(movementHistoryDo.StartVolumeGrossObserved, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					StartVolumeGrossStandardStr				= this.GetFormattedValue(movementHistoryDo.StartVolumeGrossStandard, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					StartVolumeNetStandardStr				= this.GetFormattedValue(movementHistoryDo.StartVolumeNetStandard, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					StartVolumeRoofCorrectionStr			= this.GetFormattedValue(movementHistoryDo.StartVolumeRoofCorrection, site, SITE_VARIABLE_TYPE.VCF, null),
					StartVolumeTotalObservedStr				= this.GetFormattedValue(movementHistoryDo.StartVolumeTotalObserved, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					StartVolumeWaterStr						= this.GetFormattedValue(movementHistoryDo.StartVolumeWater, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					UnitsLevelProduct						= this.GetEngineeringUnitsAbbreviation(movementHistoryDo.UnitsLevelProductIndex),
					UnitsTemperatureAmbient					= this.GetEngineeringUnitsAbbreviation(movementHistoryDo.UnitsTemperatureAmbientIndex),
					UnitsTemperatureDensity					= this.GetEngineeringUnitsAbbreviation(movementHistoryDo.UnitsTemperatureDensityIndex),
					UnitsTemperatureProduct					= this.GetEngineeringUnitsAbbreviation(movementHistoryDo.UnitsTemperatureProductIndex),
					UnitsDensityProductObserved				= this.GetEngineeringUnitsAbbreviation(movementHistoryDo.UnitsDensityProductObservedIndex),
					UnitsDensityProductStandard				= this.GetEngineeringUnitsAbbreviation(movementHistoryDo.UnitsDensityProductStandardIndex),
					UnitsVolume								= this.GetEngineeringUnitsAbbreviation(movementHistoryDo.UnitsVolumeIndex),
					UnitsMass								= this.GetEngineeringUnitsAbbreviation(movementHistoryDo.UnitsMassIndex),
					UserData01								= movementHistoryDo.UserData01,
					UserData02								= movementHistoryDo.UserData02,
					UserData03								= movementHistoryDo.UserData03,
					UserData04								= movementHistoryDo.UserData04,
					UserData05								= movementHistoryDo.UserData05,
					UserData06								= movementHistoryDo.UserData06,
					UserData07								= movementHistoryDo.UserData07,
					UserData08								= movementHistoryDo.UserData08,
					UserData09								= movementHistoryDo.UserData09,
					UserData10								= movementHistoryDo.UserData10,
					TransferDeviationStr					= this.GetFormattedValue(movementHistoryDo.TransferDeviation, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					TransferPercentDeviationStr		= this.GetFormattedValue(movementHistoryDo.TransferPercentDeviation, site, SITE_VARIABLE_TYPE.DEFAULT, movementHistoryDo.DecimalPlacesPercent),
					TransferModeStr						= this.GetTransferMode(movementHistoryDo.TransferMode),
					TransferStatusStr						= this.GetTransferStatus(movementHistoryDo.TransferStatus),
					TransferTargetStr						= (this.GetTargetUnitsType(movementHistoryDo.TransferTargetUnitsIndex) == EngineeringUnitType.FmuLength) ? this.ConvertToLevelValue(movementHistoryDo.UnitsLevelProductIndex, movementHistoryDo.TransferTarget) : this.GetFormattedValue(movementHistoryDo.TransferTarget, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					TransferTargetUnits					= this.GetEngineeringUnitsAbbreviation(movementHistoryDo.TransferTargetUnitsIndex),
					TransferLevelTargetStr				= this.ConvertToLevelValue(movementHistoryDo.UnitsLevelProductIndex, movementHistoryDo.TransferLevelTarget),
					TransferVolumeTargetStr				= this.GetFormattedValue(movementHistoryDo.TransferVolumeTarget, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					TransferTimeRemainingStr			= this.ConvertToTime(movementHistoryDo.TransferTimeRemaining),
					TransferDirection						= movementHistoryDo.TransferDirection,
					StatusStr								= movementHistoryDo.Status == null ? string.Empty : ((MovementStatus)movementHistoryDo.Status.Value).ToString(),
					LevelProductStr						= this.ConvertToLevelValue(movementHistoryDo.UnitsLevelProductIndex, movementHistoryDo.LevelProduct),
					TransferredVolumeStr					= this.GetFormattedValue(movementHistoryDo.TransferredVolume, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					TransferredVolumeWaterStr			= this.GetFormattedValue(movementHistoryDo.TransferredVolumeWater, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					VolumeWaterStr							= this.GetFormattedValue(movementHistoryDo.VolumeWater, site, SITE_VARIABLE_TYPE.VOLUME, movementHistoryDo.DecimalPlacesVolume),
					StartDensityProductStandardInAirStr		= this.GetFormattedValue(movementHistoryDo.StartDensityProductStandardInAir, site, SITE_VARIABLE_TYPE.DENSITY, movementHistoryDo.DecimalPlacesDensity),
					CommentDateTimeStr						= this.ConvertUtcToLocalTime(movementHistoryDo.CommentDateTime, site),
					CommentUserName							= movementHistoryDo.CommentUserId,
					CreatedDateStr							= this.ConvertDateTimeToLocalTime(movementHistoryDo.CreatedDate, site),
					PointGuid								= movementHistoryDo.PointGuid,
					RootParentGuid							= movementHistoryDo.RootParentGuid,
					RecordSeq								= movementHistoryDo.RecordSeq,
					
					MidnightRecord							= movementHistoryDo.MidnightRecord
				};

				// Compare for min and max dates.
				currentMinDate = this.FindMinDate(currentMinDate, movementHistoryDo.CreatedDate);
				currentMaxDate = this.FindMaxDate(currentMaxDate, movementHistoryDo.CreatedDate);

				model.MovementHistories.Add(row);
			}

			// These dates are used on the UI when it is an initial load of the page to set
			// the start and end dates.
			if(model.MovementHistories.Count > 0)
			{
				model.MovementHistories[0].MinDateTimeStr = this.ConvertDateTimeToLocalTime(currentMinDate, site);
				model.MovementHistories[0].MaxDateTimeStr = this.ConvertDateTimeToLocalTime(currentMaxDate, site);
			}

			return model;
		}

		[NonAction]
		public static MovementHistoryTabModel GetBlankModel(SecurityClass security)
		{
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(security, security.SiteGuid, false, false, false));

			var userSetting = GetViewStateSettings(security);

			var model = new MovementHistoryTabModel
			{
				MovementHistories = new List<MovementHistoryTabRow>(),
				Site = site,
				ViewStateSettings = userSetting,
				HasModifyMovementHistoryRight = security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_HISTORY),
				HasViewMovementHistoryRight = security.HasRight(RIGHT.OPERATE_VIEW_MOVEMENT_HISTORY),
				HasMovementTicketReport = !string.IsNullOrEmpty(site.MovementTicketReportName),
				HasMovementTicketPrinter = !string.IsNullOrEmpty(site.MovementTicketPrinter)
			};

			return model;
		}
		#endregion

		#region Protected static methods.
		[NonAction]
		protected void SaveViewStateSettings(SecurityClass security, MovementHistoryUserViewStateSettings movementHistoryViewStateSettings)
		{
			if (movementHistoryViewStateSettings != null)
			{
				var userSettings =
					FMChannelHelper.MakeCall<IUserViewStateSettings, UserViewStateSettingCollection>(x => x.EnumerateBySiteUserClientIpAddressWindowNameAndViewID(security, security.SiteGuid, security.UserGuid, "", MovementHistoryTabID));
				if (userSettings == null || userSettings.Count <= 0)
				{
					var userSetting = new UserViewStateSetting(security)
					{
						Value = movementHistoryViewStateSettings,
						ViewID = MovementHistoryTabID
					};

					FMChannelHelper.MakeCall<IUserViewStateSettings>(x => x.Add(security, userSetting));
				}
				else
				{
					var userSetting = userSettings[0];
					userSetting.Value = movementHistoryViewStateSettings;
					FMChannelHelper.MakeCall<IUserViewStateSettings>(x => x.Modify(security, userSetting));
				}
			}
		}

		[NonAction]
		protected static MovementHistoryUserViewStateSettings GetViewStateSettings(SecurityClass security)
		{
			var userSettings = FMChannelHelper.MakeCall<IUserViewStateSettings, UserViewStateSettingCollection>(
												x => x.EnumerateBySiteUserClientIpAddressWindowNameAndViewID(security, security.SiteGuid, security.UserGuid, "", MovementHistoryTabID));
			MovementHistoryUserViewStateSettings userSetting = null;

			if (userSettings != null && userSettings.Count > 0)
			{
				userSetting = (MovementHistoryUserViewStateSettings)userSettings[0].Value;
			}
			return userSetting;
		}
		#endregion

		#region Public methods
		// GET: InventoryManagement/MovementHistoryTab
		[HttpGet]
		public ActionResult MovementHistoryTabView()
		{
			var model = GetBlankModel(this.Security);
			return this.View(model);
		}

		[HttpGet]
		public ActionResult MovementHistoryView()
		{
			if (this.Security.HasRight(RIGHT.OPERATE_VIEW_MOVEMENT_HISTORY))
			{
				var model = GetBlankModel(this.Security);
				return this.PartialViewWithErrorMessages("MovementHistoryTabView", model, JsonRequestBehavior.AllowGet);
			}
			else
			{
				this.OnError(this.GetTranslatedText("You have no rights to access this screen."));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

		}

		/// <summary>
		/// This method is called by the UI to retrieve the movement history data.
		/// </summary>
		/// <param name="orderDir"></param>
		/// <param name="columnFilterInfoList"></param>
		/// <param name="originalColumnOrderIndex">The column index to order on.</param>
		/// <param name="draw"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult GetData(string draw
									, int start
									, int length
									, string orderDir
									, List<MovementHistoryTabColumnFilterInfo> columnFilterInfoList
									, int originalColumnOrderIndex
									, bool initialLoadRequest)
		{
			// Initialization.   
			JsonResult result;

			try
			{
				string orderColumnName = this.GetColumnOrderName(originalColumnOrderIndex);

				// Loading.   
				var model = this.GetModel(this.Security, columnFilterInfoList, initialLoadRequest, length, orderColumnName, orderDir);

				// Filter the movement history records based on the column filters.
				var filteredRecordCollection = this.FilterTheModel(model, columnFilterInfoList);
				List<MovementHistoryTabRow> data = filteredRecordCollection;

				// Total record count.   
				int totalRecords = data.Count;

				// Filter record count.   
				int recFilter = data.Count;

				// Apply pagination.   
				data = data.Skip(start).Take(length).ToList();

				// In case we have filtered out first record with date range info saved
				if(data.Count > 0 && model.MovementHistories.Count > 0 ) {
					data[0].MaxDateTimeStr = model.MovementHistories[0].MaxDateTimeStr;
                    data[0].MinDateTimeStr = model.MovementHistories[0].MinDateTimeStr;
                }

                // Loading drop down lists.   
                result = this.Json(new
				{
					draw = Convert.ToInt32(draw),
					recordsTotal = totalRecords,
					recordsFiltered = recFilter,
					data = data
				}, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				string msg = "MovementHistoryTabController: Error getting movement history data. " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

				result = this.Json(new
				{
					draw = 0,
					recordsTotal = 0,
					recordsFiltered = 0,
					data = 0
				}, JsonRequestBehavior.AllowGet);
			}

			// Return info.   
			return result;
		}

		/// <summary>
		/// This method is called by the UI to retrieve the column filters for a selected column.
		/// </summary>
		/// <param name="selectedColumn">The selected column index.</param>
		/// <param name="filterInfo">The filter info that contains the date range.</param>
		/// <returns>Returns a list of column filters.</returns>
		[HttpPost]
		public ActionResult MovementHistoryTabColumnFilterGetFilter(int selectedColumn, List<MovementHistoryTabColumnFilterInfo> filterInfo)
		{
			List<string> filterList = this.ColumnFilterDataHelper(selectedColumn, filterInfo);

			return this.Json(filterList);
		}

		/// <summary>
		/// This method updates the comment for a given movement.
		/// </summary>
		/// <param name="timeStampTicks"></param>
		/// <param name="movementHistoryRecordGuid"></param>
		/// <param name="comment"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult UpdateComment(string movementHistoryRecordGuidStr, string comment)
		{
			string commentUserId = this.Security.UserID;
			DateTime commentDateTime = DateTime.UtcNow;

			if(string.IsNullOrEmpty(movementHistoryRecordGuidStr))
			{
				this.OnError(this.GetTranslatedText("Error: Movement History record Guid is empty."));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			if (Guid.TryParse(movementHistoryRecordGuidStr, out Guid movementHistoryRecordGuid) == false)
			{
				this.OnError(this.GetTranslatedText("Error: Movement History record Guid cannot be parse."));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			try
			{
				FMChannelHelper.MakeCall<IMovementHistories>(x => x.UpdateComment(this.Security, movementHistoryRecordGuid, comment, commentUserId, commentDateTime));

				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
				var timeStr = this.ConvertUtcToLocalTime(commentDateTime, site);
				var ret = new Tuple<string, string>(commentUserId, timeStr);

				return this.Json(ret);
			}
			catch(Exception ex)
			{
				string msg = this.GetTranslatedText("Error: Movement History cannot update the Comment.");
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + ": " + ex.Message, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

				this.OnError(msg);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		public ActionResult SaveViewState(string jsonViewState)
		{
			MovementHistoryUserViewStateSettings movementHistoryViewStateSettings = new MovementHistoryUserViewStateSettings
			{
				JsonViewState = jsonViewState
			};

			this.SaveViewStateSettings(this.Security, movementHistoryViewStateSettings);
			return this.JsonWithErrorMessages(null);
		}

		[HttpPost]
		public ActionResult PrintMovementTicket(Guid movementHistoryGuid)
		{
			try
			{
				FMChannelHelper.MakeCall<IMovementHistories>(x => x.PrintMovementTicket(this.Security, movementHistoryGuid, false));
			}
			catch (Exception ex)
			{
				string msg = "MovementProcessor - Error printing Movement History. " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry("", FMEventLogEntryType.Error));
			}

			return this.JsonWithErrorMessages(null);
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will convert the date time to the site's format to the site
		/// configured format for local time.
		/// </summary>
		/// <param name="dateTime">The database date/time.</param>
		/// <param name="site">The site object used to convert.</param>
		/// <returns>Return the date time as a string.</returns>
		private string ConvertDateTimeToLocalTime(DateTimeOffset? dateTime, SiteClass site)
		{
			if(dateTime == null)
            {
				return string.Empty;
            }

			var localTime = dateTime.Value.ToLocalTime();
			var siteTimeZone = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
			DateTimeOffset convertedDate = TimeZoneInfo.ConvertTime(localTime, siteTimeZone);

			string localTimeStr = convertedDate.ToString(site.TimePattern);
			string localDateStr = convertedDate.ToString(site.ShortDatePattern);
			string localDateTimeStr = localDateStr + " " + localTimeStr;

			return localDateTimeStr;
		}

		/// <summary>
		/// This method will convert the UTC date time to the site's format to the site
		/// configured format for local time.
		/// </summary>
		/// <param name="dateTime">The database date/time in UTC.</param>
		/// <param name="site">The site object used to convert.</param>
		/// <returns>Return the date time as a string.</returns>
		private string ConvertUtcToLocalTime(DateTime? dateTime, SiteClass site)
		{
			if (dateTime == null)
			{
				return string.Empty;
			}

			var localTime = dateTime.Value.ToLocalTime();
			var siteTimeZone = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
			DateTimeOffset convertedDate = TimeZoneInfo.ConvertTime(localTime, siteTimeZone);

			string localTimeStr = convertedDate.ToString(site.TimePattern);
			string localDateStr = convertedDate.ToString(site.ShortDatePattern);
			string localDateTimeStr = localDateStr + " " + localTimeStr;

			return localDateTimeStr;
		}

		/// <summary>
		/// This method will convert the archive unit name which is actually the engineering
		/// unit enum name and return the appropriate unit abbreviation.  If not found it 
		/// will return the input name.
		/// </summary>
		/// <param name="unitIndex">The integer unit index.</param>
		/// <returns>Returns the real unit name.</returns>
		private string GetEngineeringUnitsAbbreviation(int? unitIndex)
		{
			if (unitIndex == null)
			{
				return string.Empty;
			}

			var engineeringUnit = (EngineeringUnit)unitIndex.Value;
			return EngineeringUnits.GetUnitAbbreviation(engineeringUnit);
		}


		/// <summary>
		/// This method will return thje TargeT Unit Type
		/// </summary>
		/// <param name="unitIndex">The integer unit index.</param>
		/// <returns>Returns the unit type.</returns>
		private EngineeringUnitType GetTargetUnitsType(int? unitIndex)
		{
			if (unitIndex == null)
			{
				return EngineeringUnitType.FmuNone;
			}

			var engineeringUnit = (EngineeringUnit)unitIndex.Value;
			if(engineeringUnit >= EngineeringUnit.FmlFtIn8Th
			&& engineeringUnit <= EngineeringUnit.FmlMile)
			{
				return EngineeringUnitType.FmuLength;
			}
			else if(engineeringUnit >= EngineeringUnit.FmvCm3
			&& engineeringUnit <= EngineeringUnit.FmvMsFt3)
			{
				return EngineeringUnitType.FmuVolume;
			}

			return EngineeringUnitType.FmuNone;
		}

		/// <summary>
		/// This method will return the number formated based on the site's settings.
		/// </summary>
		/// <param name="dataValue">The number to be formatted.</param>
		/// <param name="site">The site used to perform the conversion.</param>
		/// <param name="unitType">The unit type associated to the number.</param>
		/// <returns>Returns a string with the formatted number.</returns>
		private string GetFormattedValue(object dataValue, SiteClass site, SITE_VARIABLE_TYPE unitType, int? decimalPlaces)
		{
			if (dataValue == null || dataValue.Equals(string.Empty))
			{ 
				return string.Empty; 
			}

			if (dataValue is double)
			{
				var format = site.GetNumberFormatInfo(unitType);
				format.NumberDecimalDigits = decimalPlaces ?? format.NumberDecimalDigits;
                return ((double)dataValue).ToString("N", format); 
			}

			if (dataValue is int)
			{
                var format = site.GetNumberFormatInfo(unitType);
                format.NumberDecimalDigits = decimalPlaces ?? format.NumberDecimalDigits;
                return ((int)dataValue).ToString("G", format);
			}

			return string.Empty;
		}

		/// <summary>
		/// This method will convert a percent to a string.
		/// </summary>
		/// <param name="percentIn">The percent as a double.</param>
		/// <returns>Returns the fomatted percent value.</returns>
		private string GetPercentFormattedValue(double? percentIn)
        {
			if(percentIn == null)
            {
				return string.Empty;
            }

			string convertedValue = percentIn.Value.ToString("#.#") + "%";
			return convertedValue;
        }

		/// <summary>
		/// This method will convert a double to a level in feet/inches/16th or 8th.
		/// </summary>
		/// <param name="inUnitIndex">The engineer unit index.</param>
		/// <param name="levelDouble">The level value to convert from double.</param>
		/// <returns>Returns a string containing the level.</returns>
		private string ConvertToLevelValue(int? inUnitIndex, double? levelDouble)
		{
			if (levelDouble == null)
			{
				return string.Empty;
			}

			EngineeringUnit unitIndex = EngineeringUnit.FmlFtIn16Th;

			if (inUnitIndex != null)
            {
				unitIndex = (EngineeringUnit)inUnitIndex.Value;
            }

			double feet;
			double inches;
			int inchesInt;
			string level;

			switch (unitIndex)
			{
				case EngineeringUnit.FmlFtIn8Th:
					feet = Math.Floor(levelDouble.Value);
					inches = (levelDouble.Value - feet) * 12;
					double eighth = (inches - Math.Floor(inches)) / 0.125;
					int eightInt = (int)Math.Round(eighth, MidpointRounding.AwayFromZero);
					inchesInt = (int)inches;

					level = (int)feet + "-" + (inchesInt >= 10 ? inchesInt.ToString() : "0" + inchesInt) + "-" + (eightInt >= 10 ? eightInt.ToString() : "0" + eightInt);
					return level;

				case EngineeringUnit.FmlFtIn16Th:
					feet = Math.Floor(levelDouble.Value);
					inches = (levelDouble.Value - feet) * 12;
					double sixtenths = (inches - Math.Floor(inches)) / 0.0625;
					int sixteenthsInt = (int)Math.Round(sixtenths, MidpointRounding.AwayFromZero);
					inchesInt = (int)inches;

					level = (int)feet + "-" + (inchesInt >= 10 ? inchesInt.ToString() : "0" + inchesInt) + "-" + (sixteenthsInt >= 10 ? sixteenthsInt.ToString() : "0" + sixteenthsInt);
					return level;
			}

			return string.Empty;
		}

		/// <summary>
		/// This method will return the transfer mode as a word.
		/// </summary>
		/// <param name="inTransferMode">The transfer mode index.</param>
		/// <returns>Return the transfer mode in English.</returns>
		private string GetTransferMode(int? inTransferMode)
        {
			if(inTransferMode == null)
            {
				return string.Empty;
            }

			switch((TransferModes) inTransferMode.Value)
            {
				case TransferModes.Batch:
					return "Batch";
				case TransferModes.Inactive:
					return "Inactive";
				case TransferModes.Level:
					return "Level";
				default:
					return string.Empty;
            }
        }

		/// <summary>
		/// This method will return the transfer status in English.
		/// </summary>
		/// <param name="inTransferStatus">The transfer status index.</param>
		/// <returns>Return the transfer status in English.</returns>
		private string GetTransferStatus(int? inTransferStatus)
        {
			if (inTransferStatus == null)
			{
				return string.Empty;
			}

			switch ((TransferStatuses)inTransferStatus.Value)
			{
				case TransferStatuses.Complete:
					return "Complete";
				case TransferStatuses.Inactive:
					return "Inactive";
				case TransferStatuses.InProgress:
					return "In Progress";
				case TransferStatuses.TransferTarget:
					return "Transfer Target";
				default:
					return string.Empty;
			}
		}

		/// <summary>
		/// This method will convert the value to a time.
		/// </summary>
		/// <param name="inTime">The in time as a time span (long).</param>
		/// <returns>Returns the time in string format.</returns>
		private string ConvertToTime(long? inTime)
        {
			if(inTime == null)
            {
				return string.Empty;
            }

			DateTime dateTime = new DateTime(inTime.Value);
			string dateTimeStr = dateTime.Hour < 10 ? "0" + dateTime.Hour.ToString() : dateTime.Hour.ToString();
			dateTimeStr = dateTimeStr + ":";
			dateTimeStr = dateTimeStr + (dateTime.Minute < 10 ? "0" + dateTime.Minute.ToString() : dateTime.Minute.ToString());
			dateTimeStr = dateTimeStr + ":";
			dateTimeStr = dateTimeStr + (dateTime.Second < 10 ? "0" + dateTime.Second.ToString() : dateTime.Second.ToString());

			return dateTimeStr;
        }

		/// <summary>
		/// This method will find the earlier date (minimum) and return that date.
		/// </summary>
		/// <param name="currentMinDate">This is the current minimum date.</param>
		/// <param name="movementDate">This is the movement date.</param>
		/// <returns>Returns the earlier date.</returns>
		private DateTimeOffset FindMinDate(DateTimeOffset currentMinDate, DateTimeOffset movementDate)
        {
			int result = DateTimeOffset.Compare(movementDate, currentMinDate);

			// Less than zero means the first date is earlier than the second date in the compare.
			if(result < 0)
            {
				return movementDate;
            }

			return currentMinDate;
		}

		/// <summary>
		/// This method will find the later date (max) and return that date.
		/// </summary>
		/// <param name="currentMaxDate">This is the current max date.</param>
		/// <param name="movementDate">This is the movement date.</param>
		/// <returns>Returns the earlier date.</returns>
		private DateTimeOffset FindMaxDate(DateTimeOffset currentMaxDate, DateTimeOffset movementDate)
		{
			int result = DateTimeOffset.Compare(movementDate, currentMaxDate);

			// Greater than zero means the first date is later than the second date in the compare.
			if (result > 0)
			{
				return movementDate;
			}

			return currentMaxDate;
		}

		/// <summary>
		/// This method will return the site ID for a given site Guid.
		/// </summary>
		/// <param name="siteGuid">The site Guid use to get the site.</param>
		/// <returns>Returns the site ID.</returns>
		private string GetSiteId(Guid siteGuid)
        {
			if(siteGuid == null || siteGuid == Guid.Empty)
            {
				return string.Empty;
            }

			if(siteGuid != this.previousSiteGuid)
            {
				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetBasic(this.Security, siteGuid));

				if(site != null)
                {
					this.previousSiteId = site.ID;
					this.previousSiteGuid = site.IdentityGuid;
					return site.ID;
				}
            }

			return this.previousSiteId;
        }

		/// <summary>
		/// This method will get the filters for auto gauge and hand gauge.
		/// </summary>
		/// <param name="columnFilterInfoList">The filter list to search.</param>
		/// <param name="autogauge">The auto gauge filter</param>
		/// <param name="handgauge">The hand gauge filter</param>
		private void GetRecordFilters(List<MovementHistoryTabColumnFilterInfo> columnFilterInfoList, out bool autogauge, out bool handgauge)
        {
			var filterObj = columnFilterInfoList.Find(x => x.SelectedColumnFilterEnum == MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.RecordType);

			if (filterObj == null)
            {
				autogauge = false;
				handgauge = false;
			}
			else
            {
				autogauge = filterObj.ShowAutoGauge;
				handgauge = filterObj.ShowHandGauge;
			}
		}

		/// <summary>
		/// This method will get the filters for midnight record.
		/// </summary>
		/// <param name="columnFilterInfoList">The filter list to search.</param>
		private bool GetMidnightFilter(List<MovementHistoryTabColumnFilterInfo> columnFilterInfoList)
		{
			var filterObj = columnFilterInfoList.Find(x => x.SelectedColumnFilterEnum == MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.MidnightRecord);

			if (filterObj == null)
			{
				return false;
			}
			else
			{
				return filterObj.ShowMidnightRecord;
			}
		}

		/// <summary>
		/// This method will return the date filters.
		/// </summary>
		/// <param name="columnFilterInfoList">The list of column filters.</param>
		/// <param name="site">The site to use the date info.</param>
		/// <param name="startTime">The start time output.</param>
		/// <param name="endTime">The end time output.</param>
		private void GetDateFilters(List<MovementHistoryTabColumnFilterInfo> columnFilterInfoList, SiteClass site, out DateTime startTime, out DateTime endTime)
        {
			// Set default to one day from now.
			endTime = DateTime.UtcNow;
			startTime = endTime.AddDays(-1);

			var filterObj = columnFilterInfoList.Find(
								x => x.SelectedColumnFilterEnum == MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TimeStamp);

			if (filterObj != null)
			{
				DateTime? fromDate = this.ConvertDateTimeStr(filterObj.FromDateStr, site);
				DateTime? toDate = this.ConvertDateTimeStr(filterObj.ToDateStr, site);

				if (fromDate != null && toDate != null)
				{
					TimeZoneInfo siteZone = site.GetTimeZoneInfo();

					// First convert to the site's time zone.
					var startTimeSiteZone = new DateTimeOffset(fromDate.Value, siteZone.GetUtcOffset(fromDate.Value));
					var endTimeSiteZone = new DateTimeOffset(toDate.Value, siteZone.GetUtcOffset(toDate.Value));

					// Then convert to local time.
					startTime = startTimeSiteZone.UtcDateTime;
					endTime = endTimeSiteZone.UtcDateTime;
				}
			}
		}

		/// <summary>
		/// This method will convert the date string based on the site regional settings.
		/// </summary>
		/// <param name="dateStr">The date string to convert.</param>
		/// <returns></returns>
		private DateTime? ConvertDateTimeStr(string dateStr, SiteClass site)
		{
			if (string.IsNullOrEmpty(dateStr) == false && dateStr.Length >= 14)
			{
				string dateTimeFormat = site.ShortDatePattern + " " + site.TimePattern;
				var mainParts = dateStr.Split(' ');

				if (mainParts.Length == 3)
				{
					if (mainParts[2].Equals(site.PMSymbol))
					{
						mainParts[2] = "PM";
					}

					if (mainParts[2].Equals(site.AMSymbol))
					{
						mainParts[2] = "AM";
					}
				}

				if (mainParts.Length >= 2)
				{
					mainParts[0] = mainParts[0].Replace(site.DateSeparator, "/");
					mainParts[1] = mainParts[1].Replace(site.TimeSeparator, ":");
					string newDateStr = mainParts[0] + " " + mainParts[1];

					if (mainParts.Length == 3)
					{
						newDateStr = newDateStr + " " + mainParts[2];
					}

					try
					{
						var newDateTime = DateTime.ParseExact(newDateStr, dateTimeFormat, CultureInfo.InvariantCulture);
						return newDateTime;
					}
					catch (Exception)
					{
						return null;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// This method will return the column name of the column being order by.
		/// </summary>
		/// <param name="originalColumnOrderIndex">The column index of the column being order by.</param>
		/// <returns>Returns the column name.</returns>
		private string GetColumnOrderName(int originalColumnOrderIndex)
        {
			switch(originalColumnOrderIndex)
            {
				case 0:
					return "TimeStamp";
				case 2:
					return "Name";
				default:
					return "TimeStamp";
            }
        }

		/// <summary>
		/// This method will ensure that the column filter list is unique by only adding
		/// an item if it does not already exist.
		/// </summary>
		/// <param name="columnFilterResults">The column filter results list.</param>
		/// <param name="item">The item to add to the list.</param>
		private void AddColumnFilterHelper(ref List<string> columnFilterResults, string item)
		{
			string localItem = item;

			if(string.IsNullOrEmpty(item))
            {
				localItem = "{Blank}";
            }

			if (columnFilterResults.Contains(localItem))
			{
				return;
			}

			columnFilterResults.Add(localItem);

		}

		/// <summary>
		/// This method will return a list of filters for a selected column.
		/// </summary>
		/// <param name="selectedColumn">The column selected to filter on.</param>
		/// <param name="filterInfo">The filter info that contains the date range.</param>
		/// <returns>Returns a list of column filters for a given column.</returns>
		private List<string> ColumnFilterDataHelper(int selectedColumn, List<MovementHistoryTabColumnFilterInfo> filterInfo)
        {
			var columnFilterResults = new List<string>();
			MovementHistoryTabModel model = this.GetModel(this.Security, filterInfo, false, 0, "TimeStamp", "DESC");

			if(model == null || model.MovementHistories == null || model.MovementHistories.Count == 0)
            {
				return columnFilterResults;
            }

			foreach(MovementHistoryTabRow row in model.MovementHistories)
            {
				switch((MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums)selectedColumn)
                {
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Name:
						this.AddColumnFilterHelper(ref columnFilterResults,row.Name);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Node:
						this.AddColumnFilterHelper(ref columnFilterResults, row.Node);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.InitiationCount:
						if (row.InitiationCount != null) this.AddColumnFilterHelper(ref columnFilterResults, row.InitiationCount.ToString());
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Site:
						this.AddColumnFilterHelper(ref columnFilterResults, row.SiteId);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Comment:
						this.AddColumnFilterHelper(ref columnFilterResults, row.Comment);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutDataModifiedBy:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutDataModifiedBy);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutDensityProductInAir:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutDensityProductInAirStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutDensityProductObserved:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutDensityProductObservedStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutDensityProductObservedTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutDensityProductObservedTimeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutDensityProductStandard:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutDensityProductStandardStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutDensityProductStandardTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutDensityProductStandardTimeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutDensityProductStandardInAir:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutDensityProductStandardInAirStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutLevelProduct:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutLevelProductStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutLevelProductTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutLevelProductTimeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutLevelWater:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutLevelWaterStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutMassLiquid:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutMassLiquidStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutPercentBsw:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutPercentBswStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutRoofMass:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutRoofMassStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTankShellCorrection:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutTankShellCorrectionStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTemperatureAmbient:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutTemperatureAmbientStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTemperatureAmbientTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutTemperatureAmbientTimeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTemperatureDensity:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutTemperatureDensityStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTemperatureProduct:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutTemperatureProductStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutTimeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTransferGov:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutTransferGovStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTransferNsv:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutTransferNsvStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTransferVolumeWater:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutTransferVolumeWaterStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeBsw:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutVolumeBswStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeCorrectionFactor:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutVolumeCorrectionFactorStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeGrossObserved:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutVolumeGrossObservedStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeGrossStandard:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutVolumeGrossStandardStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeNetStandard:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutVolumeNetStandardStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeRoofCorrection:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutVolumeRoofCorrectionStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeTotalObserved:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutVolumeTotalObservedStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeWater:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CloseoutVolumeWaterStr);
						break;
					//case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.MovementType:
					//	break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.OrderNumber:
						this.AddColumnFilterHelper(ref columnFilterResults, row.OrderNumber);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.PlannedStartTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.PlannedStartTimeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Product:
						this.AddColumnFilterHelper(ref columnFilterResults, row.Product);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.ProductDescription:
						this.AddColumnFilterHelper(ref columnFilterResults, row.ProductDescription);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartTimeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartDensityProductObserved:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartDensityProductObservedStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartDensityProductObservedTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartDensityProductObservedTimeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartDensityProductObservedInAir:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartDensityProductObservedInAirStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartDensityProductStandard:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartDensityProductStandardStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartDensityProductStandardTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartDensityProductStandardTimeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartUserId:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartUserID);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartLevelProduct:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartLevelProductStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartLevelProductTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartLevelProductTimeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartLevelWater:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartLevelWaterStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartLevelWaterTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartLevelWaterTimeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartMassLiquid:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartMassLiquidStr);
						break;
                    case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartPercentBsw:
                        this.AddColumnFilterHelper(ref columnFilterResults, row.StartPercentBswStr);
                        break;
                    case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTankShellCorrection:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartTankShellCorrectionStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTemperatureAmbient:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartTemperatureAmbientStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTemperatureAmbientTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartTemperatureAmbientTimeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTemperatureProduct:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartTemperatureProductStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTemperatureProductTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartTemperatureProductTimeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTemperatureDensity:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartTemperatureDensityStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTemperatureDensityTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartTemperatureDensityTimeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeCorrectionFactor:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartVolumeCorrectionFactorStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolume:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartVolumeStr);
						break;
                    case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeBsw:
                        this.AddColumnFilterHelper(ref columnFilterResults, row.StartVolumeBswStr);
                        break;
                    case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeGrossObserved:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartVolumeGrossObservedStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeGrossStandard:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartVolumeGrossStandardStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeNetStandard:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartVolumeNetStandardStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeRoofCorrection:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartVolumeRoofCorrectionStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeTotalObserved:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartVolumeTotalObservedStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeWater:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StartVolumeWaterStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StopTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.StopTimeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferDeviation:
						this.AddColumnFilterHelper(ref columnFilterResults, row.TransferDeviationStr);
						break;
                    case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferPercentDeviation:
                        this.AddColumnFilterHelper(ref columnFilterResults, row.TransferPercentDeviationStr);
                        break;
                    case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferDirection:
						this.AddColumnFilterHelper(ref columnFilterResults, row.TransferDirection);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferMode:
						this.AddColumnFilterHelper(ref columnFilterResults, row.TransferModeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferStatus:
						this.AddColumnFilterHelper(ref columnFilterResults, row.TransferStatusStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferTarget:
						this.AddColumnFilterHelper(ref columnFilterResults, row.TransferTargetStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferTargetUnits:
						this.AddColumnFilterHelper(ref columnFilterResults, row.TransferTargetUnits);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferLevelTarget:
						this.AddColumnFilterHelper(ref columnFilterResults, row.TransferLevelTargetStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferVolumeTarget:
						this.AddColumnFilterHelper(ref columnFilterResults, row.TransferVolumeTargetStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferTimeRemaining:
						this.AddColumnFilterHelper(ref columnFilterResults, row.TransferTimeRemainingStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferredVolumeWater:
						this.AddColumnFilterHelper(ref columnFilterResults, row.TransferredVolumeWaterStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferredVolume:
						this.AddColumnFilterHelper(ref columnFilterResults, row.TransferredVolumeStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsLevelProduct:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UnitsLevelProduct);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsTemperatureAmbient:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UnitsTemperatureAmbient);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsTemperatureDensity:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UnitsTemperatureDensity);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsTemperatureProduct:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UnitsTemperatureProduct);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsDensityProductObserved:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UnitsDensityProductStandard);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsDensityProductStandard:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UnitsDensityProductStandard);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsVolume:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UnitsVolume);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsMass:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UnitsMass);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData01:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UserData01);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData02:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UserData02);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData03:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UserData03);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData04:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UserData04);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData05:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UserData05);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData06:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UserData06);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData07:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UserData07);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData08:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UserData08);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData09:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UserData09);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData10:
						this.AddColumnFilterHelper(ref columnFilterResults, row.UserData10);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.VolumeWater:
						this.AddColumnFilterHelper(ref columnFilterResults, row.VolumeWaterStr);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CommentUserName:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CommentUserName);
						break;
					case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CommentDateTime:
						this.AddColumnFilterHelper(ref columnFilterResults, row.CommentDateTimeStr);
						break;
				}
            }

			return columnFilterResults;
		}

		/// <summary>
		/// This method will compare the filter list to the data in the history table to determine
		/// if there is a match.
		/// </summary>
		/// <param name="modelRowColumnValue">The column value to compare.</param>
		/// <param name="filterCollection">The list of filter items for comparison.</param>
		/// <returns>Returns true if there is a match. Otherwise, returns false.</returns>
		private bool HasMatchingFilter(string modelRowColumnValue, List<string> filterCollection)
        {
			string compareValue = modelRowColumnValue;

			// Testing for a blank value.
			if (string.IsNullOrEmpty(modelRowColumnValue))
            {
				compareValue = "{Blank}";
            }

			return filterCollection.Contains(compareValue);
		}

		/// <summary>
		/// This method will AND the match results to determine if a row needs
		/// to be added based on the filters.
		/// </summary>
		/// <param name="hasMatchList">The list of has matches.</param>
		/// <returns>Return true if all the filters matched.</returns>
		private bool HasMatches(List<bool> hasMatchList)
		{
			bool matched = true;

			foreach (bool matchResult in hasMatchList)
			{
				matched &= matchResult;
			}

			return matched;
		}

		/// <summary>
		/// This method will filter the movement history grid based on the column filters.
		/// </summary>
		/// <param name="model">The model to filter.</param>
		/// <param name="filterInfo">The filtering information.</param>
		/// <returns>Returns a new movement history collection</returns>
		private List<MovementHistoryTabRow> FilterTheModel(MovementHistoryTabModel model, List<MovementHistoryTabColumnFilterInfo> filterInfo)
        {
			// No history records found.
			if(model.MovementHistories.Count == 0)
            {
				return model.MovementHistories;
            }

			bool hasFilters = false;

			// Check for column filters which is determined by the filter collection count.
			foreach(MovementHistoryTabColumnFilterInfo columnFilterInfo in filterInfo)
            {
				var columnIndex = (MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums)columnFilterInfo.Index;

				// The TimeStamp is handled by the DB call, so we can skip it.
				if (columnFilterInfo.FilterCollection.Count > 0 && columnIndex != MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TimeStamp)
                {
					hasFilters = true;
					break;
                }
            }

			// If there are no filters, just return the original collection.
			if(hasFilters == false)
            {
				return model.MovementHistories;
			}

			var filteredMovementHistories = new List<MovementHistoryTabRow>();

			// Perform filtering
			foreach (MovementHistoryTabRow row in model.MovementHistories)
			{
				bool hasMatch = false;
				var hasMatchList = new List<bool>();

				foreach (MovementHistoryTabColumnFilterInfo columnFilterInfo in filterInfo)
				{
					// Ignore filtering if the filter collection for a given column is empty.
					if(columnFilterInfo.FilterCollection.Count == 0)
                    {
						continue;
                    }

					switch ((MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums)columnFilterInfo.Index)
					{
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TimeStamp:
							continue;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.RecordType:
							continue;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.MidnightRecord:
							continue;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Name:
							hasMatch = this.HasMatchingFilter(row.Name, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Node:
							hasMatch = this.HasMatchingFilter(row.Node, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.InitiationCount:
							if(row.InitiationCount == null) continue;
							string initCountStr = row.InitiationCount.Value.ToString();
							hasMatch = this.HasMatchingFilter(initCountStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Site:
							hasMatch = this.HasMatchingFilter(row.SiteId, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Comment:
							hasMatch = this.HasMatchingFilter(row.Comment, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutDataModifiedBy:
							hasMatch = this.HasMatchingFilter(row.CloseoutDataModifiedBy, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutDensityProductInAir:
							hasMatch = this.HasMatchingFilter(row.CloseoutDensityProductInAirStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutDensityProductObserved:
							hasMatch = this.HasMatchingFilter(row.CloseoutDensityProductObservedStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutDensityProductObservedTime:
							hasMatch = this.HasMatchingFilter(row.CloseoutDensityProductObservedTimeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutDensityProductStandard:
							hasMatch = this.HasMatchingFilter(row.CloseoutDensityProductStandardStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutDensityProductStandardTime:
							hasMatch = this.HasMatchingFilter(row.CloseoutDensityProductStandardTimeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutDensityProductStandardInAir:
							hasMatch = this.HasMatchingFilter(row.CloseoutDensityProductStandardInAirStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutLevelProduct:
							hasMatch = this.HasMatchingFilter(row.CloseoutLevelProductStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutLevelProductTime:
							hasMatch = this.HasMatchingFilter(row.CloseoutLevelProductTimeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutLevelWater:
							hasMatch = this.HasMatchingFilter(row.CloseoutLevelWaterStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutMassLiquid:
							hasMatch = this.HasMatchingFilter(row.CloseoutMassLiquidStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutPercentBsw:
							hasMatch = this.HasMatchingFilter(row.CloseoutPercentBswStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutRoofMass:
							hasMatch = this.HasMatchingFilter(row.CloseoutRoofMassStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTankShellCorrection:
							hasMatch = this.HasMatchingFilter(row.CloseoutTankShellCorrectionStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTemperatureAmbient:
							hasMatch = this.HasMatchingFilter(row.CloseoutTemperatureAmbientStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTemperatureAmbientTime:
							hasMatch = this.HasMatchingFilter(row.CloseoutTemperatureAmbientTimeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTemperatureDensity:
							hasMatch = this.HasMatchingFilter(row.CloseoutTemperatureDensityStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTemperatureProduct:
							hasMatch = this.HasMatchingFilter(row.CloseoutTemperatureProductStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTime:
							hasMatch = this.HasMatchingFilter(row.CloseoutTemperatureProductStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTransferGov:
							hasMatch = this.HasMatchingFilter(row.CloseoutTransferGovStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTransferNsv:
							hasMatch = this.HasMatchingFilter(row.CloseoutTransferNsvStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutTransferVolumeWater:
							hasMatch = this.HasMatchingFilter(row.CloseoutTransferVolumeWaterStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeBsw:
							hasMatch = this.HasMatchingFilter(row.CloseoutVolumeBswStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeCorrectionFactor:
							hasMatch = this.HasMatchingFilter(row.CloseoutVolumeCorrectionFactorStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeGrossObserved:
							hasMatch = this.HasMatchingFilter(row.CloseoutVolumeGrossObservedStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeGrossStandard:
							hasMatch = this.HasMatchingFilter(row.CloseoutVolumeGrossStandardStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeNetStandard:
							hasMatch = this.HasMatchingFilter(row.CloseoutVolumeNetStandardStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeRoofCorrection:
							hasMatch = this.HasMatchingFilter(row.CloseoutVolumeRoofCorrectionStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeTotalObserved:
							hasMatch = this.HasMatchingFilter(row.CloseoutVolumeTotalObservedStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CloseoutVolumeWater:
							hasMatch = this.HasMatchingFilter(row.CloseoutVolumeWaterStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						//case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.MovementType:
						//	break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.OrderNumber:
							hasMatch = this.HasMatchingFilter(row.OrderNumber, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.PlannedStartTime:
							hasMatch = this.HasMatchingFilter(row.PlannedStartTimeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.Product:
							hasMatch = this.HasMatchingFilter(row.Product, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.ProductDescription:
							hasMatch = this.HasMatchingFilter(row.ProductDescription, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTime:
							hasMatch = this.HasMatchingFilter(row.StartTimeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartDensityProductObserved:
							hasMatch = this.HasMatchingFilter(row.StartDensityProductObservedStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartDensityProductObservedTime:
							hasMatch = this.HasMatchingFilter(row.StartDensityProductObservedTimeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartDensityProductObservedInAir:
							hasMatch = this.HasMatchingFilter(row.StartDensityProductObservedInAirStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartDensityProductStandard:
							hasMatch = this.HasMatchingFilter(row.StartDensityProductStandardStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartDensityProductStandardTime:
							hasMatch = this.HasMatchingFilter(row.StartDensityProductStandardTimeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartUserId:
							hasMatch = this.HasMatchingFilter(row.StartUserID, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartLevelProduct:
							hasMatch = this.HasMatchingFilter(row.StartLevelProductStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartLevelProductTime:
							hasMatch = this.HasMatchingFilter(row.StartLevelProductTimeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartLevelWater:
							hasMatch = this.HasMatchingFilter(row.StartLevelWaterStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartLevelWaterTime:
							hasMatch = this.HasMatchingFilter(row.StartLevelWaterTimeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartMassLiquid:
							hasMatch = this.HasMatchingFilter(row.StartMassLiquidStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
                        case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartPercentBsw:
                            hasMatch = this.HasMatchingFilter(row.StartPercentBswStr, columnFilterInfo.FilterCollection);
                            hasMatchList.Add(hasMatch);
                            break;
                        case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTankShellCorrection:
							hasMatch = this.HasMatchingFilter(row.StartTankShellCorrectionStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTemperatureAmbient:
							hasMatch = this.HasMatchingFilter(row.StartTemperatureAmbientStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTemperatureAmbientTime:
							hasMatch = this.HasMatchingFilter(row.StartTemperatureAmbientTimeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTemperatureProduct:
							hasMatch = this.HasMatchingFilter(row.StartTemperatureProductStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTemperatureProductTime:
							hasMatch = this.HasMatchingFilter(row.StartTemperatureProductTimeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTemperatureDensity:
							hasMatch = this.HasMatchingFilter(row.StartTemperatureDensityStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartTemperatureDensityTime:
							hasMatch = this.HasMatchingFilter(row.StartTemperatureDensityTimeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
                        case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeBsw:
                            hasMatch = this.HasMatchingFilter(row.StartVolumeBswStr, columnFilterInfo.FilterCollection);
                            hasMatchList.Add(hasMatch);
                            break;
                        case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeCorrectionFactor:
							hasMatch = this.HasMatchingFilter(row.StartVolumeCorrectionFactorStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeGrossObserved:
							hasMatch = this.HasMatchingFilter(row.StartVolumeGrossObservedStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeGrossStandard:
							hasMatch = this.HasMatchingFilter(row.StartVolumeGrossStandardStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeNetStandard:
							hasMatch = this.HasMatchingFilter(row.StartVolumeNetStandardStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeRoofCorrection:
							hasMatch = this.HasMatchingFilter(row.StartVolumeRoofCorrectionStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeTotalObserved:
							hasMatch = this.HasMatchingFilter(row.StartVolumeTotalObservedStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StartVolumeWater:
							hasMatch = this.HasMatchingFilter(row.StartVolumeWaterStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.StopTime:
							hasMatch = this.HasMatchingFilter(row.StopTimeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferDeviation:
							hasMatch = this.HasMatchingFilter(row.TransferDeviationStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
                  case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferPercentDeviation:
                        hasMatch = this.HasMatchingFilter(row.TransferPercentDeviationStr, columnFilterInfo.FilterCollection);
                        hasMatchList.Add(hasMatch);
                        break;
                  case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferDirection:
							hasMatch = this.HasMatchingFilter(row.TransferDirection, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferMode:
							hasMatch = this.HasMatchingFilter(row.TransferModeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferStatus:
							hasMatch = this.HasMatchingFilter(row.TransferStatusStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferTarget:
							hasMatch = this.HasMatchingFilter(row.TransferTargetStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferTargetUnits:
							hasMatch = this.HasMatchingFilter(row.TransferTargetUnits, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferLevelTarget:
							hasMatch = this.HasMatchingFilter(row.TransferLevelTargetStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferVolumeTarget:
							hasMatch = this.HasMatchingFilter(row.TransferVolumeTargetStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.TransferTimeRemaining:
							hasMatch = this.HasMatchingFilter(row.TransferTimeRemainingStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsLevelProduct:
							hasMatch = this.HasMatchingFilter(row.UnitsLevelProduct, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsTemperatureAmbient:
							hasMatch = this.HasMatchingFilter(row.UnitsTemperatureAmbient, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsTemperatureDensity:
							hasMatch = this.HasMatchingFilter(row.UnitsTemperatureDensity, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsTemperatureProduct:
							hasMatch = this.HasMatchingFilter(row.UnitsTemperatureProduct, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsDensityProductObserved:
							hasMatch = this.HasMatchingFilter(row.UnitsDensityProductObserved, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsDensityProductStandard:
							hasMatch = this.HasMatchingFilter(row.UnitsDensityProductStandard, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsVolume:
							hasMatch = this.HasMatchingFilter(row.UnitsVolume, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UnitsMass:
							hasMatch = this.HasMatchingFilter(row.UnitsMass, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData01:
							hasMatch = this.HasMatchingFilter(row.UserData01, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData02:
							hasMatch = this.HasMatchingFilter(row.UserData02, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData03:
							hasMatch = this.HasMatchingFilter(row.UserData03, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData04:
							hasMatch = this.HasMatchingFilter(row.UserData04, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData05:
							hasMatch = this.HasMatchingFilter(row.UserData05, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData06:
							hasMatch = this.HasMatchingFilter(row.UserData06, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData07:
							hasMatch = this.HasMatchingFilter(row.UserData07, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData08:
							hasMatch = this.HasMatchingFilter(row.UserData08, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData09:
							hasMatch = this.HasMatchingFilter(row.UserData09, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.UserData10:
							hasMatch = this.HasMatchingFilter(row.UserData10, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CommentUserName:
							hasMatch = this.HasMatchingFilter(row.CommentUserName, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
						case MovementHistoryTabColumnFilterInfo.ColumnFilterNameEnums.CommentDateTime:
							hasMatch = this.HasMatchingFilter(row.CommentDateTimeStr, columnFilterInfo.FilterCollection);
							hasMatchList.Add(hasMatch);
							break;
					}
				}

				// Add model history row to the filtered list;
				if(this.HasMatches(hasMatchList))
                {
					// Check for the record type of a Node type. If so, we need to find its parent to
					// add to the filtered list.
					if(row.RecordType == (int)MovementHistoryDO.MovementRecordTypes.Node)
                    {
						MovementHistoryTabRow parentRow = model.MovementHistories.Find(
													x => x.MovementHistoryGuid == row.ParentGuid && x.RecordType == (int)MovementHistoryDO.MovementRecordTypes.Movement);

						// Parent row should never be null.
						if(parentRow != null)
                        {
							// Check to see if the parent row already exists in the filtered list.
							// If not, then add the parent row.
							var filteredParentRow = filteredMovementHistories.Find(
												x => x.MovementHistoryGuid == parentRow.MovementHistoryGuid && x.RecordType == (int)MovementHistoryDO.MovementRecordTypes.Movement);

							// Parent not found in filter list, therefore add it.
							if(filteredParentRow == null)
                            {
								filteredMovementHistories.Add(parentRow);
							}
						}
					}

					filteredMovementHistories.Add(row);
                }
			}

			return filteredMovementHistories;
        }
		#endregion
	}
}