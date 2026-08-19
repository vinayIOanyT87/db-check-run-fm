CREATE FUNCTION [rpt].[udf_AuditInventory] 
(
	@Sites NVARCHAR(MAX),
	@Managers NVARCHAR(MAX),
	@Owner UNIQUEIDENTIFIER,
	@Product UNIQUEIDENTIFIER,
	@BeginDate DATETIMEOFFSET(7),
	@EndDate DATETIMEOFFSET(7),
	@GrossNet BIT,
	@SiteGuid UNIQUEIDENTIFIER,
	@UserGuid UNIQUEIDENTIFIER
)
RETURNS @TotalSum TABLE
(
						[ProductID]			NVARCHAR(100),
						[ManagerID]			NVARCHAR(100),
						[OwnerID]			NVARCHAR(100),
						[SiteID]			NVARCHAR(100),
						InventoryDate		DATETIME,
						[Begin Inventory]	FLOAT,
						[Book Inventory]    FLOAT,
						[TransAmt]			FLOAT,
						[DetailTrans]		FLOAT,
						[24 Hr]				FLOAT,
						[Adjustment]		FLOAT,
						[Bulk Issue]		FLOAT,
						[Defuel]			FLOAT,
						[Issue]				FLOAT,
						[Load Rack]			FLOAT,
						[LR Receipt]		FLOAT,
						[Receipt]			FLOAT,
						[Rotation]			FLOAT,
						[Transfer]			FLOAT

)		
AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[udf_AuditInventory] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the Audit Inventory Report
	--          for single owner and product, and multiple manager and sites.
	-- Notes:
	-- 1. @SiteGuid: Site/SiteGroup that report is being executed at for the purpose of retrieving proper units and decimal places.
	-- 2. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 3. @Manager: MasterRecordGuids assigned the role of manager that the transactions list as the manager for itself to be included in the results
	-- 4. @Owner: List of company MasterRecordGuids assigned the role of owner that the transactions list as the owner for itself to be included in the results
	-- 5. @Product: ProductGuid for wich the list of transactions are filtered on.
	-- 6. @BeginDate: Lower bound date to collect transactions meeting criteria
	-- 7. @EndDate: Upper bound date to collect transactions meeting criteria
	-- 8. @GrossNet: Boolean indicated whether the Gross or Net quantity values are to be returned for the transaction list.
	------------------------------------------------------------------------------------------------------
	DECLARE @ProductName NVARCHAR(30)
	SET @ProductName = (SELECT ProductID FROM tblProducts WHERE @Product = ProductGuid)

	INSERT INTO @TotalSum
	SELECT @ProductName AS ProductID
		, cmp.ID AS ManagerID
		, transList.OwnerID
		, transList.[SiteID]
		, transList.InventoryDate
		, transList.[Begin Inventory]
		, transList.[Book Inventory]
		, transList.TransAmt
		, transList.TransAmt
		, transList.[24 Hr]
		, transList.Adjustment
		, transList.[Bulk Issue]
		, transList.Defuel
		, transList.Issue
		, transList.[Load Rack]
		, transList.[LR Receipt]
		, transList.Receipt
		, transList.Rotation
		, transList.Transfer
	FROM 
	(
		(
			SELECT *
			FROM rpt.udf_GetTableFromStringList(@Managers)
		) mgr
		CROSS APPLY rpt.udf_AuditInventoryBySite (@Sites, mgr.Guid, @Owner, @Product, @BeginDate, @EndDate, @GrossNet, @SiteGuid, @UserGuid) transList
	) 
	LEFT JOIN tblCompanies cmp
	ON cmp.CompanyGuid = mgr.Guid

	RETURN;

	/****************
		END
		Main Query
	******************/
END