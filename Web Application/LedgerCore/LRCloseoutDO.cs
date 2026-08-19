namespace LedgerCore
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;

    // ReSharper disable once InconsistentNaming
	public class LRCloseoutDO
	{
		#region Attributes
	    // ReSharper disable once NotAccessedField.Local
		private Guid closeoutInventoryGuid;
		private string siteID;
		private Guid siteGuid;
		private string managerName;
		private Guid managerGuid;
		private string productName;
		private Guid productGuid;
		private DateTime closeoutDate;
		private DateTime brokenBlendDate;
		private DateTimeOffset lastCloseoutDate;

		private LRQuantityDO bookInventory;
		private LRQuantityDO totalPhysicalInventory;
		private LRQuantityDO totalVariance;
		private string closeoutDateStr;
		private string brokenBlendDateStr;

		private double volumeFactor;
		private double massFactor;
		private int volumePrecision;
		private int massPrecision;

		private bool closeoutRecordFound;
		private LRDateConverter dateConverter;
		#endregion Attributes

		#region Properties
		public string SiteID
		{
			get { return this.siteID; }
			set { this.siteID = value; }
		}

		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
		}

		public string ManagerName
		{
			get { return this.managerName; }
			set { this.managerName = value; }
		}

		public Guid ManagerGuid
		{
			get { return this.managerGuid; }
			set { this.managerGuid = value; }
		}

		public string ProductName
		{
			get { return this.productName; }
			set { this.productName = value; }
		}

		public Guid ProductGuid
		{
			get { return this.productGuid; }
			set { this.productGuid = value; }
		}

		public DateTime CloseoutDate
		{
			get
			{
				if (this.closeoutDate < LedgerTime.MinFMDate.Date)
				{
					return LedgerTime.MinFMDate.Date;
				}
				
				return this.closeoutDate;
			}
			set
			{
				this.closeoutDate = value;
				this.closeoutDateStr = this.dateConverter.ConvertToMonthDayYear(this.closeoutDate);
			}
		}

		public DateTimeOffset LastCloseoutDate
		{
			get { return this.lastCloseoutDate; }
			set { this.lastCloseoutDate = value; }
		}

		public DateTime BrokenBlendDate
		{
			get { return this.brokenBlendDate; }
			set { this.brokenBlendDate = value; }
		}

		/// <summary>
		/// This property sets and gets the gross book inventory attribute.
		/// </summary>
		public LRQuantityDO BookInventory
		{
			get { return this.bookInventory; }
			set { this.bookInventory = value; }
		}

		/// <summary>
		/// Total physical is the running total of all the physical inventory.
		/// </summary>
		public LRQuantityDO TotalPhysicalInventory
		{
			get { return this.totalPhysicalInventory; }
			set { this.totalPhysicalInventory = value; }
		}

		/// <summary>
		/// This property sets and gets the gross variance.
		/// </summary>
		public LRQuantityDO Variance
		{
			get
			{
				var variance = new LRQuantityDO(this.totalPhysicalInventory.Gross - this.bookInventory.Gross,
												this.totalPhysicalInventory.Net - this.bookInventory.Net,
												this.totalPhysicalInventory.Mass - this.bookInventory.Mass,
												this.totalPhysicalInventory.Package - this.bookInventory.Package,
												this.totalPhysicalInventory.GrossPrice - this.bookInventory.GrossPrice,
												this.totalPhysicalInventory.NetPrice - this.bookInventory.NetPrice,
												this.totalPhysicalInventory.MassPrice - this.bookInventory.Mass);
				return variance;
			}
		}

		/// <summary>
		/// Total variance is the running total of all the daily variances.
		/// </summary>
		public LRQuantityDO TotalVariance
		{
			get { return this.totalVariance; }
			set { this.totalVariance = value; }
		}

		/// <summary>
		/// This property sets or gets whether a closeout record was found.
		/// True means a closeout record was found.  False means a closeout
		/// record was not found.
		/// </summary>
		public bool CloseoutRecordFound
		{
			get { return this.closeoutRecordFound; }
			set
			{
				this.closeoutRecordFound = value;

				if (this.closeoutRecordFound)
				{
					this.lastCloseoutDate = this.closeoutDate;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the closeout date as a string.
		/// </summary>
		public string CloseoutDateString
		{
			get { return this.closeoutDateStr; }
			set { this.closeoutDateStr = value; }
		}

		/// <summary>
		/// This property sets and gets the closeout date as a string.
		/// </summary>
		public string BrokenBlendDateString
		{
			get { return this.brokenBlendDateStr; }
			set { this.brokenBlendDateStr = value; }
		}

		/// <summary>
		/// This property sets and gets the volume factor setting.
		/// </summary>
		public double VolumeFactor
		{
			get { return this.volumeFactor; }
			set { this.volumeFactor = value; }
		}

		/// <summary>
		/// This property sets and gets the mass factor setting.
		/// </summary>
		public double MassFactor
		{
			get { return this.massFactor; }
			set { this.massFactor = value; }
		}

		/// <summary>
		/// This property sets and gets the volume precision setting.
		/// </summary>
		public int VolumePrecision
		{
			get { return this.volumePrecision; }
			set { this.volumePrecision = value; }
		}

		/// <summary>
		/// This property sets and gets the mass precision setting.
		/// </summary>
		public int MassPrecision
		{
			get { return this.massPrecision; }
			set { this.massPrecision = value; }
		}
		#endregion Properties

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Closeout data object.
		/// </summary>
		public LRCloseoutDO()
		{
			this.Reset();
		}
		#endregion

		#region SQL Methods
		/// <summary>
		/// This method will return the SQL that will get a last day that has 
		/// a broken blend for a given manager, product, and site.
		/// </summary>
		public void GetBrokenBlendDateSelectSQL(SqlCommand command, List<LRSiteDO> siteList, DateTimeOffset ledgerEndDate)
		{
			const string Select = "SELECT MIN(TransactionInventoryDate) AS BrokenBlendDate, SiteGuid ";

			const string From = "FROM tblTransactionSubLineItems s WITH(NOLOCK) LEFT OUTER JOIN "
								+ "tblTransactions t WITH(NOLOCK) ON s.TransactionGuid = t.TransactionGuid ";

			string where = "WHERE t.InventoryDate > @LastCloseoutDate "
							+ "AND t.InventoryDate <= @LedgerEndDate " 
							+ "AND t.ManagerCompanyGuid = @ManagerCompanyGuid " 
							+ "AND (t.ReversalType IS NULL OR t.ReversalType = 'U') " 
							+ "AND (t.DeleteFlag = 0 OR t.DeleteFlag IS NULL) "
							+ "AND s.ProductGuid = @ProductGuid " 
							+ "AND s.BrokenBlend = 1 ";

			const string GroupBy = "GROUP BY SiteGuid ";

			var parm = new SqlParameter("@LedgerEndDate", SqlDbType.Date) { Value = ledgerEndDate.Date };
			command.Parameters.Add(parm);

			parm = new SqlParameter("@LastCloseoutDate", SqlDbType.Date) { Value = this.lastCloseoutDate.Date };
			command.Parameters.Add(parm);

			if (this.managerGuid != Guid.Empty)
			{
				parm = new SqlParameter("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier) { Value = this.managerGuid };
				command.Parameters.Add(parm);
			}

			if (this.productGuid != Guid.Empty)
			{
				parm = new SqlParameter("@ProductGuid", SqlDbType.UniqueIdentifier) { Value = this.productGuid };
				command.Parameters.Add(parm);
			}

			int siteCount = 0;

			foreach(LRSiteDO siteDo in siteList)
			{
				if (siteDo.SiteGroupFlag)
				{
					continue;
				}

				if (siteCount == 0)
				{
					where = where + " AND SiteGuid IN ( ";
				}

				string siteParmName = "@SiteGuid" + siteCount;
				parm = new SqlParameter(siteParmName, SqlDbType.UniqueIdentifier) { Value = siteDo.SiteGuid };
				command.Parameters.Add(parm);
				where = where + siteParmName + ", ";

				siteCount++;
			}

			if (siteCount > 0)
			{
				int lastComma = where.LastIndexOf(',');
				where = where.Remove(lastComma);
				where = where + " ) ";
			}

			command.CommandText = Select + From + where + GroupBy;
		}

		/// <summary>
		/// This method will return the SQL that will get a last day that has 
		/// a broken blend for a given manager, product, and site.
		/// </summary>
		/// <returns></returns>
		public void GetBrokenBlendDateSingleSiteSelectSQL(SqlCommand command, DateTimeOffset ledgerEndDate)
		{
			const string Select = "SELECT MIN(TransactionInventoryDate) AS BrokenBlendDate ";
			const string From = "FROM tblTransactionSubLineItems s WITH(NOLOCK) LEFT OUTER JOIN "
			                     + "tblTransactions t WITH(NOLOCK) ON s.TransactionGuid = t.TransactionGuid ";
			string where = "WHERE t.InventoryDate > @LastCloseoutDate " +
								 "AND t.InventoryDate <= @LedgerEndDate " +
								 "AND t.SiteGuid = @SiteGuid " +
								 "AND (t.ReversalType IS NULL OR t.ReversalType = 'U') " +
								 "AND (t.DeleteFlag = 0 OR t.DeleteFlag = NULL) " +
								 "AND s.BrokenBlend = 1 ";

			var parm = new SqlParameter("@LedgerEndDate", SqlDbType.Date) { Value = ledgerEndDate.Date };
			command.Parameters.Add(parm);

			parm = new SqlParameter("@LastCloseoutDate", SqlDbType.Date) { Value = this.lastCloseoutDate.Date };
			command.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = this.siteGuid };
			command.Parameters.Add(parm);

			if (this.managerGuid != Guid.Empty)
			{
				where = where + "AND t.ManagerCompanyGuid = @ManagerCompanyGuid ";
				parm = new SqlParameter("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier) { Value = this.managerGuid };
				command.Parameters.Add(parm);
			}

			if (this.productGuid != Guid.Empty)
			{
				where = where + "AND s.ProductGuid = @ProductGuid ";
				parm = new SqlParameter("@ProductGuid", SqlDbType.UniqueIdentifier) { Value = this.productGuid };
				command.Parameters.Add(parm);
			}

			command.CommandText = Select + From + where;
		}

		/// <summary>
		/// This method will return the SQL that will get a list day that have been
		/// closed out record for a given manager, product, and site.
		/// </summary>
		public void GetLatestCloseoutDateSelectSQL(SqlCommand command, List<LRSiteDO> siteList, DateTimeOffset startDate)
		{
			const string Select		= "SELECT MAX(CloseoutDate) AS CloseoutDate, SiteGuid ";
			const string From		= "FROM tblCloseoutInventory WITH (NOLOCK) ";
			string where			= "WHERE CloseoutDate >= @LedgerStartDate ";
			const string GroupBy	= "GROUP BY SiteGuid ";

			var parm = new SqlParameter("@LedgerStartDate", SqlDbType.Date) { Value = startDate.Date };
			command.Parameters.Add(parm);

			if (this.managerGuid != Guid.Empty)
			{
				where = where + " AND ManagerCompanyGuid = @ManagerCompanyGuid ";
				parm = new SqlParameter("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier) { Value = this.managerGuid };
				command.Parameters.Add(parm);
			}

			if (this.productGuid != Guid.Empty)
			{
				where = where + " AND ProductGuid = @ProductGuid";
				parm = new SqlParameter("@ProductGuid", SqlDbType.UniqueIdentifier) { Value = this.productGuid };
				command.Parameters.Add(parm);
			}

			int siteCount = 0;

			foreach(LRSiteDO siteDo in siteList)
			{
				if (siteDo.SiteGroupFlag)
				{
					continue;
				}

				if (siteCount == 0)
				{
					where = where + " AND SiteGuid IN ( ";
				}

				string siteParmName = "@SiteGuid" + siteCount;
				where = where + siteParmName + ", ";

				parm = new SqlParameter(siteParmName, SqlDbType.UniqueIdentifier) { Value = this.siteGuid };
				command.Parameters.Add(parm);

				siteCount++;
			}

			if (siteCount > 0)
			{
				int lastComma = where.LastIndexOf(',');
				where = where.Remove(lastComma);
				where = where + " ) ";
			}

			command.CommandText = Select + From + where + GroupBy;
		}

		/// <summary>
		/// This method will return the SQL that will get a list day that have been
		/// closed out record for a given manager, product, and site.
		/// </summary>
		/// <returns></returns>
		public void GetLatestCloseoutDateSingleSiteSelectSQL(SqlCommand command, DateTimeOffset beginDate)
		{
			const string Select = "SELECT MAX(CloseoutDate) AS CloseoutDate ";
			const string From	= "FROM tblCloseoutInventory WITH (NOLOCK) ";
			string where		= "WHERE CloseoutDate >= @LedgerStartDate AND SiteGuid = @SiteGuid ";

			var parm = new SqlParameter("@LedgerStartDate", SqlDbType.Date) { Value = beginDate.Date };
			command.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = this.siteGuid };
			command.Parameters.Add(parm);

			if (this.managerGuid != Guid.Empty)
			{
				where = where + " AND ManagerCompanyGuid = @ManagerCompanyGuid ";
				parm = new SqlParameter("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier) { Value = this.managerGuid };
				command.Parameters.Add(parm);
			}

			if (this.productGuid != Guid.Empty)
			{
				where = where + " AND ProductGuid = @ProductGuid";
				parm = new SqlParameter("@ProductGuid", SqlDbType.UniqueIdentifier) { Value = this.productGuid };
				command.Parameters.Add(parm);
			}

			command.CommandText = Select + From + where;
		}

		/// <summary>
		/// This method will return an SQL that will retrieve the most current closeout date and
		/// other closeout data for a given site, manager, product and less that the start date.
		/// </summary>
		/// <returns></returns>
		public string GetCurrentCloseoutSelectSQL()
		{
			const string Select = "SELECT TOP 1 CloseoutInventoryGuid, Site, SiteGuid, CloseoutDate, " +
			                       " ProductName, ProductGuid, ManagerName, ManagerCompanyGuid, " +
			                       " GrossBookInventory, NetBookInventory, MassBookInventory, " +
			                       " GrossPhysicalInventory, NetPhysicalInventory, MassPhysicalInventory, " +
			                       " GrossVariance, NetVariance, MassVariance, " +
			                       " GrossBookPrice, NetBookPrice, MassBookPrice, " +
			                       " GrossPhysicalPrice, NetPhysicalPrice, MassPhysicalPrice ";
			const string From = "FROM tblCloseoutInventory ";
			string where = "WHERE CloseoutDate < @LedgerStartDate ";
			const string OrderBy = "ORDER BY CloseoutDate DESC";

			// Make sure that the manager, product, and site info exists before placing
			// them into the where clause.
			if (this.managerGuid != Guid.Empty)
			{
				where = where + " AND ManagerCompanyGuid = @ManagerCompanyGuid ";
			}

			if (this.productGuid != Guid.Empty)
			{
				where = where + " AND ProductGuid = @ProductGuid ";
			}

			if (this.siteGuid != Guid.Empty)
			{
				where = where + " AND SiteGuid = @SiteGuid ";
			}

			return (Select + From + where + OrderBy);
		}
		#endregion

		#region Load Methods
		/// <summary>
		/// This method will loads the most current closeout date and other closeout data 
		/// for a given owner, manager, product and less that the start date.
		/// </summary>
		/// <param name="row"></param>
		public void LoadCloseout(DataRow row)
		{
			if (row != null)
			{
				this.closeoutInventoryGuid				= (row.IsNull("CloseoutInventoryGuid")) ? Guid.Empty : (Guid)row["CloseoutInventoryGuid"];
				this.siteID								= (row.IsNull("Site")) ? string.Empty : (string)row["Site"];
				this.siteGuid							= (row.IsNull("SiteGuid")) ? LedgerConstants.SiteAdminGuid : (Guid)row["SiteGuid"];
				this.productName						= (row.IsNull("ProductName")) ? string.Empty : (string)row["ProductName"];
				this.productGuid						= (row.IsNull("ProductGuid")) ? Guid.Empty : (Guid)row["ProductGuid"];
				this.managerName						= (row.IsNull("ManagerName")) ? string.Empty : (string)row["ManagerName"];
				this.managerGuid						= (row.IsNull("ManagerCompanyGuid")) ? Guid.Empty : (Guid)row["ManagerCompanyGuid"];
				this.bookInventory.Gross				= (row.IsNull("GrossBookInventory")) ? 0.0 : (double)row["GrossBookInventory"];
				this.bookInventory.Net					= (row.IsNull("NetBookInventory")) ? 0.0 : (double)row["NetBookInventory"];
				this.bookInventory.Mass					= (row.IsNull("MassBookInventory")) ? 0.0 : (double)row["MassBookInventory"];
				this.totalPhysicalInventory.Gross		= (row.IsNull("GrossPhysicalInventory")) ? 0.0 : (double)row["GrossPhysicalInventory"];
				this.totalPhysicalInventory.Net			= (row.IsNull("NetPhysicalInventory")) ? 0.0 : (double)row["NetPhysicalInventory"];
				this.totalPhysicalInventory.Mass		= (row.IsNull("MassPhysicalInventory")) ? 0.0 : (double)row["MassPhysicalInventory"];
				this.totalVariance.Gross				= (row.IsNull("GrossVariance")) ? 0.0 : (double)row["GrossVariance"];
				this.totalVariance.Net					= (row.IsNull("NetVariance")) ? 0.0 : (double)row["NetVariance"];
				this.totalVariance.Mass					= (row.IsNull("MassVariance")) ? 0.0 : (double)row["MassVariance"];
				this.bookInventory.GrossPrice			= (row.IsNull("GrossBookPrice")) ? 0.0 : (double)row["GrossBookPrice"];
				this.bookInventory.NetPrice				= (row.IsNull("NetBookPrice")) ? 0.0 : (double)row["NetBookPrice"];
				this.bookInventory.MassPrice			= (row.IsNull("MassBookPrice")) ? 0.0 : (double)row["MassBookPrice"];
				this.totalPhysicalInventory.GrossPrice	= (row.IsNull("GrossPhysicalPrice")) ? 0.0 : (double)row["GrossPhysicalPrice"];
				this.totalPhysicalInventory.NetPrice	= (row.IsNull("NetPhysicalPrice")) ? 0.0 : (double)row["NetPhysicalPrice"];
				this.totalPhysicalInventory.MassPrice	= (row.IsNull("MassPhysicalPrice")) ? 0.0 : (double)row["MassPhysicalPrice"];

				if (row.IsNull("CloseoutDate") == false)
				{
					this.CloseoutDate = (DateTime)row["CloseoutDate"];
				}

				this.closeoutRecordFound = true;

				this.bookInventory.Gross		*= this.volumeFactor;
				this.bookInventory.Net			*= this.volumeFactor;
				this.bookInventory.Mass			*= this.massFactor;
				this.bookInventory.GrossPrice	*= this.volumeFactor;
				this.bookInventory.NetPrice		*= this.volumeFactor;
				this.bookInventory.MassPrice	*= this.massFactor;

				this.bookInventory.Gross = Math.Round(this.bookInventory.Gross, this.volumePrecision, MidpointRounding.AwayFromZero);
				this.bookInventory.Net	 = Math.Round(this.bookInventory.Net, this.volumePrecision, MidpointRounding.AwayFromZero);
				this.bookInventory.Mass  = Math.Round(this.bookInventory.Mass, this.massPrecision, MidpointRounding.AwayFromZero);
			}
		}

		/// <summary>
		/// This method will load the closed out date for a given manager, 
		/// product, and site.
		/// </summary>
		/// <param name="row"></param>
		public void LoadLatestCloseoutDate(DataRow row)
		{
			this.closeoutDateStr = string.Empty;

			if (row != null)
			{
				if (row.IsNull("CloseoutDate") == false)
				{
					this.CloseoutDate = (DateTime)row["CloseoutDate"];
					this.closeoutDateStr = this.dateConverter.ConvertToMonthDayYear(this.CloseoutDate);
				}

				this.siteGuid = row.IsNull("SiteGuid") ? Guid.Empty : (Guid) row["SiteGuid"];
			}
		}

		/// <summary>
		/// This method will load the broken blend date for a given manager, 
		/// product, and site.
		/// </summary>
		/// <param name="row"></param>
		public void LoadBrokenBlendDate(DataRow row)
		{
			this.brokenBlendDateStr = string.Empty;

			if (row != null)
			{
				if (row.IsNull("BrokenBlendDate") == false)
				{
					this.BrokenBlendDate = (DateTime)row["BrokenBlendDate"];
					this.brokenBlendDateStr = this.dateConverter.ConvertToMonthDayYear(this.BrokenBlendDate);
				}

				this.siteGuid = row.IsNull("SiteGuid") ? Guid.Empty : (Guid) row["SiteGuid"];
			}
		}

		/// <summary>
		/// This method will load the closed out date for a given manager, 
		/// product, and site.
		/// </summary>
		/// <param name="row"></param>
		public void LoadLatestCloseoutDateSingleSite(DataRow row)
		{
			this.closeoutDateStr = string.Empty;

		    if (row?.IsNull("CloseoutDate") == false)
		    {
		        this.CloseoutDate = (DateTime) row["CloseoutDate"];
		        this.closeoutDateStr = this.dateConverter.ConvertToMonthDayYear(this.CloseoutDate);
		    }
		}

		/// <summary>
		/// This method will load the broken blend date for a given manager, 
		/// product, and site.
		/// </summary>
		/// <param name="row"></param>
		public void LoadBrokenBlendDateSingleSite(DataRow row)
		{
			this.brokenBlendDateStr = string.Empty;

		    if (row?.IsNull("BrokenBlendDate") == false)
		    {
		        this.BrokenBlendDate = (DateTime) row["BrokenBlendDate"];
		        this.brokenBlendDateStr = this.dateConverter.ConvertToMonthDayYear(this.BrokenBlendDate);
		    }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		public void Reset()
		{
			this.lastCloseoutDate		= new DateTime(1901, 01, 01, 00, 00, 00);
			this.closeoutDate			= new DateTime(1901, 01, 01, 00, 00, 00);
			this.brokenBlendDate		= new DateTime(1901, 01, 01, 00, 00, 00);
			this.bookInventory			= new LRQuantityDO();
			this.totalPhysicalInventory = new LRQuantityDO();
			this.totalVariance			= new LRQuantityDO();
			this.dateConverter			= new LRDateConverter();
			this.closeoutRecordFound	= false;
			this.massPrecision			= 2;
			this.massFactor				= 1.0;
			this.volumeFactor			= 1.0;
			this.volumePrecision		= 2;
			this.closeoutDateStr		= this.dateConverter.ConvertToMonthDayYear(this.closeoutDate);
		}
		#endregion
	}
}