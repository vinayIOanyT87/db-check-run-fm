using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
	public class DataTransmissionRecordCollectionClass : List<DataTransmissionRecordClass>
	{
	}

	[DataContract]
   [Serializable]
	public class DataTransmissionRecordClass
	{
		#region Constructors
		/// <summary>
		/// This is the default contructor for the Data Transmission Record Class.
		/// </summary>
		public DataTransmissionRecordClass ( )
		{
		}
		#endregion

		#region Properties
		[DataMember]
		public string OriginatingSiteID 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public EquipmentClass Equipment 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public CompanyClass Company 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public ProductClass Product 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public FuelCardClass FuelCard 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public PersonClass Person 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public TransactionDO Transaction 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public ChangeQueueRecordClass ChangeQueueRecord 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public TransactionAliasClass TransactionAlias 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public GroupClass Group 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public CloseoutDO Closeout 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public ApplicationStringClass ApplicationString 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public PIDXProfileClass PidxProfile 
		{ 
			get; 
			set; 
		}
		#endregion
	}
}
