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
	public class OrderListSR : AccountingServiceRequest
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
		private OrderListFilterCriteria criteria;
		[DataMember]
		private string sortExpression = "";
		[DataMember]
		private string allText = "{All}";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the order list
		/// service request class.
		/// </summary>
		public OrderListSR ( )
		{
			this.subRequest = RequestTypes.NONE;
			this.criteria = new OrderListFilterCriteria ( );
		}
		#endregion

		#region Properties

		public RequestTypes SubRequest
		{
			get { return this.subRequest; }
			set { this.subRequest = value; }
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

		public OrderListFilterCriteria Criteria
		{
			get { return this.criteria; }
			set { this.criteria = value; }
		}

		#endregion
	}
}
