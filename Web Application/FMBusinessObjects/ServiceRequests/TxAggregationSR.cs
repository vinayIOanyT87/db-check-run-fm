using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class TxAggregationSR : AccountingServiceRequest
	{
		#region Properties
		[DataMember] public TransactionTypes ParentTransTypeID { get; set; }
		[DataMember] public ArrayList AtxLineItemGuids { get; set; }
		#endregion // Properties

		#region Constants
		protected const TransactionTypes DEFAULT_TRANSTYPE = TransactionTypes.T_Maximum;
		#endregion // Constants

		#region Construction
		public TxAggregationSR ( ) : base ( )
		{
			ParentTransTypeID = DEFAULT_TRANSTYPE;
			AtxLineItemGuids = new ArrayList ( );
		}
		#endregion // Construction

		public bool Validate ( )
		{
			bool result = true;

			result &= ParentTransTypeID != DEFAULT_TRANSTYPE;

			if (result)
			{
				result &= AtxLineItemGuids != null;
			}
			if (result)
			{
				result &= AtxLineItemGuids.Count > 0;
			}

			return result;
		}
	}
}
