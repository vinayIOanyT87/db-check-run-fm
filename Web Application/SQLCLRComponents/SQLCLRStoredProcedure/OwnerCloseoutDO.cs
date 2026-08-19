using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class OwnerCloseoutDO
{
	#region Attributes
	private int ownerCloseoutID;
	private DateTime closeoutDate;
	private string productName;
	private int productIndex;
	private string managerName;
	private int managerIndex;
	private string ownerName;
	private int ownerIndex;
	private QuantityDO bookInventory;
	private DateTimeOffset createdDate;
	private DateTimeOffset updatedDate;
	private string createdBy;
	private string updatedBy;
	private string siteName;
	private int siteIndex;
	private string closeoutDateStr;
	DateConverter dateConverter;
	private bool foundOwnerCloseoutRecord;
	#endregion

	#region Constructor
	/// <summary>
	/// This is the default constructor for the owner closeout class.
	/// </summary>
	public OwnerCloseoutDO()
	{
		this.initial();
	}
	#endregion

	#region Properties
	/// <summary>
	/// This property return true if the owner closeout record was found
	/// (must have a valid closeout date).
	/// </summary>
	public bool FoundOwnerCloseoutRecord
	{
		get { return this.foundOwnerCloseoutRecord; }
	}

	/// <summary>
	/// This property sets and gets the owner closeout ID.
	/// </summary>
	public int OwnerCloseoutID
	{
		get { return this.ownerCloseoutID; }
		set { this.ownerCloseoutID = value; }
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
	/// This property sets and gets the product index.
	/// </summary>
	public int ProductIndex
	{
		get { return productIndex; }
		set { productIndex = value; }
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
	/// This property sets and gets the manager index.
	/// </summary>
	public int ManagerIndex
	{
		get { return managerIndex; }
		set { managerIndex = value; }
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
	/// This property sets and gets the owner index.
	/// </summary>
	public int OwnerIndex
	{
		get { return ownerIndex; }
		set { ownerIndex = value; }
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
	/// This property sets and gets the created date.
	/// </summary>
	public DateTimeOffset CreatedDate
	{
		get { return this.createdDate; }
		set { this.createdDate = value; }
	}

	/// <summary>
	/// This property sets and gets the updated date.
	/// </summary>
	public DateTimeOffset UpdatedDate
	{
		get { return this.updatedDate; }
		set { this.updatedDate = value; }
	}

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
	/// This property sets and gets the site index.
	/// </summary>
	public int SiteIndex
	{
		get { return siteIndex; }
		set { siteIndex = value; }
	}

	/// <summary>
	/// This property sets and gets the closeout date as a string.
	/// </summary>
	public string CloseoutDateString
	{
		get { return this.closeoutDateStr; }
		set { this.closeoutDateStr = value; }
	}
	#endregion

	#region SQL Methods
	/// <summary>
	/// This method will return the SQL that will get a list day that have been
	/// closed out record for a given manager, owner, product, and site.
	/// </summary>
	/// <returns></returns>
	public string GetLatestCloseoutDateSelectSQL()
	{
		string select = "SELECT MAX(CloseoutDate) AS CloseoutDate ";
		string from = "FROM tblOwnerCloseout ";
		string where = "WHERE CloseoutDate >= @LedgerStartDate ";

		if ((this.siteIndex > 0) || (this.siteIndex == -1))
		{
			where = where + " AND SiteIndex = @SiteIndex";
		}

		if (this.managerIndex > 0)
		{
			where = where + " AND ManagerIndex = @ManagerIndex";
		}

		if (this.ownerIndex > 0)
		{
			where = where + " AND OwnerIndex = @OwnerIndex";
		}

		if (this.productIndex > 0)
		{
			where = where + " AND ProductIndex = @ProductIndex";
		}

		return (select + from + where);
	}

	/// <summary>
	/// This method will return an SQL that will retrieve the most current closeout date and
	/// other closeout data for a given owner, manager, product and less that the start date.
	/// </summary>
	/// <returns></returns>
	public string GetCurrentOwnerCloseoutSelectSQL()
	{
		string select = "SELECT TOP 1 OwnerCloseoutID, CloseoutDate, ProductName, ProductIndex, " +
							  "ManagerName, ManagerIndex, OwnerName, OwnerIndex, " +
							  "GrossBookInventory, NetBookInventory, MassBookInventory, " +
							  "GrossBookPrice, NetBookPrice, MassBookPrice, Site, SiteIndex ";
		string from = " FROM tblOwnerCloseout ";
		string where = " WHERE CloseoutDate < @LedgerStartDate ";
		string orderBy = " ORDER BY CloseoutDate DESC";

		// Make sure that the manager, owner and product info exists before placing
		// them into the where clause.
		if (this.managerIndex > 0)
		{
			where = where + " AND ManagerIndex = @ManagerIndex";
		}

		if (this.productIndex > 0)
		{
			where = where + " AND ProductIndex = @ProductIndex";
		}

		if (this.ownerIndex > 0)
		{
			where = where + " AND OwnerIndex = @OwnerIndex";
		}

		if ((this.siteIndex > 0) || (this.siteIndex == -1))
		{
			where = where + " AND SiteIndex = @SiteIndex";
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
	public void LoadCurrentOwnerCloseout(System.Data.DataSet dataSet)
	{
		if (dataSet != null)
		{
			System.Data.DataTable table = dataSet.Tables[0];

			if (table.Rows.Count > 0)
			{
				System.Data.DataRow row = table.Rows[0];

				this.ownerCloseoutID = (row.IsNull("OwnerCloseoutID")) ? 0 : (int)row["OwnerCloseoutID"];
				this.productName = (row.IsNull("ProductName")) ? "" : (string)row["ProductName"];
				this.productIndex = (row.IsNull("ProductIndex")) ? -1 : (int)row["ProductIndex"];
				this.managerName = (row.IsNull("ManagerName")) ? "" : (string)row["ManagerName"];
				this.managerIndex = (row.IsNull("ManagerIndex")) ? -1 : (int)row["ManagerIndex"];
				this.ownerName = (row.IsNull("OwnerName")) ? "" : (string)row["OwnerName"];
				this.ownerIndex = (row.IsNull("OwnerIndex")) ? -1 : (int)row["OwnerIndex"];
				this.bookInventory.Gross = (row.IsNull("GrossBookInventory")) ? 0.0 : (double)row["GrossBookInventory"];
				this.bookInventory.Net = (row.IsNull("NetBookInventory")) ? 0.0 : (double)row["NetBookInventory"];
				this.bookInventory.Mass = (row.IsNull("MassBookInventory")) ? 0.0 : (double)row["MassBookInventory"];
				this.bookInventory.GrossPrice = (row.IsNull("GrossBookPrice")) ? 0.0 : (double)row["GrossBookPrice"];
				this.bookInventory.NetPrice = (row.IsNull("NetBookPrice")) ? 0.0 : (double)row["NetBookPrice"];
				this.bookInventory.MassPrice = (row.IsNull("MassBookPrice")) ? 0.0 : (double)row["MassBookPrice"];
				this.siteName = (row.IsNull("Site")) ? "" : (string)row["Site"];
				this.siteIndex = (row.IsNull("SiteIndex")) ? -1 : (int)row["SiteIndex"];

				if (row.IsNull("CloseoutDate") == false)
				{
					this.CloseoutDate = (DateTime)row["CloseoutDate"];
					this.foundOwnerCloseoutRecord = true;
				}
			}
		}
	}

	/// <summary>
	/// This method will load the closed out record for a given manager, 
	/// owner, product, and site.
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
	#endregion

	#region Private Methods
	/// <summary>
	/// This method will initialize the owner closeout class to its
	/// initial state.
	/// </summary>
	private void initial()
	{
		this.bookInventory = new QuantityDO();
		this.ownerCloseoutID = 0;
		this.productName = "";
		this.productIndex = -1;
		this.managerName = "";
		this.managerIndex = -1;
		this.ownerName = "";
		this.ownerIndex = -1;
		this.bookInventory.Gross = 0.0;
		this.bookInventory.Net = 0.0;
		this.bookInventory.Mass = 0.0;
		this.bookInventory.GrossPrice = 0.0;
		this.bookInventory.NetPrice = 0.0;
		this.bookInventory.MassPrice = 0.0;
		this.createdBy = "";
		this.updatedBy = "";
		this.siteName = "";
		this.siteIndex = -1;
		this.dateConverter = new DateConverter();
		this.foundOwnerCloseoutRecord = false;
	}
	#endregion
}
