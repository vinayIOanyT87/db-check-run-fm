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
using FMBusinessObjects.LogClient;

namespace FMBusinessServices.ServiceClasses
{
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class SaveWeightedAverageCostsProcessorClass : ISaveWeightedAverageCostsProcessor
	{
		#region Attributes
		protected Logger logger;
		protected WeightedAverageCostDBI wacDBI;
		protected static Object wacLock = new Object ( );
		private ConsolidatedDAClass consolidatedDA;
		#endregion // Attributes

		public SaveWeightedAverageCostsProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
			this.logger = new Logger ( "Save Weighted Average Costs Processor" );
		}

		#region Overrides
		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public CustomResultDO Process ( SaveWeightedAverageCostsSR sr )
		{
			StopWatch timer = new StopWatch ( StopWatch.Appnames.AccountingBLL, "***###*** SaveWeightedAverageCosts() Main Call" );
			CustomResultDO results = this.SaveWeightedAverageCosts ( sr );
			timer.Stop ( );

			// write any errors or warnings to the application event log
			timer.Start ( "***###*** WriteLog() Main Call (errors)" );
			CustomResultDO.WriteLog ( "Accounting BLL", results.Errors, EventLogEntryType.Error );
			timer.Stop ( );

			timer.Start ( "***###*** WriteLog() Main Call (warnings)" );
			CustomResultDO.WriteLog ( "Accounting BLL", results.Warnings, EventLogEntryType.Warning );
			timer.Stop ( );

			// if there has been errors, throw them
			if (results.Errors.Count > 0)
			{
				throw new AccountingServicesException ( "errors in " + MethodBase.GetCurrentMethod ( ).ToString ( ) + " check event log for more details" );
			}

			return results;
		}
		#endregion // Overrides

		#region Protected operations
		protected CustomResultDO SaveWeightedAverageCosts ( SaveWeightedAverageCostsSR sr )
		{
			CustomResultDO result = new CustomResultDO ( );

			lock (wacLock)
			{
				using (WeightedAverageCostDBI wacDBI = new WeightedAverageCostDBI(sr.Security, sr.Security.UserID, DateTimeOffset.Now))
				{
					result.SavedCount = 0;

					try
					{
						foreach (WeightedAverageCostDO wac in sr.WeightedAverageCosts)
						{
							StopWatch timer = new StopWatch(StopWatch.Appnames.AccountingBLL, "### WeightedAverageCostDBI.Save()");

							bool ok = this.ValidateWAC(ref result, wac);

							if (ok)
							{
								wacDBI.Save(wac);
								timer.Stop();

								// increase count on success
								result.SavedCount = result.SavedCount + 1;
							}
						}
					}
					catch (Exception e)
					{
						result.Errors.Add(new AccountingServicesException(e.Message));
					}
				}
			}
			return result;
		}
		#endregion // Protected operations

		#region Public utilities
		public bool ValidateWAC ( ref CustomResultDO results, WeightedAverageCostDO wac )
		{
			bool returnVal = false;

			if (wac.WacValue < 0)
			{
				results.Warnings.Add ( new AccountingServicesException ( "This WAC has an initial WAC value smaller than 0" ) );
			}
			else if (wac.CreatedBy.Length >= 64)
			{
				results.Warnings.Add ( new AccountingServicesException ( "The maximum username length is 65 characters" ) );
			}
			else if (wac.Notes.Length >= 2047)
			{
				results.Warnings.Add ( new AccountingServicesException ( "The maximum notes length is 2047 characters" ) );
			}
			else
			{
				returnVal = true;
			}

			return returnVal;
		}

		public bool QualityWasNotUsable ( TransactionDO trans, LineItemDO lineItem )
		{
			bool returnVal = false;

			// find the line item
			foreach (LineItemDO li in trans.LineItems)
			{
				if (li.TransactionLineItemGuid == lineItem.TransactionLineItemGuid && li.Quality != TransactionQuality.Usable)
				{
					returnVal = true;
					break;
				}
			}

			return returnVal;
		}

