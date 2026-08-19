namespace FuelsManager.Areas.AssetTrackingArea.ViewModels
{
	using System;
	using System.Globalization;

	using FMBusinessObjects.DataObjects;

	[Serializable]
	public class AssetMapsViewModel
	{
		#region Public data members
		public enum ActiveButtons { Refresh, None };
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetMapsViewModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public AssetMapsEquipmentModel EquipmentModel { get; set; }
		public AssetMapsDeliveryLocationModel DeliveryLocationModel { get; set; }
		public AssetMapsFacilityModel FacilityModel { get; set; }
		public AssetMapsTankModel TankModel { get; set; }
		public AssetMapsDropdownModel DropdownModel { get; set; }
		public double MapLatitude { get; set; }
		public double MapLongitude { get; set; }
		public int Zoom { get; set; }
		public string MapName { get; set; }
		public string BreadcrumbIcon { get; set; }
		public string TankIcon { get; set; }
		public string FacilityIcon { get; set; }
		public string EquipmentIcon { get; set; }
		public string DeliveryLocationIcon { get; set; }
		public string EquipmentContaminatedIcon { get; set; }
		public string EquipmentInvestigationIcon { get; set; }
		public string EquipmentCompleteInvestigationFailedIcon { get; set; }
		public string EquipmentCompleteInvestigationPassedIcon { get; set; }
		public string BreadcrumbContaminatedIcon { get; set; }
		public string BreadcrumbInvestigationIcon { get; set; }
		public string BreadcrumbCompleteInvestigationFailedIcon { get; set; }
		public string BreadcrumbCompleteInvestigationPassedIcon { get; set; }
		public int MapRefreshOn { get; set; }
		public int MapRefreshTimeInMilliSeconds { get; set; }
		public int MapSourceIndex { get; set; }
		public int ActiveButtonIndex { get; set; }
		public bool UseExtent { get; set; }
		public bool MenuItemChange { get; set; }
		public string EquipmentLabelDictionary { get; set; }
		public string FacilityLabelDictionary { get; set; }
		public string DeliveryLocationLabelDictionary { get; set; }
		public string TankLabelDictionary { get; set; }
		public string CompartmentLabelDictionary { get; set; }
		public string ProductLabelDictionary { get; set; }
		public string VolumeLabelDictionary { get; set; }
		public string DensityLabelDictionary { get; set; }
		public string TimestampLabelDictionary { get; set; }
		public string ViewHistoryLabelDictionary { get; set; }
		public string LatitudeLabelDictionary { get; set; }
		public string LongitudeLabelDictionary { get; set; }
        public string TemperatureLabelDictionary { get; set; }
        public string PressureLabelDictionary { get; set; }

        public string MapLatitudeStr 
		{
			get
			{
				return this.MapLatitude.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				this.MapLatitude = 0.0;
				double outLatitude;

				if (double.TryParse(value, out outLatitude))
				{
					this.MapLatitude = outLatitude;
				}
			}
		}

		public string MapLongitudeStr
		{
			get
			{
				return this.MapLongitude.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				this.MapLongitude = 0.0;
				double outLongitude;

				if (double.TryParse(value, out outLongitude))
				{
					this.MapLongitude = outLongitude;
				}
			}
		}

		public AssetTrackingMapConfigurationClass.MapSources MapSource
		{
			get
			{
				return (AssetTrackingMapConfigurationClass.MapSources)this.MapSourceIndex;
			}
			set
			{
				this.MapSourceIndex = (int)value;
			}
		}

		public ActiveButtons ActiveButton
		{
			get
			{
				return (ActiveButtons)this.ActiveButtonIndex;
			}
			set
			{
				this.ActiveButtonIndex = (int)value;
			}
		}

		public int UseExtentInt
		{
			get
			{
				return this.UseExtent ? 1 : 0;
			}
			set
			{
				this.UseExtent = value == 1;
			}
		}
		#endregion

		#region Private methods

		/// <summary>
		/// This method will initial the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.EquipmentModel					= null;
			this.DeliveryLocationModel			= null;
			this.FacilityModel					= null;
			this.TankModel						= null;
			this.DropdownModel					= new AssetMapsDropdownModel();
			this.MapLatitude					= 0.0;
			this.MapLongitude					= 0.0;
			this.Zoom							= 10;
			this.MapName						= "Unknown";
			this.BreadcrumbIcon					= string.Empty;
			this.TankIcon						= string.Empty;
			this.FacilityIcon					= string.Empty;
			this.EquipmentIcon					= string.Empty;
			this.DeliveryLocationIcon			= string.Empty;
			this.EquipmentContaminatedIcon		= string.Empty;
			this.EquipmentInvestigationIcon		= string.Empty;
			this.BreadcrumbContaminatedIcon		= string.Empty;
			this.BreadcrumbInvestigationIcon	= string.Empty;
			this.MapRefreshOn					= 0;
			this.MapRefreshTimeInMilliSeconds	= 0;
			this.MapSourceIndex					= (int)AssetTrackingMapConfigurationClass.MapSources.OpenStreetMap;
			this.UseExtent						= false;
			this.MenuItemChange					= false;

			this.EquipmentCompleteInvestigationFailedIcon	= string.Empty;
			this.EquipmentCompleteInvestigationPassedIcon	= string.Empty;
			this.BreadcrumbCompleteInvestigationFailedIcon	= string.Empty;
			this.BreadcrumbCompleteInvestigationPassedIcon	= string.Empty;
		}
		#endregion
	}
}