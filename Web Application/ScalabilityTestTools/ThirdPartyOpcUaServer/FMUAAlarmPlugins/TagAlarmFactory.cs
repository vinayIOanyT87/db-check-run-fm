
namespace FMUAAlarmPlugins
{
	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;

    using System;
    using Softing.Opc.Ua.Sdk;
    using Softing.Opc.Ua.Sdk.Server;

    public class TagAlarmFactory : IDynamicEntityFactory
    {
        public const string PointGuidKey = "PointGuid";
        public const string PointIDKey = "PointID";
        public const string ModuleNameKey = "ModuleName";
        public const string ModuleGuidKey = "ModuleGuid";
        public const string ModuleCalculationGuidKey = "ModuleCalculationGuid";
        public const string MethodNameKey = "MethodName";
        public const string ValueKey = "value";

        public object Create(ParameterCollection inputParams, NodeState parentNode, ushort namespaceIndex, ServerSystemContext systemContext,
            NodeIdDictionary<NodeState> predefinedNodes)
        {
            //string pointGuidString = (string)inputParams[PointGuidKey];
            string pointIdString = (string)inputParams[PointIDKey];
            string initialValue = (string)inputParams[ValueKey];
            string moduleNameString = (string)inputParams[ModuleNameKey];
            string methodNameString = (string)inputParams[MethodNameKey];
            string name = pointIdString + " " + moduleNameString + " " + methodNameString;
            string alarmName = name + " Alarm";

            return (object)new TagAlarmMonitor(systemContext, parentNode, namespaceIndex, name, alarmName, initialValue);
        }

        public string GetDynamicEntityTypeName()
        {
            return typeof(TagAlarmMonitor).Name;
        }

        public bool Delete(NodeState node, ServerSystemContext systemContext)
        {
            var hlm = node as TagAlarmMonitor;
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
            ret[ValueKey] = "Normal";
            return ret;
        }
    }
}
