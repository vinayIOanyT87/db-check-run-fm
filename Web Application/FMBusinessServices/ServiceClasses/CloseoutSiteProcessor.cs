/// <summary>
/// File name:	CloseoutSiteProcessor.cs
/// Purpose:	The purpose of this processor is to perform closeouts for each
///				site and the managers and products associated to the site.  It
///				calls the Closeout Processor to perform the closeout.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Thomas Beckum
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		2006/07/18		Richard Panachida		For CSI 3036 (db transactions) ensure
///														the connection was close prior to calling
///														the closeout processor.
///      2010-02-15		W.Gray					Revised TransID to TransIndex in ancillary transaction tables (WI 11422)
/// </summary>
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
using FMBusinessObjects.UtilityObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;
using FMBusinessObjects.LogClient;

namespace FMBusinessServices.ServiceClasses
{
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class CloseoutSiteProcessorClass : ICloseoutSiteProcessor
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		private Logger logger;
		#endregion

		#region Contructors
		public CloseoutSiteProcessorClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
			this.logger = new Logger("CloseoutSiteProcessorClass");
		}
		#endregion

		#region Public methods
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public DataObject Process(CloseoutSiteSR closeoutSiteSR)
		{
			CloseoutSiteSR sr = closeoutSiteSR;

			SitesClass sites = new SitesClass();
			SiteClass site = sites.GetUsingGuid(sr.Security, sr.Security.SiteGuid);

			DateTime convertedDate = TimeConverter.Today(site).Date;
			DateTime closeoutDate = convertedDate.AddMonths(-site._OpenTransactionWindow);
			closeoutDate = closeoutDate.AddDays(-closeoutDate.Day);

			CloseoutSR closeoutSR = new CloseoutSR();
			closeoutSR.Security = sr.Security;
			closeoutSR.Site = sr.Security.SiteID;

			ProductsClass products = new ProductsClass();
			ProductCollectionClass productCollection = products.Enumerate(sr.Security);

			CompaniesClass companies = new CompaniesClass();
			CompanyCollectionClass managerCollection = companies.EnumerateByRole(sr.Security, COMPANY_ROLE.MANAGER, false);

			DataSet dataSet = null;
			DateTime firstTransactionDate;

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = this.GetMinInventoryDateSQL();
				firstTransactionDate = closeoutDate;
				dataSet = this.consolidatedDA.GetDataSet(cmd, sr.Security);
			}

			if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
			{
				DataTable dataTable = dataSet.Tables[0];

				if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
				{
					DataRow row = dataTable.Rows[0];

					firstTransactionDate = DataObject.getValue<DateTime>(row["InventoryDate"], closeoutDate);
				}
			}

			//For each manager, for each product, calculate the ledger and then create the closeout record.
			foreach (CompanyClass manager in managerCollection)
			{
				foreach (ProductClass product in productCollection)
				{
					//Find the previous closeout record, if any exists.
					CloseoutListSR listSR = new CloseoutListSR();
					listSR.ConvertUnits = false;
					listSR.CurrentSiteGuid = sr.Security.SiteGuid;
					listSR.EndDate = closeoutDate;
					listSR.ManagerGuid = companies.GetMasterRecordGuid(sr.Security, manager.ID);
					listSR.ProductGuid = products.GetMasterRecordGuidFromID(sr.Security, product.ID);
					listSR.Security = closeoutSR.Security;
					listSR.Site = sr.Security.SiteID;

					CloseoutListProcessorClass proc = new CloseoutListProcessorClass();
					CloseoutListDO listDO = proc.Process(listSR);
					DateTime firstMonthToCloseout = firstTransactionDate;

					if ((listDO != null) && (listDO.PriorCloseout != null))
					{
						firstMonthToCloseout = CloseoutSiteProcessorClass.GetNextEndOfMonth(listDO.PriorCloseout.CloseoutDate);
					}
					else
					//If no prior closeout exists, find the earliest Inventory Date of the transactions.
					{
						firstMonthToCloseout = firstMonthToCloseout.AddDays(
								DateTime.DaysInMonth(firstMonthToCloseout.Year, firstMonthToCloseout.Month) - firstMonthToCloseout.Day);
					}

					for (DateTime closeoutMonth = firstMonthToCloseout;
						closeoutMonth <= closeoutDate;
						closeoutMonth = CloseoutSiteProcessorClass.GetNextEndOfMonth(closeoutMonth))
					{
						this.logger.Info("Site: " + sr.Security.SiteID + "  Manager: "
								 + manager.ID + "  Product: " + product.ID + "  CloseoutDate: " + closeoutMonth.ToString("d"));

						//Create the closeout record.
						closeoutSR.ManagerCode = manager.Code;
						closeoutSR.ManagerName = manager.ID;
						closeoutSR.ProductCode = product.Code;
						closeoutSR.ProductName = product.ID;
						closeoutSR.InventoryDate = closeoutMonth;

						CloseoutProcessorClass cProc = new CloseoutProcessorClass();

						closeoutSR.CloseoutCommand = CloseoutSR.CloseoutType.CALCULATE;
						CloseoutDO closeoutRecord = cProc.Process(closeoutSR);

						if (closeoutRecord != null)
						{
							closeoutSR.CloseoutCommand = CloseoutSR.CloseoutType.CREATE;
							closeoutSR.Closeout = closeoutRecord;
							cProc.Process(closeoutSR);
						}
					}
				}
			}

			return null;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void ProcessForSite(CloseoutSiteSR closeoutSiteSR)
		{
            // Get the site so we can convert today's date to site time.
			SitesClass sites = new SitesClass();
            SiteClass site = sites.Get(closeoutSiteSR.Security, closeoutSiteSR.Security.SiteGuid, false, false, false);

            // Get today's date in site time. The closeout date we'll use is the end of the month before the current month - the site's open transaction window setting.
            DateTime convertedDate = TimeConverter.Today(site).Date;
            DateTime closeoutDate = convertedDate.AddMonths(-site._OpenTransactionWindow);
            closeoutDate = closeoutDate.AddDays(-closeoutDate.Day);

            // Get the products assigned to or owned by the site.
		    ProductsClass products = new ProductsClass();
            ProductCollectionClass productCollection = products.Enumerate(closeoutSiteSR.Security);

            // Get the managers assigned to or owned by the site.
			CompaniesClass companies = new CompaniesClass();
            CompanyCollectionClass managerCollection = companies.EnumerateByRoleGetIDCodeTypesIdentityGuidOnly(closeoutSiteSR.Security, new[] { COMPANY_ROLE.MANAGER });

			// For each manager, for each product, calculate the ledger and then create the closeout record.
			foreach (CompanyClass manager in managerCollection)
			{
				foreach (ProductClass product in productCollection)
				{
                    // If the product is not configured to automatically closeout, skip it.
                    if (!product.AutomaticCloseout)
                    {
                        continue;
                    }

				    this.ProcessForSiteManagerAndProduct(closeoutSiteSR, manager, product, closeoutDate, site.ShortDatePattern);
				}
			}
		}

        /// <summary>
        /// Create closeouts for the provided product, manager, and site
        /// </summary>
        /// <param name="closeoutSiteSR">Contains the site to process closeouts for as well as security information</param>
        /// <param name="manager">The manager to process closeouts for</param>
        /// <param name="product">The product to process closeouts for</param>
        /// <param name="closeoutDate">The date to closeout. Closeouts will be created for each end of month between the previous closeout and this date.</param>       
        /// <param name="siteShortDatePattern">The site's short date pattern. This will be used if we have to create an alarm and event record
        /// when there are no physical inventory transactions</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void ProcessForSiteManagerAndProduct(CloseoutSiteSR closeoutSiteSR, CompanyClass manager, ProductClass product, DateTime closeoutDate, string siteShortDatePattern)
        {
            CloseoutSR closeoutSR = new CloseoutSR { Security = closeoutSiteSR.Security, Site = closeoutSiteSR.Security.SiteID, CurrentSiteGuid = closeoutSiteSR.Security.SiteGuid };

            // Find the previous closeout record, if any exists.
            CloseoutListSR listSR = new CloseoutListSR
            {
                ConvertUnits = false,
                CurrentSiteGuid = closeoutSiteSR.Security.SiteGuid,
                EndDate = closeoutDate,
                ManagerGuid = manager.MasterRecordGuid,
                ProductGuid = product.MasterRecordGuid,
                Security = closeoutSR.Security,
                Site = closeoutSiteSR.Security.SiteID
            };

            // Get the current, previous, and subsequent closeouts for the closeout date.
            CloseoutListProcessorClass proc = new CloseoutListProcessorClass();
            CloseoutListDO listDO = proc.Process(listSR);
            DateTime? firstMonthToCloseout;

            if (listDO != null && listDO.PriorCloseout != null)
            {
                // If there is a prior closeout, the first month we will try to closeout is the month after the prior closeout
                firstMonthToCloseout = GetNextEndOfMonth(listDO.PriorCloseout.CloseoutDate);
            }
            else
            {
                // If no prior closeout exists, the first month we need to closeout is the end of the month of the earliest transaction inventory date
                firstMonthToCloseout = this.GetMinimumInventoryDate(
                    closeoutSR.Security,
                    closeoutSiteSR.Security.SiteGuid,
                    manager.MasterRecordGuid,
                    product.MasterRecordGuid);

                // If there are no transactions for this site, manager, and product, there's no point in closing out.
                if (!firstMonthToCloseout.HasValue)
                {
                    return;
                }

                firstMonthToCloseout = firstMonthToCloseout.Value.AddDays(
                        DateTime.DaysInMonth(firstMonthToCloseout.Value.Year, firstMonthToCloseout.Value.Month) - firstMonthToCloseout.Value.Day);
            }

            closeoutSR.ManagerCode = manager.Code;
            closeoutSR.ManagerName = manager.ID;
            closeoutSR.ManagerCompanyGuid = manager.MasterRecordGuid;
            closeoutSR.ProductCode = product.Code;
            closeoutSR.ProductName = product.ID;
            closeoutSR.ProductGuid = product.MasterRecordGuid;

            // Create the closeout for each end of month between the first month to closeout and the closeout date we determined earlier.
            for (DateTime closeoutMonth = firstMonthToCloseout.Value;
                closeoutMonth <= closeoutDate;
                closeoutMonth = GetNextEndOfMonth(closeoutMonth))
            {
                this.logger.Info("Site: " + closeoutSiteSR.Security.SiteID + "  Manager: "
                         + manager.ID + "  Product: " + product.ID + "  CloseoutDate: " + closeoutMonth.ToString("d"));

                closeoutSR.InventoryDate = closeoutMonth;

                CloseoutProcessorClass closeoutProcessor = new CloseoutProcessorClass();

                // Calculate the ledger for the month we're closing out.
                closeoutSR.CloseoutCommand = CloseoutSR.CloseoutType.CALCULATE;
                CloseoutDO closeoutRecord = closeoutProcessor.Process(closeoutSR);

                // Create the records in tblOwnerCloseout and tblCloseoutInventory
                if (closeoutRecord != null)
                {
                    closeoutSR.CloseoutCommand = CloseoutSR.CloseoutType.CREATE;
                    closeoutRecord.ManagerGuid = closeoutSR.ManagerCompanyGuid;
                    closeoutRecord.SiteGuid = closeoutSR.CurrentSiteGuid;
                    closeoutRecord.ProductGuid = closeoutSR.ProductGuid;

                    closeoutSR.Closeout = closeoutRecord;

                    closeoutProcessor.Process(closeoutSR);
                }
                else if (closeoutProcessor.NoPhysicalInventories)
                {
                    // For aviation, we want to create an alarm and event log record when there were no physical inventory transactions
                    var alarmAndEventLog = new AlarmAndEventLogClass(TransactionAlarmEventDO.AutomaticCloseoutNoPhysicalInventoryEventDescriptor)
                    {
                        AssociatedData =
                            $"Manager: {manager.ID} Product: {product.ID} Date: {closeoutSR.InventoryDate.Date.ToString(siteShortDatePattern)}"
                    };

                    var alarmAndEventLogs = new AlarmAndEventLogsClass();
                    alarmAndEventLogs.Add(closeoutSiteSR.Security, alarmAndEventLog);
                }
            }         
        }
		#endregion

		#region Protected methods
		protected string GetMinInventoryDateSQL()
		{
			string sql = "SELECT MIN(InventoryDate) AS InventoryDate FROM tblTransactions";
			return sql;
		}

		protected DateTimeOffset GetMinInventoryDate(DateTimeOffset startDate,
													string managerID,
													string siteID,
													string productID,
													DateTimeOffset closeoutDate,
													SecurityClass security)
		{
			string sql =
				"SELECT MIN(InventoryDate) AS InventoryDate " +
				"FROM tblTransactions a " +
				"WHERE InventoryDate >= @BeginDate AND InventoryDate <= @EndDate AND " +
				"      site = @Site AND managerid = @Manager AND " +
				"      (TransactionGuid IN " +
				"        (SELECT TransactionGuid FROM tblTransactionLineItems " +
				"         WHERE Product = @Product AND TransactionInventoryDate >= @BeginDate AND TransactionInventoryDate <= @EndDate " +
				"         ) " +
				"      OR " +
				"       TransactionGuid IN " +
				"        (SELECT TransactionGuid FROM tblTransactionSubLineItems " +
				"         WHERE Product = @Product AND TransactionInventoryDate >= @BeginDate AND TransactionInventoryDate <= @EndDate " +
				"         ) " +
				"      )";

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = sql;
				cmd.Parameters.Add("@BeginDate", System.Data.SqlDbType.Date);
				cmd.Parameters.Add("@EndDate", System.Data.SqlDbType.Date);
				cmd.Parameters.Add("@Site", System.Data.SqlDbType.NVarChar, 30);
				cmd.Parameters.Add("@Manager", System.Data.SqlDbType.NVarChar, 100);
				cmd.Parameters.Add("@Product", System.Data.SqlDbType.NVarChar, 30);
				cmd.Prepare();

				DateTimeOffset beginDate = startDate.AddDays(1 - startDate.Day);

				while (beginDate <= closeoutDate)
				{
					DateTimeOffset endDate = CloseoutSiteProcessorClass.GetEndOfMonth(beginDate);

					cmd.Parameters["@BeginDate"].Value = beginDate.Date;
					cmd.Parameters["@EndDate"].Value = endDate.Date;
					cmd.Parameters["@Site"].Value = siteID;
					cmd.Parameters["@Manager"].Value = managerID;
					cmd.Parameters["@Product"].Value = productID;

					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);
					beginDate = endDate.AddDays(1);

					if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
					{
						DataTable dataTable = dataSet.Tables[0];

						if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
						{
							DataRow row = dataTable.Rows[0];
							DateTimeOffset minInvDate = DataObject.getValue<DateTimeOffset>(row["InventoryDate"], endDate.AddDays(1));
						}
					}

					return beginDate;
				}
			}

			return closeoutDate.AddDays(1 - closeoutDate.Day);
		}
		#endregion

        /// <summary>
        /// Determine the earliest inventory date of a transaction matching the specified site, manager, and product.
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="siteGuid">The site to match on</param>
        /// <param name="managerGuid">The manager to match on</param>
        /// <param name="productGuid">The product to match on</param>
        /// <returns>The earliest inventory date of a transaction matching the search parameters, or null if none exists</returns>
	    private DateTime? GetMinimumInventoryDate(SecurityClass security, Guid siteGuid, Guid managerGuid, Guid productGuid)
	    {
	        // Get the earliest inventory date from the transaction table.
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_TransactionsGetEarliestInventoryDate";

                cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = siteGuid;
                cmd.Parameters.Add("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier).Value = managerGuid;
                cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier).Value = productGuid;

                var dataSet = this.consolidatedDA.GetDataSet(cmd, security);

                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    DataTable dataTable = dataSet.Tables[0];

                    if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                    {
                        DataRow row = dataTable.Rows[0];

                        return DataObject.getValue<DateTime?>(row["InventoryDate"], null);
                    }
                }
            }

	        return null;
	    }

		#region Public methods
		public static DateTimeOffset GetEndOfMonth(DateTimeOffset currentDate)
		{
			DateTimeOffset tempDate = currentDate;
			tempDate = tempDate.AddMonths(1); //Skip forward a month
			tempDate = tempDate.AddDays(1 - tempDate.Day); //Set to first of the month
			tempDate = tempDate.AddDays(-1); //Skip back 1 day to last day of previous month.

			return tempDate;
		}

		public static DateTime GetNextEndOfMonth(DateTime currentDate)
		{
			DateTime tempDate = currentDate;
			tempDate = tempDate.AddDays(1);
			tempDate = tempDate.AddMonths(1);
			tempDate = tempDate.AddDays(1 - tempDate.Day);
			tempDate = tempDate.AddDays(-1);

			return tempDate;
		}
		#endregion
	}
}
