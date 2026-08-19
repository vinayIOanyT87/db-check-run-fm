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
using FMBusinessObjects.UtilityObjects;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	public class FuelOrderAssociatedTxProcessorCLass : IFuelOrderAssociatedTxProcessor
	{
		#region Private data members
		private FuelOrderAssociatedTxListDO associatedList = null;
		private FuelOrderAssociatedTxSR associatedSR = null;
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the fuel order associated transaction processor class.
		/// </summary>
		public FuelOrderAssociatedTxProcessorCLass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		public FuelOrderAssociatedTxListDO Process(FuelOrderAssociatedTxSR inAssociatedSR)
		{
			this.associatedSR = inAssociatedSR;
			this.associatedList = new FuelOrderAssociatedTxListDO();

			switch (this.associatedSR.RequestType)
			{
				case FuelOrderAssociatedTxSR.RequestTypes.GetAssociatedTransactions:
					this.GetAssociatedTransactions();
					break;

				case FuelOrderAssociatedTxSR.RequestTypes.GetAvailableTransactions:
					this.GetAvailableTransactions();
					break;
			}

			// Process the request
			return associatedList;
		}

		/// <summary>
		/// Returns the Request/Demand transactions associated with a Fuel Order
		/// </summary>
		public void GetAssociatedTransactions()
		{
			FuelOrderAssociatedTxDO associatedDO = new FuelOrderAssociatedTxDO();
			string sql = associatedDO.getSelectAssociatedTxCommand();

			DataSet dataSet = null;
			// Prepare the sql statement			
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = sql;
				SqlParameterCollection parms = cmd.Parameters;
				SqlParameter parm;
				int i = 0;

				parms.Add(new SqlParameter("@lineItemIndex", SqlDbType.Int));
				parm = (SqlParameter)parms[i++];
				parm.Value = associatedSR.SearchCriteria.transactionLineItemGuid;

				parms.Add(new SqlParameter("@transID", SqlDbType.NVarChar, 64));
				parm = (SqlParameter)parms[i++];
				parm.Value = associatedSR.SearchCriteria.transID;
				this.consolidatedDA.GetDataSet(cmd, this.associatedSR.Security);
			}

			if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
			{
				DataTable dataTable = dataSet.Tables[0];

				if (dataTable.Rows != null)
				{
					foreach (DataRow dataRow in dataTable.Rows)
					{
						FuelOrderAssociatedTxDO assocDO = new FuelOrderAssociatedTxDO();
						assocDO.TransactionID = DataObject.getString(dataRow["TransID"]);
						assocDO.TransactionDate = DataObject.getValue<DateTimeOffset>(dataRow["TransDateTime"], TimeConverter.Today());
						assocDO.InventoryDate = DataObject.getValue<DateTime>(dataRow["InventoryDate"], DateTime.Today);
						assocDO.TransactionAlias = DataObject.getString(dataRow["AliasName"]);
						assocDO.EffectiveDate = DataObject.getValue<DateTimeOffset>(dataRow["EffectiveDate"], TimeConverter.Today());
						assocDO.ExpirationDate = DataObject.getValue<DateTimeOffset>(dataRow["ExpirationDate"], TimeConverter.Today());
						assocDO.Supplier = DataObject.getString(dataRow["SupplierID"]);
						assocDO.Owner = DataObject.getString(dataRow["OwnerID"]);
						assocDO.Manager = DataObject.getString(dataRow["ManagerID"]);
						assocDO.BillToID = DataObject.getString(dataRow["BillToID"]);
						assocDO.OriginStation = DataObject.getString(dataRow["OriginStation"]);
						assocDO.Product = DataObject.getString(dataRow["Product"]);

						this.associatedList.FuelOrderAssociatedTx.Add(assocDO);
					}
				}
			}
		}

		/// <summary>
		/// Returns Request/Demand transactions that are not associated with a Fuel Order
		/// </summary>
		public void GetAvailableTransactions()
		{
			FuelOrderAssociatedTxDO associatedDO = new FuelOrderAssociatedTxDO();
			string sql = associatedDO.getSelectAvailableTxCommand();

			DataSet dataSet = null;
			// Prepare the sql statement            
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = sql;
				SqlParameterCollection parms = cmd.Parameters;
				SqlParameter parm;
				int i = 0;

				parms.Add(new SqlParameter("@product", SqlDbType.NVarChar, 30));
				parm = (SqlParameter)parms[i++];
				parm.Value = associatedSR.SearchCriteria.product;

				this.consolidatedDA.GetDataSet(cmd, this.associatedSR.Security);
			}
			if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
			{
				DataTable dataTable = dataSet.Tables[0];

				if (dataTable.Rows != null)
				{
					foreach (DataRow dataRow in dataTable.Rows)
					{
						FuelOrderAssociatedTxDO assocDO = new FuelOrderAssociatedTxDO();

						assocDO.TransactionID = DataObject.getString(dataRow["TransID"]);
						assocDO.TransactionDate = DataObject.getValue<DateTimeOffset>(dataRow["TransDateTime"], TimeConverter.Today());
						assocDO.InventoryDate = DataObject.getValue<DateTime>(dataRow["InventoryDate"], DateTime.Today);
						assocDO.TransactionAlias = DataObject.getString(dataRow["AliasName"]);
						assocDO.EffectiveDate = DataObject.getValue<DateTimeOffset>(dataRow["EffectiveDate"], TimeConverter.Today());
						assocDO.ExpirationDate = DataObject.getValue<DateTimeOffset>(dataRow["ExpirationDate"], TimeConverter.Today());
						assocDO.Supplier = DataObject.getString(dataRow["SupplierID"]);
						assocDO.Owner = DataObject.getString(dataRow["OwnerID"]);
						assocDO.Manager = DataObject.getString(dataRow["ManagerID"]);
						assocDO.BillToID = DataObject.getString(dataRow["BillToID"]);
						assocDO.OriginStation = DataObject.getString(dataRow["OriginStation"]);
						assocDO.Product = DataObject.getString(dataRow["Product"]);

						this.associatedList.FuelOrderAssociatedTx.Add(assocDO);
					}
				}
			}
		}
	}
}