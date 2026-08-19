using System;

namespace FMUAAlarmPlugins
{
	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;

    using System;
    using Softing.Opc.Ua.Sdk;
    using Softing.Opc.Ua.Sdk.Server;

    public class ConnectedLowLimitAlarmFactory : IDynamicEntityFactory
    {
        public const string PointGuidKey = "PointGuid";
        public const string PointIDKey = "PointID";
        public const string ModuleNameKey = "ModuleName";
        public const string ModuleGuidKey = "ModuleGuid";
        public const string ModuleCalculationGuidKey = "ModuleCalculationGuid";
        public const string MethodNameKey = "MethodName";
        public const string LowLimitKey = "lowLimit";
        public const string ValueKey = "value";
        public const string NodeIDSuffix = "NodeID";
        public const string LowLimitNodeIDKey = LowLimitKey + NodeIDSuffix;
        public const string ValueNodeIDKey = ValueKey + NodeIDSuffix;


        public object Create(ParameterCollection inputParams, NodeState parentNode, ushort namespaceIndex, ServerSystemContext systemContext,
            NodeIdDictionary<NodeState> predefinedNodes)
        {
            //string pointGuidString = (string)inputParams[PointGuidKey];
            string pointIdString = (string)inputParams[PointIDKey];
            double lowLimit = inputParams.HasParameter(LowLimitKey) ? (double)inputParams[LowLimitKey] : 0.00;
            double initialValue = inputParams.HasParameter(ValueKey) ? (double)inputParams[ValueKey] : 0.00;
            string moduleNameString = (string)inputParams[ModuleNameKey];
            string methodNameString = (string)inputParams[MethodNameKey];
            string name = pointIdString + " " + moduleNameString + " " + methodNameString;
            string alarmName = name + " Alarm";
            string lowLimitNodeId = (string)inputParams[LowLimitNodeIDKey];
            string valueNodeId = (string)inputParams[ValueNodeIDKey];

            return (object)new ConnectedLowLimitAlarm(systemContext, parentNode, namespaceIndex, name, alarmName, initialValue, lowLimit, valueNodeId, lowLimitNodeId, predefinedNodes);
        }

        public string GetDynamicEntityTypeName()
        {
            return "LowLimitAlarm.FMLowLimitAlarm.GetLowAlarmState";
        }

        public bool Delete(NodeState node, ServerSystemContext systemContext)
        {
            var hlm = node as HighLimitMonitor;
            if (hlm != null && hlm.Parent != null)
            {
                hlm.Delete(systemContext);
                return true;
            }
            return false;
        }

        public ParameterCollection GetDefaultParams()
        {
            var ret = new ParameterCollection();
            ret[PointGuidKey] = Guid.NewGuid().ToString();
            ret[PointIDKey] = "PointID";
            ret[ModuleNameKey] = "ModuleName";
            ret[ModuleGuidKey] = Guid.NewGuid().ToString();
            ret[ModuleCalculationGuidKey] = Guid.NewGuid().ToString();
            ret[MethodNameKey] = "MethodName";
            ret[LowLimitKey] = 300.00;
            ret[LowLimitNodeIDKey] = "ns=2;i=3";
            ret[ValueKey] = 250.00;
            ret[ValueNodeIDKey] = "ns=2;i=3";
            return ret;
        }
    }
}


