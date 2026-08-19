using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class LedgerPageSR : AccountingServiceRequest
	{
		#region Attributes
		[DataMember]
		private string month;
		#endregion Attributes

		#region Properties
		/// <summary>
		/// This property sets and gets the month/year value.
		/// </summary>
		public string Month
		{
			get { return this.month; }
			set { this.month = value; }
		}
		#endregion Properties

		#region Constructors
		/// <summary>
		/// This is the default constructor for the ledger page service request.
		/// </summary>
		public LedgerPageSR()
		{
		}
		#endregion
	}
}
