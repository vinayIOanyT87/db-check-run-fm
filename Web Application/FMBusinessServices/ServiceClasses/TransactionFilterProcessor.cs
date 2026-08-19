using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Diagnostics;
using System.Reflection;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	public class TransactionFilterProcessorClass : ITransactionFilterProcessor
	{
		#region Attributes
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the get transaction processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		public TransactionFilterProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Methods
		public GetTransactionDO Process ( TransactionFilterSR sr )
		{
			string sql = null;
			GetTransactionDO result = new GetTransactionDO ( );

			// 1=1 is there so that in requests where there are no conditions do not result in SQL errors
			sql = "SELECT * FROM tblTransactions WHERE 1=1";

			if (sr.Site.Length > 0)
			{
				sql += " AND Site = '" + sr.Site + "'";
			}

			if (sr.SupplierID.Length > 0)
			{
				sql += " AND SupplierID = '" + sr.SupplierID + "'";
			}

			if (sr.TransTypeID > 0 && sr.TransTypeID < TransactionTypes.T_Maximum)
			{
				sql += " AND LookupTransTypeIndex = " + ((short)sr.TransTypeID).ToString();
			}


			if (sr.StartDateInventory != null && sr.EndDateInventory != null)
			{
				string startDate = sr.StartDateInventory.ToString ( "\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ hh:mm:ss\\'\\}" );
				string endDate = sr.EndDateInventory.ToString ( "\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ hh:mm:ss\\'\\}" );

				if (sr.UseDate == TransactionFilterSR.DateType.TRANSACTIONDATETIME)
				{
					sql += " AND TransDateTime BETWEEN " + startDate + " AND " + endDate;
				}
				else
				{
					sql += " AND InventoryDate BETWEEN " + startDate + " AND " + endDate;
				}
			}

			using (SqlCommand cmd = new SqlCommand())
			{

				//result.TransactionDataSet = this.consolidatedDA.GetDataSet(sr.Security, sql);
			}

			return result;
		}
		#endregion
	}
}