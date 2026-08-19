using System.Web.Mvc;

namespace FuelsManager.Areas.AssetTrackingArea.Controllers
{
	using System.Collections.Generic;
	using System.Data;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.AssetTrackingArea.ViewModels;
	using FuelsManager.Areas.Controllers;

	public class AssetCalculateCoordinatesController : FMBaseController
	{
		public const string SessionCalculateCoordinateLatitude	= "CalculateCoordinate.Latitude";
		public const string SessionCalculateCoordinateLongitude = "CalculateCoordinate.Longitude";
		public const string SessionCalculateCoordinateZoom		= "CalculateCoordinate.Zoom";
		public const string SessionCalculateCoordinateMapSource = "CalculateCoordinate.MapSource";

		private const string RequestLatitude	= "Latitude";
		private const string RequestLongitude	= "Longitude";
		private const string RequestZoom		= "Zoom";
		private enum CoordinateRequestTypes { Latitude, Longitude, Zoom };

		// GET: AssetTrackingArea/AssetCalculateCoordinates
		public ActionResult CalculateCoordinates(AssetCalculateCoordinatesModel postedModel)
        {
	        if (postedModel == null)
	        {
		        var newModel = new AssetCalculateCoordinatesModel();
				this.GetPersistedInfo(newModel);

				// Get the existing items to place on the map for reference.
				this.GetDeliveryLocations(newModel);
				this.GetFacilities(newModel);
				this.GetTanks(newModel);

				// Get the configured icons.
				this.GetIcons(newModel);

				return this.View(newModel);
	        }

			this.GetPersistedInfo(postedModel);

			// Get the existing items to place on the map for reference.
			this.GetDeliveryLocations(postedModel);
			this.GetFacilities(postedModel);
			this.GetTanks(postedModel);

			// Get the configured icons.
			this.GetIcons(postedModel);

			return this.View(postedModel);
        }

		/// <summary>
		/// This method is called by the Map Configuration page.
		/// </summary>
		/// <param name="latitudeStr">String containing the latitude.</param>
		/// <param name="longitudeStr">String containing the longitude.</param>
		/// <param name="zoomStr">String containing the zoom level.</param>
		/// <returns>Returns the view object.</returns>
		[HttpGet]
		public ActionResult CalculateCoordinates(string latitudeStr, string longitudeStr, string zoomStr)
		{
			this.ModelState.Clear();
			var newModel = new AssetCalculateCoordinatesModel
			               {
								Zoom = 1,
								Latitude = 0.0,
								Longitude = 0.0,
								MapSource = MapConfigurationModel.MapSources.GoogleMap
			               };

			if (string.IsNullOrEmpty(latitudeStr) || latitudeStr.Equals("-9999"))
			{
				if (this.Session[SessionCalculateCoordinateLatitude] != null)
				{
					string latitude = this.Session[SessionCalculateCoordinateLatitude] as string;

					if (string.IsNullOrEmpty(latitude) == false)
					{
						newModel.LatitudeStr = latitude;
					}
				}
			}
			else
			{
				newModel.LatitudeStr = latitudeStr;
			}

			if (string.IsNullOrEmpty(longitudeStr) || longitudeStr.Equals("-9999"))
			{
				if (this.Session[SessionCalculateCoordinateLongitude] != null)
				{
					string longitude = this.Session[SessionCalculateCoordinateLongitude] as string;

					if (string.IsNullOrEmpty(longitude) == false)
					{
						newModel.LongitudeStr = longitude;
					}
				}
			}
			else
			{
				newModel.LongitudeStr = longitudeStr;
			}

			if (string.IsNullOrEmpty(zoomStr) || zoomStr.Equals("-9999"))
			{
				if (this.Session[SessionCalculateCoordinateZoom] != null)
				{
					string zoom = this.Session[SessionCalculateCoordinateZoom] as string;

					if (string.IsNullOrEmpty(zoom) == false)
					{
						newModel.ZoomStr = zoom;
					}
				}
			}
			else
			{
				newModel.ZoomStr = zoomStr;
			}

			if (this.Session[SessionCalculateCoordinateMapSource] != null)
			{
				newModel.MapSource = (MapConfigurationModel.MapSources)this.Session[SessionCalculateCoordinateMapSource];
			}

			// Get the existing items to place on the map for reference.
			this.GetDeliveryLocations(newModel);
			this.GetFacilities(newModel);
			this.GetTanks(newModel);

			// Get the configured icons.
			this.GetIcons(newModel);

			return this.View(newModel);
		}

