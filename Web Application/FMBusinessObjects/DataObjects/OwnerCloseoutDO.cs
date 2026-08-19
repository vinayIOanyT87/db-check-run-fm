using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class OwnerCloseoutDO : DataObject
	{
		#region Attributes

		[DataMember]
		private Guid ownerCloseoutGuid;
		[DataMember]
		private DateTimeOffset? closeoutDate;
		[DataMember]
		private string productName;
		[DataMember]
		private Guid productGuid;
		[DataMember]
		private string managerName;
		[DataMember]
		private Guid managerGuid;
		[DataMember]
		private string ownerName;
		[DataMember]
		private Guid ownerGuid;
		[DataMember]
		private QuantityDO bookInventory;
		[DataMember]
		private DateTimeOffset? createdDate;
		[DataMember]
		private DateTimeOffset? updatedDate;
		[DataMember]
		private string createdBy;
		[DataMember]
		private string updatedBy;
		[DataMember]
		private string siteName;
		[DataMember]
		private Guid siteGuid;
		private const int EMPTY_STRING = 0;
		[DataMember]
		private string closeoutDateStr;
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
		/// This property sets and gets the owner closeout ID.
		/// </summary>
		public Guid OwnerCloseoutGuid
		{
			get { return this.ownerCloseoutGuid; }
			set { this.ownerCloseoutGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the closeout date.
		/// </summary>
		public DateTimeOffset? CloseoutDate
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
			get { return productGuid; }
			set { productGuid = value; }
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
			get { return managerGuid; }
			set { managerGuid = value; }
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
			get { return ownerGuid; }
			set { ownerGuid = value; }
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
		public DateTimeOffset? CreatedDate
		{
			get { return this.createdDate; }
			set { this.createdDate = value; }
		}

		/// <summary>
		/// This property sets and gets the updated date.
		/// </summary>
		public DateTimeOffset? UpdatedDate
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
		/// This property sets and gets the site Guid.
		/// </summary>
		public Guid SiteGuid
		{
			get { return siteGuid; }
			set { siteGuid = value; }
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
		/// <param name="ledgerSR"></param>
		/// <param name="siteName"></param>
		/// <returns></returns>
		public string getLatestCloseoutDateSelectSQL(LedgerSR ledgerSR, string siteName)
		{
			string select = "SELECT MAX(CloseoutDate) ";
			string from = "FROM tblOwnerCloseout ";
			string where = "WHERE CloseoutDate >= '" + ledgerSR.GetLedgerStartDate() + "' ";

			if ((siteName != null) && (siteName.Length != EMPTY_STRING))
				where = where + "AND Site = '" + siteName + "' ";

			if ((ledgerSR.Manager != null) && (ledgerSR.Manager.Length != EMPTY_STRING))
				where = where + "AND ManagerName = '" + ledgerSR.Manager + "' ";

			if ((ledgerSR.Owner != null) && (ledgerSR.Owner.Length != EMPTY_STRING))
				where = where + "AND OwnerName = '" + ledgerSR.Owner + "' ";

			if ((ledgerSR.Product != null) && (ledgerSR.Product.Length != EMPTY_STRING))
				where = where + "AND ProductName = '" + ledgerSR.Product + "' ";

			return (select + from + where);
		}

		/// <summary>
		/// This method will return an SQL that will retrieve the most current closeout date and
		/// other closeout data for a given owner, manager, product and less that the start date.
		/// </summary>
		/// <param name="ledgerSR"></param>
		/// <returns></returns>
		public string getCurrentOwnerCloseoutSelectSQL(LedgerSR ledgerSR, string siteName)
		{
			string select = "SELECT TOP 1 OwnerCloseoutGuid, CloseoutDate, ProductName, ProductGuid, " +
						"ManagerName, ManagerCompanyGuid, OwnerName, OwnerCompanyGuid, GrossBookInventory, NetBookInventory, GrossBookPrice, " +
							"NetBookPrice, Site, SiteGuid ";
			string from = "FROM tblOwnerCloseout ";
			string where = "WHERE CloseoutDate < '" + ledgerSR.GetLedgerStartDate() + "' ";

			string orderBy = "ORDER BY CloseoutDate DESC";
			// Make sure that the manager, owner and product info exists before placing
			// them into the where clause.
			if ((ledgerSR.Manager != null) && (ledgerSR.Manager.Length != EMPTY_STRING))
				where = where + "AND ManagerName = '" + ledgerSR.Manager + "' ";

			if ((ledgerSR.Product != null) && (ledgerSR.Product.Length != EMPTY_STRING))
				where = where + "AND ProductName = '" + ledgerSR.Product + "' ";

			if ((ledgerSR.Owner != null) && (ledgerSR.Owner.Length != EMPTY_STRING))
				where = where + "AND OwnerName = '" + ledgerSR.Owner + "' ";

			if ((siteName != null) && (siteName.Length != EMPTY_STRING))
				where = where + "AND Site = '" + siteName + "' ";

			return (select + from + where + orderBy);
		}
		#endregion

		#region Load Methods
		/// <summary>
		/// This method will loads the most current closeout date and other closeout data 
		/// for a given owner, manager, product and less that the start date.
		/// </summary>
		/// <param name="dataSet"></param>
		public void loadCurrentOwnerCloseout(System.Data.DataSet dataSet)
		{
			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					System.Data.DataRow row = table.Rows[0];

					loadCurrentOwnerCloseoutUsingColumnName(row);
				}
			}
		}

		/// <summary>
		/// This method will loads a data row.
		/// </summary>
		/// <param name="row"></param>
		public void loadCurrentOwnerCloseoutUsingColumnName(System.Data.DataRow row)
		{
			if (row != null)
			{

				this.ownerCloseoutGuid = DataObject.getValue<Guid>(row["OwnerCloseoutGuid"], Guid.Empty);
				this.CloseoutDate = DataObject.getValue<DateTimeOffset>(row["CloseoutDate"], TimeConverter.Today());
				this.productName = DataObject.getValue<string>(row["ProductName"], "");
				this.productGuid = DataObject.getValue<Guid>(row["ProductGuid"], Guid.Empty);
				this.managerName = DataObject.getValue<string>(row["ManagerName"], "");
				this.managerGuid = DataObject.getValue<Guid>(row["ManagerCompanyGuid"], Guid.Empty);
				this.ownerName = DataObject.getValue<string>(row["OwnerName"], "");
				this.ownerGuid = DataObject.getValue<Guid>(row["OwnerCompanyGuid"], Guid.Empty);
				this.bookInventory.Gross = DataObject.getValue<double>(row["GrossBookInventory"], 0.0);
				this.bookInventory.Net = DataObject.getValue<double>(row["NetBookInventory"], 0.0);
				this.bookInventory.GrossPrice = DataObject.getValue<double>(row["GrossBookPrice"], 0.0);
				this.bookInventory.NetPrice = DataObject.getValue<double>(row["NetBookPrice"], 0.0);
				this.bookInventory.MassPrice = DataObject.getValue<double>(row["MassBookPrice"], 0.0);
				this.siteName = DataObject.getValue<string>(row["Site"], "");
				this.siteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
				this.createdDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
				this.createdBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
				this.updatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], createdDate.Value);
				this.updatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
				this.bookInventory.Mass = DataObject.getValue<double>(row["MassBookInventory"], 0.0);
				// ?? "TransVersion"  

			}
		}

		/// <summary>
		/// This method will load the closed out record for a given manager, 
		/// owner, product, and site.
		/// </summary>
		/// <param name="dataSet"></param>
		public void loadLatestCloseoutDate(System.Data.DataSet dataSet)
		{
			this.closeoutDateStr = "";

			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					System.Data.DataRow row = table.Rows[0];
					object dateObj = row["CloseoutDate"];
					bool dateExists = !isNull(dateObj);

					if (dateExists == true)
					{
						this.CloseoutDate = (DateTimeOffset)dateObj;
						this.closeoutDateStr = DateEfficacy.convertToMonthDayYear(this.CloseoutDate.Value);
					}
				}
			}
		}


		public string SQLToRetriveAssociatedOwnerCloseOutRecs(DateTimeOffset CloseoutDate, Guid managerGuid, Guid productGuid)
		{

			string sqlSelect = "SELECT [OwnerCloseoutGuid], [Site], [SiteGuid], [ManagerName], [ManagerC], [ProductName], [ProductGuid], " +
					"[CloseoutDate], [OwnerName], [OwnerCompanyGuid], [GrossBookInventory], [NetBookInventory], [CreatedDate], [CreatedBy], " +
					"[UpdatedDate], [UpdatedBy], [GrossBookPrice], [NetBookPrice], [TransVersion], [MassBookInventory], [MassBookPrice] " +
					"FROM [tblOwnerCloseout] ";

			string sqlWhere = String.Format(" WHERE [CloseoutDate] = '{0}'  AND  [ManagerCompanyGuid] = {1}  AND [ProductGuid] = {2} ", CloseoutDate, managerGuid, productGuid);


			string sql = sqlSelect + sqlWhere;

			return sql;
		}

		#endregion

		#region Override Methods
		override public string getUpdateCommand()
		{
			return null;
		}

		override public string getDeleteCommand()
		{
			return null;
		}

		override public string getInsertCommand()
		{
			return null;
		}

		override public string getSelectCommand()
		{
			return null;
		}

		private void prepopulate()
		{
			// Prepopulate the object fields with the static default values.
		}

		private void init()
		{
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
			this.ownerCloseoutGuid = Guid.Empty;
			this.productName = "";
			this.productGuid = Guid.Empty;
			this.managerName = "";
			this.managerGuid = Guid.Empty;
			this.ownerName = "";
			this.ownerGuid = Guid.Empty;
			this.bookInventory.Gross = 0.0;
			this.bookInventory.Net = 0.0;
			this.bookInventory.Mass = 0.0;
			this.bookInventory.Package = 0.0;
			this.bookInventory.GrossPrice = 0.0;
			this.bookInventory.NetPrice = 0.0;
			this.bookInventory.MassPrice = 0.0;
			this.createdBy = "";
			this.updatedBy = "";
			this.siteName = "";
			this.siteGuid = Guid.Empty;
			this.closeoutDate = null;
			this.createdDate = null;
			this.updatedDate = null;
		}
		#endregion
	}
}
