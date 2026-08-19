

namespace FMUAAlarmPlugins
{
    using System;
    using Softing.Opc.Ua.Sdk;

    public class SignalSelector : BaseObjectState
    {


        protected NodeIdDictionary<NodeState> PredefinedNodes;

        protected string Signal1_NodeId;

        protected string Signal2_NodeId;

        protected string Signal3_NodeId;

        protected string Signal4_NodeId;

        protected string LowSignal_NodeId;

        protected string HighSignal_NodeId;

        protected NodeState Signal1_Node
        {
            get
            {
                NodeState node = null;
                PredefinedNodes.TryGetValue(ExpandedNodeId.ToNodeId(Signal1_NodeId, null), out node);
                return node;
            }
        }

        protected NodeState Signal2_Node
        {
            get
            {
                NodeState node = null;
                PredefinedNodes.TryGetValue(ExpandedNodeId.ToNodeId(Signal2_NodeId, null), out node);
                return node;
            }
        }

        protected NodeState Signal3_Node
        {
            get
            {
                NodeState node = null;
                PredefinedNodes.TryGetValue(ExpandedNodeId.ToNodeId(Signal3_NodeId, null), out node);
                return node;
            }
        }

        protected NodeState Signal4_Node
        {
            get
            {
                NodeState node = null;
                PredefinedNodes.TryGetValue(ExpandedNodeId.ToNodeId(Signal4_NodeId, null), out node);
                return node;
            }
        }

        protected NodeState LowSignal_Node
        {
            get
            {
                NodeState node = null;
                PredefinedNodes.TryGetValue(ExpandedNodeId.ToNodeId(LowSignal_NodeId, null), out node);
                return node;
            }
        }

                protected NodeState HighSignal_Node
        {
            get
            {
                NodeState node = null;
                PredefinedNodes.TryGetValue(ExpandedNodeId.ToNodeId(HighSignal_NodeId, null), out node);
                return node;
            }
        }

        public SignalSelector(
            ISystemContext context,
            NodeState parent,
            ushort namespaceIndex,
            string name,
            string signal1_NodeId,
            string signal2_NodeId,
            string signal3_NodeId,
            string signal4_NodeId,
            string lowSignal_NodeId,
            string highSignal_NodeId,
            NodeIdDictionary<NodeState> predefinedNodes
            )
            : base(parent)
        {
            PredefinedNodes = predefinedNodes;
            Signal1_NodeId = signal1_NodeId;
            Signal2_NodeId = signal2_NodeId;
            Signal3_NodeId = signal3_NodeId;
            Signal4_NodeId = signal4_NodeId;
            LowSignal_NodeId = lowSignal_NodeId;
            HighSignal_NodeId = highSignal_NodeId;
            this.Create(
                context,
                null,
                new QualifiedName(
                    name, namespaceIndex),
                null,
                true);
            this.ProcessSignals(context);
            this.SetupEventHandler(Signal1_Node);
            this.SetupEventHandler(Signal2_Node);
            this.SetupEventHandler(Signal3_Node);
            this.SetupEventHandler(Signal4_Node);
        }

        protected void EvaluateSignal(double signal, ref double high, ref double low)
        {
            if (signal > high)
            {
                high = signal;
            }
            if (signal < low)
            {
                low = signal;
            }
        }

        protected double GetSignalValue(ISystemContext context, NodeState signalNode, NodeState changeNode, double value)
        {
            if (changeNode != null)
            {
                if (signalNode.NodeId.ToString() == changeNode.NodeId.ToString())
                {
                    return value;
                }
            }
            return this.GetNodeValue(signalNode);

        }

        protected void ProcessSignals(ISystemContext context, NodeState changeNode = null, double value = 0)
        {
            double sig1 = this.GetSignalValue(context,Signal1_Node, changeNode, value );
            double sig2 = this.GetSignalValue(context, Signal2_Node, changeNode, value);
            double sig3 = this.GetSignalValue(context, Signal3_Node, changeNode, value);
            double sig4 = this.GetSignalValue(context, Signal4_Node, changeNode, value);
            double high = sig1;
            double low = sig1;
            EvaluateSignal(sig2, ref high, ref low);
            EvaluateSignal(sig3, ref high, ref low);
            EvaluateSignal(sig4, ref high, ref low);
            this.SetNodeValue(context,LowSignal_Node,low);
            this.SetNodeValue(context,HighSignal_Node,high);
        }

        protected void SetupEventHandler(NodeState node)
        {
            if (node.GetType() == typeof(DataItemState<double>) || node.GetType().IsSubclassOf(typeof(DataItemState<double>)))
            {
                var fromDouble = ((DataItemState<double>)node);
                fromDouble.OnSimpleWriteValue += this.NodeValueSimpleEventHandler;
            }
            else
            {
                var fromDouble = ((BaseDataVariableState<double>)node);
                fromDouble.OnSimpleWriteValue += this.NodeValueSimpleEventHandler;
            }
        }

        protected double GetNodeValue(NodeState node)
        {
            if (node.GetType() == typeof(DataItemState<double>) || node.GetType().IsSubclassOf(typeof(DataItemState<double>)))
            {
                var fromDouble = ((DataItemState<double>)node);
                return fromDouble.Value;
            }
            else 
            {
                var fromDouble = ((BaseDataVariableState<double>)node);
                return fromDouble.Value;
            }
        }

        protected void SetNodeValue(ISystemContext context, NodeState node, double value)
        {
            if (node.GetType() == typeof(DataItemState<double>) || node.GetType().IsSubclassOf(typeof(DataItemState<double>)))
            {
                var nodeDouble = ((DataItemState<double>)node);
                if (Math.Abs(nodeDouble.Value - value) > Math.Pow(10,-12))
                {
                    nodeDouble.Value = value;
                    nodeDouble.ClearChangeMasks(context, false);
                    var objValue = (object)value;
                    if (nodeDouble.OnSimpleWriteValue != null)
                    {
                        nodeDouble.OnSimpleWriteValue(context, node, ref objValue);
                    }
                }
            }
            else if (node.GetType() == typeof(BaseDataVariableState<double>) || node.GetType().IsSubclassOf(typeof(BaseDataVariableState<double>)))
            {
                var nodeDouble = ((BaseDataVariableState<double>)node);
                if (Math.Abs(nodeDouble.Value - value) > Math.Pow(10, -12))
                {
                    nodeDouble.Value = value;
                    nodeDouble.ClearChangeMasks(context, false);
                    var objValue = (object)value;
                    if (nodeDouble.OnSimpleWriteValue != null)
                    {
                        nodeDouble.OnSimpleWriteValue(context, node, ref objValue);
                    }
                }
            }
        }

        public ServiceResult NodeValueSimpleEventHandler(ISystemContext context, NodeState node, ref object value)
        {
            this.ProcessSignals(context,node,(double)value);
            return ServiceResult.Good;
        }
    }
}
