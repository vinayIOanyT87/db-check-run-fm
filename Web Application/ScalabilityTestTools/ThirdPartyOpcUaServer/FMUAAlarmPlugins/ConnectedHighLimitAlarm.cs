
namespace FMUAAlarmPlugins
{
    using Softing.Opc.Ua.Sdk;

    public class ConnectedHighLimitAlarm : HighLimitMonitor
    {
        protected ConnectDoubleAndString valueConnector;

        protected ConnectDoubleAndString highLimitConnector;

        public ConnectedHighLimitAlarm(
            ISystemContext context,
            NodeState parent,
            ushort namespaceIndex,
            string name,
            string alarmName,
            double initialValue,
            double highLimit,
            string valueTagNodeId,
            string highLimitTagNodeId,
            NodeIdDictionary<NodeState> predefinedNodes)
            :base(context,parent,namespaceIndex,name,alarmName,initialValue,highLimit)
        {
            valueConnector = new ConnectDoubleAndString(context,this,namespaceIndex,valueTagNodeId,predefinedNodes);
            this.AddChild(valueConnector);
            highLimitConnector = new ConnectDoubleAndString(context,this.GetHighLimitNode(),namespaceIndex,highLimitTagNodeId,predefinedNodes);
            this.GetHighLimitNode().AddChild(highLimitConnector);
        }
    }
}
