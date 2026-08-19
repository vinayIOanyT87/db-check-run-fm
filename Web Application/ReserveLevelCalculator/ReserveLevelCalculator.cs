/******************************************************************************

	FILE NAME:		ReserveLevelCalculator.cs


	PURPOSE:		Calculates total volume of a give product and manager  


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2008

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec, Inc.


	AUTHOR(S):	A. Coker


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		09-20-2008	A. Coker	   Initial Revison to support ADF
 
      01-06-2009  A. Coker    Fixed bug 798. Added few more transaction types to include 
                                when checking reserve levels.In addition to
                                T5_PrimaryDisbursement, now also include T1_PrimaryAdjustment, T7_FillStand 
                                T13_OwnerTransfer,T14_PhysicalInventory , and T15_PrimaryRegrade.
  
      02-17-2009  A. Coker    Fix 1318. 
  
      02-20-2009  G. Kendall  WI#1663 - Added setting of warning messages to display on transaction detail
                                 when warnings/alarms are necessary for reserve levels.
 *		8-Apr-09		B. Schaal	7.5.0.0 - Modified to use the transaction inventory date instead of todays date for
 *													doing reserve level checks.

*******************************************************************************/
using System;

using FM7Accounting;
using FMCommon;
using ConsolidatedDataObjects;
using ConsolidatedBLL;

namespace ADFComponents
{
	/// <summary>
	/// Summary description for ReverseLevelInvoker.
	/// </summary>
	public class ReserveLevelCalculator
	{


		
		public  const int timeout		= 90;
		private readonly AccountingDA	dal = new AccountingDA();

		#region Attributes
		//private VolumeDO		totalVolume			= null;
		#endregion
      System.Collections.Hashtable products = new System.Collections.Hashtable();
	
		#region Constructor
		public ReserveLevelCalculator()
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Main method that calculates reserve levels and triggers alerts if below minimum and warning levels. 
		/// </summary>
		/// <param name="_security"></param>
		/// <param name="_trans"></param>
      public void AddProducts(SecurityClass security, TransactionDO trans)
      {

         if (trans != null && security != null)
         {

            //
            //	Calculate volume only for transactions that possibly affect inventory negatively.
            //
            if((trans.TransTypeID == ConsolidatedDataObjects.TransactionTypes.T1_PrimaryAdjustment ||
				trans.TransTypeID == ConsolidatedDataObjects.TransactionTypes.T2_SecondaryAdjustment ||
            trans.TransTypeID == ConsolidatedDataObjects.TransactionTypes.T5_PrimaryDisbursement ||
            trans.TransTypeID == ConsolidatedDataObjects.TransactionTypes.T6_SecondaryDisbursement ||
            trans.TransTypeID == ConsolidatedDataObjects.TransactionTypes.T13_OwnerTransfer ||
            trans.TransTypeID == ConsolidatedDataObjects.TransactionTypes.T14_PhysicalInventory ||
            trans.TransTypeID == ConsolidatedDataObjects.TransactionTypes.T15_PrimaryRegrade ||
            trans.TransTypeID == ConsolidatedDataObjects.TransactionTypes.T16_SecondaryRegrade ||
				trans.TransTypeID == ConsolidatedDataObjects.TransactionTypes.T25_Shipment))
            {
               //
               //	Get product
               //
               foreach (LineItemDO lineItem in trans.LineItems)
               {
                  string productID = lineItem.Product;

                  //Get issued volume and add to the total volume obtained so far from 
                  //transactions currently is process but not committed.
                  ProductClass product = (ProductClass)products[productID];
                  if (product == null)
                  {
                     ConsolidatedBLL.ProductsClass _products = new ProductsClass();
                     product = _products.Get(security, _products.GetIndex(security, productID), false);
                     products.Add(productID, product);
                  }
               }

            }

         }
      }

		public void CalculateVolume(SecurityClass security, SaveTransactionsResultDO resultsDO, DateTime inventoryDate)
      {
         foreach (ProductClass product in products.Values)
         {
				QuantityDO totalQuantity = this.retrieveTotalVolume(security,product.ID,inventoryDate);
            triggerAlarmAndEvent ( security, product, totalQuantity, resultsDO );
         }
      }

		#endregion


		#region Properties
      /*
		public VolumeDO TotalVolume 
		{
			get
			{
				return totalVolume;
			}
		}*/
		#endregion
      
		#region Private Methods

