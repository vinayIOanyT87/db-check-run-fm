namespace FuelsManager.Areas.AssetTrackingArea.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.Mvc;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using ViewModels;
    using Areas.Controllers;
    using FMWebApp;

    using Varec.CommonComponents.EngineeringUnitsLibrary;
    using Newtonsoft.Json;

    public class AssetDeviceConfigurationController : FMBaseController
	{
		#region Private data members
		private enum PageModes { Add, Edit };
		private enum UrlTypes { SelectPage, SummaryPage };
		private PageModes pageMode;
		private Guid deviceGuid;
        private UrlTypes urlType;
		private enum Buttons { None, New, Ok, Cancel };
		private const string ReturnUrl = "AssetDeviceConfigurationController.ReturnUrl";
		#endregion

        /// <summary>
        /// This method returns the string version of the model.
        /// </summary>
        /// <param name="model">The model to serialize.</param>
        /// <returns>Returns the string version of the model.</returns>
        [NonAction]
        public static string SerializeModel(AssetDeviceConfigurationModel model)
        {
            return JsonConvert.SerializeObject(model);
        }

        /// <summary>
        /// This method will deserialize the model string into an object.
        /// </summary>
        /// <param name="modelStr">The string version of the model.</param>
        /// <returns>Returns the model as an object.</returns>
        [NonAction]
        public static AssetDeviceConfigurationModel DeserializeModel(string modelStr)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var model = JsonConvert.DeserializeObject<AssetDeviceConfigurationModel>(modelStr, jsonSerializerSettings);

            return model;
        }

        /// <summary>
        /// This method will update the asset tracking device.
        /// </summary>
        /// <param name="modelStr">The model string to be updated.</param>
        /// <returns>Returns results data.</returns>
        [HttpPost]
        public ActionResult UpdateAssetDeviceConfigurationItem(string modelStr)
        {
            var resultData = new AtdResultDataClass();
            var model = DeserializeModel(modelStr);

            if (string.IsNullOrEmpty(modelStr))
            {
                this.ViewData.Model = model;

                resultData.ErrorFlag = true;
                resultData.ErrorMessage = "Error: Invalid asset tracking device configuration model.";
                return this.Json(resultData);
            }

            if (this.Security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES) == false)
            {
                this.ViewData.Model = model;

                resultData.ErrorFlag = true;
                resultData.ErrorMessage = "Error: Access denied.";
                return this.Json(resultData);
            }

            try 
            {
                if (model.AssetTrackingDeviceGuid == Guid.Empty)
                {
                    this.pageMode = PageModes.Add;
                    this.InsertRecordToDatabase(model);
                }
                else
                {
                    this.pageMode = PageModes.Edit;
                    this.UpdateRecordToDatabase(model);
                }
            }
            catch(Exception ex)
            {
                resultData.ErrorMessage = "Error: " + ex.Message;
                resultData.ErrorFlag = true;
                model.IsEditable = this.IsEditable();
                model.ActionListEquipment = this.GetEquipmentListToAssociate(model);
                model.ActionListAssociatedTanks = this.GetAssociatedTanks(Guid.Empty);

                if (model.SiteGuid != Guid.Empty)
                {
                    model.IsEditable = this.IsEditable(model.SiteGuid);
                }

                return this.Json(resultData);
            }

            return this.Json(resultData);
        }

        /// <summary>
        /// This method will handle the New button event.
        /// </summary>
        /// <param name="modelStr">The Model in string format.</param>
        /// <returns>Return the result data object.</returns>
        [HttpPost]
        public ActionResult NewAssetDeviceConfigurationItem(string modelStr)
        {
            var resultData = new AtdResultDataClass();
            var model = DeserializeModel(modelStr);

            if (string.IsNullOrEmpty(modelStr))
            {
                this.ViewData.Model = model;

                resultData.ErrorFlag = true;
                resultData.ErrorMessage = "Error: Invalid asset tracking device configuration model.";
                return this.Json(resultData);
            }

            if (this.Security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES) == false)
            {
                this.ViewData.Model = model;

                resultData.ErrorFlag = true;
                resultData.ErrorMessage = "Error: Access denied.";
                return this.Json(resultData);
            }

            if (string.IsNullOrEmpty(model.DeviceId))
            {
                resultData.ErrorMessage = "Error: Device ID is required.";
                resultData.ErrorFlag = true;
                return this.Json(resultData);
            }

            try
            {
                if (model.AssetTrackingDeviceGuid == Guid.Empty)
                {
                    this.InsertRecordToDatabase(model);
                }
                else
                {
                    this.UpdateRecordToDatabase(model);
                }

                this.pageMode                           = PageModes.Add;
                var deviceModel                         = new AssetDeviceConfigurationModel();
                deviceModel.ActionListEquipment         = this.GetEquipmentListToAssociate(deviceModel);
                deviceModel.ActionListAssociatedTanks   = this.GetAssociatedTanks(Guid.Empty);
                deviceModel.IsEditable                  = this.IsEditable();

                this.PopulateDeviceTypes(deviceModel, AssetDeviceConfigurationModel.AssetTrackingDeviceTypes.Standard);
                this.PopulateSourceUnits(deviceModel, EngineeringUnit.FmvMeter3);
                resultData.AssetDeviceConfigModel = deviceModel;

                return this.Json(resultData);
            }
            catch(Exception ex)
            {
                resultData.ErrorMessage = "Error: " + ex.Message;
                resultData.ErrorFlag = true;
                resultData.AssetDeviceConfigModel = model;
                return this.Json(resultData);
            }
        }

        /// <summary>
        /// This method will handle the events coming from the Device Configuration Summary
        /// page.
        /// </summary>
        /// <param name="deviceConfigurationGuid">The GUID to edit or create a new one.</param>
        /// <returns>Returns the view.</returns>
        [HttpGet]
		public ActionResult DeviceConfiguration(Guid deviceConfigurationGuid)
		{
			AssetDeviceConfigurationModel deviceConfigModel;

			if (deviceConfigurationGuid == Guid.Empty)
			{
				this.pageMode								= PageModes.Add;
				deviceConfigModel							= new AssetDeviceConfigurationModel { IsEditable = this.IsEditable() };			
				deviceConfigModel.ActionListEquipment		= this.GetEquipmentListToAssociate(deviceConfigModel);
				deviceConfigModel.ActionListAssociatedTanks = this.GetAssociatedTanks(deviceConfigurationGuid);

				this.PopulateDeviceTypes(deviceConfigModel, AssetDeviceConfigurationModel.AssetTrackingDeviceTypes.Standard);
				this.PopulateSourceUnits(deviceConfigModel, EngineeringUnit.FmvMeter3);
				this.SetMode();

			    deviceConfigModel.RedirectToEquipmentUrl = this.GetReturnUrl();

				return this.View(deviceConfigModel);
			}

			this.pageMode								= PageModes.Edit;
			this.deviceGuid								= deviceConfigurationGuid;
			deviceConfigModel							= this.GetDeviceToUpdate();
			deviceConfigModel.ActionListEquipment		= this.GetEquipmentListToAssociate(deviceConfigModel);
			deviceConfigModel.ActionListAssociatedTanks = this.GetAssociatedTanks(deviceConfigurationGuid);
			deviceConfigModel.IsEditable				= this.IsEditable(deviceConfigModel.SiteGuid);

			this.PopulateDeviceTypes(deviceConfigModel, deviceConfigModel.AssetTrackingDeviceType);
            this.PopulateSourceUnits(deviceConfigModel, (EngineeringUnit)deviceConfigModel.SourceUnit);
            this.SetMode();
            deviceConfigModel.RedirectToEquipmentUrl = this.GetReturnUrl();

            return this.View(deviceConfigModel);
		}

		/// <summary>
		/// This method will return true if the user has permission to edit.
		/// Otherwise it will return false.
		/// </summary>
		/// <returns>Returns true is editable.</returns>
		private bool IsEditable()
		{
			bool editable = this.Security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES);
			return editable;
		}

		/// <summary>
		/// This method will return true if the user has permission to edit.
		/// Otherwise it will return false.
		/// </summary>
		/// <returns>Returns true is editable.</returns>
		private bool IsEditable(Guid inDeviceGuid)
		{
			bool editable = this.Security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES) && this.Security.SiteGuid == inDeviceGuid;
			return editable;
		}

		/// <summary>
		/// This method will persist the return URL based on the type.
		/// </summary>
		private string GetReturnUrl()
        {
            const string MvcPopupContainerUrl = "FMWebApp/MvcPopupContainer.aspx?target=../FMWebApp/AssetTrackingDeviceSelectForm.aspx?unassigned=true";

			if (this.urlType == UrlTypes.SelectPage)
			{
                return MvcPopupContainerUrl;
            }

		    return string.Empty;
		}

		/// <summary>
		/// This method will set the page mode to either Add or Edit.
		/// The default is add.
		/// </summary>
		private void SetMode()
		{
			// Check to see if the calling program was the Asset Tracking Device
			// Selection form. If so, then set the mode to ADD and return.
			var selectionContext = (AssetTrackingDeviceSelectContextClass)
											this.Session["AssetTrackingDeviceSelectForm.SessionDeviceSelectContext"];

            this.urlType = UrlTypes.SummaryPage;

            if (selectionContext != null && selectionContext.CallingPage.Equals("SelectionPage"))
			{
				this.urlType = UrlTypes.SelectPage;
				this.Session.Remove("AssetTrackingDeviceSelectForm.SessionDeviceSelectContext");
			}
		}

		/// <summary>
		/// This method will retrieve the asset tracking device to update.
		/// </summary>
		/// <returns>Return the device model to edit.</returns>
		private AssetDeviceConfigurationModel GetDeviceToUpdate()
		{
			var deviceModel = new AssetDeviceConfigurationModel();

			var assetTrackingDevice = 
				FMChannelHelper.MakeCall<IAssetTrackingDevices, AssetTrackingDeviceClass>(x => x.GetByIdentityGuid(this.Security, this.deviceGuid));

			if (assetTrackingDevice == null)
			{
				throw new Exception("Invalid Asset Tracking Device GUID.");
			}

			deviceModel.AssetTrackingDeviceGuid = assetTrackingDevice.AssetTrackingDeviceGuid;
			deviceModel.SiteGuid				= assetTrackingDevice.SiteGuid;
			deviceModel.Active					= assetTrackingDevice.Active;
			deviceModel.Description				= assetTrackingDevice.Description;
			deviceModel.DeviceId				= assetTrackingDevice.DeviceId;
			deviceModel.ModelNumber				= assetTrackingDevice.ModelNumber;
			deviceModel.SerialNumber			= assetTrackingDevice.SerialNumber;
			deviceModel.EquipmentId				= assetTrackingDevice.EquipmentId;
			deviceModel.EquipmentGuidStr		= assetTrackingDevice.EquipmentGuidStr;
			deviceModel.AssetTrackingDeviceType = (AssetDeviceConfigurationModel.AssetTrackingDeviceTypes)((int)assetTrackingDevice.AssetTrackingDeviceType);
			deviceModel.SourceUnit				= (int)assetTrackingDevice.SourceUnit;

			return deviceModel;
		}

		/// <summary>
		/// This method will get a list of unassociated equipment. If in edit mode,
		/// it will the current associated equipment to the device too.
		/// </summary>
		/// <param name="deviceModel">The device model.</param>
		/// <returns>Returns a list of unassociated equipment.</returns>
		private List<SelectListItem> GetEquipmentListToAssociate(AssetDeviceConfigurationModel deviceModel)
		{
			SelectListItem selectItem;
			var unassociatedEquipmentList = new List<SelectListItem>();
            bool isSelected = false;

            // When editing an existing device, add the current associated equipment
            // to the list and select it.
            if (this.pageMode == PageModes.Edit)
			{
				Guid equipmentGuid = Guid.Empty;

				if (deviceModel.AssetTrackingDeviceGuid != Guid.Empty)
				{
					equipmentGuid =
						FMChannelHelper.MakeCall<IAssetTrackingDevices, Guid>(
							x => x.GetAssociatedEquipmentGuid(this.Security, deviceModel.AssetTrackingDeviceGuid));
				}

				if (equipmentGuid != Guid.Empty)
				{
					var equipment =
						FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.Security, equipmentGuid));

					if (equipment == null || equipment.IdentityGuid == Guid.Empty)
					{
						// Save current site GUIDs
						Guid loginSiteGuid = this.Security.LoginSiteGuid;
						Guid siteGuid = this.Security.SiteGuid;

						// Get the equipment site GUID to be used to retrieve the equipment.
						var equipmentSiteGuid = FMChannelHelper.MakeCall<IAssetTrackingDevices, Guid>(
														x => x.GetEquipmentSiteGuid(this.Security, deviceModel.AssetTrackingDeviceGuid));

						if (equipmentSiteGuid != Guid.Empty)
						{
							this.Security.LoginSiteGuid = equipmentSiteGuid;
							this.Security.SiteGuid = equipmentSiteGuid;

							equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.Security, equipmentGuid));

							// Reset the site GUIDs.
							this.Security.LoginSiteGuid = loginSiteGuid;
							this.Security.SiteGuid = siteGuid;
						}
					}

					if (equipment != null && equipment.IdentityGuid != Guid.Empty)
					{
						deviceModel.EquipmentId = equipment.ID;
						deviceModel.EquipmentGuid = equipmentGuid;

						selectItem = new SelectListItem { Value = deviceModel.EquipmentGuidStr, Text = deviceModel.EquipmentId, Selected = true };
						unassociatedEquipmentList.Add(selectItem);
                        isSelected = true;
					}
				}
			}
            
            var unassociatedEquipmentDataSet = 
                        FMChannelHelper.MakeCall<IAssetTrackingDevices, DataSet>(x => x.EnumerateAllEquipmentNotAssociateToDevices(this.Security));

            if (unassociatedEquipmentDataSet != null
                && unassociatedEquipmentDataSet.Tables.Count > 0
                && unassociatedEquipmentDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in unassociatedEquipmentDataSet.Tables[0].Rows)
                {
                    string equipmentId = row.IsNull("ID") ? string.Empty : (string)row["ID"];
                    Guid equipmentGuid = row.IsNull("EquipmentGuid") ? Guid.Empty : (Guid)row["EquipmentGuid"];

                    if (string.IsNullOrEmpty(equipmentId) == false && equipmentGuid != Guid.Empty)
                    {
                        selectItem = new SelectListItem { Value = equipmentGuid.ToString(), Text = equipmentId };
                        unassociatedEquipmentList.Add(selectItem);
                    }
                }
            }

            List<SelectListItem> sortedUnassociatedEquipmentList = unassociatedEquipmentList.OrderBy(o => o.Text).ToList();

            if (isSelected)
            { 
                selectItem = new SelectListItem { Value = "-99", Text = "-- None --" }; 
            }
            else
            {
                selectItem = new SelectListItem { Value = "-99", Text = "-- None --", Selected = true };
            }

            sortedUnassociatedEquipmentList.Insert(0, selectItem);

            return sortedUnassociatedEquipmentList;
		}


		/// <summary>
		/// This method will populate the Source Unit dropdown and set the selected item.
		/// </summary>
		/// <param name="deviceConfigModel">The device configuration model use to populate the dropdown.</param>
		/// <param name="selectedSourceUnit">The selected source unit.</param>
		private void PopulateSourceUnits(AssetDeviceConfigurationModel deviceConfigModel,
										EngineeringUnit selectedSourceUnit)
		{
			List<SelectListItem> itemList = new List<SelectListItem>();		

			var selectListItem = new SelectListItem
									{
										Value = ((int)EngineeringUnit.FmvCm3).ToString(),
										Text = EngineeringUnits.GetUnitAbbreviation(EngineeringUnit.FmvCm3),
										Selected = selectedSourceUnit == EngineeringUnit.FmvCm3
            };
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
									{
										Value = ((int) EngineeringUnit.FmvMeter3).ToString(),
										Text = EngineeringUnits.GetUnitAbbreviation(EngineeringUnit.FmvMeter3),
										Selected = selectedSourceUnit == EngineeringUnit.FmvMeter3
									};
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
									{
										Value = ((int) EngineeringUnit.FmvLitre).ToString(),
										Text = EngineeringUnits.GetUnitAbbreviation(EngineeringUnit.FmvLitre),
										Selected = selectedSourceUnit == EngineeringUnit.FmvLitre
									};
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
									{
										Value = ((int) EngineeringUnit.FmvInch3).ToString(),
										Text = EngineeringUnits.GetUnitAbbreviation(EngineeringUnit.FmvInch3),
										Selected = selectedSourceUnit == EngineeringUnit.FmvInch3
									};
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
									{
										Value = ((int) EngineeringUnit.FmvFeet3).ToString(),
										Text = EngineeringUnits.GetUnitAbbreviation(EngineeringUnit.FmvFeet3),
										Selected = selectedSourceUnit == EngineeringUnit.FmvFeet3
									};
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
									{
										Value = ((int) EngineeringUnit.FmvYard3).ToString(),
										Text = EngineeringUnits.GetUnitAbbreviation(EngineeringUnit.FmvYard3),
										Selected = selectedSourceUnit == EngineeringUnit.FmvYard3
									};
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
									{
										Value = ((int) EngineeringUnit.FmvUsGal).ToString(),
										Text = EngineeringUnits.GetUnitAbbreviation(EngineeringUnit.FmvUsGal),
										Selected = selectedSourceUnit == EngineeringUnit.FmvUsGal
            };
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
									{
										Value = ((int) EngineeringUnit.FmvImpGal).ToString(),
										Text = EngineeringUnits.GetUnitAbbreviation(EngineeringUnit.FmvImpGal),
										Selected = selectedSourceUnit == EngineeringUnit.FmvImpGal
									};
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
									{
										Value = ((int) EngineeringUnit.FmvBlOil).ToString(),
										Text = EngineeringUnits.GetUnitAbbreviation(EngineeringUnit.FmvBlOil),
										Selected = selectedSourceUnit == EngineeringUnit.FmvBlOil
									};
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
									{
										Value = ((int) EngineeringUnit.FmvBlLiq).ToString(),
										Text = EngineeringUnits.GetUnitAbbreviation(EngineeringUnit.FmvBlLiq),
										Selected = selectedSourceUnit == EngineeringUnit.FmvBlLiq
									};
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
									{
										Value = ((int) EngineeringUnit.FmvKl).ToString(),
										Text = EngineeringUnits.GetUnitAbbreviation(EngineeringUnit.FmvKl),
										Selected = selectedSourceUnit == EngineeringUnit.FmvKl
            };
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
								{
									Value = ((int)EngineeringUnit.FmvMsFt3).ToString(),
									Text = EngineeringUnits.GetUnitAbbreviation(EngineeringUnit.FmvMsFt3),
									Selected = selectedSourceUnit == EngineeringUnit.FmvMsFt3
								};
			itemList.Add(selectListItem);

            List<SelectListItem> sortedItemList = itemList.OrderBy(o => o.Text).ToList();

            deviceConfigModel.ActionSourceUnits = sortedItemList;
		}

		/// <summary>
		/// This method will get the associated tanks.
		/// </summary>
		/// <param name="assetTrackingDeviceGuid"></param>
		/// <returns>Returns a list of associated tanks.</returns>
		private List<SelectListItem> GetAssociatedTanks(Guid assetTrackingDeviceGuid)
		{
			var associatedTankList = new List<SelectListItem>();

			if (assetTrackingDeviceGuid == Guid.Empty)
			{
				return associatedTankList;
			}

			var tankList = 
				FMChannelHelper.MakeCall<IAssetTrackingDevices, List<string>>(x => x.EnumerateAssociatedTanks(this.Security, assetTrackingDeviceGuid));

			if (tankList != null && tankList.Count > 0)
			{
				foreach (string tankId in tankList)
				{
					if (string.IsNullOrEmpty(tankId))
					{
						continue;
					}

					var listItem = new SelectListItem { Value = tankId, Text = tankId };
					associatedTankList.Add(listItem);
				}
			}

            List<SelectListItem> sortedAssociatedTankList = associatedTankList.OrderBy(o => o.Text).ToList();

            return sortedAssociatedTankList;
		}

		/// <summary>
		/// This method will populate the Device Type dropdown and set the selected item.
		/// </summary>
		/// <param name="deviceConfigModel">The device configuration model use to populate the dropdown.</param>
		/// <param name="selectedDeviceType">The selected device type.</param>
		private void PopulateDeviceTypes(AssetDeviceConfigurationModel deviceConfigModel, 
										AssetDeviceConfigurationModel.AssetTrackingDeviceTypes selectedDeviceType)
		{
			List<SelectListItem> itemList = new List<SelectListItem>();

			var selectListItem = new SelectListItem
			{
				Value = AssetDeviceConfigurationModel.AssetTrackingDeviceTypes.Tdu.ToString(),
				Text = deviceConfigModel.GetAssetTrackingDeviceTypeName(AssetDeviceConfigurationModel.AssetTrackingDeviceTypes.Tdu),
				Selected = selectedDeviceType == AssetDeviceConfigurationModel.AssetTrackingDeviceTypes.Tdu
			};
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
			{
				Value = AssetDeviceConfigurationModel.AssetTrackingDeviceTypes.Wrdcu.ToString(),
				Text = deviceConfigModel.GetAssetTrackingDeviceTypeName(AssetDeviceConfigurationModel.AssetTrackingDeviceTypes.Wrdcu),
				Selected = selectedDeviceType == AssetDeviceConfigurationModel.AssetTrackingDeviceTypes.Wrdcu
			};
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
			{
				Value = AssetDeviceConfigurationModel.AssetTrackingDeviceTypes.Standard.ToString(),
				Text = deviceConfigModel.GetAssetTrackingDeviceTypeName(AssetDeviceConfigurationModel.AssetTrackingDeviceTypes.Standard),
				Selected = selectedDeviceType == AssetDeviceConfigurationModel.AssetTrackingDeviceTypes.Standard
			};
			itemList.Add(selectListItem);

            List<SelectListItem> sortedItemList = itemList.OrderBy(o => o.Text).ToList();

            deviceConfigModel.ActionListDeviceTypes = sortedItemList;
		}

		/// <summary>
		/// This method will create an asset tracking device record.
		/// </summary>
		/// <param name="postedDeviceModel">The data to save.</param>
		private void InsertRecordToDatabase(AssetDeviceConfigurationModel postedDeviceModel)
		{
			if (postedDeviceModel == null)
			{
				throw new ArgumentException("Model is null.");
			}

			if (string.IsNullOrEmpty(postedDeviceModel.DeviceId))
			{
				throw new Exception("Device ID is required.");
			}

			if (postedDeviceModel.SelectedEquipment.Equals("-99") == false)
			{
				Guid equipmentGuid;

				if (Guid.TryParse(postedDeviceModel.SelectedEquipment, out equipmentGuid))
				{
					var equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.Security, equipmentGuid));

					if (equipment != null)
					{
						postedDeviceModel.EquipmentId = equipment.ID;
						postedDeviceModel.EquipmentGuid = equipmentGuid;
					}
				}
			}
            else
            {
                postedDeviceModel.EquipmentId = string.Empty;
                postedDeviceModel.EquipmentGuid = Guid.Empty;
            }

			var device = new AssetTrackingDeviceClass
			             {
				             DeviceId					= postedDeviceModel.DeviceId,
				             Description				= postedDeviceModel.Description,
				             ModelNumber				= postedDeviceModel.ModelNumber,
				             SerialNumber				= postedDeviceModel.SerialNumber,
				             EquipmentId				= postedDeviceModel.EquipmentId,
				             EquipmentGuidStr			= postedDeviceModel.EquipmentGuidStr,
				             Active						= postedDeviceModel.Active,
							 AssetTrackingDeviceType	= (AssetTrackingDeviceClass.AssetTrackingDeviceTypes)((int)postedDeviceModel.AssetTrackingDeviceType),
							 SourceUnit					= (EngineeringUnit)postedDeviceModel.SourceUnit
			             };

			postedDeviceModel.AssetTrackingDeviceGuid = 
					FMChannelHelper.MakeCall<IAssetTrackingDevices, Guid>(x => x.Add(this.Security, device));

			postedDeviceModel.SiteGuid = this.Security.SiteGuid;
		}

		/// <summary>
		/// This method will update an asset tracking device record.
		/// </summary>
		/// <param name="postedDeviceModel">The data to save.</param>
		private void UpdateRecordToDatabase(AssetDeviceConfigurationModel postedDeviceModel)
		{
			if (postedDeviceModel == null)
			{
				throw new ArgumentException("Model is null.");
			}

			if (String.IsNullOrEmpty(postedDeviceModel.DeviceId))
			{
				throw new Exception("Device ID is required.");
			}

			// The user selected Unassign the device from the equipment.
			if (postedDeviceModel.SelectedEquipment.Equals("-99"))
			{
				postedDeviceModel.EquipmentId = string.Empty;
				postedDeviceModel.EquipmentGuid = Guid.Empty;
			}
			else
			{
				// Anything other then empty or -99 means an equipment was selected.
				Guid equipmentGuid;

				if (Guid.TryParse(postedDeviceModel.SelectedEquipment, out equipmentGuid))
				{
					var equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.Security, equipmentGuid));

					if (equipment != null)
					{
						postedDeviceModel.EquipmentId = equipment.ID;
						postedDeviceModel.EquipmentGuidStr = equipmentGuid.ToString();
					}
				}
			}

			var device = new AssetTrackingDeviceClass
							{
								AssetTrackingDeviceGuid = postedDeviceModel.AssetTrackingDeviceGuid,
								SiteGuid				= postedDeviceModel.SiteGuid,
								DeviceId				= postedDeviceModel.DeviceId,
								Description				= postedDeviceModel.Description,
								ModelNumber				= postedDeviceModel.ModelNumber,
								SerialNumber			= postedDeviceModel.SerialNumber,
								EquipmentId				= postedDeviceModel.EquipmentId,
								EquipmentGuidStr		= postedDeviceModel.EquipmentGuidStr,
								Active					= postedDeviceModel.Active,
								AssetTrackingDeviceType = (AssetTrackingDeviceClass.AssetTrackingDeviceTypes)((int)postedDeviceModel.AssetTrackingDeviceType),
								SourceUnit				= (EngineeringUnit)postedDeviceModel.SourceUnit
							};

			FMChannelHelper.MakeCall<IAssetTrackingDevices>(x => x.Modify(this.Security, device));
		}
	}


    #region Result data class
    [Serializable]
    public class AtdResultDataClass
    {
        public bool ErrorFlag;
        public string ErrorMessage;
        public AssetDeviceConfigurationModel AssetDeviceConfigModel;

        public AtdResultDataClass()
        {
            this.ErrorFlag = false;
            this.ErrorMessage = string.Empty;
            this.AssetDeviceConfigModel = new AssetDeviceConfigurationModel();
        }
    }
    #endregion
}