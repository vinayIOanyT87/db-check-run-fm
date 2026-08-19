namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;

	public class MovementStartDataEditorModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementStartDataEditorModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public Guid MovementPointGuid { get; set; }
		public string TransferStartTime { get; set; }
		public bool ApplyToNodes { get; set; }
		public string PointId { get; set; }
		public string NumberGroupSeparator { get; set; }
		public string NumberDecimalSeparator { get; set; }
		public int[] NumberGroupSizes { get; set; }
		public string ShortDatePattern { get; set; }
		public string TimePattern { get; set; }
		public string TimeZone { get; set; }


		#endregion


		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.MovementPointGuid = Guid.Empty;
			this.TransferStartTime = string.Empty;
			this.ApplyToNodes = false;
			this.PointId = string.Empty;
			this.NumberGroupSeparator = string.Empty;
			this.NumberDecimalSeparator = string.Empty;
			this.NumberGroupSizes = new int[1];
			this.ShortDatePattern = string.Empty;
			this.TimePattern = string.Empty;
			this.TimeZone = string.Empty;
		}
		#endregion


	}
}