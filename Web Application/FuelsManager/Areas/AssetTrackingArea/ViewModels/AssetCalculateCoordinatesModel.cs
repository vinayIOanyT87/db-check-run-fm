namespace FuelsManager.Areas.AssetTrackingArea.ViewModels
{
	using System;
	using System.Globalization;

	[Serializable]
	public class AssetCalculateCoordinatesModel
	{
		#region Constructors
		/// <summary>
		///     This is the default constructor.
		/// </summary>
		public AssetCalculateCoordinatesModel()
		{
			this.Init();
		}
		#endregion

		#region Public Properties
		public string LatitudeStr
		{
			get
			{
				return this.Latitude.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				double outLat;
				this.Latitude = 0.0;

				if (double.TryParse(value, out outLat))
				{
					this.Latitude = outLat;
				}
			}
		}

		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public int Zoom { get; set; }

		public string LongitudeStr
		{
			get
			{
				return this.Longitude.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				double outLng;
				this.Longitude = 0.0;

				if (double.TryParse(value, out outLng))
				{
					this.Longitude = outLng;
				}
			}
		}

		public string ZoomStr
		{
			get
			{
				return this.Zoom.ToString();
			}
			set
			{
				int outZoom;
				this.Zoom = 1;

				if (int.TryParse(value, out outZoom))
				{
					this.Zoom = outZoom;
				}
			}
		}

		public string BreadcrumbIcon { get; set; }
		public string TankIcon { get; set; }
		public string FacilityIcon { get; set; }
		public string EquipmentIcon { get; set; }
		public string DeliveryLocationIcon { get; set; }
		public string MapPinIcon { get; set; }

		public AssetMapsDeliveryLocationModel DeliveryLocationModel { get; set; }
		public AssetMapsFacilityModel FacilityModel { get; set; }
		public AssetMapsTankModel TankModel { get; set; }
		public int MapSourceIndex { get; set; }

		public MapConfigurationModel.MapSources MapSource
		{
			get { return (MapConfigurationModel.MapSources)this.MapSourceIndex; }
			set { this.MapSourceIndex = (int)value; }
		}
		#endregion

		#region Private methods
		/// <summary>
		///     This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.Latitude				= 0.0;
			this.Longitude				= 0.0;
			this.Zoom					= 1;
			this.DeliveryLocationModel	= null;
			this.FacilityModel			= null;
			this.TankModel				= null;
			this.MapPinIcon				= "~/Areas/images/AssetMapImages/MapIcons/MapPin.png";
			this.BreadcrumbIcon			= "~/Areas/images/AssetMapImages/MapIcons/Dark-green-triangle.png";
			this.TankIcon				= "~/Areas/images/AssetMapImages/MapIcons/Tank.png";
			this.FacilityIcon			= "~/Areas/images/AssetMapImages/MapIcons/Tag-Icon.png";
			this.EquipmentIcon			= "~/Areas/images/AssetMapImages/MapIcons/Truck-Icon.png";
			this.DeliveryLocationIcon	= "~/Areas/images/AssetMapImages/MapIcons/Flag.png";

			this.MapSource				= MapConfigurationModel.MapSources.OpenStreetMap;
		}
		#endregion
	}
}