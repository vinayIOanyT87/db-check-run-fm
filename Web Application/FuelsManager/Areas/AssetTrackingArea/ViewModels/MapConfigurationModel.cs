namespace FuelsManager.Areas.AssetTrackingArea.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Globalization;
	using System.Web.Mvc;

	[Serializable]
	public class MapConfigurationModel
	{
		#region Public members
		public enum MapSources { OpenStreetMap, MapServer, GoogleMap, BingMap };
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MapConfigurationModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public Guid AssetTrackingMapConfigurationGuid { get; set; }
		public Guid SiteGuid { get; set; }
		public IEnumerable<SelectListItem> ActionListMapSources { get; set; }
		public MapSources MapSource { get; set; }
		public bool Show { get; set; }
		public bool EnableShow { get; set; }
		public bool IsEditable { get; set; }

		[MaxLength(20, ErrorMessage = "Map Name can only be 20 characters.")]
		[Required]
		public string MapName { get; set; }

		[MaxLength(200, ErrorMessage = "Description can only be 200 characters.")]
		public string Description { get; set; }

		[RegularExpression("^[0-9]*$", ErrorMessage = "Zoom must be numeric.")]
		public int? Zoom { get; set; }

		[RegularExpression("^[0-9]*$", ErrorMessage = "Latitude must be numeric.")]
		public double? Latitude { get; set; }

		[RegularExpression("^[0-9]*$", ErrorMessage = "Longitude must be numeric.")]
		public double? Longitude { get; set; }

		public string LatitudeStr
		{
			get
			{
				return this.Latitude == null ? string.Empty : this.Latitude.Value.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				this.Latitude = null;

				if (string.IsNullOrEmpty(value) == false)
				{
					double outLatitude;

					if (double.TryParse(value, out outLatitude))
					{
						this.Latitude = outLatitude;
					}
				}
			}
		}

		public string LongitudeStr
		{
			get
			{
				return this.Longitude == null ? string.Empty : this.Longitude.Value.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				this.Longitude = null;

				if (string.IsNullOrEmpty(value) == false)
				{
					double outLongitude;

					if (double.TryParse(value, out outLongitude))
					{
						this.Longitude = outLongitude;
					}
				}
			}
		}

		public string ZoomStr
		{
			get
			{
				return this.Zoom == null ? string.Empty : this.Zoom.Value.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				this.Zoom = null;

				if (string.IsNullOrEmpty(value) == false)
				{
					int outZoom;

					if (int.TryParse(value, out outZoom))
					{
						this.Zoom = outZoom;
					}
				}
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will return the string name for the Map Source name.
		/// </summary>
		/// <param name="mapSource">The enumerated map type.</param>
		/// <returns>Returns a string name of the map type.</returns>
		public string GetMapSourceName(MapSources mapSource)
		{
			switch (mapSource)
			{
				case MapSources.GoogleMap:
					return "Google Map";
				case MapSources.MapServer:
					return "MapServer Map";
				case MapSources.OpenStreetMap:
					return "Open Street Map";
				case MapSources.BingMap:
					return "Bing Map";
				default:
					return "Open Street Map";
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.AssetTrackingMapConfigurationGuid	= Guid.Empty;
			this.MapName							= "Unknown";
			this.Zoom								= null;		
			this.Latitude							= null;	
			this.Longitude							= null;
			this.ActionListMapSources				= new List<SelectListItem>();
			this.MapSource							= MapSources.OpenStreetMap;
			this.Description						= string.Empty;
			this.SiteGuid							= Guid.Empty;
			this.IsEditable							= false;
			this.Show								= false;
			this.EnableShow							= true;
		}
		#endregion
	}
}