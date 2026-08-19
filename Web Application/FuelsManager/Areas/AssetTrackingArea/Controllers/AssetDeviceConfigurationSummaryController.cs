namespace FuelsManager.Areas.AssetTrackingArea.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FuelsManager.Areas.AssetTrackingArea.ViewModels;
	using FuelsManager.Areas.Controllers;

	public class AssetDeviceConfigurationSummaryController : FMBaseController, IEntityDiscovery
	{
        // GET: AssetTrackingArea/AssetDeviceConfigurationSummary
        public ActionResult DeviceConfigurationSummary(AssetDeviceConfigSummaryModel summaryModel)
        {
	        return this.DeviceConfigurationSummary(summaryModel, null);
        }

		// GET: AssetTrackingArea/AssetDeviceConfigurationSummary
		[HttpPost]
		public ActionResult DeviceConfigurationSummary(AssetDeviceConfigSummaryModel summaryModel, Guid? deleteButton)
		{
			if (summaryModel == null)
			{
				summaryModel = new AssetDeviceConfigSummaryModel();
			}

			try
			{
				if (deleteButton != null)
				{
					if (deleteButton == Guid.Empty)
					{
						this.ModelState.AddModelError("Error", "Error: Invalid GUID");
						return this.View(summaryModel);
					}

					this.DeleteRecord(deleteButton.Value);
				}

				this.SetMode(summaryModel);
				this.GetDeviceConfigurationData(summaryModel);
				summaryModel.ActionListActivationStatus = this.PopulateActivationStatusDropdown(summaryModel.ActivationStatusId);

				return this.View(summaryModel);
			}
			catch (Exception ex)
			{
				this.ModelState.AddModelError("Error", "Error in deleting or retrieving data. " + ex.Message);
				return this.View(summaryModel);
			}
		}

		#region Explicit Interface Properties
		/// <summary>
		/// Gets a value indicating whether [entity assignable].
		/// </summary>
		/// <value>
		///   <c>true</c> if [entity assignable]; otherwise, <c>false</c>.
		/// </value>
		bool IEntityDiscovery.EntityAssignable => true;

		/// <summary>
		/// Gets the type of the entity engine.
		/// </summary>
		/// <value>
		/// The type of the entity engine.
		/// </value>
		Type IEntityDiscovery.EntityEngineType => typeof(IAssetTrackingDevices);

		/// <summary>
		/// Gets the type of the entity.
		/// </summary>
		/// <value>
		/// The type of the entity.
		/// </value>
		ENTITY_TYPE IEntityDiscovery.EntityType => ENTITY_TYPE.ASSET_TRACKING_DEVICE;
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will delete a device configuration record from the database.
		/// </summary>
		/// <param name="deviceRecordGuid">The GUID of the device record to delete</param>
		private void DeleteRecord(Guid deviceRecordGuid)
		{
			FMChannelHelper.MakeCall<IAssetTrackingDevices>(x => x.Purge(this.Security, deviceRecordGuid));
		}

		/// <summary>
		/// This method will set the edit mode based on the secruity right.
		/// </summary>
		/// <param name="summaryModel">The device configuration summary model.</param>
		private void SetMode(AssetDeviceConfigSummaryModel summaryModel)
		{
			summaryModel.IsEditable = this.Security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES);
			summaryModel.SiteGuid = this.Security.SiteGuid;
		}

		/// <summary>
		/// This method will get the device configuration records from the database
		/// and populate the model.
		/// </summary>
		/// <param name="summaryModel">The model to update.</param>
		private void GetDeviceConfigurationData(AssetDeviceConfigSummaryModel summaryModel)
		{
			var deviceDataSet =
				FMChannelHelper.MakeCall<IAssetTrackingDevices, DataSet>(x => x.EnumerateAllDeviceInDataSet(this.Security, summaryModel.FindText));

			summaryModel.DeviceConfigurationList.Clear();

			if (deviceDataSet != null && deviceDataSet.Tables.Count > 0 && deviceDataSet.Tables[0].Rows.Count > 0)
			{
				foreach (DataRow row in deviceDataSet.Tables[0].Rows)
				{
					var siteGuid		= row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"];
					var deviceGuid		= row.IsNull("AssetTrackingDeviceGuid") ? Guid.Empty : (Guid)row["AssetTrackingDeviceGuid"];
					var deviceId		= row.IsNull("DeviceID") ? string.Empty : (string)row["DeviceID"];
					var description		= row.IsNull("Description") ? string.Empty : (string)row["Description"];
					var modelNumber		= row.IsNull("ModelNumber") ? string.Empty : (string)row["ModelNumber"];
					var serialNumber	= row.IsNull("SerialNumber") ? string.Empty : (string)row["SerialNumber"];
					var active			= row.IsNull("Active") ? false : (bool)row["Active"];

					var deviceModel = new AssetDeviceConfigurationModel
					{
						AssetTrackingDeviceGuid = deviceGuid,
						DeviceId				= deviceId,
						Description				= description,
						ModelNumber				= modelNumber,
						SerialNumber			= serialNumber,
						Active					= active,
						SiteGuid				= siteGuid
					};

					// Filter based on the activation status that the user set.
					if (string.IsNullOrEmpty(summaryModel.ActivationStatusId) || summaryModel.ActivationStatusId.Equals("All"))
					{
						summaryModel.DeviceConfigurationList.Add(deviceModel);
						continue;
					}

					if (summaryModel.ActivationStatusId.Equals("Active") && deviceModel.ActivationStatusStr.Equals("Active"))
					{
						summaryModel.DeviceConfigurationList.Add(deviceModel);
						continue;
					}
					
					if (summaryModel.ActivationStatusId.Equals("Inactive") && deviceModel.ActivationStatusStr.Equals("Inactive"))
					{
						summaryModel.DeviceConfigurationList.Add(deviceModel);
					}
				}
			}
		}

		/// <summary>
		/// This method will populate the activation status dropdown list.
		/// </summary>
		/// <param name="selectedValue">Selected value from user.</param>
		/// <returns>Returns the activation status dropdown list.</returns>
		private List<SelectListItem> PopulateActivationStatusDropdown(string selectedValue)
		{
			var activationStatusList = new List<SelectListItem>();
			var selectedVal = string.IsNullOrEmpty(selectedValue) ? "All" : selectedValue;

			var selectItem = new SelectListItem { Value = "All", Text = "All", Selected = selectedVal.Equals("All") };
			activationStatusList.Add(selectItem);

			selectItem = new SelectListItem { Value = "Active", Text = "Active", Selected = selectedVal.Equals("Active") };
			activationStatusList.Add(selectItem);

			selectItem = new SelectListItem { Value = "Inactive", Text = "Inactive", Selected = selectedVal.Equals("Inactive") };
			activationStatusList.Add(selectItem);

			return activationStatusList;
		}
		#endregion

		#region Entity Methods
		/// <summary>
		/// This method is used by the entity discovery infrastructure to get a list of asset tracking
		/// device entities.
		/// </summary>
		/// <param name="inSecurity"></param>
		/// <param name="entityAssignmentType"></param>
		/// <returns></returns>
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass inSecurity, ENTITY_ASSIGNMENT_TYPE entityAssignmentType)
		{
			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();
			var deviceList = FMChannelHelper.MakeCall<IAssetTrackingDevices, List<AssetTrackingDeviceClass>>(x => x.Enumerate(inSecurity));

			foreach (AssetTrackingDeviceClass device in deviceList)
			{
				if (entityAssignmentType == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (inSecurity.SiteGuid == device.SiteGuid)
					{
						continue;
					}

					if (inSecurity.LoginSiteGuid != device.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (inSecurity.SiteGuid != device.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(device);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		/// <summary>
		/// This method is used by the entity discovery infrastructure to get an asset tracking
		/// device entity GUID by the ID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="id">The id.</param>
		/// <returns>The identity guid.</returns>
		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<IAssetTrackingDevices, Guid>(x => x.GetIdentityGuid(security, id));
		}

		/// <summary>
		/// This method is used by the entity discovery infrastructure to set the site GUID for
		/// an asset tracking device entity.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="guid">The GUID.</param>
		/// <param name="siteGuid">The site GUID.</param>
		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			var device = FMChannelHelper.MakeCall<IAssetTrackingDevices, AssetTrackingDeviceClass>(x => x.GetByIdentityGuid(security, guid));

			device.SiteGuid = siteGuid;
			FMChannelHelper.MakeCall<IAssetTrackingDevices>(x => x.Modify(security, device));
		}
		#endregion
	}
}