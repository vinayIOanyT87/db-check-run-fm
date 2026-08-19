namespace FuelsManager.Areas.AssetTrackingArea.ViewModels
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class AssetMapsDeliveryLocationModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetMapsDeliveryLocationModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string DeliveryLocationId { get; set; }
		public Guid DeliveryLocationGuid { get; set; }
		public string DeliveryLocationName { get; set; }
		public List<double> LatitudeList { get; set; }
		public List<double> LongitudeList { get; set; }
		public List<string> HoverDescriptionList { get; set; } 
		public int HasCoordinates { get; set; }
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
			this.DeliveryLocationId		= string.Empty;
			this.DeliveryLocationGuid	= Guid.Empty;
			this.DeliveryLocationName	= string.Empty;
			this.LatitudeList			= new List<double>();
			this.LongitudeList			= new List<double>();
			this.HasCoordinates			= 0;
			this.HoverDescriptionList	= new List<string>();
		}
		#endregion
	}
}