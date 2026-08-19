namespace FuelsManager.Areas.AssetTrackingArea.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Web.Mvc;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

    [Serializable]
    public class AssetDeviceConfigurationModel
    {
        #region Public members
        public enum AssetTrackingDeviceTypes { Tdu, Wrdcu, Standard };
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public AssetDeviceConfigurationModel()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public string DeviceId { get; set; }
        public string Description { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public bool Active { get; set; }
        public List<SelectListItem> ActionListEquipment { get; set; }
        public bool IsEditable { get; set; }
        public string EquipmentId { get; set; }
        public string SelectedEquipment { get; set; }
        public AssetTrackingDeviceTypes AssetTrackingDeviceType { get; set; }
        public List<SelectListItem> ActionListDeviceTypes { get; set; }
        public List<SelectListItem> ActionListAssociatedTanks { get; set; }
        public List<string> SelectedTanks { get; set; }
        public List<SelectListItem> ActionSourceUnits { get; set; }
        public int SourceUnit { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
		public string EquipmentGuidStr { get; set; }
        public string AssetTrackingDeviceGuidStr { get; set; }
        public string SiteGuidStr { get; set; }
        public string RedirectToEquipmentUrl { get; set; }

        public Guid EquipmentGuid
        {
            get 
            {
                Guid equipGuid;
                if(Guid.TryParse(this.EquipmentGuidStr, out equipGuid))
                {
                    return equipGuid;
                }

                return Guid.Empty;
            }

            set { this.EquipmentGuidStr = value.ToString(); }
        }

        public Guid AssetTrackingDeviceGuid
        {
            get
            {
                Guid deviceGuid;
                if (Guid.TryParse(this.AssetTrackingDeviceGuidStr, out deviceGuid))
                {
                    return deviceGuid;
                }

                return Guid.Empty;
            }

            set { this.AssetTrackingDeviceGuidStr = value.ToString(); }
        }

        public Guid SiteGuid
        {
            get
            {
                Guid siteGuid;
                if (Guid.TryParse(this.SiteGuidStr, out siteGuid))
                {
                    return siteGuid;
                }

                return Guid.Empty;
            }

            set { this.SiteGuidStr = value.ToString(); }
        }

        public string ActivationStatusStr => this.Active ? "Active" : "Inactive";

        #endregion

		#region Public methods
		/// <summary>
		/// This method will return the string name for the Asset TrackingDevice Type name.
		/// </summary>
		/// <param name="deviceType">The enumerated asset tracking device type.</param>
		/// <returns>Returns a string name of the asset tracking device type.</returns>
		public string GetAssetTrackingDeviceTypeName(AssetTrackingDeviceTypes deviceType)
		{
			switch (deviceType)
			{
				case AssetTrackingDeviceTypes.Tdu:
					return "TDU";
				case AssetTrackingDeviceTypes.Wrdcu:
					return "WRDCU";
				case AssetTrackingDeviceTypes.Standard:
					return "Standard";
				default:
					return "Standard";
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.AssetTrackingDeviceGuidStr	= string.Empty;
			this.SiteGuidStr				= string.Empty;
			this.DeviceId					= string.Empty;
			this.Description				= string.Empty;
			this.ModelNumber				= string.Empty;
			this.SerialNumber				= string.Empty;
			this.Active						= false;
			this.EquipmentId				= string.Empty;
			this.EquipmentGuidStr			= string.Empty;
			this.SourceUnit					= (int)EngineeringUnit.FmvMeter3;
			this.ActionListEquipment		= new List<SelectListItem>();
			this.IsEditable					= false;
			this.AssetTrackingDeviceType	= AssetTrackingDeviceTypes.Standard;
			this.ActionListDeviceTypes		= new List<SelectListItem>();
			this.ActionListAssociatedTanks	= new List<SelectListItem>();
			this.ActionSourceUnits			= new List<SelectListItem>();
			this.SelectedTanks				= new List<string>();
            this.HasError                   = false;
            this.ErrorMessage               = string.Empty;
            this.SelectedEquipment          = string.Empty;
		    this.RedirectToEquipmentUrl     = string.Empty;
		}
		#endregion
	}
}