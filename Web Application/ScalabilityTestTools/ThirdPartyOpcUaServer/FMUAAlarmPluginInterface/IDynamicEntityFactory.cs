
namespace FMUAAlarmPluginInterface
{
	using FMBusinessObjects.DataObjects;

	using Softing.Opc.Ua.Sdk;
    using Softing.Opc.Ua.Sdk.Server;

    public interface IDynamicEntityFactory
    {
        object Create(ParameterCollection inputParams,
            NodeState parentNode,
            ushort namespaceIndex,
            ServerSystemContext systemContext,
            NodeIdDictionary<NodeState> predefinedNodes);

        string GetDynamicEntityTypeName();

        bool Delete(NodeState node,
            ServerSystemContext systemContext);

        ParameterCollection GetDefaultParams();
    }
}
