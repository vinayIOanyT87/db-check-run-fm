namespace FMBusinessObjects.UtilityObjects
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	using FMBusinessObjects.Attributes;

	public class CachedPropertyDictionary : Dictionary<Type, Dictionary<string, CachedPropertyInfo>>
	{
	}

	public class CachedPropertyInfo
	{
		public PropertyInfo PropertyInfo { get; set; }

		public FMPersistedField FMPersistedField { get; set; }
	}
}
