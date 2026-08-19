namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class ModuleInputOutputCollection : List<ModuleInputOutput>
	{
	}

	[Serializable]
	public enum ModuleInputOutputType
	{
		Input, Output, Property, InOut
	}

	[Serializable]
	public class ModuleInputOutput
	{
		public ModuleInputOutputType ParameterType { get; set; }

		public string ID { set; get; }

		public Type Type { get; set; }

		public bool ReadOnly { get; set; }

		public bool Required { get; set; }

		public string ValuesMethodName { get; set; }

		public ModuleInputOutput()
		{
			this.Required = true;
			this.ReadOnly = false;
		}
	}
}