		#region Private methods
		/// <summary>
		/// This method will retrieve persisted coordinate info from the last
		/// configurations.
		/// </summary>
		/// <param name="coordinatedModel">The model to update.</param>
		private void GetPersistedInfo(AssetCalculateCoordinatesModel coordinatedModel)
		{
			coordinatedModel.Zoom		= 1;
			coordinatedModel.Latitude	= 0.0;
			coordinatedModel.Longitude	= 0.0;

			string latitudeStr	= this.GetCoordinateFromRequest(CoordinateRequestTypes.Latitude);
			string longitudeStr = this.GetCoordinateFromRequest(CoordinateRequestTypes.Longitude);
			string zoomStr		= this.GetCoordinateFromRequest(CoordinateRequestTypes.Zoom);

			if (string.IsNullOrEmpty(latitudeStr))
			{
				if (this.Session[SessionCalculateCoordinateLatitude] != null)
				{
					string latitude = this.Session[SessionCalculateCoordinateLatitude] as string;

					if (string.IsNullOrEmpty(latitude) == false)
					{
						coordinatedModel.LatitudeStr = latitude;
					}
				}
			}
			else
			{
				coordinatedModel.LatitudeStr = latitudeStr;
			}

			if (string.IsNullOrEmpty(longitudeStr))
			{
				if (this.Session[SessionCalculateCoordinateLongitude] != null)
				{
					string longitude = this.Session[SessionCalculateCoordinateLongitude] as string;

					if (string.IsNullOrEmpty(longitude) == false)
					{
						coordinatedModel.LongitudeStr = longitude;
					}
				}
			}
			else
			{
				coordinatedModel.LongitudeStr = longitudeStr;
			}

			if (string.IsNullOrEmpty(zoomStr))
			{
				if (this.Session[SessionCalculateCoordinateZoom] != null)
				{
					string zoom = this.Session[SessionCalculateCoordinateZoom] as string;

					if (string.IsNullOrEmpty(zoom) == false)
					{
						coordinatedModel.ZoomStr = zoom;
					}
				}
			}
			else
			{
				coordinatedModel.ZoomStr = zoomStr;
			}
		}

		/// <summary>
		/// This method will retrieve a coordinate from the request. It will
		/// return the value or empty string.
		/// </summary>
		/// <param name="requestType">The coordinate type.</param>
		/// <returns>Returns a coordinate value or empty string.</returns>
		private string GetCoordinateFromRequest(CoordinateRequestTypes requestType)
		{
			if (requestType == CoordinateRequestTypes.Latitude)
			{
				string latitudeStr = this.Request[RequestLatitude];

				if (string.IsNullOrEmpty(latitudeStr) || latitudeStr.Equals("-9999"))
				{
					return string.Empty;
				}

				return latitudeStr;
			}

			if (requestType == CoordinateRequestTypes.Longitude)
			{
				string longitudeStr = this.Request[RequestLongitude];

				if (string.IsNullOrEmpty(longitudeStr) || longitudeStr.Equals("-9999"))
				{
					return string.Empty;
				}

				return longitudeStr;
			}

			if (requestType == CoordinateRequestTypes.Zoom)
			{
				string zoomStr = this.Request[RequestZoom];

				if (string.IsNullOrEmpty(zoomStr) || zoomStr.Equals("-9999"))
				{
					return string.Empty;
				}

				return zoomStr;
			}

			return string.Empty;
		}

		/// <summary>
		/// This method will get the delivery locations and populate the asset calculate
		/// coordinate model.
		/// </summary>
		/// <param name="coordinateModel">The model that the view will use.</param>
		private void GetDeliveryLocations(AssetCalculateCoordinatesModel coordinateModel)
		{
			coordinateModel.DeliveryLocationModel = new AssetMapsDeliveryLocationModel();

			var iataCodeCollection =
						FMChannelHelper.MakeCall<IIATACodes, IATACodeCollectionClass>(x => x.EnumerateWhereCoordinatesExist(this.Security));

			foreach (IATACodeClass iataCode in iataCodeCollection)
			{
				if (iataCode.Latitude != null && iataCode.Longitude != null)
				{
					coordinateModel.DeliveryLocationModel.LatitudeList.Add(iataCode.Latitude.Value);
					coordinateModel.DeliveryLocationModel.LongitudeList.Add(iataCode.Longitude.Value);
					coordinateModel.DeliveryLocationModel.HasCoordinates = 1;

					string description = "Delivery Location: " + iataCode.ID + "|"
										+ "Latitude: " + iataCode.Latitude.Value + "|"
										+ "Longitude: " + iataCode.Longitude.Value;

					coordinateModel.DeliveryLocationModel.HoverDescriptionList.Add(description);
				}
			}
		}

