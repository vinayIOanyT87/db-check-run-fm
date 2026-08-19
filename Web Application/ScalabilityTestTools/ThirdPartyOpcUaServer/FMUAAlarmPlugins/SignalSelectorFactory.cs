
namespace FMUAAlarmPlugins
{
	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;
    using Softing.Opc.Ua.Sdk;
    using Softing.Opc.Ua.Sdk.Server;


    public class SignalSelectorFactory : IDynamicEntityFactory
    {
        public object Create(ParameterCollection inputParams, NodeState parentNode, ushort namespaceIndex, ServerSystemContext systemContext,
            NodeIdDictionary<NodeState> predefinedNodes)
        {
            return new SignalSelector(systemContext, parentNode, namespaceIndex, (string)inputParams[NameKey], (string)inputParams[Signal1NodeIdKey], (string)inputParams[Signal2NodeIdKey], (string)inputParams[Signal3NodeIdKey], (string)inputParams[Signal4NodeIdKey], (string)inputParams[LowSignalNodeIdKey], (string)inputParams[HighSignalNodeIdKey], predefinedNodes);
        }

        public string GetDynamicEntityTypeName()
        {
            return typeof(SignalSelector).Name;
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

        public const string NameKey = "Name";

        public const string Signal1NodeIdKey = "Signal1NodeId";

        public const string Signal2NodeIdKey = "Signal2NodeId";

        public const string Signal3NodeIdKey = "Signal3NodeId";

        public const string Signal4NodeIdKey = "Signal4NodeId";

        public const string LowSignalNodeIdKey = "LowSignalNodeId";

        public const string HighSignalNodeIdKey = "HighSignalNodeId";


        public ParameterCollection GetDefaultParams()
        {
            var ret = new ParameterCollection();
            ret[NameKey] = "Default Name";
            ret[Signal1NodeIdKey] = "ns=2;i=3";
            ret[Signal2NodeIdKey] = "ns=2;i=3";
            ret[Signal3NodeIdKey] = "ns=2;i=3";
            ret[Signal4NodeIdKey] = "ns=2;i=3";
            ret[LowSignalNodeIdKey] = "ns=2;i=3";
            ret[HighSignalNodeIdKey] = "ns=2;i=3";
            return ret;
        }
    }
}
