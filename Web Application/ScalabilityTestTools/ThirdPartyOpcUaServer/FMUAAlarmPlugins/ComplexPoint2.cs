

namespace FMUAAlarmPlugins
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    using FMBusinessObjects.DataObjects;

    using FMUAAlarmPluginInterface;

    using Softing.Opc.Ua.Sdk;
    using Softing.Opc.Ua.Sdk.Server;

    public class ComplexPoint2 : FolderState
    {
        public ComplexPoint2(
             ServerSystemContext context,
             NodeState parent,
             ushort namespaceIndex,
             string name,
             string nodeId,
            ParameterCollection tags,
            double minRampValue,
            double maxRampValue,
            int rampUpdateRateInSeconds,
				double rampIncrement,
				bool rampIncreasing,
            NodeIdDictionary<NodeState> predefinedNodes
             )
            : base(parent)
        {
            this.Create(
                 context,
                 new NodeId(nodeId),
                 new QualifiedName(name, namespaceIndex),
                 new LocalizedText(name),
                 false);
            TypeDefinitionId = Softing.Opc.Ua.Sdk.ObjectTypeIds.FolderType;
            EventNotifier = EventNotifiers.SubscribeToEvents;
            parent.AddChild(this);
            parent.AddReference(Softing.Opc.Ua.Sdk.ReferenceTypeIds.Organizes, false, this.NodeId);
            this.AddReference(Softing.Opc.Ua.Sdk.ReferenceTypeIds.Organizes, true, parent.NodeId);
            var ramp = RampThread.Initialize(context, minRampValue, maxRampValue, rampUpdateRateInSeconds, rampIncrement, rampIncreasing);
            InputTypeDoubleFactory dFactory = new InputTypeDoubleFactory();
            InputTypeStringFactory sFactory = new InputTypeStringFactory();
            InputTypeFloatFactory fFactory = new InputTypeFloatFactory();
            foreach (var param in tags.Collection)
            {
                string paramType = (string)tags[param.ParameterName];
                switch (paramType)
                {
                    case "System.Float":
                        var floatParams = new ParameterCollection();
                        floatParams[InputTypeFloatFactory.NameKey] = param.ParameterName;
                        floatParams[InputTypeFloatFactory.NodeIdKey] = param.ParameterName; 
                        floatParams[InputTypeFloatFactory.ValueKey] = 0.0f;
                        InputTypeFloat f = (InputTypeFloat)fFactory.Create(floatParams, this, namespaceIndex, context, predefinedNodes);
                        this.AddReference(Softing.Opc.Ua.Sdk.ReferenceTypeIds.Organizes, false, f.NodeId);
                        f.AddReference(Softing.Opc.Ua.Sdk.ReferenceTypeIds.Organizes, true, this.NodeId);
                        f.AccessLevel = AccessLevels.CurrentReadOrWrite;
                        f.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                        f.StatusCode = StatusCodes.Good;
                        f.Timestamp = DateTime.UtcNow;
                        predefinedNodes.Add(f.NodeId, f);
                        if (param.ParameterName != "Command Output")
                        {
                            ramp.RegisterFloat(f);
                        }
                        break;
                    case "System.Double":
                        var doubleParams = new ParameterCollection();
                        doubleParams[InputTypeDoubleFactory.NameKey] = param.ParameterName;
                        doubleParams[InputTypeDoubleFactory.NodeIdKey] = param.ParameterName;
                        doubleParams[InputTypeDoubleFactory.ValueKey] = 0.00;
                        InputTypeDouble d = (InputTypeDouble)dFactory.Create(doubleParams, this, namespaceIndex, context, predefinedNodes);
                        this.AddReference(Softing.Opc.Ua.Sdk.ReferenceTypeIds.Organizes, false, d.NodeId);
                        d.AddReference(Softing.Opc.Ua.Sdk.ReferenceTypeIds.Organizes, true, this.NodeId);
                        d.AccessLevel = AccessLevels.CurrentReadOrWrite;
                        d.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                        d.StatusCode = StatusCodes.Good;
                        d.Timestamp = DateTime.UtcNow;
                        predefinedNodes.Add(d.NodeId, d);
                        if (param.ParameterName != "Command Output")
                        {
                            ramp.RegisterDouble(d);
                        }
                        break;
                    case "System.String":
                        var stringParams = new ParameterCollection();
                        stringParams[InputTypeStringFactory.NameKey] = param.ParameterName;
                        stringParams[InputTypeStringFactory.NodeIdKey] = param.ParameterName;
                        stringParams[InputTypeStringFactory.ValueKey] = string.Empty;
                        InputTypeString s = (InputTypeString)sFactory.Create(stringParams, this, namespaceIndex, context, predefinedNodes);
                        this.AddReference(Softing.Opc.Ua.Sdk.ReferenceTypeIds.Organizes, false, s.NodeId);
                        s.AddReference(Softing.Opc.Ua.Sdk.ReferenceTypeIds.Organizes, true, this.NodeId);
                        s.AccessLevel = AccessLevels.CurrentReadOrWrite;
                        s.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                        s.StatusCode = StatusCodes.Good;
                        s.Timestamp = DateTime.UtcNow;
                        predefinedNodes.Add(s.NodeId, s);
                        if (param.ParameterName != "Command Output")
                        {
                            ramp.RegisterString(s);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
