namespace LedgerCore
{
	using System;
	using System.Globalization;

	public class LRLedgerBSMEQuery : LRLedgerQueryBase
	{
		#region Private data members
		/// <summary>
		/// The date process type.
		/// </summary>
		private readonly LRLedgerProcessor.DateProcessTypes dateProcessType;

		protected const int TransactionStatusCancelled = 7;
		protected const int OriginDispatch = 3;
		protected const int OriginEnterpriseDispatch = 16;

		private bool isBaseDb;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default for the Ledger Standard Query class.
		/// </summary>
		public LRLedgerBSMEQuery(double volumeConversionFactor,
								int volumeDecimalPlaces,
								double massConversionFactor,
								int massDecimalPlaces,
								double currencyFactor,
								int currencyDecimalPlaces,
								double volumePackageSize,
								double massPackageSize,
								bool loadByWeight,
								LRLedgerProcessor.DateProcessTypes dateProcessType,
								bool isBaseDb,
								LRTransactionAliasListDO transAliasListDo)
			: base(	volumeConversionFactor, 
					volumeDecimalPlaces,
					massConversionFactor, 
					massDecimalPlaces,
					currencyFactor, 
					currencyDecimalPlaces,
					volumePackageSize, 
					massPackageSize, 
					loadByWeight,
					transAliasListDo)
		{
			this.dateProcessType = dateProcessType;
			this.isBaseDb = isBaseDb;
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method returns an SQL string containing the SQL used 
		/// to retrieve the transactional data for computing the ledger.
		/// </summary>
		/// <param name="managerGuid">
		/// The manager Index.
		/// </param>
		/// <param name="ownerGuid">
		/// The owner Index.
		/// </param>
		/// <param name="tankGuid">
		/// The tank Index.
		/// </param>
		/// <param name="siteCount">
		/// The site Count.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public override string CreateLineItemSqlStatement(Guid managerGuid, Guid ownerGuid, Guid tankGuid, int siteCount)
		{
			string select = "SELECT CONVERT(Char(10), t.InventoryDate, 111) AS InventoryDate, ";
			string from = "FROM tblTransactionLineItems l WITH(NOLOCK)INNER JOIN tblTransactions t WITH(NOLOCK) ON l.TransactionGuid = t.TransactionGuid ";
			string where = "WHERE (t.InventoryDate BETWEEN @BeginDate AND @EndDate) ";
			string siteDateTimes = "DECLARE @BeginDateSiteTime datetimeoffset(7) = dbo.udf_GetSiteMidnightDateTimeOffset(@BeginDate, @SelectedSiteGuid)," +
				" @EndDateSiteTime datetimeoffset(7) = dbo.udf_GetSiteMidnightDateTimeOffset(@EndDate, @SelectedSiteGuid), " +
				" @SiteTimeZoneId nvarchar(50) = (SELECT TimeZone From tblSites Where SiteGuid = @SelectedSiteGuid)";

			switch (this.dateProcessType)
			{
				case LRLedgerProcessor.DateProcessTypes.ByEbsPostDate:
					//this date has no time so dont convert to site time
					select = "SELECT CONVERT(Char(10), SWITCHOFFSET(t.Date03, 0), 111) AS InventoryDate, "; // convert to utc so the correct date to display on ledger 
					where = "WHERE (CONVERT(date, t.Date03) BETWEEN @BeginDate AND @EndDate) ";
					break;
				case LRLedgerProcessor.DateProcessTypes.ByCreateDate:
					select = "SELECT CONVERT(Char(10), dbo.udf_ConvertToTimeZone(t.CreatedDate, @SiteTimeZoneId), 111) AS InventoryDate, ";
					where = "WHERE (t.CreatedDate BETWEEN @BeginDateSiteTime AND @EndDateSiteTime) ";
					break;
				case LRLedgerProcessor.DateProcessTypes.ByEbsSentToDate:
					select = "SELECT CONVERT(Char(10), SWITCHOFFSET(t.Date04, 0), 111) AS InventoryDate, "; // convert to utc so the correct date to display on ledger 
					where = "WHERE (t.Date04 BETWEEN @BeginDate AND @EndDate) ";
					break;
				case LRLedgerProcessor.DateProcessTypes.ByEbsAcknowledgedDate:
					select = "SELECT CONVERT(Char(10), SWITCHOFFSET(erd.CreatedDate, 0), 111) AS InventoryDate, "; // convert to utc so the correct date to display on ledger 
					from += "INNER JOIN tblExportInterfaceResult erd WITH (NOLOCK) " +
							"ON t.TransID = erd.RecordID AND t.TransVersion = erd.TransVersion " +
							"AND (erd.CreatedDate IN (SELECT MAX(ex.CreatedDate) " +
								"FROM tblExportInterfaceResult ex WITH (NOLOCK) " +
								"WHERE (ex.CreatedDate BETWEEN @BeginDate AND @EndDate) " +
								"AND ex.InterfaceName = 'EBSTransactionResult' AND ex.InterfaceData04 IN ('51','53','68') " +
								"AND t.TransID = ex.RecordID AND t.TransVersion = ex.TransVersion " +
								"GROUP BY ex.RecordID)) ";
					where = "WHERE (erd.CreatedDate BETWEEN @BeginDate AND @EndDate) ";
					break;
			}

			select = select + "t.AliasName, " +
								"t.TransactionAliasGuid, " +
								"l.GrossQuantity AS GrossQuantity, " +
								"l.ProductPrice, " +
								"l.NetQuantity AS NetQuantity, " +
								"l.MassQuantity AS MassQuantity, " +
								"t.Site, " +
								"t.LookupTransTypeIndex, " +
								"l.Number01, " +
								"l.Number02, " +
								"l.Number03, " +
								"l.Number04, " +
								"l.Number05, " +
								"l.Number06, " +
								"t.ErrorFlag, " +
								"t.ReversalType, " +
								"t.TransVersion ";

			where = where + "AND t.DeleteFlag = cast(0 AS bit) " +
							// Dispatch transactions not yet submitted to accounting should still appear. TransactionDetail.aspx.cs will ensure they are read-only
							//"AND t.SubmittedToAccounting = 1 " +
							"AND (l.ProductGuid = @ProductGuid " +
							"OR l.ProductGuid IN (SELECT ProductGuid FROM tblProducts WHERE TrackingProductGuid = @ProductGuid)) " +
							"AND ISNULL(l.LookupQualityIndex, 1) = 1 " +
							"AND t.LookupTransactionStatusIndex <> " + TransactionStatusSuspence.ToString(CultureInfo.InvariantCulture) + " " +
							"AND ( t.LookupOriginApplicationIndex NOT IN(" + OriginDispatch.ToString(CultureInfo.InvariantCulture) + "," + OriginEnterpriseDispatch.ToString(CultureInfo.InvariantCulture) + ") OR " +
							"   (t.LookupTransactionStatusIndex <> " + TransactionStatusCancelled.ToString(CultureInfo.InvariantCulture) +
							"    AND t.LookupOriginApplicationIndex in (" + OriginDispatch.ToString(CultureInfo.InvariantCulture) + "," + OriginEnterpriseDispatch.ToString(CultureInfo.InvariantCulture) + ") ) ) " +
							"AND t.TransactionAliasGuid IN (SELECT B.TransactionAliasGuid FROM (SELECT TransactionAliasGuid FROM [dbo].[udf_AliasList](@SelectedSiteGuid)) B) " +
							"AND EXISTS (SELECT A.CompanyGuid FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SelectedSiteGuid, @UserGuid)) A  " +
							"WHERE A.CompanyGuid IS NULL OR A.CompanyGuid IN ( t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, " +
							"t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)) ";

			if (ownerGuid != Guid.Empty)
			{
				where = where + "AND (t.OwnerCompanyGuid = @OwnerCompanyGuid OR t.OwnerCompanyGuid IS NULL) ";
			}

			if (managerGuid != Guid.Empty)
			{
				where = where + "AND t.ManagerCompanyGuid = @ManagerCompanyGuid ";
			}

			if (tankGuid != Guid.Empty)
			{
				where = where + "AND l.StorageLocationTankGuid = @TankGuid ";
			}

			if (siteCount > 0)
			{
				string siteWhere = "AND t.SiteGuid IN ( ";

				for (int nextSite = 0; nextSite < siteCount; nextSite++)
				{
					string siteParmName = "@SiteGuid" + nextSite;
					siteWhere = siteWhere + siteParmName + ", ";
				}

				int lastComma = siteWhere.LastIndexOf(',');
				siteWhere = siteWhere.Remove(lastComma);
				siteWhere = siteWhere + " ) ";

				where = where + siteWhere;
			}

			return siteDateTimes + select + from + where;
		}

		/// <summary>
		/// This method returns an SQL string containing the SQL used 
		/// to retrieve the transactional sub-line item data for computing the ledger.
		/// </summary>
		/// <param name="managerGuid">
		/// The manager Index.
		/// </param>
		/// <param name="ownerGuid">
		/// The owner Index.
		/// </param>
		/// <param name="tankGuid">
		/// The tank Index.
		/// </param>
		/// <param name="siteCount">
		/// The site Count.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public override string CreateSubLineItemSqlStatement(Guid managerGuid, Guid ownerGuid, Guid tankGuid, int siteCount)
		{
			string select = "SELECT CONVERT(Char(10), t.InventoryDate, 111) AS InventoryDate, ";
			string from = "FROM tblTransactionSubLineItems l INNER JOIN tblTransactions t ON l.TransactionGuid = t.TransactionGuid ";
			string where = "WHERE (t.InventoryDate BETWEEN @BeginDate AND @EndDate) ";

			switch (this.dateProcessType)
			{
				case LRLedgerProcessor.DateProcessTypes.ByEbsPostDate:
					//this date has no time so dont convert to site time
					select = "SELECT CONVERT(Char(10), SWITCHOFFSET(t.Date03,0), 111) AS InventoryDate, "; // convert to utc so the correct date to display on ledger 
					where = "WHERE (CONVERT(date, t.Date03) BETWEEN @BeginDate AND @EndDate) ";
					break;
				case LRLedgerProcessor.DateProcessTypes.ByCreateDate:
					select = "SELECT CONVERT(Char(10), dbo.udf_ConvertToTimeZone(t.Date04, @SiteTimeZoneId), 111) AS InventoryDate, ";
					where = "WHERE (t.CreatedDate BETWEEN @BeginDateSiteTime AND @EndDateSiteTime) ";
					break;
				case LRLedgerProcessor.DateProcessTypes.ByEbsSentToDate:
					select = "SELECT CONVERT(Char(10), SWITCHOFFSET(t.CreatedDate, 0), 111) AS InventoryDate, "; // convert to utc so the correct date to display on ledger 
					where = "WHERE (t.Date04 BETWEEN @BeginDate AND @EndDate) ";
					break;
				case LRLedgerProcessor.DateProcessTypes.ByEbsAcknowledgedDate:
					select = "SELECT CONVERT(Char(10), SWITCHOFFSET(erd.CreatedDate, 0), 111) AS InventoryDate, "; // convert to utc so the correct date to display on ledger 
					from += "INNER JOIN tblExportInterfaceResult erd WITH (NOLOCK) " +
							"ON t.TransID = erd.RecordID AND t.TransVersion = erd.TransVersion " +
							"AND (erd.CreatedDate IN (SELECT MAX(ex.CreatedDate) " +
								"FROM tblExportInterfaceResult ex WITH (NOLOCK) " +
								"WHERE (ex.CreatedDate BETWEEN @BeginDate AND @EndDate) " +
								"AND ex.InterfaceName = 'EBSTransactionResult' AND ex.InterfaceData04 IN ('51','53','68') " +
								"AND t.TransID = ex.RecordID AND t.TransVersion = ex.TransVersion " +
								"GROUP BY ex.RecordID)) ";
					where = "WHERE (erd.CreatedDate BETWEEN @BeginDate AND @EndDate) ";
					break;
			}

			select = select + "t.AliasName, " +
								"t.TransactionAliasGuid, " +
								"l.GrossQuantity AS GrossQuantity, " +
								"CAST(0.0 AS FLOAT) AS ProductPrice, " +
								"l.NetQuantity AS NetQuantity, " +
								"l.MassQuantity AS MassQuantity, " +
								"t.Site, " +
								"t.LookupTransTypeIndex, " +
								"CAST(0.0 AS FLOAT) AS Number01, " +
								"CAST(0.0 AS FLOAT) AS Number02, " +
								"CAST(0.0 AS FLOAT) AS Number03, " +
								"CAST(0.0 AS FLOAT) AS Number04, " +
								"CAST(0.0 AS FLOAT) AS Number05, " +
								"CAST(0.0 AS FLOAT) AS Number06, " +
								"t.ErrorFlag, " +
								"t.ReversalType," +
								"t.TransVersion ";
			where = where + "AND t.DeleteFlag = cast(0 AS bit) " +
							// Dispatch transactions not yet submitted to accounting should still appear. TransactionDetail.aspx.cs will ensure they are read-only
							//"AND t.SubmittedToAccounting = 1 " +
							"AND (l.ProductGuid = @ProductGuid " +
							"OR l.ProductGuid IN (SELECT ProductGuid FROM tblProducts WHERE TrackingProductGuid = @ProductGuid)) " +
							"AND ISNULL(l.LookupQualityIndex, 1) = 1 " +
							"AND t.LookupTransactionStatusIndex <> " + TransactionStatusSuspence.ToString(CultureInfo.InvariantCulture) + " " +
							"AND ( t.LookupOriginApplicationIndex NOT IN(" + OriginDispatch.ToString(CultureInfo.InvariantCulture) + "," + OriginEnterpriseDispatch.ToString(CultureInfo.InvariantCulture) + ") OR " +
							"   (t.LookupTransactionStatusIndex <> " + TransactionStatusCancelled.ToString(CultureInfo.InvariantCulture) +
							"    AND t.LookupOriginApplicationIndex in (" + OriginDispatch.ToString(CultureInfo.InvariantCulture) + "," + OriginEnterpriseDispatch.ToString(CultureInfo.InvariantCulture) + ") ) ) " +
							"AND t.TransactionAliasGuid IN (SELECT B.TransactionAliasGuid FROM (SELECT TransactionAliasGuid FROM [dbo].[udf_AliasList](@SelectedSiteGuid)) B) " +
							"AND EXISTS (SELECT A.CompanyGuid FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SelectedSiteGuid, @UserGuid)) A  " +
							"WHERE A.CompanyGuid IS NULL OR A.CompanyGuid IN ( t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, " +
							"t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)) ";

			if (ownerGuid != Guid.Empty)
			{
				where = where + "AND (t.OwnerCompanyGuid = @OwnerCompanyGuid OR t.OwnerCompanyGuid IS NULL) ";
			}

			if (managerGuid != Guid.Empty)
			{
				where = where + "AND t.ManagerCompanyGuid = @ManagerCompanyGuid ";
			}

			if (tankGuid != Guid.Empty)
			{
				where = where + "AND l.StorageLocationTankGuid = @TankGuid ";
			}

			if (siteCount > 0)
			{
				string siteWhere = "AND t.SiteGuid IN ( ";

				for (int nextSite = 0; nextSite < siteCount; nextSite++)
				{
					string siteParmName = "@SiteGuid" + nextSite;
					siteWhere = siteWhere + siteParmName + ", ";
				}

				int lastComma = siteWhere.LastIndexOf(',');
				siteWhere = siteWhere.Remove(lastComma);
				siteWhere = siteWhere + " ) ";

				where = where + siteWhere;
			}

			return select + from + where;
		}
		#endregion
	}
}