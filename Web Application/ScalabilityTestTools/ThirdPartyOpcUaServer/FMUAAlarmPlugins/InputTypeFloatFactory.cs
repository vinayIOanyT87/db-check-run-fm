
namespace FMUAAlarmPlugins
{
	using System;

	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;

	using Softing.Opc.Ua.Sdk;
	using Softing.Opc.Ua.Sdk.Server;


	public class InputTypeFloatFactory : IDynamicEntityFactory
	{
		public object Create(ParameterCollection inputParams, NodeState parentNode, ushort namespaceIndex, ServerSystemContext systemContext,
			 NodeIdDictionary<NodeState> predefinedNodes)
		{
			return new InputTypeFloat(systemContext, parentNode, namespaceIndex, (string)inputParams[NameKey], (float)inputParams[ValueKey], (string)inputParams[NodeIdKey]);
		}

		public string GetDynamicEntityTypeName()
		{
			return typeof(InputTypeFloat).Name;
		}

		public bool Delete(NodeState node, ServerSystemContext systemContext)
		{
			var nodeToDelete = node as DataItemState<float>;
			if (nodeToDelete != null && nodeToDelete.Parent != null)
			{
				nodeToDelete.Delete(systemContext);
				return true;
			}
			return false;
		}

		public const string NameKey = "Name";
		public const string ValueKey = "Value";
		public const string NodeIdKey = "NodeID";


		public ParameterCollection GetDefaultParams()
		{
			var ret = new ParameterCollection();
			ret[NameKey] = "Default Name";
			float val = (float)11.5;
			ret[ValueKey] = val;
			ret[NodeIdKey] = new NodeId(Guid.NewGuid().ToString(), 2).ToString();
			return ret;
		}
	}
}
