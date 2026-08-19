using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[CollectionDataContract]
	public class QueryFilterGroupCollection : List<QueryFilterGroupClass> { }

	[Serializable]
	[DataContract]
	[XMLObject(NodeName = "QueryFilterGroup")]
	public class QueryFilterGroupClass
	{
		#region Properties
		[DataMember]
		[XMLProperty]
		public string FilterID { get; set; }

		[DataMember]
		[XMLProperty]
		public bool Filter { get; set; }

		[DataMember]
		[XMLProperty]
		public string DefaultValue1 { get; set; }

		[DataMember]
		[XMLProperty]
		public string DefaultValue2 { get; set; }

		[DataMember]
		public string SaveValue1 { get; set; }

		[DataMember]
		public string SaveValue2 { get; set; }

		[XMLProperty]
		[DataMember]
		public string DbFieldName { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public QueryFilterGroupClass()
		{
			this.Reset();
		}
		#endregion

		protected void Reset()
		{
			FilterID		= string.Empty;
			Filter			= false;
			DefaultValue1	= string.Empty;
			DefaultValue2	= string.Empty;
			SaveValue1		= null;
			SaveValue2		= null;
			DbFieldName		= string.Empty;
		}
	}
}
