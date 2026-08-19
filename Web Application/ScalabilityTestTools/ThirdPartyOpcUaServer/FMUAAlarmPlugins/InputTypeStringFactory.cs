
namespace FMUAAlarmPlugins
{
    using System;

    using FMBusinessObjects.DataObjects;

    using FMUAAlarmPluginInterface;

    using Softing.Opc.Ua.Sdk;
    using Softing.Opc.Ua.Sdk.Server;


    public class InputTypeStringFactory : IDynamicEntityFactory
    {
        public object Create(ParameterCollection inputParams, NodeState parentNode, ushort namespaceIndex, ServerSystemContext systemContext,
            NodeIdDictionary<NodeState> predefinedNodes)
        {
            return new InputTypeString(systemContext, parentNode, namespaceIndex, (string)inputParams[NameKey], (string)inputParams[ValueKey], (string)inputParams[NodeIdKey]);
        }

        public string GetDynamicEntityTypeName()
        {
            return typeof(InputTypeString).Name;
        }

        public bool Delete(NodeState node, ServerSystemContext systemContext)
        {
            var nodeToDelete = node as DataItemState<string>;
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
            ret[ValueKey] = "Default Value";
            ret[NodeIdKey] = new NodeId(Guid.NewGuid().ToString(), 2).ToString();
            return ret;
        }
    }
}
