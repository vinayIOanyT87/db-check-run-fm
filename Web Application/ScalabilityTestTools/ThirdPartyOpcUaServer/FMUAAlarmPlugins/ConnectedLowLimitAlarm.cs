
namespace FMUAAlarmPlugins
{
    using Softing.Opc.Ua.Sdk;

    public class ConnectedLowLimitAlarm : LowLimitAlarmMonitor
    {
        protected ConnectDoubleAndString valueConnector;

        protected ConnectDoubleAndString lowLimitConnector;

        public ConnectedLowLimitAlarm(
            ISystemContext context,
            NodeState parent,
            ushort namespaceIndex,
            string name,
            string alarmName,
            double initialValue,
            double lowLimit,
            string valueTagNodeId,
            string lowLimitTagNodeId,
            NodeIdDictionary<NodeState> predefinedNodes)
            : base(context, parent, namespaceIndex, name, alarmName, initialValue, lowLimit)
        {
            valueConnector = new ConnectDoubleAndString(context, this, namespaceIndex, valueTagNodeId, predefinedNodes);
            this.AddChild(valueConnector);
            lowLimitConnector = new ConnectDoubleAndString(context, this.GetLowLimitNode(), namespaceIndex, lowLimitTagNodeId, predefinedNodes);
            this.GetLowLimitNode().AddChild(lowLimitConnector);
        }
    }
}