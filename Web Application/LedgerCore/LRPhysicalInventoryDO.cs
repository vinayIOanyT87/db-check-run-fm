namespace LedgerCore
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;

	public class LrPhysicalInventoryDO
	{
		#region Private data members
		private DateTime? mostRecentDate;
		private bool hasPhysicalInvDate;
		private LRQuantityDO quantity;
		private List<LRSiteDO> siteDOList;
		private bool allSiteInventoryDatesFound;
		private Guid managerGuid;
		private Guid productGuid;
		private Guid siteGuid;
		private Guid tankGuid;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default contructor for the Physical Inventory data object.
		/// </summary>
		public LrPhysicalInventoryDO()
		{
			this.Reset();
		}
		#endregion

		#region Properties
		public DateTime? MostRecentDate => this.mostRecentDate;

	    public bool HasPhysicalInvDate => this.hasPhysicalInvDate;

	    public LRQuantityDO Quantity => this.quantity;

	    public List<LRSiteDO> SiteDOList => this.siteDOList;

	    public bool AllSiteInventoryDatesFound => this.allSiteInventoryDatesFound;

	    public Guid ManagerGuid
		{
			get { return this.managerGuid; }
			set { this.managerGuid = value; }
		}

		public Guid ProductGuid
		{
			get { return this.productGuid; }
			set { this.productGuid = value; }
		}

		public Guid TankGuid
		{
			get { return this.tankGuid; }
			set { this.tankGuid = value; }
		}

		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		public void Reset()
		{
			this.hasPhysicalInvDate			= false;
			this.mostRecentDate				= null;
			this.quantity					= null;
			this.siteDOList					= new List<LRSiteDO>();
			this.allSiteInventoryDatesFound = false;
			this.managerGuid				= Guid.Empty;
			this.productGuid				= Guid.Empty;
			this.tankGuid					= Guid.Empty;
			this.siteGuid					= Guid.Empty;
		}

		/// <summary>
		/// This method will return an SQL Command to retrieve the most recent physical
		/// inventory date a group of sites.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="beginDate"></param>
		/// <param name="endDate"></param>
		public void GetMostRecentPhysicalInventoryOneSiteSQL(SqlCommand command, DateTime beginDate, DateTime endDate)
		{
			command.Parameters.Clear();

			const string SQL = "SELECT MAX(InventoryDate) AS InventoryDate "
								+ "FROM tblTransactions t WITH (NOLOCK) LEFT OUTER JOIN tblTransactionLineItems l WITH (NOLOCK) "
								+ "ON t.TransactionGuid = l.TransactionGuid ";
			string where = "WHERE t.InventoryDate < @InventoryDate " +
			                   "AND t.InventoryDate > @InventoryEndDate " +
							   "AND t.ManagerCompanyGuid  = @ManagerCompanyGuid " +
			                   "AND l.ProductGuid  = @ProductGuid " +
			                   "AND t.DeleteFlag = cast(0 as bit) " +
			                   "AND t.LookupTransTypeIndex = 14 " +
			                   "AND t.SiteGuid = @SiteGuid ";

			var parm = new SqlParameter("@InventoryDate", SqlDbType.Date) { Value = beginDate.Date };
			command.Parameters.Add(parm);

			parm = new SqlParameter("@ProductGuid", SqlDbType.UniqueIdentifier) { Value = this.productGuid };
			command.Parameters.Add(parm);

			parm = new SqlParameter("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier) { Value = this.managerGuid };
			command.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = this.siteGuid };
			command.Parameters.Add(parm);

			parm = new SqlParameter("@InventoryEndDate", SqlDbType.Date) { Value = endDate.Date };
			command.Parameters.Add(parm);

			if (this.tankGuid != Guid.Empty)
			{
				where = where + "AND l.StorageLocationTankGuid = @TankGuid ";
				parm = new SqlParameter("@TankGuid", SqlDbType.UniqueIdentifier) { Value = this.tankGuid };
				command.Parameters.Add(parm);
			}

			command.CommandText = SQL + where;
		}

		/// <summary>
		/// This method will load the data set that contains the most recent physical inventory
		/// start date a site. 
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="defaultBeginDate"></param>
		public void LoadMostRecentPhysicalInventoryDateOneSite(DataSet dataSet, DateTime defaultBeginDate)
		{
			var siteStartingPoint = new LRSiteDO(defaultBeginDate);
			this.allSiteInventoryDatesFound = false;

			this.siteDOList.Clear();
			this.siteDOList.Add(siteStartingPoint);

			if (dataSet?.Tables[0] != null && (dataSet.Tables.Count > 0))
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					DataRow row = table.Rows[0];

					if (row.IsNull("InventoryDate") == false)
					{
						siteStartingPoint.PhysicalInvDateForLedgerStart = (DateTime) row["InventoryDate"];
						siteStartingPoint.HasPhysicalInventory = true;
						this.allSiteInventoryDatesFound = true;
					}
					else
					{
						siteStartingPoint.HasPhysicalInventory = false;
					}
				}
			}
		}

		/// <summary>
		/// This method will return an SQL Command to retrieve the most recent physical
		/// inventory date for a group of sites.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="siteList"></param>
		/// <param name="beginDate"></param>
		/// <param name="endDate"></param>
		public void GetMostRecentPhysicalInventoryDateAllSitesSQL(SqlCommand command, List<LRSiteDO> siteList, DateTime beginDate, DateTime endDate)
		{
			command.Parameters.Clear();

			const string SQL = "SELECT SiteGuid, MAX(InventoryDate) AS InventoryDate "
			                   + "FROM tblTransactions t WITH (NOLOCK) LEFT OUTER JOIN tblTransactionLineItems l WITH (NOLOCK) "
			                   + "ON t.TransactionGuid = l.TransactionGuid ";

			string where = "WHERE t.InventoryDate < @InventoryDate " 
						   + "AND t.InventoryDate > @InventoryEndDate "
			               + "AND t.ManagerCompanyGuid  = @ManagerCompanyGuid " 
						   + "AND l.ProductGuid  = @ProductGuid "
			               + "AND t.DeleteFlag = cast(0 as bit) " + "AND t.LookupTransTypeIndex = 14 ";

			const string OrderBy = "GROUP BY SiteGuid ";

			var parm = new SqlParameter("@InventoryDate", SqlDbType.Date) { Value = beginDate.Date };
			command.Parameters.Add(parm);

			parm = new SqlParameter("@InventoryEndDate", SqlDbType.Date) { Value = endDate.Date };
			command.Parameters.Add(parm);

			parm = new SqlParameter("@ProductGuid", SqlDbType.UniqueIdentifier)
			{
				Value = this.productGuid
			};
			command.Parameters.Add(parm);

			parm = new SqlParameter("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier) { Value = this.managerGuid };
			command.Parameters.Add(parm);

			if (this.tankGuid != Guid.Empty)
			{
				where = where + "AND l.StorageLocationTankGuid = @TankGuid ";
				parm = new SqlParameter("@TankGuid", SqlDbType.UniqueIdentifier) { Value = this.managerGuid };
				command.Parameters.Add(parm);
			}

			int nextSite = 0;
			where = where + "AND t.SiteGuid IN (";

			foreach(LRSiteDO siteDo in siteList)
			{
				if (siteDo.SiteGroupFlag == false)
				{
					string siteParmName = "@SiteGuid" + nextSite;
					where = where + siteParmName + ", ";

					parm = new SqlParameter(siteParmName, SqlDbType.UniqueIdentifier) { Value = siteDo.SiteGuid };
					command.Parameters.Add(parm);
					nextSite++;
				}
			}

			int lastComma = where.LastIndexOf(',');
			where = where.Remove(lastComma);

			where = where + ") ";

			command.CommandText = SQL + where + OrderBy;
		}

		/// <summary>
		/// This method will load the data set that contains the most recent physical inventory
		/// start date for each site. It creates a list of SiteStartingPoint that contains the
		/// site name, site index, flag indicating whether an inventory date was found and
		/// an inventory date.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="possiblePhysicalInvCount"></param>
		/// <param name="defaultBeginDate"></param>
		public void LoadMostRecentPhysicalInventoryDateAllSites(DataSet dataSet, int possiblePhysicalInvCount, DateTime defaultBeginDate)
		{
			this.allSiteInventoryDatesFound = false;
			this.siteDOList.Clear();

			if (dataSet?.Tables[0] != null && (dataSet.Tables.Count > 0))
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					int foundCount = 0;

					foreach (DataRow row in table.Rows)
					{
						var siteStartingPoint = new LRSiteDO(defaultBeginDate)
						                        {
							                        SiteGuid = row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"]
						                        };

						if (row.IsNull("InventoryDate") == false)
						{
							siteStartingPoint.PhysicalInvDateForLedgerStart = (DateTime) row["InventoryDate"];
							siteStartingPoint.HasPhysicalInventory = true;
							foundCount++;
						}
						else
						{
							siteStartingPoint.HasPhysicalInventory = false;
						}

						this.siteDOList.Add(siteStartingPoint);
					}

					// Need to know if all the site physical inventory dates found
					// for the date range.
					if (foundCount == possiblePhysicalInvCount)
					{
						this.allSiteInventoryDatesFound = true;
					}
				}
			}
		}

		/// <summary>
		/// This method will return the starting date for all the sites. This will be the date
		/// for the site that has the oldest physical inventory date.
		/// </summary>
		/// <returns></returns>
		public DateTime? GetStartingPointForAllSites()
		{
			DateTime? startDate = null;

			if ((this.siteDOList != null) && (this.siteDOList.Count > 0))
			{
				List<LRSiteDO> orderedList = this.siteDOList.OrderBy(x => x.PhysicalInvDateForLedgerStart).ToList();
				LRSiteDO siteStartingPoint = orderedList[0];

				startDate = siteStartingPoint.PhysicalInvDateForLedgerStart;
			}

			return startDate;
		}

		/// <summary>
		/// This method will load the most recent physical inventory date
		/// data set.
		/// </summary>
		/// <param name="dataSet"></param>
		public void LoadMostRecentPhysicalInventoryDate(DataSet dataSet)
		{
			this.mostRecentDate = LedgerTime.MinFMDate.Date;
			this.hasPhysicalInvDate = false;

		    DataTable table = dataSet?.Tables[0];

		    if (table?.Rows.Count > 0)
		    {
		        DataRow row = table.Rows[0];

		        if (row.IsNull("InventoryDate") == false)
		        {
		            this.mostRecentDate = (DateTime) row["InventoryDate"];
		            this.hasPhysicalInvDate = true;
		        }
		    }
		}

		/// <summary>
		/// This method will retrieve summed daily physical inventory quantities SQL command.
		/// </summary>
		/// <param name="command"></param>
		public void GetSummedPhysicalInventoriesSQL(SqlCommand command)
		{
			// @SiteGuid UNIQUEIDENTIFIER, @InventoryDate DATE, @ManagerCompanyGuid UNIQUEIDENTIFIER, @ProductGuid UNIQUEIDENTIFIER, 
			// @VolumeFactor FLOAT, @VolumePrecision FLOAT, @MassFactor FLOAT, @MassPrecision FLOAT, @TankGuid UNIQUEIDENTIFIER

			const string SQL = "usp_GetOneDaysPhysicalInventorySummationSelect";
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = SQL;

			command.Parameters.Clear();
			command.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			command.Parameters.Add("@InventoryDate", SqlDbType.Date);
			command.Parameters.Add("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier);
			command.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			command.Parameters.Add("@VolumeFactor", SqlDbType.Float);
			command.Parameters.Add("@VolumePrecision", SqlDbType.Float);
			command.Parameters.Add("@MassFactor", SqlDbType.Float);
			command.Parameters.Add("@MassPrecision", SqlDbType.Float);
			command.Parameters.Add("@TankGuid", SqlDbType.UniqueIdentifier);
		}

		/// <summary>
		/// This method will load the quantity data.
		/// </summary>
		/// <param name="dataSet"></param>
		public void LoadSummedPhysicalInventories(DataSet dataSet)
		{
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0] != null)
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					DataRow row = table.Rows[0];
					this.quantity = new LRQuantityDO
					                {
						                Gross = (row.IsNull("GrossQuantity")) ? 0.0 : (double)row["GrossQuantity"],
						                Net = (row.IsNull("NetQuantity")) ? 0.0 : (double)row["NetQuantity"],
						                Mass = (row.IsNull("MassQuantity")) ? 0.0 : (double)row["MassQuantity"]
					                };
				}
			}
			else
			{
				this.quantity = new LRQuantityDO
				{
					Gross = 0.0,
					Net = 0.0,
					Mass = 0.0
				};
			}
		}
		#endregion
	}
}