CREATE PROCEDURE [rpt].usp_Nspa_ReconciliationException_Part1 (
	--@SiteGuidStr VARCHAR(64)
	@SiteID VARCHAR(60)
	, @InventoryDate DATETIME
	)
AS
BEGIN
	DECLARE @FinalResults TABLE (
		SiteId VARCHAR(60)
		, ManagerId VARCHAR(60)
		, InventoryVariance FLOAT
		, MeterVariance FLOAT
		, AllowableVariance FLOAT
		, CloseoutDate DATE
		, ClosedBy VARCHAR(50)
		, EndOfMonthApproval DATE
		, ApprovalName VARCHAR(50)
		)
	DECLARE @SiteGuidStr VARCHAR(64)

	SELECT @SiteGuidStr = SiteGuid
	FROM tblSites
	WHERE ID = @SiteID

	--DECLARE @InventoryDate DATETIME
	--SET @InventoryDate = '7/7/2014'
	DECLARE @DecimalPlaces INT
	DECLARE @VolumeUnits INT

	SELECT @VolumeUnits = s.VolumeUnitIndex
		, @DecimalPlaces = s.VolumeDecimalPlaces
	FROM tblsites s
	WHERE s.SiteGuid = @SiteGuidStr

	DECLARE @DefaultDate DATE

	SET @DefaultDate = '1/1/1900'

	INSERT INTO @FinalResults
	SELECT s.ID
		, ManagersPerSite.ManagerId
		, 0
		, 0
		, 0
		, @DefaultDate
		, ''
		, @DefaultDate
		, ''
	FROM map.tblSiteToSite sts
	INNER JOIN tblSites s
		ON s.SiteGuid = sts.ChildSiteGuid
			, (
				SELECT c.ID ManagerId
				FROM map.tblCompanyToRole ctr
				INNER JOIN tblsites s
					ON s.SiteGuid = ctr.SiteGuid
				INNER JOIN tblCompanies c
					ON c.CompanyGuid = ctr.CompanyGuid
				WHERE s.SiteGuid = @SiteGuidStr
					AND ctr.LookupCompanyRoleIndex = 0
				) ManagersPerSite
	WHERE sts.ParentSiteGuid = CASE ISNULL(@SiteGuidStr, '')
			WHEN ''
				THEN (
						SELECT TOP 1 SiteGuid
						FROM tblsites
						WHERE ID = @SiteId
						)
			ELSE @SiteGuidStr
			END
		AND s.SiteGroupFlag = 0

	UPDATE @FinalResults
	SET CloseoutDate = CloseoutData.CloseoutDate
		, ClosedBy = CloseoutData.ClosedBy
	FROM (
		SELECT ProductGroupedDates.SiteId
			, ProductGroupedDates.ManagerId
			, ProductGroupedDates.CloseoutDate
			, COUNT(ProductGroupedDates.ProductName) AS CloseoutCount
			, u.NAME AS ClosedBy
		FROM (
			SELECT ci.Site AS SiteId
				, ci.ManagerName AS ManagerId
				, ci.CloseoutDate
				, ci.ProductName
				, ci.UpdatedBy
			FROM tblCloseoutInventory ci
			WHERE ci.ProductName IN (
					SELECT ProductId
					FROM vw_ProductGroupProducts pgp
					WHERE pgp.ProductGroupID = 'Fuel Products'
					)
			GROUP BY ci.Site
				, ci.ManagerName
				, ci.CloseoutDate
				, ci.ProductName
				, ci.UpdatedBy
			) ProductGroupedDates
		INNER JOIN tblUsers u
			ON u.UserID = ProductGroupedDates.UpdatedBy
		GROUP BY ProductGroupedDates.SiteId
			, ProductGroupedDates.ManagerId
			, ProductGroupedDates.CloseoutDate
			, u.NAME
		HAVING COUNT(ProductGroupedDates.ProductName) = (
				SELECT COUNT(*)
				FROM vw_ProductGroupProducts pgp
				WHERE pgp.ProductGroupID = 'Fuel Products'
				)
			--ORDER BY ProductGroupedDates.CloseoutDate DESC
		) CloseoutData
	WHERE CloseoutData.SiteId = [@FinalResults].SiteId
		AND CloseoutData.ManagerId = [@FinalResults].ManagerId

	DECLARE @ActiveProducts TABLE (
		ProductIndex INT IDENTITY(1, 1)
		, ProductId VARCHAR(30)
		)

	INSERT INTO @ActiveProducts
	SELECT ProductID
	FROM vw_ProductGroupProducts
	WHERE ProductGroupID = 'Fuel Products'

	/*
The allowable variance was originally believed to be based on the total physical inventory
for the specified date. But it is actually based on the difference between the specified
day and the day prior to the specified date.
*/
	--UPDATE @FinalResults
	--SET AllowableVariance = VarianceData.AllowableProductVariance
	--FROM (
	--	SELECT x.SiteId
	--		, X.ManagerID
	--		, x.InventoryDate
	--		, SUM(ISNULL(x.AllowableProductVariance, 0)) AS AllowableProductVariance
	--		, SUM(ISNULL(x.GrossVolume, 0)) AS ProductVolume
	--	FROM @ActiveProducts p
	--	LEFT OUTER JOIN (
	--		SELECT t.Site AS SiteId
	--			, t.ManagerID
	--			, t.InventoryDate
	--			, li.Product
	--			, sum(dbo.udf_ConvertFromSIUnits(li.GrossQuantity, @VolumeUnits, @DecimalPlaces)) AS GrossVolume
	--			, sum(dbo.udf_ConvertFromSIUnits(li.GrossQuantity, @VolumeUnits, @DecimalPlaces) * p.VarianceTolerance) / 100 AS AllowableProductVariance
	--		FROM tblTransactions t
	--		INNER JOIN tblTransactionLineItems li
	--			ON li.TransactionGuid = t.TransactionGuid
	--		INNER JOIN tblproducts p
	--			ON li.Product = p.ProductID
	--		INNER JOIN @FinalResults fr
	--			ON t.Site = fr.SiteId
	--		WHERE t.AliasName = 'Physical Inventory'
	--			AND t.InventoryDate BETWEEN @InventoryDate AND @InventoryDate
	--			AND t.Site = fr.SiteId
	--			AND t.DeleteFlag = 0
	--			AND li.DeleteFlag = 0
	--			AND (
	--				t.ReversalType IS NULL
	--				OR t.ReversalType IN ('', 'U')
	--				)
	--		GROUP BY t.Site
	--			, t.ManagerID
	--			, t.inventorydate
	--			, li.Product
	--		) x
	--		ON x.Product = p.ProductId
	--	GROUP BY x.SiteId
	--		, x.ManagerID
	--		, x.InventoryDate
	--	) VarianceData
	--WHERE VarianceData.SiteId = [@FinalResults].SiteId
	--	AND VarianceData.ManagerId = [@FinalResults].ManagerId
	UPDATE @FinalResults
	SET EndOfMonthApproval = ApprovalData.LastReportApproval
		, ApprovalName = ApprovalData.ApprovalName
	FROM (
		SELECT TOP 1 s.id AS SiteId
			, c.id AS ManagerId
			, ra.CreatedDate AS LastReportApproval
			, u.NAME AS ApprovalName
		FROM tblReportApprovals ra
		INNER JOIN tblSites s
			ON s.siteGuid = ra.SiteGuid
		INNER JOIN tblCompanies c
			ON c.CompanyGuid = ra.CompanyManagerGuid
		INNER JOIN tblusers u
			ON ra.ApprovalName = u.UserID
		WHERE LookupReportApprovalStateIndex = 3
		ORDER BY ra.CreatedDate DESC
		) ApprovalData
	WHERE ApprovalData.SiteId = [@FinalResults].SiteId
		AND ApprovalData.ManagerId = [@FinalResults].ManagerId

	SELECT *
	FROM @FinalResults
END
GO


