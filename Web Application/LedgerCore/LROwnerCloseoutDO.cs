namespace LedgerCore
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;

    // ReSharper disable once InconsistentNaming
	public class LROwnerCloseoutDO
	{
		#region Attributes
		private Guid ownerCloseoutGuid;
		private DateTime closeoutDate;
		private string productName;
		private Guid productGuid;
		private string managerName;
		private Guid managerGuid;
		private string ownerName;
		private Guid ownerGuid;
		private LRQuantityDO bookInventory;

	    private string createdBy;
		private string updatedBy;
		private string siteName;
		private Guid siteGuid;
		private string closeoutDateStr;
		private LRDateConverter dateConverter;
		private bool foundOwnerCloseoutRecord;
		private double volumeFactor;
		private int volumePrecision;
		private double massFactor;
		private int massPrecision;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the owner closeout class.
		/// </summary>
		public LROwnerCloseoutDO()
		{
			this.Initial(null);
		}

		/// <summary>
		/// Constructor to handle a default date.
		/// </summary>
		/// <param name="defaultBeginDate"></param>
		public LROwnerCloseoutDO(DateTime defaultBeginDate)
		{
			this.Initial(defaultBeginDate);
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property return true if the owner closeout record was found
		/// (must have a valid closeout date).
		/// </summary>
		public bool FoundOwnerCloseoutRecord => this.foundOwnerCloseoutRecord;

	    /// <summary>
		/// This property sets and gets the owner closeout Guid.
		/// </summary>
		public Guid OwnerCloseoutGuid
		{
			get { return this.ownerCloseoutGuid; }
			set { this.ownerCloseoutGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the closeout date.
		/// </summary>
		public DateTime CloseoutDate
		{
			get { return this.closeoutDate; }
			set { this.closeoutDate = value; }
		}

		/// <summary>
		/// This property sets and gets the product name.
		/// </summary>
		public string ProductName
		{
			get { return this.productName; }
			set { this.productName = value; }
		}

		/// <summary>
		/// This property sets and gets the product Guid.
		/// </summary>
		public Guid ProductGuid
		{
			get { return this.productGuid; }
			set { this.productGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the manager name.
		/// </summary>
		public string ManagerName
		{
			get { return this.managerName; }
			set { this.managerName = value; }
		}

		/// <summary>
		/// This property sets and gets the manager Guid.
		/// </summary>
		public Guid ManagerGuid
		{
			get { return this.managerGuid; }
			set { this.managerGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the owner name.
		/// </summary>
		public string OwnerName
		{
			get { return this.ownerName; }
			set { this.ownerName = value; }
		}

		/// <summary>
		/// This property sets and gets the owner Guid.
		/// </summary>
		public Guid OwnerGuid
		{
			get { return this.ownerGuid; }
			set { this.ownerGuid = value; }
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
		/// This property sets and gets the created date.
		/// </summary>
		public DateTimeOffset CreatedDate { get; set; }

	    /// <summary>
		/// This property sets and gets the updated date.
		/// </summary>
		public DateTimeOffset UpdatedDate { get; set; }

	    /// <summary>
		/// This property sets and gets the created by name.
		/// </summary>
		public string CreatedBy
		{
			get { return this.createdBy; }
			set { this.createdBy = value; }
		}

		/// <summary>
		/// This property sets and gets the updated by name.
		/// </summary>
		public string UpdatedBy
		{
			get { return this.updatedBy; }
			set { this.updatedBy = value; }
		}

		/// <summary>
		/// This property sets and gets the site name.
		/// </summary>
		public string SiteName
		{
			get { return this.siteName; }
			set { this.siteName = value; }
		}

		/// <summary>
		/// This property sets and gets the site Guid.
		/// </summary>
		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
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
		/// This property sets and get the volume factor for VCF.
		/// </summary>
		public double VolumeFactor
		{
			get { return this.volumeFactor; }
			set { this.volumeFactor = value; }
		}

		/// <summary>
		/// This property sets and gets the volume precision.
		/// </summary>
		public int VolumePrecision
		{
			get { return this.volumePrecision; }
			set { this.volumePrecision = value; }
		}

		/// <summary>
		/// This property sets and get the mass factor for VCF.
		/// </summary>
		public double MassFactor
		{
			get { return this.massFactor; }
			set { this.massFactor = value; }
		}

		/// <summary>
		/// This property sets and gets the mass precision.
		/// </summary>
		public int MassPrecision
		{
			get { return this.massPrecision; }
			set { this.massPrecision = value; }
		}
		#endregion

		#region SQL Methods
		/// <summary>
		/// This method will return the SQL that will get a list day that have been
		/// closed out record for a given manager, owner, product, and site.
		/// </summary>
		public void GetLatestCloseoutDateSelectSQL(SqlCommand command, List<LRSiteDO> siteList, DateTimeOffset startDate)
		{
			const string Select		= "SELECT MAX(CloseoutDate) AS CloseoutDate ";
			const string From		= "FROM tblOwnerCloseout WITH (NOLOCK) ";
			string where			= "WHERE CloseoutDate >= @LedgerStartDate ";
			const string GroupBy	= " GROUP BY Site ";

			var parm = new SqlParameter("@LedgerStartDate", SqlDbType.Date) { Value = startDate.Date };
			command.Parameters.Add(parm);

			if (this.managerGuid != Guid.Empty)
			{
				where = where + " AND ManagerCompanyGuid = @ManagerCompanyGuid";
				parm = new SqlParameter("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier) { Value = this.managerGuid };
				command.Parameters.Add(parm);
			}

			if (this.ownerGuid != Guid.Empty)
			{
				where = where + " AND OwnerCompanyGuid = @OwnerCompanyGuid";
				parm = new SqlParameter("@OwnerCompanyGuid", SqlDbType.UniqueIdentifier) { Value = this.ownerGuid };
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
				if (siteDo.SiteGroupFlag == false)
				{
					if (siteCount == 0)
					{
						where = where + " AND SiteGuid IN ( ";
					}

					string siteParmName = "@SiteGuid" + siteCount;
					where = where + siteParmName + ", ";

					parm = new SqlParameter(siteParmName, SqlDbType.UniqueIdentifier) { Value = siteDo.SiteGuid };
					command.Parameters.Add(parm);

					siteCount++;
				}
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
		/// closed out record for a given manager, owner, product, and site.
		/// </summary>
		/// <returns></returns>
		public void GetLatestCloseoutDateSingleSiteSelectSQL(SqlCommand command, DateTimeOffset startDate)
		{
			const string Select = "SELECT MAX(CloseoutDate) AS CloseoutDate ";
			const string From	= "FROM tblOwnerCloseout WITH (NOLOCK) ";
			string where		= "WHERE CloseoutDate >= @LedgerStartDate AND SiteGuid = @SiteGuid ";

			var parm = new SqlParameter("@LedgerStartDate", SqlDbType.Date) { Value = startDate.Date };
			command.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = this.siteGuid };
			command.Parameters.Add(parm);

			if (this.managerGuid != Guid.Empty)
			{
				where = where + "AND ManagerCompanyGuid = @ManagerGuid ";
				parm = new SqlParameter("@ManagerGuid", SqlDbType.UniqueIdentifier) { Value = this.managerGuid };
				command.Parameters.Add(parm);
			}

			if (this.ownerGuid != Guid.Empty)
			{
				where = where + "AND OwnerCompanyGuid = @OwnerGuid ";
				parm = new SqlParameter("@OwnerGuid", SqlDbType.UniqueIdentifier) { Value = this.ownerGuid };
				command.Parameters.Add(parm);
			}

			if (this.productGuid != Guid.Empty)
			{
				where = where + "AND ProductGuid = @ProductGuid ";
				parm = new SqlParameter("@ProductGuid", SqlDbType.UniqueIdentifier) { Value = this.productGuid };
				command.Parameters.Add(parm);
			}

			command.CommandText = Select + From + where;
		}

		/// <summary>
		/// This method will return an SQL that will retrieve the most current closeout date and
		/// other closeout data for a given owner, manager, product and less that the start date.
		/// </summary>
		public void GetCurrentOwnerCloseoutSelectSQL(SqlCommand command, List<LRSiteDO> siteList, DateTimeOffset ledgerStartDate)
		{
			const string Select2	= "SELECT SiteGuid, MAX (CloseoutDate) AS CloseoutDate ";
			const string From2		= "FROM tblOwnerCloseout WITH (NOLOCK) ";
			string where2			= "WHERE CloseoutDate < @LedgerStartDate ";
			const string GroupBy2	= "GROUP BY SiteGuid ) sq ON oc.SiteGuid = sq.SiteGuid ";

			const string Select1	= "SELECT oc.OwnerCloseoutGuid, oc.CloseoutDate, oc.ProductName, oc.ProductGuid, " +
									  "oc.ManagerName, oc.ManagerCompanyGuid, oc.OwnerName, oc.OwnerCompanyGuid, " +
			                          "oc.GrossBookInventory, oc.NetBookInventory, oc.MassBookInventory, " +
			                          "oc.GrossBookPrice, oc.NetBookPrice, oc.MassBookPrice, oc.Site, oc.SiteGuid ";
			const string From1		= "FROM tblOwnerCloseout oc WITH (NOLOCK) INNER JOIN ( ";
			string where1			= "WHERE oc.CloseoutDate = sq.CloseoutDate ";

			var parm = new SqlParameter("@LedgerStartDate", SqlDbType.Date) { Value = ledgerStartDate.Date };
			command.Parameters.Add(parm);

			// Make sure that the manager, owner and product info exists before placing
			// them into the where clause.
			if (this.managerGuid != Guid.Empty)
			{
				where1 = where1 + "AND oc.ManagerCompanyGuid = @ManagerCompanyGuid ";
				where2 = where2 + "AND ManagerCompanyGuid = @ManagerCompanyGuid ";
				parm = new SqlParameter("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier) { Value = this.managerGuid };
				command.Parameters.Add(parm);
			}

			if (this.productGuid != Guid.Empty)
			{
				where1 = where1 + "AND oc.ProductGuid = @ProductGuid ";
				where2 = where2 + "AND ProductGuid = @ProductGuid ";
				parm = new SqlParameter("@ProductGuid", SqlDbType.UniqueIdentifier) { Value = this.productGuid };
				command.Parameters.Add(parm);
			}

			if (this.ownerGuid != Guid.Empty)
			{
				where1 = where1 + "AND oc.OwnerCompanyGuid = @OwnerCompanyGuid ";
				where2 = where2 + "AND OwnerCompanyGuid = @OwnerCompanyGuid ";
				parm = new SqlParameter("@OwnerCompanyGuid", SqlDbType.UniqueIdentifier) { Value = this.ownerGuid };
				command.Parameters.Add(parm);
			}

			if (siteList.Count > 0)
			{
				where2 = where2 + " AND SiteGuid IN ( ";

				for (int nextSite = 0; nextSite < siteList.Count; nextSite++)
				{
					string siteParmName = "@SiteGuid" + nextSite;
					where2 = where2 + siteParmName + ", ";

					parm = new SqlParameter(siteParmName, SqlDbType.UniqueIdentifier) { Value = siteList[nextSite].SiteGuid };
					command.Parameters.Add(parm);
				}

				int lastComma = where2.LastIndexOf(',');
				where2 = where2.Remove(lastComma);
				where2 = where2 + " ) ";
			}

			command.CommandText = Select1 + From1 + Select2 + From2 + where2 + GroupBy2 + where1;
		}

		/// <summary>
		/// This method will return an SQL that will retrieve the most current closeout date and
		/// other closeout data for a given owner, manager, product and less that the start date.
		/// </summary>
		/// <returns></returns>
		public void GetCurrentOwnerCloseoutSingleSiteSelectSQL(SqlCommand command, DateTimeOffset ledgerStartDate)
		{
			const string Select = "SELECT TOP (1) CloseoutDate, OwnerCloseoutGuid, ProductName, ProductGuid, " +
			                       "ManagerName, ManagerCompanyGuid, OwnerName, OwnerCompanyGuid, " +
			                       "GrossBookInventory, NetBookInventory, MassBookInventory, " +
			                       "GrossBookPrice, NetBookPrice, MassBookPrice, Site, SiteGuid ";
			const string From	= "FROM tblOwnerCloseout WITH (NOLOCK) ";
			string where		= "WHERE CloseoutDate < @LedgerStartDate AND SiteGuid = @SiteGuid ";

			var parm = new SqlParameter("@LedgerStartDate", SqlDbType.Date) { Value = ledgerStartDate.Date };
			command.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = this.siteGuid };
			command.Parameters.Add (parm);

			// Make sure that the manager, owner and product info exists before placing
			// them into the where clause.
			if (this.managerGuid != Guid.Empty)
			{
				where = where + " AND ManagerCompanyGuid = @ManagerCompanyGuid";
				parm = new SqlParameter("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier) { Value = this.managerGuid };
				command.Parameters.Add(parm);
			}

			if (this.productGuid != Guid.Empty)
			{
				where = where + " AND ProductGuid = @ProductGuid";
				parm = new SqlParameter("@ProductGuid", SqlDbType.UniqueIdentifier) { Value = this.productGuid };
				command.Parameters.Add(parm);
			}

			if (this.ownerGuid != Guid.Empty)
			{
				where = where + " AND OwnerCompanyGuid = @OwnerCompanyGuid";
				parm = new SqlParameter("@OwnerCompanyGuid", SqlDbType.UniqueIdentifier) { Value = this.ownerGuid };
				command.Parameters.Add(parm);
			}

			const string order = " ORDER BY CloseoutDate DESC ";
			command.CommandText = Select + From + where + order;
		}		
		#endregion

		#region Load Methods
		/// <summary>
		/// This method will loads the most current closeout date and other closeout data 
		/// for a given owner, manager, product and less that the start date.
		/// </summary>
		/// <param name="row"></param>
		public void LoadCurrentOwnerCloseout(DataRow row)
		{
			if (row != null)
			{			
				this.ownerCloseoutGuid			= (row.IsNull("OwnerCloseoutGuid")) ? Guid.Empty : (Guid)row["OwnerCloseoutGuid"];
				this.productName				= (row.IsNull("ProductName")) ? string.Empty : (string)row["ProductName"];
				this.productGuid				= (row.IsNull("ProductGuid")) ? Guid.Empty : (Guid)row["ProductGuid"];
				this.managerName				= (row.IsNull("ManagerName")) ? string.Empty : (string)row["ManagerName"];
				this.managerGuid				= (row.IsNull("ManagerCompanyGuid")) ? Guid.Empty : (Guid)row["ManagerCompanyGuid"];
				this.ownerName					= (row.IsNull("OwnerName")) ? string.Empty : (string)row["OwnerName"];
				this.ownerGuid					= (row.IsNull("OwnerCompanyGuid")) ? Guid.Empty : (Guid)row["OwnerCompanyGuid"];
				this.bookInventory.Gross		= (row.IsNull("GrossBookInventory")) ? 0.0 : (double)row["GrossBookInventory"];
				this.bookInventory.Net			= (row.IsNull("NetBookInventory")) ? 0.0 : (double)row["NetBookInventory"];
				this.bookInventory.Mass			= (row.IsNull("MassBookInventory")) ? 0.0 : (double)row["MassBookInventory"];
				this.bookInventory.GrossPrice	= (row.IsNull("GrossBookPrice")) ? 0.0 : (double)row["GrossBookPrice"];
				this.bookInventory.NetPrice		= (row.IsNull("NetBookPrice")) ? 0.0 : (double)row["NetBookPrice"];
				this.bookInventory.MassPrice	= (row.IsNull("MassBookPrice")) ? 0.0 : (double)row["MassBookPrice"];
				this.siteName					= (row.IsNull("Site")) ? string.Empty : (string)row["Site"];
				this.siteGuid					= (row.IsNull("SiteGuid")) ? Guid.Empty : (Guid)row["SiteGuid"];

				if (row.IsNull("CloseoutDate") == false)
				{
					this.CloseoutDate = (DateTime)row["CloseoutDate"];
					this.foundOwnerCloseoutRecord = true;
				}

				this.bookInventory.GrossInventoryChange			*= this.volumeFactor;
				this.bookInventory.NetInventoryChange			*= this.volumeFactor;
				this.bookInventory.MassInventoryChange			*= this.massFactor;
				this.bookInventory.GrossPriceInventoryChange	*= this.volumeFactor;
				this.bookInventory.NetPriceInventoryChange		*= this.volumeFactor;
				this.bookInventory.MassInventoryChange			*= this.massFactor;

				this.bookInventory.GrossInventoryChange = Math.Round(this.bookInventory.GrossInventoryChange, this.volumePrecision, MidpointRounding.AwayFromZero);
				this.bookInventory.NetInventoryChange = Math.Round(this.bookInventory.NetInventoryChange, this.volumePrecision, MidpointRounding.AwayFromZero);
				this.bookInventory.MassInventoryChange = Math.Round(this.bookInventory.MassInventoryChange, this.massPrecision, MidpointRounding.AwayFromZero);
			}
		}

		/// <summary>
		/// This method will load the closed out record for a given manager, 
		/// owner, product, and site.
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
		/// This method will load the closed out record for a given manager, 
		/// owner, product, and site.
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
		#endregion

		#region Public methods
		/// <summary>
		/// This method will reset the object.
		/// </summary>
		/// <param name="defaultBeginDate"></param>
		public void Reset(DateTime? defaultBeginDate)
		{
			this.Initial(defaultBeginDate);
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will initialize the owner closeout class to its
		/// initial state.
		/// </summary>
		private void Initial(DateTime? defaultBeginDate)
		{
			this.bookInventory				= new LRQuantityDO();
			this.ownerCloseoutGuid			= Guid.Empty;
			this.productName				= string.Empty;
			this.productGuid				= Guid.Empty;
			this.managerName				= string.Empty;
			this.managerGuid				= Guid.Empty;
			this.ownerName					= string.Empty;
			this.ownerGuid					= Guid.Empty;
			this.bookInventory.Gross		= 0.0;
			this.bookInventory.Net			= 0.0;
			this.bookInventory.Mass			= 0.0;
			this.bookInventory.GrossPrice	= 0.0;
			this.bookInventory.NetPrice		= 0.0;
			this.bookInventory.MassPrice	= 0.0;
			this.createdBy					= string.Empty;
			this.updatedBy					= string.Empty;
			this.siteName					= string.Empty;
			this.siteGuid					= LedgerConstants.SiteAdminGuid;
			this.dateConverter				= new LRDateConverter();
			this.foundOwnerCloseoutRecord	= false;
			this.volumePrecision			= 2;
			this.massPrecision				= 2;
			this.volumeFactor				= 1.0;
			this.massFactor					= 1.0;

			if (defaultBeginDate == null)
			{
				this.closeoutDate = new DateTime(1901, 01, 01, 00, 00, 00);
				this.closeoutDateStr = this.dateConverter.ConvertToMonthDayYear(this.CloseoutDate);
			}
			else
			{
				this.closeoutDate = defaultBeginDate.Value.AddDays(-1);
				this.closeoutDateStr = this.dateConverter.ConvertToMonthDayYear(this.CloseoutDate);
			}
		}
		#endregion
	}
}