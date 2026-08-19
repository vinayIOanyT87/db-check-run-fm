-- ==========================================================================================
-- Description:	Stored procedure to add standard ledger list view into the system
-- Author:		Srinivasa Divyakolu
-- Create date: 08-08-2022/1.0.0
-- Last Modification date/Revision: 08-09-2022/1.0.001
-- 1. Get all existing listViewGuid from tblListViews for a given list view by ID
-- 2. Check if the given listView is entity assigned to this site from any of the parents by checking  [map].[tblEntityLedgerViewToSite]
-- 3. If it is entity assigned then do not create a new view
-- 4. If it is not entity assigned then create it and entity assign itself
-- ==========================================================================================
CREATE PROCEDURE usp_AddLedgerListView
	@SiteGuid UNIQUEIDENTIFIER,
	@ReturnMsg NVARCHAR(2000) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;

	DECLARE @LVID AS NVARCHAR(100)
	DECLARE @TransactionAliasGuid AS UNIQUEIDENTIFIER
	DECLARE @MapExists AS BIT 
	DECLARE @ListViewGuid AS UNIQUEIDENTIFIER
	DECLARE @User AS NVARCHAR(20) = N'Administrator';
	DECLARE @CUDateTime AS DATETIME = SYSDATETIMEOFFSET();
	DECLARE @ListViewExists AS BIT = 0
	DECLARE @ReturnMessage AS NVARCHAR(2000)

	DECLARE @ReceiptTAliasGuid AS UNIQUEIDENTIFIER, @BOLTAliasGuid  AS UNIQUEIDENTIFIER, @ShipmentTAliasGuid AS UNIQUEIDENTIFIER, 
			@ProductFieldGuid AS UNIQUEIDENTIFIER, @TransferTAliasGuid AS UNIQUEIDENTIFIER, @RegradeTAliasGuid AS UNIQUEIDENTIFIER,
			@AdjustmentTAliasGuid AS UNIQUEIDENTIFIER

	SET @LVID = N'Standard Ledger View'

	SET @ReturnMessage = @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityLedgerViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
--	WHERE [ListViewGuid] = @ListViewGuid AND [SiteGuid] = @SiteGuid AND [AssignedFromSiteGuid] = @SiteGuid)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

	IF @MapExists = 0
	BEGIN

		-- Only add if not already exists
		IF @ListViewGuid IS NULL
		BEGIN

			INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [LookupListViewStandardTypeIndex]) VALUES 
			(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 2, 1)

			-- Get listview guid for the newly added row
			SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

			INSERT [map].[tblEntityLedgerViewToSite] ([LedgerViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
			(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

			SET  @ReceiptTAliasGuid = (Select TransactionAliasGuid FROM dbo.tblTransactionAliases WHERE AliasName ='Receipt' AND SiteGuid = @SiteGuid)
			SET  @BOLTAliasGuid = (Select TransactionAliasGuid FROM dbo.tblTransactionAliases WHERE AliasName ='BOL' AND SiteGuid = @SiteGuid)
			SET  @ShipmentTAliasGuid = (Select TransactionAliasGuid FROM dbo.tblTransactionAliases WHERE AliasName ='Shipment' AND SiteGuid = @SiteGuid)
			SET  @TransferTAliasGuid = (Select TransactionAliasGuid FROM dbo.tblTransactionAliases WHERE AliasName ='Transfer' AND SiteGuid = @SiteGuid)
			SET  @RegradeTAliasGuid = (Select TransactionAliasGuid FROM dbo.tblTransactionAliases WHERE AliasName ='Regrade' AND SiteGuid = @SiteGuid)
			SET  @AdjustmentTAliasGuid = (Select TransactionAliasGuid FROM dbo.tblTransactionAliases WHERE AliasName ='Adjustment' AND SiteGuid = @SiteGuid)

			INSERT INTO tblListViewFields (ListViewGuid, LookupListViewFieldTypeIndex,LookupStandardFieldTypeIndex,ListViewID,ColumnOrder,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,ListViewFieldGuid)
			VALUES(@ListViewGuid, 4, 3, @LVID, 0, @CUDateTime, @User, @CUDateTime, @User, NEWID()) -- Inventory Date

			INSERT INTO tblListViewFields (ListViewGuid, LookupListViewFieldTypeIndex,LookupStandardFieldTypeIndex,ListViewID,ColumnOrder,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,ListViewFieldGuid)
			VALUES(@ListViewGuid, 4, 1, @LVID, 1, @CUDateTime, @User, @CUDateTime, @User, NEWID()) -- Begin Inventory

			IF @ReceiptTAliasGuid IS NOT NULL
			INSERT INTO tblListViewFields (ListViewGuid, LookupListViewFieldTypeIndex,TransactionAliasGuid,ListViewID,ColumnOrder,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,ListViewFieldGuid) 
			VALUES(@ListViewGuid, 1, @ReceiptTAliasGuid, @LVID, 2, @CUDateTime, @User, @CUDateTime, @User, NEWID()) -- Receipt

			IF @BOLTAliasGuid IS NOT NULL
			INSERT INTO tblListViewFields (ListViewGuid, LookupListViewFieldTypeIndex,TransactionAliasGuid,ListViewID,ColumnOrder,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,ListViewFieldGuid) 
			VALUES(@ListViewGuid, 1, @BOLTAliasGuid, @LVID, 3, @CUDateTime, @User, @CUDateTime, @User, NEWID()) -- BOL

			IF @ShipmentTAliasGuid IS NOT NULL
			INSERT INTO tblListViewFields (ListViewGuid, LookupListViewFieldTypeIndex,TransactionAliasGuid,ListViewID,ColumnOrder,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,ListViewFieldGuid) 
			VALUES(@ListViewGuid, 1, @ShipmentTAliasGuid, @LVID, 4, @CUDateTime, @User, @CUDateTime, @User, NEWID()) -- Shipment

			IF @TransferTAliasGuid IS NOT NULL
			INSERT INTO tblListViewFields (ListViewGuid, LookupListViewFieldTypeIndex,TransactionAliasGuid,ListViewID,ColumnOrder,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,ListViewFieldGuid) 
			VALUES(@ListViewGuid, 1, @TransferTAliasGuid, @LVID, 5, @CUDateTime, @User, @CUDateTime, @User, NEWID()) -- Transfer

			IF @RegradeTAliasGuid IS NOT NULL
			INSERT INTO tblListViewFields (ListViewGuid, LookupListViewFieldTypeIndex,TransactionAliasGuid,ListViewID,ColumnOrder,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,ListViewFieldGuid) 
			VALUES(@ListViewGuid, 1, @RegradeTAliasGuid, @LVID, 6, @CUDateTime, @User, @CUDateTime, @User, NEWID()) -- Regrade

			IF @AdjustmentTAliasGuid IS NOT NULL
			INSERT INTO tblListViewFields (ListViewGuid, LookupListViewFieldTypeIndex,TransactionAliasGuid,ListViewID,ColumnOrder,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,ListViewFieldGuid) 
			VALUES(@ListViewGuid, 1, @AdjustmentTAliasGuid, @LVID, 7, @CUDateTime, @User, @CUDateTime, @User, NEWID()) -- Adjustment

			INSERT INTO tblListViewFields (ListViewGuid, LookupListViewFieldTypeIndex,LookupStandardFieldTypeIndex,ListViewID,ColumnOrder,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,ListViewFieldGuid)
			VALUES(@ListViewGuid, 4, 2, @LVID, 8, @CUDateTime, @User, @CUDateTime, @User, NEWID()) -- Book Inventory

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