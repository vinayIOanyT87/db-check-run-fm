using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CloseoutDO
{
	#region Attributes
	protected int closeoutInventoryID;
	protected string siteID;
	protected int siteIndex;
	protected string managerName;
	protected int managerIndex;
	protected string productName;
	protected int productIndex;
	protected DateTime closeoutDate;
	protected DateTime brokenBlendDate;
	protected DateTime lastCloseoutDate;

	private QuantityDO bookInventory;
	private QuantityDO totalPhysicalInventory;
	private QuantityDO totalVariance;
	private string closeoutDateStr;
	private string brokenBlendDateStr;

	private bool closeoutRecordFound;
	DateConverter dateConverter;
	#endregion Attributes

	#region Properties
	public string SiteID
	{
		get { return siteID; }
		set { siteID = value; }
	}

	public int SiteIndex
	{
		get { return siteIndex; }
		set { siteIndex = value; }
	}
	public string ManagerName
	{
		get { return managerName; }
		set { managerName = value; }
	}

	public int ManagerIndex
	{
		get { return managerIndex; }
		set { managerIndex = value; }
	}
	public string ProductName
	{
		get { return productName; }
		set { productName = value; }
	}

	public int ProductIndex
	{
		get { return productIndex; }
		set { productIndex = value; }
	}

	public System.DateTime CloseoutDate
	{
		get
		{
			DateTime smallDate = new DateTime(1901, 01, 01, 00, 00, 00);

			if (this.closeoutDate < smallDate)
			{
				return smallDate;
			}
			else
			{
				return this.closeoutDate;
			}
		}

		set { this.closeoutDate = value; }
	}

	public System.DateTime LastCloseoutDate
	{
		get { return this.lastCloseoutDate; }
		set { this.lastCloseoutDate = value; }
	}

	public System.DateTime BrokenBlendDate
	{
		get { return this.brokenBlendDate; }
		set { this.brokenBlendDate = value; }
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
			QuantityDO variance = new QuantityDO(totalPhysicalInventory.Gross - bookInventory.Gross,
															totalPhysicalInventory.Net - bookInventory.Net,
															totalPhysicalInventory.Mass - bookInventory.Mass,
															totalPhysicalInventory.Package - bookInventory.Package,
															totalPhysicalInventory.GrossPrice - bookInventory.GrossPrice,
															totalPhysicalInventory.NetPrice - bookInventory.NetPrice,
															totalPhysicalInventory.MassPrice - bookInventory.Mass);
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
	#endregion Properties

	#region Constructors
	/// <summary>
	/// This is the default constructor for the Closeout data object.
	/// </summary>
	public CloseoutDO()
	{
		this.lastCloseoutDate = new DateTime(1900, 1, 1);
		this.bookInventory = new QuantityDO();
		this.totalPhysicalInventory = new QuantityDO();
		this.totalVariance = new QuantityDO();
		this.dateConverter = new DateConverter();
		this.closeoutRecordFound = false;
	}
	#endregion

	#region SQL Methods
	/// <summary>
	/// This method will return the SQL that will get a last day that has 
	/// a broken blend for a given manager, product, and site.
	/// </summary>
	/// <returns></returns>
	public string GetBrokenBlendDateSelectSQL()
	{
		string select = "SELECT MIN(TransactionInventoryDate) AS BrokenBlendDate ";
		string from = " FROM tblTransactionSubLineItems WITH(NOLOCK)";
		string where = " WHERE TransIndex IN" +
							 " (SELECT TransIndex" +
							 " FROM tblTransactions WITH(NOLOCK)" +
							 " WHERE InventoryDate > @LastCloseoutDate " +
							 " AND InventoryDate <= @LedgerEndDate " +
							 " AND SiteIndex = @SiteIndex " +
							 " AND ManagerIndex = @ManagerIndex " +
							 " AND (ReversalType IS NULL OR ReversalType = 'U')" +
							 " AND (DeleteFlag = 0 OR DeleteFlag = NULL))" +
							 " AND ProductIndex = @ProductIndex " +
							 " AND BrokenBlend = 1";

		return (select + from + where);
	}

	/// <summary>
	/// This method will return the SQL that will get a list day that have been
	/// closed out record for a given manager, product, and site.
	/// </summary>
	/// <returns></returns>
	public string GetLatestCloseoutDateSelectSQL()
	{
		string select = "SELECT MAX(CloseoutDate) AS CloseoutDate ";
		string from = "FROM tblCloseoutInventory ";
		string where = "WHERE CloseoutDate >= @LedgerStartDate ";

		if ((this.siteIndex > 0) || (this.siteIndex == -1))
		{
			where = where + " AND SiteIndex = @SiteIndex ";
		}

		if (this.managerIndex > 0)
		{
			where = where + " AND ManagerIndex = @ManagerIndex ";
		}

		if (this.productIndex > 0)
		{
			where = where + " AND ProductIndex = @ProductIndex";
		}

		return (select + from + where);
	}


	/// <summary>
	/// This method will return an SQL that will retrieve the most current closeout date and
	/// other closeout data for a given site, manager, product and less that the start date.
	/// </summary>
	/// <returns></returns>
	public string GetCurrentCloseoutSelectSQL()
	{
		string select = "SELECT TOP 1 CloseOutInventoryID, Site, SiteIndex, CloseoutDate, " +
							  " ProductName, ProductIndex, ManagerName, ManagerIndex, " +
							  " GrossBookInventory, NetBookInventory, MassBookInventory, " +
							  " GrossPhysicalInventory, NetPhysicalInventory, MassPhysicalInventory, " +
							  " GrossVariance, NetVariance, MassVariance, " +
							  " GrossBookPrice, NetBookPrice, MassBookPrice, " +
							  " GrossPhysicalPrice, NetPhysicalPrice, MassPhysicalPrice ";
		string from = "FROM tblCloseoutInventory ";
		string where = "WHERE CloseoutDate < @LedgerStartDate ";
		string orderBy = "ORDER BY CloseoutDate DESC";

		// Make sure that the manager, product, and site info exists before placing
		// them into the where clause.
		if (this.managerIndex > 0)
		{
			where = where + " AND ManagerIndex = @ManagerIndex ";
		}

		if (this.productIndex > 0)
		{
			where = where + " AND ProductIndex = @ProductIndex ";
		}

		if ((this.siteIndex > 0) == (this.siteIndex == -1))
		{
			where = where + " AND SiteIndex = @SiteIndex ";
		}

		return (select + from + where + orderBy);
	}
	#endregion

	#region Load Methods
	/// <summary>
	/// This method will loads the most current closeout date and other closeout data 
	/// for a given owner, manager, product and less that the start date.
	/// </summary>
	/// <param name="dataSet"></param>
	public void LoadCloseout(System.Data.DataSet dataSet)
	{
		if (dataSet != null)
		{
			System.Data.DataTable table = dataSet.Tables[0];

			if (table.Rows.Count > 0)
			{
				System.Data.DataRow row = table.Rows[0];

				this.closeoutInventoryID = (row.IsNull("CloseOutInventoryID")) ? 0 : (int)row["CloseOutInventoryID"];
				this.siteID = (row.IsNull("Site")) ? "" : (string)row["Site"];
				this.siteIndex = (row.IsNull("SiteIndex")) ? -1 : (int)row["SiteIndex"];
				this.productName = (row.IsNull("ProductName")) ? "" : (string)row["ProductName"];
				this.productIndex = (row.IsNull("ProductIndex")) ? -1 : (int)row["ProductIndex"];
				this.managerName = (row.IsNull("ManagerName")) ? "" : (string)row["ManagerName"];
				this.managerIndex = (row.IsNull("ManagerIndex")) ? -1 : (int)row["ManagerIndex"];
				this.bookInventory.Gross = (row.IsNull("GrossBookInventory")) ? 0.0 : (double)row["GrossBookInventory"];
				this.bookInventory.Net = (row.IsNull("NetBookInventory")) ? 0.0 : (double)row["NetBookInventory"];
				this.bookInventory.Mass = (row.IsNull("MassBookInventory")) ? 0.0 : (double)row["MassBookInventory"];
				this.totalPhysicalInventory.Gross = (row.IsNull("GrossPhysicalInventory")) ? 0.0 : (double)row["GrossPhysicalInventory"];
				this.totalPhysicalInventory.Net = (row.IsNull("NetPhysicalInventory")) ? 0.0 : (double)row["NetPhysicalInventory"];
				this.totalPhysicalInventory.Mass = (row.IsNull("MassPhysicalInventory")) ? 0.0 : (double)row["MassPhysicalInventory"];
				this.totalVariance.Gross = (row.IsNull("GrossVariance")) ? 0.0 : (double)row["GrossVariance"];
				this.totalVariance.Net = (row.IsNull("NetVariance")) ? 0.0 : (double)row["NetVariance"];
				this.totalVariance.Mass = (row.IsNull("MassVariance")) ? 0.0 : (double)row["MassVariance"];
				this.bookInventory.GrossPrice = (row.IsNull("GrossBookPrice")) ? 0.0 : (double)row["GrossBookPrice"];
				this.bookInventory.NetPrice = (row.IsNull("NetBookPrice")) ? 0.0 : (double)row["NetBookPrice"];
				this.bookInventory.MassPrice = (row.IsNull("MassBookPrice")) ? 0.0 : (double)row["MassBookPrice"];
				this.totalPhysicalInventory.GrossPrice = (row.IsNull("GrossPhysicalPrice")) ? 0.0 : (double)row["GrossPhysicalPrice"];
				this.totalPhysicalInventory.NetPrice = (row.IsNull("NetPhysicalPrice")) ? 0.0 : (double)row["NetPhysicalPrice"];
				this.totalPhysicalInventory.MassPrice = (row.IsNull("MassPhysicalPrice")) ? 0.0 : (double)row["MassPhysicalPrice"];

				if (row.IsNull("CloseoutDate") == false)
				{
					this.CloseoutDate = (DateTime)row["CloseoutDate"];
				}

				closeoutRecordFound = true;
			}
		}
	}

	/// <summary>
	/// This method will load the closed out date for a given manager, 
	/// product, and site.
	/// </summary>
	/// <param name="dataSet"></param>
	public void LoadLatestCloseoutDate(System.Data.DataSet dataSet)
	{
		this.closeoutDateStr = "";

		if (dataSet != null)
		{
			System.Data.DataTable table = dataSet.Tables[0];

			if (table.Rows.Count > 0)
			{
				System.Data.DataRow row = table.Rows[0];

				if (row.IsNull("CloseoutDate") == false)
				{
					this.CloseoutDate = (DateTime)row["CloseoutDate"];
					this.closeoutDateStr = this.dateConverter.ConvertToMonthDayYear(this.CloseoutDate);
				}
			}
		}
	}

	/// <summary>
	/// This method will load the broken blend date for a given manager, 
	/// product, and site.
	/// </summary>
	/// <param name="dataSet"></param>
	public void LoadBrokenBlendDate(System.Data.DataSet dataSet)
	{
		this.brokenBlendDateStr = "";

		if (dataSet != null)
		{
			System.Data.DataTable table = dataSet.Tables[0];

			if (table.Rows.Count > 0)
			{
				System.Data.DataRow row = table.Rows[0];

				if (row.IsNull("BrokenBlendDate") == false)
				{
					this.BrokenBlendDate = (DateTime)row["BrokenBlendDate"];
					this.brokenBlendDateStr = this.dateConverter.ConvertToMonthDayYear(this.BrokenBlendDate);
				}
			}
		}
	}
	#endregion
}
