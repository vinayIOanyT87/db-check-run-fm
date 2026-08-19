namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class ReportSummaryModel
	{
		#region Properties
		public List<ReportDetailModel> ReportDetailList { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public ReportSummaryModel()
		{
			this.ReportDetailList = new List<ReportDetailModel>();
		}
		#endregion
	}

	[Serializable]
	public class ReportDetailModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public ReportDetailModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string Name { get; set; }
		public string Description { get; set; }
		public Guid ReportGuid { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.Name			= string.Empty;
			this.Description	= string.Empty;
			this.ReportGuid		= Guid.Empty;
		}
		#endregion
	}
}