namespace FuelsManager.Areas.AssetTrackingArea.ViewModels
{
	using System;

	[Serializable]
	public class AssetIconConfigurationModel
	{
	    #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public AssetIconConfigurationModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
        public string AssetTrackingIconConfigurationGuidStr { get; set; }
        public string SiteGuidStr { get; set; }
		public string IconConfigurationId { get; set; }
		public string EquipmentIconName { get; set; }
		public string EquipmentVarianceIconName { get; set; }
		public string EquipmentInvestigationIconName { get; set; }
		public string EquipmentCompleteInvestigationFailedIconName { get; set; }
		public string EquipmentCompleteInvestigationPassedIconName { get; set; }
		public string FacilityIconName { get; set; }
		public string DeliveryLocationIconName { get; set; }
		public string BreadcrumbIconName { get; set; }
		public string BreadcrumbVarianceIconName { get; set; }
		public string BreadcrumbInvestigationIconName { get; set; }
		public string BreadcrumbCompleteInvestigationFailedIconName { get; set; }
		public string BreadcrumbCompleteInvestigationPassedIconName { get; set; }
		public string TankIconName { get; set; }
		public string MapPinIconName { get; set; }
		public string IconPath { get; set; }
		public bool IsEditable { get; set; }
		public bool PostFromPopup { get; set; }

        public Guid AssetTrackingIconConfigurationGuid
        {
            get
            {
                Guid retGuid;
                if (Guid.TryParse(this.AssetTrackingIconConfigurationGuidStr, out retGuid) == false)
                {
                    return Guid.Empty;
                }

                return retGuid;
            }
            set
            {
                this.AssetTrackingIconConfigurationGuidStr = value.ToString();
            }
        }

	    public Guid SiteGuid
	    {
	        get
	        {
                Guid retGuid;
                if (Guid.TryParse(this.SiteGuidStr, out retGuid) == false)
                {
                    return Guid.Empty;
                }

                return retGuid;
            }
	        set
	        {
                this.SiteGuidStr = value.ToString();
            }
	    }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Init()
		{
			this.AssetTrackingIconConfigurationGuidStr			= string.Empty;
			this.IconConfigurationId							= string.Empty;
			this.EquipmentIconName								= string.Empty;
			this.EquipmentVarianceIconName						= string.Empty;
			this.EquipmentInvestigationIconName					= string.Empty;
			this.EquipmentCompleteInvestigationFailedIconName	= string.Empty;
			this.EquipmentCompleteInvestigationPassedIconName	= string.Empty;
			this.FacilityIconName								= string.Empty;
			this.DeliveryLocationIconName						= string.Empty;
			this.BreadcrumbIconName								= string.Empty;
			this.BreadcrumbVarianceIconName						= string.Empty;
			this.BreadcrumbInvestigationIconName				= string.Empty;
			this.BreadcrumbCompleteInvestigationFailedIconName	= string.Empty;
			this.BreadcrumbCompleteInvestigationPassedIconName	= string.Empty;
			this.TankIconName									= string.Empty;
			this.MapPinIconName									= string.Empty;
			this.IconPath										= string.Empty;
			this.SiteGuidStr									= string.Empty;
			this.IsEditable										= false;
			this.PostFromPopup									= false;
		}
		#endregion
	}
}