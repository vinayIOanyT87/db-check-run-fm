namespace FuelsManager.Areas.AssetTrackingArea.ViewModels
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class AssetMapsFacilityModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetMapsFacilityModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string FacilityId { get; set; }
		public Guid FacilityGuid { get; set; }
		public string FacilityName { get; set; }
		public List<double> LatitudeList { get; set; }
		public List<double> LongitudeList { get; set; }
		public string SelectedFacility { get; set; }
		public int HasCoordinates { get; set; }
		public List<string> HoverDescriptionList { get; set; } 
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
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.FacilityId				= string.Empty;
			this.FacilityGuid			= Guid.Empty;
			this.FacilityName			= string.Empty;
			this.LatitudeList			= new List<double>();
			this.LongitudeList			= new List<double>();
			this.SelectedFacility		= string.Empty;
			this.HasCoordinates			= 0;
			this.HoverDescriptionList	= new List<string>();
		}
		#endregion
	}
}