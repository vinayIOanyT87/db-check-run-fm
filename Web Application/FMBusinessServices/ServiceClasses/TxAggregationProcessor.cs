using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;
using System.Data;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices;
using FMBusinessServices.InternalClasses;
using FMBusinessServices.ServiceClasses;

namespace FMBusinessServices.ServiceClasses
{
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TxAggregationProcessorClass : ITxAggregationProcessor
	{
		#region Attributes
		protected TxAggregationSR sr = null;
		protected TxAggregationDO result = null;
		#endregion // Attributes

		#region Construction
		public TxAggregationProcessorClass()
		{
		}
		#endregion // Construction

		#region Publice methods

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public TxAggregationDO Process(TxAggregationSR inSR)
		{
			this.sr = inSR;

			result = this.AggregateTransactions();

			return result;
		}
		#endregion // Overrides

		private TxAggregationDO AggregateTransactions()
		{
			TxAggregationDO result = null;

			// service request contains workable data
			if (sr.Validate())
			{
				using (TxAggregationDBI dbi = new TxAggregationDBI(this.sr.Security.UserID, DateTimeOffset.Now))
				{
					result = dbi.Aggregate(this.sr.Security, this.sr.Security.SiteGuid, (short)this.sr.ParentTransTypeID, this.sr.AtxLineItemGuids);
				}
			}

			return result;
		}
	}
}