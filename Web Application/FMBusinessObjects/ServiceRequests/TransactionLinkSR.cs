using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class TransactionLinkSR : AccountingServiceRequest
	{
		#region Public enumeration
		public enum Action : short
		{
			GET_LINKED_TRANSACTIONS,
			DELETE_LINEITEM_LINKS
		}
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the transaction link service request class.
		/// </summary>
		public TransactionLinkSR ( ) : base ( )
		{
			this.PerformAction			= Action.GET_LINKED_TRANSACTIONS;
			this.SourceTransIDs			= new List<string> ( );
			this.OriginalLineItemGuids	= new List<Guid> ( );
		}
		#endregion

		#region Properties
		[DataMember]
		public Action PerformAction 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public List<string> SourceTransIDs 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public List<Guid> OriginalLineItemGuids 
		{ 
			get; 
			set; 
		}
		#endregion
	}
}
