namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;

    [Serializable]
	[XmlRoot("Closeout")]
	[XmlType("Closeout")]
	[DataContract]
	public class CloseoutDO : BaseLineItemDO
	{
		#region Constants
		protected const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";
		#endregion

		#region Attributes
        [DataMember]
        protected string closeouterrtext;
        [DataMember]
        protected bool closeouterror;
        [DataMember]
        protected string confirmtext;
        [DataMember]
        protected ProductIrdoCollectionClass nonclosedproductsirdocollection;
        [DataMember]
        protected ProductIrdoCollectionClass allProductsIrdoCollection;
		[DataMember]
		protected Guid closeoutInventoryGuid;
		[DataMember]
		protected string siteID;
		[DataMember]
		protected Guid siteGuid;
		[DataMember]
		protected string managerName;
		[DataMember]
		protected Guid managerGuid;
		[DataMember]
		protected string productName;
		[DataMember]
		protected Guid productGuid;
		[DataMember]
		protected DateTime closeoutDate;
		[DataMember]
		protected DateTime brokenBlendDate;
		[DataMember]
		protected DateTime lastCloseoutDate;

		[DataMember]
		private QuantityDO bookInventory;
		[DataMember]
		private QuantityDO totalPhysicalInventory;
		[DataMember]
		private QuantityDO totalVariance;

		private const int EMPTY_STRING = 0;

		[DataMember]
		private string user;
		[DataMember]
		private bool closeoutRecordFound = false;

		[DataMember]
		private List<OwnerCloseoutDO> lstOwnerCloseoutDO;

		#endregion Attributes

		#region Properties

        public string Closeouterrtext
        {
            get { return closeouterrtext; }
            set { closeouterrtext = value; }
        }

        public bool Closeouterror
        {
            get { return closeouterror; }
            set { closeouterror = value; }
        }

        public string Confirmtext
        {
            get { return confirmtext; }
            set { confirmtext = value; }
        }

        public ProductIrdoCollectionClass Nonclosedproductsirdocollection
        {
            get { return nonclosedproductsirdocollection; }
            set { nonclosedproductsirdocollection = value; }
        }

        public ProductIrdoCollectionClass AllProductsIrdoCollection
        {
            get { return allProductsIrdoCollection; }
            set { allProductsIrdoCollection = value; }
        }

		public List<OwnerCloseoutDO> lstOwnerCloseouts
		{
			get { return lstOwnerCloseoutDO; }
			set { lstOwnerCloseoutDO = value; }
		}

		public string SiteID
		{
			get { return siteID; }
			set { siteID = value; }
		}

		[XmlIgnore]
		public Guid SiteGuid
		{
			get { return siteGuid; }
			set { siteGuid = value; }
		}

		public string ManagerName
		{
			get { return managerName; }
			set { managerName = value; }
		}

		[XmlIgnore]
		public Guid ManagerGuid
		{
			get { return managerGuid; }
			set { managerGuid = value; }
		}

		public string ProductName
		{
			get { return productName; }
			set { productName = value; }
		}

		[XmlIgnore]
		public Guid ProductGuid
		{
			get { return productGuid; }
			set { productGuid = value; }
		}

		[XmlElement("CloseoutDate")]
		public string CloseoutDateString
		{
			get
			{
				return this.closeoutDate.ToString(TimeFormat);
			}

			set
			{
				this.closeoutDate = DateTime.ParseExact(value, TimeFormat, null).Date;
			}
		}

		[XmlIgnore]
		public DateTime CloseoutDate
		{
			get { return closeoutDate; }
			set { closeoutDate = value; }
		}

		[XmlElement("LastCloseoutDate")]
		public string LastCloseoutDateString
		{
			get
			{
				return this.lastCloseoutDate.ToString(TimeFormat);
			}

			set
			{
				this.lastCloseoutDate = DateTime.ParseExact(value, TimeFormat, null).Date;
			}
		}

		[XmlIgnore]
		public DateTime LastCloseoutDate
		{
			get { return lastCloseoutDate; }
			set { lastCloseoutDate = value; }
		}

		[XmlElement("BrokenBlendDate")]
		public string BrokenBlendDateString
		{
			get
			{
				return this.brokenBlendDate.ToString(TimeFormat);
			}

			set
			{
				this.brokenBlendDate = DateTime.ParseExact(value, TimeFormat, null);
			}
		}

	
		[XmlIgnore]
		public DateTime BrokenBlendDate
		{
			get { return brokenBlendDate; }
			set { brokenBlendDate = value; }
		}

		/// <summary>
		/// This property sets and gets the gross book inventory attribute.
		/// </summary>
		public QuantityDO BookInventory
		{
			get { return this.bookInventory; }
			set { this.bookInventory = value; }
		}

		/// <summary>
		/// Total physical is the running total of all the physical inventory.
		/// </summary>
		public QuantityDO TotalPhysicalInventory
		{
			get { return this.totalPhysicalInventory; }
			set { this.totalPhysicalInventory = value; }
		}

		/// <summary>
		/// This property sets and gets the gross variance.
		/// </summary>
		public QuantityDO Variance
		{
			get
			{
				QuantityDO variance = new QuantityDO(totalPhysicalInventory.GrossInventoryChange - bookInventory.GrossInventoryChange,
																					 totalPhysicalInventory.NetInventoryChange - bookInventory.NetInventoryChange,
																					 totalPhysicalInventory.MassInventoryChange - bookInventory.MassInventoryChange,
																					 totalPhysicalInventory.PackageInventoryChange - bookInventory.PackageInventoryChange,
																					 totalPhysicalInventory.GrossPriceInventoryChange - bookInventory.GrossPriceInventoryChange,
																					 totalPhysicalInventory.NetPriceInventoryChange - bookInventory.NetPriceInventoryChange,
																					 totalPhysicalInventory.MassPriceInventoryChange - bookInventory.MassPriceInventoryChange);
				return variance;
			}
		}

		/// <summary>
		/// Total variance is the running total of all the daily variances.
		/// </summary>
		public QuantityDO TotalVariance
		{
			get { return this.totalVariance; }
			set { this.totalVariance = value; }
		}

		/// <summary>
		/// This property sets or gets whether a closeout record was found.
		/// True means a closeout record was found.  False means a closeout
		/// record was not found.
		/// </summary>
		[XmlIgnore]
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

		#endregion Properties

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Closeout data object.
		/// </summary>
		public CloseoutDO()
		{
			this.lastCloseoutDate = TimeConverter.MinFMDate.Date;
			this.bookInventory = new QuantityDO();
			this.totalPhysicalInventory = new QuantityDO();
			this.totalVariance = new QuantityDO();
			this.lstOwnerCloseoutDO = new List<OwnerCloseoutDO>();
		}
		#endregion

		#region SQL Methods

		public void GetInsertCommand(SqlCommand cmd, string user)
		{
			this.user = user;
			GetInsertCommand(cmd);
			this.user = null;
		}

		/// <summary>
		/// This method will return the SQL that will get a last day that has 
		/// a broken blend for a given manager, product, and site.
		/// </summary>
		/// <param name="ledgerSR"></param>
		/// <param name="siteName"></param>
		/// <returns></returns>
		public void GetBrokenBlendDateSelectSQL(SqlCommand cmd, LedgerSR ledgerSR, string siteName)
		{

			string select = "SELECT MIN(TransactionInventoryDate) AS TransactionInventoryDate"
								+ " FROM tblTransactionSubLineItems WITH(NOLOCK)";

			string where = " WHERE TransactionGuid IN" +
							" (SELECT TransactionGuid" +
							" FROM tblTransactions WITH(NOLOCK)" +
							" WHERE InventoryDate > @LastCloseoutDate " +
							" AND InventoryDate <= @LedgerEndDate " +
							" AND Site = @Site " +
							" AND ManagerID = @ManagerID" +
									  " AND (ReversalType IS NULL OR ReversalType = 'U')" +
									  " AND (DeleteFlag = 0 OR DeleteFlag = NULL))" +
							" AND Product = @Product " +
							" AND BrokenBlend = 1";

			cmd.CommandText = select + where;

			cmd.Parameters.Add("@LastCloseoutDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@LedgerEndDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@ManagerID", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@Product", SqlDbType.NVarChar, 30);

			cmd.Parameters["@LastCloseoutDate"].Value = DateEfficacy.convertToMonthDayYear(lastCloseoutDate);
			cmd.Parameters["@LedgerEndDate"].Value = ledgerSR.GetLedgerEndDate();
			cmd.Parameters["@Site"].Value = siteName;
			cmd.Parameters["@ManagerID"].Value = ledgerSR.Manager;
			cmd.Parameters["@Product"].Value = ledgerSR.Product;
		}

		/// <summary>
		/// This method will return the SQL that will get a list day that have been
		/// closed out record for a given manager, product, and site.
		/// </summary>
		/// <param name="ledgerSR"></param>
		/// <param name="siteName"></param>
		/// <returns></returns>
		public void GetLatestCloseoutDateSelectSQL(SqlCommand cmd, LedgerSR ledgerSR, string siteName)
		{
			string select = "SELECT MAX(CloseoutDate) AS CloseoutDate" +
								" FROM tblCloseoutInventory ";
			string where = "WHERE CloseoutDate >= @CloseoutDate ";

			if ((siteName != null) && (siteName.Length != EMPTY_STRING))
			{
				where = where + " AND Site = @Site ";
				cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 30);
				cmd.Parameters["@Site"].Value = siteName;
			}

			if ((ledgerSR.Manager != null) && (ledgerSR.Manager.Length != EMPTY_STRING))
			{
				where = where + "AND ManagerName = @ManagerName ";
				cmd.Parameters.Add("@ManagerName", SqlDbType.NVarChar, 100);
				cmd.Parameters["@ManagerName"].Value = ledgerSR.Manager;
			}

			if ((ledgerSR.Product != null) && (ledgerSR.Product.Length != EMPTY_STRING))
			{
				where = where + "AND ProductName = @ProductName ";
				cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 30);
				cmd.Parameters["@ProductName"].Value = ledgerSR.Product;
			}

			// The time and time offset must be zero since the data in the table is set to
			// no time and no time offset.
			cmd.Parameters.Add("@CloseoutDate", SqlDbType.Date);
			cmd.Parameters["@CloseoutDate"].Value = ledgerSR.GetLedgerStartDate();

			cmd.CommandText = select + where;
		}


		/// <summary>
		/// This method will return an SQL that will retrieve the most current closeout date and
		/// other closeout data for a given site, manager, product and less that the start date.
		/// </summary>
		/// <param name="ledgerSR"></param>
		/// <returns></returns>
		public void GetCurrentCloseoutSelectSQL(SqlCommand cmd, LedgerSR ledgerSR, string siteName)
		{
			string select = "SELECT TOP 1 CloseoutInventoryGuid, Site, SiteGuid, CloseoutDate, " +
						" ProductName, ProductGuid, ManagerName, ManagerCompanyGuid, " +
							" GrossBookInventory, NetBookInventory, GrossPhysicalInventory, NetPhysicalInventory, " +
							"GrossVariance, NetVariance, GrossBookPrice, NetBookPrice, GrossPhysicalPrice, NetPhysicalPrice " +
						"FROM tblCloseoutInventory ";
			string where = "WHERE CloseoutDate < @CloseoutDate ";
			string orderBy = "ORDER BY CloseoutDate DESC";

			// Make sure that the manager, product, and site info exists before placing
			// them into the where clause.
			if ((ledgerSR.Manager != null) && (ledgerSR.Manager.Length != EMPTY_STRING))
			{
				where = where + " AND ManagerName = @ManagerName ";
				cmd.Parameters.Add("@ManagerName", SqlDbType.NVarChar, 100);
				cmd.Parameters["@ManagerName"].Value = ledgerSR.Manager;
			}

			if ((ledgerSR.Product != null) && (ledgerSR.Product.Length != EMPTY_STRING))
			{
				where = where + " AND ProductName = @ProductName ";
				cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 30);
				cmd.Parameters["@ProductName"].Value = ledgerSR.Product;
			}

			if ((siteName != null) && (siteName.Length != EMPTY_STRING))
			{
				where = where + " AND Site = @Site ";
				cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 30);
				cmd.Parameters["@Site"].Value = siteName;
			}

			cmd.CommandText = select + where + orderBy;

			cmd.Parameters.Add("@CloseoutDate", SqlDbType.DateTimeOffset);
			cmd.Parameters["@CloseoutDate"].Value = ledgerSR.GetLedgerStartDate();

		}

		/// This method will return  SQL that will retrieve Last CloseOut Records Inserted. 
		public void GetLastCloseoutSelectSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT TOP 1 [CloseoutInventoryGuid], [Site], [SiteGuid], [CloseoutDate], [ProductName], " +
						"[ProductGuid], [ManagerName], [ManagerCompanyGuid], [GrossBookInventory], [NetBookInventory], " +
						"[GrossPhysicalInventory], [NetPhysicalInventory], [GrossVariance], [NetVariance], " +
						"[CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [GrossBookPrice], [NetBookPrice], " +
						"[GrossPhysicalPrice], [NetPhysicalPrice], [TransVersion], [MassBookInventory], " +
						"[MassPhysicalInventory], [MassVariance], [MassBookPrice], [MassPhysicalPrice] " +
						"FROM [tblCloseoutInventory] " +
						"ORDER BY [CloseoutInventoryGuid] DESC ";
		}

		public void GetSQLForCloseOutRec(SqlCommand cmd, Guid closeoutInventoryGuid)
		{
			cmd.CommandText = "SELECT [CloseoutInventoryGuid], [Site], [SiteGuid], [CloseoutDate], [ProductName], " +
						"[ProductGuid], [ManagerName], [ManagerCompanyGuid], [GrossBookInventory], [NetBookInventory], " +
						"[GrossPhysicalInventory], [NetPhysicalInventory], [GrossVariance], [NetVariance], " +
						"[CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [GrossBookPrice], [NetBookPrice], " +
						"[GrossPhysicalPrice], [NetPhysicalPrice], [TransVersion], [MassBookInventory], " +
						"[MassPhysicalInventory], [MassVariance], [MassBookPrice], [MassPhysicalPrice] " +
						"FROM [tblCloseoutInventory] WHERE [CloseoutInventoryGuid] = @CloseoutInventoryGuid";

			cmd.Parameters.Add("@CloseoutInventoryGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@CloseoutInventoryGuid"].Value = closeoutInventoryGuid;
		}


		#endregion


		#region Load Methods
		/// <summary>
		/// This method will loads the most current closeout date and other closeout data 
		/// for a given owner, manager, product and less that the start date.
		/// </summary>
		/// <param name="dataSet"></param>
		public void loadCloseout(System.Data.DataSet dataSet)
		{
			loadCloseoutUsingColumnName(dataSet);
		}


		/// <summary>
		/// This method will loads the most current closeout date and other closeout data 
		/// for a given owner, manager, product and less that the start date.
		/// </summary>
		/// <param name="dataSet"></param>
		public void loadCloseoutUsingColumnName(DataSet dataSet)
		{
			if (dataSet != null)
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					DataRow row = table.Rows[0];

					this.closeoutInventoryGuid = getValue<Guid>(row["CloseoutInventoryGuid"], Guid.Empty);
					this.siteID = getValue<string>(row["Site"], "");
					this.siteGuid = getValue<Guid>(row["SiteGuid"], Guid.Empty);
					this.CloseoutDate = getValue<DateTime>(row["CloseoutDate"], DateTime.Today);
					this.productName = getValue<string>(row["ProductName"], "");
					this.productGuid = getValue<Guid>(row["ProductGuid"], Guid.Empty);
					this.managerName = getValue<string>(row["ManagerName"], "");
					this.managerGuid = getValue<Guid>(row["ManagerCompanyGuid"], Guid.Empty);
					this.bookInventory.Gross = getValue<double>(row["GrossBookInventory"], 0.0);
					this.bookInventory.Net = getValue<double>(row["NetBookInventory"], 0.0);
					this.bookInventory.Mass = getValue<double>(row["MassBookInventory"], 0.0);
					this.totalPhysicalInventory.Gross = getValue<double>(row["GrossPhysicalInventory"], 0.0);
					this.totalPhysicalInventory.Net = getValue<double>(row["NetPhysicalInventory"], 0.0);
					this.totalPhysicalInventory.Mass = getValue<double>(row["MassPhysicalInventory"], 0.0);
					this.totalVariance.Gross = getValue<double>(row["GrossVariance"], 0.0);
					this.totalVariance.Net = getValue<double>(row["NetVariance"], 0.0);
					this.totalVariance.Mass = getValue<double>(row["MassVariance"], 0.0);
					this.bookInventory.GrossPrice = getValue<double>(row["GrossBookPrice"], 0.0);
					this.bookInventory.NetPrice = getValue<double>(row["NetBookPrice"], 0.0);
					this.bookInventory.MassPrice = getValue<double>(row["MassBookPrice"], 0.0);
					this.totalPhysicalInventory.GrossPrice = getValue<double>(row["GrossPhysicalPrice"], 0.0);
					this.totalPhysicalInventory.NetPrice = getValue<double>(row["NetPhysicalPrice"], 0.0);
					this.totalPhysicalInventory.MassPrice = getValue<double>(row["MassPhysicalPrice"], 0.0);
					closeoutRecordFound = true;
				}
			}
		}

		/// <summary>
		/// This method will load the closed out date for a given manager, 
		/// product, and site.
		/// </summary>
		/// <param name="dataSet"></param>
		public void loadLatestCloseoutDate(DataSet dataSet)
		{
			if (dataSet != null)
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					DataRow row = table.Rows[0];
					object dateObj = row["CloseoutDate"];
					bool dateExists = !isNull(dateObj);

					if (dateExists == true)
					{
						this.CloseoutDate = (DateTime)dateObj;
					}
				}
			}
		}

		/// <summary>
		/// This method will load the broken blend date for a given manager, 
		/// product, and site.
		/// </summary>
		/// <param name="dataSet"></param>
		public void loadBrokenBlendDate(System.Data.DataSet dataSet)
		{
			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					System.Data.DataRow row = table.Rows[0];
					object dateObj = row["TransactionInventoryDate"];
					bool dateExists = !isNull(dateObj);

					if (dateExists == true)
					{
						this.BrokenBlendDate = (DateTime)dateObj;
					}
				}
			}
		}


		#endregion

		#region Overrides

		public override string getDeleteCommand()
		{
			return null;
		}

		/// <summary>
		/// This method returns the insert SQL for inserting a closeout record to the
		/// database.
		/// </summary>
		/// <returns></returns>
		public override string getInsertCommand()
		{
			return null;
		}

		public override string getSelectCommand()
		{
			return null;
		}

		public override string getUpdateCommand()
		{
			return null;
		}

        /// <summary>
        /// This method returns the insert SQL for inserting a closeout record to the
        /// database.
        /// </summary>
        /// <returns></returns>
		public override void GetInsertCommand(SqlCommand cmd)
		{
			string sql =
			"INSERT INTO tblCloseoutInventory (" +
				"Site, SiteGuid, CloseoutDate, ProductName, ProductGuid, ManagerName, ManagerCompanyGuid, " +
				"GrossBookInventory, NetBookInventory, GrossPhysicalInventory, " +
				"NetPhysicalInventory, GrossVariance, NetVariance, " +
				"CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, " +
				"GrossBookPrice, NetBookPrice, GrossPhysicalPrice, NetPhysicalPrice, " +
				"MassBookInventory, MassPhysicalInventory, MassVariance, MassBookPrice, MassPhysicalPrice, CloseoutInventoryGuid) " +
			" VALUES (";

			sql = sql +
					"@SiteID, " +
					"@SiteGuid," +
					"@CloseoutDate, " +
					"@ProductName, " +
					"@ProductGuid," +
					"@ManagerName, " +
					"@ManagerCompanyGuid," +
					"@GrossBookInventory, " +
					"@NetBookInventory, " +
					"@GrossPhysicalInventory, " +
					"@NetPhysicalInventory, " +
					"@GrossVariance, " +
					"@NetVariance, " +
					"SYSDATETIMEOFFSET(), " +
					"@CreatedBy, " +
					"SYSDATETIMEOFFSET(), " +
					"@UpdatedBy, " +
					"@GrossBookPrice, " +
					"@NetBookPrice, " +
					"@GrossPhysicalPrice, " +
					"@NetPhysicalPrice, " +
					"@MassBookInventory, " +
					"@MassPhysicalInventory, " +
					"@MassVariance, " +
					"@MassBookPrice, " +
					"@MassPhysicalPrice, " +
					"@CloseoutInventoryGuid" +
					")";

			cmd.CommandText = sql;

			cmd.Parameters.Add("@SiteID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CloseoutDate", SqlDbType.Date);
			cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ManagerName", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@GrossBookInventory", SqlDbType.Float);
			cmd.Parameters.Add("@NetBookInventory", SqlDbType.Float);
			cmd.Parameters.Add("@GrossPhysicalInventory", SqlDbType.Float);
			cmd.Parameters.Add("@NetPhysicalInventory", SqlDbType.Float);
			cmd.Parameters.Add("@GrossVariance", SqlDbType.Float);
			cmd.Parameters.Add("@NetVariance", SqlDbType.Float);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@GrossBookPrice", SqlDbType.Float);
			cmd.Parameters.Add("@NetBookPrice", SqlDbType.Float);
			cmd.Parameters.Add("@GrossPhysicalPrice", SqlDbType.Float);
			cmd.Parameters.Add("@NetPhysicalPrice", SqlDbType.Float);
			cmd.Parameters.Add("@MassBookInventory", SqlDbType.Float);
			cmd.Parameters.Add("@MassPhysicalInventory", SqlDbType.Float);
			cmd.Parameters.Add("@MassVariance", SqlDbType.Float);
			cmd.Parameters.Add("@MassBookPrice", SqlDbType.Float);
			cmd.Parameters.Add("@MassPhysicalPrice", SqlDbType.Float);
			cmd.Parameters.Add("@CloseoutInventoryGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteID"].Value = SiteID;

			if (siteGuid == Guid.Empty)
			{
				cmd.Parameters["@SiteGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@SiteGuid"].Value = siteGuid;
			}

			cmd.Parameters["@CloseoutDate"].Value = CloseoutDate.Date;
			cmd.Parameters["@ProductName"].Value = ProductName;

			if (ProductGuid == Guid.Empty)
			{
				cmd.Parameters["@ProductGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@ProductGuid"].Value = ProductGuid;
			}

			cmd.Parameters["@ManagerName"].Value = ManagerName;

			if (ManagerGuid == Guid.Empty)
			{
				cmd.Parameters["@ManagerCompanyGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@ManagerCompanyGuid"].Value = ManagerGuid;
			}

			cmd.Parameters["@GrossBookInventory"].Value = BookInventory.GrossInventoryChange;
			cmd.Parameters["@NetBookInventory"].Value = BookInventory.NetInventoryChange;
			cmd.Parameters["@GrossPhysicalInventory"].Value = TotalPhysicalInventory.GrossInventoryChange;
			cmd.Parameters["@NetPhysicalInventory"].Value = TotalPhysicalInventory.NetInventoryChange;
			cmd.Parameters["@GrossVariance"].Value = TotalVariance.GrossInventoryChange;
			cmd.Parameters["@NetVariance"].Value = TotalVariance.NetInventoryChange;
			cmd.Parameters["@CreatedBy"].Value = user;
			cmd.Parameters["@UpdatedBy"].Value = user;
			cmd.Parameters["@GrossBookPrice"].Value = BookInventory.GrossPriceInventoryChange;
			cmd.Parameters["@NetBookPrice"].Value = BookInventory.NetPriceInventoryChange;
			cmd.Parameters["@GrossPhysicalPrice"].Value = TotalPhysicalInventory.GrossPriceInventoryChange;
			cmd.Parameters["@NetPhysicalPrice"].Value = TotalPhysicalInventory.NetPriceInventoryChange;
			cmd.Parameters["@MassBookInventory"].Value = BookInventory.MassInventoryChange;
			cmd.Parameters["@MassPhysicalInventory"].Value = TotalPhysicalInventory.Mass;
			cmd.Parameters["@MassVariance"].Value = TotalVariance.MassInventoryChange;
			cmd.Parameters["@MassBookPrice"].Value = BookInventory.MassPriceInventoryChange;
			cmd.Parameters["@MassPhysicalPrice"].Value = TotalPhysicalInventory.MassPrice;
			cmd.Parameters["@CloseoutInventoryGuid"].Value = Guid.NewGuid();
		}

		#endregion Overrides
	}
}
