namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;

	[Serializable]
	public class PointTemplatesFilterContext
	{
		public const string SessionKey = "PointTemplatesFilterContext";

		public PointTemplatesModel Model { get; set; }

		public PointTemplatesFilterContext()
		{
		}

		public PointTemplatesFilterContext( PointTemplatesModel model )
		{
			this.Model = model;
		}
	}
}