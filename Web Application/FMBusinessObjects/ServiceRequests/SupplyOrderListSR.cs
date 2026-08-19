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
	public class SupplyOrderListSR : AccountingServiceRequest
	{
		#region Public enumeration
		public enum RequestTypes
		{
			GET_HEADER_DATA,
			GET_DETAIL,
			NONE
		};
		#endregion

		#region Private data members
		[DataMember]
		private RequestTypes subRequest;
		[DataMember]
		private SupplyOrderListFilterCriteria criteria;
		[DataMember]
		private string sortExpression = "";
		[DataMember]
		private string allText = "{All}";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the supply order list service
		/// request class.
		/// </summary>
		public SupplyOrderListSR ( )
		{
			this.subRequest = RequestTypes.NONE;
			this.criteria   = new SupplyOrderListFilterCriteria ( );
		}
		#endregion

		#region Properties

		public RequestTypes SubRequest
		{
			get { return this.subRequest; }
			set { this.subRequest = value; }
		}

		public SupplyOrderListFilterCriteria Criteria
		{
			get { return this.criteria; }
			set { this.criteria = value; }
		}

		public string SortExpression
		{
			get { return this.sortExpression; }
			set { this.sortExpression = value; }
		}

		public string AllText
		{
			get { return this.allText; }
			set { this.allText = value; }
		}
		#endregion
	}
}
