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
	public class FuelOrderAssociatedTxSR : AccountingServiceRequest
	{
		#region Public enumeration
		public enum RequestTypes
		{
			GetAvailableTransactions,
			GetAssociatedTransactions,
			None
		};
		#endregion

		#region Private data members

		[DataMember]
		private RequestTypes requestType = RequestTypes.None;
		[DataMember]
		private FuelOrderAssociatedTxDO.SearchCriteria searchCriteria = null;

		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the fuel order associated
		/// transaction service request class.
		/// </summary>
		public FuelOrderAssociatedTxSR()
		{
			searchCriteria = new FuelOrderAssociatedTxDO.SearchCriteria();
		}
		#endregion

		#region Properties

		public RequestTypes RequestType
		{
			get { return this.requestType; }
			set { this.requestType = value; }
		}

		public FuelOrderAssociatedTxDO.SearchCriteria SearchCriteria
		{
			get { return this.searchCriteria; }
			set { this.searchCriteria = value; }
		}
		#endregion
	}
}
