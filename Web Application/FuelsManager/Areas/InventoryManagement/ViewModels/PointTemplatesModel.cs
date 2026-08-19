namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.Controllers;

	[Serializable]
	public class PointTemplatesModel: FMBaseModel
	{
		public PointTemplateCollection Templates { get; set; }

		public bool DeleteEnabled = true;

		public SecurityClass Security;
		public bool HasFCEERight = false;

      public List<KeyValuePair<string, string>> PointTypeList { get; set; }

		public PointTemplatesModel()
		{
			this.Templates = new PointTemplateCollection();
			this.PointTypeList = new List<KeyValuePair<string, string>>();
		}

		public PointTemplatesModel(PointTemplatesFilterContext context, SecurityClass security)
		{
			if (context != null && context.Model != null)
			{
				this.Templates = context.Model.Templates;
				this.DeleteEnabled = context.Model.DeleteEnabled;
			}
			this.PointTypeList = new List<KeyValuePair<string, string>>();
			this.Security = security;
		}
	}
}