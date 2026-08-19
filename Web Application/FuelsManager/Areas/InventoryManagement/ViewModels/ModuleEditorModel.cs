namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;

	using FMBusinessObjects.DataObjects;
	using System.Web.Mvc;
	using Areas.Controllers;
	using Varec.CommonComponents.EngineeringUnitsLibrary;

	#region Module Editor Tag Model
	[Serializable]
	public class ModuleEditorTagModel : FMBaseModel
	{
		public string TagName { get; set; }
		public string ParameterName { get; set; }
		public string ValueType { get; set; }
		public EngineeringUnitType EngineeringUnitsType { get; set; }
		public PointTemplateTag.PointTagInputOutputType PointTagInputOutputTypeIndex { get; set; }
		public bool Input { get; set; }
	}

	#endregion

	#region Module Editor Settings Model
	[Serializable]
	public class ModuleEditorSettingModel : FMBaseModel
	{
		public string SettingName { get; set; }
		public string PropertyName { get; set; }
		public string ValueType { get; set; }
	}

	#endregion


	public class ModuleEditorModel
	{
		public bool ModuleLibrary { get; set; }

		public Module Module { get; set; }

		public bool ReadOnly;

		public List<KeyValuePair<string, string>> TagDataTypes { get; set; }

		public List<SelectListItem> TagInputOutputTypes { get; set; }

		public List<SelectListItem> OutputTagChangeAgents { get; set; }

		public List<KeyValuePair<string, string>> PropertyDataTypes { get; set; }

		public ModuleEditorModel()
		{

		}

		public ModuleEditorModel(bool moduleLibrary, Module module, List<KeyValuePair<string, string>> tagDataTypes, List<SelectListItem> tagInputOutputTypes, List<SelectListItem> outputTagChangeAgents, List<KeyValuePair<string, string>> propertyDataTypes, bool readOnly)
		{
			this.ModuleLibrary = moduleLibrary;
			this.Module = module;
			this.TagDataTypes = tagDataTypes;
			this.TagInputOutputTypes = tagInputOutputTypes;
			this.OutputTagChangeAgents = outputTagChangeAgents;
			this.PropertyDataTypes = propertyDataTypes;
			this.ReadOnly = readOnly;
		}
	}
}