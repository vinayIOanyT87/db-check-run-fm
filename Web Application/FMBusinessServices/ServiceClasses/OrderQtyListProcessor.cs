using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Web;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;

namespace FMBusinessServices.ServiceClasses
{
	public class OrderQtyListProcessorClass : IOrderQtyListProcessor
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public OrderQtyListProcessorClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		public OrderQtyListDO Process( OrderQtyListSR orderQtyListSR )
		{
			OrderQtyListDO orderQtyListDO = new OrderQtyListDO();

			// Parameterize the command
			using (SqlCommand cmd = new SqlCommand())
			{
				this.PrepareSelectCommand(cmd, orderQtyListSR.TransactionGuid);
				DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, orderQtyListSR.Security);

				// Query the data
				try
				{
					this.ParseData(dataSet, orderQtyListDO);
				}
				catch (Exception ex)
				{
					throw ex;
				}

				return orderQtyListDO;
			}
		}

		private void PrepareSelectCommand(SqlCommand selectCMD, Guid transactionGuid)
		{
			selectCMD.CommandText = "DECLARE @OrderLineItems TABLE ( [TransactionLineItemGuid] [uniqueidentifier] NOT NULL PRIMARY KEY ) " +
					" INSERT INTO @OrderLineItems SELECT A.TransactionLineItemGuid FROM tblTransactionLineItems A " +
					" WHERE TransactionGuid = @TransactionGuid " +
					" " +
					" SELECT SUM(GrossQty) AS GrossQty, SUM(NetQty) AS NetQty, SUM(MassQty) as MassQty, OrderReferenceTransactionLineItemGuid " +
					" FROM (SELECT B.GrossQuantity as GrossQty, B.NetQuantity as NetQty, B.MassQuantity as MassQty, B.OrderReferenceTransactionLineItemGuid " +
					" FROM tblTransactions A INNER JOIN tblTransactionLineItems B on A.TransactionGuid = B.TransactionGuid " +
					" WHERE (B.OrderReferenceTransactionLineItemGuid in (SELECT * FROM @OrderLineItems))" +
					" AND (B.LookupTransactionStatusIndex = 0" +
					" OR B.LookupTransactionStatusIndex = 11" +
					" OR ((B.LookupTransactionStatusIndex <> 0 AND B.LookupTransactionStatusIndex <> 7 AND B.LookupTransactionStatusIndex <> 11) AND -B.PresetAmount >= B.NetQuantity))" +
					" AND (A.DeleteFlag = 0 OR A.DeleteFlag IS NULL)" +
					" UNION ALL" +
					" SELECT -B.PresetAmount AS GrossQty, -B.PresetAmount as NetQty, B.MassQuantity as MassQty, B.OrderReferenceTransactionLineItemGuid " +
					" FROM tblTransactions A INNER JOIN tblTransactionLineItems B on A.TransactionGuid = B.TransactionGuid " +
					" WHERE (B.OrderReferenceTransactionLineItemGuid in (SELECT * FROM @OrderLineItems))" +
					" AND ((B.LookupTransactionStatusIndex <> 0 AND B.LookupTransactionStatusIndex <> 7 AND B.LookupTransactionStatusIndex <> 11) AND -B.PresetAmount < B.NetQuantity)" +
					" AND (A.DeleteFlag = 0 OR A.DeleteFlag IS NULL)) tblQuantities" +
					" GROUP BY OrderReferenceTransactionLineItemGuid";


			selectCMD.Parameters.Add("@TransactionGuid", SqlDbType.UniqueIdentifier);

			selectCMD.Parameters[0].Value = transactionGuid;
		}

		private void ParseData( DataSet dataSet, OrderQtyListDO orderQtyListDO )
		{
			Guid transLineItemGuid;
			double gross, net, mass;

			if (( dataSet != null ) && ( dataSet.Tables != null ) && ( dataSet.Tables.Count > 0 ))
			{
				DataTable table = dataSet.Tables[0];

				if ( table.Rows != null )
				{
					foreach (DataRow row in table.Rows)
					{
						gross			= ( row.IsNull ( "GrossQty" ) == true )				? 0 : (double) row["GrossQty"];
						net				= ( row.IsNull ( "NetQty" ) == true )				? 0 : (double) row["NetQty"];
						mass			= ( row.IsNull ( "MassQty" ) == true )				? 0 : (double) row["MassQty"];
						transLineItemGuid = (row.IsNull("OrderReferenceTransactionLineItemGuid") == true) ? Guid.Empty : (Guid)row["OrderReferenceTransactionLineItemGuid"];

						orderQtyListDO.Add ( transLineItemGuid, gross, net, mass );
					}
				}
			}
		}
	}
}
