
namespace FMUAAlarmPlugins
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	using FMBusinessObjects.DataObjects;

	using FMUAAlarmPluginInterface;

	using Softing.Opc.Ua.Sdk;
	using Softing.Opc.Ua.Sdk.Server;


	public class ComplexPointFactory : IDynamicEntityFactory
	{
		public object Create(ParameterCollection inputParams, NodeState parentNode, ushort namespaceIndex, ServerSystemContext systemContext,
			 NodeIdDictionary<NodeState> predefinedNodes)
		{
			return new ComplexPoint(systemContext, parentNode, namespaceIndex, (string)inputParams[NameKey], (string)inputParams[NodeIdKey],
				(ParameterCollection)inputParams[TagsKey], (double)inputParams[MinRampValueKey], (double)inputParams[MaxRampValueKey], (int)inputParams[RampUpdateRateInSecondsKey], (double)inputParams[RampIncrementKey], (bool)inputParams[RampIncreasingKey], predefinedNodes);
		}

		public string GetDynamicEntityTypeName()
		{
			return typeof(ComplexPoint).Name;
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
		public const string NodeIdKey = "NodeID";
		public const string TagsKey = "Tags";
		public const string MaxRampValueKey = "MaxRampValue";
		public const string MinRampValueKey = "MinRampValue";
		public const string RampUpdateRateInSecondsKey = "RampUpdateRateInSeconds";
		public const string RampIncrementKey = "RampIncrement";
		public const string RampIncreasingKey = "RampIncreasing";




		public ParameterCollection GetDefaultParams()
		{
			var ret = new ParameterCollection();
			ret[NameKey] = "Default Name";
			ret[NodeIdKey] = new NodeId(Guid.NewGuid().ToString(), 2).ToString();
			ParameterCollection tags = new ParameterCollection();
			tags["ShawnTag"] = (int)VarEnum.VT_R8;
			tags["RobertTag"] = (int)VarEnum.VT_LPWSTR;
			tags["MarlinTag"] = (int)VarEnum.VT_R4;
			ret[TagsKey] = tags;
			ret[MaxRampValueKey] = 449.00;
			ret[MinRampValueKey] = 51.00;
			ret[RampUpdateRateInSecondsKey] = 1;
			ret[RampIncrementKey] = 1.00;
			ret[RampIncreasingKey] = true;
			return ret;
		}
	}
}
