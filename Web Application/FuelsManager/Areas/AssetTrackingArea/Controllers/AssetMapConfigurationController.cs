namespace FuelsManager.Areas.AssetTrackingArea.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.AssetTrackingArea.ViewModels;
	using FuelsManager.Areas.Controllers;

	public class AssetMapConfigurationController : FMBaseController
	{
		#region Private memembers
		private enum Buttons { None, New, Ok, Cancel };
		private const string SessionMapConfigModel = "AssetMapConfiguration.MapConfigModel";
		#endregion

		// GET: AssetTrackingArea/AssetMapConfiguration
		[HttpPost]
        public ActionResult MapConfiguration(MapConfigurationModel postedModel)
        {
			Buttons buttonAction = this.WhichButtonWasPressed();

	        try
	        {
				if (buttonAction == Buttons.Ok)
				{
					if (string.IsNullOrEmpty(postedModel.MapName))
					{
						this.PopulateMapSources(postedModel, postedModel.MapSource);
						postedModel.IsEditable = this.IsEditable();

						this.ModelState.AddModelError("Error", "Map name is required.");
						return this.View(postedModel);
					}

					string errorMsg;
					bool validated = this.ValidateCoordinates(postedModel, out errorMsg);

					if (validated == false)
					{
						this.PopulateMapSources(postedModel, postedModel.MapSource);
						postedModel.IsEditable = this.IsEditable();

						this.ModelState.AddModelError("Error", errorMsg);
						return this.View(postedModel);
					}

					if (postedModel.AssetTrackingMapConfigurationGuid == Guid.Empty)
					{
						this.InsertRecordToDatabase(postedModel);
					}
					else
					{
						this.UpdateRecordToDatabase(postedModel);
					}

					this.SetCoordinateInfoInSession(postedModel);
					return this.RedirectToAction("MapConfigurationSummary", "AssetMapConfigurationSummary");
				}

		        MapConfigurationModel mapConfigModel;
		        if (buttonAction == Buttons.New)
				{
					if (string.IsNullOrEmpty(postedModel.MapName))
					{
						this.PopulateMapSources(postedModel, postedModel.MapSource);
						postedModel.IsEditable = this.IsEditable();

						this.ModelState.AddModelError("Error", "Map name is required.");
						return this.View(postedModel);
					}

					string errorMsg;
					bool validated = this.ValidateCoordinates(postedModel, out errorMsg);

					if (validated == false)
					{
						this.PopulateMapSources(postedModel, postedModel.MapSource);
						postedModel.IsEditable = this.IsEditable();

						this.ModelState.AddModelError("Error", errorMsg);
						return this.View(postedModel);
					}

					if (postedModel.AssetTrackingMapConfigurationGuid == Guid.Empty)
					{
						this.InsertRecordToDatabase(postedModel);
					}
					else
					{
						this.UpdateRecordToDatabase(postedModel);
					}

					this.SetCoordinateInfoInSession(postedModel);

					this.ModelState.Clear();
					mapConfigModel = new MapConfigurationModel { IsEditable = this.IsEditable() };
					this.PopulateMapSources(mapConfigModel, MapConfigurationModel.MapSources.OpenStreetMap);

					return this.View(mapConfigModel);
				}

				if (buttonAction == Buttons.Cancel)
				{
					return this.RedirectToAction("MapConfigurationSummary", "AssetMapConfigurationSummary");
				}

				if (postedModel == null)
				{
					mapConfigModel = new MapConfigurationModel { IsEditable = this.IsEditable() };
					this.PopulateMapSources(mapConfigModel, MapConfigurationModel.MapSources.OpenStreetMap);

					return this.View(mapConfigModel);
				}

				postedModel.IsEditable = this.IsEditable();
				this.PopulateMapSources(postedModel, postedModel.MapSource);

				return this.View(postedModel);
			}
			catch (Exception ex)
			{
				this.ModelState.AddModelError("Error", "Error: " + ex.Message);
				
				this.PopulateMapSources(postedModel, (postedModel == null ? MapConfigurationModel.MapSources.OpenStreetMap : postedModel.MapSource));

				return this.View(postedModel);
			}
        }

		/// <summary>
		/// This method will handle the events coming from the Map Configuration Summary
		/// page.
		/// </summary>
		/// <param name="mapConfigurationGuid">The GUID to edit or create a new one.</param>
		/// <returns>Returns the view.</returns>
		[HttpGet]
		[RequireRouteValues(new [] { "mapConfigurationGuid" })]
		public ActionResult MapConfiguration(Guid mapConfigurationGuid)
		{
			MapConfigurationModel mapConfigModel;

			if (mapConfigurationGuid == Guid.Empty)
			{
				mapConfigModel = new MapConfigurationModel { IsEditable = this.IsEditable() };
				this.PopulateMapSources(mapConfigModel, MapConfigurationModel.MapSources.OpenStreetMap);

				return this.View(mapConfigModel);
			}

			mapConfigModel = this.GetMapConfiguration(mapConfigurationGuid);
			mapConfigModel.IsEditable = this.IsEditable(mapConfigModel.SiteGuid);
			this.PopulateMapSources(mapConfigModel, mapConfigModel.MapSource);

			// This will be used by the popup.
			this.Session[AssetCalculateCoordinatesController.SessionCalculateCoordinateMapSource] = mapConfigModel.MapSource;

			return this.View(mapConfigModel);
		}

		/// <summary>
		/// This method will handle the window modal dialog event. It will redirect to the 
		/// calculate coordinates MVC page.
		/// </summary>
		/// <param name="ispopup"></param>
		/// <param name="latitude"></param>
		/// <param name="longitude"></param>
		/// <param name="zoom"></param>
		/// <returns>Returns the view.</returns>
		[HttpGet]
		[RequireRouteValues(new[] { "ispopup", "latitude", "longitude", "zoom" })]
		public ActionResult MapConfiguration(string ispopup, string latitude, string longitude, string zoom)
		{
			if (string.IsNullOrEmpty(ispopup) == false && ispopup.Equals("True"))
			{
				return this.RedirectToAction("CalculateCoordinates", "AssetCalculateCoordinates", 
											new { latitudeStr = latitude, longitudeStr = longitude, zoomStr = zoom });
			}

			// This should never happen.
			return this.View(new MapConfigurationModel { IsEditable = this.IsEditable() });
		}

		/// <summary>
		/// This method will handle the action event coming from the Map Coordinate page.
		/// </summary>
		/// <param name="latitudeStr">The latitude as a string.</param>
		/// <param name="longitudeStr">The longitude as a string.</param>
		/// <param name="zoomStr">The zoom as a string.</param>
		/// <returns>The view with the model.</returns>
		[HttpGet]
		[RequireRouteValues(new[] { "LatitudeStr", "LongitudeStr", "ZoomStr" })]
		public ActionResult MapConfiguration(string latitudeStr, string longitudeStr, string zoomStr)
		{
			MapConfigurationModel mapConfigModel;

			if (this.Session[SessionMapConfigModel] != null)
			{
				mapConfigModel = (MapConfigurationModel)this.Session[SessionMapConfigModel];
				this.PopulateMapSources(mapConfigModel, mapConfigModel.MapSource);

				this.Session.Remove(SessionMapConfigModel);
			}
			else
			{
				mapConfigModel = new MapConfigurationModel { IsEditable = this.IsEditable() };
				this.PopulateMapSources(mapConfigModel, MapConfigurationModel.MapSources.OpenStreetMap);
			}

			if (latitudeStr.Equals("none") == false
				&& longitudeStr.Equals("none") == false
				&& zoomStr.Equals("none") == false)
			{
				mapConfigModel.LatitudeStr	= latitudeStr;
				mapConfigModel.LongitudeStr = longitudeStr;
				mapConfigModel.Zoom			= int.Parse(zoomStr);
			}

			return this.View(mapConfigModel);
		}

		/// <summary>
		/// This method will set the coordinate info into session for the popup.
		/// </summary>
		private void SetCoordinateInfoInSession(MapConfigurationModel mapModel)
		{
			if (mapModel.Latitude != null)
			{
				this.Session[AssetCalculateCoordinatesController.SessionCalculateCoordinateLatitude] = mapModel.LatitudeStr;
			}

			if (mapModel.Longitude != null)
			{
				this.Session[AssetCalculateCoordinatesController.SessionCalculateCoordinateLongitude] = mapModel.LongitudeStr;
			}

			this.Session[AssetCalculateCoordinatesController.SessionCalculateCoordinateZoom]		= mapModel.Zoom.ToString();
			this.Session[AssetCalculateCoordinatesController.SessionCalculateCoordinateMapSource]	= mapModel.MapSource;
		}

		/// <summary>
		/// This method will validate the latitude and longitude. If one is present, then
		/// must have both. Must be numeric values and be in the correct range.
		/// </summary>
		/// <param name="mapModel">The model that contains the coordinates.</param>
		/// <param name="errorMsg">The error message that will be return.</param>
		/// <returns>Returns false if there was an error and true if there was not an error.</returns>
		private bool ValidateCoordinates(MapConfigurationModel mapModel, out string errorMsg)
		{
			if (string.IsNullOrEmpty(mapModel.LatitudeStr) 
				&& string.IsNullOrEmpty(mapModel.LongitudeStr)
			    && string.IsNullOrEmpty(mapModel.ZoomStr))
			{
				errorMsg = "Latitude, Longitude, and Zoom are required.";
				return false;
			}

			if ((string.IsNullOrEmpty(mapModel.LatitudeStr) == false && string.IsNullOrEmpty(mapModel.LongitudeStr))
				|| (string.IsNullOrEmpty(mapModel.LongitudeStr) == false && string.IsNullOrEmpty(mapModel.LatitudeStr)))
			{
				errorMsg = "Must have both Latitude and Longitude.";
				return false;
			}

			if ((string.IsNullOrEmpty(mapModel.LatitudeStr) == false || string.IsNullOrEmpty(mapModel.LongitudeStr) == false)
				&& string.IsNullOrEmpty(mapModel.ZoomStr))
			{
				errorMsg = "Must have a zoom value.";
				return false;
			}

			if (mapModel.Latitude < -90 || mapModel.Latitude > 90)
			{
				errorMsg = "Latitude must be between -90 and 90 degrees.";
				return false;
			}

			if (mapModel.Longitude < -180 || mapModel.Longitude > 180)
			{
				errorMsg = "Longitude must be between -180 and 180 degrees.";
				return false;
			}

			if (mapModel.Zoom < 1 || mapModel.Zoom > 25)
			{
				errorMsg = "Zoom must be between 1 and 25.";
				return false;
			}

			errorMsg = string.Empty;
			return true;
		}

		/// <summary>
		/// This method will return true if the user has modify rights.
		/// </summary>
		/// <returns>Return true if editable.</returns>
		private bool IsEditable()
		{
			return this.Security.HasRight(RIGHT.MODIFY_MAP_CONFIGURATION);
		}

		/// <summary>
		/// This method will return true if the user has modify rights.
		/// </summary>
		/// <returns>Return true if editable.</returns>
		private bool IsEditable(Guid mapConfigGuid)
		{
			bool editable = this.Security.HasRight(RIGHT.MODIFY_MAP_CONFIGURATION) && this.Security.SiteGuid == mapConfigGuid;
			return editable;
		}

		/// <summary>
		/// This method determine which button was pressed if any.
		/// </summary>
		/// <returns>Return the button pressed enumeration.</returns>
		private Buttons WhichButtonWasPressed()
		{
			string buttonPressed = this.Request.Params.AllKeys.FirstOrDefault(
								x => x.StartsWith("NewMapConfigBtn") 
								|| x.StartsWith("OkMapConfigBtn") 
								|| x.StartsWith("CancelMapConfigBtn"));

			if (String.IsNullOrEmpty(buttonPressed))
			{
				return Buttons.None;
			}

			if (buttonPressed.Equals("NewMapConfigBtn"))
			{
				return Buttons.New;
			}

			if (buttonPressed.Equals("OkMapConfigBtn"))
			{
				return Buttons.Ok;
			}

			if (buttonPressed.Equals("CancelMapConfigBtn"))
			{
				return Buttons.Cancel;
			}

			return Buttons.None;
		}

		/// <summary>
		/// This method will get the map configuration data from the database based on the map
		/// configuration GUID.
		/// </summary>
		/// <param name="mapConfigGuid">The GUID used to get the map configuration data.</param>
		/// <returns>Returns a populated model.</returns>
		private MapConfigurationModel GetMapConfiguration(Guid mapConfigGuid)
		{
			var mapConfiguration = 
						FMChannelHelper.MakeCall<IAssetTrackingMapConfigurations, AssetTrackingMapConfigurationClass>(
													x => x.Get(this.Security, mapConfigGuid));

			if (mapConfiguration == null)
			{
				return null;
			}

			var mapConfigModel = new MapConfigurationModel
			                     {
				                     AssetTrackingMapConfigurationGuid	= mapConfiguration.AssetTrackingMapConfigurationGuid,
				                     MapName							= mapConfiguration.MapName,
				                     Latitude							= mapConfiguration.Latitude,
									 Longitude							= mapConfiguration.Longitude,
									 Zoom								= mapConfiguration.Zoom,
									 MapSource							= (MapConfigurationModel.MapSources)mapConfiguration.MapSource,
									 Description						= mapConfiguration.Description,
									 Show								= mapConfiguration.Active,
									 SiteGuid							= mapConfiguration.SiteGuid
			                     };

			return mapConfigModel;
		}

		/// <summary>
		/// This method will populate the Map Source dropdown and set the selected item.
		/// </summary>
		/// <param name="mapConfigModel">The map configuration model use to populate the dropdown.</param>
		/// <param name="selectedMapSource">The selected map type.</param>
		private void PopulateMapSources(MapConfigurationModel mapConfigModel, MapConfigurationModel.MapSources selectedMapSource)
		{
			List<SelectListItem> itemList = new List<SelectListItem>();

			var selectListItem = new SelectListItem
			                     {
				                     Value		= MapConfigurationModel.MapSources.GoogleMap.ToString(),
				                     Text		= mapConfigModel.GetMapSourceName(MapConfigurationModel.MapSources.GoogleMap),
									 Selected	= selectedMapSource == MapConfigurationModel.MapSources.GoogleMap
			                     };
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
							{
								Value		= MapConfigurationModel.MapSources.MapServer.ToString(),
								Text		= mapConfigModel.GetMapSourceName(MapConfigurationModel.MapSources.MapServer),
								Selected	= selectedMapSource == MapConfigurationModel.MapSources.MapServer
							};
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
							{
								Value		= MapConfigurationModel.MapSources.BingMap.ToString(),
								Text		= mapConfigModel.GetMapSourceName(MapConfigurationModel.MapSources.BingMap),
								Selected	= selectedMapSource == MapConfigurationModel.MapSources.BingMap
							};
			itemList.Add(selectListItem);

			selectListItem = new SelectListItem
							{
								Value		= MapConfigurationModel.MapSources.OpenStreetMap.ToString(),
								Text		= mapConfigModel.GetMapSourceName(MapConfigurationModel.MapSources.OpenStreetMap),
								Selected	= selectedMapSource == MapConfigurationModel.MapSources.OpenStreetMap
							};
			itemList.Add(selectListItem);

			mapConfigModel.ActionListMapSources = itemList;
		}

		
		/// <summary>
		/// This method will create an asset map configuration record.
		/// </summary>
		/// <param name="postedModel">The data to save.</param>
		private void InsertRecordToDatabase(MapConfigurationModel postedModel)
		{
			if (postedModel == null)
			{
				throw new ArgumentException("Model is null.");
			}

			if (string.IsNullOrEmpty(postedModel.MapName))
			{
				throw new Exception("Map name is required.");
			}

			// If null, then set to default. Otherwise, set to value.
			double lat	= postedModel.Latitude ?? 0;
			double lng	= postedModel.Longitude ?? 0;
			int zoom	= postedModel.Zoom ?? 1;
			AssetTrackingMapConfigurationClass.MapSources source;

			switch (postedModel.MapSource)
			{
				case MapConfigurationModel.MapSources.OpenStreetMap:
					source = AssetTrackingMapConfigurationClass.MapSources.OpenStreetMap;
					break;
				case MapConfigurationModel.MapSources.GoogleMap:
					source = AssetTrackingMapConfigurationClass.MapSources.GoogleMap;
					break;
				case MapConfigurationModel.MapSources.MapServer:
					source = AssetTrackingMapConfigurationClass.MapSources.MapServer;
					break;
				case MapConfigurationModel.MapSources.BingMap:
					source = AssetTrackingMapConfigurationClass.MapSources.BingMap;
					break;
				default:
					source = AssetTrackingMapConfigurationClass.MapSources.OpenStreetMap;
					break;
			}

			var mapConfig = new AssetTrackingMapConfigurationClass()
			             {
				             MapName				= postedModel.MapName.Trim(),
				             Latitude				= lat,
							 Longitude				= lng,
							 Zoom					= zoom,
							 MapSource				= source,
							 Active					= postedModel.Show,
							 Description			= postedModel.Description,
							 SiteGuid				= postedModel.SiteGuid
			             };

			postedModel.AssetTrackingMapConfigurationGuid = 
					FMChannelHelper.MakeCall<IAssetTrackingMapConfigurations, Guid>(x => x.Add(this.Security, mapConfig));
		}

		/// <summary>
		/// This method will update an asset tracking map configuration record.
		/// </summary>
		/// <param name="postedModel">The data to save.</param>
		private void UpdateRecordToDatabase(MapConfigurationModel postedModel)
		{
			if (postedModel == null)
			{
				throw new ArgumentException("Model is null.");
			}

			if (String.IsNullOrEmpty(postedModel.MapName))
			{
				throw new Exception("Map Name is required.");
			}

			// If null, then set to default. Otherwise, set to value.
			double lat	= postedModel.Latitude ?? 0;
			double lng	= postedModel.Longitude ?? 0;
			int zoom	= postedModel.Zoom ?? 1;
			AssetTrackingMapConfigurationClass.MapSources source;

			switch (postedModel.MapSource)
			{
				case MapConfigurationModel.MapSources.OpenStreetMap:
					source = AssetTrackingMapConfigurationClass.MapSources.OpenStreetMap;
					break;
				case MapConfigurationModel.MapSources.GoogleMap:
					source = AssetTrackingMapConfigurationClass.MapSources.GoogleMap;
					break;
				case MapConfigurationModel.MapSources.MapServer:
					source = AssetTrackingMapConfigurationClass.MapSources.MapServer;
					break;
				case MapConfigurationModel.MapSources.BingMap:
					source = AssetTrackingMapConfigurationClass.MapSources.BingMap;
					break;
				default:
					source = AssetTrackingMapConfigurationClass.MapSources.OpenStreetMap;
					break;
			}

			var mapConfig = new AssetTrackingMapConfigurationClass()
							{
								AssetTrackingMapConfigurationGuid	= postedModel.AssetTrackingMapConfigurationGuid,
								MapName								= postedModel.MapName.Trim(),
								Latitude							= lat,
								Longitude							= lng,
								Zoom								= zoom,
								MapSource							= source,
								Active								= postedModel.Show,
								Description							= postedModel.Description
			};

			FMChannelHelper.MakeCall<IAssetTrackingMapConfigurations>(x => x.Modify(this.Security, mapConfig));
		}
    }

	#region Require Route Value Attribute class
	/// <summary>
	/// This class allows the overloading of an ActionResult HttpGet methods.
	/// </summary>
	public class RequireRouteValuesAttribute : ActionMethodSelectorAttribute
	{
		public string[] ValueNames { get; set; }

		public RequireRouteValuesAttribute(string[] valueNames)
		{
			this.ValueNames = valueNames;
		}

		public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
		{
			bool contains = false;
			foreach (var value in this.ValueNames)
			{
				contains = controllerContext.RequestContext.HttpContext.Request.Params.AllKeys.Contains(value);

				if (!contains)
				{
					break;
				}
			}

			return contains;
		}
	}
	#endregion
}
