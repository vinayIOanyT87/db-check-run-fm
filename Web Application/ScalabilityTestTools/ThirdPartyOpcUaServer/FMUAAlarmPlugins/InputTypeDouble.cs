
namespace FMUAAlarmPlugins
{
    using Softing.Opc.Ua.Sdk;

    public class InputTypeDouble : DataItemState<double>
    {
        public InputTypeDouble(       
            ISystemContext context,
            NodeState parent,
            ushort namespaceIndex,
            string name,
            double initialValue,
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
