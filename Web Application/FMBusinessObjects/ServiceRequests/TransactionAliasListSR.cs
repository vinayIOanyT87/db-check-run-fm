using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class TransactionAliasListSR : AccountingServiceRequest
	{
		#region Private data members
		[DataMember] private short transType = 0;
		[DataMember] private bool getOwnerSiteID = true;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the transaction alias list service
		/// request class.
		/// </summary>
		public TransactionAliasListSR ( )
		{
		}
		#endregion

		#region Properties
		public short TransType
		{
			get { return this.transType; }
			set { this.transType = value; }
		}

		public bool GetOwnerSiteID
		{
			get { return this.getOwnerSiteID; }
			set { this.getOwnerSiteID = value; }
		}
		#endregion
	}
}
