namespace FuelsManager.Areas.AssetTrackingArea.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Web.Mvc;
	using System.Data;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;
	using System.Linq;
	using System.Net.Http;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using ViewModels;
	using Areas.Controllers;

	using Newtonsoft.Json;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using TankClass = FMBusinessObjects.DataObjects.TankClass;
	using TankCollectionClass = FMBusinessObjects.DataObjects.TankCollectionClass;

	public class AssetMapsController : FMBaseController
	{
		#region Private members
		private List<AssetTrackingDeviceClass> assetTrackingDeviceCollection;
		private SiteClass site;
		private enum Buttons { None, Refresh };
		private bool initialRequestFromMenu;

		private const string EquipmentIconNameKey				= "EquipmentIconDefaultName";
		private const string EquipmentVarianceIconNameKey		= "EquipmentVarianceIconDefaultName";
		private const string EquipmentInvestigationIconNameKey	= "EquipmentInvestigationIconDefaultName";
		private const string TankIconNameKey					= "TankIconDefaultName";
		private const string FacilityIconNameKey				= "FacilityIconDefaultName";
		private const string DeliveryLocationIconNameKey		= "DeliveryLocationIconDefaultName";
		private const string BreadcrumbIconNameKey				= "BreadcrumbIconDefaultName";
		private const string BreadcrumbVarianceIconNameKey		= "BreadcrumbVarianceIconDefaultName";
		private const string BreadcrumbInvestigationIconNameKey = "BreadcrumbInvestigationIconDefaultName";

		private const string EquipmentCompleteInvestigationFailedIconNameKey	= "EquipmentCompleteInvestigationFailedIconDefaultName";
		private const string EquipmentCompleteInvestigationPassedIconNameKey	= "EquipmentCompleteInvestigationPassedIconDefaultName";
		private const string BreadcrumbCompleteInvestigationFailedIconNameKey	= "BreadcrumbCompleteInvestigationFailedIconDefaultName";
		private const string BreadcrumbCompleteInvestigationPassedIconNameKey	= "BreadcrumbCompleteInvestigationPassedIconDefaultName";
		#endregion

		// GET: AssetTrackingArea/AssetMaps
		public ActionResult MapBase(AssetMapsViewModel postedModel)
		{
			try
			{
				Buttons buttonPressed = this.WhichButtonWasPressed();
				this.PopulateDropdownModel(postedModel, buttonPressed);
				this.site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetUsingGuid(this.Security, this.Security.SiteGuid));

				// Update the model with new data.
				this.GetSelectedMap(postedModel);

				postedModel.EquipmentModel			= new AssetMapsEquipmentModel();
				postedModel.DeliveryLocationModel	= new AssetMapsDeliveryLocationModel();
				postedModel.FacilityModel			= new AssetMapsFacilityModel();
				postedModel.TankModel				= new AssetMapsTankModel();

				// Data dictionary items that are dynamically created in java script.
				this.DataDictionaryItems(postedModel);

				// Retrieve the map refresh time.
				this.GetRefreshTime(postedModel);

				this.SetLastButtonPressed(postedModel, buttonPressed);

				// Must be executed prior to the Build methods below.
				postedModel.UseExtent = this.SetExtentFlag(postedModel, buttonPressed);

				return this.View(postedModel);
			}
			catch (Exception ex)
			{
				this.ModelState.AddModelError("Error", "Error: " + ex.Message);

				return this.View(postedModel);
			}
		}

		#region Private methods
		/// <summary>
		/// This method handles the equipment history dialog request to get the equipment
		/// history based on the equipment and the filters.
		/// </summary>
		/// <param name="equipmentId">The equipment ID to retrieve the history data.</param>
		/// <param name="fromDate">The from date which is the most current.</param>
		/// <param name="endDate">The end date which is a date in the past.</param>
		/// <param name="breadcrumbSelection">The breadcrump selection value.</param>
		/// <returns>Returns a collection of history items.</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult GetEquipmentHistoryDataJson(string equipmentId, string fromDate, string endDate, string breadcrumbSelection)
		{
			try
			{
				if (this.assetTrackingDeviceCollection == null || this.assetTrackingDeviceCollection.Count == 0)
				{
					this.assetTrackingDeviceCollection = FMChannelHelper.MakeCall<IAssetTrackingDevices, List<AssetTrackingDeviceClass>>(
																x => x.EnumerateAllDevicesLinkedToEquipment(this.Security));
				}

				AssetEquipmentHistoryModel equipmentHistoryModel = this.BuildEquipmentHistoryJson(equipmentId, breadcrumbSelection, fromDate, endDate);
				var jsonResult = this.Json(equipmentHistoryModel, JsonRequestBehavior.AllowGet);
				jsonResult.MaxJsonLength = int.MaxValue;

				return jsonResult;
			}
			catch (Exception ex)
			{
				string msg = "Error (GetEquipmentHistoryDataJson): " + ex.Message;
				this.WriteToEventLog(msg, EventLogEntryType.Error);
				return this.Json(null);
			}
		}

		/// <summary>
		/// The method handles the equipment history start investigate button click.  It will update the 
		/// appropriate asset tracking detail records with the start investigate date and investigate state.
		/// </summary>
		/// <param name="selectedGuids">The selected asset tracking GUIDS to set to investigate state.</param>
		/// <returns>Return a string that is empty or contains an error.</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult UpdateRecordsToInvestigationStateJson(string[] selectedGuids)
		{
			if (selectedGuids == null || selectedGuids.Length == 0)
			{
				return this.Json("No asset tracking records selected.");
			}

			var validatedGuidList = new List<string>();

			if (this.ValidateGuids(selectedGuids, validatedGuidList) == false)
			{
				return this.Json("Invalid GUID in selection list.");
			}

			string returnValue = this.UpdateRecordsToInvestigationState(validatedGuidList);
			return this.Json(returnValue);
		}

		/// <summary>
		/// The method handles the equipment history start investigate button click.  It will update the 
		/// appropriate asset tracking detail records with the start investigate date and investigate state.
		/// </summary>
		/// <param name="deviceId">The device ID to filter on.</param>
		/// <param name="investigateState"></param>
		/// <param name="remarks">Remarks to be saved.</param>
		/// <returns>Return a string that is empty or contains an error.</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult UpdateRecordsToCompleteInvestigationStateJson(string deviceId, int investigateState, string remarks)
		{
			if (string.IsNullOrEmpty(deviceId))
			{
				return this.Json("No asset tracking device ID present.");
			}

			if ((AssetTrackingDetailClass.MessageStates) investigateState != AssetTrackingDetailClass.MessageStates.InvestigateCompletedFailed
				&& (AssetTrackingDetailClass.MessageStates) investigateState != AssetTrackingDetailClass.MessageStates.InvestigateCompletedPassed)
			{
				return this.Json("Invalid investigation state.");
			}

			string returnValue = this.UpdateRecordsToCompleteInvestigationState(deviceId, (AssetTrackingDetailClass.MessageStates) investigateState, remarks);
			return this.Json(returnValue);
		}

		/// <summary>
		/// The method handles the equipment history start investigate button click.  It will update the 
		/// appropriate asset tracking detail with the start investigate date.
		/// </summary>
		/// <param name="assetTrackingDetailGuid">The asset tracking detail GUID used to update the remarks.</param>
		/// <param name="remarks">The remarks to be updated.</param>
		/// <returns>Return a string that is empty or contains an error.</returns>
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult UpdateRemarksJson(string assetTrackingDetailGuid, string remarks)
		{
			string returnValue = this.UpdateRemarksOnDetail(assetTrackingDetailGuid, remarks);
			return this.Json(returnValue);
		}

		/// <summary>
		/// This method will retreive the equipment data.
		/// </summary>
		/// <param name="breadcrumbSelection"></param>
		/// <returns>Returns a list of equipment models.</returns>
		[HttpPost]
		public ActionResult GetEquipmentMapDataJson(string breadcrumbSelection)
		{
			if (this.assetTrackingDeviceCollection == null || this.assetTrackingDeviceCollection.Count == 0)
			{
				this.assetTrackingDeviceCollection = FMChannelHelper.MakeCall<IAssetTrackingDevices, List<AssetTrackingDeviceClass>>(
															x => x.EnumerateAllDevicesLinkedToEquipment(this.Security));
			}

			List<AssetMapsEquipmentModel> equipmentDataList = this.BuildEquipmentSection(breadcrumbSelection);

			var jsonResult = this.Json(equipmentDataList, JsonRequestBehavior.AllowGet);
			jsonResult.MaxJsonLength = int.MaxValue;

			return jsonResult;
		}

		/// <summary>
		/// This method will return the model as a string representation.
		/// </summary>
		/// <param name="model">The map view model to be serialize.</param>
		/// <returns>The serialized string version of the model.</returns>
		[NonAction]
		public static string SerializeModel(AssetMapsViewModel model)
		{
			return JsonConvert.SerializeObject(model);
		}

		/// <summary>
		/// This method will retrieve the facility data.
		/// </summary>
		/// <returns>Returns a list of facility models.</returns>
		[HttpPost]
		public ActionResult GetFacilityMapDataJson()
		{
			List<AssetMapsFacilityModel> facilityDataList = this.BuildFacilitySection();
			return this.Json(facilityDataList);
		}

		/// <summary>
		/// This method will retrieve the tank data.
		/// </summary>
		/// <returns>Returns a list of tank models.</returns>
		[HttpPost]
		public ActionResult GetTankMapDataJson()
		{
			List<AssetMapsTankModel> tankDataList = this.BuildTankSection();
			return this.Json(tankDataList);
		}

		/// <summary>
		/// This method will retrieve the delivery location data.
		/// </summary>
		/// <returns>Returns a list of delivery location models.</returns>
		[HttpPost]
		public ActionResult GetDeliveryLocationMapDataJson()
		{
			List<AssetMapsDeliveryLocationModel> deliverLocationDataList = this.BuildDeliveryLocationSection();
			return this.Json(deliverLocationDataList);
		}

		/// <summary>
		/// This method will build the equipment section for the map.
		/// </summary>
		private List<AssetMapsEquipmentModel> BuildEquipmentSection(string breadcrumbSection)
		{
			var equipmentDataList = new List<AssetMapsEquipmentModel>();
			var localSite = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			List<AssetTrackingDetailClass> assetTrackingDetailList = this.GetDeviceDetail(null, breadcrumbSection, getAll: true);

			if (assetTrackingDetailList != null && assetTrackingDetailList.Count > 0)
			{
				int siteVolumeUnitIndex			= (int)localSite.VolumeUnits;
				int siteDensityUnitIndex		= (int)localSite.DensityUnits;
				int siteVolumeDecimalPlaces		= int.Parse(localSite.VolumeDecimalPlaces);
				int siteDensityDecimalPlaces	= int.Parse(localSite.DensityDecimalPlaces);

				string previousDeviceId = string.Empty;
				var equipmentModel = new AssetMapsEquipmentModel { HasCoordinates = 1 };

				foreach (AssetTrackingDetailClass detailRecord in assetTrackingDetailList)
				{
					if (detailRecord.Latitude != null && detailRecord.Longitude != null)
					{
						if (previousDeviceId.Equals(detailRecord.AssetTrackingDeviceId) == false)
						{
							equipmentModel = new AssetMapsEquipmentModel { HasCoordinates = 1 };
							equipmentDataList.Add(equipmentModel);
							previousDeviceId = detailRecord.AssetTrackingDeviceId;
						}

						string productId			= string.Empty;
						string equipmentId			= string.Empty;
						string productDensityStr	= string.Empty;
						string itemColor			= string.Empty;

						switch (detailRecord.MarkerType)
						{
							case AssetTrackingDetailClass.MarkerTypes.Crumb:
								equipmentModel.MarkerTypeList.Add("C");
								break;
							case AssetTrackingDetailClass.MarkerTypes.Marker:
								{
									equipmentModel.MarkerTypeList.Add("M");

									break;
								}
							default:
								equipmentModel.MarkerTypeList.Add("N");
								break;
						}

						switch (detailRecord.MessageState)
						{
							case AssetTrackingDetailClass.MessageStates.None:
								itemColor = "NORMAL";
								break;
							case AssetTrackingDetailClass.MessageStates.Contaminated:
								itemColor = "ORANGE";
								break;
							case AssetTrackingDetailClass.MessageStates.Investigate:
								itemColor = "YELLOW";
								break;
							case AssetTrackingDetailClass.MessageStates.InvestigateCompletedFailed:
								itemColor = "RED";
								break;
							case AssetTrackingDetailClass.MessageStates.InvestigateCompletedPassed:
								itemColor = "GREEN";
								break;
						}

						equipmentModel.ItemColorList.Add(itemColor);

						EngineeringUnit volumeUnitIndex	= localSite.VolumeUnits;
						AssetTrackingDeviceClass oneDevice = this.assetTrackingDeviceCollection.Find(x => x.DeviceId == detailRecord.AssetTrackingDeviceId);

						if (oneDevice != null)
						{
							if (oneDevice.ProductDensity != null)
							{
								var densityUnitIndex = this.GetAppropriateUnitIndex(siteDensityUnitIndex, oneDevice.EquipmentDensityUnitIndex, oneDevice.ProductDensityUnitIndex);
								double productDensity	= EngineeringUnits.Convert(oneDevice.ProductDensity.Value, EngineeringUnit.FmdKgM3, densityUnitIndex, 15);
								productDensity			= Math.Round(productDensity, siteDensityDecimalPlaces, MidpointRounding.AwayFromZero);
								productDensityStr		= productDensity.ToString(CultureInfo.InvariantCulture);
							}

							equipmentId = oneDevice.EquipmentId;
							productId = oneDevice.ProductId;

							volumeUnitIndex = this.GetAppropriateUnitIndex(siteVolumeUnitIndex, oneDevice.EquipmentVolumeUnitIndex, oneDevice.ProductVolumeUnitIndex);
						}

						equipmentModel.LatitudeList.Add(detailRecord.Latitude.Value);
						equipmentModel.LongitudeList.Add(detailRecord.Longitude.Value);
						equipmentModel.EquipmentId = equipmentId;

						string description = "TYPE_EQUIP|HEADER|" + equipmentId + "|ENDHEADER|"
											+ "PRODUCT|" + productId + "|ENDPRODUCT|";

						if (detailRecord.TrackingTanks != null)
						{
							int tankCount = 1;

							foreach (AssetTrackingTankClass wrdcuTank in detailRecord.TrackingTanks)
							{
								double volume = 0;
								string contimateFlagStr = wrdcuTank.Contaminated ? "YES" : "NO";

								if (string.IsNullOrEmpty(productDensityStr))
								{
									productDensityStr = "n/a";
								}

								if (wrdcuTank.Volume != null)
								{
									volume = EngineeringUnits.Convert(wrdcuTank.Volume.Value, EngineeringUnit.FmvMeter3, volumeUnitIndex, 15);
								}

								volume = Math.Round(volume, siteVolumeDecimalPlaces, MidpointRounding.AwayFromZero);

								description = description
												+ "TANK" + tankCount + "|" + wrdcuTank.TankId + "|"
												+ volume + "|"
												+ productDensityStr + "|"
												+ itemColor + "|"
												+ contimateFlagStr + "|"
												+ "ENDTANK" + tankCount + "|";

								tankCount++;
							}
						}

						description = description + "TIMESTAMP|" + this.FormatSessionDateTime(detailRecord.AssetSessionDateTime) + "|ENDTIMESTAMP";
						equipmentModel.HoverDescriptionList.Add(description);
					}
				}
			}

			return equipmentDataList;
		}

		/// <summary>
		/// This method will build the facility section for the map.
		/// </summary>
		private List<AssetMapsFacilityModel> BuildFacilitySection()
		{
			var facilityList = new List<AssetMapsFacilityModel>();
			var siteCollection = this.GetSites();

			if (siteCollection != null && siteCollection.Count > 0)
			{
				foreach (SiteClass localSite in siteCollection)
				{
					if (localSite.Latitude != null && localSite.Longitude != null)
					{
						var facilityModel = new AssetMapsFacilityModel { HasCoordinates = 1, FacilityId = localSite.ID };
						facilityModel.LatitudeList.Add(localSite.Latitude.Value);
						facilityModel.LongitudeList.Add(localSite.Longitude.Value);

						string description = "TYPE_FACILITY|HEADER|" + localSite.ID + "|ENDHEADER|"
												+ "COORDINATE|"
												+ Math.Round(localSite.Latitude.Value, 4, MidpointRounding.AwayFromZero) + "|"
												+ Math.Round(localSite.Longitude.Value, 4, MidpointRounding.AwayFromZero) + "|ENDCOORDINATE";

						facilityModel.HoverDescriptionList.Add(description);
						facilityList.Add(facilityModel);
					}
				}
			}

			return facilityList;
		}

		/// <summary>
		/// This method will build the facility section for the map.
		/// </summary>
		private List<AssetMapsTankModel> BuildTankSection()
		{
			var tankList = new List<AssetMapsTankModel>();
			var localSite = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			var tankDataSet =
				FMChannelHelper.MakeCall<IAssetTrackingDevices, DataSet>(x => x.EnumerateAllAssociatedTanks(this.Security));

			if (tankDataSet != null && tankDataSet.Tables.Count > 0 && tankDataSet.Tables[0].Rows.Count > 0)
			{
				foreach (DataRow row in tankDataSet.Tables[0].Rows)
				{
					Guid tankGuid = row.IsNull("TankGuid") ? Guid.Empty : (Guid)row["TankGuid"];
					string tankId = row.IsNull("TankID") ? string.Empty : (string)row["TankID"];
					double? latitude = row.IsNull("Latitude") ? null : (double?)row["Latitude"];
					double? longitude = row.IsNull("Longitude") ? null : (double?)row["Longitude"];

				    DateTime messageDateTime;
				    if (row.IsNull("AssetSessionDateTime"))
				    {
                        var createdDate = (DateTimeOffset)row["CreatedDate"];
                        messageDateTime = createdDate.DateTime;
				    }
				    else
				    {
				        messageDateTime = (DateTime)row["AssetSessionDateTime"];
                    }

					if (latitude != null && longitude != null)
					{
						var tank = FMChannelHelper.MakeCall<ITanks, TankClass>(x => x.Get(this.Security, tankGuid));
						var tankModel = new AssetMapsTankModel { HasCoordinates = 1, TankId = tankId };

						tankModel.LatitudeList.Add(latitude.Value);
						tankModel.LongitudeList.Add(longitude.Value);

						string description = "TYPE_TANK|HEADER|" + tankId + "|ENDHEADER|"
												+ "COORDINATE|"
												+ Math.Round(latitude.Value, 4, MidpointRounding.AwayFromZero) + "|"
												+ Math.Round(longitude.Value, 4, MidpointRounding.AwayFromZero) + "|ENDCOORDINATE";

						string additionDescription = this.GetTankProcessVariables(tank, localSite);
						description = description + additionDescription;

					    description = description + "|TIMESTAMP|" + this.FormatSessionDateTime(messageDateTime) + "|ENDTIMESTAMP";

                        tankModel.HoverDescriptionList.Add(description);
						tankList.Add(tankModel);
					}
				}
			}

			return tankList;
		}

		/// <summary>
		/// This method will build the delivery location section for the map.
		/// </summary>
		private List<AssetMapsDeliveryLocationModel> BuildDeliveryLocationSection()
		{
			var deliveryLocationList = new List<AssetMapsDeliveryLocationModel>();
			var iataCodeCollection =
				FMChannelHelper.MakeCall<IIATACodes, IATACodeCollectionClass>(x => x.EnumerateWhereCoordinatesExist(this.Security));

			foreach (IATACodeClass iataCode in iataCodeCollection)
			{
				if (iataCode.Latitude != null && iataCode.Longitude != null)
				{
					var deliveryLocationModel = new AssetMapsDeliveryLocationModel
					                            {
						                            HasCoordinates = 1,
						                            DeliveryLocationId = iataCode.ID
					                            };
					deliveryLocationModel.LatitudeList.Add(iataCode.Latitude.Value);
					deliveryLocationModel.LongitudeList.Add(iataCode.Longitude.Value);

					string description = "TYPE_DELIVERYLOCATION|HEADER|" + iataCode.ID + "|ENDHEADER|"
											+ "COORDINATE|"
											+ Math.Round(iataCode.Latitude.Value, 4, MidpointRounding.AwayFromZero) + "|"
											+ Math.Round(iataCode.Longitude.Value, 4, MidpointRounding.AwayFromZero) + "|ENDCOORDINATE";

					deliveryLocationModel.HoverDescriptionList.Add(description);
					deliveryLocationList.Add(deliveryLocationModel);
				}
			}

			return deliveryLocationList;
		}

		/// <summary>
		/// This method will build the equipment history used for the equipment
		/// history dialog.
		/// </summary>
		/// <param name="selectedEquipmentId"></param>
		/// <param name="breadcrumbState"></param>
		/// <param name="fromDateStr"></param>
		/// <param name="endDateStr"></param>
		/// <summary>
		/// This method will build the equipment history used for the equipment
		/// history dialog.
		/// </summary>
		private AssetEquipmentHistoryModel BuildEquipmentHistoryJson(string selectedEquipmentId, 
																			string breadcrumbState, 
																			string fromDateStr, 
																			string endDateStr)
		{
			var equipmentHistoryModel = new AssetEquipmentHistoryModel();

			if (string.IsNullOrEmpty(selectedEquipmentId))
			{
				return equipmentHistoryModel;
			}

			// Get the user rights for investigate.
			equipmentHistoryModel.HasStartInvestigateRight = this.Security.HasRight(RIGHT.MAP_INITIATE_INVESTIGATION);
			equipmentHistoryModel.HasCompleteInvestigateRight = this.Security.HasRight(RIGHT.MAP_COMPLETE_INVESTIGATION);

			var localSite = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
			var equipGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(x => x.GetIdentityGuid(this.Security, selectedEquipmentId));

			// Get selected equipment.
			List<AssetTrackingDetailClass> assetTrackingDetailList = this.GetOneDetailEquipment(equipGuid.ToString(),
																								breadcrumbState,
																								fromDateStr,
																								endDateStr);

			if (assetTrackingDetailList != null && assetTrackingDetailList.Count > 0)
			{
				int siteVolumeUnitIndex = (int)localSite.VolumeUnits;
				int siteDensityUnitIndex = (int)localSite.DensityUnits;

				equipmentHistoryModel.FoundInvestigateState = this.FoundAnyInvestigateStates(equipGuid.ToString());

				foreach (AssetTrackingDetailClass detailRecord in assetTrackingDetailList)
				{
					if (detailRecord.Latitude != null && detailRecord.Longitude != null)
					{
						string productId					= string.Empty;
						string equipmentId					= string.Empty;
						string productDensityStr			= string.Empty;

						EngineeringUnit volumeUnitIndex	= localSite.VolumeUnits;
						int siteVolumeDecimalPlaces			= int.Parse(localSite.VolumeDecimalPlaces);
						int siteDensityDecimalPlaces		= int.Parse(localSite.DensityDecimalPlaces);

						AssetTrackingDeviceClass oneDevice =
								this.assetTrackingDeviceCollection.Find(x => x.DeviceId == detailRecord.AssetTrackingDeviceId);

						if (oneDevice != null)
						{
							if (oneDevice.ProductDensity != null)
							{
								EngineeringUnit densityUnitIndex = this.GetAppropriateUnitIndex(siteDensityUnitIndex, oneDevice.EquipmentDensityUnitIndex, oneDevice.ProductDensityUnitIndex);

								double productDensity = EngineeringUnits.Convert(oneDevice.ProductDensity.Value, EngineeringUnit.FmdKgM3, densityUnitIndex, 15);
								productDensity = Math.Round(productDensity, siteDensityDecimalPlaces, MidpointRounding.AwayFromZero);
								productDensityStr = productDensity.ToString(CultureInfo.InvariantCulture);
							}

							equipmentId = oneDevice.EquipmentId;
							productId = oneDevice.ProductId;

							volumeUnitIndex = this.GetAppropriateUnitIndex(siteVolumeUnitIndex, oneDevice.EquipmentVolumeUnitIndex, oneDevice.ProductVolumeUnitIndex);
						}

						equipmentHistoryModel.EquipmentID = equipmentId;

						var historyRecordModel = new AssetEquipmentHistoryRecordModel
						{
							AssetTrackingDetailGuidStr		= detailRecord.AssetTrackingDetailGuid.ToString(),
							ProductId						= productId,
							AssetTrackingDeviceId			= detailRecord.AssetTrackingDeviceId,
							GpsCoordinatesStr				= Math.Round(detailRecord.Latitude.Value, 4, MidpointRounding.AwayFromZero) + ", " + Math.Round(detailRecord.Longitude.Value, 4, MidpointRounding.AwayFromZero),
							SessionDatetimeStr				= this.FormatSessionDateTime(detailRecord.AssetSessionDateTime),
							VolumeStr						= "",
							WaterStr						= "NO",
							DensityStr						= productDensityStr,
							IsCompartment					= false,
							IsContaminated					= detailRecord.Contaminated,
							Remarks							= detailRecord.Remarks,
							MessageState					= (int) detailRecord.MessageState
						};

						equipmentHistoryModel.EquipmentHistoryRecordList.Add(historyRecordModel);

						if (detailRecord.TrackingTanks != null)
						{
							int tankCount = 1;
							double totalVolume = 0.0;
							string saveDielectricStr = string.Empty;

							foreach (AssetTrackingTankClass wrdcuTank in detailRecord.TrackingTanks)
							{
								double volume = 0;
								string waterContent = "NO";
								string dielectricStr = string.Empty;

								if (wrdcuTank.Dielectric != null)
								{
									double dielectricRounded = Math.Round(wrdcuTank.Dielectric.Value, 4, MidpointRounding.AwayFromZero);
									dielectricStr = dielectricRounded.ToString(CultureInfo.InvariantCulture);

									// Only consider values to the 1000th place to determine zero.
									int dielectricInt = (int)wrdcuTank.Dielectric.Value * 1000;

									if (dielectricInt == 0)
									{
										waterContent = "YES";
										historyRecordModel.WaterStr = waterContent;
									}

									// Save off the dielectric in case there is only one tank.
									if (tankCount == 1)
									{
										saveDielectricStr = dielectricStr;
									}
								}

								if (wrdcuTank.Volume != null)
								{
									volume = EngineeringUnits.Convert(wrdcuTank.Volume.Value, EngineeringUnit.FmvMeter3, volumeUnitIndex, 15);
								}

								totalVolume = totalVolume + volume;
								volume = Math.Round(volume, siteVolumeDecimalPlaces, MidpointRounding.AwayFromZero);

								var historyCompartmentModel = new AssetEquipmentHistoryRecordModel
								{
									ProductId				= string.Empty,
									AssetTrackingDeviceId	= "Compartment " + tankCount,
									GpsCoordinatesStr		= string.Empty,
									SessionDatetimeStr		= string.Empty,
									VolumeStr				= volume.ToString(CultureInfo.InvariantCulture),
									WaterStr				= waterContent,
									DensityStr				= string.Empty,
									IsCompartment			= true,
									IsContaminated			= wrdcuTank.Contaminated,
									DielectricStr			= dielectricStr,
									Remarks					= ""
								};

								historyRecordModel.HasExpansion = true;
								historyRecordModel.CompartmentRecordList.Add(historyCompartmentModel);
								tankCount++;
							}

							totalVolume = Math.Round(totalVolume, siteVolumeDecimalPlaces, MidpointRounding.AwayFromZero);
							historyRecordModel.VolumeStr = totalVolume.ToString(CultureInfo.InvariantCulture);

							// A count of two means there was only one tank. In this case, we want to
							// display the dielectric value on main row since the compartment row will
							// not be displayed.
							if (tankCount == 2)
							{
								historyRecordModel.DielectricStr = saveDielectricStr;
							}
						}
					}
				}
			}

			return equipmentHistoryModel;
		}

		/// <summary>
		/// This method will update the asset tracking detail with the investigation state and date.
		/// </summary>
		/// <param name="selectedGuidList">The asset tracking GUIDs to update.</param>
		/// <returns>Returns and error or empty string.</returns>
		private string UpdateRecordsToInvestigationState(List<string> selectedGuidList )
		{
			try
			{
				// Update detail record
				FMChannelHelper.MakeCall<IAssetTrackingDetails>(x => x.UpdateRecordsToInvestigateState(this.Security, selectedGuidList));
			}
			catch (Exception ex)
			{
				string msg = "Error (UpdateRecordsToInvestigationState): " + ex.Message;
				this.WriteToEventLog(msg, EventLogEntryType.Error);
				return "Could not update the start to Investigate state.";
			}

			return string.Empty;
		}

		/// <summary>
		/// This method will update the asset tracking detail with the complete investigation state and date.
		/// </summary>
		/// <param name="deviceId">The device ID to filter on.</param>
		/// <param name="completeInvestigateState">The investigation completion state of either passed or failed.</param>
		/// <param name="remarks">The investigation remarks</param>
		/// <returns>Returns and error or empty string.</returns>
		private string UpdateRecordsToCompleteInvestigationState(string deviceId, AssetTrackingDetailClass.MessageStates completeInvestigateState, string remarks)
		{
			try
			{
				// Update detail record
				FMChannelHelper.MakeCall<IAssetTrackingDetails>(
												x => x.UpdateRecordsToInvestigateCompleteState(this.Security, deviceId, completeInvestigateState, remarks));
			}
			catch (Exception ex)
			{
				string msg = "Error (UpdateRecordsToCompleteInvestigationState): " + ex.Message;
				this.WriteToEventLog(msg, EventLogEntryType.Error);
				return "Could not update the complete investigate state.";
			}

			return string.Empty;
		}

		/// <summary>
		/// This method will update the asset tracking detail with the start investigate date.
		/// </summary>
		/// <param name="assetTrackingDetailGuidStr">The asset tracking detail GUID used to update remarks.</param>
		/// <param name="remarks">Remarks to be updated.</param>
		/// <returns>Returns and error or empty string.</returns>
		private string UpdateRemarksOnDetail(string assetTrackingDetailGuidStr, string remarks)
		{
			Guid assetTrackingDetailGuid = Guid.Parse(assetTrackingDetailGuidStr);

			try
			{
				// Update detail record
				FMChannelHelper.MakeCall<IAssetTrackingDetails>(x => x.UpdateRemarks(this.Security, assetTrackingDetailGuid, remarks));
			}
			catch (Exception ex)
			{
				string msg = "Error (UpdateRemarksOnDetail): " + ex.Message;
				this.WriteToEventLog(msg, EventLogEntryType.Error);
				return "Could not update the remarks.";
			}

			return string.Empty;
		}

		/// <summary>
		/// This method will validate that all the GUIDs are valid GUIDs. It will populate the 
		/// GUID List collects.
		/// </summary>
		/// <param name="guidArray">The incoming GUIDs to validate.</param>
		/// <param name="guidList">The validated list of GUIDs.</param>
		/// <returns>Return True if valid, otherwise returns false.</returns>
		private bool ValidateGuids(string[] guidArray, List<string> guidList)
		{
			guidList.Clear();

			foreach (string assetTrackingGuid in guidArray)
			{
				Guid outGuid;

				if (Guid.TryParse(assetTrackingGuid, out outGuid) == false)
				{
					guidList.Clear();
					return false;
				}

				guidList.Add(assetTrackingGuid);
			}

			return true;
		}

		/// <summary>
		/// This method will get the Temperature, Volume, and Pressure for a given tank.
		/// It will return the values in a string.  If null, then they were not
		/// found.
		/// </summary>
		/// <param name="tank">The Tank that has the process variables.</param>
		/// <param name="inSite">The site object to get the units.</param>
		private string GetTankProcessVariables(TankClass tank, SiteClass inSite)
		{
			string description	= string.Empty;
			double? temperature = null;
			double? pressure	= null;
			double? volume		= null;

			if (tank == null)
			{
				return description;
			}

			if (tank.ProcessVariableCollection == null || tank.ProcessVariableCollection.Count == 0)
			{
				return description;
			}

			foreach (ProcessVariableClass processVariable in tank.ProcessVariableCollection)
			{
				switch (processVariable.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.TEMPERATURE_PV:
						byte decimalPlaces = byte.Parse(inSite.TemperatureDecimalPlaces);
						temperature = processVariable.GetValue(inSite.TemperatureUnits, decimalPlaces) as double?;
						break;
					case PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV:
						decimalPlaces = byte.Parse(inSite.PressureDecimalPlaces);
						pressure = processVariable.GetValue(inSite.PressureUnits, decimalPlaces) as double?;
						break;
					case PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV:
						decimalPlaces = byte.Parse(inSite.VolumeDecimalPlaces);
						volume = processVariable.GetValue(inSite.VolumeUnits, decimalPlaces) as double?;
						break;
				}
			}

			if (temperature != null)
			{
                description = description + "|TEMPERATURE|" + temperature.Value + "|ENDTEMPERATURE";
			}

			if (pressure != null)
			{
                description = description + "|PRESSURE|" + pressure.Value + "|ENDPRESSURE";
			}

			if (volume != null)
            {
                description = description + "|GROSSVOLUME|" + volume.Value + "|ENDGROSSVOLUME";
			}

			return description;
		}

		/// <summary>
		/// This method will handle the data dictionary of items that are generated in javascript.
		/// </summary>
		/// <param name="mapsViewModel"></param>
		private void DataDictionaryItems(AssetMapsViewModel mapsViewModel)
		{
			mapsViewModel.EquipmentLabelDictionary			= this.GetTranslatedText("VEHICLE ID");
			mapsViewModel.FacilityLabelDictionary			= this.GetTranslatedText("FACILITY");
			mapsViewModel.DeliveryLocationLabelDictionary	= this.GetTranslatedText("DELIVERY LOCATION");
			mapsViewModel.TankLabelDictionary				= this.GetTranslatedText("TANK");
			mapsViewModel.CompartmentLabelDictionary		= this.GetTranslatedText("Compartment");
			mapsViewModel.ProductLabelDictionary			= this.GetTranslatedText("Product");
			mapsViewModel.VolumeLabelDictionary				= this.GetTranslatedText("Volume");
			mapsViewModel.DensityLabelDictionary			= this.GetTranslatedText("Density");
			mapsViewModel.TimestampLabelDictionary			= this.GetTranslatedText("Timestamp");
			mapsViewModel.ViewHistoryLabelDictionary		= this.GetTranslatedText("VIEW HISTORY");
			mapsViewModel.LatitudeLabelDictionary			= this.GetTranslatedText("Latitude");
			mapsViewModel.LongitudeLabelDictionary			= this.GetTranslatedText("Longitude");
            mapsViewModel.TemperatureLabelDictionary        = this.GetTranslatedText("Temperature");
            mapsViewModel.PressureLabelDictionary           = this.GetTranslatedText("Pressure");
        }

		/// <summary>
		/// This method determine which button was pressed if any.
		/// </summary>
		/// <returns>Return the button pressed enumeration.</returns>
		private Buttons WhichButtonWasPressed()
		{
			string buttonPressed = this.Request.Params.AllKeys.FirstOrDefault(x => x.StartsWith("MapBaseRefreshBtn"));

			if (string.IsNullOrEmpty(buttonPressed))
			{
				string autoRefreshValue = "OFF";

				if (this.Request.Form["autoRefresh"] != null)
				{
					autoRefreshValue = this.Request.Form["autoRefresh"];
				}

				if (autoRefreshValue.Equals("ON"))
				{
					return Buttons.Refresh;
				}

				return Buttons.None;
			}

			return Buttons.Refresh;
		}

		/// <summary>
		/// This method sets the last button pressed in the model.
		/// </summary>
		/// <param name="mapsViewModel">The Maps View Model to update.</param>
		/// <param name="selectedButton">The button selected.</param>
		private void SetLastButtonPressed(AssetMapsViewModel mapsViewModel, Buttons selectedButton)
		{
			switch (selectedButton)
			{
				case Buttons.Refresh:
					mapsViewModel.ActiveButton = AssetMapsViewModel.ActiveButtons.Refresh;
					break;
				default:
					mapsViewModel.ActiveButton = AssetMapsViewModel.ActiveButtons.None;
					break;
			}
		}

		/// <summary>
		/// This method will convert the date time to a string format
		/// of yyyy/mm/dd hh:mm:ss.
		/// </summary>
		/// <param name="sessionDateTime">The Iridium session date time.</param>
		/// <returns>Returns a string representing the date time.</returns>
		private string FormatSessionDateTime(DateTime? sessionDateTime)
		{
			if (sessionDateTime == null)
			{
				return string.Empty;
			}

			string dateSeparator = this.site == null ? "/" : this.site.DateSeparator;
			string timeSeparator = this.site == null ? ":" : this.site.TimeSeparator;

			string monthStr  = sessionDateTime.Value.Month < 10 ? "0" + sessionDateTime.Value.Month : sessionDateTime.Value.Month.ToString();
			string dayStr    = sessionDateTime.Value.Day < 10 ? "0" + sessionDateTime.Value.Day : sessionDateTime.Value.Day.ToString();
			string hourStr	 = sessionDateTime.Value.Hour < 10 ? "0" + sessionDateTime.Value.Hour : sessionDateTime.Value.Hour.ToString();
			string minuteStr = sessionDateTime.Value.Minute < 10 ? "0" + sessionDateTime.Value.Minute : sessionDateTime.Value.Minute.ToString();
			string secondStr = sessionDateTime.Value.Second < 10 ? "0" + sessionDateTime.Value.Second : sessionDateTime.Value.Second.ToString();

			string sessionDateTimeStr = sessionDateTime.Value.Year + dateSeparator
										+ monthStr + dateSeparator
										+ dayStr + " "
										+ hourStr + timeSeparator
										+ minuteStr + timeSeparator
										+ secondStr;

			return sessionDateTimeStr;
		}
		/// <summary>
		/// This method will get the selected map name from the menu selection or
		/// from the model on refresh.
		/// </summary>
		/// <returns>Returns the map name that was selected from the menu.</returns>
		private string GetMapNameFromRequest()
		{
			var urlReferrer = this.Request.UrlReferrer.ParseQueryString();

			if (urlReferrer == null || urlReferrer.HasKeys() == false)
			{
				return string.Empty;
			}

			return urlReferrer.Get("MapName");
		}

		/// <summary>
		/// This method will get a list of asset tracking detail information base on the
		/// device ID.
		/// </summary>
		/// <param name="equipmentSelections">A list of selected equipment that is linked to a device.</param>
		/// <param name="breadcrumbRange">Breadcrumb range.</param>
		/// <param name="getAll">If true, then get all the devices.</param>
		/// <returns>Returns a collection of device ID in the asset tracking detail table.</returns>
		private List<AssetTrackingDetailClass> GetDeviceDetail(List<string> equipmentSelections, string breadcrumbRange, bool getAll)
		{
			List<AssetTrackingDetailClass> assetTrackingDetailList;
			bool topOne = false;

			// Get the date range to filter on.
			DateTime startDate;
			DateTime endDate;
			string currentPosition = this.CalculateDateRange(breadcrumbRange, out startDate, out endDate);

			// If the current position was set then we only want to 
			// get the most current record.
			if (string.IsNullOrEmpty(currentPosition) == false)
			{
				topOne = true;
			}

			if (getAll == false)
			{
				var localAssetTrackingDeviceList = new List<AssetTrackingDeviceClass>();

				foreach (string guidStr in equipmentSelections)
				{
					AssetTrackingDeviceClass assetTrackingDevice = this.assetTrackingDeviceCollection.Find(x => x.EquipmentGuidStr == guidStr);
					localAssetTrackingDeviceList.Add(assetTrackingDevice);
				}

				assetTrackingDetailList = FMChannelHelper.MakeCall<IAssetTrackingDetails, List<AssetTrackingDetailClass>>(
											x => x.GetByDeviceList(this.Security, localAssetTrackingDeviceList, startDate, endDate, topOne));
			}
			else
			{
				// Only get all the asset tracking details that have equipment associated to devices.
				assetTrackingDetailList = FMChannelHelper.MakeCall<IAssetTrackingDetails, List<AssetTrackingDetailClass>>(
												x => x.GetByDeviceList(this.Security, this.assetTrackingDeviceCollection, startDate, endDate, topOne));
			}

			return assetTrackingDetailList;
		}

		/// <summary>
		/// This method will determine if any of the asset tracking detail record is in an Investigate
		/// state.
		/// </summary>
		/// <param name="selectedEquipmentGuid">The selected piece of equipment.</param>
		/// <returns>Returns True if any records is in Investigate state, otherwise returns false.</returns>
		private bool FoundAnyInvestigateStates(string selectedEquipmentGuid)
		{
			// This start date is the date 60 days in the past. That is were we want to start retrieving
			// the data.
			var currentDateTime = DateTime.Now;
			var startDate = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0);
			startDate = startDate.AddDays(-60);

			AssetTrackingDeviceClass assetTrackingDevice = this.assetTrackingDeviceCollection.Find(x => x.EquipmentGuidStr == selectedEquipmentGuid);

			var foundInvestigateStates = FMChannelHelper.MakeCall<IAssetTrackingDetails, bool>(
																			x => x.FoundInvestigateStates(this.Security, assetTrackingDevice.DeviceId, startDate));

			return foundInvestigateStates;
		}

		/// <summary>
		/// This method will get a list of asset tracking detail information base on the
		/// device ID.
		/// </summary>
		/// <param name="selectedEquipmentGuid">The selected equipment that is linked to a device.</param>
		/// <param name="inBreadcrumbState">Breadcrumb state.</param>
		/// <param name="fromDateStr">From date string, most current date.</param>
		/// <param name="endDateStr">End date string, date in the past.</param>
		/// <returns>Returns a collection of device ID in the asset tracking detail table.</returns>
		private List<AssetTrackingDetailClass> GetOneDetailEquipment(string selectedEquipmentGuid, string inBreadcrumbState, string fromDateStr, string endDateStr)
		{
			string breadcrumbState = inBreadcrumbState;
			bool topOne = false;

			// This start date is the date 60 days in the past. That is were we want to start retrieving
			// the data.
			var currentDateTime = DateTime.Now;
			var startDate = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0);
			startDate = startDate.AddDays(-60);


			DateTime filterStartingDateTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0);
			DateTime filterEndingDateTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 23, 59, 59);

			if (string.IsNullOrEmpty(inBreadcrumbState) == false)
			{
				if (inBreadcrumbState == "Current")
				{
					topOne = true;
				}

				if (inBreadcrumbState == "All")
				{
					// Get all the detail information for the last 60 days.
					breadcrumbState = "0";
				}

				if (topOne == false)
				{
					DateTime pastDate;
					DateTime currentDate;
					this.CalculateDateRange(breadcrumbState, out pastDate, out currentDate);
					filterStartingDateTime = pastDate;
					filterEndingDateTime = currentDate;
				}
			}

			// Only get asset tracking detail records if there are from and end dates.
			if (string.IsNullOrEmpty(fromDateStr) == false && string.IsNullOrEmpty(endDateStr) == false)
			{
				filterStartingDateTime = this.ConvertToDateTime(endDateStr, true);
				filterEndingDateTime = this.ConvertToDateTime(fromDateStr, false);
			}

			AssetTrackingDeviceClass assetTrackingDevice = this.assetTrackingDeviceCollection.Find(x => x.EquipmentGuidStr == selectedEquipmentGuid);

			if (assetTrackingDevice == null)
			{
				return null;
			}

			var assetTrackingDetailList = FMChannelHelper.MakeCall<IAssetTrackingDetails, List<AssetTrackingDetailClass>>(
											x => x.GetLast60DaysByDevice(this.Security, assetTrackingDevice.DeviceId, startDate, filterStartingDateTime, filterEndingDateTime, topOne));

			return assetTrackingDetailList;
		}

		/// <summary>
		/// This method will convert a date string formatted like yyyy/mm/dd
		/// to a DateTime object.
		/// </summary>
		/// <param name="inDate">The date string to be converted</param>
		/// <param name="startingDate">Flag indicating whether the inDate is a starting date.</param>
		/// <returns></returns>
		private DateTime ConvertToDateTime(string inDate, bool startingDate)
		{
			string[] parts = inDate.Split('/');

			if (parts.Length >= 3)
			{
				int yyyy;
				int mm;
				int dd;

				if(int.TryParse(parts[0], out yyyy) == false)
				{
					return DateTime.Today;
				}

				if (int.TryParse(parts[1], out mm) == false)
				{
					return DateTime.Today;
				}

				if (int.TryParse(parts[2], out dd) == false)
				{
					return DateTime.Today;
				}

				DateTime convertedDateTime = new DateTime(yyyy, mm, dd, 0, 0, 0);

				if (startingDate == false)
				{
					convertedDateTime = new DateTime(yyyy, mm, dd, 23, 59, 59);
				}

				return convertedDateTime;
			}

			return DateTime.Today;
		}

		/// <summary>
		/// This method will get the information for the dropdowns and set the selected value.
		/// </summary>
		/// <param name="mapViewModel">The view model.</param>
		/// <param name="buttonPressed">The button that was pressed.</param>
		private void PopulateDropdownModel(AssetMapsViewModel mapViewModel, Buttons buttonPressed)
		{
			this.PopulateEquipmentMenuList(mapViewModel.DropdownModel, buttonPressed);
			this.PopulateDeliveryLocationMenuList(mapViewModel.DropdownModel, buttonPressed);
			this.PopulateFacilityMenuList(mapViewModel.DropdownModel, buttonPressed);
			this.PopulateTankMenuList(mapViewModel.DropdownModel, buttonPressed);

			this.PopulateBreadcrumbDropdown(mapViewModel.DropdownModel);
		}

		/// <summary>
		/// This method will populate the tank menu list.
		/// </summary>
		/// <param name="mapDropdownModel">The Asset Map Dropdown Model</param>
		/// <param name="buttonPressed">The button that was pressed.</param>
		private void PopulateTankMenuList(AssetMapsDropdownModel mapDropdownModel, Buttons buttonPressed)
		{
			// Get tank dropdown data
			var tankCollection =
					FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(x => x.EnumerateWhereCoordinatesExist(this.Security));

			var tankList = new List<MapMenuItemClass>();

			if (tankCollection != null && tankCollection.Count > 0)
			{
				var selectedItem = new MapMenuItemClass { Value = "-88", Text = "(Select All)", Checked = false };
				tankList.Add(selectedItem);

				foreach (TankClass tank in tankCollection)
				{
					if (tank.Latitude == null || tank.Longitude == null)
					{
						continue;
					}

					selectedItem = new MapMenuItemClass
					                    {
											Value = tank.IdentityGuid.ToString(),
											Text = tank.ID,
											Checked = true
										};

					tankList.Add(selectedItem);
				}

				mapDropdownModel.SelectedTank = "-88";
				tankList[0].Checked = true;
			}
			else
			{
				mapDropdownModel.SelectedTank = string.Empty;
			}

			mapDropdownModel.TankDropdownList = tankList;
		}

		/// <summary>
		/// This method will populate the breadcrumb dropdown.
		/// </summary>
		/// <param name="mapDropdownModel">The Asset Map Dropdown Model</param>
		private void PopulateBreadcrumbDropdown(AssetMapsDropdownModel mapDropdownModel)
		{
			// Bread crumb dropdown.
			var bradcrumbDropdownList	= new List<MapMenuItemClass>();
			var breadcrumbModel			= new AssetMapsBreadcrumbModel();
			int breadcrumbValue			= 0;
			bool selectionFound			= false;

			foreach (string day in breadcrumbModel.BreadcrumbList)
			{
				MapMenuItemClass dropdownItem;

				if (string.IsNullOrEmpty(mapDropdownModel.SelectedBreadcrumb) == false
					&& mapDropdownModel.SelectedBreadcrumb.Equals(day))
				{
					dropdownItem = new MapMenuItemClass { Checked = true, Text = day, Value = breadcrumbValue.ToString() };
					selectionFound = true;
				}
				else
				{
					dropdownItem = new MapMenuItemClass { Checked = false, Text = day, Value = breadcrumbValue.ToString() };
				}

				bradcrumbDropdownList.Add(dropdownItem);
				breadcrumbValue++;
			}

			// Set the default to "Current Position" if no selection was found.
			if (selectionFound == false)
			{
				bradcrumbDropdownList[0].Checked = true;
			}

			mapDropdownModel.BreadcrumbDropdownList = bradcrumbDropdownList;
		}

		/// <summary>
		/// This method will populate the facility menu list.
		/// </summary>
		/// <param name="mapDropdownModel">The Asset Map Dropdown Model</param>
		/// <param name="buttonPressed">The button that was pressed.</param>
		private void PopulateFacilityMenuList(AssetMapsDropdownModel mapDropdownModel, Buttons buttonPressed)
		{
			// Get Facility dropdown data
			var siteCollection = this.GetSites();
			var facilityList = new List<MapMenuItemClass>();

			if (siteCollection != null && siteCollection.Count > 0)
			{
				var selectedItem = new MapMenuItemClass { Value = "-88", Text = "(Select All)", Checked = false };
				facilityList.Add(selectedItem);

				List<string> selectionList = this.GetSelectionList(mapDropdownModel.SelectedFacility);

				foreach (SiteClass facility in siteCollection)
				{
					if (facility.Latitude == null || facility.Longitude == null)
					{
						continue;
					}

					selectedItem = new MapMenuItemClass { Value = facility.IdentityGuid.ToString(), Text = facility.ID, Checked = true };

					facilityList.Add(selectedItem);
				}

				mapDropdownModel.SelectedFacility = "-88";
				facilityList[0].Checked = true;
			}
			else
			{
				mapDropdownModel.SelectedFacility = string.Empty;
			}

			mapDropdownModel.FacilityDropdownList = facilityList;
		}

		/// <summary>
		/// This method will populate the delivery location menu list.
		/// </summary>
		/// <param name="mapDropdownModel">The Asset Map Dropdown Model</param>
		/// <param name="buttonPressed">The button that was pressed.</param>
		private void PopulateDeliveryLocationMenuList(AssetMapsDropdownModel mapDropdownModel, Buttons buttonPressed)
		{
			// Get Delivery Location dropdown data
			var iataCodeCollection =
					FMChannelHelper.MakeCall<IIATACodes, IATACodeCollectionClass>(x => x.EnumerateWhereCoordinatesExist(this.Security));

			var locationList = new List<MapMenuItemClass>();

			if (iataCodeCollection != null && iataCodeCollection.Count > 0)
			{
				var selectedItem = new MapMenuItemClass { Value = "-88", Text = "(Select All)", Checked = false};
				locationList.Add(selectedItem);

				foreach (IATACodeClass iataCode in iataCodeCollection)
				{
					if (iataCode.Latitude == null || iataCode.Longitude == null)
					{
						continue;
					}

					selectedItem = new MapMenuItemClass
											{
												Value = iataCode.IdentityGuid.ToString(),
												Text = iataCode.ID,
												Checked = true
											};

					locationList.Add(selectedItem);
				}

				mapDropdownModel.SelectedDeliveryLocation = "-88";
				locationList[0].Checked = true;
			}
			else
			{
				mapDropdownModel.SelectedDeliveryLocation = string.Empty;
			}

			mapDropdownModel.DeliveryLocationDropdownList = locationList;
		}

		/// <summary>
		/// This method will populate the equipment menu list.
		/// </summary>
		/// <param name="mapDropdownModel">The Asset Map Dropdown Model</param>
		/// <param name="buttonPressed">The button that was pressed.</param>
		private void PopulateEquipmentMenuList(AssetMapsDropdownModel mapDropdownModel, Buttons buttonPressed)
		{
			// Get Equipment dropdown data.
			this.assetTrackingDeviceCollection =
					FMChannelHelper.MakeCall<IAssetTrackingDevices, List<AssetTrackingDeviceClass>>(
										x => x.EnumerateAllDevicesLinkedToEquipment(this.Security));

			var equipmentList = new List<MapMenuItemClass>();

			if (this.assetTrackingDeviceCollection != null && this.assetTrackingDeviceCollection.Count > 0)
			{
				var selectedItem = new MapMenuItemClass { Value = "-88", Text = "(Select All)", Checked = false };
				equipmentList.Add(selectedItem);

				List<string> selectionList = this.GetSelectionList(mapDropdownModel.SelectedEquipment);

				foreach (AssetTrackingDeviceClass device in this.assetTrackingDeviceCollection)
				{
					if (string.IsNullOrEmpty(device.EquipmentId) || device.EquipmentGuid == Guid.Empty)
					{
						continue;
					}

					selectedItem = new MapMenuItemClass { Value = device.EquipmentGuidStr, Text = device.EquipmentId, Checked = true };

					equipmentList.Add(selectedItem);
				}

				mapDropdownModel.SelectedEquipment = "-88";
				equipmentList[0].Checked = true;
			}
			else
			{
				mapDropdownModel.SelectedEquipment = string.Empty;
			}

			mapDropdownModel.EquipmentDropdownList = equipmentList;
		}

		/// <summary>
		/// This method will get a site collection that contains the parent site and any
		/// of its children.
		/// </summary>
		/// <returns>Returns a collection of sites.</returns>
		private SiteCollectionClass GetSites()
		{
			var currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
			var siteCollection = 
					FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(x => x.EnumerateByParentSite(this.Security, this.Security.SiteGuid));

			siteCollection.Insert(0, currentSite);

			return siteCollection;
		}

		/// <summary>
		/// This method will return the asset maps view model with the selected map,
		/// which contains the map latitude and longitude.
		/// </summary>
		private void GetSelectedMap(AssetMapsViewModel mapViewModel)
		{
			this.initialRequestFromMenu = false;
			string mapName = this.GetMapNameFromRequest();

			// If the map name is present, that means the request came from the 
			// main menu.  Otherwise, it is a post back. Only get the map
			// configuration data if new map.
			if (string.IsNullOrEmpty(mapName) == false)
			{
				var mapConfiguration =
							FMChannelHelper.MakeCall<IAssetTrackingMapConfigurations, AssetTrackingMapConfigurationClass>(
								x => x.GetByMapName(this.Security, mapName));

				mapViewModel.MapLatitude	= mapConfiguration.Latitude;
				mapViewModel.MapLongitude	= mapConfiguration.Longitude;
				mapViewModel.Zoom			= mapConfiguration.Zoom;
				mapViewModel.MapName		= mapName;
				mapViewModel.MapSource		= mapConfiguration.MapSource;

				this.initialRequestFromMenu = true;
			}

			const string SelectIcon = "SelectIcon.png";
			const string IconPathKey = "GeoTrackingMapIconPath";

			var configSettingDo = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(this.Security, IconPathKey));
			string iconPath = "~/Areas/images/AssetMapImages/MapIcons/";

			if (configSettingDo != null && string.IsNullOrEmpty(configSettingDo.SettingValue) == false)
			{
				const string Slash = "/";
				iconPath = configSettingDo.SettingValue;
				string lastChar = iconPath.Substring(iconPath.Length - 1, 1);

				if (lastChar.Equals(Slash) == false)
				{
					iconPath = iconPath + Slash;
				}
			}

			// Initialize the model icon names with the default values.
			this.InitializeDefaultIconName(mapViewModel, iconPath);

			var iconConfigList = 
							FMChannelHelper.MakeCall<IAssetTrackingIconConfigurations, List<AssetTrackingIconConfigurationClass>>(
																		x => x.Enumerate(this.Security));

			AssetTrackingIconConfigurationClass iconConfig;
			if (iconConfigList != null && iconConfigList.Count > 0)
			{
				iconConfig = iconConfigList[0];
			}
			else
			{
				iconConfig = new AssetTrackingIconConfigurationClass();
			}

			if (string.IsNullOrEmpty(iconConfig.EquipmentIconName) == false
				&& iconConfig.EquipmentIconName.Equals(SelectIcon) == false)
			{
				mapViewModel.EquipmentIcon = iconPath + iconConfig.EquipmentIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.EquipmentVarianceIconName) == false
				&& iconConfig.EquipmentVarianceIconName.Equals(SelectIcon) == false)
			{
				mapViewModel.EquipmentContaminatedIcon = iconPath + iconConfig.EquipmentVarianceIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.EquipmentInvestigationIconName) == false
				&& iconConfig.EquipmentInvestigationIconName.Equals(SelectIcon) == false)
			{
				mapViewModel.EquipmentInvestigationIcon = iconPath + iconConfig.EquipmentInvestigationIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.EquipmentCompleteInvestigationFailedIconName) == false
				&& iconConfig.EquipmentCompleteInvestigationFailedIconName.Equals(SelectIcon) == false)
			{
				mapViewModel.EquipmentCompleteInvestigationFailedIcon = iconPath + iconConfig.EquipmentCompleteInvestigationFailedIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.EquipmentCompleteInvestigationPassedIconName) == false
				&& iconConfig.EquipmentCompleteInvestigationPassedIconName.Equals(SelectIcon) == false)
			{
				mapViewModel.EquipmentCompleteInvestigationPassedIcon = iconPath + iconConfig.EquipmentCompleteInvestigationPassedIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.TankIconName) == false
				&& iconConfig.TankIconName.Equals(SelectIcon) == false)
			{
				mapViewModel.TankIcon = iconPath + iconConfig.TankIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.FacilityIconName) == false
				&& iconConfig.FacilityIconName.Equals(SelectIcon) == false)
			{
				mapViewModel.FacilityIcon = iconPath + iconConfig.FacilityIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.DeliveryLocationIconName) == false
				&& iconConfig.DeliveryLocationIconName.Equals(SelectIcon) == false)
			{
				mapViewModel.DeliveryLocationIcon = iconPath + iconConfig.DeliveryLocationIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.BreadcrumbIconName) == false
				&& iconConfig.BreadcrumbIconName.Equals(SelectIcon) == false)
			{
				mapViewModel.BreadcrumbIcon = iconPath + iconConfig.BreadcrumbIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.BreadcrumbVarianceIconName) == false
				&& iconConfig.BreadcrumbVarianceIconName.Equals(SelectIcon) == false)
			{
				mapViewModel.BreadcrumbContaminatedIcon = iconPath + iconConfig.BreadcrumbVarianceIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.BreadcrumbInvestigationIconName) == false
				&& iconConfig.BreadcrumbInvestigationIconName.Equals(SelectIcon) == false)
			{
				mapViewModel.BreadcrumbInvestigationIcon = iconPath + iconConfig.BreadcrumbInvestigationIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.BreadcrumbCompleteInvestigationFailedIconName) == false
				&& iconConfig.BreadcrumbCompleteInvestigationFailedIconName.Equals(SelectIcon) == false)
			{
				mapViewModel.BreadcrumbCompleteInvestigationFailedIcon = iconPath + iconConfig.BreadcrumbCompleteInvestigationFailedIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.BreadcrumbCompleteInvestigationPassedIconName) == false
				&& iconConfig.BreadcrumbCompleteInvestigationPassedIconName.Equals(SelectIcon) == false)
			{
				mapViewModel.BreadcrumbCompleteInvestigationPassedIcon = iconPath + iconConfig.BreadcrumbCompleteInvestigationPassedIconName;
			}
		}

		/// <summary>
		/// This method will initialize the model with the default icon names.
		/// </summary>
		/// <param name="mapViewModel"></param>
		/// <param name="iconPath"></param>
		private void InitializeDefaultIconName(AssetMapsViewModel mapViewModel, string iconPath)
		{
			mapViewModel.BreadcrumbIcon					= iconPath + "Dark-green-triangle.png";
			mapViewModel.TankIcon						= iconPath + "Tank.png";
			mapViewModel.FacilityIcon					= iconPath + "Tag-Icon.png";
			mapViewModel.EquipmentIcon					= iconPath + "Big-Truck-for-map-Blue.png";
			mapViewModel.DeliveryLocationIcon			= iconPath + "Flag.png";
			mapViewModel.EquipmentContaminatedIcon		= iconPath + "Big-Truck-for-map-Orange.png";
			mapViewModel.EquipmentInvestigationIcon		= iconPath + "Big-Truck-for-map-Yellow.png";
			mapViewModel.BreadcrumbContaminatedIcon		= iconPath + "OrangeWarning.png";
			mapViewModel.BreadcrumbInvestigationIcon	= iconPath + "YellowWarning.png";

			mapViewModel.EquipmentCompleteInvestigationFailedIcon	= iconPath + "Big-Truck-for-map-Red.png";
			mapViewModel.EquipmentCompleteInvestigationPassedIcon	= iconPath + "Big-Truck-for-map-Green.png";
			mapViewModel.BreadcrumbCompleteInvestigationFailedIcon	= iconPath + "RedWarning.png";
			mapViewModel.BreadcrumbCompleteInvestigationPassedIcon	= iconPath + "GreenWarning.png";

			string keyValue = ConfigurationManager.AppSettings.Get(EquipmentIconNameKey);
			if (string.IsNullOrEmpty(keyValue) == false)
			{
				mapViewModel.EquipmentIcon = iconPath + keyValue;
			}

			keyValue = ConfigurationManager.AppSettings.Get(EquipmentVarianceIconNameKey);
			if (string.IsNullOrEmpty(keyValue) == false)
			{
				mapViewModel.EquipmentContaminatedIcon = iconPath + keyValue;
			}

			keyValue = ConfigurationManager.AppSettings.Get(EquipmentInvestigationIconNameKey);
			if (string.IsNullOrEmpty(keyValue) == false)
			{
				mapViewModel.EquipmentInvestigationIcon = iconPath + keyValue;
			}

			keyValue = ConfigurationManager.AppSettings.Get(EquipmentCompleteInvestigationFailedIconNameKey);
			if (string.IsNullOrEmpty(keyValue) == false)
			{
				mapViewModel.EquipmentCompleteInvestigationFailedIcon = iconPath + keyValue;
			}

			keyValue = ConfigurationManager.AppSettings.Get(EquipmentCompleteInvestigationPassedIconNameKey);
			if (string.IsNullOrEmpty(keyValue) == false)
			{
				mapViewModel.EquipmentCompleteInvestigationPassedIcon = iconPath + keyValue;
			}

			keyValue = ConfigurationManager.AppSettings.Get(BreadcrumbIconNameKey);
			if (string.IsNullOrEmpty(keyValue) == false)
			{
				mapViewModel.BreadcrumbIcon = iconPath + keyValue;
			}

			keyValue = ConfigurationManager.AppSettings.Get(BreadcrumbVarianceIconNameKey);
			if (string.IsNullOrEmpty(keyValue) == false)
			{
				mapViewModel.BreadcrumbContaminatedIcon = iconPath + keyValue;
			}

			keyValue = ConfigurationManager.AppSettings.Get(BreadcrumbInvestigationIconNameKey);
			if (string.IsNullOrEmpty(keyValue) == false)
			{
				mapViewModel.BreadcrumbInvestigationIcon = iconPath + keyValue;
			}

			keyValue = ConfigurationManager.AppSettings.Get(BreadcrumbCompleteInvestigationFailedIconNameKey);
			if (string.IsNullOrEmpty(keyValue) == false)
			{
				mapViewModel.BreadcrumbCompleteInvestigationFailedIcon = iconPath + keyValue;
			}

			keyValue = ConfigurationManager.AppSettings.Get(BreadcrumbCompleteInvestigationPassedIconNameKey);
			if (string.IsNullOrEmpty(keyValue) == false)
			{
				mapViewModel.BreadcrumbCompleteInvestigationPassedIcon = iconPath + keyValue;
			}

			keyValue = ConfigurationManager.AppSettings.Get(TankIconNameKey);
			if (string.IsNullOrEmpty(keyValue) == false)
			{
				mapViewModel.TankIcon = iconPath + keyValue;
			}

			keyValue = ConfigurationManager.AppSettings.Get(FacilityIconNameKey);
			if (string.IsNullOrEmpty(keyValue) == false)
			{
				mapViewModel.FacilityIcon = iconPath + keyValue;
			}

			keyValue = ConfigurationManager.AppSettings.Get(DeliveryLocationIconNameKey);
			if (string.IsNullOrEmpty(keyValue) == false)
			{
				mapViewModel.DeliveryLocationIcon = iconPath + keyValue;
			}
		}

		/// <summary>
		/// This method will calculate the bread crumb date range. If the selection is current postion,
		/// then the range will be for one day and will return use current position only.
		/// </summary>
		/// <param name="breadcrumbRange">Breadcrumb range.</param>
		/// <param name="startDate">Returns the starting date range.</param>
		/// <param name="endDate">Returns the ending date range.</param>
		/// <returns>Returns Current Position string or empty string.</returns>
		private string CalculateDateRange(string breadcrumbRange, out DateTime startDate, out DateTime endDate)
		{
			var currentDateTime		= DateTime.Now;
			endDate					= new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 23, 59, 59);
			startDate				= new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0);

			// Zero indicates the most current position of the equipment. With that,
			// the date range needs to be set to 60 days, since we do not know
			// if the vehicle stopped for any duration.
			if (string.IsNullOrEmpty(breadcrumbRange) || breadcrumbRange.Equals("0"))
			{
				startDate = startDate.AddDays(-60);
				return AssetMapsBreadcrumbModel.CurrentPosition;
			}

			int day;

			if (int.TryParse(breadcrumbRange, out day))
			{
				startDate = startDate.AddDays(day * -1);
			}

			return string.Empty;
		}

		/// <summary>
		/// This method will retrieve the Map Refresh Time from the configuration setting tables.
		/// </summary>
		/// <param name="mapsViewModel">The model to update.</param>
		private void GetRefreshTime(AssetMapsViewModel mapsViewModel)
		{
			const string MapRefreshTimeKey = "GeoTrackingMapRefreshTimeInSeconds";

			// Set to default to NO refresh.
			mapsViewModel.MapRefreshOn = 0;
			mapsViewModel.MapRefreshTimeInMilliSeconds = 0;

			var configSettingDo = 
					FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(this.Security, MapRefreshTimeKey));

			if (configSettingDo != null && string.IsNullOrEmpty(configSettingDo.SettingValue) == false)
			{
				int seconds;

				if (int.TryParse(configSettingDo.SettingValue, out seconds))
				{
					// Refresh time must be be 1 second and 1 hour.
					if (seconds > 0 && seconds <= 3600)
					{
						mapsViewModel.MapRefreshOn = 1;
						mapsViewModel.MapRefreshTimeInMilliSeconds = seconds * 1000;
					}
				}
			}
		}

		/// <summary>
		/// This method will parse the multi-selections into a string
		/// collection.
		/// </summary>
		/// <param name="multiSelection">The string that contains the selections to parse.</param>
		/// <returns>Returns a collection of selections.</returns>
		private List<string> GetSelectionList(string multiSelection)
		{
			var selectionList = new List<string>();

			if (string.IsNullOrEmpty(multiSelection))
			{
				return selectionList;
			}

			string[] selectionParts = multiSelection.Split(',');

			foreach (string selectedItem in selectionParts)
			{
				selectionList.Add(selectedItem);
			}

			return selectionList;
		}

		/// <summary>
		/// This method determines if the selection should be selected.
		/// </summary>
		/// <param name="selectionList">The items that were selected on the UI</param>
		/// <param name="guidStr">The GUID string to compare to.</param>
		/// <returns>Returns true if the GUID string matches one of the selections.</returns>
		private bool ShouldItemBeSelected(List<string> selectionList, string guidStr)
		{
			bool shouldBeSelected = false;

			foreach (string selectedItem in selectionList)
			{
				if (selectedItem.Equals(guidStr))
				{
					shouldBeSelected = true;
				}
			}

			return shouldBeSelected;
		}

		/// <summary>
		/// This method will determine whether the map should use the user's
		/// selected zoom or the user made a menu change and the map then
		/// should use the extent.
		/// </summary>
		/// <param name="postedViewModel">The posted map view model.</param>
		/// <param name="buttonPressed">Which button was pressed.</param>
		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		private bool SetExtentFlag(AssetMapsViewModel postedViewModel, Buttons buttonPressed)
		{
			// Do not use extent when initial request from the FuelsManager main
			// menu. Use the map configured coordinates.
			if (this.initialRequestFromMenu)
			{
				return false;
			}

			// TODO: Per defect 73008 we are removing the "Extent" feature for now!
			//if (buttonPressed == Buttons.None)
			//{
			//	return true;
			//}

			//if (buttonPressed == Buttons.Refresh && postedViewModel.MenuItemChange)
			//{
			//	return true;
			//}

			return false;
		}

