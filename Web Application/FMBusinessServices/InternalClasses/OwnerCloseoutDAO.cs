// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwnerCloseoutDAO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.ServiceClasses;

	public class OwnerCloseoutDAO
	{
		#region Constants and Fields

		protected CloseoutSR sr;

		private readonly ConsolidatedDAClass consolidatedDA;

		private readonly SecurityClass security;

		private QuantityDO bookInv;

		private Guid companyGuid;

		private string companyID;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="OwnerCloseoutDAO"/> class. 
		/// This is the default constructor.
		/// </summary>
		/// <param name="inSecurity">
		/// The in Security.
		/// </param>
		public OwnerCloseoutDAO(SecurityClass inSecurity)
		{
			this.consolidatedDA = new ConsolidatedDAClass();
			this.security = inSecurity;
		}

		#endregion

		#region Public Properties

		/// <summary>
		///     This property is used by the closeout processor for database
		///     transactions.
		/// </summary>
		public QuantityDO BookInv
		{
			get
			{
				return this.bookInv;
			}

			set
			{
				this.bookInv = value;
			}
		}

		/// <summary>
		///     This property is used by the closeout processor for database
		///     transactions.
		/// </summary>
		public Guid CompanyGuid
		{
			get
			{
				return this.companyGuid;
			}

			set
			{
				this.companyGuid = value;
			}
		}

		/// <summary>
		///     This property is used by the closeout processor for database
		///     transactions.
		/// </summary>
		public string CompanyID
		{
			get
			{
				return this.companyID;
			}

			set
			{
				this.companyID = value;
			}
		}

		/// <summary>
		///     This property is used by the closeout processor for database
		///     transactions.
		/// </summary>
		public CloseoutSR SR
		{
			get
			{
				return this.sr;
			}

			set
			{
				this.sr = value;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// This method initialized an Sstarts the closeout process and emcompasses the whole process.
		/// </summary>
		/// <param name="Cmd">
		/// The Cmd.
		/// </param>
		public static void InsertSQL(SqlCommand Cmd)
		{
			Cmd.CommandText = "INSERT INTO tblOwnerCloseout "
			                  + "(Site, SiteGuid, ManagerName, ManagerCompanyGuid, ProductName, ProductGuid, "
			                  + " CloseoutDate, OwnerName, OwnerCompanyGuid, GrossBookInventory, "
			                  + "NetBookInventory, MassBookInventory, GrossBookPrice, NetBookPrice, MassBookPrice, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) "
			                  + "VALUES "
			                  + "(@Site, @SiteGuid, @ManagerName, @ManagerCompanyGuid, @ProductName, @ProductGuid, @CloseoutDate, "
			                  + "@OwnerName, @OwnerCompanyGuid, @GrossBookInventory, @NetBookInventory, @MassBookInventory, @GrossBookPrice, @NetBookPrice, @MassBookPrice, "
			                  + "SYSDATETIMEOFFSET(), @CreatedBy, SYSDATETIMEOFFSET(), @UpdatedBy) ";

			Cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 30);
			Cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			Cmd.Parameters.Add("@ManagerName", SqlDbType.NVarChar, 100);
			Cmd.Parameters.Add("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier);
			Cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 30);
			Cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			Cmd.Parameters.Add("@CloseoutDate", SqlDbType.Date);
			Cmd.Parameters.Add("@OwnerName", SqlDbType.NVarChar, 100);
			Cmd.Parameters.Add("@OwnerCompanyGuid", SqlDbType.UniqueIdentifier);
			Cmd.Parameters.Add("@GrossBookInventory", SqlDbType.Float);
			Cmd.Parameters.Add("@NetBookInventory", SqlDbType.Float);
			Cmd.Parameters.Add("@MassBookInventory", SqlDbType.Float);
			Cmd.Parameters.Add("@GrossBookPrice", SqlDbType.Float);
			Cmd.Parameters.Add("@NetBookPrice", SqlDbType.Float);
			Cmd.Parameters.Add("@MassBookPrice", SqlDbType.Float);
			Cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			Cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
		}

		/// <summary>
		/// This method starts the closeout process and emcompasses the whole process.
		/// </summary>
		/// <param name="sr">
		/// </param>
		public void Closeout(CloseoutSR sr)
		{
			this.sr = sr;
			DateTimeOffset? beginDate = this.GetStartDate(sr);

			var companies = new CompaniesClass();
			CompanyCollectionClass companyCollection = companies.EnumerateByRole(sr.Security, COMPANY_ROLE.OWNER, false);

			foreach (CompanyClass company in companyCollection)
			{
				this.companyID = company.ID;
				this.CompanyGuid = company.MasterRecordGuid;
				this.CreateOwnerCloseout(beginDate);
			}
		}

		/// <summary>
		/// This method is a legacy method to create an owner closeout. The date is not
		///     used, but is there for legacy reasons.
		/// </summary>
		/// <param name="beginDate">
		/// </param>
		private void CreateOwnerCloseout(DateTimeOffset? beginDate)
		{
			// Retrieve the book inventory for the manager, owner, product.
			this.bookInv = this.GetBookInventories();
			using (var cmd = new SqlCommand())
			{
				this.CreateOwnerCloseout(cmd);
			}
		}

		/// <summary>
		/// This method creates the owner closeout record.
		/// </summary>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		public void CreateOwnerCloseout(SqlCommand cmd)
		{
			// Create a SQL command object if not using database transactions. If using
			// database transactions, then the client is set the SQL command object.
			if (ContextUtil.IsInTransaction == false)
			{
				InsertSQL(cmd);
			}

			int i = 0;

			cmd.Parameters[i++].Value = this.sr.Site;
			cmd.Parameters[i++].Value = this.sr.CurrentSiteGuid;
			cmd.Parameters[i++].Value = this.sr.ManagerName;
			cmd.Parameters[i++].Value = this.sr.ManagerCompanyGuid;
			cmd.Parameters[i++].Value = this.sr.ProductName;
			cmd.Parameters[i++].Value = this.sr.ProductGuid;
			cmd.Parameters[i++].Value = this.sr.InventoryDate;
			cmd.Parameters[i++].Value = this.companyID;
			cmd.Parameters[i++].Value = this.companyGuid;
			cmd.Parameters[i++].Value = this.bookInv.GrossInventoryChange;
			cmd.Parameters[i++].Value = this.bookInv.NetInventoryChange;
			cmd.Parameters[i++].Value = this.bookInv.MassInventoryChange;
			cmd.Parameters[i++].Value = this.bookInv.GrossPriceInventoryChange;
			cmd.Parameters[i++].Value = this.bookInv.NetPriceInventoryChange;
			cmd.Parameters[i++].Value = this.bookInv.MassPriceInventoryChange;
			cmd.Parameters[i++].Value = this.sr.Security.UserID;
			cmd.Parameters[i++].Value = this.sr.Security.UserID;

			this.consolidatedDA.ExecuteQuery(this.security, cmd);
		}

		public OwnerCloseoutDO CreateOwnerCloseoutDO()
		{
			var ownrClsDO = new OwnerCloseoutDO();

			ownrClsDO.SiteName = this.sr.Site;
			ownrClsDO.SiteGuid = this.sr.CurrentSiteGuid;
			ownrClsDO.ManagerName = this.sr.ManagerName;
			ownrClsDO.ManagerGuid = this.sr.ManagerCompanyGuid;
			ownrClsDO.ProductName = this.sr.ProductName;
			ownrClsDO.ProductGuid = this.sr.ProductGuid;
			ownrClsDO.CloseoutDate = this.sr.InventoryDate;
			ownrClsDO.OwnerName = this.companyID;
			ownrClsDO.OwnerGuid = this.companyGuid;
			ownrClsDO.BookInventory.GrossInventoryChange = this.bookInv.GrossInventoryChange;
			ownrClsDO.BookInventory.NetInventoryChange = this.bookInv.NetInventoryChange;
			ownrClsDO.BookInventory.MassInventoryChange = this.bookInv.MassInventoryChange;
			ownrClsDO.BookInventory.GrossPriceInventoryChange = this.bookInv.GrossPriceInventoryChange;
			ownrClsDO.BookInventory.NetPriceInventoryChange = this.bookInv.NetPriceInventoryChange;
			ownrClsDO.BookInventory.MassPriceInventoryChange = this.bookInv.MassPriceInventoryChange;
			ownrClsDO.CreatedDate = this.sr.InventoryDate;
			ownrClsDO.CreatedBy = this.sr.Security.UserID;
			ownrClsDO.UpdatedDate = DateTimeOffset.Now;
			ownrClsDO.UpdatedBy = this.sr.Security.UserID;

			return ownrClsDO;
		}

		/// <summary>
		/// This method will retrieve the gross and book inventories based on the manager,
		///     owner, and product. Returns a volume object.
		/// </summary>
		/// <returns>
		/// The <see cref="QuantityDO"/>.
		/// </returns>
		public QuantityDO GetBookInventories(AccountingSite accountingSite = null)
		{
			var sites=new SitesClass();
			var site = sites.Get(this.sr.Security, this.sr.Security.SiteGuid, false, false, false);

			var ledgerSR = new LedgerSR();
			ledgerSR.Security = this.sr.Security;
			ledgerSR.Manager = this.sr.ManagerName;
			ledgerSR.Product = this.sr.ProductName;
			ledgerSR.Month = this.sr.InventoryDate.ToString("MMMM yyyy",site.GetDateTimeFormatInfo());
			ledgerSR.Site = this.sr.Security.SiteID;
			ledgerSR.CurrentSiteGuid = this.sr.Security.SiteGuid;
			ledgerSR.SetRequestType(LedgerSR.LedgerRequests.Refresh);
			ledgerSR.Owner = this.companyID;
			ledgerSR.OwnerMasterGuid = this.CompanyGuid;
			ledgerSR.ManagerMasterGuid = this.sr.ManagerCompanyGuid;

			var ledgerProcessor = new LedgerProcessorClass();
			LedgerDO ledgerDO = ledgerProcessor.Process(ledgerSR, accountingSite);

			// Find the total on the inventory date passed, don't assume the last day of the month. This fixes CSI #6017 (07-Jul-2008 IGO)
			var lineItem = ledgerDO.LedgerLineItems[this.sr.InventoryDate.Day - 1] as LedgerLineItemDO;

			QuantityDO bookInv = new QuantityDO(lineItem.BookInventory.GrossInventoryChange,
															lineItem.BookInventory.NetInventoryChange,
															lineItem.BookInventory.MassInventoryChange,
															0.0,
															lineItem.BookInventory.GrossPriceInventoryChange,
															lineItem.BookInventory.NetPriceInventoryChange,
															lineItem.BookInventory.MassPriceInventoryChange);

			return bookInv;
		}

		/// <summary>
		/// This method will retrieve the start start for the last closeout. Not used
		///     by the manual closeout process.
		/// </summary>
		/// <param name="srParam">
		/// </param>
		/// <returns>
		/// The nullable <see cref="DateTimeOffset"/>.
		/// </returns>
		public DateTimeOffset? GetStartDate(CloseoutSR srParam)
		{
            CompaniesClass companies = new CompaniesClass();
		    ProductsClass products = new ProductsClass();

		    var listSR = new CloseoutListSR
		                 {
		                     Security = srParam.Security,
		                     Site = srParam.Site,
		                     CurrentSiteGuid = srParam.CurrentSiteGuid,
		                     StartDate = srParam.InventoryDate.AddDays(-1)
		                 };

		    // We just inserted this closeout record, so look for prior record.
		    listSR.EndDate = listSR.StartDate;
			listSR.ManagerGuid = companies.GetMasterRecordGuid(srParam.Security, srParam.ManagerName);
			listSR.ProductGuid = products.GetMasterRecordGuidFromID(srParam.Security, srParam.ProductName);

			var closeoutListProcessor = new CloseoutListProcessorClass();
			CloseoutListDO listDO = closeoutListProcessor.Process(listSR);

			DateTimeOffset? beginDate = null;
			CloseoutDO closeoutRecord;

			if (listDO.CloseoutList.Count > 0)
			{
				closeoutRecord = (CloseoutDO)listDO.CloseoutList[listDO.CloseoutList.Count - 1];
				beginDate = closeoutRecord.CloseoutDate;
			}
			else if (listDO.PriorCloseout != null)
			{
				closeoutRecord = listDO.PriorCloseout;
				beginDate = closeoutRecord.CloseoutDate;
			}

			return beginDate;
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method will retrieve the previous book inventory from the owner closeout
		///     table for a particular manager, product, and owner.
		/// </summary>
		/// <param name="sr">
		/// </param>
		/// <param name="beginDate">
		/// </param>
		/// <param name="previousGrossBookInventory">
		/// </param>
		/// <param name="previousNetBookInventory">
		/// </param>
		/// <param name="previousGrossBookPrice">
		/// </param>
		/// <param name="previousNetBookPrice">
		/// </param>
		protected void GetPreviousBookInventory(
			CloseoutSR sr, 
			DateTimeOffset? beginDate, 
			out double previousGrossBookInventory, 
			out double previousNetBookInventory, 
			out double previousGrossBookPrice, 
			out double previousNetBookPrice)
		{
			previousGrossBookInventory = 0;
			previousNetBookInventory = 0;
			previousGrossBookPrice = 0;
			previousNetBookPrice = 0;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT GrossBookInventory, NetBookInventory, GrossBookPrice, NetBookPrice "
				                  + "FROM tblOwnerCloseout " + "WHERE Site = @Site AND ManagerName = @ManagerName AND "
				                  + "ProductName = @ProductName AND OwnerName = @OwnerName";

				cmd.Parameters.Add("@Site", SqlDbType.NVarChar, 30);
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@ManagerName", SqlDbType.NVarChar, 100);
				cmd.Parameters.Add("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 30);
				cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@OwnerName", SqlDbType.NVarChar, 100);
				cmd.Parameters.Add("@OwnerCompanyGuid", SqlDbType.UniqueIdentifier);

				int i = 0;

				cmd.Parameters[i++].Value = sr.Site;
				cmd.Parameters[i++].Value = sr.CurrentSiteGuid;
				cmd.Parameters[i++].Value = sr.ManagerName;
				cmd.Parameters[i++].Value = sr.ManagerCompanyGuid;
				cmd.Parameters[i++].Value = sr.ProductName;
				cmd.Parameters[i++].Value = sr.ProductGuid;
				cmd.Parameters[i++].Value = this.companyID;
				cmd.Parameters[i++].Value = this.companyGuid;

				DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, this.security);

				if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
				{
					DataTable dataTable = dataSet.Tables[0];

					if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
					{
						DataRow row = dataTable.Rows[0];

						previousGrossBookInventory = row.IsNull("GrossBookInventory") ? 0 : (double)row["GrossBookInventory"];
						previousNetBookInventory = row.IsNull("NetBookInventory") ? 0 : (double)row["NetBookInventory"];
						previousGrossBookPrice = row.IsNull("GrossBookPrice") ? 0 : (double)row["GrossBookPrice"];
						previousNetBookPrice = row.IsNull("NetBookPrice") ? 0 : (double)row["NetBookPrice"];
					}
				}
			}
		}

		#endregion

		// returns a OwnerCloseOut Record based on this DAO object.
	}
}