
namespace FMUAAlarmPlugins
{
	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;

    using Softing.Opc.Ua.Sdk;
    using Softing.Opc.Ua.Sdk.Server;

    public class HalfOfParentDoubleFactory : IDynamicEntityFactory
    {
        public const string NameKey = "Name";

        public object Create(ParameterCollection inputParams, NodeState parentNode, ushort namespaceIndex, ServerSystemContext systemContext,
            NodeIdDictionary<NodeState> predefinedNodes)
        {
            return new HalfOfParentDouble(systemContext, parentNode, namespaceIndex, (string)inputParams[NameKey]);
        }

        public string GetDynamicEntityTypeName()
        {
            return typeof(HalfOfParentDouble).Name;
        }

        public bool Delete(NodeState node, ServerSystemContext systemContext)
        {
            var nodeToDelete = node as DataItemState<double>;
            if (nodeToDelete != null && nodeToDelete.Parent != null)
            {
                nodeToDelete.Delete(systemContext);
                return true;
            }
            return false;
        }

        public ParameterCollection GetDefaultParams()
        {
            var ret = new ParameterCollection();
            ret[NameKey] = "Default Name";
            return ret;
        }
    }
}
