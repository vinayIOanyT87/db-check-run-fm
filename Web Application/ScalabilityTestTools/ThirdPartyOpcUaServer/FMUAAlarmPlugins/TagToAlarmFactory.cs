
namespace FMUAAlarmPlugins
{
	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;

    using Softing.Opc.Ua.Sdk;
    using Softing.Opc.Ua.Sdk.Server;



    public class TagToAlarmFactory : IDynamicEntityFactory
    {
        public object Create(ParameterCollection inputParams, NodeState parentNode, ushort namespaceIndex, ServerSystemContext systemContext,
            NodeIdDictionary<NodeState> predefinedNodes)
        {
            return new TagToAlarm(systemContext, parentNode,namespaceIndex);
        }

        public string GetDynamicEntityTypeName()
        {
            return typeof(TagToAlarm).Name;
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


        public ParameterCollection GetDefaultParams()
        {
            var ret = new ParameterCollection();
            return ret;
        }
    }
}
