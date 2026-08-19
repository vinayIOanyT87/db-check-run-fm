

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;
	using FMBusinessObjects.Attributes;

	[Serializable]
	[CollectionDataContract]
	public class OpcUAServerCollectionClass : List<OpcUAServerClass> { }

	[Serializable]
	[DataContract]
	public class OpcUAServerClass : BaseDataObject
	{

		public new string ID
		{
			get
			{
				return base.ID;
			}
			set
			{
				base.ID = value;
			}
		}

		[DataMember]
		[FMPersistedField]
		public Guid OpcUaServerGuid
		{
			get
			{
				return this.IdentityGuid;
			}

			set
			{
				this.IdentityGuid = value;
			}
		}

		[DataMember]
		[FMPersistedField]
		public string ServerEndPoint { get; set; }

		[DataMember]
		[FMPersistedField]
		public string SecurityMode { get; set; }

		[DataMember]
		[FMPersistedField]
		public string SecurityPolicy { get; set; }

		[DataMember]
		[FMPersistedField]
		public string MessageEncoding { get; set; }

		[DataMember]
		[FMPersistedField]
		public string UserIdentityMethod { get; set; }

		[DataMember]
		[FMPersistedField]
		public string UserId { get; set; }

		[DataMember]
		[FMPersistedField]
		public string UserPassword { get; set; }

		[DataMember]
		[FMPersistedField]
		public string UserCertificatePath { get; set; }	

	}
}
