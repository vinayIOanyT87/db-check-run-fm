

namespace FMUAAlarmPlugins
{
	using System;
	using System.Net.Mime;

	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;

	using Softing.Opc.Ua.Sdk;
	using Softing.Opc.Ua.Sdk.Server;


	public class SystemCommandFactory : IDynamicEntityFactory
	{
		public const string CmdKey = "Cmd";

		public object Create(ParameterCollection inputParams, NodeState parentNode, ushort namespaceIndex, ServerSystemContext systemContext,
			 NodeIdDictionary<NodeState> predefinedNodes)
		{
			Environment.Exit(0);
			return null;
		}

		public string GetDynamicEntityTypeName()
		{
			return "SystemCommand";
		}

		public bool Delete(NodeState node, ServerSystemContext systemContext)
		{
			return false;
		}

		public ParameterCollection GetDefaultParams()
		{
			var ret = new ParameterCollection();
			ret[CmdKey] = "Exit";
			return ret;
		}
	}
}
