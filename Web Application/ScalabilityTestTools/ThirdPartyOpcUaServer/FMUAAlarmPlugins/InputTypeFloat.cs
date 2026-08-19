
namespace FMUAAlarmPlugins
{
	using Softing.Opc.Ua.Sdk;

	public class InputTypeFloat : DataItemState<float>
	{
		public InputTypeFloat(
			 ISystemContext context,
			 NodeState parent,
			 ushort namespaceIndex,
			 string name,
			 float initialValue,
			 string nodeId
			 )
			: base(parent)
		{

			this.Create(
				  context,
				  new NodeId(nodeId),
				  new QualifiedName(name, namespaceIndex),
				  name,
				  false);
			this.Value = initialValue;
		}

	}
}
