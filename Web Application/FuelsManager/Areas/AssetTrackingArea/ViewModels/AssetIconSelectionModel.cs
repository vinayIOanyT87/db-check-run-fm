namespace FuelsManager.Areas.AssetTrackingArea.ViewModels
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class AssetIconSelectionModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public AssetIconSelectionModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public List<IconModel> IconModelList { get; set; }
		public string FindText { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.IconModelList = new List<IconModel>();
			this.FindText = string.Empty;
		}
		#endregion
	}

	[Serializable]
	public class IconModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public IconModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public int IconKey { get; set; }
		public string IconImage { get; set; }
		public string IconFileName { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.IconFileName	= string.Empty;
			this.IconImage		= string.Empty;
			this.IconKey		= 0;
		}
		#endregion
	}
}