		/// <summary>
		/// This method will get the tanks and populate the asset calculate
		/// coordinate model.
		/// </summary>
		/// <param name="coordinateModel">The model that the view will use.</param>
		private void GetTanks(AssetCalculateCoordinatesModel coordinateModel)
		{
			coordinateModel.TankModel = new AssetMapsTankModel();

			var tankDataSet =
						FMChannelHelper.MakeCall<IAssetTrackingDevices, DataSet>(x => x.EnumerateAllAssociatedTanks(this.Security));

			if (tankDataSet != null && tankDataSet.Tables.Count > 0 && tankDataSet.Tables[0].Rows.Count > 0)
			{
				foreach (DataRow row in tankDataSet.Tables[0].Rows)
				{
					string tankId = row.IsNull("TankID") ? string.Empty : (string)row["TankID"];
					double? latitude = row.IsNull("Latitude") ? null : (double?)row["Latitude"];
					double? longitude = row.IsNull("Longitude") ? null : (double?)row["Longitude"];


					if (latitude != null && longitude != null)
					{
						coordinateModel.TankModel.LatitudeList.Add(latitude.Value);
						coordinateModel.TankModel.LongitudeList.Add(longitude.Value);
						coordinateModel.TankModel.HasCoordinates = 1;

						string description = "Tank: " + tankId + "|"
											+ "Latitude: " + latitude.Value + "|"
											+ "Longitude: " + longitude.Value;

						coordinateModel.TankModel.HoverDescriptionList.Add(description);
					}
				}
			}
		}

		/// <summary>
		/// This method will get the facilities and populate the asset calculate
		/// coordinate model.
		/// </summary>
		/// <param name="coordinateModel">The model that the view will use.</param>
		private void GetFacilities(AssetCalculateCoordinatesModel coordinateModel)
		{
			coordinateModel.FacilityModel = new AssetMapsFacilityModel();

			var siteCollection =
						FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(x => x.Enumerate(this.Security));

			if (siteCollection != null && siteCollection.Count > 0)
			{
				foreach (SiteClass localSite in siteCollection)
				{
					if (localSite.Latitude != null && localSite.Longitude != null)
					{
						coordinateModel.FacilityModel.LatitudeList.Add(localSite.Latitude.Value);
						coordinateModel.FacilityModel.LongitudeList.Add(localSite.Longitude.Value);
						coordinateModel.FacilityModel.HasCoordinates = 1;

						string description = "Facility: " + localSite.ID + "|"
											+ "Latitude: " + localSite.Latitude.Value + "|"
											+ "Longitude: " + localSite.Longitude.Value;

						coordinateModel.FacilityModel.HoverDescriptionList.Add(description);
					}
				}
			}
		}

		private void GetIcons(AssetCalculateCoordinatesModel coordinateModel)
		{
			const string SelectIcon = "SelectIcon.png";
			const string IconPathKey = "GeoTrackingMapIconPath";

			var configSettingDo = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(this.Security, IconPathKey));
			string iconPath = "~/Areas/images/AssetMapImages/MapIcons";

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
				coordinateModel.EquipmentIcon = iconPath + iconConfig.EquipmentIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.TankIconName) == false
				&& iconConfig.TankIconName.Equals(SelectIcon) == false)
			{
				coordinateModel.TankIcon = iconPath + iconConfig.TankIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.FacilityIconName) == false
				&& iconConfig.FacilityIconName.Equals(SelectIcon) == false)
			{
				coordinateModel.FacilityIcon = iconPath + iconConfig.FacilityIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.DeliveryLocationIconName) == false
				&& iconConfig.DeliveryLocationIconName.Equals(SelectIcon) == false)
			{
				coordinateModel.DeliveryLocationIcon = iconPath + iconConfig.DeliveryLocationIconName;
			}

			if (string.IsNullOrEmpty(iconConfig.MapPinIconName) == false
				&& iconConfig.MapPinIconName.Equals(SelectIcon) == false)
			{
				coordinateModel.MapPinIcon = iconPath + iconConfig.MapPinIconName;
			}
		}
		#endregion
	}
}