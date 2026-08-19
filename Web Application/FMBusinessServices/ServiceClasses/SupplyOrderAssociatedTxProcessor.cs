using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Web;
using System.Diagnostics;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	public class SupplyOrderAssociatedTxProcessorClass : ISupplyOrderAssociatedTxProcessor
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		private SupplyOrderAssociatedTxDO supplyorderAssociatedTxDO;
		private SupplyOrderAssociatedTxSR supplyorderAssociatedTxSR;
		#endregion

		#region Constructors
		public SupplyOrderAssociatedTxProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
			this.supplyorderAssociatedTxDO = null;
			this.supplyorderAssociatedTxSR = null;
		}
		#endregion

		public SupplyOrderAssociatedTxDO Process ( SupplyOrderAssociatedTxSR supplyorderAssociatedTxSR )
		{
			// Save & Create necessary objects
			this.supplyorderAssociatedTxSR = supplyorderAssociatedTxSR;
			this.supplyorderAssociatedTxDO = new SupplyOrderAssociatedTxDO ( );

			// Process the request
			switch (this.supplyorderAssociatedTxSR.SubRequest)
			{
				case SupplyOrderAssociatedTxSR.RequestTypes.GET_ASSOCIATED_TRANSACTIONS:
					this.GetAssociatedTransactions ( );
					break;
			}

			return this.supplyorderAssociatedTxDO;
		}


		private void GetAssociatedTransactions ( )
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				this.supplyorderAssociatedTxDO.GetSelectCommand(cmd,
																				this.supplyorderAssociatedTxSR.Security,
																				this.supplyorderAssociatedTxSR.TransactionLineItemGuid,
																				this.supplyorderAssociatedTxSR.SortExpression);

				// Get the dataset from the database system
				DataSet ds = this.consolidatedDA.GetDataSet(cmd, this.supplyorderAssociatedTxSR.Security);

				foreach (DataRow Row in ds.Tables[0].Rows)
				{
					// Create a line item object
					SupplyOrderAssociatedTxLineItemDO lineItem = new SupplyOrderAssociatedTxLineItemDO();

					// Load it
					lineItem.TransactionID = (Row.IsNull("TransactionID") == true) ? null : (string)Row["TransactionID"];
					lineItem.TransactionAlias = (Row.IsNull("TransactionAlias") == true) ? null : (string)Row["TransactionAlias"];
					lineItem.OrderStatus = (Row.IsNull("LookupTransactionStatusIndex") == true) ? null : Row["LookupTransactionStatusIndex"].ToString();

                    lineItem.TransactionDate = (Row.IsNull("TransactionDate") == true) ? null : Row["TransactionDate"].ToString();
					lineItem.TransactionDateTime = (Row.IsNull("TransactionDate") == true) ? DateTimeOffset.Now : (DateTimeOffset)Row["TransactionDate"];

					lineItem.InventoryDateTime = (Row.IsNull("InventoryDate") == true) ? DateTimeOffset.Now : new DateTimeOffset((DateTime)Row["InventoryDate"], lineItem.TransactionDateTime.Offset); 
					lineItem.InventoryDate = DateEfficacy.convertToMonthDayYear(lineItem.InventoryDateTime);

					lineItem.DocumentNumber = (Row.IsNull("DocumentNumber") == true) ? null : Row["DocumentNumber"].ToString();
					lineItem.PONumber = (Row.IsNull("PONumber") == true) ? null : Row["PONumber"].ToString();
					lineItem.SupplierID = (Row.IsNull("SupplierID") == true) ? null : Row["SupplierID"].ToString();
					lineItem.Manager = (Row.IsNull("ManagerID") == true) ? null : Row["ManagerID"].ToString();
					lineItem.Owner = (Row.IsNull("OwnerID") == true) ? null : Row["OwnerID"].ToString();
					lineItem.BillToID = (Row.IsNull("BillToID") == true) ? null : Row["BillToID"].ToString();
					lineItem.ShipperID = (Row.IsNull("ShipperID") == true) ? null : Row["ShipperID"].ToString();
					lineItem.ShipToID = (Row.IsNull("ShipToID") == true) ? null : Row["ShipToID"].ToString();
					lineItem.CarrierID = (Row.IsNull("CarrierID") == true) ? null : Row["CarrierID"].ToString();
					lineItem.SiteID = (Row.IsNull("Site") == true) ? null : Row["Site"].ToString();

					// Save it if it does not already exist in the list
					if (this.TransIDAlreadyExists(lineItem.TransactionID) == false)
					{
						this.supplyorderAssociatedTxDO.Transactions.Add(lineItem);
					}
				}
			}
		}

		private bool TransIDAlreadyExists ( string transID )
		{
			foreach (SupplyOrderAssociatedTxLineItemDO ExistingItem in this.supplyorderAssociatedTxDO.Transactions)
			{
				if (ExistingItem.TransactionID == transID)
				{
					return true;
				}
			}

			return false;
		}

		private DataObject view ( )
		{
			return null;
		}

		private DataObject add ( )
		{
			return null;
		}

		private DataObject delete ( )
		{
			return null;
		}

		private DataObject modify ( )
		{
			return null;
		}
	}
}