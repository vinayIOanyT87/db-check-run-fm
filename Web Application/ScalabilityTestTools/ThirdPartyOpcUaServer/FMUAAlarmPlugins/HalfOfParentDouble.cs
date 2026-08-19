
namespace FMUAAlarmPlugins
{
    using Softing.Opc.Ua.Sdk;

    public class HalfOfParentDouble : DataItemState<double>
    {
        public HalfOfParentDouble(
            ISystemContext context,
            NodeState parent,
            ushort namespaceIndex,
            string name
            )
            : base(parent)
        {
            this.Create(
                context,
                null,
                new QualifiedName(name, namespaceIndex),
                null,
                true);
            var parentDouble = ((DataItemState<double>)parent);
            this.Value = parentDouble.Value/2.00;
            parentDouble.OnSimpleWriteValue += this.NodeValueSimpleEventHandler;
        }
                
        public ServiceResult NodeValueSimpleEventHandler(ISystemContext context, NodeState node, ref object value)
        {
            var dVal = (double)value;
            this.Value = dVal/2.00;
            this.ClearChangeMasks(context,false);
            return ServiceResult.Good;
        }
    }
}

