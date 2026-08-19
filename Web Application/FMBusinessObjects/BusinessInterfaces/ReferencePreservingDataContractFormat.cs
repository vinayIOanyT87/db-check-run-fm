using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace FMBusinessObjects.BusinessInterfaces
{
	/// <summary>
	/// (Kendall) The purpose of this attribute is to support data object cycles during serialization of WCF service calls.
	/// By default object references are not preserved by the DataContractSerializer; Values of an object referenced 
	/// multiple times are serialized multiple times. If the object is part of mutual (cyclic) 
	/// reference (e.g. circular linked list) an exception is thrown during serialization.
	/// 
	/// The exception thrown is: The InnerException message was 'Object graph for type 
	/// 'FMBusinessObjects.DataObjects.QueryWriterFieldCollection' contains cycles and cannot be serialized if reference 
	/// tracking is disabled.'
	/// </summary>
	/// <example>
	/// [ServiceContract]
	/// public interface IQueries
	/// {
	///		[OperationContract]
	///		[ReferencePreservingDataContractFormat]
	///		QueryClass NewQuery( SecurityClass security, QueryWriterTopic topic )
	/// }
	/// </example>
	public class ReferencePreservingDataContractFormatAttribute : Attribute, IOperationBehavior
	{
		#region IOperationBehavior Members
		public void AddBindingParameters( OperationDescription description, BindingParameterCollection parameters )
		{
		}

		public void ApplyClientBehavior( OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy )
		{
			IOperationBehavior innerBehavior = new ReferencePreservingDataContractSerializerOperationBehavior( description );
			innerBehavior.ApplyClientBehavior( description, proxy );
		}

		public void ApplyDispatchBehavior( OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch )
		{
			IOperationBehavior innerBehavior = new ReferencePreservingDataContractSerializerOperationBehavior( description );
			innerBehavior.ApplyDispatchBehavior( description, dispatch );
		}

		public void Validate( OperationDescription description )
		{
		}

		#endregion
	}

	class ReferencePreservingDataContractSerializerOperationBehavior : DataContractSerializerOperationBehavior
	{
		public ReferencePreservingDataContractSerializerOperationBehavior( OperationDescription operationDescription ) : base( operationDescription ) { }
		public override XmlObjectSerializer CreateSerializer( Type type, string name, string ns, IList<Type> knownTypes )
		{
			return CreateDataContractSerializer( type, name, ns, knownTypes );
		}

		private static XmlObjectSerializer CreateDataContractSerializer( Type type, string name, string ns, IList<Type> knownTypes )
		{
			return CreateDataContractSerializer( type, name, ns, knownTypes );
		}

		public override XmlObjectSerializer CreateSerializer( Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes )
		{
			return new DataContractSerializer( type, name, ns, knownTypes,
				0x7FFFFFFF /*maxItemsInObjectGraph*/,
				false/*ignoreExtensionDataObject*/,
				true/*preserveObjectReferences*/,
				null/*dataContractSurrogate*/);
		}
	}
}
