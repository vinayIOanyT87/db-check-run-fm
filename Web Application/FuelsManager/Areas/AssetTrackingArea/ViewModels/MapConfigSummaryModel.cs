namespace FuelsManager.Areas.AssetTrackingArea.ViewModels
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class MapConfigSummaryModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public MapConfigSummaryModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public bool IsEditable { get; set; }
		public List<MapConfigurationModel> MapConfigurationList { get; set; }
		public string FindText { get; set; }
		public Guid SiteGuid { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.IsEditable				= false;
			this.MapConfigurationList	= new List<MapConfigurationModel>();
			this.FindText				= string.Empty;
			this.SiteGuid				= Guid.Empty;
		}
		#endregion
	}
}