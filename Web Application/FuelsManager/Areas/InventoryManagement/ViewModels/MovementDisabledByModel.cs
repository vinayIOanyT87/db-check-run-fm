namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class MovementDisabledByModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementDisabledByModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public Guid MovementPointGuid { get; set; }
		public string MovementPointId { get; set; }
		public List<string> InterlockedActiveMovementList { get; set; }
		#endregion


		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			MovementPointGuid = Guid.Empty;
			MovementPointId = string.Empty;
			InterlockedActiveMovementList = new List<string>();
		}
		#endregion

	}
}