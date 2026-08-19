CREATE PROCEDURE [dbo].[gsp_ExStarsEndingInventorySelect]
	 @SiteGuid UNIQUEIDENTIFIER=NULL
	,@ManagerCompanyGuid UNIQUEIDENTIFIER=NULL
	,@InventoryDate Date
AS
BEGIN
	-- There may be multiple inventories on the same day, representing multiple tanks
	-- From the IRS perspective, prodocts withe the same 
	-- without regard for ProductGuid
	DECLARE @NotInventoried int = 0
	DECLARE @TransTypePhyInventory int = 14
	SELECT 
		ReportYear, ReportMonth, Reportday, Site, SiteGuid, ManagerID, ManagerCompanyGuid
		, Product, TaxCode, ProductGuid, AviationFuelFlag, PriorInventoryExists, Count(*) as count
		-- Sums are used, because inventory may be made on multiple tanks with the same product
		, SUM( GrossVolume) as TotalGrossVolume
		, SUM( NetVolume) as TotalNetVolume
	FROM (
		SELECT 
			DATEPART( YEAR, t.InventoryDate) AS ReportYear
			, DATEPART( MONTH, t.InventoryDate) AS ReportMonth
			, DATEPART( DAY, t.InventoryDate) AS Reportday
			, t.Site
			, t.SiteGuid
			, t.ManagerID
			, t.ManagerCompanyGuid
			, ( select top 1 ISNULL( ProductID, '') as Product from tblProducts p2 where li.ProductGuid = p2.ProductGuid) as Product
			, p.TaxCode
			, p.ProductGuid
			, p.AviationFuelFlag
			, ISNULL([PriorInventoryExists], @NotInventoried)   as PriorInventoryExists
			--ConvertUNitIndex(46) = US_Gallon
			, [dbo].[udf_ConvertFromSIUnits]( ISNULL( li.GrossQuantity, 0.0), 46, 0) as GrossVolume
			, [dbo].[udf_ConvertFromSIUnits]( ISNULL( li.NetQuantity, 0.0), 46, 0) as NetVolume
		FROM 
			tblTransactions t
			INNER JOIN tblTransactionAliases ta on ta.TransactionAliasGuid = t.TransactionAliasGuid
			INNER JOIN tblTransactionLineItems li on li.TransactionGuid=t.TransactionGuid
			INNER JOIN tblProducts p on li.ProductGuid = p.ProductGuid
			LEFT JOIN tblExStarsProductPriorInventory ppi 
				ON t.SiteGuid = ppi.SiteGuid 
				AND p.TaxCode=ppi.TaxCode
				AND t.ManagerCompanyGuid = ppi.ManagerCompanyGuid		
		WHERE 
			ta.LookupTransTypeIndex = @TransTypePhyInventory
			AND t.SiteGuid=@SiteGuid
			AND t.ManagerCompanyGuid=@ManagerCompanyGuid
			AND t.InventoryDate = @InventoryDate
	) SubQuery
	GROUP BY
		ReportYear, ReportMonth, Reportday, Site, SiteGuid, ManagerID, ManagerCompanyGuid
		, Product, ProductGuid, TaxCode, ProductGuid, AviationFuelFlag, PriorInventoryExists


END
