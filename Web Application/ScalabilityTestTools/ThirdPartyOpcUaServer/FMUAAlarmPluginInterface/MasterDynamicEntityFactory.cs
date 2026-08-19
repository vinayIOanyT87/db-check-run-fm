
namespace FMUAAlarmPluginInterface
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;

	using FMBusinessObjects.DataObjects;

	using Softing.Opc.Ua.Sdk;
	using Softing.Opc.Ua.Sdk.Server;

	public class MasterDynaicEntityFactory
	{
		#region Dictionary defines for memory leaks
		static Dictionary<string, Assembly> AssemblyDictionary = new Dictionary<string, Assembly>();
		#endregion

		protected string dllDir;

		protected Dictionary<string, IDynamicEntityFactory> FactoryDictionary = new Dictionary<string, IDynamicEntityFactory>();

		protected void HandleRegistration(IDynamicEntityFactory factory)
		{
			try
			{
				FactoryDictionary.Add(factory.GetDynamicEntityTypeName(), factory);
			}
			catch (Exception e)
			{

				Console.WriteLine("Error in GetRegistration: " + e.Message);
			}
		}

		protected void GetRegistration(Assembly dynEnt, Type t)
		{
			try
			{
				var moduleInstance = dynEnt.CreateInstance(t.FullName);
				if (moduleInstance == null)
				{
					throw new Exception("CreateInstance returned null");
				}
				this.HandleRegistration((IDynamicEntityFactory)moduleInstance);
			}
			catch (Exception e)
			{

				Console.WriteLine("Error in GetRegistration: " + e.Message);
			}
		}

		protected bool GetIDynamicEntityFactory(Assembly dynEnt, Type t, Type iface)
		{
			try
			{
				if (iface.Equals(typeof(IDynamicEntityFactory)))
				{
					this.GetRegistration(dynEnt, t);
					return true;
				}
				return false;

			}
			catch (Exception e)
			{

				Console.WriteLine("Error in GetIDynamicEntityFactory: " + e.Message);
				return false;
			}
		}

		protected void GetInterfaces(Assembly dynEnt, Type t)
		{
			try
			{
				foreach (Type iface in t.GetInterfaces())
				{
					if (GetIDynamicEntityFactory(dynEnt, t, iface))
					{
						break;
					}
				}
			}
			catch (Exception e)
			{

				Console.WriteLine("Error in GetInterfaces: " + e.Message);
			}
		}

		protected void GetTypes(Assembly dynEnt)
		{
			if (dynEnt.ManifestModule.Name != "FMBusinessObjects.dll")
			{
				try
				{
					var tArr = dynEnt.GetTypes();
					foreach (var t in tArr)
					{
						this.GetInterfaces(dynEnt, t);
					}
				}
				catch (Exception e)
				{

					Console.WriteLine("Error in GetTypes: " + e.Message);
				}
			}
		}

		protected void GetAssembly(string dll)
		{
			Assembly dynEnt = null;
			if (!AssemblyDictionary.ContainsKey(dll.ToLower()))
			{
				try
				{
					dynEnt = Assembly.LoadFile(dll);
				}
				catch (Exception e)
				{
					Console.WriteLine("Error in GetAssembly: " + e.Message);
				}
				if (dynEnt != null)
				{
					AssemblyDictionary.Add(dll.ToLower(), dynEnt);
				}
			}
			else
			{
				dynEnt = AssemblyDictionary[dll.ToLower()];
			}
			if (dynEnt != null)
			{
				this.GetTypes(dynEnt);
			}
		}

		protected bool PopulateFactory = true;

		public MasterDynaicEntityFactory(string dllPath, bool populateFactory = true)
		{

			dllDir = dllPath;
			if (!dllDir.EndsWith("\\"))
			{
				dllDir += "\\";
			}
			PopulateFactory = populateFactory;
			if (populateFactory)
			{
				this.GetDlls();
			}
		}

		public object Create(string typeName, ParameterCollection inputParams,
			 NodeState parentNode,
			 ushort namespaceIndex,
			 ServerSystemContext systemContext,
			 NodeIdDictionary<NodeState> predefinedNodes)
		{
			try
			{
				if (FactoryDictionary.ContainsKey(typeName))
				{
					return FactoryDictionary[typeName].Create(
						 inputParams,
						 parentNode,
						 namespaceIndex,
						 systemContext,
						 predefinedNodes);
				}
				else
				{
					if (PopulateFactory)
					{
						this.GetDlls();
						if (FactoryDictionary.ContainsKey(typeName))
						{
							return FactoryDictionary[typeName].Create(
								 inputParams,
								 parentNode,
								 namespaceIndex,
								 systemContext,
								 predefinedNodes);
						}
					}
				}
			}
			// ReSharper disable once EmptyGeneralCatchClause
			catch (Exception e)
			{
				System.Console.WriteLine(e.Message);

			}
			return null;
		}

		public ParameterCollection GetDefaultParameters(string typeName)
		{
			if (FactoryDictionary.ContainsKey(typeName))
			{
				return FactoryDictionary[typeName].GetDefaultParams();
			}
			else
			{
				if (PopulateFactory)
				{
					this.GetDlls();
					if (FactoryDictionary.ContainsKey(typeName))
					{
						return FactoryDictionary[typeName].GetDefaultParams();
					}
				}
			}
			throw new Exception("Dynamic Entity Type Not Found!");
		}

		public bool Delete(NodeState node, ServerSystemContext systemContext)
		{
			foreach (var typeDynamicEntity in FactoryDictionary.Values)
			{
				if (typeDynamicEntity.Delete(node, systemContext))
				{
					return true;
				}
			}
			return false;
		}

		public bool HasType(string typeName)
		{
			return FactoryDictionary.ContainsKey(typeName);
		}

		public List<string> GetFactoryTypes()
		{
			var ret = new List<string>();
			foreach (var key in FactoryDictionary.Keys)
			{
				ret.Add(key);
			}
			return ret;
		}

		public void GetDlls()
		{
			try
			{
				string[] dlls = Directory.GetFiles(dllDir, "*.dll");
				string excludeDll = dllDir + "OpcUAExtensibleInterface.dll";
				excludeDll = excludeDll.ToUpper();
				foreach (var dll in dlls)
				{
					if (dll != excludeDll)
					{

					}

					this.GetAssembly(dll);
				}
			}
			catch (Exception e)
			{

				Console.WriteLine("Error in GetDlls: " + e.Message);
			}
		}
	}
}
