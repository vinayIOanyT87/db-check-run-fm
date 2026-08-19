using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using System;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	public class OrderAssociatedTxProcessorClass : IOrderAssociatedTxProcessor
	{
		#region Private data members
		private OrderAssociatedTxDO orderAssociatedTxDO;
		private OrderAssociatedTxSR orderAssociatedTxSR;
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public OrderAssociatedTxProcessorClass()
		{
			this.orderAssociatedTxDO = null;
			this.orderAssociatedTxSR = null;
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		public OrderAssociatedTxDO Process(OrderAssociatedTxSR inOrderAssociatedTxSR)
		{
			// Save & Create necessary objects
			this.orderAssociatedTxSR = inOrderAssociatedTxSR;
			this.orderAssociatedTxDO = new OrderAssociatedTxDO();

			// Process the request
			switch (this.orderAssociatedTxSR.SubRequest)
			{
				case OrderAssociatedTxSR.RequestTypes.GET_ASSOCIATED_TRANSACTIONS:
					this.GetAssociatedTransactions();
					break;
			}

			return this.orderAssociatedTxDO;
		}


		private void GetAssociatedTransactions()
		{
			// Get the dataset from the database system
			DataSet ds = null;
			using (SqlCommand cmd = this.orderAssociatedTxDO.getSelectCommand(this.orderAssociatedTxSR.Security,
																	 this.orderAssociatedTxSR.TransactionLineItemGuid,
																	 this.orderAssociatedTxSR.SortExpression))
			{
				ds = this.consolidatedDA.GetDataSet(cmd, this.orderAssociatedTxSR.Security);
			}

			foreach (DataRow Row in ds.Tables[0].Rows)
			{
				// Create a line item object
				OrderAssociatedTxLineItemDO lineItem = new OrderAssociatedTxLineItemDO();

				// Load it
				lineItem.TransactionID = (Row.IsNull("TransactionID") == true) ? null : (string)Row["TransactionID"];
				lineItem.TransactionAlias = (Row.IsNull("TransactionAlias") == true) ? null : (string)Row["TransactionAlias"];
				lineItem.OrderStatus = (Row.IsNull("LookupTransactionStatusIndex") == true) ? "0" : Row["LookupTransactionStatusIndex"].ToString();

				lineItem.TransactionDate = (Row.IsNull("TransactionDate") == true) ? null : Row["TransactionDate"].ToString();
				lineItem.TransactionDateTime = (Row.IsNull("TransactionDate") == true) ? DateTimeOffset.Now : (DateTimeOffset)Row["TransactionDate"];

				lineItem.InventoryDateTime = (Row.IsNull("InventoryDate") == true) ? DateTimeOffset.Now : (DateTimeOffset)(DateTime)Row["InventoryDate"];
				lineItem.InventoryDate = DateEfficacy.convertToMonthDayYear(lineItem.InventoryDateTime);

				lineItem.SupplierID = (Row.IsNull("SupplierID") == true) ? null : (string)Row["SupplierID"];
				lineItem.Manager = (Row.IsNull("ManagerID") == true) ? null : (string)Row["ManagerID"];
				lineItem.Owner = (Row.IsNull("OwnerID") == true) ? null : (string)Row["OwnerID"];
				lineItem.BillToID = (Row.IsNull("BillToID") == true) ? null : (string)Row["BillToID"];
				lineItem.ShipperID = (Row.IsNull("ShipperID") == true) ? null : (string)Row["ShipperID"];
				lineItem.ShipToID = (Row.IsNull("ShipToID") == true) ? null : (string)Row["ShipToID"];
				lineItem.CarrierID = (Row.IsNull("CarrierID") == true) ? null : (string)Row["CarrierID"];

				lineItem.DocumentNumber = (Row.IsNull("DocumentNumber") == true) ? null : (string)Row["DocumentNumber"];
				lineItem.SiteID = (Row.IsNull("Site") == true) ? null : (string)Row["Site"];

				// Save it if it does not already exist in the list
				if (TransIDAlreadyExists(lineItem.TransactionID) == false)
				{
					this.orderAssociatedTxDO.Transactions.Add(lineItem);
				}
			}
		}

		private bool TransIDAlreadyExists(string TransID)
		{
			foreach (OrderAssociatedTxLineItemDO ExistingItem in this.orderAssociatedTxDO.Transactions)
			{
				if (ExistingItem.TransactionID == TransID)
				{
					return true;
				}
			}

			return false;
		}

		private DataObject view()
		{
			return null;
		}

		private DataObject add()
		{
			return null;
		}

		private DataObject delete()
		{
			return null;
		}

		private DataObject modify()
		{
			return null;
		}
	}
}