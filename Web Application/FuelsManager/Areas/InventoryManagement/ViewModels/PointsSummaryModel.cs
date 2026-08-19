namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System.Collections.Generic;
	using FMBusinessObjects.DataObjects;

	using Areas.Controllers;

	public class PointsSummaryModel : FMBaseModel
	{
		public const string SessionKey = "PointsSummaryContext";

		#region Constructors
		/// <summary>
		/// This method is the default constructor
		/// </summary>
		public PointsSummaryModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public List<Point> Points { get; set; }
		public bool DeleteEnabled { get; set; }
		public List<KeyValuePair<string, string>> PointTypeslist { get; set; }
		public List<KeyValuePair<string, string>> PointCategoriesList { get; set; }

		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.DeleteEnabled	= true;
			this.Points			= new List<Point>();
		}
		#endregion
	}
}