/*
		/// <summary>
		/// This method will return the unit abbreviation based on the unit index passed into
		/// the methods. The heirarchy is to return the unit abbreviation from Equipement if
		/// present, then product, then site.
		/// </summary>
		/// <param name="siteUnit">The site unit index.</param>
		/// <param name="equipmentUnit">The equipment unit index.</param>
		/// <param name="productUnit">The product unit index.</param>
		/// <returns>Returns the appropriate unit abbreviation.</returns>
		private string GetUnitAbbrivation(int? siteUnit, int? equipmentUnit, int? productUnit)
		{
			string unitAbbreviation = string.Empty;

			if (productUnit != null)
			{
				unitAbbreviation = EngineeringUnits.GetUnitAbbreviation((EngineeringUnit) productUnit);
				return unitAbbreviation;
			}

			if (equipmentUnit != null)
			{
				unitAbbreviation = EngineeringUnits.GetUnitAbbreviation((EngineeringUnit) equipmentUnit);
				return unitAbbreviation;
			}

			if (siteUnit != null)
			{
				unitAbbreviation = EngineeringUnits.GetUnitAbbreviation((EngineeringUnit)siteUnit);
			}

			return unitAbbreviation;
		}
*/

		/// <summary>
		/// This method will return the appropriate units based on the unit index passed into
		/// the methods. The heirarchy is to return the unit index from Equipement if
		/// present, then product, then site.
		/// </summary>
		/// <param name="siteUnit">The site unit index.</param>
		/// <param name="equipmentUnit">The equipment unit index.</param>
		/// <param name="productUnit">The product unit index.</param>
		/// <returns>Returns the appropriate unit abbreviation.</returns>
		private EngineeringUnit GetAppropriateUnitIndex(int siteUnit, int? equipmentUnit, int? productUnit)
		{
			EngineeringUnit unitIndex = (EngineeringUnit)siteUnit;

			if (productUnit != null)
			{
				unitIndex = (EngineeringUnit) productUnit;
				return unitIndex;
			}

			if (equipmentUnit != null)
			{
				unitIndex = (EngineeringUnit) equipmentUnit;
				return unitIndex;
			}

			return unitIndex;
		}

		/// <summary>
		/// This method will write to the windows event log.
		/// </summary>
		/// <param name="message">The message content.</param>
		/// <param name="messageType">Type of message i.e. error, warning, etc.</param>
		private void WriteToEventLog(string message, EventLogEntryType messageType)
		{
			using (EventLog eventLog = new EventLog("Application", ".", "FuelsManager"))
			{
				eventLog.WriteEntry(message, messageType);
			}
		}
		#endregion
	}
}
