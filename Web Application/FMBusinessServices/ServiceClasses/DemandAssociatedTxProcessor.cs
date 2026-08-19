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
using FMBusinessObjects.UtilityObjects;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	public class DemandAssociatedTxProcessorClass : IDemandAssociatedTxProcessor
	{
		#region Private data members
		private DemandAssociatedTxListDO associatedList;
		private DemandAssociatedTxSR associatedSR;
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public DemandAssociatedTxProcessorClass()
		{
			this.associatedList = null;
			this.associatedSR = null;
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		public DemandAssociatedTxListDO Process(DemandAssociatedTxSR demandAssociatedTxSR)
		{
			this.associatedSR = demandAssociatedTxSR;
			this.associatedList = new DemandAssociatedTxListDO();

			switch (this.associatedSR.RequestType)
			{
				case DemandAssociatedTxSR.RequestTypes.GetAssociatedTransactions:
					this.GetAssociatedTransactions();
					break;

				case DemandAssociatedTxSR.RequestTypes.GetAvailableTransactions:
					this.GetAvailableTransactions();
					break;

				default:
					break;
			}

			return associatedList;
		}

		/// <summary>
		/// Returns the available Receipt transactions
		/// </summary>
		private void GetAvailableTransactions()
		{
			DemandAssociatedTxDO associatedDO = new DemandAssociatedTxDO();

			// Get the dataset
			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				associatedDO.GetSelectAvailableTxCommand(cmd, associatedSR.SearchCriteria.product);
				dataSet = this.consolidatedDA.GetDataSet(cmd, associatedSR.Security);
			}

			if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
			{
				DataTable dataTable = dataSet.Tables[0];

				if (dataTable.Rows != null)
				{
					foreach (DataRow row in dataTable.Rows)
					{
						DemandAssociatedTxDO assocDO = new DemandAssociatedTxDO();
						assocDO.TransactionAlias = DataObject.getString(row["AliasName"]);
						assocDO.BillToID = DataObject.getString(row["BillToID"]);
						assocDO.Product = DataObject.getString(row["Product"]);
						assocDO.InventoryDate = DataObject.getValue<DateTime>(row["InventoryDate"], DateTime.Today);
						assocDO.Manager = DataObject.getString(row["ManagerID"]);
						assocDO.Owner = DataObject.getString(row["OwnerID"]);
						assocDO.PONumber = DataObject.getString(row["PONumber"]);
						assocDO.ShipToID = DataObject.getString(row["ShipToID"]);
						assocDO.TransactionDate = DataObject.getValue<DateTimeOffset>(row["TransDateTime"], TimeConverter.Today());
						assocDO.TransactionID = DataObject.getString(row["TransID"]);
						assocDO.ShipmentNumber = DataObject.getString(row["ShipmentNumber"]);

						this.associatedList.DemandAssociatedTrans.Add(assocDO);
					}
				}
			}
		}

		/// <summary>
		/// Returns the Receipt transactions associated with a Demand
		/// </summary>
		private void GetAssociatedTransactions()
		{
			DemandAssociatedTxDO associatedDO = new DemandAssociatedTxDO();

			// Get the dataset
			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				associatedDO.GetSelectAssociatedTxCommand(cmd, associatedSR.SearchCriteria.transactionLineItemGuid, associatedSR.SearchCriteria.transID);
				dataSet = this.consolidatedDA.GetDataSet(cmd, associatedSR.Security);
			}

			if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
			{
				DataTable dataTable = dataSet.Tables[0];

				if (dataTable.Rows != null)
				{
					foreach (DataRow row in dataTable.Rows)
					{
						DemandAssociatedTxDO assocDO = new DemandAssociatedTxDO();
						assocDO.TransactionAlias = DataObject.getString(row["AliasName"]);
						assocDO.BillToID = DataObject.getString(row["BillToID"]);
						assocDO.Product = DataObject.getString(row["Product"]);
						assocDO.InventoryDate = DataObject.getValue<DateTime>(row["InventoryDate"], DateTime.Today);
						assocDO.Manager = DataObject.getString(row["ManagerID"]);
						assocDO.Owner = DataObject.getString(row["OwnerID"]);
						assocDO.PONumber = DataObject.getString(row["PONumber"]);
						assocDO.ShipToID = DataObject.getString(row["ShipToID"]);
						assocDO.TransactionDate = DataObject.getValue<DateTimeOffset>(row["TransDateTime"], TimeConverter.Today());
						assocDO.TransactionID = DataObject.getString(row["TransID"]);
						assocDO.ShipmentNumber = DataObject.getString(row["ShipmentNumber"]);

						this.associatedList.DemandAssociatedTrans.Add(assocDO);
					}
				}
			}
		}

	}
}