		/// <summary>
		/// Trigger event to send email if volume is below minimum and warning reserve levels.
		/// </summary>
		private void triggerAlarmAndEvent(SecurityClass security,ProductClass product,QuantityDO totalQuantity,SaveTransactionsResultDO resultsDO)
		{
         AccountingSite accountingSite =  new AccountingSite();
         accountingSite.loadSiteInfo(security, security.SiteIndex);
         AccountingUnitConversion converter = new AccountingUnitConversion(accountingSite.CurrentSite, product);			

			//Get minimum and warning reserve levels 
			ReserveLevelsClass reserveLevels = new ReserveLevelsClass();
			ReserveLevelClass reserveLevel = reserveLevels.Get(security, product.ID);

			//Check againsts warning and minimum reservel levels
			//Trigger alarm (send email) if below the specified levels.
			if(totalQuantity.Gross < reserveLevel.MinimumLevel)
			{
				double level = reserveLevel.MinimumLevel * converter.VolumeConversionFactorFromSI;
				AlarmAndEventLogsClass	AlarmAndEventLogs=new AlarmAndEventLogsClass();
				AlarmAndEventLogClass alarmAndEventLogClass = new AlarmAndEventLogClass(TransactionAlarmEventDO.ReserveLevelAlarmEventDescriptor);
				alarmAndEventLogClass.AssociatedData = "Reserve Level for product " + product.ID + " is below the minimum level of " + level;
				AlarmAndEventLogs.Add(security,alarmAndEventLogClass);

            // Register a message to display when we return to TransactionDetail
            // A warning message back to Transaction Detail is appropriate here since this is not an error that would
            // require the transaction save to be aborted.
            resultsDO.Results.Add( new TransactionValidationResult() { WarningList = {alarmAndEventLogClass.AssociatedData } } );

				AlarmAndEventLogs.Dispose();
			}
			else if(totalQuantity.Gross < reserveLevel.WarningLevel)
			{
				double level = reserveLevel.WarningLevel * converter.VolumeConversionFactorFromSI;
				AlarmAndEventLogsClass	AlarmAndEventLogs=new AlarmAndEventLogsClass();
             AlarmAndEventLogClass alarmAndEventLogClass = new AlarmAndEventLogClass(TransactionAlarmEventDO.ReserveLevelAlarmEventDescriptor);
             alarmAndEventLogClass.AssociatedData = "Reserve Level for product " + product.ID + " is below the warning level of " + level;

             // Register a message to display when we return to TransactionDetail
             resultsDO.Results.Add( new TransactionValidationResult() { WarningList = { alarmAndEventLogClass.AssociatedData } } );

             AlarmAndEventLogs.Add( security, alarmAndEventLogClass );
				AlarmAndEventLogs.Dispose();
			}
		}



		/// <summary>
		/// This method will retrieve the transaction volume information for a site or sites under a site group .
		/// </summary>
		/// <param name="product"></param>
		/// <param name="conversionFactor"></param>
		/// <param name="precision"></param>
		private QuantityDO retrieveTotalVolume(SecurityClass security,string productID,DateTime inventoryDate)
		{
         AccountingSite accountingSite =  new AccountingSite();
         accountingSite.loadSiteInfo(security, security.SiteIndex);
			QuantityDO totalQuantity = new QuantityDO(0.0,0.0,0.0,0.0);
         string msg = "";

         try
         {
            bool singleOwnerSystem = accountingSite.CurrentSite.EnforceSingleOwner;
				DateTime currentInventoryDate = inventoryDate;
            CompaniesClass companies = new CompaniesClass();
            CompanyCollectionClass Managers = companies.EnumerateByRoleGetIDCodeIndexOnly( security, COMPANY_ROLE.MANAGER );
            string managerID = "ADO";
            if (Managers.Count == 1)
            {
               managerID = Managers[0].ID;
            }
            // Sum volumes from each site in the site list.
            foreach (Site site in accountingSite.SiteList)
            {


               //Get volume from subsequent transactions affecting volume levels in primary storages.
               string sql = "EXEC fm_GetTotalProductVolume " + singleOwnerSystem + ", " + currentInventoryDate.ToString("\\{\\d\\ \\'yyyy\\-MM\\-dd\\'\\}") + ", '" +
                  site.Name + "', '" + productID + "', '" +
                  managerID + "', " + security.LoginSiteIndex + ", " +
                  security.SiteIndex + ", " + security.UserIndex;
               msg = sql;
               System.Data.DataSet dataSet = dal.GetDataSet(security, sql, timeout);
               msg = "After sql execute.";

               if (dataSet != null)
               {
                  System.Data.DataTable table = dataSet.Tables[0];

                  if (table.Rows.Count > 0)
                  {
                     System.Data.DataRow row;
                     row = table.Rows[0];

                     double grossQuantity = DataObject.getDouble(row[0]);
                     double netQuantity = DataObject.getDouble(row[1]);

							totalQuantity.GrossInventoryChange += grossQuantity;
							totalQuantity.NetInventoryChange += netQuantity;

                  }
               }
            }
         }
         catch(Exception e)
         {
            string emsg = "[" + e.Message + "] : " + msg ;
            throw new Exception(emsg);
         }
			return totalQuantity;
		}


		#endregion


	}
}
