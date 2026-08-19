namespace FuelsManager.Areas.AssetTrackingArea.ViewModels
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class AssetEquipmentHistoryModel
	{
		#region Contructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetEquipmentHistoryModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string EquipmentID { get; set; }
		public List<AssetEquipmentHistoryRecordModel> EquipmentHistoryRecordList { get; set; }
		public bool HasStartInvestigateRight { get; set; }
		public bool HasCompleteInvestigateRight { get; set; }
		public bool FoundInvestigateState { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.EquipmentHistoryRecordList			= new List<AssetEquipmentHistoryRecordModel>();
			this.EquipmentID						= string.Empty;
			this.HasCompleteInvestigateRight		= false;
			this.HasStartInvestigateRight			= false;
			this.FoundInvestigateState				= false;
		}
		#endregion
	}

	[Serializable]
	public class AssetEquipmentHistoryRecordModel
	{
		#region Contructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetEquipmentHistoryRecordModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string AssetTrackingDetailGuidStr { get; set; }
		public string SessionDatetimeStr { get; set; }
		public string AssetTrackingDeviceId { get; set; }
		public string GpsCoordinatesStr { get; set; }
		public string ProductId { get; set; }
		public string VolumeStr { get; set; }
		public string WaterStr { get; set; }
		public string DensityStr { get; set; }
		public bool IsCompartment { get; set; }
		public string CompartmentName { get; set; }
		public bool IsContaminated { get; set; }
		public int MessageState { get; set; }
		public List<AssetEquipmentHistoryRecordModel> CompartmentRecordList { get; set; }
		public string DielectricStr { get; set; }
		public string Remarks { get; set; }
		public bool HasExpansion { get; set; }
		#endregion

		#region Public methods
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.AssetTrackingDetailGuidStr			= string.Empty;
			this.SessionDatetimeStr					= string.Empty;
			this.AssetTrackingDeviceId				= string.Empty;
			this.GpsCoordinatesStr					= string.Empty;
			this.ProductId							= string.Empty;
			this.VolumeStr							= string.Empty;
			this.WaterStr							= "NO";
			this.DensityStr							= string.Empty;
			this.CompartmentName					= string.Empty;
			this.IsCompartment						= false;
			this.CompartmentRecordList				= new List<AssetEquipmentHistoryRecordModel>();
			this.IsContaminated						= false;
			this.DielectricStr						= string.Empty;
			this.Remarks							= string.Empty;
			this.MessageState						= 0;
			this.HasExpansion						= false;
		}
		#endregion
	}
}