		public bool ShouldWacUpdate ( TransactionDO trans, LineItemDO lineItem, TransactionDO origTrans )
		{
			// wac should never update for orders, demands and recoveries
			if (trans.TransTypeID == TransactionTypes.T9_Request ||
				trans.TransTypeID == TransactionTypes.T18_SupplyOrder ||
				trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
			{
				return false;
			}

			double qtyChange = this.QuantityChangedSinceLastSaveWithOrigTrans ( trans, lineItem, origTrans );

			if (qtyChange != 0.0 || 
				( lineItem.WacCalculated && 
				  trans.TransTypeID == TransactionTypes.T8_Receipt && 
				  lineItem.Quality == TransactionQuality.Usable && 
				  QualityWasNotUsable ( origTrans, lineItem ) ) )
			{
				return true;
			}

			bool returnVal = false;

			if (qtyChange != lineItem.Quantity.Gross && qtyChange != 0)
			{
				// means a change to existing inventory, need to check if latest WAC is different to current for inventory
				// transactions
				switch (trans.TransTypeID)
				{
					case TransactionTypes.T5_PrimaryDisbursement:
					case TransactionTypes.T6_SecondaryDisbursement:
					case TransactionTypes.T3_PrimaryDefuel:
					case TransactionTypes.T4_SecondaryDefuel:
					case TransactionTypes.T1_PrimaryAdjustment:
					case TransactionTypes.T2_SecondaryAdjustment:
					case TransactionTypes.T14_PhysicalInventory:
					case TransactionTypes.T25_Shipment:
						// find the line item and see if WAC is different
						if (origTrans != null)
						{
							foreach (LineItemDO li in origTrans.LineItems)
							{
								if (li.TransactionLineItemGuid == lineItem.TransactionLineItemGuid)
								{
									returnVal = li.Tax4 != lineItem.Tax4;
								}
							}
						}
						break;
				}
			}

			if (!returnVal)
			{
				switch (trans.TransTypeID)
				{
					case TransactionTypes.T8_Receipt:
						{
							// only if usable
							returnVal = lineItem.Quality == TransactionQuality.Usable && qtyChange != 0.0;
							break;
						}
					case TransactionTypes.T15_PrimaryRegrade:
						{
							returnVal = qtyChange != 0.0;
							break;
						}
				}
			}

			if (!returnVal &&
				( trans.ReversalType == TransactionDO.ReversalWithUpdate ||
				  trans.ReversalType == TransactionDO.Reversal ||
				  trans.ReversalType == TransactionDO.Update ))
			{
				returnVal = lineItem.DeleteFlag && lineItem.Quantity.GrossInventoryChange != 0;

				if (!returnVal)
				{
					returnVal = lineItem.WacCalculated;
				}
			}

			return returnVal;
		}

		public double QuantityChangedSinceLastSave ( TransactionDO trans, LineItemDO lineItem )
		{
			return this.QuantityChangedSinceLastSaveWithOrigTrans ( trans, lineItem, null );
		}

		// when a_origTrans is null, will load from database
		public double QuantityChangedSinceLastSaveWithOrigTrans ( TransactionDO trans, LineItemDO lineItem, TransactionDO origTrans )
		{
			//double returnVal = a_lineItem.Volume.Gross;
			double returnVal = lineItem.Quantity.NetInventoryChange;

			if (0 == trans.TransVersion) // 0 transaction version means new transaction
			{
				// new item is always considered as changed
				return returnVal;
			}

			// load the current line item from the database
			string transID = trans.TransID;

			SecurityClass security = new SecurityClass ( );

			// the necessary security rights needed to get the transaction
			security.AddRight( RIGHT.VIEW_TRANSACTION_DATA );
			security.AddRight( RIGHT.MODIFY_TRANSACTION_DATA );

			// setup security for accessing the information we need
			security.UserGuid = Guid.Empty;

			TransactionDO currentTrans = origTrans;

			if (null == currentTrans)
			{
				TransactionSR currentSR = new TransactionSR ( );
				currentSR.Security = security;
				currentSR.TransID = transID;

				try
				{
					TransactionProcessorClass transProcessor = new TransactionProcessorClass ( );
					currentTrans = transProcessor.Process ( currentSR );
				}
				catch (Exception)
				{
					currentTrans = null;
				}
			}

			// find the current line item in the retrieved transaction
			if (currentTrans != null)
			{
				foreach (LineItemDO li in currentTrans.LineItems)
				{
					if (li.TransactionLineItemGuid == lineItem.TransactionLineItemGuid)
					{
						returnVal = ( lineItem.Quantity.Net - li.Quantity.Net );
						break;
					}
				}
			}

			return returnVal;
		}
		#endregion
	}
}