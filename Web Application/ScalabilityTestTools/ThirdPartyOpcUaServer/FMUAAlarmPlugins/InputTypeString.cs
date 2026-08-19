
namespace FMUAAlarmPlugins
{
    using Softing.Opc.Ua.Sdk;

    public class InputTypeString : DataItemState<string>
    {
        public InputTypeString(
            ISystemContext context,
            NodeState parent,
            ushort namespaceIndex,
            string name,
            string initialValue,
            string nodeId
            )
            : base(parent)
        {

            this.Create(
                 context,
                 new NodeId(nodeId),
                 new QualifiedName(name, namespaceIndex),
                 name,
                 false);
            this.Value = initialValue;
        }

    }
}
