

namespace FMUAAlarmPlugins
{
    using System;
    using Softing.Opc.Ua.Sdk;

    public class ConnectDoubles : BaseObjectState
    {

        protected string ToNodeId;

        protected NodeIdDictionary<NodeState> PredefinedNodes;

        protected NodeState ToNode
        {
            get
            {
                NodeState toNode = null;
                PredefinedNodes.TryGetValue(ExpandedNodeId.ToNodeId(ToNodeId, null), out toNode);
                return toNode;
            }
        }

        public ConnectDoubles(
            ISystemContext context,
            NodeState parent,
            ushort namespaceIndex,
            string toNodeID,
            NodeIdDictionary<NodeState> predefinedNodes
            )
            : base(parent)
        {
            PredefinedNodes = predefinedNodes;
            ToNodeId = toNodeID;
            this.Create(
                context,
                null,
                new QualifiedName(
                    "ToNode = " + ToNodeId, namespaceIndex),
                null,
                true);
            NodeState toNode = this.ToNode;
            this.SetupEventHandler(this.Parent);
            this.SetupEventHandler(toNode);
            this.SetNodeValue(context, this.Parent, this.GetNodeValue(toNode));
            //this.SetNodeValue(context, toNode, this.GetNodeValue(this.Parent));
        }

        protected void SetupEventHandler(NodeState node)
        {
            if (node.GetType() == typeof(DataItemState<double>) || node.GetType().IsSubclassOf(typeof(DataItemState<double>)))
            {
                var doub = ((DataItemState<double>)node);
                doub.OnSimpleWriteValue += this.NodeValueSimpleEventHandler;
            }
            else if (node.GetType() == typeof(BaseDataVariableState<double>) || node.GetType().IsSubclassOf(typeof(BaseDataVariableState<double>)))
            {
                var doub = ((BaseDataVariableState<double>)node);
                doub.OnSimpleWriteValue += this.NodeValueSimpleEventHandler;
            }
            else if (node.GetType() == typeof(PropertyState<double>) || node.GetType().IsSubclassOf(typeof(PropertyState<double>)))
            {
                var doub = ((PropertyState<double>)node);
                doub.OnSimpleWriteValue += this.NodeValueSimpleEventHandler;
            }
        }

        protected double GetNodeValue(NodeState node)
        {
            if (node.GetType() == typeof(DataItemState<double>) || node.GetType().IsSubclassOf(typeof(DataItemState<double>)))
            {
                var doub = ((DataItemState<double>)node);
                return doub.Value;
            }
            else if (node.GetType() == typeof(BaseDataVariableState<double>) || node.GetType().IsSubclassOf(typeof(BaseDataVariableState<double>)))
            {
                var doub = ((BaseDataVariableState<double>)node);
                return doub.Value;
            }
            else if (node.GetType() == typeof(PropertyState<double>) || node.GetType().IsSubclassOf(typeof(PropertyState<double>)))
            {
                var doub = ((PropertyState<double>)node);
                return doub.Value;
            }
            throw new Exception("Bad Type in ConnectDoubles.GetNodeValue");
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
            else if (node.GetType() == typeof(PropertyState<double>) || node.GetType().IsSubclassOf(typeof(PropertyState<double>)))
            {
                var nodeDouble = ((PropertyState<double>)node);
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
            if (node.NodeId.ToString() == this.Parent.NodeId.ToString())
            {
                this.SetNodeValue(context, this.ToNode, (double)value);
            }
            else
            {
                this.SetNodeValue(context, this.Parent, (double)value);
            }
            return ServiceResult.Good;
        }
    }
}
