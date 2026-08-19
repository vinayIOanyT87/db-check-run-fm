
namespace FMUAAlarmPlugins
{
	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;

    using System;
    using Softing.Opc.Ua.Sdk;
    using Softing.Opc.Ua.Sdk.Server;

    public class ExclusiveLimitFactory : IDynamicEntityFactory
    {
        public object Create(ParameterCollection inputParams, NodeState parentNode, ushort namespaceIndex, ServerSystemContext systemContext,
            NodeIdDictionary<NodeState> predefinedNodes)
        {
            //string pointGuidString = (string)inputParams[PointGuidKey];
            string pointIdString = (string)inputParams[PointIDKey];
            string moduleNameString = (string)inputParams[ModuleNameKey];
            string methodNameString = (string)inputParams[MethodNameKey];
            double highLimit = (double)inputParams[HighLimitKey];
            double highHighLimit = (double)inputParams[HighHighLimitKey];
            double lowLimit = (double)inputParams[LowLimitKey];
            double lowLowLimit = (double)inputParams[LowLowLimitKey];
            double initialValue = (double)inputParams[ValueKey];

            string name = pointIdString + " " + moduleNameString + " " + methodNameString;
            return new ExclusiveLimitMonitor(
                systemContext,
                parentNode,
                namespaceIndex,
                name,
                name + " Alarm",
                initialValue,
                highLimit,
                highHighLimit,
                lowLimit,
                lowLowLimit);
        }

        public string GetDynamicEntityTypeName()
        {
            return "ExclusiveLimitAlarm.FMExclusiveLimitAlarm";
        }

        public bool Delete(NodeState node, ServerSystemContext systemContext)
        {
            var elm = node as ExclusiveLimitMonitor;
            if (elm != null && elm.Parent != null)
            {
                elm.Delete(systemContext);
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
        public const string HighLimitKey = "highLimit";
        public const string HighHighLimitKey = "highHighLimit";
        public const string LowLimitKey = "lowLimit";
        public const string LowLowLimitKey = "lowLowLimit";
        public const string ValueKey = "value";


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
            ret[HighHighLimitKey] = 400.00;
            ret[LowLimitKey] = 200.00;
            ret[LowLowLimitKey] = 100.00;
            ret[ValueKey] = 250.00;
            return ret;
        }
    }
}

