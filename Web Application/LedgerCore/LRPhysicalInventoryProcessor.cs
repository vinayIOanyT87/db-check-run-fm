namespace LedgerCore
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;

	public class LRPhysicalInventoryProcessor
	{
		#region Private data members
		private readonly LedgerConnection ledgerConnection;
		#endregion

		#region Contructors
		/// <summary>
		/// This is the default constructor for the Physical Inventory business layer.
		/// </summary>
		public LRPhysicalInventoryProcessor(LedgerConnection inLedgerConnection)
		{
			this.ledgerConnection = inLedgerConnection;
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method retrieves the starting date to retrieve ledger data for the all sites.
		/// Therefore, it get the oldest date of the group of sites.  In addition, it returns
		/// a Site Starting Point object that contains site index, site name, has physical
		/// inventory date flag, and the starting point for that site.
		/// </summary>
		/// <param name="beginDate"></param>
		/// <param name="productGuid"></param>
		/// <param name="managerGuid"></param>
		/// <param name="tankGuid"></param>
		/// <param name="siteList"></param>
		/// <param name="physicalInvSiteList"></param>
		/// <returns></returns>
		public DateTime GetMostRecentPhysicalInventoryDateAllSites( DateTime beginDate,
																	Guid productGuid,
																	Guid managerGuid,
																	Guid tankGuid,
																	List<LRSiteDO> siteList,
																	out List<LRSiteDO> physicalInvSiteList)
		{
			// Set end date 1 months from the begin date
			// Set end date 6 months from the begin date
			var endDates = new List<DateTime> { beginDate.AddMonths(-1), beginDate.AddMonths(-6) };

			using (var command = new SqlCommand())
			{
				var physicalInvDO = new LrPhysicalInventoryDO();

				foreach (DateTime endDate in endDates)
				{
					physicalInvDO = new LrPhysicalInventoryDO
					                {
						                ManagerGuid = managerGuid,
						                ProductGuid = productGuid,
						                TankGuid = tankGuid
					                };

					physicalInvDO.GetMostRecentPhysicalInventoryDateAllSitesSQL(command, siteList, beginDate, endDate);
					DataSet dataSet = this.ledgerConnection.GetDataSet(command);

					int siteCount = 0;
					foreach (LRSiteDO siteDo in siteList)
					{
						if (siteDo.SiteGroupFlag == false)
						{
							siteCount++;
						}
					}

					physicalInvDO.LoadMostRecentPhysicalInventoryDateAllSites(dataSet, siteCount, beginDate);

					if (physicalInvDO.AllSiteInventoryDatesFound)
					{
						break;
					}
				}

				physicalInvSiteList = physicalInvDO.SiteDOList;
				DateTime? physicalInvDate = physicalInvDO.GetStartingPointForAllSites();

				// If there was not a starting point for all sites, then
				// return the ledger begin date minus one day as the default.
				if (physicalInvDate == null)
				{
					return beginDate.AddDays(-1);
				}

				return physicalInvDate.Value;
			}
		}

		/// <summary>
		/// This method retrieves the starting date to retrieve ledger data for one site.
		/// In addition, it returns a Site Starting Point object that contains site index, 
		/// site name, has physical inventory date flag, and the starting point for that site.
		/// </summary>
		/// <param name="beginDate"></param>
		/// <param name="productGuid"></param>
		/// <param name="managerGuid"></param>
		/// <param name="tankGuid"></param>
		/// <param name="siteList"></param>
		/// <param name="physicalInvSiteList"></param>
		/// <returns></returns>
		public DateTime GetMostRecentPhysicalInventoryDateOneSite(	DateTime beginDate,
																	Guid productGuid,
																	Guid managerGuid,
																	Guid tankGuid,
																	List<LRSiteDO> siteList,
																	out List<LRSiteDO> physicalInvSiteList)
		{
			// Set end date 1 months from the begin date
			// Set end date 6 months from the begin date
			var endDates = new List<DateTime> { beginDate.AddMonths(-1), beginDate.AddMonths(-6) };

			using (var command = new SqlCommand())
			{
				var physicalInvDO = new LrPhysicalInventoryDO
				                    {
					                    ManagerGuid = managerGuid,
					                    ProductGuid = productGuid,
					                    SiteGuid = siteList[0].SiteGuid,
					                    TankGuid = tankGuid
				                    };

				foreach (DateTime endDate in endDates)
				{
					physicalInvDO.GetMostRecentPhysicalInventoryOneSiteSQL(command, beginDate, endDate);
					DataSet dataSet = this.ledgerConnection.GetDataSet(command);

					physicalInvDO.LoadMostRecentPhysicalInventoryDateOneSite(dataSet, beginDate);

					if (physicalInvDO.AllSiteInventoryDatesFound)
					{
						break;
					}
				}

				physicalInvSiteList = physicalInvDO.SiteDOList;
				physicalInvSiteList[0].SiteGuid = siteList[0].SiteGuid;
				DateTime? physicalInvDate = physicalInvDO.GetStartingPointForAllSites();

				// If there was not a starting point for all sites, then
				// return the ledger begin date minus one day as the default.
				if (physicalInvDate == null)
				{
					return beginDate.AddDays(-1);
				}

				return physicalInvDate.Value;
			}
		}

		/// <summary>
		/// This method will retrieve summed daily physical inventory quantities based on the 
		/// criterion passed into the method. It will return the gross and net quantities.
		/// </summary>
		/// <param name="siteList"></param>
		/// <param name="productGuid"></param>
		/// <param name="managerGuid"></param>
		/// <param name="tankGuid"></param>
		/// <param name="volumeFactor"></param>
		/// <param name="volumePrecision"></param>
		/// <param name="massFactor"></param>
		/// <param name="massPrecision"></param>
		/// <param name="systemEdition"></param>
		public void GetSummedPhysicalInventories(ref List<LRSiteDO> siteList,
												Guid productGuid,
												Guid managerGuid,
												Guid tankGuid,
												double volumeFactor,
												double volumePrecision,
												double massFactor,
												double massPrecision,
												LRLedgerProcessor.SystemEditions systemEdition)
		{
			using (var command = new SqlCommand())
			{
				var physicalInvDO = new LrPhysicalInventoryDO();
				physicalInvDO.GetSummedPhysicalInventoriesSQL(command);

				command.Parameters["@ProductGuid"].Value		= productGuid;
				command.Parameters["@ManagerCompanyGuid"].Value = managerGuid;
				command.Parameters["@VolumeFactor"].Value		= volumeFactor;
				command.Parameters["@VolumePrecision"].Value	= volumePrecision;
				command.Parameters["@MassFactor"].Value			= massFactor;
				command.Parameters["@MassPrecision"].Value		= massPrecision;
				command.Parameters["@TankGuid"].Value			= tankGuid;

				foreach (LRSiteDO siteDO in siteList)
				{
					siteDO.InitialBookInventory = new LRQuantityDO(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);

					// Only retrieve physical inventories for non-site group sites.
					if (siteDO.SiteGroupFlag == false)
					{
						bool getData = !(systemEdition == LRLedgerProcessor.SystemEditions.Bsme && (siteDO.PhysicalOnLastDay == false));

						// For BSME do not sum up the physical inventories unless the physical inventories are on the
						// last day of the month.
						if (getData)
						{
							physicalInvDO = new LrPhysicalInventoryDO();
							command.Parameters["@SiteGuid"].Value = siteDO.SiteGuid;
							command.Parameters["@InventoryDate"].Value = siteDO.PhysicalInvDateForLedgerStart;

							DataSet dataSet = this.ledgerConnection.GetDataSet(command);

							// Load the quantities
							physicalInvDO.LoadSummedPhysicalInventories(dataSet);
							siteDO.InitialBookInventory = physicalInvDO.Quantity;
						}
					}
				}
			}
		}
		#endregion
	}
}