namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Globalization;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	[Serializable]
	public class MovementHistoryMovementDataEditorModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementHistoryMovementDataEditorModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public Guid MovementHistoryGuid { get; set; }
		public Guid MovementPointGuid { get; set; }
		public Guid RootParentGuid { get; set; }
		public Guid ParentGuid { get; set; }
		public string PointId { get; set; }
		public string NodeId { get; set; }
		public NumberFormatInfo NumberFormatInfo { get; set; }
		public string NumberGroupSeparator { get; set; }
		public string NumberDecimalSeparator { get; set; }
		public int[] NumberGroupSizes { get; set; }
		public string ShortDatePattern { get; set; }
		public string TimePattern { get; set; }
		public string TimeZone { get; set; }

		public string StartDateTimeStr { get; set; }
		public string CloseoutDateTimeStr { get; set; }

		public bool HasModifyRights { get; set; }
		#endregion


		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.MovementPointGuid		= Guid.Empty;
			this.MovementHistoryGuid	= Guid.Empty;
			this.RootParentGuid			= Guid.Empty;
			this.ParentGuid				= Guid.Empty;
			this.PointId				= string.Empty;
			this.NodeId					= string.Empty;
			this.NumberGroupSeparator	= string.Empty;
			this.NumberDecimalSeparator = string.Empty;
			this.NumberGroupSizes		= new int[1];
			this.ShortDatePattern		= string.Empty;
			this.TimePattern			= string.Empty;
			this.TimeZone				= string.Empty;
			this.CloseoutDateTimeStr	= string.Empty;
			this.StartDateTimeStr		= string.Empty;
			this.HasModifyRights		= false;
			#endregion
		}
	}
}