-- ==========================================================================================
-- Description:	Stored procedure to add standard list views into the system
-- Author:		Srinivasa Divyakolu
-- Create date/Revision: 07-27-2022/1.0.0
-- Last Modification date/Revision: 08-09-2022/1.0.001
-- 1. Get all existing listViewGuid from tblListViews for a given list view by ID
-- 2. Check if the given listView is entity assigned to this site from any of the parents by checking  [map].[tblEntityListViewToSite]
-- 3. If it is entity assigned then do not create a new view
-- 4. If it is not entity assigned then create it and entity assign itself
-- ==========================================================================================
CREATE PROCEDURE usp_AddStandardListViews 
	@SiteGuid UNIQUEIDENTIFIER,
	@ReturnMsg NVARCHAR(2000) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;

	DECLARE @LVID AS NVARCHAR(100)
	DECLARE @MapExists AS BIT 
	DECLARE @ListViewGuid as UNIQUEIDENTIFIER
	DECLARE @User AS NVARCHAR(20) = N'Administrator';
	DECLARE @CUDateTime AS DATETIME = SYSDATETIMEOFFSET();
	DECLARE @ListViewExists AS BIT = 0
	DECLARE @ReturnMessage AS NVARCHAR(2000)


	SET @LVID = N'Inventory Reconciliation'

	SET @ReturnMessage = @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

	IF @MapExists = 0
	BEGIN

		-- Only add if not already exists
		IF @ListViewGuid IS NULL
		BEGIN

			INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [LookupListViewStandardTypeIndex], [LedgerAggregateColumnGuid], [TransactionAliasGuid]) VALUES 
			(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 2, 4, NULL, NULL)

			-- Get listview guid for the newly added row
			SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

			INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
			(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

			DECLARE @SupplyOrderGuid AS UNIQUEIDENTIFIER, @MeterCloseoutGuid  AS UNIQUEIDENTIFIER, @ShipmentGuid AS UNIQUEIDENTIFIER, @RegradeGuid AS UNIQUEIDENTIFIER
			DECLARE @AdjustmentGuid AS UNIQUEIDENTIFIER, @TransferGuid AS UNIQUEIDENTIFIER, @PhysicalInventoryGuid AS UNIQUEIDENTIFIER, @ReceiptGuid AS UNIQUEIDENTIFIER
			DECLARE @OrderGuid AS UNIQUEIDENTIFIER, @BOLGuid AS UNIQUEIDENTIFIER

			SET  @SupplyOrderGuid = (SELECT _MasterRecordGuid FROM [dbo].[tblTransactionAliases] Where AliasName = N'Supply Order' and SiteGuid =  @SiteGuid)
			SET  @MeterCloseoutGuid = (SELECT _MasterRecordGuid FROM [dbo].[tblTransactionAliases] Where AliasName = N'Meter Closeout' and SiteGuid =  @SiteGuid)
			SET  @ShipmentGuid = (SELECT _MasterRecordGuid FROM [dbo].[tblTransactionAliases] Where AliasName = N'Shipment' and SiteGuid =  @SiteGuid)
			SET  @RegradeGuid = (SELECT _MasterRecordGuid FROM [dbo].[tblTransactionAliases] Where AliasName = N'Regrade' and SiteGuid =  @SiteGuid)
			SET  @AdjustmentGuid = (SELECT _MasterRecordGuid FROM [dbo].[tblTransactionAliases] Where AliasName = N'Adjustment' and SiteGuid =  @SiteGuid)
			SET  @TransferGuid = (SELECT _MasterRecordGuid FROM [dbo].[tblTransactionAliases] Where AliasName = N'Transfer' and SiteGuid =  @SiteGuid)
			SET  @PhysicalInventoryGuid = (SELECT _MasterRecordGuid FROM [dbo].[tblTransactionAliases] Where AliasName = N'Physical Inventory' and SiteGuid =  @SiteGuid)
			SET  @ReceiptGuid = (SELECT _MasterRecordGuid FROM [dbo].[tblTransactionAliases] Where AliasName = N'Receipt' and SiteGuid =  @SiteGuid)
			SET  @OrderGuid = (SELECT _MasterRecordGuid FROM [dbo].[tblTransactionAliases] Where AliasName = N'Order' and SiteGuid =  @SiteGuid)
			SET  @BOLGuid = (SELECT _MasterRecordGuid FROM [dbo].[tblTransactionAliases] Where AliasName = N'BOL' and SiteGuid =  @SiteGuid)

			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid], [TransactionAliasGuid]) 
			VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 3, @ListViewGuid, NULL)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid], [TransactionAliasGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 1, @ListViewGuid, NULL)
			
			IF @ReceiptGuid IS NOT NULL
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid], [TransactionAliasGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 1, NULL, @ListViewGuid, @ReceiptGuid)

			IF @BOLGuid IS NOT NULL
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid], [TransactionAliasGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 1, NULL, @ListViewGuid, @BOLGuid)

			IF @ShipmentGuid IS NOT NULL
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid], [TransactionAliasGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 1, NULL, @ListViewGuid, @ShipmentGuid)

			IF @RegradeGuid IS NOT NULL
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid], [TransactionAliasGuid]) VALUES (5, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 1, NULL, @ListViewGuid, @RegradeGuid)

			IF @AdjustmentGuid IS NOT NULL
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid], [TransactionAliasGuid]) VALUES (6, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 1, NULL, @ListViewGuid, @AdjustmentGuid)

			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid], [TransactionAliasGuid]) VALUES (7, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 2, @ListViewGuid, NULL)

			IF @PhysicalInventoryGuid IS NOT NULL
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid], [TransactionAliasGuid]) VALUES (8, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 1, NULL, @ListViewGuid, @PhysicalInventoryGuid)

			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid], [TransactionAliasGuid]) VALUES (9, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 9, @ListViewGuid, NULL)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid], [TransactionAliasGuid]) VALUES (10, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 13, @ListViewGuid, NULL)

			IF @MeterCloseoutGuid IS NOT NULL
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid], [TransactionAliasGuid]) VALUES (11, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 1, NULL, @ListViewGuid, @MeterCloseoutGuid)

			SET @ReturnMessage += 'View created,'
		END
		ELSE
			SET @ReturnMessage += 'View already exists,'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned,', 'View already exists,')

	--------------------------------------------------------------
	SET @LVID = N'Closeout'

	SET @ReturnMessage += @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

	IF @MapExists = 0
	BEGIN

		-- Only add if not already exists
		IF @ListViewGuid IS NULL
		BEGIN

			INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [LookupListViewStandardTypeIndex], [LedgerAggregateColumnGuid], [TransactionAliasGuid]) VALUES 
			(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 2, 5, NULL, NULL)

			-- Get listview guid for the newly added row
			SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

			INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
			(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 62, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 2, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 43, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 9, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 13, @ListViewGuid)

			SET @ReturnMessage += 'View created,'
		END
		ELSE
			SET @ReturnMessage += 'View already exists,'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned,', 'View already exists,')

	--------------------------------------------------------------
	SET @LVID = N'Equipment Transaction'

	SET @ReturnMessage += @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

	IF @MapExists = 0
	BEGIN

		-- Only add if not already exists
		IF @ListViewGuid IS NULL
		BEGIN

			INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [LookupListViewStandardTypeIndex], [LedgerAggregateColumnGuid], [TransactionAliasGuid]) VALUES 
			(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 2, 6, NULL, NULL)

			-- Get listview guid for the newly added row
			SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

			INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
			(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 22, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 5, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 6, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 23, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 7, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (5, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 9, @ListViewGuid)

			SET @ReturnMessage += 'View created,'
		END
		ELSE
			SET @ReturnMessage += 'View already exists,'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned,', 'View already exists,')

	--------------------------------------------------------------
	SET @LVID = N'Order Summary'

	SET @ReturnMessage += @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

	IF @MapExists = 0
	BEGIN

		-- Only add if not already exists
		IF @ListViewGuid IS NULL
		BEGIN

			INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [LookupListViewStandardTypeIndex], [LedgerAggregateColumnGuid], [TransactionAliasGuid]) VALUES 
			(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 2, 11, NULL, NULL)

			-- Get listview guid for the newly added row
			SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

			INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
			(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 46, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 10, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 49, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 47, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 50, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (5, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 48, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (6, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 53, @ListViewGuid)

			SET @ReturnMessage += 'View created,'
		END
		ELSE
			SET @ReturnMessage += 'View already exists,'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned,', 'View already exists,')

--------------------------------------------------------------
	SET @LVID = N'BOL Summary'

	SET @ReturnMessage += @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

	IF @MapExists = 0
	BEGIN

		-- Only add if not already exists
		IF @ListViewGuid IS NULL
		BEGIN
			INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [LookupListViewStandardTypeIndex], [LedgerAggregateColumnGuid], [TransactionAliasGuid]) VALUES 
			(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 2, 13, NULL, NULL)

			-- Get listview guid for the newly added row
			SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

			INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
			(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 57,  @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 54,  @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 48,  @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 60,  @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 44,  @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (5, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 45,  @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (6, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 58,  @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (7, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 175, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (8, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 56,  @ListViewGuid)

			SET @ReturnMessage += 'View created,'
		END
		ELSE
			SET @ReturnMessage += 'View already exists,'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned,', 'View already exists,')

	--------------------------------------------------------------
	SET @LVID = N'Supply Order Summary'

	SET @ReturnMessage += @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

	IF @MapExists = 0
	BEGIN

		-- Only add if not already exists
		IF @ListViewGuid IS NULL
		BEGIN
			INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [LookupListViewStandardTypeIndex], [LedgerAggregateColumnGuid], [TransactionAliasGuid]) VALUES 
			(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 2, 14, NULL, NULL)

			-- Get listview guid for the newly added row
			SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

			INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
			(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 3, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 41, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 50, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 48, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 67, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (5, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 55, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (6, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 45, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (7, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 40, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (8, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 68, @ListViewGuid)

			SET @ReturnMessage += 'View created,'
		END
		ELSE
			SET @ReturnMessage += 'View already exists,'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned,', 'View already exists,')

	--------------------------------------------------------------
	SET @LVID = N'Supply Order Associated Transactions'

	SET @ReturnMessage += @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

	IF @MapExists = 0
	BEGIN

		-- Only add if not already exists
		IF @ListViewGuid IS NULL
		BEGIN
			INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [LookupListViewStandardTypeIndex], [LedgerAggregateColumnGuid], [TransactionAliasGuid]) VALUES 
			(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 2, 15, NULL, NULL)

			-- Get listview guid for the newly added row
			SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

			INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
			(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 3, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 47, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 48, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 40, @ListViewGuid)
			INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [LookupStandardFieldTypeIndex], [ListViewGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 4, 111, @ListViewGuid)

			SET @ReturnMessage += 'View created.'
		END
		ELSE
			SET @ReturnMessage += 'View already exists.'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned.', 'View already exists.')

	SET @ReturnMsg = @ReturnMessage
END
GO
