
namespace FMUAAlarmPlugins
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;

	using Softing.Opc.Ua.Sdk;
	using Softing.Opc.Ua.Sdk.Server;


	public class MultipleInputTypeDoublesFactory : IDynamicEntityFactory
	{
		public object Create(ParameterCollection inputParams, NodeState parentNode, ushort namespaceIndex, ServerSystemContext systemContext,
			 NodeIdDictionary<NodeState> predefinedNodes)
		{
			return new MultipleInputTypeDoubles(systemContext, parentNode, namespaceIndex, (string)inputParams[NameKey], (string)inputParams[NodeIdKey],
				(int)inputParams[NumDoublesKey], (string)inputParams[DoublePrefixKey], (double)inputParams[InitialValueKey], (double)inputParams[MinRampValueKey], (double)inputParams[MaxRampValueKey], (int)inputParams[RampUpdateRateInSecondsKey], (double)inputParams[RampIncrementKey], (bool)inputParams[RampIncreasingKey], predefinedNodes);
		}

		public string GetDynamicEntityTypeName()
		{
			return typeof(MultipleInputTypeDoubles).Name;
		}

		public bool Delete(NodeState node, ServerSystemContext systemContext)
		{
			var nodeToDelete = node as FolderState;
			if (nodeToDelete != null && nodeToDelete.Parent != null)
			{
				List<BaseInstanceState> children = new List<BaseInstanceState>();
				nodeToDelete.GetChildren(systemContext, children);
				foreach (var child in children)
				{
					if (child != null)
					{
						//Remove references
						child.Delete(systemContext);
					}
				}
				nodeToDelete.Delete(systemContext);
				return true;
			}
			return false;
		}

		public const string NameKey = "Name";
		public const string InitialValueKey = "InitialValue";
		public const string NodeIdKey = "NodeID";
		public const string NumDoublesKey = "NumDoubles";
		public const string DoublePrefixKey = "DoublePrefix";
		public const string MaxRampValueKey = "MaxRampValue";
		public const string MinRampValueKey = "MinRampValue";
		public const string RampUpdateRateInSecondsKey = "RampUpdateRateInSeconds";
		public const string RampIncrementKey = "RampIncrement";
		public const string RampIncreasingKey = "RampIncreasing";



		public ParameterCollection GetDefaultParams()
		{
			var ret = new ParameterCollection();
			ret[NameKey] = "Default Name";
			ret[InitialValueKey] = 11.55;
			ret[NodeIdKey] = new NodeId(Guid.NewGuid().ToString(), 2).ToString();
			ret[NumDoublesKey] = (int)10;
			ret[DoublePrefixKey] = "D";
			ret[MaxRampValueKey] = 449.00;
			ret[MinRampValueKey] = 51.00;
			ret[RampUpdateRateInSecondsKey] = 1;
			ret[RampIncrementKey] = 1.00;
			ret[RampIncreasingKey] = true;
			return ret;
		}
	}
}
