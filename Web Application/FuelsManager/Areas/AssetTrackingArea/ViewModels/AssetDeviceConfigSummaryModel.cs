namespace FuelsManager.Areas.AssetTrackingArea.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;

	[Serializable]
	public class AssetDeviceConfigSummaryModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public AssetDeviceConfigSummaryModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public bool IsEditable { get; set; }
		public List<AssetDeviceConfigurationModel> DeviceConfigurationList { get; set; }
		public string FindText { get; set; }
		public Guid SiteGuid { get; set; }
		public List<SelectListItem> ActionListActivationStatus { get; set; }
		public string ActivationStatusId { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.IsEditable					= false;
			this.DeviceConfigurationList	= new List<AssetDeviceConfigurationModel>();
			this.FindText					= string.Empty;
			this.SiteGuid					= Guid.Empty;
			this.ActionListActivationStatus = new List<SelectListItem>();
		}
		#endregion
	}
}