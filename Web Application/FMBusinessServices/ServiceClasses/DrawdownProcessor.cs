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
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class DrawdownProcessorClass : IDrawdownProcessor
	{
		#region Private data members
		private DrawdownDO drawdownDO;
		private DrawdownSR drawdownSR;
		private TransactionHierarchyUtil util;
		private AccountingSite accountingSite;
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public DrawdownProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public DrawdownDO Process(DrawdownSR inDrawdownSR)
		{
			this.drawdownSR = inDrawdownSR;
			this.drawdownDO = new DrawdownDO ( );

			// See if the line item is part of any hierarchy
			this.util = new TransactionHierarchyUtil ( this.drawdownSR.Security );
			DataSet dataSet = util.GetHierarchy ( drawdownSR.LineItem.TransactionLineItemGuid );

			if (( dataSet == null ) || ( dataSet.Tables == null ) || ( dataSet.Tables.Count < 2 ))
			{
				return drawdownDO;
			}

			if ((dataSet.Tables["Parents"].Rows.Count == 0) &&
				(dataSet.Tables["Children"].Rows.Count == 0))
			{
				return drawdownDO;
			}

			double parentQty	= 0;
			double parentValue	= 0;
			bool performChecks	= false;
			bool checkTolerance = false;

			AccountingSites accountingSites = new AccountingSites ( );
			accountingSite = accountingSites.LoadSiteInfo(drawdownSR.Security, drawdownSR.Security.SiteGuid);

			// If the line item passed belongs to a transaction whose
			// alias (supply order) has drawdown warnings enabled, the line item is
			// considered to be the top of the hierarchy.  In this
			// case only the children of the line item are considered
			if (drawdownSR.Alias.EnableQtyToleranceExceededWarning ||
				drawdownSR.Alias.EnableTotalQtyExceededWarning ||
				drawdownSR.Alias.EnableTotalValueExceededWarning ||
				drawdownSR.Alias.EnableValueToleranceExceededWarning)
			{
				checkTolerance = ( drawdownSR.Alias.EnableQtyToleranceExceededWarning || drawdownSR.Alias.EnableValueToleranceExceededWarning );

				// This is the parent supply order so calculate values
				this.util.CalculateValues ( dataSet.Tables["Children"], accountingSite );
				parentQty = drawdownSR.LineItem.Quantity.Gross;
				parentValue = drawdownSR.LineItem.ProductPrice.Value * parentQty;
				performChecks = true;
			}
			else
			{
				// Search the line item's parents for a supply order
				// with warnings enabled
				foreach (DataRow row in dataSet.Tables["Parents"].Rows)
				{
					if (Convert.ToInt32(row["LookupTransTypeIndex"]) != (int)TransactionTypes.T18_SupplyOrder)
					{
						continue;
					}

					// Check for warnings
					if (	DataObject.getValue<bool>(row["CheckQuantity"], false) ||
							DataObject.getValue<bool>(row["CheckValue"], false) ||
							DataObject.getValue<bool>(row["CheckQtyTolerance"], false) ||
							DataObject.getValue<bool>(row["CheckValueTolerance"], false) )
					{
						checkTolerance =
								DataObject.getValue<bool>(row["CheckQtyTolerance"], false) ||
								DataObject.getValue<bool>(row["CheckValueTolerance"], false);

						parentQty = DataObject.getValue<double>(row["GrossQuantity"], 0.0);

						// The value for qty in this case is pulled directly from the database
						// so it must be converted from SI units
						parentQty = accountingSite.ConvertFromSi ( parentQty, AccountingSite.ConversionUnits.VOLUME );
						parentValue = DataObject.getValue<double>(row["ProductPrice"], 0.0) * parentQty;

						// The parent supply order has been found so calculate values
						DataSet childDS = util.GetHierarchy((Guid)row["ParentTransactionLineItemGuid"]);
						this.util.CalculateValues ( childDS.Tables["Children"], accountingSite );
						performChecks = true;
						break;
					}
				}
			}

			if (performChecks)
			{
				// Check the total quantity/value
				drawdownDO.QuantityLimitExceeded = ( this.util.TotalReceived > parentQty );
				drawdownDO.ValueLimitExceeded = ( this.util.TotalValueReceived > parentValue );

				// See if tolerance needs to be calculated
				if (checkTolerance)
				{
					// Get the tolerance from the DB
					double tolerance = this.GetTolerance ( );

					if (tolerance == 0)
					{
						drawdownDO.QuantityToleranceExceeded = drawdownDO.QuantityLimitExceeded;
						drawdownDO.ValueToleranceExceeded = drawdownDO.ValueLimitExceeded;
					}
					else
					{
						double toleranceQty = ( 1 - tolerance ) * parentQty;
						double toleranceValue = ( 1 - tolerance ) * parentValue;

						drawdownDO.QuantityToleranceExceeded = ( this.util.TotalReceived > toleranceQty );
						drawdownDO.ValueToleranceExceeded = ( this.util.TotalValueReceived > toleranceValue );
					}
				}
			}

			return drawdownDO;
		}

		private double GetTolerance ( )
		{
			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT UserData2 " +
							 "FROM  tblSites " +
							 "WHERE SiteGuid = @SiteGuid";

				cmd.Parameters.Add(new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier));
				cmd.Parameters["@SiteGuid"].Value = drawdownSR.Security.SiteGuid;

				dataSet = this.consolidatedDA.GetDataSet(cmd, drawdownSR.Security);
			} 

			double tolerance = 0;

			if (( dataSet != null ) && ( dataSet.Tables != null ) && ( dataSet.Tables.Count > 0 ))
			{
				DataTable dataTable = dataSet.Tables[0];

				if (( dataTable.Rows != null ) && ( dataTable.Rows.Count > 0 ))
				{
					string sTolerance = dataTable.Rows[0][0].ToString ( );

					if (( string.IsNullOrEmpty(sTolerance) == false) && (sTolerance.Trim ( ).Length > 0))
					{
						// The assumption is that the value stored is a percentage so convert it
						// to a decimal
						tolerance = Convert.ToDouble ( sTolerance );
						tolerance = tolerance / 100;
					}
				}
			}

			return tolerance;
		}
	}
}