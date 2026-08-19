using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class GetFuelOrderReceiptedLineItemsSR : AccountingServiceRequest
	{
		#region Properties
		[DataMember]
		public string TransID { get; set; }
		#endregion // Properties

		#region Construction
		public GetFuelOrderReceiptedLineItemsSR ( ) : base ( )
		{
			this.TransID = string.Empty;
		}
		#endregion // Construction
	}
}
