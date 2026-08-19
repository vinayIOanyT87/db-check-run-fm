using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class AssociationQuantitySR : AccountingServiceRequest
	{
		#region Properties
		[DataMember]
		public string ChildTransID 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public TransactionTypes ParentTypeID 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public TransactionTypes ChildTypeID 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public string Product 
		{ 
			get; 
			set; 
		}
		#endregion // Properties

		#region Construction
		/// <summary>
		/// This is the default constructor for the Association Quantity Service Request class.
		/// </summary>
		public AssociationQuantitySR ( ) : base ( )
		{
			this.Product		= "";
			this.ChildTransID	= "";
			this.ParentTypeID	= TransactionTypes.T_Maximum;
			this.ChildTypeID	= TransactionTypes.T_Maximum;
		}
		#endregion // Construction

		#region Public Methods
		public bool Validate ( )
		{
			return base.Security != null &&
					this.ChildTransID.Length > 0 &&
					this.Product.Length > 0 &&
					this.ParentTypeID != TransactionTypes.T_Maximum &&
					this.ChildTypeID != TransactionTypes.T_Maximum;
		}
		#endregion
	}
}
