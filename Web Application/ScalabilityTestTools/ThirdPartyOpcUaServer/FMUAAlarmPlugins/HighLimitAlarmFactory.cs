
namespace FMUAAlarmPlugins
{
	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;

    using System;
    using Softing.Opc.Ua.Sdk;
    using Softing.Opc.Ua.Sdk.Server;

    public class HighLimitAlarmFactory : IDynamicEntityFactory
    {
        public const string PointGuidKey = "PointGuid";
        public const string PointIDKey = "PointID";
        public const string ModuleNameKey = "ModuleName";
        public const string ModuleGuidKey = "ModuleGuid";
        public const string ModuleCalculationGuidKey = "ModuleCalculationGuid";
        public const string MethodNameKey = "MethodName";
        public const string HighLimitKey = "highLimit";
        public const string ValueKey = "value";

        public object Create(ParameterCollection inputParams, NodeState parentNode, ushort namespaceIndex, ServerSystemContext systemContext,
            NodeIdDictionary<NodeState> predefinedNodes)
        {
            //string pointGuidString = (string)inputParams[PointGuidKey];
            string pointIdString = (string)inputParams[PointIDKey];
            double highLimit = (double)inputParams[HighLimitKey];
            double initialValue = (double)inputParams[ValueKey];
            string moduleNameString = (string)inputParams[ModuleNameKey];
            string methodNameString = (string)inputParams[MethodNameKey];
            string name = pointIdString + " " + moduleNameString + " " + methodNameString;
            string alarmName = name + " Alarm";

            return (object)new HighLimitMonitor(systemContext, parentNode, namespaceIndex, name, alarmName, initialValue, highLimit);
        }

        public string GetDynamicEntityTypeName()
        {
            return typeof(HighLimitMonitor).Name;
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
            ret[HighLimitKey] = 300.00;
            ret[ValueKey] = 250.00;
            return ret;
        }
    }
}
