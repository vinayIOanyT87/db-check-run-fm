
namespace FMUAAlarmPlugins
{
	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;

    using System;
    using Softing.Opc.Ua.Sdk;
    using Softing.Opc.Ua.Sdk.Server;

    public class LowLimitAlarmFactory : IDynamicEntityFactory
    {
        public object Create(ParameterCollection inputParams, NodeState parentNode, ushort namespaceIndex, ServerSystemContext systemContext,
            NodeIdDictionary<NodeState> predefinedNodes)
        {
            string pointGuidString = (string)inputParams[PointGuidKey];
            string pointIdString = (string)inputParams[PointIDKey];
            double lowLimit = (double)inputParams[LowLimitKey];
            double initialValue = (double)inputParams[ValueKey];
            string moduleNameString = (string)inputParams[ModuleNameKey];
            string methodNameString = (string)inputParams[MethodNameKey];
            string name = pointIdString + " " + moduleNameString + " " + methodNameString;
            string alarmName = name + " Alarm";


            return (object)new LowLimitAlarmMonitor(systemContext, parentNode, namespaceIndex, name, alarmName, initialValue, lowLimit);
        }

        public string GetDynamicEntityTypeName()
        {
            return typeof(LowLimitAlarmMonitor).Name;
        }

        public bool Delete(NodeState node, ServerSystemContext systemContext)
        {
            var llm = node as LowLimitAlarmMonitor;
            if (llm != null && llm.Parent != null)
            {
                llm.Delete(systemContext);
                return true;
            }
            return false;
        }

        public const string PointGuidKey = "PointGuid";
        public const string PointIDKey = "PointID";
        public const string ModuleNameKey = "ModuleName";
        public const string ModuleGuidKey = "ModuleGuid";
        public const string ModuleCalculationGuidKey = "ModuleCalculationGuid";
        public const string MethodNameKey = "MethodName";
        public const string AlarmNameKey = "AlarmName";
        public const string LowLimitKey = "lowLimit";
        public const string ValueKey = "value";


        public ParameterCollection GetDefaultParams()
        {
            var ret = new ParameterCollection();
            ret[PointGuidKey] = Guid.NewGuid().ToString();
            ret[PointIDKey] = "PointID";
				ret[MethodNameKey] = "MethodName";
            ret[ModuleNameKey] = "ModuleName";
            ret[ModuleGuidKey] = Guid.NewGuid().ToString();
            ret[ModuleCalculationGuidKey] = Guid.NewGuid().ToString();
            ret[AlarmNameKey] = "AlarmName";
            ret[LowLimitKey] = 200.00;
            ret[ValueKey] = 250.00;
            return ret;
        }
    }
}
