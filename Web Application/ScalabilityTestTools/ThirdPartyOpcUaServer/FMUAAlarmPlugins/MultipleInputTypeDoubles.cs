

namespace FMUAAlarmPlugins
{
	using System;

	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;

	using Softing.Opc.Ua.Sdk;
	using Softing.Opc.Ua.Sdk.Server;

	public class MultipleInputTypeDoubles : FolderState
	{
		public MultipleInputTypeDoubles(
			 ServerSystemContext context,
			 NodeState parent,
			 ushort namespaceIndex,
			 string name,
			 string nodeId,
			int numDoubles,
			string doublePrefix,
			double intialValue,
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
			var ramp = RampThread.Initialize(context,minRampValue,maxRampValue, rampUpdateRateInSeconds,rampIncrement,rampIncreasing);
			InputTypeDoubleFactory dFactory = new InputTypeDoubleFactory();
			for (int i = 0; i < numDoubles; i++)
			{
				var doubleParams = new ParameterCollection();
				doubleParams[InputTypeDoubleFactory.NameKey] = doublePrefix + i;
				doubleParams[InputTypeDoubleFactory.ValueKey] = intialValue;
				//doubleParams[InputTypeDoubleFactory.NodeIdKey] = new NodeId(Guid.NewGuid().ToString(), namespaceIndex).ToString();
				doubleParams[InputTypeDoubleFactory.NodeIdKey] = new NodeId(name + "_" + doublePrefix + i, namespaceIndex).ToString();
				InputTypeDouble d = (InputTypeDouble)dFactory.Create(doubleParams, this, namespaceIndex, context, predefinedNodes);
				this.AddReference(Softing.Opc.Ua.Sdk.ReferenceTypeIds.Organizes, false, d.NodeId);
				d.AddReference(Softing.Opc.Ua.Sdk.ReferenceTypeIds.Organizes, true, this.NodeId);
				d.AccessLevel = AccessLevels.CurrentReadOrWrite;
				d.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
				d.StatusCode = StatusCodes.Good;
				d.Timestamp = DateTime.UtcNow;
				predefinedNodes.Add(d.NodeId,d);
				ramp.RegisterDouble(d);
			}
		}
	}
}
