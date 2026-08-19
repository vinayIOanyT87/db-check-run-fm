namespace FuelsManager.Areas.FCEE.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;

	using FMBusinessObjects.DataObjects;
	using System.Web.Mvc;
	using Areas.Controllers;
	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public class FCEEMappingEditorModel
	{

		public FCEEMapping FCEEMapping { get; set; }

		public bool ReadOnly;

		public FCEEMappingEditorModel()
		{

		}

		public FCEEMappingEditorModel(FCEEMapping FCEEMapping, bool readOnly)
		{
			this.FCEEMapping = FCEEMapping;
			this.ReadOnly = readOnly;
		}
	}
}