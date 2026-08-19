
namespace FMUAAlarmPlugins
{
	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;

    using Softing.Opc.Ua.Sdk;
    using Softing.Opc.Ua.Sdk.Server;



    public class ConnectDoubleFactory : IDynamicEntityFactory
    {
        public object Create(ParameterCollection inputParams, NodeState parentNode, ushort namespaceIndex, ServerSystemContext systemContext,
            NodeIdDictionary<NodeState> predefinedNodes)
        {
            return new ConnectDoubles(systemContext, parentNode, namespaceIndex, (string)inputParams[ToNodeIdKey], predefinedNodes);
        }

        public string GetDynamicEntityTypeName()
        {
            return typeof(ConnectDoubles).Name;
        }

        public bool Delete(NodeState node, ServerSystemContext systemContext)
        {
            var nodeToDelete = node as BaseObjectState;
            if (nodeToDelete != null && nodeToDelete.Parent != null)
            {
                nodeToDelete.Delete(systemContext);
                return true;
            }
            return false;
        }

        public const string ToNodeIdKey = "ToNodeId";

        public ParameterCollection GetDefaultParams()
        {
            var ret = new ParameterCollection();
            ret[ToNodeIdKey] = "ns=2;i=3";
            return ret;
        }
    }
}
