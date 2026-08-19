namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
    using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	[Serializable]
	public class PointTemplateDetailModel
	{
		public const string SessionKey = "PointTemplateDetailModel";
		public bool HasFCEERight = false;

      public bool ModifyEnabled = true;

		public PointTemplate Template { get; set; }

        public List<DrawingName> AssociatedDrawings { get; set; }

        public string DefaultDrawingGuidString { get; set; }
	}
}