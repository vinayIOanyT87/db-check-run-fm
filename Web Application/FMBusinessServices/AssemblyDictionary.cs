namespace FMBusinessServices
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	public sealed class AssemblyDictionary
	{
		private static readonly AssemblyDictionary instance = new AssemblyDictionary();

		// Dictionary of loaded assemblies (defined for memory leaks)
		private Dictionary<string, Assembly> _assemblyDictionary = new Dictionary<string, Assembly>();

		// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
		static AssemblyDictionary()
		{
		}

		private AssemblyDictionary()
		{
		}

		public static AssemblyDictionary Instance
		{
			get
			{
				return instance;
			}
		}

		public static bool ContainsKey(string assemblyName)
		{
			if (!string.IsNullOrWhiteSpace(assemblyName))
				return Instance._assemblyDictionary.ContainsKey(assemblyName.Trim().ToLower());

			return false;
		}

		public static void Add(string assemblyName, Assembly dll)
		{
			if (!string.IsNullOrWhiteSpace(assemblyName))
			{
				lock (Instance)
				{
					if (!Instance._assemblyDictionary.ContainsKey(assemblyName.Trim().ToLower()))
					{
						try
						{
							Instance._assemblyDictionary.Add(assemblyName.Trim().ToLower(), dll);
						}
						catch (Exception) { }
					}
				}
			}
		}

		public static Assembly Get(string assemblyName)
		{
			if (!string.IsNullOrWhiteSpace(assemblyName))
			{
				if (Instance._assemblyDictionary.ContainsKey(assemblyName.Trim().ToLower()))
					return Instance._assemblyDictionary[assemblyName.Trim().ToLower()];
			}
			return null;
		}
	}
}
