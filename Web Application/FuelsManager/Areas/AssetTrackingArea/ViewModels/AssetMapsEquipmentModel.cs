namespace FuelsManager.Areas.AssetTrackingArea.ViewModels
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class AssetMapsEquipmentModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetMapsEquipmentModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public Guid EquipmentGuid { get; set; }
		public string EquipmentId { get; set; }
		public int HasCoordinates { get; set; }
		public string SelectedEquipment { get; set; }
		public List<double> LatitudeList { get; set; }
		public List<double> LongitudeList { get; set; }
		public List<string> MarkerTypeList { get; set; }
		public List<string> HoverDescriptionList { get; set; }
		public List<string> ItemColorList { get; set; } 
		#endregion

		#region Public methods
		/// <summary>
		/// This method will reset the object to its initial state.
		/// </summary>
		public void Reset()
		{
			this.Init();
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initial the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.EquipmentGuid				= Guid.Empty;
			this.EquipmentId				= string.Empty;
			this.HasCoordinates				= 0;
			this.SelectedEquipment			= string.Empty;
			this.LatitudeList				= new List<double>();
			this.LongitudeList				= new List<double>();
			this.MarkerTypeList				= new List<string>();
			this.HoverDescriptionList		= new List<string>();
			this.ItemColorList					= new List<string>();
		}
		#endregion
	}
}