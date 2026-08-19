namespace FuelsManager.Areas.AssetTrackingArea.ViewModels
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class AssetMapsTankModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetMapsTankModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string TankId { get; set; }
		public Guid TankGuid { get; set; }
		public string TankName { get; set; }
		public List<double> LatitudeList { get; set; }
		public List<double> LongitudeList { get; set; }
		public string SelectedTank { get; set; }
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
			this.TankId					= string.Empty;
			this.TankGuid				= Guid.Empty;
			this.TankName				= string.Empty;
			this.LatitudeList			= new List<double>();
			this.LongitudeList			= new List<double>();
			this.SelectedTank			= string.Empty;
			this.HasCoordinates			= 0;
			this.HoverDescriptionList	= new List<string>();
		}
		#endregion
	}
}