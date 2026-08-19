
namespace FMUAAlarmPlugins
{
    using System;

    using FMBusinessObjects.DataObjects;

    using FMUAAlarmPluginInterface;

    using Softing.Opc.Ua.Sdk;
    using Softing.Opc.Ua.Sdk.Server;


    public class FolderFactory : IDynamicEntityFactory
    {
        public const string NameKey = "Name";

        public const string NodeIDKey = "NodeID";

        public object Create(ParameterCollection inputParams, NodeState parentNode, ushort namespaceIndex, ServerSystemContext systemContext,
            NodeIdDictionary<NodeState> predefinedNodes)
        {
            return new Folder(systemContext, parentNode, namespaceIndex, (string)inputParams[NameKey], (string)inputParams[NodeIDKey]);
        }

        public string GetDynamicEntityTypeName()
        {
            return typeof(Folder).Name;
        }

        public bool Delete(NodeState node, ServerSystemContext systemContext)
        {
            var nodeToDelete = node as FolderState;
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
            ret[NameKey] = "Folder Name";
            ret[NodeIDKey] = new NodeId(Guid.NewGuid().ToString(),2).ToString();
            return ret;
        }
    }
}
