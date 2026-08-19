namespace FuelsManager.Areas.AssetTrackingArea.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FuelsManager.Areas.AssetTrackingArea.ViewModels;
	using FuelsManager.Areas.Controllers;

	public class AssetMapConfigurationSummaryController : FMBaseController, IEntityDiscovery
	{
		public ActionResult MapConfigurationSummary(MapConfigSummaryModel summaryModel)
		{
			return this.MapConfigurationSummary(summaryModel, null);
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
		Type IEntityDiscovery.EntityEngineType => typeof(IAssetTrackingMapConfigurations);

		/// <summary>
		/// Gets the type of the entity.
		/// </summary>
		/// <value>
		/// The type of the entity.
		/// </value>
		ENTITY_TYPE IEntityDiscovery.EntityType => ENTITY_TYPE.ASSET_TRACKING_MAP_CONFIGURATION;
		#endregion

		// GET: AssetTrackingArea/AssetMapConfigurationSummary
		[HttpPost]
		public ActionResult MapConfigurationSummary(MapConfigSummaryModel summaryModel, Guid? deleteButton)
        {
			if (summaryModel == null)
			{
				summaryModel = new MapConfigSummaryModel();
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
				this.GetMapConfigurationData(summaryModel);

				return this.View(summaryModel);
			}
			catch (Exception ex)
			{
				this.ModelState.AddModelError("Error", "Error in deleting or retrieving data. " + ex.Message);
				return this.View(summaryModel);
			}
        }

		#region Private methods
		/// <summary>
		/// This method will delete a map configuration record from the database.
		/// </summary>
		/// <param name="mapRecordGuid">The GUID of the map record to delete</param>
		private void DeleteRecord(Guid mapRecordGuid)
		{
			FMChannelHelper.MakeCall<IAssetTrackingMapConfigurations>(x => x.Purge(this.Security, mapRecordGuid));
		}

		/// <summary>
		/// This method will set the edit mode based on the secruity right.
		/// </summary>
		/// <param name="summaryModel">The map configuration summary model.</param>
		private void SetMode(MapConfigSummaryModel summaryModel)
		{
			summaryModel.IsEditable = this.Security.HasRight(RIGHT.MODIFY_MAP_CONFIGURATION);
			summaryModel.SiteGuid = this.Security.SiteGuid;
		}

		/// <summary>
		/// This method will get the map configuration records from the database
		/// and populate the model.
		/// </summary>
		/// <param name="summaryModel">The model to update.</param>
		private void GetMapConfigurationData(MapConfigSummaryModel summaryModel)
		{
			var mapConfigList =
					FMChannelHelper.MakeCall<IAssetTrackingMapConfigurations, List<AssetTrackingMapConfigurationClass>>(
										x => x.EnumerateByFilter(this.Security, summaryModel.FindText));

			summaryModel.MapConfigurationList.Clear();

			if (mapConfigList != null && mapConfigList.Count > 0)
			{
				foreach (AssetTrackingMapConfigurationClass mapConfig in mapConfigList)
				{
					var mapModel = new MapConfigurationModel
					               {
						               AssetTrackingMapConfigurationGuid = mapConfig.AssetTrackingMapConfigurationGuid,
						               MapName							 = mapConfig.MapName,
						               Description						 = mapConfig.Description,
						               SiteGuid							 = mapConfig.SiteGuid
					               };

					summaryModel.MapConfigurationList.Add(mapModel);
				}
			}
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