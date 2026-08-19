

namespace FMUAAlarmPlugins
{
    using Softing.Opc.Ua.Sdk;

    public class Folder : FolderState
    {
        public Folder(       
            ISystemContext context,
            NodeState parent,
            ushort namespaceIndex,
            string name,
            string nodeID
            )
            : base(parent)
        {
            this.Create(
                context,
                new NodeId(nodeID),
                new QualifiedName(name, namespaceIndex),
                name,
                false);
            TypeDefinitionId = Softing.Opc.Ua.Sdk.ObjectTypeIds.FolderType;
            EventNotifier = EventNotifiers.SubscribeToEvents;
            parent.AddChild(this);
            parent.AddReference(Softing.Opc.Ua.Sdk.ReferenceTypeIds.Organizes, false, this.NodeId);
            this.AddReference(Softing.Opc.Ua.Sdk.ReferenceTypeIds.Organizes, true, parent.NodeId);
            parent.AddNotifier(context, ReferenceTypeIds.HasEventSource, false, this);
            this.AddNotifier(context, ReferenceTypeIds.HasEventSource, true, parent);
        }

	    public override void Delete(ISystemContext context)
	    {
			 Parent.RemoveNotifier(context, this, false);
			 this.RemoveNotifier(context,Parent,true);
		    Parent.RemoveReference(Softing.Opc.Ua.Sdk.ReferenceTypeIds.Organizes, false, this.NodeId);
			 this.RemoveReference(Softing.Opc.Ua.Sdk.ReferenceTypeIds.Organizes, true, Parent.NodeId);
			 Parent.RemoveChild(this);
			 base.Delete(context);
		    
	    }
    }
}
