namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;

    using FMBusinessServices.DataAccessLayer;

    public class InventoryDateProcessorClass : IInventoryDateProcessor
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public InventoryDateProcessorClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}

        #endregion
        /// <summary>
        /// Performs the actual processing of the request.  Processing in this case means determining the
        /// current Inventory Date for the requesting site.  The Inventory Date is the site's current day
        /// if:
        /// <list type="bullet">
        /// <item><description>the site inhibits automatic physical inventories</description></item>
        /// <item><description>the site inhibits end-of-day operations</description></item>
        /// <item><description>no prior end-of-day or end-of-month marker transaction can be found</description></item>
        /// </list>
        /// If none of the above conditions are met, then the current inventory date is based on the most recent
        /// end-of-day (EOD) or end-of-month(EOM) transaction.  If the last EOD or EOM transaction occured before 12:00 PM (site time), then
        /// the current inventory date is the same day as that last EOD or EOM transaction.  If the last EOD or EOM transaction happened
        /// at or after 12:00 PM (site time), then the current inventory date is the day after that of the last EOD/EOM.
        /// </summary>
        /// <param name="inventoryDateSR"></param>
        /// <returns></returns>
        public InventoryDateDO Process(InventoryDateSR inventoryDateSR)
		{
			if (inventoryDateSR == null)
			{
				throw new ArgumentNullException(nameof(inventoryDateSR));
			}

			InventoryDateDO inventoryDateDO = new InventoryDateDO();
			TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);

			Guid siteGuid = inventoryDateSR.CurrentSiteGuid;

			SitesClass sites = new SitesClass();
			SiteClass site = sites.Get(inventoryDateSR.Security, siteGuid, false, false, false);

			// If the site does not do automatic physical inventories or
			// if the site does not use End of Day, then the inventory date is the current day
			// of the current site.
			if (site.InhibitAutomaticPhysicalInventory || site.InhibitEndOfDayOperations)
			{
				inventoryDateDO.InventoryDate = TimeConverter.Today(site).Date;
				return inventoryDateDO;
			}

			// The site does not inhibit automatic physical inventory and
			// does not inhibit end of day ops.  Calculate the inventory date
			// The rule is:  calculate the current inventory date from the transaction date-time
			// of the last end of date OR end of month type transaction:  The inventory date is the same day as
			// the transaction date-time if the transaction date-time is AM; the inventory date is the day following the
			// transaction date-time if the transaction date-time is PM.  

            var sql = "SELECT TOP 1 InventoryDate, TransDateTime, AliasName FROM tblTransactions WITH(NOLOCK)"
                         + " WHERE SiteGuid = @siteGuid AND LookupTransTypeIndex IN (19,20) "
                         + " ORDER BY TransDateTime DESC";


			DataSet dataSet;
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = sql;
			    SqlParameter siteIndexParameter = new SqlParameter("@siteGuid", siteGuid)
			                                      {
			                                          SqlDbType =
			                                              SqlDbType.UniqueIdentifier
			                                      };
			    cmd.Parameters.Add(siteIndexParameter);
				dataSet = this.consolidatedDA.GetDataSet(cmd, inventoryDateSR.Security);
			}

			// Expect the dataset to have one table, having one row, having two columns.
			// The first column is the inventory date, the second column is the transaction date/time
			// If there is no row, then there is no available EOD/EOM transaction.  Use the site's current date in this case
			if (dataSet.Tables[0].Rows.Count == 0)
			{
				inventoryDateDO.InventoryDate = TimeConverter.Today(site).Date;
				return inventoryDateDO;
			}

			DataRow eodRow = dataSet.Tables[0].Rows[0];

			DateTimeOffset? lastEodTransactionDateTimeGmt = eodRow.Field<DateTimeOffset?>("TransDateTime");
			DateTime? lastEodInventoryDateTime = eodRow.Field<DateTime?>("InventoryDate");
			string lastEodAliasName = eodRow.Field<string>("AliasName");

            // calculate the current inventory date based on last EOD/EOM
			if (lastEodTransactionDateTimeGmt.HasValue)
			{
				var lastEodTransactionDateTimeLocal = TimeConverter.ToSiteTime(site, lastEodTransactionDateTimeGmt.Value);
				inventoryDateDO.InventoryDate = TimeConverter.ToDate(lastEodTransactionDateTimeLocal).Date;

				if (lastEodTransactionDateTimeLocal.Hour >= 12)
				{
					// Normally when the Manual EOD is initated after noon, the Inventory Date is equal to the Trans Date Time
					// If so, then the Current Inventory Date is the Trans Date Time date + 1 day
					// else the Current Inventory Date is the Trans Date Time date
					// 
					// This is done so the Inventory Date can be readily set to the Current Date in cases where
					// an EOD was inadvertangly missed and the current time is afternoon.
					if ((string.IsNullOrEmpty(lastEodAliasName)) ||
						(!lastEodInventoryDateTime.HasValue) ||
						(lastEodAliasName != "Manual End Of Day") ||
						(lastEodInventoryDateTime.Value.Date >= lastEodTransactionDateTimeLocal.Date))
					{
						inventoryDateDO.InventoryDate += oneDay;
					}
				}
			}
			else
			{
				inventoryDateDO.InventoryDate = TimeConverter.Today(site).Date;
			}

			return inventoryDateDO;
		}

	}

}
