using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
	[Serializable]
	public class DrawdownSR : AccountingServiceRequest
	{
		#region Public data members
		public enum RequestTypes
		{
			SupplyOrderSaved,
			ChildSaved,
			None
		}
		#endregion

		#region Private data members
		[DataMember]
		private LineItemDO lineItem;
		[DataMember]
		private TransactionAliasClass alias;
		[DataMember]
		private RequestTypes requestType = RequestTypes.None;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Drawdown service request class.
		/// </summary>
		public DrawdownSR()
		{
		}
		#endregion

		#region Properties

		public LineItemDO LineItem
		{
			get { return this.lineItem; }
			set { this.lineItem = value; }
		}

		public TransactionAliasClass Alias
		{
			get { return this.alias; }
			set { this.alias = value; }
		}

		public RequestTypes RequestType
		{
			get { return this.requestType; }
			set { this.requestType = value; }
		}
		#endregion
	}
}
