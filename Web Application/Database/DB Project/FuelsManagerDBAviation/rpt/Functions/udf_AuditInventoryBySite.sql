CREATE FUNCTION [rpt].[udf_AuditInventoryBySite]
(
	@Sites NVARCHAR(MAX),
	@Manager UNIQUEIDENTIFIER,
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
						[SiteID]			NVARCHAR(30),
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
	-- Stored Procedure: [rpt].[udf_AuditInventoryBySite] 
	-- Author: Paul Carpenter
	-- Version/Date:  2014-01-06 16:27:10
	-- Purpose: Retrieve the transaction records for the Audit Inventory Report 
	--          for single manager, owner and product and multiple sites.
	-- Notes:
	-- 1. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 2. @Manager: MasterRecordGuids assigned the role of manager that the transactions list as the manager for itself to be included in the results
	-- 3. @Owner: MasterRecordGuids assigned the role of owner that the transactions list as the owner for itself to be included in the results
	-- 4. @Product: ProductGuid for wich the list of transactions are filtered on.
	-- 5. @BeginDate: Lower bound date to collect transactions meeting criteria
	-- 6. @EndDate: Upper bound date to collect transactions meeting criteria
	-- 7. @GrossNet: Boolean indicated whether the Gross or Net quantity values are to be returned for the transaction list.
	-- 8. @SiteGuid: Identifies the site the report is being run from
	-- 9. @UserGuid: Identifies the user running the report
	------------------------------------------------------------------------------------------------------
	DECLARE @ProductName NVARCHAR(30)
	SET @ProductName = (SELECT ProductID FROM tblProducts WHERE @Product = ProductGuid)

	INSERT INTO @TotalSum
	SELECT @ProductName AS ProductID
		, @Manager AS ManagerID
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
		SELECT *
		FROM rpt.udf_GetTableFromStringList(@Sites)
	) s
	CROSS APPLY rpt.udf_AuditInventorySub (s.Guid, @Manager, @Owner, @Product, @BeginDate, @EndDate, @GrossNet, @SiteGuid, @UserGuid) transList

	RETURN;

	/****************
		END
		Main Query
	******************/
END